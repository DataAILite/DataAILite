Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.IO.Compression
Imports System.Math
Imports System.Net
Imports System.Net.Mail
Imports System.Reflection
Imports System.Text.RegularExpressions
Imports System.Web.Configuration
Imports System.Web.UI.Page
Imports System.Xml
Imports Microsoft.Data.Sqlite

Imports MySql.Data.MySqlClient
Imports Mysqlx.XDevAPI.Common
Imports Newtonsoft.Json
Public Module mFunctions
    Public sqliteconn As SqliteConnection
    Public applpath As String
    Public Function getMaxRequestLength() As Long
        Dim requestLength As Long = 4096
        Dim runTime As HttpRuntimeSection = CType(ConfigurationManager.GetSection("system.web/httpRuntime"), HttpRuntimeSection)
        If runTime IsNot Nothing Then
            requestLength = runTime.MaxRequestLength
        Else
            Dim cfg As System.Configuration.Configuration = ConfigurationManager.OpenMachineConfiguration()
            Dim cs As ConfigurationSection = cfg.SectionGroups("system.web").Sections("httpRuntime")
            If cs IsNot Nothing Then
                requestLength = Convert.ToInt64(cs.ElementInformation.Properties("maxRequestLength").Value)
            End If
        End If

        Return requestLength
    End Function
    Public Function CorrectTableNameWithDots(ByVal tblname As String, Optional userconnprv As String = "") As String
        If userconnprv = "System.Data.SqlClient" AndAlso tblname.IndexOf(".") > 0 Then
            Return ("[" & tblname & "]").Replace("[[", "[").Replace("]]", "]")
        End If
        'no more than 1 dot 
        Dim tblparts() As String = tblname.Split(".")
        If tblparts.Length < 3 Then Return tblname
        Dim i, n As Integer
        n = tblparts.Length  '>2
        Dim ret As String = tblparts(0)
        For i = 1 To n - 1
            If i < n - 1 Then
                ret = ret & "_"
            Else
                ret = ret & "."
            End If
            ret = ret & tblparts(i)
        Next
        Return ret
    End Function
    Public Function CorrectFieldNameWithDots(ByVal fldname As String) As String
        'no more than 2 dots 
        Dim fldparts() As String = fldname.Split(".")
        If fldparts.Length < 4 Then Return fldname
        Dim i, n As Integer
        n = fldparts.Length  '>3
        Dim ret As String = fldparts(0)
        For i = 1 To n - 1
            If i < n - 2 Then
                ret = ret & "_"
            Else
                ret = ret & "."
            End If
            ret = ret & fldparts(i)
        Next
        Return ret
    End Function
    Public Function WriteXMLStringToFile(ByVal rfile As String, ByVal rstr As String) As String
        'write the string rdlstr into the file downrepfile
        Try
            Dim doc As New XmlDocument
            doc.Load(New StringReader(rstr))
            File.Delete(rfile)
            doc.Save(rfile)
            Return rfile
        Catch ex As Exception
            Return "ERROR!! writing to " & rfile & ": " & ex.Message
        End Try
    End Function
    Public Function CopyArray(ByVal objArray As Object) As Object
        Try
            Dim objRet As Object = Nothing
            If Not objArray Is Nothing Then
                Dim typeObj As System.Type = objArray.GetType

                'Object must be an array
                If typeObj.HasElementType Then
                    Dim typeElement As System.Type = typeObj.GetElementType
                    Dim i As Integer
                    Dim arr As Array = CType(objArray, Array)
                    Dim n As Integer = arr.Length
                    'Create a copy of the array (no values in it yet)
                    Dim arCopy As Array = Array.CreateInstance(typeElement, n)
                    Dim obj As Object
                    'Put copied values in copied array
                    For i = 0 To n - 1
                        obj = arr.GetValue(i)
                        If Not obj Is Nothing Then
                            obj = CopyObject(obj)
                            arCopy.SetValue(obj, i)
                        End If
                    Next
                    objRet = arCopy
                Else
                    objRet = CopyObject(objArray)
                End If
            End If
            Return objRet
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Function CopyObject(ByVal objSource As Object) As Object
        Try
            Dim objRet As Object = Nothing
            If Not objSource Is Nothing Then
                Dim typeObj As System.Type = objSource.GetType
                Dim i As Integer

                'Only creates new object and copies fields/properties if objSource is a class
                'and not string 
                If typeObj.IsClass And typeObj.Name <> "String" Then
                    Dim Fields As FieldInfo() = typeObj.GetFields(BindingFlags.Public Or BindingFlags.Instance)
                    Dim Field As FieldInfo
                    Dim Props As PropertyInfo() = typeObj.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
                    Dim Prop As PropertyInfo
                    Dim objConstructor As ConstructorInfo = typeObj.GetConstructor(BindingFlags.Public Or BindingFlags.Instance, Nothing, CallingConventions.Standard, System.Type.EmptyTypes, Nothing)

                    If Not objConstructor Is Nothing Then
                        objRet = objConstructor.Invoke(Nothing)
                        Dim obj As Object

                        If Not objRet Is Nothing Then
                            'Copy fields
                            If Not Fields Is Nothing Then
                                For i = 0 To Fields.Length - 1
                                    Field = Fields(i)
                                    Try
                                        obj = Field.GetValue(objSource)
                                        'If field is not a value type, make a copy of it too
                                        If Not obj Is Nothing AndAlso
                                           obj.GetType.IsClass AndAlso
                                           obj.GetType.Name <> "String" Then
                                            If Not obj.GetType.HasElementType Then
                                                obj = CopyObject(obj)
                                            Else
                                                obj = CopyArray(obj)
                                            End If
                                        End If
                                        Field.SetValue(objRet, obj)
                                    Catch ex As Exception
                                        'no processing
                                    End Try
                                Next
                            End If
                            'Copy properties
                            If Not Props Is Nothing Then
                                For i = 0 To Props.Length - 1
                                    Prop = Props(i)
                                    Dim Params As ParameterInfo()

                                    Try
                                        'Only copy read/write properties
                                        If Prop.CanRead And Prop.CanWrite Then
                                            Params = Prop.GetIndexParameters
                                            If Params.Length = 0 Then
                                                obj = Prop.GetValue(objSource, Nothing)
                                                Prop.SetValue(objRet, obj, Nothing)
                                            End If
                                        End If
                                    Catch ex As Exception
                                        'no processing
                                    End Try
                                Next
                            End If
                        Else
                            objRet = objSource
                        End If
                    Else
                        objRet = objSource
                    End If
                Else
                    objRet = objSource
                End If
            End If

            Return objRet
        Catch ex As Exception
            'Err.ProcessError(ex)
            Return Nothing
        End Try
    End Function
    Public Function Pieces(ByVal sStr As String, ByVal Delim As String) As Integer
        'This function returns the number of pieces in sStr delimited by
        'Delim.

        Dim i As Integer = 0
        Dim b As Integer = sStr.IndexOf(Delim)
        While b > -1
            i = i + 1
            b = sStr.IndexOf(Delim, b + Delim.Length)
        End While

        Return i + 1
    End Function

    Public Function Pieces(ByVal sStr As String, ByVal Delim As Char) As Integer
        'This function returns the number of pieces in sStr delimited by
        'Delim.
        Dim i As Integer = 0
        Dim b As Integer = sStr.IndexOf(Delim)

        While b > -1
            i = i + 1
            b = sStr.IndexOf(Delim, b + 1)
        End While

        Return i + 1
    End Function
    Public Function Piece(ByVal sStr As String, ByVal Delim As String,
                     Optional ByVal iStart As Integer = 0,
                     Optional ByVal iEnd As Integer = 0) As String
        'This function acts like the $Piece function in Mumps
        'Usage: sStr=Piece(sAnotherString,sDelimeter,iStartPiece,iEndPiece)

        Dim iPrev As Integer = 1
        Dim b As Integer
        Dim iPieces As Integer = Pieces(sStr, Delim)
        Dim sRetStr As String = ""
        Dim i As Integer

        If iStart < 1 Then iStart = 1
        If iEnd < iStart Then iEnd = iStart

        If iPieces = 1 Then
            If iStart = 1 Then sRetStr = sStr
        Else
            For i = 1 To iPieces
                b = sStr.IndexOf(Delim, iPrev - 1) + 1
                If b = 0 Then b = sStr.Length + 1
                If i = iStart Then
                    sRetStr = Mid(sStr, iPrev, b - iPrev)
                ElseIf i > iStart Then
                    sRetStr = sRetStr & Delim & Mid(sStr, iPrev, b - iPrev)
                End If
                If i = iEnd Then Exit For
                iPrev = b + Delim.Length
            Next i
        End If
        Return sRetStr
    End Function
    Public Function SetPiece(ByVal sStr As String, ByVal sPiece As String, ByVal sDelim As String, ByVal nPiece As Integer) As String
        Dim sRet As String
        Dim sS As String
        Dim nPieces As Integer


        sRet = sStr
        If nPiece > 0 Then
            nPieces = Pieces(sStr, sDelim)
            If nPiece > nPieces Then
                sS = StringOf(sDelim, nPiece - nPieces)
                sRet = sRet & sS
                nPieces = nPiece
            ElseIf nPieces = 1 Then
                sRet = sPiece
            End If

            If nPieces > 1 Then
                If nPiece = 1 Then
                    sRet = sPiece & sDelim & Piece(sStr, sDelim, 2, nPieces)
                ElseIf nPiece = nPieces Then
                    sRet = Piece(sStr, sDelim, 1, nPiece - 1) & sDelim & sPiece
                Else
                    sRet = Piece(sStr, sDelim, 1, nPiece - 1) & sDelim & sPiece & sDelim & Piece(sStr, sDelim, nPiece + 1, nPieces)
                End If
            End If
        End If
        Return sRet
    End Function

    Public Function StringOf(ByVal sS As String, ByVal n As Integer) As String
        Dim sb As New StringBuilder
        Dim i As Integer

        For i = 1 To n
            sb.Append(sS)
        Next

        Return sb.ToString
    End Function

    Public Function mString(ByVal myString As String) As String
        If IsDBNull(myString) = True Then
            myString = ""
        End If
        Return CType(Trim(myString), String)
    End Function
    Public Function PartOfString(ByVal myString As String, ByVal StringStart As String, ByVal StringFinished As String) As String
        'old - ONLY FOR SQL Server
        Dim partstring As String
        Dim pos1, pos2, i, j As Integer
        pos1 = 0
        pos2 = 0
        i = 0
        partstring = ""
        If IsDBNull(myString) = True Then
            myString = ""
            Return CType(Trim(myString), String)
            Exit Function
        End If

        Dim myConnection As SqlConnection
        Dim myCommand As New SqlClient.SqlCommand
        Dim myAdapter As SqlClient.SqlDataAdapter
        Dim myRecords As DataTable
        Dim myView As DataView

        'myCommand = (New Comd).Comd
        Dim myconstring As String
        myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
        myConnection = New SqlConnection(myconstring)
        myCommand.Connection = myConnection
        myCommand.CommandType = CommandType.StoredProcedure
        myCommand.CommandText = "xp_util_PartOfString"
        'parameters to store-procedure
        myCommand.Parameters.Add(New System.Data.SqlClient.SqlParameter("@StringStart", System.Data.SqlDbType.NVarChar))
        myCommand.Parameters.Add(New System.Data.SqlClient.SqlParameter("@StringFinished", System.Data.SqlDbType.NVarChar))
        myCommand.Parameters.Item(0).Value = StringStart
        myCommand.Parameters.Item(1).Value = StringFinished

        If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
        myAdapter = New SqlClient.SqlDataAdapter(myCommand)
        myRecords = New DataTable
        myAdapter.Fill(myRecords)
        myView = myRecords.DefaultView
        myAdapter.Dispose()
        myCommand.Connection.Close()
        myCommand.Dispose()


        Return CType(Trim(myString), String)
    End Function
    Public Function cleanTextFromRepeatedCommas(ByVal strText As String) As String
        Dim i, l, j, k As Integer
        j = 0
        k = 0
        Dim letter, newstr As String
        newstr = ""
        strText = Trim(strText)
        newstr = strText
        l = Len(strText)
        If l > 0 Then
            For i = 1 To l 'CHECK the beginning commas
                letter = Mid(strText, i, 1)
                If j = 0 Then
                    If (letter = ",") Then
                        newstr = Mid(strText, i + 1, l - i)
                    Else
                        j = i ' not "," found in i position
                        strText = Trim(newstr)
                        Exit For
                    End If
                End If
            Next
        End If
        l = Len(strText)
        If l > 0 Then
            For i = 0 To l - 1 'CHECK the ending commas
                letter = Mid(strText, l - i, 1)
                If k = 0 Then
                    If (letter = "," Or letter = " ") Then
                        newstr = Mid(strText, 1, l - i - 1)
                    Else
                        k = i 'not "," found in i position
                        strText = Trim(newstr)
                        l = Len(strText)
                        Exit For
                    End If
                End If
            Next
        End If
        j = 0
        k = 0
        'TODO CHECK the middle commas
        newstr = ""
        Replace(strText, " ", "")
        'CHECK the middle commas
        Replace(strText, ",,", ",", l)
        Return cleanText(strText)
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

    Public Function cleanTextLight(ByVal strText As String) As String
        'TODO more!!
        If strText Is Nothing OrElse strText = "" Then
            Return ""
        End If
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
                If (letter = ">") Or (letter = "<") Or (letter = "%") Or (letter = "'") Or (letter = Chr(39)) Then 'Or (letter = Chr(10)) Or (letter = Chr(13)) Then
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
        Return strText.Trim
    End Function
    Public Function cleanTextShort(ByVal strText As String) As String
        'TODO more!!
        If strText Is Nothing OrElse strText = "" Then
            Return ""
        End If
        strText = strText.Replace("<%", "***")
        strText = strText.Replace("%>", "***")
        strText = strText.Replace("</", "***")
        strText = strText.Replace("/>", "***")
        strText = strText.Replace("'", "***")
        strText = strText.Replace("<>", "!=")
        strText = strText.Replace("<", "***less than***")
        strText = strText.Replace(">", "***more than***")
        strText = strText.Replace("%", " ")
        strText = strText.Replace(Chr(39), " ")
        'Dim i, l As Integer
        'Dim letter As String
        'l = Len(strText)
        'If l > 0 Then
        '    For i = 1 To l
        '        letter = Mid(strText, i, 1)
        '        If (letter = ">") Or (letter = "<") Or (letter = "%") Or (letter = "'") Or (letter = Chr(39)) Then 'Or (letter = Chr(10)) Or (letter = Chr(13)) Then
        '            If (i = 1) Then
        '                strText = " " & Right(strText, l - 1)
        '            Else
        '                If (i = l) Then
        '                    strText = Left(strText, l - 1)
        '                Else
        '                    strText = Left(strText, i - 1) & " " & Right(strText, l - i)
        '                End If
        '            End If
        '        End If
        '    Next
        'End If
        Return strText.Trim
    End Function
    Public Function cleanTextOfFile(ByVal strText As String) As String
        strText = strText.Replace("http://", "...").Replace("https://", "...").Replace("file:///", "...")
        strText = strText.Replace("schemas.microsoft.com/", "http://schemas.microsoft.com/")
        strText = strText.Replace("<%", "*")
        strText = strText.Replace("%>", "*")
        strText = strText.Replace("</", "*")
        strText = strText.Replace("/>", "*")
        strText = strText.Replace("'", "*")
        Return strText
    End Function
    Public Function cleanFileText(ByVal filepath As String) As String
        Dim ret As String = String.Empty
        Try
            Dim localfile As String = cleanTextOfFile(File.ReadAllText(filepath))
            File.WriteAllText(filepath, localfile)
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function cleanFile(ByVal fpath As String, Optional ByVal clean As Boolean = True) As String
        'TODO!! XXE
        'show external url
        Try
            If fpath.EndsWith(".xls") Then
                'TODO clean Excel file
            Else
                Dim filetext As String
                'read file to filetext
                filetext = File.ReadAllText(fpath)
                'look for possible urls
                Dim urladdress As String = String.Empty
                Dim k As Integer
                k = filetext.IndexOf("http://")
                If k > 0 AndAlso filetext.Substring(k, 29) <> "http://schemas.microsoft.com/" Then
                    If clean = False Then
                        Return "XXE " & filetext.Substring(k, 29) & "... in the file " & fpath & " , upload is prohibited for security reason!!"
                    End If

                End If
                k = filetext.IndexOf("https://")
                If k > 0 Then
                    If clean = False Then
                        Return "XXE " & filetext.Substring(k, 29) & "... in the file " & fpath & " , upload is prohibited for security reason!!"
                    End If

                End If
                k = filetext.IndexOf("file:///")
                If k > 0 Then
                    If clean = False Then
                        Return "XXE " & filetext.Substring(k, 29) & "... in the file " & fpath & " , upload is prohibited for security reason!!"
                    End If
                End If
                'clean and save filetext in fpath
                filetext = filetext.Replace("http://", "").Replace("https://", "").Replace("file:///", "")
                filetext = filetext.Replace("schemas.microsoft.com/", "http://schemas.microsoft.com/")
                File.Delete(fpath)
                File.WriteAllText(fpath, filetext)
            End If

            Return fpath
        Catch ex As Exception
            Return ex.Message
        End Try
        Return ""
    End Function
    Public Function cleanSQL(ByVal strText As String) As String
        'TODO!!
        'Dim i, l As Integer
        'Dim letter As String
        'l = Len(strText)
        ''If l > 0 Then
        ''    For i = 1 To l
        ''        letter = Mid(strText, i, 1)
        ''        If (letter = ">") Or (letter = "<") Or (letter = "%") Or (letter = "'") Or (letter = Chr(39)) Then
        ''            If (i = 1) Then
        ''                strText = " " & Right(strText, l - 1)
        ''            Else
        ''                If (i = l) Then
        ''                    strText = Left(strText, l - 1)
        ''                Else
        ''                    strText = Left(strText, i - 1) & " " & Right(strText, l - i)
        ''                End If
        ''            End If
        ''        End If
        ''    Next
        ''End If
        cleanSQL = Trim(strText)
    End Function
    Public Function TermName(ByVal trmcod As String) As String
        Dim term() As String = {"Presummer", "Spring", "SummerI", "SummerII", "Fall", "Winter"}
        Dim R, L As String
        R = Right(trmcod, 1)
        L = Left(trmcod, 2)
        If CInt(L) < 80 Then
            TermName = term(CInt(R)) & "-" & "20" & L
        Else
            TermName = term(CInt(R)) & "-" & "19" & L
        End If
    End Function
    Public Function DrawDropDown(ByVal SQLs As String, ByVal DropDownName As DropDownList, ByVal FieldNameItem As String, ByVal FieldNameValue As String) As String
        'old - ONLY FOR SQL Server
        Dim myconstring, drvalue, drname As String
        Dim i As Integer
        i = 0
        myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
        'draw dropdown
        Dim comdTemp As New SqlCommand
        comdTemp.Connection = New SqlConnection(myconstring)
        If comdTemp.Connection.State = ConnectionState.Closed Then comdTemp.Connection.Open()
        comdTemp.CommandType = CommandType.Text
        comdTemp.CommandTimeout = 30000
        'If Session("admin") = "admin" Then
        comdTemp.CommandText = SQLs
        'End If
        Dim rsc As SqlDataReader
        rsc = comdTemp.ExecuteReader()
        'fill out the dropdown list 
        Do While (rsc.Read())
            drvalue = Trim(rsc(FieldNameValue).ToString)
            drname = Trim(rsc(FieldNameItem).ToString)
            If drvalue <> "" Then
                DropDownName.Items.Add(drvalue)
                DropDownName.Items(i).Text = rsc(FieldNameItem)
                i = i + 1
            End If
        Loop
        rsc.Close()
        Return ""
    End Function
    Public Function DrawDropDownFromDataTable(ByVal dt As DataTable, ByVal DropDownName As DropDownList, ByVal FieldNameItem As String, ByVal FieldNameValue As String) As String
        Dim drvalue, drname As String
        Dim i As Integer
        i = 0
        'fill out the dropdown list 
        For i = 0 To dt.Rows.Count - 1
            drvalue = Trim(dt.Rows(i)(FieldNameValue).ToString)
            drname = Trim(dt.Rows(i)(FieldNameItem).ToString)
            If drvalue <> "" Then
                DropDownName.Items.Add(drvalue)
                DropDownName.Items(i).Text = drname
            End If
        Next
        Return ""
    End Function
    Public Function DrawDropDownFiles(ByVal root As String, ByVal DropDownName As DropDownList) As String
        Dim files() As String = Directory.GetFiles(root)
        Dim f As String
        Dim drvalue As String = String.Empty
        For Each f In files
            Dim filename As String = Path.GetFileName(f)
            drvalue = Trim(filename)
            If drvalue <> "" Then
                DropDownName.Items.Add(drvalue)
            End If
        Next
        Return drvalue
    End Function
    Public Function DownloadFile(ByVal root As String, ByVal FilePath As String) As String
        If Not FilePath Is Nothing Then
            If File.Exists(FilePath) And FilePath.StartsWith(root) Then
                Dim filename As String = Path.GetFileName(FilePath)
                'Response.Clear()
                'Response.ContentType = "application/octet-stream"
                'Response.AddHeader("Content-Disposition", _
                '  "attachment; filename=""" & filename & """")
                'Response.Flush()
                'Response.WriteFile(FilePath)
            End If
            Return filename
        Else
            Return ""
        End If
    End Function
    Public Function FormatAsHTML(ByVal comments As String) As String
        Dim textHTML As String
        Dim i As Integer
        textHTML = "<b><u>"
        For i = 1 To Len(comments)
            If Mid(comments, i, 1) = "|" Then
                textHTML = textHTML & "<br/><b><u>"
            ElseIf Mid(comments, i, 2) = vbCrLf Then
                textHTML &= "<br/>"
            Else
                If Mid(comments, i, 2) = ": " Then
                    textHTML = textHTML & ": </b></u>"
                ElseIf Mid(comments, i, 2) = "):" Then
                    textHTML = textHTML & ")</b></u>"
                Else
                    textHTML = textHTML & Mid(comments, i, 1)
                End If
            End If
        Next
        'textHTML = MakeLinks(textHTML)  'not allowed by security of .NET
        If textHTML.IndexOf("File attached: </b></u> ") >= 0 Then
            textHTML = FileAttachLink(textHTML)
        End If
        Return textHTML
    End Function
    Public Function MakeLinks(ByVal comments As String) As String
        Dim textHTML, tmp, addr As String
        Dim i, j, n As Integer
        textHTML = ""
        '<a href="Default.aspx">Log Off</a>
        For i = 1 To Len(comments)
            If Mid(comments, i, 7) = "http://" Then
                textHTML = textHTML & "<br><b><a href="""
                tmp = comments.Substring(i + 6)
                j = tmp.Length
                If tmp.IndexOf(" ") > 0 OrElse tmp.IndexOf("|") OrElse tmp.IndexOf("|") OrElse tmp.IndexOf(",") OrElse tmp.IndexOf("!") OrElse tmp.IndexOf(";") Then
                    If tmp.IndexOf(" ") > 0 Then j = Min(j, tmp.IndexOf(" "))
                    If tmp.IndexOf("|") > 0 Then j = Min(j, tmp.IndexOf("|"))
                    If tmp.IndexOf(",") > 0 Then j = Min(j, tmp.IndexOf(","))
                    If tmp.IndexOf("!") > 0 Then j = Min(j, tmp.IndexOf("!"))
                    If tmp.IndexOf(";") > 0 Then j = Min(j, tmp.IndexOf(";"))
                    addr = "http://" & tmp.Substring(0, j).Trim
                    i = i + 6 + j
                Else
                    addr = tmp
                    i = i + 6
                End If
                textHTML = textHTML & addr & """>" & addr & "</a> </b></u>"
                Continue For
            ElseIf Mid(comments, i, 8) = "https://" Then
                textHTML = textHTML & "<br><b><a href="""
                tmp = comments.Substring(i + 7)
                j = tmp.Length
                If tmp.IndexOf(" ") > 0 OrElse tmp.IndexOf("|") OrElse tmp.IndexOf("|") OrElse tmp.IndexOf(",") OrElse tmp.IndexOf("!") OrElse tmp.IndexOf(";") Then
                    If tmp.IndexOf(" ") > 0 Then j = Min(j, tmp.IndexOf(" "))
                    If tmp.IndexOf("|") > 0 Then j = Min(j, tmp.IndexOf("|"))
                    If tmp.IndexOf(",") > 0 Then j = Min(j, tmp.IndexOf(","))
                    If tmp.IndexOf("!") > 0 Then j = Min(j, tmp.IndexOf("!"))
                    If tmp.IndexOf(";") > 0 Then j = Min(j, tmp.IndexOf(";"))
                    addr = "https://" & tmp.Substring(0, j).Trim
                    i = i + 7 + j
                Else
                    addr = tmp
                    i = i + 7
                End If
                textHTML = textHTML & addr & """>" & addr & "</a> </b></u>"
                Continue For
            Else
                textHTML = textHTML & Mid(comments, i, 1)
            End If
        Next
        Return textHTML
    End Function
    Public Function FileAttachLink(ByVal comments As String) As String
        Dim textHTML, tmp, addr As String
        Dim i, j, n As Integer
        textHTML = "<b>"
        '<a href="Default.aspx">Log Off</a>
        For i = 1 To Len(comments)
            If Mid(comments, i, 24) = "File attached: </b></u> " Then
                textHTML = textHTML & "<br><b>File attached: <a href="""
                tmp = comments.Substring(i + 23)
                If tmp.IndexOf(" ") > 0 Then
                    j = tmp.IndexOf(" ")
                    addr = tmp.Substring(0, j).Trim
                    i = i + 23 + j
                Else
                    addr = tmp
                    i = i + 23
                End If
                textHTML = textHTML & addr & """>" & addr & "</a> </b></u>"
                Continue For
            Else
                textHTML = textHTML & Mid(comments, i, 1)
            End If
        Next
        Return textHTML
    End Function
    Public Function FormatAsHTMLsimple(ByVal comments As String) As String
        Dim textHTML As String
        Dim i As Integer
        textHTML = ""
        For i = 1 To Len(comments)
            If Mid(comments, i, 1) = "|" Then
                textHTML = textHTML & "<br/>"
            Else
                textHTML = textHTML & Mid(comments, i, 1)
            End If

        Next
        FormatAsHTMLsimple = textHTML
    End Function
    Function SendTextEmail(ByVal attach As String, ByVal subj As String, ByVal textbody As String, ByVal towhom As String, ByVal fromwho As String) As String
        'NOT IN USE
        Dim iMsg As Object
        Dim iConf As Object
        iMsg = CreateObject("CDO.Message")
        iConf = CreateObject("CDO.Configuration")
        iMsg.Configuration = iConf
        If attach > "0" Then iMsg.AddAttachment(attach)
        iMsg.To = towhom
        iMsg.CC = ""
        iMsg.BCC = ""
        iMsg.From = fromwho
        iMsg.Subject = subj
        iMsg.TextBody = textbody
        '----------------------------------------------------
        iMsg.Send()
        Return ""
    End Function
    Function SendHTMLEmailOld(ByVal attach As String, ByVal subj As String, ByVal textbody As String, ByVal towhom As String, ByVal fromwho As String) As String
        'NOT IN USE
        Dim ret As String = String.Empty
        Try
            Dim iMsg As Object
            Dim iConf As Object
            iMsg = CreateObject("CDO.Message")
            iConf = CreateObject("CDO.Configuration")
            iMsg.Configuration = iConf
            If attach > "0" Then iMsg.AddAttachment(attach)
            iMsg.To = towhom
            iMsg.CC = ""
            iMsg.BCC = ""
            iMsg.From = fromwho
            iMsg.Subject = subj
            iMsg.HTMLBody = textbody
            '----------------------------------------------------
            If Trim(towhom) <> "" Then iMsg.Send()
            ret = "Email was send to " & towhom & " with subject " & subj
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function

    Function SendHTMLEmail(ByVal attach As String, ByVal subj As String, ByVal textbody As String, ByVal towhom As String, ByVal fromwho As String) As String
        If ConfigurationManager.AppSettings("smtpemail").ToString Is Nothing OrElse ConfigurationManager.AppSettings("smtpemail").ToString.Trim = "" Then
            Return "Setting for the email sending is not defined. Email cannot be sent."
        End If
        'for cloud2
        Dim ret As String = String.Empty
        Dim at As Attachment = Nothing

        If towhom = "" OrElse fromwho = "" Then
            ret = "Email has not been sent. No email addresses are found."
            Return ret
        End If
        Dim iMsg As MailMessage
        iMsg = New MailMessage
        iMsg.IsBodyHtml = False
        Dim addrs(1) As MailAddress
        Try
            'MailAddress validates email address format starting from .Net 4.0
            addrs(0) = New MailAddress(towhom)
            addrs(1) = New MailAddress(fromwho)
        Catch ex As Exception
            ret = ex.Message
            Return ret
        End Try
        iMsg.From = New MailAddress(fromwho)
        iMsg.To.Add(addrs(0))
        iMsg.Bcc.Add(addrs(1))
        iMsg.Subject = subj
        iMsg.Body = textbody
        If attach <> String.Empty Then
            For i As Integer = 1 To Pieces(attach, ",")
                at = New Attachment(Piece(attach, ",", i))
                iMsg.Attachments.Add(at)
            Next
        End If
        '----------------------------------------------------
        Try
            'set up smtp client
            Dim smtpClnt As New SmtpClient()
            'smtpClnt.Credentials = CredentialCache.DefaultNetworkCredentials
            'smtpClnt.UseDefaultCredentials = True
            'smtpClnt.Port = 25
            'smtpClnt.Credentials = New Net.NetworkCredential("support@oureports.com", ConfigurationManager.AppSettings("emailpass").ToString)

            smtpClnt.Host = ConfigurationManager.AppSettings("SmtpCred").ToString
            Dim smtpemail As String = ConfigurationManager.AppSettings("smtpemail").ToString
            Dim smtpemailpass = ConfigurationManager.AppSettings("smtpemailpass").ToString
            smtpClnt.Credentials = New Net.NetworkCredential(smtpemail, smtpemailpass)
            smtpClnt.Port = 587
            smtpClnt.EnableSsl = True

            ' send the message  
            smtpClnt.Send(iMsg)
            iMsg = Nothing
            'inform user that message has sent   
            ret = "Email has been sent to: " & towhom & " with subject: " & subj
        Catch ex As Exception
            ret = ex.Message
            'ret = SendHTMLEmailOrg(attach, subj, textbody, towhom, fromwho)
        End Try
        Return ret
    End Function
    Function SendHTMLEmailOrg(ByVal attach As String, ByVal subj As String, ByVal textbody As String, ByVal towhom As String, ByVal fromwho As String) As String
        'NOT IN USE
        'for cloud1
        Dim ret As String = String.Empty
        Dim at As Attachment = Nothing

        If towhom = "" OrElse fromwho = "" Then
            ret = "Email has not been sent. No email addresses are found."
            Return ret
        End If
        Dim iMsg As MailMessage
        iMsg = New MailMessage
        iMsg.IsBodyHtml = False
        Dim addrs(1) As MailAddress
        addrs(0) = New MailAddress(towhom)
        addrs(1) = New MailAddress(fromwho)
        iMsg.From = New MailAddress(fromwho)
        iMsg.To.Add(addrs(0))
        iMsg.Bcc.Add(addrs(1))
        iMsg.Subject = subj
        iMsg.Body = textbody
        If attach <> String.Empty Then
            For i As Integer = 1 To Pieces(attach, ",")
                at = New Attachment(Piece(attach, ",", i))
                iMsg.Attachments.Add(at)
            Next
        End If
        '----------------------------------------------------
        Try
            ' set up smtp client And credentials  
            Dim SmtpCred As String = ConfigurationManager.AppSettings("SmtpCred").ToString
            'Dim smtpClnt As New SmtpClient()

            Dim smtpClnt As New SmtpClient(SmtpCred)
            smtpClnt.Credentials = CredentialCache.DefaultNetworkCredentials
            smtpClnt.UseDefaultCredentials = True
            smtpClnt.Port = 25
            ' send the message  
            smtpClnt.Send(iMsg)
            'inform user that message has sent   
            ret = "Email has been sent To: " & towhom & " with subject: " & subj
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Public Function ExpressionToolTip(ByVal func As String) As String
        Dim ret As String = String.Empty
        Try
            'text
            If func = "Format" Then
                ret = "Examples:"
                ret &= vbCrLf & "Format(Fields!StartDate.Value,""Short Date"")"
                ret &= vbCrLf & "Format(Fields!StartDate.Value,""MM/dd/yyyy"")"
                ret &= vbCrLf & "Format(Fields!Cost.Value,""$####.00"")"
                ret &= vbCrLf & "Format(Fields!Cost.Value,""c2"")"
                ret &= vbCrLf & "Format((Fields!PartTimeJobs.Value/Fields!TotalJobs.Value)*100),""##.0 %"")"
                ret &= vbCrLf & "Format(Fields!PartTimeJobs.Value/Fields!TotalJobs.Value,""p1"")"
                ret &= vbCrLf & "Format(Fields!Weight.Value,""#,###0.00"")"
                ret &= vbCrLf & "Format(Fields!Weight.Value,""n2"")"
            ElseIf func = "FormatCurrency" Then
                ret = "FormatCurrency([Currency Value],[Decimal Places])"
                ret &= vbCrLf & vbCrLf & "Example: FormatCurrency(Fields!Cost.Value,2)"
            ElseIf func = "FormatDateTime" Then
                ret = "FormatDateTime([DateTime Value],[DateFormat])"
                ret &= vbCrLf & "Date Formats:"
                ret &= vbCrLf & "DateFormat.GeneralDate - Display a date and/or time. Display a date part as a short date. If there is a time part, display it as a long time. If present, both parts display."
                ret &= vbCrLf & vbCrLf & "DateFormat.LongDate - Display a date using the long date format specified in your computer's regional settings."
                ret &= vbCrLf & vbCrLf & "DateFormat.ShortDate - Display a date using the short date format specified in your computer's regional settings."
                ret &= vbCrLf & vbCrLf & "DateFormat.LongTime - Display a time using the time format specified in your computer's regional settings."
                ret &= vbCrLf & vbCrLf & "DateFormat.ShortTime - Display a time using the 24-hour format (hh:mm)."
            ElseIf func = "FormatNumber" Then
                ret = "FormatNumber([Numeric Value],[Decimal Places])"
                ret &= vbCrLf & vbCrLf & "Example: FormatNumber(Fields!Weight.Value,2)"
            ElseIf func = "FormatPercent" Then
                ret = "FormatPercent([Numeric Value (before being multiplied by 100)],[Decimal Places])"
                ret &= vbCrLf & vbCrLf & "Example: FormatPercent(Fields!PartTimeJobs.Value/Fields!TotalJobs.Value,1)"
            ElseIf func = "LCase" Then
                ret = "Example: LCase(Fields!Description.Value)"
            ElseIf func = "Left" Then
                ret = "Example:Left(Fields!Description.Value,4)"
            ElseIf func = "Len" Then
                ret = "Example: Len(Fields!Description.Value)"
            ElseIf func = "LTrim" Then
                ret = "Example: LTrim(Fields!Description.Value)"
            ElseIf func = "Mid" Then
                ret = "Example: Mid(Fields!Description.Value,3,4)"
            ElseIf func = "Replace" Then
                ret = "Example: Replace(Fields!Description.Value,""lamp"",""headlight"")"
            ElseIf func = "Right" Then
                ret = "Example: Right(Fields!Description.Value,4)"
            ElseIf func = "RTrim" Then
                ret = "Example: RTrim(Fields!Description.Value)"
            ElseIf func = "Trim" Then
                ret = "Example: Trim(Fields!Description.Value)"
            ElseIf func = "UCase" Then
                ret = "Example: UCase(Fields!Description.Value)"
                'math
            ElseIf func = "Abs" Then
                ret = "Example: Abs(Fields!YearlyIncome.Value-80000)"
            ElseIf func = "Acos" Then
                ret = "Example: Acos(Fields!Consine.Value)"
            ElseIf func = "Asin" Then
                ret = "Example: Asin(Fields!Sin.Value)"
            ElseIf func = "Atan" Then
                ret = "Example: Atan(Fields!Tangent.Value)"
            ElseIf func = "Cos" Then
                ret = "Example: Cos(Fields!Angle.Value)"
            ElseIf func = "Exp" Then
                ret = "Example: Exp(Fields!IntegerCounter.Value)"
            ElseIf func = "Log" Then
                ret = "Example: Log(Fields!NumberValue.Value)"
            ElseIf func = "Log10" Then
                ret = "Example: Log10(Fields!NumberValue.Value)"
            ElseIf func = "Round" Then
                ret = "Example: Round(Fields!YearlyIncome.Value/12,2)"
            ElseIf func = "Sign" Then
                ret = "Example: Sign(Fields!YearlyIncome.Value-60000)"
            ElseIf func = "Sin" Then
                ret = "Example: Sin(Fields!Angle.Value)"
            ElseIf func = "Tan" Then
                ret = "Example: Tan(Fields!Angle.Value)"
                'datetime
            ElseIf func = "DateAdd" Then
                ret = "Example: DateAdd(""d"",3,Fields!BirthDate.Value)"
            ElseIf func = "DateValue" Then
                ret = "Example: DateValue(Fields!BirthDate.Value)"
            ElseIf func = "Day" Then
                ret = "Example: Day(Fields!BirthDate.Value)"
            ElseIf func = "Hour" Then
                ret = "Example: Hour(Fields!StartDate.Value)"
            ElseIf func = "Minute" Then
                ret = "Example: Minute(Fields!StartDate.Value)"
            ElseIf func = "Month" Then
                ret = "Example: Month(Fields!StartDate.Value)"
            ElseIf func = "MonthName" Then
                ret = "Example: MonthName(Month(Fields!StartDate.Value))"
            ElseIf func = "Second" Then
                ret = "Example: Second(Fields!StartDate.Value)"
            ElseIf func = "TimeValue" Then
                ret = "Example: TimeValue(Fields!StartDate.Value)"
            ElseIf func = "Weekday" Then
                ret = "Example: Weekday(Fields!StartDate.Value,FirstDayOfWeek.System)"
            ElseIf func = "WeekdayName" Then
                ret = "Example: WeekdayName(Weekday(Fields!StartDate.Value,FirstDayOfWeek.System))"
            ElseIf func = "Year" Then
                ret = "Example: Year(Fields!StartDate.Value)"
                'aggregate
            ElseIf func = "Avg" Then
                ret = "Avg..."
            ElseIf func = "Count" Then
                ret = "Count..."
            ElseIf func = "CountDistinct" Then
                ret = "CountDistinct..."
            ElseIf func = "CountRows" Then
                ret = "CountRows..."
            ElseIf func = "First" Then
                ret = "First..."
            ElseIf func = "Last" Then
                ret = "Last..."
            ElseIf func = "Max" Then
                ret = "Max..."
            ElseIf func = "Min" Then
                ret = "Min..."
            ElseIf func = "StDev" Then
                ret = "StDev..."
            ElseIf func = "StDevP" Then
                ret = "StDevP..."
            ElseIf func = "Sum" Then
                ret = "Sum..."
            ElseIf func = "Var" Then
                ret = "Var..."
            ElseIf func = "VarP" Then
                ret = "VarP..."
            ElseIf func = "RunningValue" Then
                ret = "RunningValue..."
            ElseIf func = "Aggregate" Then
                ret = "Aggregate..."
                'financial
            ElseIf func = "DDB" Then
                ret = "Double-declining balance - Example: DDB(Fields!CostOfProperty.Value,Fields!SalvageValue.Value,Fields!UserfulLife.Value,Fields!DepreciationPeriod.Value)"
            ElseIf func = "FV" Then
                ret = "Future value of an annuity - Example: FV(Fields!PaymentRate.Value,Fields!NumberOfPayments.Value,Fields!PaymentAmount.Value)"
            ElseIf func = "IPmt" Then
                ret = "Interest payment for a given period - Example: IPmt(Fields!InterestRate.Value,Fields!PaymentPeriod.Value,Fields!NumberOfPayments.Value,Fields!PresentValue.Value)"
            ElseIf func = "NPer" Then
                ret = "The number of periods of an annuity based on fixed payments and a fixed interest rate - Example: NPer(Fields!Rate.Value,Fields!PaymentAmt.Value,Fields!PresentVal.Value,Fields!FutureVal.Value,DueDate.EndofPeriod)"
            ElseIf func = "Pmt" Then
                ret = "Calculates the payment for a loan based on fixed payments and fixed interest rate - Example: Pmt(Fields!Rate.Value,Fields!TotalPayments.Value,Fields!PresentVal.Value,Fields!FutureVal.Value,DueDate.EndOfPeriod)"
            ElseIf func = "PPmt" Then
                ret = "Calculates principal payment for a given period based on periodic constant payments and a constant interest rate - Example: PPmt(Fields!Rate.Value,Fields!Period.Value,Fields!NoOfPayments.Value,Fields!PresentVal.Value,Fields!FutureVal.Value,DueDate.EndOfPeriod)"
            ElseIf func = "PV" Then
                ret = "Calculates present value based on a constant interest rate - Example: PV(Fields!InterestRate.Value,Fields!NoOfPayments.Value,Fields!Payment.Value,Fields!FutureVal.Value,DueDate.EndOfOPeriod)"
            ElseIf func = "Rate" Then
                ret = "Calculates interest rate per period - Example: Rate(Fields!NoOfPayments.Value,Fields!PeriodPayment.Value,Fields!PresentValue,DueDate.EndOfPeriod)"
            ElseIf func = "SLN" Then
                ret = "Calculates straight-line depreciation for one period - Example: SLN(Fields!Cost.Value,Fields!Salvage.Value,Fields!UsefulLife.Value)"
            ElseIf func = "SYD" Then
                ret = "Calculate sum-of-years' digits depreciation for a specified period - Example: SYD(Fields!Cost.Value,Fields!Salvage.Value,Fields!UsefulLife.Value,Fields!Period.Value)"
                'Conversion
            ElseIf func = "Fix" Then
                ret = "Returns the integer portion of a number - Example: Fix(Fields!YearlyIncome.Value/-3)"
            ElseIf func = "Hex" Then
                ret = "Returns the hexadecimal value of a number as a string - Example: Hex(Fields!CellColor.Value)"
            ElseIf func = "Int" Then
                ret = "Returns the integer portion of a number - Example: Int(Fields!YearlyIncome.Value/12)"
            ElseIf func = "Oct" Then
                ret = "Returns the octal value of a number as a string - Example: Oct(Fields!BitString.Value)"
            ElseIf func = "Str" Then

                ret = "Returns a string representing a number - Example: Str(Fields!YearlyIncome.Value)"
            ElseIf func = "Val" Then
                ret = "Returns the numbers contained in a string as a numeric value of appropriate type - Example: Val(Fields!Address1.Value)"
            Else 'If func = " " Then
                ret = " "
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function CreateStatsTable() As DataTable
        Dim dtb As New DataTable
        Dim col As DataColumn
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "Friendly Name"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "Field"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Int32")
        col.ColumnName = "Count"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Int32")
        col.ColumnName = "Count Distinct"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "First Value"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "Last Value"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Int32")
        col.ColumnName = "Sum"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Int32")
        col.ColumnName = "Min"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Int32")
        col.ColumnName = "Max"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Double")
        col.ColumnName = "Average"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Double")
        col.ColumnName = "StDev"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "95% CI"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Double")
        col.ColumnName = "Var"
        dtb.Columns.Add(col)

        Return dtb
    End Function
    Public Function CreateFieldStatsCorrelationTable() As DataTable
        Dim dtb As New DataTable
        Dim col As DataColumn

        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "Field"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Int32")
        col.ColumnName = "Count"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Double")
        col.ColumnName = "Min"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Double")
        col.ColumnName = "Avg"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Double")
        col.ColumnName = "Sum"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Double")
        col.ColumnName = "StDev"
        dtb.Columns.Add(col)

        Return dtb
    End Function
    Public Function Create2FieldsCorrelationTable() As DataTable
        Dim dtb As New DataTable
        Dim col As DataColumn
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "Field1"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "Field2"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Int32")
        col.ColumnName = "Count1"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Double")
        col.ColumnName = "Sum1"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Double")
        col.ColumnName = "Avg1"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Int32")
        col.ColumnName = "Count2"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Double")
        col.ColumnName = "Sum2"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Double")
        col.ColumnName = "Avg2"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Double")
        col.ColumnName = "SumField1ByField2"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Double")
        col.ColumnName = "CorrelationCoefficient"
        dtb.Columns.Add(col)

        'col = New DataColumn
        'col.DataType = System.Type.GetType("System.String")
        'col.ColumnName = "Last Value"
        'dtb.Columns.Add(col)

        'col = New DataColumn
        'col.DataType = System.Type.GetType("System.Int32")
        'col.ColumnName = "Sum"
        'dtb.Columns.Add(col)

        'col = New DataColumn
        'col.DataType = System.Type.GetType("System.Int32")
        'col.ColumnName = "Min"
        'dtb.Columns.Add(col)

        'col = New DataColumn
        'col.DataType = System.Type.GetType("System.Int32")
        'col.ColumnName = "Max"
        'dtb.Columns.Add(col)

        'col = New DataColumn
        'col.DataType = System.Type.GetType("System.Double")
        'col.ColumnName = "Average"
        'dtb.Columns.Add(col)

        'col = New DataColumn
        'col.DataType = System.Type.GetType("System.Double")
        'col.ColumnName = "StDev"
        'dtb.Columns.Add(col)

        'col = New DataColumn
        'col.DataType = System.Type.GetType("System.String")
        'col.ColumnName = "95% CI"
        'dtb.Columns.Add(col)

        'col = New DataColumn
        'col.DataType = System.Type.GetType("System.Double")
        'col.ColumnName = "Var"
        'dtb.Columns.Add(col)

        Return dtb
    End Function

    'Public Function CalcStatsCorrelations(ByVal rep As String, ByVal dtt As DataTable, ByVal dtb As DataTable, Optional ByRef err As String = "") As DataTable
    '    Dim i, j, cnt As Integer
    '    Dim fldname As String
    '    Dim dt As DataTable
    '    Dim fldnames(0) As String
    '    Dim dv3t As DataTable = CleanDBNulls(dtt, err)
    '    Try

    '        'calculate count and distinct count for each field
    '        For i = 0 To dtb.Rows.Count - 1
    '            'record count
    '            cnt = dv3t.Rows.Count
    '            dtb.Rows(i)("Count") = cnt
    '            fldname = dtb.Rows(i)("Field").ToString

    '            Try
    '                fldnames(0) = fldname
    '                dt = dv3t.DefaultView.ToTable(1, fldnames)
    '                dtb.Rows(i)("Count Distinct") = dt.Compute("Count(" & fldname & ")", "")
    '            Catch ex As Exception
    '                err = err & " Error in Count Distinct compute for field " & fldname & " - " & ex.Message & ", "
    '            End Try
    '        Next

    '        ''add existing groups for this report
    '        'Dim dtg As DataTable = GetReportGroups(rep)
    '        'If Not dtg Is Nothing AndAlso dtg.Rows.Count > 0 Then
    '        '    For j = 0 To dtg.Rows.Count - 1
    '        '        Dim g As Boolean = False
    '        '        For i = 0 To dtb.Rows.Count - 1
    '        '            If dtg.Rows(j)("GroupField").ToString = dtb.Rows(i)("Field").ToString Then
    '        '                g = True
    '        '            End If
    '        '        Next
    '        '        If g = False Then 'the group is not in dtb
    '        '            Dim Row As DataRow = dtb.NewRow()
    '        '            Row("Field") = dtg.Rows(j)("GroupField").ToString
    '        '            Row("Count") = 100
    '        '            Row("Count Distinct") = 10
    '        '            dtb.Rows.Add(Row)
    '        '        End If
    '        '    Next
    '        'End If

    '    Catch ex As Exception
    '        err = ex.Message
    '    End Try
    '    Return dtb
    'End Function
    Public Function CalcCorrelations(dtt As DataTable, dtb As DataTable, Optional ByRef err As String = "") As DataTable
        Dim i, j, cnt As Integer
        Dim fldname As String
        Dim sm, mi As Integer
        Dim av, st As Double
        'Dim dt As DataTable
        Dim fldnames(0) As String
        Dim dv3t As DataTable = CleanDBNulls(dtt, err)
        Try
            'calculate statistics for each field
            For i = 0 To dtb.Rows.Count - 1
                'record count
                cnt = dv3t.Rows.Count
                dtb.Rows(i)("Count") = cnt
                fldname = dtb.Rows(i)("Field").ToString

                If ColumnTypeIsNumeric(dv3t.Columns(fldname)) Then

                    Try
                        mi = dv3t.Compute("MIN(" & fldname & ")", "")
                        dtb.Rows(i)("Min") = mi
                    Catch ex As Exception
                        err = err & " Error in MIN compute for field " & fldname & " - " & ex.Message & ", "
                    End Try

                    Try
                        av = dv3t.Compute("AVG(" & fldname & ")", "")
                        av = FormatNumber(av, 2)
                        dtb.Rows(i)("Avg") = av
                    Catch ex As Exception
                        err = err & " Error in AVG compute for field " & fldname & " - " & ex.Message & ", "
                    End Try

                    Try
                        sm = dv3t.Compute("SUM(" & fldname & ")", "")
                        dtb.Rows(i)("Sum") = sm
                    Catch ex As Exception
                        err = err & " Error in SUM compute for field " & fldname & " - " & ex.Message & ", "
                    End Try

                    Try
                        st = dv3t.Compute("StDev(" & fldname & ")", "")
                        st = FormatNumber(st, 2)
                        dtb.Rows(i)("StDev") = st
                    Catch ex As Exception
                        err = err & " Error in StDev compute for field " & fldname & " - " & ex.Message & ", "
                    End Try

                End If

                'Try
                '    fldnames(0) = fldname
                '    dt = dv3t.DefaultView.ToTable(1, fldnames)
                '    dtb.Rows(i)("Count Distinct") = dt.Compute("Count(" & fldname & ")", "")
                'Catch ex As Exception
                '    err = err & " Error in Count Distinct compute for field " & fldname & " - " & ex.Message & ", "
                'End Try

            Next
        Catch ex As Exception
            err = ex.Message
        End Try
        Return dtb
    End Function
    Public Function ExtractParameters(ByVal spname As String, ByVal wheretext As String, Optional ByRef Nparameters As Integer = -1, Optional ByRef ParamName As Array = Nothing, Optional ByRef ParamType As Array = Nothing, Optional ByRef ParamValue As Array = Nothing, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim i, j As Integer
        Dim temp As String = String.Empty
        Try
            ' get list of parameters for sp
            Dim ParamList As ListItemCollection = New ListItemCollection
            Dim params As ListItemCollection = New ListItemCollection
            ParamList = GetListOfStoredProcedureParameters(spname, userconstr, userconprv)
            Nparameters = 0
            For i = 0 To ParamList.Count - 1
                If ParamList(i).Text.Trim <> "" Then
                    params.Add(New ListItem(ParamList(i).Text, ParamList(i).Value))
                    Nparameters += 1
                End If
            Next
            If Nparameters = 0 Then
                Return ret
            End If
            'get parameters' values from wheretext
            Dim ParamNames(Nparameters - 1) As String
            Dim ParamTypes(Nparameters - 1) As String
            Dim ParamValues(Nparameters - 1) As String
            For i = 0 To Nparameters - 1
                ParamNames(i) = params(i).Text.Trim
                ParamTypes(i) = params(i).Value
                'extract value for params(i)
                If Not wheretext.Contains(ParamNames(i)) OrElse ParamNames(i) = "" Then
                    ParamValues(i) = ""
                    Continue For
                End If
                temp = wheretext
                While temp.IndexOf(ParamNames(i)) >= 0
                    j = temp.IndexOf(ParamNames(i))
                    If temp.Substring(j + ParamNames(i).Length).Trim.StartsWith("=") Then
                        temp = temp.Substring(j + ParamNames(i).Length).Trim
                        Exit While
                    End If
                    temp = temp.Substring(j + ParamNames(i).Length).Trim
                End While
                If temp.Trim <> "" AndAlso temp.Trim.StartsWith("=") Then
                    temp = temp.Substring(1)
                    If temp.ToUpper.IndexOf(" AND ") > -1 Then temp = temp.Substring(0, temp.ToUpper.IndexOf(" AND "))
                    If temp.ToUpper.IndexOf(") ") > -1 Then temp = temp.Substring(0, temp.ToUpper.IndexOf(") "))
                End If
                ParamValues(i) = temp
            Next
            ParamName = ParamNames
            ParamType = ParamTypes
            ParamValue = ParamValues
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Public Function RetrieveReportData(ByVal repid As String, ByVal wheretext As String, ByVal hidedupl As Boolean, Optional ByVal Nparameters As Integer = -1, Optional ByVal ParamName As Array = Nothing, Optional ByVal ParamType As Array = Nothing, Optional ByVal ParamValue As Array = Nothing, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional ByRef err As String = "", Optional ByRef sqlWhere As String = "", Optional ByRef sqlsforexport As String = "") As DataView
        'retrieve data for report
        Dim dv3n As DataView
        Dim ret As String = String.Empty
        Dim mSql As String = String.Empty
        Dim dri As DataTable = GetReportInfo(repid)
        If dri Is Nothing OrElse dri.Rows.Count = 0 Then
            err = "Report is not found"
            Return Nothing
        End If
        'constract filter
        If UCase(dri.Rows(0)("ReportAttributes").ToString) = "SQL" Then  'SQL statement
            mSql = dri.Rows(0)("SQLquerytext").ToString
            'find WHERE, ORDER BY, and GROUP BY: add to WHERE or place WHERE in front of GROUP, and place ORDER BY in the end of SQL
            Dim wherenum, ordernum, groupnum As Integer
            Dim dirsort As String = String.Empty
            wherenum = 0
            ordernum = 0
            groupnum = 0
            wherenum = InStr(UCase(mSql), " WHERE ")
            ordernum = InStr(UCase(mSql), " ORDER BY ")
            groupnum = InStr(UCase(mSql), " GROUP BY ")
            If ordernum > 0 Then
                dirsort = mSql.Substring(ordernum)
            End If
            If wherenum > 0 Then        'mSql has " WHERE "
                sqlWhere = mSql.Substring(wherenum + 6, Len(mSql) - wherenum - 6)
                sqlWhere = sqlWhere.Replace("""", "'")
                If userconprv.ToString.StartsWith("InterSystems.Data.") Then
                    Dim parts = sqlWhere.Split(" "c)
                    Dim sWhere As String = String.Empty
                    For p As Integer = 0 To parts.Length - 1
                        If parts(p).Contains(".") AndAlso Not parts(p).Contains("'%'") Then
                            parts(p) = parts(p).Replace("'", """")
                        End If
                        If sWhere = String.Empty Then
                            sWhere = parts(p)
                        Else
                            sWhere &= " " & parts(p)
                        End If
                    Next
                    sqlWhere = sWhere
                End If
                If Trim(wheretext) <> "" Then
                    mSql = mSql.Substring(0, wherenum + 6) & wheretext & " AND " & sqlWhere
                Else
                    mSql = mSql.Substring(0, wherenum + 6) & sqlWhere
                End If

            Else    'mSql does not have " WHERE "
                If groupnum > 0 Then       'mSql has " GROUP BY " and mSql does not have " WHERE "
                    If Trim(wheretext) <> "" Then mSql = mSql.Substring(0, groupnum - 1) & " WHERE " & wheretext & " " & mSql.Substring(groupnum, Len(mSql) - groupnum)
                Else                       'mSql does not have " GROUP BY " and mSql does not have " WHERE "
                    If ordernum > 0 Then      'mSql has " ORDER BY " and mSql does not have " WHERE "
                        If Trim(wheretext) <> "" Then mSql = mSql.Substring(0, ordernum - 1) & " WHERE " & wheretext & " " & mSql.Substring(ordernum, Len(mSql) - ordernum)
                    Else                      'mSql does not have " ORDER BY " and mSql does not have " WHERE "
                        'if no GROUP and no ORDER
                        If Trim(wheretext) <> "" Then mSql = mSql & " WHERE " & wheretext
                        'ORDER BY in the end always
                        If Trim(dirsort) <> "" Then mSql = mSql & " ORDER BY " & dirsort
                    End If
                End If
            End If
            'run sql in user database !!! Correct mSql for user database provider:
            mSql = ConvertSQLSyntaxFromOURdbToUserDB(mSql, userconprv, err)
            sqlsforexport = mSql
            err = ""
            'Data for report by SQL statement from the user database
            dv3n = mRecords(mSql, err, userconstr, userconprv)

            'Stored procedure  --------------------------------------------------------------
        Else 'sp                                                  
            mSql = Trim(dri.Rows(0)("SQLquerytext"))
            '!!!!!!!!!!!!!!!!!!!!!  Should be this one:
            If userconprv.StartsWith("InterSystems.Data.") AndAlso mSql.Contains("||") Then
                Dim cls As String = Piece(mSql, "||", 1)
                Dim sp As String = Piece(mSql, "||", 2)
                cls = Piece(cls, ".", 1, Pieces(cls, ".") - 1).Replace(".", "_")
                mSql = cls & "." & sp
            End If
            If Nparameters = -1 Then
                ret = ExtractParameters(mSql, wheretext, Nparameters, ParamName, ParamType, ParamValue, userconstr, userconprv)
            End If
            dv3n = mRecordsFromSP(mSql, Nparameters, ParamName, ParamType, ParamValue, userconstr, userconprv)  'Data for report from SP
        End If
        Dim userdb As String = userconstr
        If userconprv <> "Oracle.ManagedDataAccess.Client" Then
            If userdb.ToUpper.IndexOf("USER ID") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("USER ID")).Trim
        Else
            If userdb.ToUpper.IndexOf("PASSWORD") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("PASSWORD")).Trim
        End If
        If dv3n Is Nothing OrElse dv3n.Table Is Nothing OrElse err <> "" Then
            ret = ExequteSQLquery("UPDATE OurReportInfo SET Param1type='1' WHERE ReportID='" & repid & "' AND ReportDB LIKE '%" & userdb.Trim.Replace(" ", "%") & "%'")
            Return dv3n
        Else
            ret = ExequteSQLquery("UPDATE OurReportInfo SET Param1type='0' WHERE ReportID='" & repid & "' AND ReportDB LIKE '%" & userdb.Trim.Replace(" ", "%") & "%'")
        End If
        Dim er As String = String.Empty
        If userconprv = "MySql.Data.MySqlClient" Then
            dv3n = ConvertMySqlTable(dv3n.Table, er).DefaultView
        ElseIf userconprv = "Oracle.ManagedDataAccess.Client" Then
            dv3n = ConvertOracleTable(dv3n.Table, er).DefaultView
            'dv3n = CorrectDataset(dv3n.Table, er).DefaultView

            'TODO maybe not needed for PostgreSQL:   ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
        Else
            dv3n = CorrectDatasetColumns(dv3n.Table, er).DefaultView  'from single quote
        End If
        If hidedupl AndAlso Not dv3n Is Nothing AndAlso dv3n.Count > 0 AndAlso dv3n.Count < 10000 Then
            dv3n = dv3n.ToTable(True).DefaultView
        End If
        Dim dts As New DataTable
        dts = dv3n.Table
        dts = MakeDTColumnsNamesCLScompliant(dts, userconprv, ret)
        dv3n = dts.DefaultView
        Return dv3n
    End Function
    Public Function MakeMultidimesionalTables(ByVal dtt As DataTable, ByVal fn As String, ByVal y1 As String, ByVal fltr As String, ByVal dims As String, Optional ByRef er As String = "", Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "") As DataTable
        Dim ret As String = String.Empty
        Dim flt As String = String.Empty
        Dim srt As String = String.Empty 'x1 & " ASC," & x2 & " ASC"
        Dim flds As String()
        flds = Split(dims, ",")
        Dim n As Integer = flds.Length
        Dim val As Double = 0
        Dim i, j, k As Integer
        'Dim fldnames(0) As String
        Dim dtr As New DataTable
        'create table dtr 
        Dim col As DataColumn
        For j = 0 To n - 1
            col = New DataColumn
            col.DataType = System.Type.GetType("System.String")
            col.ColumnName = flds(j)
            dtr.Columns.Add(col)
        Next
        col = New DataColumn
        col.DataType = System.Type.GetType("System.Double")
        col.ColumnName = "Value"
        dtr.Columns.Add(col)
        Try
            Dim dvt As DataTable = dtt 'CleanDBNullsFields(dtt, y1, flds(0), "", er, True)
            For j = 0 To n - 1
                If j = 0 Then
                    srt = flds(j) & " ASC"
                Else
                    srt = srt & "," & flds(j) & " ASC"
                End If

                dvt = CleanDBNullsFields(dvt, y1, flds(j), "", er, True)
            Next

            'dv3 and dvt are already filtered
            'TODO check for search and etc...
            'If fltr.Trim <> "" Then
            '    dvt.DefaultView.RowFilter = fltr
            'End If
            Dim dvr As DataTable = dvt.DefaultView.ToTable(True, flds)

            For i = 0 To dvr.Rows.Count - 1
                Dim Row As DataRow = dtr.NewRow()
                For j = 0 To n - 1
                    Row(flds(j)) = dvr.Rows(i)(flds(j)).ToString
                    If j = 0 Then
                        flt = flds(j) & "='" & dvr.Rows(i)(flds(j)).ToString & "'"
                    Else
                        flt = flt & " AND " & flds(j) & "='" & dvr.Rows(i)(flds(j)).ToString & "'"
                    End If
                Next
                If fltr.Trim <> "" Then
                    flt = flt & " AND " & fltr
                End If
                val = 0
                Try
                    If fn.ToUpper.Trim = "DISTCOUNT" Then
                        Dim ddt As DataTable = dvt.DefaultView.ToTable(True, flds)
                        Row("Value") = ddt.Compute("Count([" & y1 & "])", flt).ToString
                    ElseIf fn.ToUpper.Trim <> "VALUE" Then
                        If IsDBNull(dvt.Compute(fn & "(" & y1 & ")", flt)) Then
                            val = 0
                        Else
                            val = dvt.Compute(fn & "(" & y1 & ")", flt)
                        End If
                        If fn.ToUpper.Trim = "COUNT" Then
                            val = FormatNumber(val, 0)
                        Else
                            val = FormatNumber(val, 2)
                        End If
                        Row("Value") = val.ToString
                    Else
                        Row("Value") = dvt.Rows(i)(y1).ToString
                    End If
                    dtr.Rows.Add(Row)
                Catch ex As Exception
                    er = "ERROR!! " & ex.Message & ", "
                    Continue For
                End Try
            Next
            dtr.DefaultView.Sort = srt
            dtr = dtr.DefaultView.ToTable
            Return dtr
        Catch ex As Exception
            ret = ex.Message
            Return Nothing
        End Try
    End Function
    Public Function ComputeStats(ByVal dtt As DataTable, ByVal fn As String, ByVal y1 As String, ByVal x1 As String, ByVal x2 As String, ByVal fltr As String, Optional ByRef er As String = "", Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional doRowFilterFirst As Boolean = True) As DataTable
        Dim ret As String = String.Empty
        Dim x1v As String = String.Empty
        Dim x2v As String = String.Empty
        Dim flt As String = String.Empty
        Dim val As Double = 0
        Dim i As Integer

        Dim dtr As New DataTable
        'create table dtr 
        Dim col As DataColumn
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = x1
        dtr.Columns.Add(col)
        If x1 <> x2 Then
            col = New DataColumn
            col.DataType = System.Type.GetType("System.String")
            col.ColumnName = x2
            dtr.Columns.Add(col)
        End If
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "ARR"
        dtr.Columns.Add(col)
        Try
            Dim dvt As DataTable = CleanDBNullsFields(dtt, y1, x1, x2, er, True)
            Dim fldnames(0) As String
            fldnames(0) = x1
            If x1 <> x2 Then
                ReDim Preserve fldnames(1)
                fldnames(1) = x2
            End If
            'dv3 and dvt are already filtered
            ' check for search and etc...
            If doRowFilterFirst AndAlso fltr.Trim <> "" Then
                dvt.DefaultView.RowFilter = fltr
            End If
            Dim dvr As DataTable = dvt.DefaultView.ToTable(True, fldnames)

            For i = 0 To dvr.Rows.Count - 1
                Dim Row As DataRow = dtr.NewRow()
                Row(x1) = dvr.Rows(i)(x1).ToString
                Row(x2) = dvr.Rows(i)(x2).ToString
                x1v = dvr.Rows(i)(x1).ToString
                x2v = dvr.Rows(i)(x2).ToString
                flt = x1 & "='" & x1v & "' AND " & x2 & "='" & x2v & "'"
                If doRowFilterFirst = False AndAlso fltr.Trim <> "" Then
                    flt = flt & " AND " & fltr
                End If
                val = 0
                Try
                    If fn.ToUpper.Trim = "DISTCOUNT" OrElse fn.ToUpper.Trim = "COUNTDISTINCT" Then
                        If x1 = x2 Then
                            ReDim Preserve fldnames(1)
                            fldnames(1) = y1
                        Else
                            ReDim Preserve fldnames(2)
                            fldnames(2) = y1
                        End If
                        Dim ddt As DataTable = dvt.DefaultView.ToTable(True, fldnames)
                        Dim fldc As String = "Count(" & y1 & ")"
                        Row("ARR") = ddt.Compute(fldc, flt).ToString
                    ElseIf fn.ToUpper.Trim <> "VALUE" Then
                        If IsDBNull(dvt.Compute(fn & "(" & y1 & ")", flt)) Then
                            val = 0
                        Else
                            val = dvt.Compute(fn & "(" & y1 & ")", flt)
                        End If
                        If fn.ToUpper.Trim = "COUNT" Then
                            val = FormatNumber(val, 0)
                        Else
                            val = FormatNumber(val, 2)
                        End If
                        Row("ARR") = val.ToString
                    Else
                        Row("ARR") = dvt.Rows(i)(y1).ToString
                    End If
                    dtr.Rows.Add(Row)
                Catch ex As Exception
                    er = "ERROR!! " & ex.Message & ", "
                    Continue For
                End Try
            Next
            Return dtr
        Catch ex As Exception
            ret = ex.Message
            Return Nothing
        End Try
    End Function
    Public Function ComputeStatsM(ByVal dtt As DataTable, ByVal fn As String, ByVal ysel() As String, ByVal xsel() As String, ByVal fltr As String, Optional ByRef er As String = "", Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional doRowFilterFirst As Boolean = True) As DataTable
        Dim ret As String = String.Empty
        Dim flt As String = String.Empty
        Dim val As Double = 0
        Dim i As Integer

        Dim dtr As New DataTable
        'create table dtr 
        Dim col As DataColumn
        For i = 0 To xsel.Length - 1
            col = New DataColumn
            col.DataType = System.Type.GetType("System.String")
            col.ColumnName = xsel(i)
            dtr.Columns.Add(col)
        Next
        For i = 0 To ysel.Length - 1
            col = New DataColumn
            col.DataType = System.Type.GetType("System.String")
            col.ColumnName = "ARR" & ysel(i)
            dtr.Columns.Add(col)
        Next

        Try
            Dim dvt As DataTable = CleanDBNullsFieldsM(dtt, ysel, xsel, er, True)
            'dv3 and dvt are already filtered
            'check for search and etc...
            If doRowFilterFirst AndAlso fltr.Trim <> "" Then
                dvt.DefaultView.RowFilter = fltr
            End If
            Dim dvr As DataTable = dvt.DefaultView.ToTable(True, xsel)
            flt = ""
            For i = 0 To dvr.Rows.Count - 1
                Dim Row As DataRow = dtr.NewRow()
                'filter for each calculation
                For j = 0 To xsel.Length - 1
                    Row(xsel(j)) = dvr.Rows(i)(xsel(j)).ToString
                    If j = 0 Then
                        flt = xsel(j) & "='" & dvr.Rows(i)(xsel(j)).ToString & "'"
                    Else
                        flt = flt & " AND " & xsel(j) & "='" & dvr.Rows(i)(xsel(j)).ToString & "'"
                    End If
                Next
                If Not doRowFilterFirst AndAlso fltr.Trim <> "" Then
                    flt = flt & " AND " & fltr
                End If
                val = 0
                Try
                    Dim fldnames(0) As String
                    Dim n As Integer = 0
                    For j = 0 To ysel.Length - 1
                        If fn.ToUpper.Trim = "DISTCOUNT" OrElse fn.ToUpper.Trim = "COUNTDISTINCT" Then
                            fldnames(0) = ysel(j)
                            For n = 0 To xsel.Length - 1
                                ReDim Preserve fldnames(n)
                                fldnames(n) = xsel(n)
                            Next
                            ReDim Preserve fldnames(n)
                            fldnames(n) = ysel(j)
                            Dim ddt As DataTable = dvt.DefaultView.ToTable(True, fldnames)
                            Dim fldc As String = "Count(" & ysel(j) & ")"
                            Row("ARR" & ysel(j)) = FormatNumber(ddt.Compute(fldc, flt), 0).ToString
                        ElseIf fn.ToUpper.Trim <> "VALUE" Then
                            If IsDBNull(dvt.Compute(fn & "(" & ysel(j) & ")", flt)) Then
                                val = 0
                            Else
                                val = dvt.Compute(fn & "(" & ysel(j) & ")", flt)
                            End If
                            If fn.ToUpper.Trim = "COUNT" Then
                                val = FormatNumber(val, 0)
                            Else
                                val = FormatNumber(val, 2)
                            End If
                            Row("ARR" & ysel(j)) = val.ToString
                        Else
                            Row("ARR" & ysel(j)) = dvt.Rows(i)(ysel(j)).ToString
                        End If

                    Next
                    dtr.Rows.Add(Row)
                Catch ex As Exception
                    er = "ERROR!! " & ex.Message & ", "
                    Continue For
                End Try
            Next
            Return dtr
        Catch ex As Exception
            ret = er & " " & ex.Message
            Return Nothing
        End Try
    End Function
    Public Function ComputeStatsV(ByVal dtt As DataTable, ByVal fn As String, ByVal ysel() As String, ByVal xsel() As String, ByVal mfld As String, ByVal vsel() As String, ByVal fltr As String, Optional ByRef er As String = "", Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional doRowFilterFirst As Boolean = True) As DataTable
        Dim ret As String = String.Empty
        Dim flt As String = String.Empty
        Dim fltv As String = String.Empty
        Dim val As Double = 0
        Dim i As Integer

        Dim dtr As New DataTable
        'create table dtr 
        Dim col As DataColumn
        For i = 0 To xsel.Length - 1
            col = New DataColumn
            col.DataType = System.Type.GetType("System.String")
            col.ColumnName = xsel(i)
            dtr.Columns.Add(col)
        Next
        For i = 0 To vsel.Length - 1
            col = New DataColumn
            col.DataType = System.Type.GetType("System.String")
            col.ColumnName = "ARR" & vsel(i).ToString
            dtr.Columns.Add(col)
        Next

        Try
            Dim dvt As DataTable = CleanDBNullsFieldsM(dtt, ysel, xsel, er, True)
            'dv3 and dvt are already filtered
            'check for search and etc...
            If doRowFilterFirst AndAlso fltr.Trim <> "" Then
                dvt.DefaultView.RowFilter = fltr
            End If
            Dim dvr As DataTable = dvt.DefaultView.ToTable(True, xsel)
            flt = ""
            For i = 0 To dvr.Rows.Count - 1
                Dim Row As DataRow = dtr.NewRow()
                'filter for each calculation
                For j = 0 To xsel.Length - 1
                    Row(xsel(j)) = dvr.Rows(i)(xsel(j)).ToString

                    If ColumnTypeIsNumeric(dvt.Columns(xsel(j))) Then
                        If j = 0 Then
                            flt = xsel(j) & "=" & dvr.Rows(i)(xsel(j)).ToString
                        Else
                            flt = flt & " AND " & xsel(j) & "='" & dvr.Rows(i)(xsel(j)).ToString & "'"
                        End If
                    Else
                        If j = 0 Then
                            flt = xsel(j) & "='" & dvr.Rows(i)(xsel(j)).ToString & "'"
                        Else
                            flt = flt & " AND " & xsel(j) & "='" & dvr.Rows(i)(xsel(j)).ToString & "'"
                        End If
                    End If

                Next
                If Not doRowFilterFirst AndAlso fltr.Trim <> "" Then
                    flt = flt & " AND " & fltr
                End If
                val = 0
                Try
                    Dim fldnames(0) As String
                    fldnames(0) = ysel(0)
                    For j = 0 To vsel.Length - 1
                        If ColumnTypeIsNumeric(dvt.Columns(mfld)) Then
                            fltv = mfld & "=" & vsel(j).ToString
                        Else
                            fltv = mfld & "='" & vsel(j).ToString.Trim & "'"
                        End If
                        fltv = fltv & " AND " & flt

                        If fn.ToUpper.Trim = "DISTCOUNT" OrElse fn.ToUpper.Trim = "COUNTDISTINCT" Then
                            Dim n As Integer = 0
                            For n = 0 To xsel.Length - 1
                                ReDim Preserve fldnames(n)
                                fldnames(n) = xsel(n)
                            Next
                            ReDim Preserve fldnames(n)
                            fldnames(n) = ysel(0)
                            Dim ddt As DataTable = dvt.DefaultView.ToTable(True, fldnames)
                            Dim fldc As String = "Count(" & ysel(0) & ")"
                            Row("ARR" & vsel(j)) = FormatNumber(ddt.Compute(fldc, flt), 0).ToString
                        ElseIf fn.ToUpper.Trim = "VALUE" Then
                            'wrong
                            dvt.DefaultView.RowFilter = fltv
                            For n = 0 To xsel.Length - 1
                                ReDim Preserve fldnames(n)
                                fldnames(n) = xsel(n)
                            Next
                            ReDim Preserve fldnames(xsel.Length)
                            fldnames(xsel.Length) = ysel(0)

                            Dim ddt As DataTable = dvt.DefaultView.ToTable(True, fldnames)
                            Dim fldc As String = "Count(" & ysel(0) & ")"
                            If FormatNumber(ddt.Compute(fldc, flt), 0) > 1 Then
                                If IsDBNull(dvt.Compute("Avg(" & ysel(0) & ")", fltv)) Then
                                    Row("ARR" & vsel(j)) = 0
                                Else
                                    Row("ARR" & vsel(j)) = dvt.Compute("Avg(" & ysel(0) & ")", fltv)
                                End If
                                er = "Average return"
                                fn = "Avg"
                            Else
                                If ddt.Rows.Count > 0 Then
                                    Row("ARR" & vsel(j)) = ddt.Rows(0)(ysel(0)).ToString
                                Else
                                    Row("ARR" & vsel(j)) = 0
                                End If
                            End If

                        ElseIf fn.ToUpper.Trim <> "VALUE" Then
                            If IsDBNull(dvt.Compute(fn & "(" & ysel(0) & ")", fltv)) Then
                                val = 0
                            Else
                                val = dvt.Compute(fn & "(" & ysel(0) & ")", fltv)
                            End If
                            If fn.ToUpper.Trim = "COUNT" Then
                                val = FormatNumber(val, 0)
                            Else
                                val = FormatNumber(val, 2)
                            End If
                            Row("ARR" & vsel(j)) = val.ToString

                        End If

                    Next
                    dtr.Rows.Add(Row)
                Catch ex As Exception
                    er = "ERROR!! " & ex.Message & ", "
                    Continue For
                End Try
            Next
            dvt.DefaultView.RowFilter = ""
            Return dtr
        Catch ex As Exception
            ret = er & " " & ex.Message
            Return Nothing
        End Try
    End Function
    Public Function CalcStats(dtt As DataTable, dtb As DataTable, Optional ByRef err As String = "") As DataTable
        Dim i, cnt As Integer
        Dim fldname As String
        Dim sm, mi, ma As Integer
        Dim av, st, vr As Double
        Dim dt As DataTable
        Dim fldnames(0) As String
        Dim dv3t As DataTable = CleanDBNulls(dtt, err)
        Try
            'calculate statistics for each field
            For i = 0 To dtb.Rows.Count - 1
                'record count
                cnt = dv3t.Rows.Count
                dtb.Rows(i)("Count") = cnt
                fldname = dtb.Rows(i)("Field").ToString
                If ColumnTypeIsNumeric(dv3t.Columns(fldname)) Then
                    Try
                        sm = dv3t.Compute("SUM(" & fldname & ")", "")
                        dtb.Rows(i)("Sum") = sm
                    Catch ex As Exception
                        err = err & " Error in SUM compute for field " & fldname & " - " & ex.Message & ", "
                    End Try

                    Try
                        mi = dv3t.Compute("MIN(" & fldname & ")", "")
                        dtb.Rows(i)("Min") = mi
                    Catch ex As Exception
                        err = err & " Error in MIN compute for field " & fldname & " - " & ex.Message & ", "
                    End Try

                    Try
                        ma = dv3t.Compute("MAX(" & fldname & ")", "")
                        dtb.Rows(i)("Max") = ma
                    Catch ex As Exception
                        err = err & " Error in MAX compute for field " & fldname & " - " & ex.Message & ", "
                    End Try

                    Try
                        av = dv3t.Compute("AVG(" & fldname & ")", "")
                        av = FormatNumber(av, 2) ', , , TriState.True)
                        dtb.Rows(i)("Average") = av
                    Catch ex As Exception
                        err = err & " Error in AVG compute for field " & fldname & " - " & ex.Message & ", "
                    End Try

                    Try
                        st = dv3t.Compute("StDev(" & fldname & ")", "")
                        st = FormatNumber(st, 2)
                        dtb.Rows(i)("StDev") = st
                    Catch ex As Exception
                        err = err & " Error in StDev compute for field " & fldname & " - " & ex.Message & ", "
                    End Try

                    Try
                        vr = 1.96 * st / Sqrt(cnt)
                        vr = FormatNumber(vr, 2)
                        dtb.Rows(i)("95% CI") = av.ToString & " +- " & vr.ToString
                    Catch ex As Exception
                        err = err & " Error in CI compute for field " & fldname & " - " & ex.Message & ", "
                    End Try

                    Try
                        vr = dv3t.Compute("VAR(" & fldname & ")", "")
                        vr = FormatNumber(vr, 2)
                        dtb.Rows(i)("Var") = vr
                    Catch ex As Exception
                        err = err & " Error in VAR compute for field " & fldname & " - " & ex.Message & ", "
                    End Try

                End If
                Try
                    If ColumnTypeIsDateTime(dv3t.Columns(fldname)) AndAlso dv3t.Rows(0)(fldname) = DateTime.MinValue Then
                        dtb.Rows(i)("First Value") = ""
                    ElseIf ColumnTypeIsNumeric(dv3t.Columns(fldname)) OrElse ColumnTypeIsString(dv3t.Columns(fldname)) Then
                        dtb.Rows(i)("First Value") = dv3t.Rows(0)(fldname).ToString
                    End If
                Catch ex As Exception
                    err = err & " Error in First Value for field " & fldname & " - " & ex.Message & ", "
                End Try

                Try
                    If ColumnTypeIsDateTime(dv3t.Columns(fldname)) AndAlso dv3t.Rows(dv3t.Rows.Count - 1)(fldname) = DateTime.MinValue Then
                        dtb.Rows(i)("Last Value") = ""
                    ElseIf ColumnTypeIsNumeric(dv3t.Columns(fldname)) OrElse ColumnTypeIsString(dv3t.Columns(fldname)) Then
                        dtb.Rows(i)("Last Value") = dv3t.Rows(dv3t.Rows.Count - 1)(fldname).ToString
                    End If
                Catch ex As Exception
                    err = err & " Error in Last Value for field " & fldname & " - " & ex.Message & ", "
                End Try

                Try
                    fldnames(0) = fldname
                    dt = dv3t.DefaultView.ToTable(1, fldnames)
                    dtb.Rows(i)("Count Distinct") = dt.Compute("Count(" & fldname & ")", "")
                Catch ex As Exception
                    err = err & " Error in Count Distinct compute for field " & fldname & " - " & ex.Message & ", "
                End Try

            Next
        Catch ex As Exception
            err = ex.Message
        End Try
        Return dtb
    End Function
    Public Function CreateStatsAnalyticsTable() As DataTable
        Dim dtb As New DataTable
        Dim col As DataColumn

        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "Field"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Int32")
        col.ColumnName = "Count"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Int32")
        col.ColumnName = "Count Distinct"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "comment"
        dtb.Columns.Add(col)

        Return dtb
    End Function
    Public Function CreateAnalyticsTable() As DataTable
        Dim dtb As New DataTable
        Dim col As DataColumn
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "Category1"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "Category2"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Int32")
        col.ColumnName = "Count1"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Int32")
        col.ColumnName = "CountDistinct1"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Int32")
        col.ColumnName = "Count2"
        dtb.Columns.Add(col)

        col = New DataColumn
        col.DataType = System.Type.GetType("System.Int32")
        col.ColumnName = "CountDistinct2"
        dtb.Columns.Add(col)

        'col = New DataColumn
        'col.DataType = System.Type.GetType("System.String")
        'col.ColumnName = "First Value"
        'dtb.Columns.Add(col)

        'col = New DataColumn
        'col.DataType = System.Type.GetType("System.String")
        'col.ColumnName = "Last Value"
        'dtb.Columns.Add(col)

        'col = New DataColumn
        'col.DataType = System.Type.GetType("System.Int32")
        'col.ColumnName = "Sum"
        'dtb.Columns.Add(col)

        'col = New DataColumn
        'col.DataType = System.Type.GetType("System.Int32")
        'col.ColumnName = "Min"
        'dtb.Columns.Add(col)

        'col = New DataColumn
        'col.DataType = System.Type.GetType("System.Int32")
        'col.ColumnName = "Max"
        'dtb.Columns.Add(col)

        'col = New DataColumn
        'col.DataType = System.Type.GetType("System.Double")
        'col.ColumnName = "Average"
        'dtb.Columns.Add(col)

        'col = New DataColumn
        'col.DataType = System.Type.GetType("System.Double")
        'col.ColumnName = "StDev"
        'dtb.Columns.Add(col)

        'col = New DataColumn
        'col.DataType = System.Type.GetType("System.String")
        'col.ColumnName = "95% CI"
        'dtb.Columns.Add(col)

        'col = New DataColumn
        'col.DataType = System.Type.GetType("System.Double")
        'col.ColumnName = "Var"
        'dtb.Columns.Add(col)

        Return dtb
    End Function
    Public Function CalcStatsAnalytics(ByVal rep As String, ByVal dtt As DataTable, ByVal dtb As DataTable, Optional ByRef err As String = "") As DataTable
        Dim i, j, cnt As Integer
        Dim fldname As String
        Dim dt As DataTable
        Dim fldnames(0) As String
        Dim dv3t As DataTable = CleanDBNulls(dtt, err)
        Try

            'calculate count and distinct count for each field
            For i = 0 To dtb.Rows.Count - 1
                'record count
                cnt = dv3t.Rows.Count
                dtb.Rows(i)("Count") = cnt
                fldname = dtb.Rows(i)("Field").ToString
                Try
                    'fldnames(0) = FixReservedWords(fldname)
                    fldnames(0) = fldname
                    dt = dv3t.DefaultView.ToTable(1, fldnames)
                    dtb.Rows(i)("Count Distinct") = dt.Compute("Count([" & fldname & "])", "")
                Catch ex As Exception
                    err = err & " Error in Count Distinct compute for field " & fldname & " - " & ex.Message & ", "
                    dtb.Rows(i)("Count Distinct") = cnt
                End Try
            Next

            'add existing groups for this report
            Dim dtg As DataTable = GetReportGroups(rep)
            If Not dtg Is Nothing AndAlso dtg.Rows.Count > 0 Then
                For j = 0 To dtg.Rows.Count - 1
                    Dim g As Boolean = False
                    For i = 0 To dtb.Rows.Count - 1
                        If dtg.Rows(j)("GroupField").ToString = dtb.Rows(i)("Field").ToString Then
                            g = True
                        End If
                    Next
                    If g = False AndAlso dtg.Rows(j)("GroupField").ToString <> "Overall" Then 'the group is not in dtb
                        Dim Row As DataRow = dtb.NewRow()
                        Row("Field") = dtg.Rows(j)("GroupField").ToString
                        Row("Count") = 100  'to be sure they included
                        Row("Count Distinct") = 10
                        Row("comment") = "repgroup"
                        dtb.Rows.Add(Row)
                    End If
                Next
            End If

        Catch ex As Exception
            err = ex.Message
        End Try
        Return dtb
    End Function
    Public Function CleanDBNullsFields(ByRef dv3t As DataTable, ByVal y1 As String, ByVal x1 As String, ByVal x2 As String, Optional ByRef err As String = "", Optional ByVal bnum As Boolean = False) As DataTable
        'Dim coltype As String = ""
        Dim i, j As Integer
        Try
            For i = 0 To dv3t.Rows.Count - 1
                For j = 0 To dv3t.Columns.Count - 1
                    If (dv3t.Columns(j).Caption.Trim = x1 OrElse dv3t.Columns(j).Caption.Trim = x2) Then
                        bnum = False
                    ElseIf dv3t.Columns(j).Caption.Trim = y1 Then
                        bnum = True
                    Else
                        Continue For
                    End If
                    If IsDBNull(dv3t.Rows(i)(j)) Then
                        If bnum = True Then
                            If ColumnTypeIsNumeric(dv3t.Columns(j)) Then
                                dv3t.Rows(i)(j) = 0
                            ElseIf ColumnTypeIsDateTime(dv3t.Columns(j)) Then
                                dv3t.Rows(i)(j) = DateTime.MinValue
                            Else
                                dv3t.Rows(i)(j) = "0"
                            End If
                        Else
                            If ColumnTypeIsNumeric(dv3t.Columns(j)) Then
                                dv3t.Rows(i)(j) = 0
                            ElseIf ColumnTypeIsDateTime(dv3t.Columns(j)) Then
                                dv3t.Rows(i)(j) = DateTime.MinValue
                            ElseIf ColumnTypeIsString(dv3t.Columns(j)) Then
                                dv3t.Rows(i)(j) = ""
                            Else
                                dv3t.Rows(i)(j) = ""
                            End If
                        End If
                    End If
                Next
            Next
        Catch ex As Exception
            'err = coltype & " " & ex.Message
            err = err & " " & ex.Message
        End Try
        Return dv3t
    End Function
    Public Function CleanDBNullsFieldsM(ByRef dv3t As DataTable, ByVal ysel() As String, ByVal xsel() As String, Optional ByRef er As String = "", Optional ByVal bnum As Boolean = False) As DataTable
        Dim i, j As Integer
        Try
            For i = 0 To dv3t.Rows.Count - 1
                For j = 0 To xsel.Length - 1
                    If IsDBNull(dv3t.Rows(i)(xsel(j))) Then
                        If ColumnTypeIsNumeric(dv3t.Columns(xsel(j))) Then
                            dv3t.Rows(i)(xsel(j)) = 0
                        ElseIf ColumnTypeIsDateTime(dv3t.Columns(xsel(j))) Then
                            dv3t.Rows(i)(xsel(j)) = DateTime.MinValue
                        ElseIf ColumnTypeIsString(dv3t.Columns(xsel(j))) Then
                            dv3t.Rows(i)(xsel(j)) = ""
                        Else
                            dv3t.Rows(i)(xsel(j)) = ""
                        End If
                    End If
                Next
                For j = 0 To ysel.Length - 1
                    If IsDBNull(dv3t.Rows(i)(ysel(j))) Then
                        If ColumnTypeIsNumeric(dv3t.Columns(ysel(j))) Then
                            dv3t.Rows(i)(ysel(j)) = 0
                        ElseIf ColumnTypeIsDateTime(dv3t.Columns(ysel(j))) Then
                            dv3t.Rows(i)(ysel(j)) = DateTime.MinValue
                        Else
                            dv3t.Rows(i)(ysel(j)) = "0"
                        End If
                    End If
                Next
            Next
        Catch ex As Exception
            er = er & " " & ex.Message
        End Try
        Return dv3t
    End Function
    Public Function CleanDBNulls(ByRef dv3t As DataTable, Optional ByRef err As String = "", Optional ByRef bnum As Boolean = False) As DataTable
        'Dim coltype As String = ""
        Dim i, j As Integer
        Try
            For i = 0 To dv3t.Rows.Count - 1
                For j = 0 To dv3t.Columns.Count - 1
                    'coltype = dv3t.Columns(j).DataType.FullName
                    If IsDBNull(dv3t.Rows(i)(j)) Then
                        If bnum = True Then
                            If ColumnTypeIsNumeric(dv3t.Columns(j)) Then
                                dv3t.Rows(i)(j) = 0
                            ElseIf ColumnTypeIsDateTime(dv3t.Columns(j)) Then
                                dv3t.Rows(i)(j) = DateTime.MinValue
                            Else
                                dv3t.Rows(i)(j) = "0"
                            End If
                        Else
                            If ColumnTypeIsNumeric(dv3t.Columns(j)) Then
                                dv3t.Rows(i)(j) = 0
                            ElseIf ColumnTypeIsDateTime(dv3t.Columns(j)) Then
                                dv3t.Rows(i)(j) = DateTime.MinValue
                            ElseIf ColumnTypeIsString(dv3t.Columns(j)) Then
                                dv3t.Rows(i)(j) = ""
                            Else
                                dv3t.Rows(i)(j) = ""
                            End If
                        End If
                    End If
                Next
            Next
        Catch ex As Exception
            'err = coltype & " " & ex.Message
            err = err & " " & ex.Message
        End Try
        Return dv3t
    End Function
    'Public Function CalcStats(ByVal dv3t As DataTable, ByVal dtb As DataTable, Optional ByRef err As String = "") As DataTable
    '    Dim i, j, cnt As Integer
    '    Dim fldname As String
    '    Dim sm, mi, ma As Integer
    '    Dim av, st, vr As Double
    '    Dim dt As DataTable
    '    Dim fldnames(0) As String
    '    For i = 0 To dv3t.Rows.Count - 1
    '        For j = 0 To dv3t.Columns.Count - 1
    '            If IsDBNull(dv3t(i)(j)) Then
    '                If ColumnTypeIsNumeric(dv3t.Columns(j)) Then
    '                    dv3t(i)(j) = 0
    '                ElseIf ColumnTypeIsDateTime(dv3t.Columns(j)) Then
    '                    dv3t(i)(j) = DateTime.MinValue
    '                Else
    '                    dv3t(i)(j) = ""
    '                End If
    '            End If
    '        Next
    '    Next
    '    Try

    '        'calculate statistics for each field
    '        For i = 0 To dtb.Rows.Count - 1
    '            'record count
    '            cnt = dv3t.Rows.Count
    '            dtb.Rows(i)("Count") = cnt
    '            fldname = dtb.Rows(i)("Field").ToString
    '            If ColumnTypeIsNumeric(dv3t.Columns(fldname)) Then
    '                Try
    '                    sm = dv3t.Compute("SUM(" & fldname & ")", "")
    '                    dtb.Rows(i)("Sum") = sm
    '                Catch ex As Exception
    '                    err = err & " Error in SUM compute for field " & fldname & " - " & ex.Message & ", "
    '                End Try

    '                Try
    '                    mi = dv3t.Compute("MIN(" & fldname & ")", "")
    '                    dtb.Rows(i)("Min") = mi
    '                Catch ex As Exception
    '                    err = err & " Error in MIN compute for field " & fldname & " - " & ex.Message & ", "
    '                End Try

    '                Try
    '                    ma = dv3t.Compute("MAX(" & fldname & ")", "")
    '                    dtb.Rows(i)("Max") = ma
    '                Catch ex As Exception
    '                    err = err & " Error in MAX compute for field " & fldname & " - " & ex.Message & ", "
    '                End Try

    '                Try
    '                    av = dv3t.Compute("AVG(" & fldname & ")", "")
    '                    av = FormatNumber(av, 2) ', , , TriState.True)
    '                    dtb.Rows(i)("Average") = av
    '                Catch ex As Exception
    '                    err = err & " Error in AVG compute for field " & fldname & " - " & ex.Message & ", "
    '                End Try

    '                Try
    '                    st = dv3t.Compute("StDev(" & fldname & ")", "")
    '                    st = FormatNumber(st, 2)
    '                    dtb.Rows(i)("StDev") = st
    '                Catch ex As Exception
    '                    err = err & " Error in StDev compute for field " & fldname & " - " & ex.Message & ", "
    '                End Try

    '                Try
    '                    vr = 1.96 * st / Sqrt(cnt)
    '                    vr = FormatNumber(vr, 2)
    '                    dtb.Rows(i)("95% CI") = av.ToString & " +- " & vr.ToString
    '                Catch ex As Exception
    '                    err = err & " Error in CI compute for field " & fldname & " - " & ex.Message & ", "
    '                End Try

    '                Try
    '                    vr = dv3t.Compute("VAR(" & fldname & ")", "")
    '                    vr = FormatNumber(vr, 2)
    '                    dtb.Rows(i)("Var") = vr
    '                Catch ex As Exception
    '                    err = err & " Error in VAR compute for field " & fldname & " - " & ex.Message & ", "
    '                End Try

    '            End If
    '            Try
    '                If ColumnTypeIsDateTime(dv3t.Columns(fldname)) AndAlso dv3t.Rows(0)(fldname) = DateTime.MinValue Then
    '                    dtb.Rows(i)("First Value") = ""
    '                Else
    '                    dtb.Rows(i)("First Value") = dv3t.Rows(0)(fldname).ToString
    '                End If
    '            Catch ex As Exception
    '                err = err & " Error in First Value for field " & fldname & " - " & ex.Message & ", "
    '            End Try

    '            Try
    '                If ColumnTypeIsDateTime(dv3t.Columns(fldname)) AndAlso dv3t.Rows(dv3t.Rows.Count - 1)(fldname) = DateTime.MinValue Then
    '                    dtb.Rows(i)("Last Value") = ""
    '                Else
    '                    dtb.Rows(i)("Last Value") = dv3t.Rows(dv3t.Rows.Count - 1)(fldname).ToString
    '                End If
    '            Catch ex As Exception
    '                err = err & " Error in Last Value for field " & fldname & " - " & ex.Message & ", "
    '            End Try

    '            Try
    '                fldnames(0) = fldname
    '                dt = dv3t.DefaultView.ToTable(1, fldnames)
    '                dtb.Rows(i)("Count Distinct") = dt.Compute("Count(" & fldname & ")", "")
    '            Catch ex As Exception
    '                err = err & " Error in Count Distinct compute for field " & fldname & " - " & ex.Message & ", "
    '            End Try

    '        Next
    '    Catch ex As Exception
    '        err = ex.Message
    '    End Try
    '    Return dtb
    'End Function
    Public Function ColumnTypeIsNumeric(ByVal col As DataColumn) As Boolean
        Try
            If col Is Nothing OrElse col.Caption.Trim = "" Then
                Return False
            End If
            If (col.DataType.FullName = "System.Single" OrElse col.DataType.FullName = "System.Double" OrElse col.DataType.FullName = "System.Decimal" OrElse col.DataType.FullName = "System.Byte" OrElse col.DataType.FullName = "System.Int16" OrElse col.DataType.FullName = "System.Int32" OrElse col.DataType.FullName = "System.Int64" OrElse col.DataType.FullName = "System.Integer" OrElse col.DataType.FullName = "Numeric") Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function
    Public Function ColumnTypeIsDateTime(ByVal col As DataColumn) As Boolean
        'Problem: in DataTable column is String even if it is DateTime one...
        If (col.DataType.FullName = "System.DateTime" OrElse col.DataType.FullName = "System.Date" OrElse col.DataType.FullName.StartsWith("System.Time")) Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Function IsDateTime(ByVal txtDate As String) As Boolean
        Dim tempDate As DateTime
        Return DateTime.TryParse(txtDate, tempDate)
    End Function
    Public Function IsDateTimeValue(ByVal txtDate As String, ByRef dtResult As DateTime) As Boolean
        Dim dtReturn As New DateTime

        If Regex.IsMatch(txtDate, "^((Sept\.) ([1-9]|1\d|2\d|30), (19\d\d|20\d\d))$") Then
            txtDate = txtDate.Replace("Sept.", "September")
        End If

        Dim formats As String() = {"MM/dd/yyyy",
                                                 "MM/dd/yyyy HH:mm",
                                                 "MM/dd/yyyy HH:mm:ss",
                                                 "MM/dd/yyyy hh:mm tt",
                                                 "MM/dd/yyyy hh:mm:ss tt",
                                                 "M/d/yyyy",
                                                 "M/d/yyyy HH:mm",
                                                 "M/d/yyyy HH:mm:ss",
                                                 "M/d/yyyy hh:mm tt",
                                                 "M/d/yyyy hh:mm:ss tt",
                                                  "yyyy-MM-dd HH:mm",
                                                  "yyyy-MM-dd HH:mm:ss",
                                                  "yyyy-MM-dd hh:mm tt",
                                                   "yyyy-MM-dd hh:mm:ss tt",
                                                   "yyyy-MM-dd HH:mm:ss.fff",
                                                   "yyyy-MM-dd HH:mm:ss.ffffff",
                                                   "yyyy-MM-ddTHH:mm:ss+|-HH:mm",
                                                   "yyyy-MM-ddTHH:mm:sszzz",
                                                   "MMM. d, yyyy",
                                                   "MMM d, yyyy",
                                                   "MMMM d, yyyy",
                                                   "dd-MMM-yy hh.mm.ss tt",
                                                   "dd-MMM-yyyy hh.mm.ss tt",
                                                   "dd-MMM-yy",
                                                   "dd-MMM-yyyy"
                                                                           }

        If DateTime.TryParseExact(txtDate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, dtReturn) Then
            dtResult = dtReturn
            Return True
        Else
            Return False
        End If


    End Function
    Public Function ColumnTypeIsString(col As DataColumn) As Boolean
        If col.DataType.FullName = "System.String" Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Function ColumnValuesType(ByVal dt As DataTable, ByVal colname As String, ByRef coltype As String, ByRef collen As String, ByRef coldate As String) As DataTable
        Dim j As Integer
        Dim m As Integer = dt.Rows.Count
        collen = "1"
        coltype = "num"
        coldate = "date"
        For j = 0 To m - 1
            'length
            If Not dt.Rows(j)(colname) Is Nothing AndAlso dt.Rows(j)(colname).ToString.Length > CInt(collen) Then
                collen = dt.Rows(j)(colname).ToString.Length.ToString
            End If
            'type
            If Not dt.Rows(j)(colname) Is Nothing AndAlso dt.Rows(j)(colname).ToString <> "" Then
                'correct K and M in the end of number
                If dt.Rows(j)(colname).ToString.EndsWith("K") AndAlso IsNumeric(dt.Rows(j)(colname).ToString.Replace("K", "")) Then
                    dt.Rows(j)(colname) = 1000 * (dt.Rows(j)(colname).Replace("K", ""))
                    Continue For
                ElseIf dt.Rows(j)(colname).ToString.EndsWith("M") AndAlso IsNumeric(dt.Rows(j)(colname).ToString.Replace("M", "")) Then
                    dt.Rows(j)(colname) = 1000000 * (dt.Rows(j)(colname).Replace("M", ""))
                    Continue For
                End If
                'string
                If Not IsNumeric(dt.Rows(j)(colname).ToString) Then
                    coltype = "str"
                ElseIf IsNumeric(dt.Rows(j)(colname).ToString) AndAlso dt.Rows(j)(colname).ToString <> "0" AndAlso dt.Rows(j)(colname).ToString.StartsWith("0") AndAlso Not dt.Rows(j)(colname).ToString.StartsWith("0.") Then
                    coltype = "str"
                End If
                'date
                If dt.Rows(j)(colname).ToString.Trim <> "" AndAlso Not IsDate(dt.Rows(j)(colname).ToString) Then
                    coldate = ""
                End If
            End If
        Next
        If coldate = "date" Then
            For j = 0 To m - 1
                If dt.Rows(j)(colname).ToString.Trim <> "" Then
                    dt.Rows(j)(colname) = DateToString(dt.Rows(j)(colname)).Substring(0, DateToString(dt.Rows(j)(colname)).IndexOf(" "))
                End If
            Next
        End If
        Return dt
    End Function
    Public Function ColumnsValuesTypes(ByVal dt As DataTable, ByRef coltypes() As String, ByRef collens() As String, ByRef coldates() As String) As DataTable
        'NOT IN USE
        Dim i, j As Integer
        Dim m As Integer = dt.Rows.Count
        Dim n As Integer = dt.Columns.Count
        ReDim coltypes(n - 1)
        ReDim coldates(n - 1)
        ReDim collens(n - 1)
        'initial types
        For i = 0 To n - 1
            collens(i) = "12"
            coltypes(i) = "num"
            coldates(i) = "date"
        Next
        For i = 0 To n - 1
            For j = 0 To m - 1
                'length
                If Not dt.Rows(j)(i) Is Nothing AndAlso dt.Rows(j)(i).ToString.Length > CInt(collens(i)) Then
                    collens(i) = dt.Rows(j)(i).ToString.Length.ToString
                End If
                'type
                If Not dt.Rows(j)(i) Is Nothing AndAlso dt.Rows(j)(i).ToString <> "" Then
                    'correct K and M in the end of number
                    If dt.Rows(j)(i).ToString.EndsWith("K") AndAlso IsNumeric(dt.Rows(j)(i).ToString.Replace("K", "")) Then
                        dt.Rows(j)(i) = 1000 * (dt.Rows(j)(i).Replace("K", ""))
                        Continue For
                    ElseIf dt.Rows(j)(i).ToString.EndsWith("M") AndAlso IsNumeric(dt.Rows(j)(i).ToString.Replace("M", "")) Then
                        dt.Rows(j)(i) = 1000000 * (dt.Rows(j)(i).Replace("M", ""))
                        Continue For
                    End If
                    'string
                    If Not IsNumeric(dt.Rows(j)(i).ToString) Then
                        coltypes(i) = "str"
                    ElseIf IsNumeric(dt.Rows(j)(i).ToString) AndAlso dt.Rows(j)(i).ToString <> "0" AndAlso dt.Rows(j)(i).ToString.StartsWith("0") AndAlso Not dt.Rows(j)(i).ToString.StartsWith("0.") Then
                        coltypes(i) = "str"
                    End If
                    'date
                    If dt.Rows(j)(i).ToString.Trim <> "" AndAlso Not IsDate(dt.Rows(j)(i).ToString) Then
                        coldates(i) = ""
                    End If
                End If
            Next
        Next
        Return dt
    End Function
    Public Function ConvertTypeToOUR(ByVal typ As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        'NOT IN USE
        Dim myconstring As String = myconstr
        Dim myprovider As String = myconprv
        If myconstr = "" Then
            myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        End If
        Dim ret As String = String.Empty
        If typ.ToUpper.Contains("STRING") OrElse typ.ToUpper.Contains("CHAR") OrElse typ.ToUpper.Contains("TEXT") Then
            ret = "nvarchar"
        ElseIf typ.ToUpper.Contains("DATE") OrElse typ.ToUpper.Contains("TIME") Then
            ret = "datetime"
        Else
            ret = "int"
        End If
        If myprovider.StartsWith("InterSystems.Data.") Then

        ElseIf myprovider = "MySql.Data.MySqlClient" Then

        ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then

        ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql

        Else

        End If
        Return ret
    End Function
    Public Function ConvertTypeToOURTemplate(ByVal typ As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim myconstring As String = myconstr
        Dim myprovider As String = myconprv
        If myconstr = "" Then
            myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        End If
        Dim ret As String = String.Empty
        If typ.ToUpper.Contains("STRING") OrElse typ.ToUpper.Contains("CHAR") OrElse typ.ToUpper.Contains("TEXT") Then
            ret = "nvarchar"
        ElseIf typ.ToUpper.Contains("DATE") OrElse typ.ToUpper.Contains("TIME") Then
            ret = "datetime"
        Else
            ret = "int"
        End If
        If myprovider.StartsWith("InterSystems.Data.") Then

        ElseIf myprovider = "MySql.Data.MySqlClient" Then

        ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then

        ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql

        ElseIf myconprv = "Sqlite" Then  'Sqlite

        Else

        End If
        Return ret
    End Function
    Public Function ConvertTypeToOUR(ByVal typ As String) As String
        Dim ret As String = String.Empty
        If typ.ToUpper.Contains("STRING") OrElse typ.ToUpper.Contains("CHAR") OrElse typ.ToUpper.Contains("TEXT") Then
            ret = "nvarchar"
        ElseIf typ.ToUpper.Contains("DATE") OrElse typ.ToUpper.Contains("TIME") Then
            ret = "datetime"
        ElseIf typ.ToUpper = "REF CURSOR" Then 'only for oracle
            ret = "cursor"
        Else
            ret = "int"
        End If
        Return ret
    End Function
    Public Function MakeDateRelative(ByVal curdate As Date, ByVal dat As Date) As String
        Dim ret As String = String.Empty
        Try
            Dim datinterval As Integer = DateDiff(DateInterval.Day, curdate, dat)
            If datinterval > -1 Then datinterval = datinterval + 1
            If datinterval = 1 Then
                Dim date1 As String = Format(curdate, "M/d/yyyy")
                Dim date2 As String = Format(dat, "M/d/yyyy")
                If date1 = date2 Then datinterval = 0
            End If
            ret = datinterval.ToString
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function ExponentToNumber(sExponential As String) As String
        Dim sConverted As String = sExponential
        sExponential = sExponential.ToUpper
        If sExponential.Contains("E") Then
            Dim sNumber As String = Piece(sExponential, "E", 1)
            Dim sExponent As String = Piece(sExponential, "E", 2)
            Dim dblNumber As Double = Val(sNumber)
            Dim nExponent As Integer = CInt(sExponent)
            Dim dbl As Double = dblNumber * (10 ^ nExponent)

            sConverted = Convert.ToString(dbl)
        End If
        Return sConverted
    End Function
    Public Function ConvertMySqlTableOriginal(dt3 As DataTable, Optional ByRef err As String = "", Optional ByVal fld As String = "", Optional num As Boolean = False) As DataTable
        Dim dtb As New DataTable
        If dt3 Is Nothing Then
            Return dtb
        End If
        Dim col As DataColumn
        col = New DataColumn
        Dim i, j As Integer
        Try
            For j = 0 To dt3.Columns.Count - 1
                col = New DataColumn
                col.ColumnName = dt3.Columns(j).Caption
                'col.ColumnName = dt3.Columns(j).Caption.Replace("_", " ").Trim.Replace(" ", "_").Trim
                If dt3.Columns(j).DataType.FullName = "MySql.Data.Types.MySqlDateTime" OrElse dt3.Columns(j).DataType.FullName = "System.TimeSpan" Then
                    col.DataType = System.Type.GetType("System.String")
                ElseIf num = True AndAlso dt3.Columns(j).Caption = fld Then
                    col.DataType = System.Type.GetType("System.Double")
                Else
                    col.DataType = System.Type.GetType(dt3.Columns(j).DataType.FullName)
                End If
                dtb.Columns.Add(col)

            Next
            'fixing values in table
            For i = 0 To dt3.Rows.Count - 1
                Dim Row As DataRow = dtb.NewRow()
                For j = 0 To dt3.Columns.Count - 1
                    If dt3.Columns(j).DataType.FullName = "MySql.Data.Types.MySqlDateTime" Then
                        If IsDBNull(dt3.Rows(i)(j)) OrElse dt3.Rows(i)(j).ToString.Trim = "" OrElse dt3.Rows(i)(j).ToString = "0" Then
                            Row(j) = ""
                        Else
                            Row(j) = DateToString(CDate(dt3.Rows(i)(j).ToString), "MySql.Data.MySqlClient")
                        End If
                    ElseIf num = True AndAlso dt3.Columns(j).Caption = fld Then
                        If IsDBNull(dt3.Rows(i)(j)) OrElse dt3.Rows(i)(j).ToString.Trim = "" Then
                            Row(j) = "0"
                        Else
                            Row(j) = CType(dt3.Rows(i)(j).ToString, Double)
                        End If
                    ElseIf dt3.Columns(j).DataType = System.Type.GetType("System.String") Then
                        If IsDBNull(dt3.Rows(i)(j)) OrElse dt3.Rows(i)(j).ToString.Trim = "" Then
                            Row(j) = ""
                        Else
                            Row(j) = dt3.Rows(i)(j).Replace("'", "`")
                        End If

                    Else
                        Row(j) = dt3.Rows(i)(j)
                    End If
                Next
                dtb.Rows.Add(Row)
            Next
        Catch ex As Exception
            err = ex.Message
        End Try
        Return dtb
    End Function
    Public Function ConvertMySqlTable(ByVal dt3 As DataTable, Optional ByRef er As String = "", Optional ByVal fld As String = "", Optional num As Boolean = False) As DataTable
        Dim i, j As Integer
        If dt3 Is Nothing Then
            Return Nothing
        End If

        For j = 0 To dt3.Columns.Count - 1
            Try
                If dt3.Columns(j).DataType.FullName = "MySql.Data.Types.MySqlDateTime" OrElse dt3.Columns(j).DataType.FullName = "System.TimeSpan" OrElse dt3.Columns(j).DataType.FullName = "System.DateTime" Then
                    'col.DataType = System.Type.GetType("System.String")
                    Dim colname As String = dt3.Columns(j).Caption
                    Dim col As New DataColumn
                    col.ColumnName = colname & "_new"
                    col.DataType = System.Type.GetType("System.String")
                    Dim ordinal As Integer = dt3.Columns(j).Ordinal  'j
                    dt3.Columns.Add(col)
                    Dim ordinalnew As Integer = dt3.Columns(col.ColumnName).Ordinal
                    'fixing values in datatable
                    For i = 0 To dt3.Rows.Count - 1
                        If IsDBNull(dt3.Rows(i)(j)) OrElse dt3.Rows(i)(j).ToString.Trim = "" OrElse dt3.Rows(i)(j).ToString = "0" Then
                            dt3.Rows(i)(ordinalnew) = ""
                        Else
                            dt3.Rows(i)(ordinalnew) = DateToString(CDate(dt3.Rows(i)(j).ToString), "MySql.Data.MySqlClient").Replace("00:00:00", "").Replace("0001-01-01", "").Trim
                        End If
                    Next
                    dt3.Columns.Remove(dt3.Columns(j).Caption)
                    dt3.Columns(col.ColumnName).ColumnName = colname
                    dt3.Columns(col.ColumnName).SetOrdinal(ordinal)

                ElseIf num = True AndAlso dt3.Columns(j).Caption = fld Then
                    dt3.Columns(j).DataType = Convert.ChangeType(dt3.Columns(j), System.Type.GetType("System.Double"))
                    'fixing values in datatable
                    For i = 0 To dt3.Rows.Count - 1
                        If IsDBNull(dt3.Rows(i)(j)) OrElse dt3.Rows(i)(j).ToString.Trim = "" Then
                            dt3.Rows(i)(j) = "0"
                        Else
                            dt3.Rows(i)(j) = CType(dt3.Rows(i)(j).ToString, Double)
                        End If
                    Next

                ElseIf dt3.Columns(j).DataType = System.Type.GetType("System.String") Then
                    For i = 0 To dt3.Rows.Count - 1
                        If IsDBNull(dt3.Rows(i)(j)) OrElse dt3.Rows(i)(j).ToString.Trim = "" Then
                            dt3.Rows(i)(j) = ""
                        Else
                            dt3.Rows(i)(j) = dt3.Rows(i)(j).Replace("'", "`")
                        End If
                    Next
                End If

            Catch ex As Exception
                er = er & " - " & ex.Message
            End Try
        Next
        Return dt3
    End Function
    Public Function CorrectColumnFromDateToString(dt3 As DataTable, Optional ByRef err As String = "", Optional ByVal fld As String = "", Optional num As Boolean = False) As DataTable
        Dim dtb As New DataTable
        Dim col As DataColumn
        col = New DataColumn
        Dim i, j As Integer
        Try
            For j = 0 To dt3.Columns.Count - 1
                col = New DataColumn
                col.ColumnName = dt3.Columns(j).Caption
                'col.ColumnName = dt3.Columns(j).Caption.Replace("_", " ").Trim.Replace(" ", "_").Trim
                If dt3.Columns(j).DataType.FullName = "MySql.Data.Types.MySqlDateTime" Then
                    col.DataType = System.Type.GetType("System.String")
                    'ElseIf num = True AndAlso dt3.Columns(j).Caption = fld Then
                    '    col.DataType = System.Type.GetType("System.Double")
                Else
                    col.DataType = System.Type.GetType(dt3.Columns(j).DataType.FullName)
                End If
                dtb.Columns.Add(col)

            Next
            'fixing values in table
            For i = 0 To dt3.Rows.Count - 1
                Dim Row As DataRow = dtb.NewRow()
                For j = 0 To dt3.Columns.Count - 1
                    If dt3.Columns(j).DataType.FullName = "MySql.Data.Types.MySqlDateTime" Then
                        If IsDBNull(dt3.Rows(i)(j)) OrElse dt3.Rows(i)(j).ToString.Trim = "" OrElse dt3.Rows(i)(j).ToString = "0" Then
                            Row(j) = ""
                        Else
                            Row(j) = DateToString(CDate(dt3.Rows(i)(j).ToString), "MySql.Data.MySqlClient")
                        End If
                        'ElseIf num = True AndAlso dt3.Columns(j).Caption = fld Then
                        '    If IsDBNull(dt3.Rows(i)(j)) OrElse dt3.Rows(i)(j).ToString.Trim = "" Then
                        '        Row(j) = "0"
                        '    Else
                        '        Row(j) = CType(dt3.Rows(i)(j).ToString, Double)
                        '    End If
                    ElseIf Not IsDBNull(dt3.Rows(i)(j)) AndAlso dt3.Rows(i)(j).ToString.Trim <> "" AndAlso dt3.Columns(j).DataType = System.Type.GetType("System.String") Then
                        Row(j) = dt3.Rows(i)(j).Replace("'", "`")
                    Else
                        Row(j) = dt3.Rows(i)(j)
                    End If
                Next
                dtb.Rows.Add(Row)
            Next
        Catch ex As Exception
            err = ex.Message
        End Try
        Return dtb
    End Function
    Public Function CorrectColumnAsNumeric(dt3 As DataTable, Optional ByRef err As String = "", Optional ByVal fld As String = "", Optional num As Boolean = False) As DataTable
        Dim dtb As New DataTable
        Dim col As DataColumn
        col = New DataColumn
        Dim i, j As Integer
        Try
            For j = 0 To dt3.Columns.Count - 1
                col = New DataColumn
                col.ColumnName = dt3.Columns(j).Caption
                'col.ColumnName = dt3.Columns(j).Caption.Replace("_", " ").Trim.Replace(" ", "_").Trim
                If num = True AndAlso dt3.Columns(j).Caption = fld Then
                    col.DataType = System.Type.GetType("System.Double")
                Else
                    col.DataType = System.Type.GetType(dt3.Columns(j).DataType.FullName)
                End If
                dtb.Columns.Add(col)
            Next
            'fixing values in table
            For i = 0 To dt3.Rows.Count - 1
                Dim Row As DataRow = dtb.NewRow()
                For j = 0 To dt3.Columns.Count - 1
                    If num = True AndAlso dt3.Columns(j).Caption = fld Then
                        If IsDBNull(dt3.Rows(i)(j)) OrElse dt3.Rows(i)(j).ToString.Trim = "" Then
                            Row(j) = "0"
                        Else
                            Row(j) = CType(dt3.Rows(i)(j).ToString, Double)
                        End If
                    Else
                        Row(j) = dt3.Rows(i)(j)
                    End If
                Next
                dtb.Rows.Add(Row)
            Next
        Catch ex As Exception
            err = ex.Message
        End Try
        Return dtb
    End Function
    Public Function CorrectColumnsAsNumeric(ByVal dt3 As DataTable, ByVal fldsnames As String, Optional ByRef err As String = "", Optional num As Boolean = False) As DataTable
        If num = False Then
            Return dt3
        End If
        Dim dtb As New DataTable
        Dim col As DataColumn
        col = New DataColumn
        Dim flds() As String
        flds = Split(fldsnames, ",")
        Dim j As Integer
        Try
            For j = 0 To flds.Length - 1
                If flds(j).Trim <> "" AndAlso Not ColumnTypeIsNumeric(dt3.Columns(flds(j))) Then
                    col = New DataColumn
                    col.ColumnName = flds(j).Trim
                    col.DataType = System.Type.GetType("System.Double")

                    dt3.Columns(flds(j)).Caption = flds(j) & "OLD"
                    dt3.Columns(flds(j)).ColumnName = flds(j) & "OLD"

                    dt3.Columns.Add(col)
                    'data from old column
                    For i = 0 To dt3.Rows.Count - 1
                        If IsDBNull(dt3.Rows(i)(flds(j) & "OLD")) OrElse dt3.Rows(i)(flds(j) & "OLD").ToString.Trim = "" Then
                            dt3.Rows(i)(flds(j)) = "0"
                        Else
                            dt3.Rows(i)(flds(j)) = CType(dt3.Rows(i)(flds(j) & "OLD").ToString, Double)
                        End If
                    Next
                    dt3.Columns.Remove(flds(j) & "OLD")
                End If
            Next
        Catch ex As Exception
            err = ex.Message
        End Try
        Return dt3
    End Function
    Public Function CorrectDatasetColumns(dt3 As DataTable, Optional ByRef err As String = "") As DataTable
        Dim j As Integer
        Try
            For j = 0 To dt3.Columns.Count - 1
                If dt3.Columns(j).Caption.IndexOf("_") >= 0 Then
                    dt3.Columns(j).Caption = dt3.Columns(j).Caption.Replace("_", " ").Trim.Replace(" ", "_").Trim.Replace("'", "`")
                    dt3.Columns(j).ColumnName = dt3.Columns(j).Caption
                End If
            Next
        Catch ex As Exception
            err = ex.Message
        End Try
        Return dt3
    End Function
    'Public Function CorrectDataset(dt3 As DataTable, Optional ByRef err As String = "") As DataTable
    '    'NOT IN USE
    '    Dim i, j As Integer
    '    Try
    '        For j = 0 To dt3.Columns.Count - 1
    '            If dt3.Columns(j).Caption.IndexOf("_") >= 0 Then
    '                dt3.Columns(j).Caption = dt3.Columns(j).Caption.Replace("_", " ").Trim.Replace(" ", "_").Trim.Replace("'", "`")
    '            End If
    '            For i = 0 To dt3.Rows.Count - 1
    '                If dt3.Columns(j).DataType = System.Type.GetType("System.String") Then
    '                    dt3.Rows(i)(j) = dt3.Rows(i)(j).Replace("'", "`")
    '                Else
    '                    dt3.Rows(i)(j) = dt3.Rows(i)(j)
    '                End If
    '            Next
    '        Next
    '    Catch ex As Exception
    '        err = ex.Message
    '    End Try
    '    Return dt3
    'End Function
    Public Function ConvertOracleTable(dt3 As DataTable, Optional ByRef er As String = "", Optional ByVal fld As String = "", Optional num As Boolean = False) As DataTable
        Dim dtb As New DataTable
        Dim col As DataColumn
        col = New DataColumn
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim m As Integer = dt3.Columns.Count
        Try
            While j < m
                col = New DataColumn
                col.ColumnName = dt3.Columns(j).Caption
                col.ColumnName = dt3.Columns(j).Caption.Replace("_", " ").Trim.Replace(" ", "_").Replace("'", "`")
                If dt3.Columns(j).DataType.FullName.StartsWith("System.Byte") Then
                    dt3.Columns.Remove(dt3.Columns(j))
                    m = m - 1
                Else
                    j = j + 1
                End If
            End While
        Catch ex As Exception
            er = ex.Message
        End Try

        If dt3 Is Nothing Then
            Return Nothing
        End If

        For j = 0 To dt3.Columns.Count - 1
            Try
                If dt3.Columns(j).DataType.FullName.Contains("Date") OrElse dt3.Columns(j).DataType.FullName.Contains("Time") Then '= "MySql.Data.Types.MySqlDateTime" OrElse dt3.Columns(j).DataType.FullName = "System.TimeSpan" OrElse dt3.Columns(j).DataType.FullName = "System.DateTime" Then
                    'col.DataType = System.Type.GetType("System.String")
                    Dim colname As String = dt3.Columns(j).Caption
                    col = New DataColumn
                    col.ColumnName = colname & "_new"
                    col.DataType = System.Type.GetType("System.String")
                    Dim ordinal As Integer = dt3.Columns(j).Ordinal  'j
                    dt3.Columns.Add(col)
                    Dim ordinalnew As Integer = dt3.Columns(col.ColumnName).Ordinal
                    'fixing values in datatable
                    For i = 0 To dt3.Rows.Count - 1
                        If IsDBNull(dt3.Rows(i)(j)) OrElse dt3.Rows(i)(j).ToString.Trim = "" OrElse dt3.Rows(i)(j).ToString = "0" Then
                            dt3.Rows(i)(ordinalnew) = ""
                        Else
                            dt3.Rows(i)(ordinalnew) = DateToString(CDate(dt3.Rows(i)(j).ToString), "MySql.Data.MySqlClient").Replace("00:00:00", "").Replace("0001-01-01", "").Trim
                        End If
                    Next
                    dt3.Columns.Remove(dt3.Columns(j).Caption)
                    dt3.Columns(col.ColumnName).ColumnName = colname
                    dt3.Columns(col.ColumnName).SetOrdinal(ordinal)

                ElseIf num = True AndAlso dt3.Columns(j).Caption = fld Then
                    dt3.Columns(j).DataType = Convert.ChangeType(dt3.Columns(j), System.Type.GetType("System.Double"))
                    'fixing values in datatable
                    For i = 0 To dt3.Rows.Count - 1
                        If IsDBNull(dt3.Rows(i)(j)) OrElse dt3.Rows(i)(j).ToString.Trim = "" Then
                            dt3.Rows(i)(j) = "0"
                        Else
                            dt3.Rows(i)(j) = CType(dt3.Rows(i)(j).ToString, Double)
                        End If
                    Next

                ElseIf dt3.Columns(j).DataType = System.Type.GetType("System.String") Then
                    For i = 0 To dt3.Rows.Count - 1
                        If IsDBNull(dt3.Rows(i)(j)) OrElse dt3.Rows(i)(j).ToString.Trim = "" Then
                            dt3.Rows(i)(j) = ""
                        Else
                            dt3.Rows(i)(j) = dt3.Rows(i)(j).Replace("'", "`")
                        End If
                    Next
                End If

            Catch ex As Exception
                er = er & " - " & ex.Message
            End Try
        Next
        Return dt3

    End Function
    Public Function IsParensBalanced(sCheckString As String) As Boolean

        Dim numOpen As Integer = 0
        Dim numClosed As Integer = 0

        For Each character As Char In sCheckString
            If character = "("c Then numOpen += 1
            If character = ")"c Then
                numClosed += 1
                If numClosed > numOpen Then Return False
            End If
        Next
        Return numOpen = numClosed
    End Function
    Public Function IsArrayBalanced(sArray() As String) As Boolean
        Dim ret = True
        For i As Integer = 0 To sArray.Length - 1
            If Not IsParensBalanced(sArray(i)) Then Return False
        Next
        Return ret
    End Function
    Public Function GetTextFromEmbeddedResource(ByVal filename As String, ByVal ext As String) As String
        Dim restext As String = String.Empty
        Dim ret As String = String.Empty
        Try
            Dim myassembly As Assembly = Assembly.GetExecutingAssembly
            Dim AppName As String = myassembly.FullName.Substring(0, myassembly.FullName.IndexOf(","))
            Dim mytextStreamReader As StreamReader = New StreamReader(myassembly.GetManifestResourceStream(AppName & "." & filename & ext))
            restext = mytextStreamReader.ReadToEnd
            Return restext
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Public Function GetTextFromFile(ByVal dir As String, ByVal filename As String, ByVal ext As String) As String
        Dim restext As String = String.Empty
        Dim ret As String = String.Empty
        Try
            Dim mytextStreamReader As StreamReader = New StreamReader(dir & filename & ext)
            restext = mytextStreamReader.ReadToEnd
            Return restext
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Public Function CreateRoutine(ByVal RoutineName As String, ByVal RoutineText As String, ByVal connstr As String, ByVal connprv As String) As String
        Try
            Dim StoredProcName As String = String.Empty
            Dim ParamName(1) As String
            Dim ParamType(1) As String
            Dim ParamValue(1) As String
            Dim ret As String = String.Empty
            If RoutineName <> String.Empty Then
                ParamName(0) = "RoutineName"
                ParamType(0) = "String"
                ParamValue(0) = RoutineName
                ParamName(1) = "RoutineText"
                ParamType(1) = "String"
                ParamValue(1) = RoutineText
                StoredProcName = "OUR.BUILDROUTINE"
                'run storproc
                ret = RunSP(StoredProcName, 2, ParamName, ParamType, ParamValue, connstr, connprv)
            End If
            Return ret
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Public Function CreateClass(ByVal ClassText As String, ByVal connstr As String, ByVal connprv As String) As String
        Dim ret As String = String.Empty
        Try
            Dim StoredProcName As String = String.Empty
            Dim ParamName(0) As String
            Dim ParamType(0) As String
            Dim ParamValue(0) As String
            If ClassText <> String.Empty Then
                'make params
                ReDim ParamName(0)
                ReDim ParamType(0)
                ReDim ParamValue(0)
                ParamName(0) = "ClassText"
                ParamName(0) = "String"
                ParamValue(0) = ClassText
                StoredProcName = "OUR.BUILDCLASSFROMSTRING"  'StorProc in OUR.INIT class 
                'run storproc
                ret = RunSP(StoredProcName, 1, ParamName, ParamType, ParamValue, connstr, connprv)
            End If
            Return ret
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Public Function ImportXMLintoDbTables(ByRef tblname() As String, ByVal xmlpath As String, ByVal userconnstr As String, ByVal userconnprv As String, ByRef ntables As Integer) As String
        Dim ret As String = String.Empty
        Dim dset As New DataSet
        Dim n As Integer = 0
        Try
            'read xml into xmldoc
            dset = ReadDataSetFromXML(xmlpath, ret)

            If Not dset Is Nothing Then
                ntables = dset.Tables.Count
                If ntables = 0 Then
                    Return "No data"
                End If
                'TODO Make MySql tables and forein keys from dset an xml doc

                ret = CreateDbTablesForDataSet(tblname, dset, userconnstr, userconnprv)
            Else
                Return "No data"
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function ImportJSONintoMySqlTables(ByRef tblname() As String, ByVal xmlpath As String, ByVal userconnstr As String, ByVal userconnprv As String, ByRef ntables As Integer) As String
        Dim ret As String = String.Empty
        Dim dset As New DataSet
        Dim n As Integer = 0
        Try
            'read xml into xmldoc
            dset = ReadDataSetFromXML(xmlpath, ret)

            If Not dset Is Nothing Then
                ntables = dset.Tables.Count
                If ntables = 0 Then
                    Return "No data"
                End If
                'TODO Make MySql tables and forein keys from dset an xml doc

                ret = CreateDbTablesForDataSet(tblname, dset, userconnstr, userconnprv)
            Else
                Return "No data"
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function ReadDataSetFromXML(ByVal XmlFile As String, Optional er As String = "") As DataSet
        Dim ret As String = String.Empty
        If XmlFile = String.Empty OrElse File.Exists(XmlFile) = False Then
            Return Nothing
            Exit Function
        End If
        Dim dset As DataSet
        dset = New DataSet
        Try
            Dim fstr As New System.IO.StreamReader(XmlFile)
            dset.ReadXml(fstr)
            Return dset
        Catch ex As Exception
            er = "Close XML file first." & ex.Message
            Return Nothing
        End Try
    End Function
    Public Function MakeArrayFromStringWithQuotes(ByVal str As String, ByVal d As String) As String()
        'make array with column names with double quotes
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim cols(0) As String
        Dim bq As Boolean = False
        Try
            If str.IndexOf("""") < 0 Then
                cols = str.Split(d)
            Else
                'quotes
                For i = 0 To str.Length - 1
                    If str(i) = """" Then
                        bq = Not bq
                    End If
                    If bq = True Then
                        cols(j) = cols(j) & str(i)
                    Else
                        If str(i) = d Then
                            ReDim Preserve cols(j + 1)
                            j = j + 1
                        Else
                            cols(j) = cols(j) & str(i)
                        End If
                    End If
                Next
            End If

            Return cols
        Catch ex As Exception
            Return cols
        End Try
    End Function
    Public Function DashboardAvailable(UserID As String, DashboardName As String) As Boolean
        Dim ret As Boolean = False
        Dim sqlq As String = "Select Dashboard From ourdashboards Where UserId='" & UserID & "' AND Dashboard='" & DashboardName & "'"
        Dim dt As DataTable = mRecords(sqlq).Table
        If dt IsNot Nothing AndAlso dt.Rows.Count < 12 Then ret = True
        Return ret
    End Function
    Public Function GetDashBoards(UserID As String) As ListItemCollection
        Dim items As New ListItemCollection
        Dim sqlq As String = "Select Distinct Dashboard From ourdashboards Where UserId='" & UserID & "' Order By Dashboard"
        Dim dt1 As DataTable = mRecords(sqlq).Table
        Dim dt2 As DataTable = Nothing
        Dim n As Integer = 0

        If dt1 IsNot Nothing AndAlso dt1.Rows.Count > 0 Then
            Dim dr As DataRow = Nothing
            Dim dashbrd As String = String.Empty
            Dim li As ListItem = Nothing

            For i As Integer = 0 To dt1.Rows.Count - 1
                dr = dt1.Rows(i)
                dashbrd = dr("Dashboard")
                li = New ListItem(dashbrd, dashbrd)
                li.Text = dashbrd
                If Not DashboardAvailable(UserID, dashbrd) Then
                    li.Enabled = False
                End If
                items.Add(li)
                'sqlq = "Select * From ourdashboards where Dashboard='" & dashbrd & "'"
                'dt2 = mRecords(sqlq).Table
                'If dt2 IsNot Nothing AndAlso dt2.Rows.Count > 0 AndAlso dt2.Rows.Count < 12 Then
                '    ReDim Preserve items(n)
                '    items(n) = dashbrd
                '    n += 1
                'End If
            Next
        End If
        Return items
    End Function
    Public Function ImportXMLorJSONintoDatabase(ByVal rep As String, ByVal tblname As String, ByVal xmljsonpath As String, ByVal userconnstr As String, ByVal userconnprv As String, ByVal unit As String, ByVal logon As String, ByVal userdb As String, Optional ByVal xmlorjson As Boolean = False, Optional funiq As Boolean = False) As String
        Dim ret As String = String.Empty
        Dim er As String = String.Empty
        Dim n As Integer = 0
        Dim m As Integer = 0
        Dim doc As New XmlDocument
        Dim firsttblname As String = String.Empty
        Dim node As XmlNode = Nothing

        Try
            If xmlorjson Then
                'xml
                doc.Load(xmljsonpath)
            Else
                'json
                'read json into xmldoc
                Dim jsonstr As String = File.ReadAllText(xmljsonpath)
                Dim Xdoc As XDocument
                Try
                    Xdoc = JsonConvert.DeserializeXNode(jsonstr, "Root")
                Catch ex As Exception
                    Xdoc = JsonConvert.DeserializeXNode(jsonstr, "")
                End Try
                'Xdoc.Nodes.Count
                doc.LoadXml(Xdoc.ToString)
            End If

            'create db 
            Dim sqlq As String = String.Empty
            Dim db As String = GetDataBase(userconnstr, userconnprv).ToLower  'user db
            If userconnprv = "MySql.Data.MySqlClient" Then
                sqlq = "Select * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME='" & db & "'"
                If Not HasRecords(sqlq, userconnstr, userconnprv) Then
                    sqlq = "CREATE DATABASE `" & db & "`"
                    ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
                    If ret = "Query executed fine." Then
                        ret = "" ' ret & db & " created. "
                    Else
                        ret = ret & db & " not created: " & ret & " "
                        Return ret
                    End If
                End If
            End If

            'find first node with ChildNodes
            For i = 0 To doc.ChildNodes.Count - 1
                node = doc.ChildNodes(i)
                If node.HasChildNodes Then
                    If node.Name.Trim <> "" Then
                        firsttblname = node.Name
                    Else
                        firsttblname = tblname  'rep

                    End If
                    Exit For
                End If
            Next

            Dim task As Threading.Tasks.Task(Of String) = ProcessXMLJSONNodeIntoTable(rep, m, tblname, node, firsttblname, tblname, 0, userconnstr, userconnprv, unit, logon, userdb, er, funiq)

            ret = "input done"

        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        doc = Nothing
        Return ret
    End Function

    Public Function ImportXMLorJSONintoDatabaseFromURL(ByVal rep As String, ByVal tblname As String, ByVal xmljsonpath As String, ByVal userconnstr As String, ByVal userconnprv As String, ByVal unit As String, ByVal logon As String, ByVal userdb As String, Optional ByVal xmlorjson As Boolean = False, Optional funiq As Boolean = False) As String
        Dim ret As String = String.Empty
        Dim er As String = String.Empty
        Dim n As Integer = 0
        Dim m As Integer = 0
        Dim doc As New XmlDocument
        Dim firsttblname As String = String.Empty
        Dim node As XmlNode = Nothing
        Dim client = New WebClient
        Dim Stream As Stream = client.OpenRead(xmljsonpath)
        Dim sr As StreamReader = New StreamReader(Stream)
        Dim jsonstr As String = String.Empty
        Try
            If xmlorjson Then
                'xml
                'doc.Load(xmljsonpath)
                doc.Load(Stream)

            Else
                'json
                'read json into xmldoc
                'Dim jsonstr As String = File.ReadAllText(xmljsonpath)
                jsonstr = sr.ReadToEnd()
                Dim Xdoc As XDocument
                Try
                    Xdoc = JsonConvert.DeserializeXNode(jsonstr, "Root")
                Catch ex As Exception
                    Xdoc = JsonConvert.DeserializeXNode(jsonstr, "")
                End Try
                doc.LoadXml(Xdoc.ToString)

            End If

            'create db 
            Dim sqlq As String = String.Empty
            Dim db As String = GetDataBase(userconnstr, userconnprv).ToLower  'user db
            If userconnprv = "MySql.Data.MySqlClient" Then
                sqlq = "Select * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME='" & db & "'"
                If Not HasRecords(sqlq, userconnstr, userconnprv) Then
                    sqlq = "CREATE DATABASE `" & db & "`"
                    ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
                    If ret = "Query executed fine." Then
                        ret = "" ' ret & db & " created. "
                    Else
                        ret = ret & db & " not created: " & ret & " "
                        Return ret
                    End If
                End If
            End If

            'find first node with ChildNodes
            For i = 0 To doc.ChildNodes.Count - 1
                node = doc.ChildNodes(i)
                If node.HasChildNodes Then
                    If node.Name.Trim <> "" Then
                        firsttblname = node.Name
                    Else
                        firsttblname = tblname  'rep

                    End If
                    Exit For
                End If
            Next

            Dim task As Threading.Tasks.Task(Of String) = ProcessXMLJSONNodeIntoTable(rep, m, tblname, node, firsttblname, tblname, 0, userconnstr, userconnprv, unit, logon, userdb, er, funiq)

            ret = "input done"

        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        doc = Nothing
        Return ret
    End Function

    Public Async Function ProcessXMLJSONNodeIntoTable(ByVal rep As String, ByVal m As Integer, ByVal tblname As String, ByVal node As XmlNode, ByVal tbl As String, ByVal parenttbl As String, ByVal parentid As Integer, ByVal userconnstr As String, ByVal userconnprv As String, ByVal unit As String, ByVal logon As String, ByVal userdb As String, Optional er As String = "", Optional funiq As Boolean = False) As Threading.Tasks.Task(Of String)
        'recursive
        Dim ret As String = String.Empty
        Dim repdb As String = userconnstr
        'If userconnstr.ToUpper.IndexOf("USER ID") > 0 Then repdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("USER ID")).Trim
        'If userconnstr.IndexOf("UID") > 0 Then repdb = userconnstr.Substring(0, userconnstr.IndexOf("UID")).Trim
        If userconnstr.ToUpper.IndexOf("USER ID") > 0 AndAlso userconnprv <> "Oracle.ManagedDataAccess.Client" Then
            repdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("USER ID")).Trim
        ElseIf userconnstr.ToUpper.IndexOf("PASSWORD") > 0 AndAlso userconnprv = "Oracle.ManagedDataAccess.Client" Then
            repdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("PASSWORD")).Trim
        End If
        If userconnstr.IndexOf("UID") > 0 AndAlso userconnprv <> "Oracle.ManagedDataAccess.Client" Then
            repdb = userconnstr.Substring(0, userconnstr.IndexOf("UID")).Trim
        End If
        Dim tims As String = Now.ToString
        Dim db As String = GetDataBase(userconnstr, userconnprv)
        Dim str As String = String.Empty
        Dim chldtbl As String = String.Empty
        Dim fixedstr As String = String.Empty
        Dim tblid As String = String.Empty
        Dim msql As String = String.Empty
        Dim sqlc As String = String.Empty
        Dim tblorg As String = String.Empty
        tblorg = tbl.ToLower
        tbl = tblname & "" & tbl.ToLower
        If tbl = parenttbl Then
            tbl = tbl & "" & tblorg
        End If
        Dim i, n, j, k, l As Integer
        Dim sqlj As String = "UPDATE [" & tbl & "] SET "
        Dim sqln(0) As String
        j = sqln.Length
        Dim sqli As String = "UPDATE [" & tbl & "] SET "
        m = m + 1
        n = tblname.Length
        If node Is Nothing Then
            Return "done - node Is Nothing"
        End If
        Try
            'recursive for subclasses
            Dim prntlink As String = String.Empty
            Dim bTableExists As Boolean = TableExists(tbl, userconnstr, userconnprv)
            If userconnprv = "MySql.Data.MySqlClient" Then
                db = db.ToLower
            End If
            If Not bTableExists Then
                'create table tbl
                If userconnprv = "MySql.Data.MySqlClient" Then
                    sqlc = "CREATE TABLE `" & db & "`.`" & tbl & "`(parent TEXT NULL DEFAULT NULL,parentid INT NOT NULL DEFAULT 0,timstmp TEXT NULL DEFAULT NULL, ID int(11) NOT NULL AUTO_INCREMENT PRIMARY KEY );"
                ElseIf userconnprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlc = "CREATE TABLE " & tbl & " ( "
                    sqlc &= "parent VARCHAR2(4000 CHAR) DEFAULT NULL,"
                    sqlc &= "parentid NUMBER(11,0) DEFAULT 0 NOT NULL,"
                    sqlc &= "timstmp VARCHAR2(4000 CHAR) DEFAULT NULL,"
                    sqlc &= "ID Integer GENERATED ALWAYS As IDENTITY,"
                    sqlc &= "CONSTRAINT " & tbl.ToUpper & "_PK PRIMARY KEY (ID)"
                    sqlc &= ")"
                ElseIf userconnprv = "System.Data.Odbc" OrElse userconnprv = "System.Data.OleDb" Then
                    'Sql server syntax
                    sqlc = "CREATE TABLE [" & tbl & "] ("
                    'If userconnprv.StartsWith("InterSystems.Data.") Then
                    sqlc = sqlc & " [parent] [nvarchar](4000) NULL,"
                    sqlc = sqlc & " [parentid] [Int] Not NULL Default 0,"
                    sqlc = sqlc & " [timstmp] [nvarchar](4000) NULL,"
                    sqlc = sqlc & " [ID] [Int] IDENTITY(1,1) Not NULL)"
                ElseIf userconnprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlc = "CREATE Table [" & tbl & "]("
                    sqlc = sqlc & " [parent] character varying(4000) NULL,"
                    sqlc = sqlc & " [parentid] integer Not NULL Default 0,"
                    sqlc = sqlc & " [timstmp] character varying(4000) NULL,"
                    sqlc = sqlc & "[ID] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlc = sqlc & "CONSTRAINT [" & tbl & "_pkey] PRIMARY KEY ([ID]))"

                ElseIf userconnprv = "Sqlite" Then  'in memory
                    sqlc = "CREATE Table [" & tbl & "]("
                    sqlc = sqlc & " [parent] character varying(4000) NULL,"
                    sqlc = sqlc & " [parentid] integer Not NULL Default 0,"
                    sqlc = sqlc & " [timstmp] character varying(4000) NULL,"
                    sqlc = sqlc & " [ID] INTEGER PRIMARY KEY AUTOINCREMENT"
                    If m > 1 Then sqlc = sqlc & " , FOREIGN KEY(parentid) REFERENCES [" & parenttbl & "](ID)"  'child table
                    sqlc = sqlc & ")"

                Else 'SQL Server, Cache, or Iris
                    sqlc = "CREATE TABLE [" & tbl & "] ("
                    sqlc = sqlc & " [parent] [nvarchar](4000) NULL,"
                    sqlc = sqlc & " [parentid] [Int] Not NULL Default 0,"
                    sqlc = sqlc & " [timstmp] [nvarchar](4000) NULL,"
                    sqlc = sqlc & " [ID] [Int] IDENTITY(1,1) Not NULL)"
                End If

                ret = ExequteSQLquery(sqlc, userconnstr, userconnprv)
                'forein keys
                If ret = "Query executed fine." AndAlso parenttbl.Trim <> "" AndAlso tbl.Trim <> "" Then
                    ret = CreateForeinKeyRelationshipForTables(rep, parenttbl, "ID", tbl, "parentid", userconnstr, userconnprv)
                End If

                'add table tbl into list for OURUserTables
                ret = InsertTableIntoOURUserTables(tbl, tbl, unit, logon, userdb, "", rep)
                'add join
                If parenttbl.Trim <> "" AndAlso tbl.Trim <> "" AndAlso parenttbl.Trim <> tblname.Trim Then
                    ret = AddJoin(rep, parenttbl, tbl, "ID", "parentid", "INNERJOIN", repdb, "custom", "parent To child", m)
                End If

            End If

            If parenttbl.Trim <> "" AndAlso parentid > 0 Then
                prntlink = parenttbl & "^" '& "ID^" & parentid.ToString & "^prn"
            End If
            'get tblid
            Dim msql1 As String = String.Empty
            msql = ""
            If userconnprv = "MySql.Data.MySqlClient" Then
                msql = "INSERT INTO `" & db & "`.`" & tbl & "` Set parent='" & prntlink & "',parentid=" & parentid.ToString & ",timstmp= '" & tims & "'"
                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                'find last record ID
                msql1 = "SELECT * FROM `" & db & "`.`" & tbl & "` WHERE `parent`='" & prntlink & "' AND `parentid`=" & parentid.ToString & " AND `timstmp`= '" & tims & "'"

            ElseIf userconnprv = "Npgsql" Then
                msql = "INSERT INTO `" & tbl & "` (`parent`,`parentid`,`timstmp`) VALUES('" & prntlink & "','" & parentid.ToString & "','" & tims & "')"
                ret = ExequteSQLquery(msql, userconnstr, userconnprv) '
                msql1 = "SELECT * FROM `" & tbl & "` WHERE `parent`='" & prntlink & "' AND `parentid`=" & parentid.ToString & " AND `timstmp`= '" & tims & "'"

            ElseIf userconnprv = "Oracle.ManagedDataAccess.Client" Then
                Dim sFields As String = "parent,parentid,timstmp"
                Dim sValues As String = "'" & prntlink & "'," & parentid.ToString & ",'" & tims & "'"
                msql = "INSERT INTO " & tbl & " (" & sFields & ") VALUES (" & sValues & ")"
                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                msql1 = "SELECT * FROM " & tbl & " WHERE parent='" & prntlink & "' AND parentid=" & parentid.ToString & " AND timstmp= '" & tims & "'"

            ElseIf userconnprv = "Sqlite" Then
                Dim sFields As String = "parent,parentid,timstmp"
                Dim sValues As String = "'" & prntlink & "'," & parentid.ToString & ",'" & tims & "'"
                msql = "INSERT INTO [" & tbl & "] (" & sFields & ") VALUES (" & sValues & ")"
                er = ExequteSQLquery(msql, userconnstr, userconnprv)
                msql1 = "Select * FROM [" & tbl & "] WHERE parent='" & prntlink & "' AND parentid=" & parentid.ToString & " AND timstmp= '" & tims & "'"

            ElseIf userconnprv = "System.Data.Odbc" OrElse userconnprv = "System.Data.OleDb" Then
                msql = "INSERT INTO " & tbl
                msql &= " SET parent='" & prntlink & "',"
                msql &= "parentid=" & parentid.ToString & ","
                msql &= "timstmp= '" & tims & "'"
                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                'find last record ID
                msql1 = "SELECT * FROM " & tbl & " WHERE parent='" & prntlink & "' AND parentid=" & parentid.ToString & " AND timstmp= '" & tims & "'"

            Else 'SQL Server, Cache, or Iris
                msql = "INSERT INTO [" & tbl
                msql &= "] SET parent='" & prntlink & "',"
                msql &= "parentid=" & parentid.ToString & ","
                msql &= "timstmp= '" & tims & "'"
                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                'find last record ID
                msql1 = "SELECT * FROM [" & tbl & "] WHERE parent='" & prntlink & "' AND parentid=" & parentid.ToString & " AND timstmp= '" & tims & "'"
            End If
            Dim dv As DataView = mRecords(msql1, er, userconnstr, userconnprv)
            If dv Is Nothing OrElse dv.Table Is Nothing Then
                Return "done - table is not created"
            End If

            If dv.Table.Rows.Count = 0 Then
                tblid = 0
            Else
                tblid = dv.Table.Rows(0)("ID")
            End If


            'add attributes as fields in tbl and insert their values 
            For i = 0 To node.Attributes.Count - 1
                str = node.Attributes(i).Name.Replace(":", "").Replace("#", "")
                fixedstr = FixReservedWords(str, userconnprv)

                If userconnprv = "MySql.Data.MySqlClient" Then
                    sqlc = "ALTER TABLE `" & db & "`.`" & tbl & "` ADD COLUMN `" & str & "` TEXT NULL DEFAULT NULL ;"
                ElseIf userconnprv = "Npgsql" Then
                    sqlc = "ALTER TABLE `" & tbl & "` ADD COLUMN `" & fixedstr & "` TEXT NULL DEFAULT NULL ;"
                ElseIf userconnprv = "Sqlite" Then
                    sqlc = "ALTER TABLE [" & tbl & "] ADD COLUMN " & fixedstr & " TEXT NULL DEFAULT NULL ;"
                ElseIf userconnprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlc = "ALTER TABLE " & tbl & " ADD " & fixedstr & " VARCHAR2(4000 CHAR) DEFAULT NULL"
                ElseIf userconnprv = "System.Data.Odbc" OrElse userconnprv = "System.Data.OleDb" Then
                    sqlc = "ALTER TABLE " & tbl & " ADD [" & str & "] VARCHAR(4000) DEFAULT NULL"
                ElseIf userconnprv.StartsWith("InterSystems.Data.") Then
                    sqlc = "ALTER TABLE " & tbl & " ADD [" & str & "] VARCHAR(4000) NULL DEFAULT NULL"
                Else
                    sqlc = "ALTER TABLE [" & tbl & "] ADD [" & str & "] TEXT NULL DEFAULT NULL"
                End If
                ret = ExequteSQLquery(sqlc, userconnstr, userconnprv)

                j = sqln.Length
                ReDim Preserve sqln(j)
                sqln(j) = "[" & fixedstr & "]='" & node.Attributes(i).Value.ToString & "'"
                sqlc = sqlj & sqln(j) & " WHERE [ID]=" & tblid
                ret = ExequteSQLquery(sqlc, userconnstr, userconnprv)
            Next

            'TODO check if columns has duplicate names
            n = node.ChildNodes.Count - 1
            Dim strfix(n) As String
            For i = 0 To node.ChildNodes.Count - 1
                str = node.ChildNodes(i).Name.Replace(":", "").Replace("#", "")
                If str.Trim = "" Then
                    str = "column" & "" & i.ToString
                End If
                If funiq = False AndAlso node.ChildNodes(i).ChildNodes.Count = 1 AndAlso Not node.ChildNodes(i).FirstChild.Value Is Nothing Then 'simple node
                    l = 0
                    For k = 0 To i - 1
                        If str = node.ChildNodes(k).Name.Replace(":", "").Replace("#", "") AndAlso funiq = False AndAlso node.ChildNodes(k).ChildNodes.Count = 1 AndAlso Not node.ChildNodes(k).FirstChild.Value Is Nothing Then
                            strfix(k) = str & "" & l.ToString
                            l = l + 1
                        End If
                    Next
                    If l > 0 Then
                        strfix(i) = str & "" & l.ToString
                    Else
                        strfix(i) = str
                    End If
                Else
                    strfix(i) = str
                End If
            Next

            'nodes
            For i = 0 To node.ChildNodes.Count - 1
                str = strfix(i)
                fixedstr = FixReservedWords(str, userconnprv)
                If Not IsColumnFromTable(tbl, str, userconnstr, userconnprv, er) Then
                    If userconnprv = "MySql.Data.MySqlClient" Then
                        sqlc = "ALTER TABLE `" & db & "`.`" & tbl & "` ADD COLUMN `" & fixedstr & "` TEXT NULL DEFAULT NULL ;"
                    ElseIf userconnprv = "Npgsql" Then
                        sqlc = "ALTER TABLE `" & tbl & "` ADD COLUMN `" & fixedstr & "` TEXT NULL DEFAULT NULL ;"
                    ElseIf userconnprv = "Sqlite" Then
                        sqlc = "ALTER TABLE [" & tbl & "] ADD COLUMN " & fixedstr & " TEXT NULL DEFAULT NULL ;"
                    ElseIf userconnprv = "Oracle.ManagedDataAccess.Client" Then
                        sqlc = "ALTER TABLE " & tbl & " ADD " & fixedstr & " VARCHAR2(4000 CHAR) DEFAULT NULL"
                    ElseIf userconnprv = "System.Data.Odbc" OrElse userconnprv = "System.Data.OleDb" Then
                        sqlc = "ALTER TABLE " & tbl & " ADD [" & fixedstr & "] VARCHAR(10000) DEFAULT NULL"
                    ElseIf userconnprv.StartsWith("InterSystems.Data.") Then
                        sqlc = "ALTER TABLE " & tbl & " ADD [" & fixedstr & "] VARCHAR(10000) NULL DEFAULT NULL"
                    Else
                        sqlc = "ALTER TABLE [" & tbl & "] ADD [" & fixedstr & "] TEXT NULL DEFAULT NULL"
                    End If
                    ret = ExequteSQLquery(sqlc, userconnstr, userconnprv)
                End If
                j = sqln.Length
                ReDim Preserve sqln(j)
                'fixedstr = FixReservedWords(str, userconnprv)
                If node.ChildNodes(i).ChildNodes.Count = 0 AndAlso node.ChildNodes(i).InnerText.Trim = "" Then
                    sqln(j) = fixedstr & "=' '"
                    sqlc = sqlj & sqln(j) & " WHERE [ID]=" & tblid
                    ret = ExequteSQLquery(sqlc, userconnstr, userconnprv)

                ElseIf node.ChildNodes(i).ChildNodes.Count = 1 AndAlso Not node.ChildNodes(i).FirstChild.Value Is Nothing Then
                    sqln(j) = fixedstr & "='" & node.ChildNodes(i).FirstChild.Value.ToString & "'"
                    sqlc = sqlj & sqln(j) & " WHERE [ID]=" & tblid
                    ret = ExequteSQLquery(sqlc, userconnstr, userconnprv)
                    If ret <> "Query executed fine." Then
                        sqlc = sqlj & sqln(j).Replace("'", "\\").Replace("""", """""") & " WHERE ID=" & tblid
                        ret = ExequteSQLquery(sqlc, userconnstr, userconnprv)
                    End If
                Else    'If node.ChildNodes(i).ChildNodes.Count > 1 Then has children
                    If str.ToLower = tblorg Then
                        chldtbl = tbl & "" & tblorg
                    Else
                        chldtbl = tblname & "" & str.ToLower
                    End If
                    sqln(j) = fixedstr & "='" & chldtbl & "~'" '& "~parentid~" & tblid & "~cld~" & tbl & "~parent'"
                    sqlc = sqlj & sqln(j) & " WHERE [ID]=" & tblid
                    ret = ExequteSQLquery(sqlc, userconnstr, userconnprv)

                    'go to the deeper level async
                    Dim result As String = Await ProcessXMLJSONNodeIntoTable(rep, m, tblname, node.ChildNodes(i), str, tbl, tblid, userconnstr, userconnprv, unit, logon, userdb, er, funiq)
                    Threading.Thread.Sleep(1000)

                End If
            Next
            Return "done " & ret
        Catch ex As Exception
            er = "ERROR!! " & ex.Message
            Return "done - " & er
        End Try
    End Function
    Public Function CreateForeinKeyRelationshipForTables(ByVal rep As String, ByVal ParentTable As String, ByVal pfield As String, ByVal ChildTable As String, ByVal cfield As String, ByVal userconnstr As String, ByVal userconnprv As String) As String
        'this is needed for CSV user, and userconnstr is CSV database, in our case always MySql for now...
        Dim ret As String = String.Empty
        Dim er As String = String.Empty
        Dim j, m As Integer
        Dim sqlq As String = String.Empty
        Dim KeyName As String = ChildTable & "CID"
        Dim dvkc As DataView = Nothing

        Try
            If userconnprv = "MySql.Data.MySqlClient" Then
                Dim dbname As String = GetDataBase(userconnstr, userconnprv).ToLower
                Dim sqlkeycol As String = " Select * From INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA='" & dbname & "' AND REFERENCED_TABLE_SCHEMA='" & dbname & "'  AND TABLE_NAME='" & ParentTable & "' AND REFERENCED_TABLE_NAME='" & ChildTable & "'   AND COLUMN_NAME='" & pfield & "' AND REFERENCED_COLUMN_NAME='" & cfield & "' ORDER BY TABLE_NAME, REFERENCED_TABLE_NAME, ORDINAL_POSITION"
                er = ""
                dvkc = mRecords(sqlkeycol, er, userconnstr, userconnprv)
                If dvkc Is Nothing OrElse dvkc.Count = 0 OrElse dvkc.Table.Rows.Count = 0 Then
                    'forein key does not exist
                    sqlq = "ALTER Table `" & dbname & "`.`" & ChildTable & "` ADD CONSTRAINT " & ChildTable & "CID" & " FOREIGN KEY " & ChildTable & "CID" & "(" & cfield & ") REFERENCES " & ParentTable & "(" & pfield & ") On DELETE NO ACTION On UPDATE NO ACTION"
                    ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
                Else
                    'forein key exists
                    Return ""
                End If

            ElseIf userconnprv.StartsWith("InterSystems.Data.") Then

                sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS  WHERE TABLE_NAME='" & ChildTable & "'  AND CONSTRAINT_NAME ='" & KeyName & "'"
                dvkc = mRecords(sqlq, er, userconnstr, userconnprv)
                If dvkc Is Nothing OrElse dvkc.Count = 0 OrElse dvkc.Table.Rows.Count = 0 Then
                    'foreign key does not exist
                    sqlq = "ALTER TABLE " & ChildTable
                    sqlq &= " ADD CONSTRAINT " & KeyName
                    sqlq &= "( FOREIGN KEY (" & cfield & ") REFERENCES " & ParentTable & "(" & pfield & ") ON DELETE NO ACTION ON UPDATE NO ACTION)"
                    ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
                Else
                    'foreign key exists
                    Return ""
                End If

            ElseIf userconnprv = "Npgsql" Then  'PostgreSQL  Npgsql
                Dim dbname As String = GetDataBase(userconnstr, userconnprv)
                sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS  WHERE TABLE_SCHEMA ='public' AND LOWER(TABLE_CATALOG) ='" & dbname.ToLower & "' AND TABLE_NAME='" & ChildTable & "'  AND CONSTRAINT_NAME ='" & KeyName & "'"
                dvkc = mRecords(sqlq, er, userconnstr, userconnprv)
                If dvkc Is Nothing OrElse dvkc.Count = 0 OrElse dvkc.Table.Rows.Count = 0 Then
                    'foreign key does not exist
                    sqlq = "ALTER TABLE '" & ChildTable & "`"
                    sqlq &= " ADD CONSTRAINT `" & KeyName & "`"
                    sqlq &= " FOREIGN KEY ('" & cfield & "') REFERENCES '" & ParentTable & "'('" & pfield & "') ON DELETE NO ACTION ON UPDATE NO ACTION "
                    ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
                Else
                    'foreign key exists
                    Return ""
                End If

            ElseIf userconnprv = "Sqlite" Then

                'forein key created when child table was created

            ElseIf userconnprv = "System.Data.SqlClient" Then
                sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS  WHERE TABLE_NAME='" & ChildTable & "'  AND CONSTRAINT_NAME ='" & KeyName & "'"
                dvkc = mRecords(sqlq, er, userconnstr, userconnprv)
                If dvkc Is Nothing OrElse dvkc.Count = 0 OrElse dvkc.Table.Rows.Count = 0 Then
                    'foreign key does not exist
                    sqlq = "ALTER TABLE " & ChildTable
                    sqlq &= " ADD CONSTRAINT " & KeyName
                    sqlq &= " FOREIGN KEY (" & cfield & ") REFERENCES " & ParentTable & "(" & pfield & ") ON DELETE NO ACTION ON UPDATE NO ACTION "
                    ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
                Else
                    'foreign key exists
                    Return ""
                End If

            ElseIf userconnprv = "Oracle.ManagedDataAccess.Client" Then

                sqlq = "SELECT * FROM user_constraints  WHERE CONSTRAINT_TYPE='R' And TABLE_NAME='" & ChildTable.ToUpper & "' And CONSTRAINT_NAME='" & KeyName & "'"
                dvkc = mRecords(sqlq, er, userconnstr, userconnprv)
                If dvkc Is Nothing OrElse dvkc.Count = 0 OrElse dvkc.Table.Rows.Count = 0 Then
                    'foreign key does not exist
                    sqlq = "ALTER TABLE " & ChildTable
                    sqlq &= " ADD CONSTRAINT " & KeyName
                    sqlq &= " FOREIGN KEY (" & cfield & ") REFERENCES " & ParentTable & "(" & pfield & ") ENABLE;"
                    ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
                Else
                    'foreign key exists
                    Return ""
                End If

            ElseIf userconnprv = "System.Data.Odbc" Then
                'ODBC - create index!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                Dim spnames(2) As String
                spnames(2) = ChildTable
                Dim dtx As DataTable
                dtx = GetListByODBC(System.Data.Odbc.OdbcMetaDataCollectionNames.Indexes, userconnstr, userconnprv, spnames)
                dvkc = dtx.DefaultView
                dvkc.RowFilter = "COLUMN_NAME='" & cfield & "'"
                'dtx = dvkc.ToTable

                If dvkc Is Nothing OrElse dvkc.Count = 0 OrElse dvkc.Table.Rows.Count = 0 Then
                    'foreign key does not exist
                    sqlq = "ALTER TABLE " & ChildTable
                    sqlq &= " ADD CONSTRAINT " & KeyName
                    sqlq &= " FOREIGN KEY (" & cfield & ") REFERENCES " & ParentTable & "(" & pfield & ")"
                    ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
                Else
                    'foreign key exists
                    Return ""
                End If

            ElseIf userconnprv = "System.Data.OleDb" Then
                'OleDb - create index!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                Dim spnames(2) As String
                spnames(2) = ChildTable
                Dim dtx As DataTable
                dtx = GetListByODBC(System.Data.Odbc.OdbcMetaDataCollectionNames.Indexes, userconnstr, userconnprv, spnames)
                dvkc = dtx.DefaultView
                dvkc.RowFilter = "COLUMN_NAME='" & cfield & "'"
                'dtx = dvkc.ToTable

                If dvkc Is Nothing OrElse dvkc.Count = 0 OrElse dvkc.Table.Rows.Count = 0 Then
                    'foreign key does not exist
                    sqlq = "ALTER TABLE " & ChildTable
                    sqlq &= " ADD CONSTRAINT " & KeyName
                    sqlq &= " FOREIGN KEY (" & cfield & ") REFERENCES " & ParentTable & "(" & pfield & ")"
                    ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
                Else
                    'foreign key exists
                    Return ""
                End If
            Else

            End If
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function

    'Public Function CreateDatabaseTablesForDataSet(ByVal tblname() As String, ByVal dset As DataSet, ByVal userconnstr As String, ByVal userconnprv As String, Optional ByRef er As String = "") As String
    '    'Not IN USE

    '    Dim ret As String = String.Empty
    '    Dim j, m As Integer
    '    Dim sqlq As String = String.Empty
    '    Try
    '        'create db 
    '        Dim db As String = GetDataBase(userconnstr).ToLower
    '        If userconnprv = "MySql.Data.MySqlClient" Then
    '            sqlq = "Select * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME='" & db & "'"
    '            If Not HasRecords(sqlq, userconnstr, userconnprv) Then
    '                sqlq = "CREATE DATABASE `" & db & "`"
    '                ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
    '                If ret = "Query executed fine." Then
    '                    ret = "" ' ret & "<br/> " & db & " created. "
    '                Else
    '                    ret = ret & "<br/> " & db & " not created: " & ret & " "
    '                    Return ret
    '                End If
    '            End If
    '            m = dset.Tables.Count
    '            For j = 0 To m - 1
    '                'create table
    '                ret = ImportDataTableIntoMySql(dset.Tables(j), tblname(j), userconnstr, userconnprv)
    '            Next
    '            'TODO make relations

    '        Else
    '            'TODO other providers

    '        End If
    '        Return ret.Replace("Query executed fine.", "")
    '    Catch ex As Exception
    '        ret = ex.Message
    '    End Try
    '    Return ret
    'End Function
    'Public Function ReadDataSetFromXML(ByVal XmlFile As String, ByVal userconnstr As String, ByVal userconnprv As String, Optional er As String = "") As DataSet
    '    Dim ret As String = String.Empty
    '    If XmlFile = String.Empty OrElse File.Exists(XmlFile) = False Then
    '        Return Nothing
    '        Exit Function
    '    End If
    '    Dim dset As DataSet
    '    dset = New DataSet
    '    Dim str As String = String.Empty
    '    Dim tbl1 As String = String.Empty
    '    Dim str2 As String = String.Empty
    '    Dim temp() As String
    '    Try
    '        Dim fstr As New System.IO.StreamReader(XmlFile)
    '        'dset.ReadXml(fstr)
    '        str = fstr.ReadLine.Trim
    '        If str.StartsWith("<?") Then
    '            str = fstr.ReadLine.Trim
    '        End If
    '        temp = str.Split(" ")
    '        tbl1 = temp(0).Replace("<", "").Replace(">", "")
    '        ret = ImportXMLIntoTable(fstr, str, tbl1, temp, userconnstr, userconnprv, er)


    '        Return dset
    '    Catch ex As Exception
    '        er = "Close XML file first." & ex.Message
    '        Return Nothing
    '    End Try
    'End Function

    'Public Function ImportXMLIntoTable(ByRef fstr As StreamReader, ByVal str As String, ByVal tbl As String, ByVal rowstr() As String, ByVal userconnstr As String, ByVal userconnprv As String, Optional er As String = "") As String
    '    'recursive
    '    Dim ret As String = String.Empty
    '    Dim sqlq As String = String.Empty
    '    If fstr.EndOfStream Then
    '        Return Nothing
    '    End If
    '    Try
    '        'create table tbl

    '        'create db 
    '        Dim db As String = GetDataBase(userconnstr).ToLower
    '        If userconnprv = "MySql.Data.MySqlClient" Then
    '            sqlq = "Select * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME='" & db & "'"
    '            If Not HasRecords(sqlq, userconnstr, userconnprv) Then
    '                sqlq = "CREATE DATABASE `" & db & "`"
    '                ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
    '                If ret = "Query executed fine." Then
    '                    ret = "" ' ret & "<br/> " & db & " created. "
    '                Else
    '                    ret = ret & "<br/> " & db & " not created: " & ret & " "
    '                    Return ret
    '                End If
    '            End If
    '        End If

    '        'add fields from rowstr()

    '        'recursive for subclasses

    '        'make relationship/forein keys

    '        'run msql inserting values in tbl


    '        Return ret
    '    Catch ex As Exception
    '        er = ex.Message
    '        Return Nothing
    '    End Try
    'End Function
    'Public Function ImportXMLrowfieldIntoRow(ByRef fstr As StreamReader, ByVal tbl As String, ByVal row As String, ByVal fld As String, Optional er As String = "") As DataRow
    '    Dim ret As String = String.Empty
    '    If fstr.EndOfStream Then
    '        Return Nothing
    '    End If
    '    Dim drow As DataRow
    '    Try
    '        Return drow
    '    Catch ex As Exception
    '        er = ex.Message
    '        Return Nothing
    '    End Try
    'End Function
    Public Function FieldPointsToParentClass(ByVal fld As String, ByVal tbl As String, ByRef prf As String, ByVal userconnstr As String, ByVal userconnprv As String) As String
        Dim sqlt As String = String.Empty
        Dim er As String = String.Empty
        Try
            If userconnprv.ToString.StartsWith("InterSystems.Data.") Then
                'tbl is persistent class
                sqlt = "SELECT * FROM %Dictionary.PropertyDefinition WHERE parent='" & tbl & "' AND Name='" & fld & "' AND Relationship=1 AND Cardinality='parent'"
                Dim dvp As DataView
                dvp = mRecords(sqlt, er, userconnstr, userconnprv)
                If er <> "" Then
                    Return "ERROR!! " & er
                End If
                If Not dvp Is Nothing AndAlso dvp.Count > 0 AndAlso dvp.Table.Rows.Count > 0 Then
                    prf = dvp.Table.Rows(0)("Inverse").ToString
                    Return dvp.Table.Rows(0)("Type").ToString
                Else
                    Return ""
                End If
            End If
        Catch ex As Exception
            Return "ERROR!! " & ex.Message
        End Try
        Return er
    End Function

    Public Function AddRelationshipColumnsToDataTable(ByVal dv As DataView, ByVal tbl As String, ByVal dbname As String, ByVal userconnstr As String, ByVal userconnprv As String, Optional ByRef syschk As Boolean = False) As DataView
        'USED ONLY FOR InterSystems.Data and maybe for Oracle(?) !!!
        'add cardinality or forein keys fields to dt, loop in property definition class records for this class.
        Dim er As String = String.Empty
        Dim sqlt As String = String.Empty
        Dim ret As String = String.Empty
        Dim tbl1 As String = String.Empty
        Dim fld1 As String = String.Empty
        Dim tbl2 As String = String.Empty
        Dim fld2 As String = String.Empty
        Dim dt As DataTable = dv.Table
        Dim i, j As Integer
        Dim col As DataColumn
        Try
            If userconnprv.ToString.StartsWith("InterSystems.Data.") Then
                'tbl is persistent class
                sqlt = "SELECT * FROM %Dictionary.PropertyDefinition WHERE parent='" & tbl & "' AND Relationship=1"
                Dim dvp As DataView
                dvp = mRecords(sqlt, er, userconnstr, userconnprv)
                If Not dvp Is Nothing AndAlso dvp.Count > 0 AndAlso dvp.Table.Rows.Count > 0 Then
                    For i = 0 To dvp.Table.Rows.Count - 1
                        Try
                            If dvp.Table.Rows(i)("Cardinality").ToString = "children" OrElse dvp.Table.Rows(i)("Cardinality").ToString = "many" Then
                                tbl1 = dvp.Table.Rows(i)("parent").ToString  '=tbl
                                tbl2 = dvp.Table.Rows(i)("Type").ToString
                                'fld1 = "ID"
                                fld1 = dvp.Table.Rows(i)("Name").ToString
                                fld2 = dvp.Table.Rows(i)("Inverse").ToString
                                If IsCacheClassPersistant(tbl2, userconnstr, userconnprv, er, syschk) Then
                                    'add column (child)
                                    col = New DataColumn
                                    col.DataType = System.Type.GetType("System.String")
                                    col.ColumnName = fld1
                                    col.Caption = fld1
                                    dt.Columns.Add(col)
                                    'fill out that column 
                                    For j = 0 To dt.Rows.Count - 1
                                        dt.Rows(j)(fld1) = dvp.Table.Rows(i)("Type").ToString & "~" & dvp.Table.Rows(i)("Inverse").ToString & "~" & dt.Rows(j)("ID") & "~cld"
                                    Next
                                End If
                            ElseIf dvp.Table.Rows(i)("Cardinality").ToString = "parent" OrElse dvp.Table.Rows(i)("Cardinality").ToString = "one" Then
                                tbl1 = dvp.Table.Rows(i)("parent").ToString
                                fld1 = dvp.Table.Rows(i)("Name").ToString
                                tbl2 = dvp.Table.Rows(i)("Type").ToString
                                fld2 = "ID"
                                'dt.Columns(fld1).DataType = System.Type.GetType("System.String")
                                If IsCacheClassPersistant(tbl2, userconnstr, userconnprv, er, syschk) Then
                                    'add column (parent)
                                    col = New DataColumn
                                    col.DataType = System.Type.GetType("System.String")
                                    col.ColumnName = "Parent_" & fld1
                                    col.Caption = "Parent_" & fld1
                                    dt.Columns.Add(col)
                                    'fill out that column 
                                    For j = 0 To dt.Rows.Count - 1
                                        dt.Rows(j)("Parent_" & fld1) = dvp.Table.Rows(i)("Type").ToString & "^" & dvp.Table.Rows(i)("Inverse").ToString & "^" & dt.Rows(j)(fld1) & "^prn"
                                    Next
                                End If
                            End If
                        Catch ex As Exception
                            ret = ex.Message
                        End Try
                    Next
                End If

                'TODO maybe not needed:  ElseIf userconnprv = "Npgsql" Then  'PostgreSQL  Npgsql

            ElseIf userconnprv = "System.Data.SqlClient" Then
                ''TODO fix it, NOT TO DELETE !!!!!!!!!!!!!!:
                ''create columns with links from forein indexes
                'Dim sqlrefcons As String = " Select * From INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINTS_SCHEMA='" & dbname & "' AND UNIQUE_CONSTRAINTS_SCHEMA='" & dbname & "'"
                'Dim dvrc As DataView = mRecords(sqlrefcons, er, userconnstr, userconnprv)
                'If dvrc Is Nothing OrElse dvrc.Count = 0 OrElse dvrc.Table.Rows.Count = 0 Then
                '    Return dv
                'End If
                'Dim sqlcolusg As String = "Select * From INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WHERE TABLE_SCHEMA='" & dbname & "' "
                'Dim fk, pk As String
                'er = ""
                'Dim dvtc As DataView
                'For i = 0 To dvrc.Table.Rows.Count - 1
                '    fk = dvrc.Table.Rows(i)("CONSTRAINT_NAME")
                '    pk = dvrc.Table.Rows(i)("UNIQUE_CONSTRAINT_NAME")
                '    dvtc = mRecords(sqlcolusg & " AND CONSTRAINT_NAME='" & fk & "'", er, userconnstr, userconnprv)
                '    If dvtc Is Nothing OrElse dvtc.Count = 0 OrElse dvtc.Table.Rows.Count = 0 Then
                '        Continue For
                '    End If
                '    tbl1 = dvtc.Table.Rows(i)("TABLE_NAME")
                '    fld1 = dvtc.Table.Rows(i)("COLUMN_NAME")
                '    dvtc = mRecords(sqlcolusg & " AND CONSTRAINT_NAME='" & pk & "'", er, userconnstr, userconnprv)
                '    If dvtc Is Nothing OrElse dvtc.Count = 0 OrElse dvtc.Table.Rows.Count = 0 Then
                '        Continue For
                '    End If
                '    tbl2 = dvtc.Table.Rows(i)("TABLE_NAME")
                '    fld2 = dvtc.Table.Rows(i)("COLUMN_NAME")

                '    If tbl1 = tbl Then
                '        'add column (parent)
                '        col = New DataColumn
                '        col.DataType = System.Type.GetType("System.String")
                '        col.ColumnName = "Parent_" & fld1
                '        col.Caption = "Parent_" & fld1
                '        dt.Columns.Add(col)
                '        'fill out that column 
                '        For j = 0 To dt.Rows.Count - 1
                '            dt.Rows(j)("Parent_" & fld1) = tbl2 & "^" & fld2 & "^" & dt.Rows(j)(fld1) & "^prn"
                '        Next
                '    ElseIf tbl2 = tbl Then
                '        'add column (child)
                '        col = New DataColumn
                '        col.DataType = System.Type.GetType("System.String")
                '        col.ColumnName = fld1
                '        col.Caption = fld1
                '        dt.Columns.Add(col)
                '        'fill out that column 
                '        For j = 0 To dt.Rows.Count - 1
                '            dt.Rows(j)(fld1) = tbl2 & "~" & fld2 & "~" & dt.Rows(j)("ID") & "~cld~" & tbl1
                '        Next
                '    End If
                'Next
            ElseIf userconnprv = "MySql.Data.MySqlClient" Then
                ''TODO fix it, NOT TO DELETE !!!!!!!!!!!!!!:
                ''create columns with links from forein indexes
                'Dim sqlkeycol As String = " Select * From INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA='" & dbname & "' AND REFERENCED_TABLE_SCHEMA='" & dbname & "' ORDER BY TABLE_NAME, REFERENCED_TABLE_NAME, ORDINAL_POSITION"
                'er = ""
                'Dim dvkc As DataView = mRecords(sqlkeycol, er, userconnstr, userconnprv)
                'If dvkc Is Nothing OrElse dvkc.Count = 0 OrElse dvkc.Table.Rows.Count = 0 Then
                '    Return dv
                'End If
                'For i = 0 To dvkc.Table.Rows.Count - 1
                '    tbl1 = dvkc.Table.Rows(i)("TABLE_NAME")
                '    fld1 = dvkc.Table.Rows(i)("COLUMN_NAME")
                '    tbl2 = dvkc.Table.Rows(i)("REFERENCED_TABLE_NAME")
                '    fld2 = dvkc.Table.Rows(i)("REFERENCED_COLUMN_NAME")

                '    If tbl2.ToUpper = tbl.ToUpper Then
                '        'add column (parent)
                '        col = New DataColumn
                '        col.DataType = System.Type.GetType("System.String")
                '        col.ColumnName = "Parent_" & tbl1 & "_" & fld1
                '        col.Caption = "Parent_" & tbl1 & "_" & fld1
                '        dt.Columns.Add(col)
                '        'fill out that column 
                '        For j = 0 To dt.Rows.Count - 1
                '            dt.Rows(j)("Parent_" & tbl1 & "_" & fld1) = tbl2 & "^" & fld2 & "^" & dt.Rows(j)(fld1) & "^prn" ' ^" & tbl1 & "^" & fld1
                '        Next
                '    ElseIf tbl1.ToUpper = tbl.ToUpper Then
                '        'add column (child)
                '        col = New DataColumn
                '        col.DataType = System.Type.GetType("System.String")
                '        col.ColumnName = "Children_" & fld1
                '        col.Caption = "Children_" & fld1
                '        dt.Columns.Add(col)
                '        'fill out that column 
                '        For j = 0 To dt.Rows.Count - 1
                '            dt.Rows(j)("Children_" & fld1) = tbl2 & "~" & fld2 & "~" & dt.Rows(j)("ID") & "~cld~" & tbl1 & "~" & fld1
                '        Next
                '    End If

                'Next
            ElseIf userconnprv = "System.Data.Odbc" Then
                'Not for ODBC for now, not needed.  GetSchema("indexes")
                'dt = GetListByODBC(System.Data.Odbc.OdbcMetaDataCollectionNames.Indexes, userconnstr, userconnprv)
                'Dim dv As DataView = dt.DefaultView
                'dv.RowFilter = "UCASE(TABLE_NAME)=UCASE('" & ChildTable & "') AND UCASE(COLUMN_NAME)=UCASE('" & KeyName & "')"  'Is Primary?
                'dt = dv.ToTable
                'If dt.Rows.Count = 0 Then
                '    'create index field
                'End If
            ElseIf userconnprv = "System.Data.OleDb" Then

            ElseIf userconnprv = "Oracle.ManagedDataAccess.Client" Then
                'for Oracle.ManagedDataAccess.Client use r-constraints
                er = ""
                Dim sqlconsts As String = "select * from user_constraints WHERE constraint_type='R' order by table_name "
                Dim dvrc As DataView = mRecords(sqlconsts, er, userconnstr, userconnprv)
                If dvrc Is Nothing OrElse dvrc.Count = 0 OrElse dvrc.Table.Rows.Count = 0 Then
                    Return dv
                End If
                Dim sqlcolusg As String = "select * from user_cons_columns "
                Dim fk, pk As String
                er = ""
                Dim dvtc As DataView
                For i = 0 To dvrc.Table.Rows.Count - 1
                    fk = dvrc.Table.Rows(i)("CONSTRAINT_NAME")
                    pk = dvrc.Table.Rows(i)("R_CONSTRAINT_NAME")
                    dvtc = mRecords(sqlcolusg & " WHERE CONSTRAINT_NAME='" & fk & "'", er, userconnstr, userconnprv)
                    If dvtc Is Nothing OrElse dvtc.Count = 0 OrElse dvtc.Table.Rows.Count = 0 Then
                        Continue For
                    End If
                    tbl1 = dvtc.Table.Rows(i)("TABLE_NAME")
                    fld1 = dvtc.Table.Rows(i)("COLUMN_NAME")
                    dvtc = mRecords(sqlcolusg & " WHERE CONSTRAINT_NAME='" & pk & "'", er, userconnstr, userconnprv)
                    If dvtc Is Nothing OrElse dvtc.Count = 0 OrElse dvtc.Table.Rows.Count = 0 Then
                        Continue For
                    End If
                    tbl2 = dvtc.Table.Rows(i)("TABLE_NAME")
                    fld2 = dvtc.Table.Rows(i)("COLUMN_NAME")

                    If tbl1 = tbl Then
                        'add column (parent)
                        col = New DataColumn
                        col.DataType = System.Type.GetType("System.String")
                        col.ColumnName = "Parent_" & fld1
                        col.Caption = "Parent_" & fld1
                        dt.Columns.Add(col)
                        'fill out that column 
                        For j = 0 To dt.Rows.Count - 1
                            dt.Rows(j)("Parent_" & fld1) = tbl2 & "^" & fld2 & "^" & dt.Rows(j)(fld1) & "^prn"
                        Next
                    ElseIf tbl2 = tbl Then
                        'add column (child)
                        col = New DataColumn
                        col.DataType = System.Type.GetType("System.String")
                        col.ColumnName = fld1
                        col.Caption = fld1
                        dt.Columns.Add(col)
                        'fill out that column 
                        For j = 0 To dt.Rows.Count - 1
                            dt.Rows(j)(fld1) = tbl2 & "~" & fld2 & "~" & dt.Rows(j)("ID") & "~cld~" & tbl1
                        Next
                    End If

                Next
            End If
            dv = dt.DefaultView
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return dv
    End Function
    'Public Function CreateOURComparisonTable(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
    '    'NOT IN USE
    '    Dim ret As String = String.Empty
    '    Dim err As String = String.Empty
    '    Dim sqlq As String = String.Empty
    '    Try
    '        If myconstr = "" Then
    '            myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
    '            myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
    '        End If
    '        If TableExists("ourcomparison", myconstr, myconprv, err) Then
    '            ret = "Table exists"
    '        Else
    '            Dim db As String = GetDataBase(myconstr, myconprv).ToLower
    '            'create table OURPermits
    '            If myconprv = "MySql.Data.MySqlClient" Then
    '                sqlq = "CREATE TABLE `" & db & "`.`ourcomparison` ("
    '                sqlq = sqlq & " `Feature` VARCHAR( 250 ) Not NULL ,"
    '                sqlq = sqlq & " `Comments` VARCHAR( 120 ) NULL ,"
    '                sqlq = sqlq & " `OUReports` BOOL NULL ,"
    '                sqlq = sqlq & " `MSPowerBI` BOOL NULL ,"
    '                sqlq = sqlq & " `GoogleReports` BOOL NULL ,"
    '                sqlq = sqlq & " `MidasPlus` BOOL NULL ,"
    '                sqlq = sqlq & " `Indx` SMALLINT Not NULL AUTO_INCREMENT ,"
    '                sqlq = sqlq & " PRIMARY KEY(  `Indx` ),"
    '                sqlq = sqlq & " Index(  `Indx` )"
    '                sqlq = sqlq & " )"
    '            End If
    '            ret = ExequteSQLquery(sqlq, myconstr, myconprv)
    '        End If
    '    Catch ex As Exception
    '        ret = ex.Message
    '    End Try
    '    Return ret
    'End Function
    Public Function CorrectRecordOrder(ByVal dt As DataTable, ByVal orderfld As String) As DataTable
        Dim dtt As DataTable = dt
        Dim i As Integer
        'reorder
        For i = 0 To dtt.Rows.Count - 1
            dtt.Rows(i)(orderfld) = i + 1
        Next
        Return dtt
    End Function
    Public Function UpRowInDataTable(ByVal dt As DataTable, ByVal orderfld As String, ByVal uprowid As String) As DataTable
        Dim exm As String = String.Empty
        Dim ord1 As Integer = 0
        Dim ord2 As Integer = 0
        CorrectRecordOrder(dt, orderfld)
        Try
            ord1 = dt.Rows(uprowid - 1)(orderfld)
            ord2 = dt.Rows(uprowid)(orderfld)
            dt.Rows(uprowid - 1)(orderfld) = ord2
            dt.Rows(uprowid)(orderfld) = ord1
        Catch ex As Exception
            exm = ex.Message
        End Try
        Return dt
    End Function
    Public Function DownRowInDataTable(ByVal dt As DataTable, ByVal orderfld As String, ByVal uprowid As String) As DataTable
        Dim exm As String = String.Empty
        Dim ord1 As Integer = 0
        Dim ord2 As Integer = 0
        CorrectRecordOrder(dt, orderfld)
        Try
            ord1 = dt.Rows(uprowid + 1)(orderfld)
            ord2 = dt.Rows(uprowid)(orderfld)
            dt.Rows(uprowid + 1)(orderfld) = ord2
            dt.Rows(uprowid)(orderfld) = ord1
        Catch ex As Exception
            exm = ex.Message
        End Try
        Return dt
    End Function
    Public Function UpdateRecordOrderInDB(ByVal tbl As String, ByVal fldord As String, ByVal fldindx As String, ByVal fldindxtyp As String, ByVal dt As DataTable, Optional ByVal fld As String = "", Optional ByVal fldvalue As String = "", Optional ByVal fldtyp As String = "") As String
        Try
            Dim i As Integer
            Dim sqly As String = String.Empty
            Dim ret As String = String.Empty
            For i = 0 To dt.Rows.Count - 1
                sqly = "UPDATE " & tbl & " SET " & fldord & "=" & dt.Rows(i)(fldord).ToString & " WHERE " & fldindx & "="
                If fldindxtyp = "int" Then
                    sqly = sqly & dt.Rows(i)(fldindx).ToString
                Else
                    sqly = sqly & "'" & dt.Rows(i)(fldindx).ToString & "'"
                End If

                If fld <> "" AndAlso fldvalue <> "" AndAlso fldtyp = "int" Then
                    sqly = sqly & " AND " & fld & "=" & fldvalue
                ElseIf fld <> "" AndAlso fldvalue <> "" AndAlso fldtyp <> "int" Then
                    sqly = sqly & " AND " & fld & "='" & fldvalue & "'"
                End If
                ret = ExequteSQLquery(sqly)
                If ret <> "Query executed fine." Then
                    Return ret
                End If
            Next
        Catch ex As Exception
            Return ex.Message
        End Try
        Return ""
    End Function
    Public Function GetTaskListSetting(ByVal unit As String, ByVal logon As String, ByVal prop1 As String, Optional ByRef fldvalue As String = "") As String
        Dim ret As String = String.Empty
        Dim sqli As String = String.Empty
        If fldvalue.Trim <> "" Then
            sqli = "Select * FROM ourtasklistsetting WHERE Unit=" & unit.ToString & " AND [User]='" & logon & "' AND Prop1='" & prop1 & "' AND FldText='" & fldvalue & "'"
        Else
            sqli = "Select * FROM ourtasklistsetting WHERE Unit=" & unit.ToString & " AND [User]='" & logon & "' AND Prop1='" & prop1 & "'"
        End If
        Dim dvi As DataView = mRecords(sqli)
        If Not dvi Is Nothing AndAlso Not dvi.Table Is Nothing AndAlso dvi.Table.Rows.Count > 0 Then
            ret = dvi.Table.Rows(0)("FldColor")
            fldvalue = dvi.Table.Rows(0)("FldText")
        End If
        Return ret
    End Function
    Public Function GetDefaultColors(ByVal unit As String, ByVal unitname As String, ByVal logon As String) As String
        Dim ret As String = String.Empty
        Dim er As String = String.Empty
        Dim sql As String = String.Empty
        Dim dv As DataView = Nothing
        Try
            If Not HasRecords("Select * FROM ourtasklistsetting WHERE UnitName='" & unitname & "' AND [User]='" & logon & "'") Then
                If Not HasRecords("Select * FROM ourtasklistsetting WHERE UnitName='" & unitname & "'") Then
                    'set default TaskList setting
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('header1','default','Task List',1,'#52573e','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('header2','default','Task',2,'#52573e','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('version','default','current',1,'#ee8011','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('version','default','next',2,'#ee8011','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('version','default','old',3,'#ee8011','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('version','default','undefined',4,'#ee8011','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)

                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('status','default','in progress',1,'#ee8011','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('status','default','meeting',2,'#e9c46a','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('status','default','documentation',1,'#ee8011','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('status','default','knowledge',2,'#ee8011','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('status','default','how to',3,'#888677','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('status','default','problem',4,'#ff0000','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('status','default','done',4,'#6e6e6e','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)

                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('status','default','bug',1,'#ee8011','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('status','default','eventually',2,'#e9c46a','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('status','default','planning',1,'#ee8011','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('status','default','known bug',2,'#ee8011','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('status','default','dismissed',3,'#888677','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('status','default','asap',4,'#ff0000','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('status','default','deleted',4,'#6e6e6e','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)

                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('status','default','redesign',1,'#ee8011','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('status','default','testing',2,'#e9c46a','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('status','default','soon',1,'#ee8011','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)
                    sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('status','default','other',2,'#ee8011','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                    er = ExequteSQLquery(sql)

                    ret = ret & "Task List default setting assigned."
                Else
                    'copy records
                    Dim dr As DataTable = mRecords("Select * FROM ourtasklistsetting WHERE UnitName='" & unitname & "' AND Prop3='default'").Table
                    For i = 0 To dr.Rows.Count - 1
                        sql = "INSERT INTO ourtasklistsetting (Prop1,Prop3,FldText,FldOrder,FldColor,Unit,UnitName,[User]) VALUES ('" & dr.Rows(i)("Prop1").ToString & "','','" & dr.Rows(i)("FldText").ToString & "'," & dr.Rows(i)("FldOrder").ToString & ",'" & dr.Rows(i)("FldColor").ToString & "','" & unit.ToString & "','" & unitname.ToString & "','" & logon & "')"
                        er = ExequteSQLquery(sql)
                    Next
                    ret = ret & "Task List default user setting assigned."
                End If
            End If
            dv = mRecords("Select * FROM ourtasklistsetting WHERE UnitName='" & unitname & "' AND [User]='" & logon & "'", er)
            If dv Is Nothing OrElse dv.Table Is Nothing Then
                ret = ret & "Task List setting failed."
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function HexToRGB(HexColor As String) As RGB
        Dim retRGB As RGB = Nothing
        Dim sHex As String = HexColor.Replace("#", "")
        If sHex.Length = 6 Then
            retRGB = New RGB
            retRGB.R = Convert.ToByte(sHex.Substring(0, 2), 16)
            retRGB.G = Convert.ToByte(sHex.Substring(2, 2), 16)
            retRGB.B = Convert.ToByte(sHex.Substring(4, 2), 16)
        End If
        Return retRGB
    End Function
    Public Function RGBToHex(rgb As RGB) As String
        Dim sHex As String = String.Empty
        Dim r As String = Hex(rgb.R)
        Dim g As String = Hex(rgb.G)
        Dim b As String = Hex(rgb.B)

        If r.Length = 1 Then r = "0" & r
        If g.Length = 1 Then g = "0" & g
        If b.Length = 1 Then b = "0" & b

        sHex = "#" & r & g & b

        Return sHex.ToLower
    End Function
    Public Function HSLToRGB(hsl As HSL) As RGB
        Dim rgb As New RGB
        Dim byteTemp As Byte
        Dim rgb_r As Single = 0
        Dim rgb_g As Single = 0
        Dim rgb_b As Single = 0
        Dim temp1 As Single = 0
        Dim temp2 As Single = 0
        Dim temp3 As Single = 0
        Dim tempR As Single = 0
        Dim tempG As Single = 0
        Dim tempB As Single = 0

        If hsl.S = 0.0 Then
            temp1 = Math.Round(hsl.L * 255)
            byteTemp = Convert.ToByte(temp1)
            rgb.R = byteTemp
            rgb.G = byteTemp
            rgb.B = byteTemp
        Else
            If hsl.L < 0.5 Then
                temp1 = hsl.L * (1.0 + hsl.S)
            Else
                temp1 = (hsl.L + hsl.S) - (hsl.L * hsl.S)
            End If
            temp2 = 2 * hsl.L - temp1
            'hue/360
            temp3 = hsl.H / 360

            tempR = temp3 + 0.333
            tempG = temp3
            tempB = temp3 - 0.333

            If tempR > 1.0 Then
                tempR = tempR - 1.0
            ElseIf tempR < 0 Then
                tempR = tempR + 1.0
            End If
            If tempG > 1.0 Then
                tempG = tempG - 1.0
            ElseIf tempG < 0 Then
                tempG = tempG + 1.0
            End If
            If tempB > 1.0 Then
                tempB = tempB - 1.0
            ElseIf tempB < 0 Then
                tempB = tempB + 1.0
            End If
            'Red
            temp3 = 6 * tempR
            If temp3 < 1.0 Then
                rgb_r = temp2 + (temp1 - temp2) * temp3
            ElseIf temp3 > 1.0 Then
                temp3 = 2 * tempR
                If temp3 < 1.0 Then
                    rgb_r = temp1
                ElseIf temp3 > 1.0 Then
                    temp3 = 3 * tempR
                    If temp3 < 2.0 Then
                        rgb_r = temp2 + (temp1 - temp2) * (0.666 - tempR) * 6
                    ElseIf temp3 > 2.0 Then
                        rgb_r = temp2
                    End If
                End If
            End If
            'Green
            temp3 = 6 * tempG
            If temp3 < 1.0 Then
                rgb_g = temp2 + (temp1 - temp2) * temp3
            ElseIf temp3 > 1.0 Then
                temp3 = 2 * tempG
                If temp3 < 1.0 Then
                    rgb_g = temp1
                ElseIf temp3 > 1.0 Then
                    temp3 = 3 * tempG
                    If temp3 < 2.0 Then
                        rgb_g = temp2 + (temp1 - temp2) * (0.666 - tempG) * 6
                    ElseIf temp3 > 2.0 Then
                        rgb_g = temp2
                    End If
                End If
            End If
            'Blue
            temp3 = 6 * tempB
            If temp3 < 1.0 Then
                rgb_b = temp2 + (temp1 - temp2) * temp3
            ElseIf temp3 > 1.0 Then
                temp3 = 2 * tempB
                If temp3 < 1.0 Then
                    rgb_b = temp1
                ElseIf temp3 > 1.0 Then
                    temp3 = 3 * tempB
                    If temp3 < 2.0 Then
                        rgb_b = temp2 + (temp1 - temp2) * (0.666 - tempB) * 6
                    ElseIf temp3 > 2.0 Then
                        rgb_b = temp2
                    End If
                End If
            End If
            rgb_r = Math.Round(rgb_r * 255)
            rgb_g = Math.Round(rgb_g * 255)
            rgb_b = Math.Round(rgb_b * 255)

            rgb.R = Convert.ToByte(rgb_r)
            rgb.G = Convert.ToByte(rgb_g)
            rgb.B = Convert.ToByte(rgb_b)
        End If
        Return rgb
    End Function
    Public Function RGBToHSL(rgb As RGB) As HSL
        Dim hsl As New HSL()

        Dim r As Single = (rgb.R / 255.0F)
        Dim g As Single = (rgb.G / 255.0F)
        Dim b As Single = (rgb.B / 255.0F)

        Dim min As Single = Math.Min(Math.Min(r, g), b)
        Dim max As Single = Math.Max(Math.Max(r, g), b)
        Dim delta As Single = max - min

        hsl.L = (max + min) / 2

        If delta = 0 Then
            hsl.H = 0
            hsl.S = 0.0F
        Else
            hsl.S = If((hsl.L <= 0.5), (delta / (max + min)), (delta / (2 - max - min)))

            Dim hue As Single

            If r = max Then
                hue = ((g - b) / 6) / delta
            ElseIf g = max Then
                hue = (1.0F / 3) + ((b - r) / 6) / delta
            Else
                hue = (2.0F / 3) + ((r - g) / 6) / delta
            End If

            If hue < 0 Then
                hue += 1
            End If
            If hue > 1 Then
                hue -= 1
            End If

            hsl.H = CInt(Math.Truncate(hue * 360))
        End If

        Return hsl
    End Function

    Public Function GetHistoricKMLsetting(ByVal rep As String, ByVal mapname As String) As DataTable
        Dim ret As String = String.Empty
        Dim msql As String = String.Empty
        If Not mapname.Contains(" *(") Then
            Return Nothing
        End If
        Try
            msql = "SELECT * FROM ourkmlhistory WHERE ReportId='" & rep & "' AND MapName='"
            ret = mapname.Substring(0, mapname.IndexOf(" *(")).Trim
            msql = msql & ret & "' AND Saved='"
            ret = mapname.Substring(mapname.IndexOf(" *(")).Replace("*(", "").Replace(")", "").Trim
            msql = msql & ret & "'"
            Dim ds As DataTable = mRecords(msql).Table
            Return ds
        Catch ex As Exception
            ret = ex.Message
            Return Nothing
        End Try
    End Function
    Public Function CreateZipFile(ByVal repid As String) As String
        Dim i As Integer
        Dim ret As String = String.Empty
        Dim ErrorLog = String.Empty
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim reppath As String = String.Empty
        Dim repfile As String = String.Empty
        Dim extfile As String = String.Empty
        Dim downrepfile As String = String.Empty
        Dim downrepzip As String = "y" & repid & "Dnld.zip" 'name of zip file should stay last in directory sorted by alphabetical order (!!)
        Dim rdlstr As String = String.Empty
        Dim rdlfile As String = String.Empty
        Dim dirpath As String = applpath & "TEMP\" & repid & "\"
        Directory.CreateDirectory(dirpath)
        If File.Exists(dirpath & downrepzip) Then
            File.Delete(dirpath & downrepzip)
        End If
        repfile = repid & "Dnld"
        'create the zip file 
        Dim dv As DataView = mRecords("Select * FROM OURFiles WHERE ReportId='" & repid & "'")
        If Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
            For i = 0 To dv.Table.Rows.Count - 1
                If dv.Table.Rows(i)("Type").ToString.ToUpper = "RPT" Then
                    extfile = ".rpt"
                ElseIf dv.Table.Rows(i)("Type").ToString.ToUpper = "RDL" Then
                    extfile = ".rdl"
                ElseIf dv.Table.Rows(i)("Type").ToString.ToUpper = "XSD" Then
                    extfile = ".xsd"
                End If
                'read rdl,xsd,rpt from OURFiles
                downrepfile = repfile & extfile
                rdlstr = dv.Table.Rows(i)("FileText").ToString  'string with file content
                If rdlstr <> "" Then
                    If extfile = ".rdl" Then
                        'remove connection string
                        Dim s1 As Integer = rdlstr.IndexOf("<ConnectString>")
                        Dim s2 As Integer = rdlstr.IndexOf("</ConnectString>")
                        rdlstr = rdlstr.Substring(0, s1) & "<ConnectString>" & rdlstr.Substring(s2)
                    End If
                    'write the string rdlstr into the file downrepfile
                    ErrorLog = WriteXMLStringToFile(dirpath & downrepfile, rdlstr)
                    If ErrorLog = dirpath & downrepfile Then
                    Else
                        ErrorLog = "ERROR!! " & ErrorLog
                    End If
                Else
                    ErrorLog = "ERROR!! No file exist."
                    Return ErrorLog
                    Exit Function
                End If
                'read user rdl from OURFiles
                rdlstr = dv.Table.Rows(i)("UserFile").ToString  'string with file content
                If rdlstr <> "" Then
                    downrepfile = repid & "User" & extfile
                    If extfile = ".rdl" Then
                        'remove connection string
                        Dim s1 As Integer = rdlstr.IndexOf("<ConnectString>")
                        Dim s2 As Integer = rdlstr.IndexOf("</ConnectString>")
                        rdlstr = rdlstr.Substring(0, s1) & "<ConnectString>" & rdlstr.Substring(s2)
                    End If
                    'write the string rdlstr into the file downrepfile
                    ErrorLog = WriteXMLStringToFile(dirpath & downrepfile, rdlstr)
                    If ErrorLog = dirpath & downrepfile Then
                    Else
                        ErrorLog = "ERROR!! " & ErrorLog
                    End If
                End If
            Next
            Try
                ZipFile.CreateFromDirectory(dirpath, dirpath & downrepzip)
            Catch ex As Exception
            End Try
            ret = dirpath & downrepzip
        End If
        Return ret
    End Function

    Public Function ClearTable(ByVal tblname As String, ByVal userconstr As String, ByVal userconprv As String, ByVal unit As String, ByVal logon As String, ByVal userdb As String, ByVal rep As String, ByRef er As String) As String
        Dim ret As String = String.Empty
        Dim sqlm As String = String.Empty
        Dim dtv As DataView = Nothing
        Dim db As String = GetDataBase(userconstr, userconprv)
        If userconprv = "MySql.Data.MySqlClient" Then
            tblname = (db & "." & tblname).ToLower
        ElseIf userconprv.StartsWith("InterSystems.Data.") AndAlso tblname.StartsWith("User.") Then
            tblname = tblname.Replace("User.", "")
        End If
        Dim tblnameDEL = tblname & "DELETED"
        tblname = FixReservedWords(tblname, userconprv)

        If userconprv = "MySql.Data.MySqlClient" Then
            tblname = "`" & tblname.Replace(".", "`.`") & "`"
            tblnameDEL = "`" & tblnameDEL.Replace(".", "`.`") & "`"
        End If

        Try
            'delete tblnameDEL
            sqlm = "DROP TABLE " & tblnameDEL
            ret = ExequteSQLquery(sqlm, userconstr, userconprv)
            If ret <> "Query executed fine." Then
                'Return "ERROR!! " & ret
            End If

            'TODO other providers testing 
            'copy from tblname into tblnameDEL
            If userconprv = "System.Data.SqlClient" Then
                sqlm = "Select * INTO " & tblnameDEL & " FROM " & tblname
                ret = ExequteSQLquery(sqlm, userconstr, userconprv)
            ElseIf userconprv = "Sqlite" Then
                sqlm = "Select * INTO " & tblnameDEL & " FROM " & tblname
                ret = ExequteSQLquery(sqlm, userconstr, userconprv)
            ElseIf userconprv = "MySql.Data.MySqlClient" Then
                sqlm = "CREATE TABLE " & tblnameDEL & " Select * FROM " & tblname
                ret = ExequteSQLquery(sqlm, userconstr, userconprv)

            ElseIf userconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                sqlm = "Select * INTO `" & tblnameDEL & "` FROM `" & tblname & "`"
                ret = ExequteSQLquery(sqlm, userconstr, userconprv)

            ElseIf userconprv = "Oracle.ManagedDataAccess.Client" Then
                sqlm = "CREATE TABLE " & tblnameDEL & " As Select * FROM " & tblname
                ret = ExequteSQLquery(sqlm, userconstr, userconprv)

            ElseIf userconprv.StartsWith("InterSystems.Data.") Then
                'create DELETED table with the same structure as tblname table
                ret = CreateNewEmptyTableFromTable(tblname, tblnameDEL, userconstr, userconprv)
                If ret <> "Query executed fine." Then
                    Return "ERROR!! " & ret
                End If
                'copy records from tblname  into tblnameDEL
                dtv = mRecords("Select * FROM " & tblname, ret, userconstr, userconprv)
                If dtv Is Nothing OrElse dtv.Table.Rows.Count = 0 Then
                    Return "ERROR!! DELETED table Is empty"
                End If
                'ret = AddDataTableIntoDbTable(dtv.Table, tblnameDEL, userconstr, userconprv)
                For i = 0 To dtv.Table.Rows.Count - 1
                    ret = AddRowIntoTable(dtv.Table.Rows(i), tblnameDEL, userconstr, userconprv)
                    If ret <> "Query executed fine." Then
                        ret = "ERROR!! " & ret
                        Return ret
                    End If
                Next
            End If
            If ret <> "Query executed fine." Then
                Return "ERROR!! " & ret
            End If
            'insert in ourusertables
            ret = InsertTableIntoOURUserTables(tblname & "DELETED", tblname & "DELETED", unit, logon, userdb, "", rep)
            'delete all records from tblname 
            sqlm = "DELETE FROM " & tblname
            ret = ExequteSQLquery(sqlm, userconstr, userconprv)
            If ret <> "Query executed fine." Then
                Return "Table copied, but not deleted: " & ret
            End If
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Public Function ClearTables(ByVal tblname As String, ByVal userconstr As String, ByVal userconprv As String, ByVal rep As String, ByVal unit As String, ByVal logon As String, ByVal userdb As String, ByRef er As String, Optional ByVal onemany As String = "one") As String
        Dim ret As String = String.Empty
        If onemany = "one" Then
            ret = ClearTable(tblname, userconstr, userconprv, unit, logon, userdb, rep, er)
            Return ret
        End If
        Dim dv As DataView
        ', Session("Unit"), Session("logon"), Session("UserDB")
        dv = GetReportTablesFromOURUserTables(repid, unit, logon, userdb, ret)
        If dv Is Nothing OrElse dv.Table Is Nothing Then
            Return "ERROR!! " & ret
        End If
        Dim ntables As Integer = dv.Table.Rows.Count
        Dim rtbl As String = String.Empty
        Dim i As Integer
        For i = 0 To ntables - 1
            rtbl = dv.Table.Rows(i)("TableName").ToString.Trim
            ret = ClearTable(rtbl, userconstr, userconprv, unit, logon, userdb, rep, er)
            If ret.StartsWith("ERROR!!") Then
                Return ret
            ElseIf ret.StartsWith("Table copied, but not deleted: " & rtbl) Then
                Return "ERROR!! " & ret
            End If
        Next
        Return ntables.ToString & " deleted"
    End Function
    Public Function CreateNewEmptyTableFromTable(ByVal tblname As String, ByVal tblnamenew As String, ByVal userconstr As String, ByVal userconprv As String) As String
        Dim ret As String = String.Empty
        Dim sqlm As String = String.Empty
        Dim dtv As DataView = Nothing
        Dim sqlq, col, typ As String
        Dim i As Integer = 0
        Dim n As Integer = 0
        Try
            If Not TableExists(tblname, userconstr, userconprv) Then
                Return "ERROR!! Table " & tblname & " don't exist"
            End If
            dtv = mRecords("SELECT * FROM " & tblname, ret, userconstr, userconprv)
            If dtv Is Nothing OrElse dtv.Table.Rows.Count = 0 Then
                Return "ERROR!! DELETED table is empty"
            End If
            Dim dt As DataTable = dtv.Table
            n = dtv.Table.Columns.Count
            'TODO other providers , here for Intersystems
            sqlq = "CREATE TABLE " & tblnamenew.ToLower & " ( "
            For i = 0 To n - 1
                col = dt.Columns(i).Caption
                'TODO for other providers and reserved words to test !!!
                sqlq = sqlq & FixReservedWords(col, userconprv)
                typ = dt.Columns(i).DataType.FullName
                If ColumnTypeIsNumeric(dt.Columns(i)) Then
                    sqlq = sqlq & " FLOAT NOT NULL DEFAULT 0"
                ElseIf ColumnTypeIsDateTime(dt.Columns(i)) Then
                    sqlq = sqlq & " TEXT NULL DEFAULT NULL"
                ElseIf ColumnTypeIsString(dt.Columns(i)) Then
                    sqlq = sqlq & " TEXT NULL DEFAULT NULL"
                Else
                    sqlq = sqlq & " TEXT NULL DEFAULT NULL"
                End If
                If i < n - 1 Then sqlq = sqlq & ","
            Next
            'sqlq = sqlq & " `Indx` int(11) NOT NULL AUTO_INCREMENT PRIMARY KEY "
            sqlq = sqlq & " )"
            sqlq = ConvertSQLSyntaxFromOURdbToUserDB(sqlq, userconprv, ret)
            ret = ExequteSQLquery(sqlq, userconstr, userconprv)
            If ret <> "Query executed fine." Then
                Return "ERROR!! " & ret
            End If
        Catch ex As Exception
            ret = "ERROR!! " & ret & " " & ex.Message
        End Try
        Return ret
    End Function
    Public Function RestoreDataFromDELETED(ByVal tblname As String, ByVal userconstr As String, ByVal userconprv As String) As String
        Dim ret As String = String.Empty
        Dim sqlm As String = String.Empty
        Dim dtv As DataView = Nothing
        Dim i As Integer = 0
        Dim db As String = GetDataBase(userconstr, userconprv)
        If userconprv.StartsWith("InterSystems.Data.") OrElse userconprv = "MySql.Data.MySqlClient" Then
            If tblname.StartsWith("User.") Then tblname = tblname.Replace("User.", "")
        End If
        Dim tblnameDEL = tblname & "DELETED"
        If userconprv = "MySql.Data.MySqlClient" Then
            tblname = "`" & tblname.Replace(".", "`.`") & "`"
            tblnameDEL = "`" & tblnameDEL.Replace(".", "`.`") & "`"
        End If
        Try
            'TODO other providers
            'copy from tblnameDEL  into tblname
            If userconprv = "System.Data.SqlClient" Then
                'delete tblname
                sqlm = "DELETE FROM " & tblname
                ret = ExequteSQLquery(sqlm, userconstr, userconprv)
                If ret <> "Query executed fine." Then
                    Return "ERROR!! " & ret
                End If

                'copy from tblnameDEL  into tblname
                sqlm = "SELECT * INTO " & tblname & " FROM " & tblnameDEL
                ret = ExequteSQLquery(sqlm, userconstr, userconprv)
            ElseIf userconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                'delete tblname
                sqlm = "DELETE FROM `" & tblname & "`"
                ret = ExequteSQLquery(sqlm, userconstr, userconprv)
                If ret <> "Query executed fine." Then
                    Return "ERROR!! " & ret
                End If

                'copy from tblnameDEL  into tblname
                sqlm = "SELECT * INTO `" & tblname & "` FROM `" & tblnameDEL & "`"
                ret = ExequteSQLquery(sqlm, userconstr, userconprv)
            ElseIf userconprv = "MySql.Data.MySqlClient" Then
                'delete tblname
                sqlm = "DELETE FROM " & tblname
                ret = ExequteSQLquery(sqlm, userconstr, userconprv)
                If ret <> "Query executed fine." Then
                    Return "ERROR!! " & ret
                End If

                'copy from tblnameDEL  into tblname
                sqlm = "INSERT INTO " & tblname & " SELECT * FROM " & tblnameDEL
                ret = ExequteSQLquery(sqlm, userconstr, userconprv)

                'TODO other providers - to test
            ElseIf userconprv.StartsWith("InterSystems.Data.") Then
                'delete all records from tblname 
                sqlm = "DELETE FROM " & tblname
                ret = ExequteSQLquery(sqlm, userconstr, userconprv)
                If ret <> "Query executed fine." Then
                    Return "ERROR!! " & ret
                End If
                'copy records from tblnameDEL  into tblname
                dtv = mRecords("SELECT * FROM " & tblnameDEL, ret, userconstr, userconprv)
                If dtv Is Nothing OrElse dtv.Table.Rows.Count = 0 Then
                    Return "ERROR!! DELETED table is empty"
                End If
                ret = LoadDataTableIntoDbTable(dtv.Table, tblname, userconstr, userconprv)
                'For i = 0 To dtv.Table.Rows.Count - 1
                '    ret = AddRowIntoTable(dtv.Table.Rows(i), tblname, userconstr, userconprv)
                '    If ret <> "Query executed fine." Then
                '        ret = ret & " ERROR!! " & ret
                '    End If
                'Next
            ElseIf userconprv = "Oracle.ManagedDataAccess.Client" Then
                'delete all records from tblname 
                sqlm = "DELETE FROM " & tblname
                ret = ExequteSQLquery(sqlm, userconstr, userconprv)
                If ret <> "Query executed fine." Then
                    Return "ERROR!! " & ret
                End If
                'copy records from tblnameDEL  into tblname
                'copy records from tblnameDEL  into tblname
                dtv = mRecords("SELECT * FROM " & tblnameDEL)
                If dtv Is Nothing OrElse dtv.Table.Rows.Count = 0 Then
                    Return "ERROR!! DELETED table is empty"
                End If
                ret = LoadDataTableIntoDbTable(dtv.Table, tblname, userconstr, userconprv)
                'For i = 0 To dtv.Table.Rows.Count - 1
                '    ret = AddRowIntoTable(dtv.Table.Rows(i), tblname, userconstr, userconprv)
                '    If ret <> "Query executed fine." Then
                '        ret = ret & " ERROR!! " & ret
                '    End If
                'Next
            End If

            If ret <> "Query executed fine." Then
                Return "ERROR!! " & ret
            End If

        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function


    Public Function UpdateDashboard(ByVal dashboardname As String, ByRef n As Integer, ByVal userconstr As String, ByVal userconprv As String, Optional ByRef er As String = "") As String
        Dim ret As String = String.Empty
        Dim arr As String = String.Empty
        Dim sqlq As String = String.Empty
        Dim dtv As DataView = Nothing
        Dim i As Integer = 0
        Dim ind As Integer = 0
        Try
            sqlq = "SELECT * FROM ourdashboards WHERE Dashboard='" & dashboardname & "'"
            dtv = mRecords(sqlq)
            If dtv Is Nothing OrElse dtv.Table Is Nothing OrElse dtv.Table.Rows.Count = 0 Then
                Return "0"
            End If
            n = dtv.Table.Rows.Count
            'update Prop1 in ourdashboards - update start
            sqlq = "UPDATE ourdashboards SET Prop1='0' WHERE Dashboard='" & dashboardname & "'"
            ret = ExequteSQLquery(sqlq)
            For i = 0 To n - 1
                ind = dtv.Table.Rows(i)("Indx")
                Dim repid As String = dtv.Table.Rows(i)("ReportID").ToString
                Dim charttype As String = dtv.Table.Rows(i)("ChartType").ToString
                Dim mapname As String = dtv.Table.Rows(i)("MapName").ToString
                Dim mapyesno As String = dtv.Table.Rows(i)("MapYesNo").ToString
                Dim x1 As String = dtv.Table.Rows(i)("x1").ToString
                Dim x2 As String = dtv.Table.Rows(i)("x2").ToString
                Dim y1 As String = dtv.Table.Rows(i)("y1").ToString
                Dim y2 As String = dtv.Table.Rows(i)("y2").ToString
                Dim fn1 As String = dtv.Table.Rows(i)("fn1").ToString
                Dim fn2 As String = dtv.Table.Rows(i)("fn2").ToString
                Dim yM As String = String.Empty
                Dim xM As String = String.Empty
                Dim mfld As String = String.Empty
                Dim vM As String = String.Empty
                yM = dtv.Table.Rows(i)("y2").ToString.Trim
                xM = dtv.Table.Rows(i)("Prop3").ToString.Trim
                mfld = dtv.Table.Rows(i)("Prop4").ToString.Trim
                vM = dtv.Table.Rows(i)("Prop5").ToString.Trim

                Dim wherestms As String = dtv.Table.Rows(i)("WhereStm").ToString
                Dim chartregn As String = ""
                arr = UpdateDashboardARR(dashboardname, repid, charttype, mapname, mapyesno, x1, x2, y1, y2, fn1, fn2, wherestms, "0", xM, mfld, vM, ind, chartregn, userconstr, userconprv, er)
                If arr.Trim <> "" Then
                    'update ARR in ourdashboards - completed
                    sqlq = "UPDATE ourdashboards SET Prop2='" & chartregn & "', ARR ='" & arr.Replace("'", "^^").Replace("[", "**").Replace("]", "##") & "' WHERE Indx=" & ind
                    ret = ExequteSQLquery(sqlq)

                    'update Prop1 in ourdashboards - completed
                    sqlq = "UPDATE ourdashboards SET Prop1='1' WHERE Indx=" & ind
                    ret = ExequteSQLquery(sqlq)
                End If
            Next
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function

    Public Function UpdateDashboardsWithReport(ByVal repid As String, ByRef n As Integer, ByVal userconstr As String, ByVal userconprv As String, Optional ByRef er As String = "") As String
        Dim ret As String = String.Empty
        Dim arr As String = String.Empty
        Dim sqlq As String = String.Empty
        Dim dtv As DataView = Nothing
        Dim i As Integer = 0
        Dim ind As Integer = 0
        Dim m As Integer = 0
        Try
            sqlq = "SELECT * FROM ourdashboards WHERE ReportID='" & repid & "'"
            dtv = mRecords(sqlq)
            If dtv Is Nothing OrElse dtv.Table Is Nothing OrElse dtv.Table.Rows.Count = 0 Then
                Return "Report is not found in dashboards"
            End If
            n = dtv.Table.Rows.Count
            'update Prop1 in ourdashboards - update start
            sqlq = "UPDATE ourdashboards SET Prop1='0' WHERE ReportID='" & repid & "'"
            ret = ExequteSQLquery(sqlq)
            For i = 0 To n - 1
                ind = dtv.Table.Rows(i)("Indx")
                Dim dashname As String = dtv.Table.Rows(i)("Dashboard").ToString
                Dim charttype As String = dtv.Table.Rows(i)("ChartType").ToString
                Dim mapname As String = dtv.Table.Rows(i)("MapName").ToString
                Dim mapyesno As String = dtv.Table.Rows(i)("MapYesNo").ToString
                Dim x1 As String = dtv.Table.Rows(i)("x1").ToString
                Dim x2 As String = dtv.Table.Rows(i)("x2").ToString
                Dim y1 As String = dtv.Table.Rows(i)("y1").ToString
                Dim y2 As String = dtv.Table.Rows(i)("y2").ToString
                Dim fn1 As String = dtv.Table.Rows(i)("fn1").ToString
                Dim fn2 As String = dtv.Table.Rows(i)("fn2").ToString
                Dim yM As String = String.Empty
                Dim xM As String = String.Empty
                Dim mfld As String = String.Empty
                Dim vM As String = String.Empty
                yM = dtv.Table.Rows(i)("y2").ToString.Trim
                xM = dtv.Table.Rows(i)("Prop3").ToString.Trim
                mfld = dtv.Table.Rows(i)("Prop4").ToString.Trim
                vM = dtv.Table.Rows(i)("Prop5").ToString.Trim

                Dim wherestms As String = dtv.Table.Rows(i)("WhereStm").ToString
                Dim chartregn As String = "world"
                arr = UpdateDashboardARR(dashname, repid, charttype, mapname, mapyesno, x1, x2, y1, yM, fn1, fn2, wherestms, "0", xM, mfld, vM, ind, chartregn, userconstr, userconprv, er)
                If arr.Trim <> "" Then
                    'update ARR in ourdashboards - completed
                    sqlq = "UPDATE ourdashboards SET Prop2='" & chartregn & "', ARR='" & arr.Replace("'", "^^").Replace("[", "**").Replace("]", "##") & "' WHERE Indx=" & ind
                    ret = ExequteSQLquery(sqlq)

                    'update Prop1 in ourdashboards - completed
                    sqlq = "UPDATE ourdashboards SET Prop1='1' WHERE Indx=" & ind
                    ret = ExequteSQLquery(sqlq)
                End If
                m = m + 1
            Next
            ret = m.ToString & " dashboards updated with new data for report"
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function

    Public Function UpdateDashboardARR(ByVal dashname As String, ByVal repid As String, ByVal charttype As String, ByVal mapname As String, ByVal mapyesno As String, ByVal x1 As String, ByVal x2 As String, ByVal y1 As String, ByVal y2 As String, ByVal fn1 As String, ByVal fn2 As String, ByVal wherestms As String, ByVal prop1 As String, ByVal prop3 As String, ByVal prop4 As String, ByVal prop5 As String, ByVal ind As Integer, ByRef chartregn As String, ByVal userconstr As String, ByVal userconprv As String, Optional ByRef er As String = "") As String
        Dim ret As String = String.Empty
        Dim arr As String = String.Empty
        Dim sqlq As String = String.Empty
        Dim dv3 As DataView = Nothing
        Dim dri As DataTable
        Dim dt As DataTable
        Dim grp As String = String.Empty
        Try
            dri = GetReportInfo(repid)
            If dri Is Nothing OrElse dri.Rows.Count = 0 Then
                Return ""
            End If
            If prop1.Trim = "" Then

            End If
            If UCase(dri.Rows(0)("ReportAttributes").ToString) = "SQL" Then  'SQL statement
                sqlq = dri.Rows(0)("SQLquerytext").ToString
                If sqlq.ToUpper.IndexOf(" WHERE ") > 0 Then sqlq = sqlq.Substring(0, sqlq.ToUpper.IndexOf(" WHERE "))
                If sqlq.ToUpper.IndexOf(" GROUP BY ") > 0 Then sqlq = sqlq.Substring(0, sqlq.ToUpper.IndexOf(" GROUP BY "))
                If sqlq.ToUpper.IndexOf(" ORDER BY ") > 0 Then sqlq = sqlq.Substring(0, sqlq.ToUpper.IndexOf(" ORDER BY "))
                'wherestms - conditions, parameters and searh
                If wherestms.Trim <> "" Then
                    'TODO other providers
                    If userconstr.ToString.StartsWith("InterSystems.Data.") Then
                        sqlq = sqlq & " WHERE " & wherestms.Trim.Replace("^", "'").Replace("`", "").Replace("[", "").Replace("]", "")
                    Else
                        sqlq = sqlq & " WHERE " & wherestms.Trim.Replace("^", "'")
                    End If
                End If

                dv3 = mRecords(sqlq, ret, userconstr, userconprv)
                'dt = dv3.Table
            Else  ' Stored Procedure
                'retrieve dv3
                dv3 = RetrieveReportData(repid, wherestms.Replace("^", ""), True, -1, Nothing, Nothing, Nothing, userconstr, userconprv, ret)
                If dv3 Is Nothing OrElse dv3.Table.Rows.Count = 0 Then
                    Return ""
                End If
                'dt = dv3.Table
            End If
            Dim dts As New DataTable
            dts = dv3.Table
            dts = MakeDTColumnsNamesCLScompliant(dts, userconprv, ret)
            dv3 = dts.DefaultView
            dt = dv3.Table
            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                Return ""
            End If
            '-------------------------------------------Multi---------------------------------------------------------------------------------------------
            If y2 <> "" OrElse prop3 <> "" OrElse (prop4 <> "" AndAlso prop5 <> "") Then  'multi
                'calc arr for MultiLineChart !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                If y2 = "" Then
                    y2 = y1
                End If
                If prop3 = "" Then
                    prop3 = x1 & "," & x2
                End If
                Dim xselected As String = prop3
                Dim yselected As String = y2
                Dim xsels() As String = Split(xselected, ",")
                Dim ysels() As String = Split(yselected, ",")
                Dim nyselected As Integer = ysels.Count
                Dim fnM As String = fn1
                Dim mfld As String = prop4
                Dim valsM As String = prop5
                Dim srtm As String = prop3
                Dim ssrtm As String = xselected
                Dim rt As String = String.Empty
                Dim flt As String = String.Empty
                Dim ttl As String = String.Empty
                'chart titles
                If fnM = "Value" Then
                    ttl = "Value of [" & yselected & "] in group by [" & xselected & "]"
                ElseIf fnM = "Count" Then
                    ttl = "Count of records in group by [" & xselected & "]"
                ElseIf fnM = "CountDistinct" Then
                    ttl = "Distinct Count of [" & yselected & "] in group by [" & xselected & "]"
                ElseIf fnM = "Sum" Then
                    ttl = "Sum of [" & yselected & "] in group by [" & xselected & "]"
                ElseIf fnM = "Avg" Then
                    ttl = "Avg of [" & yselected & "] in group by [" & xselected & "]"
                ElseIf fnM = "StDev" Then
                    ttl = "StDev of [" & yselected & "] in group by [" & xselected & "]"
                ElseIf fnM = "Max" Then
                    ttl = "Max of [" & yselected & "] in group by [" & xselected & "]"
                ElseIf fnM = "Min" Then
                    ttl = "Min of [" & yselected & "] in group by [" & xselected & "]"
                End If
                If valsM <> "" Then
                    ttl = ttl & " for value of " & mfld & "  in " & valsM.ToString.Trim
                End If

                Dim n As Integer = 0
                If nyselected = 1 AndAlso valsM <> "" Then
                    '    'multiple lines for each selected value in valsM of the field mfld
                    Dim vsels() As String = Split(valsM, ",")
                    '    For i = 0 To vsels.Length - 1
                    '        Dim coln As New DataColumn
                    '        coln.DataType = System.Type.GetType("System.String")
                    '        coln.ColumnName = "ARR" & vsels(i)
                    '        dt.Columns.Add(coln)
                    '    Next

                    '    If fnM = "Value" Then
                    '        Dim fldnames(0) As String
                    '        'fldnames(0) = ysels(0)
                    '        For n = 0 To xsels.Length - 1
                    '            ReDim Preserve fldnames(n)
                    '            fldnames(n) = xsels(n)
                    '        Next
                    '        ReDim Preserve fldnames(n)
                    '        fldnames(n) = mfld
                    '        ReDim Preserve fldnames(n + 1)
                    '        fldnames(n + 1) = ysels(0)
                    '        Dim ddt As DataTable = dv3.ToTable(True, fldnames)


                    '        For i = 0 To vsels.Length - 1
                    '            If ColumnTypeIsNumeric(dt.Columns(mfld)) Then
                    '                flt = mfld & "=" & vsels(i).ToString
                    '            Else
                    '                flt = mfld & "='" & vsels(i).ToString.Trim & "'"
                    '            End If
                    '            dv3.RowFilter = flt
                    '            dt = dv3.ToTable
                    '            For n = 0 To dt.Rows.Count - 1
                    '                dt.Rows(n)("ARR" & vsels(i)) = dt.Rows(n)(ysels(0)).ToString
                    '            Next
                    '            dv3.RowFilter = ""
                    '            dt = dv3.ToTable
                    '        Next
                    'Else
                    dt = ComputeStatsV(dv3.Table, fnM, ysels, xsels, mfld, vsels, "", rt, userconstr, userconprv)
                    If dt Is Nothing Then
                        Return ""
                    End If
                    dt.DefaultView.Sort = xselected
                    dt = dt.DefaultView.ToTable
                    'End If

                    Dim m As Integer
                    arr = "['" & xselected & "','" & valsM.Replace(",", "','") & "'],"
                    For i = 0 To dt.Rows.Count - 1
                        grp = ""
                        For m = 0 To xsels.Length - 1
                            grp = grp & dt.Rows(i)(xsels(m)).ToString
                            If m < xsels.Length - 1 Then
                                grp = grp & ","
                            End If
                        Next
                        arr = arr & "['" & grp & "',"
                        For n = 0 To vsels.Length - 1
                            arr = arr & dt.Rows(i)("ARR" & vsels(n)).ToString
                            If n < vsels.Length - 1 Then
                                arr = arr & ","
                            End If
                        Next
                        arr = arr & "]"
                        If i < dt.Rows.Count - 1 Then arr = arr & ","
                    Next

                Else
                    'multiple lines for each selected field from yselected
                    If fnM = "Value" Then
                        For i = 0 To ysels.Length - 1
                            Dim coln As New DataColumn
                            coln.DataType = System.Type.GetType("System.String")
                            coln.ColumnName = "ARR" & ysels(i)
                            dt.Columns.Add(coln)
                            For n = 0 To dt.Rows.Count - 1
                                If dt.Rows(n)(ysels(i)).ToString.Trim = "" Then
                                    dt.Rows(n)(coln.ColumnName) = "0"
                                Else
                                    dt.Rows(n)(coln.ColumnName) = dt.Rows(n)(ysels(i))
                                End If
                            Next
                        Next
                    Else
                        dt = ComputeStatsM(dv3.Table, fnM, ysels, xsels, "", rt, userconstr, userconprv)
                        If dt Is Nothing Then
                            Return ""
                        End If
                        dt.DefaultView.Sort = xselected
                        dt = dt.DefaultView.ToTable
                    End If

                    Dim m As Integer
                    arr = "['" & xselected & "','" & yselected.Replace(",", "','") & "'],"
                    For i = 0 To dt.Rows.Count - 1
                        grp = ""
                        For m = 0 To xsels.Length - 1
                            grp = grp & dt.Rows(i)(xsels(m)).ToString
                            If m < xsels.Length - 1 Then
                                grp = grp & ","
                            End If
                        Next
                        arr = arr & "['" & grp & "',"
                        For n = 0 To ysels.Length - 1
                            arr = arr & dt.Rows(i)("ARR" & ysels(n)).ToString
                            If n < ysels.Length - 1 Then
                                arr = arr & ","
                            End If
                        Next
                        arr = arr & "]"
                        If i < dt.Rows.Count - 1 Then arr = arr & ","
                    Next
                End If
                ret = arr
                Return ret
            End If

            '-------------------------------------------Not Multi---------------------------------------------------------------------------------------------
            Dim mcolors() As String = {"blue", "gold", "lightgreen", "red", "yellow", "darkred", "lightblue", "green", "#e5e4e2", "orange", "darkgreen", "pink", "darkblue", "lightyellow", "maroon", "salmon", "darkorange"}
            Dim k As Integer = 0
            Dim j As Integer = 0
            Dim lastk As Integer = 0
            Dim maxvalue As Integer = 1
            If mapyesno = "yes" Then   'MapChart and GeoChart !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
                'Dim chartregn As String = ""
                chartregn = "world"
                If mapname = "" Then
                    ret = "no Map name found"
                    Return ""
                End If

                Dim lon As String = String.Empty
                Dim lat As String = String.Empty
                Dim latlon As String = String.Empty
                Dim nm As String = String.Empty
                Dim des As String = String.Empty
                Dim lonend As String = String.Empty
                Dim latend As String = String.Empty
                Dim starttimecol As String = String.Empty
                Dim endtimecol As String = String.Empty
                Dim lonadd As String = String.Empty
                Dim latadd As String = String.Empty
                Dim coordadd As String = String.Empty
                Dim coor As String = String.Empty
                Dim dm As DataTable
                Dim dcl As DataTable
                Dim dce As DataTable
                Dim drk As DataTable
                dm = GetReportMapFields(repid, mapname)
                dcl = GetReportColorField(repid, mapname)
                dce = GetReportExtrudedFields(repid, mapname)
                drk = GetReportKeyFields(repid, mapname)
                If dm Is Nothing Then
                    ret = "no Map fields found"
                    Return ""
                End If
                For i = 0 To dm.Rows.Count - 1
                    If dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkName" AndAlso dm.Rows(i)("ord") = 0 Then
                        nm = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
                        If dm.Rows(i)("descrtext").ToString.Trim <> "" Then
                            chartregn = dm.Rows(i)("descrtext").ToString.Trim
                        End If
                        If dm.Rows(i)("Prop7").ToString.Trim.ToUpper = "TRUE" Then
                            latlon = "True"
                        Else
                            latlon = "False"
                        End If
                    ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkDescription" Then
                        If dm.Rows(i)("descrtext").ToString <> "" Then
                            des = des & "txt#" & dm.Rows(i)("ord") & "*" & dm.Rows(i)("descrtext").ToString & "|"
                        Else
                            des = des & "fld#" & dm.Rows(i)("ord") & "*" & dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1) & "|"
                        End If
                    ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkLongitude" AndAlso dm.Rows(i)("ord") = 0 Then
                        lon = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
                    ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkLatitude" AndAlso dm.Rows(i)("ord") = 0 Then
                        lat = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
                    ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkLongitudeEnd" AndAlso dm.Rows(i)("ord") = 1 Then
                        lonend = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
                    ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkLatitudeEnd" AndAlso dm.Rows(i)("ord") = 1 Then
                        latend = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
                    ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkStartTime" Then
                        starttimecol = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
                    ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkEndTime" Then
                        endtimecol = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
                    ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkLongitude" AndAlso dm.Rows(i)("ord") > 0 Then
                        lonadd = coordadd & "lon[" & dm.Rows(i)("ord") & "]" & dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1) & "|"
                    ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkLatitude" AndAlso dm.Rows(i)("ord") > 0 Then
                        latadd = coordadd & "lat[" & dm.Rows(i)("ord") & "]" & dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1) & "|"
                    End If
                Next
                Dim p1, p2 As String
                If charttype = "MapChart" Then
                    arr = "['Long','Lat','Name','Marker'],"
                    For i = 0 To dt.Rows.Count - 1
                        'lon,lat
                        If lat = "POINT" Then
                            If dt.Rows(i)(lon).ToString.Trim = "" Then
                                Continue For
                            End If
                        Else
                            If i > 0 AndAlso (dt.Rows(i)(lon).ToString.Trim = "" OrElse dt.Rows(i)(lat).ToString.Trim = "") Then
                                Continue For
                            End If
                            If i > 0 AndAlso dt.Rows(i)(lon) = 0 AndAlso dt.Rows(i)(lat) = 0 Then
                                Continue For
                            End If
                            If i > 0 AndAlso dt.Rows(i)(lon) = dt.Rows(i - 1)(lon) AndAlso dt.Rows(i)(lat) = dt.Rows(i - 1)(lat) Then
                                Continue For
                            End If
                        End If

                        coor = ""
                        arr = arr & "["
                        If lat = "POINT" Then
                            p1 = Piece(dt.Rows(i)(lon).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                            p2 = Piece(dt.Rows(i)(lon).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                            If latlon = "True" Then
                                arr = arr & p1 & "," & p2 & ","
                            Else
                                arr = arr & p2 & "," & p1 & ","
                            End If

                        ElseIf lon <> lat AndAlso IsNumeric(dt.Rows(i)(lon).ToString) AndAlso IsNumeric(dt.Rows(i)(lat).ToString) Then
                            arr = arr & dt.Rows(i)(lat).ToString & "," & dt.Rows(i)(lon).ToString & ","
                        ElseIf lon = lat AndAlso Not IsNumeric(dt.Rows(i)(lon).ToString) Then
                            coor = CoordinatesLatLngGeocoding(dt.Rows(i)(lon).ToString, chartregn)
                            arr = arr & coor & ","
                        End If
                        arr = arr & "'" & (dt.Rows(i)(nm).ToString & DescriptionText(dt, dm, i, False, True).Replace("<![CDATA[", " ").Replace("<br>", " ").Replace("</br>", " ").Replace("<b>", " ").Replace("</b>", " ")).Trim & "'"
                        arr = arr & ",'pink'"
                        arr = arr & "]"
                        If i < dt.Rows.Count - 1 Then arr = arr & ","

                        If lonend.Trim <> "" AndAlso latend.Trim <> "" Then
                            'lonend,latend
                            If latend = "POINT" Then
                                If dt.Rows(i)(lonend).ToString.Trim = "" Then
                                    Continue For
                                End If
                            Else
                                If i > 0 AndAlso (dt.Rows(i)(lonend).ToString.Trim = "" OrElse dt.Rows(i)(latend).ToString.Trim = "") Then
                                    Continue For
                                End If
                                If i > 0 AndAlso dt.Rows(i)(lonend) = 0 AndAlso dt.Rows(i)(latend) = 0 Then
                                    Continue For
                                End If
                                If i > 0 AndAlso dt.Rows(i)(lonend) = dt.Rows(i - 1)(lonend) AndAlso dt.Rows(i)(latend) = dt.Rows(i - 1)(latend) Then
                                    Continue For
                                End If
                            End If

                            arr = arr & "["
                            If latend = "POINT" Then
                                p1 = Piece(dt.Rows(i)(lonend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                                p2 = Piece(dt.Rows(i)(lonend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                                If latlon = "True" Then
                                    arr = arr & p1 & "," & p2 & ","
                                Else
                                    arr = arr & p2 & "," & p1 & ","
                                End If

                            ElseIf lonend <> latend AndAlso IsNumeric(dt.Rows(i)(lonend).ToString) AndAlso IsNumeric(dt.Rows(i)(latend).ToString) Then
                                arr = arr & dt.Rows(i)(latend).ToString & "," & dt.Rows(i)(lonend).ToString & ","
                            ElseIf lonend = latend AndAlso Not IsNumeric(dt.Rows(i)(lonend).ToString) Then
                                'arr = arr & "'" & dt.Rows(i)(lonend).ToString & "'" & ","
                                coor = CoordinatesLatLngGeocoding(dt.Rows(i)(lonend).ToString, chartregn)
                                arr = arr & coor & ","
                            End If
                            'arr = arr & "'" & dt.Rows(i)(nm).ToString & "'"
                            arr = arr & "'" & (dt.Rows(i)(nm).ToString & DescriptionText(dt, dm, i, False, True).Replace("<![CDATA[", " ").Replace("<br>", " ").Replace("</br>", " ").Replace("<b>", " ").Replace("</b>", " ")).Trim & "'"
                            arr = arr & ",'blue'"
                            arr = arr & "]"
                            If i < dt.Rows.Count - 1 Then arr = arr & ","
                        End If

                        If latadd = "POINT" Then

                            If dt.Rows(i)(lonadd).ToString.Trim = "" Then
                                Continue For
                            End If

                            p1 = Piece(dt.Rows(i)(lonadd).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                            p2 = Piece(dt.Rows(i)(lonadd).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                            If latlon = "True" Then
                                arr = arr & p1 & "," & p2 & ","
                            Else
                                arr = arr & p2 & "," & p1 & ","
                            End If

                        ElseIf lonadd.Trim <> "" AndAlso latadd.Trim <> "" Then
                            'lonadd,latedd
                            If i > 0 AndAlso (dt.Rows(i)(lonadd).ToString.Trim = "" OrElse dt.Rows(i)(latadd).ToString.Trim = "") Then
                                Continue For
                            End If
                            If i > 0 AndAlso dt.Rows(i)(lonadd) = 0 AndAlso dt.Rows(i)(latadd) = 0 Then
                                Continue For
                            End If
                            If i > 0 AndAlso dt.Rows(i)(lonadd) = dt.Rows(i - 1)(lonadd) AndAlso dt.Rows(i)(latadd) = dt.Rows(i - 1)(latadd) Then
                                Continue For
                            End If
                            'If Not arr.EndsWith(",") Then
                            '    arr = arr & ","
                            'End If
                            arr = arr & "["
                            If lonadd <> latadd AndAlso IsNumeric(dt.Rows(i)(lonadd).ToString) AndAlso IsNumeric(dt.Rows(i)(latadd).ToString) Then
                                arr = arr & dt.Rows(i)(latadd).ToString & "," & dt.Rows(i)(lonadd).ToString & ","
                            ElseIf lonadd = latadd AndAlso Not IsNumeric(dt.Rows(i)(lonadd).ToString) Then
                                coor = CoordinatesLatLngGeocoding(dt.Rows(i)(lonadd).ToString, chartregn)
                                arr = arr & coor & ","
                            End If
                            arr = arr & "'" & (dt.Rows(i)(nm).ToString & DescriptionText(dt, dm, i, False, True).Replace("<![CDATA[", " ").Replace("<br>", " ").Replace("</br>", " ").Replace("<b>", " ").Replace("</b>", " ")).Trim & "'"
                            arr = arr & ",'green'"
                            arr = arr & "]"
                            If i < dt.Rows.Count - 1 Then arr = arr & ","
                        End If
                    Next
                    arr = arr.Replace("][", "],[")  'correct if record for end or add were skipped
                    charttype = "Map"
                Else  'GeoChart '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
                    Dim fldtoclr As String = String.Empty
                    Dim fldtoext As String = String.Empty
                    If dcl Is Nothing OrElse dcl.Rows.Count = 0 Then
                    Else
                        fldtoclr = dcl.Rows(0)("Val").ToString
                    End If
                    If dce Is Nothing OrElse dce.Rows.Count = 0 Then
                    Else
                        fldtoext = dce.Rows(0)("Val").ToString
                    End If
                    arr = "['Long','Lat'," ''" & fldtoclr & "','" & fldtoext & "'],"
                    If lon <> lat Then
                        If fldtoclr <> "" Then arr = arr & "'" & fldtoclr & "'"
                        If fldtoclr <> "" AndAlso fldtoext <> "" AndAlso fldtoclr <> fldtoext Then
                            arr = arr & ",'" & fldtoext & "'"
                        End If
                        arr = arr & "],"
                    Else
                        'arr = "['Address'," ''" & fldtoclr & "','" & fldtoext & "'],"
                        arr = "['Long','Lat'," ''" & fldtoclr & "','" & fldtoext & "'],"

                        If fldtoclr <> "" Then arr = arr & "'" & fldtoclr & "'"
                        If fldtoclr <> "" AndAlso fldtoext <> "" AndAlso fldtoclr <> fldtoext Then
                            arr = arr & ",'" & fldtoext & "'"
                        End If
                        arr = arr & "],"
                    End If

                    For i = 0 To dt.Rows.Count - 1
                        'lon,lat
                        arr = arr & "["
                        If lat = "POINT" Then
                            If dt.Rows(i)(lon).ToString.Trim = "" Then
                                Continue For
                            End If

                            p1 = Piece(dt.Rows(i)(lon).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                            p2 = Piece(dt.Rows(i)(lon).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                            If latlon = "True" Then
                                arr = arr & p1 & "," & p2 & ","
                            Else
                                arr = arr & p2 & "," & p1 & ","
                            End If

                        ElseIf lon <> lat AndAlso IsNumeric(dt.Rows(i)(lon).ToString) AndAlso IsNumeric(dt.Rows(i)(lat).ToString) Then
                            arr = arr & dt.Rows(i)(lat).ToString & "," & dt.Rows(i)(lon).ToString & ","
                        ElseIf lon = lat AndAlso Not IsNumeric(dt.Rows(i)(lon).ToString) Then
                            coor = CoordinatesLatLngGeocoding(dt.Rows(i)(lon).ToString, chartregn)
                            arr = arr & coor & ","
                        End If
                        If fldtoclr <> "" AndAlso IsNumeric(dt.Rows(i)(fldtoclr).ToString) Then
                            arr = arr & dt.Rows(i)(fldtoclr).ToString
                        ElseIf fldtoclr <> "" AndAlso Not IsNumeric(dt.Rows(i)(fldtoclr).ToString) Then
                            arr = arr & "'" & dt.Rows(i)(fldtoclr).ToString & "'"
                        End If
                        If fldtoclr <> "" AndAlso fldtoext <> "" AndAlso fldtoclr <> fldtoext AndAlso IsNumeric(dt.Rows(i)(fldtoext).ToString) Then
                            arr = arr & "," & dt.Rows(i)(fldtoext).ToString
                        ElseIf fldtoclr <> "" AndAlso fldtoext <> "" AndAlso fldtoclr <> fldtoext AndAlso Not IsNumeric(dt.Rows(i)(fldtoext).ToString) Then
                            arr = arr & ",'" & dt.Rows(i)(fldtoext).ToString & "'"
                        End If
                        'arr = arr & ",'" & (dt.Rows(i)(nm).ToString & DescriptionText(dt, dm, i, False, True).Replace("<![CDATA[", " ").Replace("<br>", " ").Replace("</br>", " ").Replace("<b>", " ").Replace("</b>", " ")).Trim & "'"

                        arr = arr & "]"
                        If i < dt.Rows.Count - 1 Then arr = arr & ","
                    Next
                End If
                chartregn = chartregn.Substring(chartregn.IndexOf(":") + 1)
                If chartregn.Trim = "" Then
                    chartregn = "world"
                End If

            Else
                '----------------------------------  NO MAP ------------------------------------------------------
                Try
                    Dim fn As String = fn1

                    If y1 = "" OrElse fn = "" OrElse (x1 = "" And x2 = "") Then
                        Return ""
                    End If
                    Dim msql As String = String.Empty

                    'fix names of fields according dt
                    Dim l As Integer
                    For l = 0 To dt.Columns.Count - 1
                        If dt.Columns(l).Caption = x1 Then
                            Exit For
                        ElseIf dt.Columns(l).Caption = x1.Replace("_", "") Then
                            x1 = x1.Replace("_", "")
                            Exit For
                        End If
                    Next
                    For l = 0 To dt.Columns.Count - 1
                        If dt.Columns(l).Caption = x2 Then
                            Exit For
                        ElseIf dt.Columns(l).Caption = x2.Replace("_", "") Then
                            x2 = x2.Replace("_", "")
                            Exit For
                        End If
                    Next
                    For l = 0 To dt.Columns.Count - 1
                        If dt.Columns(l).Caption = y1 Then
                            Exit For
                        ElseIf dt.Columns(l).Caption = y1.Replace("_", "") Then
                            y1 = y1.Replace("_", "")
                            Exit For
                        End If
                    Next
                    Dim selflds As String = x1 & "," & x2 & "," & y1
                    Dim srt As String

                    If x1 = x2 Then
                        srt = x1
                    Else
                        srt = x1 & ", " & x2
                    End If
                    Dim rt As String = String.Empty
                    If x1 <> x2 Then
                        rt = AddGroupBy(repid, x1, x2, "custom", userconstr, userconprv, er)
                    End If

                    If UCase(dri.Rows(0)("ReportAttributes").ToString) = "SQL" Then  'SQL statement !!!!!!!!!!!!!!!!!!!

                        selflds = FixSelectedFields(repid, selflds, userconstr, userconprv)
                        Dim xx1, xx2, yy1, ssrt As String
                        xx1 = Piece(selflds, ",", 1)
                        xx2 = Piece(selflds, ",", 2)
                        yy1 = Piece(selflds, ",", 3)
                        ssrt = xx1 & ", " & xx2

                        If sqlq.ToUpper.IndexOf(" WHERE ") > 0 Then sqlq = sqlq.Substring(0, sqlq.ToUpper.IndexOf(" WHERE "))
                        If sqlq.ToUpper.IndexOf(" GROUP BY ") > 0 Then sqlq = sqlq.Substring(0, sqlq.ToUpper.IndexOf(" GROUP BY "))
                        If sqlq.ToUpper.IndexOf(" ORDER BY ") > 0 Then sqlq = sqlq.Substring(0, sqlq.ToUpper.IndexOf(" ORDER BY "))
                        sqlq = sqlq.Substring(sqlq.ToUpper.IndexOf(" FROM "))
                        'wherestms - conditions, parameters and searh
                        If wherestms.Trim <> "" Then
                            'TODO other providers ?
                            If userconprv.ToString.StartsWith("InterSystems.Data.") Then
                                sqlq = sqlq & " WHERE " & wherestms.Trim.Replace("^", "'").Replace("`", "").Replace("[", "").Replace("]", "")
                            Else
                                sqlq = sqlq & " WHERE " & wherestms.Trim.Replace("^", "'")
                            End If
                        End If
                        sqlq = sqlq & " ORDER BY " & ssrt

                        If fn = "Value" Then
                            'ttl = "Value of [" & y1 & "] in group by [" & srt & "]"
                            msql = "SELECT " & ssrt & "," & yy1 & " AS ARR "
                        ElseIf fn = "Count" Then
                            'ttl = "Count of records in group by [" & srt & "]"
                            msql = "SELECT " & ssrt & ",Count(" & yy1 & ") AS ARR "
                        ElseIf fn = "CountDistinct" Then
                            'ttl = "Distinct Count of [" & y1 & "] in group by [" & srt & "]"
                            msql = "SELECT " & ssrt & ",Count(Distinct " & yy1 & ") AS ARR "

                        ElseIf fn = "Sum" Then
                            'ttl = "Sum of [" & y1 & "] in group by [" & srt & "]"
                            msql = "SELECT " & ssrt & ",Sum(" & yy1 & ") AS ARR "

                        ElseIf fn = "Avg" Then
                            'ttl = "Avg of [" & y1 & "] in group by [" & srt & "]"
                            msql = "SELECT " & ssrt & ", AVG(" & yy1 & ") AS ARR "

                        ElseIf fn = "StDev" Then
                            'ttl = "StDev of [" & y1 & "] in group by [" & srt & "]"

                            'other providers
                            Dim std As String = String.Empty
                            If userconprv = "System.Data.SqlClient" Then 'SQL Server
                                std = "STDEVP"
                            Else 'All others
                                std = "STDDEV"
                            End If

                            msql = "SELECT " & ssrt & ", " & std & "(" & yy1 & ") AS ARR "

                        ElseIf fn = "Max" Then
                            'ttl = "Max of [" & y1 & "] in group by [" & srt & "]"
                            msql = "SELECT " & ssrt & ", MAX(" & yy1 & ") AS ARR "

                        ElseIf fn = "Min" Then
                            'ttl = "Min of [" & y1 & "] in group by [" & srt & "]"
                            msql = "SELECT " & ssrt & ", MIN(" & yy1 & ") AS ARR "
                        End If

                        msql = msql & sqlq
                        msql = msql.Replace(" ORDER BY ", " GROUP BY " & ssrt & " ORDER BY ")
                        dt = mRecords(msql, ret, userconstr, userconprv).Table

                    Else 'STORED PROCEDURE !!!!!!!!!!!!!!!!!!!
                        ''chart titles
                        'If fn = "Value" Then
                        '    ttl = "Value of [" & y1 & "] in group by [" & srt & "]"
                        'ElseIf fn = "Count" Then
                        '    ttl = "Count of records in group by [" & srt & "]"
                        'ElseIf fn = "CountDistinct" Then
                        '    ttl = "Distinct Count of [" & y1 & "] in group by [" & srt & "]"
                        'ElseIf fn = "Sum" Then
                        '    ttl = "Sum of [" & y1 & "] in group by [" & srt & "]"
                        'ElseIf fn = "Avg" Then
                        '    ttl = "Avg of [" & y1 & "] in group by [" & srt & "]"
                        'ElseIf fn = "StDev" Then
                        '    ttl = "StDev of [" & y1 & "] in group by [" & srt & "]"
                        'ElseIf fn = "Max" Then
                        '    ttl = "Max of [" & y1 & "] in group by [" & srt & "]"
                        'ElseIf fn = "Min" Then
                        '    ttl = "Min of [" & y1 & "] in group by [" & srt & "]"
                        'End If

                        'dt = ComputeStats(dv3.Table, fn, y1, x1, x2, wherestms, ret, userconstr, userconprv)
                        dt = ComputeStats(dv3.Table, fn, y1, x1, x2, "", ret, userconstr, userconprv)
                    End If

                    If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                        Return ""
                    End If
                    k = 0
                    j = 0
                    lastk = 0
                    maxvalue = 1

                    Dim col = New DataColumn
                    col.DataType = System.Type.GetType("System.String")
                    col.ColumnName = "x2color"
                    dt.Columns.Add(col)

                    'If charttype = "Gauge" Then
                    '    chartp1 = 90
                    '    chartp2 = 100
                    '    chartp3 = 75
                    '    chartp4 = 90
                    '    For i = 0 To dt.Rows.Count - 1
                    '        If CInt(dt.Rows(i)("ARR").ToString) > maxvalue Then
                    '            maxvalue = CInt(dt.Rows(i)("ARR").ToString)
                    '        End If
                    '    Next
                    'End If

                    For i = 0 To dt.Rows.Count - 1
                        If charttype = "CandlestickChart" Then
                            If i = 0 Then
                                arr = arr & "["
                                arr = arr & "'" & dt.Rows(i)(x1).ToString & "'"
                                arr = arr & "," & "0" 'dt.Rows(i)("ARR").ToString
                                arr = arr & "," & "0" ' & dt.Rows(i)("ARR").ToString
                            ElseIf i > 0 AndAlso dt.Rows(i)(x1).ToString = dt.Rows(i - 1)(x1).ToString Then
                                'arr = arr & "," & dt.Rows(i)("ARR").ToString
                            ElseIf i > 0 AndAlso dt.Rows(i)(x1).ToString <> dt.Rows(i - 1)(x1).ToString Then
                                arr = arr & "," & dt.Rows(i - 1)("ARR").ToString
                                arr = arr & "," & dt.Rows(i - 1)("ARR").ToString
                                arr = arr & "],["
                                arr = arr & "'" & dt.Rows(i)(x1).ToString & "'"
                                arr = arr & "," & "0" ' & dt.Rows(i)("ARR").ToString
                                arr = arr & "," & "0" ' & dt.Rows(i)("ARR").ToString
                            End If
                            If i = dt.Rows.Count - 1 Then
                                arr = arr & "," & dt.Rows(i)("ARR").ToString
                                arr = arr & "," & dt.Rows(i)("ARR").ToString
                                arr = arr & "]"
                            End If
                        ElseIf charttype = "Sankey" Then
                            arr = arr & "['" & dt.Rows(i)(x1).ToString & "','" & dt.Rows(i)(x2).ToString & "'," & dt.Rows(i)("ARR").ToString & "]"
                            If i < dt.Rows.Count - 1 Then arr = arr & ","

                        ElseIf charttype = "Gauge" Then
                            If x1 = x2 Then
                                grp = dt.Rows(i)(x1).ToString
                            Else
                                grp = dt.Rows(i)(x1).ToString & "," & dt.Rows(i)(x2).ToString
                            End If
                            arr = arr & "['" & grp & ", " & ExponentToNumber(dt.Rows(i)("ARR").ToString).ToString & "',"
                            arr = arr & 100 * Round(CInt(dt.Rows(i)("ARR").ToString) / maxvalue, 5)
                            arr = arr & "]"
                            If i < dt.Rows.Count - 1 Then arr = arr & ","
                        Else
                            If x1 = x2 Then
                                grp = dt.Rows(i)(x1).ToString
                            Else
                                grp = dt.Rows(i)(x1).ToString & "," & dt.Rows(i)(x2).ToString
                            End If
                            arr = arr & "['" & grp & "'," & dt.Rows(i)("ARR").ToString

                            If charttype = "BubbleChart" Then
                                arr = arr & "," & dt.Rows(i)("ARR").ToString & ",'" & grp & "'"

                            Else
                                For j = 0 To i
                                    If dt.Rows(j)("x2color").ToString.Trim <> "" AndAlso dt.Rows(j)(x2).ToString = dt.Rows(i)(x2).ToString Then
                                        dt.Rows(i)("x2color") = dt.Rows(j)("x2color")
                                        Exit For
                                    End If
                                Next
                                If IsDBNull(dt.Rows(i)("x2color")) OrElse dt.Rows(i)("x2color").ToString.Trim = "" Then
                                    k = lastk Mod mcolors.Length
                                    dt.Rows(i)("x2color") = mcolors(k)
                                    lastk = k + 1
                                End If
                                'arr = arr & ",'" & mcolors(k) & "'"
                                arr = arr & ",'" & dt.Rows(i)("x2color") & "'"
                            End If
                            arr = arr & "]"
                            If i < dt.Rows.Count - 1 Then arr = arr & ","
                        End If

                    Next
                    If charttype = "BubbleChart" Then
                        arr = "['" & srt & "','" & y1 & "','" & y1 & "','" & srt & "']," & arr
                    ElseIf charttype = "Sankey" Then
                        arr = "['From','To','" & y1 & "']," & arr
                    ElseIf charttype = "Gauge" Then
                        arr = "['Label','Value']," & arr
                    ElseIf charttype = "CandlestickChart" Then
                        Dim m As Integer = 4
                        Dim brr As String = String.Empty
                        Dim arrs() As String = Split(arr, "],[")
                        Dim aris() As String
                        For i = 0 To arrs.Length - 1
                            aris = arrs(i).Split(",")
                            brr = brr & aris(0)
                            For j = aris.Length - m To aris.Length - 1
                                brr = brr & "," & aris(j)
                            Next
                            If i < arrs.Length - 1 Then brr = brr & "],["
                        Next
                        arr = brr
                        'chartwidth = arrs.Length * 100
                        'If chartwidth < 1900 Then chartwidth = 1900
                    Else
                        arr = "['" & srt & "','" & y1 & "',{ role: 'style' }]," & arr
                    End If

                Catch ex As Exception
                    ret = ex.Message
                    Return ""
                End Try

            End If
            ret = arr
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
            Return ""
        End Try
        Return ret
    End Function
    Public Function SendEmailAdminScheduledReports() As String
        Dim ret As String = String.Empty
        Dim sqlu As String = String.Empty
        Dim userlgn As String = String.Empty
        Dim i, j, n As Integer
        Dim dtv As DataView
        Dim dts As DataTable
        Dim useremail As String = String.Empty
        sqlu = "SELECT * FROM ourscheduledreports WHERE NOT ([Status] = 'processed') AND NOT ([Status] = 'email sent') AND ((Deadline LIKE '" & DateToString(Now()).Substring(0, 10) & "%') OR (Deadline LIKE '" & DateToString(Now.AddDays(-1)).Substring(0, 10) & "%')) AND Deadline < '" & DateToString(Now()) & "' ORDER BY ID"

        dtv = mRecords(sqlu, ret)
        If dtv Is Nothing OrElse dtv.Count = 0 Then
            Return "0 emails sent"
        End If
        dts = dtv.ToTable
        Dim emails() As String
        n = 0
        Try
            For i = 0 To dts.Rows.Count - 1
                'send email with link default.aspx?srd=6&sched=yes&rep=" & repid & "&reid=" & indf & "&rundate=" & rundate
                Dim webour As String = ConfigurationManager.AppSettings("weboureports").ToString.Trim
                Dim lnk As String = webour & "default.aspx?srd=6&sched=yes&rep=" & dts.Rows(i)("ReportId").ToString.Trim & "&reid=" & dts.Rows(i)("Prop3").ToString.Trim & "&rundate=" & dts.Rows(i)("Deadline").ToString.Trim.Substring(0, 10)
                Dim cntus As String = webour & "ContactUs.aspx"
                Dim emailbody As String = String.Empty
                'emailbody = "Click to download your up to date report: " & lnk & " . Please be patient, it might take longer to complete big reports. | Report is available today only. | | Do not answer to this email. | Feel free to contact us at " & cntus & " if you have any questions. | OUReports"
                emailbody = "Click to download your up to date report: " & lnk & " . | Access to the report is available only today.  Please be patient, it might take longer to complete big reports. |  | Do not answer to this email. | Feel free to contact us at " & cntus & " if you have any questions. | OUReports"
                emailbody = emailbody.Replace("|", Chr(10))
                emails = dts.Rows(i)("ToWhom").ToString.Trim.Split(",")
                For j = 0 To emails.Length - 1
                    If emails(j).Trim <> "" Then
                        ret = SendHTMLEmail("", "Up To Date Report is available for you (today only!).", emailbody, emails(j).Trim, emails(j).Trim)
                        If Not ret.StartsWith("ERROR!! ") Then
                            'update OurScheduledReports
                            sqlu = "UPDATE ourscheduledreports SET [Status]='email sent' WHERE ID=" & dts.Rows(i)("ID")
                            ret = ExequteSQLquery(sqlu)
                            n = n + 1
                        Else
                            WriteToAccessLog("admin", "Email to " & emails(j) & " crashed with error: " & ret, 2)
                        End If
                    End If
                Next
            Next
            Return n.ToString & " emails sent"
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Public Function SendEmailUserScheduledReports(ByVal logon As String, ByVal email As String, Optional ByRef reps As String = "") As String
        Dim ret As String = String.Empty
        reps = String.Empty
        Dim sqlu As String = String.Empty
        Dim userlgn As String = String.Empty
        Dim i, j, n As Integer
        Dim dtv As DataView
        Dim dts As DataTable
        Dim useremail As String = String.Empty

        'AND Deadline LIKE '" & DateToString(Now()).Substring(0, 10) & "%'  AND UserId='" & logon & "' "
        sqlu = "SELECT * FROM ourscheduledreports WHERE NOT ([Status] = 'processed') AND UserId='" & logon & "' AND ((Deadline LIKE '" & DateToStringFormat(Now(), "", "yyyy-MM-dd") & "%') OR (Deadline LIKE '" & DateToStringFormat(Now.AddDays(-1), "", "yyyy-MM-dd") & "%')) AND Deadline < '" & DateToStringFormat(Now(), "", "yyyy-MM-dd HH:mm:ss") & "' ORDER BY ID"

        dtv = mRecords(sqlu, ret)
        If dtv Is Nothing OrElse dtv.Count = 0 Then
            Return "0 emails sent"
        End If
        dts = dtv.ToTable
        Dim emails() As String
        n = 0
        Try
            reps = String.Empty
            For i = 0 To dts.Rows.Count - 1

                reps = dts.Rows(i)("ID").ToString.Trim & ";" & dts.Rows(i)("ReportId").ToString.Trim & "," & reps

                'send email with link default.aspx?srd=6&sched=yes&rep=" & repid & "&reid=" & indf & "&rundate=" & rundate
                Dim webour As String = ConfigurationManager.AppSettings("weboureports").ToString.Trim
                Dim lnk As String = webour & "default.aspx?srd=6&sched=yes&rep=" & dts.Rows(i)("ReportId").ToString.Trim & "&reid=" & dts.Rows(i)("Prop3").ToString.Trim & "&rundate=" & dts.Rows(i)("Deadline").ToString.Trim.Substring(0, 10)
                Dim cntus As String = webour & "ContactUs.aspx"
                Dim emailbody As String = String.Empty
                emailbody = "Click to download your up to date report: " & lnk & " . | Access to the report is available only today.  Please be patient, it might take longer to complete big reports. |  | Do not answer to this email. | Feel free to contact us at " & cntus & " if you have any questions. | OUReports"
                emailbody = emailbody.Replace("|", Chr(10))
                emails = dts.Rows(i)("ToWhom").ToString.Trim.Split(",")
                For j = 0 To emails.Length - 1
                    If emails(j).Trim <> "" Then
                        ret = SendHTMLEmail("", "Up To Date Report is available for you (today only!).", emailbody, emails(j).Trim, emails(j).Trim)
                        If Not ret.StartsWith("ERROR!! ") Then
                            'update ScheduledReports
                            sqlu = "UPDATE ourscheduledreports SET [Status]='email sent' WHERE ID=" & dts.Rows(i)("ID")
                            ret = ExequteSQLquery(sqlu)
                            n = n + 1
                        Else
                            WriteToAccessLog("admin", "Email to " & emails(j) & " crashed with error: " & ret, 2)
                        End If
                    End If
                Next
            Next
            Return n.ToString & " emails sent"
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Public Function ScheduledDownloadsAndSendEmail(ByVal logon As String, ByVal email As String) As String
        Dim ret As String = String.Empty
        Dim retfiles As String = String.Empty
        Dim sqlu As String = String.Empty
        Dim userlgn As String = String.Empty
        Dim i, j, n As Integer
        Dim dtv As DataView
        Dim dts As DataTable
        Dim useremail As String = String.Empty
        sqlu = "SELECT * FROM ourscheduleddownloads WHERE UserId='" & logon & "' AND ([Status]='scheduled' OR [Status]='downloaded on server') AND ((Deadline LIKE '" & DateToStringFormat(Now(), "", "yyyy-MM-dd") & "%') OR (Deadline LIKE '" & DateToStringFormat(Now.AddDays(-1), "", "yyyy-MM-dd") & "%')) AND Deadline < '" & DateToStringFormat(Now(), "", "yyyy-MM-dd HH:mm:ss") & "'"

        dtv = mRecords(sqlu, ret)
        If dtv Is Nothing OrElse dtv.Count = 0 Then
            Return ""
        End If
        dts = dtv.ToTable
        Dim emails() As String
        n = 0
        Try
            For i = 0 To dts.Rows.Count - 1
                'download
                Try

                    If dts.Rows(i)("Status").ToString.Trim = "downloaded on server" Then
                        retfiles = retfiles & dts.Rows(i)("Prop5").ToString.Trim & ","
                        Continue For
                    End If


                    Dim ext As String = String.Empty
                    If dts.Rows(i)("URL").ToString.Trim.LastIndexOf(".") > 7 Then
                        ext = dts.Rows(i)("URL").ToString.Trim.Substring(dts.Rows(i)("URL").ToString.Trim.LastIndexOf("."))
                    Else
                        WriteToAccessLog(logon, "Format of URL does not supported: " & dts.Rows(i)("URL").ToString & " It should end with .CSV, .XML, .JSON, .TXT, .XLS, .XLSX, .MDB, .ACCDB ", 2)
                        sqlu = "UPDATE ourscheduleddownloads SET [Status]='wrong format of url' WHERE ID=" & dts.Rows(i)("ID")
                        ret = ExequteSQLquery(sqlu)
                        Continue For
                    End If
                    If ",.CSV,.XML,.JSON,.TXT,.XLS,.XLSX,.MDB,.ACCDB,".IndexOf(ext.ToUpper) < 0 Then
                        sqlu = "UPDATE ourscheduleddownloads SET [Status]='wrong format of url' WHERE ID=" & dts.Rows(i)("ID")
                        ret = ExequteSQLquery(sqlu)
                        WriteToAccessLog(logon, "Format of URL does not supported: " & dts.Rows(i)("URL").ToString & " It should end with .CSV, .XML, .JSON, .TXT, .XLS, .XLSX, .MDB, .ACCDB ", 2)
                        Continue For
                    End If

                    Dim fileuploadDir As String = applpath & "Temp\"
                    Dim filename As String = logon & "_" & dts.Rows(i)("URL").Trim.Replace(" ", "").Replace("/", "").Replace("\", "").Replace("_", "").Replace("%", "").Replace("http", "").Replace(":", "").Replace(".", "")
                    If filename.Length > 70 Then
                        filename = filename.Substring(0, 70)
                    End If
                    filename = filename & dts.Rows(i)("Deadline").ToString.Replace("-", "").Replace(":", "").Replace(" ", "") & "_" & Now.ToShortDateString.Replace("/", "") & Now.ToShortTimeString.Replace(":", "").Replace(" ", "")
                    filename = filename & "." & ext
                    filename = filename.Replace("..", ".")

                    'save file in the \Temp\filename
                    Dim strFile As String = fileuploadDir.Replace("/", "\") & "\" & filename
                    strFile = strFile.Replace("\\", "\")

                    Try
                        'WebClient - another way to download data
                        'Dim urldnloads As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                        Dim client = New WebClient
                        client.DownloadFile(dts.Rows(i)("URL").Trim, strFile)  '"C:\Uploads\" & filename
                        ''DO NOT DELETE! We can use it for big files and cut them to 4 mg pieces 
                        'Dim Stream As Stream = client.OpenRead(dts.Rows(i)("URL").Trim)
                        'Dim SR As StreamReader = New StreamReader(Stream)
                        'Dim strsr As String = SR.ReadToEnd()  'ReadLine
                        'Stream.Close()

                        'Dim urisite As Uri
                        'urisite = New Uri(dts.Rows(i)("URL").Trim)
                        'client.DownloadFile(urisite, "C:\Uploads\" & filename)
                        'client.DownloadFileAsync(urisite, "C:\Uploads\" & filename)
                        'update OURScheduledDownloads
                        sqlu = "UPDATE ourscheduleddownloads SET [Status]='downloaded on server',Prop2='" & filename & "',Prop5='" & strFile & "' WHERE ID=" & dts.Rows(i)("ID")
                        ret = ExequteSQLquery(sqlu)

                    Catch ex As Exception
                        WriteToAccessLog(logon, "Scheduled download with WebClient from url  " & dts.Rows(i)("URL") & " crashed with error: " & ex.Message, 2)
                        Dim wrequest As WebRequest = WebRequest.Create(dts.Rows(i)("URL").Trim)
                        Dim wresponse As WebResponse = wrequest.GetResponse()
                        Dim xstream As Stream = wresponse.GetResponseStream()
                        Dim tx As New StreamReader(xstream)
                        'clean text of the file
                        Dim xstring As String = cleanTextOfFile(tx.ReadToEnd())
                        File.WriteAllText(strFile, xstring)
                        tx.Close()
                        sqlu = "UPDATE ourscheduleddownloads SET [Status]='downloaded on server',Prop2='" & filename & "',Prop5='" & strFile & "' WHERE ID=" & dts.Rows(i)("ID")
                        ret = ExequteSQLquery(sqlu)
                    End Try


                    WriteToAccessLog(logon, "Scheduled download from url  " & dts.Rows(i)("URL") & " completed into the file " & strFile, 2)

                    retfiles = retfiles & strFile & ","
                    'download file when sub ends

                    'send email 
                    Dim webour As String = ConfigurationManager.AppSettings("weboureports").ToString.Trim
                    Dim cntus As String = webour & "ContactUs.aspx"
                    Dim emailbody As String = String.Empty
                    emailbody = "Scheduled download from url  " & dts.Rows(i)("URL") & " has been completed. See it in your local Downloads folder. | | Do not answer to this email. | Feel free to contact us at " & cntus & " if you have any questions. | OUReports"
                    emailbody = emailbody.Replace("|", Chr(10))
                    emails = dts.Rows(i)("ToWhom").ToString.Trim.Split(",")
                    For j = 0 To emails.Length - 1
                        If emails(j).Trim <> "" Then
                            ret = SendHTMLEmail("", "Scheduled download from url  " & dts.Rows(i)("URL") & " has been completed", emailbody, emails(j).Trim, email)
                            If ret.StartsWith("ERROR!! ") Then
                                'update OURScheduledDownloads
                                sqlu = "UPDATE ourscheduleddownloads SET [Status]='downloaded on server',Prop4='email crashed' WHERE ID=" & dts.Rows(i)("ID")
                                ret = ExequteSQLquery(sqlu)
                                WriteToAccessLog(logon, "Email to " & emails(j) & " crashed with error: " & ret, 2)
                            Else
                                'update OURScheduledDownloads
                                sqlu = "UPDATE ourscheduleddownloads SET [Status]='downloaded on server',Prop4='email sent' WHERE ID=" & dts.Rows(i)("ID")
                                ret = ExequteSQLquery(sqlu)
                                n = n + 1
                            End If
                        End If
                    Next
                Catch ex As Exception
                    'send email 
                    WriteToAccessLog(logon, "Scheduled download from url  " & dts.Rows(i)("URL") & " crashed with error: " & ex.Message, 2)
                    sqlu = "UPDATE ourscheduleddownloads SET [Status]='download crashed' WHERE ID=" & dts.Rows(i)("ID")
                    ret = ExequteSQLquery(sqlu)
                    Dim webour As String = ConfigurationManager.AppSettings("weboureports").ToString.Trim
                    Dim cntus As String = webour & "ContactUs.aspx"
                    Dim emailbody As String = String.Empty
                    emailbody = "Scheduled download from url  " & dts.Rows(i)("URL") & " crashed with error: " & ex.Message & " | | Do not answer to this email. | Feel free to contact us at " & cntus & " if you have any questions. | OUReports"
                    emailbody = emailbody.Replace("|", Chr(10))
                    emails = dts.Rows(i)("ToWhom").ToString.Trim.Split(",")
                    For j = 0 To emails.Length - 1
                        If emails(j).Trim <> "" Then
                            ret = SendHTMLEmail("", "ERROR!! Scheduled download from url  " & dts.Rows(i)("URL") & " crashed", emailbody, emails(j).Trim, email)
                            If ret.StartsWith("ERROR!! ") Then
                                'update OURScheduledDownloads
                                sqlu = "UPDATE ourscheduleddownloads SET [Status]='download and email crashed' WHERE ID=" & dts.Rows(i)("ID")
                                ret = ExequteSQLquery(sqlu)
                                WriteToAccessLog(logon, "Email to " & emails(j) & " crashed with error: " & ret, 2)
                            Else
                                'update OURScheduledDownloads
                                sqlu = "UPDATE ourscheduleddownloads SET [Status]='download crashed, email sent' WHERE ID=" & dts.Rows(i)("ID")
                                ret = ExequteSQLquery(sqlu)

                            End If
                        End If
                    Next
                End Try
            Next

            Return retfiles
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function

    Public Function DirectoryCopy(ByVal sourceDirName As String, ByVal destDirName As String, ByVal copySubDirs As Boolean) As String
        Dim ret As String = String.Empty
        Try
            ' Get the subdirectories for the specified directory.
            Dim dir As DirectoryInfo = New DirectoryInfo(sourceDirName)
            If Not dir.Exists Then
                Return "Source directory does not exist or could not be found: " & sourceDirName
            End If
            Dim dirs() As DirectoryInfo = dir.GetDirectories
            ' If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName)
            Dim i As Integer
            ' Get the files in the directory And copy them to the New location.
            Dim files() As FileInfo = dir.GetFiles
            Dim file As FileInfo
            Dim tempPath As String = String.Empty
            For i = 0 To files.Length - 1
                file = files(i)
                tempPath = Path.Combine(destDirName, file.Name)
                file.CopyTo(tempPath, False)
            Next
            ' If copying subdirectories, copy them And their contents to New location.
            If (copySubDirs) Then
                Dim subdir As DirectoryInfo
                For i = 0 To dirs.Length - 1
                    subdir = dirs(i)
                    tempPath = Path.Combine(destDirName, subdir.Name)
                    ret = DirectoryCopy(subdir.FullName, tempPath, copySubDirs)
                Next
            End If
            Return ret
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Public Function DatabaseExist(ByVal dbnm As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "", Optional ByRef er As String = "") As Boolean
        Dim ret As Boolean = True
        Dim dv As DataView = Nothing
        Dim sq As String = String.Empty
        Dim myconstring, myprovider As String
        Try
            If myconstr.Trim = "" Then
                myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
                myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            Else
                myconstring = myconstr
                myprovider = myconprv
            End If

            If myprovider.StartsWith("InterSystems.Data.") Then  ' = "InterSystems.Data.IRISClient" Then 'ElseIf myprovider = "InterSystems.Data.CacheClient" Then
                ret = False
                sq = "CALL %SYS.Namespace_List()"
                dv = mRecords(sq, er, myconstring, myprovider)
                If (dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0) OrElse er <> "" Then
                    ret = False
                Else
                    For i = 0 To dv.Table.Rows.Count - 1
                        If dv.Table.Rows(i)("Nsp").ToString.ToUpper = dbnm.ToUpper Then
                            ret = True
                            Exit For
                        End If
                    Next
                End If

            ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then
                sq = "SELECT username AS schema_name FROM all_users WHERE UCASE(username)='" & dbnm.ToUpper & "'"
                dv = mRecords(sq, er, myconstring, myprovider)
                If (dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0) OrElse er <> "" Then
                    ret = False
                End If

            ElseIf myprovider = "MySql.Data.MySqlClient" Then
                sq = "SELECT schema_name FROM information_schema.schemata WHERE UCASE(schema_name)='" & dbnm.ToUpper & "';"
                dv = mRecords(sq, er, myconstring, myprovider)
                If (dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0) OrElse er <> "" Then
                    ret = False
                End If

            ElseIf myprovider = "System.Data.SqlClient" Then
                sq = "Select NAME FROM sys.databases WHERE NAME='" & dbnm & "'"
                If Not HasRecords(sq, myconstr, myconprv) Then
                    ret = False
                End If

            ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
                sq = "SELECT schema_name FROM information_schema.schemata WHERE SCHEMA_NAME ='public' AND LOWER(CATALOG_NAME) ='" & dbnm.ToLower & "';"
                dv = mRecords(sq, er, myconstring, myprovider)
                If (dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0) OrElse er <> "" Then
                    ret = False
                End If
            End If
        Catch ex As Exception
            er = ex.Message
            ret = True
        End Try
        Return ret
    End Function

    Public Function GetIPAddress() As String
        Try
            Dim context As System.Web.HttpContext = System.Web.HttpContext.Current
            Dim sIPAddress As String = context.Request.UserHostAddress.ToString
            If sIPAddress Is Nothing OrElse sIPAddress.Trim = "" Then
                sIPAddress = context.Request.ServerVariables("REMOTE_ADDR")
            End If
            Return sIPAddress
        Catch ex As Exception
            Return ""
        End Try
    End Function
    Public Function MakeDatadrivenJoins(ByVal dvusertbls As DataTable, Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "", Optional er As String = "", Optional redo As Boolean = False) As String
        Dim ret As String = String.Empty
        Dim msql As String = String.Empty
        Dim n As Integer = 0
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim tbl1 As String = String.Empty
        Dim tbl2 As String = String.Empty
        Dim dbname As String = String.Empty
        Dim repdb As String = String.Empty
        Try
            Dim myconstring As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            Dim myconnprv As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            If userconnstr = "" Then
                userconnstr = myconstring
                userconnprv = myconnprv
            End If

            'Dim ourrepdb As String = myconstring
            'If ourrepdb.ToUpper.IndexOf("USER ID") > 0 Then ourrepdb = ourrepdb.Substring(0, ourrepdb.ToUpper.IndexOf("USER ID")).Trim
            'If userconnstr.ToUpper.IndexOf("USER ID") > 0 Then repdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("USER ID")).Trim
            Dim ourrepdb As String = myconstring
            If ourrepdb.ToUpper.IndexOf("USER ID") > 0 AndAlso myconnprv <> "Oracle.ManagedDataAccess.Client" Then
                ourrepdb = ourrepdb.Substring(0, myconstring.ToUpper.IndexOf("USER ID")).Trim
            ElseIf ourrepdb.ToUpper.IndexOf("PASSWORD") > 0 AndAlso myconnprv = "Oracle.ManagedDataAccess.Client" Then
                ourrepdb = ourrepdb.Substring(0, myconstring.ToUpper.IndexOf("PASSWORD")).Trim
            End If
            'If ourrepdb.IndexOf("UID") > 0 Then ourrepdb = ourrepdb.Substring(0, userconnstr.IndexOf("UID")).Trim
            'Dim repdb As String = userconnstr
            If userconnstr.ToUpper.IndexOf("USER ID") > 0 AndAlso userconnprv <> "Oracle.ManagedDataAccess.Client" Then
                repdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("USER ID")).Trim
            ElseIf userconnstr.ToUpper.IndexOf("PASSWORD") > 0 AndAlso userconnprv = "Oracle.ManagedDataAccess.Client" Then
                repdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("PASSWORD")).Trim
            End If
            If userconnstr.IndexOf("UID") > 0 AndAlso userconnprv <> "Oracle.ManagedDataAccess.Client" Then
                repdb = userconnstr.Substring(0, userconnstr.IndexOf("UID")).Trim
            End If

            'If userconnstr.IndexOf("UID") > 0 Then repdb = userconnstr.Substring(0, userconnstr.IndexOf("UID")).Trim
            dbname = GetDataBase(userconnstr, userconnprv)

            For i = 0 To dvusertbls.Rows.Count - 1
                If Not ReportForTableExist(dvusertbls.Rows(i)("TABLE_NAME"), er, repdb) Then Continue For
                For j = 0 To i  'dvusertbls.Rows.Count - 1
                    If i = j Then Continue For
                    If Not ReportForTableExist(dvusertbls.Rows(j)("TABLE_NAME"), er, repdb) Then Continue For
                    ret = MakeDatadrivenJoins2tables(dbname, dvusertbls.Rows(i)("TABLE_NAME"), dvusertbls.Rows(j)("TABLE_NAME"), userconnstr, userconnprv, er)
                Next
            Next


            ''create report with repid like dbname & "_joins" to show all joins existing for repdb
            'msql = "SELECT * FROM OURReportInfo WHERE ReportId = '" & repid & "' And ReportDB Like '%" & ourrepdb & "%' And ReportName Like '%" & repdb & "%'"
            'If Not HasRecords(msql) Then  'initial joins' report does not exist with repid like dbname & "_joins" 
            '    'create report on ourdb (!) with repid like dbname & "_joins" to show all joins existing for userrepdb, or if already exists add permissions
            '    msql = "SELECT * FROM OURReportSQLquery WHERE (ReportId=""" & repid & """ AND Doing=""JOIN"")"
            '    ret = MakeNewStanardReport("super", repid, "Joins from relationships", ourrepdb, msql, dbname, "support@yanbor.com", DateToString(CDate(DateAndTime.DateAdd(DateInterval.Day, 3000, Now()))), userconnstr, myconnprv, er, False)
            'End If

        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function MakeDatadrivenJoins2tables(ByVal dbname As String, ByVal tbl1 As String, ByVal tbl2 As String, Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "", Optional er As String = "", Optional redo As Boolean = False, Optional systables As Boolean = False) As String
        Dim ret As String = String.Empty
        Dim msql As String = String.Empty
        Dim n As Integer = 0
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim cnt1 As Integer = 0
        Dim cnt2 As Integer = 0
        Dim cnt12 As Integer = 0
        Dim dvc1 As DataView
        Dim dvc2 As DataView
        Dim cnt As String = String.Empty
        Dim fld1 As String = String.Empty
        'Dim tbl2 As String = String.Empty
        Dim fld2 As String = String.Empty
        Dim repdb As String = String.Empty
        Dim dvcnt As DataView = Nothing
        dbname = dbname.Replace(" ", "").Replace("#", "")
        Try
            Dim myconstring As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            Dim myconnprv As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            If userconnstr = "" Then
                userconnstr = myconstring
                userconnprv = myconnprv
            End If

            Dim ourrepdb As String = myconstring
            'If ourrepdb.ToUpper.IndexOf("USER ID") > 0 Then ourrepdb = ourrepdb.Substring(0, ourrepdb.ToUpper.IndexOf("USER ID")).Trim
            'If userconnstr.ToUpper.IndexOf("USER ID") > 0 Then repdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("USER ID")).Trim
            'If userconnstr.IndexOf("UID") > 0 Then repdb = userconnstr.Substring(0, userconnstr.IndexOf("UID")).Trim
            If ourrepdb.ToUpper.IndexOf("USER ID") > 0 AndAlso myconnprv <> "Oracle.ManagedDataAccess.Client" Then
                ourrepdb = ourrepdb.Substring(0, myconstring.ToUpper.IndexOf("USER ID")).Trim
            ElseIf ourrepdb.ToUpper.IndexOf("PASSWORD") > 0 AndAlso myconnprv = "Oracle.ManagedDataAccess.Client" Then
                ourrepdb = ourrepdb.Substring(0, myconstring.ToUpper.IndexOf("PASSWORD")).Trim
            End If

            If userconnstr.ToUpper.IndexOf("USER ID") > 0 AndAlso userconnprv <> "Oracle.ManagedDataAccess.Client" Then
                repdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("USER ID")).Trim
            ElseIf userconnstr.ToUpper.IndexOf("PASSWORD") > 0 AndAlso userconnprv = "Oracle.ManagedDataAccess.Client" Then
                repdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("PASSWORD")).Trim
            End If

            If userconnstr.IndexOf("UID") > 0 AndAlso userconnprv <> "Oracle.ManagedDataAccess.Client" Then
                repdb = userconnstr.Substring(0, userconnstr.IndexOf("UID")).Trim
            End If

            'create datadriven Joins
            dvc1 = GetListOfTableFields(tbl1, userconnstr, userconnprv, er)
            dvc2 = GetListOfTableFields(tbl2, userconnstr, userconnprv, er)
            'dvc1 = GetListOfTableColumns(tbl1, userconnstr, userconnprv, er)
            'dvc2 = GetListOfTableColumns(tbl2, userconnstr, userconnprv, er)
            If dvc1 Is Nothing OrElse dvc1.Table Is Nothing OrElse dvc1.Table.Rows.Count = 0 OrElse er = "Not data in the table " & tbl1 Then
                ret = "no rows in table1"
                Return ret
            End If
            If dvc2 Is Nothing OrElse dvc2.Table Is Nothing OrElse dvc2.Table.Rows.Count = 0 OrElse er = "Not data in the table " & tbl2 Then
                ret = "no rows in table2"
                Return ret
            End If
            For i = 0 To dvc1.Table.Rows.Count - 1
                Try
                    cnt1 = 0
                    cnt = ""
                    fld1 = dvc1.Table.Rows(i)("COLUMN_NAME")

                    For j = 0 To dvc2.Table.Rows.Count - 1
                        Try
                            'check if both numeric, datetime, or text
                            fld2 = dvc2.Table.Rows(j)("COLUMN_NAME")
                            If tbl1.ToUpper = tbl2.ToUpper AndAlso fld1.ToUpper = fld2.ToUpper Then
                                Continue For
                            End If
                            If dvc1.Table.Rows(i)("DATA_TYPE") <> dvc2.Table.Rows(j)("DATA_TYPE") Then
                                Continue For
                            End If
                            If JoinExist(tbl1, tbl2, fld1, fld2, repdb) Then
                                'If JoinExist(tbl1, tbl2, fld1, fld2, repdb, dbname & "_joins") Then
                                Continue For
                            End If

                            'msql = "SELECT DISTINCT [" & fld1 & "] FROM " & tbl1 & " WHERE [" & fld1 & "] IS NOT NULL"
                            'cnt = CountOfRecords(msql, userconnstr, userconnprv)
                            ret = ""
                            msql = "SELECT COUNT(DISTINCT [" & fld1 & "]) AS CNT FROM [" & tbl1 & "] WHERE [" & fld1 & "] IS NOT NULL"
                            dvcnt = mRecords(msql, ret, userconnstr, userconnprv)
                            If ret <> "" OrElse dvcnt Is Nothing OrElse dvcnt.Table Is Nothing OrElse dvcnt.Table.Rows.Count = 0 Then
                                Continue For
                            End If
                            cnt = dvcnt.Table.Rows(0)("CNT")
                            If Not IsNumeric(cnt) OrElse CInt(cnt) < 3 Then
                                Continue For
                            End If
                            cnt1 = CInt(cnt)

                            cnt2 = 0
                            cnt = ""
                            'fld2 = dvc2.Table.Rows(j)("COLUMN_NAME")
                            If fld1 = "ID" AndAlso fld2 = "Indx" Then
                                Continue For
                            ElseIf fld2 = "ID" AndAlso fld1 = "Indx" Then
                                Continue For
                            ElseIf fld2 = "ID" AndAlso fld1 = "ID" Then
                                Continue For
                            ElseIf fld2 = "Indx" AndAlso fld1 = "Indx" Then
                                Continue For
                                'done above
                                'ElseIf TblFieldIsNumeric(tbl1, fld1, userconnstr, userconnprv) AndAlso Not TblFieldIsNumeric(tbl2, fld2, userconnstr, userconnprv) Then
                                '    Continue For
                                'ElseIf TblFieldIsDateTime(tbl1, fld1, userconnstr, userconnprv) AndAlso Not TblFieldIsDateTime(tbl2, fld2, userconnstr, userconnprv) Then
                                '    Continue For
                                'ElseIf Not TblFieldIsNumeric(tbl1, fld1, userconnstr, userconnprv) AndAlso TblFieldIsNumeric(tbl2, fld2, userconnstr, userconnprv) Then
                                '    Continue For
                                'ElseIf Not TblFieldIsDateTime(tbl1, fld1, userconnstr, userconnprv) AndAlso TblFieldIsDateTime(tbl2, fld2, userconnstr, userconnprv) Then
                                '    Continue For
                            End If

                            ret = ""
                            msql = "SELECT COUNT(DISTINCT [" & fld2 & "]) AS CNT FROM [" & tbl2 & "] WHERE [" & fld2 & "] IS NOT NULL"
                            dvcnt = mRecords(msql, ret, userconnstr, userconnprv)
                            If ret <> "" OrElse dvcnt Is Nothing OrElse dvcnt.Table Is Nothing OrElse dvcnt.Table.Rows.Count = 0 Then
                                Continue For
                            End If
                            cnt = dvcnt.Table.Rows(0)("CNT")
                            If Not IsNumeric(cnt) OrElse CInt(cnt) < 3 Then
                                Continue For
                            End If
                            cnt2 = CInt(cnt)
                            Try
                                cnt12 = 0
                                cnt = ""
                                'msql = "SELECT DISTINCT [" & fld1 & "],[" & fld2 & "] FROM " & tbl1 & " INNER JOIN " & tbl2 & " ON ([" & tbl1 & "].[" & fld1 & "]=[" & tbl2 & "].[" & fld2 & "]) WHERE [" & fld1 & "] IS NOT NULL AND  [" & fld2 & "] IS NOT NULL"
                                'cnt = CountOfRecords(msql, userconnstr, userconnprv)
                                ret = ""
                                msql = "SELECT COUNT(DISTINCT [" & tbl1 & "].[" & fld1 & "],[" & tbl2 & "].[" & fld2 & "]) AS CNT FROM [" & tbl1 & "] INNER JOIN [" & tbl2 & "] ON (UCASE([" & tbl1 & "].[" & fld1 & "])=UCASE([" & tbl2 & "].[" & fld2 & "])) WHERE [" & tbl1 & "].[" & fld1 & "] IS NOT NULL AND  [" & tbl2 & "].[" & fld2 & "] IS NOT NULL"
                                dvcnt = mRecords(msql, ret, userconnstr, userconnprv)
                                If ret <> "" OrElse dvcnt Is Nothing OrElse dvcnt.Table Is Nothing OrElse dvcnt.Table.Rows.Count = 0 Then
                                    Continue For
                                End If
                                cnt = dvcnt.Table.Rows(0)("CNT")
                                If Not IsNumeric(cnt) OrElse CInt(cnt) < 3 Then
                                    Continue For
                                End If
                                cnt12 = CInt(cnt)
                            Catch ex As Exception
                                ret = ex.Message
                                cnt12 = 0
                            End Try
                            If cnt12 > 0.99 * cnt1 OrElse cnt12 > 0.99 * cnt2 Then
                                ret = AddJoin(dbname & "_joins", tbl1, tbl2, fld1, fld2, "INNERJOIN", repdb, "datadriven")
                            End If
                        Catch ex As Exception
                            ret = ex.Message
                        End Try
                    Next
                Catch ex As Exception
                    ret = ex.Message
                End Try
            Next
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function DeleteDatadrivenOrCustomJoins(ByVal userdb As String, ByVal indx As String) As String
        Dim ret As String = String.Empty
        Try
            'delete datadriven or custom joins
            Dim sqls = "UPDATE OURReportSQLquery SET Param3='deleted' WHERE DOING='JOIN' AND Param1 LIKE '%" & userdb.Trim.Replace(" ", "%") & "%' AND (Param2 = 'datadriven' OR Param2 = 'custom' OR Param2 = '') AND Indx = " & indx
            ret = ExequteSQLquery(sqls)  'from OUR db
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function UndoDeleteDatadrivenOrCustomJoins(ByVal userdb As String, ByVal indx As String) As String
        Dim ret As String = String.Empty
        Try
            Dim sqls = "UPDATE OURReportSQLquery SET Param3='' WHERE DOING='JOIN' AND Param1 LIKE '%" & userdb.Trim.Replace(" ", "%") & "%' AND (Param2 = 'datadriven' OR Param2 = 'custom') AND Indx = " & indx
            ret = ExequteSQLquery(sqls)  'from OUR db
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function GetDBJoins(ByVal dvusertbls As DataView, ByVal logon As String, ByVal userdb As String, ByVal origin As String, ByVal filtr As String, ByRef er As String) As DataView
        Dim i As Integer = 0
        Dim dvj As New DataView
        Try
            Dim sqls = "SELECT * FROM OURReportSQLquery WHERE DOING='JOIN' AND Param1 LIKE '%" & userdb.Trim.Replace(" ", "%") & "%'"
            If filtr.Trim <> "" Then
                sqls = sqls & " AND ( " & filtr & " ) "
            End If
            If origin = "deleted" Then
                sqls = sqls & " AND Param3='" & origin & "'" ' AND Not ReportId LIKE '%_joins' "
            ElseIf origin = "custom" Then
                sqls = sqls & " AND (Param2='" & origin & "' OR Param2='' OR Param2 IS NULL) AND (Param3 IS NULL OR Param3='') " ' AND Not ReportId LIKE '%_joins' "
            ElseIf origin = "initial" OrElse origin = "datadriven" Then
                sqls = sqls & " AND Param2='" & origin & "' AND (Param3 IS NULL OR Param3='')" ' AND Not ReportId LIKE '%_joins' "
            ElseIf origin = "not used" Then

                sqls = "SELECT ReportID,Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2, Param1,Param2,Param3, comments,Indx, Count(Indx) As CountIndx FROM OURReportSQLquery WHERE DOING='JOIN' AND Param1 LIKE '%" & userdb.Trim.Replace(" ", "%") & "%' GROUP BY Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2 HAVING Count(Indx)=1"
            End If
            If origin <> "not used" Then
                sqls = sqls & " ORDER BY Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2"
            End If
            dvj = mRecords(sqls, er)  'from OUR db
            If dvj Is Nothing OrElse dvj.Table Is Nothing OrElse dvj.Table.Rows.Count = 0 Then
                Return dvj
            End If
            'If origin = "not used" Then
            '    'dvj.RowFilter = "CountIndx=1"
            '    Return dvj.ToTable.DefaultView
            'End If
            'only joins for user tables IF CSV USER
            If IsCSVuser(userdb, "", er) = "yes" Then
                For i = 0 To dvj.Table.Rows.Count - 1
                    If IsValueInColumnTable(dvj.Table.Rows(i)("Tbl1").ToString, "TABLE_NAME", False, dvusertbls) AndAlso IsValueInColumnTable(dvj.Table.Rows(i)("Tbl2").ToString, "TABLE_NAME", False, dvusertbls) Then
                        dvj.Table.Rows(i)("Friendly") = "yes"
                    End If
                Next
                dvj.RowFilter = "Friendly='yes'"
                Return dvj.ToTable.DefaultView
            Else
                Return dvj.ToTable.DefaultView
            End If
        Catch ex As Exception
            er = ex.Message
            Return Nothing
        End Try
    End Function
    Public Function MakeReportOfJoinAnd15fieldsFromEachTable(rep As String, ByVal tbl1 As String, ByVal tbl2 As String, ByVal fld1 As String, ByVal fld2 As String, ByVal logon As String, ByVal dbname As String, ByVal repdb As String, ByVal useremail As String, ByVal userenddate As String, ByVal userconnstr As String, ByVal userconnprv As String, Optional origin As String = "", Optional er As String = "") As String
        Dim m As Integer = 0
        Dim fld As String = String.Empty
        Dim ddf As DataTable
        Dim ret As String = String.Empty
        Try
            'add join to the new report
            ret = AddJoin(rep, tbl1, tbl2, fld1, fld2, "INNERJOIN", repdb, origin)
            'add fields into OURReportSQLquery
            ret = AddField(rep, tbl1, fld1, repdb)
            ret = AddField(rep, tbl2, fld2, repdb)

            'insert few more fields for each table and update sqlquery for report
            ddf = GetListOfTableColumns(tbl1, userconnstr, userconnprv).Table
            m = ddf.Rows.Count
            If m > 15 Then m = 15
            For j = 0 To m - 1
                If ddf.Rows(j)("COLUMN_NAME").ToUpper <> fld1.ToUpper Then
                    fld = ddf.Rows(j)("COLUMN_NAME").ToString
                    ret = AddField(rep, tbl1, fld, repdb)
                End If
            Next
            ddf = GetListOfTableColumns(tbl2, userconnstr, userconnprv).Table
            m = ddf.Rows.Count
            If m > 15 Then m = 15
            For j = 0 To m - 1
                If ddf.Rows(j)("COLUMN_NAME").ToUpper <> fld2.ToUpper Then
                    fld = ddf.Rows(j)("COLUMN_NAME").ToString
                    ret = AddField(rep, tbl2, fld, repdb)
                End If
            Next

            'make new report with initial sql statement for the join
            Dim sqlt1t2 As String = MakeSQLQueryFromDB(rep, userconnstr, userconnprv)

            ret = MakeNewStanardReport(logon, rep, tbl1 & " JOIN " & tbl2 & " ON " & fld1 & "=" & fld2, repdb, sqlt1t2, dbname, useremail, userenddate, userconnstr, userconnprv, er, True, "Initial report has no more than 15 fields from each table. You can edit it to add more fields.", origin)
            Return ret

        Catch ex As Exception
            Return "ERROR!! " & ex.Message
        End Try

    End Function
    Public Function DeleteReportImages(ReportID As String) As String
        Dim ret As String = String.Empty
        Dim dr As DataRow
        Dim imagePath As String
        Dim BaseAppPath As String = System.AppDomain.CurrentDomain.BaseDirectory()
        Dim FullImagePath As String

        If ReportID = "" Then
            WriteToAccessLog("DeleteReportImages", "WARNING!! deleting Image Files has failed for report: " & ReportID & " with this message: No Report ID found.", 3)
            Return "No Report ID found"
        End If
        Try
            Dim dt As DataTable = GetReportImages(ReportID)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For i As Integer = 0 To dt.Rows.Count - 1
                    dr = dt.Rows(i)
                    imagePath = dr("imagePath").ToString
                    If imagePath <> String.Empty Then
                        FullImagePath = (BaseAppPath & imagePath).Replace("/", "\")
                        If File.Exists(FullImagePath) Then
                            File.Delete(FullImagePath)
                        End If
                    End If
                Next
            End If
            WriteToAccessLog("DeleteReportImages", "Image Files  for report: " & ReportID & " have been deleted. ", 3)
        Catch ex As Exception
            ret = ex.Message
            WriteToAccessLog("DeleteReportImages", "WARNING!! deleting Image Files has failed for report: " & ReportID & " with this message: " & ret, 3)
        End Try

        Return ret
    End Function
    Public Function DeleteReport(ByVal ReportID As String, Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "") As String
        Dim ret As String = String.Empty
        If ReportID = "" Then
            Return "No Report ID found"
        End If
        ret = ExequteSQLquery("DELETE FROM OURReportShow WHERE ReportID='" & ReportID & "'", userconnstr, userconnprv)
        ret = ExequteSQLquery("DELETE FROM OURReportInfo WHERE ReportID='" & ReportID & "'", userconnstr, userconnprv)
        ret = ExequteSQLquery("DELETE FROM OURReportGroups WHERE ReportID='" & ReportID & "'", userconnstr, userconnprv)
        ret = ExequteSQLquery("DELETE FROM OURReportFormat WHERE ReportID='" & ReportID & "'", userconnstr, userconnprv)
        ret = ExequteSQLquery("DELETE FROM OURReportLists WHERE ReportID='" & ReportID & "'", userconnstr, userconnprv)
        ret = ExequteSQLquery("DELETE FROM OURReportSQLquery WHERE ReportID='" & ReportID & "' AND (Param3='' OR Param3 IS NULL) ", userconnstr, userconnprv)
        ret = ExequteSQLquery("DELETE FROM OURReportChildren WHERE ReportID='" & ReportID & "'", userconnstr, userconnprv)
        ret = ExequteSQLquery("DELETE FROM ourdashboards WHERE ReportID='" & ReportID & "'", userconnstr, userconnprv)
        ret = ExequteSQLquery("DELETE FROM ourscheduledreports WHERE ReportID='" & ReportID & "'", userconnstr, userconnprv)
        ret = ExequteSQLquery("DELETE FROM OURPermissions WHERE PARAM1='" & ReportID & "'", userconnstr, userconnprv)
        ret = ExequteSQLquery("DELETE FROM OURReportView WHERE ReportID='" & ReportID & "'", userconnstr, userconnprv)
        ret = DeleteReportImages(ReportID)
        ret = ExequteSQLquery("DELETE FROM OURReportItems WHERE ReportID='" & ReportID & "'", userconnstr, userconnprv)
        Return ret
    End Function
    Public Function RedoReports(ByVal dvusertbls As DataTable, ByVal origin As String, ByVal logon As String, ByVal dbname As String, ByVal repdb As String, ByVal useremail As String, ByVal userenddate As String, Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "", Optional er As String = "", Optional redo As Boolean = False) As String
        Dim ret As String = String.Empty
        Dim msql As String = String.Empty
        Dim i As Integer
        Dim dv As DataView
        Dim tbl1 As String = String.Empty
        Dim fld1 As String = String.Empty
        Dim tbl2 As String = String.Empty
        Dim fld2 As String = String.Empty
        Dim rep As String = String.Empty
        Dim reporigin As String = dbname & "_JOIN_%"
        If origin = "datadriven" Then
            reporigin = reporigin & "_dd"
        ElseIf origin = "initial" Then
            reporigin = reporigin & "_in"
        ElseIf origin = "custom" Then
            reporigin = reporigin & "_cu"
        End If
        Dim tmst As String = Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
        Try
            If dvusertbls Is Nothing OrElse dvusertbls.Rows.Count = 0 Then
                msql = "SELECT * FROM OURReportSQLquery WHERE DOING='JOIN' AND Param1 LIKE '%" & repdb.Trim.Replace(" ", "%") & "%' AND Param2 = '" & origin & "' AND (Param3='' OR Param3 IS NULL)  AND ReportId = '" & dbname & "_joins'"
                dv = mRecords(msql, er)
            Else
                dv = dvusertbls.DefaultView
            End If

            If er = "" AndAlso Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
                For i = 0 To dv.Table.Rows.Count - 1
                    If tbl1 <> dv.Table.Rows(i)("Tbl1") OrElse tbl2 <> dv.Table.Rows(i)("Tbl2") OrElse (tbl1 = dv.Table.Rows(i)("Tbl1") And tbl2 = dv.Table.Rows(i)("Tbl2") And (fld1 <> dv.Table.Rows(i)("Tbl1Fld1") Or fld2 <> dv.Table.Rows(i)("Tbl2Fld2"))) Then
                        'new report
                        tbl1 = dv.Table.Rows(i)("Tbl1")
                        tbl2 = dv.Table.Rows(i)("Tbl2")
                        fld1 = dv.Table.Rows(i)("Tbl1Fld1")
                        fld2 = dv.Table.Rows(i)("Tbl2Fld2")

                        If JoinExist(tbl1, tbl2, fld1, fld2, repdb, reporigin) Then
                            Continue For
                        End If

                        rep = dbname & "_JOIN_" & tmst & "_" & i.ToString
                        rep = rep.Replace(".", "")
                        If dv.Table.Rows(i)("Param2") = "datadriven" Then
                            rep = rep & "_dd"
                            ret = MakeReportOfJoinAnd15fieldsFromEachTable(rep, tbl1, tbl2, fld1, fld2, logon, dbname, repdb, useremail, userenddate, userconnstr, userconnprv, "datadriven")

                        ElseIf dv.Table.Rows(i)("Param2") = "initial" Then
                            rep = rep & "_in"
                            ret = MakeReportOfJoinAnd15fieldsFromEachTable(rep, tbl1, tbl2, fld1, fld2, logon, dbname, repdb, useremail, userenddate, userconnstr, userconnprv, "initial")

                        ElseIf dv.Table.Rows(i)("Param2") = "custom" Then
                            rep = rep & "_cu"
                            ret = MakeReportOfJoinAnd15fieldsFromEachTable(rep, tbl1, tbl2, fld1, fld2, logon, dbname, repdb, useremail, userenddate, userconnstr, userconnprv, "custom")

                        Else
                            ret = MakeReportOfJoinAnd15fieldsFromEachTable(rep, tbl1, tbl2, fld1, fld2, logon, dbname, repdb, useremail, userenddate, userconnstr, userconnprv, "initial")

                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Public Function RedoReportForJoin(ByVal indx As String, ByVal logon As String, ByVal dbname As String, ByVal repdb As String, ByVal useremail As String, ByVal userenddate As String, Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "", Optional er As String = "", Optional redo As Boolean = False) As String
        Dim ret As String = String.Empty
        Dim msql As String = String.Empty
        Dim origin As String = String.Empty
        Dim i As Integer
        Dim dv As DataView
        Dim rep As String = String.Empty
        Try
            msql = "SELECT * FROM OURReportSQLquery WHERE DOING='JOIN' AND Param1 LIKE '%" & repdb.Trim.Replace(" ", "%") & "%' AND Indx = " & indx.ToString
            dv = mRecords(msql, er)
            If er = "" AndAlso Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count = 1 Then
                i = 0
                origin = dv.Table.Rows(i)("Param2")
                rep = dv.Table.Rows(i)("ReportId")

                msql = "SELECT * FROM OURReportInfo WHERE ReportDB LIKE '%" & repdb.Trim.Replace(" ", "%") & "%' AND ReportId='" & rep & "'"
                If Not HasRecords(msql) Then
                    'If report does not exist
                    Dim tbl1 As String = String.Empty
                    Dim fld1 As String = String.Empty
                    Dim tbl2 As String = String.Empty
                    Dim fld2 As String = String.Empty
                    'Dim tmst As String = Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
                    'rep = dbname & "_JOIN_" & tmst & "_" & i.ToString
                    'rep = rep.Replace(".", "")
                    'If dv.Table.Rows(i)("Param2") = "datadriven" Then
                    '    rep = rep & "_dd"
                    '    'ret = MakeReportOfJoinAnd15fieldsFromEachTable(rep, tbl1, tbl2, fld1, fld2, logon, dbname, repdb, useremail, userenddate, userconnstr, userconnprv, "datadriven")
                    'ElseIf dv.Table.Rows(i)("Param2") = "initial" Then
                    '    rep = rep & "_in"
                    '    'ret = MakeReportOfJoinAnd15fieldsFromEachTable(rep, tbl1, tbl2, fld1, fld2, logon, dbname, repdb, useremail, userenddate, userconnstr, userconnprv, "initial")
                    'ElseIf dv.Table.Rows(i)("Param2") = "custom" Then
                    '    rep = rep & "_cu"
                    '    'ret = MakeReportOfJoinAnd15fieldsFromEachTable(rep, tbl1, tbl2, fld1, fld2, logon, dbname, repdb, useremail, userenddate, userconnstr, userconnprv, "custom")
                    'End If
                    'new report
                    tbl1 = dv.Table.Rows(i)("Tbl1")
                    tbl2 = dv.Table.Rows(i)("Tbl2")
                    fld1 = dv.Table.Rows(i)("Tbl1Fld1")
                    fld2 = dv.Table.Rows(i)("Tbl2Fld2")
                    ret = MakeReportOfJoinAnd15fieldsFromEachTable(rep, tbl1, tbl2, fld1, fld2, logon, dbname, repdb, useremail, userenddate, userconnstr, userconnprv, origin)
                End If

            End If
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Public Function ReportForTableExist(ByVal tbl As String, ByRef er As String, ByVal userdb As String, Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "") As Boolean
        Dim b As Boolean = False
        Dim msql As String = String.Empty
        Try
            'msql = "SELECT * FROM OURReportSQLquery WHERE DOING='SELECT' AND Param1 LIKE '%" & userdb.Trim.Replace(" ", "%") & "%' AND UCASE([Tbl1])='" & tbl.ToUpper & "'"
            msql = "SELECT * FROM OURReportSQLquery WHERE DOING='SELECT' AND UCASE([Tbl1])='" & tbl.ToUpper & "'"
            b = HasRecords(msql)
        Catch ex As Exception
            er = ex.Message
        End Try
        Return b
    End Function
    Public Function DeleteUserJoinsAndReports(ByVal dvusertbls As DataView, ByVal origin As String, ByVal userdb As String, ByVal csv As String, Optional ByRef er As String = "", Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "") As String
        Dim ret As String = String.Empty
        Dim msql As String = String.Empty
        Dim dv As DataView
        Try
            msql = "SELECT DISTINCT ReportId,Tbl1,Tbl2 FROM OURReportSQLquery WHERE (Param2 = '" & origin & "' AND (Param3='' OR Param3 IS NULL) AND Doing='JOIN' AND Param1 LIKE '%" & userdb.Trim.Replace(" ", "%") & "%' )"
            dv = mRecords(msql, er)
            If er = "" AndAlso Not dv Is Nothing AndAlso Not dv.Table Is Nothing Then
                For i = 0 To dv.Table.Rows.Count - 1
                    'delete report from all tables including joins
                    If csv = "yes" Then
                        If IsValueInColumnTable(dv.Table.Rows(i)("Tbl1").ToString, "TABLE_NAME", False, dvusertbls) AndAlso IsValueInColumnTable(dv.Table.Rows(i)("Tbl2").ToString, "TABLE_NAME", False, dvusertbls) Then
                            ret = DeleteReport(dv.Table.Rows(i)("ReportId").ToString)
                        End If
                    Else
                        ret = DeleteReport(dv.Table.Rows(i)("ReportId").ToString)
                    End If
                Next
            End If

            ''delete datadriven or initial reports in userdb 
            'msql = "SELECT * FROM OURReportInfo WHERE (Param4Type = '" & origin & "' AND ReportDB LIKE '%" & userdb.Trim.Replace(" ", "%") & "%' )"
            'dv = mRecords(msql, er)
            'If er = "" AndAlso Not dv Is Nothing AndAlso Not dv.Table Is Nothing Then
            '    For i = 0 To dv.Table.Rows.Count - 1
            '        ret = DeleteReport(dv.Table.Rows(i)("ReportId").ToString)
            '    Next
            'End If
            ''clearing not deleted datadriven or initial Joins in userdb 
            'msql = "DELETE FROM OURReportSQLquery WHERE (Param2 = '" & origin & "' AND (Param3='' OR Param3 IS NULL) AND Doing='JOIN' AND Param1 LIKE '%" & Session("UserDB").ToString.Trim.Replace(" ", "%") & "%' )"
            'er = ExequteSQLquery(msql)
        Catch ex As Exception
            er = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Public Function IsValueInColumnTable(ByVal val As String, ByVal col As String, ByVal isnum As Boolean, ByRef dv As DataView) As Boolean
        Dim b As Boolean = False
        Dim fltr As String = String.Empty
        If isnum Then
            fltr = col & "=" & val
        Else
            fltr = col & "='" & val & "'"
        End If
        dv.RowFilter = fltr
        Dim dtt As New DataTable
        dtt = dv.ToTable
        If dtt.Rows Is Nothing Then
            Return b
        End If
        If dtt.Rows.Count > 0 Then
            b = True
        Else
            Return b
        End If
        Return b
    End Function
    Public Function HowManySomeValueInColumnTable(ByVal val As String, ByVal col As String, ByVal isnum As Boolean, ByVal dv As DataView) As Integer
        Dim n As Integer = 0
        Dim fltr As String = String.Empty
        If isnum Then
            fltr = col & "=" & val
        Else
            fltr = col & "='" & val & "'"
        End If
        dv.RowFilter = fltr
        Dim dtt As New DataTable
        dtt = dv.ToTable
        If dtt.Rows Is Nothing Then
            Return 0
        End If
        If dtt.Rows.Count > 0 Then
            n = dtt.Rows.Count
        Else
            Return 0
        End If
        Return n
    End Function
    Public Function InsertTableIntoOURUserTables(ByVal tblname As String, ByVal tblfriendlyname As String, ByVal unit As String, ByVal logon As String, ByVal userdb As String, Optional ByVal groupadd As String = "", Optional ByVal rep As String = "") As String
        'different from the same function in ReportEdit, groups are here
        Dim ret As String = String.Empty
        Dim msql As String = String.Empty
        If Not HasRecords("SELECT * FROM OURUserTables WHERE Unit='" & unit & "' AND UserID='" & logon & "' AND [Group]='" & groupadd & "' AND TableName='" & tblname & "' AND UserDB='" & userdb & "'") Then
            'register the table tblname in OURUserTables of OURdb
            msql = "INSERT INTO OURUserTables (Unit,UserID,TableName,Prop1,UserDB,Prop2,[Group]) VALUES ('" & unit & "','" & logon & "','" & tblname & "','" & tblfriendlyname & "','" & userdb & "','" & rep & "','" & groupadd & "')"
            ret = ExequteSQLquery(msql)
            'WriteToAccessLog(tblname, "Load table into OURUserTables: " & msql & " - with result: " & ret, 111)
        End If
        Return ret
    End Function
    Public Function GetTableFriendlyName(ByVal tblname As String, ByVal unit As String, ByVal logon As String, ByVal userdb As String, Optional ByVal rep As String = "", Optional ByVal groupadd As String = "") As String
        'get friendly table name from OURUserTables
        Dim ret As String = String.Empty
        Dim dv As DataView
        Dim msql As String = String.Empty
        If tblname.Trim = "" And rep.Trim = "" And groupadd.Trim = "" Then
            Return ""
        End If
        msql = "SELECT * FROM OURUserTables WHERE Unit='" & unit & "' AND UserID='" & logon & "' And UserDB='" & userdb & "' "
        If tblname.Trim <> "" Then
            msql = msql & " AND TableName='" & tblname & "'"
        End If
        If groupadd.Trim <> "" Then
            msql = msql & " AND [Group]='" & groupadd & "'"
        End If
        If rep.Trim <> "" Then
            msql = msql & " AND Prop2='" & rep & "' ORDER BY TableName"
        End If

        dv = mRecords(msql, ret)
        If ret <> "" Then
            Return "ERROR!! " & ret
        End If
        If dv Is Nothing OrElse dv.Count = 0 OrElse dv.Table Is Nothing Then
            Return tblname
        Else
            ret = dv.Table.Rows(0)("Prop1")
            If ret = "" Then ret = tblname
        End If
        Return ret
    End Function
    Public Function GetReportTablesFromOURUserTables(ByVal rep As String, ByVal unit As String, ByVal logon As String, ByVal userdb As String, Optional ByVal er As String = "") As DataView
        Dim ret As String = String.Empty
        Dim dv As DataView
        Dim msql As String = "SELECT [TableName] FROM [OURUserTables] WHERE [Unit]='" & unit & "' AND [UserID]='" & logon & "' AND [UserDB]='" & userdb & "' AND [Prop2]='" & rep & "'"
        dv = mRecords(msql, ret)
        If ret <> "" Then
            er = "ERROR!! " & ret
        End If
        If dv Is Nothing OrElse dv.Count = 0 OrElse dv.Table Is Nothing Then
            Return Nothing
        Else
            Return dv
        End If
    End Function
    Public Function CreateTable(ByVal tblname As String, ByVal db As String, Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "", Optional ByRef er As String = "") As Boolean
        'TEMPLATE: create tblname with only ID field
        Dim ret As String = String.Empty
        If Not TableExists(tblname, userconnstr, userconnprv) Then
            Dim sqlc As String = String.Empty
            If userconnprv = "MySql.Data.MySqlClient" Then
                sqlc = "CREATE TABLE `" & db & "`.`" & tblname & "`(timstmp TEXT NULL DEFAULT NULL, ID int(11) NOT NULL AUTO_INCREMENT PRIMARY KEY );"

            ElseIf userconnprv = "Oracle.ManagedDataAccess.Client" Then
                sqlc = "CREATE TABLE " & tblname & " ( "
                sqlc &= "timstmp VARCHAR2(4000 CHAR) DEFAULT NULL,"
                sqlc &= "ID Integer GENERATED ALWAYS As IDENTITY,"
                sqlc &= "CONSTRAINT " & tblname.ToUpper & "_PK PRIMARY KEY (ID)"
                sqlc &= ")"

            ElseIf userconnprv = "Npgsql" Then  'PostgreSQL  Npgsql
                sqlc = "CREATE TABLE `" & tblname & "`("
                sqlc = sqlc & "`timstmp` Text NULL DEFAULT NULL "
                sqlc = sqlc & "[ID] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                sqlc = sqlc & "CONSTRAINT [" & tblname & "_pkey] PRIMARY KEY ([ID]))"

            ElseIf userconnprv = "Sqlite" Then
                sqlc = "CREATE TABLE [" & tblname & "] ("
                sqlc = sqlc & " [timstmp] [nvarchar](4000) NULL,"
                sqlc = sqlc & " [ID] INTEGER PRIMARY KEY AUTOINCREMENT)"

            Else 'SQL Server, Cache, or Iris, Odbc, OleDb
                sqlc = "CREATE TABLE [" & tblname & "] ("
                sqlc = sqlc & " [timstmp] [nvarchar](4000) NULL,"
                sqlc = sqlc & " [ID] [Int] IDENTITY(1,1) Not NULL)"
            End If
            ret = ExequteSQLquery(sqlc, userconnstr, userconnprv)
            sqlc = "INSERT INTO " & tblname & " (timstmp) VALUES ('" & Now.ToString & "')"
            ret = ExequteSQLquery(sqlc, userconnstr, userconnprv)
        End If
        Return ret
    End Function
    Public Function CorrectTablesArrayFromSQLquery(ByVal sql As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "", Optional ByRef er As String = "") As String
        'Dim dt As New DataTable
        If sql = "" Then
            Return sql
        End If
        Dim i, j, n, k, l As Integer
        Dim tbl As String = String.Empty
        Dim tblspace As String = String.Empty
        Dim sqlj As String = String.Empty
        Try
            i = sql.ToUpper.IndexOf(" FROM ")
            sql = sql.Substring(i + 6).Trim
            k = sql.ToUpper.IndexOf("WHERE")
            If k > 0 Then
                sql = sql.Substring(0, k)
            End If
            k = sql.ToUpper.IndexOf("GROUP BY")
            If k > 0 Then
                sql = sql.Substring(0, k)
            End If
            k = sql.ToUpper.IndexOf("ORDER BY")
            If k > 0 Then
                sql = sql.Substring(0, k)
            End If
            k = sql.ToUpper.IndexOf("HAVING")
            If k > 0 Then
                sql = sql.Substring(0, k)
            End If
            'sql = sql.Trim & " "
            'at this point sql is list of tables separated with commas or JOINs

            k = sql.ToUpper.IndexOf(" JOIN ")
            If k < 0 Then
                Dim ar = sql.Split(",")
                For j = 0 To ar.Length - 1
                    tbl = ar(j)
                    If tbl.Trim.Length = 0 Then
                        Exit For
                    End If
                    tbl = tbl.Replace("`", "")
                    tbl = tbl.Replace("[", "").Replace("]", "").Trim
                    tbl = tbl.Replace("""", "")
                    'check if tbl is a table
                    If myconstr.Trim <> "" AndAlso Not TableExists(tbl.Trim, myconstr, myconprv, er) Then
                        Continue For
                    Else
                        'myRow = dt.NewRow
                        'myRow.Item(0) = tbl
                        'dt.Rows.Add(myRow)
                        ar(j) = FixReservedWords(tbl.Trim, myconprv) '"[" & tbl & "]"
                        sql = sql & ar(j) & ", "
                    End If
                Next
                If sql.EndsWith(", ") Then
                    sql = sql.Substring(0, sql.LastIndexOf(","))
                End If
                Return sql
            Else
                sqlj = sql '.ToUpper
                sqlj = sqlj.Replace(" INNER JOIN ", " JOIN ")
                sqlj = sqlj.Replace(" LEFT JOIN ", " JOIN ")
                sqlj = sqlj.Replace(" RIGHT JOIN ", " JOIN ")
                sqlj = sqlj.Replace(" OUTER JOIN ", " JOIN ")
                sqlj = sqlj.Replace(" JOIN ", "|")
                'after JOIN
                Dim ar = sqlj.Split("|")
                For j = 0 To ar.Length - 1
                    tbl = ar(j)
                    If tbl.Trim.Length = 0 Then
                        Exit For
                    End If
                    If j = 0 AndAlso tbl.IndexOf(",") > 0 Then
                        Dim tb = tbl.Split(",")
                        For n = 0 To tb.Length - 1
                            If tb(n).Trim.Length = 0 Then
                                Exit For
                            End If
                            'table name tb(n) put in Row
                            tb(n) = tb(n).Replace("`", "")
                            tb(n) = tb(n).Replace("[", "").Replace("]", "").Trim
                            tb(n) = tb(n).Replace("""", "")
                            'check if tbl is a table
                            If myconstr.Trim <> "" AndAlso Not TableExists(tb(n).Trim, myconstr, myconprv, er) Then
                                Continue For
                            Else
                                'myRow = dt.NewRow
                                'myRow.Item(0) = tb(n).Trim
                                'dt.Rows.Add(myRow)
                                tb(n) = FixReservedWords(tb(n).Trim, myconprv) '"[" & tbl & "]"
                                sql = sql.Replace(ar(j) & ",", tb(n) & ",").Replace(ar(j) & " ", tb(n) & " ").Replace("," & ar(j), "," & tb(n)).Replace(" " & ar(j), " " & tb(n))
                            End If
                        Next
                    End If
                    'remove ON statement
                    l = tbl.ToUpper.IndexOf(" ON ")
                    If l > 0 Then
                        tbl = tbl.Substring(0, l).Trim
                    End If
                    'table name tbl put in Row
                    tbl = tbl.Replace("`", "")
                    tbl = tbl.Replace("[", "").Replace("]", "").Trim
                    tbl = tbl.Replace("""", "")
                    'check if tbl is a table
                    If myconstr.Trim <> "" AndAlso Not TableExists(tbl.Trim, myconstr, myconprv, er) Then
                        Continue For
                    Else
                        tbl = FixReservedWords(tbl.Trim, myconprv) '"[" & tbl & "]"
                        sql = sql.Replace(ar(j) & " ON ", tbl & " ON ")
                    End If
                Next
            End If

        Catch ex As Exception
            er = ex.Message
            Return Nothing
        End Try
        Return sql
    End Function
    Public Function CorrectFieldsArrayFromSQLquery(ByVal Sql As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "", Optional ByRef er As String = "") As String
        Dim selflgs As String = String.Empty
        If Sql = "" Then
            Return ""
        End If
        Dim i, j As Integer
        Dim tbl As String = String.Empty
        Try
            i = Sql.ToUpper.IndexOf(" FROM ")
            If i >= 0 Then Sql = Sql.Substring(0, i).Trim
            If Sql.ToUpper.StartsWith("SELECT ") Then
                Sql = Sql.Substring(6).Trim
            End If
            If Sql.ToUpper.StartsWith("DISTINCT ") Then
                Sql = Sql.Substring(8).Trim
            End If
            If Sql = "*" Then
                Return Sql
            End If
            Dim flds() As String
            flds = Sql.Split(",")
            For i = 0 To flds.Length - 1
                j = flds(i).ToUpper.IndexOf(" AS ")
                If j > 0 Then flds(i) = flds(i).Substring(0, j).Trim
                j = flds(i).ToUpper.IndexOf("(")
                If j > 0 Then flds(i) = flds(i).Substring(j).Trim
                flds(i) = flds(i).Replace(")", "")
                flds(i) = flds(i).Replace("(", "")
                'no need to add table name
                If selflgs = String.Empty Then
                    selflgs = FixReservedWords(flds(i).Trim, myconprv, myconstr)
                Else
                    selflgs = selflgs & ", " & FixReservedWords(flds(i).Trim, myconprv, myconstr)
                End If
            Next
            Return selflgs
        Catch ex As Exception
            Return "" '"ERROR!! " & ex.Message.ToString
        End Try
    End Function
    Public Function CorrectWhereHavingFromSQLquery(ByVal wheresql As String, ByVal dtb As DataTable, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "", Optional ByRef er As String = "") As String
        Dim ret As String = String.Empty
        Dim i, j, n, m, k, l As Integer
        Dim g As Integer = 0 'group number
        If wheresql = "" OrElse dtb Is Nothing OrElse dtb.Rows.Count = 0 Then
            Return wheresql
        End If
        Dim sql As String = wheresql
        Dim dt As New DataTable
        If sql.ToUpper.Trim = "WHERE" Then
            k = sql.ToUpper.IndexOf(" WHERE ")
            sql = sql.Substring(k + 6).Trim
            k = sql.ToUpper.IndexOf("HAVING")
            If k > 0 Then
                sql = sql.Substring(0, k)
            End If
        ElseIf sql.ToUpper.Trim = "HAVING" Then
            k = sql.ToUpper.IndexOf(" HAVING ")
            sql = sql.Substring(k + 7).Trim
            k = sql.ToUpper.IndexOf("WHERE")
            If k > 0 Then
                sql = sql.Substring(0, k)
            End If
        End If
        k = sql.ToUpper.IndexOf(" ORDER BY ")
        If k > 0 Then
            sql = sql.Substring(0, k).Trim
        End If
        k = sql.ToUpper.IndexOf("GROUP BY")
        If k > 0 Then
            sql = sql.Substring(0, k)
        End If

        If sql.ToUpper.Contains("BETWEEN") Then
            sql = AdjustBetweens(sql)
        End If

        'at this point sql is set of conditions connected with AND or OR
        Dim fldfullname As String = String.Empty
        Dim tbl1 As String = String.Empty
        Dim fld1 As String = String.Empty
        Dim tbl2 As String = String.Empty
        Dim fld2 As String = String.Empty
        Dim tbl3 As String = String.Empty
        Dim fld3 As String = String.Empty
        Dim logical As String = "And"
        Dim group As String = String.Empty
        Dim cond As String = String.Empty
        Dim opr As String = String.Empty
        Dim notopr As String = String.Empty
        Dim typ As String = String.Empty
        Dim sta As String = String.Empty
        Dim lside As String = String.Empty
        Dim rside As String = String.Empty
        Dim opers(), notopers() As String
        Dim oper As String = String.Empty
        'opr = ",=,<>,<,>,<=,>=, not between , between, StartsWith , Not StartsWith , Contains , Not Contains , In , Not In , EndsWith , Not EndsWith ,"
        opr = ",=,<>,<,>,<=,>=,Not between,between,not like,like,Not In, In ,"
        notopr = ",<>,=,>=,<=,>,<, Not between, between , Not StartsWith , StartsWith , Not Contains , Contains , Not In , In , Not EndsWith , EndsWith ,"  'Not oper
        opers = opr.Split(",")
        n = opers.Length
        notopers = notopr.Split(",")  'length is the same
        opr = ""
        notopr = ""
        Try
            dt.TableName = "Table1"
            dt.Columns.Add("Tbl1")
            dt.Columns.Add("Tbl1Fld1")
            dt.Columns.Add("Tbl2")
            dt.Columns.Add("Tbl2Fld2")
            dt.Columns.Add("Tbl3")
            dt.Columns.Add("Tbl3Fld3")
            dt.Columns.Add("Oper")
            dt.Columns.Add("Type")
            dt.Columns.Add("comments")
            dt.Columns.Add("RecOrder")
            dt.Columns.Add("Group")
            dt.Columns.Add("Logical")

            Dim myRow As DataRow
            m = 0
            Dim conds() As String

            Dim AndConds() As String
            Dim OrConds() As String
            Dim ok As Boolean = False
            Dim insSQL As String = String.Empty
            Dim sctSQL As String = String.Empty
            Dim ProcessConds As ConditionToProcess()
            Dim CondToProcess As ConditionToProcess
            ReDim ProcessConds(-1)

            sql = sql.Replace(" And ", " AND ").Replace(" and ", " AND ").Replace(" Or ", " OR ").Replace(" or ", " OR ")

            If sql.ToUpper.Contains(" AND ") AndAlso sql.ToUpper.Contains(" OR ") Then
                AndConds = Split(sql, " AND ")
                OrConds = Split(sql, " OR ")
                If IsArrayBalanced(AndConds) Then
                    For i = 0 To AndConds.Length - 1
                        cond = AndConds(i).Replace("(", "").Replace(")", "").Trim
                        If cond.ToUpper.Contains(" OR ") Then
                            g += 1
                            group = "Group " & g.ToString

                            'make group condition to process
                            CondToProcess = New ConditionToProcess
                            CondToProcess.LogicalOperator = "And"
                            CondToProcess.GroupName = group
                            CondToProcess.IsGroup = True
                            l = ProcessConds.Length
                            ReDim Preserve ProcessConds(l)
                            ProcessConds(l) = CondToProcess

                            logical = "Or"
                            OrConds = Split(cond, " OR ")
                            For j = 0 To OrConds.Length - 1
                                cond = OrConds(j)
                                If cond.ToUpper.Contains(" BETWEEN ") Then
                                    cond = cond.Replace(" & ", " AND ")
                                End If
                                l = ProcessConds.Length
                                ReDim Preserve ProcessConds(l)
                                CondToProcess = New ConditionToProcess
                                CondToProcess.Condition = cond
                                CondToProcess.LogicalOperator = logical
                                CondToProcess.GroupName = group
                                ProcessConds(l) = CondToProcess
                            Next
                        Else
                            If cond.ToUpper.Contains(" BETWEEN ") Then
                                cond = cond.Replace(" & ", " AND ")
                            End If

                            l = ProcessConds.Length
                            ReDim Preserve ProcessConds(l)
                            CondToProcess = New ConditionToProcess
                            CondToProcess.Condition = cond
                            CondToProcess.LogicalOperator = "And"
                            CondToProcess.GroupName = String.Empty
                            ProcessConds(l) = CondToProcess
                        End If
                    Next
                ElseIf IsArrayBalanced(OrConds) Then
                    For i = 0 To OrConds.Length - 1
                        cond = OrConds(i).Replace("(", "").Replace(")", "").Trim
                        If cond.ToUpper.Contains(" AND ") Then
                            g += 1
                            group = "Group " & g.ToString

                            'make group condition to process
                            CondToProcess = New ConditionToProcess
                            CondToProcess.LogicalOperator = "Or"
                            CondToProcess.GroupName = group
                            CondToProcess.IsGroup = True
                            l = ProcessConds.Length
                            ReDim Preserve ProcessConds(l)
                            ProcessConds(l) = CondToProcess

                            logical = "And"
                            AndConds = Split(cond, " AND ")
                            For j = 0 To AndConds.Length - 1
                                cond = AndConds(j)
                                If cond.ToUpper.Contains(" BETWEEN ") Then
                                    cond = cond.Replace(" & ", " AND ")
                                End If
                                l = ProcessConds.Length
                                ReDim Preserve ProcessConds(l)
                                CondToProcess = New ConditionToProcess
                                CondToProcess.Condition = cond
                                CondToProcess.LogicalOperator = logical
                                CondToProcess.GroupName = group
                                ProcessConds(l) = CondToProcess
                            Next
                        Else
                            If cond.ToUpper.Contains(" BETWEEN ") Then
                                cond = cond.Replace(" & ", " AND ")
                            End If

                            l = ProcessConds.Length
                            ReDim Preserve ProcessConds(l)
                            CondToProcess = New ConditionToProcess
                            CondToProcess.Condition = cond
                            CondToProcess.LogicalOperator = "Or"
                            CondToProcess.GroupName = String.Empty
                            ProcessConds(l) = CondToProcess
                        End If
                    Next
                End If
            Else
                If sql.ToUpper.Contains(" OR ") Then logical = "Or"
                If logical = "And" Then
                    conds = Split(sql, " AND ")
                Else
                    conds = Split(sql, " OR ")
                End If

                For i = 0 To conds.Length - 1
                    cond = conds(i)
                    If cond.ToUpper.Contains(" BETWEEN ") Then
                        cond = cond.Replace(" & ", " AND ")
                    End If
                    l = ProcessConds.Length
                    ReDim Preserve ProcessConds(l)
                    CondToProcess = New ConditionToProcess
                    CondToProcess.Condition = cond
                    CondToProcess.LogicalOperator = logical
                    CondToProcess.GroupName = String.Empty
                    ProcessConds(l) = CondToProcess
                Next
            End If

            For i = 0 To ProcessConds.Length - 1

                'handle group
                If ProcessConds(i).IsGroup Then
                    logical = ProcessConds(i).LogicalOperator
                    group = ProcessConds(i).GroupName
                    m = m + 1
                    myRow = dt.NewRow
                    myRow.Item("Tbl1") = String.Empty
                    myRow.Item("Tbl1Fld1") = String.Empty
                    myRow.Item("Tbl2") = String.Empty
                    myRow.Item("Tbl2Fld2") = String.Empty
                    myRow.Item("Tbl3") = String.Empty
                    myRow.Item("Tbl3Fld3") = String.Empty
                    myRow.Item("Oper") = String.Empty
                    myRow.Item("Type") = String.Empty
                    myRow.Item("comments") = String.Empty
                    myRow.Item("RecOrder") = m
                    myRow.Item("Logical") = logical
                    myRow.Item("Group") = group
                    dt.Rows.Add(myRow)

                    Continue For
                End If

                cond = ProcessConds(i).Condition
                If cond.Trim.Length = 0 Then
                    Continue For
                End If
                logical = ProcessConds(i).LogicalOperator
                If logical = String.Empty Then logical = "And"
                group = ProcessConds(i).GroupName
                m = m + 1
                lside = ""
                rside = ""
                typ = ""
                sta = ""
                opr = ""
                oper = ""

                For j = 0 To n - 1
                    If opers(j).Trim = "" Then Continue For
                    l = cond.ToUpper.IndexOf(opers(j).ToUpper)
                    If cond.IndexOf("'") < l Then
                        Continue For   'oper(j) is in the right side of condition
                    End If
                    If l > 0 Then
                        opr = opers(j).Trim
                        oper = opr
                        If opr.ToUpper.Contains("BETWEEN") Then _
                            cond = cond.Replace(" & ", " AND ")
                        lside = cond.Substring(0, l).Trim.Replace("(", "").Replace(")", "")

                        rside = cond.Substring(l + opers(j).Length).Trim.Replace("(", "").Replace(")", "").Replace("""", "")
                        If opr.ToUpper.Contains("LIKE") Then
                            Dim res As String = rside.Replace("'", "")
                            Dim first As String = res.Substring(0, 1)
                            Dim last As String = res.Substring(res.Length - 1, 1)
                            If first = "%" And last = "%" Then
                                If opr.ToUpper.Contains("NOT") Then
                                    oper = "Not Contains"
                                Else
                                    oper = "Contains"
                                End If
                            ElseIf first = "%" Then
                                If opr.ToUpper.Contains("NOT") Then
                                    oper = "Not EndsWith"
                                Else
                                    oper = "EndsWith"
                                End If
                            ElseIf last = "%" Then
                                If opr.ToUpper.Contains("NOT") Then
                                    oper = "Not StartsWith"
                                Else
                                    oper = "StartsWith"
                                End If
                            End If
                            rside = rside.Replace("%", "")
                        End If
                        opr = oper
                        Exit For
                    End If
                Next
                If l = -1 Then Continue For

                tbl1 = ""
                fld1 = lside.Trim
                fldfullname = FixTableAndField(dtb, tbl1, fld1, myconstr, myconprv, er)
                If fldfullname = "" Then
                    'lside is static
                    tbl2 = ""
                    fld2 = ""
                    sta = lside.Trim
                    'opr = oper
                    typ = "Static"
                    tbl1 = ""
                    fld1 = rside.Trim
                    fldfullname = FixTableAndField(dtb, tbl1, fld1, myconstr, myconprv, er)
                    If fldfullname = "" Then
                        ret = ret & "ERROR!! converting the condition: " & cond
                        Continue For
                    Else
                        tbl3 = ""
                        fld3 = ""
                    End If
                Else
                    If rside.ToUpper.Contains(" AND ") Then  'between
                        Dim b As Boolean = IsDateTimeField(fldfullname, myconstr, myconprv, er)
                        If b Then
                            tbl2 = "Date1"
                            tbl3 = "Date2"
                        Else
                            tbl2 = "Value1"
                            tbl3 = "Value2"
                        End If
                        typ = "Btw"
                        k = rside.ToUpper.IndexOf(" AND ")
                        fld2 = rside.Substring(0, k).Trim
                        If fld2.ToUpper.StartsWith("DATE") AndAlso fld2.Contains("(") AndAlso fld2.Contains(")") Then
                            'tbl2 = "Date1"
                            'relative
                            typ = typ & "RD1"
                        Else 'not relative date
                            fldfullname = FixTableAndField(dtb, "", fld2, myconstr, myconprv, er)
                            If fldfullname = "" Then  'value
                                fld2 = rside.Substring(0, k).Trim
                                If b Then
                                    'tbl2 = "Date1"
                                    typ = typ & "DT1"
                                Else
                                    'tbl2 = ""
                                    typ = typ & "ST1"
                                End If
                            Else 'field
                                typ = typ & "FD1"
                            End If
                        End If
                        'tbl3 = ""
                        fld3 = rside.Substring(k + 5).Trim
                        If fld3.ToUpper.StartsWith("DATE") AndAlso fld3.Contains("(") AndAlso fld3.Contains(")") Then
                            'tbl3 = "Date2"
                            'relative
                            typ = typ & "RD2"
                        Else 'not relative date
                            fldfullname = FixTableAndField(dtb, tbl3, fld3, myconstr, myconprv, er)
                            If fldfullname = "" Then  'value
                                fld3 = rside.Substring(k + 5).Trim
                                If b Then
                                    'tbl3 = "Date1"
                                    typ = typ & "DT2"
                                Else
                                    'tbl2 = ""
                                    typ = typ & "ST2"
                                End If
                            Else 'field
                                typ = typ & "FD2"
                            End If
                        End If
                    Else  'not between
                        fldfullname = FixTableAndField(dtb, tbl2, fld2, myconstr, myconprv, er)
                        If fldfullname = "" Then
                            typ = "Static"
                            sta = rside.Trim
                            tbl2 = ""
                            fld2 = ""
                            tbl3 = ""
                            fld3 = ""
                        Else
                            typ = "Field"
                            tbl3 = ""
                            fld3 = ""
                            sta = fldfullname
                        End If
                    End If
                End If

                myRow = dt.NewRow
                myRow.Item("Tbl1") = tbl1
                myRow.Item("Tbl1Fld1") = fld1
                myRow.Item("Tbl2") = tbl2.Replace("'", "").Replace("""", "")
                myRow.Item("Tbl2Fld2") = fld2.Replace("'", "").Replace("""", "")
                myRow.Item("Tbl3") = tbl3.Replace("'", "").Replace("""", "")
                myRow.Item("Tbl3Fld3") = fld3.Replace("'", "").Replace("""", "")
                myRow.Item("Oper") = opr
                myRow.Item("Type") = typ
                myRow.Item("comments") = sta.Replace("'", "").Replace("""", "")
                myRow.Item("RecOrder") = m
                myRow.Item("Logical") = logical
                myRow.Item("Group") = group
                dt.Rows.Add(myRow)
                'insert 
                Dim fieldtype As String = GetFieldDataType(tbl1, fld1, myconstr, myconprv)
                Dim IsDate As Boolean = ((fieldtype.ToUpper <> "TIME") AndAlso (fieldtype.ToUpper.Contains("DATE") OrElse fieldtype.ToUpper.Contains("TIME")))

                Dim sorttype As String = typ
            Next

            'make WHERE statement from dt
            'WHEREs
            Dim val As String = String.Empty
            'Dim typ As String = String.Empty

            Dim RecOrder As String = String.Empty
            'Dim Logical As String = String.Empty
            'Dim Group As String = String.Empty

            Dim HasNot As Boolean = False
            Dim qt As String = """"
            Dim dblqt = qt & qt
            Dim dv As New DataView
            dv = dt.DefaultView
            Dim fltr As String = String.Empty
            If Not dt Is Nothing AndAlso Not dt.Rows Is Nothing AndAlso dt.Rows.Count > 0 Then
                'Dim myprovider As String = userconprv
                Dim qfield As String = String.Empty
                ret = ret & "  WHERE "

                Dim htGroup As New Hashtable
                Dim PrevGroup As String = String.Empty
                For i = 0 To dt.Rows.Count - 1
                    'ret = ret & " ( "
                    oper = dt.Rows(i)("Oper").ToString
                    tbl1 = FixReservedWords(CorrectTableNameWithDots(dt.Rows(i)("Tbl1").ToString, myconprv), myconprv, myconstr)
                    fld1 = FixReservedWords(CorrectFieldNameWithDots(dt.Rows(i)("Tbl1Fld1").ToString), myconprv, myconstr)
                    tbl2 = FixReservedWords(CorrectTableNameWithDots(dt.Rows(i)("Tbl2").ToString, myconprv), myconprv, myconstr)
                    fld2 = FixReservedWords(CorrectFieldNameWithDots(dt.Rows(i)("Tbl2Fld2").ToString), myconprv, myconstr)
                    tbl3 = FixReservedWords(CorrectTableNameWithDots(dt.Rows(i)("Tbl3").ToString, myconprv), myconprv, myconstr)
                    fld3 = FixReservedWords(CorrectFieldNameWithDots(dt.Rows(i)("Tbl3Fld3").ToString), myconprv, myconstr)
                    fltr = ""

                    val = dt.Rows(i)("Comments").ToString
                    logical = dt.Rows(i)("Logical").ToString
                    group = dt.Rows(i)("Group").ToString

                    If (group <> String.Empty) Then
                        If htGroup(group) Is Nothing Then
                            If PrevGroup <> String.Empty Then
                                ret &= " )"
                            End If
                            PrevGroup = group
                            htGroup(group) = 1

                            'logical = GetConditionGroupLogical(rep, group)  '!!!!!!
                            'Dim sql = "SELECT Logical FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND DOING = 'GROUP' AND " & FixReservedWords("Group") & " = '" & GroupName & "'"
                            logical = ""
                            'filter dt for Group=group
                            fltr = "DOING = 'GROUP' AND Group='" & group & "'"
                            dv.RowFilter = fltr
                            If dv.ToTable.DefaultView.Table.Rows.Count = 1 Then
                                logical = dv.ToTable.Rows(0)("Logical").ToString
                            End If
                            fltr = ""

                            If i > 0 Then
                                logical = logical & " ("
                            Else
                                logical = " ("
                            End If
                        End If
                    Else
                        If PrevGroup <> String.Empty Then
                            ret &= " )"
                            PrevGroup = String.Empty
                        End If

                    End If
                    If logical = String.Empty Then logical = "And"

                    If logical = " (" OrElse i > 0 Then _
                        ret = ret & " " & logical.ToUpper & " "

                    If val = dblqt Then val = ""

                    typ = dt.Rows(i)("Type").ToString.Trim

                    HasNot = oper.ToUpper.Contains("NOT")
                    If typ = "Field" Then   'to field
                        qfield = tbl2 & "." & fld2
                        If oper.ToUpper.Contains("STARTSWITH") Then
                            If HasNot Then
                                oper = "Not Like"
                            Else
                                oper = "Like"
                            End If
                            If myconprv.StartsWith("InterSystems.Data.") Then
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " STRING(" & qfield & ",'%')) "
                            Else
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " CONCAT(" & qfield & ",'%')) "
                            End If
                        ElseIf oper.ToUpper.Contains("CONTAINS") Then
                            If HasNot Then
                                oper = "Not Like"
                            Else
                                oper = "Like"
                            End If
                            If myconprv.StartsWith("InterSystems.Data.") Then
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " STRING('%'," & qfield & ",'%')) "
                            Else
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " CONCAT('%'," & qfield & ",'%')) "
                            End If
                        ElseIf oper.ToUpper.Contains("ENDSWITH") Then
                            If HasNot Then
                                oper = "Not Like"
                            Else
                                oper = "Like"
                            End If
                            If myconprv.StartsWith("InterSystems.Data.") Then
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " STRING('%'," & qfield & ")) "
                            Else
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " CONCAT('%'," & qfield & ")) "
                            End If
                        Else
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & qfield & ") "
                        End If
                    ElseIf typ = "RelDate" Then
                        If oper.ToUpper.Contains("STARTSWITH") Then
                            If HasNot Then
                                oper = "Not Like"
                            Else
                                oper = "Like"
                            End If
                            If myconprv.StartsWith("InterSystems.Data.") Then
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " STRING(" & val & ",'%')) "
                            Else
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " CONCAT(" & val & ",'%')) "
                            End If
                        ElseIf oper.ToUpper.Contains("CONTAINS") Then
                            If HasNot Then
                                oper = "Not Like"
                            Else
                                oper = "Like"
                            End If
                            If myconprv.StartsWith("InterSystems.Data.") Then
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " STRING('%'," & val & ",'%')) "
                            Else
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " CONCAT('%'," & val & ",'%')) "
                            End If
                        ElseIf oper.ToUpper.Contains("ENDSWITH") Then
                            If HasNot Then
                                oper = "Not Like"
                            Else
                                oper = "Like"
                            End If
                            If myconprv.StartsWith("InterSystems.Data.") Then
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " STRING('%'," & val & ")) "
                            Else
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " CONCAT('%'," & val & ")) "
                            End If
                        Else
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & val & ") "
                        End If
                    ElseIf typ = "Static" Then   'to static
                        If TblFieldIsNumeric(tbl1, fld1, myconstr, myconprv) Then  'numeric
                            If oper.ToUpper = "IN" OrElse oper.ToUpper = "NOT IN" Then
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " (" & val & ")) "
                            ElseIf oper.ToUpper.Contains("STARTSWITH") Then
                                If HasNot Then
                                    oper = "Not Like"
                                Else
                                    oper = "Like"
                                End If
                                If myconprv.StartsWith("InterSystems.Data.") Then
                                    ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " STRING(" & val & ",'%')) "
                                Else
                                    ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " CONCAT(" & val & ",'%')) "
                                End If
                            ElseIf oper.ToUpper.Contains("CONTAINS") Then
                                If HasNot Then
                                    oper = "Not Like"
                                Else
                                    oper = "Like"
                                End If
                                If myconprv.StartsWith("InterSystems.Data.") Then
                                    ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " STRING('%'," & val & ",'%')) "
                                Else
                                    ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " CONCAT('%'," & val & ",'%')) "
                                End If
                            ElseIf oper.ToUpper.Contains("ENDSWITH") Then
                                If HasNot Then
                                    oper = "Not Like"
                                Else
                                    oper = "Like"
                                End If
                                If myconprv.StartsWith("InterSystems.Data.") Then
                                    ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " STRING('%'," & val & ")) "
                                Else
                                    ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " CONCAT('%'," & val & ")) "
                                End If
                            ElseIf oper.ToUpper.Contains("IS NULL") Then
                                If Not HasNot Then
                                    ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & ") "
                                Else
                                    ret = ret & " ( Not " & tbl1 & "." & fld1 & " IS NULL) "
                                End If
                            Else
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & val & ") "
                            End If
                        Else                                                                                       'string, date, time, or else
                            If oper.ToUpper = "IN" OrElse oper.ToUpper = "NOT IN" Then
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " ('" & val.Replace(",", "','") & "')) "
                            ElseIf oper.ToUpper.Contains("STARTSWITH") Then
                                If HasNot Then
                                    oper = "Not Like"
                                Else
                                    oper = "Like"
                                End If
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '" & val & "%') "
                            ElseIf oper.ToUpper.Contains("CONTAINS") Then
                                If HasNot Then
                                    oper = "Not Like"
                                Else
                                    oper = "Like"
                                End If
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '%" & val & "%') "
                            ElseIf oper.ToUpper.Contains("ENDSWITH") Then
                                If HasNot Then
                                    oper = "Not Like"
                                Else
                                    oper = "Like"
                                End If
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '%" & val & "') "
                            ElseIf oper.ToUpper.Contains("IS NULL") Then
                                If Not HasNot Then
                                    ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & ") "
                                Else
                                    ret = ret & " ( Not " & tbl1 & "." & fld1 & " IS NULL) "
                                End If

                            Else
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '" & val & "') "
                            End If
                        End If
                    ElseIf typ = "DateTime" Then
                        If oper.ToUpper.Contains("STARTSWITH") Then
                            If HasNot Then
                                oper = "Not Like"
                            Else
                                oper = "Like"
                            End If
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '" & DateToString(CDate(val), myconprv) & "%') "
                        ElseIf oper.ToUpper.Contains("CONTAINS") Then
                            If HasNot Then
                                oper = "Not Like"
                            Else
                                oper = "Like"
                            End If
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '%" & DateToString(CDate(val), myconprv) & "%') "

                        ElseIf oper.ToUpper.Contains("ENDSWITH") Then
                            If HasNot Then
                                oper = "Not Like"
                            Else
                                oper = "Like"
                            End If
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '%" & DateToString(CDate(val), myconprv) & "') "
                        Else
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '" & DateToString(CDate(val), myconprv) & "') "
                        End If
                    ElseIf typ = "BtwFields" OrElse typ = "BtwFD1FD2" Then   'between fields  BtwFD1FD2
                        ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & tbl2 & "." & fld2 & " AND " & tbl3 & "." & fld3 & ") "
                    ElseIf typ = "BtwDates" OrElse typ = "BtwDT1DT2" Then   'between dates   BtwDT1DT2
                        ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '" & DateToString(CDate(fld2), myconprv) & "' AND '" & DateToString(CDate(fld3), myconprv) & "') "
                    ElseIf typ = "BwRD1Date2" OrElse typ = "BtwRD1DT2" Then   ' between relative date and date   BtwRD1DT2
                        ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & fld2 & " AND '" & DateToString(CDate(fld3), myconprv) & "') "
                    ElseIf typ = "BwDate1RD2" OrElse typ = "BtwDT1RD2" Then   'between date and relative date    BtwDT1RD2
                        ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '" & DateToString(CDate(fld2), myconprv) & "' AND " & fld3 & ") "
                    ElseIf typ = "BtwRD1RD2" Then   'between relative dates   BtwRD1RD2
                        ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & fld2 & " AND " & fld3 & ") "
                    ElseIf typ = "BtwFldDate" OrElse typ = "BtwFD1DT2" Then   'between field and date  BtwFD1DT2
                        ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & tbl2 & "." & fld2 & " And '" & DateToString(CDate(fld3), myconprv) & "') "
                    ElseIf typ = "BtwFldRD2" OrElse typ = "BtwFD1RD2" Then   'between field and relative date   BtwFD1RD2
                        ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & tbl2 & "." & fld2 & " AND " & fld3 & ") "
                    ElseIf typ = "BtwDateFld" OrElse typ = "BtwDT1FD2" Then   'between date and field   BtwDT1FD2
                        ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '" & DateToString(CDate(fld2), myconprv) & "' AND " & tbl3 & "." & fld3 & ") "
                    ElseIf typ = "BtwRD1Fld" OrElse typ = "BtwRD1FD2" Then   'between relative date and field  BtwRD1FD2
                        ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & fld2 & " AND " & tbl3 & "." & fld3 & ") "
                    ElseIf typ = "BtwValues" OrElse typ = "BtwST1ST2" Then   'between static values    BtwST1ST2
                        If TblFieldIsNumeric(tbl1, fld1, myconstr, myconprv) Then
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & fld2 & " AND " & fld3 & ") "
                        Else
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '" & fld2 & "' AND '" & fld3 & "') "
                        End If
                    ElseIf typ = "BtwValFld" OrElse typ = "BtwST1FD2" Then   'between static value and field BtwST1FD2
                        If TblFieldIsNumeric(tbl1, fld1, myconstr, myconprv) Then
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & fld2 & " AND " & tbl3 & "." & fld3 & ") "
                        Else
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '" & fld2 & "' AND " & tbl3 & "." & fld3 & ") "
                        End If
                    ElseIf typ = "BtwFldVal" OrElse typ = "BtwFD1ST2" Then   'between field and static value  BtwFD1ST2
                        If TblFieldIsNumeric(tbl1, fld1, myconstr, myconprv) Then
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & tbl2 & "." & fld2 & " And " & fld3 & ") "
                        Else
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & tbl2 & "." & fld2 & " And '" & fld3 & "') "
                        End If

                    End If

                    If i = dt.Rows.Count - 1 AndAlso PrevGroup <> "" Then ret &= ")"

                Next
            End If
            Return ret
        Catch ex As Exception
            er = ex.Message
            Return "ERROR!! " & er
        End Try
        Return ret
    End Function
    Public Function CorrectTablesJoinsFromSQLquery(ByVal msql As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "", Optional ByRef er As String = "") As String
        Dim ret As String = String.Empty
        Dim sql As String = msql.Trim 'dri.Rows(0)("SQLquerytext").ToString
        If sql = "" Then
            Return msql
            Exit Function
        End If
        sql = sql.Replace("`", "").Replace("[", "").Replace("]", "").Replace("""", "")
        Dim ddt As DataTable = GetListOfTablesFromSQLquery(msql, myconstr, myconprv, er)
        Dim i, j, n, m, k, l, q As Integer
        Dim tbl As String = String.Empty
        Dim tbls As String = String.Empty
        Dim fld2 As String = String.Empty
        Dim tbl1 As String = String.Empty
        Dim tbl2 As String = String.Empty
        Dim fld1 As String = String.Empty
        Dim typ As String = String.Empty
        Dim sqlu As String = String.Empty
        Dim sqlj As String = String.Empty
        Dim sqlon As String = String.Empty
        Try
            i = sql.ToUpper.IndexOf(" FROM ")
            sql = sql.Substring(i + 6).Trim
            k = sql.ToUpper.IndexOf("WHERE")
            If k > 0 Then
                sql = sql.Substring(0, k)
            End If
            k = sql.ToUpper.IndexOf("GROUP BY")
            If k > 0 Then
                sql = sql.Substring(0, k)
            End If
            k = sql.ToUpper.IndexOf("ORDER BY")
            If k > 0 Then
                sql = sql.Substring(0, k)
            End If
            k = sql.ToUpper.IndexOf("HAVING")
            If k > 0 Then
                sql = sql.Substring(0, k)
            End If

            'at this point here can be sets of Joins and single tables separated by commas
            Dim sqls() As String
            sqls = sql.Split(",")
            For q = 0 To sqls.Length - 1
                sql = sqls(q).Trim
                If sql.ToUpper.IndexOf(" ON ") < 0 Then 'add single tables to tbls separated by commas
                    'If tbls = "" Then
                    '    tbls = Sql
                    'Else
                    '    tbls = tbls & "," & Sql
                    'End If
                    msql = ReplaceWholeWord(msql, sql, """" & sql & """")
                    msql = msql.Replace("""""", """")
                    Continue For
                End If

                Dim oper As String = "1"
                'at this point sql is list of tables separated with JOINs, no commas
                sqlu = sql.ToUpper & " "
                k = sqlu.IndexOf(" JOIN ")
                If k > 0 Then
                    Dim tbar() As String = Split(sqlu, " JOIN ")
                    m = tbar.Length
                    For j = 0 To m - 1
                        l = tbar(j).Length
                        tbl = tbar(j).Trim
                        If tbl.Trim.Length = 0 Then
                            Exit For
                        End If
                        If j = 0 Then
                            n = tbl.IndexOf(" ")  'tbl has " " in the end
                            If n > 0 Then
                                tbl1 = tbl.Substring(0, n)
                                tbl1 = FixTableName(tbl1, myconstr, myconprv, er)
                                typ = tbl.Substring(n).Trim.ToUpper  'LEFT, RIGHT, INNER, OUTER or empty
                                If typ <> "LEFT" AndAlso typ <> "RIGHT" AndAlso typ <> "OUTER" Then
                                    typ = "INNER"
                                End If
                                typ = typ & " JOIN"
                            End If
                            Continue For
                        End If
                        sqlj = tbl.Trim.Substring(tbl.LastIndexOf(" ")).Trim.ToUpper
                        If sqlj = "LEFT" OrElse sqlj = "RIGHT" OrElse sqlj = "OUTER" OrElse sqlj = "INNER" Then
                            tbl = tbl.Trim.Substring(0, tbl.LastIndexOf(" ")).Trim.ToUpper
                        End If
                        If tbl.IndexOf(" ON ") > 0 Then  'it is not the very first table
                            tbl2 = tbl.Substring(0, tbl.ToUpper.IndexOf(" ON ")).Trim
                            tbl2 = FixTableName(tbl2, myconstr, myconprv, er)
                            sqlj = tbl.Substring(tbl.ToUpper.IndexOf(" ON ") + 4)
                            'there could be few ONs
                            Dim fdar() As String = Split(sqlj, " AND ")
                            For i = 0 To fdar.Length - 1
                                sqlon = fdar(i)
                                fld1 = sqlon.Substring(0, sqlon.IndexOf("="))
                                fld2 = sqlon.Substring(sqlon.IndexOf("="))
                                fld1 = FixFieldName(ddt, fld1, tbl1, myconstr, myconprv, er)
                                fld2 = FixFieldName(ddt, fld2, tbl2, myconstr, myconprv, er)

                                'TODO fix ON statement
                                msql = ReplaceWholeWord(msql, tbl1, """" & tbl1.Trim & """")
                                msql = msql.Replace("""""", """")
                                msql = ReplaceWholeWord(msql, tbl2, """" & tbl2.Trim & """")
                                msql = msql.Replace("""""", """")
                                msql = ReplaceWholeWord(msql, fld1, """" & fld1.Trim & """")
                                msql = msql.Replace("""""", """")
                                msql = ReplaceWholeWord(msql, fld2, """" & fld2.Trim & """")
                                msql = msql.Replace("""""", """")

                            Next
                            tbl1 = tbl2
                        End If
                    Next
                End If
            Next
            er = "Single Tables: " & tbls
            Return msql
        Catch ex As Exception
            er = "ERROR !! " & ex.Message
            Return Nothing
        End Try
        Return msql
    End Function
    Function ReplaceWholeWord(Inp As String, Fnd As String, Repl As String) As String
        Dim LettrsNum As String = "ABCDEFGHIJKLMNOPQRSTUVWYXZabcdefghijklmnopqrstuvwyxz0123456789_@"
        Dim FndIndx As Integer = 0
        Dim len As Integer = Fnd.Length
        'Dim ret As String = Inp
        Do Until False
            FndIndx = Inp.IndexOf(Fnd, FndIndx)
            If FndIndx < 0 Then Exit Do
            If FndIndx = 0 AndAlso LettrsNum.Contains(Inp.Substring(FndIndx + len, 1)) = False Then  'Fnd Is in the beginning of Inp
                Inp = Inp.Substring(0, FndIndx) & Repl & Inp.Substring(FndIndx + len)

            ElseIf FndIndx > 0 AndAlso (FndIndx + len = Inp.Length OrElse LettrsNum.Contains(Inp.Substring(FndIndx + len, 1)) = False) Then
                Inp = Inp.Substring(0, FndIndx) & Repl & Inp.Substring(FndIndx + len)
            End If
            FndIndx = FndIndx + Repl.Length
        Loop
        Return Inp
    End Function
    Public Function CanTargetDataTableBalancedFromStartingOne(ByRef aa As DataTable, ByRef bb As DataTable, ByRef aaSums As DataTable, ByRef bbSums As DataTable, ByVal dims As String, l As Integer, ByVal q As Double, ByRef er As String, ByRef xbal As DataTable, ByRef dtk As DataTable, ByRef nsteps As Integer, ByRef llsums() As Integer, Optional ByVal v() As Integer = Nothing) As String
        'a - starting array, c - sums for columns, d -sums for rows, x - balanced array, dtk - coefficients, l -number of steps, q - precision
        Dim ret As String = String.Empty
        Try
            ''fix null values for starting matrix and target matrix - already done

            ret = BalanceMultidimensionalMatrix(aa, aaSums, bbSums, dims, l, q, er, xbal, dtk, nsteps, llsums, v)

        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Public Function DifferenceBetweenSteps(ByRef dat As DataTable, ByVal fld1 As String, ByVal fld2 As String, ByRef er As String) As Decimal
        Dim mx As Double = 0
        Dim i As Integer = 0
        Dim j As Integer = 0
        'at this point dat1.Rows.Count=dat2.Rows.Count
        Dim n As Integer = dat.Rows.Count

        For i = 0 To n - 1 'loop through column  flds(j) & "Sum"  in dat1Sums
            Try
                If IsNumeric(dat(i)(fld1)) AndAlso IsNumeric(dat(i)(fld2)) Then
                    mx = Max(mx, Abs(dat(i)(fld1) - dat(i)(fld2)))
                End If
            Catch ex As Exception
                er = er & " ERROR!! " & ex.Message
            End Try
        Next
        Return mx
    End Function
    Public Function DifferenceBetweenSums(ByRef dat1Sums As DataTable, ByRef dat2Sums As DataTable, ByVal dims As String, ByRef er As String) As Decimal
        Dim mx As Double = 0
        Dim flds() As String = Split(dims, ",")
        Dim m As Integer = flds.Length
        Dim i As Integer = 0
        Dim j As Integer = 0
        'at this point dat1Sums.Rows.Count=dat2Sums.Rows.Count
        Dim n As Integer = dat1Sums.Rows.Count
        Try
            For j = 0 To m - 1  'loop through flds
                For i = 0 To n - 1 'loop through column  flds(j) & "Sum"  in dat1Sums
                    Try
                        If IsNumeric(dat1Sums(i)(flds(j) & "Sum")) AndAlso IsNumeric(dat2Sums(i)(flds(j) & "Sum")) Then
                            mx = Max(mx, Abs(dat1Sums(i)(flds(j) & "Sum") - dat2Sums(i)(flds(j) & "Sum")))
                        End If
                    Catch ex As Exception
                        er = er & " ERROR!! " & ex.Message
                        mx = 0
                        Return mx
                    End Try
                Next
            Next
        Catch ex As Exception
            er = ex.Message
            Return 0
        End Try
        Return mx
    End Function


    Public Function BalanceMultidimensionalMatrix(ByVal aa As DataTable, ByRef aaSums As DataTable, ByRef bbSums As DataTable, ByVal dims As String, l As Integer, ByVal q As Double, ByRef er As String, ByRef xx As DataTable, ByRef dtk As DataTable, ByRef nsteps As Integer, ByRef llsums() As Integer, Optional ByVal v() As Integer = Nothing) As String
        'at this point aa.Rows.Count=bb.Rows.Count
        Dim n As Integer = aa.Rows.Count
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim k As Integer = 1
        Dim r As Integer = 0
        Dim d As Integer = 0
        Dim t As Integer = 0
        Dim stp As Integer = 0
        Dim cf As Double = 1
        Dim col As DataColumn
        Dim yySums As New DataTable
        Dim valfld As String = String.Empty
        Dim flt As String = String.Empty
        Dim flds() As String = Split(dims, ",")
        Dim m As Integer = flds.Length
        Dim bw As Double = 0
        Dim ret As String = String.Empty
        Dim val As String = String.Empty
        Dim fldval As String = String.Empty
        Dim coef As Double = 0
        Dim pres As Double = 0
        dtk = aa
        Dim precs(m - 1) As Double
        Try
            'steps
            While k <= l * m
                bw = 0
                stp = stp + 1
                For j = 0 To m - 1  'horizontal loop through flds
                    'new columns into aa = dstart
                    col = New DataColumn
                    col.DataType = System.Type.GetType("System.String")
                    col.ColumnName = "By" & flds(j) & "_" & k.ToString
                    dtk.Columns.Add(col)
                    col = New DataColumn
                    col.DataType = System.Type.GetType("System.Double")
                    col.ColumnName = "Step" & k.ToString & "coeff"
                    dtk.Columns.Add(col)
                    col = New DataColumn
                    col.DataType = System.Type.GetType("System.Double")
                    col.ColumnName = "Step" & k.ToString & "precision"
                    dtk.Columns.Add(col)
                    col = New DataColumn
                    col.DataType = System.Type.GetType("System.Double")
                    col.ColumnName = "Step" & k.ToString & "value"
                    dtk.Columns.Add(col)
                    'calc current sums
                    If k = 1 Then
                        valfld = "Value"
                        yySums = aaSums

                    ElseIf k > 1 Then
                        valfld = "Step" & (k - 1).ToString & "value"
                        'calculated in previous step:
                        'ret = MakeMultidimensionalSumsFromDataTable(dtk, dims, "Step" & k.ToString & "value", yySums, llsums, True)
                    End If
                    'balancing step
                    bw = 0
                    For i = 0 To llsums(j) - 1  'vertical loop through bbSums field flds(j) values
                        If bbSums(i)(flds(j)).ToString.Trim <> "" Then
                            fldval = bbSums(i)(flds(j))
                            If yySums(i)(flds(j) & "Sum") = 0 Then yySums(i)(flds(j) & "Sum") = 1
                            If bbSums(i)(flds(j) & "Sum") = 0 Then bbSums(i)(flds(j) & "Sum") = 0.001
                            'coef = Round(bbSums(i)(flds(j) & "Sum") / yySums(i)(flds(j) & "Sum"), 4)
                            coef = bbSums(i)(flds(j) & "Sum") / yySums(i)(flds(j) & "Sum")
                            pres = Round(Abs(bbSums(i)(flds(j) & "Sum") - yySums(i)(flds(j) & "Sum")), 2)
                            bw = Max(bw, pres)
                            For r = 0 To dtk.Rows.Count - 1
                                If dtk(r)(flds(j)) = fldval Then
                                    dtk(r)("By" & flds(j) & "_" & k.ToString) = fldval
                                    dtk(r)("Step" & k.ToString & "coeff") = Round(coef, 2)
                                    dtk(r)("Step" & k.ToString & "precision") = pres
                                    dtk(r)("Step" & k.ToString & "value") = Round(dtk(r)(valfld) * coef, 3)
                                End If
                            Next
                        End If
                    Next

                    'checking summary for all fields, bw is difference of column sums in target bbSums and yySums
                    If bw < q Then
                        ret = "Balanced in " & stp.ToString & " steps. Precision = " & bw.ToString & " reached on the balancing by the sums of the " & flds(j) & " field."
                        xx = yySums
                        'TODO add fields to dtk for final coeffs and final precision
                        col = New DataColumn
                        col.DataType = System.Type.GetType("System.Double")
                        col.ColumnName = "BalancedCoeff"
                        dtk.Columns.Add(col)
                        col = New DataColumn
                        col.DataType = System.Type.GetType("System.Double")
                        col.ColumnName = "BalancedPrecision"
                        dtk.Columns.Add(col)
                        col = New DataColumn
                        col.DataType = System.Type.GetType("System.Double")
                        col.ColumnName = "BalancedValue"
                        dtk.Columns.Add(col)
                        'max precisions
                        nsteps = k - 1
                        ret = ret & " Maximum precisions for selected fields: "
                        For t = 0 To m - 1
                            precs(t) = 0
                            For i = 0 To dtk.Rows.Count - 1
                                If t = 0 Then
                                    cf = 1
                                    For d = 1 To k
                                        cf = cf * dtk(i)("Step" & d.ToString & "coeff")
                                    Next
                                    dtk(i)("BalancedCoeff") = Round(cf, 2)
                                    dtk(i)("BalancedPrecision") = Round(dtk(i)("Step" & k.ToString & "precision"), 2)
                                    dtk(i)("BalancedValue") = Round(dtk(i)("Step" & k.ToString & "value"), 2)
                                    dtk(i)("Value") = Round(dtk(i)("Value"), 2)
                                End If
                                If nsteps <= t * m Then
                                    Exit For
                                End If
                                Try
                                    precs(t) = Max(precs(t), dtk(i)("Step" & (nsteps - t * m).ToString & "precision"))
                                Catch ex As Exception
                                    ret = ret & " ERROR!! " & ex.Message
                                End Try
                            Next
                            If t = m - 1 Then
                                ret = ret & precs(m - 1).ToString & " "
                            Else
                                ret = ret & precs(t).ToString & ", "
                            End If
                        Next

                        Return ret
                    End If

                    ret = MakeMultidimensionalSumsFromDataTable(dtk, dims, "Step" & k.ToString & "value", yySums, llsums, True)

                    k = k + 1

                Next

            End While
            col = New DataColumn
            col.DataType = System.Type.GetType("System.Double")
            col.ColumnName = "BalancedCoeff"
            dtk.Columns.Add(col)
            col = New DataColumn
            col.DataType = System.Type.GetType("System.Double")
            col.ColumnName = "BalancedPrecision"
            dtk.Columns.Add(col)
            col = New DataColumn
            col.DataType = System.Type.GetType("System.Double")
            col.ColumnName = "BalancedValue"
            dtk.Columns.Add(col)

            'max precisions
            ret = "Not balanced in " & l.ToString & " steps for each balancing selected field for requested precision = " & q.ToString & ". " & ret & " Maximum precisions for selected fields: "
            nsteps = k - 1
            For j = 0 To m - 1
                precs(j) = 0
                For i = 0 To dtk.Rows.Count - 1
                    If j = 0 Then
                        cf = 1
                        For d = 1 To k - 1
                            cf = cf * dtk(i)("Step" & d.ToString & "coeff")
                        Next
                        dtk(i)("BalancedCoeff") = Round(cf, 2)
                        dtk(i)("BalancedPrecision") = Round(dtk(i)("Step" & (k - 1).ToString & "precision"), 2)
                        dtk(i)("BalancedValue") = Round(dtk(i)("Step" & (k - 1).ToString & "value"), 2)
                        dtk(i)("Value") = Round(dtk(i)("Value"), 2)

                    End If
                    If nsteps <= j * m Then
                        Exit For
                    End If
                    Try
                        precs(j) = Max(precs(j), dtk(i)("Step" & (nsteps - j * m).ToString & "precision"))
                    Catch ex As Exception
                        ret = ret & " ERROR!! " & ex.Message
                    End Try
                Next
                If j = m - 1 Then
                    ret = ret & precs(m - 1).ToString & " "
                Else
                    ret = ret & precs(j).ToString & ", "
                End If
            Next

            xx = yySums

        Catch ex As Exception
            ret = ret & " ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Public Function CanTargetMatrixBalancedFromStartingOne(ByVal a As Double(,), ByVal b As Double(,), ByVal l As Integer, ByVal q As Double, ByRef er As String, ByRef x As Double(,), ByRef dtk As DataTable, Optional ByVal u As Integer = 0, Optional ByVal v As Integer = 0, Optional ByVal addjustbystart As Boolean = False) As String
        'a - starting array, c - sums for columns, d -sums for rows, x - balanced array, dtk - coefficients, l -number of steps, q - precision
        Dim ret As String = String.Empty
        If a.GetLength(0) <> b.GetLength(0) OrElse a.GetLength(1) <> b.GetLength(1) Then
            ret = "no, not balanced, Matrixs should be the same dimensions"
            Return ret
        End If
        Dim i As Integer = 0
        Dim j As Integer = 0
        'Dim k As Integer = 0
        Dim n As Integer = a.GetLength(0)
        Dim m As Integer = a.GetLength(1)
        Dim c(n - 1) As Double
        Dim d(m - 1) As Double

        Try
            'fix null values for starting matrix
            For i = 0 To n - 1
                'kc(i) = 1
                For j = 0 To m - 1
                    'kd(j) = 1
                    If a(i, j) = 0 Then
                        x(i, j) = 0.001
                    Else
                        x(i, j) = a(i, j)
                    End If
                    If b(i, j) = 0 Then
                        b(i, j) = 0.00001
                    End If
                Next
            Next
            'make arrays of sums
            If u = 0 OrElse v = 0 Then
                'sums of columns
                For i = 0 To n - 1
                    c(i) = 0
                    'cx(i) = 0
                    For j = 0 To m - 1
                        c(i) = c(i) + b(i, j)  'target sums of columns
                        'cx(i) = cx(i) + x(i, j)
                    Next
                    'bw = bw + Abs(c(i) - cx(i))
                Next
                'sums of rows
                For j = 0 To m - 1
                    d(j) = 0
                    'dx(j) = 0
                    For i = 0 To n - 1
                        d(j) = d(j) + b(i, j)   'target sums of rows
                        'dx(j) = dx(j) + x(i, j)
                    Next
                    'bw = bw + Abs(d(j) - dx(j))
                Next

            Else  'u,v are not 0,0 - partial balancig
                'sums of rows
                For i = 0 To u - 1
                    c(i) = 0
                    For j = 0 To v - 1
                        c(i) = c(i) + b(i, j)  'target sums of top left rows
                    Next
                Next
                For i = u To n - 1
                    c(i) = 0
                    For j = v To m - 1
                        c(i) = c(i) + b(i, j)  'target sums of low right rows
                    Next
                Next
                'sums of columns
                For j = 0 To v - 1
                    d(j) = 0
                    For i = 0 To u - 1
                        d(j) = d(j) + b(i, j)   'target sums of top left columns
                    Next
                Next
                For j = v To m - 1
                    d(j) = 0
                    For i = u To n - 1
                        d(j) = d(j) + b(i, j)   'target sums of low right columns
                    Next
                Next
            End If


            ret = BalanceMatrix(a, c, d, l, q, er, x, dtk, u, v)


        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Public Function BalanceMatrix(ByRef a As Double(,), ByVal c As Double(), ByVal d As Double(), ByVal l As Integer, ByVal q As Double, ByRef er As String, ByRef x As Double(,), ByRef dtk As DataTable, Optional ByVal u As Integer = 0, Optional ByVal v As Integer = 0) As String
        'a - starting array, c - sums for columns, d -sums for rows, x - balanced array, dtk - coefficients, l -number of steps, q - precision
        Dim ret As String = String.Empty
        If a.GetLength(0) <> c.Length OrElse a.GetLength(1) <> d.Length Then
            ret = "no, not balanced, Matrixs should be the same dimensions as arrays of sums"
            Return ret
        End If
        Dim i As Integer = 0
        Dim j As Integer = 1
        Dim k As Integer = 0
        Dim r As Integer = 0
        Dim n As Integer = a.GetLength(0)
        Dim m As Integer = a.GetLength(1)
        Dim cx(n - 1) As Double
        Dim dx(m - 1) As Double
        Dim kc(n - 1) As Double
        Dim kd(m - 1) As Double
        Dim bw As Double = 0
        'create DataTable dtk
        Dim col As DataColumn
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "Steps"
        dtk.Columns.Add(col)

        For i = 1 To n
            col = New DataColumn
            col.DataType = System.Type.GetType("System.String")
            col.ColumnName = "ki" & i.ToString
            dtk.Columns.Add(col)
        Next
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "-"
        dtk.Columns.Add(col)
        For j = 1 To m
            col = New DataColumn
            col.DataType = System.Type.GetType("System.String")
            col.ColumnName = "kj" & j.ToString
            dtk.Columns.Add(col)
        Next
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "Precision"
        dtk.Columns.Add(col)

        Try
            'fix null values for starting matrix
            For i = 0 To n - 1
                kc(i) = 1
                For j = 0 To m - 1
                    kd(j) = 1
                    If a(i, j) = 0 Then
                        x(i, j) = 0.001
                    Else
                        x(i, j) = a(i, j)
                    End If

                Next
            Next

            'sums of rows
            For i = 0 To n - 1
                cx(i) = 0
                For j = 0 To m - 1
                    cx(i) = cx(i) + x(i, j)
                Next
                bw = bw + Abs(c(i) - cx(i))
            Next
            'sums of columns
            For j = 0 To m - 1
                dx(j) = 0
                For i = 0 To n - 1
                    dx(j) = dx(j) + x(i, j)
                Next
                bw = bw + Abs(d(j) - dx(j))
            Next
            If bw < q Then
                er = Round(bw, 3)
                If er = 0 Then
                    er = q
                End If
                ret = " Already balanced, precision=" & er.ToString & ", 0 steps."
                Return ret
            End If

            'Dim bal As Boolean = False
            Dim bb As Double = 0

            'balancing steps
            For k = 0 To l - 1

                If u = 0 OrElse v = 0 Then
                    Dim Row As DataRow = dtk.NewRow()
                    Row("Steps") = (k + 1).ToString
                    Row("-") = "  "

                    'Do balancing step for rows for whole x matrix
                    For i = 0 To n - 1
                        For j = 0 To m - 1
                            x(i, j) = x(i, j) * c(i) / cx(i)
                        Next
                    Next
                    'at this point x balanced to sums of rows

                    'save coefficiens in dtk row
                    bb = 0
                    For i = 0 To n - 1
                        Row("ki" & (i + 1).ToString) = Round(c(i) / cx(i), 5).ToString
                        kc(i) = kc(i) * c(i) / cx(i)
                    Next

                    'new sums of columns and difference
                    bw = 0
                    For j = 0 To m - 1
                        dx(j) = 0
                        For i = 0 To n - 1
                            dx(j) = dx(j) + x(i, j)
                        Next
                        bw = bw + Abs(d(j) - dx(j))
                    Next

                    'Do balancing step for columns for whole x matrix
                    For i = 0 To n - 1
                        For j = 0 To m - 1
                            x(i, j) = x(i, j) * d(j) / dx(j)
                        Next
                    Next
                    'at this point x balanced to sums of columns

                    'save coefficiens in dtk row
                    For j = 0 To m - 1
                        Row("kj" & (j + 1).ToString) = Round(d(j) / dx(j), 5).ToString
                        kd(j) = kd(j) * d(j) / dx(j)
                    Next

                    Row("Precision") = Round(bw, 5).ToString
                    dtk.Rows.Add(Row)

                    'new sums of rows and difference
                    bw = 0
                    For i = 0 To n - 1
                        cx(i) = 0
                        For j = 0 To m - 1
                            cx(i) = cx(i) + x(i, j)
                        Next
                        bw = bw + Abs(c(i) - cx(i))
                    Next

                    'check if matrix x is balanced
                    If bw < q Then
                        k = k + 1
                        er = Round(bw, 5)
                        If er = 0 Then
                            er = q
                        End If
                        ret = " Balanced, precision: " & er.ToString & ", steps: " & k.ToString
                        Exit For
                    End If

                    'compare Row with previous row in dtk
                    If dtk.Rows.Count > l - 1 Then
                        'calc difference to previous row in dtk
                        For i = 0 To n - 1
                            bb = bb + Abs(Row("ki" & (i + 1).ToString) - dtk.Rows(dtk.Rows.Count - 1)("ki" & (i + 1).ToString))
                        Next
                        For j = 0 To m - 1
                            bb = bb + Abs(Row("kj" & (j + 1).ToString) - dtk.Rows(dtk.Rows.Count - 1)("kj" & (j + 1).ToString))
                        Next

                        'no difference from previous dtk row
                        If bb < 0.0000001 Then
                            er = Round(bw, 5)
                            If er = 0 Then
                                er = q
                            End If
                            ret = " Balanced, but precision: " & er.ToString & ", steps: " & k.ToString
                            bb = 1
                            Exit For
                        End If
                    End If

                    'not balanced yet

                Else  'u,v are not 0,0 - partial balancig

                    'make top left corner matrix
                    Dim atl(u - 1, v - 1) As Double
                    Dim ctl(u - 1) As Double
                    Dim dtl(v - 1) As Double
                    Dim xtl(u - 1, v - 1) As Double
                    Dim dtktl As New DataTable
                    Dim sc As Double = 0
                    Dim sd As Double = 0
                    For i = 0 To u - 1
                        kc(i) = 1
                        ctl(i) = c(i)
                        sc = sc + ctl(i)
                        For j = 0 To v - 1
                            atl(i, j) = a(i, j)
                        Next
                    Next
                    For j = 0 To v - 1
                        kd(j) = 1
                        dtl(j) = d(j)
                        sd = sd + dtl(j)
                    Next
                    'adjust sums of ctl and dtl
                    For j = 0 To v - 1
                        dtl(j) = dtl(j) * sc / sd
                    Next
                    'balance top left corner matrix
                    ret = BalanceMatrix(atl, ctl, dtl, l, q, er, xtl, dtktl, 0, 0)

                    'make low right corner matrix
                    Dim alr(n - 1 - u, m - 1 - v) As Double
                    Dim clr(n - 1 - u) As Double
                    Dim dlr(m - 1 - v) As Double
                    Dim xlr(n - 1 - u, m - 1 - v) As Double
                    Dim dtklr As New DataTable
                    sc = 0
                    sd = 0
                    For i = 0 To n - u - 1
                        kc(i) = 1
                        clr(i) = c(i + u)
                        sc = sc + clr(i)
                        For j = 0 To m - v - 1
                            alr(i, j) = a(i + u, j + v)
                        Next
                    Next
                    For j = 0 To m - v - 1
                        kd(j) = 1
                        dlr(j) = d(j + v)
                        sd = sd + dlr(j)
                    Next
                    'adjust sums of ctl and dtl
                    For j = 0 To m - v - 1
                        dlr(j) = dlr(j) * sc / sd
                    Next
                    'balance low right corner matrix
                    ret = "Top Left Corner: " & ret & " , Low Right Corner: " & BalanceMatrix(alr, clr, dlr, l, q, er, xlr, dtklr, 0, 0)

                    'combine matrixs applying proper coefficients to top right corner and low left corner of matrix

                    'top left corner
                    For i = 0 To u - 1
                        For j = 0 To v - 1
                            x(i, j) = xtl(i, j)
                        Next
                    Next
                    'low right corner
                    For i = 0 To n - u - 1
                        For j = 0 To m - v - 1
                            x(i + u, j + v) = xlr(i, j)
                        Next
                    Next

                    'coefficients
                    'top left corner
                    For r = 0 To dtktl.Rows.Count - 1
                        Dim Rowr As DataRow = dtk.NewRow()
                        Rowr("Steps") = dtktl.Rows(r)("Steps").ToString
                        Rowr("-") = dtktl.Rows(r)("-").ToString
                        For i = 0 To u - 1
                            Rowr("ki" & (i + 1).ToString) = dtktl.Rows(r)("ki" & (i + 1).ToString).ToString
                        Next
                        For j = 0 To v - 1
                            Rowr("kj" & (j + 1).ToString) = dtktl.Rows(r)("kj" & (j + 1).ToString).ToString
                        Next
                        For i = u To n - 1
                            Rowr("ki" & (i + 1).ToString) = " "
                        Next
                        For j = v To m - 1
                            Rowr("kj" & (j + 1).ToString) = " "
                        Next
                        Rowr("Precision") = dtktl.Rows(r)("Precision")
                        dtk.Rows.Add(Rowr)
                    Next
                    'low right corner
                    For r = 0 To dtklr.Rows.Count - 1
                        Dim Rowr As DataRow = dtk.NewRow()
                        Rowr("Steps") = dtklr.Rows(r)("Steps").ToString
                        Rowr("-") = dtklr.Rows(r)("-").ToString
                        For i = 0 To u - 1
                            Rowr("ki" & (i + 1).ToString) = " "
                        Next
                        For j = 0 To v - 1
                            Rowr("kj" & (j + 1).ToString) = " "
                        Next
                        For i = u To n - 1
                            Rowr("ki" & (i + 1).ToString) = dtklr.Rows(r)("ki" & (i - u + 1).ToString).ToString
                        Next
                        For j = v To m - 1
                            Rowr("kj" & (j + 1).ToString) = dtklr.Rows(r)("kj" & (j - v + 1).ToString).ToString
                        Next
                        Rowr("Precision") = dtklr.Rows(r)("Precision")
                        dtk.Rows.Add(Rowr)
                    Next
                    'final dtk row
                    Dim Rowt As DataRow = dtk.NewRow()
                    Rowt("Steps") = "Final:"
                    Rowt("-") = "  "
                    For i = 0 To u - 1
                        Rowt("ki" & (i + 1).ToString) = dtktl.Rows(dtktl.Rows.Count - 1)("ki" & (i + 1).ToString).ToString
                    Next
                    For j = 0 To v - 1
                        Rowt("kj" & (j + 1).ToString) = dtktl.Rows(dtktl.Rows.Count - 1)("kj" & (j + 1).ToString).ToString
                    Next
                    For i = u To n - 1
                        Rowt("ki" & (i + 1).ToString) = dtklr.Rows(dtklr.Rows.Count - 1)("ki" & (i - u + 1).ToString).ToString
                    Next
                    For j = v To m - 1
                        Rowt("kj" & (j + 1).ToString) = dtklr.Rows(dtklr.Rows.Count - 1)("kj" & (j - v + 1).ToString).ToString
                    Next
                    Rowt("Precision") = dtktl.Rows(dtktl.Rows.Count - 1)("Precision") & "/" & dtklr.Rows(dtklr.Rows.Count - 1)("Precision")
                    dtk.Rows.Add(Rowt)

                    'top right corner
                    For i = 0 To u - 1
                        For j = 0 To m - v - 1
                            x(i, j + v) = a(i, j + v) * Rowt("ki" & (i + 1).ToString) * Rowt("kj" & (j + v + 1).ToString)
                        Next
                    Next
                    'low left corner
                    For i = 0 To n - u - 1
                        For j = 0 To v - 1
                            x(i + u, j) = a(i + u, j) * Rowt("ki" & (i + u + 1).ToString) * Rowt("kj" & (j + 1).ToString)
                        Next
                    Next
                    Return ret
                End If
            Next
            If u = 0 OrElse v = 0 Then
                Dim Rowt As DataRow = dtk.NewRow()
                Rowt("Steps") = "Result:"
                Rowt("-") = "  "
                'For i = 0 To n - 1
                '    Rowt("ki" & (i + 1).ToString) = Round(kc(i), 5).ToString
                'Next
                'For j = 0 To m - 1
                '    Rowt("kj" & (j + 1).ToString) = Round(kd(j), 5).ToString
                'Next
                Dim kcc As Double = 1
                Dim kdd As Double = 1
                Dim kk As Integer
                For i = 0 To n - 1
                    kcc = 1
                    For kk = 0 To dtk.Rows.Count - 1
                        kcc = kcc * dtk.Rows(kk)("ki" & (i + 1).ToString)
                    Next
                    Rowt("ki" & (i + 1).ToString) = Round(kcc, 5).ToString
                Next
                For j = 0 To m - 1
                    kdd = 1
                    For kk = 0 To dtk.Rows.Count - 1
                        kdd = kdd * dtk.Rows(kk)("kj" & (j + 1).ToString)
                    Next
                    Rowt("kj" & (j + 1).ToString) = Round(kdd, 5).ToString
                Next
                Rowt("Precision") = Round(bw, 5).ToString 'dtk.Rows(dtk.Rows.Count - 1)("Precision")
                dtk.Rows.Add(Rowt)
                If bb = 1 Then
                    Return ret
                ElseIf bw > q AndAlso bb <> 1 Then
                    er = Round(bw, 5)
                    If er = 0 Then
                        er = q
                    End If
                    ret = " Not balanced, precision=" & er.ToString & " for " & l.ToString & " steps "
                    Return ret
                End If
            End If

        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret

    End Function
    Public Function CanBalanceMatrixToColRowSums(ByRef a As Double(,), ByVal c As Double(), ByVal d As Double(), ByVal l As Integer, ByVal q As Double, ByRef er As String, ByRef x As Double(,), ByRef dtk As DataTable, Optional ByVal u As Integer = 0, Optional ByVal v As Integer = 0) As String
        'a - starting array, c - sums for rows, d -sums for columns, x - balanced array, dtk - coefficients, l -number of steps, q - precision
        Dim ret As String = String.Empty
        If a.GetLength(0) <> c.Length OrElse a.GetLength(1) <> d.Length Then
            ret = "no, not balanced, Matrixs should be the same dimensions as arrays of sums"
            Return ret
        End If
        Dim i As Integer = 0
        Dim j As Integer = 1
        Dim k As Integer = 0
        Dim r As Integer = 0
        Dim n As Integer = a.GetLength(0)
        Dim m As Integer = a.GetLength(1)
        Dim cx(n - 1) As Double
        Dim dx(m - 1) As Double
        Dim kc(n - 1) As Double
        Dim kd(m - 1) As Double
        Dim bw As Double = 0

        Try
            'fix null values for starting matrix
            For i = 0 To n - 1
                'kc(i) = 1
                For j = 0 To m - 1
                    'kd(j) = 1
                    If a(i, j) = 0 Then
                        x(i, j) = 0.001
                    Else
                        x(i, j) = a(i, j)
                    End If

                Next
            Next

            ''sums of rows
            'For i = 0 To n - 1
            '    cx(i) = 0
            '    For j = 0 To m - 1
            '        cx(i) = cx(i) + x(i, j)
            '    Next
            '    bw = bw + Abs(c(i) - cx(i))
            'Next
            ''sums of columns
            'For j = 0 To m - 1
            '    dx(j) = 0
            '    For i = 0 To n - 1
            '        dx(j) = dx(j) + x(i, j)
            '    Next
            '    bw = bw + Abs(d(j) - dx(j))
            'Next
            'If bw < q Then
            '    er = Round(bw, 3)
            '    If er = 0 Then
            '        er = q
            '    End If
            '    ret = " Balanced, precision=" & er.ToString & ", 0 steps."
            '    Return ret
            'End If

            ret = BalanceMatrix(a, c, d, l, q, er, x, dtk, u, v)

        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret

    End Function
    Public Function MakeRowsColumnsArraysFromMultiColumns(ByVal dt1 As DataTable, ByVal x1 As String, ByVal x2 As String, ByVal y1 As String, ByRef x1vals() As String, ByRef x2vals() As String) As String
        Dim k As Integer = 0
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim l As Integer = 0
        Dim lf As Integer = -1

        Dim ret1 As String = String.Empty
        Dim ret2 As String = String.Empty
        Dim er As String = String.Empty
        Try
            'dv3 and dvt are already filtered
            'TODO check for search and etc...

            Dim dvt As DataTable = CleanDBNullsFields(dt1, y1, x1, x2, er, True)

            Dim fldnames(0) As String
            fldnames(0) = x1
            Dim dvr As DataTable = dvt.DefaultView.ToTable(True, fldnames)
            For k = 0 To dvr.Rows.Count - 1
                ReDim Preserve x1vals(k)
                x1vals(k) = dvr.Rows(k)(x1)
                If k > 0 Then ret1 = ret1 & ","
                ret1 = ret1 & x1vals(k)
            Next

            'x2vals(k) is already defined
            'fldnames(0) = x2
            'dvr = dvt.DefaultView.ToTable(True, fldnames)
            For k = 0 To x2vals.Length - 1
                'ReDim Preserve x2vals(k)
                'x2vals(k) = dvr.Rows(k)(x2)
                If k > 0 Then ret2 = ret2 & ","
                ret2 = ret2 & x2vals(k)
            Next
            Return ret1 & " | " & ret2

        Catch ex As Exception
            Dim ret As String = ex.Message
            Return Nothing
        End Try
    End Function
    Public Function MakeRowsColumnsArraysFromDataView(ByVal dt1 As DataTable, ByVal x1 As String, ByVal x2 As String, ByVal y1 As String, ByRef x1vals() As String, ByRef x2vals() As String) As String
        Dim k As Integer = 0
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim l As Integer = 0
        Dim lf As Integer = -1
        'Dim dt1 As DataTable = dv.Table
        'If x1vals Is Nothing OrElse x2vals Is Nothing OrElse x1vals.Length = 0 OrElse x2vals.Length = 0 OrElse x1vals(0) Is Nothing OrElse x2vals(0) Is Nothing Then
        '    ReDim x1vals(0)
        '    ReDim x2vals(0)
        '    x1vals(0) = dt1.Rows(0)(x1)
        '    x2vals(0) = dt1.Rows(0)(x2)
        'End If
        'Dim ret1 As String = x1vals(0).ToString
        'Dim ret2 As String = x2vals(0).ToString
        Dim ret1 As String = String.Empty
        Dim ret2 As String = String.Empty
        Dim er As String = String.Empty
        Try
            'dv3 and dvt are already filtered
            'TODO check for search and etc...

            Dim dvt As DataTable = CleanDBNullsFields(dt1, y1, x1, x2, er, True)

            Dim fldnames(0) As String
            fldnames(0) = x1
            Dim dvr As DataTable = dvt.DefaultView.ToTable(True, fldnames)
            For k = 0 To dvr.Rows.Count - 1
                ReDim Preserve x1vals(k)
                x1vals(k) = dvr.Rows(k)(x1)
                If k > 0 Then ret1 = ret1 & ","
                ret1 = ret1 & x1vals(k)
            Next

            fldnames(0) = x2
            dvr = dvt.DefaultView.ToTable(True, fldnames)
            For k = 0 To dvr.Rows.Count - 1
                ReDim Preserve x2vals(k)
                x2vals(k) = dvr.Rows(k)(x2)
                If k > 0 Then ret2 = ret2 & ","
                ret2 = ret2 & x2vals(k)
            Next
            Array.Sort(x1vals)
            Array.Sort(x2vals)
            Return ret1 & " | " & ret2
        Catch ex As Exception
            Dim ret As String = ex.Message
            Return Nothing
        End Try
    End Function
    Public Function MakeStartAndTargetEven(ByRef dstart As DataTable, ByRef dtarget As DataTable, ByVal dims As String) As String
        Dim ret As String = String.Empty
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim flt As String = String.Empty
        Dim flds() As String = Split(dims, ",")
        Dim srt As String = String.Empty
        For j = 0 To flds.Count - 1
            If j = 0 Then
                srt = flds(j) & " ASC"
            Else
                srt = srt & "," & flds(j) & " ASC"
            End If
        Next
        Try
            'TODO add missing rows from dv3?

            Dim dt1 As DataTable = dstart.DefaultView.ToTable(True, flds)

            Dim dt2 As DataTable = dtarget.DefaultView.ToTable(True, flds)

            Dim col As DataColumn
            col = New DataColumn
            col.DataType = System.Type.GetType("System.Double")
            col.ColumnName = "TargetValue"
            dstart.Columns.Add(col)

            'compare dt1 and dt2 and add missing rows into dstart and dtarget with Value = 0
            For i = 0 To dt2.Rows.Count - 1
                'make filter to check if dt3.Rows(i) is in dstart oor in dtartget
                flt = ""
                For j = 0 To flds.Count - 1
                    flt = flt & flds(j) & "='" & dt2.Rows(i)(flds(j)) & "'"
                    If j < flds.Count - 1 Then
                        flt = flt & " AND "
                    End If
                Next
                dt1.DefaultView.RowFilter = ""
                dt1.DefaultView.RowFilter = flt
                'add missing Row in dstart
                If dt1.DefaultView.ToTable.Rows.Count = 0 Then
                    'add row into dstart
                    Dim Row As DataRow = dstart.NewRow()
                    For j = 0 To flds.Count - 1
                        Row(flds(j)) = dt2.Rows(i)(flds(j))
                    Next
                    Row("Value") = 0
                    dstart.Rows.Add(Row)
                End If

            Next
            For i = 0 To dt1.Rows.Count - 1
                'make filter to check if dt3.Rows(i) is in dstart or in dtarget
                flt = ""
                For j = 0 To flds.Count - 1
                    flt = flt & flds(j) & "='" & dt1.Rows(i)(flds(j)) & "'"
                    If j < flds.Count - 1 Then
                        flt = flt & " AND "
                    End If
                Next
                dt2.DefaultView.RowFilter = ""
                dt2.DefaultView.RowFilter = flt
                'add missing Row in dtarget
                If dt2.DefaultView.ToTable.Rows.Count = 0 Then
                    'add row ito dstart
                    Dim Row As DataRow = dtarget.NewRow()
                    For j = 0 To flds.Count - 1
                        Row(flds(j)) = dt1.Rows(i)(flds(j))
                    Next
                    Row("Value") = 0
                    dtarget.Rows.Add(Row)
                End If
            Next
            dstart.DefaultView.Sort = srt
            dstart = dstart.DefaultView.ToTable
            dtarget.DefaultView.Sort = srt
            dtarget = dtarget.DefaultView.ToTable
            For i = 0 To dstart.Rows.Count - 1
                dstart.Rows(i)("TargetValue") = dtarget.Rows(i)("Value")
            Next
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function

    Public Function MakeMultidimensionalSumsFromDataTable(ByRef dt As DataTable, ByVal dims As String, ByVal fld As String, ByRef dtSums As DataTable, ByRef lensums() As Integer, Optional fixnulls As Boolean = False) As String
        Dim n As Integer = 0
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim val As Double = 0
        Dim ret As String = String.Empty

        'fix null values for starting matrix and target matrix
        If fixnulls Then
            For i = 0 To dt.Rows.Count - 1
                If IsDBNull(dt.Rows(i)(fld)) OrElse dt.Rows(i)(fld).ToString = "" OrElse dt.Rows(i)(fld).ToString = "0" Then
                    dt.Rows(i)(fld) = 0.001
                End If
            Next
        End If

        'multi dimensions
        Dim ddt As New DataTable
        Dim fldnames(0) As String
        Dim flt As String = String.Empty
        Dim adddims() As String = Split(dims, ",")
        ReDim Preserve lensums(adddims.Length - 1)
        For n = 0 To adddims.Length - 1
            fldnames(0) = adddims(n)
            ddt = dt.DefaultView.ToTable(True, fldnames)
            ddt.Columns(0).ColumnName = adddims(n)
            lensums(n) = ddt.Rows.Count
            Dim col As DataColumn
            col = New DataColumn
            col.DataType = System.Type.GetType("System.Double")
            col.ColumnName = adddims(n) & "Sum"
            ddt.Columns.Add(col)
            For i = 0 To ddt.Rows.Count - 1
                flt = adddims(n) & "='" & ddt.Rows(i)(adddims(n)).ToString & "'"
                Try
                    If IsDBNull(dt.Compute("Sum" & "(" & fld & ")", flt)) Then
                        val = 0.001
                    Else
                        val = dt.Compute("Sum" & "(" & fld & ")", flt)
                    End If
                    val = FormatNumber(val, 3)
                    ddt.Rows(i)(adddims(n) & "Sum") = Round(val, 3)
                Catch ex As Exception
                    ret = ret & " ERROR!! " & ex.Message
                End Try
            Next
            If n = 0 Then
                dtSums = ddt
            Else
                'add ddt to dtSums
                col = New DataColumn
                col.DataType = System.Type.GetType("System.String")
                col.ColumnName = adddims(n)
                dtSums.Columns.Add(col)
                col = New DataColumn
                col.DataType = System.Type.GetType("System.Double")
                col.ColumnName = adddims(n) & "Sum"
                dtSums.Columns.Add(col)
                While (ddt.Rows.Count > dtSums.Rows.Count)
                    dtSums.Rows.Add()
                End While
                For j = 0 To ddt.Rows.Count - 1
                    dtSums.Rows(j)(adddims(n)) = ddt.Rows(j)(adddims(n))
                    dtSums.Rows(j)(adddims(n) & "Sum") = Round(ddt.Rows(j)(adddims(n) & "Sum"), 3)
                Next
            End If
        Next
        Return ret
    End Function


    Public Function MakeRowsColumnsArraysFromDataTables(ByRef dt1 As DataTable, ByRef dt2 As DataTable, ByVal x1 As String, ByVal x2 As String, ByVal y1 As String, ByRef x1vals() As String, ByRef x2vals() As String) As String
        Dim k As Integer = 0
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim l As Integer = 0
        Dim lf As Integer = -1
        If x1vals Is Nothing OrElse x2vals Is Nothing OrElse x1vals.Length = 0 OrElse x2vals.Length = 0 OrElse x1vals(0) Is Nothing OrElse x2vals(0) Is Nothing Then
            ReDim x1vals(0)
            ReDim x2vals(0)
            x1vals(0) = dt1.Rows(0)(x1)
            x2vals(0) = dt1.Rows(0)(x2)
        End If
        Dim ret1 As String = x1vals(0).ToString
        Dim ret2 As String = x2vals(0).ToString

        'find x1vals and x2vals from dt1 
        For k = 1 To dt1.Rows.Count - 1
            'find lf as i in x1vals
            lf = -1
            For l = 0 To x1vals.Length - 1
                If x1vals(l) = dt1.Rows(k)(x1) Then
                    lf = l
                    Exit For
                End If
            Next
            If lf = -1 Then
                i = x1vals.Length
                ReDim Preserve x1vals(i)
                x1vals(i) = dt1.Rows(k)(x1)
                ret1 = ret1 & "," & x1vals(i)
            End If
            'find lf as j in x2vals
            lf = -1
            For l = 0 To x2vals.Length - 1
                If x2vals(l) = dt1.Rows(k)(x2) Then
                    lf = l
                    Exit For
                End If
            Next
            If lf = -1 Then
                j = x2vals.Length
                ReDim Preserve x2vals(j)
                x2vals(j) = dt1.Rows(k)(x2)
                ret2 = ret2 & "," & x2vals(j)
            End If
        Next

        'add x1vals and x2vals from dt2 
        For k = 1 To dt2.Rows.Count - 1
            'find lf as i in x1vals
            lf = -1
            For l = 0 To x1vals.Length - 1
                If x1vals(l) = dt2.Rows(k)(x1) Then
                    lf = l
                    Exit For
                End If
            Next
            If lf = -1 Then
                i = x1vals.Length
                ReDim Preserve x1vals(i)
                x1vals(i) = dt2.Rows(k)(x1)
                ret1 = ret1 & "," & x1vals(i)
            End If

            'find lf as j in x2vals
            lf = -1
            For l = 0 To x2vals.Length - 1
                If x2vals(l) = dt2.Rows(k)(x2) Then
                    lf = l
                    Exit For
                End If
            Next
            If lf = -1 Then
                j = x2vals.Length
                ReDim Preserve x2vals(j)
                x2vals(j) = dt2.Rows(k)(x2)
                ret2 = ret2 & "," & x2vals(j)
            End If
        Next
        Array.Sort(x1vals)
        Array.Sort(x2vals)
        Return ret1 & " | " & ret2
    End Function
    Public Function MakeMatrixFromDataTableMultiColumns(ByRef dt As DataTable, ByVal x1 As String, ByVal x1vals() As String, ByVal x2vals() As String, Optional ByRef er As String = "") As Double(,)
        Dim m(x1vals.Length - 1, x2vals.Length - 1) As Double
        Try
            Dim i, j, k As Integer
            For i = 0 To x1vals.Length - 1
                For j = 0 To x2vals.Length - 1
                    m(i, j) = 0.001
                Next
            Next
            For j = 0 To x2vals.Length - 1
                For i = 0 To x1vals.Length - 1
                    For k = 0 To dt.Rows.Count - 1
                        If dt.Rows(k)(x1).ToString = x1vals(i) Then
                            m(i, j) = dt.Rows(k)(x2vals(j)).ToString
                        End If
                    Next
                Next
            Next
            Return m
        Catch ex As Exception
            er = ex.Message
            Return m
        End Try
    End Function
    Public Function MakeDataTableFromMatrixMultiColumns(ByVal arr As Double(,), ByVal x1 As String, ByVal x1vals() As String, ByVal x2vals() As String, ByVal q As Double, Optional ByRef er As String = "") As DataTable
        'q - precision
        ReDim Preserve arr(x1vals.Length - 1, x2vals.Length - 1)
        Dim i, j As Integer
        For i = 0 To x1vals.Length - 1
            For j = 0 To x2vals.Length - 1
                If arr(i, j) < q Then
                    arr(i, j) = 0
                End If
            Next
        Next

        Dim ret As String = String.Empty
        Dim n As Integer = x1vals.Length
        Dim m As Integer = x2vals.Length

        Dim sumi(n - 1) As Double
        Dim sumj(m - 1) As Double
        'create table dtr 
        Dim dtr As New DataTable
        Dim col As DataColumn
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = x1
        dtr.Columns.Add(col)
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "Sum of row " & x1
        dtr.Columns.Add(col)
        For j = 0 To m - 1
            If x2vals(j).Trim = "" Then x2vals(j) = "-"
            col = New DataColumn
            col.DataType = System.Type.GetType("System.String")
            col.ColumnName = x2vals(j)
            dtr.Columns.Add(col)
        Next
        Try
            For i = 0 To n - 1
                sumi(i) = 0
                Dim Row As DataRow = dtr.NewRow()
                Row(x1) = x1vals(i).ToString
                For j = 0 To m - 1
                    sumi(i) = sumi(i) + arr(i, j)
                    arr(i, j) = Round(arr(i, j), 2)
                    If arr(i, j) = Round(arr(i, j), 0) Then
                        arr(i, j) = Round(arr(i, j), 0)
                    End If
                    Row(x2vals(j)) = arr(i, j).ToString
                Next
                sumi(i) = Round(Convert.ToDouble(sumi(i)), 2)
                If sumi(i) = Round(Convert.ToDouble(sumi(i)), 0) Then
                    sumi(i) = Round(Convert.ToDouble(sumi(i)), 0)
                End If
                Row("Sum of row " & x1) = sumi(i).ToString
                dtr.Rows.Add(Row)
            Next
            'sum by x2
            Dim Rows As DataRow = dtr.NewRow()
            Dim sumt As Double = 0
            Rows("Sum of row " & x1) = "Sum by columns:"
            For j = 0 To m - 1
                sumj(j) = 0
                For i = 0 To n - 1
                    sumj(j) = sumj(j) + arr(i, j)
                Next
                sumj(j) = Round(Convert.ToDouble(sumj(j)), 2)
                If sumj(j) = Round(Convert.ToDouble(sumj(j)), 0) Then
                    sumj(j) = Round(Convert.ToDouble(sumj(j)), 0)
                End If
                Rows(x2vals(j)) = sumj(j).ToString
                sumt = sumt + sumj(j)
            Next
            Rows(x1) = "Total: " & sumt
            dtr.Rows.Add(Rows)
        Catch ex As Exception
            er = "ERROR!! " & ex.Message
            Return Nothing
        End Try
        Return dtr
    End Function
    Public Function MakeMatrixFromDataTable(ByRef dt As DataTable, ByVal x1 As String, ByVal x2 As String, ByVal y1 As String, ByRef x1vals() As String, ByRef x2vals() As String) As Double(,)
        Dim m(x1vals.Length - 1, x2vals.Length - 1) As Double
        'add columns for i,j
        Dim col As DataColumn
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "I" ' x1
        Try
            dt.Columns.Add(col)
        Catch ex As Exception

        End Try

        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "J" 'x2
        Try
            dt.Columns.Add(col)
        Catch ex As Exception

        End Try


        If dt.Rows.Count = 0 Then
            Return m
        End If

        Dim k As Integer = 0
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim l As Integer = 0

        'If x1vals Is Nothing OrElse x2vals Is Nothing OrElse x1vals.Length = 0 OrElse x2vals.Length = 0 OrElse x1vals(0) Is Nothing OrElse x2vals(0) Is Nothing Then
        '    ReDim x1vals(0)
        '    ReDim x2vals(0)
        '    x1vals(0) = dt.Rows(0)(x1)
        '    x2vals(0) = dt.Rows(0)(x2)
        'End If
        dt.Rows(0)("I") = "0"
        dt.Rows(0)("J") = "0"
        'dt sorted by x1,x2
        For k = 1 To dt.Rows.Count - 1

            'find l as i in x1vals
            For l = 0 To x1vals.Length - 1
                If x1vals(l) = dt.Rows(k)(x1) Then
                    dt.Rows(k)("I") = l.ToString
                    Exit For
                End If
            Next

            'find l as j in x2vals
            For l = 0 To x2vals.Length - 1
                If x2vals(l) = dt.Rows(k)(x2) Then
                    dt.Rows(k)("J") = l.ToString
                    Exit For
                End If
            Next
        Next


        m(0, 0) = dt.Rows(0)(y1)
        i = 0
        j = 0
        For k = 1 To dt.Rows.Count - 1
            If IsNumeric(dt.Rows(k)("I")) AndAlso IsNumeric(dt.Rows(k)("J")) Then
                i = CInt(dt.Rows(k)("I"))
                j = CInt(dt.Rows(k)("J"))
                m(i, j) = dt.Rows(k)(y1)
            End If
        Next
        For i = 0 To x1vals.Length - 1
            For j = 0 To x2vals.Length - 1
                If m(i, j) = Nothing OrElse m(i, j) = 0 Then m(i, j) = 0.001
            Next
        Next
        Return m
    End Function
    Public Function AdjustSumsByMatrix(ByRef a As Double(,), ByRef c As Double(), ByRef d As Double(), ByRef sm As Double) As String
        Dim ret As String = String.Empty
        Try
            Dim n As Integer = c.Length
            Dim m As Integer = d.Length
            'overal starting sum
            Dim sumt As Double = 0
            For i = 0 To n - 1
                For j = 0 To m - 1
                    sumt = sumt + a(i, j)
                Next
            Next
            'starting sums
            Dim sumc As Double = 0
            For i = 0 To n - 1
                If c(i) = 0 Then c(i) = 0.001
                sumc = sumc + c(i)
            Next
            'adjust starting sums
            If sumt <> sumc Then
                For i = 0 To n - 1
                    c(i) = c(i) * (sumt / sumc)
                Next
            End If
            'overal target sum
            Dim sumd As Double = 0
            For j = 0 To m - 1
                If d(j) = 0 Then d(j) = 0.001
                sumd = sumd + d(j)
            Next
            'adjust target sums
            If sumt <> sumd Then
                For j = 0 To m - 1
                    d(j) = d(j) * (sumt / sumd)
                Next
            End If
            sm = sumt
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function AdjustDataTableByOverallSum(ByRef xx As DataTable, ByVal sm As Double) As String
        Dim ret As String = String.Empty
        Dim i As Integer
        Try
            'overal matrix sum
            Dim sumt As Double = 0
            For i = 0 To xx.Rows.Count - 1
                sumt = sumt + xx.Rows(i)("Value")
            Next

            'adjust the matrix
            For i = 0 To xx.Rows.Count - 1
                xx.Rows(i)("Value") = xx.Rows(i)("Value") * (sm / sumt)
            Next
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function AdjustSumsTableByOverallSum(ByRef xx As DataTable, ByVal dims As String, ByVal sm As Double, ByVal llsums() As Integer) As String
        Dim ret As String = String.Empty
        Dim i, j As Integer
        Dim sumt As Double = 0
        Dim flds() As String = Split(dims, ",")
        Dim m = flds.Count
        Try
            For j = 0 To m - 1
                'overal matrix sum
                sumt = 0
                For i = 0 To llsums(j) - 1
                    sumt = sumt + xx.Rows(i)(flds(j) & "Sum")
                Next

                'adjust the matrix
                For i = 0 To llsums(j) - 1
                    xx.Rows(i)(flds(j) & "Sum") = Round(xx.Rows(i)(flds(j) & "Sum") * (sm / sumt), 2)
                Next
            Next
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function AdjustMatrixByOverallSum(ByRef xx As Double(,), ByVal sm As Double) As String
        Dim ret As String = String.Empty
        Dim n As Integer = xx.GetLength(0)
        Dim m As Integer = xx.GetLength(1)
        Try
            'overal matrix sum
            Dim sumt As Double = 0
            For i = 0 To n - 1
                For j = 0 To m - 1
                    sumt = sumt + xx(i, j)
                Next
            Next

            'adjust the matrix
            For i = 0 To n - 1
                For j = 0 To m - 1
                    xx(i, j) = xx(i, j) * (sm / sumt)
                Next
            Next
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function MakeDataTableFromSumsOfRowsCols(ByVal a As Double(,), ByRef c As Double(), ByRef d As Double(), ByVal x1 As String, ByVal x2 As String, ByVal y1 As String, ByRef x1vals() As String, ByRef x2vals() As String, Optional ByRef er As String = "", Optional ByVal fnc As String = "") As DataTable
        Dim ret As String = String.Empty
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim n As Integer = x1vals.Length
        Dim m As Integer = x2vals.Length
        If c.Length <> n OrElse d.Length <> m Then
            er = "ERROR!! Arrays of sums should be the same dimensions as Matrix columns and rows"
            Return Nothing
        End If
        Dim sumt As Double = 0
        'For i = 0 To n - 1
        '    For j = 0 To m - 1
        '        sumt = sumt + a(i, j)
        '    Next
        'Next
        'create table dtr 
        Dim dtr As New DataTable
        Dim col As DataColumn
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = x1
        dtr.Columns.Add(col)
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "Sum of " & fnc & " of " & y1.ToString & " by " & x1
        dtr.Columns.Add(col)
        For j = 0 To m - 1
            col = New DataColumn
            col.DataType = System.Type.GetType("System.String")
            col.ColumnName = x2vals(j)
            dtr.Columns.Add(col)
        Next
        Try
            For i = 0 To n - 1
                Dim Row As DataRow = dtr.NewRow()
                Row(x1) = x1vals(i).ToString
                c(i) = Round(c(i), 2)
                Row("Sum of " & fnc & " of " & y1.ToString & " by " & x1) = c(i).ToString
                dtr.Rows.Add(Row)
            Next
            'sum by x2
            Dim Rows As DataRow = dtr.NewRow()
            sumt = 0
            Rows("Sum of " & fnc & " of " & y1.ToString & " by " & x1) = "Sum of " & fnc & " of " & y1.ToString & " by " & x2.ToString & ":"
            For j = 0 To m - 1
                d(j) = Round(d(j), 2)
                Rows(x2vals(j)) = d(j).ToString
                sumt = sumt + d(j)
            Next
            Rows(x1) = "Total: " & sumt
            dtr.Rows.Add(Rows)
        Catch ex As Exception
            er = "ERROR!! " & ex.Message
            Return Nothing
        End Try
        Return dtr
    End Function
    Public Function MakeDataTableMatrixFromSumsOfRowsCols(ByRef cc As Double(), ByRef dd As Double(), ByVal x1 As String, ByRef x1vals() As String, ByRef x2vals() As String, Optional ByRef er As String = "") As DataTable
        Dim ret As String = String.Empty
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim n As Integer = x1vals.Length
        Dim m As Integer = x2vals.Length
        If cc.Length <> n OrElse dd.Length <> m Then
            er = "ERROR!! Arrays of sums should be the same dimensions as Matrix columns and rows"
            Return Nothing
        End If
        Dim sumt As Double = 0
        'create table dtr 
        Dim dtr As New DataTable
        Dim col As DataColumn
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = x1
        dtr.Columns.Add(col)
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "Sum by " & x1
        dtr.Columns.Add(col)
        For j = 0 To m - 1
            col = New DataColumn
            col.DataType = System.Type.GetType("System.String")
            col.ColumnName = x2vals(j)
            dtr.Columns.Add(col)
        Next
        Try
            For i = 0 To n - 1
                Dim Row As DataRow = dtr.NewRow()
                Row(x1) = x1vals(i).ToString
                cc(i) = Round(cc(i), 2)
                Row("Sum by " & x1) = cc(i).ToString
                dtr.Rows.Add(Row)
            Next
            'sum by columns
            Dim Rows As DataRow = dtr.NewRow()
            sumt = 0
            Rows("Sum by " & x1) = "Sum by columns:"
            For j = 0 To m - 1
                dd(j) = Round(dd(j), 2)
                Rows(x2vals(j)) = dd(j).ToString
                sumt = sumt + dd(j)
            Next
            Rows(x1) = "Total: " & sumt
            dtr.Rows.Add(Rows)
        Catch ex As Exception
            er = "ERROR!! " & ex.Message
            Return Nothing
        End Try
        Return dtr
    End Function
    Public Function MakeDataTableFromMatrix(ByVal arr(,) As Double, ByVal x1 As String, ByVal x2 As String, ByVal y1 As String, ByRef x1vals() As String, ByRef x2vals() As String, Optional ByRef er As String = "", Optional ByVal fnc As String = "") As DataTable
        Dim ret As String = String.Empty
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim n As Integer = x1vals.Length
        Dim m As Integer = x2vals.Length
        If arr.GetLength(0) <> n OrElse arr.GetLength(1) <> m Then
            er = "ERROR!! Arrays of colunms values should be the same dimensions as Matrix columns and rows"
            Return Nothing
        End If
        For i = 0 To x1vals.Length - 1
            For j = 0 To x2vals.Length - 1
                If arr(i, j) = 0.001 Then arr(i, j) = 0
            Next
        Next
        Dim sumi(n - 1) As Double
        Dim sumj(m - 1) As Double
        'create table dtr 
        Dim dtr As New DataTable
        Dim col As DataColumn
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = x1
        dtr.Columns.Add(col)
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "Sum of " & fnc & " of " & y1.ToString & " by " & x1
        dtr.Columns.Add(col)
        For j = 0 To m - 1
            If x2vals(j).Trim = "" Then x2vals(j) = "-"
            col = New DataColumn
            col.DataType = System.Type.GetType("System.String")
            col.ColumnName = x2vals(j)
            dtr.Columns.Add(col)
        Next
        Try
            For i = 0 To n - 1
                sumi(i) = 0
                Dim Row As DataRow = dtr.NewRow()
                Row(x1) = x1vals(i).ToString
                For j = 0 To m - 1
                    sumi(i) = sumi(i) + arr(i, j)
                    arr(i, j) = Round(arr(i, j), 2)
                    If arr(i, j) = Round(arr(i, j), 0) Then
                        arr(i, j) = Round(arr(i, j), 0)
                    End If
                    Row(x2vals(j)) = arr(i, j).ToString
                Next
                sumi(i) = Round(Convert.ToDouble(sumi(i)), 2)
                If sumi(i) = Round(Convert.ToDouble(sumi(i)), 0) Then
                    sumi(i) = Round(Convert.ToDouble(sumi(i)), 0)
                End If
                Row("Sum of " & fnc & " of " & y1.ToString & " by " & x1) = sumi(i).ToString
                dtr.Rows.Add(Row)
            Next
            'sum by x2
            Dim Rows As DataRow = dtr.NewRow()
            Dim sumt As Double = 0
            Rows("Sum of " & fnc & " of " & y1.ToString & " by " & x1) = "Sum of " & fnc & " of " & y1.ToString & " by " & x2.ToString & ":"
            For j = 0 To m - 1
                sumj(j) = 0
                For i = 0 To n - 1
                    sumj(j) = sumj(j) + arr(i, j)
                Next
                sumj(j) = Round(Convert.ToDouble(sumj(j)), 2)
                If sumj(j) = Round(Convert.ToDouble(sumj(j)), 0) Then
                    sumj(j) = Round(Convert.ToDouble(sumj(j)), 0)
                End If
                Rows(x2vals(j)) = sumj(j).ToString
                sumt = sumt + sumj(j)
            Next
            Rows(x1) = "Total: " & sumt
            dtr.Rows.Add(Rows)
        Catch ex As Exception
            er = "ERROR!! " & ex.Message
            Return Nothing
        End Try
        Return dtr
    End Function
    Public Function MakeDataTableArrayFromMatrix(ByVal arr(,) As Double, ByVal x1 As String, ByVal x2 As String, ByVal y1 As String, ByRef x1vals() As String, ByRef x2vals() As String, Optional ByRef er As String = "") As DataTable
        'TODO use in google chart
        Dim ret As String = String.Empty
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim n As Integer = x1vals.Length
        Dim m As Integer = x2vals.Length
        If arr.GetLength(0) <> n OrElse arr.GetLength(1) <> m Then
            er = "ERROR!! Arrays of colunms values should be the same dimensions as Matrix columns and rows"
            Return Nothing
        End If
        'create table dtr 
        Dim dtr As New DataTable
        Dim col As DataColumn
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = x1
        dtr.Columns.Add(col)
        If x1 <> x2 Then
            col = New DataColumn
            col.DataType = System.Type.GetType("System.String")
            col.ColumnName = x2
            dtr.Columns.Add(col)
        End If
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "ARR"
        dtr.Columns.Add(col)
        Try
            For i = 0 To n - 1
                For j = 0 To m - 1
                    Dim Row As DataRow = dtr.NewRow()
                    Row(x1) = x1vals(i).ToString
                    Row(x2) = x2vals(j).ToString
                    Row("ARR") = arr(i, j).ToString
                    dtr.Rows.Add(Row)
                Next
            Next
        Catch ex As Exception
            er = "ERROR!! " & ex.Message
            Return Nothing
        End Try
        Return dtr
    End Function
    Public Function ColorOfTask(ByVal taskid As Integer, ByVal logon As String, ByRef tooltp As String) As String
        Try
            Dim dv As DataView = mRecords("SELECT * FROM OURHelpDesk WHERE ID=" & taskid.ToString)
            If dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then
                Return ""
            End If
            Dim stas As String = dv.Table.Rows(0)("Status")
            tooltp = tooltp & ": " & dv.Table.Rows(0)("Ticket") & ", status = " & stas
            If stas.ToString.Trim = "" Then
                Return ""
            End If
            Dim unit = dv.Table.Rows(0)("Prop1")
            dv = mRecords("SELECT * FROM ourtasklistsetting WHERE Prop1='status' AND UnitName='" & unit & "' AND [User] ='" & logon & "' AND FldText ='" & stas & "'")
            If dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then
                Return "green"
            End If
            Return dv.Table.Rows(0)("FldColor")
        Catch ex As Exception
            Return ""
        End Try
    End Function
    Public Function IsColumnFromDataTable(ByVal dt As DataTable, ByVal col As String, Optional ByRef cer As String = "") As Boolean
        Dim bcol As Boolean = False
        Dim i As Integer
        Try
            For i = 0 To dt.Columns.Count - 1
                If dt.Columns(i).Caption.ToUpper = col.ToUpper Then
                    cer = dt.Columns(i).Caption
                    bcol = True
                End If
            Next
        Catch ex As Exception
            cer = ex.Message
            bcol = False
        End Try
        Return bcol
    End Function
    Public Function CleanTableFromDeletedFields(ByVal rep As String, ByVal tabl As String, ByVal col As String, ByVal selfld() As String, Optional ByRef cer As String = "") As String
        'delete records from tabl where ReportId = rep and col value are not in selfields()
        Dim ret As String = String.Empty
        Dim qsql As String = String.Empty
        Dim i As Integer = 0
        Dim m As Integer = 0
        Try
            Dim nf As Boolean = False
            qsql = "SELECT * FROM " & tabl & " WHERE (ReportID='" & rep & "')"
            Dim dvs As DataView = mRecords(qsql)
            If Not dvs Is Nothing AndAlso Not dvs.Table Is Nothing AndAlso dvs.Table.Rows.Count > 0 Then
                If IsColumnFromDataTable(dvs.Table, col) Then
                    For i = 0 To selfld.Length - 1
                        If selfld(i).ToUpper = dvs.Table.Rows(col).ToString.ToUpper Then
                            nf = True
                            Continue For
                        End If
                    Next
                    If nf = False Then
                        'delete record from tabl
                        qsql = "DELETE FROM " & tabl & " WHERE (ReportID='" & rep & "' AND " & col & "=" & dvs.Table.Rows(col).ToString & ")"
                        ret = ExequteSQLquery(qsql)
                        If ret = "Query executed fine." Then
                            ret = ""
                        Else
                            cer = cer & " " & ret
                        End If
                        m = m + 1
                    End If
                End If
            End If
        Catch ex As Exception
            ret = m.ToString & " " & ex.Message
        End Try
        cer = cer & "! " & m.ToString & " records were deleted from " & tabl & " by field names in column " & col & " not selected in the report query. "
        Return cer
    End Function
    Function MakeDTColumnsNamesCLScompliant(ByVal dt As DataTable, Optional ByVal myconprv As String = "", Optional ByVal er As String = "") As DataTable
        Dim ret As String = String.Empty
        If myconprv = "" Then
            myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        End If
        If dt Is Nothing Then
            ret = "ERROR!! No data"
            Return Nothing
        End If
        Try
            For i = 0 To dt.Columns.Count - 1
                dt.Columns(i).Caption = cleanText(FixReservedWords(dt.Columns(i).Caption, myconprv))
                dt.Columns(i).Caption = Regex.Replace(dt.Columns(i).Caption, "[^a-zA-Z0-9_]", "")  'CLS-compliant
                If myconprv.StartsWith("InterSystems.Data.") Then
                    dt.Columns(i).Caption = dt.Columns(i).Caption.Replace("_", "")
                End If
                dt.Columns(i).ColumnName = dt.Columns(i).Caption
            Next
            dt.AcceptChanges()
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return dt
    End Function
    Public Function MakeTableColumnNameTypeCLScompliant(ByVal tblname As String, ByVal colname As String, ByVal datatype As String, ByVal userconnstr As String, ByVal userconnprv As String, Optional ByRef er As String = "") As String
        'ON HOLD
        'Rename column and make datatype nvarchar or text
        Dim ret As String = String.Empty
        Dim col As String = String.Empty
        Dim typ As String = String.Empty
        Dim msql As String = String.Empty
        Try
            col = Regex.Replace(colname, "[^a-zA-Z0-9_]", "")  'CLS-compliant
            col = cleanText(FixReservedWords(col, userconnprv))
            If userconnprv.StartsWith("InterSystems.Data.") Then
                col = col.Replace("_", "")
                msql = "UPDATE %Dictionary.PropertyDefinition SET SqlFieldName='" & col & "'"
                msql &= "WHERE parent='" & tblname & "' AND Name='" & colname & "'"
                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                If colname <> col Then
                    msql = "ALTER TABLE " & tblname & " MODIFY " & colname & " RENAME " & col & ""
                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                End If
            End If
            Dim collen As String = "4000"

            '=============================================================COLUMN NAME==================================================================

            If colname <> col Then
                'rename column
                If userconnprv = "MySql.Data.MySqlClient" Then

                    Dim db As String = GetDataBase(userconnstr, userconnprv)
                    msql = "ALTER TABLE `" & db & "`.`" & tblname & "` CHANGE COLUMN `" & colname & "` `" & col & "` varchar(" & collen & ") DEFAULT NULL ;"
                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)

                ElseIf userconnprv.StartsWith("InterSystems.Data") Then
                    'SqlFieldName

                    msql = "ALTER TABLE " & tblname & " MODIFY " & colname & " RENAME " & col & ""
                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    If ret <> "Query executed fine." Then
                        msql = "UPDATE %Dictionary.PropertyDefinition SET Name='" & col & "', SqlFieldName='' "
                        msql &= "WHERE parent='" & tblname & "' AND Name='" & colname & "'"
                        ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    End If

                    'msql = "ALTER TABLE " & tblname & " MODIFY " & col & " NVARCHAR(" & collen & ") NULL "
                    'ret = ExequteSQLquery(msql, userconnstr, userconnprv)

                ElseIf userconnprv = "Oracle.ManagedDataAccess.Client" Then

                    msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " To " & col
                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    msql = "ALTER TABLE " & tblname & " MODIFY " & col & " VARCHAR2(" & collen & " CHAR)"
                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)

                    'RetypeAndReplaceColumn(tblname, col, "str", userconnstr, userconnprv)

                ElseIf userconnprv = "System.Data.SqlClient" Then

                    msql = "sp_rename '" & tblname & "." & colname & "', '" & col & "', 'COLUMN'"
                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & colname & " VARCHAR(" & collen & ")  NULL"
                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)

                    'RetypeAndReplaceColumn(tblname, col, "str", userconnstr, userconnprv)


                ElseIf userconnprv = "Npgsql" Then  'PostgreSQL  Npgsql

                    msql = "ALTER TABLE `" & tblname & "` RENAME COLUMN `" & colname & "` TO `" & col & "`"
                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    msql = "ALTER TABLE `" & tblname & "` ALTER COLUMN `" & col & "` TYPE character varying(" & collen & ")  NULL"
                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)

                    'RetypeAndReplaceColumn(tblname, col, "str", userconnstr, userconnprv)

                Else
                    'TODO odbc, ole
                    Dim userODBCdriver As String = String.Empty
                    Dim userODBCdatabase As String = String.Empty
                    Dim userODBCdatasource As String = String.Empty
                    If userconnprv.StartsWith("System.Data.Odbc") Then
                        userconnstr = userconnstr.Replace("Password", "Pwd").Replace("User ID", "UID")
                        Dim bConnect As Boolean = DatabaseConnected(userconnstr, userconnprv, ret, userODBCdriver, userODBCdatabase, userODBCdatasource)
                        If Not bConnect Then
                            ret = "ERROR!! Database not connected. " & ret
                            Return ret
                        End If
                    ElseIf userconnprv.StartsWith("System.Data.OleDb") Then
                        'OleDb
                        Dim bConnect As Boolean = DatabaseConnected(userconnstr, userconnprv, ret, userODBCdriver, userODBCdatabase, userODBCdatasource)
                        If Not bConnect Then
                            ret = "ERROR!! Database not connected. " & ret
                            Return ret
                        End If
                    End If

                    If userconnprv.StartsWith("System.Data.Odbc") Then
                        msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " To " & col
                        ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        colname = col
                        If userODBCdriver.ToUpper.StartsWith("CACHE") OrElse userODBCdriver.StartsWith("IRIS") Then
                            msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & colname & " NVARCHAR(" & collen & ")"
                        ElseIf userODBCdriver.ToUpper.StartsWith("SQL") Then
                            msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & colname & " VARCHAR(" & collen & ")  NULL"
                        ElseIf userODBCdriver.ToUpper.StartsWith("MYSQL") Then
                            Dim db As String = GetDataBase(userconnstr, userconnprv)
                            msql = "ALTER TABLE `" & db & "`.`" & tblname & "` CHANGE COLUMN `" & colname & "` `" & colname & "` VARCHAR(" & collen & ")  ;"  'NULL DEFAULT NULL
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        ElseIf userODBCdriver.ToUpper.StartsWith("ORACLE") Then
                            msql = "ALTER TABLE " & tblname & " MODIFY " & colname & " VARCHAR2(" & collen & " CHAR)"
                        ElseIf userODBCdriver.ToUpper.StartsWith("PSQL") Then  'PostgreSQL  Npgsql
                            msql = "ALTER TABLE `" & tblname & "` ALTER COLUMN `" & colname & "` TYPE character varying(" & collen & ")  NULL"
                        End If

                    ElseIf userconnprv.StartsWith("System.Data.OleDb") Then
                        msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " To " & col
                        ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        colname = col
                        If userODBCdriver.ToUpper.StartsWith("CACHE") OrElse userODBCdriver.StartsWith("IRIS") Then
                            msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & colname & " NVARCHAR(" & collen & ")"
                        ElseIf userODBCdriver.ToUpper.StartsWith("SQL") Then
                            msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & colname & " VARCHAR(" & collen & ")  NULL"
                        ElseIf userODBCdriver.ToUpper.StartsWith("MYSQL") Then
                            Dim db As String = GetDataBase(userconnstr, userconnprv)
                            msql = "ALTER TABLE `" & db & "`.`" & tblname & "` CHANGE COLUMN `" & colname & "` `" & colname & "` VARCHAR(" & collen & ")  ;"  'NULL DEFAULT NULL
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        ElseIf userODBCdriver.ToUpper.StartsWith("ORACLE") Then
                            msql = "ALTER TABLE " & tblname & " MODIFY " & colname & " VARCHAR2(" & collen & " CHAR)"
                        ElseIf userODBCdriver.ToUpper.StartsWith("PSQL") Then  'PostgreSQL  Npgsql
                            msql = "ALTER TABLE `" & tblname & "` ALTER COLUMN `" & colname & "` TYPE character varying(" & collen & ")  NULL"
                        End If
                    End If
                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                End If

                'Else
                '    msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " To " & col
                '    ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                '    msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & col & " VARCHAR(4000)  NULL"
                '    ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                '    'RetypeAndReplaceColumn(tblname, col, "str", userconnstr, userconnprv)
                Return ret
            End If

            '=============================================================DATA TYPE==================================================================

            'colname=col
            If userconnprv = "MySql.Data.MySqlClient" Then

                Dim db As String = GetDataBase(userconnstr, userconnprv)
                msql = "ALTER TABLE `" & db & "`.`" & tblname & "` CHANGE COLUMN `" & colname & "` `" & col & "` varchar(" & collen & ") DEFAULT NULL ;"
                ret = ExequteSQLquery(msql, userconnstr, userconnprv)

            ElseIf userconnprv.StartsWith("InterSystems.Data") Then

                'Try
                '    msql = "ALTER TABLE " & tblname & " MODIFY " & colname & " RENAME " & col & ""
                '    ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                'Catch ex As Exception
                '    msql = "UPDATE %Dictionary.PropertyDefinition SET Name='" & col & "' "
                '    msql &= "WHERE parent='" & tblname & "' AND Name='" & colname & "'"
                '    ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                'End Try

                'msql = "ALTER TABLE " & tblname & " MODIFY " & col & " NVARCHAR(" & collen & ") NULL "
                'ret = ExequteSQLquery(msql, userconnstr, userconnprv)

                ret = RetypeAndReplaceColumn(tblname, col, "str", userconnstr, userconnprv)

            ElseIf userconnprv = "Oracle.ManagedDataAccess.Client" Then

                'msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " To " & col
                'ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                msql = "ALTER TABLE " & tblname & " MODIFY " & col & " VARCHAR2(" & collen & " CHAR)"
                ret = ExequteSQLquery(msql, userconnstr, userconnprv)

                'RetypeAndReplaceColumn(tblname, col, "str", userconnstr, userconnprv)

            ElseIf userconnprv = "System.Data.SqlClient" Then

                'msql = "sp_rename '" & tblname & "." & colname & "', '" & col & "', 'COLUMN'"
                'ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                'msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & colname & " VARCHAR(" & collen & ")  NULL"
                'ret = ExequteSQLquery(msql, userconnstr, userconnprv)

                ret = RetypeAndReplaceColumn(tblname, col, "str", userconnstr, userconnprv)


            ElseIf userconnprv = "Npgsql" Then  'PostgreSQL  Npgsql

                'msql = "ALTER TABLE `" & tblname & "` RENAME COLUMN `" & colname & "` TO `" & col & "`"
                'ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                'msql = "ALTER TABLE `" & tblname & "` ALTER COLUMN `" & col & "` TYPE character varying(" & collen & ")  NULL"
                'ret = ExequteSQLquery(msql, userconnstr, userconnprv)

                ret = RetypeAndReplaceColumn(tblname, col, "str", userconnstr, userconnprv)

            Else
                'TODO odbc, ole
                Dim userODBCdriver As String = String.Empty
                Dim userODBCdatabase As String = String.Empty
                Dim userODBCdatasource As String = String.Empty
                If userconnprv.StartsWith("System.Data.Odbc") Then
                    userconnstr = userconnstr.Replace("Password", "Pwd").Replace("User ID", "UID")
                    Dim bConnect As Boolean = DatabaseConnected(userconnstr, userconnprv, ret, userODBCdriver, userODBCdatabase, userODBCdatasource)
                    If Not bConnect Then
                        ret = "ERROR!! Database not connected. " & ret
                        Return ret
                    End If
                ElseIf userconnprv.StartsWith("System.Data.OleDb") Then
                    'OleDb
                    Dim bConnect As Boolean = DatabaseConnected(userconnstr, userconnprv, ret, userODBCdriver, userODBCdatabase, userODBCdatasource)
                    If Not bConnect Then
                        ret = "ERROR!! Database not connected. " & ret
                        Return ret
                    End If
                End If

                If userconnprv.StartsWith("System.Data.Odbc") Then
                    If userODBCdriver.ToUpper.StartsWith("CACHE") OrElse userODBCdriver.StartsWith("IRIS") Then
                        msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & colname & " NVARCHAR(" & collen & ")"
                    ElseIf userODBCdriver.ToUpper.StartsWith("SQL") Then
                        msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & colname & " VARCHAR(" & collen & ")  NULL"
                    ElseIf userODBCdriver.ToUpper.StartsWith("MYSQL") Then
                        Dim db As String = GetDataBase(userconnstr, userconnprv)
                        msql = "ALTER TABLE `" & db & "`.`" & tblname & "` CHANGE COLUMN `" & colname & "` `" & colname & "` VARCHAR(" & collen & ")  ;"  'NULL DEFAULT NULL
                        ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    ElseIf userODBCdriver.ToUpper.StartsWith("ORACLE") Then
                        msql = "ALTER TABLE " & tblname & " MODIFY " & colname & " VARCHAR2(" & collen & " CHAR)"
                    ElseIf userODBCdriver.ToUpper.StartsWith("PSQL") Then  'PostgreSQL  Npgsql
                        msql = "ALTER TABLE `" & tblname & "` ALTER COLUMN `" & colname & "` TYPE character varying(" & collen & ")  NULL"
                    End If

                ElseIf userconnprv.StartsWith("System.Data.OleDb") Then
                    If userODBCdriver.ToUpper.StartsWith("CACHE") OrElse userODBCdriver.StartsWith("IRIS") Then
                        msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & colname & " NVARCHAR(" & collen & ")"
                    ElseIf userODBCdriver.ToUpper.StartsWith("SQL") Then
                        msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & colname & " VARCHAR(" & collen & ")  NULL"
                    ElseIf userODBCdriver.ToUpper.StartsWith("MYSQL") Then
                        Dim db As String = GetDataBase(userconnstr, userconnprv)
                        msql = "ALTER TABLE `" & db & "`.`" & tblname & "` CHANGE COLUMN `" & colname & "` `" & colname & "` VARCHAR(" & collen & ")  ;"  'NULL DEFAULT NULL
                        ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    ElseIf userODBCdriver.ToUpper.StartsWith("ORACLE") Then
                        msql = "ALTER TABLE " & tblname & " MODIFY " & colname & " VARCHAR2(" & collen & " CHAR)"
                    ElseIf userODBCdriver.ToUpper.StartsWith("PSQL") Then  'PostgreSQL  Npgsql
                        msql = "ALTER TABLE `" & tblname & "` ALTER COLUMN `" & colname & "` TYPE character varying(" & collen & ")  NULL"
                    End If
                End If
                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
            End If

            'Else
            '    msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " To " & col
            '    ret = ExequteSQLquery(msql, userconnstr, userconnprv)
            '    msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & col & " VARCHAR(4000)  NULL"
            '    ret = ExequteSQLquery(msql, userconnstr, userconnprv)
            '    'ret=RetypeAndReplaceColumn(tblname, col, "str", userconnstr, userconnprv)



        Catch ex As Exception
            ret = "ERROR!!" & ex.Message
        End Try
        Return ret
    End Function
    Public Function CreateCleanReportColumnsFieldsItems(ByVal repid As String, ByVal db As String, ByVal userconnstring As String, ByVal userconnprov As String) As String
        Dim ret As String = String.Empty
        Try
            ret = SetReportFieldData(repid)
            '------------------------------------------------------------------------------------
            'Put data in OURReportFormat
            Dim ddt As DataTable = GetListOfReportFields(repid)  'List of Report fields from xsd
            If ddt Is Nothing OrElse ddt.Columns Is Nothing OrElse ddt.Columns.Count = 0 Then
                ret = "There are no data found for this report or error while saving report."
                Return ret
            End If

            Dim dtrf As DataTable = GetReportFields(repid) 'from OURReportFormat
            Dim frname As String = String.Empty
            Dim insSQL As String = String.Empty
            If dtrf Is Nothing OrElse dtrf.Rows.Count = 0 Then
                'no records of Report Fields in OURReportFormat, insert them from ddt aka xsd fields...
                'add all fields from ddt from xsd
                For i = 0 To ddt.Columns.Count - 1
                    If ddt.Columns(i).Caption <> "Indx" Then
                        frname = GetFriendlySQLFieldName(repid, ddt.Columns(i).Caption)
                        If frname.Trim = ddt.Columns(i).Caption.Trim Then frname = ""
                        insSQL = "INSERT INTO OURReportFormat (ReportID,Prop,Val,[Order], Prop1) VALUES('" & repid & "','FIELDS','" & ddt.Columns(i).Caption & "'," & (i + 1).ToString & ",'" & frname & "')"
                        ExequteSQLquery(insSQL)
                    End If
                Next
            End If
            Dim HasCol As Boolean = HasReportColumns(repid)  'from OURReportFormat
            Dim HasRepItems As Boolean = ReportItemsExist(repid) 'from ourreportitems
            Dim HasSQLQueryData As Boolean = HasSQLData(repid) 'from OURReportSQLquery

            'Put data in OURReportSqlQuery
            If Not HasSQLQueryData Then
                Dim ItemList As ListItemCollection = GetReportFieldItems(repid, userconnstring, userconnprov)
                Dim li As ListItem
                Dim fld As String
                Dim tbl As String
                For i = 0 To ItemList.Count - 1
                    li = ItemList.Item(i)
                    If li.Text.Trim <> String.Empty Then
                        tbl = li.Text.Substring(0, li.Text.LastIndexOf("."))
                        fld = li.Text.Substring(li.Text.LastIndexOf(".")).Replace(".", "")
                        AddField(repid, tbl, fld, "")
                    End If
                Next
            End If

            'Put data in OURReportItems
            If HasCol AndAlso Not HasRepItems Then
                'CreateReportItems(Session("REPORTID"), Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
                ret = CreateReportItems(repid, db, userconnstring, userconnprov)
                If ret.StartsWith("ERROR!!") Then
                    Return ret
                End If
            End If
            '------------------------------------------------------------------------------------
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function GetReportFieldItems(ByVal repid As String, ByVal userconnstring As String, ByVal userconnprov As String) As ListItemCollection
        'repid = Session("REPORTID")
        Dim dvm As DataView
        Dim par, ret As String
        Dim i As Integer
        Dim ItemList As New ListItemCollection
        'all parameters
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim xsdfile As String = applpath & "XSDFILES\" & repid & ".xsd"
        dvm = ReturnDataViewFromXML(xsdfile)

        If dvm Is Nothing OrElse dvm.Table Is Nothing Then
            'If dvm Is Nothing OrElse dvm.Count = 0 Then
            Try
                Dim sqlquerytext As String = MakeSQLQueryFromDB(repid, userconnstring, userconnprov)
                'update XSD
                Dim xsdpath As String = applpath & "XSDFILES\"
                Dim er As String = String.Empty
                Dim dt As DataTable = mRecords(sqlquerytext, er, userconnstring, userconnprov).Table
                If er <> String.Empty Then
                    ret = "ERROR!! " & er
                    ItemList.Add(New ListItem("Error"))
                    Return ItemList
                End If
                ret = CreateXSDForDataTable(dt, repid, xsdpath)
                dvm = ReturnDataViewFromXML(xsdfile)
            Catch ex As Exception
                ret = "ERROR!! " & ex.Message
                ItemList.Add(New ListItem("Error"))
                Return ItemList
            End Try
        End If
        Dim li As ListItem
        ItemList.Add(New ListItem(" "))

        Dim htFields As Hashtable = GetExpandedFields(repid, userconnstring, userconnprov)
        Dim typ As String = String.Empty
        For i = 0 To dvm.Table.Columns.Count - 1
            'par = FixReservedWords(dvm.Table.Columns(i).Caption)
            par = dvm.Table.Columns(i).Caption
            If htFields(par) IsNot Nothing AndAlso
               htFields(par).ToString <> String.Empty Then
                par = htFields(par).ToString
                typ = dvm.Table.Columns(i).DataType.FullName
                li = New ListItem(par, typ)
                li.Selected = IsSelected(repid, li.Text)
                ItemList.Add(li)
            End If
        Next
        Return ItemList
    End Function
    Public Function GetExpandedFields(ByVal repid As String, ByVal userconnstring As String, ByVal userconnprov As String) As Hashtable
        Dim htFields As New Hashtable
        Dim ExpandedSql As String = GetExpandedReportSQL(repid, userconnstring, userconnprov)
        Dim SelectList As String = String.Empty
        Dim n As Integer = 0
        Dim i As Integer
        Dim ret As String = String.Empty
        If ExpandedSql <> String.Empty Then
            SelectList = Piece(Piece(ExpandedSql, "SELECT ", 2), " FROM ", 1).Trim   'fixing by additing spaces
            Dim DistinctIndex As Integer = SelectList.ToUpper.IndexOf("DISTINCT ")
            If DistinctIndex > -1 Then SelectList = SelectList.Substring(DistinctIndex + 9).Trim
            Dim sFields As String() = Split(SelectList, ",")
            Dim sFld As String = String.Empty
            For i = 0 To sFields.Count - 1
                Try
                    n = Pieces(sFields(i), ".")
                    sFld = Piece(sFields(i).Trim, ".", n)
                    If sFld.Contains(" AS ") Then sFld = Piece(sFld, " AS ", 2)
                    htFields.Add(sFld.Replace("[", "").Replace("]", "").Replace("`", "").Replace("""", ""), sFields(i).Trim.Replace("[", "").Replace("]", "").Replace("`", "").Replace("""", ""))
                Catch ex As Exception
                    ret = ex.Message
                End Try
            Next
        End If
        Return htFields
    End Function
    Public Function GetExpandedReportSQL(ByVal repid As String, ByVal userconnstring As String, ByVal userconnprov As String) As String
        Dim ret As String = String.Empty
        'repid = Session("REPORTID")

        Dim dvr As DataView = mRecords("Select * FROM OURReportInfo WHERE (ReportID='" & repid & "')")
        If dvr.Table.Rows.Count = 1 Then
            If dvr.Table.Rows(0)("ReportAttributes").ToString = "sql" Then
                ret = MakeSQLQueryFromDB(repid, userconnstring, userconnprov) 'dvr.Table.Rows(0)("SQLquerytext").ToString
                If ret.ToUpper.IndexOf(" FROM ") < 0 Then
                    Return ret
                End If
                Dim SQLqFrom As String = ret.Substring(ret.ToUpper.IndexOf(" FROM "))
                If SQLqFrom.ToUpper.IndexOf(" ORDER BY ") > 0 Then SQLqFrom = SQLqFrom.Substring(0, SQLqFrom.ToUpper.IndexOf(" ORDER BY "))
                Dim SQLqSelect As String = ret.Substring(0, ret.ToUpper.IndexOf(" FROM"))
                'Dim SQLqSelect As String = MakeSQLQueryFromDB(repid, Session("UserConnString"), Session("UserConnProvider"))
                Dim SQLqWhere As String = String.Empty
                Dim idxWhere As Integer = SQLqFrom.ToUpper.IndexOf(" WHERE ")
                If idxWhere > -1 Then
                    Dim sFrom As String = SQLqFrom.Substring(0, idxWhere)
                    SQLqWhere = SQLqFrom.Substring(idxWhere).Replace("""", "'")
                    SQLqFrom = sFrom & SQLqWhere
                End If
                If IsCacheDatabase(userconnprov) Then
                    'Remove TOP n, because Cache working strange if included sql has TOP n , it return subset of distinct values
                    Dim k As Integer = SQLqSelect.ToUpper.IndexOf(" TOP ")
                    If k > 0 Then
                        SQLqSelect = SQLqSelect.Substring(k + 5).Trim
                        SQLqSelect = SQLqSelect.Substring(SQLqSelect.IndexOf(" "))
                        SQLqSelect = " SELECT " & SQLqSelect
                    End If
                End If
                SQLqSelect = FixDoubleFieldNames(repid, SQLqSelect, userconnprov)
                ret = SQLqSelect & " " & SQLqFrom
            ElseIf dvr.Table.Rows(0)("ReportAttributes").ToString = "sp" Then
                'TODO for sp

            End If
        End If

        Return ret
    End Function
    Public Function IsSelected(rep As String, fld As String) As Boolean
        Dim sql As String = "Select * From ourreportshow Where DropDownID = '" & fld & "' AND ReportId = '" & rep & "'"
        Dim er As String = String.Empty
        Dim dt As DataTable = mRecords(sql, er).Table
        If er = String.Empty AndAlso dt.Rows.Count = 1 Then
            Return True
        ElseIf er = String.Empty AndAlso dt.Rows.Count = 0 Then
            'check if old style parameter
            Dim n As Integer = Pieces(fld, ".")
            If n > 1 Then
                Dim f As String = Piece(fld, ".", n)
                sql = "Select * From ourreportshow Where DropDownID = '" & f & "' AND ReportId = '" & rep & "'"
                dt = mRecords(sql, er).Table
                If er = String.Empty AndAlso dt.Rows.Count = 1 Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If
        Else
            If er <> String.Empty Then er = "ERROR!! " & er
            Return False
        End If
    End Function
    Public Function SetReportFieldData(ByVal repid As String) As String
        Dim bfld As Boolean = False
        Dim insSQL As String = String.Empty
        Dim delSQL As String = String.Empty
        Dim ddt As DataTable = Nothing
        Dim dtrf As DataTable = Nothing
        Dim frname As String = String.Empty
        Dim ret As String = String.Empty

        ddt = GetListOfReportFields(repid)  'List of Report fields from xsd
        If ddt Is Nothing Then
            ret = "xsd is not created."
            Return ret
        End If
        'add report fields from  OURReportFormat
        dtrf = GetReportFields(repid) ' from  OURReportFormat
        If dtrf Is Nothing OrElse dtrf.Rows.Count = 0 Then  'no records of Report Fields in OURReportFormat, insert them from ddt aka xsd fields...
            'add all fields from ddt
            For i As Integer = 0 To ddt.Columns.Count - 1
                If ddt.Columns(i).Caption <> "Indx" Then
                    frname = GetFriendlySQLFieldName(repid, ddt.Columns(i).Caption)
                    If frname.Trim = ddt.Columns(i).Caption.Trim Then frname = ""
                    insSQL = "INSERT INTO OURReportFormat (ReportID,Prop,Val,[Order], Prop1) VALUES('" & repid & "','FIELDS','" & ddt.Columns(i).Caption & "'," & (i + 1).ToString & ",'" & frname & "')"
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
                    DeleteReportGroup(repid, dtrf.Rows(i)("Val").ToString)
                    'delSQL = "DELETE FROM OURReportGroups WHERE ReportID='" & repid & "' AND GroupField='" & dtrf.Rows(i)("Val").ToString & "' "
                    'ret = ExequteSQLquery(delSQL)
                    'delete from ourreportgroups
                    DeleteReportGroupByCalcFld(repid, dtrf.Rows(i)("Val").ToString)
                    'delSQL = "DELETE FROM OURReportGroups WHERE ReportID='" & repid & "' AND CalcField='" & dtrf.Rows(i)("Val").ToString & "' "
                    'ret = ExequteSQLquery(delSQL)
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
        Return ret
    End Function
    Public Function DifferencesOfTwoMatrices(ByVal tbl1 As DataTable, ByVal tbl2 As DataTable) As DataTable
        For j = 0 To tbl1.Columns.Count - 1
            If tbl1.Columns(j).Caption.ToString.StartsWith("Sums by ") Then
                tbl1.Columns(j).Caption = "Differences of Target and Balanced Sums of " & tbl1.Columns(j).Caption.Substring(7)
                tbl1.Columns(j).ColumnName = "Differences of Target and Balanced Sums of " & tbl1.Columns(j).ColumnName.Substring(7)
            End If
            For i = 0 To tbl1.Rows.Count - 1
                If IsNumeric(tbl1(i)(j)) AndAlso CInt(tbl1(i)(j).ToString) > 1 AndAlso (tbl1.Columns(j).Caption.StartsWith("Differences of") OrElse tbl1.Columns(j).Caption.StartsWith("Sums by ")) Then
                    tbl1(i)(j) = Round(tbl1(i)(j) - tbl2(i)(j), 2)
                End If
                If tbl1(i)(j).ToString.StartsWith("Total:") Then
                    tbl1(i)(j) = "Difference in Total: " '& CInt(tbl1(i)(j).ToString.Replace("Total:", "").Replace(" ", "")) - CInt(tbl2(i)(j).ToString.Replace("Total:", "").Replace(" ", ""))
                End If
            Next
        Next
        Return tbl1
    End Function
    Public Function MakeTableWithAllJoins(ByVal rep As String, Optional ByRef sqlstatement As String = "", Optional ByRef er As String = "") As DataTable
        Dim i As Integer = 0
        Dim ret As String = String.Empty
        Dim msql As String = String.Empty
        Dim asql As String = "SELECT * FROM "
        Dim dv As DataView
        Dim dtj As DataTable
        Dim retj As String = String.Empty
        Dim tblj1 As String = String.Empty
        Dim tblj2 As String = String.Empty
        Dim fldj1 As String = String.Empty
        Dim fldj2 As String = String.Empty
        Try
            'array of tables
            'asql = asql & tbs & " "

            ''msql = "SELECT * FROM OURReportSQLquery WHERE (ReportId Like '" & dbname & "_joins_" & "'  AND Doing='JOIN' AND (Param1 LIKE '%" & repdb & "%' OR Comments LIKE '%" & repdb.Trim.Replace(" ", "%") & "%'))  ORDER BY Tbl1, Tbl2, Tbl1Fld1, Tbl2Fld2"
            'msql = "SELECT * FROM OURReportSQLquery WHERE ReportId='" & rep & "' AND  Doing='JOIN'  ORDER BY RecOrder "   'Tbl1, Tbl2, Tbl1Fld1, Tbl2Fld2"
            'dv = mRecords(msql, er)  'all possible joins
            'If dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then
            '    Return Nothing
            'End If
            ''TODO - DO NOT repeat table if it alredy joined, add another one if it was not joined or add only the ON statement. !!
            'dtj = dv.Table

            dtj = GetReportJoins(rep) 'sorted by RecOrder,Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2
            dtj = CorrectJoinOrder(dtj)

            For i = 0 To dtj.Rows.Count - 1
                'tblj1 = FixReservedWords(CorrectTableNameWithDots(dtj.Rows(i)("Tbl1").ToString, userconprv), userconprv)
                'tblj2 = FixReservedWords(CorrectTableNameWithDots(dtj.Rows(i)("Tbl2").ToString, userconprv), userconprv)
                'fldj1 = FixReservedWords(CorrectFieldNameWithDots(dtj.Rows(i)("Tbl1Fld1").ToString), userconprv)
                'fldj2 = FixReservedWords(CorrectFieldNameWithDots(dtj.Rows(i)("Tbl2Fld2").ToString), userconprv)

                tblj1 = "[" & dtj.Rows(i)("Tbl1").ToString & "]"
                tblj2 = "[" & dtj.Rows(i)("Tbl2").ToString & "]"
                fldj1 = "[" & dtj.Rows(i)("Tbl1Fld1").ToString & "]"
                fldj2 = "[" & dtj.Rows(i)("Tbl2Fld2").ToString & "]"
                If i = 0 OrElse dtj.Rows(i)("Oper").ToString = "1" Then
                    If i > 0 AndAlso dtj.Rows(i)("Oper").ToString = "1" Then
                        'start new group of joins
                        retj = retj & " , "
                    End If
                    'add first join, Comments field has type of JOIN: INNER JOIN, RIGHT JOIN, etc...
                    retj = retj & tblj1 & " " & dtj.Rows(i)("Comments").ToString & " " & tblj2 & " ON "
                    'ON statement
                    retj = retj & "(" & tblj1 & "." & fldj1 & "=" & tblj2 & "." & fldj2 & ") "
                Else
                    'not first join
                    If dtj.Rows(i)("Tbl1").ToString = dtj.Rows(i - 1)("Tbl1").ToString Then
                        'the same table as on previous row
                        If dtj.Rows(i)("Tbl2").ToString = dtj.Rows(i - 1)("Tbl2").ToString Then
                            'add ON statememnt
                            retj = retj & " AND "
                            retj = retj & "(" & tblj1 & "." & fldj1 & "=" & tblj2 & "." & fldj2 & ") "
                        Else
                            'add new JOIN statement to Tbl2
                            retj = retj & dtj.Rows(i)("Comments").ToString & " " & tblj2 & " ON "
                            retj = retj & "(" & tblj1 & "." & fldj1 & "=" & tblj2 & "." & fldj2 & ") "
                        End If
                    Else
                        'add new JOIN statement to new tblj1 if tblj1 is not in already 
                        If retj.Contains(" " & tblj1 & " ") AndAlso retj.Contains(" " & tblj2 & " ") Then
                            'add ON statement
                            retj = retj & " AND "
                            retj = retj & "(" & tblj1 & "." & fldj1 & "=" & tblj2 & "." & fldj2 & ") "
                        ElseIf retj.Contains(" " & tblj1 & " ") AndAlso Not retj.Contains(" " & tblj2 & " ") Then
                            'add join and ON statement
                            retj = retj & " " & dtj.Rows(i)("Comments").ToString & " " & tblj2 & " ON "
                            retj = retj & "(" & tblj1 & "." & fldj1 & "=" & tblj2 & "." & fldj2 & ") "
                        ElseIf Not retj.Contains(" " & tblj1 & " ") AndAlso retj.Contains(" " & tblj2 & " ") Then
                            'add join and ON statement
                            retj = retj & " " & dtj.Rows(i)("Comments").ToString & " " & tblj1 & " ON "
                            retj = retj & "(" & tblj1 & "." & fldj1 & "=" & tblj2 & "." & fldj2 & ") "
                        ElseIf Not retj.Contains(" " & tblj1 & " ") AndAlso Not retj.Contains(" " & tblj2 & " ") Then
                            'add join and ON statement
                            retj = retj & " " & dtj.Rows(i)("Comments").ToString & " " & tblj1 & " ON "
                            retj = retj & "(" & tblj1 & "." & fldj1 & "=" & tblj2 & "." & fldj2 & ") "
                        End If

                    End If
                End If
                'tbs = tbs.Replace(tblj1 & ",", "")
                'tbs = tbs.Replace(tblj2 & ",", "")
            Next

            asql = asql & " " & retj   '& tbs
            sqlstatement = asql
            'for sqlite:
            dv = mRecords(asql, er)
            If dv Is Nothing OrElse dv.Table Is Nothing Then
                Return Nothing
            End If
            Return dv.Table

        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
            Return Nothing
        End Try
    End Function
    Public Function CleanFolders(ByVal lgn As String) As String
        Dim ret As String = String.Empty
        If applpath Is Nothing OrElse applpath.Trim = "" Then
            applpath = System.AppDomain.CurrentDomain.BaseDirectory()
        End If
        Try
            'clean up Temp, RDLFILES, XSDFILES, KMLS folders from files starts with lgn
            Dim folderPath As String = String.Empty  'applpath & "Temp"
            For Each foldername In {"Temp", "RDLFILES", "XSDFILES", "KMLS", "ImageFiles"}
                ' Get all files starting with "lgn"
                folderPath = applpath & foldername
                If Directory.Exists(folderPath) Then
                    For Each filePath As String In Directory.GetFiles(folderPath, lgn & "*")
                        Try
                            File.Delete(filePath)
                        Catch ex As Exception
                            ret = ret & " Error!! " & ex.Message
                        End Try
                    Next
                Else
                    Directory.CreateDirectory(folderPath)
                End If
            Next
        Catch ex As Exception
            ret = "Error!! " & ex.Message
        End Try
        Return ret
    End Function
End Module

