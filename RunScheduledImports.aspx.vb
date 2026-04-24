
Imports System.Threading
Imports System.Data
Imports System.Net
Imports System.IO
Partial Class RunScheduledImports
    Inherits System.Web.UI.Page
    Private Sub RunScheduledImports_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If
        If Session("ntimerticks") = 0 Then
            Session.Timeout = 600
        End If
        Session("ntimerticks") = Session("ntimerticks") + 1
        Session.Timeout = Session.Timeout + 5
        'Dim cook As New HttpCookie("ASP.NET_SessionId")
        'cook.Value = Session.SessionID
        'cook.Expires = Date.Now.AddYears(10)
        'Response.Cookies.Add(cook)
        'HyperLink1.Focus()
        Dim i As Integer = 0
        Dim re As String = ScheduledImportsAndSendEmail(Session("logon"), Session("SupportEmail"))
    End Sub
    Private Sub RunScheduledImports_Load(sender As Object, e As EventArgs) Handles Me.Load
        LabelNruns.Text = "Number of Imports to local computer: " & Session("nruntimes").ToString & ",  Number of running the page: " & Session("ntimerticks").ToString & ", timeout in " & Session.Timeout.ToString & " minutes"
        LabelNruns.Focus()
    End Sub

    Private Sub RunTimer_Tick(sender As Object, e As EventArgs) Handles RunTimer.Tick
        Session("ntimerticks") = Session("ntimerticks") + 1
        Session.Timeout = Session.Timeout + 5
        Thread.Sleep(5000)
        Response.Redirect("RunScheduledImports.aspx")
    End Sub
    Public Function ScheduledImportsAndSendEmail(ByVal logon As String, ByVal email As String) As String
        Dim ret As String = String.Empty
        Dim er As String = String.Empty
        Dim rep As String = String.Empty
        Dim retfiles As String = String.Empty
        Dim sqlu As String = String.Empty
        Dim userlgn As String = String.Empty
        Dim i, j, n As Integer
        Dim dtv As DataView
        Dim dts As DataTable
        Dim useremail As String = String.Empty
        Dim ErrorLog As String = String.Empty
        Dim tblname As String = String.Empty
        sqlu = "SELECT * FROM ourscheduledImports WHERE UserId='" & logon & "' AND ([Status]='scheduled') AND ((Deadline LIKE '" & DateToStringFormat(Now(), "", "yyyy-MM-dd") & "%') OR (Deadline LIKE '" & DateToStringFormat(Now.AddDays(-1), "", "yyyy-MM-dd") & "%')) AND Deadline < '" & DateToStringFormat(Now(), "", "yyyy-MM-dd HH:mm:ss") & "' ORDER BY ID"

        'AND ((Deadline LIKE '" & DateToString(Now()).Substring(0, 10) & "%') OR (Deadline LIKE '" & DateToString(Now.AddDays(-1)).Substring(0, 10) & "%')) AND Deadline < '" & DateToString(Now()) & "' ORDER BY ID"
        dtv = mRecords(sqlu, ret)
        If dtv Is Nothing OrElse dtv.Count = 0 Then
            Return ""
        End If
        dts = dtv.ToTable
        Dim emails() As String
        'Dim smallfile As Boolean = True
        n = 0
        Try
            For i = 0 To dts.Rows.Count - 1
                LabelAlert.Text = " "
                'import line by line
                tblname = dts.Rows(i)("TableName").ToString.Trim
                Dim cleartable As String = dts.Rows(i)("Prop5").ToString
                Dim exst As Boolean = False
                Dim d As String = dts.Rows(i)("Prop6").ToString  'delimetr


                Dim ext As String = String.Empty
                If dts.Rows(i)("URL").ToString.Trim.LastIndexOf(".") > 7 Then
                    ext = dts.Rows(i)("URL").ToString.Trim.Substring(dts.Rows(i)("URL").ToString.Trim.LastIndexOf("."))
                Else
                    WriteToAccessLog(logon, "Format of URL does not supported: " & dts.Rows(i)("URL").ToString & " It should end with .CSV, .XML, .JSON, .TXT, .XLS, .XLSX, .MDB, .ACCDB ", 2)
                    sqlu = "UPDATE ourscheduledImports SET [Status]='wrong format of url' WHERE ID=" & dts.Rows(i)("ID")
                    ret = ExequteSQLquery(sqlu)
                    Continue For
                End If
                If ",.CSV,.XML,.JSON,.TXT,.XLS,.XLSX,.MDB,.ACCDB,".IndexOf(ext.ToUpper) < 0 Then
                    sqlu = "UPDATE ourscheduledImports SET [Status]='wrong format of url' WHERE ID=" & dts.Rows(i)("ID")
                    ret = ExequteSQLquery(sqlu)
                    WriteToAccessLog(logon, "Format of URL does not supported: " & dts.Rows(i)("URL").ToString & " It should end with .CSV, .XML, .JSON, .TXT, .XLS, .XLSX, .MDB, .ACCDB ", 2)
                    Continue For
                End If

                'If ext.ToUpper <> ".CSV" Then
                '    LabelAlert.Text = "Only .CSV files can be imported for now from URL. You can download your file, export in csv format if possible and use the DataImport page to try importing of local files."
                '    'TODO testing
                '    'Continue For
                'End If

                'WebClient - another way to read or download data
                Dim client = New WebClient

                Try
                    Dim onemany As String = "one"
                    If ext.ToUpper = ".XML" OrElse ext.ToUpper = ".JSON" OrElse ext.ToUpper = ".RDL" OrElse ext.ToUpper = ".TXT" Then
                        onemany = "many"
                    End If

                    'clear table if Prop5 = True (to clear table)  'dangerous
                    'only for testing:
                    'cleartable = "True"

                    If TableExists(tblname, Session("UserConnString"), Session("UserConnProvider")) AndAlso cleartable = "True" Then
                        Dim res As String = ClearTables(tblname, Session("UserConnString"), Session("UserConnProvider"), Session("Unit"), Session("logon"), Session("UserDB"), repid, ret, onemany)
                        If res.StartsWith("ERROR!!") OrElse res.StartsWith("Table copied, but not deleted: ") Then
                            WriteToAccessLog(logon, "Scheduled import from url  " & dts.Rows(i)("URL").ToString & " is not started into the table " & dts.Rows(i)("TableName").ToString & ", cleaning table crashed with error: " & res, 2)
                            sqlu = "UPDATE ourscheduledImports SET [Status]='import crashed' WHERE ID=" & dts.Rows(i)("ID")
                            ret = ExequteSQLquery(sqlu)
                            Continue For
                        End If
                    End If

                    exst = TableExists(tblname, Session("UserConnString"), Session("UserConnProvider"))

                    '============================================================Import starts =======================================================================

                    If ext.Trim.ToUpper.EndsWith(".CSV") Then  'OrElse FileUpl.PostedFile.FileName.ToUpper.EndsWith(".PRN")
                        'Import CSV into Table
                        'ret = ImportCSVintoDbTable(Session("logon"), tblname, strFile, d, Session("UserConnString"), Session("UserConnProvider"), exst)
                        'NEW Line by Line from URL  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        ret = ImportCSVintoDbTableByLine(Session("logon"), tblname, dts.Rows(i)("URL").ToString.Trim, d, Session("UserConnString"), Session("UserConnProvider"), exst)

                        If (ret <> "Query executed fine." OrElse ret.StartsWith("ERROR!!")) AndAlso exst AndAlso cleartable Then
                            'restore data in the table from DELETED
                            ret = ret & ", " & RestoreDataFromDELETED(tblname, Session("UserConnString"), Session("UserConnProvider"))
                        End If

                        '--------------------------------------------------------------------------------------------------------------
                    Else

                        'DOWNLOAD IN LOCAL SERVER FILE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        Dim strFile As String = String.Empty   'dts.Rows(i)("URL").ToString.Trim
                        Dim k As Integer
                        'Dim fileuploadDir As String = ConfigurationManager.AppSettings("fileupload").ToString & "Temp\"
                        Dim filename As String = logon & "_" & dts.Rows(i)("URL").Trim.Replace(" ", "").Replace("/", "").Replace("\", "").Replace("_", "").Replace("%", "").Replace("http", "").Replace(":", "").Replace(".", "")
                        If filename.Length > 70 Then
                            filename = filename.Substring(0, 70)
                        End If
                        filename = filename & dts.Rows(i)("Deadline").ToString.Replace("-", "").Replace(":", "").Replace(" ", "") & "_" & Now.ToShortDateString.Replace("/", "") & Now.ToShortTimeString.Replace(":", "").Replace(" ", "")
                        filename = filename & "." & ext
                        filename = filename.Replace("..", ".")
                        strFile = applpath & filename
                        'strFile = strFile.Replace("\\", "\")
                        'save file in the \Temp\filename
                        Try
                            'smallfile = True
                            client.DownloadFile(dts.Rows(i)("URL").Trim, strFile)  '"C:\Uploads\" & filename

                            If Not (ext.ToUpper.EndsWith(".MDB") OrElse ext.ToUpper.EndsWith(".ACCDB") OrElse ext.ToUpper.EndsWith(".XLS") OrElse ext.ToUpper.EndsWith(".XLSX")) Then
                                'clean and save filetext in fpath
                                Dim filetext As String = File.ReadAllText(strFile)
                                filetext = filetext.Replace("http://", "").Replace("https://", "").Replace("file:///", "")
                                filetext = filetext.Replace("schemas.microsoft.com/", "http://schemas.microsoft.com/")
                                'if we want to keep file on our server
                                filetext = cleanTextOfFile(filetext)
                                File.Delete(strFile)
                                File.WriteAllText(strFile, filetext)
                            End If
                            'update OURScheduledImports
                            sqlu = "UPDATE ourscheduledImports SET [Status]='downloaded on server',Prop2='" & filename & "',Prop7='" & strFile & "' WHERE ID=" & dts.Rows(i)("ID")
                            ret = ExequteSQLquery(sqlu)

                            'Do Import as usual from local file strFile (as in DataImport)
                            ''TODO testing:
                            'Return "test"

                        Catch ex As Exception
                            'smallfile = False
                            WriteToAccessLog(logon, "Download from url  " & dts.Rows(i)("URL") & " crashed with error: " & ex.Message, 2)
                            sqlu = "UPDATE ourscheduledImports SET [Status]='not dowloaded' WHERE ID=" & dts.Rows(i)("ID")
                            ret = ExequteSQLquery(sqlu)
                            'Do download by Line from URL or cut by chunks and import local files from directory
                            'TODO download in local file or folder with chunks

                            'WebClient - another way to download data
                            'Dim urldnloads As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                            'Dim client = New WebClient
                            'client.DownloadFile(dts.Rows(i)("URL").Trim, strFile)  '"C:\Uploads\" & filename
                            ''DO NOT DELETE! We can use it for big files or else cut them to 4 mg pieces 
                            Dim Stream As Stream = client.OpenRead(dts.Rows(i)("URL").Trim)
                            Dim SR As StreamReader = New StreamReader(Stream)
                            Dim SW As StreamWriter = New StreamWriter(strFile)
                            Dim strsr As String = String.Empty
                            'SR.ReadToEnd()  'ReadLine
                            While Not SR.EndOfStream
                                strsr = cleanText(SR.ReadLine)
                                k = k + 1
                                SW.WriteLine(strFile, strsr)
                            End While
                            SR.Close()
                            SW.Close()
                            Stream.Close()
                        End Try

                        'Do Import as usual from local sever file strFile (as in DataImport): ---------------------------------------------------------------------------

                        If ext.ToUpper.EndsWith(".XLS") OrElse ext.ToUpper.EndsWith(".XLSX") OrElse ext.ToUpper.EndsWith(".ACCDB") OrElse ext.ToUpper.EndsWith(".MDB") Then   'OrElse txtURI.Text.Trim.ToUpper.EndsWith(".PRN")
                            'import Excel file into Table

                            ret = ImportExcelIntoDbTable(tblname, strFile, Session("UserConnString"), Session("UserConnProvider"), exst)
                            If (ret <> "Query executed fine." OrElse ret.StartsWith("ERROR!!")) Then
                                ret = ret & " Attention!! Save your file or Access table as Microsoft Excel 97-2003 Worksheet, or export them in CSV file and try to import it again."
                                WriteToAccessLog(logon, "Scheduled import from url  " & dts.Rows(i)("URL").ToString & " and downloaded file " & strFile & " was not completed into the table " & dts.Rows(i)("TableName").ToString & ", error: " & ret, 2)

                                If exst AndAlso cleartable Then
                                    'restore data in the table from DELETED
                                    ret = ret & ", " & RestoreDataFromDELETED(tblname, Session("UserConnString"), Session("UserConnProvider"))
                                End If
                            Else
                                ret = "Query executed fine."
                            End If

                            '--------------------------------------------------------------------------------------------------------------

                        ElseIf ext.ToUpper.EndsWith(".ACCDB") OrElse ext.ToUpper.EndsWith(".MDB") Then
                            'import Access table into Table
                            'TODO download in local file
                            ret = ImportExcelIntoDbTable(tblname, strFile, Session("UserConnString"), Session("UserConnProvider"), exst)
                            If (ret <> "Query executed fine." OrElse ret.StartsWith("ERROR!!")) Then
                                ret = ret & " Attention!! You can export Access table into the Microsoft Excel 97-2003 Worksheet and try to import it again."
                                WriteToAccessLog(logon, "Scheduled import from url  " & dts.Rows(i)("URL").ToString & " and downloaded file " & strFile & "  was not completed into the table " & dts.Rows(i)("TableName").ToString & ", error: " & ret, 2)

                                If exst AndAlso cleartable Then
                                    'restore data in the table from DELETED
                                    ret = ret & ", " & RestoreDataFromDELETED(tblname, Session("UserConnString"), Session("UserConnProvider"))
                                End If
                            Else
                                ret = "Query executed fine."
                            End If

                            '--------------------------------------------------------------------------------------------------------------

                        ElseIf ext.ToUpper.EndsWith(".XML") OrElse ext.ToUpper.EndsWith(".RDL") Then
                            'Import XML into Table
                            'TODO download in local file or folder with chunks
                            ret = ImportXMLorJSONintoDatabase(Session("REPORTID"), tblname, strFile, Session("UserConnString"), Session("UserConnProvider"), Session("Unit"), Session("logon"), Session("UserDB"), True, False)
                            If ret.StartsWith("ERROR!!") Then
                                WriteToAccessLog(logon, "Scheduled import from url  " & dts.Rows(i)("URL").ToString & " and downloaded file " & strFile & "  was not completed into the table " & dts.Rows(i)("TableName").ToString & ", error: " & ret, 2)

                                'delete strFile
                                Try
                                    File.Delete(strFile)
                                Catch ex As Exception
                                    ret = ret & "ERROR!! " & ex.Message
                                    LabelAlert.Text = ret
                                    LabelAlert.Visible = True
                                End Try
                            Else
                                ret = "Query executed fine."
                            End If

                            '--------------------------------------------------------------------------------------------------------------
                        ElseIf ext.ToUpper.EndsWith(".TXT") OrElse ext.ToUpper.EndsWith(".JSON") Then
                            'Import JSON into tables
                            'TODO download in local file or folder with chunks
                            ret = ImportXMLorJSONintoDatabase(Session("REPORTID"), tblname, strFile, Session("UserConnString"), Session("UserConnProvider"), Session("Unit"), Session("logon"), Session("UserDB"), False, False)
                            WriteToAccessLog(logon, "Scheduled import from url  " & dts.Rows(i)("URL").ToString & " and downloaded file " & strFile & "  was not completed into the table " & dts.Rows(i)("TableName").ToString & ", error: " & ret, 2)

                            If ret.StartsWith("ERROR!!") Then
                                'delete strFile
                                Try
                                    File.Delete(strFile)
                                Catch ex As Exception
                                    ret = ret & "ERROR!! " & ex.Message
                                    LabelAlert.Text = ret
                                    LabelAlert.Visible = True
                                End Try
                            Else
                                ret = "Query executed fine."
                            End If

                        End If
                    End If
                    '============================================================Import ends =======================================================================
                    If ret = "Query executed fine." Then
                        'insert into list of user tables and create report if new one
                        If Not exst Then
                            'insert into list of user tables and create report if new one
                            er = ""
                            rep = Session("dbname") & "_INIT_" & i.ToString & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
                            ret = MakeNewStanardReport(Session("logon"), rep, tblname, Session("UserDB"), "SELECT * FROM " & tblname, Session("dbname"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)
                            ret = InsertTableIntoOURUserTables(tblname, tblname, Session("Unit"), Session("logon"), Session("UserDB"), "", rep)
                            rep = Session("logon") & "_" & i.ToString & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
                            ret = MakeNewUserReport(Session("logon"), rep, tblname, Session("UserDB"), "SELECT * FROM " & tblname, Session("dbname"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)

                        End If
                        sqlu = "UPDATE ourscheduledImports SET [Status]='imported' WHERE ID=" & dts.Rows(i)("ID")
                        ret = ExequteSQLquery(sqlu)
                        WriteToAccessLog(logon, "Scheduled import from url  " & dts.Rows(i)("URL").ToString & " completed into the table " & dts.Rows(i)("TableName").ToString, 2)

                        'send email 
                        Dim webour As String = ConfigurationManager.AppSettings("weboureports").ToString.Trim
                        Dim cntus As String = webour & "ContactUs.aspx"
                        Dim emailbody As String = String.Empty
                        emailbody = "Scheduled import from url  " & dts.Rows(i)("URL").ToString & " has been completed. See data in your database table " & dts.Rows(i)("TableName").ToString & ". | | Do not answer to this email. | Feel free to contact us at " & cntus & " if you have any questions. | OUReports"
                        emailbody = emailbody.Replace("|", Chr(10))
                        emails = dts.Rows(i)("ToWhom").ToString.Trim.Split(",")
                        For j = 0 To emails.Length - 1
                            If emails(j).Trim <> "" Then
                                ret = SendHTMLEmail("", "Scheduled import from url  " & dts.Rows(i)("URL") & " into table " & dts.Rows(i)("TableName").ToString & " has been completed", emailbody, emails(j).Trim, email)
                                If ret.StartsWith("ERROR!! ") Then
                                    'update OURScheduledDownloads
                                    sqlu = "UPDATE ourscheduledImports SET [Status]='imported to database',Prop8='email crashed' WHERE ID=" & dts.Rows(i)("ID")
                                    ret = ExequteSQLquery(sqlu)
                                    WriteToAccessLog(logon, "Email to " & emails(j) & " crashed with error: " & ret, 2)
                                Else
                                    'update OURScheduledDownloads
                                    sqlu = "UPDATE ourscheduledImports SET [Status]='imported to database',Prop8='email sent' WHERE ID=" & dts.Rows(i)("ID")
                                    ret = ExequteSQLquery(sqlu)
                                    n = n + 1
                                End If
                            End If
                        Next
                        Return ""

                    Else
                        'ret is bad
                        WriteToAccessLog(logon, "Scheduled import from url  " & dts.Rows(i)("URL").ToString & " was not completed into the table " & dts.Rows(i)("TableName").ToString & ", error: " & ret, 2)
                        sqlu = "UPDATE ourscheduledImports SET [Status]='Import crashed.' WHERE ID=" & dts.Rows(i)("ID")
                        ret = ExequteSQLquery(sqlu)
                        'send email 
                        WriteToAccessLog(logon, "Scheduled import from url  " & dts.Rows(i)("URL") & " crashed with error: " & ret, 2)
                        sqlu = "UPDATE ourscheduledImports SET [Status]='import crashed' WHERE ID=" & dts.Rows(i)("ID")
                        ret = ExequteSQLquery(sqlu)
                        Dim emailbody As String = String.Empty
                        Dim webour As String = ConfigurationManager.AppSettings("weboureports").ToString.Trim
                        Dim cntus As String = webour & "ContactUs.aspx"
                        emailbody = "Scheduled import from url  " & dts.Rows(i)("URL") & " in your database table " & dts.Rows(i)("TableName").ToString & " crashed with error: " & ret & " | | Do not answer to this email. | Feel free to contact us at " & cntus & " if you have any questions. | OUReports"
                        emailbody = emailbody.Replace("|", Chr(10))
                        emails = dts.Rows(i)("ToWhom").ToString.Trim.Split(",")
                        For j = 0 To emails.Length - 1
                            If emails(j).Trim <> "" Then
                                ret = SendHTMLEmail("", "ERROR!! Scheduled import from url  " & dts.Rows(i)("URL") & " crashed", emailbody, emails(j).Trim, email)
                                If ret.StartsWith("ERROR!! ") Then
                                    'update OURScheduledDownloads
                                    sqlu = "UPDATE ourscheduledimports SET [Status]='import and email crashed' WHERE ID=" & dts.Rows(i)("ID")
                                    ret = ExequteSQLquery(sqlu)
                                    WriteToAccessLog(logon, "Email to " & emails(j) & " crashed with error: " & ret, 2)
                                Else
                                    'update OURScheduledDownloads
                                    sqlu = "UPDATE ourscheduledimports SET [Status]='import crashed, email sent' WHERE ID=" & dts.Rows(i)("ID")
                                    ret = ExequteSQLquery(sqlu)

                                End If
                            End If
                        Next
                    End If
                Catch ex As Exception

                    sqlu = "UPDATE ourscheduledImports SET [Status]='Import crashed.' WHERE ID=" & dts.Rows(i)("ID")
                    ret = ExequteSQLquery(sqlu)
                    WriteToAccessLog(logon, "Scheduled import from url  " & dts.Rows(i)("URL").ToString & " was not completed into the table " & dts.Rows(i)("TableName").ToString, 2)

                    'send email 
                    WriteToAccessLog(logon, "Scheduled import from url  " & dts.Rows(i)("URL") & " crashed with error: " & ex.Message, 2)
                    sqlu = "UPDATE ourscheduledImports SET [Status]='import crashed' WHERE ID=" & dts.Rows(i)("ID")
                    ret = ExequteSQLquery(sqlu)

                    Dim webour As String = ConfigurationManager.AppSettings("weboureports").ToString.Trim
                    Dim cntus As String = webour & "ContactUs.aspx"
                    Dim emailbody As String = String.Empty
                    emailbody = "Scheduled import from url  " & dts.Rows(i)("URL") & " in your database table " & dts.Rows(i)("TableName").ToString & " crashed with error: " & ex.Message & " | | Do not answer to this email. | Feel free to contact us at " & cntus & " if you have any questions. | OUReports"
                    emailbody = emailbody.Replace("|", Chr(10))
                    emails = dts.Rows(i)("ToWhom").ToString.Trim.Split(",")
                    For j = 0 To emails.Length - 1
                        If emails(j).Trim <> "" Then
                            ret = SendHTMLEmail("", "ERROR!! Scheduled import from url  " & dts.Rows(i)("URL") & " crashed", emailbody, emails(j).Trim, email)
                            If ret.StartsWith("ERROR!! ") Then
                                'update OURScheduledImports
                                sqlu = "UPDATE ourscheduledimports SET [Status]='import and email crashed' WHERE ID=" & dts.Rows(i)("ID")
                                ret = ExequteSQLquery(sqlu)
                                WriteToAccessLog(logon, "Email to " & emails(j) & " crashed with error: " & ret, 2)
                            Else
                                'update OURScheduledImports
                                sqlu = "UPDATE ourscheduledimports SET [Status]='import crashed, email sent' WHERE ID=" & dts.Rows(i)("ID")
                                ret = ExequteSQLquery(sqlu)

                            End If
                        End If
                    Next
                End Try
            Next

            Return ""
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
End Class
