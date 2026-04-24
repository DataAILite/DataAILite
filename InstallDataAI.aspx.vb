
Imports System.IO
Imports System.IO.Compression
Imports Microsoft.Web.Administration
Imports System.Net
Imports System.Drawing
Partial Class InstallDataAI
    Inherits System.Web.UI.Page
    Private WithEvents client As New WebClient()
    Private Sub ButtonInstall_Click(sender As Object, e As EventArgs) Handles ButtonInstall.Click
        Dim ret As String = String.Empty
        'check entered values in textboxes
        If cleanTextShort(txtOperationalDatabaseServer.Text) <> txtOperationalDatabaseServer.Text Then
            txtOperationalDatabaseServer.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtOperationalDatabaseServer.BackColor = Color.White
        End If
        If cleanTextShort(txtOperationalDatabaseName.Text) <> txtOperationalDatabaseName.Text Then
            txtOperationalDatabaseName.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtOperationalDatabaseName.BackColor = Color.White
        End If
        If cleanTextShort(txtoperationaldatabaseuserID.Text) <> txtoperationaldatabaseuserID.Text Then
            txtoperationaldatabaseuserID.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtoperationaldatabaseuserID.BackColor = Color.White
        End If
        If cleanTextShort(txtoperationaldatabasepassword.Text) <> txtoperationaldatabasepassword.Text Then
            txtoperationaldatabasepassword.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtoperationaldatabasepassword.BackColor = Color.White
        End If
        If cleanTextShort(txtoperationaldatabaseprovider.Text) <> txtoperationaldatabaseprovider.Text Then
            txtoperationaldatabaseprovider.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtoperationaldatabaseprovider.BackColor = Color.White
        End If
        If cleanTextShort(txtoperationaldatabaseport.Text) <> txtoperationaldatabaseport.Text Then
            txtoperationaldatabaseport.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtoperationaldatabaseport.BackColor = Color.White
        End If

        If cleanTextShort(txtyourdataserver.Text) <> txtyourdataserver.Text Then
            txtyourdataserver.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtyourdataserver.BackColor = Color.White
        End If
        If cleanTextShort(txtyourDatabaseName.Text) <> txtyourDatabaseName.Text Then
            txtyourDatabaseName.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtyourDatabaseName.BackColor = Color.White
        End If
        If cleanTextShort(txtyourdatabaseuserID.Text) <> txtyourdatabaseuserID.Text Then
            txtyourdatabaseuserID.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtyourdatabaseuserID.BackColor = Color.White
        End If
        If cleanTextShort(txtyourdatabasepassword.Text) <> txtyourdatabasepassword.Text Then
            txtyourdatabasepassword.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtyourdatabasepassword.BackColor = Color.White
        End If
        If cleanTextShort(txtyourdatabaseprovider.Text) <> txtyourdatabaseprovider.Text Then
            txtyourdatabaseprovider.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtyourdatabaseprovider.BackColor = Color.White
        End If
        If cleanTextShort(txtyourdatabaseport.Text) <> txtyourdatabaseport.Text Then
            txtyourdatabaseport.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtyourdatabaseport.BackColor = Color.White
        End If

        If cleanTextShort(txtyoursmtpemail.Text) <> txtyoursmtpemail.Text Then
            txtyoursmtpemail.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtyoursmtpemail.BackColor = Color.White
        End If
        If cleanTextShort(txtyoursmtpemailpassword.Text) <> txtyoursmtpemailpassword.Text Then
            txtyoursmtpemailpassword.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtyoursmtpemailpassword.BackColor = Color.White
        End If

        If cleanTextShort(txtyoururlDataAI.Text) <> txtyoururlDataAI.Text Then
            txtyoururlDataAI.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtyoururlDataAI.BackColor = Color.White
        End If
        If cleanTextShort(txtyourwebsitetitle.Text) <> txtyourwebsitetitle.Text Then
            txtyourwebsitetitle.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtyourwebsitetitle.BackColor = Color.White
        End If
        If cleanTextShort(txtyoursupportemail.Text) <> txtyoursupportemail.Text Then
            txtyoursupportemail.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtyoursupportemail.BackColor = Color.White
        End If
        If cleanTextShort(txtyourfoldertouploadfiles.Text) <> txtyourfoldertouploadfiles.Text Then
            txtyourfoldertouploadfiles.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtyourfoldertouploadfiles.BackColor = Color.White
        End If
        If cleanTextShort(txtyourgooglemapkey.Text) <> txtyourgooglemapkey.Text Then
            txtyourgooglemapkey.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtyourgooglemapkey.BackColor = Color.White
        End If

        If cleanTextShort(txtyourOpenAIkey.Text) <> txtyourOpenAIkey.Text Then
            txtyourOpenAIkey.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtyourOpenAIkey.BackColor = Color.White
        End If
        If cleanTextShort(txtyourOpenAImodel.Text) <> txtyourOpenAImodel.Text Then
            txtyourOpenAImodel.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtyourOpenAImodel.BackColor = Color.White
        End If
        If cleanTextShort(txtyourmaximumtokens.Text) <> txtyourmaximumtokens.Text Then
            txtyourmaximumtokens.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtyourmaximumtokens.BackColor = Color.White
        End If
        If cleanTextShort(txtyourOpenAIorganizationcode.Text) <> txtyourOpenAIorganizationcode.Text Then
            txtyourOpenAIorganizationcode.BackColor = Color.Pink
            MsgBox("Wrong value in textbox")
            Exit Sub
        Else
            txtyourOpenAIorganizationcode.BackColor = Color.White
        End If


        'download dataAI.zip
        Dim downloadsPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads")
        Try
            'smallfile = True
            client.DownloadFile("https://oureports.net/OUReports/SAVEDFILES/DataAI.zip", downloadsPath)
            ''big file
            'client.DownloadFileAsync(New Uri("https://oureports.net/OUReports/SAVEDFILES/DataAI.zip"), downloadsPath)

        Catch ex As Exception
            ret = "Error: " & ex.Message
            MsgBox(ret)
            Exit Sub
        End Try


        'unzip
        downloadsPath = downloadsPath & "\DataAI.zip"
        Dim extractPath As String = "C:\inetpub\wwwroot\DataAItest" ' "C:\inetpub\wwwroot\DataAI"
        Try
            ZipFile.ExtractToDirectory(downloadsPath, extractPath)
        Catch ex As Exception
            ret = "Error: " & ex.Message
            MsgBox(ret)
            Exit Sub
        End Try

        'update web.config
        Dim webconfigpath As String = "C:\inetpub\wwwroot\DataAItest\web.config"
        Dim wcfg As String = File.ReadAllText(webconfigpath)
        wcfg.Replace("[operational database server]", txtOperationalDatabaseServer.Text)
        wcfg.Replace("[operational database/namespace name]", txtOperationalDatabaseName.Text)
        wcfg.Replace("[operational database user ID]", txtoperationaldatabaseuserID.Text)
        wcfg.Replace("[operational database password]", txtoperationaldatabasepassword.Text)
        wcfg.Replace("[operational database provider name]", txtoperationaldatabaseprovider.Text)
        wcfg.Replace("[operational database port if needed]", txtoperationaldatabaseport.Text)
        wcfg.Replace("[system database password if needed]", txtsystemdatabasepassword.Text)

        wcfg.Replace("[your data server]", txtyourdataserver.Text)
        wcfg.Replace("[your database/namespace name]", txtyourDatabaseName.Text)
        wcfg.Replace("[your database user ID]", txtyourdatabaseuserID.Text)
        wcfg.Replace("[your database password]", txtyourdatabasepassword.Text)
        wcfg.Replace("[your database provider name]", txtyourdatabaseprovider.Text)
        wcfg.Replace("[your database port if needed]", txtyourdatabaseport.Text)

        wcfg.Replace("[your smtp email]", txtyoursmtpemail.Text)
        wcfg.Replace("[password for your smtp email]", txtyoursmtpemailpassword.Text)

        wcfg.Replace("[your web url for DataAI]", txtyoururlDataAI.Text)
        wcfg.Replace("[your web site title]", txtyourwebsitetitle.Text)
        wcfg.Replace("[your support email]", txtyoursupportemail.Text)
        wcfg.Replace("[your folder to upload files]", txtyourfoldertouploadfiles.Text)
        wcfg.Replace("[your google map key]", txtyourgooglemapkey.Text)

        wcfg.Replace("[your OpenAI key]", txtyourOpenAIkey.Text)
        wcfg.Replace("[your OpenAI model]", txtyourOpenAImodel.Text)
        wcfg.Replace("[OpenAI maximum tokens limit]", txtyourmaximumtokens.Text)
        wcfg.Replace("[your OpenAI organization code]", txtyourOpenAIorganizationcode.Text)

        File.WriteAllText(webconfigpath, wcfg)






        'make extractPath as IIS application
        Try
            Dim serverManager As New ServerManager()
            Dim site = serverManager.Sites("Default Web Site")

            Dim appPath As String = "/DataAItest"  'DataAI
            Dim physicalPath As String = "C:\inetpub\wwwroot\DataAItest"

            Dim app = site.Applications.Add(appPath, physicalPath) 'DataAI
            app.ApplicationPoolName = "DefaultAppPool"

            serverManager.CommitChanges()

        Catch ex As Exception
            ret = "Error: " & ex.Message
            MsgBox(ret)
            Exit Sub
        End Try

        If ret.Trim = "" Then
            Response.Redirect("Default.aspx")
        End If
    End Sub
    Private Sub client_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles client.DownloadProgressChanged
        'progressBar.Value = e.ProgressPercentage
    End Sub

    Private Sub client_DownloadFileCompleted(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs) Handles client.DownloadFileCompleted
        MessageBox.Show("Download complete!")
    End Sub
End Class
