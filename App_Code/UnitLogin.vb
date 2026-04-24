Imports Microsoft.VisualBasic
Imports System.Data

Public Module UnitLogin
    Function UnitAuthenticate(ByVal sUserid As String, ByVal sPasswd As String, Optional ByVal erro As String = "") As Boolean
        'call Unit authentication
        Dim UnitAuth As Boolean = False
        'example how make authentication with oracle db using LDAP
        If (sPasswd = "") Or (sUserid = "") Then
            Return False
            Exit Function
        End If
        Err.Number = 0
        Dim oProvider = GetObject("LDAP:")
        If Err.Number <> 0 Then
            MsgBox("GetObject = " & Err.Number)
            UnitAuth = False
        Else
            'Dim oUser = oProvider.OpenDSObject("LDAP://netid...../ou=UNIT/ou=NetID/ou=Accounts/uid=" & sUserid, "uid=" & sUserid & ",ou=Accounts,ou=NetID,ou=UNIT,o=Unit,c=US", sPasswd, 0)
            'or other method of Unit login authentication
            If Err.Number = 0 AndAlso erro = "" Then
                UnitAuth = True
            Else
                UnitAuth = False
            End If
        End If
        Return UnitAuth
    End Function

    Function UnitAuthorize(ByVal ps As Boolean, ByVal unit As String, ByVal tablename As String, ByVal rolefieldname As String, ByVal emailfieldname As String, ByVal logonfieldname As String, ByVal passwordfieldname As String, ByVal logon As String, Optional ByRef password As String = "", Optional ByRef userEmail As String = "") As String
        'call Unit autorization
        'ps=True if check password
        'EXAMPLE how make custom autorization using unit autorization table - tablename
        Dim Autorize As String = ""
        'autorization
        Dim listofpermits As DataView
        'user autorization
        Dim sqlst As String = String.Empty
        If ps = True Then
            sqlst = "SELECT * FROM " & tablename & " WHERE (" & logonfieldname & "='" & logon & "') And (" & passwordfieldname & " ='" & password & "')"
        Else
            sqlst = "SELECT * FROM " & tablename & " WHERE (" & logonfieldname & "='" & logon & "')"
        End If
        listofpermits = DataModule.mRecords(sqlst)
        If listofpermits Is Nothing OrElse listofpermits.Table Is Nothing OrElse listofpermits.Table.Rows.Count = 0 Then
            Return "WrongLogonPassword"
            Exit Function
        End If
        If listofpermits.Table.Rows.Count = 1 Then
            If Not IsDBNull(listofpermits.Table.Rows(0)("emailfieldname")) Then
                userEmail = listofpermits.Table.Rows(0)("emailfieldname")
            End If
            If Trim(listofpermits.Table.Rows(0)(rolefieldname)) = "admin" Then
                Autorize = "admin"
            ElseIf Trim(listofpermits.Table.Rows(0)(rolefieldname)) = "super" Then
                Autorize = "super"
            ElseIf Trim(listofpermits.Table.Rows(0)(rolefieldname)) = "user" Then
                Autorize = "user"
            Else
                Autorize = "public"
            End If
        Else
            Autorize = ""
        End If
        Return Autorize
    End Function

    'Function OURAuthenticate(ByVal sUserid As String, ByVal sPasswd As String, ByVal tablename As String, Optional ByRef issuper As String = "", Optional ByVal erro As String = "", Optional ByVal ourconstring As String = "") As Boolean
    '    Dim OURAuth As Boolean = False
    '    If (sPasswd = "") Or (sUserid = "") Then
    '        OURAuth = False
    '        Return OURAuth
    '    End If
    '    Dim mSQL As String = String.Empty
    '    Try
    '        Dim listofpermits As DataView
    '        mSQL = "SELECT * FROM " & tablename & " WHERE (Application='InteractiveReporting' AND ([NetId])='" & Trim(sUserid) & "') AND (([localpass])='" & Trim(sPasswd) & "')"
    '        listofpermits = DataModule.mRecords(mSQL, erro)
    '        If erro <> "" Then
    '            OURAuth = False
    '        Else
    '            If listofpermits.Table.Rows.Count > 0 Then
    '                OURAuth = True
    '                issuper = listofpermits.Table.Rows(0)("RoleApp")
    '            Else
    '                OURAuth = False
    '            End If
    '        End If
    '    Catch ex As Exception
    '        OURAuth = False
    '    End Try
    '    Return OURAuth
    'End Function
    'Function OURAutorize(ByVal tablename As String, ByVal logon As String, ByVal password As String, ByVal appl As String, Optional ByRef ourConnStr As String = "", Optional ByRef userConnStr As String = "", Optional ByRef userConnPrv As String = "", Optional ByRef userEmail As String = "", Optional ByRef issuper As String = "") As String
    '    Dim AutorizeApplication As String
    '    AutorizeApplication = "public"
    '    'autorization
    '    Dim listofpermits As DataView
    '    Dim pass As String = Now.ToShortDateString
    '    If logon = "super" OrElse issuper = "super" Then
    '        password = password.Replace(pass, "")
    '        listofpermits = DataModule.mRecords("SELECT * FROM " & tablename & " WHERE (NetId='" & logon & "') AND (Localpass='" & password & "') AND (Application='" & appl & "')", "", ourConnStr)
    '        If listofpermits Is Nothing OrElse listofpermits.Table Is Nothing OrElse listofpermits.Table.Rows.Count = 0 Then
    '            Return "WrongLogonPassword"
    '            Exit Function
    '        End If
    '    Else
    '        Dim userconnstrnopass As String = String.Empty
    '        If userConnStr.ToUpper.IndexOf("PASSWORD") > 0 Then
    '            userconnstrnopass = userConnStr.Substring(0, userConnStr.ToUpper.IndexOf("PASSWORD")).Trim
    '            userconnstrnopass = userconnstrnopass.Substring(0, userconnstrnopass.ToUpper.IndexOf("USER ID")).Trim
    '        End If
    '        If userConnStr.ToUpper.IndexOf("PWD") > 0 Then
    '            userconnstrnopass = userConnStr.Substring(0, userConnStr.ToUpper.IndexOf("PWD")).Trim
    '            userconnstrnopass = userconnstrnopass.Substring(0, userconnstrnopass.ToUpper.IndexOf("UID")).Trim
    '        End If
    '        Dim sqls As String = "SELECT * FROM " & tablename & " WHERE (ConnStr LIKE '" & userconnstrnopass.Trim & "%') AND (NetId='" & logon & "') AND (Localpass='" & password & "') AND (Application='" & appl & "')"
    '        listofpermits = DataModule.mRecords(sqls, "", ourConnStr)
    '        If listofpermits Is Nothing OrElse listofpermits.Table Is Nothing OrElse listofpermits.Table.Rows.Count = 0 Then
    '            Return "WrongLogonPassword"
    '            Exit Function
    '        End If
    '        If userConnStr = "" AndAlso Not IsDBNull(listofpermits.Table.Rows(0)("connstr")) Then
    '            userConnStr = listofpermits.Table.Rows(0)("connstr").ToString
    '            If userConnPrv = "" AndAlso Not IsDBNull(listofpermits.Table.Rows(0)("connprv")) Then
    '                userConnPrv = listofpermits.Table.Rows(0)("connprv").ToString
    '            End If
    '        ElseIf userConnStr = "" AndAlso IsDBNull(listofpermits.Table.Rows(0)("connstr")) Then
    '            userConnPrv = ""
    '        End If
    '    End If
    '    If Not IsDBNull(listofpermits.Table.Rows(0)("Email")) Then
    '        userEmail = listofpermits.Table.Rows(0)("Email")
    '    End If
    '    If Trim(listofpermits.Table.Rows(0)("RoleApp")).ToString = "admin" Then
    '        AutorizeApplication = "admin"
    '    ElseIf Trim(listofpermits.Table.Rows(0)("RoleApp")).ToString = "super" Then
    '        AutorizeApplication = "super"
    '    ElseIf Trim(listofpermits.Table.Rows(0)("RoleApp")).ToString = "user" Then
    '        AutorizeApplication = "user"
    '    ElseIf Trim(listofpermits.Table.Rows(0)("RoleApp")).ToString = "public" Then
    '        AutorizeApplication = "public"
    '    End If
    '    Return AutorizeApplication
    'End Function

End Module

