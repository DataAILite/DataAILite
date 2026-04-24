Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.ComponentModel
Partial Class Controls_CalendarDropDown
    Inherits System.Web.UI.UserControl
#Region "Event Definitions"
    'Public Event CalendarOpened As EventHandler
    'Protected Overridable Sub OnCalendarOpened(e As EventArgs)
    '    hdnOpened.Value = "open"
    '    RaiseEvent CalendarOpened(Me, e)
    'End Sub
    'Public Event CalendarClosed As EventHandler
    'Protected Overridable Sub OnCalendarClosed(e As EventArgs)
    '    hdnOpened.Value = "closed"
    '    RaiseEvent CalendarClosed(Me, e)
    'End Sub
    Public Event DateSelectionChanged As EventHandler
    Protected Overridable Sub OnDateSelectionChanged(e As EventArgs)
        RaiseEvent DateSelectionChanged(Me, e)
    End Sub
#End Region

#Region "Public Methods"
    Public Overrides Sub Focus()
        txtDate.Focus()
    End Sub
#End Region

#Region "Private Subs/Functions"
    Private Function DateTimeIsOK(ByVal sUserInput As String) As Boolean
        Dim dtTime As DateTime

        ' check if input (after any previous massaging) is a valid date time format
        Try
            dtTime = DateTime.Parse(sUserInput)
        Catch
            Return False
        End Try
        Return True
    End Function
    Private Function TimeIsOK(ByVal sT As String) As Boolean
        Try
            Dim bRet As Boolean = False
            Dim sTime As String = sT.ToUpper
            Dim sCkTime As String = String.Empty
            Dim sS As String

            If (sTime = "NOON" Or sTime = "MID" Or sTime = "NOW" Or sTime = "N" Or Regex.IsMatch(sTime, "^N[+-]\d+[H]?$") Or Regex.IsMatch(sTime, "^NOW[+-]\d+[H]?$") Or DateTimeIsOK(sT)) Then
                Return True
            End If
            If Regex.IsMatch(sTime, "^\d{4}$") Then
                'i.e. 1100
                sCkTime = sTime.Substring(0, 2) & ":" & sTime.Substring(sTime.Length - 2, 2)
            ElseIf Regex.IsMatch(sTime, "^\d{3}$") Then
                'i.e. 100
                sCkTime = sTime.Substring(0, 1) & ":" & sTime.Substring(sTime.Length - 2, 2)
            ElseIf Regex.IsMatch(sTime, "^\d{1,2}$") Then
                'i.e 6 or 15
                If sTime.Length = 1 Then
                    sCkTime = "0:0" & sTime & " AM"
                Else
                    sCkTime = "0:" & sTime & " AM"
                End If
            ElseIf Regex.IsMatch(sTime, "^\d{1,4}[AP]$") Then
                'i.e. 1A or 10P or 100A or 1115P
                sS = sTime.Substring(sTime.Length - 1, 1)
                sT = sTime.Substring(0, sTime.Length - 1)
                If sT.Length < 3 Then
                    sT = sT & ":00"
                ElseIf sT.Length = 3 Then
                    sT = sT.Substring(0, 1) & ":" & sT.Substring(sT.Length - 2, 2)
                Else
                    sT = sT.Substring(0, 2) & ":" & sT.Substring(sT.Length - 2, 2)
                End If
                If sS = "A" Then
                    sCkTime = sT & " AM"
                Else
                    sCkTime = sT & " PM"
                End If
            ElseIf Regex.IsMatch(sTime, "^\d{1,4} [AP]$") Then
                'i.e. 1 A or 10 P or 100 A or 1115 P
                sS = sTime.Substring(sTime.Length - 1, 1)
                sT = sTime.Substring(0, sTime.Length - 2)
                If sT.Length < 3 Then
                    sT = sT & ":00"
                ElseIf sT.Length = 3 Then
                    sT = sT.Substring(0, 1) & ":" & sT.Substring(sT.Length - 2, 2)
                Else
                    sT = sT.Substring(0, 2) & ":" & sT.Substring(sT.Length - 2, 2)
                End If
                If sS = "A" Then
                    sCkTime = sT & " AM"
                Else
                    sCkTime = sT & " PM"
                End If
            ElseIf Regex.IsMatch(sTime, "^\d{1,4}AM$") Or
                   Regex.IsMatch(sTime, "^\d{1,4}PM$") Then
                'i.e. 1AM or 10PM or 100AM or 1115PM
                sS = sTime.Substring(sTime.Length - 2, 2)
                sT = sTime.Substring(0, sTime.Length - 2)
                If sT.Length < 3 Then
                    sT = sT & ":00"
                ElseIf sT.Length = 3 Then
                    sT = sT.Substring(0, 1) & ":" & sT.Substring(sT.Length - 2, 2)
                Else
                    sT = sT.Substring(0, 2) & ":" & sT.Substring(sT.Length - 2, 2)
                End If
                sCkTime = sT & " " & sS
            ElseIf Regex.IsMatch(sTime, "^\d{3,4} AM$") Or
                   Regex.IsMatch(sTime, "^\d{3,4} PM$") Then
                'i.e. 1 AM or 10 PM or 100 AM or 1115 PM

                sS = sTime.Substring(sTime.Length - 2, 2)
                sT = sTime.Substring(0, sTime.Length - 3)
                If sT.Length < 3 Then
                    sT = sT & ":00"
                ElseIf sT.Length = 3 Then
                    sT = sT.Substring(0, 1) & ":" & sT.Substring(sT.Length - 2, 2)
                Else
                    sT = sT.Substring(0, 2) & ":" & sT.Substring(sT.Length - 2, 2)
                End If

                sCkTime = sT & sS
            End If
            bRet = DateTimeIsOK(sCkTime)

            Return bRet
        Catch ex As Exception
            Return False
        End Try
    End Function
    Private Function Piece(sStr As String, Optional Delim As String = "@", Optional nStart As Integer = 1, Optional nEnd As Integer = 0) As String
        Dim sSeparator As String() = {Delim}
        Dim sRet = sStr
        Dim sPieces As String() = sStr.Split(sSeparator, StringSplitOptions.None)
        Dim len As Integer = sPieces.Length
        Dim i As Integer = 0

        If len < nStart Then _
            Return String.Empty

        If nEnd <= nStart Then
            sRet = sPieces(nStart - 1)
        Else
            If len < nEnd Then _
                nEnd = len
            sRet = ""
            For i = nStart - 1 To nEnd - 1
                If i = nStart - 1 Then
                    sRet = sPieces(i)
                Else
                    sRet &= (Delim & sPieces(i))
                End If
            Next
        End If
        Return sRet
    End Function
    Private Function ToTime(ByVal sT As String) As String

        Dim sRet As String = sT
        Dim sTime As String = sT.ToUpper
        Dim sS As String
        Dim sOp As String
        Dim nMinutes As Integer
        Dim nHours As Integer

        If DateTimeIsOK(sT) Then _
            Return Format(DateTime.Parse(sT), "h:mm tt")

        If sTime = "NOON" Then
            sRet = "12:00 PM"
        ElseIf sTime = "MID" Then
            sRet = "12:00 AM"
        ElseIf sTime = "N" Or sTime = "NOW" Then
            sRet = Format(Date.Now, "h:mm tt")
        ElseIf Regex.IsMatch(sTime, "^N[+-]\d+$") Then
            sOp = Mid(sTime, 2, 1)
            nMinutes = CType(sTime.Remove(0, 2), Integer)
            If sOp = "-" Then _
                nMinutes = nMinutes * -1
            sRet = Format(DateTime.Now.AddMinutes(nMinutes), "h:mm tt")
        ElseIf Regex.IsMatch(sTime, "^N[+-]\d+[H]$") Then
            sOp = Mid(sTime, 2, 1)
            nHours = CType(Val(sTime.Remove(0, 2)), Integer)
            If sOp = "-" Then _
                nHours = nHours * -1
            sRet = Format(DateTime.Now.AddHours(nHours), "h:mm tt")
        ElseIf Regex.IsMatch(sTime, "^NOW[+-]\d+$") Then
            sOp = Mid(sTime, 4, 1)
            nMinutes = CType(sTime.Remove(0, 4), Integer)
            If sOp = "-" Then _
                nMinutes = nMinutes * -1
            sRet = Format(DateTime.Now.AddMinutes(nMinutes), "h:mm tt")
        ElseIf Regex.IsMatch(sTime, "^NOW[+-]\d+[H]$") Then
            sOp = Mid(sTime, 4, 1)
            nHours = CType(Val(sTime.Remove(0, 4)), Integer)
            If sOp = "-" Then _
                nHours = nHours * -1
            sRet = Format(DateTime.Now.AddHours(nHours), "h:mm tt")
        ElseIf Regex.IsMatch(sTime, "^\d{4}$") Then
            'i.e. 1100
            sRet = sTime.Substring(0, 2) & ":" & sTime.Substring(sTime.Length - 2, 2)
        ElseIf Regex.IsMatch(sTime, "^\d{3}$") Then
            'i.e. 100
            sRet = sTime.Substring(0, 1) & ":" & sTime.Substring(sTime.Length - 2, 2)
        ElseIf Regex.IsMatch(sTime, "^\d(1,2)$") Then
            'i.e 6 or 15
            If sTime.Length = 1 Then
                sRet = "0:0" & sTime & " AM"
            Else
                sRet = "0:" & sTime & " AM"
            End If
        ElseIf Regex.IsMatch(sTime, "^\d{1,4}[AP]$") Then
            'i.e. 1A or 10P or 100A or 1115P
            sS = sTime.Substring(sTime.Length - 1, 1)
            sT = sTime.Substring(0, sTime.Length - 1)
            If sT.Length < 3 Then
                sT = sT & ":00"
            ElseIf sT.Length = 3 Then
                sT = sT.Substring(0, 1) & ":" & sT.Substring(sT.Length - 2, 2)
            Else
                sT = sT.Substring(0, 2) & ":" & sT.Substring(sT.Length - 2, 2)
            End If
            If sS = "A" Then
                sRet = sT & " AM"
            Else
                sRet = sT & " PM"
            End If
        ElseIf Regex.IsMatch(sTime, "^\d{1,4} [AP]$") Then
            'i.e. 1 A or 10 P or 100 A or 1115 P
            sS = sTime.Substring(sTime.Length - 1, 1)
            sT = sTime.Substring(0, sTime.Length - 2)
            If sT.Length < 3 Then
                sT = sT & ":00"
            ElseIf sT.Length = 3 Then
                sT = sT.Substring(0, 1) & ":" & sT.Substring(sT.Length - 2, 2)
            Else
                sT = sT.Substring(0, 2) & ":" & sT.Substring(sT.Length - 2, 2)
            End If
            If sS = "A" Then
                sRet = sT & " AM"
            Else
                sRet = sT & " PM"
            End If
        ElseIf Regex.IsMatch(sTime, "^\d{1,4}AM$") Or
               Regex.IsMatch(sTime, "^\d{1,4}PM$") Then
            'i.e. 1AM or 10PM or 100AM or 1115PM

            sS = sTime.Substring(sTime.Length - 2, 2)
            sT = sTime.Substring(0, sTime.Length - 2)
            If sT.Length < 3 Then
                sT = sT & ":00"
            ElseIf sT.Length = 3 Then
                sT = sT.Substring(0, 1) & ":" & sT.Substring(sT.Length - 2, 2)
            Else
                sT = sT.Substring(0, 2) & ":" & sT.Substring(sT.Length - 2, 2)
            End If
            sRet = sT & " " & sS
        ElseIf Regex.IsMatch(sTime, "^\d{3,4} AM$") Or
               Regex.IsMatch(sTime, "^\d{3,4} PM$") Then
            'i.e. 1 AM or 10 PM or 100 AM or 1115 PM

            sS = sTime.Substring(sTime.Length - 3, 3)
            sT = sTime.Substring(0, sTime.Length - 3)

            If sT.Length = 3 Then
                sT = sT.Substring(0, 1) & ":" & sT.Substring(sT.Length - 2, 2)
            Else
                sT = sT.Substring(0, 2) & ":" & sT.Substring(sT.Length - 2, 2)
            End If
            sRet = sT & sS
        End If
        If DateTimeIsOK(sRet) Then _
            Return Format(DateTime.Parse(sRet), "h:mm tt")

        Return sRet
    End Function
    Private Function ConvertToDateTime(sDateTime As String) As DateTime
        If DateTimeIsOK(sDateTime) Then _
            Return DateTime.Parse(sDateTime)

        Dim dtm As DateTime = Nothing
        Dim sD As String = Piece(sDateTime, "@", 1).ToUpper
        Dim sT As String = Piece(sDateTime, "@", 2).ToUpper
        Dim sDate As String = String.Empty
        Dim sTime As String = String.Empty
        Dim sOp As String = String.Empty
        Dim sUnit As String = String.Empty
        Dim nDays, nMinutes As Integer

        If Regex.IsMatch(sD, "^\d{2}[A-Z]{3}\d{2}$") Then
            'i.e 23JUN03
            sDate = sD.Substring(0, 2) & " " & sD.Substring(2, 3) & " " & sD.Substring(5, 2)
        ElseIf Regex.IsMatch(sD, "^\d{2}[A-Z]{3}\d{4}$") Then
            'i.e. 23JUN2003
            sDate = sD.Substring(0, 2) & " " & sD.Substring(2, 3) & " " & sD.Substring(5, 4)
        ElseIf Regex.IsMatch(sD, "^\d{1}[A-Z]{3}\d{2}$") Then
            'i.e. 5JUN03"
            sDate = sD.Substring(0, 1) & " " & sD.Substring(1, 3) & " " & sD.Substring(4, 2)
        ElseIf Regex.IsMatch(sD, "^\d{1}[A-Z]{3}\d{4}$") Then
            'i.e 5JUN2003
            sDate = sD.Substring(0, 1) & " " & sD.Substring(1, 3) & " " & sD.Substring(4, 4)
        ElseIf Regex.IsMatch(sD, "^[A-Z]{3}\d{4}$") Then
            'i.e JUN2303
            sDate = sD.Substring(0, 3) & " " & sD.Substring(3, 2) & " " & sD.Substring(5, 2)
        ElseIf Regex.IsMatch(sD, "^[A-Z]{3}\d{6}$") Then
            'i.e. JUN232003
            sDate = sD.Substring(0, 3) & " " & sD.Substring(3, 2) & " " & sD.Substring(5, 4)
        ElseIf Regex.IsMatch(sD, "^[A-Z]{3}\d{3}$") Then
            'i.e. JUN503
            sDate = sD.Substring(0, 3) & " " & sD.Substring(3, 1) & " " & sD.Substring(4, 2)
        ElseIf Regex.IsMatch(sD, "^[A-Z]{3}\d{5}$") Then
            'i.e. JUN52003
            sDate = sD.Substring(0, 3) & " " & sD.Substring(3, 1) & " " & sD.Substring(4, 4)
        ElseIf Regex.IsMatch(sD, "^\d{6}$") Then
            'i.e. 062303
            sDate = sD.Substring(0, 2) & "/" & sD.Substring(2, 2) & "/" & sD.Substring(4, 2)
        ElseIf Regex.IsMatch(sD, "^\d{8}$") Then
            'i.e 06232003
            sDate = sD.Substring(0, 2) & "/" & sD.Substring(2, 2) & "/" & sD.Substring(4, 4)
        ElseIf sD = "T" Or sD = "TODAY" Then
            sDate = Format(DateTime.Today, "M/d/yyyy")
        ElseIf sD = "NOW" Or sD = "N" Then
            sDate = Format(DateTime.Now, "M/d/yyyy")
        ElseIf sD = "MID" And sT = String.Empty Then
            sDate = Format(DateTime.Today, "M/d/yyyy")
        ElseIf sD = "NOON" And sT = String.Empty Then
            sDate = Format(DateTime.Today, "M/d/yyyy")
        ElseIf Regex.IsMatch(sD, "^NOW[+-]\d+[HDMW]$") Then
            sOp = sD.Substring(3, 1)
            sUnit = sD.Substring(sD.Length - 1, 1)
            nDays = CType(sD.Substring(4, sD.Length - 5), Integer)
            If sOp = "-" Then _
                nDays = nDays * -1
            'Begin here next time
            If sUnit = "W" Then _
                nDays = nDays * 7
            If sUnit = "M" Then
                sDate = Format(DateTime.Now.AddMonths(nDays), "M/d/yyyy")
            ElseIf sUnit = "H" Then
                sDate = Format(DateTime.Now.AddHours(nDays), "M/d/yyyy")
            Else
                sDate = Format(DateTime.Now.AddDays(nDays), "M/d/yyyy")
            End If
        ElseIf Regex.IsMatch(sD, "^NOW[+-]\d+$") Then
            sOp = sD.Substring(3, 1)
            nMinutes = CType(sD.Remove(0, 4), Integer)
            If sOp = "-" Then _
                nMinutes = nMinutes * -1
            sDate = Format(DateTime.Now.AddMinutes(nMinutes), "M/d/yyyy")
        ElseIf Regex.IsMatch(sD, "^N[+-]\d+$") Then
            sOp = sD.Substring(1, 1)
            nMinutes = CType(sD.Remove(0, 2), Integer)
            If sOp = "-" Then _
                nMinutes = nMinutes * -1
            sDate = Format(DateTime.Now.AddMinutes(nMinutes), "M/d/yyyy")
        ElseIf Regex.IsMatch(sD, "^N[+-]\d+[HDMW]$") Then
            sOp = sD.Substring(1, 1)
            sUnit = sD.Substring(sD.Length - 1, 1)
            nDays = CType(sD.Substring(2, sD.Length - 3), Integer)
            If sOp = "-" Then _
                nDays = nDays * -1
            'Begin here next time
            If sUnit = "W" Then _
                nDays = nDays * 7
            If sUnit = "M" Then
                sDate = Format(DateTime.Now.AddMonths(nDays), "M/d/yyyy")
            ElseIf sUnit = "H" Then
                sDate = Format(DateTime.Now.AddHours(nDays), "M/d/yyyy")
            Else
                sDate = Format(DateTime.Now.AddDays(nDays), "M/d/yyyy")
            End If
        ElseIf Regex.IsMatch(sD, "^[+-]\d+$") Then
            sOp = sD.Substring(0, 1)
            nDays = CType(sD.Remove(0, 1), Integer)
            If sOp = "-" Then _
                nDays = nDays * -1
            sDate = Format(DateTime.Today.AddDays(nDays), "M/d/yyyy")
        ElseIf Regex.IsMatch(sD, "^[+-]\d+[DMW]$") Then
            sOp = sD.Substring(0, 1)
            sUnit = sD.Substring(sD.Length - 1, 1)
            nDays = CType(sD.Substring(1, sD.Length - 2), Integer)
            If sOp = "-" Then _
                nDays = nDays * -1
            If sUnit = "W" Then _
                nDays = nDays * 7
            If sUnit = "M" Then
                sDate = Format(DateTime.Today.AddMonths(nDays), "M/d/yyyy")
            Else
                sDate = Format(DateTime.Today.AddDays(nDays), "M/d/yyyy")
            End If
        ElseIf Regex.IsMatch(sD, "^T[+-]\d+$") Then
            sOp = sD.Substring(1, 1)
            nDays = CType(sD.Remove(0, 2), Integer)
            If sOp = "-" Then _
                nDays = nDays * -1
            sDate = Format(DateTime.Today.AddDays(nDays), "M/d/yyyy")
        ElseIf Regex.IsMatch(sD, "^T[+-]\d+[DMW]$") Then
            sOp = Mid(sD, 2, 1)
            sOp = sD.Substring(1, 1)
            sUnit = sD.Substring(sD.Length - 1, 1)
            nDays = CType(sD.Substring(2, sD.Length - 3), Integer)
            If sOp = "-" Then _
                nDays = nDays * -1
            If sUnit = "W" Then _
                nDays = nDays * 7
            If sUnit = "M" Then
                sDate = Format(DateTime.Today.AddMonths(nDays), "M/d/yyyy")
            Else
                sDate = Format(DateTime.Today.AddDays(nDays), "M/d/yyyy")
            End If
        ElseIf Regex.IsMatch(sD, "^TODAY[+-]\d+$") Then
            sOp = sD.Substring(5, 1)
            nDays = CType(sD.Remove(0, 6), Integer)
            If sOp = "-" Then _
                nDays = nDays * -1
            sDate = Format(DateTime.Today.AddDays(nDays), "M/d/yyyy")
        ElseIf Regex.IsMatch(sD, "^TODAY[+-]\d+[DMW]$") Then
            sOp = sD.Substring(5, 1)
            sUnit = sD.Substring(sD.Length - 1, 1)
            nDays = CType(sD.Substring(2, sD.Length - 7), Integer)
            If sOp = "-" Then _
                nDays = nDays * -1
            If sUnit = "W" Then _
                nDays = nDays * 7
            If sUnit = "M" Then
                sDate = Format(DateTime.Today.AddMonths(nDays), "M/d/yyyy")
            Else
                sDate = Format(DateTime.Today.AddDays(nDays), "M/d/yyyy")
            End If
        End If
        'If sT <> String.Empty And DateTimeIsOK(sDate) Then
        '    Dim dtm1 As DateTime = DateTime.Parse(sDate)

        '    If TimeIsOK(sT) Then
        '        sT = ToTime(sT)
        '        sDate = Format(dtm1, "M/d/yyyy") & " " & sT
        '    Else
        '        sDate = String.Empty
        '    End If

        'End If
        If DateTimeIsOK(sDate) Then _
            dtm = DateTime.Parse(sDate)
        Return dtm
    End Function
#End Region

#Region "Property Definitions"
    Public Property SelectedDate As DateTime
        Get
            Return Calendar1.SelectedDate
        End Get
        Set(value As DateTime)
            Dim sValue As String = Format(value, "M/d/yyyy")
            If sValue <> "1/1/0001" Then
                Calendar1.SelectedDate = value
                Calendar1.VisibleDate = value
                txtDate.Text = Format(value, "M/d/yyyy")
            Else
                Text = ""
            End If
        End Set
    End Property
    <Browsable(False)>
    Public ReadOnly Property IsOpen As Boolean
        Get
            Return (hdnOpened.Value = "open")
        End Get
    End Property
    <Browsable(True), DefaultValue("225px")>
    Public Property Width As String
        Get
            Return pnlCalendar.Style("width")
        End Get
        Set(value As String)
            If value <> String.Empty Then
                pnlCalendar.Style("width") = value
                Dim CalWidth As Double = Val(value) - 5
                DivCalendar.Style("width") = CalWidth.ToString & "px"
            End If

        End Set
    End Property
    <Browsable(True), DefaultValue(1)>
    Public Property BorderWidth As Integer
        Get
            Dim bordwidth As String = Calendar1.Style("border-width").ToString
            If bordwidth <> String.Empty Then
                Return CInt(Val(bordwidth))
            Else
                Return 1
            End If

        End Get
        Set(value As Integer)
            Calendar1.Style("border-width") = value.ToString & "px"
            txtDate.Style("border-width") = value.ToString & "px"
            tdButton.Style("border-width") = value.ToString & "px"
        End Set
    End Property

    <Browsable(True), DefaultValue(GetType(Drawing.Color), "DarkGray")>
    Public Property BorderColor As Drawing.Color
        Get
            Return Drawing.Color.FromName(Calendar1.Style("border-color"))
        End Get
        Set(value As Drawing.Color)
            If value <> Drawing.Color.Transparent Then
                Calendar1.Style("border-color") = "rgba(" & value.R.ToString & "," & value.G.ToString & "," & value.B.ToString & "," & value.A.ToString & ")"
                txtDate.Style("border-color") = Calendar1.Style("border-color")
                tdButton.Style("border-color") = Calendar1.Style("border-color")
            End If
        End Set
    End Property

    Public Property CssClass As String
        Get
            Return Calendar1.CssClass
        End Get
        Set(value As String)
            If value <> String.Empty Then
                If Calendar1.CssClass <> value Then
                    txtDate.CssClass = value
                    Calendar1.CssClass = value
                End If
            End If
        End Set
    End Property
    Public Property FontName As String
        Get
            Return Calendar1.Font.Name
        End Get
        Set(value As String)
            If Calendar1.Font.Name <> value Then
                txtDate.Font.Name = value
                Calendar1.Font.Name = value
                Calendar1.DayStyle.Font.Name = value
                Calendar1.DayHeaderStyle.Font.Name = value
            End If
        End Set
    End Property
    Public Property FontBold As Boolean
        Get
            Return Calendar1.Font.Bold
        End Get
        Set(value As Boolean)
            If value <> Calendar1.Font.Bold Then
                txtDate.Font.Bold = value
                Calendar1.Font.Bold = value
                Calendar1.DayStyle.Font.Bold = value
            End If
        End Set
    End Property

    Public Property FontSize As FontUnit
        Get
            Return Calendar1.Font.Size
        End Get
        Set(value As FontUnit)
            If value <> Calendar1.Font.Size Then
                txtDate.Font.Size = value
                Calendar1.Font.Size = value
                Calendar1.DayStyle.Font.Size = value
                Calendar1.DayHeaderStyle.Font.Size = value
            End If
        End Set
    End Property
    Public Property ForeColor As System.Drawing.Color
        Get
            Return Calendar1.ForeColor
        End Get
        Set(value As System.Drawing.Color)
            If Not value.Equals(System.Drawing.Color.Transparent) AndAlso
               Calendar1.ForeColor <> value Then
                txtDate.ForeColor = value
                Calendar1.ForeColor = value
            End If
        End Set
    End Property
    Public Property BackColor As System.Drawing.Color
        Get
            Return Calendar1.BackColor
        End Get
        Set(value As System.Drawing.Color)
            If value.Equals(System.Drawing.Color.Transparent) Then
                txtDate.BackColor = Drawing.Color.White
                Calendar1.BackColor = txtDate.BackColor
            ElseIf value <> Calendar1.BackColor Then
                txtDate.BackColor = value
                Calendar1.BackColor = value
            End If
        End Set
    End Property
    Public Property Text As String
        Get
            Return txtDate.Text
        End Get
        Set(value As String)
            If value = "" Then
                Calendar1.SelectedDate = Nothing
                Calendar1.VisibleDate = Nothing
            Else
                If IsDate(value) Then
                    Calendar1.SelectedDate = CDate(value)
                    Calendar1.VisibleDate = CDate(value)
                Else
                    Calendar1.SelectedDate = Nothing
                    Calendar1.VisibleDate = Nothing
                    value = ""
                End If
            End If
            txtDate.Text = value
        End Set
    End Property
#End Region

#Region "Event Handlers"


    Protected Sub PageInit() Handles Me.Init
        Dim ctlID As String = Me.ClientID & "_"
        hdnOpened.Value = "closed"

        txtDate.Attributes.Add("onkeydown", "txtDateKeyDown(event,'" & ctlID & "');")
        tdButton.Attributes.Add("onclick", "OpenCalendarDropDown('" & ctlID & "');")
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            lblToday.Text = "Todays Date: " & Format(DateTime.Today, "M/d/yyyy")
        Else
            Dim CheckID As String = Me.ClientID & "_"
            If hdnOpened.Value = "open" Then
                DivCalendar.Style.Item("visibility") = "visible"
                Page.ClientScript.RegisterStartupScript(GetType(Page), "AddListener", "AddListener('" & CheckID & "');", True)
            End If
            'If Request("__EVENTARGUMENT") = "tdButton_Click" AndAlso
            '   Request("__EVENTTARGET") = CheckID Then
            '    'If hdnOpened.Value = "closed" Then _
            '    '    Page.ClientScript.RegisterStartupScript(GetType(Page), "AddListener", "AddListener('" & CheckID & "');", True)
            '    btnDate_Click(Calendar1, New EventArgs)
            'End If
        End If
    End Sub
    Protected Sub txtDate_TextChanged(sender As Object, e As EventArgs) Handles txtDate.TextChanged
        If txtDate.Text = "/" Then
            'txtDate.Text = ""
            If Calendar1.SelectedDate <> Nothing Then
                txtDate.Text = Format(Calendar1.SelectedDate, "M/d/yyyy")
            Else
                txtDate.Text = String.Empty
            End If
            txtDate.Focus()
        ElseIf txtDate.Text = String.Empty Then
            Calendar1.SelectedDate = Nothing
            Calendar1.VisibleDate = Date.Today
            SelectedDate = Nothing
        Else
            Dim dtm As DateTime = ConvertToDateTime(txtDate.Text)
            If dtm <> Nothing Then
                txtDate.Text = Format(dtm, "M/d/yyyy")
                Calendar1.SelectedDate = dtm
                Calendar1.VisibleDate = dtm
                SelectedDate = dtm
                OnDateSelectionChanged(New EventArgs)
                'If DivCalendar.Style.Item("visibility") = "visible" Then
                If hdnOpened.Value = "open" Then
                    DivCalendar.Style.Item("visibility") = "hidden"
                    TodayRow.Style.Item("visibility") = "hidden"
                    hdnOpened.Value = "closed"
                End If
            End If
        End If

    End Sub

    Protected Sub btnDate_Click(sender As Object, e As EventArgs)
        If hdnOpened.Value = "open" Then
            hdnOpened.Value = "closed"
            DivCalendar.Style.Item("visibility") = "hidden"
            TodayRow.Style.Item("visibility") = "hidden"
            txtDate.Focus()
        Else
            hdnOpened.Value = "open"
            DivCalendar.Style.Item("visibility") = "visible"
            lblToday.Text = "Todays Date: " & Format(DateTime.Today, "M/d/yyyy")
            TodayRow.Style.Item("visibility") = "visible"
            txtDate.Focus()
        End If
    End Sub
    Protected Sub btnToday_Click(sender As Object, e As EventArgs) Handles btnToday.Click
        Dim OldDate As String = txtDate.Text
        txtDate.Text = Format(DateTime.Today, "M/d/yyyy")
        DivCalendar.Style.Item("visibility") = "hidden"
        Calendar1.SelectedDate = DateTime.Today
        Calendar1.VisibleDate = DateTime.Today
        SelectedDate = Calendar1.SelectedDate
        TodayRow.Style.Item("visibility") = "hidden"
        hdnOpened.Value = "closed"
        If OldDate <> txtDate.Text Then _
            OnDateSelectionChanged(New EventArgs)
        txtDate.Focus()
    End Sub
    Protected Sub Calendar1_SelectionChanged(sender As Object, e As EventArgs) Handles Calendar1.SelectionChanged
        Dim OldDate As String = txtDate.Text
        Dim sSelected As String = Format(Calendar1.SelectedDate, "M/d/yyyy")
        If sSelected = "1/1/0001" Then _
            Return
        txtDate.Text = sSelected
        DivCalendar.Style.Item("visibility") = "hidden"
        SelectedDate = Calendar1.SelectedDate
        TodayRow.Style.Item("visibility") = "hidden"
        hdnOpened.Value = "closed"
        txtDate.Focus()
        If OldDate <> txtDate.Text Then _
            OnDateSelectionChanged(New EventArgs)
        Page.ClientScript.RegisterStartupScript(GetType(Page), "RemoveListener", "RemoveListener();", True)
    End Sub


#End Region

End Class
