Imports System.Data

Partial Class ProductionCustomer
    Inherits System.Web.UI.Page

    Private Sub ProductionCustomer_Init(sender As Object, e As EventArgs) Handles Me.Init
        btnSubmit.Enabled = False
        btnSubmit.Visible = False
    End Sub

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LblMessage.Text = ""
        End If
    End Sub

    Protected Sub chkme_CheckedChanged(sender As Object, e As EventArgs) Handles chkme.CheckedChanged
        btnSubmit.Enabled = chkme.Checked
        btnSubmit.Visible = chkme.Checked
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Dim ret As String = String.Empty
        Try
            ' ── Validate required fields ──────────────────────────────────────
            Dim distrMode As String = ddProduct.SelectedValue.Trim
            If distrMode = "" Then
                ret = "Please select a marketplace product."
                Exit Try
            End If

            Dim unit As String = cleanText(txtUnit.Text.Trim)
            If unit = "" OrElse unit <> txtUnit.Text.Trim Then
                ret = "Company name is required and must not contain special characters."
                Exit Try
            End If

            Dim official As String = cleanText(txtOfficial.Text.Trim)
            If official = "" OrElse official <> txtOfficial.Text.Trim Then
                ret = "Contact name &amp; title is required and must not contain special characters."
                Exit Try
            End If

            Dim email As String = cleanText(txtEmail.Text.Trim)
            If email = "" OrElse email <> txtEmail.Text.Trim Then
                ret = "A valid email address is required."
                Exit Try
            End If

            ' ── Validate required fields (phone, address) ────────────────────
            Dim phone As String = cleanText(txtPhone.Text.Trim)
            If phone = "" OrElse phone <> txtPhone.Text.Trim Then
                ret = "Phone is required and must not contain special characters."
                Exit Try
            End If

            Dim address As String = cleanText(txtAddress.Text.Trim)
            If address = "" OrElse address <> txtAddress.Text.Trim Then
                ret = "Address is required and must not contain special characters."
                Exit Try
            End If

            Dim unitweb As String = cleanText(txtUnitWeb.Text.Trim)
            If unitweb <> txtUnitWeb.Text.Trim Then
                ret = "Company website contains illegal characters. Please retype."
                Exit Try
            End If

            Dim comments As String = cleanText(txtComments.Text.Trim, True)
            If comments <> txtComments.Text.Trim Then
                ret = "Notes field contains illegal characters. Please retype."
                Exit Try
            End If

            ' ── Duplicate check ───────────────────────────────────────────────
            Dim dupSql As String = "SELECT * FROM OURUnits WHERE Unit='" & unit & "' AND Prop1='DataAI ETL'"
            If HasRecords(dupSql) Then
                ret = "This company is already registered for DataAI ETL production. Please contact us if you need assistance."
                Exit Try
            End If

            ' ── Build and run INSERT ──────────────────────────────────────────
            Dim startDate As String = DateToString(CDate(Now()))
            Dim fullComments As String = "DataAI ETL production registration on " & startDate
            If comments <> "" Then fullComments = fullComments & ". Notes: " & comments

            Dim sFields As String = "Unit,DistrMode,UnitWeb,Comments,StartDate,OURConnStr,OURConnPrv,UserConnStr,UserConnPrv,Official,Address,Phone,Email,Prop1"
            Dim sValues As String = String.Empty
            sValues = "'" & unit & "',"
            sValues &= "'" & distrMode & "',"
            sValues &= "'" & unitweb & "',"
            sValues &= "'" & fullComments & "',"
            sValues &= "'" & startDate & "',"
            sValues &= "'',"
            sValues &= "'',"
            sValues &= "'',"
            sValues &= "'',"
            sValues &= "'" & official & "',"
            sValues &= "'" & address & "',"
            sValues &= "'" & phone & "',"
            sValues &= "'" & email & "',"
            sValues &= "'DataAI ETL'"

            Dim sqlt As String = "INSERT INTO OURUnits (" & sFields & ") VALUES (" & sValues & ")"
            ret = ExequteSQLquery(sqlt)

            If ret = "Query executed fine." Then
                ' ── Notify super users ───────────────────────────────────────
                Try
                    Dim emailTable As DataTable = mRecords("SELECT * FROM OURPERMITS WHERE APPLICATION='InteractiveReporting' AND RoleApp='super'").Table
                    If emailTable IsNot Nothing AndAlso emailTable.Rows.Count > 0 Then
                        Dim subject As String = "DataAI ETL Production Registration: " & unit
                        Dim body As String = "New DataAI ETL production registration received." & vbCrLf
                        body &= "-------------------------------------------" & vbCrLf
                        body &= "Product:         " & ddProduct.SelectedItem.Text & vbCrLf
                        body &= "Company:         " & unit & vbCrLf
                        body &= "Contact:         " & official & vbCrLf
                        body &= "Email:           " & email & vbCrLf
                        body &= "Phone:           " & phone & vbCrLf
                        body &= "Address:         " & address & vbCrLf
                        body &= "Website:         " & unitweb & vbCrLf
                        body &= "Notes:           " & comments & vbCrLf
                        body &= "Registered on:   " & startDate & vbCrLf
                        For j As Integer = 0 To emailTable.Rows.Count - 1
                            SendHTMLEmail("", subject, body, emailTable.Rows(j)("Email"), ConfigurationManager.AppSettings("supportemail").ToString)
                        Next
                    End If
                Catch exEmail As Exception
                    WriteToAccessLog("ProductionCustomer", "Email notification failed: " & exEmail.Message, 2)
                End Try

                WriteToAccessLog("ProductionCustomer", "DataAI ETL production registration: " & unit & " / " & email, 1)
                lnkLicense.NavigateUrl = "SAVEDFILES/DataAIETLproduction/CommercialLicense/" & GetLicenseFile(distrMode)
                pnlForm.Visible = False
                pnlSuccess.Visible = True
            Else
                ret = "Registration could not be saved. Please try again or contact us directly. (" & ret & ")"
            End If

        Catch ex As Exception
            ret = ex.Message
            WriteToAccessLog("ProductionCustomer", "Exception: " & ret, 3)
        End Try

        If ret <> "" AndAlso pnlForm.Visible Then
            LblMessage.Text = "<span style=""color:var(--red);"">" & ret & "</span>"
        End If
    End Sub

    Private Function GetLicenseFile(distrMode As String) As String
        Select Case distrMode
            Case "AWS"       : Return "aws-glue-emr.html"
            Case "Alteryx"   : Return "alteryx.html"
            Case "Databricks": Return "databricks.html"
            Case "DotNet"    : Return "dotnet.html"
            Case "Google"    : Return "google-cloud-dataproc.html"
            Case "IRIS"      : Return "intersystems-iris.html"
            Case "Oracle"    : Return "oracle-aidp-spark.html"
            Case "PowerBI"   : Return "power-bi.html"
            Case "SSIS"      : Return "ssis.html"
            Case "Tableau"   : Return "tableau.html"
            Case "Talend"    : Return "talend-mulesoft.html"
            Case Else        : Return "index.html"
        End Select
    End Function

End Class
