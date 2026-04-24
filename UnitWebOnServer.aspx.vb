Imports System.IO
Imports System.Data
Partial Class UnitWebOnServer
    Inherits System.Web.UI.Page

    Private Sub UnitWebOnServer_Init(sender As Object, e As EventArgs) Handles Me.Init
        'If Request("UnitWebIndex") Is Nothing OrElse Request("UnitWebKey") Is Nothing Then
        '    Label1.Text = "Unit was not defined. Unit Web has not been created."
        '    Response.Redirect("UnitWebOnServer.aspx?lbl=" & Label1.Text)
        'End If
        If Not Request("lbl") Is Nothing AndAlso Request("lbl").ToString <> "" Then
            Label1.Text = Request("lbl").ToString
            'Response.End()
        End If
    End Sub

    Private Sub UnitWebOnServer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim ret As String = String.Empty
        Dim unitindx As String = String.Empty
        Dim unitname As String = String.Empty
        Dim unitweb As String = String.Empty
        Dim dbstr As String = String.Empty
        Dim tm As String = String.Empty
        Dim unitourdbname As String = String.Empty
        Dim unitcsvdbname As String = String.Empty
        Try
            If Not Request("UnitWebIndex") Is Nothing AndAlso Request("UnitWebIndex").ToString <> "" Then
                Dim nm As String = ConfigurationManager.AppSettings("unit").ToString
                unitindx = Request("UnitWebIndex").ToString.Trim
                unitourdbname = Request("UnitOurdb").ToString.Trim
                unitcsvdbname = Request("UnitCsvdb").ToString.Trim
                unitweb = Request("UnitWeb").ToString.Trim
                unitname = Request("UnitName").ToString.Trim.Replace(" ", "")
                'dbstr = ConfigurationManager.AppSettings("unitOURdbConnStr").ToString.Replace("OURdataUnit", "OURdataUnit" & unitname & unitindx & nm)
                dbstr = ConfigurationManager.AppSettings("unitOURdbConnStr").ToString.Replace("OURdataUnit", unitourdbname)
                WriteToAccessLog("Unit registration", "dbstr: " & dbstr, 1)

                If IsNumeric(unitindx) Then
                    'On Server the actual folder
                    Dim sourceFolder As String = Request.PhysicalApplicationPath
                    Dim n As Integer = sourceFolder.LastIndexOf("\")
                    If n = sourceFolder.Length - 1 Then
                        sourceFolder = sourceFolder.Substring(0, n)
                        n = sourceFolder.LastIndexOf("\")
                    End If
                    sourceFolder = sourceFolder.Substring(0, n) & "\Unit0" & nm
                    'unitweb="Unit" & nm & unitindx & unitname
                    Dim outputFolder As String = sourceFolder.Replace("Unit0" & nm, unitweb)
                    If Directory.Exists(outputFolder) Then
                        'Label1.Text = "Site Unit" & unitindx & nm & " already exists. Please choose another unit abbreviation."
                        'Exit Sub
                        ' Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
                        tm = DateToString(Now).Replace("-", "").Replace(":", "").Replace(" ", "")
                        outputFolder = outputFolder & tm
                    End If
                    'unitweb = ConfigurationManager.AppSettings("unitOUReportsWeb").ToString.Replace("OURUnitWeb", "Unit" & nm & unitindx & unitname & tm)
                    Dim unitweburl As String = ConfigurationManager.AppSettings("unitOUReportsWeb").ToString.Replace("OURUnitWeb", unitweb & tm)
                    WriteToAccessLog("Unit registration", "unitweb: " & unitweb, 1)

                    'update unitweb in original ourdb
                    Dim sqlt As String = "UPDATE OURUnits SET UnitWeb='" & unitweburl & "'  WHERE Indx='" & unitindx & "'"
                    ret = ExequteSQLquery(sqlt)

                    ret = DirectoryCopy(sourceFolder, outputFolder, True)

                    If ret = "" Then 'created
                        'update web.config in outputFolder:  web.config should have the connection string to OURdataUnit" & unitindx & " OUR database
                        Dim webconfigpath As String = outputFolder & "\web.config"
                        Dim sr() As String = File.ReadAllLines(webconfigpath)
                        Dim i As Integer
                        For i = 0 To sr.Length - 1

                            sr(i) = sr(i).Replace("OURdataUnit;", unitourdbname & ";")
                            sr(i) = sr(i).Replace("CSVdataUnit;", unitcsvdbname & ";")

                            'make user connection string in web.config from the very first record in the ourunit table from dbstr database
                            If sr(i).Contains("UserSqlConnection") Then
                                Dim dv As DataView = mRecords("SELECT * FROM ourunits", ret, dbstr, System.Configuration.ConfigurationManager.ConnectionStrings.Item("MySqlConnection").ProviderName.ToString)
                                If Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
                                    Dim userdbstr As String = dv.Table.Rows(0)("UserConnStr")
                                    Dim userdbprv As String = dv.Table.Rows(0)("UserConnPrv")
                                    WriteToAccessLog("Unit registration", "Userdbstr:" & userdbstr, 1)
                                    sr(i) = "      <add name=""UserSqlConnection"" connectionString=""" & userdbstr & """ providerName=""" & userdbprv & """ />"
                                    WriteToAccessLog("Unit registration", "sr(" & i.ToString & "):" & sr(i), 2)

                                    'update unitweb in unit db
                                    Dim unitdbstr As String = dv.Table.Rows(0)("OURConnStr")
                                    Dim unitdbprv As String = dv.Table.Rows(0)("OURConnPrv")
                                    sqlt = "UPDATE OURUnits SET UnitWeb='" & unitweburl & "'  WHERE Unit='" & unitname & "'"
                                    ret = ExequteSQLquery(sqlt, unitdbstr, unitdbprv)
                                Else
                                    WriteToAccessLog("Unit registration", "dbstr: " & dbstr & " ret: " & ret, 3)
                                End If
                            End If

                            If sr(i).Contains("<add key=""unit"" value=") Then
                                sr(i) = "   <add key=""unit"" value=""" & unitname & """/>"
                            End If
                            sr(i) = sr(i).Replace("/OURUnitWeb/", "/" & unitweb & tm & "/")

                            If sr(i).Contains("<add key=""SiteFor"" value=""") Then
                                sr(i) = "   <add key=""SiteFor"" value=""OUReports for " & unitname & """/>"
                            End If
                            If sr(i).Contains("<add key=""pagettl"" value=""") Then
                                sr(i) = "   <add key=""pagettl"" value=""" & "Online User Reporting and Analytics for " & unitname & """/>"
                            End If

                        Next
                        'write in file
                        File.WriteAllLines(webconfigpath, sr)

                        Label1.Text = "Unit Web will be available at " & unitweb & " in 24 hours. Please contact us if it does not happen."

                        Dim EmailTable As DataTable
                        Dim j As Integer
                        EmailTable = mRecords("SELECT  * FROM OURPERMITS WHERE APPLICATION='InteractiveReporting' AND RoleApp='super'").Table
                        If Not ret.Contains("ERROR!!") Then
                            'send emails 
                            Dim emailbody As String = "Unit web site " & unitweb & tm & " should be created ASAP! Folder " & outputFolder & " should already be created. "
                            emailbody = emailbody & "If Not - copy the folder " & sourceFolder & " from wwwroot to wwwroot, rename it to " & outputFolder & " and correct web.config with proper values of unitourdbname, unitcsvdbname, unitweburl, etc.. "
                            emailbody = emailbody & "In IIS right click on Default Web Site And right click on " & outputFolder & ". "
                            emailbody = emailbody & "From right click menu select ""Convert to application"" Or click Add Application. The form should be as this: "
                            'emailbody = emailbody & "Alias: Unit" & unitname & unitindx & nm & tm & ", Physical path: browse and find the folder " & outputFolder & ". Click OK. "
                            emailbody = emailbody & "Alias: " & unitweb & ", Physical path: browse and find the folder " & outputFolder & ". Click OK. "
                            emailbody = emailbody & "After that check that web.config had been updated: web.config should have the unit's OURdata database as " & unitourdbname & " the unit's CSVdata database as " & unitcsvdbname

                            If EmailTable.Rows.Count > 0 Then
                                For j = 0 To EmailTable.Rows.Count - 1
                                    ret = SendHTMLEmail("", "Unit #" & unitindx & " has been registered", emailbody, EmailTable.Rows(j)("Email"), Session("SupportEmail"))
                                Next
                            End If
                            ret = "Your OUReports web site will be available in 24-48 hours."
                        Else
                            ''create urgent ticket
                            ''send emails
                            'If EmailTable.Rows.Count > 0 Then
                            '    For j = 0 To EmailTable.Rows.Count - 1
                            '        ret = SendHTMLEmail("", "Update OURdb To CurrentVersion return: " & ret & ". Unit #" & unitindx & " has been registered", "Unit web site " & txtUnitWeb.Text & " should be created ASAP!  Unit" & unitindx & "OUR should be created in the wwwroot. If not - copy Unit0OUR folder from wwwroot To wwwroot, rename it to Unit" & unitindx & "OUR. In IIS right click on Default Web Site And click Add Application. Fill out the form as this: Alias: Unit" & unitindx & "OUR, Physical path: browse and find the wwwroot/Unit" & unitindx & "OUR folder. Click OK. After that check that web.config had been updated: web.config should have the connection string to OURdataUnit" & unitindx & " OUR database as " & txtOURdb.Text, EmailTable.Rows(j)("Email"), Session("SupportEmail"))
                            '    Next
                            'End If
                            'ret = "Your OUReports web site will be available in 24-48 hours."

                        End If
                    Else
                        Label1.Text = ret


                    End If
                Else
                    Label1.Text = "Index is not numeric. Unit Web has not been created."
                    'Response.Redirect("UnitWebOnServer.aspx?lbl=" & Label1.Text)
                End If
            Else
                Label1.Text = "Unit Web has been created. Unit Web will be available at " & unitweb & " in 24 hours. Please contact us if it does not happen."
                'Response.End()
            End If
        Catch ex As Exception
            Label1.Text = Label1.Text & ex.Message
            'Response.End()
        End Try
    End Sub
End Class
