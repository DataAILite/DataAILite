Imports System.Net
Imports System.Net.Http
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports System.IO

'Imports System.Threading
'Imports System.Text.Json
Public Class RequestBody
    Public model As String
    Public max_tokens As Integer
    Public organization As String
    Public prompt As String
End Class
Partial Class ChatAI
    Inherits System.Web.UI.Page
    Dim apiKey As String '= ConfigurationManager.AppSettings("openaikey").ToString
    Dim OpenAIOrganization As String '= ConfigurationManager.AppSettings("openaiorganization").ToString
    Dim apiUrl As String '= ConfigurationManager.AppSettings("apiURL").ToString
    Dim model As String '= ConfigurationManager.AppSettings("openaimodel").ToString
    Dim maxTokens As Integer '= CInt(ConfigurationManager.AppSettings("openaimaxTokens").ToString)
    Public chatrequest As String = String.Empty
    Public chatresponse As String = String.Empty
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        'Dim apiKey As String = String.Empty
        'Dim OpenAIOrganization As String = String.Empty
        'Dim apiUrl As String = String.Empty
        'Dim model As String = String.Empty
        'Dim maxTokens As Integer = 0
        Try
            apiKey = ConfigurationManager.AppSettings("openaikey").ToString.Trim
        Catch ex As Exception
            apiKey = ""
        End Try
        If apiKey.Trim = "" AndAlso Session("OpenAIkey") IsNot Nothing AndAlso Session("OpenAIkey").ToString.Trim <> "" Then
            apiKey = Session("OpenAIkey").ToString.Trim
        End If
        Session("OpenAIkey") = apiKey
        If apiKey.Trim = "" Then
            chatresponse = "OpenAI user setting is not defined. AI is not available."
            Exit Sub
        End If

        Try
            OpenAIOrganization = ConfigurationManager.AppSettings("openaiorganization").ToString.Trim
        Catch ex As Exception
            OpenAIOrganization = ""
        End Try
        If OpenAIOrganization.Trim = "" AndAlso Session("OpenAIOrganization") IsNot Nothing AndAlso Session("OpenAIOrganization").ToString.Trim <> "" Then
            OpenAIOrganization = Session("OpenAIOrganization").ToString.Trim
        End If
        Session("OpenAIOrganization") = OpenAIOrganization

        Try
            apiUrl = ConfigurationManager.AppSettings("apiURL").ToString.Trim
        Catch ex As Exception
            apiUrl = ""
            'apiUrl = "https://api.openai.com/v1/chat/completions"
        End Try
        If apiUrl.Trim = "" AndAlso Session("OpenAIurl") IsNot Nothing AndAlso Session("OpenAIurl").ToString.Trim <> "" Then
            apiUrl = Session("OpenAIurl").ToString.Trim
        End If
        Session("OpenAIurl") = apiUrl

        Try
            model = ConfigurationManager.AppSettings("openaimodel").ToString.Trim
        Catch ex As Exception
            model = ""
            'model = "gpt-4o-mini"
        End Try
        If model.Trim = "" AndAlso Session("OpenAImodel") IsNot Nothing AndAlso Session("OpenAImodel").ToString.Trim <> "" Then
            model = Session("OpenAImodel").ToString.Trim
        End If
        Session("OpenAImodel") = model


        Try
            maxTokens = CInt(ConfigurationManager.AppSettings("openaimaxTokens").ToString.Trim)
        Catch ex As Exception
            maxTokens = 128000
        End Try
        If maxTokens = 128000 AndAlso Session("maxTokens") IsNot Nothing AndAlso Session("maxTokens").ToString.Trim <> "" Then
            maxTokens = CInt(Session("maxTokens").ToString.Trim)
        End If
        Session("maxTokens") = maxTokens


        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If
        If Session("DataToChatAI") Is Nothing Then
            Session("DataToChatAI") = ""
        End If
        If Request("qu") Is Nothing OrElse Request("qu").ToString.Trim = "" OrElse Request("qu").ToString.Trim <> "yes" Then
            If Session("GraphType") IsNot Nothing AndAlso Session("GraphType") <> "matrix" Then
                Session("QuestionToAI") = ""
            End If
        End If
        If Session("grpstats") = "yes" Then
            'model = "DALL-E" 'cannot find
        End If
        If Session("QuestionToAI") Is Nothing OrElse Session("QuestionToAI").ToString.Trim = "" Then
            Session("QuestionToAI") = "Interpret the data and make meaningful analytical reports studying trends, correlations, etc... "
        End If
        If Not Request("srd") Is Nothing AndAlso Not Request("pg") Is Nothing Then
            HyperLinkDataAI.NavigateUrl = "DataAI.aspx?pg=" & Request("pg") & "&srd=" & Request("srd")
            HyperLinkDataAI1.NavigateUrl = "DataAI.aspx?pg=" & Request("pg") & "&srd=" & Request("srd")
        End If
        'Session("OURConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        If Session("OURConnProvider") = "Sqlite" AndAlso apiKey.Trim = "" Then
            HyperLinkOpenAIcreds.Visible = True
            HyperLinkOpenAIcreds.Enabled = True
        Else
            HyperLinkOpenAIcreds.Visible = False
            HyperLinkOpenAIcreds.Enabled = False
        End If
    End Sub
    Private Sub ChatAI_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack Then
            Exit Sub
        End If
        If apiKey.Trim = "" Then
            chatresponse = "OpenAI user setting is not defined. AI is not available."
            Exit Sub
        End If
        chatrequest = Session("QuestionToAI")  '"Interpret this data."
        Dim prompt As String = Session("QuestionToAI") & Environment.NewLine() & Session("DataToChatAI").ToString

        If prompt.Length > maxTokens Then
            prompt = prompt.Substring(0, maxTokens)
            'cut for last new line
            Dim ln = prompt.LastIndexOf(Environment.NewLine())
            Dim cutted As String = prompt.Substring(ln)
            If cutted.Length > 200 Then
                cutted = cutted.Substring(0, 200)
            End If
            If ln > 0 Then
                prompt = prompt.Substring(0, ln)
            End If
            'chatresponse = "DATA WERE TOO BIG AND HAVE BEEN TRUNCATED!!! The last " & (prompt.Length - 128000).ToString & " characters of the data have been cut out. Click the DataAI link on the top of this page for more analytics and for filtering data to submit something shorter to AI. " & Environment.NewLine() & Environment.NewLine()
            chatresponse = "DATA WERE TOO BIG AND HAVE BEEN TRUNCATED!!! Only the first " & ln.ToString & " characters of the data have been analyzed.  It was truncated from the record: " & cutted & "....  " & Environment.NewLine() & Environment.NewLine() & " Click the DataAI link on the top of this page for more analytics and for filtering data to submit something shorter to AI. " & Environment.NewLine() & Environment.NewLine()

        End If

        Dim ret As String = String.Empty
        Try
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12

            Dim request As HttpWebRequest = WebRequest.Create(apiUrl)
            request.Method = "POST"
            request.ContentType = "application/json"
            request.Headers.Add("Authorization", "Bearer " & apiKey)
            request.Headers.Add("OpenAI-Organization", OpenAIOrganization)

            Dim data As String = "{" &
                """model"":""" & model & """," &
                 """messages"": [{""role"":""user"", ""content"": """ & PadQuotes(prompt).Replace("""", "\""") & """}]" &
            "}"
            'For testing:  """max_tokens"":""" & maxTokens & """," &
            'chatrequest = data
            Using streamWriter As New StreamWriter(request.GetRequestStream())
                streamWriter.Write(data)
                streamWriter.Flush()
            End Using

            Using response As HttpWebResponse = request.GetResponse()
                Using streamReader As New StreamReader(response.GetResponseStream())
                    Dim sJson As String = streamReader.ReadToEnd()
                    'For testing:
                    'chatresponse = "              <br/><br/><br/>       JSON Response: " + sJson

                    Dim oJavaScriptSerializer As New System.Web.Script.Serialization.JavaScriptSerializer
                    Dim oJson As Hashtable = oJavaScriptSerializer.Deserialize(Of Hashtable)(sJson)

                    Dim responseBody As String = ""
                    If oJson.ContainsKey("choices") Then
                        Dim choices As Object() = CType(oJson("choices"), Object())
                        If choices.Length > 0 Then
                            Dim firstChoice As Dictionary(Of String, Object) = CType(choices(0), Dictionary(Of String, Object))

                            If firstChoice.ContainsKey("message") Then
                                Dim message As Dictionary(Of String, Object) = CType(firstChoice("message"), Dictionary(Of String, Object))

                                If message.ContainsKey("content") Then
                                    responseBody = CType(message("content"), String)
                                    chatresponse = FormatResponse(chatresponse & responseBody)
                                End If
                            Else
                                chatresponse = " ERROR!! Key 'content' not found in the message." & chatresponse
                            End If
                        Else

                            chatresponse = " ERROR!! Key 'message' not found in the first choice." & chatresponse
                        End If
                    Else

                        chatresponse = " ERROR!! Key 'choices' not found in JSON." & chatresponse
                    End If

                    'For testing:
                    'chatresponse = "'" & responseBody & "'" & chatresponse  

                End Using
            End Using
        Catch ex As Exception
            If ex.Message.Contains("The remote server returned an error: (429) Too Many Requests") Then
                chatresponse = "ERROR!! Too big data for our access to the ChatGPT-4o. <br/>The data you submitted was too long, <br/>please see our DataAI page for more analytics <br/>and for filtering data to submit something shorter to AI: "

                'truncate the big data ==========================================================================================================================
                Dim maxretries As Integer = 5
                If Not ConfigurationManager.AppSettings("maxretries") Is Nothing Then
                    maxretries = CInt(ConfigurationManager.AppSettings("maxretries").ToString)
                End If
                Dim retrydelay As Integer = 2000
                Dim len As Integer
                For i = 1 To maxretries - 1
                    len = prompt.Length 'Session("DataToChatAI").ToString.Length
                    len = len - CInt(len * (i / maxretries))
                    'prompt = Session("QuestionToAI") & Environment.NewLine() & Session("DataToChatAI").ToString.Substring(0, len)
                    prompt = prompt.Substring(0, len)

                    'cut for last new line
                    len = prompt.LastIndexOf(Environment.NewLine())
                    Dim cutted As String = prompt.Substring(len)
                    If cutted.Length > 200 Then
                        cutted = cutted.Substring(200)
                    End If
                    If len > 0 Then
                        prompt = prompt.Substring(0, len)
                    End If

                    ret = String.Empty
                    Try
                        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12
                        Dim request As HttpWebRequest = WebRequest.Create(apiUrl)
                        request.Method = "POST"
                        request.ContentType = "application/json"
                        request.Headers.Add("Authorization", "Bearer " & apiKey)
                        request.Headers.Add("OpenAI-Organization", OpenAIOrganization)
                        Dim data As String = "{" &
                            """model"":""" & model & """," &
                            """messages"": [{""role"":""user"", ""content"": """ & PadQuotes(prompt).Replace("""", "\""") & """}]" & "}"
                        'chatrequest = data
                        Using streamWriter As New StreamWriter(request.GetRequestStream())
                            streamWriter.Write(data)
                            streamWriter.Flush()
                        End Using
                        Using response As HttpWebResponse = request.GetResponse()
                            Using streamReader As New StreamReader(response.GetResponseStream())
                                Dim sJson As String = streamReader.ReadToEnd()
                                'For testing:
                                'chatresponse = "              <br/><br/><br/>       JSON Response: " + sJson
                                Dim oJavaScriptSerializer As New System.Web.Script.Serialization.JavaScriptSerializer
                                Dim oJson As Hashtable = oJavaScriptSerializer.Deserialize(Of Hashtable)(sJson)
                                Dim responseBody As String = ""
                                If oJson.ContainsKey("choices") Then
                                    Dim choices As Object() = CType(oJson("choices"), Object())
                                    If choices.Length > 0 Then
                                        Dim firstChoice As Dictionary(Of String, Object) = CType(choices(0), Dictionary(Of String, Object))
                                        If firstChoice.ContainsKey("message") Then
                                            Dim message As Dictionary(Of String, Object) = CType(firstChoice("message"), Dictionary(Of String, Object))
                                            If message.ContainsKey("content") Then
                                                responseBody = CType(message("content"), String)
                                                'responseBody = "DATA WERE TOO BIG AND HAVE BEEN TRUNCATED!!! The last " & i.ToString & "/" & maxretries.ToString & " of the data have been cut out. Click the DataAI link on the top of this page for more analytics and for filtering data to submit something shorter to AI. " & Environment.NewLine() & Environment.NewLine() & responseBody
                                                responseBody = "DATA WERE TOO BIG AND HAVE BEEN TRUNCATED!!! Only the first " & len.ToString & " characters of the data have been analyzed. It was truncated from the record: " & cutted & "....  " & Environment.NewLine() & Environment.NewLine() & " Click the DataAI link on the top of this page for more analytics and for filtering the data to submit something shorter to AI. " & Environment.NewLine() & Environment.NewLine() & responseBody
                                                chatresponse = FormatResponse(responseBody)
                                            End If
                                        Else
                                            chatresponse = " ERROR!! Key 'content' not found in the message." & chatresponse
                                        End If
                                    Else
                                        chatresponse = " ERROR!! Key 'message' not found in the first choice." & chatresponse
                                    End If
                                Else
                                    chatresponse = " ERROR!! Key 'choices' not found in JSON." & chatresponse
                                End If

                                'For testing:
                                'chatresponse = "'" & responseBody & "'" & chatresponse
                                If Not chatresponse.Trim.StartsWith("ERROR!!") Then
                                    Exit For
                                End If
                            End Using
                        End Using
                    Catch exc As Exception
                        ret = exc.Message
                    End Try
                    'retrydelay = retrydelay * 2
                Next
                ''trancated big data ================================================================================================================

                ''big data ==========================================================================================================================
                'Dim maxretries As Integer = 5
                'Dim retrydelay As Integer = 2000
                'For i = 0 To maxretries - 1
                '    'Sleep(2 seconds)
                '    Thread.Sleep(retrydelay)
                '    'Dim prompt As String = Session("QuestionToAI") & Environment.NewLine() & Session("DataToChatAI").ToString
                '    'Dim ret As String = String.Empty
                '    Try
                '        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12
                '        Dim request As HttpWebRequest = WebRequest.Create(apiUrl)
                '        request.Method = "POST"
                '        request.ContentType = "application/json"
                '        request.Headers.Add("Authorization", "Bearer " & apiKey)
                '        request.Headers.Add("OpenAI-Organization", OpenAIOrganization)
                '        Dim data As String = "{" &
                '            """model"":""" & model & """," &
                '            """messages"": [{""role"":""user"", ""content"": """ & PadQuotes(prompt).Replace("""", "\""") & """}]" & "}"
                '        'chatrequest = data
                '        Using streamWriter As New StreamWriter(request.GetRequestStream())
                '            streamWriter.Write(data)
                '            streamWriter.Flush()
                '        End Using
                '        Using response As HttpWebResponse = request.GetResponse()
                '            Using streamReader As New StreamReader(response.GetResponseStream())
                '                Dim sJson As String = streamReader.ReadToEnd()
                '                'For testing:
                '                'chatresponse = "              <br/><br/><br/>       JSON Response: " + sJson
                '                Dim oJavaScriptSerializer As New System.Web.Script.Serialization.JavaScriptSerializer
                '                Dim oJson As Hashtable = oJavaScriptSerializer.Deserialize(Of Hashtable)(sJson)
                '                Dim responseBody As String = ""
                '                If oJson.ContainsKey("choices") Then
                '                    Dim choices As Object() = CType(oJson("choices"), Object())
                '                    If choices.Length > 0 Then
                '                        Dim firstChoice As Dictionary(Of String, Object) = CType(choices(0), Dictionary(Of String, Object))
                '                        If firstChoice.ContainsKey("message") Then
                '                            Dim message As Dictionary(Of String, Object) = CType(firstChoice("message"), Dictionary(Of String, Object))
                '                            If message.ContainsKey("content") Then
                '                                responseBody = CType(message("content"), String)
                '                                chatresponse = FormatResponse(responseBody)
                '                            End If
                '                        Else
                '                            chatresponse = " ERROR!! Key 'content' not found in the message." & chatresponse
                '                        End If
                '                    Else
                '                        chatresponse = " ERROR!! Key 'message' not found in the first choice." & chatresponse
                '                    End If
                '                Else
                '                    chatresponse = " ERROR!! Key 'choices' not found in JSON." & chatresponse
                '                End If

                '                'For testing:
                '                'chatresponse = "'" & responseBody & "'" & chatresponse
                '                If Not chatresponse.Trim.StartsWith("ERROR!!") Then
                '                    Exit For
                '                End If
                '            End Using
                '        End Using
                '    Catch exc As Exception
                '        ret = exc.Message
                '    End Try
                '    retrydelay = retrydelay * 2
                'Next
                ''big data ==========================================================================================================================

            Else
                chatresponse = "ERROR!! " & ex.Message & ", " & chatresponse
            End If

        End Try

        'Dim taskResult As Task = GenerateText(prompt)
    End Sub

    Public Async Function GenerateText(prompt As String) As Task(Of String)
        Dim ret As String = String.Empty
        Try
            Dim requestBody = New RequestBody
            requestBody.model = model
            requestBody.max_tokens = maxTokens
            requestBody.prompt = prompt

            requestBody.organization = OpenAIOrganization

            Dim jsonString As String = JsonConvert.SerializeObject(requestBody)
            chatrequest = jsonString
            Dim requestContent As System.Net.Http.HttpContent
            requestContent = New StringContent(jsonString, Encoding.UTF8, "application/json")

            Dim client As New System.Net.Http.HttpClient
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " & apiKey)
            client.DefaultRequestHeaders.Add("OpenAI-Organization", OpenAIOrganization)
            'client.DefaultRequestHeaders.Add("Content-Type", "application/json")
            Dim response = Await client.PostAsync(apiUrl, requestContent)
            response.EnsureSuccessStatusCode()
            Dim responseBody As String = Await response.Content.ReadAsStringAsync()

            chatresponse = "ChatGPT: '" & responseBody & "'"
            Return responseBody

        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Private Function PadQuotes(ByVal s As String) As String

        If s.IndexOf("\") <> -1 Then
            s = Replace(s, "\", "\\")
        End If

        If s.IndexOf(vbCrLf) <> -1 Then
            s = Replace(s, vbCrLf, "\n")
        End If

        If s.IndexOf(vbCr) <> -1 Then
            s = Replace(s, vbCr, "\r")
        End If

        If s.IndexOf(vbLf) <> -1 Then
            s = Replace(s, vbLf, "\f")
        End If

        If s.IndexOf(vbTab) <> -1 Then
            s = Replace(s, vbTab, "\t")
        End If

        If s.IndexOf("""") = -1 Then
            Return s
        Else
            Return Replace(s, """", "\""")
        End If
    End Function
    Private Function FormatResponse(ByVal s As String) As String
        Dim ret As String = String.Empty
        Try
            If s.IndexOf(vbCrLf) <> -1 Then
                s = Replace(s, vbCrLf, "<br/>")
            End If

            If s.IndexOf(vbCr) <> -1 Then
                s = Replace(s, vbCr, "<br/>")
            End If

            If s.IndexOf(vbLf) <> -1 Then
                s = Replace(s, vbLf, "<br/>")
            End If

            If s.IndexOf(vbTab) <> -1 Then
                s = Replace(s, vbTab, "&nbsp;&nbsp;&nbsp;")
            End If
            ret = s.Replace("Summary:", "<br/><br/><b>Summary:</b><br/><br/>")
            ret = s.Replace("Conclusion:", "<br/><br/><b>Conclusion:</b><br/><br/>")
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function

    Private Sub btnQuestion_Click(sender As Object, e As EventArgs) Handles btnQuestion.Click
        Session("QuestionToAI") = txtQuestion.Text
        Response.Redirect("ChatAI.aspx?qu=yes")
    End Sub
End Class


