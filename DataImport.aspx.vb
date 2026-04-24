Imports System.Data
Imports System.IO
Imports System.Drawing
Imports System.IO.Compression
Imports System.Net
Imports System.Math
Imports OracleInternal.Secure
Imports Mysqlx
Imports Microsoft.Data
Partial Class DataImport
    Inherits System.Web.UI.Page
    Dim tblname As String
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If
        If Not IsPostBack Then
            Session("REPORTID") = ""
        End If
        If Request("rep") IsNot Nothing AndAlso Request("rep").ToString.Trim <> "" Then
            Session("REPORTID") = Request("rep")
            If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString.Trim <> "" Then
                TextBoxReportTitle.Text = Session("REPTITLE").ToString.Trim.Replace(":", "-")
                TextboxRepID.Text = Session("REPORTID")
            End If
            Session("CameFromReport") = True
        Else
            Session("REPORTID") = ""
            Session("CameFromReport") = False
        End If
        ButtonUploadFile.OnClientClick = "showSpinner();"

        btnBrowse.OnClientClick = "clickFileUpload();return false;"
        FileRDL.Attributes.Add("onchange", "getAttachedFile();")
        TextboxOrientation.Enabled = False
        TextBoxReportTitle.Enabled = False
        HyperLinkReportEdit.Visible = False
        If Not IsPostBack Then
            'DropDownTables
            Dim er As String = String.Empty
            Dim ddtv As DataView = GetListOfUserTables(False, Session("UserConnString"), Session("UserConnProvider"), er, Session("logon"), Session("CSV"))
            If ddtv Is Nothing OrElse ddtv.Table Is Nothing Then
                LabelAlert1.Text = "No tables in db available for the user yet..."
                LabelAlert1.Visible = True
                Exit Sub
            End If
            If er <> "" Then
                LabelAlert1.Text = er
                LabelAlert1.Visible = True
                Exit Sub
            End If
            DropDownTables.Items.Add(" ")
            For i = 0 To ddtv.Table.Rows.Count - 1
                Dim li As New ListItem
                'li.Text = ddtv.Table.Rows(i)("TABLENAME").ToString & "(" & ddtv.Table.Rows(i)("TABLE_NAME").ToString & ")"
                li.Text = ddtv.Table.Rows(i)("TABLE_NAME").ToString
                Try
                    If Not ddtv.Table.Rows(i)("TABLENAME") Is Nothing AndAlso ddtv.Table.Rows(i)("TABLENAME").ToString.Trim <> "" AndAlso ddtv.Table.Rows(i)("TABLENAME").ToString.Trim <> ddtv.Table.Rows(i)("TABLE_NAME").ToString.Trim Then
                        li.Text = ddtv.Table.Rows(i)("TABLE_NAME").ToString & "(" & ddtv.Table.Rows(i)("TABLENAME").ToString & ")"
                    End If
                Catch ex As Exception

                End Try

                li.Value = ddtv.Table.Rows(i)("TABLE_NAME").ToString
                DropDownTables.Items.Add(li)
            Next

            Session("tablenm") = " "
            If Session("UserDB") = Nothing OrElse Session("UserDB") = String.Empty Then
                Dim userprov = Session("UserConnProvider")

                If userprov <> "Oracle.ManagedDataAccess.Client" Then
                    If Session("UserConnString").ToUpper.IndexOf("USER ID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("USER ID")).Trim
                    If Session("UserConnString").IndexOf("UID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").IndexOf("UID")).Trim
                Else
                    If Session("UserConnString").ToUpper.IndexOf("PASSWORD") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("PASSWORD")).Trim
                End If
            End If
            Session("dbname") = GetDataBase(Session("UserConnString"), Session("UserConnProvider")).Replace("#", "")
        End If
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If TextboxRepID.Text.Trim = "" Then
            hlkSeeImportedData.Visible = False
            hlkSeeImportedData.Enabled = False
        End If

        hfSizeLimit.Value = getMaxRequestLength().ToString

        Dim target As String = Request("__EVENTTARGET")
        Dim data As String = Request("__EVENTARGUMENT")

        If target IsNot Nothing AndAlso data IsNot Nothing Then
            If target = "FileSizeExceeded" Then
                MessageBox.Show(data, "File Size Exceeded", target, Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning)
                Exit Sub
            End If
        End If

        Session("tablenm") = DropDownTables.Text

    End Sub

    Protected Sub ButtonUploadFile_Click(sender As Object, e As EventArgs) Handles ButtonUploadFile.Click
        Dim fnum As Integer = 0
        'Dim fileuploadDir As String = System.AppDomain.CurrentDomain.BaseDirectory() & "\Temp\"
        Dim fileuploadDir As String = applpath 'ConfigurationManager.AppSettings("fileupload").ToString.Replace("/", "\") & "\"
        'fileuploadDir = fileuploadDir.Replace("\\", "\")
        Dim filename As String = String.Empty
        Dim fileExt As String = String.Empty
        Dim strFile As String = String.Empty
        Dim weborlocal As String = String.Empty
        Dim csvpath As String = String.Empty

        If (txtURI.Text.Trim = "" OrElse txtURI.Text.Trim = "https://" OrElse txtURI.Text.Trim = "http://") AndAlso Not FileRDL.HasFile Then
            Session("NewFileCreated") = False
            btnBrowse.Focus()
            MessageBox.Show("File is not selected.", "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            Exit Sub
        End If
        If txtURI.Text.Trim <> "" AndAlso txtURI.Text.Trim <> "https://" AndAlso txtURI.Text.Trim <> "http://" Then    'web file
            weborlocal = "web"
            fnum = 1
            ' Validate Download the file from website - proper url format
            If Not Uri.IsWellFormedUriString(txtURI.Text, UriKind.Absolute) Then
                LabelAlert.Text = "ERROR!! The URL format is not valid. Please enter a valid URL."
                txtURI.BorderColor = Color.Red
                Exit Sub
            End If
            If txtURI.Text.Trim.LastIndexOf(".") < 0 Then
                WriteToAccessLog(Session("logon"), "Format of URL does not supported: " & txtURI.Text & " It should end with .CSV, .XML, .JSON, .TXT, .XLS, .XLSX, .MDB, .ACCDB ", 2)
                LabelAlert.Text = "ERROR!! Format of URL is not supported. It should end with .CSV, .XML, .JSON, .TXT, .XLS, .XLSX, .MDB, .ACCDB "
                txtURI.BorderColor = Color.Red
                Exit Sub
            End If
            If txtURI.Text.Trim.LastIndexOf(".") > 0 Then
                fileExt = txtURI.Text.Trim.Substring(txtURI.Text.Trim.LastIndexOf("."))
                If ",.CSV,.XML,.JSON,.TXT,.XLS,.XLSX,.MDB,.ACCDB,".IndexOf(fileExt.ToUpper) < 0 Then
                    WriteToAccessLog(Session("logon"), "Format of URL does not supported. It should end with .CSV, .XML, .JSON, .TXT, .XLS, .XLSX, .MDB, .ACCDB ", 2)
                    LabelAlert.Text = "ERROR!! Format of URL is not supported. It should end with .CSV, .XML, .JSON, .TXT, .XLS, .XLSX, .MDB, .ACCDB "
                    txtURI.BorderColor = Color.Red
                    Exit Sub
                End If
            End If
            filename = Session("logon") & "_" & txtURI.Text.Replace(" ", "").Replace("/", "").Replace("\", "").Replace("_", "").Replace("%", "").Replace("http", "").Replace(":", "").Replace(".", "")
            If filename.Length > 70 Then
                filename = filename.Substring(0, 70)
            End If
            filename = filename.Replace(".", "") & fileExt
            strFile = fileuploadDir & filename
            strFile = strFile.Replace(" ", "\")
        Else
            weborlocal = "local"
            If Not FileRDL.HasFiles OrElse Not FileRDL.HasFile OrElse FileRDL.PostedFiles.Count = 0 Then
                MessageBox.Show("No file selected", "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                Exit Sub
            End If
            fnum = FileRDL.PostedFiles.Count
            filename = FileRDL.PostedFile.FileName.Trim
            fileExt = filename.Substring(filename.LastIndexOf(".")).Trim
            'strFile = fileuploadDir & Session("logon") & "_" & filename.Replace("/", "").Replace("\", "").Replace(":", "").Replace(" ", "").Replace(".", "") & Now.ToShortDateString.Replace("/", "") & Now.ToShortTimeString.Replace(":", "").Replace(" ", "").Replace(".", "") & "." & fileExt
            strFile = fileuploadDir & Session("logon") & "_" & filename.Replace("/", "").Replace("\", "").Replace(":", "").Replace(" ", "").Replace(".", "") & Now.ToShortDateString.Replace("/", "") & Now.ToShortTimeString.Replace(":", "").Replace(" ", "").Replace(".", "") & fileExt
            strFile = strFile.ToLower.Replace(" ", "")
        End If

        Dim ret As String = String.Empty
        Dim ErrorLog = String.Empty
        Dim n As Integer = 0
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim exst As Boolean = False
        Dim er As String = String.Empty
        Dim ntables As Integer = 0
        Dim dv As DataView
        Dim rtbl As String = String.Empty
        Dim k As Integer
        Dim msql As String = String.Empty

        If fnum = 1 Then
            '============================================  1 file  ==================================================
            If weborlocal = "web" Then   'web file
                If fileExt.ToUpper <> ".CSV" AndAlso fileExt.ToUpper <> ".XML" AndAlso fileExt.ToUpper <> ".JSON" Then

                    'DOWNLOAD from URL IN LOCAL SERVER FILE starts---------------------------------------------------------------------------------
                    'download with WebClient - another way to read or download data!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    Dim client = New WebClient
                    'save file in strFile
                    Try
                        'smallfile = True
                        client.DownloadFile(txtURI.Text, strFile)  '"C:\Uploads\" & filename
                        If Not (fileExt.ToUpper.EndsWith(".MDB") OrElse fileExt.ToUpper.EndsWith(".ACCDB") OrElse fileExt.ToUpper.EndsWith(".XLS") OrElse fileExt.ToUpper.EndsWith(".XLSX")) Then
                            'clean and save filetext in fpath
                            Dim filetext As String = File.ReadAllText(strFile)
                            filetext = filetext.Replace("http://", "").Replace("https://", "").Replace("file:///", "")
                            filetext = filetext.Replace("schemas.microsoft.com/", "http://schemas.microsoft.com/")
                            'TODO ? if we want to keep file on our server
                            filetext = cleanTextOfFile(filetext)
                            File.Delete(strFile)
                            File.WriteAllText(strFile, filetext)
                        End If

                    Catch ex As Exception
                        'smallfile = False
                        Try
                            'DO NOT DELETE! We can use it for big files or cut them to 4 mg pieces 
                            Dim Stream As Stream = client.OpenRead(txtURI.Text)
                            Dim SR As StreamReader = New StreamReader(Stream)
                            Dim SW As StreamWriter = New StreamWriter(strFile)
                            Dim strsr As String = String.Empty
                            While Not SR.EndOfStream
                                k = k + 1
                                strsr = SR.ReadLine   'SR.ReadToEnd()  'ReadLine
                                SW.WriteLine(strFile, strsr)
                            End While
                            SR.Close()
                            SW.Close()
                            Stream.Close()

                        Catch ex1 As Exception
                            WriteToAccessLog(Session("logon"), "Download from url  " & txtURI.Text & " crashed with error: " & ex.Message, 2)
                            LabelAlert.Text = "ERROR!! " & ex1.Message
                            Exit Sub
                        End Try
                    End Try

                    'from local downloaded from web
                    TextboxPageFtr.Text = "Last imported from the " & txtURI.Text & " on " & Now.ToString & ", downloaded to " & strFile

                ElseIf fileExt.ToUpper = ".CSV" OrElse fileExt.ToUpper = ".XML" OrElse fileExt.ToUpper = ".JSON" Then
                    'csv, xml, json on web - import from web without downloads ---------------------------------------------------------------------

                    TextboxPageFtr.Text = "Last imported from the " & txtURI.Text & " on " & Now.ToString

                End If

                'DOWNLOAD from URL IN LOCAL SERVER FILE ends------------------------------------------------------------------------------------------

            ElseIf FileRDL.HasFile AndAlso FileRDL.PostedFile IsNot Nothing AndAlso FileRDL.PostedFile.FileName.Trim <> "" Then  'local file, not uri

                'UPLOAD from LOCAL FILE to LOCAL SERVER starts----------------------------------------------------------------------------------------
                Try
                    FileRDL.PostedFile.SaveAs(strFile)
                Catch ex As Exception
                    btnBrowse.Focus()
                    MessageBox.Show(ex.Message, "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                    Exit Sub
                End Try

                TextboxPageFtr.Text = "Last imported from the file " & FileRDL.PostedFile.FileName & " on " & Now & ", downloaded to " & strFile

                'UPLOAD from LOCAL FILE to LOCAL SERVER ends---------------------------------------------------------------------------------

            End If

            'After this - Do Import if needed from the local file strFile 
            csvpath = strFile

            'check if file is ok (not for Access db)
            If weborlocal = "local" AndAlso (fileExt.ToUpper.EndsWith(".CSV") OrElse fileExt.ToUpper.EndsWith(".XML") OrElse fileExt.ToUpper.EndsWith(".JSON") OrElse fileExt.ToUpper.EndsWith(".TXT")) Then
                ret = cleanFile(csvpath)
                If ret <> csvpath Then
                    Try
                        ret = ""
                        'delete csvpath
                        File.Delete(csvpath)
                        File.WriteAllText(csvpath, ret)
                    Catch ex As Exception
                        ret = ex.Message
                    End Try
                    ErrorLog = "File is dangerous and not imported. " & ret
                    WriteToAccessLog(Session("logon"), "File was dangerous and has been clean up. " & ret, 2)
                    LabelAlert.Text = "File was dangerous and had been cleaned up. "
                    'Exit Sub
                End If
            End If


            'table---------------------------------------------------------------------------------------------------------

            'name of table
            tblname = cleanText(txtTableName.Text.Trim)
            If Session("UserConnProvider").StartsWith("InterSystems.Data.") Then
                If tblname.StartsWith("User.") Then tblname = tblname.Replace("User.", "")
                If tblname.IndexOf(".") < 0 Then
                    tblname = "userdata." & tblname
                End If
            End If
            Dim tblfriendlyname As String = cleanText(txtTableName.Text)
            If tblname.Trim <> "" Then
                'check if this table name exist in user tables dropdown
                For i = 0 To DropDownTables.Items.Count - 1
                    If DropDownTables.Items(i).Value.Trim.ToUpper = tblname.ToUpper Then
                        DropDownTables.SelectedItem.Value = tblname
                        txtTableName.Text = ""
                        Exit For
                    End If
                Next
            End If
            If DropDownTables.Items.Count > 1 AndAlso DropDownTables.SelectedItem.Value.Trim <> "" Then
                'existing table from dropdown
                tblname = DropDownTables.SelectedItem.Value
                exst = True
            Else 'tblname is not in dropdown of user tables, table name from textbox
                If tblname.Trim = "" Then
                    'textbox with table name is empty 
                    tblname = Session("logon") & Now.ToShortDateString.Replace("/", "") & Now.ToShortTimeString.Replace(":", "").Replace(" ", "")
                    tblname = tblname.ToLower.Replace(" ", "")
                    txtTableName.Text = tblname
                    exst = False
                End If
                'if exist it belong to some other user
                If TableExists(txtTableName.Text.Trim, Session("UserConnString"), Session("UserConnProvider"), er) Then
                    'not clear data from existing table - create new table correcting name because it exist, but was not selected from the list of existing tables
                    tblname = txtTableName.Text.Trim & Now.ToShortDateString.Replace("/", "") & Now.ToShortTimeString.Replace(":", "").Replace(" ", "")
                    tblname = tblname.ToLower.Replace(" ", "")
                    txtTableName.Text = tblname
                    exst = False
                Else 'new table
                    tblname = tblname.ToLower.Replace(" ", "")
                    exst = False
                End If

            End If

            '-------------------------------------------------------------------------------------------------------------------

            'Create report
            TextBoxSQL.Text = "SELECT * FROM " & tblname
            If Session("REPORTID") = "" Then
                TextBoxReportTitle.Text = tblname & " updated on " & Now.ToString.Replace(":", "-").Replace("/", "-")
            Else
                If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString.Trim <> "" Then
                    TextBoxReportTitle.Text = Session("REPTITLE").ToString.Trim.Replace(":", "-")
                Else
                    TextBoxReportTitle.Text = tblname & " updated on " & Now.ToString.Replace(":", "-").Replace("/", "-")
                End If
            End If
            Session("REPTITLE") = TextBoxReportTitle.Text.Replace(":", "-")
            If TextboxPageFtr.Text.Trim = "" Then
                TextboxPageFtr.Text = TextBoxReportTitle.Text
            End If

            '----------------------------------------------------------------------------
            CreateReport()
            '----------------------------------------------------------------------------

            If Session("REPORTID") = "" Then
                repid = ""
                LabelAlert.Text = "Error!! Creating the report crashed. "
                Exit Sub
            End If
            repid = Session("REPORTID")
            TextboxRepID.Text = Session("REPORTID")
            HyperLinkReportEdit.Visible = True

            If weborlocal = "web" Then
                'save uri in ourreportinfo
                ErrorLog = ExequteSQLquery("UPDATE OURReportInfo SET Param4id='" & txtURI.Text.Trim & "' WHERE (ReportID='" & repid & "')")
            End If
            '----------------------------------------------------------------------------
            'ret = InsertTableIntoOURUserTables(tblname, tblfriendlyname, Session("Unit"), Session("logon"), Session("UserDB"), "", repid)
            '----------------------------------------------------------------------------

            Dim onemany As String = "one"
            If fileExt.ToUpper = ".XML" OrElse fileExt.ToUpper = ".JSON" OrElse fileExt.ToUpper = ".RDL" OrElse fileExt.ToUpper = ".TXT" Then
                onemany = "many"
                msql = "UPDATE [OURReportInfo] SET Param6id='" & fileExt & "' WHERE([ReportID] ='" & repid & "')"
                ret = ExequteSQLquery(msql)
            End If

            'clear tables
            If exst AndAlso chkboxClearTable.Checked Then
                Dim res As String = ClearTables(tblname, Session("UserConnString"), Session("UserConnProvider"), Session("Unit"), Session("logon"), Session("UserDB"), repid, er, onemany)
                If res.StartsWith("ERROR!!") OrElse res.StartsWith("Table copied, but not deleted: ") Then
                    MessageBox.Show(tblname & " - " & res, "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                    Exit Sub
                End If
            End If


            '============================================================ start 1 file import =======================================================================

            If fileExt.Trim.ToUpper.EndsWith(".CSV") Then  'OrElse FileUpl.PostedFile.FileName.ToUpper.EndsWith(".PRN")
                'Import CSV into Table
                Dim d As String = TextboxDelimiter.Text.Trim.Substring(0, 1)
                If d = "" Then d = ","
                If weborlocal = "web" Then
                    'NEW Line by Line from URL  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    ret = ImportCSVintoDbTableByLine(Session("logon"), tblname, txtURI.Text.Trim, d, Session("UserConnString"), Session("UserConnProvider"), exst)
                Else  'local file

                    ret = ImportCSVintoDbTable(Session("logon"), tblname, csvpath, d, Session("UserConnString"), Session("UserConnProvider"), exst)
                End If
                If (ret <> "Query executed fine." OrElse ret.StartsWith("ERROR!!")) AndAlso exst AndAlso chkboxClearTable.Checked Then
                    'restore data in the table from DELETED
                    ret = ret & ", " & RestoreDataFromDELETED(tblname, Session("UserConnString"), Session("UserConnProvider"))
                End If

                '--------------------------------------------------------------------------------------------------------------
            ElseIf fileExt.ToUpper.EndsWith(".XLS") OrElse fileExt.ToUpper.EndsWith(".XLSX") Then   'OrElse txtURI.Text.Trim.ToUpper.EndsWith(".PRN")
                'import Excel file into Table
                ret = ImportExcelIntoDbTable(tblname, csvpath, Session("UserConnString"), Session("UserConnProvider"), exst)
                If (ret <> "Query executed fine." OrElse ret.StartsWith("ERROR!!")) Then
                    'ret = ret & " Attention!! Save your file as Microsoft Excel 97-2003 Worksheet and try to import it again."
                    If exst AndAlso chkboxClearTable.Checked Then
                        'restore data in the table from DELETED
                        ret = ret & ", " & RestoreDataFromDELETED(tblname, Session("UserConnString"), Session("UserConnProvider"))
                    End If
                End If

                '--------------------------------------------------------------------------------------------------------------
            ElseIf fileExt.ToUpper.EndsWith(".ACCDB") OrElse fileExt.ToUpper.EndsWith(".MDB") Then
                'import Access table into Table
                ret = ImportExcelIntoDbTable(tblname, csvpath, Session("UserConnString"), Session("UserConnProvider"), exst)
                If (ret <> "Query executed fine." OrElse ret.StartsWith("ERROR!!")) Then
                    ret = ret & " Attention!! You can export Access table into the Microsoft Excel 97-2003 Worksheet and try to import it again."
                    If exst AndAlso chkboxClearTable.Checked Then
                        'restore data in the table from DELETED
                        ret = ret & ", " & RestoreDataFromDELETED(tblname, Session("UserConnString"), Session("UserConnProvider"))
                    End If
                End If

                '--------------------------------------------------------------------------------------------------------------
            ElseIf fileExt.ToUpper.EndsWith(".XML") OrElse fileExt.ToUpper.EndsWith(".RDL") Then
                'Import XML into Table
                If weborlocal = "web" Then
                    ret = ImportXMLorJSONintoDatabaseFromURL(Session("REPORTID"), tblname, txtURI.Text.Trim, Session("UserConnString"), Session("UserConnProvider"), Session("Unit"), Session("logon"), Session("UserDB"), True, chkboxUniqueFields.Checked)
                Else  'local file
                    ret = ImportXMLorJSONintoDatabase(Session("REPORTID"), tblname, csvpath, Session("UserConnString"), Session("UserConnProvider"), Session("Unit"), Session("logon"), Session("UserDB"), True, chkboxUniqueFields.Checked)
                End If
                If ret.StartsWith("ERROR!!") Then
                    'delete csvpath
                    Try
                        File.Delete(csvpath)
                    Catch ex As Exception
                        ret = ret & "ERROR!! " & ex.Message
                    End Try
                    LabelAlert.Text = ret
                    LabelAlert.Visible = True
                    Exit Sub
                End If
                Dim sqlq As String = String.Empty
                If Session("UserConnProvider") = "Sqlite" Then
                    'Make dv3 joining all tables: "
                    dt = MakeTableWithAllJoins(Session("REPORTID"))
                    'tblname = Session("REPORTID")
                    ret = ImportDataTableIntoDb(dt, tblname, "Sqlite", "Sqlite", er)
                    TextBoxSQL.Text = "SELECT * FROM " & tblname
                    'ret = MakeNewUserReport(Session("logon"), Session("REPORTID"), "All Joined Data for " & Session("REPORTID"), Session("UserDB"), TextBoxSQL.Text, Session("dbname"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)
                    sqlq = "UPDATE OURReportInfo SET SQLquerytext='" & TextBoxSQL.Text & "', Param8type='USESQLTEXT' WHERE ReportId ='" & Session("REPORTID") & "'  And ReportDB Like '%" & Session("dbname") & "%'"
                    ret = ExequteSQLquery(sqlq)
                Else
                    'make report with all joins
                    Dim sqlst As String = String.Empty
                    dt = MakeTableWithAllJoins(Session("REPORTID"), sqlst, er)
                    'ret = MakeNewUserReport(Session("logon"), Session("REPORTID"), "All Joined Data for " & Session("REPORTID"), Session("UserDB"), sqlst, Session("dbname"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)
                    sqlq = "UPDATE OURReportInfo SET SQLquerytext='" & sqlst.ToString & "', Param8type='USESQLTEXT' WHERE ReportId ='" & Session("REPORTID") & "'  And ReportDB Like '%" & Session("dbname") & "%'"
                    ret = ExequteSQLquery(sqlq)
                    TextBoxSQL.Text = sqlst
                End If


                '--------------------------------------------------------------------------------------------------------------
            ElseIf fileExt.ToUpper.EndsWith(".TXT") OrElse fileExt.ToUpper.EndsWith(".JSON") Then
                'Import JSON into tables
                If weborlocal = "web" Then
                    ret = ImportXMLorJSONintoDatabaseFromURL(Session("REPORTID"), tblname, txtURI.Text.Trim, Session("UserConnString"), Session("UserConnProvider"), Session("Unit"), Session("logon"), Session("UserDB"), False, chkboxUniqueFields.Checked)
                Else  'local file
                    ret = ImportXMLorJSONintoDatabase(Session("REPORTID"), tblname, csvpath, Session("UserConnString"), Session("UserConnProvider"), Session("Unit"), Session("logon"), Session("UserDB"), False, chkboxUniqueFields.Checked)
                End If
                If ret.StartsWith("ERROR!!") Then
                    'delete csvpath
                    Try
                        File.Delete(csvpath)
                    Catch ex As Exception
                        ret = ret & "ERROR!! " & ex.Message
                    End Try
                    LabelAlert.Text = ret
                    LabelAlert.Visible = True
                    Exit Sub
                End If

                Dim sqlq As String = String.Empty
                If Session("UserConnProvider") = "Sqlite" Then
                    'Make dv3 joining all tables: "
                    dt = MakeTableWithAllJoins(Session("REPORTID"))
                    'tblname = Session("REPORTID")
                    ret = ImportDataTableIntoDb(dt, tblname, "Sqlite", "Sqlite", er)
                    TextBoxSQL.Text = "SELECT * FROM " & tblname
                    'ret = MakeNewUserReport(Session("logon"), Session("REPORTID"), "All Joined Data for " & Session("REPORTID"), Session("UserDB"), TextBoxSQL.Text, Session("dbname"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)
                    sqlq = "UPDATE OURReportInfo SET SQLquerytext='" & TextBoxSQL.Text & "', Param8type='USESQLTEXT' WHERE ReportId ='" & Session("REPORTID") & "'  And ReportDB Like '%" & Session("dbname") & "%'"
                    ret = ExequteSQLquery(sqlq)
                Else
                    'make report with all joins
                    Dim sqlst As String = String.Empty
                    dt = MakeTableWithAllJoins(Session("REPORTID"), sqlst, er)
                    'ret = MakeNewUserReport(Session("logon"), Session("REPORTID"), "All Joined Data for " & Session("REPORTID"), Session("UserDB"), sqlst, Session("dbname"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)
                    sqlq = "UPDATE OURReportInfo SET SQLquerytext='" & sqlst.ToString & "', Param8type='USESQLTEXT' WHERE ReportId ='" & Session("REPORTID") & "'  And ReportDB Like '%" & Session("dbname") & "%'"
                    ret = ExequteSQLquery(sqlq)
                    TextBoxSQL.Text = sqlst

                End If


            End If

            If ret.StartsWith("ERROR!!") Then
                'delete csvpath
                Try
                    File.Delete(csvpath)
                Catch ex As Exception
                    ret = ret & "ERROR!! " & ex.Message
                End Try
                LabelAlert.Text = ret
                LabelAlert.Visible = True
                Exit Sub
            End If

            '============================================================ end 1 file import =======================================================================
            Dim rep As String = Session("REPORTID")
            If onemany = "many" Then
                'make loop to register the tables tblname(i) in OURUserTables of OURdb using main report that have all joins of subtables
                dv = GetReportTablesFromOURUserTables(repid, Session("Unit"), Session("logon"), Session("UserDB"), ret)
                If dv Is Nothing OrElse dv.Table Is Nothing Then
                    Label1.Text = "There are no tables found for this report."
                    Exit Sub
                End If
                ntables = dv.Table.Rows.Count
                'make loop to make initial reports for the tables in OURUserTables of OURdb
                For i = 0 To ntables - 1
                    j = j + 1
                    rtbl = FixReservedWords(dv.Table.Rows(i)("TableName").ToString.Trim, Session("UserConnProvider"))
                    'TODO correct fields types
                    ret = CorrectFieldsInTable(rtbl, Session("UserConnString"), Session("UserConnProvider"), True)

                    If ret.Contains("ERROR!!") AndAlso ret.Contains("Table '" & rtbl.ToUpper & "' not found") Then
                        ret = ExequteSQLquery("DELETE FROM OURUserTables WHERE  [Unit]='" & Session("Unit") & "' AND [UserID]='" & Session("logon") & "' AND [UserDB]='" & Session("UserDB") & "' AND [Prop2]='" & rep & "' And TableName='" & rtbl & "'", Session("UserConnString"), Session("UserConnProvider"))
                        Continue For
                    End If

                    ' Make new reports for the table !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    Dim repsqlquery As String = "SELECT * FROM " & rtbl
                    rep = Session("REPORTID") & i.ToString
                    ret = MakeNewUserReport(Session("logon"), rep, rtbl, Session("UserDB"), repsqlquery, Session("dbname"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)
                    WriteToAccessLog(Session("logon"), "User Report " & rep & " has been created with result:   " & ret, 111)
                    rep = Session("dbname") & "_INIT_" & i.ToString & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
                    ret = MakeNewStanardReport(Session("logon"), rep, rtbl, Session("UserDB"), repsqlquery, Session("dbname"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)

                Next
            End If

            'save report and update dashboards
            ButtonSubmit_Click(repid, tblname)

            n = 0
            ret = UpdateDashboardsWithReport(Session("REPORTID"), n, Session("UserConnString"), Session("UserConnProvider"), er)

            'delete csvpath
            Try
                File.Delete(csvpath)
            Catch ex As Exception
                ret = "ERROR!! " & ex.Message
                LabelAlert.Text = ret
                LabelAlert.Visible = True
            End Try
            '============================================  1 file import end ==================================================
            'FindReportsWithTable(tblname)
            If onemany = "many" Then
                Response.Redirect("Analytics.aspx")
            End If
            '==================================================================================================================
            '============================================  multiple files from local computer ===========================================
        Else  'multiple files from local computer only CSV, XML, JSON, RDL, TXT
            Dim ext As String = String.Empty
            Dim onemany As String = "one" 'one table
            If TextboxPageFtr.Text.Trim = "" Then
                TextboxPageFtr.Text = "Last imported"
            Else
                TextboxPageFtr.Text = TextboxPageFtr.Text & ", Last imported"
            End If

            For k = 0 To FileRDL.PostedFiles.Count - 1
                'post file to fileuploadDir
                filename = FileRDL.PostedFiles(k).FileName.Trim
                Try
                    FileRDL.PostedFiles(k).SaveAs(fileuploadDir & "/" & filename)
                Catch ex As Exception
                    'btnBrowse.Focus()
                    MessageBox.Show(ex.Message & ". File " & filename & "is erroring. Import stopped.", "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                    Exit Sub
                End Try
                TextboxPageFtr.Text = TextboxPageFtr.Text & " from the file " & filename & " on " & Now
                If k < FileRDL.PostedFiles.Count - 1 Then
                    TextboxPageFtr.Text = TextboxPageFtr.Text & ", "
                End If
                'check if file is not dangerous
                csvpath = fileuploadDir & "/" & filename
                ret = cleanFile(csvpath)
                If ret <> csvpath Then
                    Try
                        'delete csvpath
                        File.Delete(csvpath)
                    Catch ex As Exception
                        ret = ex.Message
                    End Try
                    ErrorLog = "File " & filename & " is dangerous and not imported. Import stopped. " & ret
                    MessageBox.Show(ErrorLog, "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                    Exit Sub
                End If
                '==============================================================================

                If k = 0 Then

                    'check extension
                    fileExt = filename.Substring(filename.LastIndexOf(".")).Trim
                    ext = fileExt
                    If fileExt.ToUpper = ".XML" OrElse fileExt.ToUpper = ".JSON" OrElse fileExt.ToUpper = ".RDL" OrElse fileExt.ToUpper = ".TXT" Then
                        onemany = "many"  'many tables
                    ElseIf fileExt.ToUpper = ".CSV" OrElse fileExt.ToUpper = ".XLS" OrElse fileExt.ToUpper = ".XLSX" Then
                        onemany = "one"
                    Else
                        MessageBox.Show("File " & filename & "with this extension is not supported. Import stopped. ", "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                        Exit Sub
                    End If

                    'clear tables if requested, put if tblname & "DELETED"
                    If exst AndAlso chkboxClearTable.Checked Then
                        Dim res As String = ClearTables(tblname, Session("UserConnString"), Session("UserConnProvider"), Session("Unit"), Session("logon"), Session("UserDB"), repid, er, onemany)
                        If res.StartsWith("ERROR!!") OrElse res.StartsWith("Table copied, but not deleted:  ") Then
                            MessageBox.Show(tblname & " - " & res, "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                            Exit Sub
                        End If
                    End If
                Else
                    If fileExt <> ext Then
                        MessageBox.Show("All selected files should have the same extension. File " & filename & " has a different extension. Import stopped.", "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                        Exit Sub
                    End If
                    exst = True
                End If

                '============================================================ start import =======================================================================

                If fileExt.Trim.ToUpper.EndsWith(".CSV") Then  'OrElse FileUpl.PostedFile.FileName.ToUpper.EndsWith(".PRN")
                    'Import CSV into Table
                    Dim d As String = TextboxDelimiter.Text.Trim.Substring(0, 1)
                    If d = "" Then d = ","
                    ret = ImportCSVintoDbTable(Session("logon"), tblname, csvpath, d, Session("UserConnString"), Session("UserConnProvider"), exst)
                    If (ret <> "Query executed fine." OrElse ret.StartsWith("ERROR!!")) AndAlso exst AndAlso chkboxClearTable.Checked Then
                        'restore data in the table from DELETED if they were cleared, only for first file
                        If k = 0 Then
                            ret = ret & ", " & RestoreDataFromDELETED(tblname, Session("UserConnString"), Session("UserConnProvider"))
                        End If
                        MessageBox.Show("Import CSV File " & filename & " result: " & ret & ". Import stopped. ", "Import CSV File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                        Exit For
                    ElseIf ret = "Query executed fine." Then
                        'If exst Then
                        '    MessageBox.Show("Import CSV File result: " & tblname & " table updated from " & filename & ".", "Import CSV File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information)
                        'Else
                        '    MessageBox.Show("Import CSV File result: " & tblname & " table created from " & filename & ".", "Import CSV File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information)
                        'End If
                    Else
                        Session("REPORTID") = ""
                        Session("REPTITLE") = ""
                        TextBoxSQL.Text = ""
                        TextBoxReportTitle.Text = ""
                        TextboxPageFtr.Text = ""
                        TextboxRepID.Text = ""
                        MessageBox.Show("Import CSV File " & filename & " result: " & ret & ". Import stopped. ", "Import CSV File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                        Exit Sub
                    End If

                    '--------------------------------------------------------------------------------------------------------------
                ElseIf fileExt.ToUpper.EndsWith(".XLS") OrElse fileExt.ToUpper.EndsWith(".XLSX") Then   'OrElse txtURI.Text.Trim.ToUpper.EndsWith(".PRN")
                    'import Excel file into Table
                    ret = ImportExcelIntoDbTable(tblname, csvpath, Session("UserConnString"), Session("UserConnProvider"), exst)
                    If (ret <> "Query executed fine." OrElse ret.StartsWith("ERROR!!")) Then
                        ret = ret & " Attention!! Save your file as Microsoft Excel 97-2003 Worksheet and try to import it again."
                        If exst AndAlso chkboxClearTable.Checked Then
                            'restore data in the table from DELETED
                            ret = ret & ", " & RestoreDataFromDELETED(tblname, Session("UserConnString"), Session("UserConnProvider"))
                        End If
                    End If
                    'MessageBox.Show(ret, "Import Excel File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                    '--------------------------------------------------------------------------------------------------------------
                    'ElseIf fileExt.ToUpper.EndsWith(".ACCDB") OrElse fileExt.ToUpper.EndsWith(".MDB") Then
                    '    'import Access table into Table
                    '    ret = ImportExcelIntoDbTable(tblname, csvpath, Session("UserConnString"), Session("UserConnProvider"), exst)
                    '    If (ret <> "Query executed fine." OrElse ret.StartsWith("ERROR!!")) Then
                    '        ret = ret & " Attention!! You can export Access table into the Microsoft Excel 97-2003 Worksheet and try to import it again."
                    '        If exst AndAlso chkboxClearTable.Checked Then
                    '            'restore data in the table from DELETED
                    '            ret = ret & ", " & RestoreDataFromDELETED(tblname, Session("UserConnString"), Session("UserConnProvider"))
                    '        End If
                    '    End If
                    '    MessageBox.Show(ret, "Import table from Access db", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                    '--------------------------------------------------------------------------------------------------------------
                ElseIf fileExt.ToUpper.EndsWith(".XML") OrElse fileExt.ToUpper.EndsWith(".RDL") Then
                    'Import XML into Table
                    'ret = ImportXMLintoDatabase(Session("REPORTID"), tblname, csvpath, Session("UserConnString"), Session("UserConnProvider"), Session("Unit"), Session("logon"), Session("UserDB"), ntables)
                    ret = ImportXMLorJSONintoDatabase(Session("REPORTID"), tblname, csvpath, Session("UserConnString"), Session("UserConnProvider"), Session("Unit"), Session("logon"), Session("UserDB"), True, chkboxUniqueFields.Checked)
                    If ret.StartsWith("ERROR!!") Then
                        'delete csvpath
                        Try
                            File.Delete(csvpath)
                        Catch ex As Exception
                            ret = ret & "ERROR!! " & ex.Message
                            LabelAlert.Text = ret
                            LabelAlert.Visible = True
                        End Try
                        MessageBox.Show(ret, "Import XML File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                        Exit Sub
                    End If
                    TextBoxSQL.Text = "SELECT * FROM " & tblname & "root"
                    '--------------------------------------------------------------------------------------------------------------
                ElseIf fileExt.ToUpper.EndsWith(".TXT") OrElse fileExt.ToUpper.EndsWith(".JSON") Then
                    'Import JSON into tables
                    'ret = ImportJSONintoDatabase(Session("REPORTID"), tblname, csvpath, Session("UserConnString"), Session("UserConnProvider"), Session("Unit"), Session("logon"), Session("UserDB"), ntables, chkboxUniqueFields.Checked)
                    ret = ImportXMLorJSONintoDatabase(Session("REPORTID"), tblname, csvpath, Session("UserConnString"), Session("UserConnProvider"), Session("Unit"), Session("logon"), Session("UserDB"), False, chkboxUniqueFields.Checked)
                    If ret.StartsWith("ERROR!!") Then
                        'delete csvpath
                        Try
                            File.Delete(csvpath)
                        Catch ex As Exception
                            ret = ret & "ERROR!! " & ex.Message
                            LabelAlert.Text = ret
                            LabelAlert.Visible = True
                        End Try
                        MessageBox.Show(ret, "Import JSON File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                        Exit Sub
                    End If
                    TextBoxSQL.Text = "SELECT * FROM " & tblname & "root"
                End If

                '============================================================ end import =======================================================================
                ' If exst = False Then

                If onemany = "many" Then
                    'fileExt.ToUpper.EndsWith(".TXT") OrElse fileExt.ToUpper.EndsWith(".JSON") OrElse fileExt.ToUpper.EndsWith(".XML") OrElse fileExt.ToUpper.EndsWith(".RDL")

                    'make loop to register the tables tblname(i) in OURUserTables of OURdb
                    dv = GetReportTablesFromOURUserTables(repid, Session("Unit"), Session("logon"), Session("UserDB"), ret)
                    If dv Is Nothing OrElse dv.Table Is Nothing Then
                        Label1.Text = "There are no tables found for this report."
                        Exit Sub
                    End If
                    ntables = dv.Table.Rows.Count
                    'make loop to make initial reports for the tables in OURUserTables of OURdb
                    For i = 0 To ntables - 1
                        j = j + 1
                        rtbl = FixReservedWords(dv.Table.Rows(i)("TableName").ToString.Trim, Session("UserConnProvider"))
                        'TODO correct fields types
                        ret = CorrectFieldsInTable(rtbl, Session("UserConnString"), Session("UserConnProvider"), True)

                        If ret.Contains("ERROR!!") AndAlso ret.Contains("Table '" & rtbl.ToUpper & "' not found") Then
                            ret = ExequteSQLquery("DELETE FROM OURUserTables WHERE  [Unit]='" & Session("Unit") & "' AND [UserID]='" & Session("logon") & "' AND [UserDB]='" & Session("UserDB") & "' AND [Prop2]='" & Session("REPORTID") & "' And TableName='" & rtbl & "'", Session("UserConnString"), Session("UserConnProvider"))
                            Continue For
                        End If

            ' Make new report for the table !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Dim repsqlquery As String = "SELECT * FROM " & rtbl
                        'Dim er As String = String.Empty
                        Dim rep As String = Session("REPORTID")
                        rep = Session("REPORTID") & i.ToString
                        ret = MakeNewUserReport(Session("logon"), rep, rtbl, Session("UserDB"), repsqlquery, Session("dbname"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)
                        WriteToAccessLog(Session("logon"), "User Report " & rep & " has been created with result: " & ret, 111)
                    Next
                End If

                'save report and update dashboards
                ButtonSubmit_Click(repid, tblname)

                n = 0
                ret = UpdateDashboardsWithReport(Session("REPORTID"), n, Session("UserConnString"), Session("UserConnProvider"), er)

                '=================================================================================
                'delete file from fileuploadDir, delete file filename
                Try
                    File.Delete(fileuploadDir & "/" & filename)
                Catch ex As Exception
                    ret = "ERROR!! " & ex.Message
                    LabelAlert.Text = ret
                    LabelAlert.Visible = True
                End Try

            Next

        End If

        '---------------------------------------------------------------------------------------------------------------------------------------------
        'update DropDownTables
        Dim ddtv As DataView = GetListOfUserTables(False, Session("UserConnString"), Session("UserConnProvider"), er, Session("logon"), Session("CSV"))
        If ddtv Is Nothing OrElse ddtv.Table Is Nothing Then
            LabelAlert1.Text = "No tables in db available for the user yet..."
            LabelAlert1.Visible = True
            Exit Sub
        End If
        If er <> "" Then
            LabelAlert1.Text = er
            LabelAlert1.Visible = True
            Exit Sub
        End If
        DropDownTables.Items.Clear()
        DropDownTables.Items.Add(" ")
        For i = 0 To ddtv.Table.Rows.Count - 1
            Dim li As New ListItem
            'li.Text = ddtv.Table.Rows(i)("TABLENAME").ToString & "(" & ddtv.Table.Rows(i)("TABLE_NAME").ToString & ")"
            li.Text = ddtv.Table.Rows(i)("TABLE_NAME").ToString
            Try
                If Not ddtv.Table.Rows(i)("TABLENAME") Is Nothing AndAlso ddtv.Table.Rows(i)("TABLENAME").ToString.Trim <> "" AndAlso ddtv.Table.Rows(i)("TABLENAME").ToString.Trim <> ddtv.Table.Rows(i)("TABLE_NAME").ToString.Trim Then
                    li.Text = ddtv.Table.Rows(i)("TABLE_NAME").ToString & "(" & ddtv.Table.Rows(i)("TABLENAME").ToString & ")"
                End If
            Catch ex As Exception

            End Try

            li.Value = ddtv.Table.Rows(i)("TABLE_NAME").ToString
            If li.Value = tblname Then
                li.Selected = True
            End If
            DropDownTables.Items.Add(li)
        Next
        DropDownTables.SelectedItem.Value = tblname
        '---------------------------------------------------------------------------------------------------------------------------------------------

        hlkSeeImportedData.Visible = True
        hlkSeeImportedData.Enabled = True
        HyperLinkReportEdit.Visible = True
        HyperLinkReportEdit.Enabled = True
        'if existing table show list of reports affected and update dashboards
        tdreports.Visible = "true"
        Session("dv3") = Nothing
        txtTableName.Text = ""
        LabelAlert.Visible = True
        LabelAlert.Text = "Import finished. "
        If n > 0 Then
            LabelAlert.Text = LabelAlert.Text & ret
        End If
        FindReportsWithTable(tblname)

    End Sub
    Private Sub SetReportFieldData(ByVal repid As String)
        Dim bfld As Boolean = False
        Dim insSQL As String = String.Empty
        Dim delSQL As String = String.Empty
        Dim ddt As DataTable = Nothing
        Dim dtrf As DataTable = Nothing
        Dim frname As String = String.Empty
        Dim ret As String = String.Empty

        ddt = GetListOfReportFields(repid)  'List of Report fields from xsd
        If ddt Is Nothing Then
            Exit Sub
        End If
        'add report fields from  OURReportFormat
        dtrf = GetReportFields(repid) ' from  OURReportFormat
        If dtrf Is Nothing OrElse dtrf.Rows.Count = 0 Then  'no records of Report Fields in OURReportFormat, insert them from ddt aka xsd fields...
            'add all fields from ddt
            For i As Integer = 0 To ddt.Columns.Count - 1
                If ddt.Columns(i).Caption <> "Indx" Then
                    frname = GetFriendlySQLFieldName(repid, ddt.Columns(i).Caption)
                    If frname.Trim = ddt.Columns(i).Caption.Trim Then frname = ""
                    insSQL = "INSERT INTO OURReportFormat (ReportID,Prop,Val,[Order], Prop1) VALUES('" & Session("REPORTID") & "','FIELDS','" & ddt.Columns(i).Caption & "'," & (i + 1).ToString & ",'" & frname & "')"
                    ExequteSQLquery(insSQL)
                End If
            Next
        Else
            ''delete from OURReportFormat fields that are not in ddt(xsd) and are not computed
            'WHY IS IT? This is for deleted fields
            For i As Integer = 0 To dtrf.Rows.Count - 1
                bfld = False
                For j = 0 To ddt.Columns.Count - 1
                    'check if this is computed column or from data - ddt (xsd)
                    If dtrf.Rows(i)("Prop3").ToString = "1" OrElse ddt.Columns(j).Caption = dtrf.Rows(i)("Val").ToString Then
                        bfld = True
                    End If
                Next
                If bfld = False Then
                    'delete from OURReportFormat the field not found in the data fields
                    delSQL = "DELETE FROM OURReportFormat WHERE ReportID='" & repid & "' AND Prop='FIELDS' AND Indx=" & dtrf.Rows(i)("Indx").ToString
                    ret = ExequteSQLquery(delSQL)
                    'delete from ourreportgroups
                    delSQL = "DELETE FROM OURReportGroups WHERE ReportID='" & repid & "' AND GroupField='" & dtrf.Rows(i)("Val").ToString & "' "
                    ret = ExequteSQLquery(delSQL)
                    'delete from ourreportgroups
                    delSQL = "DELETE FROM OURReportGroups WHERE ReportID='" & repid & "' AND CalcField='" & dtrf.Rows(i)("Val").ToString & "' "
                    ret = ExequteSQLquery(delSQL)
                    'delete from ourreportlists
                    delSQL = "DELETE FROM OURReportLists WHERE ReportID='" & repid & "' AND ListFld='" & dtrf.Rows(i)("Val").ToString & "' "
                    ret = ExequteSQLquery(delSQL)
                    'delete from ourreportlists
                    delSQL = "DELETE FROM OURReportLists WHERE ReportID='" & repid & "' AND RecFld='" & dtrf.Rows(i)("Val").ToString & "' "
                    ret = ExequteSQLquery(delSQL)
                    delSQL = "DELETE FROM OURReportItems WHERE ReportID = '" & repid & "' AND SQLField = '" & dtrf.Rows(i)("Val").ToString & "'"
                    ret = ExequteSQLquery(delSQL)
                    'TODO update Report or show the message to update report
                End If
            Next
        End If

    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As WebControls.TreeNode = TreeView1.SelectedNode
        Dim url As String = node.Value
        Response.Redirect(url)
    End Sub

    Protected Sub ButtonSubmit_Click(ByVal repid As String, ByVal tblname As String)
        Dim SQLq As String = String.Empty
        'Dim dv5 As DataView
        Dim er As String = String.Empty
        Dim retr As String = String.Empty
        Dim SQLtext As String = String.Empty
        Dim sUseSQLText As String = ""
        Dim ReportDate As String = DateToString(Now)
        Dim ReportEndDate As String = DateToString(Session("UserEndDate"))
        Dim PageFtr As String = TextboxPageFtr.Text
        LabelAlert.Text = ""

        SQLtext = TextBoxSQL.Text.Trim
        If SQLtext = String.Empty OrElse Piece(SQLtext.ToUpper, "FROM", 2) = String.Empty Then
            MessageBox.Show("Report tables and fields have not been chosen.", "Fields Not Chosen", "FieldsNotChosen", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information)
            Exit Sub
        Else
            retr = SaveSQLquery(repid, SQLtext)
            'page footer
            SQLq = "UPDATE OURReportInfo SET Comments='" & TextboxPageFtr.Text & "'  WHERE (ReportID='" & repid & "')"
            retr = ExequteSQLquery(SQLq)
        End If

        'update parameters
        LabelAlert.Visible = True
        SQLtext = TextBoxSQL.Text
        retr = UpdateParameters(repid, SQLtext)
        LabelAlert.Text = LabelAlert.Text & " , " & retr & " "

        'update xsd and rdl
        Dim repfile As String = String.Empty
        Session("dv3") = Nothing
        Dim ret As String = UpdateXSDandRDL(repid, Session("UserConnString"), Session("UserConnProvider"), repfile)

        ret = ret & ", " & CreateCleanReportColumnsFieldsItems(repid, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))

        WriteToAccessLog(Session("logon"), ret & "  " & LabelAlert.Text & "  " & ret, 5)
        If ret.StartsWith("ERROR!!") Then
            LabelAlert.ForeColor = Color.Gray
            LabelAlert.Text = LabelAlert.Text & "  " & ret
            LabelAlert.Text = "Report format has been updated with errors. "
        Else
            LabelAlert.Text = "Report format has been updated. "
        End If

    End Sub
    Private Sub CreateReport()
        Dim SQLq As String
        Dim dv5 As DataView
        Dim repttl As String = Session("REPTITLE")
        Dim userdb As String = Session("UserDB").trim
        Dim msg As String = String.Empty
        Dim SQLtext As String = TextBoxSQL.Text.Trim

        If Session("REPORTID") = "" Then
            repid = Session("logon") & Session("logonindx") & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
            'repid = repid.Replace(" ", "").Replace("@", "").Replace(".", "").Replace("+", "").Replace("-", "")
            repid = repid.Replace(" ", "").Replace("@", "").Replace(".", "").Replace("+", "").Replace("-", "").Replace("\", "").Replace("/", "").Replace(":", "").Replace("*", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("|", "").Replace("""", "").Replace("'", "").Replace(",", "")

            Session("REPORTID") = repid
        End If
        dv5 = mRecords("SELECT * FROM [OURReportInfo] WHERE ([ReportID]='" & repid & "')")
        If dv5.Count = 0 OrElse
           dv5.Table Is Nothing OrElse
           dv5.Table.Rows.Count = 0 Then

            SQLq = "INSERT INTO [OURReportInfo] "
            SQLq &= "([ReportId],"
            SQLq &= "[ReportName],"
            SQLq &= "[ReportTtl],"
            SQLq &= "[ReportType],"
            SQLq &= "[ReportAttributes],"
            SQLq &= "[Param7type],"
            SQLq &= "[Param9type],"
            SQLq &= "[ReportDB])"
            SQLq &= "VALUES "
            SQLq &= "('" & repid & "',"
            SQLq &= "'" & repid & "',"
            SQLq &= "'" & repttl & "',"
            SQLq &= "'rdl',"
            SQLq &= "'sql',"
            SQLq &= "'user',"
            SQLq &= "'portrait',"
            SQLq &= "'" & userdb & "')"
            msg = ExequteSQLquery(SQLq)

            msg = SaveSQLquery(repid, SQLtext)
        ElseIf Session("CameFromReport") Then 'it means that no tables was in report, but report exist
            msg = SaveSQLquery(repid, SQLtext)
        Else
            MessageBox.Show("Report ID should be unique, import denied...", "Create Report Error", "CreateError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
            Session("REPORTID") = ""
            Exit Sub
        End If
        If Session("REPORTID") <> "" AndAlso Session("logon") <> "" AndAlso Not Session("CameFromReport") Then
            Dim ret As String = String.Empty
            SQLq = "INSERT INTO [OURReportView] ("
            SQLq &= "[ReportID],"
            SQLq &= "[ReportTemplate],"
            SQLq &= "[ReportTitle],"
            SQLq &= "[CreatedBy],"
            SQLq &= "[DateCreated],"
            SQLq &= "[UpdatedBy],"
            SQLq &= "[LastUpdate])"
            SQLq &= "VALUES ("
            SQLq &= "'" & repid & "',"
            SQLq &= "'Tabular',"
            SQLq &= "'" & repttl & "',"
            SQLq &= "'" & Session("logon") & "',"
            SQLq &= "'" & DateToString(Now) & "',"
            SQLq &= "'" & Session("logon") & "',"
            SQLq &= "'" & DateToString(Now) & "')"

            ret = ExequteSQLquery(SQLq)

            If ret <> "Query executed fine." Then
                DeleteReport(repid)
                MessageBox.Show(ret, "Create Report Error", "ReportViewError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
                Session("REPORTID") = ""
                Exit Sub
            End If

            SQLq = "INSERT INTO [OURPermissions] "
            SQLq &= "([NetId],"
            SQLq &= "[Application],"
            SQLq &= "[Param1],"
            SQLq &= "[Param2],"
            SQLq &= "[Param3],"
            SQLq &= "[AccessLevel],"
            SQLq &= "[OpenFrom],"
            SQLq &= "[OpenTo],"
            SQLq &= "[Comments])"
            SQLq &= "VALUES"
            SQLq &= "('" & Session("logon") & "',"
            SQLq &= "'InteractiveReporting',"
            SQLq &= "'" & repid & "',"
            SQLq &= "'" & repttl & "',"
            SQLq &= "'" & Session("email") & "',"
            SQLq &= "'admin',"
            SQLq &= "'" & DateToString(Now) & "',"
            SQLq &= "'" & Session("UserEndDate") & "',"
            SQLq &= "'" & Session("logon") & "')"

            ret = ExequteSQLquery(SQLq)

            If ret <> "Query executed fine." Then
                MessageBox.Show(ret, "Create Report Error", "PermissionsError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
                Session("REPORTID") = ""
                Exit Sub
            End If
        End If
    End Sub
    Private Sub FindReportsWithTable(ByVal tblname As String)
        tblname = cleanText(tblname)
        If Session("UserConnProvider").StartsWith("InterSystems.Data.") Then
            If tblname.StartsWith("User.") Then tblname = tblname.Replace("User.", "")
            If tblname.IndexOf(".") < 0 Then
                tblname = "userdata." & tblname
            End If
        End If
        If tblname = "" AndAlso DropDownTables.Items.Count > 1 AndAlso DropDownTables.SelectedItem.Value.Trim <> "" Then
            tblname = DropDownTables.SelectedItem.Value.Trim
        ElseIf tblname = "" AndAlso DropDownTables.SelectedItem.Value.Trim = "" Then
            tblname = txtTableName.Text.Trim
        End If
        Session("tablenm") = tblname
        If tblname = "" Then
            Session("REPORTID") = ""
            Session("REPTITLE") = ""
            TextBoxSQL.Text = ""
            TextBoxReportTitle.Text = ""
            TextboxPageFtr.Text = ""
            TextboxRepID.Text = ""
            Exit Sub
        End If
        Try
            DropDownTables.SelectedItem.Value = tblname
        Catch ex As Exception

        End Try
        Dim SQLq As String = String.Empty ' 
        Dim dv6 As DataView = Nothing
        Dim dr As DataTable
        Dim repttl As String = String.Empty ' Session("REPTITLE")
        Dim rep As String = String.Empty
        Dim userdb As String = Session("UserDB").trim
        Dim fnd As String = String.Empty
        Dim i As Integer = 0
        Dim er As String = String.Empty
        Dim SQLtext As String = String.Empty ' TextBoxSQL.Text.Trim
        'SQLq = "SELECT DISTINCT OURReportInfo.ReportId, OURReportInfo.SQLquerytext, OURReportSQLquery.Tbl1, OURReportInfo.ReportDB FROM OURReportSQLquery INNER JOIN OURPermissions INNER JOIN OURReportInfo ON (OURReportSQLquery.ReportId = OURPermissions.Param1) AND (OURReportSQLquery.ReportId = OURReportInfo.ReportId) WHERE Tbl1='" & tblname & "' AND NetId='" & Session("logon") & "' AND ReportDB LIKE '%" & userdb.Trim.Replace(" ", "%") & "%'"

        SQLq = "SELECT DISTINCT OURReportInfo.ReportId, OURReportInfo.SQLquerytext, OURReportSQLquery.Tbl1, OURReportInfo.ReportDB,OURReportInfo.Comments,OURReportInfo.ReportTtl FROM OURReportSQLquery INNER JOIN OURPermissions  ON (OURReportSQLquery.ReportId = OURPermissions.Param1) INNER JOIN OURReportInfo ON (OURReportSQLquery.ReportId = OURReportInfo.ReportId) WHERE Tbl1='" & tblname & "' AND NetId='" & Session("logon") & "' AND ReportDB LIKE '%" & userdb.Trim.Replace(" ", "%") & "%'"

        dv6 = mRecords(SQLq, er)
        If dv6 IsNot Nothing AndAlso dv6.Table IsNot Nothing AndAlso dv6.Table.Rows.Count > 0 Then
            LabelReports.Visible = True
            LabelReports.Text = "Reports with the table " & tblname & ": "
            trheaders.Visible = True
            list.Rows(1).Cells(0).InnerHtml = "  Data "
            list.Rows(1).Cells(0).Align = "left"
            list.Rows(1).Cells(1).InnerHtml = "  Analytics  "
            list.Rows(1).Cells(1).Align = "center"
            list.Rows(1).Cells(2).InnerHtml = "  Report  "
            list.Rows(1).Cells(2).Align = "center"

            For i = 1 To dv6.Table.Rows.Count
                rep = dv6.Table.Rows(i - 1)("ReportId").ToString
                'dr = GetReportInfo(rep)
                repttl = dv6.Table.Rows(i - 1)("ReportTtl").ToString
                If dv6.Table.Rows(i - 1)("SQLquerytext").ToString.ToUpper = "SELECT * FROM " & tblname.ToUpper Then  'fnd = "" AndAlso 
                    Session("REPORTID") = dv6.Table.Rows(i - 1)("ReportId").ToString
                    TextBoxSQL.Text = dv6.Table.Rows(i - 1)("SQLquerytext").ToString
                    TextBoxReportTitle.Text = repttl
                    Session("REPTITLE") = repttl
                    TextboxPageFtr.Text = dv6.Table.Rows(i - 1)("comments").ToString
                    TextboxRepID.Text = Session("REPORTID")
                    'fnd = "found"
                End If

                AddRowIntoHTMLtable(dv6.Table.Rows(i - 1), list)
                For j = 0 To list.Rows(i + 1).Cells.Count - 1
                    list.Rows(i + 1).Cells(j).InnerText = ""
                    list.Rows(i + 1).Cells(j).InnerHtml = ""
                Next
                If i Mod 2 = 0 Then
                    list.Rows(i + 1).BgColor = "#EFFBFB"
                Else
                    list.Rows(i + 1).BgColor = "white"
                End If

                'Dim ctl As New LinkButton
                'ctl.Text = repttl
                'ctl.ID = rep & "*" & repttl
                'ctl.ToolTip = "Show " & rep
                'ctl.CssClass = "NodeStyle"
                'ctl.Font.Size = 10
                'AddHandler ctl.Click, AddressOf btnShow_Click
                'list.Rows(i + 1).Cells(0).InnerText = ""
                'list.Rows(i + 1).Cells(0).Controls.Add(ctl)

                list.Rows(i + 1).Cells(0).InnerText = repttl
                list.Rows(i + 1).Cells(0).InnerHtml = "<a href='ShowReport.aspx?didata=yes&Report=" & rep & "' data-toggle=""tooltip"" title=""" & dv6.Table.Rows(i - 1)("SQLquerytext").ToString & """ Target=""_blank"">data</a>"
                list.Rows(i + 1).Cells(0).Align = "left"

                list.Rows(i + 1).Cells(1).InnerText = "    analytics   "
                list.Rows(i + 1).Cells(1).InnerHtml = "&nbsp;&nbsp;<a href='ShowReport.aspx?didata=yes&srd=11&Report=" & rep & "' data-toggle=""tooltip"" title=""DataAI"" Target=""_blank"">analytics</a>&nbsp;&nbsp;"
                list.Rows(i + 1).Cells(1).Align = "center"

                list.Rows(i + 1).Cells(2).InnerText = rep
                list.Rows(i + 1).Cells(2).InnerHtml = "&nbsp;&nbsp;<a href='ShowReport.aspx?didata=yes&srd=3&Report=" & rep & "' data-toggle=""tooltip"" title=""" & rep & """ Target=""_blank"">" & repttl & "</a>&nbsp;&nbsp;"
                list.Rows(i + 1).Cells(2).Align = "center"

                list.Rows(i + 1).Cells(3).InnerText = "  AI   "
                list.Rows(i + 1).Cells(3).InnerHtml = "&nbsp;&nbsp;<a href='ShowReport.aspx?didata=yes&srd=15&Report=" & rep & "' data-toggle=""tooltip"" title=""DataAI"" Target=""_blank"">AI</a>&nbsp;&nbsp;"
                list.Rows(i + 1).Cells(3).Align = "center"

                If i Mod 2 = 0 Then
                    list.Rows(i + 1).BgColor = "#EFFBFB"
                Else
                    list.Rows(i + 1).BgColor = "white"
                End If
            Next
        Else
            txtTableName.Text = tblname
            DropDownTables.SelectedIndex = 0
            DropDownTables.SelectedItem.Value = " "
            LabelReports.Visible = False
            LabelReports.Text = " "
            trheaders.Visible = False
            If Not Session("CameFromReport") Then
                TextBoxSQL.Text = ""
                TextboxPageFtr.Text = ""
                TextBoxReportTitle.Text = ""
                Session("REPTITLE") = ""
                TextboxRepID.Text = ""
            End If

        End If

    End Sub
    'Protected Sub btnShow_Click(sender As Object, e As System.EventArgs)
    '    Dim ctl As LinkButton = CType(sender, LinkButton)
    '    Dim RptToShow As String = Piece(ctl.ID, "*", 1)
    '    Dim RptTtlToShow As String = Piece(ctl.ID, "*", 2)
    '    Session("REPORTID") = RptToShow
    '    Response.Redirect("ReportViews.aspx?see=yes")
    'End Sub
    Protected Sub TextBoxReportTitle_TextChanged(sender As Object, e As EventArgs) Handles TextBoxReportTitle.TextChanged
        TextBoxReportTitle.Text = cleanText(TextBoxReportTitle.Text)
        TextBoxReportTitle.TextMode = TextBoxMode.SingleLine
    End Sub
    Private Sub TextBoxReportTitle_Unload(sender As Object, e As EventArgs) Handles TextBoxReportTitle.Unload
        TextBoxReportTitle.Text = cleanText(TextBoxReportTitle.Text)
    End Sub

    Private Sub TextBoxReportTitle_PreRender(sender As Object, e As EventArgs) Handles TextBoxReportTitle.PreRender
        TextBoxReportTitle.Text = cleanText(TextBoxReportTitle.Text)
    End Sub
    Protected Sub TextBoxPageFtr_PreRender(sender As Object, e As EventArgs) Handles TextboxPageFtr.PreRender
        TextboxPageFtr.Text = cleanText(TextboxPageFtr.Text)
    End Sub

    'Private Sub btnSeeImportedData_Click(sender As Object, e As EventArgs) Handles btnSeeImportedData.Click
    '    Response.Redirect("ShowReport.aspx?srd=0")
    'End Sub

    Private Sub DropDownTables_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownTables.SelectedIndexChanged
        txtTableName.Text = ""
        tdreports.Visible = "true"
        Session("dv3") = Nothing
        FindReportsWithTable(DropDownTables.SelectedValue)
        LabelAlert.Text = ""
    End Sub

    Private Sub txtTableName_TextChanged(sender As Object, e As EventArgs) Handles txtTableName.TextChanged
        LabelAlert.Text = ""
        tblname = cleanText(txtTableName.Text.Trim)
        If Session("UserConnProvider").StartsWith("InterSystems.Data.") Then
            If tblname.StartsWith("User.") Then tblname = tblname.Replace("User.", "")
            If tblname.IndexOf(".") < 0 Then
                tblname = "userdata." & tblname
            End If
        End If
        txtTableName.Text = tblname
        txtSearch.Text = ""

        'DropDownTables restore all
        Dim er As String = String.Empty
        Dim ddtv As DataView = GetListOfUserTables(False, Session("UserConnString"), Session("UserConnProvider"), er, Session("logon"), Session("CSV"))
        If ddtv Is Nothing OrElse ddtv.Table Is Nothing Then
            LabelAlert1.Text = "No tables in db available for the user yet..."
            LabelAlert1.Visible = True
            Exit Sub
        End If
        If er <> "" Then
            LabelAlert1.Text = er
            LabelAlert1.Visible = True
            Exit Sub
        End If
        DropDownTables.Items.Clear()
        DropDownTables.Items.Add(" ")
        For i = 0 To ddtv.Table.Rows.Count - 1
            Dim li As New ListItem
            'li.Text = ddtv.Table.Rows(i)("TABLENAME").ToString & "(" & ddtv.Table.Rows(i)("TABLE_NAME").ToString & ")"
            li.Text = ddtv.Table.Rows(i)("TABLE_NAME").ToString
            Try
                If Not ddtv.Table.Rows(i)("TABLENAME") Is Nothing AndAlso ddtv.Table.Rows(i)("TABLENAME").ToString.Trim <> "" AndAlso ddtv.Table.Rows(i)("TABLENAME").ToString.Trim <> ddtv.Table.Rows(i)("TABLE_NAME").ToString.Trim Then
                    li.Text = ddtv.Table.Rows(i)("TABLE_NAME").ToString & "(" & ddtv.Table.Rows(i)("TABLENAME").ToString & ")"
                End If
            Catch ex As Exception

            End Try

            li.Value = ddtv.Table.Rows(i)("TABLE_NAME").ToString
            DropDownTables.Items.Add(li)
        Next

        If txtTableName.Text.Trim <> "" Then
            Try
                DropDownTables.Text = txtTableName.Text
                txtTableName.Text = ""
            Catch ex As Exception
                DropDownTables.SelectedIndex = 0
                DropDownTables.SelectedItem.Value = " "
            End Try

        End If
        tdreports.Visible = "true"
        Session("dv3") = Nothing
        FindReportsWithTable(tblname)
    End Sub

    Private Sub ButtonSearch_Click(sender As Object, e As EventArgs) Handles ButtonSearch.Click
        Dim srch As String = cleanText(txtSearch.Text)
        Dim er As String = String.Empty

        DropDownTables.Items.Clear()
        If srch = "" Then
            'DropDownTables
            Dim ddtv As DataView = GetListOfUserTables(False, Session("UserConnString"), Session("UserConnProvider"), er, Session("logon"), Session("CSV"))
            If ddtv Is Nothing OrElse ddtv.Table Is Nothing Then
                LabelAlert1.Text = "No tables in db available for the user yet..."
                LabelAlert1.Visible = True
                Exit Sub
            End If
            If er <> "" Then
                LabelAlert1.Text = er
                LabelAlert1.Visible = True
                Exit Sub
            End If
            DropDownTables.Items.Add(" ")
            For i = 0 To ddtv.Table.Rows.Count - 1
                Dim li As New ListItem
                'li.Text = ddtv.Table.Rows(i)("TABLENAME").ToString & "(" & ddtv.Table.Rows(i)("TABLE_NAME").ToString & ")"
                li.Text = ddtv.Table.Rows(i)("TABLE_NAME").ToString
                Try
                    If Not ddtv.Table.Rows(i)("TABLENAME") Is Nothing AndAlso ddtv.Table.Rows(i)("TABLENAME").ToString.Trim <> "" AndAlso ddtv.Table.Rows(i)("TABLENAME").ToString.Trim <> ddtv.Table.Rows(i)("TABLE_NAME").ToString.Trim Then
                        li.Text = ddtv.Table.Rows(i)("TABLE_NAME").ToString & "(" & ddtv.Table.Rows(i)("TABLENAME").ToString & ")"
                    End If
                Catch ex As Exception

                End Try

                li.Value = ddtv.Table.Rows(i)("TABLE_NAME").ToString
                DropDownTables.Items.Add(li)
            Next

        Else  'srch<>""
            txtTableName.Text = ""
            'DropDownTables with filter
            Dim ddtv As DataView = GetListOfUserTables(False, Session("UserConnString"), Session("UserConnProvider"), er, Session("logon"), Session("CSV"))
            ddtv.RowFilter = "TABLE_NAME LIKE '%" & srch & "%'"
            Dim dt As DataTable = ddtv.ToTable
            For i = 0 To dt.Rows.Count - 1
                Dim li As New ListItem
                li.Text = dt.Rows(i)("TABLE_NAME").ToString
                Try
                    If Not dt.Rows(i)("TABLENAME") Is Nothing AndAlso dt.Rows(i)("TABLENAME").ToString.Trim <> "" AndAlso dt.Rows(i)("TABLENAME").ToString.Trim <> dt.Rows(i)("TABLE_NAME").ToString.Trim Then
                        li.Text = dt.Rows(i)("TABLE_NAME").ToString & "(" & dt.Rows(i)("TABLENAME").ToString & ")"
                    End If
                Catch ex As Exception

                End Try

                li.Value = dt.Rows(i)("TABLE_NAME").ToString
                DropDownTables.Items.Add(li)
            Next
            tdreports.Visible = "true"
            Session("dv3") = Nothing
            If DropDownTables.SelectedItem IsNot Nothing AndAlso DropDownTables.SelectedIndex >= 0 Then
                FindReportsWithTable(DropDownTables.SelectedItem.Value)
            End If

        End If
        'Try
        '    If srch <> "" AndAlso DropDownTables.SelectedValue <> Session("tablenm") Then
        '        tdreports.Visible = "true"
        '        Session("dv3") = Nothing
        '        FindReportsWithTable("")
        '    End If
        '    DropDownTables.SelectedValue = Session("tablenm")
        'Catch ex As Exception

        'End Try
    End Sub
End Class
