Imports System.Activities.Expressions
Imports System.Collections
Imports System.Data
Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Math
Imports System.Net
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports Microsoft.Data.Sqlite
Imports Microsoft.VisualBasic.DateAndTime
Imports MySql.Data.MySqlClient
Imports Mysqlx.XDevAPI.Common
Imports Newtonsoft.Json
Imports Oracle.ManagedDataAccess.Client
Public Module DataModule
    Public userdbcase As String
    'Public sqliteconn As SqliteConnection
#Region "Customize Logic Classes"
    Public Class ConditionToProcess
        Public Condition As String = String.Empty
        Public LogicalOperator As String = String.Empty
        Public GroupName As String = String.Empty
        Public IsGroup As Boolean = False
    End Class
    Public Class DeletedGroups
        Public Groups As ReportCondition()
    End Class
    Public Class ReportCondition
        Public Condition As String = String.Empty
        Public LogicalOperator As String = String.Empty
        Public GroupName As String = String.Empty
        Public ContainedBy As String = String.Empty
        Public RecordOrder As String = String.Empty
        Public ConditionId As String = String.Empty
    End Class
    Public Class TreeView
        Public CSSName As String = "gray"
        Public EditLabel As String = "false"
        Public ShowLines As String = "true"
        Public LineStyle As String = "gray"
        Public Visible As String = "true"
        Public PathSeparator As String = "\\"
        Public ToggleOnSelect As String = ""
        Public SelectedValuePath As String = String.Empty
        Public Nodes As TreeNode()
    End Class
    Public Class TreeNode
        Public Text As String = String.Empty
        Public TextColor As String = String.Empty
        Public Value As String = String.Empty
        Public ValuePath As String = String.Empty
        Public AllowKeyNavigation As String = "true"
        Public NavigationURL As String = String.Empty
        Public Target As String = String.Empty
        Public CollapsedImageURL As String = String.Empty
        Public ExpandedImageURL As String = String.Empty
        Public IsSelected As String = "false"
        Public IsExpanded As String = "false"
        Public Visible As String = "true"
        Public Level As String = String.Empty
        Public Index As String = String.Empty
        Public WordWrap As String = String.Empty
        Public Tag As String = String.Empty
        Public ConditionData As ReportCondition = Nothing
        Public ChildNodes As TreeNode()
    End Class

#End Region

#Region "Color Space Classes"
    Public Class RGB
        Private mR As Byte
        Private mG As Byte
        Private mB As Byte

        Public Sub New()
            MyBase.New
        End Sub
        Public Sub New(r As Byte, g As Byte, b As Byte)
            mR = r
            mG = g
            mB = b
        End Sub
#Region "Properties"
        Public Property R() As Byte
            Get
                Return mR
            End Get
            Set(value As Byte)
                mR = value
            End Set
        End Property
        Public Property G() As Byte
            Get
                Return mG
            End Get
            Set(value As Byte)
                mG = value
            End Set
        End Property
        Public Property B() As Byte
            Get
                Return mB
            End Get
            Set(value As Byte)
                mB = value
            End Set
        End Property
#End Region

#Region "Methods"
        Public Overloads Function Equals(rgb As RGB) As Boolean
            Return (R = rgb.R) AndAlso (G = rgb.G) AndAlso (B = rgb.B)
        End Function
#End Region

    End Class
    Public Class HSL
        Private mH As Integer
        Private mS As Single
        Private mL As Single

        Public Sub New()
            MyBase.New
        End Sub
        Public Sub New(h As Integer, s As Single, l As Single)
            mH = h
            mS = s
            mL = l
        End Sub
        Public Property H() As Integer
            Get
                Return mH
            End Get
            Set(value As Integer)
                mH = value
            End Set
        End Property

        Public Property S() As Single
            Get
                Return mS
            End Get
            Set(value As Single)
                mS = value
            End Set
        End Property

        Public Property L() As Single
            Get
                Return mL
            End Get
            Set(value As Single)
                mL = value
            End Set
        End Property

        Public Overloads Function Equals(hsl As HSL) As Boolean
            Return (H = hsl.H) AndAlso (S = hsl.S) AndAlso (L = hsl.L)
        End Function
    End Class
#End Region


#Region "Report View Classes"
    Public Class ReportItem
        Public TabularColumnWidth As String
        Public Caption As String
        Public CaptionHeight As String
        Public CaptionWidth As String
        Public CaptionX As String
        Public CaptionY As String
        Public CaptionFontName As String
        Public CaptionFontSize As String
        Public CaptionForeColor As String
        Public CaptionBackColor As String 'white' <----- add

        Public CaptionBorderColor As String 'lightgray'  <----- add
        Public CaptionBorderStyle As String 'Solid' <----- add
        Public CaptionBorderWidth As String '1 (pixel)' <----- add

        Public CaptionFontStyle As String 'regular', <----- add
        Public CaptionUnderline As Boolean ' false, <----- add
        Public CaptionStrikeout As Boolean ' false, <----- add
        Public CaptionTextAlign As String  'Left',  <----- add
        Public ReportItemType As String
        Public ImagePath As String
        Public ImageHeight As String
        Public ImageWidth As String
        Public ImageData As String
        'Public ImageType As String
        Public FieldLayout As String
        Public SQLTable As String
        Public SQLDatabase As String
        Public SQLField As String
        Public SQLDataType As String
        Public ItemID As String
        Public ItemOrder As String
        Public DataHeight As String
        Public DataWidth As String
        Public DataX As String
        Public DataY As String
        Public Height As String
        Public Width As String
        Public X As String
        Public Y As String
        Public FontName As String
        Public FontSize As String
        Public ForeColor As String
        Public FontStyle As String 'regular', <----- add
        Public Underline As Boolean ' false, <----- add
        Public Strikeout As Boolean ' false, <----- add 
        Public TextAlign As String  'Left',  <----- add
        Public BackColor As String 'white' <----- add

        Public BorderColor As String 'black'  <----- add
        Public BorderStyle As String 'None' <----- add
        Public BorderWidth As String '1 (pixel)' <----- add

        Public Section As String
    End Class
    Public Class ReportView
        Public ReportID As String
        Public ReportTemplate As String
        Public Title As String
        Public Orientation As String 'portrait', <----- add
        Public ReportFieldLayout As String 'Block', <----- add
        Public HeaderHeight As String '1 in <------ add
        Public HeaderBackColor As String 'white <------ add
        Public HeaderBorderColor As String 'lightgrey <------ add
        Public HeaderBorderStyle As String 'Solid <------ add
        Public HeaderBorderWidth As String '1 (pixel) <------ add

        Public FooterHeight As String '1 in <------ add
        Public FooterBackColor As String 'white <------ add
        Public FooterBorderColor As String 'lightgrey <------ add
        Public FooterBorderStyle As String 'Solid <------ add
        Public FooterBorderWidth As String '1 (pixel) <------ add

        Public DataFontName As String
        Public DataFontSize As String
        Public DataForeColor As String
        Public DataBackColor As String 'white' <----- add
        Public DataBorderColor As String 'lightgray'  <----- add
        Public DataBorderStyle As String 'Solid' <----- add
        Public DataBorderWidth As String '1 (pixel)' <----- add

        Public DataFontStyle As String 'regular', <----- add
        Public DataUnderline As Boolean ' false, <----- add
        Public DataStrikeout As Boolean ' false, <----- add

        Public ReportDetailAlign As String 'Left

        Public LabelFontName As String
        Public LabelFontSize As String
        Public LabelForeColor As String
        Public LabelBackColor As String 'white' <----- add
        Public LabelBorderColor As String 'lightgray'  <----- add
        Public LabelBorderStyle As String 'Solid' <----- add
        Public LabelBorderWidth As String '1 (pixel)' <----- add

        Public LabelFontStyle As String 'regular', <----- add
        Public LabelUnderline As Boolean ' false, <----- add
        Public LabelStrikeout As Boolean ' false, <----- add

        Public ReportCaptionAlign As String 'Left

        Public MarginBottom As String '0.5
        Public MarginLeft As String   '0.25
        Public MarginRight As String  '0.25
        Public MarginTop As String    '0.5

        'Public CreatedBy As String
        'Public DateCreated As String
        'Public LastUpdate As String
        'Public UpdateBy As String

        Public Items As ReportItem()


        Public Function Exists(id As String, tbl As String, fld As String) As Boolean
            Dim bRet As Boolean = False
            Dim itm As ReportItem
            If Me.Items.Count > 0 Then
                For i As Integer = 0 To Me.Items.Count - 1
                    itm = Me.Items(i)
                    If itm.ItemID = id Then
                        bRet = True
                        Exit For
                    End If
                Next
                If tbl <> String.Empty AndAlso fld <> String.Empty Then
                    For i As Integer = 0 To Me.Items.Count - 1
                        itm = Me.Items(i)
                        If itm.SQLTable = tbl AndAlso itm.SQLField = fld Then
                            bRet = True
                            Exit For
                        End If
                    Next
                End If
            End If
            Return bRet
        End Function

        Public Function FindItem(id As String, tbl As String, fld As String) As ReportItem
            Dim ret As ReportItem = Nothing
            Dim bRet As Boolean = False
            Dim itm As ReportItem = Nothing

            If Me.Items.Count > 0 Then
                For i As Integer = 0 To Me.Items.Count - 1
                    itm = Me.Items(i)
                    If itm.ItemID = id Then
                        bRet = True
                        Exit For
                    End If
                Next
                If tbl <> String.Empty AndAlso fld <> String.Empty Then
                    For i As Integer = 0 To Me.Items.Count - 1
                        itm = Me.Items(i)
                        If itm.SQLTable = tbl AndAlso itm.SQLField = fld Then
                            bRet = True
                            Exit For
                        End If
                    Next
                End If
                If bRet Then ret = itm
            End If
            Return ret
        End Function
    End Class

    Public Class FontItem
        Public name As String
        Public boldAvailable As Boolean
        Public italicAvailable As Boolean
        Public regularAvailable As Boolean
        Public strikeoutAvailable As Boolean
        Public underlineAvailable As Boolean
    End Class

    Public Class FontData
        Public Items As FontItem()
    End Class

#End Region

#Region "Field Order Classes"
    Public Class OrderItem
        Public Field As String
        Public ItemOrder As String
    End Class

    Public Class FieldOrder
        Public OrderItems As OrderItem()
        Public Function Exists(Field As String) As Boolean
            Dim bRet As Boolean = False
            Dim itm As OrderItem
            If Me.OrderItems.Count > 0 Then
                For i As Integer = 0 To Me.OrderItems.Count - 1
                    itm = Me.OrderItems(i)
                    If itm.Field = Field Then
                        bRet = True
                        Exit For
                    End If
                Next
            End If
            Return bRet
        End Function
    End Class
#End Region

    Public Function GetDataBase(userconstr As String, Optional ByVal userconprv As String = "") As String
        Dim ret As String = "DB"
        If userconstr.ToUpper.IndexOf("NAMESPACE") >= 0 Then
            Dim items As String() = userconstr.Split(";")
            For i As Integer = 0 To items.Count - 1
                If items(i).ToUpper.Contains("NAMESPACE") Then
                    ret = Piece(items(i), "=", 2).Trim
                    Exit For
                End If
            Next
        ElseIf userconstr.ToUpper.IndexOf("DATABASE") >= 0 Then
            Dim items As String() = userconstr.Split(";")
            For i As Integer = 0 To items.Count - 1
                If items(i).ToUpper.Contains("DATABASE") Then
                    ret = Piece(items(i), "=", 2).Trim
                    Exit For
                End If
            Next
        ElseIf userconstr.ToUpper.IndexOf("DATA SOURCE") >= 0 Then
            Dim items As String() = userconstr.Split(";")
            If userconprv = "Oracle.ManagedDataAccess.Client" Then
                ret = GetUserIDFromConnectionString(userconstr)  '.Replace("C##", "")
                Return ret
            End If
            'For i As Integer = 0 To items.Count - 1
            'If items(i).ToUpper.Contains("USER ID") Then
            '    ret = Piece(items(i), "=", 2).Trim
            'ret = ret.ToUpper.Replace("C##", "")
            'ret = Piece(ret, "/", 2).Trim.Replace(";", "")
            'ret = ret.Replace("PDB1", "")
            'Exit For
            'Return ret
            '    End If
            'Next
            ret = items(0)
            Return ret
            If userconstr.IndexOf("/") > 0 Then
                ret = userconstr.Substring(userconstr.LastIndexOf("/"))
                ret = ret.Replace(".", "_").Replace(";", "").Replace("/", "")
                Return ret
            End If
        ElseIf userconstr.ToUpper.IndexOf("DSN") >= 0 Then
            Dim items As String() = userconstr.Split(";")
            For i As Integer = 0 To items.Count - 1
                If items(i).ToUpper.Contains("DSN") Then
                    ret = Piece(items(i), "=", 2).Trim
                    Exit For
                End If
            Next
        End If
        If userconprv <> "Oracle.ManagedDataAccess.Client" Then
            ret = FixReservedWords(ret, userconprv)
        End If
        Return ret
    End Function

    Public Function SetFieldOrder(ByVal repid As String) As FieldOrder
        Dim ret As FieldOrder = Nothing
        Dim dtf As DataTable = Nothing
        Dim dr As DataRow

        dtf = GetReportFields(repid)
        If dtf IsNot Nothing AndAlso dtf.Rows.Count > 0 Then
            Dim FldOrder As New FieldOrder
            Dim itm As OrderItem
            ReDim FldOrder.OrderItems(-1)
            Dim j As Integer = 1
            Dim val As String
            For i As Integer = 0 To dtf.Rows.Count - 1
                dr = dtf.Rows(i)
                val = dr("Val")
                itm = New OrderItem
                itm.Field = val
                itm.ItemOrder = j.ToString
                ReDim Preserve FldOrder.OrderItems(j - 1)
                FldOrder.OrderItems(j - 1) = itm
                j += 1
            Next
            ret = FldOrder 'JsonConvert.SerializeObject(FldOrder)
        End If
        Return ret
    End Function

    Public Function TemplateFunction(ByVal rep As String) As String
        Dim ret As String = String.Empty
        Try

        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function AddDataTableIntoDBTable(ByVal dt As DataTable, ByVal Tbl As String) As String
        Dim ret As String = String.Empty
        Dim i, j As Integer
        Dim msql As String = String.Empty
        Dim mfields As String = String.Empty
        Dim mvalues As String = String.Empty
        Dim mval As String = String.Empty
        Try
            'mfields
            For j = 0 To dt.Columns.Count - 1
                If dt.Columns(j).Caption.ToUpper <> "INDX" AndAlso dt.Columns(j).Caption.ToUpper <> "IDX" AndAlso dt.Columns(j).Caption.ToUpper <> "ID" Then
                    If mfields = "" Then
                        'mfields = "[" & dt.Columns(j).Caption & "]"
                        mfields = FixReservedWords(dt.Columns(j).Caption)
                    Else
                        mfields = mfields & "," & FixReservedWords(dt.Columns(j).Caption)
                    End If
                End If
            Next
            For i = 0 To dt.Rows.Count - 1
                'Make insert statement
                mvalues = ""
                msql = "INSERT INTO " & Tbl & " (" & mfields & ") VALUES ("
                For j = 0 To dt.Columns.Count - 1
                    If dt.Columns(j).Caption.ToUpper <> "INDX" AndAlso dt.Columns(j).Caption.ToUpper <> "IDX" AndAlso dt.Columns(j).Caption.ToUpper <> "ID" Then
                        If IsDBNull(dt.Rows(i)(dt.Columns(j).Caption)) Then
                            mval = "NULL"
                        Else
                            mval = dt.Rows(i)(dt.Columns(j).Caption).ToString
                            If dt.Columns(j).DataType.FullName = "System.String" OrElse dt.Columns(j).DataType.FullName = "System.DateTime" OrElse dt.Columns(j).DataType.FullName = "MySql.Data.Types.MySqlDateTime" Then
                                mval = "'" & mval & "'"
                            End If
                        End If
                        If mvalues = "" Then
                            mvalues = mval
                        Else
                            mvalues = mvalues & "," & mval
                        End If
                    End If
                Next
                msql = msql & mvalues & ")"
                ret = ExequteSQLquery(msql)
            Next
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function AddRowIntoTable(ByVal mRow As DataRow, ByVal Tbl As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "") As String
        'Tbl should have a primary key field to make update possible !!!!!!!!!!!!
        Dim ret As String = String.Empty
        Dim myRecords As DataTable = New DataTable
        Dim myconstring, myprovider As String
        Dim dbcase As String = String.Empty
        Try
            If userconstr.Trim = "" Then
                myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
                myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                If myprovider = "Npgsql" Then dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
            Else
                If userconprv = "Npgsql" Then
                    If userconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("csvdbcase").ToString
                    ElseIf userconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
                    Else 'userdbcase
                        dbcase = userdbcase
                    End If
                End If
                myconstring = userconstr
                myprovider = userconprv
            End If
            If myprovider = "InterSystems.Data.CacheClient" Then
                ret = AddRowIntoTable_Cache(mRow, Tbl, myconstring)

            ElseIf myprovider = "InterSystems.Data.IRISClient" Then
                ret = AddRowIntoTable_IRIS(mRow, Tbl, myconstring)

            ElseIf myprovider = "MySql.Data.MySqlClient" Then
                Dim cmdBuilder As MySqlCommandBuilder
                Dim myConnection As MySqlConnection
                Dim myCommand As New MySqlCommand
                Dim myAdapter As MySqlDataAdapter
                myConnection = New MySqlConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New MySqlDataAdapter(myCommand)
                '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                Dim db As String = GetDataBase(myconstring, myprovider)
                myCommand.CommandText = "SELECT * FROM `" & db & "`.`" & Tbl.ToLower & "` "
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New MySqlDataAdapter(myCommand)
                cmdBuilder = New MySqlCommandBuilder(myAdapter)     '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Fill(myRecords)
                'add row
                myRecords.ImportRow(mRow)                         '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Update(myRecords)                       '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()

                'for Oracle - to test
            ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then
                ret = AddRowIntoTable_Oracle(mRow, Tbl, myconstring)

            ElseIf myprovider = "System.Data.Odbc" Then
                myconstring = myconstring.Replace("Password", "Pwd").Replace("User ID", "UID")
                Dim cmdBuilder As System.Data.Odbc.OdbcCommandBuilder
                Dim myConnection As System.Data.Odbc.OdbcConnection
                Dim myCommand As New System.Data.Odbc.OdbcCommand
                myConnection = New System.Data.Odbc.OdbcConnection(myconstring)
                Dim myAdapter As System.Data.Odbc.OdbcDataAdapter
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = "Select * FROM " & Tbl
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New System.Data.Odbc.OdbcDataAdapter(myCommand)
                cmdBuilder = New System.Data.Odbc.OdbcCommandBuilder(myAdapter)     '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                'add row
                myRecords.ImportRow(mRow)                         '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Update(myRecords)                       '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()

            ElseIf myprovider = "System.Data.OleDb" Then
                myconstring = ConfigurationManager.AppSettings("ACEOLEDBversion").ToString & "Persist Security Info=True;" & myconstring
                Dim cmdBuilder As System.Data.OleDb.OleDbCommandBuilder
                Dim myConnection As System.Data.OleDb.OleDbConnection
                Dim myCommand As New System.Data.OleDb.OleDbCommand
                myConnection = New System.Data.OleDb.OleDbConnection(myconstring)
                Dim myAdapter As System.Data.OleDb.OleDbDataAdapter
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = "Select * FROM " & Tbl
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New System.Data.OleDb.OleDbDataAdapter(myCommand)
                cmdBuilder = New System.Data.OleDb.OleDbCommandBuilder(myAdapter)     '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                'add row
                myRecords.ImportRow(mRow)                         '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Update(myRecords)                       '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()

            ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
                myconstring = CorrectConnstringForPostgres(myconstring, dbcase)
                Dim msql As String = "Select * FROM [" & Tbl & "]"
                msql = ConvertFromSqlServerToPostgres(msql, dbcase, myconstring, myprovider)
                Dim cmdBuilder As Npgsql.NpgsqlCommandBuilder
                Dim myConnection As Npgsql.NpgsqlConnection
                Dim myCommand As New Npgsql.NpgsqlCommand
                myConnection = New Npgsql.NpgsqlConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = msql
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                Dim myAdapter As New Npgsql.NpgsqlDataAdapter(myCommand)
                cmdBuilder = New Npgsql.NpgsqlCommandBuilder(myAdapter)     '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Fill(myRecords)
                'add row
                myRecords.ImportRow(mRow)                         '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Update(myRecords)                       '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()

                'ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
                '    myconstring = CorrectConnstringForPostgres(myconstring)
                '    Dim cmdBuilder As Npgsql.NpgsqlCommandBuilder                  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                '    Dim myAdapter As Npgsql.NpgsqlDataAdapter
                '    Dim myConnection As Npgsql.NpgsqlConnection
                '    Dim myCommand As New Npgsql.NpgsqlCommand
                '    myConnection = New Npgsql.NpgsqlConnection(myconstring)
                '    myCommand.Connection = myConnection
                '    myCommand.CommandType = CommandType.Text
                '    myCommand.CommandTimeout = 300000
                '    myCommand.CommandText = "Select * FROM " & Tbl
                '    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                '    myAdapter = New Npgsql.NpgsqlDataAdapter(myCommand)
                '    cmdBuilder = New Npgsql.NpgsqlCommandBuilder(myAdapter)     '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                '    myAdapter.Fill(myRecords)
                '    'add row
                '    myRecords.ImportRow(mRow)                         '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                '    myAdapter.Update(myRecords)                       '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                '    myAdapter.Dispose()
                '    myCommand.Connection.Close()
                '    myCommand.Dispose()

            Else  'SQL Server client
                Dim cmdBuilder As SqlCommandBuilder                  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                Dim myConnection As SqlConnection
                Dim myCommand As New SqlClient.SqlCommand
                Dim myAdapter As SqlClient.SqlDataAdapter
                myConnection = New SqlConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = "Select * FROM " & Tbl
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New SqlClient.SqlDataAdapter(myCommand)
                cmdBuilder = New SqlCommandBuilder(myAdapter)     '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Fill(myRecords)
                'add row
                myRecords.ImportRow(mRow)                         '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Update(myRecords)                       '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function LoadDataTableIntoDbTable(ByVal dtt As DataTable, ByVal Tbl As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "") As String
        'Tbl should have a primary key field to make update possible !!!!!!!!!!!!
        Dim ret As String = String.Empty
        Dim i As Integer = 0
        Dim myRecords As DataTable = New DataTable
        Dim myconstring, myprovider As String
        Dim dbcase As String = String.Empty
        Try
            If userconstr.Trim = "" Then
                myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
                myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                If myprovider = "Npgsql" Then dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
            Else
                If userconprv = "Npgsql" Then
                    If System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing AndAlso userconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("csvdbcase").ToString
                    ElseIf userconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
                    Else 'userdbcase
                        dbcase = userdbcase
                    End If
                End If
                myconstring = userconstr
                myprovider = userconprv
            End If
            If myprovider = "InterSystems.Data.CacheClient" Then
                ret = LoadDataTableIntoDbTable_Cache(dtt, Tbl, myconstring)

            ElseIf myprovider = "InterSystems.Data.IRISClient" Then
                ret = LoadDataTableIntoDbTable_IRIS(dtt, Tbl, myconstring)

            ElseIf myprovider = "MySql.Data.MySqlClient" Then
                Dim cmdBuilder As MySqlCommandBuilder
                Dim myConnection As MySqlConnection
                Dim myCommand As New MySqlCommand
                Dim myAdapter As MySqlDataAdapter
                myConnection = New MySqlConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New MySqlDataAdapter(myCommand)
                '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                Dim db As String = GetDataBase(myconstring, myprovider)
                myCommand.CommandText = "SELECT * FROM `" & db & "`.`" & Tbl.ToLower & "` "
                '= "Select * FROM " & Tbl.ToLower
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New MySqlDataAdapter(myCommand)
                cmdBuilder = New MySqlCommandBuilder(myAdapter)     '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Fill(myRecords)
                'add row
                For i = 0 To dtt.Rows.Count - 1
                    myRecords.ImportRow(dtt.Rows(i))
                Next
                myAdapter.Update(myRecords)                       '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()

                'TODO for Oracle - to test
            ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then
                ret = LoadDataTableIntoDbTable_Oracle(dtt, Tbl, myconstring)

            ElseIf myprovider = "System.Data.Odbc" Then
                myconstring = myconstring.Replace("Password", "Pwd").Replace("User ID", "UID")
                Dim cmdBuilder As System.Data.Odbc.OdbcCommandBuilder
                Dim myConnection As System.Data.Odbc.OdbcConnection
                Dim myCommand As New System.Data.Odbc.OdbcCommand
                Dim myAdapter As System.Data.Odbc.OdbcDataAdapter
                myConnection = New System.Data.Odbc.OdbcConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = "Select * FROM " & Tbl
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New System.Data.Odbc.OdbcDataAdapter(myCommand)
                cmdBuilder = New System.Data.Odbc.OdbcCommandBuilder(myAdapter)     '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Fill(myRecords)
                'add row
                For i = 0 To dtt.Rows.Count - 1
                    myRecords.ImportRow(dtt.Rows(i))
                Next
                myAdapter.Update(myRecords)                       '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()

            ElseIf myprovider = "System.Data.OleDb" Then
                myconstring = ConfigurationManager.AppSettings("ACEOLEDBversion").ToString & "Persist Security Info=True;" & myconstring
                Dim cmdBuilder As System.Data.OleDb.OleDbCommandBuilder
                Dim myConnection As System.Data.OleDb.OleDbConnection
                Dim myCommand As New System.Data.OleDb.OleDbCommand
                Dim myAdapter As System.Data.OleDb.OleDbDataAdapter
                myConnection = New System.Data.OleDb.OleDbConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = "Select * FROM " & Tbl
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New System.Data.OleDb.OleDbDataAdapter(myCommand)
                cmdBuilder = New System.Data.OleDb.OleDbCommandBuilder(myAdapter)     '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Fill(myRecords)
                'add row
                For i = 0 To dtt.Rows.Count - 1
                    myRecords.ImportRow(dtt.Rows(i))
                Next
                myAdapter.Update(myRecords)                       '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()

            ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
                myconstring = CorrectConnstringForPostgres(myconstring, dbcase)
                Dim msql As String = "Select * FROM [" & Tbl & "]"
                msql = ConvertFromSqlServerToPostgres(msql, dbcase, myconstring, myprovider)
                Dim cmdBuilder As Npgsql.NpgsqlCommandBuilder
                Dim myConnection As Npgsql.NpgsqlConnection
                Dim myCommand As New Npgsql.NpgsqlCommand
                myConnection = New Npgsql.NpgsqlConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = msql
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                Dim myAdapter As New Npgsql.NpgsqlDataAdapter(myCommand)
                cmdBuilder = New Npgsql.NpgsqlCommandBuilder(myAdapter)     '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Fill(myRecords)
                'add row
                For i = 0 To dtt.Rows.Count - 1
                    myRecords.ImportRow(dtt.Rows(i))
                Next
                myAdapter.Update(myRecords)                       '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()

            Else  'SQL Server client
                Dim cmdBuilder As SqlCommandBuilder                  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                Dim myConnection As SqlConnection
                Dim myCommand As New SqlClient.SqlCommand
                Dim myAdapter As SqlClient.SqlDataAdapter
                myConnection = New SqlConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = "Select * FROM " & Tbl
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New SqlClient.SqlDataAdapter(myCommand)
                cmdBuilder = New SqlCommandBuilder(myAdapter)     '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Fill(myRecords)
                'add row
                For i = 0 To dtt.Rows.Count - 1
                    myRecords.ImportRow(dtt.Rows(i))
                Next
                myAdapter.Update(myRecords)                       '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function HasSQLData(rep As String) As Boolean
        Dim sqls As String = "SELECT * FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND DOING = 'SELECT'"
        If Not mRecords(sqls) Is Nothing AndAlso Not mRecords(sqls).Table Is Nothing AndAlso mRecords(sqls).Table.Rows.Count > 0 Then
            Return True
        End If
        Return False
    End Function
    Public Function HasReportColumns(rep As String) As Boolean
        Dim dt As DataTable = GetReportFields(rep)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Return True
        End If
        'Dim sqls As String = "SELECT * FROM OURReportFormat WHERE ReportId = '" & rep & "'"
        'If mRecords(sqls).table.rows.count > 0 Then
        '    Return True
        'End If
        Return False
    End Function

    Public Function MakeSQLQueryFromDB(ByVal rep As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "") As String
        Dim ret As String = "SELECT "
        Dim dr As DataTable = GetReportInfo(rep)
        Try
            If dr.Rows.Count > 0 Then
                If Not dr Is Nothing AndAlso dr.Rows.Count > 0 AndAlso Not IsDBNull(dr.Rows(0)("Param6type")) AndAlso dr.Rows(0)("Param6type").ToString.ToUpper = "TRUE" Then
                    ret = ret & "DISTINCT "
                End If
                If dr.Rows(0)("Param8type").ToString.ToUpper = "USESQLTEXT" Then
                    ret = dr.Rows(0)("SQLquerytext").ToString
                    Exit Try
                End If
            End If
            'SELECT
            Dim i, j, n, m As Integer
            Dim tbl As String = String.Empty
            Dim tblf As String = String.Empty
            Dim tbld As String = String.Empty
            Dim tbs As String = String.Empty
            Dim dtf As DataTable
            Dim fldf As String = String.Empty
            Dim fldd As String = String.Empty
            Dim selflds As String = String.Empty
            If Not HasSQLData(rep) Then
                selflds = GetListOfSelectedFieldsFromSQLquery(rep)
                If selflds = "" Then
                    selflds = "*"
                Else
                    selflds = FixSelectedFields(rep, selflds, userconstr, userconprv)
                End If
                ret = ret & " " & selflds
            End If

            'fixing field names
            Dim dt As DataTable = GetReportTables(rep, userconstr, userconprv)
            If dt Is Nothing Then
                Return ret
            End If
            If dt.Rows.Count > 0 Then
                'loop for tables
                For i = 0 To dt.Rows.Count - 1
                    tbl = dt.Rows(i)("Tbl1").ToString
                    If userconprv.StartsWith("InterSystems.Data.") Then
                        tbl = CorrectTableNameWithDots(tbl, userconprv)  'replace dots with underscore but last one
                        tbld = FixReservedWords(tbl).ToString 'return dots
                        tbld = CorrectTableNameWithDots(tbld, userconprv)  'replace dots with underscore but last one
                        'ElseIf userconprv = "Npgsql" Then
                        '    'TODO for PostgreSQL

                    Else
                        'TODO other providers , check if it is right for SQL Server and MySql !!! or something else needed?
                        tbl = CorrectTableNameWithDots(tbl, userconprv)  'replace dots with underscore but last one
                        tbld = FixReservedWords(tbl).ToString 'return dots
                        tbld = CorrectTableNameWithDots(tbld, userconprv)  'replace dots with underscore but last one if needed
                    End If
                    If HasSQLData(rep) Then
                        'check if select * from ...
                        n = GetListOfTableColumns(tbl, userconstr, userconprv).Count  'properties in table/class of user db
                        m = GetSQLTableFields(rep, tbl).Rows.Count  'selected fields to SQL query
                        If m = 0 Then
                            m = GetSQLTableFields(rep, tbl.Replace("_", ".")).Rows.Count
                        End If
                        If n = m Then  'all fields in tbl are selected to SQL query
                            If i > 0 Then ret = ret & ", "
                            ret = ret & tbld & ".*"
                        ElseIf (m = 1 AndAlso selflds = "ID" AndAlso userconprv.StartsWith("InterSystems.Data.")) Then 'no fields are in OURSqlQuery yet
                            Dim dft As DataView = GetListOfTableColumns(tbl, userconstr, userconprv)
                            selflds = ""
                            For j = 0 To dft.Table.Rows.Count - 1
                                selflds = dft.Table.Rows(j)(0)
                            Next
                            'add table name:
                            selflds = FixSelectedFields(rep, selflds, userconstr, userconprv)
                            If selflds <> String.Empty Then
                                ret = ret & " " & selflds
                            End If

                        ElseIf m = 0 Then 'no fields are in OURSqlQuery yet
                            selflds = GetListOfSelectedFieldsFromSQLquery(rep)
                            'add table name:
                            selflds = FixSelectedFields(rep, selflds, userconstr, userconprv)
                            If selflds <> String.Empty Then
                                ret = ret & " " & selflds
                            End If
                        Else
                            'dtf = GetSQLTableFields(rep, tbl.Replace("_", "."))
                            dtf = GetSQLTableFields(rep, tbl)
                            If dtf Is Nothing OrElse dtf.Rows.Count = 0 Then
                                dtf = GetSQLTableFields(rep, tbl.Replace("_", "."))
                            End If
                            If dtf Is Nothing OrElse dtf.Rows.Count = 0 Then
                                Return ""
                            End If
                            If dtf.Rows(0)("Comments").ToString.ToUpper = "TRUE" Then ret = ret & " DISTINCT "
                            For j = 0 To dtf.Rows.Count - 1
                                fldf = dtf.Rows(j)("Tbl1Fld1").ToString
                                If userconprv.StartsWith("InterSystems.Data.") Then
                                    fldf = CorrectFieldNameWithDots(fldf)  'replace dots with underscore but last two
                                Else
                                    'TODO check if it is right for SQL Server and MySql !!! or something else needed?
                                    fldf = CorrectFieldNameWithDots(fldf)  'replace dots with underscore but last two
                                End If
                                If j > 0 OrElse i > 0 Then ret = ret & ", "
                                ret = ret & tbld & "." & FixReservedWords(fldf).ToString
                            Next
                        End If
                    End If

                    tbs = tbs & tbld
                    If i < dt.Rows.Count - 1 Then tbs = tbs & ","
                Next
            Else
                'Return ""
            End If

            ret = ret & " FROM "

            Dim retj As String = String.Empty
            Dim tblj1 As String = String.Empty
            Dim tblj2 As String = String.Empty
            Dim fldj1 As String = String.Empty
            Dim fldj2 As String = String.Empty
            'JOINs
            Dim dtj As DataTable = GetReportJoins(rep) 'sorted by RecOrder,Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2

            If dtj Is Nothing OrElse dtj.Rows Is Nothing OrElse dtj.Rows.Count = 0 Then
                ret = ret & tbs & " "
            Else
                'TODO - DO NOT repeat table if it alredy joined, add another one if it was not joined or add only the ON statement. !!
                For i = 0 To dtj.Rows.Count - 1
                    tblj1 = FixReservedWords(CorrectTableNameWithDots(dtj.Rows(i)("Tbl1").ToString, userconprv), userconprv)
                    tblj2 = FixReservedWords(CorrectTableNameWithDots(dtj.Rows(i)("Tbl2").ToString, userconprv), userconprv)
                    fldj1 = FixReservedWords(CorrectFieldNameWithDots(dtj.Rows(i)("Tbl1Fld1").ToString), userconprv)
                    fldj2 = FixReservedWords(CorrectFieldNameWithDots(dtj.Rows(i)("Tbl2Fld2").ToString), userconprv)
                    'If userconprv = "Npgsql" Then
                    '    tblj1 = "[" & tblj1 & "]"
                    '    tblj2 = "[" & tblj2 & "]"
                    '    fldj1 = "[" & fldj1 & "]"
                    '    fldj2 = "[" & fldj2 & "]"
                    'End If
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
                    tbs = tbs.Replace(tblj1 & ",", "")
                    tbs = tbs.Replace(tblj2 & ",", "")
                Next
            End If
            ret = ret & retj
            'WHEREs
            Dim dtw As DataTable = GetSQLConditions(rep)
            Dim oper As String = String.Empty
            Dim tbl1 As String = String.Empty
            Dim fld1 As String = String.Empty
            Dim tbl2 As String = String.Empty
            Dim fld2 As String = String.Empty
            Dim tbl3 As String = String.Empty
            Dim fld3 As String = String.Empty
            Dim val As String = String.Empty
            Dim typ As String = String.Empty

            Dim RecOrder As String = String.Empty
            Dim Logical As String = String.Empty
            Dim Group As String = String.Empty

            Dim HasNot As Boolean = False
            Dim qt As String = """"
            Dim dblqt = qt & qt

            If Not dtw Is Nothing AndAlso Not dtw.Rows Is Nothing AndAlso dtw.Rows.Count > 0 Then
                Dim myprovider As String = userconprv
                Dim qfield As String = String.Empty

                If userconstr = "" Then
                    'strConnect = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
                    myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                    'Else
                    '    strConnect = userconstr
                    '    myprovider = userconprv
                End If
                ret = ret & "  WHERE "

                Dim htGroup As New Hashtable
                Dim PrevGroup As String = String.Empty
                For i = 0 To dtw.Rows.Count - 1
                    'ret = ret & " ( "
                    oper = dtw.Rows(i)("Oper").ToString
                    tbl1 = FixReservedWords(CorrectTableNameWithDots(dtw.Rows(i)("Tbl1").ToString, userconprv), userconprv)
                    fld1 = FixReservedWords(CorrectFieldNameWithDots(dtw.Rows(i)("Tbl1Fld1").ToString), userconprv)
                    tbl2 = FixReservedWords(CorrectTableNameWithDots(dtw.Rows(i)("Tbl2").ToString, userconprv), userconprv)
                    fld2 = FixReservedWords(CorrectFieldNameWithDots(dtw.Rows(i)("Tbl2Fld2").ToString), userconprv)
                    tbl3 = FixReservedWords(CorrectTableNameWithDots(dtw.Rows(i)("Tbl3").ToString, userconprv), userconprv)
                    fld3 = FixReservedWords(CorrectFieldNameWithDots(dtw.Rows(i)("Tbl3Fld3").ToString), userconprv)
                    'If userconprv = "Npgsql" Then
                    '    tbl1 = "[" & tbl1 & "]"
                    '    tbl2 = "[" & tbl2 & "]"
                    '    tbl3 = "[" & tbl3 & "]"
                    '    fld1 = "[" & fld1 & "]"
                    '    fld1 = "[" & fld1 & "]"
                    '    fld3 = "[" & fld3 & "]"
                    'End If
                    val = dtw.Rows(i)("Comments").ToString
                    Logical = dtw.Rows(i)("Logical").ToString
                    Group = dtw.Rows(i)("Group").ToString

                    If (Group <> String.Empty) Then
                        If htGroup(Group) Is Nothing Then
                            If PrevGroup <> String.Empty Then
                                ret &= " )"
                            End If
                            PrevGroup = Group
                            htGroup(Group) = 1
                            Logical = GetConditionGroupLogical(rep, Group)
                            If i > 0 Then
                                Logical = Logical & " ("
                            Else
                                Logical = " ("
                            End If
                        End If
                    Else
                        If PrevGroup <> String.Empty Then
                            ret &= " )"
                            PrevGroup = String.Empty
                        End If

                    End If
                    If Logical = String.Empty Then Logical = "And"

                    If Logical = " (" OrElse i > 0 Then _
                        ret = ret & " " & Logical.ToUpper & " "

                    If val = dblqt Then val = ""

                    typ = dtw.Rows(i)("Type").ToString.Trim

                    HasNot = oper.ToUpper.Contains("NOT")
                    If typ = "Field" Then   'to field
                        qfield = tbl2 & "." & fld2
                        If oper.ToUpper.Contains("STARTSWITH") Then
                            If HasNot Then
                                oper = "Not Like"
                            Else
                                oper = "Like"
                            End If
                            If myprovider.StartsWith("InterSystems.Data.") Then
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
                            If myprovider.StartsWith("InterSystems.Data.") Then
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
                            If myprovider.StartsWith("InterSystems.Data.") Then
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
                            If myprovider.StartsWith("InterSystems.Data.") Then
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
                            If myprovider.StartsWith("InterSystems.Data.") Then
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
                            If myprovider.StartsWith("InterSystems.Data.") Then
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " STRING('%'," & val & ")) "
                            Else
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " CONCAT('%'," & val & ")) "
                            End If
                        Else
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & val & ") "
                        End If
                    ElseIf typ = "Static" Then   'to static
                        If TblFieldIsNumeric(tbl1, fld1, userconstr, userconprv) Then  'numeric
                            If oper.ToUpper = "IN" OrElse oper.ToUpper = "NOT IN" Then
                                ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " (" & val & ")) "
                            ElseIf oper.ToUpper.Contains("STARTSWITH") Then
                                If HasNot Then
                                    oper = "Not Like"
                                Else
                                    oper = "Like"
                                End If
                                If myprovider.StartsWith("InterSystems.Data.") Then
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
                                If myprovider.StartsWith("InterSystems.Data.") Then
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
                                If myprovider.StartsWith("InterSystems.Data.") Then
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
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '" & DateToString(CDate(val), userconprv) & "%') "
                        ElseIf oper.ToUpper.Contains("CONTAINS") Then
                            If HasNot Then
                                oper = "Not Like"
                            Else
                                oper = "Like"
                            End If
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '%" & DateToString(CDate(val), userconprv) & "%') "

                        ElseIf oper.ToUpper.Contains("ENDSWITH") Then
                            If HasNot Then
                                oper = "Not Like"
                            Else
                                oper = "Like"
                            End If
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '%" & DateToString(CDate(val), userconprv) & "') "
                        Else
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '" & DateToString(CDate(val), userconprv) & "') "
                        End If
                    ElseIf typ = "BtwFields" OrElse typ = "BtwFD1FD2" Then   'between fields  BtwFD1FD2
                        ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & tbl2 & "." & fld2 & " AND " & tbl3 & "." & fld3 & ") "
                    ElseIf typ = "BtwDates" OrElse typ = "BtwDT1DT2" Then   'between dates   BtwDT1DT2
                        ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '" & DateToString(CDate(fld2), userconprv) & "' AND '" & DateToString(CDate(fld3), userconprv) & "') "
                    ElseIf typ = "BwRD1Date2" OrElse typ = "BtwRD1DT2" Then   ' between relative date and date   BtwRD1DT2
                        ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & fld2 & " AND '" & DateToString(CDate(fld3), userconprv) & "') "
                    ElseIf typ = "BwDate1RD2" OrElse typ = "BtwDT1RD2" Then   'between date and relative date    BtwDT1RD2
                        ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '" & DateToString(CDate(fld2), userconprv) & "' AND " & fld3 & ") "
                    ElseIf typ = "BtwRD1RD2" Then   'between relative dates   BtwRD1RD2
                        ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & fld2 & " AND " & fld3 & ") "
                    ElseIf typ = "BtwFldDate" OrElse typ = "BtwFD1DT2" Then   'between field and date  BtwFD1DT2
                        ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & tbl2 & "." & fld2 & " And '" & DateToString(CDate(fld3), userconprv) & "') "
                    ElseIf typ = "BtwFldRD2" OrElse typ = "BtwFD1RD2" Then   'between field and relative date   BtwFD1RD2
                        ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & tbl2 & "." & fld2 & " AND " & fld3 & ") "
                    ElseIf typ = "BtwDateFld" OrElse typ = "BtwDT1FD2" Then   'between date and field   BtwDT1FD2
                        ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '" & DateToString(CDate(fld2), userconprv) & "' AND " & tbl3 & "." & fld3 & ") "
                    ElseIf typ = "BtwRD1Fld" OrElse typ = "BtwRD1FD2" Then   'between relative date and field  BtwRD1FD2
                        ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & fld2 & " AND " & tbl3 & "." & fld3 & ") "
                    ElseIf typ = "BtwValues" OrElse typ = "BtwST1ST2" Then   'between static values    BtwST1ST2
                        If TblFieldIsNumeric(tbl1, fld1, userconstr, userconprv) Then
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & fld2 & " AND " & fld3 & ") "
                        Else
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '" & fld2 & "' AND '" & fld3 & "') "
                        End If
                    ElseIf typ = "BtwValFld" OrElse typ = "BtwST1FD2" Then   'between static value and field BtwST1FD2
                        If TblFieldIsNumeric(tbl1, fld1, userconstr, userconprv) Then
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & fld2 & " AND " & tbl3 & "." & fld3 & ") "
                        Else
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " '" & fld2 & "' AND " & tbl3 & "." & fld3 & ") "
                        End If
                    ElseIf typ = "BtwFldVal" OrElse typ = "BtwFD1ST2" Then   'between field and static value  BtwFD1ST2
                        If TblFieldIsNumeric(tbl1, fld1, userconstr, userconprv) Then
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & tbl2 & "." & fld2 & " And " & fld3 & ") "
                        Else
                            ret = ret & " ( " & tbl1 & "." & fld1 & " " & oper & " " & tbl2 & "." & fld2 & " And '" & fld3 & "') "
                        End If

                    End If

                    If i = dtw.Rows.Count - 1 AndAlso PrevGroup <> "" Then ret &= ")"
                    'If (i < dtw.Rows.Count - 1) Then
                    '    ret = ret & " AND "
                    'End If
                Next
            End If
            'ORDER BY
            Dim dts As DataTable = GetSQLSorts(rep)
            If Not dts Is Nothing AndAlso Not dts.Rows Is Nothing AndAlso dts.Rows.Count > 0 Then
                ret = ret & " ORDER BY "
                'loop for s orts
                For i = 0 To dts.Rows.Count - 1
                    tbl1 = FixReservedWords(CorrectTableNameWithDots(dts.Rows(i)("Tbl1").ToString, userconprv), userconprv)
                    fld1 = FixReservedWords(CorrectFieldNameWithDots(dts.Rows(i)("Tbl1Fld1").ToString), userconprv)
                    'If userconprv = "Npgsql" Then
                    '    tbl1 = "[" & tbl1 & "]"
                    '    fld1 = "[" & fld1 & "]"
                    'End If
                    If i = 0 Then
                        ret = ret & " " & tbl1 & "." & fld1
                    Else
                        ret = ret & "," & tbl1 & "." & fld1
                    End If
                Next
                ret = ret & " " & dts.Rows(0)("Type").ToString
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function CorrectSQLforSQLServer(ByVal sql As String) As String
        sql = sql.Replace("OUR.HelpDesk", "OURHelpDesk")
        sql = sql.Replace("OUR.AccessLog", "OURAccessLog")
        sql = sql.Replace("OUR.Dashboards", "OURDashboards")
        sql = sql.Replace("OUR.Permits", "OURPermits")
        sql = sql.Replace("OUR.Permissions", "OURPermissions")
        sql = sql.Replace("OUR.ReportInfo", "OURReportInfo")
        sql = sql.Replace("OUR.ReportShow", "OURReportShow")
        sql = sql.Replace("OUR.ReportSQLquery", "OURReportSQLquery")
        sql = sql.Replace("OUR.ReportGroups", "OURReportGroups")
        sql = sql.Replace("OUR.ReportFormat", "OURReportFormat")
        sql = sql.Replace("OUR.ReportLists", "OURReportLists")
        sql = sql.Replace("OUR.ReportChildren", "OURReportChildren")
        sql = sql.Replace("OUR.ReportView", "OURReportView")
        sql = sql.Replace("OUR.ReportItems", "OURReportItems")
        sql = sql.Replace("OUR.FriendlyNames", "OURFriendlyNames")
        sql = sql.Replace("OUR.Files", "OURFiles")
        sql = sql.Replace("OURFILES", "OURFiles")
        sql = sql.Replace("OURFRIENDLYNAMES", "OURFriendlyNames")
        sql = sql.Replace("OURHELPDESK", "OURHelpDesk")
        sql = sql.Replace("OURACCESSLOG", "OURAccessLog")
        sql = sql.Replace("OURDASHBOARDS", "OURDashboards")
        sql = sql.Replace("OURPERMITS", "OURPermits")
        sql = sql.Replace("OURPERMISSIONS", "OURPermissions")
        sql = sql.Replace("OURREPORTINFO", "OURReportInfo")
        sql = sql.Replace("OURREPORTSHOW", "OURReportShow")
        sql = sql.Replace("OURREPORTSQLQUERY", "OURReportSQLquery")
        sql = sql.Replace("OURREPORTGROUPS", "OURReportGroups")
        sql = sql.Replace("OURREPORTFORMAT", "OURReportFormat")
        sql = sql.Replace("OURREPORTLISTS", "OURReportLists")
        sql = sql.Replace("OURREPORTCHILDREN", "OURReportChildren")
        sql = sql.Replace("OURREPORTVIEW", "OURReportView")
        sql = sql.Replace("OURREPORTITEMS", "OURReportItems")
        sql = sql.Replace("ourfiles", "OURFiles")
        sql = sql.Replace("ourfriendlynames", "OURFriendlyNames")
        sql = sql.Replace("ourhelpdesk", "OURHelpDesk")
        sql = sql.Replace("ouraccesslog", "OURAccessLog")
        sql = sql.Replace("ourdashboards", "OURDashboards")
        sql = sql.Replace("ourpermits", "OURPermits")
        sql = sql.Replace("ourpermissions", "OURPermissions")
        sql = sql.Replace("ourreportinfo", "OURReportInfo")
        sql = sql.Replace("ourreportshow", "OURReportShow")
        sql = sql.Replace("ourreportsqlquery", "OURReportSQLquery")
        sql = sql.Replace("ourreportgroups", "OURReportGroups")
        sql = sql.Replace("ourreportformat", "OURReportFormat")
        sql = sql.Replace("ourreportlists", "OURReportLists")
        sql = sql.Replace("ourreportchildren", "OURReportChildren")

        sql = sql.Replace("ourreportview", "OURReportView")
        sql = sql.Replace("ourreportitems", "OURReportItems")
        sql = sql.Replace("ourtasklistsetting", "OurTaskListSetting")
        sql = sql.Replace("OURTASKLISTSETTING", "OurTaskListSetting")
        sql = sql.Replace("OUR.TaskListSetting", "OurTaskListSetting")
        sql = sql.Replace("ourkmlhistory", "OURKMLHistory")
        sql = sql.Replace("OURKMLHISTORY", "OURKMLHistory")
        sql = sql.Replace("OUR.KMLHistory", "OURKMLHistory")

        'OURComparison
        sql = sql.Replace("OurComparison", "OURComparison")
        sql = sql.Replace("OURCOMPARISON", "OURComparison")
        sql = sql.Replace("OURcomparison", "OURComparison")
        sql = sql.Replace("ourcomparison", "OURComparison")

        sql = sql.Replace("[[", "[").Replace("]]", "]").Replace("`", "")
        Return sql
    End Function
    Public Function CorrectSQLforCache(ByVal sql As String, Optional ByVal donotreplace As String = "") As String
        sql = sql.Replace("OURHelpDesk", "OUR.HelpDesk")
        sql = sql.Replace("OURAccessLog", "OUR.AccessLog")
        sql = sql.Replace("OURDashboards", "OUR.Dashboards")
        sql = sql.Replace("OURPermits", "OUR.Permits")
        sql = sql.Replace("OURPermissions", "OUR.Permissions")
        sql = sql.Replace("OURReportInfo", "OUR.ReportInfo")
        sql = sql.Replace("OURReportShow", "OUR.ReportShow")
        sql = sql.Replace("OURReportSQLquery", "OUR.ReportSQLquery")
        sql = sql.Replace("OURReportGroups", "OUR.ReportGroups")
        sql = sql.Replace("OURReportFormat", "OUR.ReportFormat")
        sql = sql.Replace("OURReportLists", "OUR.ReportLists")
        sql = sql.Replace("OURReportChildren", "OUR.ReportChildren")
        sql = sql.Replace("OURFriendlyNames", "OUR.FriendlyNames")
        sql = sql.Replace("OURFiles", "OUR.Files")
        sql = sql.Replace("OURUnits", "OUR.Units")
        sql = sql.Replace("OURUserTables", "OUR.UserTables")
        sql = sql.Replace("OURReportView", "OUR.ReportView")
        sql = sql.Replace("OURReportItems", "OUR.ReportItems")
        sql = sql.Replace("OurHelpDesk", "OUR.HelpDesk")
        sql = sql.Replace("OurAccessLog", "OUR.AccessLog")
        sql = sql.Replace("OurDashboards", "OUR.Dashboards")
        sql = sql.Replace("OurPermits", "OUR.Permits")
        sql = sql.Replace("OurPermissions", "OUR.Permissions")
        sql = sql.Replace("OurReportInfo", "OUR.ReportInfo")
        sql = sql.Replace("OurReportShow", "OUR.ReportShow")
        sql = sql.Replace("OurReportSQLquery", "OUR.ReportSQLquery")
        sql = sql.Replace("OurReportGroups", "OUR.ReportGroups")
        sql = sql.Replace("OurReportFormat", "OUR.ReportFormat")
        sql = sql.Replace("OurReportLists", "OUR.ReportLists")
        sql = sql.Replace("OurReportView", "OUR.ReportView")
        sql = sql.Replace("OurReportItems", "OUR.ReportItems")
        sql = sql.Replace("OurReportChildren", "OUR.ReportChildren")
        sql = sql.Replace("OurFriendlyNames", "OUR.FriendlyNames")
        sql = sql.Replace("OurFiles", "OUR.Files")
        sql = sql.Replace("OurUnits", "OUR.Units")
        sql = sql.Replace("OurUserTables", "OUR.UserTables")
        sql = sql.Replace("OURUSERTABLES", "OUR.UserTables")
        sql = sql.Replace("OURUNITS", "OUR.Units")
        sql = sql.Replace("OURFILES", "OUR.Files")
        sql = sql.Replace("OURFRIENDLYNAMES", "OUR.FriendlyNames")
        sql = sql.Replace("OURHELPDESK", "OUR.HelpDesk")
        sql = sql.Replace("OURACCESSLOG", "OUR.AccessLog")
        sql = sql.Replace("OURDASHBOARDS", "OUR.Dashboards")
        sql = sql.Replace("OURPERMITS", "OUR.Permits")
        sql = sql.Replace("OURPERMISSIONS", "OUR.Permissions")
        sql = sql.Replace("OURREPORTINFO", "OUR.ReportInfo")
        sql = sql.Replace("OURREPORTSHOW", "OUR.ReportShow")
        sql = sql.Replace("OURREPORTSQLQUERY", "OUR.ReportSQLquery")
        sql = sql.Replace("OURREPORTGROUPS", "OUR.ReportGroups")
        sql = sql.Replace("OURREPORTFORMAT", "OUR.ReportFormat")
        sql = sql.Replace("OURREPORTLISTS", "OUR.ReportLists")
        sql = sql.Replace("OURREPORTCHILDREN", "OUR.ReportChildren")
        sql = sql.Replace("OURREPORTVIEW", "OUR.ReportView")
        sql = sql.Replace("OURREPORTITEMS", "OUR.ReportItems")
        sql = sql.Replace("OURAgents", "OUR.Agents")
        sql = sql.Replace("OURAGENTS", "OUR.Agents")
        sql = sql.Replace("OURActivity", "OUR.Activity")
        sql = sql.Replace("OURACTIVITY", "OUR.Activity")
        sql = sql.Replace("ourunits", "OUR.Units")
        sql = sql.Replace("ourfiles", "OUR.Files")
        sql = sql.Replace("ourfriendlynames", "OUR.FriendlyNames")
        sql = sql.Replace("ourhelpdesk", "OUR.HelpDesk")
        sql = sql.Replace("ouraccesslog", "OUR.AccessLog")
        sql = sql.Replace("ourdashboards", "OUR.Dashboards")
        sql = sql.Replace("ourpermits", "OUR.Permits")
        sql = sql.Replace("ourpermissions", "OUR.Permissions")
        sql = sql.Replace("ourreportinfo", "OUR.ReportInfo")
        sql = sql.Replace("ourreportshow", "OUR.ReportShow")
        sql = sql.Replace("ourreportsqlquery", "OUR.ReportSQLquery")
        sql = sql.Replace("ourreportgroups", "OUR.ReportGroups")
        sql = sql.Replace("ourreportformat", "OUR.ReportFormat")
        sql = sql.Replace("ourreportlists", "OUR.ReportLists")
        sql = sql.Replace("ourreportchildren", "OUR.ReportChildren")
        sql = sql.Replace("ourreportview", "OUR.ReportView")
        sql = sql.Replace("ourreportitems", "OUR.ReportItems")

        sql = sql.Replace("ourusertables", "OUR.UserTables")

        sql = sql.Replace("ourtasklistsetting", "OUR.TaskListSetting")
        sql = sql.Replace("OURTASKLISTSETTING", "OUR.TaskListSetting")
        sql = sql.Replace("OurTaskListSetting", "OUR.TaskListSetting")
        sql = sql.Replace("OURTaskListSetting", "OUR.TaskListSetting")

        sql = sql.Replace("ourkmlhistory", "OUR.KMLHistory")
        sql = sql.Replace("OURKMLHISTORY", "OUR.KMLHistory")
        sql = sql.Replace("OURKMLHistory", "OUR.KMLHistory")
        sql = sql.Replace("OurKMLHistory", "OUR.KMLHistory")
        sql = sql.Replace("OurKmlHistory", "OUR.KMLHistory")
        sql = sql.Replace("OURKmlHistory", "OUR.KMLHistory")

        sql = sql.Replace("ourscheduledreports", "OUR.ScheduledReports")
        sql = sql.Replace("OURSCHEDULEDREPORTS", "OUR.ScheduledReports")
        sql = sql.Replace("OurScheduledReports", "OUR.ScheduledReports")
        sql = sql.Replace("OURScheduledReports", "OUR.ScheduledReports")

        sql = sql.Replace("ourscheduleddownloads", "OUR.ScheduledDownloads")
        sql = sql.Replace("OURSCHEDULEDDOWNLOADS", "OUR.ScheduledDownloads")
        sql = sql.Replace("OurScheduledDownloads", "OUR.ScheduledDownloads")
        sql = sql.Replace("OURScheduledDownloads", "OUR.ScheduledDownloads")

        sql = sql.Replace("ourscheduledimports", "OUR.ScheduledImports")
        sql = sql.Replace("OURSCHEDULEDIMPORTS", "OUR.ScheduledImports")
        sql = sql.Replace("OurScheduledImports", "OUR.ScheduledImports")
        sql = sql.Replace("OURScheduledImports", "OUR.ScheduledImports")
        sql = sql.Replace("ourscheduledImports", "OUR.ScheduledImports")

        'OURComparison
        sql = sql.Replace("OURComparison", "OUR.Comparison")
        sql = sql.Replace("OurComparison", "OUR.Comparison")
        sql = sql.Replace("OURCOMPARISON", "OUR.Comparison")
        sql = sql.Replace("OURcomparison", "OUR.Comparison")
        sql = sql.Replace("ourcomparison", "OUR.Comparison")
        sql = sql.Replace("User.", "")

        sql = sql.Replace("[[", "[").Replace("]]", "]").Replace("``", "`")
        '.Replace(";", "") - no, cut last ; in the end
        If sql.EndsWith(";") Then
            sql = sql.Substring(0, sql.Length - 1)
        End If
        If sql.ToUpper.Contains(" AFTER [") Then
            sql = sql.Substring(0, sql.ToUpper.IndexOf("AFTER")).Trim
        End If
        'TODO remove fields with not SQL compatible types

        'TODO make function to  replace [] aroung reserved word to ""
        sql = sql.Replace("[Order]", """Order""").Replace("[Group]", """Group""").Replace("[User]", """User""").Replace("[Count]", """Count""").Replace("[Status]", """Status""").Replace("[Section]", """Section""")
        sql = sql.Replace("`Order`", """Order""").Replace("`Group`", """Group""").Replace("`User`", """User""").Replace("`Count`", """Count""").Replace("`Status`", """Status""").Replace("`Section`", """Section""")

        If donotreplace = "" Then sql = sql.Replace("[", "").Replace("]", "").Replace("`", "")
        Return sql
    End Function
    Public Function CorrectSQLforOracle(sql As String, ByRef myconstring As String, Optional ByVal donotreplace As String = "") As String
        sql = sql.Replace("OUR.HelpDesk", "OURHELPDESK")
        sql = sql.Replace("OUR.AccessLog", "OURACCESSLOG")
        sql = sql.Replace("OUR.Dashboards", "OURDASHBOARDS")
        sql = sql.Replace("OUR.KMLHistory", "OURKMLHISTORY")
        sql = sql.Replace("OUR.kmlhistory", "OURKMLHISTORY")
        sql = sql.Replace("OUR.Permits", "OURPERMITS")
        sql = sql.Replace("OUR.Permissions", "OURPERMISSIONS")
        sql = sql.Replace("OUR.ReportInfo", "OURREPORTINFO")
        sql = sql.Replace("OUR.ReportShow", "OURREPORTSHOW")
        sql = sql.Replace("OUR.ReportSQLquery", "OURREPORTSQLQUERY")
        sql = sql.Replace("OUR.ReportGroups", "OURREPORTGROUPS")
        sql = sql.Replace("OUR.ReportFormat", "OURREPORTFORMAT")
        sql = sql.Replace("OUR.ReportLists", "OURREPORTLISTS")
        sql = sql.Replace("OUR.ReportChildren", "OURREPORTCHILDREN")
        sql = sql.Replace("OUR.ReportView", "OURREPORTVIEW")
        sql = sql.Replace("OUR.ReportItems", "OURREPORTITEMS")
        sql = sql.Replace("OUR.FriendlyNames", "OURFRIENDLYNAMES")
        sql = sql.Replace("OUR.Files", "OURFILES")
        sql = sql.Replace("ourfiles", "OURFILES")
        sql = sql.Replace("ourfriendlynames", "OURFRIENDLYNAMES")
        sql = sql.Replace("ourhelpdesk", "OURHELPDESK")
        sql = sql.Replace("ouraccesslog", "OURACCESSLOG")
        sql = sql.Replace("ourdashboards", "OURDASHBOARDS")
        sql = sql.Replace("ourkmlhistory", "OURKMLHISTORY")
        sql = sql.Replace("ourpermits", "OURPERMITS")
        sql = sql.Replace("ourpermissions", "OURPERMISSIONS")
        sql = sql.Replace("ourreportinfo", "OURREPORTINFO")
        sql = sql.Replace("ourreportshow", "OURREPORTSHOW")
        sql = sql.Replace("ourreportsqlquery", "OURREPORTSQLQUERY")
        sql = sql.Replace("ourreportgroups", "OURREPORTGROUPS")
        sql = sql.Replace("ourreportformat", "OURREPORTFORMAT")
        sql = sql.Replace("ourreportlists", "OURREPORTLISTS")
        sql = sql.Replace("ourreportchildren", "OURREPORTCHILDREN")
        sql = sql.Replace("ourreportview", "OURREPORTVIEW")
        sql = sql.Replace("ourreportitems", "OURREPORTITEMS")
        sql = sql.Replace("OURHelpDesk", "OURHELPDESK")
        sql = sql.Replace("OURAccessLog", "OURACCESSLOG")
        sql = sql.Replace("OURDashboards", "OURDASHBOARDS")
        sql = sql.Replace("OURKMLHistory", "OURKMLHISTORY")
        sql = sql.Replace("OURPermits", "OURPERMITS")
        sql = sql.Replace("OURPermissions", "OURPERMISSIONS")
        sql = sql.Replace("OURReportInfo", "OURREPORTINFO")
        sql = sql.Replace("OURReportShow", "OURREPORTSHOW")
        sql = sql.Replace("OURReportSQLquery", "OURREPORTSQLQUERY")
        sql = sql.Replace("OURReportGroups", "OURREPORTGROUPS")
        sql = sql.Replace("OURReportFormat", "OURREPORTFORMAT")
        sql = sql.Replace("OURReportLists", "OURREPORTLISTS")
        sql = sql.Replace("OURReportChildren", "OURREPORTCHILDREN")
        sql = sql.Replace("OURReportView", "OURREPORTVIEW")
        sql = sql.Replace("OURReportItems", "OURREPORTITEMS")
        sql = sql.Replace("OURFriendlyNames", "OURFRIENDLYNAMES")
        sql = sql.Replace("OURFiles", "OURFILES")
        sql = sql.Replace("ourtasklistsetting", "OURTASKLISTSETTING")
        sql = sql.Replace("OurTaskListSetting", "OURTASKLISTSETTING")
        sql = sql.Replace("OUR.TaskListSetting", "OURTASKLISTSETTING")
        'ScheduledReports
        sql = sql.Replace("ourscheduledreports", "OURSCHEDULEDREPORTS")
        sql = sql.Replace("OurScheduledReports", "OURSCHEDULEDREPORTS")
        sql = sql.Replace("OUR.ScheduledReports", "OURSCHEDULEDREPORTS")
        'OURComparison
        sql = sql.Replace("OURComparison", "OURCOMPARISON")
        sql = sql.Replace("OurComparison", "OURCOMPARISON")
        sql = sql.Replace("OURcomparison", "OURCOMPARISON")
        sql = sql.Replace("ourcomparison", "OURCOMPARISON")
        sql = sql.Replace("[[", "[").Replace("]]", "]").Replace("``", "`")
        sql = sql.Replace("`", "")
        sql = sql.Replace("[Order]", """Order""").Replace("[Group]", """Group""").Replace("[User]", """User""").Replace("[Count]", """Count""").Replace("[Access]", """Access""").Replace("[ACCESS]", """Access""").Replace("[Start]", """Start""")
        sql = sql.Replace("`Order`", """Order""").Replace("`Group`", """Group""").Replace("`User`", """User""").Replace("`Count`", """Count""").Replace("`Access`", """Access""").Replace("`ACCESS`", """Access""").Replace("`Start`", """Start""")
        'replace [] aroung reserved word to ""
        'If donotreplace = "" Then sql = sql.Replace("[", """").Replace("]", """")
        If donotreplace = "" Then sql = sql.Replace("[", "").Replace("]", "")
        Return sql
    End Function

    Public Function CorrectSQLforMySql(ByVal sql As String, ByRef myconstring As String, Optional ByVal donotreplace As String = "") As String
        sql = sql.Replace("OURHelpDesk", "ourhelpdesk")
        sql = sql.Replace("OURAccessLog", "ouraccesslog")
        sql = sql.Replace("OURDashboards", "ourdashboards")
        sql = sql.Replace("OURKMLHistory", "ourkmlhistory")
        sql = sql.Replace("OURPermits", "ourpermits")
        sql = sql.Replace("OURPermissions", "ourpermissions")
        sql = sql.Replace("OURReportInfo", "ourreportinfo")
        sql = sql.Replace("OURReportShow", "ourreportshow")
        sql = sql.Replace("OURReportSQLquery", "ourreportsqlquery")
        sql = sql.Replace("OURReportGroups", "ourreportgroups")
        sql = sql.Replace("OURReportFormat", "ourreportformat")
        sql = sql.Replace("OURReportLists", "ourreportlists")
        sql = sql.Replace("OURReportChildren", "ourreportchildren")
        sql = sql.Replace("OURFriendlyNames", "ourfriendlynames")
        sql = sql.Replace("OURFiles", "ourfiles")
        sql = sql.Replace("OURUnits", "ourunits")
        sql = sql.Replace("OURUserTables", "ourusertables")
        sql = sql.Replace("OURUSERTABLES", "ourusertables")
        sql = sql.Replace("OURAgents", "ouragents")
        sql = sql.Replace("OURAGENTS", "ouragents")
        sql = sql.Replace("OURActivity", "ouractivity")
        sql = sql.Replace("OURACTIVITY", "ouractivity")
        sql = sql.Replace("OURUNITS", "ourunits")
        sql = sql.Replace("OURFILES", "ourfiles")
        sql = sql.Replace("OURFRIENDLYNAMES", "ourfriendlynames")
        sql = sql.Replace("OURHELPDESK", "ourhelpdesk")
        sql = sql.Replace("OURACCESSLOG", "ouraccessLog")
        sql = sql.Replace("OURDASHBOARDS", "ourdashboards")
        sql = sql.Replace("OURKMLHISTORY", "ourkmlhistory")
        sql = sql.Replace("OURPERMITS", "ourpermits")
        sql = sql.Replace("OURPERMISSIONS", "ourpermissions")
        sql = sql.Replace("OURREPORTINFO", "ourreportinfo")
        sql = sql.Replace("OURREPORTSHOW", "ourreportshow")
        sql = sql.Replace("OURREPORTSQLQUERY", "ourreportsqlquery")
        sql = sql.Replace("OURREPORTGROUPS", "ourreportgroups")
        sql = sql.Replace("OURREPORTFORMAT", "ourreportformat")
        sql = sql.Replace("OURREPORTLISTS", "ourreportlists")
        sql = sql.Replace("OURREPORTCHILDREN", "ourreportchildren")
        sql = sql.Replace("OURTASKLISTSETTING", "ourtasklistsetting")
        sql = sql.Replace("OurTaskListSetting", "ourtasklistsetting")
        sql = sql.Replace("OurScheduledReports", "ourscheduledreports")
        sql = sql.Replace("OURScheduledReports", "ourscheduledreports")
        sql = sql.Replace("OURSCHEDULEDREPORTS", "ourscheduledreports")
        sql = sql.Replace("OURREPORTVIEW", "ourreportview")
        sql = sql.Replace("OURREPORTITEMS", "ourreportitems")
        sql = sql.Replace("OURReportView", "ourreportview")
        sql = sql.Replace("OURReportItems", "ourreportitems")

        'OURComparison
        sql = sql.Replace("OURComparison", "ourcomparison")
        sql = sql.Replace("OurComparison", "ourcomparison")
        sql = sql.Replace("OURcomparison", "ourcomparison")
        sql = sql.Replace("OURCOMPARISON", "ourcomparison")

        sql = sql.Replace("[[", "[").Replace("]]", "]").Replace("``", "`")
        If donotreplace = "" Then sql = sql.Replace("[", "`").Replace("]", "`")
        'remove TOP N
        Dim t As Integer = sql.ToUpper.IndexOf(" TOP ")
        If t > 0 AndAlso sql.Trim.ToUpper.StartsWith("SELECT ") Then
            Dim temp As String = sql.Substring(t + 5).Trim
            t = temp.IndexOf(" ")
            If t > 0 Then
                sql = "SELECT " & temp.Substring(t + 1)
            End If
        End If
        'correct myconstring
        'If myconstring.IndexOf("; Convert Zero Datetime=True; Allow Zero Datetime=True;") < 1 Then
        '    myconstring = (myconstring & "; Convert Zero Datetime=True; Allow Zero Datetime=True;").Replace(";;", ";")
        'End If
        If myconstring.IndexOf("; Convert Zero Datetime=True;") < 1 Then
            myconstring = (myconstring & "; Convert Zero Datetime=True;").Replace(";;", ";")
        End If

        Return sql
    End Function
    Public Function ReportItemsExist(ReportID As String) As Boolean
        Dim sqls As String = String.Empty

        If ReportID <> String.Empty Then
            sqls = "Select * From ourreportitems Where ReportID = '" & ReportID & "'"
            Return HasRecords(sqls)
        End If
        Return False
    End Function
    Public Function FileInOURFiles(ReportID As String, Optional FileType As String = "RDL") As Boolean
        Dim sqls As String = String.Empty

        If ReportID <> String.Empty Then
            sqls = "SELECT * FROM OURFiles WHERE ReportId='" & ReportID & "' AND Type='" & FileType & "'"
            Return HasRecords(sqls)
        End If
        Return False
    End Function
    Public Function ReportViewExists(ReportID As String) As Boolean
        Dim sqls As String = String.Empty

        If ReportID <> String.Empty Then
            sqls = "Select * From ourreportview Where ReportID = '" & ReportID & "'"
            Return HasRecords(sqls)
        End If
        Return False
    End Function
    Public Function ReportItemExists(ReportID As String, tbl As String, fld As String) As Boolean
        Dim sqls As String = String.Empty
        If ReportID <> String.Empty AndAlso tbl <> String.Empty AndAlso fld <> String.Empty Then
            sqls = "Select * From ourreportitems Where ReportID = '" & ReportID & "' And SQLTable = '" & tbl & "' AND SQLField = '" & fld & "'"
            Return HasRecords(sqls)
        End If
        Return False
    End Function
    Public Function ReportItemIDExists(ReportID As String, ItemID As String) As Boolean
        Dim sqls As String = String.Empty
        If ReportID <> String.Empty AndAlso ItemID <> String.Empty Then
            sqls = "Select * From ourreportitems Where ReportID = '" & ReportID & "' And ItemID = '" & ItemID & "'"
            Return HasRecords(sqls)
        End If
        Return False
    End Function
    Public Function SQLFieldExists(rep As String, tblfld As String) As Boolean
        Dim tbl As String = Piece(tblfld, ".", 1)
        Dim fld As String = Piece(tblfld, ".", 2)
        Dim sqls = "SELECT * FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND DOING = 'SELECT' And Tbl1 = '" & tbl & "' And Tbl1Fld1 = '" & fld & "'"
        If tbl <> String.Empty AndAlso fld <> String.Empty Then
            Return HasRecords(sqls)
        End If
        Return False
    End Function

    Public Function ReportFieldExists(rep As String, val As String) As Boolean
        If rep <> String.Empty Then
            Dim sqls = "SELECT * FROM OURReportFormat WHERE ReportId = '" & rep & "' AND Prop = 'FIELDS' And Val = '" & val & "'"
            Return HasRecords(sqls)
        End If
        Return False
    End Function
    Public Function HasRecords(ByVal mySQL As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "", Optional ByRef er As String = "") As Boolean
        mySQL = mySQL.Replace("""", "'")
        Dim hasrec As Boolean
        mySQL = cleanSQL(mySQL)
        mySQL = Replace(mySQL, Chr(34), Chr(39))
        Dim myRecords As DataTable = New DataTable
        Dim myconstring, myprovider As String
        Dim dbcase As String = String.Empty
        Try
            If myconstr.Trim = "" Then
                myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
                myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                If myprovider = "Npgsql" Then dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
            Else
                If myconprv = "Npgsql" Then
                    If System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing AndAlso myconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("csvdbcase").ToString
                    ElseIf myconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
                    ElseIf myconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("userdbcase").ToString
                    Else 'postgres, etc...
                        dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
                    End If
                End If
                myconstring = myconstr
                myprovider = myconprv
            End If

            If myprovider = "InterSystems.Data.IRISClient" Then
                mySQL = CorrectSQLforCache(mySQL)
                Dim propResult As String = HasRecords_IRIS(mySQL, myconstring, myRecords)
                If propResult = "RETURN_FALSE" Then Return False

            ElseIf myprovider = "InterSystems.Data.CacheClient" Then
                mySQL = CorrectSQLforCache(mySQL)
                Dim propResult As String = HasRecords_Cache(mySQL, myconstring, myRecords)
                If propResult = "RETURN_FALSE" Then Return False

            ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
                myconstring = CorrectConnstringForPostgres(myconstring, dbcase)
                mySQL = ConvertFromSqlServerToPostgres(mySQL, dbcase, myconstring, myprovider)
                Dim myConnection As Npgsql.NpgsqlConnection
                Dim myCommand As New Npgsql.NpgsqlCommand
                myConnection = New Npgsql.NpgsqlConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = mySQL
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                Dim myAdapter As New Npgsql.NpgsqlDataAdapter(myCommand)
                myAdapter.Fill(myRecords)
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()

            ElseIf myprovider = "Sqlite" Then
                'Dim myConnection As SqlConnection
                'Dim myCommand As New SqlClient.SqlCommand
                'Dim myAdapter As SqlClient.SqlDataAdapter
                'myConnection = New SqlConnection(myconstring)
                'myCommand.Connection = myConnection
                'myCommand.CommandType = CommandType.Text
                'myCommand.CommandTimeout = 300000
                'myCommand.CommandText = mySQL
                'If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                'myAdapter = New SqlClient.SqlDataAdapter(myCommand)
                'myAdapter.Fill(myRecords)
                'myAdapter.Dispose()
                'myCommand.Connection.Close()
                'myCommand.Dispose()
                mySQL = CorrectSQLforSQLServer(mySQL)
                myRecords = mRecords(mySQL).Table

            ElseIf myprovider = "System.Data.SqlClient" Then
                mySQL = CorrectSQLforSQLServer(mySQL)
                Dim myConnection As SqlConnection
                Dim myCommand As New SqlClient.SqlCommand
                Dim myAdapter As SqlClient.SqlDataAdapter
                myConnection = New SqlConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = mySQL
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New SqlClient.SqlDataAdapter(myCommand)
                myAdapter.Fill(myRecords)
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()
            ElseIf myprovider = "MySql.Data.MySqlClient" Then
                mySQL = CorrectSQLforMySql(mySQL, myconstring)
                Dim myConnection As MySqlConnection
                Dim myCommand As New MySqlCommand
                Dim myAdapter As MySqlDataAdapter
                myConnection = New MySqlConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = mySQL
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New MySqlDataAdapter(myCommand)
                myAdapter.Fill(myRecords)
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()

            ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then
                mySQL = CorrectSQLforOracle(mySQL, myconstring)
                HasRecords_Oracle(mySQL, myconstring, myRecords)

            ElseIf myprovider = "System.Data.Odbc" Then
                myconstring = myconstring.Replace("Password", "Pwd").Replace("User ID", "UID")
                mySQL = CorrectSQLforSQLServer(mySQL)
                mySQL = ConvertFromSqlServerToODBC(mySQL, myconstring, myprovider)
                Dim myConnection As System.Data.Odbc.OdbcConnection
                Dim myCommand As New System.Data.Odbc.OdbcCommand
                Dim myAdapter As System.Data.Odbc.OdbcDataAdapter
                myConnection = New System.Data.Odbc.OdbcConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = mySQL
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New System.Data.Odbc.OdbcDataAdapter(myCommand)
                myAdapter.Fill(myRecords)
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()
            ElseIf myprovider = "System.Data.OleDb" Then
                'for OleDb
                myconstring = ConfigurationManager.AppSettings("ACEOLEDBversion").ToString & "Persist Security Info=True;" & myconstring
                mySQL = CorrectSQLforSQLServer(mySQL)
                mySQL = ConvertFromSqlServerToODBC(mySQL, myconstring, myprovider)
                Dim myConnection As System.Data.OleDb.OleDbConnection
                Dim myCommand As New System.Data.OleDb.OleDbCommand
                Dim myAdapter As System.Data.OleDb.OleDbDataAdapter
                myConnection = New System.Data.OleDb.OleDbConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = mySQL
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New System.Data.OleDb.OleDbDataAdapter(myCommand)
                myAdapter.Fill(myRecords)
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()
            End If
            If myRecords Is Nothing Then
                hasrec = False
            ElseIf myRecords.Rows.Count > 0 Then
                hasrec = True
            Else
                hasrec = False
            End If
        Catch ex As Exception
            er = ex.Message
            Return False
        End Try
        Return hasrec
    End Function
    Public Function CountOfRecords(ByVal mySQL As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        mySQL = mySQL.Replace("""", "'")
        mySQL = cleanSQL(mySQL)
        mySQL = Replace(mySQL, Chr(34), Chr(39))
        Dim myRecords As DataTable = New DataTable
        Dim myconstring, myprovider As String
        Dim dbcase As String = String.Empty
        Try
            If myconstr.Trim = "" Then
                myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
                myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                If myprovider = "Npgsql" Then dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
            Else
                If myconprv = "Npgsql" Then
                    If System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing AndAlso myconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("csvdbcase").ToString
                    ElseIf myconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
                    ElseIf myconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("userdbcase").ToString
                    Else 'postgres, etc...
                        dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
                    End If
                End If
                myconstring = myconstr
                myprovider = myconprv
            End If

            If myprovider = "InterSystems.Data.IRISClient" Then
                mySQL = CorrectSQLforCache(mySQL)
                Dim propResult As String = CountOfRecords_IRIS(mySQL, myconstring, myRecords)
                If propResult <> String.Empty Then Return propResult

            ElseIf myprovider = "InterSystems.Data.CacheClient" Then
                mySQL = CorrectSQLforCache(mySQL)
                Dim propResult As String = CountOfRecords_Cache(mySQL, myconstring, myRecords)
                If propResult <> String.Empty Then Return propResult


            ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
                myconstring = CorrectConnstringForPostgres(myconstring, dbcase)
                mySQL = ConvertFromSqlServerToPostgres(mySQL, dbcase, myconstring, myprovider)
                Dim myConnection As Npgsql.NpgsqlConnection
                Dim myCommand As New Npgsql.NpgsqlCommand
                myConnection = New Npgsql.NpgsqlConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = mySQL
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                Dim myAdapter As New Npgsql.NpgsqlDataAdapter(myCommand)
                myAdapter.Fill(myRecords)
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()
            ElseIf myprovider = "System.Data.SqlClient" Then
                Dim myConnection As SqlConnection
                Dim myCommand As New SqlClient.SqlCommand
                Dim myAdapter As SqlClient.SqlDataAdapter
                myConnection = New SqlConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = mySQL
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New SqlClient.SqlDataAdapter(myCommand)
                myAdapter.Fill(myRecords)
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()
            ElseIf myprovider = "MySql.Data.MySqlClient" Then
                mySQL = CorrectSQLforMySql(mySQL, myconstring)
                Dim myConnection As MySqlConnection
                Dim myCommand As New MySqlCommand
                Dim myAdapter As MySqlDataAdapter
                myConnection = New MySqlConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = mySQL
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New MySqlDataAdapter(myCommand)
                myAdapter.Fill(myRecords)
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()

            ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then
                mySQL = CorrectSQLforOracle(mySQL, myconstring)
                Dim propResult As String = CountOfRecords_Oracle(mySQL, myconstring, myRecords)

            ElseIf myprovider = "System.Data.Odbc" Then
                myconstring = myconstring.Replace("Password", "Pwd").Replace("User ID", "UID")
                mySQL = CorrectSQLforSQLServer(mySQL)
                mySQL = ConvertFromSqlServerToODBC(mySQL, myconstring, myprovider)
                Dim myConnection As System.Data.Odbc.OdbcConnection
                Dim myCommand As New System.Data.Odbc.OdbcCommand
                Dim myAdapter As System.Data.Odbc.OdbcDataAdapter
                myConnection = New System.Data.Odbc.OdbcConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = mySQL
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New System.Data.Odbc.OdbcDataAdapter(myCommand)
                myAdapter.Fill(myRecords)
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()
            ElseIf myprovider = "System.Data.OleDb" Then
                'for OleDb
                myconstring = ConfigurationManager.AppSettings("ACEOLEDBversion").ToString & "Persist Security Info=True;" & myconstring
                mySQL = CorrectSQLforSQLServer(mySQL)
                mySQL = ConvertFromSqlServerToODBC(mySQL, myconstring, myprovider)
                Dim myConnection As System.Data.OleDb.OleDbConnection
                Dim myCommand As New System.Data.OleDb.OleDbCommand
                Dim myAdapter As System.Data.OleDb.OleDbDataAdapter
                myConnection = New System.Data.OleDb.OleDbConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = mySQL
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New System.Data.OleDb.OleDbDataAdapter(myCommand)
                myAdapter.Fill(myRecords)
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()
            End If
            If myRecords.Rows.Count > 0 Then
                Return myRecords.Rows.Count.ToString
            Else
                Return 0.ToString
            End If
        Catch ex As Exception
            Return "ERROR!! " & ex.Message
        End Try
        'Return hasrec
    End Function
    Public Function GetGlobalNodeValue(ByVal glb As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Try
            Dim myconstring, myprovider As String
            If myconstr.Trim = "" Then
                myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
                myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            Else
                myconstring = myconstr
                myprovider = myconprv
            End If
            If myprovider = "InterSystems.Data.IRISClient" Then
                ret = GetGlobalNodeValue_IRIS(glb, myconstring)
                Return ret
            ElseIf myprovider = "InterSystems.Data.CacheClient" Then
                ret = GetGlobalNodeValue_Cache(glb, myconstring)
                Return ret
            Else
                ret = "Global node value is only for InterSystems databases..."

            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function GetProviderNameFromConnString(ByVal constr As String) As String
        Dim ret As String = String.Empty
        Dim i As Integer
        Try
            i = constr.ToUpper.IndexOf("PROVIDERNAME")
            ret = constr.Substring(i + 13)
            ret = ret.Replace("=", "").Replace(";", "").Replace("""", "")
        Catch ex As Exception
            ret = ret = ret & " " & ex.Message
        End Try
        Return ret.Trim
    End Function
    Public Function GetProviderNameByConnString(ByVal connstr As String, Optional ByRef er As String = "", Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim prov As String = String.Empty
        Dim dv As DataView = mRecords("SELECT DISTINCT ConnPrv FROM OURPermits WHERE ConnStr LIKE '%" & connstr.Trim.Replace(" ", "%") & "%'", er, myconstr, myconprv)
        If dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then
            Dim myconstring As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            Dim myprovider As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            If myconstring.Contains(connstr) Then
                Return myprovider
            End If
            er = "No provider found for Connection String: " & connstr & " !"
            Return ""
        Else
            prov = dv.Table.Rows(0)("ConnPrv").ToString.Trim
        End If
        Return prov
    End Function
    Public Function mRecords(ByVal mySQL As String, Optional ByRef er As String = "", Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As DataView
        Dim myView As DataView = New DataView
        Dim myRecords As DataTable = New DataTable
        Dim myconstring, myprovider As String
        Dim dbcase As String = String.Empty
        Try
            If myconstr.Trim = "" Then
                myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
                myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                If myprovider = "Npgsql" Then dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
            Else
                If myconprv = "Npgsql" Then
                    If System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing AndAlso myconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("csvdbcase").ToString
                    ElseIf myconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
                    ElseIf myconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("userdbcase").ToString
                    Else 'postgres, etc...
                        dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
                    End If
                End If
                myconstring = myconstr
                myprovider = myconprv
            End If

            If myprovider = "InterSystems.Data.IRISClient" Then
                mySQL = CorrectSQLforCache(mySQL)
                Dim propResult As String = mRecords_IRIS(mySQL, er, myconstring, myRecords)
                If propResult = "USE_DISTINCT" Then
                    er = GetDistinctDataViewWithProperCase(mySQL, myView, myconstring, myprovider)
                    Return myView
                ElseIf propResult = "RETURN_VIEW" Then
                    Return myView
                End If

            ElseIf myprovider = "InterSystems.Data.CacheClient" Then
                mySQL = CorrectSQLforCache(mySQL)
                Dim propResult As String = mRecords_Cache(mySQL, er, myconstring, myRecords)
                If propResult = "USE_DISTINCT" Then
                    er = GetDistinctDataViewWithProperCase(mySQL, myView, myconstring, myprovider)
                    Return myView
                ElseIf propResult = "RETURN_VIEW" Then
                    Return myView
                End If


            ElseIf myprovider = "MySql.Data.MySqlClient" Then
                Dim myConnection As MySqlConnection
                Dim myCommand As New MySqlCommand
                Dim myAdapter As MySqlDataAdapter
                Try
                    mySQL = CorrectSQLforMySql(mySQL, myconstring)
                    myConnection = New MySqlConnection(myconstring)
                    myCommand.Connection = myConnection
                    myCommand.CommandType = CommandType.Text
                    myCommand.CommandTimeout = 300000
                    myCommand.CommandText = mySQL
                    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                    myAdapter = New MySqlDataAdapter(myCommand)
                    myAdapter.Fill(myRecords)
                    myAdapter.Dispose()
                Catch ex As Exception
                    'assign error er
                    er = "ERROR!! " & ex.Message
                End Try
                myCommand.Connection.Close()
                myCommand.Dispose()

            ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then
                mySQL = CorrectSQLforOracle(mySQL, myconstring)
                mRecords_Oracle(mySQL, er, myconstring, myRecords)

            ElseIf myprovider = "System.Data.Odbc" Then
                Dim myConnection As System.Data.Odbc.OdbcConnection
                Dim myCommand As New System.Data.Odbc.OdbcCommand
                Try
                    myconstring = myconstring.Replace("Password", "Pwd").Replace("User ID", "UID")
                    mySQL = mySQL.Replace("`", "")
                    mySQL = CorrectSQLforSQLServer(mySQL)
                    mySQL = ConvertFromSqlServerToODBC(mySQL, myconstring, myprovider)

                    myConnection = New System.Data.Odbc.OdbcConnection(myconstring)
                    Dim myAdapter As System.Data.Odbc.OdbcDataAdapter
                    myCommand.Connection = myConnection
                    myCommand.CommandType = CommandType.Text
                    myCommand.CommandTimeout = 300000
                    myCommand.CommandText = mySQL
                    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                    myAdapter = New System.Data.Odbc.OdbcDataAdapter(myCommand)
                    myAdapter.Fill(myRecords)
                    myAdapter.Dispose()
                Catch ex As Exception
                    'assign error er
                    er = "ERROR!! " & ex.Message
                End Try
                myCommand.Connection.Close()
                myCommand.Dispose()

            ElseIf myprovider = "System.Data.OleDb" Then
                Dim myConnection As System.Data.OleDb.OleDbConnection
                Dim myCommand As New System.Data.OleDb.OleDbCommand
                Dim myAdapter As System.Data.OleDb.OleDbDataAdapter
                Try
                    myconstring = ConfigurationManager.AppSettings("ACEOLEDBversion").ToString & "Persist Security Info=True;" & myconstring
                    mySQL = mySQL.Replace("`", "")
                    mySQL = CorrectSQLforSQLServer(mySQL)
                    mySQL = ConvertFromSqlServerToODBC(mySQL, myconstring, myprovider)
                    myConnection = New System.Data.OleDb.OleDbConnection(myconstring)
                    myCommand.Connection = myConnection
                    myCommand.CommandType = CommandType.Text
                    myCommand.CommandTimeout = 300000
                    myCommand.CommandText = mySQL
                    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                    myAdapter = New System.Data.OleDb.OleDbDataAdapter(myCommand)
                    myAdapter.Fill(myRecords)
                    myAdapter.Dispose()
                Catch ex As Exception
                    'assign error er
                    er = "ERROR!! " & ex.Message
                End Try
                myCommand.Connection.Close()
                myCommand.Dispose()

            ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
                Dim myConnection As Npgsql.NpgsqlConnection
                Dim myCommand As New Npgsql.NpgsqlCommand
                Dim myAdapter As New Npgsql.NpgsqlDataAdapter
                Try
                    myconstring = CorrectConnstringForPostgres(myconstring, dbcase)
                    mySQL = ConvertFromSqlServerToPostgres(mySQL, dbcase, myconstring, myprovider)
                    myConnection = New Npgsql.NpgsqlConnection(myconstring)
                    myCommand.Connection = myConnection
                    myCommand.CommandType = CommandType.Text
                    myCommand.CommandTimeout = 300000
                    myCommand.CommandText = mySQL
                    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                    myAdapter = New Npgsql.NpgsqlDataAdapter(myCommand)
                    myAdapter.Fill(myRecords)
                Catch ex As Exception
                    'assign error er
                    er = "ERROR!! " & ex.Message
                End Try
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()

            ElseIf myprovider = "Sqlite" Then
                Dim myCommand As New SqliteCommand
                Try
                    mySQL = CorrectSQLforSQLServer(mySQL).Replace("UCASE(", "UPPER(")  '.Replace("[", "").Replace("]", "")
                    mySQL = mySQL.Replace("""", "'")
                    mySQL = mySQL.Replace("`", "")
                    myCommand.Connection = sqliteconn
                    myCommand.CommandType = CommandType.Text
                    myCommand.CommandTimeout = 300000
                    myCommand.CommandText = mySQL
                    Dim myReader As SqliteDataReader = myCommand.ExecuteReader
                    myRecords.Load(myReader)
                Catch ex As Exception
                    'assign error er
                    er = "ERROR!! " & ex.Message
                End Try
                myCommand.Dispose()
            Else
                Dim myConnection As SqlConnection
                Dim myCommand As New SqlClient.SqlCommand
                Dim myAdapter As SqlClient.SqlDataAdapter
                Try
                    mySQL = CorrectSQLforSQLServer(mySQL)
                    mySQL = mySQL.Replace("""", "'")
                    mySQL = mySQL.Replace("`", "")
                    myConnection = New SqlConnection(myconstring)
                    myCommand.Connection = myConnection
                    myCommand.CommandType = CommandType.Text
                    myCommand.CommandTimeout = 300000
                    myCommand.CommandText = mySQL
                    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                    myAdapter = New SqlClient.SqlDataAdapter(myCommand)
                    myAdapter.Fill(myRecords)
                    myAdapter.Dispose()
                Catch ex As Exception
                    'assign error er
                    er = "ERROR!! " & ex.Message
                End Try
                myCommand.Connection.Close()
                myCommand.Dispose()
            End If
            myView = myRecords.DefaultView

            'Dim dt As DataTable = MakeDTColumnsNamesCLScompliant(myRecords, myprovider, er)
            'myView = dt.DefaultView
            ''fix upper case in InterSystems
            'If myprovider.StartsWith("InterSystems.Data.") AndAlso mySQL.ToUpper.IndexOf(" DISTINCT ") > 0 Then
            '    err = GetDistinctDataViewWithProperCase(mySQL, myView, myconstring, myprovider)
            'End If

        Catch ex As Exception
            'return error somehow
            er = "ERROR!! " & ex.Message
        End Try
        Return myView
    End Function
    Public Function mRecordsFromSP(ByVal mySP As String, ByVal Nparameters As Integer, ByVal ParamName As Array, ByVal ParamType As Array, ByVal ParamValue As Array, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As DataView
        Dim myView As New DataView
        Dim myRecords As DataTable = New DataTable
        Dim myconstring, myprovider As String
        Dim n As Integer = Nparameters
        Dim dbcase As String = String.Empty
        Try
            If myconstr.Trim = "" Then
                myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
                myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                If myprovider = "Npgsql" Then dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
            Else
                If myconprv = "Npgsql" Then
                    If System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing AndAlso myconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("csvdbcase").ToString
                    ElseIf myconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
                    Else 'userdbcase
                        dbcase = userdbcase
                    End If
                End If
                myconstring = myconstr
                myprovider = myconprv
            End If
            If myprovider = "InterSystems.Data.IRISClient" Then
                Dim propResult As String = mRecordsFromSP_IRIS(mySP, Nparameters, ParamName, ParamType, ParamValue, myconstring, myRecords)
                Return myRecords.DefaultView
            ElseIf myprovider = "InterSystems.Data.CacheClient" Then
                Dim propResult As String = mRecordsFromSP_Cache(mySP, Nparameters, ParamName, ParamType, ParamValue, myconstring, myRecords)
                Return myRecords.DefaultView
            ElseIf myprovider = "MySql.Data.MySqlClient" Then
                Dim ret As String = String.Empty
                ret = CorrectSQLforMySql(ret, myconstring)
                Dim myConnection As MySqlConnection
                Dim myCommand As New MySqlCommand
                Dim myAdapter As MySqlDataAdapter
                myConnection = New MySqlConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = mySP
                Dim param(Nparameters) As MySqlParameter
                For i = 0 To Nparameters - 1
                    If ParamType(i) = "nvarchar" Then
                        param(i) = New MySqlParameter("@" + ParamName(i), MySqlDbType.VarChar, 255, ParameterDirection.Input)
                    ElseIf ParamType(i) = "datetime" Then
                        param(i) = New MySqlParameter("@" + ParamName(i), MySqlDbType.DateTime, 255, ParameterDirection.Input)
                    Else
                        param(i) = New MySqlParameter("@" + ParamName(i), MySqlDbType.Int16, 255, ParameterDirection.Input)
                    End If
                    param(i).Value = ParamValue(i)
                    myCommand.Parameters.Add(param(i))
                Next
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New MySqlDataAdapter(myCommand)
                myAdapter.Fill(myRecords)
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()
            ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then
                mRecordsFromSP_Oracle(mySP, Nparameters, ParamName, ParamType, ParamValue, myconstring, myconstr, myconprv, myRecords)
            ElseIf myprovider = "System.Data.Odbc" Then
                myconstring = myconstring.Replace("Password", "Pwd").Replace("User ID", "UID")
                Dim myConnection As System.Data.Odbc.OdbcConnection
                Dim myCommand As New System.Data.Odbc.OdbcCommand
                Dim myAdapter As System.Data.Odbc.OdbcDataAdapter
                myConnection = New System.Data.Odbc.OdbcConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.StoredProcedure
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = mySP
                Dim param(Nparameters) As MySqlParameter
                For i = 0 To Nparameters - 1
                    If ParamType(i) = "nvarchar" Then
                        param(i) = New MySqlParameter("@" + ParamName(i), MySqlDbType.VarChar, 255, ParameterDirection.Input)
                    ElseIf ParamType(i) = "datetime" Then
                        param(i) = New MySqlParameter("@" + ParamName(i), MySqlDbType.DateTime, 255, ParameterDirection.Input)
                    Else
                        param(i) = New MySqlParameter("@" + ParamName(i), MySqlDbType.Int16, 255, ParameterDirection.Input)
                    End If
                    param(i).Value = ParamValue(i)
                    myCommand.Parameters.Add(param(i))
                Next
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New System.Data.Odbc.OdbcDataAdapter(myCommand)
                myAdapter.Fill(myRecords)
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()

            ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
                myconstring = CorrectConnstringForPostgres(myconstring, dbcase)
                Dim myConnection As Npgsql.NpgsqlConnection
                Dim myCommand As New Npgsql.NpgsqlCommand
                myConnection = New Npgsql.NpgsqlConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandType = CommandType.StoredProcedure
                myCommand.CommandText = mySP
                Dim param(Nparameters) As Npgsql.NpgsqlParameter
                For i = 0 To Nparameters - 1
                    If ParamType(i) = "text" OrElse ParamType(i) = "nvarchar" Then
                        param(i) = New Npgsql.NpgsqlParameter("@" + ParamName(i), NpgsqlTypes.NpgsqlDbType.Text, 255, ParamName(i))
                    ElseIf ParamType(i) = "datetime" Then
                        param(i) = New Npgsql.NpgsqlParameter("@" + ParamName(i), NpgsqlTypes.NpgsqlDbType.Date, 255, ParamName(i))
                    Else
                        param(i) = New Npgsql.NpgsqlParameter("@" + ParamName(i), NpgsqlTypes.NpgsqlDbType.Integer, 255, ParamName(i))
                    End If
                    param(i).Value = ParamValue(i)
                    myCommand.Parameters.Add(param(i))
                Next
                Dim myAdapter As New Npgsql.NpgsqlDataAdapter(myCommand)
                myAdapter.Fill(myRecords)
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()
            Else
                Dim myConnection As SqlConnection
                Dim myCommand As New SqlClient.SqlCommand
                Dim myAdapter As SqlClient.SqlDataAdapter
                myConnection = New SqlConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandType = CommandType.StoredProcedure
                myCommand.CommandText = mySP
                Dim param(Nparameters) As SqlParameter
                For i = 0 To Nparameters - 1
                    If ParamType(i) = "nvarchar" Then
                        param(i) = New SqlParameter("@" + ParamName(i), SqlDbType.NVarChar, 255, ParameterDirection.Input)
                    ElseIf ParamType(i) = "datetime" Then
                        param(i) = New SqlParameter("@" + ParamName(i), SqlDbType.DateTime, 255, ParameterDirection.Input)
                    Else
                        param(i) = New SqlParameter("@" + ParamName(i), SqlDbType.Int, 255, ParameterDirection.Input)
                    End If
                    param(i).Value = ParamValue(i)
                    myCommand.Parameters.Add(param(i))
                Next
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New SqlClient.SqlDataAdapter(myCommand)
                myAdapter.Fill(myRecords)
                myAdapter.Dispose()
                myCommand.Connection.Close()
                myCommand.Dispose()
            End If
            myView = myRecords.DefaultView
            'Dim er As String = String.Empty
            'Dim dt As DataTable = MakeDTColumnsNamesCLScompliant(myRecords, myprovider, er)
            'myView = dt.DefaultView
        Catch ex As Exception
            Return Nothing
        End Try
        Return myView
    End Function
    Public Function RunSP(ByVal mySP As String, ByVal Nparameters As Integer, ByVal ParamName As Array, ByVal ParamType As Array, ByVal ParamValue As Array, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim r As String
        r = ""
        Dim myconstring, myprovider As String
        Dim dbcase As String = String.Empty
        Try
            If myconstr.Trim = "" Then
                myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
                myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                If myprovider = "Npgsql" Then dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
            Else
                If myconprv = "Npgsql" Then
                    If System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing AndAlso myconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("csvdbcase").ToString
                    ElseIf myconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
                    Else 'userdbcase
                        dbcase = userdbcase
                    End If
                End If
                myconstring = myconstr
                myprovider = myconprv
            End If
            If myprovider = "InterSystems.Data.IRISClient" Then
                r = RunSP_IRIS(mySP, Nparameters, ParamName, ParamType, ParamValue, myconstring)
            ElseIf myprovider = "InterSystems.Data.CacheClient" Then
                r = RunSP_Cache(mySP, Nparameters, ParamName, ParamType, ParamValue, myconstring)
            ElseIf myprovider = "MySql.Data.MySqlClient" Then
                Dim ret As String = String.Empty
                ret = CorrectSQLforMySql(ret, myconstring)
                Dim myConnection As MySqlConnection
                Dim myCommand As New MySqlCommand
                myConnection = New MySqlConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = mySP
                Dim param(Nparameters) As MySqlParameter
                For i = 0 To Nparameters - 1
                    If ParamType(i) = "nvarchar" Then
                        param(i) = New MySqlParameter("@" + ParamName(i), MySqlDbType.VarChar, 255, ParameterDirection.Input)
                    ElseIf ParamType(i) = "datetime" Then
                        param(i) = New MySqlParameter("@" + ParamName(i), MySqlDbType.DateTime, 255, ParameterDirection.Input)
                    Else
                        param(i) = New MySqlParameter("@" + ParamName(i), MySqlDbType.Int16, 255, ParameterDirection.Input)
                    End If
                    param(i).Value = ParamValue(i)
                    myCommand.Parameters.Add(param(i))
                Next
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myCommand.ExecuteNonQuery()
                myCommand.Connection.Close()
                myCommand.Dispose()
            ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then
                r = RunSP_Oracle(mySP, Nparameters, ParamName, ParamType, ParamValue, myconstring)
            ElseIf myprovider = "System.Data.Odbc" Then
                myconstring = myconstring.Replace("Password", "Pwd").Replace("User ID", "UID")
                Dim myConnection As System.Data.Odbc.OdbcConnection
                Dim myCommand As New System.Data.Odbc.OdbcCommand
                myConnection = New System.Data.Odbc.OdbcConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.StoredProcedure
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = mySP
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                Dim param(Nparameters) As MySqlParameter
                For i = 0 To Nparameters - 1
                    If ParamType(i) = "nvarchar" Then
                        param(i) = New MySqlParameter("@" + ParamName(i), MySqlDbType.VarChar, 255, ParameterDirection.Input)
                    ElseIf ParamType(i) = "datetime" Then
                        param(i) = New MySqlParameter("@" + ParamName(i), MySqlDbType.DateTime, 255, ParameterDirection.Input)
                    Else
                        param(i) = New MySqlParameter("@" + ParamName(i), MySqlDbType.Int16, 255, ParameterDirection.Input)
                    End If
                    param(i).Value = ParamValue(i)
                    myCommand.Parameters.Add(param(i))
                Next
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myCommand.ExecuteNonQuery()
                myCommand.Connection.Close()
                myCommand.Dispose()
            ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
                myconstring = CorrectConnstringForPostgres(myconstring, dbcase)
                Dim myConnection As Npgsql.NpgsqlConnection
                Dim myCommand As New Npgsql.NpgsqlCommand
                myConnection = New Npgsql.NpgsqlConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandType = CommandType.StoredProcedure
                myCommand.CommandText = mySP
                Dim param(Nparameters) As Npgsql.NpgsqlParameter
                For i = 0 To Nparameters - 1
                    If ParamType(i) = "text" OrElse ParamType(i) = "nvarchar" Then
                        param(i) = New Npgsql.NpgsqlParameter("@" + ParamName(i), NpgsqlTypes.NpgsqlDbType.Text, 255, ParamName(i))
                    ElseIf ParamType(i) = "datetime" Then
                        param(i) = New Npgsql.NpgsqlParameter("@" + ParamName(i), NpgsqlTypes.NpgsqlDbType.Date, 255, ParamName(i))
                    Else
                        param(i) = New Npgsql.NpgsqlParameter("@" + ParamName(i), NpgsqlTypes.NpgsqlDbType.Integer, 255, ParamName(i))
                    End If
                    param(i).Value = ParamValue(i)
                    myCommand.Parameters.Add(param(i))
                Next
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myCommand.ExecuteNonQuery()
                myCommand.Connection.Close()
                myCommand.Dispose()
            Else
                Dim myConnection As SqlConnection
                Dim myCommand As New SqlClient.SqlCommand
                myConnection = New SqlConnection(myconstring)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandType = CommandType.StoredProcedure
                myCommand.CommandText = mySP
                Dim param(Nparameters) As SqlParameter
                For i = 0 To Nparameters - 1
                    If ParamType(i) = "nvarchar" Then
                        param(i) = New SqlParameter("@" + ParamName(i), SqlDbType.NVarChar, 255, ParameterDirection.Input)
                    ElseIf ParamType(i) = "datetime" Then
                        param(i) = New SqlParameter("@" + ParamName(i), SqlDbType.DateTime, 255, ParameterDirection.Input)
                    Else
                        param(i) = New SqlParameter("@" + ParamName(i), SqlDbType.Int, 255, ParameterDirection.Input)
                    End If
                    param(i).Value = ParamValue(i)
                    myCommand.Parameters.Add(param(i))
                Next
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myCommand.ExecuteNonQuery()
                myCommand.Connection.Close()
                myCommand.Dispose()
            End If
        Catch exc As Exception
            r = exc.Message
            Return r
        End Try
        Return r
    End Function
    Public Function DateToString(dt As DateTime, Optional ByVal userconprv As String = "", Optional IsTimeStamp As Boolean = False) As String
        Dim myprovider As String = userconprv
        If myprovider = String.Empty Then _
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        Try
            'TODO for other providers(?)
            If myprovider = "MySql.Data.MySqlClient" Then
                If IsTimeStamp Then
                    Return Format(dt, "HH:mm:ss")
                Else 'default
                    Return Format(dt, "yyyy-MM-dd HH:mm:ss")
                End If

            ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then
                If IsTimeStamp Then
                    'Return Format(dt, "dd-MMM-yyyy hh.mm.ss tt")
                    Return Format(dt, "yyyy-MM-dd hh.mm.ss tt")
                Else
                    Return Format(dt, "dd-MMM-yyyy")
                    'Return Format(dt, "yyyy-MM-dd")
                End If
                'TODO if needed for PostgreSQL: ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
            Else
                'Convert.ToDateTime(Your date variable, CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff")
                Return CDate(dt).ToString("yyyy-MM-dd HH:mm:ss")
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Public Function DateToStringFormat(dt As DateTime, Optional ByVal userconprv As String = "", Optional ByVal datetimeformat As String = "yyyy-MM-dd HH:mm:00") As String
        Dim myprovider As String = userconprv
        If myprovider = String.Empty Then _
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        Try
            If myprovider = "MySql.Data.MySqlClient" Then
                Return Format(dt, datetimeformat)
            ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then

                Return Format(dt, datetimeformat)
            Else
                'Convert.ToDateTime(Your date variable, CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss.fff")
                Return CDate(dt).ToString(datetimeformat)
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Public Function DateCurrentFunction() As String
        Dim myprovider As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        If myprovider = "MySql.Data.MySqlClient" Then
            Return "CURDATE()"
        ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then

            'TODO if needed for PostgreSQL: ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql

            'TODO !!!!!! for other providers

            Return "CURRENT_DATE"
        Else
            Return "GETDATE()"
        End If
    End Function
    Public Function ConvertSQLSyntaxFromOURdbToUserDB(ByVal mSql As String, Optional ByVal userprovider As String = "", Optional ByRef er As String = "", Optional ByVal userconnstring As String = "") As String
        Dim rSql As String = mSql
        Dim myprovider As String
        Dim dbcase As String = String.Empty
        Try
            'OUR db
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            'User db
            If userprovider = "" Then
                Return mSql
            ElseIf userconnstring.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString.Trim.ToUpper Then
                dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
            Else
                dbcase = userdbcase
            End If

            If myprovider.StartsWith("InterSystems.Data.") Then
                If userprovider = "MySql.Data.MySqlClient" Then
                    rSql = ConvertFromCacheToMySql(mSql)
                ElseIf userprovider = "System.Data.SqlClient" Then
                    rSql = ConvertFromCacheToSqlServer(mSql)
                ElseIf userprovider = "Oracle.ManagedDataAccess.Client" Then
                    rSql = ConvertFromCacheToOracle(mSql)
                ElseIf userprovider = "Npgsql" Then  'PostgreSQL  Npgsql
                    userconnstring = CorrectConnstringForPostgres(userconnstring, dbcase)
                    rSql = ConvertFromSqlServerToPostgres(rSql, dbcase, userconnstring, userprovider)
                End If
            ElseIf myprovider = "MySql.Data.MySqlClient" Then
                If userprovider.StartsWith("InterSystems.Data.") Then
                    rSql = ConvertFromMySqlToCache(mSql)
                ElseIf userprovider = "System.Data.SqlClient" Then
                    rSql = ConvertFromMySqlToSqlServer(mSql)
                ElseIf userprovider = "Oracle.ManagedDataAccess.Client" Then
                    rSql = ConvertFromMySqlToOracle(mSql)
                ElseIf userprovider = "Npgsql" Then  'PostgreSQL  Npgsql
                    userconnstring = CorrectConnstringForPostgres(userconnstring, dbcase)
                    rSql = ConvertFromSqlServerToPostgres(rSql, dbcase, userconnstring, userprovider)
                End If
            ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then

                'NOT NEEDED !!!!!! for Oracle.ManagedDataAccess.Client : Oracle will be OURdb only if the user db is Oracle

                'If userprovider.StartsWith("InterSystems.Data.") Then
                '    'rSql = ConvertFromOracleToCache(mSql)
                'ElseIf userprovider = "MySql.Data.MySqlClient" Then
                '    'rSql = ConvertFromOracleToMySql(mSql)
                'ElseIf userprovider = "System.Data.SqlClient" Then
                '    'rSql = ConvertFromOracleToSqlServer(mSql)
                'End If
            ElseIf myprovider = "System.Data.Odbc" Then

                'NOT TODO for Odbc - OURdb will never be ODBC one - only the user db can be ODBC

                'If userprovider.StartsWith("InterSystems.Data.") Then
                '    'rSql = ConvertFromODBCToCache(mSql)
                'ElseIf userprovider = "MySql.Data.MySqlClient" Then
                '    'rSql = ConvertFromODBCToMySql(mSql)
                'ElseIf userprovider = "System.Data.SqlClient" Then
                '    'rSql = ConvertFromODBCToSqlServer(mSql)
                'End If
            ElseIf myprovider = "System.Data.OleDb" Then

                'NOT TODO for Odbc - OURdb will never be ODBC one - only the user db can be ODBC

                'If userprovider.StartsWith("InterSystems.Data.") Then
                '    'rSql = ConvertFromODBCToCache(mSql)
                'ElseIf userprovider = "MySql.Data.MySqlClient" Then
                '    'rSql = ConvertFromODBCToMySql(mSql)
                'ElseIf userprovider = "System.Data.SqlClient" Then
                '    'rSql = ConvertFromODBCToSqlServer(mSql)
                'End If
            ElseIf myprovider = "System.Data.SqlClient" Then
                If userprovider.StartsWith("InterSystems.Data.") Then
                    rSql = ConvertFromSqlServerToCache(mSql)
                ElseIf userprovider = "MySql.Data.MySqlClient" Then
                    rSql = ConvertFromSqlServerToMySql(mSql)
                ElseIf userprovider = "Oracle.ManagedDataAccess.Client" Then
                    rSql = ConvertFromSqlServerToOracle(mSql)
                ElseIf userprovider = "Npgsql" Then  'PostgreSQL  Npgsql
                    userconnstring = CorrectConnstringForPostgres(userconnstring, dbcase)
                    rSql = ConvertFromSqlServerToPostgres(rSql, dbcase, userconnstring, userprovider)
                End If
            End If
        Catch exc As Exception
            er = exc.Message
            rSql = mSql
        End Try
        Return rSql
    End Function
    Public Function ConvertFromCacheToMySql(ByVal mSql As String) As String
        Dim er As String = ""
        Dim rSql As String = ""
        Try
            rSql = mSql.Replace("""", "`")
        Catch exc As Exception
            er = exc.Message
            rSql = ""
        End Try
        Return rSql
    End Function
    Public Function ConvertFromCacheToSqlServer(ByVal mSql As String) As String
        Dim er As String = ""
        Dim rSql As String = ""
        Try
            Dim sqlparts() As String = mSql.Split("""")
            If sqlparts.Length > 0 Then
                For i = 0 To sqlparts.Length - 1
                    rSql = rSql & " " & sqlparts(i)
                    If i < sqlparts.Length - 1 Then
                        rSql = rSql & "[" & sqlparts(i + 1) & "]"
                        i = i + 1
                    End If
                Next
            End If
        Catch exc As Exception
            er = exc.Message
            rSql = ""
        End Try
        Return rSql
    End Function
    Public Function ConvertFromMySqlToCache(ByVal mSql As String) As String
        Dim er As String = ""
        Dim rSql As String = ""
        Try
            rSql = mSql.Replace("`", """")
        Catch exc As Exception
            er = exc.Message
            rSql = ""
        End Try

        'TODO fix syntax for classes names and field names (dots)

        Return rSql
    End Function
    Public Function ConvertFromMySqlToSqlServer(ByVal mSql As String) As String
        Dim er As String = ""
        Dim rSql As String = ""
        Try
            Dim sqlparts() As String = mSql.Split("`")
            If sqlparts.Length > 0 Then
                For i = 0 To sqlparts.Length - 1
                    rSql = rSql & " " & sqlparts(i)
                    If i < sqlparts.Length - 1 Then
                        rSql = rSql & "[" & sqlparts(i + 1) & "]"
                        i = i + 1
                    End If
                Next
            End If
        Catch exc As Exception
            er = exc.Message
            rSql = mSql
        End Try
        Return rSql.Trim
    End Function
    Public Function ConvertFromSqlServerToCache(ByVal mSql As String) As String
        Dim er As String = ""
        Dim rSql As String = ""
        Try
            rSql = mSql.Replace("[", """").Replace("]", """")
            'TODO remove fields with not SQL compatible types

        Catch exc As Exception
            er = exc.Message
            rSql = ""
        End Try
        Return rSql
    End Function
    Public Function ConvertFromSqlServerToMySql(ByVal mSql As String) As String
        Dim er As String = ""
        Dim rSql As String = ""
        Try
            rSql = mSql.Replace("[", "`").Replace("]", "`")
        Catch exc As Exception
            er = exc.Message
            rSql = ""
        End Try
        Return rSql
    End Function
    Public Function CorrectConnstringForPostgres(ByVal mconnstr As String, ByVal dbcase As String) As String
        If dbcase = "mix" OrElse dbcase = "doublequoted" Then
            Return mconnstr
        End If
        Dim db As String = GetDataBase(mconnstr, "Npgsql")
        Dim dbcorrected As String = db
        Dim connstrcorrected As String = mconnstr
        If userdbcase = "upper" Then
            dbcorrected = dbcorrected.ToUpper
        Else
            dbcorrected = dbcorrected.ToLower
        End If
        Return connstrcorrected.Replace(db, dbcorrected).Trim
    End Function
    Public Function ConvertFromSqlServerToPostgres(ByVal mSql As String, ByVal dbcase As String, Optional ByVal myconstring As String = "", Optional ByVal myprovider As String = "") As String
        Dim er As String = ""
        Dim Sql As String = mSql.Replace("[[", "[").Replace("]]", "]").Replace("``", "`")
        Try
            Sql = Sql.Replace("[Order]", """Order""").Replace("[Group]", """Group""").Replace("[User]", """User""").Replace("[Count]", """Count""").Replace("[Status]", """Status""")
            Sql = Sql.Replace("`Order`", """Order""").Replace("`Group`", """Group""").Replace("`User`", """User""").Replace("`Count`", """Count""").Replace("`Status`", """Status""")
            If dbcase = "doublequoted" Then
                Sql = Sql.Replace("[", """").Replace("]", """").Replace("`", """")

                Sql = Sql.Replace("OUR.HelpDesk", "OURHelpDesk")
                Sql = Sql.Replace("OUR.AccessLog", "OURAccessLog")
                Sql = Sql.Replace("OUR.Dashboards", "OURDashboards")
                Sql = Sql.Replace("OUR.Permits", "OURPermits")
                Sql = Sql.Replace("OUR.Permissions", "OURPermissions")
                Sql = Sql.Replace("OUR.ReportInfo", "OURReportInfo")
                Sql = Sql.Replace("OUR.ReportShow", "OURReportShow")
                Sql = Sql.Replace("OUR.ReportSQLquery", "OURReportSQLquery")
                Sql = Sql.Replace("OUR.ReportGroups", "OURReportGroups")
                Sql = Sql.Replace("OUR.ReportFormat", "OURReportFormat")
                Sql = Sql.Replace("OUR.ReportLists", "OURReportLists")
                Sql = Sql.Replace("OUR.ReportChildren", "OURReportChildren")
                Sql = Sql.Replace("OUR.FriendlyNames", "OURFriendlyNames")
                Sql = Sql.Replace("OUR.Files", "OURFiles")
                Sql = Sql.Replace("OURFILES", "OURFiles")
                Sql = Sql.Replace("OURFRIENDLYNAMES", "OURFriendlyNames")
                Sql = Sql.Replace("OURHELPDESK", "OURHelpDesk")
                Sql = Sql.Replace("OURACCESSLOG", "OURAccessLog")
                Sql = Sql.Replace("OURDASHBOARDS", "OURDashboards")
                Sql = Sql.Replace("OURPERMITS", "OURPermits")
                Sql = Sql.Replace("OURPERMISSIONS", "OURPermissions")
                Sql = Sql.Replace("OURREPORTINFO", "OURReportInfo")
                Sql = Sql.Replace("OURREPORTSHOW", "OURReportShow")
                Sql = Sql.Replace("OURREPORTSQLQUERY", "OURReportSQLquery")
                Sql = Sql.Replace("OURREPORTGROUPS", "OURReportGroups")
                Sql = Sql.Replace("OURREPORTFORMAT", "OURReportFormat")
                Sql = Sql.Replace("OURREPORTLISTS", "OURReportLists")
                Sql = Sql.Replace("OURREPORTCHILDREN", "OURReportChildren")
                Sql = Sql.Replace("ourfiles", "OURFiles")
                Sql = Sql.Replace("ourfriendlynames", "OURFriendlyNames")
                Sql = Sql.Replace("ourhelpdesk", "OURHelpDesk")
                Sql = Sql.Replace("ouraccesslog", "OURAccessLog")
                Sql = Sql.Replace("ourdashboards", "OURDashboards")
                Sql = Sql.Replace("ourpermits", "OURPermits")
                Sql = Sql.Replace("ourpermissions", "OURPermissions")
                Sql = Sql.Replace("ourreportinfo", "OURReportInfo")
                Sql = Sql.Replace("ourreportshow", "OURReportShow")
                Sql = Sql.Replace("ourreportsqlquery", "OURReportSQLquery")
                Sql = Sql.Replace("ourreportgroups", "OURReportGroups")
                Sql = Sql.Replace("ourreportformat", "OURReportFormat")
                Sql = Sql.Replace("ourreportlists", "OURReportLists")
                Sql = Sql.Replace("ourreportchildren", "OURReportChildren")
                Sql = Sql.Replace("OUR.ScheduledReports", "OURScheduledReports")
                Sql = Sql.Replace("ourscheduledreports", "OURScheduledReports")
                Sql = Sql.Replace("OURSCHEDULEDREPORTS", "OURScheduledReports")
                Sql = Sql.Replace("ourtasklistsetting", "OurTaskListSetting")
                Sql = Sql.Replace("OURTASKLISTSETTING", "OurTaskListSetting")
                Sql = Sql.Replace("OUR.TaskListSetting", "OurTaskListSetting")
                Sql = Sql.Replace("ourkmlhistory", "OURKMLHistory")
                Sql = Sql.Replace("OURKMLHISTORY", "OURKMLHistory")
                Sql = Sql.Replace("OUR.KMLHistory", "OURKMLHistory")

                'OURComparison
                Sql = Sql.Replace("OurComparison", "OURComparison")
                Sql = Sql.Replace("OURCOMPARISON", "OURComparison")
                Sql = Sql.Replace("OURcomparison", "OURComparison")
                Sql = Sql.Replace("ourcomparison", "OURComparison")

                Sql = Sql.Replace("""""", """").Replace("""""", """")
                If Not Sql.ToUpper.Contains("FROM INFORMATION_SCHEMA") Then
                    'correct tables and fields in Sql with double quotes
                    Sql = CorrectSQLforPostgreWithDoublequotes(Sql, dbcase, myconstring, "Npgsql")
                End If
                Sql = Sql.Replace("""""", """").Replace("""""", """")
            Else
                Sql = Sql.Replace("[", "").Replace("]", "").Replace("`", "")


                Sql = Sql.Replace("OURHelpDesk", "ourhelpdesk")
                Sql = Sql.Replace("OURAccessLog", "ouraccesslog")
                Sql = Sql.Replace("OURDashboards", "ourdashboards")
                Sql = Sql.Replace("OURKMLHistory", "ourkmlhistory")
                Sql = Sql.Replace("OURPermits", "ourpermits")
                Sql = Sql.Replace("OURPermissions", "ourpermissions")
                Sql = Sql.Replace("OURReportInfo", "ourreportinfo")
                Sql = Sql.Replace("OURReportShow", "ourreportshow")
                Sql = Sql.Replace("OURReportSQLquery", "ourreportsqlquery")
                Sql = Sql.Replace("OURReportGroups", "ourreportgroups")
                Sql = Sql.Replace("OURReportFormat", "ourreportformat")
                Sql = Sql.Replace("OURReportLists", "ourreportlists")
                Sql = Sql.Replace("OURReportChildren", "ourreportchildren")
                Sql = Sql.Replace("OURFriendlyNames", "ourfriendlynames")
                Sql = Sql.Replace("OURFiles", "ourfiles")
                Sql = Sql.Replace("OURUnits", "ourunits")
                Sql = Sql.Replace("OURUserTables", "ourusertables")
                Sql = Sql.Replace("OURUSERTABLES", "ourusertables")
                Sql = Sql.Replace("OURAgents", "ouragents")
                Sql = Sql.Replace("OURAGENTS", "ouragents")
                Sql = Sql.Replace("OURActivity", "ouractivity")
                Sql = Sql.Replace("OURACTIVITY", "ouractivity")
                Sql = Sql.Replace("OURUNITS", "ourunits")
                Sql = Sql.Replace("OURFILES", "ourfiles")
                Sql = Sql.Replace("OURFRIENDLYNAMES", "ourfriendlynames")
                Sql = Sql.Replace("OURHELPDESK", "ourhelpdesk")
                Sql = Sql.Replace("OURACCESSLOG", "ouraccessLog")
                Sql = Sql.Replace("OURDASHBOARDS", "ourdashboards")
                Sql = Sql.Replace("OURKMLHISTORY", "ourkmlhistory")
                Sql = Sql.Replace("OURPERMITS", "ourpermits")
                Sql = Sql.Replace("OURPERMISSIONS", "ourpermissions")
                Sql = Sql.Replace("OURREPORTINFO", "ourreportinfo")
                Sql = Sql.Replace("OURREPORTSHOW", "ourreportshow")
                Sql = Sql.Replace("OURREPORTSQLQUERY", "ourreportsqlquery")
                Sql = Sql.Replace("OURREPORTGROUPS", "ourreportgroups")
                Sql = Sql.Replace("OURREPORTFORMAT", "ourreportformat")
                Sql = Sql.Replace("OURREPORTLISTS", "ourreportlists")
                Sql = Sql.Replace("OURREPORTCHILDREN", "ourreportchildren")
                Sql = Sql.Replace("OURTASKLISTSETTING", "ourtasklistsetting")
                Sql = Sql.Replace("OurTaskListSetting", "ourtasklistsetting")
                Sql = Sql.Replace("OurScheduledReports", "ourscheduledreports")
                Sql = Sql.Replace("OURScheduledReports", "ourscheduledreports")
                Sql = Sql.Replace("OURSCHEDULEDREPORTS", "ourscheduledreports")
                'OURComparison
                Sql = Sql.Replace("OURComparison", "ourcomparison")
                Sql = Sql.Replace("OurComparison", "ourcomparison")
                Sql = Sql.Replace("OURcomparison", "ourcomparison")
                Sql = Sql.Replace("OURCOMPARISON", "ourcomparison")
            End If


        Catch exc As Exception
            er = exc.Message
            Sql = ""
        End Try
        Return Sql
    End Function
    Public Function CorrectSQLforPostgreWithDoublequotes(ByVal mSql As String, ByVal dbcase As String, Optional ByVal myconstring As String = "", Optional ByVal myprovider As String = "") As String
        Dim ret As String = String.Empty
        Dim er As String = String.Empty
        Dim fromstmt As String = String.Empty
        Dim fieldsarr As String = String.Empty
        Dim fldname As String = String.Empty
        Dim vals As String = String.Empty
        Dim wherestmt As String = String.Empty
        Dim i, k As Integer
        Try
            If dbcase.Trim <> "doublequoted" Then
                Return mSql
            End If
            If myconstring.Trim = "" Then
                myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            If myprovider.Trim = "" Then
                myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            End If
            If myprovider.Trim <> "Npgsql" Then
                Return mSql
            End If
            'CorrectWhereHavingFromSQLquery, CorrectFieldsArrayFromSQLquery, CorrectTablesArrayFromSQLquery
            If mSql.TrimStart.ToUpper.StartsWith("INSERT ") Then
                'has only array of fields msql = "INSERT INTO " & Tbl & " (" & mfields & ") VALUES ("
                fromstmt = mSql.Substring(11, mSql.IndexOf("(") - 11).Trim
                fieldsarr = mSql.Substring(mSql.IndexOf("("), mSql.IndexOf(")") - mSql.IndexOf("(")).Replace("(", "").Replace(")", "").Trim
                vals = mSql.Substring(mSql.IndexOf(")"))
                fieldsarr = CorrectFieldsArrayFromSQLquery("SELECT " & fieldsarr & " FROM " & fromstmt, myconstring, myprovider, er)
                ret = "INSERT INTO """ & fromstmt & """ (" & fieldsarr & vals
            ElseIf mSql.ToUpper.TrimStart.StartsWith("UPDATE ") Then
                'has sets and where
                fromstmt = mSql.Substring(7, mSql.IndexOf(" SET ")).Trim  'table name from UPDATE query
                Dim dtb As New DataTable
                dtb.TableName = "Table1"
                dtb.Columns.Add("Tbl1")
                Dim myRow As DataRow
                myRow = dtb.NewRow
                myRow.Item(0) = fromstmt
                dtb.Rows.Add(myRow)

                wherestmt = String.Empty
                If mSql.IndexOf(" WHERE ") > 0 OrElse mSql.ToUpper.IndexOf(" HAVING ") > 0 Then
                    If mSql.ToUpper.IndexOf(" WHERE ") > 0 Then
                        wherestmt = mSql.Substring(mSql.IndexOf(" WHERE ") + 6)
                    ElseIf mSql.ToUpper.IndexOf(" HAVING ") > 0 Then
                        wherestmt = mSql.Substring(mSql.IndexOf(" HAVING ") + 7)
                    End If
                    wherestmt = CorrectWhereHavingFromSQLquery(wherestmt, dtb, myconstring, myprovider, er)
                End If
                fieldsarr = mSql.Substring(mSql.IndexOf(" SET ") + 4)
                Dim fieldsvals() As String = fieldsarr.Split(",")
                For i = 0 To fieldsvals.Length - 1
                    fldname = """" & Piece(fieldsvals(i), "=", 1).Trim & """"
                    vals = Piece(fieldsvals(i), "=", 2)
                    fieldsvals(i) = fldname & vals
                Next
                fieldsarr = ""
                For i = 0 To fieldsvals.Length - 1
                    fieldsarr = fieldsarr & fieldsvals(i)
                    If i < fieldsvals.Length - 1 Then
                        fieldsarr = fieldsarr & ", "
                    End If
                Next
                ret = "UPDATE """ & fromstmt & """ SET " & fieldsarr
                If wherestmt.Trim <> "" Then
                    ret = ret & " WHERE " & wherestmt
                End If

            ElseIf mSql.ToUpper.TrimStart.StartsWith("SELECT ") Then
                'has arrays of fields for selected fields, order by, group by and where or having statement
                'Dim dt As DataTable = GetListOfTablesFromSQLquery(mSql, myconstring, myprovider, er)
                'If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                '    For i = 0 To dt.Rows.Count - 1
                '        If Not dt.Rows(i)("Tbl1").ToString.Trim.StartsWith("""") OrElse Not dt.Rows(i)("Tbl1").ToString.Trim.EndsWith("""") Then
                '            mSql = ReplaceWholeWord(mSql, dt.Rows(i)("Tbl1"), """" & dt.Rows(i)("Tbl1").trim & """")
                '            mSql = mSql.Replace("""""", """")
                '        End If
                '    Next
                'End If

                'Fix JOINS ON statements
                mSql = CorrectTablesJoinsFromSQLquery(mSql, myconstring, myprovider, er)
                fieldsarr = CorrectFieldsArrayFromSQLquery(mSql, myconstring, myprovider, er)
                wherestmt = String.Empty
                If mSql.IndexOf(" WHERE ") > 0 OrElse mSql.ToUpper.IndexOf(" HAVING ") > 0 Then
                    If mSql.ToUpper.IndexOf(" WHERE ") > 0 Then
                        wherestmt = mSql.Substring(mSql.IndexOf(" WHERE ") + 7)
                    ElseIf mSql.ToUpper.IndexOf(" HAVING ") > 0 Then
                        wherestmt = mSql.Substring(mSql.IndexOf(" HAVING ") + 8)
                    End If
                    wherestmt = CorrectWhereHavingFromSQLquery(wherestmt, dt, myconstring, myprovider, er)
                End If
                'group by fields
                Dim groupbyflds As String = String.Empty
                If mSql.ToUpper.IndexOf("GROUP BY") > 0 Then
                    groupbyflds = mSql.Substring(mSql.ToUpper.IndexOf("GROUP BY") + 8)

                    k = groupbyflds.ToUpper.IndexOf(" ORDER BY")
                    If k > 0 Then
                        groupbyflds = groupbyflds.Substring(0, k)
                    End If
                    k = groupbyflds.ToUpper.IndexOf(" WHERE ")
                    If k > 0 Then
                        groupbyflds = groupbyflds.Substring(0, k)
                    End If
                    k = groupbyflds.ToUpper.IndexOf(" HAVING ")
                    If k > 0 Then
                        groupbyflds = groupbyflds.Substring(0, k)
                    End If
                    groupbyflds = CorrectFieldsArrayFromSQLquery(groupbyflds, myconstring, myprovider, er)
                End If
                'order by fields
                Dim orderbyflds As String = String.Empty
                If mSql.ToUpper.IndexOf("ORDER BY") > 0 Then
                    orderbyflds = mSql.Substring(mSql.ToUpper.IndexOf("ORDER BY") + 8).Trim
                    Dim ord As String = orderbyflds.Substring(orderbyflds.LastIndexOf(" "))
                    If ord.Trim.ToUpper = "ASC" OrElse ord.Trim.ToUpper = "DESC" Then
                        orderbyflds = orderbyflds.Substring(0, orderbyflds.LastIndexOf(" ")).Trim
                        orderbyflds = CorrectFieldsArrayFromSQLquery(orderbyflds, myconstring, myprovider, er)
                        orderbyflds = orderbyflds & " " & ord
                    Else
                        orderbyflds = CorrectFieldsArrayFromSQLquery(orderbyflds, myconstring, myprovider, er)
                    End If
                End If
                fromstmt = mSql.Substring(mSql.IndexOf(" FROM ") + 6).Trim
                k = fromstmt.ToUpper.IndexOf(" WHERE ")
                If k > 0 Then
                    fromstmt = fromstmt.Substring(0, k)
                End If
                k = fromstmt.ToUpper.IndexOf(" HAVING ")
                If k > 0 Then
                    fromstmt = fromstmt.Substring(0, k)
                End If
                k = fromstmt.ToUpper.IndexOf(" GROUP BY ")
                If k > 0 Then
                    fromstmt = fromstmt.Substring(0, k)
                End If
                k = fromstmt.ToUpper.IndexOf(" ORDER BY ")
                If k > 0 Then
                    fromstmt = fromstmt.Substring(0, k)
                End If

                If mSql.ToUpper.StartsWith("SELECT ") Then
                    ret = "SELECT "
                End If
                If mSql.ToUpper.StartsWith("SELECT DISTINCT ") Then
                    ret = "SELECT DISTINCT "
                End If
                ret = ret & fieldsarr & " FROM " & fromstmt

                If groupbyflds.Trim <> "" Then
                    ret = ret & " GROUP BY " & groupbyflds
                End If

                If wherestmt.Trim <> "" Then
                    ret = ret & " WHERE " & wherestmt
                End If

                If orderbyflds.Trim <> "" Then
                    ret = ret & " ORDER BY " & orderbyflds
                End If
            End If

        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Public Function ConvertFromMySqlToOracle(ByVal mSql As String) As String
        Dim er As String = ""
        Dim rSql As String = ""
        Try
            rSql = mSql.Replace("`", """")
            'Dim sqlparts() As String = mSql.Split("`")
            'If sqlparts.Length > 0 Then
            '    For i = 0 To sqlparts.Length - 1
            '        rSql = rSql & " " & sqlparts(i)
            '        If i < sqlparts.Length - 1 Then
            '            rSql = rSql & "[" & sqlparts(i + 1) & "]"
            '            i = i + 1
            '        End If
            '    Next
            'End If
            'remove TOP N
            'rSql = rSql.Replace(" TOP ", " FIRST ")
            Dim t As Integer = rSql.ToUpper.IndexOf(" TOP ")
            If t > 0 AndAlso rSql.Trim.ToUpper.StartsWith("SELECT ") Then
                Dim temp As String = rSql.Substring(t + 5).Trim
                t = temp.IndexOf(" ")
                If t > 0 Then
                    rSql = "SELECT " & temp.Substring(t + 1)
                End If
            End If
        Catch exc As Exception
            er = exc.Message
            rSql = mSql
        End Try
        Return rSql.Trim
    End Function
    Public Function ConvertFromCacheToOracle(ByVal mSql As String) As String
        Dim er As String = ""
        Dim rSql As String = ""
        Try
            'rSql = mSql.Replace("""", "")
            'remove TOP N
            Dim t As Integer = rSql.ToUpper.IndexOf(" TOP ")
            If t > 0 AndAlso rSql.Trim.ToUpper.StartsWith("SELECT ") Then
                Dim temp As String = rSql.Substring(t + 5).Trim
                t = temp.IndexOf(" ")
                If t > 0 Then
                    rSql = "SELECT " & temp.Substring(t + 1)
                End If
            End If
        Catch exc As Exception
            er = exc.Message
            rSql = ""
        End Try
        Return rSql
    End Function
    Public Function ConvertFromSqlServerToOracle(ByVal mSql As String) As String
        Dim er As String = ""
        Dim rSql As String = ""
        Try
            rSql = mSql.Replace("[", "").Replace("]", "")
            rSql = mSql.Replace("`", "")
            'remove TOP N
            Dim t As Integer = rSql.ToUpper.IndexOf(" TOP ")
            If t > 0 AndAlso rSql.Trim.ToUpper.StartsWith("SELECT ") Then
                Dim temp As String = rSql.Substring(t + 5).Trim
                t = temp.IndexOf(" ")
                If t > 0 Then
                    rSql = "SELECT " & temp.Substring(t + 1)
                End If
            End If
        Catch exc As Exception
            er = exc.Message
            rSql = ""
        End Try
        Return rSql
    End Function
    Public Function DatabaseConnected(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "", Optional ByRef er As String = "", Optional ByRef userODBCdriver As String = "", Optional ByRef userODBCdatabase As String = "", Optional ByRef userODBCdatasource As String = "") As Boolean
        Dim r As Boolean = False
        Dim myconstring, myprovider As String
        Dim dbcase As String = String.Empty
        Try
            If myconstr.Trim = "" Then
                myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
                myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                If myprovider = "Npgsql" Then dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
            Else
                If myconprv = "Npgsql" Then
                    If System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing AndAlso myconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("csvdbcase").ToString
                    ElseIf myconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
                    Else 'userdbcase
                        dbcase = userdbcase
                    End If
                End If
                myconstring = myconstr
                myprovider = myconprv
            End If

            If myprovider = "InterSystems.Data.IRISClient" Then
                r = DatabaseConnected_IRIS(myconstring, er)
            ElseIf myprovider = "InterSystems.Data.CacheClient" Then
                r = DatabaseConnected_Cache(myconstring, er)
            ElseIf myprovider = "MySql.Data.MySqlClient" Then
                Dim myConnection As MySqlConnection
                myConnection = New MySqlConnection(myconstring)
                Try
                    If myConnection.State = ConnectionState.Closed Then myConnection.Open()
                    If myConnection.State = ConnectionState.Open Then r = True
                    myConnection.Close()
                    myConnection.Dispose()
                Catch ex As Exception
                    myConnection.Close()
                    myConnection.Dispose()
                    er = ex.Message
                    r = False
                End Try
            ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then
                r = DatabaseConnected_Oracle(myconstring, er)
            ElseIf myprovider = "System.Data.Odbc" Then
                myconstring = myconstring.Replace("Password", "Pwd").Replace("User ID", "UID")
                Dim myConnection As System.Data.Odbc.OdbcConnection
                myConnection = New System.Data.Odbc.OdbcConnection(myconstring)
                Try
                    If myConnection.State = ConnectionState.Closed Then myConnection.Open()
                    If myConnection.State = ConnectionState.Open Then r = True
                    userODBCdriver = myConnection.Driver.ToString
                    userODBCdatabase = myConnection.Database.ToString
                    userODBCdatasource = myConnection.DataSource.ToString
                    myConnection.Close()
                    myConnection.Dispose()
                Catch ex As Exception
                    myConnection.Close()
                    myConnection.Dispose()
                    er = ex.Message
                    r = False
                End Try
            ElseIf myprovider = "System.Data.OleDb" Then
                myconstring = ConfigurationManager.AppSettings("ACEOLEDBversion").ToString & "Persist Security Info=True;" & myconstring
                Dim myConnection As System.Data.OleDb.OleDbConnection
                myConnection = New System.Data.OleDb.OleDbConnection(myconstring)
                Try
                    If myConnection.State = ConnectionState.Closed Then myConnection.Open()
                    If myConnection.State = ConnectionState.Open Then r = True
                    userODBCdriver = myConnection.Provider.ToString
                    userODBCdatabase = myConnection.Database.ToString
                    userODBCdatasource = myConnection.DataSource.ToString
                    myConnection.Close()
                    myConnection.Dispose()
                Catch ex As Exception
                    myConnection.Close()
                    myConnection.Dispose()
                    er = ex.Message
                    r = False
                End Try
            ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
                myconstring = CorrectConnstringForPostgres(myconstring, dbcase)
                Dim myConnection As Npgsql.NpgsqlConnection
                myConnection = New Npgsql.NpgsqlConnection(myconstring)
                Try
                    If myConnection.State = ConnectionState.Closed Then myConnection.Open()
                    If myConnection.State = ConnectionState.Open Then
                        r = True
                    Else
                        r = False
                    End If
                    myConnection.Close()
                    myConnection.Dispose()
                Catch ex As Exception
                    myConnection.Close()
                    myConnection.Dispose()
                    r = False
                    er = ex.Message
                End Try
            Else
                Dim myConnection As SqlConnection
                myConnection = New SqlConnection(myconstring)
                Try
                    If myConnection.State = ConnectionState.Closed Then myConnection.Open()
                    If myConnection.State = ConnectionState.Open Then
                        r = True
                    Else
                        r = False
                    End If
                    myConnection.Close()
                    myConnection.Dispose()
                Catch ex As Exception
                    myConnection.Close()
                    myConnection.Dispose()
                    r = False
                    er = ex.Message
                End Try
            End If
        Catch exc As Exception
            r = False
            er = exc.Message
        End Try
        Return r
    End Function
    Public Function ConvertFromSqlServerToODBC(ByVal SQLq As String, ByVal userconnstr As String, ByVal userconnprv As String) As String
        'the original query should be made in the Sql Server format
        'we don't know what kind of db is behind ODBC, we check syntax for the main ones
        Dim er As String = String.Empty
        Dim userODBCdriver As String = String.Empty
        Dim userODBCdatabase As String = String.Empty
        Dim userODBCdatasource As String = String.Empty
        userconnstr = userconnstr.Replace("Password", "Pwd").Replace("User ID", "UID")
        Dim bConnect As Boolean = DatabaseConnected(userconnstr, userconnprv, er, userODBCdriver, userODBCdatabase, userODBCdatasource)
        If Not bConnect Then
            er = "ERROR!! Database not connected. " & er
            Return er
        End If
        Dim sqlf As String = SQLq
        If userODBCdriver.ToUpper.StartsWith("CACHE") OrElse userODBCdriver.StartsWith("IRIS") Then
            sqlf = ConvertFromSqlServerToCache(SQLq)
        ElseIf userODBCdriver.ToUpper.StartsWith("MYSQL") Then
            sqlf = ConvertFromSqlServerToMySql(SQLq)
        ElseIf userODBCdriver.ToUpper.StartsWith("ORACLE") Then
            sqlf = ConvertFromSqlServerToOracle(SQLq)
        ElseIf userODBCdriver.ToUpper.StartsWith("PSQL") Then
            userconnstr = CorrectConnstringForPostgres(userconnstr, userdbcase)
            sqlf = ConvertFromSqlServerToPostgres(SQLq, userdbcase, userconnstr, userconnprv)
        End If
        Return sqlf
    End Function
    Public Sub DeleteReportItem(ReportID As String, ItemID As String)
        Dim sqls As String = "Delete From OurReportItems Where ReportID = '" & ReportID & "' And ItemID = '" & ItemID & "'"
        Dim ret As String = String.Empty
        ret = ExequteSQLquery(sqls)
    End Sub
    Public Function ExequteSQLquery(ByVal SQLq As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim r As String
        r = "Query executed fine."
        Dim myconstring, myprovider As String
        Dim dbcase As String = String.Empty
        Try
            If myconstr.Trim = "" Then
                myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
                myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                If myprovider = "Npgsql" Then dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
            Else
                If myconprv = "Npgsql" Then
                    If System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing AndAlso myconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("csvdbcase").ToString
                    ElseIf myconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
                    ElseIf myconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection").ToString.Trim.ToUpper Then
                        dbcase = ConfigurationManager.AppSettings("userdbcase").ToString
                    Else 'postgres, etc...
                        dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
                    End If
                End If
                myconstring = myconstr
                myprovider = myconprv
            End If
            If myprovider = "InterSystems.Data.IRISClient" Then
                SQLq = CorrectSQLforCache(SQLq)
                r = ExequteSQLquery_IRIS(SQLq, myconstring)
                If r <> "Query executed fine." Then Return r
            ElseIf myprovider = "InterSystems.Data.CacheClient" Then
                SQLq = CorrectSQLforCache(SQLq)
                r = ExequteSQLquery_Cache(SQLq, myconstring)
                If r <> "Query executed fine." Then Return r
            ElseIf myprovider = "MySql.Data.MySqlClient" Then
                SQLq = CorrectSQLforMySql(SQLq, myconstring)
                Dim myConnection As MySqlConnection
                Dim myCommand As New MySqlCommand
                myConnection = New MySqlConnection(myconstring)
                Try
                    myCommand.Connection = myConnection
                    myCommand.CommandType = CommandType.Text
                    myCommand.CommandTimeout = 300000
                    myCommand.CommandText = SQLq
                    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                    myCommand.ExecuteNonQuery()
                    myCommand.Connection.Close()
                    myCommand.Dispose()
                    myConnection.Dispose()
                Catch ex As Exception
                    myCommand.Connection.Close()
                    myCommand.Dispose()
                    myConnection.Dispose()
                    r = ex.Message
                    Return r
                End Try
            ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
                myconstring = CorrectConnstringForPostgres(myconstring, dbcase)
                SQLq = ConvertFromSqlServerToPostgres(SQLq, dbcase, myconstring, myprovider)
                Dim myConnection As Npgsql.NpgsqlConnection
                Dim myCommand As New Npgsql.NpgsqlCommand
                myConnection = New Npgsql.NpgsqlConnection(myconstring)
                Try
                    myCommand.Connection = myConnection
                    myCommand.CommandType = CommandType.Text
                    myCommand.CommandTimeout = 300000
                    myCommand.CommandText = SQLq
                    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                    myCommand.ExecuteNonQuery()
                    myCommand.Connection.Close()
                    myCommand.Dispose()
                    myConnection.Dispose()
                Catch ex As Exception
                    myCommand.Connection.Close()
                    myCommand.Dispose()
                    myConnection.Dispose()
                    r = ex.Message
                    Return r
                End Try
            ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then
                SQLq = CorrectSQLforOracle(SQLq, myconstring)
                r = ExequteSQLquery_Oracle(SQLq, myconstring)
                If r <> "Query executed fine." Then Return r
            ElseIf myprovider = "System.Data.Odbc" Then
                myconstring = myconstring.Replace("Password", "Pwd").Replace("User ID", "UID")
                SQLq = CorrectSQLforSQLServer(SQLq)
                SQLq = ConvertFromSqlServerToODBC(SQLq, myconstring, myprovider)
                Dim myConnection As System.Data.Odbc.OdbcConnection
                Dim myCommand As New System.Data.Odbc.OdbcCommand
                myConnection = New System.Data.Odbc.OdbcConnection(myconstring)
                Try
                    myCommand.Connection = myConnection
                    myCommand.CommandType = CommandType.Text
                    myCommand.CommandTimeout = 300000
                    myCommand.CommandText = SQLq
                    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                    myCommand.ExecuteNonQuery()
                    myCommand.Connection.Close()
                    myCommand.Dispose()
                    myConnection.Dispose()
                Catch ex As Exception
                    myCommand.Connection.Close()
                    myCommand.Dispose()
                    myConnection.Dispose()
                    r = ex.Message
                    Return r
                End Try
            ElseIf myprovider = "Sqlite" Then
                SQLq = CorrectSQLforSQLServer(SQLq).Replace("UCASE(", "UPPER(")   '.Replace("[", "").Replace("]", "")
                Dim myCommand As New SqliteCommand
                Try
                    myCommand.Connection = sqliteconn
                    myCommand.CommandType = CommandType.Text
                    myCommand.CommandTimeout = 300000
                    myCommand.CommandText = SQLq
                    myCommand.ExecuteNonQuery()
                    myCommand.Dispose()
                Catch ex As Exception
                    myCommand.Dispose()
                    r = ex.Message
                    Return r
                End Try
            ElseIf myprovider = "System.Data.OleDb" Then
                SQLq = CorrectSQLforSQLServer(SQLq)
                Dim myConnection As SqlConnection
                Dim myCommand As New SqlClient.SqlCommand
                myConnection = New SqlConnection(myconstring)
                Try
                    myCommand.Connection = myConnection
                    myCommand.CommandType = CommandType.Text
                    myCommand.CommandTimeout = 300000
                    myCommand.CommandText = SQLq
                    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                    myCommand.ExecuteNonQuery()
                    myCommand.Connection.Close()
                    myCommand.Dispose()
                    myConnection.Dispose()
                Catch ex As Exception
                    myCommand.Connection.Close()
                    myCommand.Dispose()
                    myConnection.Dispose()
                    r = ex.Message
                    Return r
                End Try
            Else
                SQLq = CorrectSQLforSQLServer(SQLq)
                Dim myConnection As SqlConnection
                Dim myCommand As New SqlClient.SqlCommand
                myConnection = New SqlConnection(myconstring)
                Try
                    myCommand.Connection = myConnection
                    myCommand.CommandType = CommandType.Text
                    myCommand.CommandTimeout = 300000
                    myCommand.CommandText = SQLq
                    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                    myCommand.ExecuteNonQuery()
                    myCommand.Connection.Close()
                    myCommand.Dispose()
                    myConnection.Dispose()
                Catch ex As Exception
                    myCommand.Connection.Close()
                    myCommand.Dispose()
                    myConnection.Dispose()
                    r = ex.Message
                    Return r
                End Try
            End If
        Catch exc As Exception
            r = exc.Message
            Return r
        End Try
        Return r
    End Function
    Public Function GetAllUserTablesWithField(ByVal fld As String, ByVal logon As String, Optional ByVal bcsv As String = "", Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional ByRef er As String = "") As DataView
        Dim dv As DataView = Nothing
        Dim dvr As DataView = Nothing
        Dim dvc As DataView = Nothing
        Dim dvg As DataView = Nothing
        Dim sqls As String = String.Empty
        Dim sqlf As String = String.Empty
        Dim msql As String = String.Empty
        Dim r As Boolean
        If userconstr = "" Then 'no user connection string
            userconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            userconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        End If
        Dim userdb As String = userconstr
        If userconprv <> "Oracle.ManagedDataAccess.Client" Then
            If userdb.ToUpper.IndexOf("USER ID") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("USER ID")).Trim
        Else
            'userdb = Piece(userdb, ";", 1, 2) & ";"
            If userdb.ToUpper.IndexOf("PASSWORD") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("PASSWORD")).Trim
        End If

        Try
            If bcsv = "yes" Then
                sqlf = "SELECT Tbl1 AS TABLE_NAME,Tbl1Fld1 AS COLUMN_NAME FROM OURReportSQLquery INNER JOIN OURReportInfo ON (OURReportSQLquery.ReportId=OURReportInfo.ReportId) INNER JOIN OURUserTables ON (OURUserTables.Tablename=OURReportSQLquery.Tbl1)  WHERE OURUserTables.UserId='" & logon & "'  AND Doing='SELECT' AND Tbl1Fld1 = '" & fld & "' AND ReportDB LIKE '%" & userdb.Trim.Replace(" ", "%") & "%'"

                'sqlf = "SELECT Tbl1 AS TABLE_NAME,Tbl1Fld1 AS COLUMN_NAME FROM OURReportSQLquery INNER JOIN OURReportInfo INNER JOIN OURUserTables ON OURReportSQLquery.ReportId=OURReportInfo.ReportId AND OURUserTables.Tablename=OURReportSQLquery.Tbl1  WHERE OURUserTables.UserId='" & logon & "'  AND Doing='SELECT' AND Tbl1Fld1 = '" & fld & "' AND ReportDB LIKE '%" & userdb & "%'"
                'sqlf = "SELECT Tbl1 AS TABLE_NAME,Tbl1Fld1 AS COLUMN_NAME FROM OURReportSQLquery INNER JOIN OURUserTables ON OURUserTables.TableName=OURReportSQLquery.Tbl1  WHERE OURUserTables.UserId='" & logon & "'  AND Doing='SELECT' AND Tbl1Fld1 = '" & fld & "'"

                dv = mRecords(sqlf, er)  'from ourdb !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Else
                If userconprv.StartsWith("InterSystems.Data.") Then
                    sqlf = "SELECT parent AS TABLE_NAME,Name AS COLUMN_NAME FROM %Dictionary.PropertyDefinition WHERE Name = '" & fld & "' AND Cardinality Is NULL"
                ElseIf userconprv = "MySql.Data.MySqlClient" Then
                    Dim db As String = GetDataBase(userconstr, userconprv)
                    sqlf = "SELECT TABLE_NAME,COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA ='" & db.ToLower & "' AND COLUMN_NAME = '" & fld & "'"
                ElseIf userconprv = "Oracle.ManagedDataAccess.Client" Then
                    'Dim db As String = GetDataBase(userconstr)
                    'sqlf = "SELECT TABLE_NAME,COLUMN_NAME FROM all_tab_cols WHERE OWNER ='" & db & "' AND COLUMN_NAME = '" & fld & "'"
                    sqlf = "SELECT TABLE_NAME,COLUMN_NAME FROM all_tab_cols WHERE COLUMN_NAME = '" & fld & "'"
                ElseIf userconprv = "System.Data.Odbc" Then
                    'for ODBC 
                    er = ""
                    dv = GetAllUserTablesWithColumnODBC(fld, userconstr, userconprv, er)
                ElseIf userconprv = "System.Data.OleDb" Then
                    'for OleDb
                    er = ""
                    dv = GetAllUserTablesWithColumnODBC(fld, userconstr, userconprv, er)

                ElseIf userconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    Dim db As String = GetDataBase(userconstr, userconprv)
                    sqlf = "SELECT TABLE_NAME,COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE LOWER(TABLE_CATALOG) ='" & db.ToLower & "' AND TABLE_SCHEMA ='public' AND COLUMN_NAME = '" & fld & "'"

                Else
                    sqlf = "SELECT TABLE_NAME,COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = '" & fld & "'"
                End If
                If userconprv <> "System.Data.Odbc" AndAlso userconprv <> "System.Data.OleDb" Then 'dv is not fill out yet
                    dv = mRecords(sqlf, er, userconstr, userconprv) ' list of tables from user db that include the field !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
                End If

            End If

            Dim dt As DataTable = dv.Table  'from user db all table,field for the selected field
            dt.Columns.Add("GlobalFriendlyName")
            dt.Columns.Add("Reports")
            dt.Columns.Add("ReportDataFieldFriendlyName")
            dt.Columns.Add("ReportColumnFriendlyName")
            dt.Columns.Add("ReportGroupFriendlyName")
            dt.Columns.Add("ReportParameterFriendlyName")

            'ReportDataFieldFriendlyName  OURReportSQLquery
            If bcsv = "yes" Then
                msql = "SELECT OURReportInfo.ReportId,Doing,Tbl1,Tbl1Fld1,Friendly FROM OURReportSQLquery INNER JOIN OURReportInfo ON (OURReportSQLquery.ReportId=OURReportInfo.ReportId) INNER JOIN OURUserTables ON (OURUserTables.Tablename=OURReportSQLquery.Tbl1)  WHERE OURUserTables.UserId='" & logon & "'  AND Doing='SELECT' AND Tbl1Fld1 = '" & fld & "' AND ReportDB LIKE '%" & userdb.Trim.Replace(" ", "%") & "%'"
                'msql = "SELECT OURReportInfo.ReportId,Doing,Tbl1,Tbl1Fld1,Friendly FROM OURReportSQLquery INNER JOIN OURUserTables ON OURUserTables.TableName=OURReportSQLquery.Tbl1  WHERE OURUserTables.UserId='" & logon & "'  AND Doing='SELECT' AND Tbl1Fld1 = '" & fld & "'"

            Else
                msql = "SELECT OURReportInfo.ReportId,Doing,Tbl1,Tbl1Fld1,Friendly FROM OURReportSQLquery INNER JOIN OURReportInfo ON (OURReportSQLquery.ReportId=OURReportInfo.ReportId) WHERE Doing='SELECT' AND Tbl1Fld1 = '" & fld & "' AND ReportDB LIKE '%" & userdb.Trim.Replace(" ", "%") & "%'"
            End If
            dvr = mRecords(msql, er) 'from ourdb !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 
            If dvr Is Nothing OrElse dvr.Table Is Nothing OrElse er <> "" Then
                r = True  'not data field fld in reports in OURReportSQLquery
            End If
            Dim dtr As DataTable = dvr.Table
            Dim nl As String = ""  '"<br/>" '& Chr(13) & Chr(10)
            'update dt 
            Dim i, j, n As Integer
            For i = 0 To dt.Rows.Count - 1
                dt.Rows(i)("GlobalFriendlyName") = GlobalFriendlyName(dt.Rows(i)("TABLE_NAME"), dt.Rows(i)("COLUMN_NAME"), userdb, er)
                n = 0
                If r = False Then    'ReportDataFieldFriendlyName
                    'dt.Rows(i)("Reports") = dt.Rows(i)("Reports") & nl
                    For j = 0 To dtr.Rows.Count - 1
                        If dt.Rows(i)("TABLE_NAME") = dtr.Rows(j)("Tbl1") AndAlso dt.Rows(i)("COLUMN_NAME") = dtr.Rows(j)("Tbl1Fld1") Then
                            n = n + 1
                            nl = n.ToString & "."
                            If n > 1 Then
                                nl = "<br>" & nl
                            End If
                            dt.Rows(i)("Reports") = dt.Rows(i)("Reports") & nl & dtr.Rows(j)("ReportId")
                            dt.Rows(i)("ReportDataFieldFriendlyName") = dt.Rows(i)("ReportDataFieldFriendlyName") & nl & dtr.Rows(j)("Friendly")
                            dt.Rows(i)("ReportColumnFriendlyName") = dt.Rows(i)("ReportColumnFriendlyName") & nl & ReportColumnFriendlyName(dtr.Rows(j)("ReportId"), dtr.Rows(j)("Tbl1Fld1"))
                            dt.Rows(i)("ReportGroupFriendlyName") = dt.Rows(i)("ReportGroupFriendlyName") & nl & ReportGroupFriendlyName(dtr.Rows(j)("ReportId"), dtr.Rows(j)("Tbl1Fld1"))
                            dt.Rows(i)("ReportParameterFriendlyName") = dt.Rows(i)("ReportParameterFriendlyName") & nl & ReportParameterFriendlyName(dtr.Rows(j)("ReportId"), dtr.Rows(j)("Tbl1Fld1"))
                        End If
                    Next
                End If
            Next

            Return dt.DefaultView
        Catch ex As Exception
            er = ex.Message
            Return Nothing
        End Try

    End Function
    Public Function GetAllUserTablesWithColumnODBC(ByVal fld As String, ByVal userconstr As String, ByVal userconprv As String, Optional ByRef err As String = "") As DataView
        Dim dv As DataView = Nothing
        ' Dim dataConnection As New System.Data.Odbc.OdbcConnection(userconstr)
        If userconprv = "System.Data.Odbc" Then
            Dim dataConnection As New System.Data.Odbc.OdbcConnection(userconstr)
            Try
                If dataConnection.State = ConnectionState.Closed Then dataConnection.Open()
                Dim dtsh As New DataTable
                dtsh = dataConnection.GetSchema("Columns")
                dv = dtsh.DefaultView

                dv.RowFilter = "COLUMN_NAME='" & fld & "'"

                Return dv.ToTable.DefaultView
            Catch ex As Exception
                dataConnection.Close()
                dataConnection.Dispose()
                Return Nothing
            End Try
        ElseIf userconprv = "System.Data.OleDb" Then
            userconstr = ConfigurationManager.AppSettings("ACEOLEDBversion").ToString & "Persist Security Info=True;" & userconstr
            Dim dataConnection As New System.Data.OleDb.OleDbConnection(userconstr)
            Try
                If dataConnection.State = ConnectionState.Closed Then dataConnection.Open()
                Dim dtsh As New DataTable
                dtsh = dataConnection.GetSchema("Columns")
                dv = dtsh.DefaultView

                dv.RowFilter = "COLUMN_NAME='" & fld & "'"

                Return dv.ToTable.DefaultView
            Catch ex As Exception
                dataConnection.Close()
                dataConnection.Dispose()
                Return Nothing
            End Try
        End If
        Return dv
    End Function
    Public Function ReportParameterFriendlyName(ByVal rep As String, ByVal fild As String) As String
        Dim er As String = String.Empty
        Dim msql As String = "SELECT ReportId,DropDownID,DropDownLabel FROM OURReportShow WHERE DropDownID  = '" & fild & "' AND ReportId = '" & rep & "'"
        Dim dvc As DataView = mRecords(msql, er) 'from ourdata
        If dvc Is Nothing OrElse dvc.Table Is Nothing OrElse dvc.Table.Rows.Count = 0 OrElse er <> "" Then
            Return ""
        End If
        Return dvc.Table.Rows(0)("DropDownLabel")
    End Function
    Public Function ReportColumnFriendlyName(ByVal rep As String, ByVal fild As String) As String
        Dim er As String = String.Empty
        Dim msql As String = "SELECT ReportId,Prop,Val,Prop1 FROM OURReportFormat WHERE Prop='FIELDS' AND Val = '" & fild & "' AND ReportId = '" & rep & "'"
        Dim dvc As DataView = mRecords(msql, er) 'from ourdata
        If dvc Is Nothing OrElse dvc.Table Is Nothing OrElse dvc.Table.Rows.Count = 0 OrElse er <> "" Then
            Return ""
        End If
        Return dvc.Table.Rows(0)("Prop1")
    End Function

    Public Function ReportGroupFriendlyName(ByVal rep As String, ByVal fild As String) As String
        Dim er As String = String.Empty
        Dim msql As String = "SELECT ReportId,GroupField,comments FROM OURReportGroups WHERE GroupField = '" & fild & "' AND ReportId = '" & rep & "'"
        Dim dvg As DataView = mRecords(msql, er) 'from ourdata
        If dvg Is Nothing OrElse dvg.Table Is Nothing OrElse dvg.Table.Rows.Count = 0 OrElse er <> "" Then
            Return ""
        End If
        Return dvg.Table.Rows(0)("comments")
    End Function
    Public Function GlobalFriendlyName(ByVal tbl As String, ByVal fld As String, ByVal userdb As String, Optional ByRef err As String = "") As String
        Dim sqlf As String = String.Empty
        sqlf = "SELECT Friendly FROM OURFriendlyNames WHERE TableName = '" & tbl & "' AND FieldName = '" & fld & "' AND UnitDB = '" & userdb & "'"
        Dim dvn As DataView = mRecords(sqlf, err) 'from ourdata
        If err <> "" OrElse dvn Is Nothing OrElse dvn.Table Is Nothing OrElse dvn.Table.Rows.Count = 0 Then
            Return ""
        End If
        Return dvn.Table.Rows(0)("Friendly").ToString
    End Function

    Public Function GetListOfGroupTables(ByVal unit As String, ByVal grp As String) As DataView
        Dim dv As DataView = Nothing
        Dim er As String = String.Empty
        Dim sqls = "SELECT TableName As TABLE_NAME,Prop1 AS TableName FROM OURUserTables WHERE Unit='" & unit & "' AND [Group] = '" & grp & "' ORDER BY TableName"
        dv = mRecords(sqls, er)
        Return dv
    End Function

    Public Function GetListOfUserTables(ByVal syschk As Boolean, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional ByRef err As String = "", Optional ByVal logon As String = "", Optional ByVal csvuser As String = "") As DataView
        Dim dv As DataView = Nothing
        Dim i As Integer
        Dim sqls = String.Empty
        Dim unit = String.Empty
        Dim grps = String.Empty
        Dim ourprovider As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("MySqlConnection").ProviderName.ToString
        grps = UserGroups(logon, userconstr, userconprv, unit, err)
        If userconstr = "" OrElse csvuser = "yes" Then 'no user connection string
            userconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("MySqlConnection").ToString
            userconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("MySqlConnection").ProviderName.ToString
        End If
        Dim dtt As New DataTable
        If (csvuser = "yes" OrElse IsCSVuser(userconstr, userconprv, err) = "yes") AndAlso ourprovider <> "Sqlite" Then
            'sqls = "SELECT [TableName]  As TABLE_NAME,[Prop1] AS TableName FROM [OURUserTables] WHERE [Unit]='CSV' AND [UserID]='" & logon & "' ORDER BY TableName"
            sqls = "SELECT [TableName]  As TABLE_NAME,[Prop1] AS TableName FROM [OURUserTables] WHERE [UserID]='" & logon & "' ORDER BY TableName"
            dv = mRecords(sqls, err)
            dv.Sort = "TABLE_NAME ASC"
            dtt = dv.ToTable
            Return dtt.DefaultView
        ElseIf grps.Trim <> "" AndAlso grps.Trim.ToUpper <> "ALL" AndAlso ourprovider <> "Sqlite" Then
            'for Unit the OURUserTables located in ourdb
            sqls = "SELECT [TableName]  As TABLE_NAME,[Prop1] AS TableName FROM [OURUserTables] WHERE [Unit]='" & unit & "' AND [Group] IN ('" & grps & "') ORDER BY TableName"
            err = ""
            dv = mRecords(sqls, err)
            dv.Sort = "TABLE_NAME ASC"
            dtt = dv.ToTable
            Return dtt.DefaultView
        Else
            If userconprv.StartsWith("InterSystems.Data.") Then
                If syschk = False OrElse UserRole(logon) <> "super" Then 'not show system tables
                    sqls = "Select ID As TABLE_NAME FROM %Dictionary.ClassDefinition WHERE Not (ID %STARTSWITH '%') AND Not (ID LIKE '%OUR.%')  AND Not (ID LIKE '%MPU.%')  AND Not (ID LIKE '%INFORMATION_SCHEMA.%') AND (Super='%Library.Persistent' OR Super='%Persistent')"
                ElseIf UserRole(logon) = "super" Then
                    sqls = "Select ID As TABLE_NAME FROM %Dictionary.ClassDefinition WHERE ( (%Dictionary.ClassDefinition.ClassType = 'persistent' OR %Dictionary.ClassDefinition.Super LIKE '%%Persistent%') AND Not (ID LIKE '%OUR.%')  AND Not (ID LIKE '%MPU.%')) OR (Abstract=0 AND ID LIKE '%%Dictionary.%' AND Super LIKE '%%Persistent%')"
                End If
                sqls = sqls & " ORDER BY TABLE_NAME "
            ElseIf userconprv = "MySql.Data.MySqlClient" Then
                Dim db As String = GetDataBase(userconstr, userconprv)
                If syschk = False OrElse UserRole(logon) <> "super" Then 'not show system tables
                    sqls = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA ='" & db.ToLower & "' ORDER BY TABLE_NAME"
                ElseIf UserRole(logon) = "super" Then                    'show system tables
                    sqls = "SELECT * FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_NAME"
                End If
            ElseIf userconprv = "Oracle.ManagedDataAccess.Client" Then
                'always show user tables for oracle
                sqls = "SELECT TABLE_NAME FROM user_tables ORDER BY TABLE_NAME"
            ElseIf userconprv = "System.Data.Odbc" Then
                'DO NOT DELETE - for user tables from ourusertables
                'Dim userconnstrnopass As String = userconstr
                'If userconnstrnopass.IndexOf("Password") > 0 Then userconnstrnopass = userconnstrnopass.Substring(0, userconnstrnopass.IndexOf("Password")).Trim()
                'If userconnstrnopass.ToUpper.IndexOf("USER ID") > 0 Then userconnstrnopass = userconnstrnopass.Substring(0, userconnstrnopass.ToUpper.IndexOf("USER ID")).Trim()
                'sqls = "SELECT TableName  As TABLE_NAME,Prop1 AS TableName FROM OURUserTables WHERE UserID='" & logon & "' AND UserDB LIKE '%" & userconnstrnopass & "%' ORDER BY TableName"
                'userconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("MySqlConnection").ToString
                'userconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("MySqlConnection").ProviderName.ToString

                'ODBC
                Dim dtsh As New DataTable
                dtsh = GetListByODBC(System.Data.Odbc.OdbcMetaDataCollectionNames.Tables, userconstr, userconprv)
                dv = dtsh.DefaultView
                If syschk = False Then 'not show system tables
                    dv.RowFilter = "TABLE_TYPE='TABLE'"
                End If
                Dim dtr As DataTable = dv.ToTable
                For i = 0 To dtr.Rows.Count - 1
                    If dtr.Rows(i)("TABLE_SCHEM").ToString <> "" Then
                        dtr.Rows(i)("TABLE_NAME") = dtr.Rows(i)("TABLE_SCHEM") & "." & dtr.Rows(i)("TABLE_NAME")
                    End If
                Next
                dtr.DefaultView.Sort = "TableName ASC"
                Return dtr.DefaultView
            ElseIf userconprv = "System.Data.OleDb" Then
                'OleDb
                Dim dtsh As New DataTable
                dtsh = GetListByODBC(System.Data.OleDb.OleDbMetaDataCollectionNames.Tables, userconstr, userconprv)
                dv = dtsh.DefaultView
                If syschk = False Then 'not show system tables
                    dv.RowFilter = "TABLE_TYPE='TABLE'"
                End If
                Dim dtr As DataTable = dv.ToTable
                For i = 0 To dtr.Rows.Count - 1
                    If dtr.Rows(i)("TABLE_SCHEMA").ToString <> "" Then
                        dtr.Rows(i)("TABLE_NAME") = dtr.Rows(i)("TABLE_SCHEM") & "." & dtr.Rows(i)("TABLE_NAME")
                    End If
                Next
                dtr.DefaultView.Sort = "TableName ASC"
                Return dtr.DefaultView

            ElseIf userconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                Dim db As String = GetDataBase(userconstr, userconprv)
                If syschk = False OrElse UserRole(logon) <> "super" Then 'not show system tables
                    sqls = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA ='public' AND LOWER(TABLE_CATALOG) ='" & db.ToLower & "' ORDER BY TABLE_NAME"
                ElseIf UserRole(logon) = "super" Then                   'show system tables ?????!!!!!!!!!!!!!!! and other schemas???
                    sqls = "SELECT * FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_NAME"
                End If
            ElseIf userconprv = "System.Data.SqlClient" Then 'SQL Server
                Dim db As String = GetDataBase(userconstr, userconprv)
                If syschk = False OrElse UserRole(logon) <> "super" Then
                    sqls = "SELECT * From " & db & ".INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE' ORDER BY TABLE_NAME ASC"
                ElseIf UserRole(logon) = "super" Then
                    sqls = "SELECT * From " & db & ".INFORMATION_SCHEMA.TABLES ORDER BY TABLE_NAME ASC"
                End If
            ElseIf userconprv = "Sqlite" Then
                If syschk = False OrElse UserRole(logon) <> "super" Then
                    sqls = "SELECT name AS TABLE_NAME FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' AND name NOT LIKE 'OUR%'"
                ElseIf UserRole(logon) = "super" Then
                    sqls = "Select name As TABLE_NAME FROM sqlite_master WHERE type='table'"
                End If
            Else
                Dim db As String = GetDataBase(userconstr, userconprv)
                If syschk = False OrElse UserRole(logon) <> "super" Then
                    sqls = "SELECT * From " & db & ".INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE' ORDER BY TABLE_NAME ASC"
                ElseIf UserRole(logon) = "super" Then
                    sqls = "SELECT * From " & db & ".INFORMATION_SCHEMA.TABLES ORDER BY TABLE_NAME ASC"
                End If
            End If
        End If
        err = ""
        dv = mRecords(sqls, err, userconstr, userconprv)
        If Not dv Is Nothing Then
            dv.Sort = "TABLE_NAME ASC"
            dtt = dv.ToTable
            Return dtt.DefaultView
        Else
            Return Nothing
        End If

    End Function
    Public Function UserGroups(ByVal logon As String, ByVal userconstr As String, ByVal userconprv As String, ByRef unit As String, Optional ByRef er As String = "") As String
        Dim dv As DataView = Nothing
        Dim sqls = String.Empty
        Dim grps = String.Empty
        Try
            If userconstr.ToUpper.IndexOf("USER ID") > 0 AndAlso userconprv <> "Oracle.ManagedDataAccess.Client" Then
                userconstr = userconstr.Substring(0, userconstr.ToUpper.IndexOf("USER ID")).Trim
            ElseIf userconstr.ToUpper.IndexOf("PASSWORD") > 0 AndAlso userconprv = "Oracle.ManagedDataAccess.Client" Then
                userconstr = userconstr.Substring(0, userconstr.ToUpper.IndexOf("PASSWORD")).Trim
            End If

            sqls = "SELECT * FROM [OURPermits] WHERE [NetId]='" & logon & "' AND [ConnStr] LIKE '%" & userconstr.Replace(" ", "%") & "%'"
            dv = mRecords(sqls, er)
            If dv Is Nothing OrElse dv.Count = 0 OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then
                grps = ""
                unit = ""
                Return grps
            End If
            grps = dv.Table.Rows(0)("Group3").ToString
            unit = dv.Table.Rows(0)("Unit").ToString
        Catch ex As Exception
            er = ex.Message
            grps = ""
            unit = ""
            Return grps
        End Try
        Return grps
    End Function
    Public Function IsCacheClassPersistant(ByVal cls As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional ByRef err As String = "", Optional ByRef syschk As Boolean = False) As Boolean
        If userconprv.StartsWith("InterSystems.Data.") Then
            Dim sqls As String '= "Select * FROM %Dictionary.ClassDefinition WHERE ID='" & cls & "' AND Not (ID %STARTSWITH '%') AND Not (ID %STARTSWITH 'OUR.') AND (Super='%Library.Persistent' OR Super='%Persistent')"
            If syschk = False Then 'not show system tables
                sqls = "Select * FROM %Dictionary.ClassDefinition WHERE ID='" & cls & "' AND Not (ID %STARTSWITH '%') AND Not (ID LIKE '%OUR.%')  AND Not (ID LIKE '%MPU.%')  AND Not (ID LIKE '%INFORMATION_SCHEMA.%') AND (Super='%Library.Persistent' OR Super='%Persistent' OR ClassType = 'persistent')"
            Else
                sqls = "Select ID As TABLE_NAME FROM %Dictionary.ClassDefinition WHERE ID='" & cls & "' AND ((( %Dictionary.ClassDefinition.ClassType = 'persistent' OR Super LIKE '%%Persistent%') AND Not (ID LIKE '%OUR.%')  AND Not (ID LIKE '%MPU.%')) OR (Abstract=0 AND ID LIKE '%%Dictionary.%' AND Super LIKE '%%Persistent%'))"
            End If
            If HasRecords(sqls, userconstr, userconprv) Then
                Return True
            Else
                Return False
            End If
        End If
        Return False
    End Function
    Public Function GetDistinctDataViewWithProperCase(ByVal sqlDistinct As String, ByRef dtdist As DataView, Optional myconnstring As String = "", Optional myprovider As String = "") As String
        Dim er As String = String.Empty
        If sqlDistinct.ToUpper.IndexOf(" DISTINCT ") < 0 Then
            Return er
        End If
        Dim col As String = String.Empty
        Dim sqls As String = String.Empty
        Dim FLD As String = String.Empty
        Dim fild As String = String.Empty
        Dim dt As DataView
        If myprovider.Trim = "" Then
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            myconnstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
        End If
        Try
            sqls = sqlDistinct.Replace("DISTINCT ", "").Replace("distinct ", "")
            If sqls.ToUpper.IndexOf(" DISTINCT ") > 0 Then
                sqls = sqls.Substring(0, sqls.ToUpper.IndexOf(" DISTINCT ")) & " " & sqls.Substring(sqls.ToUpper.IndexOf(" DISTINCT ") + 9)
            End If
            dt = mRecords(sqls, er, myconnstring, myprovider)
            Dim dattbl As DataTable = dt.ToTable(True)
            dtdist = dattbl.DefaultView
            Return er
            'dt.Table.CaseSensitive = False
            'If dtdist IsNot Nothing AndAlso dtdist.Table.Rows.Count > 0 Then
            '    If myprovider.StartsWith("InterSystems.Data.") Then
            '        'DISTINCT returns upper case in string fields, fixing dtdist
            '        For j = 0 To dtdist.Table.Columns.Count - 1
            '            If Not ColumnTypeIsNumeric(dtdist.Table.Columns(j)) Then
            '                For i = 0 To dtdist.Table.Rows.Count - 1
            '                    col = dtdist.Table.Columns(j).Caption
            '                    FLD = dtdist.Table.Rows(i)(j)  'upper case
            '                    'dt.RowFilter = "(UCASE(er))=" & "'" & FLD & "'"    'cannot recognize UCASE in InterSystems
            '                    'If Not dt.ToTable Is Nothing AndAlso dt.ToTable.Rows.Count > 0 Then
            '                    '    dtdist.Table.Rows(i)(j) = dt.ToTable.Rows(0)(j)
            '                    'End If
            '                    'dt.RowFilter = ""
            '                    sqls = sqlDistinct.Replace(" DISTINCT ", " TOP 1 ").Replace(" distinct ", " TOP 1 ")
            '                    If sqls.ToUpper.IndexOf(" DISTINCT ") > 0 Then 'still has Distinct in mix case
            '                        sqls = sqlDistinct.Substring(0, sqlDistinct.ToUpper.IndexOf(" DISTINCT ")) & " TOP 1 " & sqlDistinct.Substring(sqlDistinct.ToUpper.IndexOf(" DISTINCT ") + 9)
            '                    End If
            '                    If sqls.ToUpper.IndexOf(" ORDER BY ") > 0 Then
            '                        sqls = sqls.Substring(0, sqls.ToUpper.IndexOf(" ORDER BY "))
            '                    End If
            '                    If sqls.ToUpper.IndexOf(" GROUP BY ") > 0 Then
            '                        sqls = sqls.Substring(0, sqls.ToUpper.IndexOf(" GROUP BY "))
            '                    End If
            '                    If sqls.ToUpper.IndexOf(" HAVING") > 0 Then
            '                        sqls = sqls.Substring(0, sqls.ToUpper.IndexOf(" HAVING"))
            '                    End If
            '                    If sqls.ToUpper.IndexOf(" WHERE ") > 0 Then
            '                        sqls = sqls.Substring(0, sqls.ToUpper.IndexOf(" WHERE ") + 7) & " UCASE(" & col & ")='" & FLD.ToUpper & "' AND " & sqls.Substring(sqls.ToUpper.IndexOf(" WHERE ") + 7)
            '                    Else
            '                        sqls = sqls & " WHERE (UCASE(" & col & ")='" & FLD.ToUpper & "')"
            '                    End If
            '                    dt = mRecords(sqls, er, myconnstring, myprovider)
            '                    If er.Trim = "" AndAlso Not dt.ToTable Is Nothing AndAlso dt.ToTable.Rows.Count > 0 Then
            '                        dtdist.Table.Rows(i)(j) = dt.Table.Rows(0)(j)
            '                    End If
            '                Next
            '            End If
            '        Next
            '    End If
            '    Return er
            'End If
        Catch ex As Exception
            er = "ERROR!! " & ex.Message
            Return Nothing
        End Try
        Return Nothing
    End Function
    Public Function GetReportTables(ByVal rep As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "") As DataTable
        Dim dt As DataTable = Nothing
        Dim i As Integer
        Dim myprovider, sqls, tbl As String
        sqls = "SELECT DISTINCT Tbl1,Tbl1 AS TABLE_NAME FROM OURReportSQLquery WHERE Doing='SELECT' AND ReportId = '" & rep & "' ORDER BY Tbl1"
        dt = mRecords(sqls).Table  'from OUR db
        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            If myprovider.StartsWith("InterSystems.Data.") Then
                'DISTINCT returns upper case, fixing dt
                Dim dtemp As DataTable = Nothing
                For i = 0 To dt.Rows.Count - 1
                    tbl = dt.Rows(i)("Tbl1")  'upper case
                    sqls = "Select TOP 1 Tbl1 FROM OURReportSQLquery WHERE UCASE(Tbl1)='" & tbl & "' "
                    dtemp = mRecords(sqls).Table  'from OUR db
                    If Not dtemp Is Nothing AndAlso dtemp.Rows.Count > 0 Then
                        dt.Rows(i)("Tbl1") = dtemp.Rows(0)("Tbl1")
                    End If
                Next
            End If
            Return dt
        Else
            'get tables from sqlquerytext
            dt = GetReportTablesFromSQLqueryText(rep, userconstr, userconprv)
            Return dt
        End If
    End Function
    Public Function GetReportImages(ReportID As String) As DataTable
        Dim dv As DataView = Nothing
        Dim dt As DataTable = Nothing
        Dim sqls As String = String.Empty
        Dim err As String = String.Empty
        If ReportID <> String.Empty Then
            sqls = "Select * From ourreportitems Where ReportID = '" & ReportID & "' AND ReportItemType = 'Image'  "
            sqls &= "Order By ItemOrder "
            dv = mRecords(sqls, err)
            If dv IsNot Nothing AndAlso dv.Table IsNot Nothing Then
                dt = dv.Table
            End If
        End If
        Return dt
    End Function
    Public Function GetReportItems(ReportID As String) As DataTable
        Dim dv As DataView = Nothing
        Dim dt As DataTable = Nothing
        Dim sqls As String = String.Empty
        Dim err As String = String.Empty

        If ReportID <> String.Empty Then
            sqls = "Select * From ourreportitems Where ReportID = '" & ReportID & "' "
            sqls &= "Order By ItemOrder "
            dv = mRecords(sqls, err)
            If dv IsNot Nothing AndAlso dv.Table IsNot Nothing Then
                dt = dv.Table
            End If
        End If
        Return dt
    End Function
    Public Function GetReportView(ReportID As String) As DataTable
        Dim dv As DataView = Nothing
        Dim dt As DataTable = Nothing
        Dim sqls As String = String.Empty
        Dim err As String = String.Empty

        If ReportID <> String.Empty Then
            sqls = "Select * From ourreportview Where ReportID = '" & ReportID & "' "
            dv = mRecords(sqls, err)
            If dv IsNot Nothing AndAlso dv.Table IsNot Nothing Then
                dt = dv.Table
            End If
        End If
        Return dt
    End Function
    Public Function GetReportTablesFromSQLqueryText(ByVal rep As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "") As DataTable
        'get tables from sqlquerytext
        Dim dt As DataTable = Nothing
        Dim dri As DataTable = GetReportInfo(rep)
        If Not dri Is Nothing AndAlso dri.Rows.Count > 0 Then
            Dim sqlquerytxt As String = dri.Rows(0)("SQLquerytext").ToString
            dt = GetListOfTablesFromSQLquery(sqlquerytxt, userconstr, userconprv)
        End If
        'do NOT save tables!! - tables are saved only with fields
        Return dt
    End Function
    Public Function FixSelectedFields(ByVal rep As String, ByVal selfs As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        'add table names as needed
        Dim n, m, k As Integer
        Dim tabl As String = String.Empty
        Dim fild As String = String.Empty
        Dim selflds As String = String.Empty
        Dim myconnddbtype As String = myconprv
        If myconnddbtype = "" Then
            myconnddbtype = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        End If
        If myconprv.StartsWith("InterSystems.Data.") Then
            myconnddbtype = "InterSystems.Data."
        End If

        Dim ddt As DataTable = GetReportTablesFromSQLqueryText(rep)
        If ddt Is Nothing Then
            Return ""
        End If
        Dim ddf As DataTable
        Dim selfld As String() = selfs.Split(",")
        For m = 0 To selfld.Length - 1
            If selfld(m).Trim = "" Then
                Continue For
            End If
            If selfld(m).Trim = "*" Then
                selfld(m) = ""
                For n = 0 To ddt.Rows.Count - 1
                    tabl = ddt.Rows(n)("Tbl1")
                    ddf = GetListOfTableColumns(tabl, myconstr, myconprv).Table
                    For k = 0 To ddf.Rows.Count - 1
                        ''TODO remove fields with not SQL compatible types
                        'If myconnddbtype = "InterSystems.Data." And Not IsSqlCompatibleField(tabl, ddf.Rows(k)("COLUMN_NAME").ToString, myconstr, myconprv) Then
                        '    Continue For
                        'End If

                        If selfld(m) = String.Empty Then
                            selfld(m) = tabl & "." & FixReservedWords(ddf.Rows(k)("COLUMN_NAME").ToString, myconnddbtype)
                        Else
                            selfld(m) = selfld(m).Trim & ", " & tabl & "." & FixReservedWords(ddf.Rows(k)("COLUMN_NAME").ToString, myconnddbtype)
                        End If

                    Next
                Next

            ElseIf selfld(m).Trim.LastIndexOf(".") < 0 AndAlso selfld(m).Trim <> "*" Then
                If ddt.Rows.Count = 1 Then
                    tabl = ddt.Rows(0)("Tbl1")
                    ''TODO remove fields with not SQL compatible types
                    'If myconnddbtype = "InterSystems.Data." And Not IsSqlCompatibleField(tabl, selfld(m).Trim, myconstr, myconprv) Then
                    '    Continue For
                    'End If
                    tabl = ddt.Rows(0)("Tbl1")
                    selfld(m) = tabl & "." & selfld(m).Trim
                Else
                    'find field in tables
                    For n = 0 To ddt.Rows.Count - 1
                        tabl = ddt.Rows(n)("Tbl1")
                        ddf = GetListOfTableColumns(tabl, myconstr, myconprv).Table
                        For k = 0 To ddf.Rows.Count - 1
                            If ddf.Rows(k)("COLUMN_NAME") = selfld(m).Trim Then
                                ''TODO remove fields with not SQL compatible types
                                'If myconnddbtype = "InterSystems.Data." And Not IsSqlCompatibleField(tabl, selfld(m).Trim, myconstr, myconprv) Then
                                '    Continue For
                                'End If
                                selfld(m) = tabl & "." & FixReservedWords(selfld(m).Trim.ToString, myconnddbtype)
                                Exit For
                            End If
                        Next
                    Next
                End If
            ElseIf selfld(m).Trim.LastIndexOf(".") > 0 Then
                tabl = selfld(m).Trim.Substring(0, selfld(m).Trim.LastIndexOf(".")).Trim
                fild = selfld(m).Trim.Substring(selfld(m).Trim.LastIndexOf(".")).Replace(".", "").Trim
                If fild = "*" Then
                    selfld(m) = ""
                    ddf = GetListOfTableColumns(tabl, myconstr, myconprv).Table
                    For k = 0 To ddf.Rows.Count - 1
                        ''TODO remove fields with not SQL compatible types
                        'If myconnddbtype = "InterSystems.Data." And Not IsSqlCompatibleField(tabl, ddf.Rows(k)("COLUMN_NAME").ToString, myconstr, myconprv) Then
                        '    Continue For
                        'End If
                        If selfld(m) = String.Empty Then
                            selfld(m) = FixReservedWords(tabl, myconnddbtype) & "." & FixReservedWords(ddf.Rows(k)("COLUMN_NAME").ToString, myconnddbtype)
                        Else
                            selfld(m) = selfld(m).Trim & ", " & FixReservedWords(tabl, myconnddbtype) & "." & FixReservedWords(ddf.Rows(k)("COLUMN_NAME").ToString, myconnddbtype)
                        End If

                    Next
                Else
                    ''TODO remove fields with not SQL compatible types
                    'If myconnddbtype = "InterSystems.Data." And Not IsSqlCompatibleField(tabl, fild, myconstr, myconprv) Then
                    '    Continue For
                    'End If
                End If
            End If
            If selflds = String.Empty Then
                'selflds = FixReservedWords(selfld(m).Trim, myconnddbtype)
                selflds = selfld(m).Trim
            Else
                'selflds = selflds & "," & FixReservedWords(selfld(m).Trim, myconnddbtype)
                selflds = selflds & "," & selfld(m).Trim
            End If

        Next
        selflds = selflds.Replace(",,", ",")
        'If myconnddbtype = "Npgsql" Then
        '    selflds = selflds.Replace(" ", "").Replace("[", "").Replace("]", "")
        '    selflds = selflds.Replace(".", "].[").Replace(",", "],[")
        '    selflds = "[" & selflds & "]"
        'End If
        Return selflds
    End Function
    'Public Function GetListOfFieldsFromSQLquery(ByVal rep As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional ByRef err As String = "") As String
    '    'NOT IN USE
    '    Dim dt As New DataTable
    '    Dim Sql As String = String.Empty
    '    Dim selflds As String = String.Empty
    '    Sql = "Select SQLquerytext FROM OURReportInfo WHERE (ReportID='" & rep & "')"
    '    dt = mRecords(Sql).Table  'from OUR db
    '    If dt Is Nothing OrElse dt.Rows.Count = 0 Then
    '        Return ""
    '    End If
    '    Sql = dt.Rows(0)("SQLquerytext").ToString
    '    If Sql = "" Then
    '        Return ""
    '    End If
    '    'Dim dv As DataView
    '    Dim i, j As Integer
    '    Dim tbl As String = String.Empty
    '    Try
    '        i = Sql.ToUpper.IndexOf(" FROM ")
    '        Sql = Sql.Substring(6, i - 6).Trim
    '        Dim flds() As String
    '        flds = Sql.Split(",")
    '        For i = 0 To flds.Length - 1
    '            j = flds(i).ToUpper.IndexOf(" AS ")
    '            If j > 0 Then flds(i) = flds(i).Substring(0, j).Trim
    '            j = flds(i).ToUpper.IndexOf("(")
    '            If j > 0 Then flds(i) = flds(i).Substring(j).Trim
    '            flds(i) = flds(i).Replace(")", "")
    '            flds(i) = flds(i).Replace("(", "")
    '            selflds = selflds & "," & flds(i).Trim
    '        Next
    '        'add table name if needed
    '        selflds = FixSelectedFields(rep, selflds, userconstr, userconprv)
    '        Return selflds & ","
    '    Catch ex As Exception
    '        Return "ERROR!! " & ex.Message.ToString
    '    End Try
    'End Function
    Public Function GetFieldsFromSQLquery(ByVal Sql As String) As String
        Dim selflgs As String = String.Empty
        If Sql = "" Then
            Return ""
        End If
        Dim i, j As Integer
        Dim tbl As String = Piece(Sql.ToUpper, " FROM ", 2).Trim

        If tbl = "" Then Return ""

        Try
            i = Sql.ToUpper.IndexOf(" FROM ")
            Sql = Sql.Substring(6, i - 6).Trim
            If Sql = "" Then Return ""

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
                    selflgs = "," & flds(i).Trim
                Else
                    'fix is here
                    selflgs = selflgs & "," & flds(i).Trim
                End If
            Next
            Return selflgs & ","
        Catch ex As Exception
            Return "" '"ERROR!! " & ex.Message.ToString
        End Try
    End Function
    Public Function GetListOfSelectedFieldsFromSQLquery(ByVal rep As String) As String
        Dim dt As New DataTable
        Dim Sql As String = String.Empty
        Dim selflgs As String = String.Empty
        Sql = "Select SQLquerytext FROM OURReportInfo WHERE (ReportID='" & rep & "')"
        dt = mRecords(Sql).Table  'from OUR db
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            Return ""
        End If
        Sql = dt.Rows(0)("SQLquerytext").ToString
        If Sql = "" Then
            Return ""
        End If
        Dim i, j As Integer
        Dim tbl As String = Piece(Sql.ToUpper, " FROM ", 2).Trim

        If tbl = "" Then Return ""

        Try
            i = Sql.ToUpper.IndexOf(" FROM ")
            Sql = Sql.Substring(6, i - 6).Trim
            If Sql = "" Then Return ""

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
                    selflgs = "," & flds(i).Trim
                Else
                    'fix is here
                    selflgs = selflgs & "," & flds(i).Trim
                End If
            Next
            Return selflgs & ","
        Catch ex As Exception
            Return "" '"ERROR!! " & ex.Message.ToString
        End Try
    End Function
    Public Function GetListOfTablesFromSQLquery(ByVal sql As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "", Optional ByRef er As String = "") As DataTable
        Dim ret As String = String.Empty
        Dim dt As New DataTable
        If sql = "" Then
            Return dt
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
            dt.TableName = "Table1"
            dt.Columns.Add("Tbl1")
            Dim myRow As DataRow
            k = sql.ToUpper.IndexOf(" JOIN ")
            If k < 0 Then
                Dim ar = sql.Split(",")
                For j = 0 To ar.Length - 1
                    tbl = ar(j).ToLower
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
                        myRow = dt.NewRow
                        myRow.Item(0) = tbl
                        dt.Rows.Add(myRow)
                    End If
                Next

            Else
                sqlj = sql

                Dim htTable As New Hashtable

                If sqlj.Contains(" INNER ") Then sqlj = sqlj.Replace(" INNER ", "")
                If sqlj.Contains(" LEFT ") Then sqlj = sqlj.Replace(" LEFT ", "")
                If sqlj.Contains(" RIGHT ") Then sqlj = sqlj.Replace(" RIGHT ", "")
                If sqlj.Contains(" OUTER ") Then sqlj = sqlj.Replace(" OUTER ", "")
                sqlj = sqlj.Replace("JOIN", "|")

                'sqlj = sqlj.Replace(" INNER JOIN ", " JOIN ")
                'sqlj = sqlj.Replace(" LEFT JOIN ", " JOIN ")
                'sqlj = sqlj.Replace(" RIGHT JOIN ", " JOIN ")
                'sqlj = sqlj.Replace(" OUTER JOIN ", " JOIN ")
                'sqlj = sqlj.Replace(" JOIN ", "|")

                'after JOIN
                Dim ar = sqlj.Split("|")
                For j = 0 To ar.Length - 1
                    tbl = ar(j).Trim
                    If tbl.Length = 0 Then
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
                            tb(n) = tb(n).Replace("[", "").Replace("]", "")
                            tb(n) = tb(n).Replace("""", "")
                            tb(n) = tb(n).ToLower.Trim


                            'tbl = tbl.Replace("""", "")
                            'check if tbl is a table
                            If myconstr.Trim <> "" AndAlso Not TableExists(tb(n).Trim, myconstr, myconprv, er) Then
                                Continue For
                            Else
                                If htTable(tb(n)) = String.Empty Then
                                    htTable.Add(tb(n), "1")
                                    myRow = dt.NewRow
                                    myRow.Item(0) = tb(n).ToLower.Trim
                                    dt.Rows.Add(myRow)
                                End If
                            End If
                        Next
                        Continue For
                    End If
                    'remove ON statement
                    l = tbl.ToUpper.IndexOf(" ON ")
                    If l > 0 Then
                        tbl = tbl.Substring(0, l).Trim
                    End If
                    'table name tbl put in Row
                    tbl = tbl.Replace("`", "")
                    tbl = tbl.Replace("[", "").Replace("]", "")
                    tbl = tbl.Replace("""", "")
                    tbl = tbl.ToLower.Trim

                    'check if tbl is a table
                    If myconstr.Trim <> "" AndAlso Not TableExists(tbl.Trim, myconstr, myconprv, er) Then
                        Continue For
                    Else
                        If htTable(tbl) = String.Empty Then
                            myRow = dt.NewRow
                            myRow.Item(0) = tbl.ToLower
                            dt.Rows.Add(myRow)
                            htTable.Add(tbl, "1")
                        End If
                    End If
                Next
            End If
            Dim dv As New DataView
            dv = dt.DefaultView
            dv.Sort = "Tbl1"
            dt = dv.Table
            Return dt
        Catch ex As Exception
            er = ex.Message
            Return Nothing
        End Try
        Return dt
    End Function

    Public Function GetListOfJoinsFromSQLquery(ByVal repid As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional ByRef err As String = "") As DataTable
        Dim ret As String = String.Empty
        Dim dt As New DataTable
        Dim dri As DataTable = GetReportInfo(repid)
        If dri Is Nothing OrElse dri.Rows.Count = 0 Then
            Return dt
            Exit Function
        End If
        Dim sql As String = dri.Rows(0)("SQLquerytext").ToString
        If sql = "" Then
            Return dt
            Exit Function
        End If
        sql = sql.Replace("`", "").Replace("[", "").Replace("]", "").Replace("""", "")
        Dim ddt As DataTable = GetReportTablesFromSQLqueryText(repid)
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

            dt.TableName = "Table1"
            dt.Columns.Add("Tbl1")
            dt.Columns.Add("Tbl1Fld1")
            dt.Columns.Add("Tbl2")
            dt.Columns.Add("Tbl2Fld2")
            dt.Columns.Add("JoinType")
            dt.Columns.Add("Oper")

            'at this point here can be sets of Joins and single tables separated by commas
            Dim sqls() As String
            sqls = sql.Split(",")
            For q = 0 To sqls.Length - 1
                sql = sqls(q)
                If sql.ToUpper.IndexOf(" ON ") < 0 Then 'add single tables to tbls separated by commas
                    If tbls = "" Then
                        tbls = sql
                    Else
                        tbls = tbls & "," & sql
                    End If
                    Continue For
                End If
                'k = sql.ToUpper.LastIndexOf(",")
                'If k > 0 Then
                '    sql = sql.Substring(k).Replace(",", "").Trim
                'End If
                Dim oper As String = "1"
                'at this point sql is list of tables separated with JOINs, no commas
                Dim myRow As DataRow
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
                                tbl1 = FixTableName(tbl1, userconstr, userconprv, err)
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
                            tbl2 = FixTableName(tbl2, userconstr, userconprv, err)
                            sqlj = tbl.Substring(tbl.ToUpper.IndexOf(" ON ") + 4)
                            'there could be few ONs
                            Dim fdar() As String = Split(sqlj, " AND ")
                            For i = 0 To fdar.Length - 1
                                sqlon = fdar(i)
                                fld1 = sqlon.Substring(0, sqlon.IndexOf("="))
                                fld2 = sqlon.Substring(sqlon.IndexOf("="))
                                fld1 = FixFieldName(ddt, fld1, tbl1, userconstr, userconprv, err)
                                fld2 = FixFieldName(ddt, fld2, tbl2, userconstr, userconprv, err)
                                'fld1.Replace(tbl1 & ".", "")
                                '.Replace(tbl2 & ".", "")
                                myRow = dt.NewRow
                                myRow.Item(0) = tbl1
                                myRow.Item(1) = fld1
                                myRow.Item(2) = tbl2
                                myRow.Item(3) = fld2
                                myRow.Item(4) = typ
                                myRow.Item(5) = oper
                                dt.Rows.Add(myRow)
                                'insert
                                Dim jointype As String = typ
                                Dim insSQL As String = String.Empty
                                Dim sctSQL As String = "SELECT * FROM OURReportSQLquery WHERE (ReportID='" & repid & "' AND Doing='JOIN' AND Tbl1='" & tbl1 & "' AND Tbl1Fld1='" & fld1 & "' AND Tbl2='" & tbl2 & "' AND Tbl2Fld2='" & fld2 & "')"
                                If Not HasRecords(sctSQL) Then
                                    insSQL = "INSERT INTO OURReportSQLquery (ReportID, Doing,Oper, Tbl1, Tbl1Fld1, Tbl2, Tbl2Fld2, Comments) VALUES('" & repid & "','JOIN','" & oper & "','" & tbl1 & "','" & fld1 & "','" & tbl2 & "','" & fld2 & "','" & typ & "')"
                                    err = ExequteSQLquery(insSQL)
                                End If
                                oper = ""
                            Next
                            tbl1 = tbl2
                        End If
                    Next
                End If
            Next
            err = "Single Tables: " & tbls
            Return dt
        Catch ex As Exception
            err = "ERROR !! " & ex.Message
            Return Nothing
        End Try
        Return dt
    End Function
    Public Function UpdateJoins(ByVal repid As String, ByVal repjoins As String, ByVal userdb As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional ByRef err As String = "") As String
        Dim ret As String = String.Empty
        Try
            'Dim dtb As DataTable = GetReportTablesFromSQLqueryText(repid)
            Dim dtb As DataTable = GetReportJoins(repid)
            Dim reporttables As String = String.Empty
            Dim i As Integer
            For i = 0 To dtb.Rows.Count - 1
                err = err & " " & AddJoin(repjoins, dtb.Rows(i)("Tbl1"), dtb.Rows(i)("Tbl2"), dtb.Rows(i)("Tbl1Fld1"), dtb.Rows(i)("Tbl2Fld2"), "INNERJOIN", userdb, dtb.Rows(i)("Param2").ToString)
            Next

            'For i = 0 To dtb.Rows.Count - 1
            '    reporttables = reporttables & "'" & FixTableName(dtb.Rows(i)("Tbl1"), userconstr, userconprv, err) & "'"
            '    If i < dtb.Rows.Count - 1 Then
            '        reporttables = reporttables & ","
            '    End If
            'Next
            'If err = "" Then
            '    ret = reporttables
            '    'Select * from ourreportsqlquery where doing=?join? And dB=userdb and reportid endswith ?_joins? and tbl1 in (reporttables) and tbl2 In (reporttables)
            '    Dim sqlt As String = String.Empty
            '    'Dim userdb As String = userconstr.Substring(0, userconstr.IndexOf("User ID")).Trim
            '    Dim userdb As String = userconstr
            '    If userdb.ToUpper.IndexOf("USER ID") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("USER ID")).Trim
            '    If userdb.ToUpper.IndexOf("UID") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("UID")).Trim
            '    sqlt = "SELECT * FROM OURReportSQLquery WHERE Doing='JOIN' AND Param1 LIKE '%" & userdb.Trim.Replace(" ", "%") & "%' AND Tbl1 IN (" & reporttables & ") AND Tbl2 IN (" & reporttables & ")"
            '    Dim dtj As DataTable = mRecords(sqlt).Table
            '    'add preexisting joins 
            '    For i = 0 To dtj.Rows.Count - 1
            '        err = AddJoin(repid, dtj.Rows(i)("Tbl1"), dtj.Rows(i)("Tbl2"), dtj.Rows(i)("Tbl1Fld1"), dtj.Rows(i)("Tbl2Fld2"), "INNERJOIN", userdb, dtj.Rows(i)("Param2").ToString)
            '    Next
            'Else
            '    ret = ""
            'End If
        Catch ex As Exception
            err = err & " " & ex.Message
            ret = ""
        End Try
        Return ret
    End Function
    Public Function FixTableName(ByVal tbl As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional ByRef err As String = "") As String
        tbl = tbl.Replace("`", "")
        tbl = tbl.Replace("[", "").Replace("]", "").Trim
        tbl = tbl.Replace("""", "")
        tbl = tbl.Replace("(", "").Replace(")", "").Trim
        Dim dbcase As String = String.Empty
        If userconprv = "Npgsql" Then
            'If userconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("MySqlConnection").ToString.Trim.ToUpper Then
            '    dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
            'ElseIf userconstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString.Trim.ToUpper Then
            '    dbcase = ConfigurationManager.AppSettings("csvdbcase").ToString
            'Else 'userdbcase
            '    dbcase = userdbcase
            'End If
            tbl = FixReservedWords(tbl, userconprv, userconstr)
            Return tbl
        End If
        'get real table name from the list of report tables
        Dim ddt As DataTable = GetListOfUserTables(False, userconstr, userconprv, err).Table
        Dim i As Integer
        For i = 0 To ddt.Rows.Count - 1
            If ddt.Rows(i)("TABLE_NAME").ToString.ToUpper = tbl.ToUpper Then
                tbl = ddt.Rows(i)("TABLE_NAME").ToString
                Exit For
            End If
        Next
        tbl = FixReservedWords(tbl, userconprv, userconstr)
        Return tbl
    End Function
    Public Function FixFieldName(ByVal ddt As DataTable, ByVal fld As String, ByRef tabl As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional ByRef err As String = "") As String
        'If ddt Is Nothing OrElse ddt.Rows.Count = 0 Then
        '    ddt = GetReportTablesFromSQLqueryText(repid)
        'End If
        fld = fld.Replace("=", "")
        fld = fld.Replace("`", "")
        fld = fld.Replace("[", "").Replace("]", "").Trim
        fld = fld.Replace("""", "")
        fld = fld.Replace("(", "").Replace(")", "").Trim
        'get real field name from the list of report tables and their columns
        Dim ddf As DataTable
        Dim tbl As String = String.Empty
        If fld.IndexOf(".") > 0 Then
            tbl = fld.Substring(0, fld.LastIndexOf("."))
            fld = fld.Substring(fld.LastIndexOf(".") + 1)
            For i = 0 To ddt.Rows.Count - 1
                If ddt.Rows(i)("Tbl1").ToUpper = tbl.ToUpper Then
                    tabl = ddt.Rows(i)("Tbl1")
                    Exit For
                End If
            Next

            ddf = GetListOfTableColumns(tabl, userconstr, userconprv).Table
            For i = 0 To ddf.Rows.Count - 1
                If ddf.Rows(i)("COLUMN_NAME").ToUpper = fld.ToUpper Then
                    fld = ddf.Rows(i)("COLUMN_NAME")
                    Exit For
                End If
            Next
        Else
            'table name is missing, find table to the field
            For i = 0 To ddt.Rows.Count - 1
                ddf = GetListOfTableColumns(ddt.Rows(i)("Tbl1").ToString, userconstr, userconprv).Table
                For j = 0 To ddf.Rows.Count - 1
                    If ddf.Rows(j)("COLUMN_NAME").ToUpper = fld.ToUpper Then
                        fld = ddf.Rows(j)("COLUMN_NAME").ToString
                        tabl = ddt.Rows(i)("Tbl1").ToString
                        Exit For
                    End If
                Next
            Next
        End If
        'fld = tbl & "." & fld
        fld = FixReservedWords(fld, userconprv, userconstr)
        Return fld
    End Function
    Public Function FindTableToTheField(ByVal rep As String, ByVal fld As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional ByRef er As String = "") As String
        'table name is missing, find table to the field
        Dim tabl As String = String.Empty
        Try
            Dim ddt As DataTable = GetReportTablesFromSQLqueryText(rep)
            If ddt IsNot Nothing Then
                For i = 0 To ddt.Rows.Count - 1
                    Dim ddf As DataTable = GetListOfTableColumns(ddt.Rows(i)("Tbl1").ToString, userconstr, userconprv).Table
                    For j = 0 To ddf.Rows.Count - 1
                        If ddf.Rows(j)("COLUMN_NAME").ToUpper = fld.ToUpper Then
                            fld = ddf.Rows(j)("COLUMN_NAME").ToString
                            tabl = ddt.Rows(i)("Tbl1").ToString
                            Exit For
                        End If
                    Next
                Next
            End If
        Catch ex As Exception
            er = ex.Message
        End Try
        Return tabl
    End Function
    Public Function GetListOfOrderByFromSQLquery(ByVal repid As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional ByRef err As String = "") As DataTable
        'ORDER BY
        Dim ret As String = String.Empty
        Dim dt As New DataTable
        Dim dri As DataTable = GetReportInfo(repid)
        If dri Is Nothing OrElse dri.Rows.Count = 0 Then
            Return dt
            Exit Function
        End If
        Dim sql As String = dri.Rows(0)("SQLquerytext").ToString
        If sql = "" Then
            Return dt
            Exit Function
        End If
        Dim i, j, n, m, k As Integer
        k = sql.ToUpper.IndexOf(" ORDER BY ")
        If k < 0 Then
            Return dt
        End If
        Dim dtb As New DataTable
        dtb = GetListOfTablesFromSQLquery(sql, userconstr, userconprv, err)
        If sql = "" OrElse err <> "" Then
            Return dt
        End If
        Dim tbl As String = String.Empty
        Dim fld As String = String.Empty
        Dim typ As String = String.Empty
        Dim sqlj As String = String.Empty
        Try
            If k > 0 Then
                sql = sql.Substring(k + 10).Trim
            End If
            If sql.LastIndexOf(" ") > 0 Then
                If sql.Substring(sql.LastIndexOf(" ")).Trim = "DESC" Then
                    typ = "DESC"
                Else
                    typ = "ASC"
                End If
                sql = sql.Substring(0, sql.LastIndexOf(" ")).Trim
            Else
                typ = "ASC"
            End If
            'at this point sql is list of fields separated with commas
            dt.TableName = "Table1"
            dt.Columns.Add("Tbl1")
            dt.Columns.Add("Tbl1Fld1")
            dt.Columns.Add("Oper")
            dt.Columns.Add("Type")
            Dim myRow As DataRow
            Dim flds() As String = sql.Split(",")
            For i = 0 To flds.Length - 1
                fld = flds(i)
                If fld.Trim.Length = 0 Then
                    Continue For
                End If
                m = m + 1
                n = fld.LastIndexOf(".")
                If n > 0 Then
                    'table and field
                    tbl = fld.Substring(0, n)
                    fld = fld.Substring(n + 1)
                Else
                    'fld itself, to find the table
                    For j = 0 To dtb.Rows.Count - 1
                        tbl = dtb.Rows(j)("Tbl1")
                        If IsColumnFromTable(tbl, fld, userconstr, userconprv, err) Then
                            Exit For
                        End If
                    Next
                End If
                'table and field names fix and put in Row
                tbl = FixTableName(tbl, userconstr, userconprv, err)
                fld = FixFieldName(dtb, fld, tbl, userconstr, userconprv, err)
                myRow = dt.NewRow
                myRow.Item(0) = tbl
                myRow.Item(1) = fld
                myRow.Item(2) = m.ToString
                myRow.Item(3) = typ
                dt.Rows.Add(myRow)
                'insert 
                'Dim ret As String = String.Empty
                Dim sorttype As String = typ
                Dim insSQL As String = String.Empty
                Dim sctSQL As String = "SELECT * FROM OURReportSQLquery WHERE (ReportID='" & repid & "' AND Doing='ORDER BY' AND Tbl1='" & tbl & "' AND Tbl1Fld1='" & fld & "')"
                'If IsCacheDatabase() Then
                '    sctSQL = "SELECT * FROM OURReportSQLquery WHERE (ReportID='" & Session("REPORTID") & "' AND Doing='ORDER BY' AND UCASE(Tbl1)='" & UCase(DropDownTableS1.Text) & "' AND UCASE(Tbl1Fld1)='" & UCase(DropDownFieldS1.Text) & "' AND Comments='" & sorttype & "' ) "
                'End If
                If Not HasRecords(sctSQL) Then
                    'Oper will contain the order
                    insSQL = "INSERT INTO OURReportSQLquery (ReportID, Doing, Tbl1, Tbl1Fld1, Type, Oper) VALUES('" & repid & "','ORDER BY','" & tbl & "','" & fld & "','" & typ & "','" & m.ToString & "')"
                    ret = ExequteSQLquery(insSQL)
                End If
            Next
            Return dt
        Catch ex As Exception
            err = ex.Message
            Return Nothing
        End Try
        Return dt
    End Function
    Public Function IsDateTimeField(ByVal fldfullname As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional ByRef err As String = "") As Boolean
        Dim b As Boolean = False

        Return b
    End Function
    Public Function AdjustBetweens(sWhere As String) As String
        Dim sRet As String = sWhere
        Dim k As Integer = 0

        If sWhere.ToUpper.Contains("BETWEEN") Then
            k = sWhere.ToUpper.IndexOf(" WHERE ")
            If k > 0 Then
                sWhere = sWhere.Substring(k + 6).Trim
            End If
            k = sWhere.ToUpper.IndexOf(" ORDER BY ")
            If k > 0 Then
                sWhere = sWhere.Substring(0, k)
            End If
            k = sWhere.ToUpper.IndexOf("GROUP BY")
            If k > 0 Then
                sWhere = sWhere.Substring(0, k)
            End If
            k = sWhere.ToUpper.IndexOf("HAVING")
            If k > 0 Then
                sWhere = sWhere.Substring(0, k)
            End If
            k = sWhere.ToUpper.IndexOf(" BETWEEN ")
            Dim part1 As String = sWhere.Substring(0, k)
            Dim part2 As String = sWhere.Substring(k + 9)
            Dim i As Integer = part2.ToUpper.IndexOf(" AND ")
            Dim BeforeAnd As String = part2.Substring(0, i)
            Dim AfterAnd As String = part2.Substring(i + 5)
            If AfterAnd.ToUpper.Contains("BETWEEN") Then
                AfterAnd = AdjustBetweens(AfterAnd)
            End If
            sRet = part1 & " Between " & BeforeAnd & " & " & AfterAnd
        End If
        Return sRet
    End Function
    Public Function GetListOfConditionsFromSQLquery(ByVal repid As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional ByRef err As String = "") As DataTable
        Dim ret As String = String.Empty
        Dim dt As New DataTable
        Dim i, j, n, m, k, l As Integer
        Dim g As Integer = 0 'group number

        'check that report is defined
        Dim dri As DataTable = GetReportInfo(repid)
        If dri Is Nothing OrElse dri.Rows.Count = 0 Then
            Return dt
        End If

        Dim dtw As DataTable = GetSQLConditions(repid)
        If dtw IsNot Nothing AndAlso dtw.Rows.Count > 0 Then
            Return dt
        End If

        Dim sql As String = dri.Rows(0)("SQLquerytext").ToString
        If sql = "" OrElse sql.ToUpper.IndexOf(" WHERE ") = -1 Then
            Return dt
        End If

        Dim dtb As DataTable = GetListOfTablesFromSQLquery(sql, userconstr, userconprv, err)
        If err <> "" Then
            Return dt
        End If
        If dtb Is Nothing OrElse dtb.Rows.Count = 0 Then
            dtb = GetReportTablesFromSQLqueryText(repid)
        End If

        k = sql.ToUpper.IndexOf(" WHERE ")
        sql = sql.Substring(k + 6).Trim
        k = sql.ToUpper.IndexOf(" ORDER BY ")
        If k > 0 Then
            sql = sql.Substring(0, k).Trim
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

                    insSQL = "INSERT INTO OURReportSQLquery "
                    insSQL &= "(ReportID,Doing,RecOrder," & FixReservedWords("Group") & ",Logical )"
                    insSQL &= "VALUES('" & repid & "','"
                    insSQL &= "GROUP',"
                    insSQL &= m.ToString & ",'"
                    insSQL &= group & "','"
                    insSQL &= logical & "')"
                    ret = ExequteSQLquery(insSQL)
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
                fldfullname = FixTableAndField(dtb, tbl1, fld1, userconstr, userconprv, err)
                If fldfullname = "" Then
                    'lside is static
                    tbl2 = ""
                    fld2 = ""
                    sta = lside.Trim
                    'opr = oper
                    typ = "Static"
                    tbl1 = ""
                    fld1 = rside.Trim
                    fldfullname = FixTableAndField(dtb, tbl1, fld1, userconstr, userconprv, err)
                    If fldfullname = "" Then
                        ret = ret & "ERROR!! converting the condition: " & cond
                        Continue For
                    Else
                        tbl3 = ""
                        fld3 = ""
                    End If
                Else
                    If rside.ToUpper.Contains(" AND ") Then  'between
                        Dim b As Boolean = IsDateTimeField(fldfullname, userconstr, userconprv, err)
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
                            fldfullname = FixTableAndField(dtb, "", fld2, userconstr, userconprv, err)
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
                            fldfullname = FixTableAndField(dtb, tbl3, fld3, userconstr, userconprv, err)
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
                        fldfullname = FixTableAndField(dtb, tbl2, fld2, userconstr, userconprv, err)
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
                Dim fieldtype As String = GetFieldDataType(tbl1, fld1, userconstr, userconprv)
                Dim IsDate As Boolean = ((fieldtype.ToUpper <> "TIME") AndAlso (fieldtype.ToUpper.Contains("DATE") OrElse fieldtype.ToUpper.Contains("TIME")))

                Dim sorttype As String = typ

                'insSQL = "INSERT INTO OURReportSQLquery "
                'insSQL &= "SET ReportID='" & repid & "',"
                'insSQL &= "Doing='WHERE',"
                'insSQL &= "Tbl1='" & tbl1 & "',"
                'insSQL &= "Tbl1Fld1='" & fld1 & "',"
                'If tbl2 <> String.Empty Then
                '    insSQL &= "Tbl2='" & tbl2 & "',"
                '    insSQL &= "Tbl2Fld2='" & fld2 & "',"
                'End If
                'If tbl3 <> String.Empty Then
                '    insSQL &= "Tbl3='" & tbl3 & "',"
                '    insSQL &= "Tbl3Fld3='" & fld3 & "',"
                'End If
                'insSQL &= "Oper='" & opr & "',"
                'insSQL &= "Type='" & typ & "',"
                'If sta <> String.Empty Then _
                '       insSQL &= "comments='" & sta & "',"
                'insSQL &= "RecOrder=" & m.ToString & ","
                'If group <> String.Empty Then
                '    insSQL &= FixReservedWords("Group") & " = '" & group & "',"
                'End If
                'insSQL &= "Logical='" & logical & "'"
                insSQL = "INSERT INTO OURReportSQLquery "
                insSQL &= "(ReportID,Doing,Tbl1,Tbl1Fld1,"
                If tbl2 <> String.Empty Then
                    insSQL &= "Tbl2,Tbl2Fld2,"
                End If
                If tbl3 <> String.Empty Then
                    insSQL &= "Tbl3,Tbl2Fld3,"
                End If
                insSQL &= "Oper,Type,"
                If sta <> String.Empty Then _
                   insSQL &= "Comments,"
                insSQL &= "RecOrder,"
                If group <> String.Empty Then _
                   insSQL &= FixReservedWords("Group") & ","
                insSQL &= "Logical) "
                insSQL &= "VALUES('" & repid & "','"
                insSQL &= "WHERE','"
                insSQL &= tbl1 & "','"
                insSQL &= fld1 & "','"
                If tbl2 <> String.Empty Then
                    insSQL &= tbl2 & "','"
                    insSQL &= fld2 & "','"
                End If
                If tbl3 <> String.Empty Then
                    insSQL &= tbl3 & "','"
                    insSQL &= fld3 & "','"
                End If
                insSQL &= opr & "','"
                insSQL &= typ & "',"
                If sta <> String.Empty Then _
                   insSQL &= "'" & sta & "',"
                insSQL &= m.ToString & ",'"
                If group <> String.Empty Then _
                   insSQL &= group & "','"
                insSQL &= logical & "')"
                ret = ExequteSQLquery(insSQL)
            Next
            Return dt
        Catch ex As Exception
            err = ex.Message
            Return Nothing
        End Try
        Return dt
    End Function
    Public Function FixTableAndField(ByVal dtb As DataTable, ByRef tbl As String, ByRef fld As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional ByVal er As String = "") As String
        'dtb is list of report tables
        Dim ret As String = "Field Not found: "
        If fld = "" Then
            Return ""
        End If
        tbl = tbl.Replace("`", "")
        tbl = tbl.Replace("[", "").Replace("]", "").Trim
        tbl = tbl.Replace("""", "")
        tbl = tbl.Replace("(", "").Replace(")", "").Trim
        fld = fld.Replace("=", "")
        fld = fld.Replace("`", "")
        fld = fld.Replace("[", "").Replace("]", "").Trim
        fld = fld.Replace("""", "")
        fld = fld.Replace("(", "").Replace(")", "").Trim

        Dim sTable As String = tbl.ToUpper
        If sTable = "DATE1" OrElse sTable = "DATE2" OrElse sTable = "VALUE1" OrElse sTable = "VALUE2" Then _
            Return ""

        Dim j, n As Integer
        n = fld.LastIndexOf(".")
        If n > 0 Then
            'table and field
            tbl = fld.Substring(0, n)
            fld = fld.Substring(n + 1)
            ret = "Field found: "
        ElseIf n < 0 AndAlso tbl <> "" Then
            If Not IsColumnFromTable(tbl, fld, userconstr, userconprv, er) Then
                ' to find the table
                For j = 0 To dtb.Rows.Count - 1
                    tbl = dtb.Rows(j)("Tbl1")
                    If IsColumnFromTable(tbl, fld, userconstr, userconprv, er) Then
                        ret = "Field found: "
                        Exit For
                    End If
                Next
            End If
        Else
            'fld itself, to find the table
            For j = 0 To dtb.Rows.Count - 1
                tbl = dtb.Rows(j)("Tbl1")
                If IsColumnFromTable(tbl, fld, userconstr, userconprv, er) Then
                    ret = "Field found: "
                    Exit For
                End If
            Next
        End If
        If ret = "Field found: " Then
            'table and field names fix and put them back together separated with dot
            tbl = FixTableName(tbl, userconstr, userconprv, er)
            fld = FixFieldName(dtb, fld, tbl, userconstr, userconprv, er)
        Else
            Return ""
        End If
        Return tbl & "." & fld
    End Function
    Public Function GetListOfTablesFromSQLqueryOld(ByVal sql As String) As DataTable
        Dim ret As String = String.Empty
        Dim dt As New DataTable
        If sql = "" Then
            Return dt
        End If
        'Dim dv As DataView
        Dim i, j, k, l As Integer
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
            sql = sql.Trim & " "

            dt.TableName = "Table1"
            dt.Columns.Add("Tbl1")
            Dim myRow As DataRow
            k = sql.ToUpper.IndexOf(" JOIN")
            l = sql.Length

            If k > 5 Then
                sqlj = sql.Substring(k + 5).Trim
                sql = sql.Substring(0, k).Trim
            End If
            'before JOIN
            For i = 0 To l - 1
                If sql.Substring(i, 1) = " " OrElse sql.Substring(i, 1) = "," Then
                    If tbl.Length > 0 Then
                        'table name tbl put in Row
                        myRow = dt.NewRow
                        myRow.Item(0) = tbl.Trim
                        'dt.ImportRow(myRow)
                        dt.Rows.Add(myRow)
                        sql.Replace(tbl, tblspace)
                    End If
                    tbl = ""
                Else
                    tbl = tbl & sql.Substring(i, 1)    'add charachter to table name
                    tblspace = tblspace & "."
                End If
            Next
            'after JOIN
            Dim ar = sqlj.Split(" JOIN")
            For j = 0 To ar.Length - 1
                sql = ar(j)
                If sql.Trim.Length = 0 Then
                    Exit For
                End If
                For i = 0 To l - 1
                    If sql.Substring(i, 1) = " " Then
                        If tbl.Length > 0 Then
                            'table name tbl put in Row
                            myRow = dt.NewRow
                            myRow.Item(0) = tbl.Replace("(", "").Trim
                            'dt.ImportRow(myRow)
                            dt.Rows.Add(myRow)
                            sql.Replace(tbl, tblspace)
                        End If
                        tbl = ""
                    Else
                        tbl = tbl & sql.Substring(i, 1)    'add charachter to table name
                        tblspace = tblspace & "."
                    End If
                Next
            Next
            Return dt
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return dt
    End Function
    Public Function IsColumnFromTable(ByVal tbl As String, ByVal fld As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional ByRef err As String = "") As Boolean
        Dim re As Boolean = False
        Dim i As Integer
        Dim dv As DataView
        Try
            dv = GetListOfTableColumns(tbl, userconstr, userconprv, err)
            If dv Is Nothing OrElse dv.Count = 0 OrElse dv.Table.Rows.Count = 0 Then
                Return False
            Else
                For i = 0 To dv.Table.Rows.Count - 1
                    If dv.Table.Rows(i)("COLUMN_NAME").ToString.ToUpper = fld.ToUpper Then
                        Return True
                    End If
                Next
            End If
        Catch ex As Exception
            err = ex.Message
            Return False
        End Try
        Return False
    End Function
    Public Function IsFieldInReportFormat(ByVal rep As String, ByVal fld As String) As Boolean
        Dim sctSQL As String = "SELECT * FROM OURReportFormat WHERE (ReportID='" & rep & "' AND Prop='FIELDS' AND VAL='" & fld & "' )"
        If HasRecords(sctSQL) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function GetListOfTableColumnsFromSchema(ByVal tbl As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional ByRef err As String = "") As DataView
        'NOT IN USE
        Dim dv As DataView = Nothing
        Dim myprovider, sqls, strConnect As String
        sqls = String.Empty
        If userconstr = "" Then
            strConnect = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        Else
            strConnect = userconstr
            myprovider = userconprv
        End If
        If myprovider.StartsWith("InterSystems.Data.") Then
            sqls = "Select Name As COLUMN_NAME FROM %Dictionary.PropertyDefinition WHERE UCASE(parent) = UCASE('" & tbl & "') AND Cardinality Is NULL"

        ElseIf myprovider = "MySql.Data.MySqlClient" Then
            Dim db As String = GetDataBase(strConnect, myprovider)
            sqls = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA ='" & db.ToLower & "' AND TABLE_NAME = '" & tbl & "'"

        ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then
            'Dim db As String = GetDataBase(strConnect)
            sqls = "SELECT * FROM all_tab_cols WHERE UPPER(TABLE_NAME) = UPPER('" & tbl & "')"

            'ElseIf userconprv = "System.Data.Odbc" Then


            ' dv = GetListOfTableFields(tbl, userconstr, userconprv)

            'ElseIf userconprv = "System.Data.LoeDb" Then

        ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
            Dim db As String = GetDataBase(strConnect, myprovider)
            sqls = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA ='public' AND LOWER(TABLE_CATALOG) ='" & db.ToLower & "' AND TABLE_NAME = '" & tbl & "'"

        Else
            sqls = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" & tbl & "'"
        End If
        If userconprv <> "System.Data.Odbc" AndAlso userconprv <> "System.Data.OleDb" Then
            dv = mRecords(sqls, err, userconstr, userconprv)
        End If

        If myprovider.StartsWith("InterSystems.Data") AndAlso err = "" Then
            Dim dt As DataTable = dv.Table
            Dim NewRow As Object() = New Object(0) {"ID"}
            dt.BeginLoadData()
            dt.LoadDataRow(NewRow, True)
            dt.EndLoadData()
        End If
        Return dv
    End Function
    Public Function GetListOfTablesAndColumns(ByVal tbl As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional ByRef err As String = "") As DataView
        'NOT IN USE
        Dim dv As DataView = Nothing
        Dim myprovider, sqls, strConnect As String
        sqls = String.Empty
        If userconstr = "" Then
            strConnect = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        Else
            strConnect = userconstr
            myprovider = userconprv
        End If
        If myprovider.StartsWith("InterSystems.Data.") Then
            'TODO fix name of class with dots
            'tbl = tbl.Replace("_", ".")
            'If tbl.ToUpper.StartsWith("INFORMATION.SCHEMA") Then
            '    sqls = "Select parent as TABLE_NAME,Name As COLUMN_NAME FROM %Dictionary.PropertyDefinition WHERE Cardinality Is NULL  AND Inverse Is NULL AND Private=0 AND Transient=0 AND NOT SqlFieldName IS NULL"
            'Else

            'After return use SqlCOLUMN_NAME as COLUMN_NAME, otherwise use COLUMN_NAME
            sqls = "Select  parent as TABLE_NAME,Name As COLUMN_NAME,SqlFieldName As SqlCOLUMN_NAME FROM %Dictionary.PropertyDefinition WHERE UCASE(parent) = UCASE('" & tbl & "') AND  Cardinality Is NULL  AND Inverse Is NULL AND Private=0 AND Transient=0"
            'End If
        ElseIf myprovider = "MySql.Data.MySqlClient" Then
            Dim db As String = GetDataBase(strConnect, myprovider)
            sqls = "SELECT TABLE_NAME,COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA ='" & db.ToLower & "' AND TABLE_NAME = '" & tbl & "'"

        ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then
            'Dim db As String = GetDataBase(strConnect,myprovider)
            sqls = "SELECT TABLE_NAME,COLUMN_NAME FROM all_tab_cols WHERE UPPER(TABLE_NAME) = UPPER('" & tbl & "')"

            'ElseIf userconprv = "System.Data.Odbc" Then

            ' dv = GetListOfTableFields(tbl, userconstr, userconprv)

            'ElseIf userconprv = "System.Data.OLeDb" Then

        ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
            Dim db As String = GetDataBase(strConnect, myprovider)
            sqls = "SELECT TABLE_NAME,COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA ='public' AND LOWER(TABLE_CATALOG) ='" & db.ToLower & "' AND TABLE_NAME = '" & tbl & "'"

        Else
            sqls = "SELECT TABLE_NAME,COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" & tbl & "'"
        End If
        dv = mRecords(sqls, err, userconstr, userconprv)

        'TODO 
        If myprovider.StartsWith("InterSystems.Data") AndAlso err = "" Then
            Dim dt As DataTable = dv.Table
            'Dim NewRow As Object() = New Object(0) {"ID"}
            'dt.BeginLoadData()
            'dt.LoadDataRow(NewRow, True)
            'dt.EndLoadData()

            Dim Row As DataRow = dt.NewRow()
            Row("TABLE_NAME") = tbl
            Row("COLUMN_NAME") = "ID"
            dt.Rows.Add(Row)
        End If

        Return dv
    End Function
    Public Function GetListOfTableColumns(ByVal tbl As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional ByRef err As String = "") As DataView
        Dim dv As DataView = Nothing
        Dim myprovider, sqls, strConnect As String
        sqls = String.Empty
        If userconstr = "" Then
            strConnect = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        Else
            strConnect = userconstr
            myprovider = userconprv
        End If
        If myprovider.StartsWith("InterSystems.Data.") Then
            'TODO fix name of class with dots
            tbl = tbl.Replace("_", ".")
            If tbl.ToUpper.StartsWith("INFORMATION.SCHEMA") Then
                'AND Hidden = 0 AND TypeClass NOT LIKE '%Stream%' AND TypeClass NOT LIKE '%List%' AND TypeClass NOT LIKE '%Object%' AND  TypeClass %IN ('%Library.String', '%Library.Integer', '%Library.Numeric', '%Library.Double', '%Library.Float', '%Library.SmallInt', '%Library.TinyInt', '%Library.BigInt', '%Library.Date', '%Library.Time', '%Library.TimeStamp')
                sqls = "Select SqlFieldName As COLUMN_NAME, Type AS DATA_TYPE FROM %Dictionary.PropertyDefinition WHERE UCASE(parent) = UCASE('" & tbl & "') AND Collection IS NULL AND Cardinality Is NULL  AND Inverse Is NULL AND Private=0 AND Transient=0 AND NOT SqlFieldName IS NULL"
            Else
                If Not tbl.Contains(".") Then
                    tbl = "UserData" & "." & tbl
                End If
                sqls = "Select Name As COLUMN_NAME, Type AS DATA_TYPE FROM %Dictionary.PropertyDefinition WHERE UCASE(parent) = UCASE('" & tbl & "') AND Cardinality Is NULL  AND Inverse Is NULL AND Private=0 AND Transient=0"
            End If
        ElseIf myprovider = "MySql.Data.MySqlClient" Then
            Dim db As String = GetDataBase(strConnect, myprovider)
            sqls = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA ='" & db.ToLower & "' AND TABLE_NAME = '" & tbl & "'"

        ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then
            'Dim db As String = GetDataBase(strConnect,myprovider)
            sqls = "SELECT * FROM all_tab_cols WHERE UPPER(TABLE_NAME) = UPPER('" & tbl & "')"

        ElseIf myprovider = "System.Data.Odbc" OrElse myprovider = "System.Data.OleDb" Then

            dv = GetListOfTableFields(tbl, userconstr, userconprv)
            Return dv

        ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
            Dim db As String = GetDataBase(strConnect, myprovider)
            sqls = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA ='public' AND LOWER(TABLE_CATALOG) ='" & db.ToLower & "' AND TABLE_NAME = '" & tbl & "'"

        ElseIf myprovider = "Sqlite" Then  'in memory

            'sqls = "PRAGMA table_info('" & tbl & "')"
            dv = GetListOfTableFields(tbl, userconstr, userconprv)
            Return dv

        Else
            sqls = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" & tbl & "'"
        End If
        dv = mRecords(sqls, err, userconstr, userconprv)
        If myprovider.StartsWith("InterSystems.Data") AndAlso err = "" Then
            Dim dt As DataTable = dv.Table
            Dim NewRow As Object() = New Object(0) {"ID"}
            dt.BeginLoadData()
            dt.LoadDataRow(NewRow, True)
            dt.EndLoadData()
        End If
        Return dv
    End Function
    Public Function IsSqlCompatibleField(ByVal className As String, ByVal fieldName As String, Optional ByVal myconnstr As String = "", Optional ByVal myconnprv As String = "") As Boolean
        Dim ret As Boolean = True
        Dim er As String = String.Empty
        If myconnstr.Trim = "" Then
            myconnstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            myconnprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        End If
        'CollectionType IS NULL AND  Hidden=0 AND NOT SqlFieldName IS NULL AND TypeClass %IN ('%Library.String', '%Library.Integer', '%Library.Numeric', '%Library.Double', '%Library.Float', '%Library.SmallInt', '%Library.TinyInt', '%Library.BigInt', '%Library.Date', '%Library.Time', '%Library.TimeStamp') AND TypeClass NOT LIKE '%Stream%' AND TypeClass NOT LIKE '%List%' AND TypeClass NOT LIKE '%Object%'
        Dim sql As String = "SELECT * FROM %Dictionary.PropertyDefinition WHERE Collection IS NULL AND Cardinality Is NULL AND Inverse Is NULL AND Private=0 AND Transient=0  AND Parent = '" & className & "' AND Name = '" & fieldName & "'"
        sql = sql & " AND Type IN ('%Library.String', '%Library.Integer', '%Library.Numeric', '%Library.Double', '%Library.Float', '%Library.SmallInt', '%Library.TinyInt', '%Library.BigInt', '%Library.Date', '%Library.Time', '%Library.TimeStamp')"
        Dim dv As DataView = mRecords(sql, er, myconnstr, myconnprv)
        If dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then
            ret = False
        End If
        Return ret
    End Function

    Public Function GetListOfTableFields(ByVal tbl As String, ByVal userconstr As String, ByVal userconprv As String, Optional ByRef er As String = "") As DataView
        Dim ert As String = String.Empty
        Dim dv As DataView = Nothing
        Dim i As Integer
        Dim dtb As New DataTable
        Dim col As DataColumn
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "COLUMN_NAME"
        dtb.Columns.Add(col)
        col = New DataColumn
        col.DataType = System.Type.GetType("System.String")
        col.ColumnName = "DATA_TYPE"
        dtb.Columns.Add(col)
        Dim dt As DataTable
        Try
            Dim sqls As String = "Select * FROM [" & tbl & "]"

            If userconprv = "MySql.Data.MySqlClient" Then
                sqls = sqls & " LIMIT 1;"
            ElseIf userconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                sqls = sqls & " LIMIT 1;"
            ElseIf userconprv = "Sqlite" Then
                sqls = sqls.Replace("[", "").Replace("]", "") & " LIMIT 1;"
            ElseIf userconprv <> "Oracle.ManagedDataAccess.Client" Then
                sqls = "Select TOP 1 " & sqls.Substring(7)
            End If

            Dim dvt As DataView = mRecords(sqls, er, userconstr, userconprv)
            If dvt Is Nothing OrElse dvt.Table Is Nothing OrElse dvt.Table.Columns.Count = 0 Then  '
                er = "No table exists: " & tbl
                Return dvt
            End If
            If er = "" AndAlso dvt IsNot Nothing AndAlso dvt.Table IsNot Nothing Then
                dt = dvt.Table
                If dt.Rows.Count = 0 Then
                    er = "Not data In the table " & tbl
                End If
                For i = 0 To dt.Columns.Count - 1
                    Dim Row As DataRow = dtb.NewRow()
                    Row("COLUMN_NAME") = dt.Columns(i).Caption
                    Row("DATA_TYPE") = dt.Columns(i).DataType
                    dtb.Rows.Add(Row)
                Next
                dv = dtb.DefaultView
            Else

                dv = GetListOfTableColumns(tbl, userconstr, userconprv, ert)

            End If

        Catch ex As Exception
            er = ex.Message
            dv = GetListOfTableColumns(tbl, userconstr, userconprv, ert)
        End Try
        Return dv
    End Function
    Public Function IsTimeStamp(tbl As String, col As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "") As Boolean
        Dim ret As Boolean = False
        Dim myprovider, strConnect As String

        If userconstr = "" Then
            strConnect = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        Else
            strConnect = userconstr
            myprovider = userconprv
        End If
        'TODO maybe not needed for PostgreSQL: ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql

        If myprovider = "Oracle.ManagedDataAccess.Client" AndAlso GetFieldDataType(tbl, col, strConnect, myprovider).ToUpper.StartsWith("TIMESTAMP") Then
            ret = True
        ElseIf myprovider = "MySql.Data.MySqlClient" Then
            If GetFieldDataType(tbl, col, strConnect, myprovider).ToUpper.StartsWith("TIMESTAMP") Then
                ret = False
            ElseIf GetFieldDataType(tbl, col, strConnect, myprovider).ToUpper.StartsWith("Date") Then
                ret = False
            ElseIf GetFieldDataType(tbl, col, strConnect, myprovider).ToUpper = "TIME" Then
                ret = True
            End If
        End If
        Return ret
    End Function
    Public Function GetFieldDataType(tbl As String, ColumnName As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim dv As DataView = Nothing
        Dim myprovider, sqls, strConnect As String
        sqls = String.Empty
        If userconstr = "" Then
            strConnect = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        Else
            strConnect = userconstr
            myprovider = userconprv
        End If
        If myprovider.StartsWith("InterSystems.Data.") Then
            If tbl.ToUpper.StartsWith("INFORMATION.SCHEMA") Then
                sqls = "Select Type As DATA_TYPE FROM %Dictionary.PropertyDefinition WHERE parent = '" & tbl & "' AND Cardinality Is NULL AND SqlFieldName = '" & ColumnName & "'"
            Else
                If Not tbl.Contains(".") Then
                    tbl = "userdata" & "." & tbl
                End If
                sqls = "SELECT Type AS DATA_TYPE FROM %Dictionary.PropertyDefinition WHERE parent = '" & tbl & "' AND Cardinality Is NULL AND Name = '" & ColumnName & "'"
            End If
        ElseIf myprovider = "MySql.Data.MySqlClient" Then
            Dim db As String = GetDataBase(strConnect, myprovider)
            sqls = "SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE LOWER(TABLE_SCHEMA) ='" & db.ToLower & "' AND LOWER(TABLE_NAME) = '" & tbl.Replace("`", "").ToLower & "' AND LOWER(COLUMN_NAME) ='" & ColumnName.ToLower & "'"
        ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then
            'Dim db As String = GetDataBase(strConnect)
            'sqls = "SELECT * FROM all_tab_cols WHERE owner ='" & db & "' AND UPPER(TABLE_NAME) = UPPER('" & tbl & "') AND UPPER(COLUMN_NAME) =UPPER('" & ColumnName & "')"
            sqls = "SELECT * FROM all_tab_cols WHERE UPPER(TABLE_NAME) = UPPER('" & tbl & "') AND UPPER(COLUMN_NAME) =UPPER('" & ColumnName & "')"

        ElseIf userconprv = "System.Data.Odbc" Then
            'ODBC
            dv = GetListOfTableFields(tbl, strConnect, myprovider)
            dv.RowFilter = "COLUMN_NAME='" & ColumnName & "'"
            dv = dv.ToTable.DefaultView
        ElseIf userconprv = "System.Data.OleDb" Then
            'OleDb
            dv = GetListOfTableFields(tbl, strConnect, myprovider)
            dv.RowFilter = "COLUMN_NAME='" & ColumnName & "'"
            dv = dv.ToTable.DefaultView

        ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
            Dim db As String = GetDataBase(strConnect, myprovider)
            sqls = "SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA ='public' AND LOWER(TABLE_CATALOG) ='" & db.ToLower & "' AND TABLE_NAME = '" & tbl & "' AND COLUMN_NAME ='" & ColumnName & "'"

        ElseIf myprovider = "Sqlite" Then  'Sqlite
            dv = GetListOfTableFields(tbl, strConnect, myprovider)
            dv.RowFilter = "COLUMN_NAME='" & ColumnName & "'"
            dv = dv.ToTable.DefaultView

        Else
            sqls = "SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" & tbl & "' AND COLUMN_NAME ='" & ColumnName & "'"
        End If
        If userconprv <> "System.Data.Odbc" AndAlso userconprv <> "System.Data.OleDb" AndAlso userconprv <> "Sqlite" Then
            dv = mRecords(sqls, "", strConnect, myprovider)
        End If

        If Not dv Is Nothing AndAlso dv.Count > 0 AndAlso dv.Table.Rows.Count = 1 Then
            ret = dv.Table.Rows(0)("DATA_TYPE").ToString
        End If

        Return ret
    End Function
    Public Function GetListOfStoredProcedures(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As DataView
        Dim er As String = ""
        Dim dv As DataView = Nothing
        Dim sqls As String = String.Empty
        Dim myconstring As String = myconstr
        Dim myprovider As String = myconprv
        If myconstr = "" Then
            myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        End If
        If myprovider.StartsWith("InterSystems.Data.") Then
            sqls = "SELECT ID AS ROUTINE_NAME FROM %Dictionary.MethodDefinition WHERE SqlProc=1 AND ReturnResultsets=1 AND NOT ID %STARTSWITH '%' ORDER BY ID"
            'sqls = "SELECT SQLName AS ROUTINE_NAME FROM %Dictionary.CompiledMethod WHERE SqlProc=1 AND ReturnResultsets=1 AND NOT Origin %STARTSWITH '%' ORDER BY SQLName"
        ElseIf myprovider = "MySql.Data.MySqlClient" Then
            Dim db As String = GetDataBase(myconstring, myprovider)
            sqls = "select ROUTINE_NAME from INFORMATION_SCHEMA.ROUTINES where  ROUTINE_SCHEMA='" & db.ToLower & "' AND ROUTINE_TYPE='PROCEDURE'"
        ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then
            sqls = "SELECT * FROM USER_PROCEDURES WHERE OBJECT_TYPE ='PROCEDURE'"
        ElseIf myprovider = "System.Data.Odbc" Then
            'ODBC - not in use, sp is hidden for now
            Try
                dv = GetListByODBC(System.Data.Odbc.OdbcMetaDataCollectionNames.Procedures, myconstring, myprovider).DefaultView
                Dim dtr As DataTable = dv.ToTable
                For i = 0 To dtr.Rows.Count - 1
                    If dtr.Rows(i)("PROCEDURE_SCHEM").ToString <> "" Then
                        dtr.Rows(i)("PROCEDURE_NAME") = dtr.Rows(i)("PROCEDURE_SCHEM") & "." & dtr.Rows(i)("PROCEDURE_NAME")  'SCHEM is needed ???
                    End If
                Next
                dv = dtr.DefaultView
            Catch ex As Exception
                er = ex.Message
            End Try

        ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
            Dim db As String = GetDataBase(myconstring, myprovider)
            sqls = "select ROUTINE_NAME from INFORMATION_SCHEMA.ROUTINES where  ROUTINE_SCHEMA ='public' AND LOWER(ROUTINE_CATALOG) ='" & db.ToLower & "' AND ROUTINE_TYPE='PROCEDURE'"

        Else
            sqls = "select * from information_schema.routines where routine_type = 'PROCEDURE' ORDER BY ROUTINE_NAME"
        End If
        If myprovider <> "System.Data.Odbc" AndAlso myprovider <> "System.Data.OleDb" Then
            dv = mRecords(sqls, er, myconstring, myprovider)
        End If
        Return dv
    End Function
    Public Function GetStoredProcedureDetails(ByVal spname As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As DataView
        Dim dv As DataView = Nothing
        Dim sqls As String = String.Empty
        Dim myconstring As String = myconstr
        Dim myprovider As String = myconprv
        If myconstr = "" Then
            myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        End If
        If myprovider.StartsWith("InterSystems.Data.") Then
            Dim cls As String = spname.Substring(0, spname.IndexOf("||"))
            Dim sp As String = spname.Substring(spname.IndexOf("||") + 2)
            sqls = "SELECT * FROM %Dictionary.MethodDefinition WHERE (SqlProc=1 AND ReturnResultsets=1 AND parent='" & cls & "' AND Name='" & sp & "')"
            'sqls = "SELECT * FROM %Dictionary.CompiledMethod WHERE (SqlProc=1 AND ReturnResultsets=1 AND Name'" & spname & "')"
        ElseIf myprovider = "MySql.Data.MySqlClient" Then
            Dim db As String = GetDataBase(myconstring, myprovider)
            sqls = "select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_SCHEMA='" & db.ToLower & "' AND ROUTINE_TYPE='PROCEDURE' AND ROUTINE_NAME='" & spname & "')"
        ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then

            sqls = "SELECT * FROM USER_PROCEDURES WHERE OBJECT_TYPE IN ('PROCEDURE','FUNCTION') AND OBJECT_NAME='" & spname & "'"
        ElseIf myprovider = "System.Data.Odbc" Then
            'ODBC - not in use, sp is hidden for now
            Dim spnames(2) As String
            spnames(2) = spname
            dv = GetListByODBC(System.Data.Odbc.OdbcMetaDataCollectionNames.ProcedureColumns, myconstring, myprovider, spnames).DefaultView

        ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
            Dim db As String = GetDataBase(myconstring, myprovider)
            sqls = "select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_SCHEMA ='public' AND LOWER(ROUTINE_CATALOG) ='" & db.ToLower & "' AND ROUTINE_TYPE='PROCEDURE' AND ROUTINE_NAME='" & spname & "')"

        Else
            sqls = "select * from information_schema.routines where (routine_type = 'PROCEDURE' AND ROUTINE_NAME='" & spname & "')"
        End If
        Dim er As String = ""
        If myprovider <> "System.Data.Odbc" AndAlso myprovider <> "System.Data.OleDb" Then
            dv = mRecords(sqls, er, myconstring, myprovider)
        End If

        Return dv
    End Function
    Public Function GetCacheMethodParameters(FormalSpecs As String) As String
        Dim ret = ""
        If FormalSpecs <> String.Empty Then
            Dim parms() As String = FormalSpecs.Split(",")
            Dim param As String = ""
            Dim name As String = ""
            Dim typ As String = ""

            For i As Integer = 0 To parms.Length - 1
                param = parms(i)
                If param.Length > 0 AndAlso param.IndexOf(":") > 0 Then
                    name = Piece(param, ":", 1)
                    typ = Piece(param, ":", 2)
                    If ret = "" Then
                        ret = name & " As " & typ
                    Else
                        ret &= "," & name & " As " & typ
                    End If
                End If
            Next
        End If
        Return ret
    End Function

    Public Function GetStoredProcedureText(ByVal spname As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        'Dim dv As dataview = Nothing
        Dim dtDetails As DataTable = GetStoredProcedureDetails(spname, myconstr, myconprv).Table
        Dim dtBody As DataTable = Nothing
        Dim sqls As String = String.Empty
        Dim myconstring As String = myconstr
        Dim myprovider As String = myconprv
        If myconstr = "" Then
            myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        End If
        Dim sptext As String = String.Empty
        'Dim i As Integer

        If myprovider.StartsWith("InterSystems.Data.") Then
            If dtDetails IsNot Nothing AndAlso dtDetails.Rows.Count > 0 Then
                Dim cls As String = Piece(spname, "||", 1)
                Dim sp As String = Piece(spname, "||", 2)
                Dim params As String = ""
                Dim body As String = ""
                Dim er As String = ""

                sqls = "SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE CLASSNAME='" & cls & "' AND METHOD_OR_QUERY_NAME='" & sp & "'"
                dtBody = mRecords(sqls, er, myconstring, myprovider).Table
                If dtBody IsNot Nothing AndAlso dtBody.Rows.Count > 0 Then
                    Dim drBody As DataRow = dtBody.Rows(0)
                    Dim drDetails As DataRow = dtDetails.Rows(0)
                    sptext = "ClassMethod " & sp & "("
                    body = drBody("ROUTINE_DEFINITION").ToString
                    params = GetCacheMethodParameters(drDetails("FormalSpec").ToString)
                    sptext &= params & ") [ ReturnResultsets, SqlName = " & drDetails("SqlName") & ", SqlProc] {" & body & vbCrLf & "}"
                End If
            End If
            'Dim cls As String = spname.Substring(0, spname.IndexOf("||"))
            'Dim sp As String = spname.Substring(spname.IndexOf("||") + 2)
            'sqls = "SELECT Description FROM %Dictionary.MethodDefinition WHERE (SqlProc=1 AND ReturnResultsets=1 AND parent='" & cls & "' AND Name='" & sp & "')"

            'Dim er As String = ""
            'dv = mRecords(sqls, er, myconstring, myprovider)
            'If Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
            '    sptext = dv.Table.Rows(0)("Description").ToString
            'End If
            'TODO find in what class subscript 30 is of ^oddDEF(cls,m,sp,30)


            'Dim cls As String = spname.Substring(0, spname.IndexOf("||"))
            'Dim sp As String = spname.Substring(spname.IndexOf("||") + 2)
            'Dim n As Integer = 0
            'Dim nod As String = "^oddDEF(""" & cls & """,""m"",""" & sp & """,30)"
            'Try
            '    n = CInt(GetGlobalNodeValue(nod).ToString)
            'Catch ex As Exception
            '    n = 0
            '    sptext = ex.Message
            'End Try
            'If n > 0 Then
            '    For i = 1 To n
            '        nod = "^oddDEF(""" & cls & """,""m"",""" & sp & """,30," & i.ToString & ")"
            '        sptext = sptext & GetGlobalNodeValue(nod).ToString & " " & Chr(13) + Chr(10)  'Required installation of OUR classes
            '    Next
            'End If
        ElseIf myprovider = "MySql.Data.MySqlClient" Then

            If dtDetails IsNot Nothing AndAlso dtDetails.Rows.Count > 0 Then
                sptext = dtDetails.Rows(0)("ROUTINE_DEFINITION").ToString
            End If


        ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then

            If dtDetails IsNot Nothing AndAlso dtDetails.Rows.Count > 0 Then
                Dim er As String = ""
                sqls = "SELECT TEXT FROM USER_SOURCE WHERE NAME='" & spname & "'"
                dtBody = mRecords(sqls, er, myconstring, myprovider).Table
                If dtBody IsNot Nothing AndAlso dtBody.Rows.Count > 0 Then
                    For i As Integer = 0 To dtBody.Rows.Count - 1
                        If i > 0 Then
                            sptext &= dtBody.Rows(i)("TEXT")
                        Else
                            sptext = dtBody.Rows(i)("TEXT")
                        End If
                    Next
                End If
            End If

        ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql  NOT TESTED YET !!!!!!!!!!!!!!!!!!!!!!1
            If dtDetails IsNot Nothing AndAlso dtDetails.Rows.Count > 0 Then
                sptext = dtDetails.Rows(0)("ROUTINE_DEFINITION").ToString
            End If

        ElseIf myprovider = "System.Data.Odbc" Then
            'ODBC - not in use, sp is hidden for now
            Dim spnames(2) As String
            spnames(2) = spname
            Dim dpt As DataTable = GetListByODBC(System.Data.Odbc.OdbcMetaDataCollectionNames.Procedures, myconstring, myprovider, spnames)
        Else ' SQL Server
            If dtDetails IsNot Nothing AndAlso dtDetails.Rows.Count > 0 Then
                sptext = dtDetails.Rows(0)("ROUTINE_DEFINITION").ToString.Substring(7)
            End If
        End If
        Return cleanTextLight(sptext)
    End Function
    'Public Function GetListOfStoredProcedureParametersOld(ByVal spname As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As ListItemCollection
    '    'NOT IN USE
    '    Dim params As ListItemCollection = New ListItemCollection
    '    Dim dv As DataView = Nothing
    '    Dim sqls As String = String.Empty
    '    Dim myconstring As String = myconstr
    '    Dim myprovider As String = myconprv
    '    If myconstr = "" Then
    '        myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
    '        myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
    '    End If
    '    Dim i As Integer
    '    params.Add(" ")
    '    If myprovider.StartsWith("InterSystems.Data.") Then
    '        Dim cls As String = spname.Substring(0, spname.IndexOf("||"))
    '        Dim sp As String = spname.Substring(spname.IndexOf("||") + 2)
    '        sqls = "SELECT FormalSpec FROM %Dictionary.MethodDefinition WHERE (SqlProc=1 AND ReturnResultsets=1 AND parent='" & cls & "' AND Name='" & sp & "')"
    '        Dim er As String = ""
    '        dv = mRecords(sqls, er, myconstring, myprovider)
    '        If Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
    '            Dim pars As String = dv.Table.Rows(0)("FormalSpec").ToString
    '            Dim parms() As String = pars.Split(",")
    '            For i = 0 To parms.Length - 1
    '                If parms(i).Length > 0 AndAlso parms(i).IndexOf(":") > 0 Then
    '                    params.Add(parms(i).Substring(0, parms(i).IndexOf(":")))
    '                End If
    '            Next
    '        End If
    '    ElseIf myprovider = "MySql.Data.MySqlClient" Then
    '        Dim db As String = GetDataBase(myconstring)
    '        sqls = "select * from INFORMATION_SCHEMA.PARAMETERS where SPECIFIC_SCHEMA='" & db & "' AND ROUTINE_TYPE='PROCEDURE' AND SPECIFIC_NAME='" & spname & "')"
    '        Dim er As String = ""
    '        dv = mRecords(sqls, er, myconstring, myprovider)
    '        If Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
    '            For i = 0 To dv.Table.Rows.Count - 1
    '                'PARAMETER_NAME~PARAMETER_MODE
    '                'PARAMETER_MODE=IN,OUT, OR INOUT
    '                'params = params & dv.Table.Rows(i)("PARAMETER_NAME").ToString & "~" & dv.Table.Rows(i)("PARAMETER_MODE")
    '                params.Add(dv.Table.Rows(i)("PARAMETER_NAME").ToString)
    '                'If i <> dv.Table.Rows.Count - 1 Then
    '                '    params = params & ","
    '                'End If
    '            Next
    '        End If
    '    ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then

    '        'TODO !!!!!! for Oracle.ManagedDataAccess.Client

    '        'ElseIf myprovider = "System.Data.Odbc" Then
    '        'TODO for ODBC???
    '    Else
    '        sqls = "Select t1.[name] As [SP_name],t2.[name] As [Parameter_name], t3.[name] as [Type], t2.[Length], t2.colorder as [Param_order] From sysobjects t1 inner Join syscolumns t2 on t1.[id]=t2.[id] inner Join systypes t3 on t2.xtype=t3.xtype where t1.[name] ='" & spname & "' order by [Param_order]"
    '        Dim er As String = ""
    '        dv = mRecords(sqls, er, myconstring, myprovider)
    '        If Not dv Is Nothing AndAlso Not dv.Table Is Nothing Then
    '            For i = 0 To dv.Table.Rows.Count - 1
    '                'params = params & dv.Table.Rows(i)("PARAMETER_NAME").ToString.Replace("@", " ") & ", "
    '                params.Add(dv.Table.Rows(i)("PARAMETER_NAME").ToString.Replace("@", ""))
    '            Next
    '        End If
    '    End If
    '    Return params
    'End Function
    Public Function SPHasCursor(spname As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As Boolean
        'only for Oracle right now
        Dim ret As Boolean = False
        Dim sqls As String = String.Empty
        Dim myconstring As String = myconstr
        Dim myprovider As String = myconprv
        If myconstr = "" Then
            myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        End If
        'TODO mayb not needed for PostgreSQL: ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql

        If myprovider = "Oracle.ManagedDataAccess.Client" Then
            sqls = "SELECT * FROM USER_ARGUMENTS WHERE OBJECT_NAME='" & spname & "' AND IN_OUT='OUT' AND DATA_TYPE='REF CURSOR'"
            ret = HasRecords(sqls, myconstring, myprovider)
        End If
        Return ret
    End Function
    Public Function GetListOfSPOutParams(spname As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As ListItemCollection
        'only for Oracle right now
        Dim params As ListItemCollection = New ListItemCollection
        Dim dv As DataView = Nothing
        Dim sqls As String = String.Empty
        Dim myconstring As String = myconstr
        Dim myprovider As String = myconprv
        If myconstr = "" Then
            myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        End If
        'TODO maybe not needed for PostgreSQL: ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
        Dim i As Integer
        If myprovider = "Oracle.ManagedDataAccess.Client" Then
            sqls = "SELECT ARGUMENT_NAME,DATA_TYPE,IN_OUT,DATA_LENGTH,DATA_PRECISION FROM USER_ARGUMENTS WHERE OBJECT_NAME='" & spname & "' AND IN_OUT='OUT' ORDER BY POSITION"
            Dim er As String = ""
            dv = mRecords(sqls, er, myconstring, myprovider)
            If Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
                Dim dt As DataTable = dv.Table
                For i = 0 To dt.Rows.Count - 1
                    Dim li As New ListItem
                    li.Text = dt.Rows(i)("ARGUMENT_NAME").ToString
                    li.Value = ConvertTypeToOUR(dt.Rows(i)("DATA_TYPE").ToString)
                    params.Add(li)
                Next
            End If
        End If
        Return params
    End Function

    Public Function GetListOfStoredProcedureParameters(ByVal spname As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As ListItemCollection
        Dim params As ListItemCollection = New ListItemCollection
        Dim dv As DataView = Nothing
        Dim sqls As String = String.Empty
        Dim myconstring As String = myconstr
        Dim myprovider As String = myconprv
        If myconstr = "" Then
            myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        End If
        Dim i As Integer
        params.Add(" ")
        If myprovider.StartsWith("InterSystems.Data.") Then
            Dim cls As String = spname.Substring(0, spname.IndexOf("||"))
            Dim sp As String = spname.Substring(spname.IndexOf("||") + 2)
            sqls = "SELECT FormalSpec FROM %Dictionary.MethodDefinition WHERE (SqlProc=1 AND ReturnResultsets=1 AND parent='" & cls & "' AND Name='" & sp & "')"
            Dim er As String = ""
            dv = mRecords(sqls, er, myconstring, myprovider)
            If Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
                Dim pars As String = dv.Table.Rows(0)("FormalSpec").ToString
                Dim parms() As String = pars.Split(",")
                For i = 0 To parms.Length - 1
                    If parms(i).Length > 0 AndAlso parms(i).IndexOf(":") > 0 Then
                        Dim li As New ListItem
                        li.Text = parms(i).Substring(0, parms(i).IndexOf(":"))
                        li.Value = ConvertTypeToOUR(parms(i).Substring(parms(i).IndexOf(":") + 1))
                        params.Add(li)
                    End If
                Next
            End If
        ElseIf myprovider = "MySql.Data.MySqlClient" Then
            Dim db As String = GetDataBase(myconstring, myprovider)
            Dim er As String = ""
            Try
                sqls = "select * from INFORMATION_SCHEMA.PARAMETERS where SPECIFIC_SCHEMA='" & db.ToLower & "' AND ROUTINE_TYPE='PROCEDURE' AND SPECIFIC_NAME='" & spname & "'"
                dv = mRecords(sqls, er, myconstring, myprovider)
                If Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
                    For i = 0 To dv.Table.Rows.Count - 1
                        Dim li As New ListItem
                        li.Text = dv.Table.Rows(i)("PARAMETER_NAME").ToString
                        li.Value = ConvertTypeToOUR(dv.Table.Rows(i)("DATA_TYPE").ToString)
                        params.Add(li)
                    Next
                End If
                If er <> "" Then
                    'MySQL versions before 5.5
                    sqls = "select param_list FROM mysql.proc where db='" & db & "' AND NAME='" & spname & "'"
                    dv = mRecords(sqls, er, myconstring, myprovider)
                    Dim prms() As String = dv.Table.Rows(0)("param_list").Split(",")
                    For i = 0 To prms.Count - 1
                        If Piece(prms(i), " ", 1).ToString.Trim.ToUpper = "IN" Then
                            Dim li As New ListItem
                            li.Text = Piece(prms(i), " ", 2)
                            li.Value = ConvertTypeToOUR(Piece(prms(i), " ", 3))
                            params.Add(li)
                        End If
                    Next
                End If
            Catch ex As Exception

            End Try
        ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then

            sqls = "SELECT ARGUMENT_NAME,DATA_TYPE,IN_OUT,DATA_LENGTH,DATA_PRECISION FROM USER_ARGUMENTS WHERE OBJECT_NAME='" & spname & "' ORDER BY POSITION"
            Dim er As String = ""
            dv = mRecords(sqls, er, myconstring, myprovider)
            If Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
                Dim dt As DataTable = dv.Table
                For i = 0 To dt.Rows.Count - 1
                    If dt.Rows(i)("IN_OUT") = "IN" Then
                        Dim li As New ListItem
                        li.Text = dt.Rows(i)("ARGUMENT_NAME").ToString
                        li.Value = ConvertTypeToOUR(dt.Rows(i)("DATA_TYPE").ToString)
                        params.Add(li)
                    End If
                Next
            End If
        ElseIf myprovider = "System.Data.Odbc" Then
            'ODBC 
            Dim spnames(2) As String
            spnames(2) = spname
            dv = GetListByODBC(System.Data.Odbc.OdbcMetaDataCollectionNames.ProcedureParameters, myconstring, myprovider, spnames).DefaultView

        ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
            Dim db As String = GetDataBase(myconstring, myprovider)
            Dim er As String = ""
            Try
                sqls = "select * from INFORMATION_SCHEMA.PARAMETERS where SPECIFIC_SCHEMA ='public' AND LOWER(SPECIFIC_CATALOG) ='" & db.ToLower & "' AND SPECIFIC_NAME='" & spname & "'"
                dv = mRecords(sqls, er, myconstring, myprovider)
                If Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
                    For i = 0 To dv.Table.Rows.Count - 1
                        Dim li As New ListItem
                        li.Text = dv.Table.Rows(i)("PARAMETER_NAME").ToString
                        li.Value = ConvertTypeToOUR(dv.Table.Rows(i)("DATA_TYPE").ToString)
                        params.Add(li)
                    Next
                End If
                If er <> "" Then
                    'MySQL versions before 5.5
                    sqls = "select param_list FROM mysql.proc where db='" & db & "' AND NAME='" & spname & "'"
                    dv = mRecords(sqls, er, myconstring, myprovider)
                    Dim prms() As String = dv.Table.Rows(0)("param_list").Split(",")
                    For i = 0 To prms.Count - 1
                        If Piece(prms(i), " ", 1).ToString.Trim.ToUpper = "IN" Then
                            Dim li As New ListItem
                            li.Text = Piece(prms(i), " ", 2)
                            li.Value = ConvertTypeToOUR(Piece(prms(i), " ", 3))
                            params.Add(li)
                        End If
                    Next
                End If
            Catch ex As Exception

            End Try

        Else
            'sqls = "Select DISTINCT t1.[name] As [SP_name],t2.[name] As [Parameter_name], t3.[name] as [Parameter_Type], t2.[Length], t2.colorder as [Param_order] From sysobjects t1 inner Join syscolumns t2 on t1.[id]=t2.[id] inner Join systypes t3 on t2.xtype=t3.xtype where t1.[name] ='" & spname & "' order by [Param_order]"
            sqls = "Select DISTINCT t1.[name] As [SP_name],t2.[name] As [Parameter_name], t3.[name] as [Parameter_Type],t2.colorder as [Param_order] From sysobjects t1 inner Join syscolumns t2 on t1.[id]=t2.[id] inner Join systypes t3 on t2.xtype=t3.xtype where t1.[name] ='" & spname & "' order by [Param_order]"
            Dim er As String = ""
            dv = mRecords(sqls, er, myconstring, myprovider)
            If Not dv Is Nothing AndAlso Not dv.Table Is Nothing Then
                For i = 0 To dv.Table.Rows.Count - 1
                    Dim li As New ListItem
                    li.Text = dv.Table.Rows(i)("PARAMETER_NAME").ToString.Replace("@", "")
                    li.Value = ConvertTypeToOUR(dv.Table.Rows(i)("Parameter_Type").ToString)
                    params.Add(li)
                Next
            End If
        End If
        Return params
    End Function
    'Ole
    Public Function mDataView(ByVal mySQL As String, ByVal myStr As String) As DataView
        'NOT IN USE
        Dim myCommand As OleDbCommand
        Dim myAdapter As OleDbDataAdapter
        Dim myRecords As DataTable
        Dim myView As DataView
        myCommand = New OleDbCommand
        myCommand.Connection.ConnectionString = myStr
        myCommand.CommandType = CommandType.Text
        myCommand.CommandText = mySQL
        If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
        myAdapter = New OleDbDataAdapter(myCommand)
        myRecords = New DataTable
        myAdapter.Fill(myRecords)
        myView = myRecords.DefaultView
        'Dim dt As DataTable = MakeDTColumnsNamesCLScompliant(myRecords)  ', myprovider, er
        'myView = dt.DefaultView
        myAdapter.Dispose()
        myCommand.Connection.Close()
        myCommand.Dispose()
        Return myView
    End Function
    'Ole
    Public Function mAccessTables(ByVal myStr As String) As DataTable
        Dim myRecords As DataTable
        Dim myConn As New OleDbConnection(myStr)
        'myConn = (New Comd(myStr)).OleDbConn
        myConn.Open()
        myRecords = New DataTable
        myRecords = myConn.GetSchema("Tables")
        Return myRecords
    End Function
    'Ole
    Public Function mDataGridView(ByVal mySQL As String, ByVal myStr As String) As DataView
        'NOT IN USE
        Dim myCommand As OleDbCommand
        Dim myAdapter As OleDbDataAdapter
        Dim myRecords As DataTable
        Dim myView As DataView
        myCommand = New OleDbCommand
        myCommand.Connection.ConnectionString = myStr
        myCommand.CommandType = CommandType.Text
        myCommand.CommandText = mySQL
        If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
        myAdapter = New OleDbDataAdapter(myCommand)
        myRecords = New DataTable
        myAdapter.Fill(myRecords)
        myView = myRecords.DefaultView
        'Dim dt As DataTable = MakeDTColumnsNamesCLScompliant(myRecords) ', myprovider, er
        'myView = dt.DefaultView
        myAdapter.Dispose()
        myCommand.Connection.Close()
        myCommand.Dispose()
        Return myView
    End Function

    Public Function DataSetFromDataView(ByVal dv As DataView) As DataSet
        '// Clone the structure of the table behind the view
        Dim dtTemp As DataTable = dv.Table.Clone
        dtTemp.Clear()
        ''// Populate the table with rows in the view
        Dim drv As DataRowView
        For Each drv In dv
            dtTemp.ImportRow(drv.Row)
        Next
        Dim dsTemp As DataSet = New DataSet
        '// Add the new table to a DataSet
        dsTemp.Tables.Add(dtTemp)
        Return dsTemp
    End Function
    Public Function myTableFromRow(ByVal dv As DataRow) As DataTable
        'NOT IN USE
        '// Clone the structure of the table behind the view
        Dim dtTemp As DataTable = dv.Table.Clone
        dtTemp.Clear()
        dtTemp.ImportRow(dv)
        Return dtTemp
    End Function

    '    Public Function ExportDataXLSfromDataView(ByVal myDataView As DataView, ByVal myfilename As String) As String
    '        'NOT IN USE
    '        'open excel file
    '        Dim dataFile, txtline As String
    '        On Error GoTo ErrMsg
    '        'Dim swr As System.IO.StringWriter = New System.IO.StringWriter(myfilename)
    '        Dim fso, swr
    '        fso = CreateObject("Scripting.FileSystemObject")
    '        swr = fso.CreateTextFile(myfilename, True)

    '        Dim n, i, j As Integer
    '        n = myDataView.Table.Rows.Count
    '        If myDataView.Table.Rows.Count > 0 Then  'if table has rows
    '            'first row in the file - names of columns
    '            For j = 1 To myDataView.Table.Columns.Count
    '                txtline = txtline & Chr(9) & myDataView.Table.Columns.Item(j - 1).Caption
    '            Next
    '            txtline = txtline & Chr(9)
    '            swr.WriteLine(txtline)
    '            txtline = ""
    '            'write rows
    '            For i = 1 To myDataView.Table.Rows.Count
    '                For j = 1 To myDataView.Table.Columns.Count
    '                    txtline = txtline & Chr(9) & myDataView.Table.Rows(i - 1)(j - 1).ToString
    '                Next
    '                txtline = txtline & Chr(9)
    '                swr.WriteLine(txtline)
    '                txtline = ""
    '            Next
    '        Else
    '            ExportDataXLSfromDataView = "Table is empty."
    '        End If
    '        ExportDataXLSfromDataView = "Table is exported."
    '        swr.close()
    '        fso = Nothing
    '        Exit Function
    'ErrMsg:
    '        ExportDataXLSfromDataView = "You do not have a permission to write into this file. Contact the system administrator."
    '    End Function
    'Public Function ExportDataXMLfromDataView(ByVal myDataView As DataView, ByVal myfilename As String) As String
    '    'NOT IN USE
    '    'open xml file 
    '    Dim dataFile, txtline As String

    '    Try


    '        'Dim swr As System.IO.StringWriter = New System.IO.StringWriter(myfilename)
    '        Dim fso, swr
    '        fso = CreateObject("Scripting.FileSystemObject")
    '        swr = fso.CreateTextFile(myfilename, True)

    '        swr.WriteLine("<?xml version='1.0' encoding='UTF-8' ?> ")
    '        swr.WriteLine("<table> ")
    '        Dim n, i, j As Integer
    '        n = myDataView.Table.Rows.Count
    '        If myDataView.Table.Rows.Count > 0 Then  'if table has rows

    '            'write rows
    '            For i = 1 To myDataView.Table.Rows.Count
    '                txtline = "<row>"
    '                swr.WriteLine(txtline)
    '                txtline = "<columns>"
    '                swr.WriteLine(txtline)
    '                'write columns
    '                For j = 1 To myDataView.Table.Columns.Count
    '                    txtline = "<" & myDataView.Table.Columns.Item(j - 1).Caption & ">"
    '                    txtline = txtline & myDataView.Table.Rows(i - 1)(j - 1).ToString
    '                    txtline = txtline & "</" & myDataView.Table.Columns.Item(j - 1).Caption & ">"
    '                    swr.WriteLine(txtline)
    '                Next

    '                txtline = "</columns>"
    '                swr.WriteLine(txtline)

    '                txtline = "</row>"
    '                swr.WriteLine(txtline)
    '                txtline = ""
    '            Next

    '        End If
    '        swr.WriteLine("</table> ")
    '        swr.close()

    '        fso = Nothing
    '        Return "Table is exported."
    '    Catch ex As Exception
    '        Return "ERROR!! You do not have a permission to write into this file. Contact the system administrator.")
    '    End Try
    'End Function
    Public Function ExportToExcel(ByVal dt As DataTable, ByVal Expdir As String, ByVal Expfile As String, Optional ByVal hdr As String = "", Optional ByVal ftr As String = "", Optional ByVal dlm As String = "") As String
        Dim txtline As String = String.Empty
        Dim ret As String = String.Empty
        Dim m, i, j As Integer
        If dt Is Nothing AndAlso hdr.Trim = "" Then
            Return ret
            Exit Function
        End If

        If dlm = "" Then dlm = Chr(9)
        Dim MyFile As StreamWriter = New StreamWriter(Expdir & Expfile)
        Try
            txtline = ""
            MyFile.WriteLine(hdr)
            txtline = ""
            MyFile.WriteLine(txtline)
            If Not dt Is Nothing Then
                m = dt.Columns.Count
                For i = 0 To m - 1
                    If dt.Columns(i).ColumnName.ToString.IndexOf(dlm) >= 0 Then
                        txtline = txtline & """" & dt.Columns(i).ColumnName & """"
                    Else
                        txtline = txtline & dt.Columns(i).ColumnName
                    End If

                    If i < m - 1 Then txtline = txtline & dlm
                Next
                MyFile.WriteLine(txtline)
                txtline = ""
                For j = 0 To dt.Rows.Count - 1
                    txtline = ""
                    For i = 0 To m - 1
                        If dt.Rows(j).Item(i).ToString.IndexOf(dlm) >= 0 Then
                            txtline = txtline & """" & dt.Rows(j).Item(i).ToString & """"
                        Else
                            txtline = txtline & dt.Rows(j).Item(i).ToString
                        End If

                        If i < m - 1 Then txtline = txtline & dlm
                    Next
                    If Trim(txtline) <> "" Then
                        MyFile.WriteLine(txtline)
                    End If
                Next
            End If
            txtline = ""
            MyFile.WriteLine(txtline)
            MyFile.WriteLine(ftr)
            MyFile.Flush()
            MyFile.Close()
            MyFile = Nothing
            ret = Expdir & Expfile
        Catch ex As Exception
            ret = "Error creating Excel file" & ex.Message
            MyFile.Close()
            MyFile = Nothing
        End Try
        Return ret
    End Function
    Public Function ExportToXML(ByVal dt As DataTable, ByVal Expdir As String, ByVal Expfile As String, Optional ByVal hdr As String = "", Optional ByVal ftr As String = "", Optional ByVal er As String = "") As String
        Dim txtline As String = String.Empty
        Dim ret As String = String.Empty
        'Dim m, i, j As Integer
        If dt Is Nothing Then
            Return ret
            Exit Function
        End If
        Dim MyFile As StreamWriter = New StreamWriter(Expdir & Expfile)
        Try
            dt.TableName = hdr
            dt.WriteXml(MyFile)

            'txtline = ""
            'MyFile.WriteLine(hdr)
            'txtline = ""
            'MyFile.WriteLine(txtline)
            'm = dt.Columns.Count
            'For i = 0 To m - 1
            '    txtline = txtline & dt.Columns(i).ColumnName
            '    If i < m - 1 Then txtline = txtline & dlm
            'Next
            'MyFile.WriteLine(txtline)
            'txtline = ""
            'For j = 0 To dt.Rows.Count - 1
            '    txtline = ""
            '    For i = 0 To m - 1
            '        txtline = txtline & dt.Rows(j).Item(i).ToString
            '        If i < m - 1 Then txtline = txtline & dlm
            '    Next
            '    If Trim(txtline) <> "" Then
            '        MyFile.WriteLine(txtline)
            '    End If
            'Next
            'txtline = ""
            'MyFile.WriteLine(txtline)
            'MyFile.WriteLine(ftr)
            MyFile.Flush()
            MyFile.Close()
            MyFile = Nothing
            ret = Expdir & Expfile
        Catch ex As Exception
            ret = "Error creating Excel file" & ex.Message
            MyFile.Close()
            MyFile = Nothing
        End Try
        Return ret
    End Function
    Public Function ExportToCSV(ByVal dt As DataTable, ByVal Expdir As String, ByVal csvfile As String, ByVal delimtr As String, Optional ByVal hdr As String = "", Optional ByVal ftr As String = "") As String
        'NOT IN USE
        Dim txtline As String
        Dim m, i, j As Integer
        Dim fso = CreateObject("Scripting.FileSystemObject")
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim MyFile As Object
        On Error GoTo ErrMsg
        MyFile = fso.CreateTextFile(Expdir & csvfile, True)
        On Error GoTo ErrMsg
        txtline = ""
        MyFile.WriteLine(hdr)
        txtline = ""
        MyFile.WriteLine(txtline)
        m = dt.Columns.Count
        For i = 0 To m - 1
            txtline = txtline & dt.Columns(i).ColumnName
            If i < m - 1 Then txtline = txtline & delimtr
        Next
        MyFile.WriteLine(txtline)
        txtline = ""
        For j = 0 To dt.Rows.Count - 1
            txtline = ""
            For i = 0 To m - 1
                txtline = txtline & dt.Rows(j).Item(i).ToString
                If i < m - 1 Then txtline = txtline & delimtr
            Next
            If Trim(txtline) <> "" Then
                MyFile.WriteLine(txtline)
            End If
        Next
        txtline = ""
        MyFile.WriteLine(txtline)
        MyFile.WriteLine(ftr)
        MyFile.Close()
        MyFile = Nothing
        fso = Nothing
        ExportToCSV = csvfile
        Exit Function
ErrMsg:
        MyFile.Close()
        MyFile = Nothing
        fso = Nothing
        ExportToCSV = "Error creating Excel file"
    End Function

    Public Function ExportToCSVtext(ByVal dt As DataTable, Optional ByVal dlm As String = "", Optional ByVal hdr As String = "", Optional ByVal ftr As String = "") As String
        Dim ExpText As String
        Dim txtline As String = String.Empty
        Dim ret As String = String.Empty
        Dim m, i, j As Integer
        If dt Is Nothing AndAlso hdr.Trim = "" Then
            Return ret
            Exit Function
        End If
        If dlm = "" Then dlm = Chr(9)
        Dim MyFile As StringBuilder = New StringBuilder

        Try
            txtline = ""
            MyFile.AppendLine(hdr)
            'MyFile.AppendLine(txtline)
            If Not dt Is Nothing Then
                'dt columns names
                m = dt.Columns.Count
                For i = 0 To m - 1
                    If dt.Columns(i).ColumnName.ToString.IndexOf(dlm) >= 0 Then
                        txtline = txtline & """" & dt.Columns(i).ColumnName & """"
                    Else
                        txtline = txtline & dt.Columns(i).ColumnName
                    End If

                    If i < m - 1 Then txtline = txtline & dlm
                Next
                MyFile.AppendLine(txtline)
                'dt rows
                txtline = ""
                For j = 0 To dt.Rows.Count - 1
                    txtline = ""
                    For i = 0 To m - 1
                        If dt.Rows(j).Item(i).ToString.IndexOf(dlm) >= 0 Then
                            txtline = txtline & """" & dt.Rows(j).Item(i).ToString & """"
                        Else
                            txtline = txtline & dt.Rows(j).Item(i).ToString
                        End If

                        If i < m - 1 Then txtline = txtline & dlm
                    Next
                    If Trim(txtline) <> "" Then
                        MyFile.AppendLine(txtline)
                    End If
                Next
            End If
            txtline = ""
            MyFile.AppendLine(txtline)
            MyFile.AppendLine(ftr)
            ExpText = MyFile.ToString

            MyFile = Nothing
            ret = ExpText.ToString
        Catch ex As Exception
            ret = "Error creating csv text" & ex.Message
            MyFile = Nothing
        End Try
        Return ret
    End Function
    Public Function ExportGroupsToCSVtext(ByVal dt As DataTable, Optional ByVal dlm As String = "", Optional ByVal hdr As String = "", Optional ByVal ftr As String = "") As String
        Dim ExpText As String
        Dim txtline As String = String.Empty
        Dim ret As String = String.Empty
        Dim m, i, j As Integer
        If dt Is Nothing AndAlso hdr.Trim = "" Then
            Return ret
            Exit Function
        End If
        If dlm = "" Then dlm = Chr(9)
        Dim MyFile As StringBuilder = New StringBuilder

        Try
            txtline = ""
            MyFile.AppendLine(hdr)
            'MyFile.AppendLine(txtline)
            If Not dt Is Nothing Then
                'dt columns names
                m = dt.Columns.Count
                For i = 0 To m - 1
                    If dt.Columns(i).ColumnName.ToString.IndexOf(dlm) >= 0 Then
                        txtline = txtline & """" & dt.Columns(i).ColumnName & """"
                    Else
                        txtline = txtline & dt.Columns(i).ColumnName
                    End If

                    If i < m - 1 Then txtline = txtline & " versus " & dlm
                Next
                txtline = txtline.Replace("Tbl1Fld1", "Group1").Replace("Tbl2Fld2", "Group2") & ": " & dlm
                MyFile.AppendLine(txtline)
                'dt rows
                txtline = ""
                For j = 0 To dt.Rows.Count - 1
                    txtline = ""
                    For i = 0 To m - 1
                        If i > 0 AndAlso (txtline).Contains(dt.Rows(j).Item(i).ToString) Then
                            txtline = ""
                            Exit For
                        End If
                        If dt.Rows(j).Item(i).ToString.IndexOf(dlm) >= 0 Then
                            txtline = txtline & """" & dt.Rows(j).Item(i).ToString & """"
                        Else
                            txtline = txtline & dt.Rows(j).Item(i).ToString
                        End If

                        If i < m - 1 Then txtline = txtline & " versus " & dlm
                    Next
                    If Trim(txtline) <> "" Then
                        txtline = " (" & txtline & ") "
                        MyFile.AppendLine(txtline)
                    End If
                Next
            End If
            txtline = ""
            MyFile.AppendLine(txtline)
            MyFile.AppendLine(ftr)
            ExpText = MyFile.ToString

            MyFile = Nothing
            ret = ExpText.ToString
        Catch ex As Exception
            ret = "Error creating csv text" & ex.Message
            MyFile = Nothing
        End Try
        Return ret
    End Function
    '    Public Function ImportDataTableIntoSQLserver(ByVal dv As DataView, ByVal constr As String, ByVal tablename As String)
    '        'NOT IN USE, very old
    '        Dim mSQL As String

    '        Dim myConn As New SqlConnection
    '        myConn.ConnectionString = constr
    '        myConn.Open()

    '        Dim myCommand As New SqlCommand
    '        myCommand.Connection = myConn
    '        myCommand.CommandType = CommandType.Text
    '        If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()

    '        'drop table if exists in SQL Server database
    '        On Error Resume Next
    '        mSQL = "DROP TABLE [dbo].[" & tablename & "]"
    '        myCommand.CommandText = mSQL
    '        myCommand.ExecuteNonQuery()
    '        MsgBox("Previous Table is deleted.")

    '        On Error GoTo ErrMsg  '!!!!!!!!!!!!!uncomment for production
    '        'create new table in SQL Server database 
    '        Dim n, i, j As Integer
    '        mSQL = "CREATE TABLE [dbo].[" & tablename & "]("

    '        n = dv.Table.Rows.Count
    '        If dv.Table.Rows.Count > 0 Then  'if table has rows
    '            For j = 1 To dv.Table.Columns.Count
    '                mSQL = mSQL & " [" & dv.Table.Columns.Item(j - 1).Caption & "] " & "  [nvarchar](255) NULL ,"
    '            Next
    '            mSQL = mSQL & " [ID1] [int] IDENTITY(1,1) NOT NULL ) " ', CONSTRAINT [PK_" & tablename & "] PRIMARY KEY CLUSTERED ( [ID1](Asc) ) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]) ON [PRIMARY] "
    '        End If

    '        myCommand.CommandText = mSQL
    '        myCommand.ExecuteNonQuery()
    '        MsgBox("Table is created.")

    '        Dim sbc As New SqlBulkCopy(myConn)
    '        sbc.DestinationTableName = tablename
    '        sbc.WriteToServer(dv.Table)
    '        MsgBox("Table is exported.")

    '        myConn.Close()
    '        myConn.Dispose()
    '        myCommand.Dispose()

    '        Exit Function
    'ErrMsg:
    '        MsgBox("You do not have a permission to write into the database. Contact the system administrator.")

    '    End Function
    Public Function ImportDataTableIntoAccess(ByVal dv As DataView, ByVal constr As String, ByVal tablename As String) As String

        'not currently used

        Dim ret As String = String.Empty
        Dim myConn As New OleDbConnection
        myConn.ConnectionString = constr
        myConn.Open()

        Dim myCommand As New OleDbCommand
        myCommand.Connection = myConn
        myCommand.CommandType = CommandType.Text
        If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()

        'drop table if exists in SQL Server database
        On Error Resume Next                                            '!!!!!!!!!!!!!uncomment for production
        myCommand.CommandText = "DROP TABLE [" & tablename & "]"
        myCommand.ExecuteNonQuery()
        MsgBox("Previous Table is deleted.")

        On Error GoTo ErrMsg                                            '!!!!!!!!!!!!!uncomment for production

        'create new table in SQL Server database 
        Dim n, i, j As Integer
        Dim mSQL, m2SQL, mfields As String
        n = dv.Table.Rows.Count
        mfields = ""
        If dv.Table.Rows.Count > 0 Then  'if table has rows
            mSQL = "CREATE TABLE [" & tablename & "]("
            mfields = ""

            For j = 1 To dv.Table.Columns.Count - 1
                mSQL = mSQL & " [" & dv.Table.Columns.Item(j - 1).Caption & "] TEXT(255),"
                mfields = mfields & " [" & dv.Table.Columns.Item(j - 1).Caption & "], "
            Next

            mSQL = mSQL & " [ID1] COUNTER )"
            mfields = mfields & " [ID1] "
            ' VALUES ("
            'For j = 1 To dv.Table.Columns.Count - 1
            'mSQL = mSQL & " '" & dv.Table.Rows(0)(j - 1).ToString & "',"
            'Next
            'j = dv.Table.Columns.Count - 1
            'mSQL = mSQL & " '" & dv.Table.Rows(0)(j).ToString & "')"

            myCommand.CommandText = mSQL
            myCommand.ExecuteNonQuery()
            MsgBox("Table is created.")
        End If



        'Dim sbc As New SqlBulkCopy(myConn)
        'sbc.DestinationTableName = tablename
        'sbc.WriteToServer(dv.Table)
        'MsgBox("Table is exported.")
        m2SQL = ""
        'mfields = Left(mfields, Len(mfields) - 2)
        For i = 0 To n - 1
            m2SQL = ""
            m2SQL = "INSERT INTO [" & tablename & "](" & mfields & ") VALUES ("
            For j = 0 To dv.Table.Columns.Count - 2
                m2SQL = m2SQL & " '" & dv.Table.Rows(i)(j).ToString & "',"
            Next
            j = dv.Table.Columns.Count - 1
            m2SQL = m2SQL & dv.Table.Rows(i)(j).ToString & ")"
            myCommand.CommandText = m2SQL
            myCommand.ExecuteNonQuery()

        Next
        'MsgBox("Table is exported into Access database.")

        myConn.Close()
        myConn.Dispose()
        myCommand.Dispose()
        Return ret
        Exit Function
ErrMsg:
        'MsgBox("Error occured. You might not have a permission to write into the database. Contact the system administrator.")

        Return ret
    End Function
    Public Function ReturnDataTblFromXML(ByVal XmlFile As String, Optional er As String = "") As DataTable
        If XmlFile = String.Empty OrElse File.Exists(XmlFile) = False Then
            Return Nothing
            Exit Function
        End If
        Dim dset As DataSet
        dset = New DataSet

        Try   'On Error GoTo ErrMsg

            Dim fstr As New System.IO.StreamReader(XmlFile)
            dset.ReadXml(fstr)

            Dim dtbl As DataTable
            dtbl = dset.Tables(0)

            fstr.Close()
            dset.Dispose()

            Return dtbl

            Exit Function

        Catch ex As Exception
            er = "Close XML file first." & ex.Message
        End Try
        Return Nothing
    End Function
    Public Function ReturnDataTblFromOURFiles(ByVal repid As String, Optional er As String = "") As DataTable
        Dim dset As DataSet
        dset = New DataSet
        Dim dtbl As DataTable = Nothing
        Dim xsdstr As String = String.Empty
        Try
            Dim dv As DataView = mRecords("SELECT * From OURFiles WHERE ReportId='" & repid & "' AND Type='XSD'")
            If Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
                xsdstr = dv.Table.Rows(0)("FileText")
            End If
            Dim xsdstrm As New System.IO.StringReader(xsdstr)
            dset.ReadXml(xsdstrm)
            dtbl = dset.Tables(0)
            dset.Dispose()
        Catch ex As Exception
            er = ex.Message
        End Try
        Return dtbl
    End Function

    Public Function ReturnDataViewFromXML(ByVal XmlFile As String) As DataView
        If XmlFile = String.Empty OrElse File.Exists(XmlFile) = False Then
            Return Nothing
            Exit Function
        End If
        Dim dset As DataSet
        dset = New DataSet

        'On Error GoTo ErrMsg

        Dim fstr As New System.IO.StreamReader(XmlFile)
        dset.ReadXml(fstr)

        Dim dtbl As DataTable
        dtbl = dset.Tables(0)

        fstr.Close()
        dset.Dispose()

        Return dtbl.DefaultView

        Exit Function
ErrMsg:
        MsgBox("Close XML file first or other error occured.")
    End Function
    'Public Function SearchBank(ByVal myString, ByVal myfaceid, ByVal cont) As DataView

    '    If IsDBNull(myString) = True Then
    '        myString = ""
    '        Exit Function
    '    End If

    '    Dim myConnection As SqlConnection
    '    Dim myCommand As New SqlClient.SqlCommand
    '    Dim myAdapter As SqlClient.SqlDataAdapter
    '    Dim myRecords As DataTable
    '    Dim myView As DataView

    '    Dim myconstring As String
    '    myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
    '    myConnection = New SqlConnection(myconstring)
    '    myCommand.Connection = myConnection
    '    myCommand.CommandType = CommandType.StoredProcedure

    '    If cont = 1 Then
    '        myCommand.CommandText = "xp_cust_SearchBankDept"
    '    Else
    '        myCommand.CommandText = "xp_cust_SearchBank"
    '    End If

    '    'parameters to store-procedure
    '    myCommand.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SearchString", System.Data.SqlDbType.NVarChar))
    '    myCommand.Parameters.Add(New System.Data.SqlClient.SqlParameter("@faceid", System.Data.SqlDbType.NVarChar))
    '    myCommand.Parameters.Item(0).Value = myString
    '    myCommand.Parameters.Item(1).Value = myfaceid

    '    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
    '    myAdapter = New SqlClient.SqlDataAdapter(myCommand)
    '    myRecords = New DataTable
    '    myAdapter.Fill(myRecords)
    '    myView = myRecords.DefaultView
    '    myAdapter.Dispose()
    '    myCommand.Connection.Close()
    '    myCommand.Dispose()

    '    Return myView
    'End Function
    Function PullUpdatableDataFromSQLtable(ByVal SQLq As String, ByVal connstr As String) As DataTable
        'get updatable DataTable
        Dim cmdBuilder As SqlCommandBuilder
        Dim cmdTmp As New SqlCommand
        cmdTmp.Connection.ConnectionString = connstr
        If cmdTmp.Connection.State = ConnectionState.Closed Then cmdTmp.Connection.Open()
        cmdTmp.CommandType = CommandType.Text
        cmdTmp.CommandText = SQLq
        Dim rs = New SqlClient.SqlDataAdapter(cmdTmp)
        cmdBuilder = New SqlCommandBuilder(rs)
        Dim myTable = New DataTable
        rs.Fill(myTable)
        Return myTable
    End Function
    Public Function AddRowIntoSQLtable(ByVal mRow As DataRow, ByVal SQLq As String, ByVal connstr As String) As String
        'get updatable DataTable
        Dim cmdBuilder As SqlCommandBuilder
        Dim cmdTmp As New SqlCommand
        cmdTmp.Connection.ConnectionString = connstr
        If cmdTmp.Connection.State = ConnectionState.Closed Then cmdTmp.Connection.Open()
        cmdTmp.CommandType = CommandType.Text
        cmdTmp.CommandText = SQLq
        Dim rs = New SqlClient.SqlDataAdapter(cmdTmp)
        cmdBuilder = New SqlCommandBuilder(rs)
        Dim myTable = New DataTable
        rs.Fill(myTable)
        'add row
        On Error GoTo ErrMsg
        myTable.ImportRow(mRow)
        rs.Update(myTable)
        AddRowIntoSQLtable = "The row has been inserted into the table."
        Exit Function
ErrMsg:
        AddRowIntoSQLtable = "Error inserting the row into the table."

    End Function
    Public Function GetReportTemplate(repid As String) As String
        Dim ReportTemplate As String = "Tabular"
        Dim dt As DataTable = GetReportInfo(repid)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            If Not IsDBNull(dt.Rows(0)("Param0type")) AndAlso dt.Rows(0)("Param0type") <> "" Then
                ReportTemplate = dt.Rows(0)("Param0type").ToString
            End If
        End If

        Return ReportTemplate
    End Function
    Public Function AddReportItem(repid As String, field As String, dbname As String, usrconnstr As String, usrprovider As String) As String
        Dim retr As String = String.Empty
        If Not ReportItemsExist(repid) Then
            retr = CreateReportItems(repid, dbname, usrconnstr, usrprovider)
            If retr.StartsWith("ERROR!!") Then
                Return retr
            End If
        Else
            Dim htFields As Hashtable = GetXSDFieldHashtable(repid)
            Dim tbl As String = String.Empty
            Dim fld As String = String.Empty
            Dim tblfld As String = String.Empty
            Dim sSql As String = String.Empty
            Dim j As Integer = 0

            Dim FieldLayout As String = "Block"

            Dim dt As DataTable = GetReportInfo(repid)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                If Not IsDBNull(dt.Rows(0)("Param0type")) AndAlso dt.Rows(0)("Param0type") <> "" Then
                    Dim ReportTemplate As String = dt.Rows(0)("Param0type").ToString
                    If ReportTemplate.ToUpper() = "FREEFORM" Then FieldLayout = "Inline"
                End If
            End If
            Dim dtf As DataTable = Nothing
            Dim dr As DataRow

            dtf = GetReportFields(repid)
            If dtf IsNot Nothing AndAlso dtf.Rows.Count > 0 Then
                For i As Integer = 0 To dtf.Rows.Count - 1
                    dr = dtf.Rows(i)
                    Dim val As String = dr("Val")
                    Dim caption As String = field
                    Dim ord As String = String.Empty
                    Dim fldtype As String = String.Empty

                    If htFields(val) IsNot Nothing Then
                        j = j + 1
                        ord = j.ToString
                        tblfld = htFields(val).ToString
                        tbl = Piece(tblfld, ".", 1)
                        fld = Piece(tblfld, ".", 2)

                        If val = field Then
                            If Not IsDBNull(dr("Prop1")) AndAlso dr("Prop1") <> String.Empty Then
                                caption = dr("Prop1")
                            End If
                            fldtype = GetFieldDataType(tbl, fld, usrconnstr, usrprovider)
                            If Not ReportItemExists(repid, tbl, fld) Then
                                sSql = "INSERT INTO OurReportItems "
                                sSql &= "(ReportID, ItemID,Caption,CaptionFontName,CaptionFontSize,"
                                sSql &= "CaptionFontStyle,CaptionUnderline,CaptionStrikeout,"
                                sSql &= "CaptionTextAlign,CaptionForeColor,CaptionBackColor,"
                                sSql &= "CaptionBorderColor,CaptionBorderStyle,CaptionBorderWidth,ReportItemType,"
                                sSql &= "FieldLayout,SQLDatabase,SQLTable,SQLField,SQLDataType,"
                                sSql &= "ItemOrder,FontName,FontSize,ForeColor,"
                                sSql &= "FontStyle,Underline,Strikeout,TextAlign,"
                                sSql &= "BackColor,BorderColor,BorderStyle,BorderWidth,[Section]) "
                                sSql &= "VALUES ('" & repid & "','"
                                sSql &= field & "','"
                                sSql &= caption & "','"
                                sSql &= "Tahoma','"
                                sSql &= "12px','"
                                sSql &= "Regular',"
                                sSql &= "0,"
                                sSql &= "0,'"
                                sSql &= "Left','"
                                sSql &= "black','"
                                sSql &= "white','"
                                sSql &= "lightgrey','"
                                sSql &= "Solid','"
                                sSql &= "1','"
                                sSql &= "DataField','"
                                sSql &= FieldLayout & "','"
                                sSql &= dbname & "','"
                                sSql &= tbl & "','"
                                sSql &= fld & "','"
                                sSql &= fldtype & "','"
                                sSql &= ord & "','"
                                sSql &= "Tahoma','"
                                sSql &= "12px','"
                                sSql &= "black','"
                                sSql &= "Regular',"
                                sSql &= "0,"
                                sSql &= "0,'"
                                sSql &= "Left','"
                                sSql &= "white','"
                                sSql &= "lightgrey','"
                                sSql &= "Solid','"
                                sSql &= "1','"
                                sSql &= "Details')"
                                retr = ExequteSQLquery(sSql)
                                If retr <> "Query executed fine." Then
                                    Return "ERROR!! " & retr
                                End If
                            End If
                        Else
                            sSql = "UPDATE OURReportItems "
                            sSql &= "SET ItemOrder = '" & ord.ToString & "' "
                            sSql &= "Where ReportID = '" & repid & "' AND SQLTable = '" & tbl & "' AND SQLField = '" & fld & "'"
                            retr = ExequteSQLquery(sSql)
                            If retr <> "Query executed fine." Then
                                Return "ERROR!! " & retr
                            End If
                        End If
                    End If

                Next

            End If
        End If
        Return retr
    End Function
    Public Function CreateReportItems(repid As String, dbname As String, usrconnstr As String, usrprovider As String) As String
        Dim ret As String = String.Empty
        If Not ReportItemsExist(repid) Then
            Dim htFields As Hashtable = GetXSDFieldHashtable(repid)
            'Dim htTblCol As New Hashtable
            'Dim htDuplicate As New Hashtable
            'Dim htFieldType As New Hashtable
            'Dim htListId As New Hashtable
            Dim tbl As String = String.Empty
            Dim fld As String = String.Empty
            'Dim liID As String = String.Empty
            Dim tblfld As String = String.Empty
            Dim sSql As String = String.Empty
            Dim j As Integer = 0
            Dim dtf As DataTable = Nothing
            Dim dr As DataRow

            'Dim dt As DataTable = GetSQLFields(repid)
            'If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            If HasSQLData(repid) Then
                'For i As Integer = 0 To dt.Rows.Count - 1
                '    dr = dt.Rows(i)
                '    tbl = dr("Tbl1").ToString.ToLower
                '    fld = dr("Tbl1Fld1")
                '    tblfld = tbl & "." & fld
                '    If htFields(fld) IsNot Nothing Then
                '        If htDuplicate(fld) IsNot Nothing Then
                '            j = CInt(htDuplicate(fld))
                '            Dim f As String = fld
                '            fld = fld & j.ToString
                '            j += 1
                '            htDuplicate(f) = j.ToString
                '        Else
                '            htDuplicate.Add(fld, "2")
                '            fld = fld & "1"
                '        End If
                '    End If
                '    htFields.Add(fld, tblfld)
                '    'htTblCol.Add(tblfld, fld)
                'Next

                j = 0
                dtf = GetReportFields(repid)
                If dtf IsNot Nothing AndAlso dtf.Rows.Count > 0 Then
                    For i As Integer = 0 To dtf.Rows.Count - 1
                        dr = dtf.Rows(i)
                        Dim field As String = dr("Val")
                        Dim caption As String = field
                        Dim ord As String = String.Empty
                        Dim fldtype As String = String.Empty
                        'Dim fldID As String = String.Empty

                        If htFields(field) IsNot Nothing Then
                            ord = (j + 1).ToString
                            j += 1
                            tblfld = htFields(field).ToString
                            tbl = Piece(tblfld, ".", 1)
                            fld = Piece(tblfld, ".", 2)
                            If Not IsDBNull(dr("Prop1")) AndAlso dr("Prop1") <> String.Empty Then
                                caption = dr("Prop1")
                            End If
                            fldtype = GetFieldDataType(tbl, fld, usrconnstr, usrprovider)
                            'fldID = tblfld.Replace(".", "_") & "_" & ord

                            sSql = "INSERT INTO OurReportItems "
                            sSql &= "(ReportID, ItemID,Caption,CaptionFontName,CaptionFontSize,"
                            sSql &= "CaptionFontStyle,CaptionUnderline,CaptionStrikeout,"
                            sSql &= "CaptionTextAlign,CaptionForeColor,CaptionBackColor,"
                            sSql &= "CaptionBorderColor,CaptionBorderStyle,CaptionBorderWidth,ReportItemType,"
                            sSql &= "FieldLayout,SQLDatabase,SQLTable,SQLField,SQLDataType,"
                            sSql &= "ItemOrder,FontName,FontSize,ForeColor,"
                            sSql &= "FontStyle,Underline,Strikeout,TextAlign,"
                            sSql &= "BackColor,BorderColor,BorderStyle,BorderWidth,[Section]) "
                            sSql &= "VALUES ('" & repid & "','"
                            sSql &= field & "','"
                            sSql &= caption & "','"
                            sSql &= "Tahoma','"
                            sSql &= "12px','"
                            sSql &= "Regular',"
                            sSql &= "0,"
                            sSql &= "0,'"
                            sSql &= "Left','"
                            sSql &= "black','"
                            sSql &= "white','"
                            sSql &= "lightgrey','"
                            sSql &= "Solid','"
                            sSql &= "1','"
                            sSql &= "DataField','"
                            sSql &= "Block','"
                            sSql &= dbname & "','"
                            sSql &= tbl & "','"
                            sSql &= fld & "','"
                            sSql &= fldtype & "','"
                            sSql &= ord & "','"
                            sSql &= "Tahoma','"
                            sSql &= "12px','"
                            sSql &= "black','"
                            sSql &= "Regular',"
                            sSql &= "0,"
                            sSql &= "0,'"
                            sSql &= "Left','"
                            sSql &= "white','"
                            sSql &= "lightgrey','"
                            sSql &= "Solid','"
                            sSql &= "1','"
                            sSql &= "Details')"
                            ret = ExequteSQLquery(sSql)
                            If Not ret = "Query executed fine." Then
                                ret = "ERROR!! " & ret
                            End If
                        End If
                    Next
                End If
            End If
        End If
        Return ret
    End Function
    Public Function CreateRowInHTMLtable(ByVal mTable As HtmlTable) As String
        'create new row in mTable
        Dim j As Integer
        Dim AddRow As HtmlTableRow = New HtmlTableRow()
        For j = 0 To mTable.Rows(0).Cells.Count - 1
            Dim cell As HtmlTableCell
            cell = New HtmlTableCell()
            cell.Controls.Add(New LiteralControl("row0, " & "column " & j.ToString()))
            AddRow.Cells.Add(cell)
        Next j
        mTable.Rows.Add(AddRow)
        Return "done"
    End Function
    Public Function CreateNewRowInHtmlTable(ByVal mTable As HtmlTable) As String
        Try
            'create new row in mTable
            Dim AddRow As HtmlTableRow = New HtmlTableRow()
            For j = 0 To mTable.Rows(0).Cells.Count - 1
                Dim cell As HtmlTableCell
                cell = New HtmlTableCell()
                cell.Controls.Add(New LiteralControl("row0, " & "column " & j.ToString()))
                AddRow.Cells.Add(cell)
            Next j
            mTable.Rows.Add(AddRow)
            Return "Record created"
        Catch ex As Exception
            Return "ERROR!! " & ex.Message
        End Try
    End Function
    Public Function AddRowIntoHTMLtableWithNcols(ByVal mRow As DataRow, ByVal mTable As HtmlTable, ByVal m As Integer) As String
        Dim j As Integer
        Dim ret As String = String.Empty
        Try
            If mRow.RowState = DataRowState.Deleted Then Return "Error inserting the row into the table."
            'create new row in mTable
            Dim AddRow As HtmlTableRow = New HtmlTableRow()
            For j = 0 To m - 1
                Dim cell As HtmlTableCell
                cell = New HtmlTableCell()
                cell.Controls.Add(New LiteralControl("row0, " & "column " & j.ToString()))
                AddRow.Cells.Add(cell)
            Next j

            'assign values for row's fields
            For j = 0 To Min(m - 1, mRow.ItemArray.Length - 1)
                AddRow.Cells(j).InnerText = mRow(j).ToString
            Next
            mTable.Rows.Add(AddRow)
            ret = "Row has been inserted into the table."

        Catch ex As Exception
            ret = "Error inserting the row into the table."
        End Try
        Return ret
    End Function
    Public Function AddRowIntoHTMLtable(ByVal mRow As DataRow, ByVal mTable As HtmlTable) As String
        Dim j As Integer
        Dim ret As String = String.Empty
        Try
            If mRow.RowState = DataRowState.Deleted Then Return "Error inserting the row into the table."
            'create new row in mTable
            Dim AddRow As HtmlTableRow = New HtmlTableRow()
            For j = 0 To mTable.Rows(0).Cells.Count - 1
                Dim cell As HtmlTableCell
                cell = New HtmlTableCell()
                cell.Controls.Add(New LiteralControl("row0, " & "column " & j.ToString()))
                AddRow.Cells.Add(cell)
            Next j

            'assign values for row's fields
            For j = 0 To Min(mTable.Rows(0).Cells.Count - 1, mRow.ItemArray.Length - 1)
                AddRow.Cells(j).InnerText = mRow(j).ToString
            Next
            mTable.Rows.Add(AddRow)
            ret = "Row has been inserted into the table."

        Catch ex As Exception
            ret = "Error inserting the row into the table."
        End Try
        Return ret
    End Function
    Public Function AddArrayIntoHTMLtable(ByVal mRow As Array, ByVal mTable As HtmlTable) As String
        Dim i, j As Integer

        'create new row in mTable
        Dim AddRow As HtmlTableRow = New HtmlTableRow()
        For j = 0 To mTable.Rows(0).Cells.Count - 1
            Dim cell As HtmlTableCell
            cell = New HtmlTableCell()
            cell.Controls.Add(New LiteralControl("row " & i.ToString() & ", " & "column " & j.ToString()))
            AddRow.Cells.Add(cell)
        Next j

        'assign values for row's fields
        For j = 1 To mTable.Rows(0).Cells.Count - 2
            AddRow.Cells(j).InnerText = mRow(j - 1)
        Next
        mTable.Rows.Add(AddRow)

        AddArrayIntoHTMLtable = "Row has been inserted into the table."
        Exit Function
ErrMsg:
        AddArrayIntoHTMLtable = "Error inserting the row into the table."

    End Function
    '    Public Function AddArrayIntoSQLtable(ByVal mRow As Array, ByVal SQLq As String) As String
    '        'NOT IN USE
    '        'TODO MySQL, Cache ?
    '        'get updatable DataTable
    '        Dim cmdBuilder As SqlCommandBuilder
    '        Dim cmdTmp As New SqlCommand
    '        Dim conn As New SqlConnection
    '        Dim myconstring As String
    '        myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
    '        conn.ConnectionString = myconstring
    '        cmdTmp.Connection = conn
    '        If cmdTmp.Connection.State = ConnectionState.Closed Then cmdTmp.Connection.Open()
    '        cmdTmp.CommandType = CommandType.Text
    '        cmdTmp.CommandText = SQLq
    '        Dim rs = New SqlClient.SqlDataAdapter(cmdTmp)
    '        cmdBuilder = New SqlCommandBuilder(rs)
    '        Dim myTable = New DataTable
    '        rs.Fill(myTable)

    '        'add row
    '        'On Error GoTo ErrMsg
    '        Dim myRow As DataRow = Nothing
    '        Dim j As Integer

    '        'NOT READY !!!!!!!!!!!!!!
    '        For j = 0 To myTable.Columns.Count - 2
    '            myRow(j) = mRow(j)
    '        Next

    '        myTable.ImportRow(myRow)
    '        rs.Update(myTable)
    '        AddArrayIntoSQLtable = "The row has been inserted into the table."
    '        Exit Function
    'ErrMsg:
    '        AddArrayIntoSQLtable = "Error inserting the row into the table."

    '    End Function
    Public Function GetListOfReportFormatFields(ByVal repid As String) As DataView
        'get list of report format fields
        Dim sctSQL As String = "SELECT * FROM OURReportFormat WHERE (ReportID='" & repid & "' AND Prop='FIELDS')"
        Dim dv As DataView = mRecords(sctSQL)
        Return dv
    End Function
    Public Function GetListOfFieldsFromOURReportSQLquery(ByVal repid As String) As DataView
        'get list of report fields
        Dim sctSQL As String = "SELECT * FROM OURReportSQLquery WHERE (ReportID='" & repid & "' AND Doing='SELECT')"
        Dim dv As DataView = mRecords(sctSQL)
        Return dv
    End Function
    Public Function GetListOfReportFields(ByVal repid As String) As DataTable
        'get list of fields from OURFiles
        Dim er As String = String.Empty
        Dim dt As DataTable = Nothing
        dt = ReturnDataTblFromOURFiles(repid, er)
        If dt Is Nothing OrElse er <> "" Then
            'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            Dim xsdfile As String = applpath & "XSDFILES\" & repid & ".xsd"
            dt = ReturnDataTblFromXML(xsdfile)
            If Not dt Is Nothing Then
                CreateXSDForDataTable(dt, repid, xsdfile)
            End If
        End If
        Return dt
    End Function
    Public Function GetReportGroups(ByVal rep As String) As DataTable
        Dim dt As DataTable = Nothing
        Dim sqls = "SELECT * FROM OURReportGroups WHERE ReportId = '" & rep & "' ORDER BY GrpOrder"
        dt = mRecords(sqls).Table 'from OUR db
        Return dt
    End Function
    Public Function HasReportGroups(rep As String) As Boolean
        Dim dt As DataTable = GetReportGroups(rep)
        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
            Return True
        End If
        Return False
    End Function

    Public Function HasGroupReportItems(ReportID As String) As Boolean
        Dim dt As DataTable = Nothing
        Dim sqls As String = "SELECT * FROM OURReportItems WHERE ReportId = '" & ReportID & "' AND ReportItemType = 'Group'"
        dt = mRecords(sqls).Table
        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
            Return True
        End If

        Return False
    End Function
    Public Function GetGroupReportItemsByItemOrder(ReportID As String) As DataTable
        Dim dt As DataTable = Nothing
        Dim sqls As String = "SELECT * FROM OURReportItems WHERE ReportId = '" & ReportID & "' AND ReportItemType = 'Group' Order By ItemOrder"
        dt = mRecords(sqls).Table
        Return dt
    End Function
    Public Function GetGroupReportItem(ReportID As String, ItemID As String) As DataTable
        Dim dt As DataTable = Nothing
        Dim sqls As String = "SELECT * FROM OURReportItems WHERE ReportId = '" & ReportID & "' AND ReportItemType = 'Group' AND Caption = '" & ItemID & "'"
        dt = mRecords(sqls).Table
        Return dt
    End Function

    Public Function GroupReportItemExists(ReportID As String, ItemID As String) As Boolean
        Dim dt As DataTable = GetGroupReportItem(ReportID, ItemID)

        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
            Return True
        End If

        Return False
    End Function
    Public Sub UpdateGroupReportItemOrders(ReportID As String)
        Dim dt As DataTable = GetDistinctReportGroups(ReportID)
        Dim htGroupFields As New Hashtable
        Dim dr As DataRow
        Dim j As Integer = 0
        Dim grp As String


        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            htGroupFields.Add("Overall", "1")
            For i As Integer = 0 To dt.Rows.Count - 1
                dr = dt.Rows(i)
                grp = dr("GroupField").ToString
                If htGroupFields(grp) Is Nothing OrElse htGroupFields(grp) <> "1" Then
                    j += 1
                    CreateGroupReportItem(ReportID, grp, j.ToString)
                    htGroupFields(grp) = "1"
                End If
            Next
        End If

    End Sub
    Public Function CreateGroupReportItem(ReportID As String, GroupField As String, ItemOrder As String) As DataTable
        Dim dt As New DataTable
        Dim sSql As String
        Dim ItemID As String = GroupField & "_" & ItemOrder
        Dim ret As String

        If Not GroupReportItemExists(ReportID, GroupField) Then 'Create

            sSql = "INSERT INTO OurReportItems "
            sSql &= "(ReportID,ItemID,Caption,ReportItemType,"
            sSql &= "ItemOrder,FontName,FontSize,ForeColor,"
            sSql &= "FontStyle,Underline,Strikeout,Height,TextAlign,"
            sSql &= "BackColor,BorderColor,BorderStyle,BorderWidth,`Section`) "

            sSql &= "VALUES ('" & ReportID & "','"
            sSql &= ItemID & "','"
            sSql &= GroupField & "','"
            sSql &= "Group','"
            sSql &= ItemOrder & "','"
            sSql &= "Tahoma','"
            sSql &= "12px','"
            sSql &= "White','"
            sSql &= "Regular',"
            sSql &= "0,"
            sSql &= "0,'"
            sSql &= "24','"
            sSql &= "Left','"
            sSql &= "DimGray','"
            sSql &= "lightgrey','"
            sSql &= "None','"
            sSql &= "1','"
            sSql &= "Groups')"

            ret = ExequteSQLquery(sSql)

            If ret = "Query executed fine." Then
                dt = GetGroupReportItem(ReportID, GroupField)
            End If
        Else  'update ItemID and ItemOrder
            sSql = "UPDATE OURReportItems "
            sSql &= "SET ItemID = '" & ItemID & "',"
            sSql &= "ItemOrder = '" & ItemOrder & "' "
            sSql &= "WHERE ReportID = '" & ReportID & "' And Caption = '" & GroupField & "' And ReportItemType = 'Group'"

            ret = ExequteSQLquery(sSql)

            If ret = "Query executed fine." Then
                dt = GetGroupReportItem(ReportID, GroupField)
            End If

        End If

        Return dt
    End Function
    Public Sub DeleteGroupReportItems(ReportID As String, Optional ItemID As String = "")
        Dim sqls As String = "DELETE FROM OURReportItems WHERE ReportId = '" & ReportID & "' AND ReportItemType = 'Group'"
        If ItemID <> String.Empty Then
            sqls = "DELETE FROM OURReportItems WHERE ReportId = '" & ReportID & "' AND ReportItemType = 'Group' AND Caption = '" & ItemID & "'"
        End If
        ExequteSQLquery(sqls)
    End Sub
    Public Function GetDistinctReportGroups(ByVal rep As String) As DataTable
        Dim dt As DataTable = Nothing
        Dim sqls As String = "SELECT DISTINCT GrpOrder,GroupField,PageBrk FROM OURReportGroups WHERE ReportId = '" & rep & "' ORDER BY GrpOrder"

        dt = mRecords(sqls).Table  'from OUR db

        ' DISTINCT in Cache return upper case. It needed a correction: !!!!!!!!!!
        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
            Dim i As Integer
            Dim myprovider, temp As String
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            If myprovider.StartsWith("InterSystems.Data.") Then
                'DISTINCT returns upper case, fixing dt
                Dim dtemp As DataTable = Nothing
                For i = 0 To dt.Rows.Count - 1
                    temp = dt.Rows(i)("GroupField")  'upper case to translate to original
                    sqls = "Select TOP 1 GroupField FROM OURReportGroups WHERE UCASE(GroupField)='" & temp & "' "
                    dtemp = mRecords(sqls).Table 'from OUR db
                    If Not dtemp Is Nothing AndAlso dtemp.Rows.Count > 0 Then
                        dt.Rows(i)("GroupField") = dtemp.Rows(0)("GroupField")
                    End If
                Next
            End If
            Return dt
        End If
        Return dt
    End Function
    Public Function GetReportListGroups(ByVal rep As String) As DataTable
        Dim dt As DataTable = Nothing
        Dim sqls = "SELECT DISTINCT GroupField FROM OURReportGroups WHERE ReportId = '" & rep & "'"

        dt = mRecords(sqls).Table 'from OUR db

        ' DISTINCT in Cache return upper case. It needed a correction !!!!!!!!!!
        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
            Dim i As Integer
            Dim myprovider, temp As String
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            If myprovider.StartsWith("InterSystems.Data.") Then
                'DISTINCT returns upper case, fixing dt
                Dim dtemp As DataTable = Nothing
                For i = 0 To dt.Rows.Count - 1
                    temp = dt.Rows(i)("GroupField")  'upper case to translate to original
                    sqls = "Select TOP 1 GroupField FROM OURReportGroups WHERE UCASE(GroupField)='" & temp & "' "
                    dtemp = mRecords(sqls).Table 'from OUR db
                    If Not dtemp Is Nothing AndAlso dtemp.Rows.Count > 0 Then
                        dt.Rows(i)("GroupField") = dtemp.Rows(0)("GroupField")
                    End If
                Next
            End If
            Return dt
        End If
        Return dt
    End Function
    Public Function GetReportGroupsByGrpFld(ByVal rep As String, ByVal grp As String, ByVal fld As String) As DataTable
        Dim dt As DataTable = Nothing
        Dim sqls = "SELECT * FROM OURReportGroups WHERE ReportId = '" & rep & "' AND GroupField = '" & grp & "' AND CalcField = '" & fld & "' ORDER BY  GrpOrder"
        dt = mRecords(sqls).Table 'from OUR db
        Return dt
    End Function
    Public Function DeleteReportGroup(ByVal rep As String, ByVal grp As String, ByVal Optional fld As String = "") As String
        Dim exm As String = String.Empty
        Dim sqls As String = "DELETE FROM OURReportGroups Where ReportId = '" & rep & "'  AND GroupField = '" & grp & "'"
        If fld <> String.Empty Then
            sqls = "DELETE FROM OURReportGroups WHERE ReportId = '" & rep & "' AND GroupField = '" & grp & "' AND CalcField = '" & fld & "'"
        End If
        exm = ExequteSQLquery(sqls)
        If grp <> "Overall" Then
            If fld = String.Empty Then
                DeleteGroupReportItems(rep, grp)
            Else
                sqls = "SELECT * FROM OURReportGroups Where ReportId = '" & rep & "'  AND GroupField = '" & grp & "'"
                Dim dt As DataTable = mRecords(sqls).Table
                If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                    DeleteGroupReportItems(rep, grp)
                End If
            End If

        End If
    End Function
    Public Function DeleteReportGroupByCalcFld(ByVal rep As String, ByVal CalcFld As String) As String
        If CalcFld = String.Empty Then Return " CalcFld not specified."

        Dim exm As String = String.Empty

        Dim sqls As String = "SELECT * FROM OURReportGroups Where ReportId = '" & rep & "'  AND CalcField = '" & CalcFld & "'"
        Dim dt As DataTable = mRecords(sqls).Table
        Dim dtGroups As DataTable

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then Return "No records exist for " & CalcFld

        Dim sGrp As String = String.Empty
        Dim htGrp As New Hashtable

        Dim dr As DataRow
        For i As Integer = 0 To dt.Rows.Count - 1
            dr = dt.Rows(i)
            sGrp = dr("GroupField").ToString
            If htGrp(sGrp) <> "1" Then
                htGrp.Add(sGrp, "1")
                sqls = "SELECT GroupField FROM OURReportGroups Where ReportID = '" & rep & "' AND GroupField = '" & sGrp & "'"
                dtGroups = mRecords(sqls).Table
                If dtGroups.Rows.Count = 1 Then
                    DeleteGroupReportItems(rep, sGrp)
                End If
            End If
        Next
        sqls = "DELETE FROM OURReportGroups Where ReportId = '" & rep & "'  AND CalcField = '" & CalcFld & "'"
        exm = ExequteSQLquery(sqls) 'from OUR db
        Return exm
    End Function
    Public Function SaveGroupsAndTotals(ByVal rep As String, ByVal dt As DataTable) As DataTable
        Dim insSQL As String
        Dim sqls = "DELETE FROM OURReportGroups WHERE ReportId = '" & rep & "'"
        ExequteSQLquery(sqls)
        For i = 0 To dt.Rows.Count - 1
            'insert record
            insSQL = "INSERT INTO OURReportGroups (ReportID,GroupField,CalcField,COMMENTS,CntChk,SumChk,MaxChk,MinChk,AvgChk,StDevChk,CntDistChk,FirstChk,LastChk,GrpOrder,PageBrk) "
            insSQL = insSQL & " VALUES('" & rep & "','" & dt.Rows(i)("GroupField") & "','" & dt.Rows(i)("CalcField") & "','" & dt.Rows(i)("Comments") & "','" & dt.Rows(i)("CntChk") & "','" & dt.Rows(i)("SumChk") & "','" & dt.Rows(i)("MaxChk") & "','" & dt.Rows(i)("MinChk") & "','" & dt.Rows(i)("AvgChk") & "','" & dt.Rows(i)("StDevChk") & "','" & dt.Rows(i)("CntDistChk") & "','" & dt.Rows(i)("FirstChk") & "','" & dt.Rows(i)("LastChk") & "','" & dt.Rows(i)("GrpOrder") & "','" & dt.Rows(i)("PageBrk") & "')"
            ExequteSQLquery(insSQL)
            If dt.Rows(i)("GroupField").ToString.ToUpper <> "OVERALL" Then
                CreateGroupReportItem(rep, dt.Rows(i)("GroupField"), dt.Rows(i)("GrpOrder"))
            End If
        Next
        sqls = "SELECT * FROM OURReportGroups WHERE ReportId = '" & rep & "' ORDER BY GrpOrder"
        Dim newdt = mRecords(sqls).Table  'from OUR db
        Return newdt
    End Function
    Public Function SaveSorts(ByVal rep As String, ByVal dt As DataTable, Optional ByRef err As String = "") As DataTable
        Dim insSQL As String
        Dim ret As String = String.Empty
        Dim sqls = "DELETE FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND Doing='ORDER BY'"
        ExequteSQLquery(sqls)
        For i = 0 To dt.Rows.Count - 1
            'insert record
            insSQL = "INSERT INTO OURReportSQLquery (ReportID, Doing, Tbl1, Tbl1Fld1, Type, Oper) "
            insSQL = insSQL & " VALUES('" & rep & "','ORDER BY','" & dt.Rows(i)("Tbl1") & "','" & dt.Rows(i)("Tbl1Fld1") & "','" & dt.Rows(i)("Type") & "','" & dt.Rows(i)("Oper") & "')"
            ret = ExequteSQLquery(insSQL)
            If ret = "Query executed fine." Then
                ret = ""
            Else
                err = err & "  " & ret
            End If
        Next
        err = err.Trim
        sqls = "SELECT * FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND Doing='ORDER BY' ORDER BY Oper"
        Dim newdt = mRecords(sqls).Table  'from OUR db
        Return newdt
    End Function
    Public Function SaveJoins(ByVal rep As String, ByVal dt As DataTable, Optional ByRef err As String = "") As DataTable
        Dim insSQL As String
        Dim ret As String = String.Empty
        Dim sqls = "DELETE FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND Doing='JOIN'"
        ExequteSQLquery(sqls)
        For i = 0 To dt.Rows.Count - 1
            'insert record
            insSQL = "INSERT INTO OURReportSQLquery (ReportID,Doing,Tbl1,Tbl1Fld1,Tbl2,Tbl2Fld2,Type,Oper,RecOrder,comments) "
            insSQL = insSQL & " VALUES('" & rep & "','JOIN','" & dt.Rows(i)("Tbl1").ToString & "','" & dt.Rows(i)("Tbl1Fld1").ToString & "','" & dt.Rows(i)("Tbl2").ToString & "','" & dt.Rows(i)("Tbl2Fld2").ToString & "','" & dt.Rows(i)("Type").ToString & "','" & dt.Rows(i)("Oper").ToString & "','" & dt.Rows(i)("RecOrder").ToString & "','" & dt.Rows(i)("comments").ToString & "')"
            ret = ExequteSQLquery(insSQL)
            If ret = "Query executed fine." Then
                ret = ""
            Else
                err = err & "  " & ret
            End If
        Next
        err = err.Trim
        sqls = "SELECT * FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND Doing='JOIN' ORDER BY RecOrder,Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2"
        Dim newdt = mRecords(sqls).Table  'from OUR db
        Return newdt
    End Function
    Public Function SaveReportFields(ByVal rep As String, ByVal dt As DataTable) As DataTable
        Dim insSQL As String
        Dim sqls = "DELETE FROM OURReportFormat WHERE ReportId = '" & rep & "' AND Prop='FIELDS'"
        ExequteSQLquery(sqls)
        For i = 0 To dt.Rows.Count - 1
            'insert record
            insSQL = "INSERT INTO OURReportFormat (ReportID,Prop,Val," & FixReservedWords("Order") & ") "
            insSQL &= "VALUES('" & rep & "','FIELDS','" & dt.Rows(i)("Val") & "'," & dt.Rows(i)("Order").ToString & ")"
            ExequteSQLquery(insSQL)  'from OUR db
        Next
        sqls = "SELECT * FROM OURReportFormat WHERE ReportId = '" & rep & "'  AND Prop='FIELDS' ORDER BY [Order]"
        Dim newdt = mRecords(sqls).Table  'from OUR db
        Return newdt
    End Function


    Public Function GetReportListFields(ByVal rep As String) As DataTable
        Dim dt As DataTable = Nothing
        Dim sqls = "SELECT DISTINCT ListFld,Prop1,Prop3 FROM OURReportLists WHERE ReportId = '" & rep & "'"

        dt = mRecords(sqls).Table 'from OUR db

        ' DISTINCT in Cache return upper case. It needed a correction !!!!!!!!!!
        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
            Dim i As Integer
            Dim myprovider, temp As String
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            If myprovider.StartsWith("InterSystems.Data.") Then
                'DISTINCT returns upper case, fixing dt
                Dim dtemp As DataTable = Nothing
                For i = 0 To dt.Rows.Count - 1
                    temp = dt.Rows(i)("ListFld")  'upper case to translate to original
                    sqls = "Select TOP 1 ListFld FROM OURReportLists WHERE UCASE(ListFld)='" & temp & "' "
                    dtemp = mRecords(sqls).Table  'from OUR db
                    If Not dtemp Is Nothing AndAlso dtemp.Rows.Count > 0 Then
                        dt.Rows(i)("ListFld") = dtemp.Rows(0)("ListFld")
                    End If
                Next
            End If
            Return dt
        End If
        Return dt
    End Function
    Public Function GetReportLists(ByVal rep As String) As DataTable
        Dim dt As DataTable = Nothing
        Dim sqls = "SELECT * FROM OURReportLists WHERE ReportId = '" & rep & "' ORDER BY ListFld,RecFld"
        dt = mRecords(sqls).Table  'from OUR db
        Return dt
    End Function
    Public Function GetReportRecFldsByListFld(ByVal rep As String, ByVal listfld As String) As DataTable
        Dim dt As DataTable = Nothing
        Dim sqls = "SELECT * FROM OURReportLists WHERE ReportId = '" & rep & "' AND ListFld = '" & listfld & "' ORDER BY  RecFld"
        dt = mRecords(sqls).Table 'from OUR db
        Return dt
    End Function
    Public Function DeleteReportList(ByVal rep As String, ByVal fld As String, ByVal rec As String) As String
        Dim exm As String = String.Empty
        Dim sqls = "DELETE FROM OURReportLists WHERE ReportId = '" & rep & "' AND ListFld = '" & fld & "' AND Prop1 = '" & rec & "'"
        exm = ExequteSQLquery(sqls)
        sqls = "DELETE FROM OURReportFormat WHERE ReportID='" & rep & "' AND Prop='FIELDS' AND Val= '" & rec & "'"
        exm = ExequteSQLquery(sqls)
        sqls = "DELETE FROM OURReportItems WHERE ReportID = '" & rep & "' AND SQLTable = '" & rec & "'"
        exm = ExequteSQLquery(sqls)
        Return exm
    End Function
    Public Function GetReportFields(ByVal rep As String, Optional ByVal fixval As Boolean = False) As DataTable
        Dim dt As DataTable = Nothing
        Dim sqls = "SELECT * FROM OURReportFormat WHERE ReportId = '" & rep & "' AND Prop = 'FIELDS' ORDER BY " & FixReservedWords("Order")
        dt = mRecords(sqls).Table 'from OUR db
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            Return dt
        End If
        If fixval Then
            Dim i As Integer
            For i = 0 To dt.Rows.Count - 1
                If dt.Rows(i)("Val").ToString.IndexOf("_") >= 0 Then
                    dt.Rows(i)("Val") = dt.Rows(i)("Val").ToString.Replace("_", " ").Trim.Replace(" ", "_").Trim
                End If
            Next
        End If
        Return dt
    End Function
    Public Function GetFriendlyReportGroupName(ByVal rep As String, ByVal grp As String, ByVal fld As String) As String
        Dim dt As DataTable = Nothing
        Dim sqls = "SELECT *  FROM OURReportGroups WHERE (ReportID='" & rep & "' AND GroupField='" & grp & "' AND CalcField='" & fld & "' )"
        dt = mRecords(sqls).Table 'from OUR db
        If dt.Rows.Count > 0 AndAlso dt.Rows(0)("comments") IsNot Nothing AndAlso dt.Rows(0)("comments").ToString.Trim <> "" Then
            Return dt.Rows(0)("Comments").ToString
        Else
            Return grp
        End If
    End Function
    Public Function GetSuggestedFriendlyGroupName(ByVal rep As String, ByVal grp As String) As String
        Dim dt As DataTable = Nothing
        Dim sqls As String
        sqls = "SELECT *  FROM OURReportGroups WHERE (ReportID='" & rep & "' AND GroupField='" & grp & "' AND COMMENTS IS NOT NULL )"
        dt = mRecords(sqls).Table 'from OUR db
        If dt.Rows.Count > 0 AndAlso dt.Rows(0)("Comments") IsNot Nothing AndAlso dt.Rows(0)("Comments").ToString.Trim <> "" Then
            Return dt.Rows(0)("Comments").ToString
        Else
            sqls = "SELECT * FROM OURReportFormat WHERE Prop = 'FIELDS' AND VAL = '" & grp & "' AND Prop1 IS NOT NULL"
            dt = mRecords(sqls).Table 'from OUR db
            If dt.Rows.Count > 0 AndAlso dt.Rows(0)("Prop1") IsNot Nothing AndAlso dt.Rows(0)("Prop1").ToString.Trim <> "" Then
                Return dt.Rows(0)("Prop1").ToString
            Else
                Return ""
            End If
        End If
    End Function
    Public Function GetUnitDB(ByVal rep As String, Optional ByRef er As String = "") As String
        Dim msql As String = "SELECT ReportDB FROM OURReportInfo WHERE ReportId = '" & rep & "'"
        Dim dvu As DataView = mRecords(msql, er) 'from ourdata
        If dvu Is Nothing OrElse dvu.Table Is Nothing OrElse er <> "" OrElse dvu.Table.Rows.Count = 0 Then
            Return ""
        Else
            Return dvu.Table.Rows(0)("ReportDB").ToString
        End If
    End Function
    Public Function GetFriendlyFieldName(ByVal rep As String, ByVal fld As String, Optional ByRef nw As String = "") As String
        Dim frname As String = ""
        Dim er As String = ""
        Try
            Dim frn As String = ""
            'find friendly name
            frname = GetFriendlySQLFieldName(rep, fld, nw)  'look in OURReportSQLquery and OURFriendlyNames
            If frname = fld Then   'no friendly name in OURReportSQLquery and OURFriendlyNames
                frname = GetFriendlyReportFieldName(rep, fld, nw) 'look in OURReportFormat
                Return frname
            End If
        Catch ex As Exception
            nw = ex.Message
            Return ""
        End Try
        Return frname
    End Function
    Public Function GetFriendlySQLFieldName(ByVal rep As String, ByVal fld As String, Optional ByRef nw As String = "") As String
        If fld = "" Then
            nw = "ERROR!! Empty field name!"
            Return ""
        End If
        Dim dt As DataTable = Nothing
        Dim frname As String = fld
        Dim er As String = String.Empty
        Dim sqls As String = "SELECT * FROM OURReportSQLquery WHERE Doing = 'SELECT' AND Tbl1Fld1 = '" & fld & "' AND ReportId='" & rep & "'"
        dt = mRecords(sqls).Table  'from OUR db
        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
            If dt.Rows(0)("Friendly") IsNot Nothing AndAlso dt.Rows(0)("Friendly").ToString.Trim <> "" Then
                frname = dt.Rows(0)("Friendly").ToString
            ElseIf (dt.Rows(0)("Friendly") Is Nothing OrElse dt.Rows(0)("Friendly").ToString.Trim = "") Then
                frname = GlobalFriendlyName(dt.Rows(0)("Tbl1"), fld, GetUnitDB(rep), er)
                If er = "" AndAlso frname.Trim <> "" Then
                    Return frname
                Else
                    frname = fld
                End If
            End If
        Else
            'frname = GlobalFriendlyName(dt.Rows(0)("Tbl1"), fld, GetUnitDB(rep), er)
            If frname.Trim <> "" Then
                Return frname
            Else
                frname = fld
            End If
        End If
        Return frname
    End Function
    Public Function GetFriendlyReportFieldName(ByVal rep As String, ByVal fld As String, Optional ByRef nw As String = "") As String
        Dim dt As DataTable = Nothing
        Dim sqls = "SELECT * FROM OURReportFormat WHERE ReportId = '" & rep & "' AND Prop = 'FIELDS' AND VAL = '" & fld & "'"
        dt = mRecords(sqls).Table  'from OURReportFormat
        If dt.Rows.Count > 0 AndAlso dt.Rows(0)("Prop1") IsNot Nothing AndAlso dt.Rows(0)("Prop1").ToString.Trim <> "" Then
            Return dt.Rows(0)("Prop1").ToString
        Else
            Return fld
        End If
    End Function
    Public Function UpdateAllFriendlyNames(ByVal fcname As String, ByVal frname As String, ByVal logon As String, Optional userconnprv As String = "") As String
        'NOT IN USE?
        'update only for logon unit
        Dim ret, rep As String
        Dim i As Integer
        Dim updSQL As String = "UPDATE OURReportFormat SET Prop1='" & frname & "' WHERE (Prop='FIELDS' AND VAL='" & fcname & "' )"
        ret = ExequteSQLquery(updSQL)
        updSQL = "UPDATE OURReportSQLquery SET Friendly='" & frname & "' WHERE (Doing='SELECT' AND Tbl1Fld1='" & fcname & "' )"
        ret = ExequteSQLquery(updSQL)
        'update friendly names for all fields with current label
        Dim sqlq As String = "Select ReportId,SQLquerytext FROM OURReportInfo"
        dt = mRecords(sqlq).Table  'from OUR db
        For j = 0 To dt.Rows.Count - 1
            sqlq = dt.Rows(j)("SQLquerytext").ToString
            rep = dt.Rows(j)("ReportId").ToString
            If sqlq.IndexOf(".*") > 0 OrElse sqlq.IndexOf(" * ") > 0 Then
                sqlq = FixDoubleFieldNames(rep, sqlq, userconnprv)
            End If
            i = sqlq.ToUpper.IndexOf(" AS " & fcname)
            If i > 0 Then
                Dim temp As String = sqlq.Substring(0, i).Replace(",", " ").Replace(".", " ").Trim
                i = temp.LastIndexOf(" ")
                Dim fldnm As String = temp.Substring(i).Trim
                updSQL = "UPDATE OURReportFormat SET Prop1='" & frname & "' WHERE (Prop='FIELDS' AND VAL='" & fldnm & "' )"
                ret = ExequteSQLquery(updSQL)
                updSQL = "UPDATE OURReportSQLquery SET Friendly='" & frname & "' WHERE (Doing='SELECT' AND Tbl1Fld1='" & fldnm & "' )"
                ret = ExequteSQLquery(updSQL)
            End If
        Next
        Return ret
    End Function
    Public Function GetFieldExpression(rep As String, fld As String) As String
        Dim sExpression As String = String.Empty
        Dim dt As DataTable = Nothing
        Dim sqls = "SELECT Prop2 FROM OURReportFormat WHERE ReportId = '" & rep & "' AND Prop = 'FIELDS' AND VAL = '" & fld & "'"
        dt = mRecords(sqls).Table  'from OUR db
        If dt.Rows.Count > 0 AndAlso dt.Rows(0)("Prop2") IsNot Nothing AndAlso dt.Rows(0)("Prop2").ToString <> "" Then
            sExpression = dt.Rows(0)("Prop2").ToString
        End If
        Return sExpression
    End Function
    Public Function GetSuggestedFriendlyFieldName(ByVal fld As String, Optional ByVal rep As String = "", Optional ByRef nw As String = "", Optional userconnprv As String = "") As String
        'NOT IN USE
        Dim i As Integer
        Dim dt As DataTable = Nothing
        Dim sqls As String
        'Dim sqls As String = "SELECT * FROM OURReportFormat WHERE Prop = 'FIELDS' AND VAL = '" & fld & "' AND (Prop1 IS NOT NULL AND Prop1<>'')"
        'dt = mRecords(sqls).Table  'from OUR db
        'If Not dt Is Nothing AndAlso dt.Rows.Count > 0 AndAlso dt.Rows(0)("Prop1") IsNot Nothing AndAlso dt.Rows(0)("Prop1").ToString.Trim <> "" Then
        '    nw = "found"
        '    Return dt.Rows(0)("Prop1").ToString
        'Else
        '    sqls = "SELECT * FROM OURReportSQLquery WHERE Doing = 'SELECT' AND Tbl1Fld1 = '" & fld & "' AND (Friendly IS NOT NULL AND Friendly<>'')"
        '    dt = mRecords(sqls).Table   'from OUR db
        '    If Not dt Is Nothing AndAlso dt.Rows.Count > 0 AndAlso dt.Rows(0)("Friendly") IsNot Nothing AndAlso dt.Rows(0)("Friendly").ToString.Trim <> "" Then
        '        Return dt.Rows(0)("Friendly").ToString
        '    Else
        'look for field name using fld as field label
        Dim fldnm, temp As String
        'get report sql
        Dim dri As DataTable = GetReportInfo(rep)
        Dim sqlq As String = dri.Rows(0)("SQLquerytext").ToString
        sqlq = FixDoubleFieldNames(rep, sqlq, userconnprv)
        i = sqlq.ToUpper.IndexOf(" AS " & fld)
        If i > 0 Then
            temp = sqlq.Substring(0, i).Replace(",", " ").Replace(".", " ").Trim
            i = temp.LastIndexOf(" ")
            fldnm = temp.Substring(i).Trim
            sqls = "SELECT * FROM OURReportFormat WHERE Prop = 'FIELDS' AND VAL = '" & fldnm & "' AND (Prop1 IS NOT NULL AND Prop1<>'')"
            dt = mRecords(sqls).Table  'from OUR db
            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 AndAlso dt.Rows(0)("Prop1") IsNot Nothing AndAlso dt.Rows(0)("Prop1").ToString.Trim <> "" Then
                nw = "found"
                Return dt.Rows(0)("Prop1").ToString
            Else
                sqls = "SELECT * FROM OURReportSQLquery WHERE Doing = 'SELECT' AND Tbl1Fld1 = '" & fldnm & "' AND (Friendly IS NOT NULL AND Friendly<>'')"
                dt = mRecords(sqls).Table  'from OUR db
                If Not dt Is Nothing AndAlso dt.Rows.Count > 0 AndAlso dt.Rows(0)("Prop1") IsNot Nothing AndAlso dt.Rows(0)("Prop1").ToString.Trim <> "" Then
                    nw = "found"
                    Return dt.Rows(0)("Prop1").ToString
                Else
                    Return ""
                End If
            End If
        Else
            Return ""
        End If
        '    End If
        'End If
    End Function
    Public Function DeleteReportColumn(ByVal rep As String, ByVal fld As String) As String
        Dim ret As String = String.Empty
        Dim htFields As Hashtable = GetXSDFieldHashtable(rep)
        Dim tblfld As String = String.Empty

        If htFields(fld) IsNot Nothing Then
            tblfld = htFields(fld).ToString
        End If
        Dim sqls = "DELETE FROM OURReportFormat WHERE ReportId = '" & rep & "' AND Prop = 'FIELDS' AND Val = '" & fld & "'"
        ret = ExequteSQLquery(sqls)
        'delete group for the column
        sqls = "DELETE FROM OURReportGroups WHERE ReportId = '" & rep & "' AND GroupField = '" & fld & "'"
        ret = ret & " " & ExequteSQLquery(sqls)
        sqls = "DELETE FROM OURReportGroups WHERE ReportId = '" & rep & "' AND CalcField = '" & fld & "'"
        ret = ret & " " & ExequteSQLquery(sqls)
        'delete from OURReportLists
        sqls = "DELETE FROM OURReportLists WHERE ReportId = '" & rep & "' AND Prop1 = '" & fld & "'"
        ret = ret & " " & ExequteSQLquery(sqls)
        'sqls = "DELETE FROM OURReportLists WHERE ReportId = '" & rep & "' AND RecFld = '" & fld & "'"
        'ret = ret & " " & ExequteSQLquery(sqls)

        'delete from OURReportItems
        If fld.ToUpper.EndsWith("_COMBINED") Then
            sqls = "DELETE FROM OURReportItems WHERE ReportID = '" & rep & "' AND SQLTable = '" & fld & "'"
            ret = ret & " " & ExequteSQLquery(sqls)
        ElseIf htFields.Count > 0 AndAlso tblfld <> String.Empty AndAlso tblfld.Contains(".") Then
            Dim tbl As String = Piece(tblfld, ".", 1)
            Dim field As String = Piece(tblfld, ".", 2)

            sqls = "DELETE FROM OURReportItems WHERE ReportID = '" & rep & "' AND SQLTable = '" & tbl & "' AND SQLField = '" & field & "'"
            ret = ret & " " & ExequteSQLquery(sqls)

        End If

        Return ret

    End Function
    Public Function UpReportField(ByVal repid As String, ByVal rfld As String) As DataTable
        Dim i As Integer
        Dim sqls As String = String.Empty
        Dim fldord As Integer = 0
        Dim dtb As DataTable = GetReportFields(repid) 'sorted by Order
        dtb = CorrectFieldOrder(repid, dtb, "Order", "OURReportFormat", "Val", "Prop", "FIELDS")
        For i = 0 To dtb.Rows.Count - 1
            If dtb.Rows(i)("VAL") = rfld Then
                fldord = dtb.Rows(i - 1)("Order")
                dtb.Rows(i - 1)("Order") = dtb.Rows(i)("Order")
                dtb.Rows(i)("Order") = fldord
                sqls = "UPDATE OURReportFormat SET [Order]=" & dtb.Rows(i - 1)("Order") & " WHERE ReportId = '" & repid & "' AND Val='" & dtb.Rows(i - 1)("Val") & "' AND Prop = 'FIELDS'"
                ExequteSQLquery(sqls)
                sqls = "UPDATE OURReportFormat SET [Order]=" & dtb.Rows(i)("Order") & " WHERE ReportId = '" & repid & "' AND Val= '" & dtb.Rows(i)("Val") & "' AND Prop = 'FIELDS'"
                ExequteSQLquery(sqls)
            End If
        Next
        CopyFormattedFieldOrder(repid)
        Return dtb
    End Function
    Public Sub CopyReportImage(ReportID As String, ReportIDNew As String, ByRef dr As DataRow)
        Dim fromImagePath As String = System.AppDomain.CurrentDomain.BaseDirectory() & dr("ImagePath").ToString.Replace("/", "\")
        Dim newImagePath As String = dr("ImagePath").ToString.Replace(ReportID, ReportIDNew)
        Dim toImagePath As String = System.AppDomain.CurrentDomain.BaseDirectory() & newImagePath.Replace("/", "\")
        Dim err As String

        Try
            File.Copy(fromImagePath, toImagePath)
            WriteToAccessLog("CopyReportImage", fromImagePath & " copied to " & toImagePath & ".", 3)
            dr("ImagePath") = newImagePath
        Catch ex As Exception
            WriteToAccessLog("CopyReportImage", "WARNING!! Copy from " & fromImagePath & " to " & toImagePath & " has failed with this message: " & ex.Message, 3)
        End Try
    End Sub

    Public Sub CopyFormattedFieldOrder(repid As String)
        If ReportItemsExist(repid) Then
            Dim htFields As Hashtable = GetXSDFieldHashtable(repid)
            Dim tblfld As String = String.Empty
            Dim tbl As String = String.Empty
            Dim field As String = String.Empty
            Dim rfld As String = String.Empty
            Dim fldord As Integer = 1
            Dim sSql As String = String.Empty
            Dim dtb As DataTable = GetReportFields(repid) 'sorted by Order

            For i As Integer = 0 To dtb.Rows.Count - 1
                rfld = dtb.Rows(i)("VAL")
                tblfld = htFields(rfld)

                If htFields.Count > 0 AndAlso tblfld <> String.Empty AndAlso tblfld.Contains(".") Then
                    tbl = Piece(tblfld, ".", 1)
                    field = Piece(tblfld, ".", 2)
                    If ReportItemExists(repid, tbl, field) Then
                        sSql = "UPDATE OURReportItems "
                        sSql &= "SET ItemOrder = '" & fldord.ToString & "' "
                        sSql &= "Where ReportID = '" & repid & "' AND SQLTable = '" & tbl & "' AND SQLField = '" & field & "'"
                        ExequteSQLquery(sSql)
                        fldord += 1
                    End If
                End If

            Next
        End If
    End Sub
    Public Function DownReportField(ByVal repid As String, ByVal rfld As String) As DataTable
        Dim i As Integer
        Dim sqls As String = String.Empty
        Dim fldord As Integer = 0
        Dim dtb As DataTable = GetReportFields(repid) 'sorted by Order
        dtb = CorrectFieldOrder(repid, dtb, "Order", "OURReportFormat", "Val", "Prop", "FIELDS")
        For i = 0 To dtb.Rows.Count - 1
            If dtb.Rows(i)("VAL") = rfld Then
                fldord = dtb.Rows(i + 1)("Order")
                dtb.Rows(i + 1)("Order") = dtb.Rows(i)("Order")
                dtb.Rows(i)("Order") = fldord
                sqls = "UPDATE OURReportFormat SET [Order]=" & dtb.Rows(i + 1)("Order") & " WHERE ReportId = '" & repid & "' AND Val= '" & dtb.Rows(i + 1)("Val") & "' AND Prop = 'FIELDS'"
                ExequteSQLquery(sqls)
                sqls = "UPDATE OURReportFormat SET [Order]=" & dtb.Rows(i)("Order") & " WHERE ReportId = '" & repid & "' AND Val= '" & dtb.Rows(i)("Val") & "' AND Prop = 'FIELDS'"
                ExequteSQLquery(sqls)
            End If
        Next
        CopyFormattedFieldOrder(repid)
        Return dtb
    End Function
    Public Function CorrectFieldOrder(ByVal rep As String, ByVal dtb As DataTable, ByVal orderfld As String, ByVal TblName As String, ByVal FldName As String, ByVal PropName As String, ByVal PropValue As String) As DataTable
        For i = 0 To dtb.Rows.Count - 1
            dtb.Rows(i)(orderfld) = i + 1
            Dim sqls = "UPDATE " & TblName & " SET [" & orderfld & "]=" & dtb.Rows(i)(orderfld) & " WHERE ReportId = '" & rep & "' AND " & FldName & "= '" & dtb.Rows(i)(FldName) & "' AND " & PropName & " = '" & PropValue & "'"
            ExequteSQLquery(sqls)
        Next
        Return dtb
    End Function
    Public Function ShowField(ByVal rep As String, ByVal FldName As String) As Boolean
        Dim show As Boolean = False
        Dim dtb As DataTable = GetReportFields(rep) 'sorted by Order
        For i = 0 To dtb.Rows.Count - 1
            If dtb.Rows(i)("Val") = FldName Then
                show = True
                Return show
                Exit For
            End If
        Next
        Return show
    End Function
    Public Function GetSQLFields(ByVal rep As String) As DataTable
        Dim dt As DataTable = Nothing
        Dim sqls = "SELECT * FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND DOING = 'SELECT' ORDER BY Tbl1,Tbl1Fld1"
        dt = mRecords(sqls).Table   'from OUR db
        Return dt
    End Function

    Public Function GetSQLTableFields(ByVal rep As String, ByVal tbl As String) As DataTable
        Dim dt As DataTable = Nothing
        Dim sqls = "SELECT * FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND DOING = 'SELECT' AND Tbl1='" & tbl & "' ORDER BY Tbl1Fld1"
        dt = mRecords(sqls).Table   'from OUR db
        Return dt
    End Function
    Public Function TblFieldIsNumeric(ByVal tbl As String, ByVal fld As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "") As Boolean
        Dim num As Boolean = Nothing
        Dim i As Integer
        Dim er As String = String.Empty
        Dim typ As String = String.Empty
        If tbl.Trim = "" OrElse fld.Trim = "" Then
            Return num
            Exit Function
        End If
        Dim dvt As DataTable = Nothing
        Dim myprovider As String = userconprv
        Dim myconstring As String = userconstr
        If userconstr = "" Then
            myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        End If
        Dim sqls As String
        sqls = String.Empty
        If myprovider.StartsWith("InterSystems.Data.") Then
            sqls = "SELECT Name AS COLUMN_NAME, Type AS DATA_TYPE FROM %Dictionary.PropertyDefinition WHERE UCASE(parent) = UCASE('" & tbl & "')"
        ElseIf myprovider = "MySql.Data.MySqlClient" Then
            Dim db As String = GetDataBase(myconstring, myprovider)
            sqls = "SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" & tbl & "' AND TABLE_SCHEMA='" & db.ToLower & "'"
        ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then
            sqls = "SELECT * FROM all_tab_cols WHERE UPPER(TABLE_NAME) = UPPER('" & tbl & "')"
        ElseIf myprovider = "System.Data.Odbc" Then
            'for System.Data.Odbc
            dvt = GetListOfTableFields(tbl, myconstring, myprovider, er).Table
        ElseIf myprovider = "System.Data.OleDb" Then
            'for System.Data.OleDb
            dvt = GetListOfTableFields(tbl, myconstring, myprovider, er).Table

        ElseIf myprovider = "Sqlite" Then  'Sqlite
            'for Sqlite
            dvt = GetListOfTableFields(tbl, myconstring, myprovider, er).Table
        ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
            Dim db As String = GetDataBase(myconstring, myprovider)
            sqls = "SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" & tbl & "' AND TABLE_SCHEMA ='public' AND LOWER(TABLE_CATALOG) ='" & db.ToLower & "'"

        Else
            sqls = "SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" & tbl & "'"
        End If
        If myprovider <> "System.Data.Odbc" AndAlso myprovider <> "System.Data.OleDb" AndAlso myprovider <> "Sqlite" Then
            dvt = mRecords(sqls, er, myconstring, myprovider).Table
        End If

        For i = 0 To dvt.Rows.Count - 1
            If (dvt.Rows(i)("COLUMN_NAME") = fld) Then
                typ = dvt.Rows(i)("DATA_TYPE").ToString.Replace("%", "").ToUpper
                If typ.ToUpper.Contains("INTEGER") OrElse typ.Contains("SmallInt".ToUpper) OrElse typ = "int".ToUpper OrElse typ.Contains("smallint".ToUpper) OrElse typ.Contains("float".ToUpper) OrElse typ.Contains("bigint".ToUpper) OrElse typ.ToUpper.Contains("DECIMAL") OrElse typ.ToUpper.Contains("NUMERIC") OrElse typ.ToUpper.Contains("DOUBLE") OrElse typ.ToUpper.Contains("BINARY") OrElse typ.ToUpper.Contains("NUMBER") Then
                    num = True
                Else
                    num = False
                End If
                Return num
                Exit Function
            End If
        Next
        Return num
    End Function
    Public Function TblFieldIsDateTime(ByVal tbl As String, ByVal fld As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "") As Boolean
        Dim num As Boolean = Nothing
        Dim i As Integer
        Dim er As String = String.Empty
        Dim typ As String = String.Empty
        If tbl.Trim = "" OrElse fld.Trim = "" Then
            Return num
            Exit Function
        End If
        Dim dvt As DataTable = Nothing
        Dim myprovider As String = userconprv
        Dim myconstring As String = userconstr
        If userconstr = "" Then
            myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        End If
        Dim sqls As String
        sqls = String.Empty
        If myprovider.StartsWith("InterSystems.Data.") Then
            sqls = "SELECT Name AS COLUMN_NAME, Type AS DATA_TYPE FROM %Dictionary.PropertyDefinition WHERE parent = '" & tbl & "'"
        ElseIf myprovider = "MySql.Data.MySqlClient" Then
            Dim db As String = GetDataBase(myconstring, myprovider)
            sqls = "SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" & tbl & "' AND TABLE_SCHEMA='" & db.ToLower & "'"
        ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then
            'for Oracle.ManagedDataAccess.Client
            sqls = "SELECT * FROM all_tab_cols WHERE UPPER(TABLE_NAME) = UPPER('" & tbl & "')"
        ElseIf myprovider = "System.Data.Odbc" Then
            dvt = GetListOfTableFields(tbl, myconstring, myprovider, er).Table
        ElseIf myprovider = "System.Data.OleDb" Then
            dvt = GetListOfTableFields(tbl, myconstring, myprovider, er).Table

        ElseIf myprovider = "Npgsql" Then  'PostgreSQL  Npgsql
            Dim db As String = GetDataBase(myconstring, myprovider)
            sqls = "SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" & tbl & "' AND TABLE_SCHEMA ='public' AND LOWER(TABLE_CATALOG) ='" & db.ToLower & "'"

        Else
            sqls = "SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" & tbl & "'"
        End If
        If myprovider <> "System.Data.Odbc" AndAlso myprovider <> "System.Data.OleDb" Then
            dvt = mRecords(sqls, er, myconstring, myprovider).Table
        End If

        For i = 0 To dvt.Rows.Count - 1
            If (dvt.Rows(i)("COLUMN_NAME") = fld) Then
                typ = dvt.Rows(i)("DATA_TYPE").ToString.Replace("%", "").ToUpper
                If typ.Contains("DATE") OrElse typ.Contains("Time".ToUpper) Then
                    num = True
                Else
                    num = False
                End If
                Return num
                Exit Function
            End If
        Next
        Return num
    End Function
    Public Function TblSQLqueryFieldIsNumeric(ByVal rep As String, ByVal fld As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "") As Boolean
        Dim num As Boolean = Nothing
        'Dim i As Integer
        Dim typ As String = String.Empty
        If rep.Trim = "" OrElse fld.Trim = "" Then
            Return num
            Exit Function
        End If
        Dim dvf As DataView = mRecords("SELECT * FROM OURReportSQLquery WHERE (ReportID='" & rep & "' AND Doing='SELECT' AND Tbl1Fld1='" & fld & "')")
        If dvf Is Nothing OrElse dvf.Count = 0 OrElse dvf.Table.Rows.Count = 0 Then
            Return num
            Exit Function
        End If
        If TblFieldIsNumeric(dvf.Table.Rows(0)("Tbl1").ToString, dvf.Table.Rows(0)("Tbl1Fld1").ToString, userconstr, userconprv) Then
            num = True
        Else
            num = False
        End If
        Return num
    End Function
    Public Function DeleteSQLField(ByVal rep As String, ByVal tbl As String, ByVal fld As String) As String
        Dim ret As String = String.Empty
        Try
            'delete from SELECT statement
            Dim sqls As String = "DELETE FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND Tbl1 = '" & tbl & "' And Tbl1Fld1 = '" & fld & "'"
            ret = ExequteSQLquery(sqls)
            sqls = "DELETE FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND Tbl2 = '" & tbl & "' And Tbl2Fld2 = '" & fld & "'"
            ret = ExequteSQLquery(sqls)
            sqls = "DELETE FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND Tbl3 = '" & tbl & "' And Tbl3Fld3 = '" & fld & "'"
            ret = ExequteSQLquery(sqls)

            '!! DO NOT DELETE THESE LINES, THEY MIGHT BE NEEDED FOR CACHE OR MYSQL:
            'Dim sqls = "DELETE FROM OURReportSQLquery WHERE DOING='SELECT' AND ReportId = '" & rep & "' AND Tbl1 = '" & tbl & "' And Tbl1Fld1 = '" & fld & "'"
            ''delete from JOIN statement
            'sqls = "DELETE FROM OURReportSQLquery WHERE DOING='JOIN' AND ReportId = '" & rep & "' AND Tbl1 = '" & tbl & "' And Tbl1Fld1 = '" & fld & "'"
            'ret = ExequteSQLquery(sqls)
            'sqls = "DELETE FROM OURReportSQLquery WHERE DOING='JOIN' AND ReportId = '" & rep & "' AND Tbl2 = '" & tbl & "' And Tbl2Fld2 = '" & fld & "'"
            'ret = ExequteSQLquery(sqls)
            ''delete from all conditions in WHERE
            'sqls = "DELETE FROM OURReportSQLquery WHERE DOING='WHERE' AND ReportId = '" & rep & "' AND Tbl1 = '" & tbl & "' And Tbl1Fld1 = '" & fld & "'"
            'ret = ExequteSQLquery(sqls)
            'sqls = "DELETE FROM OURReportSQLquery WHERE DOING='WHERE' AND ReportId = '" & rep & "' AND Tbl2 = '" & tbl & "' And Tbl2Fld2 = '" & fld & "'"
            'ret = ExequteSQLquery(sqls)
            'sqls = "DELETE FROM OURReportSQLquery WHERE DOING='WHERE' AND ReportId = '" & rep & "' AND Tbl3 = '" & tbl & "' And Tbl3Fld3 = '" & fld & "'"
            'ret = ExequteSQLquery(sqls)

            ''delete from Report Fields
            'sqls = "DELETE FROM OURReportFormat WHERE ReportId = '" & rep & "' AND Prop = 'FIELDS' And Val = '" & fld & "'"
            'ret = ExequteSQLquery(sqls)
            'sqls = "DELETE FROM OURReportLists WHERE ReportId = '" & rep & "' AND ListFld = '" & fld & "'"
            'ret = ExequteSQLquery(sqls)
            'sqls = "DELETE FROM OURReportLists WHERE ReportId = '" & rep & "' AND RecFld = '" & fld & "'"
            'ret = ExequteSQLquery(sqls)
            'sqls = "DELETE FROM OURReportShow WHERE ReportId = '" & rep & "' AND DropDownFieldValue = '" & fld & "'"
            'ret = ExequteSQLquery(sqls)
            'sqls = "DELETE FROM OURReportGroups WHERE ReportId = '" & rep & "' AND GroupField = '" & fld & "'"
            'ret = ExequteSQLquery(sqls)
            'sqls = "DELETE FROM OURReportGroups WHERE ReportId = '" & rep & "' AND CalcField = '" & fld & "'"
            'ret = ExequteSQLquery(sqls)
            'sqls = "DELETE FROM OURReportItems WHERE ReportID = '" & rep & "' AND SQLTable = '" & tbl & "' AND SQLField = '" & fld & "'"
            'ret = ExequteSQLquery(sqls)

            DeleteReportField(rep, tbl, fld)

        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function DeleteReportField(ByVal rep As String, ByVal tbl As String, ByVal fld As String) As String
        Dim ret As String = String.Empty
        Try
            Dim sqls As String = String.Empty

            'delete from Report Fields
            sqls = "DELETE FROM OURReportFormat WHERE ReportId = '" & rep & "' AND Prop = 'FIELDS' And Val = '" & fld & "'"
            ret = ExequteSQLquery(sqls)
            sqls = "DELETE FROM OURReportLists WHERE ReportId = '" & rep & "' AND ListFld = '" & fld & "'"
            ret = ExequteSQLquery(sqls)
            sqls = "DELETE FROM OURReportLists WHERE ReportId = '" & rep & "' AND RecFld = '" & fld & "'"
            ret = ExequteSQLquery(sqls)
            sqls = "DELETE FROM OURReportShow WHERE ReportId = '" & rep & "' AND DropDownFieldValue = '" & fld & "'"
            ret = ExequteSQLquery(sqls)
            sqls = "DELETE FROM OURReportGroups WHERE ReportId = '" & rep & "' AND GroupField = '" & fld & "'"
            ret = ExequteSQLquery(sqls)
            sqls = "DELETE FROM OURReportGroups WHERE ReportId = '" & rep & "' AND CalcField = '" & fld & "'"
            ret = ExequteSQLquery(sqls)
            sqls = "DELETE FROM OURReportItems WHERE ReportID = '" & rep & "' AND SQLTable = '" & tbl & "' AND SQLField = '" & fld & "'"
            ret = ExequteSQLquery(sqls)
            sqls = "DELETE FROM OURReportItems WHERE ReportID = '" & rep & "' AND Caption = '" & fld & "' AND ReportItemType = 'Group'"
            ret = ExequteSQLquery(sqls)

        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function GetSQLJoinsPrev(ByVal rep As String) As DataTable
        Dim dt As DataTable = Nothing
        Dim sqls = "SELECT * FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND DOING = 'JOIN' ORDER BY RecOrder,Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2"
        dt = mRecords(sqls).Table  'from OUR db
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            Return dt
        End If
        'Fix order of tables, Oper field has primary table in the set indicator = 1
        Dim i As Integer
        Dim tb, fl As String
        For i = 0 To dt.Rows.Count - 1
            'the first record leave as it is. User can move rows on top and reverse it to make one table the very First one for the LEFT JOIN (!)
            'TODO - then starting from second row move up all rows having the table(s) from the first row, then from previous rows, etc.... 
            'This way all connected tables will be concentrated on top as first set. After that the Oper=1 assigned and new set starts.
            If i > 0 Then
                'first table on current row is the same as first table on previous row
                If dt.Rows(i - 1)("Tbl1") = dt.Rows(i)("Tbl1") Then
                    Continue For
                End If
                'TODO - compare not only to previous row but throw hole table  !!!
                'at this point the first table on current and previous rows are different
                'second table on current row is the same as first table on previous row
                If dt.Rows(i - 1)("Tbl1") = dt.Rows(i)("Tbl2") Then
                    tb = dt.Rows(i)("Tbl1")
                    fl = dt.Rows(i)("Tbl1Fld1")
                    dt.Rows(i)("Tbl1") = dt.Rows(i)("Tbl2")
                    dt.Rows(i)("Tbl1Fld1") = dt.Rows(i)("Tbl2Fld2")
                    dt.Rows(i)("Tbl2") = tb
                    dt.Rows(i)("Tbl2Fld2") = fl
                    dt.Rows(i)("Oper") = ""
                    'first table on current row is the same as second table on previous row
                ElseIf dt.Rows(i - 1)("Tbl2") = dt.Rows(i)("Tbl1") Then
                    tb = dt.Rows(i)("Tbl1")
                    fl = dt.Rows(i)("Tbl1Fld1")
                    dt.Rows(i)("Tbl1") = dt.Rows(i)("Tbl2")
                    dt.Rows(i)("Tbl1Fld1") = dt.Rows(i)("Tbl2Fld2")
                    dt.Rows(i)("Tbl2") = tb
                    dt.Rows(i)("Tbl2Fld2") = fl
                    dt.Rows(i)("Oper") = ""
                Else
                    'TODO - compare not only to previous row but throw hole table  !!!
                    'tables on current row are both different compare to previous row tables: add comma in Oper field
                    dt.Rows(i)("Oper") = "1"  'indicate that comma, not Join is needed
                End If
            End If
        Next
        Return dt
    End Function
    Public Function GetReportJoins(ByVal rep As String, Optional er As String = "") As DataTable
        Dim sqls As String = ""
        Dim dt As DataTable = Nothing
        Try
            sqls = "SELECT * FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND DOING = 'JOIN' ORDER BY RecOrder,Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2"
            dt = mRecords(sqls).Table  'from OUR db
            dt = CorrectJoinOrder(dt)
            Return dt
        Catch ex As Exception
            er = ex.Message
            Return Nothing
        End Try
    End Function
    Public Function GetSQLJoins(ByVal rep As String, Optional rpeat As Integer = 0, Optional newset As Integer = 0) As DataTable
        Dim dt As DataTable = Nothing
        'Dim sqls As String = "SELECT * FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND DOING = 'JOIN' ORDER BY RecOrder,Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2"
        'dt = mRecords(sqls).Table  'from OUR db
        dt = GetReportJoins(rep)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            Return dt
        End If
        'Fix order of tables, Oper field has primary table in the set indicator = 1
        Dim i, j As Integer
        Dim tblfirst, tbsecond As String
        Dim tb, fl, tbs As String
        newset = newset + 1
        If rpeat < dt.Rows.Count Then
            j = rpeat
            tblfirst = dt.Rows(j)("Tbl1")
            tbsecond = dt.Rows(j)("Tbl2")
            dt.Rows(j)("RecOrder") = newset
            tbs = "," & tblfirst & ","
            If rpeat > 0 Then
                dt.Rows(j)("Oper") = 1  'new set start
            Else
                dt.Rows(j)("Oper") = 0
            End If
            For i = j To dt.Rows.Count - 1
                If rpeat = 0 Then dt.Rows(j)("Oper") = 0  'first set 
                If dt.Rows(i)("Tbl1") = tblfirst AndAlso dt.Rows(i)("Tbl2") = tbsecond Then
                    dt.Rows(i)("RecOrder") = j + newset  'preposition: the count of different tables or joins <100
                    dt.Rows(i)("Param1") = newset 'current set (1 in first attempt)
                    j = j + 1
                ElseIf dt.Rows(i)("Tbl1") = tblfirst Then
                    dt.Rows(i)("RecOrder") = 100 * newset
                    dt.Rows(i)("Param1") = newset
                    If tbs.IndexOf("," & dt.Rows(i)("Tbl2") & ",") < 0 Then
                        tbs = tbs & dt.Rows(i)("Tbl2") & ","
                    End If
                    j = j + 1
                ElseIf dt.Rows(i)("Tbl2") = tblfirst Then
                    dt.Rows(i)("RecOrder") = 100 * newset
                    dt.Rows(i)("Param1") = newset
                    If tbs.IndexOf("," & dt.Rows(i)("Tbl1") & ",") < 0 Then
                        tbs = tbs & dt.Rows(i)("Tbl1") & ","
                    End If
                    'reverse row ?
                    tb = dt.Rows(i)("Tbl1").ToString
                    fl = dt.Rows(i)("Tbl1Fld1").ToString
                    dt.Rows(i)("Tbl1") = dt.Rows(i)("Tbl2").ToString
                    dt.Rows(i)("Tbl1Fld1") = dt.Rows(i)("Tbl2Fld2").ToString
                    dt.Rows(i)("Tbl2") = tb
                    dt.Rows(i)("Tbl2Fld2") = fl
                    j = j + 1
                ElseIf tbs.IndexOf("," & dt.Rows(i)("Tbl1") & ",") > 0 OrElse tbs.IndexOf("," & dt.Rows(i)("Tbl2") & ",") > 0 Then
                    dt.Rows(i)("RecOrder") = 200 * newset
                    dt.Rows(i)("Param1") = newset
                    If tbs.IndexOf("," & dt.Rows(i)("Tbl1") & ",") < 0 Then
                        tbs = tbs & dt.Rows(i)("Tbl1") & ","
                    End If
                    If tbs.IndexOf("," & dt.Rows(i)("Tbl2") & ",") < 0 Then
                        tbs = tbs & dt.Rows(i)("Tbl1") & ","
                    End If
                    j = j + 1
                Else 'both tables are not in the first set above
                    dt.Rows(i)("RecOrder") = 300 * newset
                    dt.Rows(i)("Param1") = 0
                End If
            Next
            'at that point tbs has all tables from the first set, but there might be still rows connected to the first set with RecOrder=2000
            For i = 1 To dt.Rows.Count - 1
                If dt.Rows(i)("RecOrder") = 300 * newset AndAlso (tbs.IndexOf("," & dt.Rows(i)("Tbl1") & ",") > 0 OrElse tbs.IndexOf("," & dt.Rows(i)("Tbl2") & ",") > 0) Then
                    dt.Rows(i)("RecOrder") = 200 * newset
                    dt.Rows(i)("Param1") = newset
                    j = j + 1
                End If
            Next
            'at this point j is number of rows in the all previous and the current set
            SaveJoins(rep, dt)
            If j = dt.Rows.Count Then 'all joins are checked
                dt = GetReportJoins(rep)  'from OUR db real joins at that moment ORDER BY RecOrder,Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2
                Return dt
            ElseIf j < dt.Rows.Count Then
                dt = GetSQLJoins(rep, j, newset)  'recursive
            End If
        End If
        'SaveJoins(rep, dt)
        'sqls = "SELECT * FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND DOING = 'JOIN' ORDER BY RecOrder,Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2"
        'dt = mRecords(sqls).Table  'from OUR db
        'dt = CorrectJoinOrder(dt)
        Return dt

        'For i = 0 To dt.Rows.Count - 1
        '    'the first record leave as it is. User can move rows on top and reverse it to make one table the very First one for the LEFT JOIN (!)
        '    'TODO - then starting from second row move up all rows having the table(s) from the first row, then from previous rows, etc.... 
        '    'This way all connected tables will be concentrated on top as first set. After that the Oper=1 assigned and new set starts.
        '    If i > 0 Then
        '        'first table on current row is the same as first table on previous row
        '        If dt.Rows(i - 1)("Tbl1") = dt.Rows(i)("Tbl1") Then
        '            Continue For
        '        End If
        '        'TODO - compare not only to previous row but throw hole table  !!!
        '        'at this point the first table on current and previous rows are different
        '        'second table on current row is the same as first table on previous row
        '        If dt.Rows(i - 1)("Tbl1") = dt.Rows(i)("Tbl2") Then
        '            tb = dt.Rows(i)("Tbl1")
        '            fl = dt.Rows(i)("Tbl1Fld1")
        '            dt.Rows(i)("Tbl1") = dt.Rows(i)("Tbl2")
        '            dt.Rows(i)("Tbl1Fld1") = dt.Rows(i)("Tbl2Fld2")
        '            dt.Rows(i)("Tbl2") = tb
        '            dt.Rows(i)("Tbl2Fld2") = fl
        '            dt.Rows(i)("Oper") = ""
        '            'first table on current row is the same as second table on previous row
        '        ElseIf dt.Rows(i - 1)("Tbl2") = dt.Rows(i)("Tbl1") Then
        '            tb = dt.Rows(i)("Tbl1")
        '            fl = dt.Rows(i)("Tbl1Fld1")
        '            dt.Rows(i)("Tbl1") = dt.Rows(i)("Tbl2")
        '            dt.Rows(i)("Tbl1Fld1") = dt.Rows(i)("Tbl2Fld2")
        '            dt.Rows(i)("Tbl2") = tb
        '            dt.Rows(i)("Tbl2Fld2") = fl
        '            dt.Rows(i)("Oper") = ""
        '        Else
        '            'TODO - compare not only to previous row but throw hole table  !!!
        '            'tables on current row are both different compare to previous row tables: add 1 in Oper field
        '            dt.Rows(i)("Oper") = "1"  'indicate that comma, not Join is needed
        '        End If
        '    End If
        'Next
        'Return dt
    End Function
    Public Function CorrectJoinOrder(ByVal dt As DataTable) As DataTable
        'reorder
        For i = 0 To dt.Rows.Count - 1
            dt.Rows(i)("RecOrder") = i + 1
        Next
        Return dt
    End Function
    Public Function ListAllSQLJoins(ByVal repid As String, ByVal repjoins As String, ByVal repdb As String, Optional ByVal tblj1 As String = "", Optional ByVal tblj2 As String = "", Optional ByVal userconnprv As String = "") As DataTable
        'for report
        Dim i As Integer
        Dim tbs As String = ","
        Dim tb1 As String = String.Empty
        Dim tb2 As String = String.Empty
        'Dim dtj As DataTable = GetAllSQLJoins() 'sorted by Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2
        'If dtj Is Nothing OrElse dtj.Rows.Count = 0 Then
        '    Return Nothing
        'End If
        Dim dt As DataTable = GetDistinctSQLJoins(repdb, repjoins)
        'TODO clean dt from doubles, if join reversed
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            Return Nothing
        End If

        Dim dtr As DataTable = GetReportTablesFromSQLqueryText(repid)
        If dtr Is Nothing OrElse dtr.Rows.Count = 0 Then
            Return Nothing
        End If
        For i = 0 To dtr.Rows.Count - 1
            tbs = tbs & FixReservedWords(CorrectTableNameWithDots(dtr.Rows(i)("Tbl1").ToString, userconnprv), userconnprv).ToUpper & ","
        Next
        tbs = tbs & FixReservedWords(CorrectTableNameWithDots(tblj1, userconnprv), userconnprv).ToUpper & "," & FixReservedWords(CorrectTableNameWithDots(tblj2, userconnprv), userconnprv).ToUpper & ","
        'at this point we have tbs in upper case
        Dim dtc As DataTable = dt
        dtc.Columns.Add("DEL")
        For i = 0 To dt.Rows.Count - 1
            tb1 = "," & FixReservedWords(CorrectTableNameWithDots(dt.Rows(i)("Tbl1").ToString, userconnprv).ToUpper, userconnprv) & ","   '.Replace("[", "").Replace("]", "")
            tb2 = "," & FixReservedWords(CorrectTableNameWithDots(dt.Rows(i)("Tbl2").ToString, userconnprv).ToUpper, userconnprv) & ","   '.Replace("[", "").Replace("]", "")
            If (tbs.IndexOf(tb1) >= 0 Or tbs.IndexOf(tb2) >= 0) Then
                dtc.Rows(i)("DEL") = 0
            Else
                dtc.Rows(i)("DEL") = 1
            End If
            If dtc.Rows(i)("Param3").ToString = "deleted" Then
                dtc.Rows(i)("DEL") = 1
            End If
        Next
        Return dtc
    End Function
    Public Function GetSQLSorts(ByVal rep As String) As DataTable
        Dim dt As DataTable = Nothing
        Dim sqls = "SELECT * FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND DOING = 'ORDER BY' ORDER BY Oper"
        dt = mRecords(sqls).Table  'from OUR db
        Return dt
    End Function
    Public Function GetAllSQLJoins() As DataTable
        Dim dt As DataTable = Nothing
        Dim sqls = "SELECT * FROM OURReportSQLquery WHERE DOING = 'JOIN' ORDER BY Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2"
        dt = mRecords(sqls).Table  'from OUR db
        Return dt
    End Function
    Public Function GetDistinctSQLJoins(ByVal repdb As String, ByVal rep As String) As DataTable
        Dim dt As DataTable = Nothing
        Dim sqls As String = String.Empty
        ' get all distinct Joins for User DB
        sqls = sqls & "SELECT DISTINCT Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2,Param1,OURReportSQLquery.Comments,OURReportSQLquery.Param2,OURReportSQLquery.Param3 FROM OURReportSQLquery WHERE OURReportSQLquery.ReportId='" & rep & "' AND OURReportSQLquery.DOING = 'JOIN' AND (OURReportSQLquery.Comments LIKE '%" & repdb.Trim.Replace(" ", "%") & "%' OR OURReportSQLquery.Param1 LIKE '%" & repdb.Trim.Replace(" ", "%") & "%') ORDER BY Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2"
        dt = mRecords(sqls).Table   'from OUR db all predefined joins for the same userdb
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            sqls = "SELECT DISTINCT Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2,OURReportSQLquery.Param1,'custom' As Param2,OURReportSQLquery.Param3 FROM OURReportSQLquery INNER JOIN OURReportInfo ON (OURReportInfo.ReportID=OURReportSQLquery.ReportID) WHERE OURReportSQLquery.DOING = 'JOIN' AND OURReportInfo.ReportDB LIKE '%" & repdb.Trim.Replace(" ", "%") & "%' ORDER BY Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2"
            dt = mRecords(sqls).Table   'from OUR db all predefined custom manually created joins 
        End If
        Return dt
    End Function
    Public Function DeleteSQLJoin(ByVal rep As String, ByVal indx As String) As String
        Dim ret As String = String.Empty
        Try
            Dim sqls = "DELETE FROM OURReportSQLquery WHERE DOING='JOIN' AND ReportId = '" & rep & "' AND Indx = " & indx
            ExequteSQLquery(sqls)  'from OUR db
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function DeleteSQLSort(ByVal rep As String, ByVal indx As String) As String
        Dim ret As String = String.Empty
        Try
            Dim sqls = "DELETE FROM OURReportSQLquery WHERE DOING='ORDER BY' AND ReportId = '" & rep & "' AND Indx = " & indx
            ExequteSQLquery(sqls)  'from OUR db
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function DeleteSQLWhere(ByVal rep As String, ByVal indx As String) As String
        Dim ret As String = String.Empty
        Try
            Dim dt As DataTable = GetSQLWhereCondition(rep, indx)
            Dim Group As String = String.Empty
            If dt.Rows IsNot Nothing AndAlso dt.Rows.Count > 0 Then _
                Group = dt.Rows(0)("Group").ToString

            Dim sqls As String = "DELETE FROM OURReportSQLquery WHERE DOING='WHERE' AND ReportId = '" & rep & "' AND Indx = " & indx
            ExequteSQLquery(sqls)
            If Group <> String.Empty Then
                dt = GetConditionsInGroup(rep, Group)
                'if there are 0 or 1 conditions left in the group then delete it
                If dt IsNot Nothing AndAlso dt.Rows.Count < 2 Then _
                    DeleteConditionGroup(rep, Group)
            End If

            dt = GetSQLConditions(rep)
            ' if only one record is left set Logical to 'And'
            If dt IsNot Nothing AndAlso dt.Rows.Count = 1 Then
                sqls = "UPDATE OURReportSQLquery "
                sqls &= "SET Logical = 'And' "
                sqls &= "WHERE Indx = " & dt.Rows(0)("Indx").ToString
                ExequteSQLquery(sqls)
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function GetSQLWhereCondition(ByVal rep As String, ByVal indx As String) As DataTable
        Dim dt As DataTable = Nothing
        Dim sqls = "SELECT * FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND indx = " & indx & " ORDER BY Tbl1,Tbl1Fld1"
        dt = mRecords(sqls).Table  'from OUR db
        Return dt
    End Function
    Public Function GetSQLConditions(ByVal rep As String) As DataTable
        Dim dt As DataTable = Nothing
        'Dim sqls = "SELECT * FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND DOING = 'WHERE' ORDER BY Tbl1,Tbl1Fld1"
        Dim sqls = "SELECT * FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND DOING = 'WHERE' ORDER BY RecOrder"
        dt = mRecords(sqls).Table  'from OUR db
        Return dt
    End Function
    Public Function DeleteConditionGroup(rep As String, GroupName As String) As String
        Dim ret As String = String.Empty
        Try
            Dim LogicalOp As String = GetConditionGroupLogical(rep, GroupName)
            If LogicalOp = String.Empty Then LogicalOp = "And"
            Dim sql As String = String.Empty
            Dim dt As DataTable = GetConditionsInGroup(rep, GroupName)

            'Change group conditions, if any
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For i As Integer = 0 To dt.Rows.Count - 1
                    sql = "UPDATE OURReportSQLquery "
                    sql &= "SET " & FixReservedWords("Group") & " = NULL,"
                    sql &= "Logical = '" & LogicalOp & "'"
                    sql &= " WHERE Indx = " & dt.Rows(i)("Indx").ToString()
                    ExequteSQLquery(sql)
                Next
            End If
            sql = "DELETE FROM OURReportSQLquery WHERE DOING='GROUP' AND ReportId = '" & rep & "' AND " & FixReservedWords("Group") & " = '" & GroupName & "'"
            ExequteSQLquery(sql)
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function GetConditionGroupCount(rep As String) As Integer
        Dim n As Integer = 0
        Dim dt As DataTable = GetSQLConditionGroups(rep)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then n = dt.Rows.Count
        Return n
    End Function
    Public Function GetUngroupedConditions(rep As String) As DataTable
        Dim dt As DataTable = Nothing
        Dim sqls = "SELECT * FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND DOING = 'WHERE' AND " & FixReservedWords("Group") & " IS NULL ORDER BY RecOrder"
        dt = mRecords(sqls).Table  'from OUR db
        Return dt
    End Function
    Public Function GetConditionGroupLogical(rep As String, GroupName As String) As String
        Dim sql = "SELECT Logical FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND DOING = 'GROUP' AND " & FixReservedWords("Group") & " = '" & GroupName & "'"
        Dim dt As DataTable = mRecords(sql).Table
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Return dt.Rows(0)("Logical").ToString
        Else
            Return String.Empty
        End If
    End Function
    Public Function GetConditionsInGroup(rep As String, GroupName As String) As DataTable
        Dim sql As String = "SELECT * FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND DOING = 'WHERE' AND " & FixReservedWords("Group") & " = '" & GroupName & "' ORDER BY RecOrder"
        Dim dt As DataTable = mRecords(sql).Table
        Return dt
    End Function
    Public Function GetSQLConditionGroups(ByVal rep As String) As DataTable
        Dim dt As DataTable = Nothing
        Dim sqls = "SELECT * FROM OURReportSQLquery WHERE ReportId = '" & rep & "' AND DOING = 'GROUP' ORDER BY RecOrder"
        dt = mRecords(sqls).Table  'from OUR db
        Return dt
    End Function
    Public Function GetConditionString(dr As DataRow, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "") As String
        Dim typ As String = dr("Type").ToString
        Dim tbl As String = dr("Tbl1").ToString
        Dim tabl2 As String = dr("Tbl2").ToString
        Dim tabl3 As String = dr("Tbl3").ToString
        Dim fld As String = dr("Tbl1Fld1").ToString
        Dim fld2 As String = dr("Tbl2Fld2").ToString
        Dim fld3 As String = dr("Tbl3Fld3").ToString
        Dim oper As String = dr("Oper").ToString
        Dim val As String = dr("Comments").ToString.Trim
        Dim field2 As String = tabl2 & "." & fld2
        Dim field3 As String = tabl3 & "." & fld3
        Dim sCondition As String = tbl & "." & fld & " " & oper & " "
        Dim qt As String = """"
        Dim dblqt = qt & qt

        If typ = "Field" Then
            sCondition &= field2
        ElseIf typ = "RelDate" Then
            sCondition &= val
        ElseIf typ = "Static" Then
            If TblFieldIsNumeric(tbl, fld, userconstr, userconprv) Then
                If val = dblqt Then val = "NULL"
                If oper.ToUpper = "IN" OrElse oper.ToUpper = "NOT IN" Then
                    sCondition &= "(" & val & ")"
                Else
                    sCondition &= val
                End If
            Else
                If oper.ToUpper = "IN" OrElse oper.ToUpper = "NOT IN" Then
                    sCondition &= "('" & val.Replace(",", "','") & "')"
                Else
                    If val = dblqt Then val = ""
                    sCondition &= "'" & val & "'"
                End If
            End If
        ElseIf typ = "DateTime" Then
            sCondition &= "'" & val & "'"
        ElseIf typ = "BtwFields" OrElse typ = "BtwFD1FD2" Then
            sCondition &= field2 & " AND " & field3
        ElseIf typ = "BtwDates" OrElse typ = "BtwDT1DT2" Then
            sCondition &= "'" & fld2 & "' AND '" & fld3 & "'"
        ElseIf typ = "BwRD1Date2" OrElse typ = "BtwRD1DT2" Then
            sCondition &= fld2 & " AND '" & fld3 & "'"
        ElseIf typ = "BtwRD1RD2" Then
            sCondition &= fld2 & " AND " & fld3
        ElseIf typ = "BtwFldDate" OrElse typ = "BtwFD1DT2" Then
            sCondition &= field2 & " AND '" & fld3 & "'"
        ElseIf typ = "BtwFldRD2" OrElse typ = "BtwFD1RD2" Then
            sCondition &= field2 & " AND " & fld3
        ElseIf typ = "BtwDateFld" OrElse typ = "BtwDT1FD2" Then
            sCondition &= "'" & fld2 & "' AND " & field3
        ElseIf typ = "BtwRD1Fld" OrElse typ = "BtwRD1FD2" Then
            sCondition &= fld2 & " AND " & field3
        ElseIf typ = "BtwValues" OrElse typ = "BtwST1ST2" Then
            If TblFieldIsNumeric(tbl, fld, userconstr, userconprv) Then
                sCondition &= fld2 & " AND " & fld3
            Else
                sCondition &= "'" & fld2 & "' AND '" & fld3 & "'"
            End If
        ElseIf typ = "BtwValFld" OrElse typ = "BtwST1FD2" Then
            If TblFieldIsNumeric(tbl, fld, userconstr, userconprv) Then
                sCondition &= fld2 & " AND " & field3
            Else
                sCondition &= "'" & fld2 & "' AND " & field3
            End If
        ElseIf "BtwFldVal" OrElse typ = "BtwFD1ST2" Then
            If TblFieldIsNumeric(tbl, fld, userconstr, userconprv) Then
                sCondition &= field2 & " AND " & fld3
            Else
                sCondition &= field2 & " AND '" & fld3 & "'"
            End If
        End If
        Return sCondition
    End Function
    Public Function GetConditionTree(rpt As String, dt As DataTable, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "") As TreeView
        Dim tv As New TreeView
        Dim n As Integer = 0

        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
            Dim condition As String
            Dim dr As DataRow
            Dim htConditionGroups As New Hashtable
            Dim htGroupChildCount As New Hashtable
            Dim nGroups As Integer = 0
            Dim dtGroups As DataTable = GetSQLConditionGroups(rpt)
            Dim tn As TreeNode
            Dim tnGroup As TreeNode
            Dim CondData As ReportCondition

            'create group nodes
            If dtGroups IsNot Nothing AndAlso dtGroups.Rows.Count > 0 Then
                nGroups = dtGroups.Rows.Count
                For i As Integer = 0 To dtGroups.Rows.Count - 1
                    dr = dtGroups.Rows(i)
                    CondData = New ReportCondition
                    CondData.LogicalOperator = dr("Logical").ToString
                    CondData.GroupName = dr("Group").ToString
                    CondData.ConditionId = dr("Indx").ToString
                    tnGroup = New TreeNode
                    tnGroup.Text = CondData.GroupName
                    tnGroup.TextColor = "Purple"
                    tnGroup.Value = CondData.GroupName
                    tnGroup.ConditionData = CondData
                    htConditionGroups(CondData.GroupName) = tnGroup
                    htGroupChildCount(CondData.GroupName) = 0
                Next
            End If
            ' create condition nodes 
            For i As Integer = 0 To dt.Rows.Count - 1
                dr = dt.Rows(i)
                condition = GetConditionString(dr, userconstr, userconprv)

                CondData = New ReportCondition
                CondData.Condition = condition.Replace("'", "&apos;")
                CondData.LogicalOperator = dr("Logical").ToString
                If CondData.LogicalOperator = String.Empty Then CondData.LogicalOperator = "And"
                If Not IsDBNull(dr("Group")) Then CondData.ContainedBy = dr("Group").ToString
                CondData.ConditionId = dr("Indx").ToString
                Dim grp = CondData.ContainedBy
                If grp = String.Empty Then
                    If n > 0 Then
                        tn = New TreeNode
                        tn.Text = CondData.LogicalOperator
                        tn.Value = tn.Text
                        tn.TextColor = "ForestGreen"
                        ReDim Preserve tv.Nodes(n)
                        tv.Nodes(n) = tn
                        n = n + 1
                    End If
                    tn = New TreeNode
                    tn.Text = CondData.Condition
                    tn.Value = tn.Text
                    tn.ConditionData = CondData
                    ReDim Preserve tv.Nodes(n)
                    tv.Nodes(n) = tn
                    n = n + 1
                Else
                    Dim nNodes As Integer = CType(htGroupChildCount(grp), Integer)
                    tnGroup = CType(htConditionGroups(grp), TreeNode)
                    If nNodes = 0 Then
                        If n > 0 Then
                            tn = New TreeNode
                            tn.Text = tnGroup.ConditionData.LogicalOperator
                            tn.Value = tn.Text
                            tn.TextColor = "ForestGreen"
                            ReDim Preserve tv.Nodes(n)
                            tv.Nodes(n) = tn
                            n = n + 1
                        End If
                        ReDim Preserve tv.Nodes(n)
                        tv.Nodes(n) = tnGroup
                        n = n + 1
                    Else
                        tn = New TreeNode
                        tn.Text = CondData.LogicalOperator
                        tn.Value = tn.Text
                        tn.TextColor = "ForestGreen"
                        ReDim Preserve tnGroup.ChildNodes(nNodes)
                        tnGroup.ChildNodes(nNodes) = tn
                        nNodes = nNodes + 1
                    End If
                    tn = New TreeNode
                    tn.Text = CondData.Condition
                    tn.Value = tn.Text
                    tn.ConditionData = CondData
                    ReDim Preserve tnGroup.ChildNodes(nNodes)
                    tnGroup.ChildNodes(nNodes) = tn
                    nNodes = nNodes + 1
                    htGroupChildCount(grp) = nNodes
                End If
            Next
        End If
        Return tv
    End Function
    Public Function GetReportInfo(ByVal rep As String) As DataTable
        Dim dri As DataTable = Nothing
        Dim sqls = "SELECT * FROM OURReportInfo WHERE (ReportID='" & rep & "')"
        dri = mRecords(sqls).Table  'from OUR db
        Return dri
    End Function
    Public Function SaveSQLquery(ByVal rep As String, ByVal sqlquery As String) As String
        sqlquery = cleanSQL(sqlquery)
        Dim ret As String = ""
        Dim SQLq As String = String.Empty
        Try
            If sqlquery.ToUpper.IndexOf(" DISTINCT ") > 0 Then
                SQLq = "UPDATE OURReportInfo SET Param6type='True' WHERE (ReportID='" & rep & "')"
                ret = ExequteSQLquery(SQLq)
            End If
            sqlquery = sqlquery.Replace("'", """")
            SQLq = "UPDATE OURReportInfo SET SQLquerytext='" & sqlquery & "' WHERE (ReportID='" & rep & "')"
            ret = ExequteSQLquery(SQLq)
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function

    Private Function MakeStringFromDS(ByVal mRecordSet As System.Data.DataSet) As String
        Try
            Dim RecordSetStr As String = String.Empty
            Dim m As Integer = 0
            Dim n As Integer = 0
            Dim i, j As Integer
            If Not mRecordSet Is Nothing Then
                m = mRecordSet.Tables(0).Rows.Count
                n = mRecordSet.Tables(0).Columns.Count
                For i = 0 To m - 1
                    For j = 0 To n - 1
                        RecordSetStr = RecordSetStr & mRecordSet.Tables(0).Rows(i).Item(j).ToString & Chr(13) & Chr(10)
                    Next
                Next
            End If
            Return RecordSetStr
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Function CreateInitialClass(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim rt As String = String.Empty
        Try
            Dim sqlq As String = String.Empty
            sqlq = "DROP TABLE OUR.INIT"
            rt = ExequteSQLquery(sqlq, myconstr, myconprv)
            sqlq = "CREATE TABLE OUR.INIT (STAT VARCHAR(30), DJ VARCHAR(30), NODE VARCHAR(1000), VALUE VARCHAR(32500))"
            rt = ExequteSQLquery(sqlq, myconstr, myconprv)
            sqlq = "CREATE PROCEDURE IMPORTCLASSFROMXMLFILE(IN ClassPath VARCHAR(100)) FOR OUR.INIT LANGUAGE OBJECTSCRIPT { D $system.OBJ.Load(ClassPath,""c-lfr-d"")  q 1}"
            rt = ExequteSQLquery(sqlq, myconstr, myconprv)
            sqlq = "CREATE PROCEDURE BUILDCLASSFROMSTRING(IN ClassText VARCHAR(32000)) FOR OUR.INIT LANGUAGE OBJECTSCRIPT { set stream=##class(%GlobalBinaryStream).%New()  d stream.Write(ClassText)	D $system.OBJ.LoadStream(stream,""c-lfr-d"") q 1 }"
            rt = ExequteSQLquery(sqlq, myconstr, myconprv)

            sqlq = "CREATE PROCEDURE BUILDROUTINE(IN RoutineName VARCHAR(100), RoutineText VARCHAR(32000)) FOR OUR.INIT LANGUAGE OBJECTSCRIPT { "
            sqlq = sqlq & " S routine=##class(%Routine).%New(RoutineName) "
            'sqlq = sqlq & " ;create routine RoutineName from RoutineText string "
            sqlq = sqlq & " S rtext=RoutineText	"
            sqlq = sqlq & " S n=$l(rtext,$c(13,10)) "
            sqlq = sqlq & " S i=1 "
            sqlq = sqlq & " while i<=n { "
            sqlq = sqlq & "  s textline=$P(rtext,$c(13,10),i) "
            sqlq = sqlq & "  D routine.WriteLine(textline) "
            sqlq = sqlq & "  S i=i+1 "
            sqlq = sqlq & " } "
            sqlq = sqlq & " D routine.WriteLine($c(13,10)) "
            sqlq = sqlq & " D routine.Save() "
            ' Silent compile ; -dk : -Don't display and Keep source code"
            sqlq = sqlq & " S sc=routine.Compile("" - dk"") "
            sqlq = sqlq & " IF 'sc K err D $system.Status.DecomposeStatus(sc,.err) "
            sqlq = sqlq & " D routine.%Close() q 1 }"
            rt = ExequteSQLquery(sqlq, myconstr, myconprv)

            sqlq = "CREATE PROCEDURE GETGLOBALNODEDATA(IN glb VARCHAR(1000)) FOR OUR.INIT RESULT SETS LANGUAGE OBJECTSCRIPT { "
            sqlq = sqlq & " s value=$G(@glb)  s tag=+$J "
            sqlq = sqlq & " s rst = ##class(%ResultSet).%New() "
            sqlq = sqlq & " SET sc=rst.Prepare(""DELETE FROM OUR.INIT WHERE DJ='""_tag_""'"") "
            sqlq = sqlq & " SET sc=rst.Execute() "
            sqlq = sqlq & " s clm = ##class(OUR.INIT).%New()	"
            sqlq = sqlq & " SET clm.STAT=tag "
            sqlq = sqlq & " SET clm.NODE=glb "
            sqlq = sqlq & " SET clm.VALUE=value "
            sqlq = sqlq & " SET clm.DJ=tag "
            sqlq = sqlq & " s sc=clm.%Save() "
            sqlq = sqlq & " s sqls=""Select * FROM OUR.INIT WHERE DJ='""_tag_""'"" "
            sqlq = sqlq & " s rset = ##class(%ResultSet).%New() "
            sqlq = sqlq & " SET sc=rset.Prepare(sqls) "
            sqlq = sqlq & " SET sc=rset.Execute() "
            sqlq = sqlq & " If '$isobject($Get(%sqlcontext)) {set %sqlcontext = ##class(%Library.ProcedureContext).%New()} "
            sqlq = sqlq & " do %sqlcontext.AddResultSet(rset) }"
            'sqlq = sqlq & " q 1 }"
            ExequteSQLquery(sqlq, myconstr, myconprv)
            If rt = "Query executed fine." Then
                rt = "Class OUR.INIT created"
            Else
                Return rt
            End If

            'create Fred's class
            'inc
            'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            Dim routinetext As String = GetTextFromFile(applpath & "include\", "GenASys_SQL_inc", ".xml")
            rt = CreateRoutine("OUR.GenASys.SQL.inc", routinetext, myconstr, myconprv)
            Dim classtext As String = GetTextFromFile(applpath & "include\", "OUR.Utils.StoredProcs", ".CLS")
            rt = CreateClass(classtext, myconstr, myconprv)

            'create routine to create new db and namespace
            Dim routinetext1 As String = GetTextFromFile(applpath & "include\", "CreateDbAndNamespace", ".txt")
            rt = CreateRoutine("OUR.CreateDbAndNamespace.mac", routinetext1, myconstr, myconprv)

        Catch ex As Exception
            Return ex.Message
        End Try
        Return rt
    End Function
    Private Function CreateClass(ByVal ClassText As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        'Only for Cache !!
        Dim ret As String = String.Empty
        Try
            Dim myprovider As String
            If myconstr = "" Then
                myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            Else
                myprovider = myconprv
            End If
            If myprovider = "InterSystems.Data.CacheClient" AndAlso ClassText <> String.Empty Then
                Dim StoredProcName As String = String.Empty
                Dim ParamName(0) As String
                Dim ParamType(0) As String
                Dim ParamValue(0) As String
                'make params
                ReDim ParamName(0)
                ReDim ParamType(0)
                ReDim ParamValue(0)
                ParamName(0) = "ClassText"
                ParamValue(0) = ClassText
                StoredProcName = "OUR.BUILDCLASSFROMSTRING"  'StorProc in INIT class has [ SqlName = BUILDCLASSFROMSTRING, SqlProc ] 
                ret = RunSP(StoredProcName, 1, ParamName, ParamType, ParamValue)
                Return ret
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
        Return ret
    End Function

    Public Function UnInstallOURMySQL(db As String) As String
        Dim myconstring As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("MySQLconnection").ToString
        Dim ret As String = String.Empty
        Dim retr As String = String.Empty
        Dim sqlq As String = "SELECT * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME='" & db & "'"
        Dim myView As DataView = mRecords(sqlq) 'from OUR db
        If myView IsNot Nothing AndAlso myView.Table.Rows.Count = 1 Then
            'delete ouracesslog
            sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" & db & "' And TABLE_NAME='ouraccesslog'"
            myView = mRecords(sqlq)  'from OUR db
            If myView IsNot Nothing AndAlso myView.Table.Rows.Count = 1 Then
                sqlq = "DROP TABLE `" & db & "`.`ouraccesslog`;"
                ret = ExequteSQLquery(sqlq)
                If ret = "Query executed fine." Then
                    retr = retr & "<br/> " & " Table ouraccesslog deleted"
                Else
                    retr = retr & "<br/> " & " Table ouraccesslog NOT deleted: " & ret
                End If
            End If
            'delete ourpermissions
            sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" & db & "' And TABLE_NAME='ourpermissions'"
            myView = mRecords(sqlq)
            If myView IsNot Nothing AndAlso myView.Table.Rows.Count = 1 Then
                sqlq = "DROP TABLE `" & db & "`.`ourpermissions`;"
                ret = ExequteSQLquery(sqlq)
                If ret = "Query executed fine." Then
                    retr = retr & "<br/> " & " Table ourpermissions deleted"
                Else
                    retr = retr & "<br/> " & " Table ourpermissions NOT deleted: " & ret
                End If
            End If
            'delete ourpermits
            sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" & db & "' And TABLE_NAME='ourpermits'"
            myView = mRecords(sqlq)
            If myView IsNot Nothing AndAlso myView.Table.Rows.Count = 1 Then
                sqlq = "DROP TABLE `" & db & "`.`ourpermits`;"
                ret = ExequteSQLquery(sqlq)
                If ret = "Query executed fine." Then
                    retr = retr & "<br/> " & " Table ourpermits deleted"
                Else
                    retr = retr & "<br/> " & " Table ourpermits NOT deleted: " & ret
                End If
            End If
            'delete ourreportchildren
            sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" & db & "' And TABLE_NAME='ourreportchildren'"
            myView = mRecords(sqlq)
            If myView IsNot Nothing AndAlso myView.Table.Rows.Count = 1 Then
                sqlq = "DROP TABLE `" & db & "`.`ourreportchildren`;"
                ret = ExequteSQLquery(sqlq)
                If ret = "Query executed fine." Then
                    retr = retr & "<br/> " & " Table ourreportchildren deleted"
                Else
                    retr = retr & "<br/> " & " Table ourreportchildren NOT deleted: " & ret
                End If
            End If
            'delete ourreportformat
            sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" & db & "' And TABLE_NAME='ourreportformat'"
            myView = mRecords(sqlq)
            If myView IsNot Nothing AndAlso myView.Table.Rows.Count = 1 Then
                sqlq = "DROP TABLE `" & db & "`.`ourreportformat`;"
                ret = ExequteSQLquery(sqlq)
                If ret = "Query executed fine." Then
                    retr = retr & "<br/> " & " Table ourreportformat deleted"
                Else
                    retr = retr & "<br/> " & " Table ourreportformat NOT deleted: " & ret
                End If
            End If
            'delete ourreportgroups
            sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" & db & "' And TABLE_NAME='ourreportgroups'"
            myView = mRecords(sqlq)
            If myView IsNot Nothing AndAlso myView.Table.Rows.Count = 1 Then
                sqlq = "DROP TABLE `" & db & "`.`ourreportgroups`;"
                ret = ExequteSQLquery(sqlq)
                If ret = "Query executed fine." Then
                    retr = retr & "<br/> " & " Table ourreportgroups deleted"
                Else
                    retr = retr & "<br/> " & " Table ourreportgroups NOT deleted: " & ret
                End If
            End If
            'delete ourreportinfo
            sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" & db & "' And TABLE_NAME='ourreportinfo'"
            myView = mRecords(sqlq)
            If myView IsNot Nothing AndAlso myView.Table.Rows.Count = 1 Then
                sqlq = "DROP TABLE `" & db & "`.`ourreportinfo`;"
                ret = ExequteSQLquery(sqlq)
                If ret = "Query executed fine." Then
                    retr = retr & "<br/> " & " Table ourreportinfo deleted"
                Else
                    retr = retr & "<br/> " & " Table ourreportinfo NOT deleted: " & ret
                End If
            End If
            'delete ourreportlists
            sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" & db & "' And TABLE_NAME='ourreportlists'"
            myView = mRecords(sqlq)
            If myView IsNot Nothing AndAlso myView.Table.Rows.Count = 1 Then
                sqlq = "DROP TABLE `" & db & "`.`ourreportlists`;"
                ret = ExequteSQLquery(sqlq)
                If ret = "Query executed fine." Then
                    retr = retr & "<br/> " & " Table ourreportlists deleted"
                Else
                    retr = retr & "<br/> " & " Table ourreportlists NOT deleted: " & ret
                End If
            End If
            'delete ourreportshow
            sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" & db & "' And TABLE_NAME='ourreportshow'"
            myView = mRecords(sqlq)
            If myView IsNot Nothing AndAlso myView.Table.Rows.Count = 1 Then
                sqlq = "DROP TABLE `" & db & "`.`ourreportshow`;"
                ret = ExequteSQLquery(sqlq)
                If ret = "Query executed fine." Then
                    retr = retr & "<br/> " & " Table ourreportshow deleted"
                Else
                    retr = retr & "<br/> " & " Table ourreportshow NOT deleted: " & ret
                End If
            End If
            'delete ourreportsqlquery
            sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" & db & "' And TABLE_NAME='ourreportsqlquery'"
            myView = mRecords(sqlq)
            If myView IsNot Nothing AndAlso myView.Table.Rows.Count = 1 Then
                sqlq = "DROP TABLE `" & db & "`.`ourreportsqlquery`;"
                ret = ExequteSQLquery(sqlq)
                If ret = "Query executed fine." Then
                    retr = retr & "<br/> " & " Table ourreportsqlquery deleted"
                Else
                    retr = retr & "<br/> " & " Table ourreportsqlquery NOT deleted: " & ret
                End If
            End If
            'delete ourreportsqlquery
            sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" & db & "' And TABLE_NAME='ourhelpdesk'"
            myView = mRecords(sqlq)
            If myView IsNot Nothing AndAlso myView.Table.Rows.Count = 1 Then
                sqlq = "DROP TABLE `" & db & "`.`ourhelpdesk`;"
                ret = ExequteSQLquery(sqlq)
                If ret = "Query executed fine." Then
                    retr = retr & "<br/> " & " Table ourhelpdesk deleted"
                Else
                    retr = retr & "<br/> " & " Table ourhelpdesk NOT deleted: " & ret
                End If
            End If
        End If
        Return retr
    End Function

    Public Function ColumnExists(tbl As String, column As String, Optional myconstr As String = "", Optional ByVal myconprv As String = "", Optional ByRef er As String = "") As Boolean
        Dim ret As Boolean = False
        Dim sqlq As String = String.Empty
        Dim err As String = String.Empty

        Dim i As Integer
        If TableExists(tbl, myconstr, myconprv, err) Then
            Try
                If myconstr = "" Then
                    myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                    myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
                End If
                If myconprv = "MySql.Data.MySqlClient" Then
                    Dim db As String = GetDataBase(myconstr, myconprv)
                    sqlq = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='" & db.ToLower & "' And TABLE_NAME='" & tbl.ToLower & "' And COLUMN_NAME='" & column & "'"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    If Not column.Contains("""") Then
                        column = column.ToUpper
                    Else
                        column = column.Replace("""", "")
                    End If
                    'sqlq = "SELECT * FROM all_tab_cols WHERE UPPER(TABLE_NAME) = UPPER('" & tbl & "') AND COLUMN_NAME='" & column & "'"
                    sqlq = "SELECT * FROM all_tab_cols WHERE UPPER(TABLE_NAME) = UPPER('" & tbl & "') AND UPPER(COLUMN_NAME)='" & column.ToUpper & "'"
                ElseIf myconprv = "System.Data.Odbc" Then
                    Dim dt As DataTable = GetListOfTableFields(tbl, myconstr, myconprv).Table
                    For i = 0 To dt.Rows.Count - 1
                        If dt.Rows(i)("COLUMN_NAME").ToString.ToUpper = column.ToUpper Then
                            ret = True
                        End If
                    Next
                    Return ret
                ElseIf myconprv = "System.Data.OleDb" Then
                    Dim dt As DataTable = GetListOfTableFields(tbl, myconstr, myconprv).Table
                    For i = 0 To dt.Rows.Count - 1
                        If dt.Rows(i)("COLUMN_NAME").ToString.ToUpper = column.ToUpper Then
                            ret = True
                        End If
                    Next
                    Return ret
                ElseIf myconprv.StartsWith("InterSystems.Data.") Then
                    sqlq = "Select Name As COLUMN_NAME FROM %Dictionary.PropertyDefinition WHERE parent = '" & tbl.ToUpper & "' AND Cardinality Is NULL AND Name = '" & column & "'"

                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsq
                    Dim db As String = GetDataBase(myconstr, myconprv)
                    sqlq = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA ='public' AND LOWER(TABLE_CATALOG) ='" & db.ToLower & "' And TABLE_NAME='" & tbl.ToLower & "' And COLUMN_NAME='" & column & "'"

                ElseIf myconprv = "Sqlite" Then
                    Dim dv As DataView = GetListOfTableFields(tbl, myconstr, myconprv)
                    dv.RowFilter = "COLUMN_NAME='" & column & "'"
                    dv = dv.ToTable.DefaultView
                    If dv IsNot Nothing AndAlso dv.Count > 0 AndAlso dv.Table.Rows.Count = 1 Then
                        ret = True
                    End If
                    Return ret
                Else 'sql server
                    sqlq = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='" & tbl & "' AND COLUMN_NAME='" & column & "'"
                End If
                ret = HasRecords(sqlq, myconstr, myconprv)
            Catch ex As Exception
                er = ex.Message
            End Try
        End If
        If err <> String.Empty Then _
            er = err
        Return ret
    End Function
    Public Function TableExists(ByVal tbl As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "", Optional ByRef er As String = "") As Boolean
        Dim rt As Boolean = False
        Dim i As Integer
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If

            If myconprv = "MySql.Data.MySqlClient" Then
                Dim db As String = GetDataBase(myconstr, myconprv)
                sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" & db.ToLower & "' AND TABLE_NAME='" & tbl.ToLower & "'"

            ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                Dim db As String = GetDataBase(myconstr, myconprv)
                'sqlq = "SELECT * FROM all_tables WHERE TABLE_SCHEMA='" & db & "' AND TABLE_NAME='" & tbl.ToLower & "'"
                sqlq = "SELECT * FROM all_tables WHERE UPPER(TABLE_NAME) = UPPER('" & tbl & "')"

            ElseIf myconprv = "System.Data.Odbc" Then
                'ODBC
                Dim dtsh As New DataTable
                Dim dv As DataView = GetListOfUserTables(True, myconstr, myconprv)
                dv.RowFilter = "TABLE_NAME = '" & tbl & "' OR (TABLE_NAME LIKE '%." & tbl & "')"
                ' loop if dv.totable.rows.count>1   TABLE_NAME = TABLE_SCHEM & '." & tbl & "'
                If dv.ToTable.DefaultView.Table.Rows.Count = 1 Then
                    Return True
                ElseIf dv.ToTable.DefaultView.Table.Rows.Count > 1 Then
                    For i = 0 To dv.ToTable.DefaultView.Table.Rows.Count = 1
                        If dv.ToTable.DefaultView.Table.Rows("TABLE_NAME").ToString.Trim = dv.ToTable.DefaultView.Table.Rows("TABLE_SCHEM").ToString.Trim & "." & tbl Then
                            Return True
                        End If
                    Next
                    Return False
                Else
                    Return False
                End If
            ElseIf myconprv = "System.Data.OleDb" Then
                'OleDb
                Dim dtsh As New DataTable
                Dim dv As DataView = GetListOfUserTables(True, myconstr, myconprv)
                dv.RowFilter = "TABLE_NAME = '" & tbl & "' OR (TABLE_NAME LIKE '%." & tbl & "')"
                ' loop if dv.totable.rows.count>1   TABLE_NAME = TABLE_SCHEM & '." & tbl & "'
                If dv.ToTable.DefaultView.Table.Rows.Count = 1 Then
                    Return True
                ElseIf dv.ToTable.DefaultView.Table.Rows.Count > 1 Then
                    For i = 0 To dv.ToTable.DefaultView.Table.Rows.Count = 1
                        If dv.ToTable.DefaultView.Table.Rows("TABLE_NAME").ToString.Trim = dv.ToTable.DefaultView.Table.Rows("TABLE_SCHEM").ToString.Trim & "." & tbl Then
                            Return True
                        End If
                    Next
                    Return False
                Else
                    Return False
                End If
            ElseIf myconprv.StartsWith("InterSystems.Data.") Then
                If Not tbl.Contains(".") AndAlso Not tbl.ToUpper.StartsWith("OUR") Then tbl = "userdata." & tbl
                sqlq = "Select ID As TABLE_NAME FROM %Dictionary.ClassDefinition WHERE Not (ID %STARTSWITH '%') AND UCASE(ID)=UCASE('" & tbl & "')"

            ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                Dim db As String = GetDataBase(myconstr, myconprv)
                sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA ='public' AND LOWER(TABLE_CATALOG) ='" & db.ToLower & "' AND TABLE_NAME='" & tbl & "'"

            ElseIf myconprv = "Sqlite" Then  'Sqlite
                sqlq = "SELECT name FROM sqlite_master WHERE type = 'table' AND lower(name) ='" & tbl.ToLower & "'"

            Else
                sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='" & tbl & "'"
            End If
            'myView = mRecords(sqlq, er, myconstr, myconprv)
            rt = HasRecords(sqlq, myconstr, myconprv)
        Catch ex As Exception
            er = ex.Message
        End Try
        Return rt
    End Function

    Public Function JoinExist(ByVal tbl1 As String, ByVal tbl2 As String, ByVal tbl1fld1 As String, ByVal tbl2fld2 As String, ByVal repdb As String, Optional ByVal rep As String = "") As Boolean
        Dim ret As String = String.Empty
        Dim sqls As String = String.Empty
        Dim repst As String = String.Empty
        Dim er As String = String.Empty
        Dim b As Boolean = False
        Try
            If rep.Trim <> "" Then
                If rep.Contains("%") Then
                    repst = " AND ReportId LIKE '" & rep & "' "
                Else
                    repst = " AND ReportId = '" & rep & "' "
                End If
            End If
            repst = " AND ReportId "
            'check if join exist
            sqls = "SELECT * FROM OURReportSQLquery WHERE (Param1 LIKE '%" & repdb.Trim.Replace(" ", "%") & "%' AND Doing = 'JOIN' AND UCASE([Tbl1])='" & UCase(tbl1) & "' AND UCASE([Tbl2])='" & UCase(tbl2) & "' AND UCASE([Tbl1Fld1])='" & UCase(tbl1fld1) & "' AND UCASE([Tbl2Fld2])='" & UCase(tbl2fld2) & "' ) "
            If rep.Trim <> "" Then
                sqls = sqls & repst
            End If
            If HasRecords(sqls, "", "", er) Then
                b = True
                Return b
            Else
                'check if join reverted exist
                sqls = "SELECT * FROM OURReportSQLquery WHERE (Param1 LIKE '%" & repdb.Trim.Replace(" ", "%") & "%' AND Doing = 'JOIN' AND UCASE([Tbl1])='" & UCase(tbl2) & "' AND UCASE([Tbl2])='" & UCase(tbl1) & "' AND UCASE([Tbl1Fld1])='" & UCase(tbl2fld2) & "' AND UCASE([Tbl2Fld2])='" & UCase(tbl1fld1) & "' ) "
                If rep.Trim <> "" Then
                    sqls = sqls & repst
                End If
                If HasRecords(sqls, "", "", er) Then
                    b = True
                    Return b
                End If
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return b
    End Function
    Public Function AddJoin(ByVal rep As String, ByVal tbl1 As String, ByVal tbl2 As String, ByVal tbl1fld1 As String, ByVal tbl2fld2 As String, ByVal jointype As String, Optional ByVal repdb As String = "", Optional ByVal relatn As String = "", Optional ByVal commnt As String = "", Optional ByVal recodr As Integer = 100) As String
        Dim ret As String = String.Empty
        Dim sqls As String = String.Empty
        Dim insSQL As String = String.Empty
        'correct join type
        jointype = jointype.Replace("JOIN", " JOIN")
        Try
            'add requested join between tables tbl1 and tbl2
            'If IsCacheDatabase() Then
            sqls = "SELECT * FROM OURReportSQLquery WHERE (Param1 LIKE '%" & repdb.Trim.Replace(" ", "%") & "%' AND ReportId = '" & rep & "' AND DOING = 'JOIN' AND UCASE([Tbl1])='" & UCase(tbl1) & "' AND UCASE([Tbl2])='" & UCase(tbl2) & "' AND UCASE([Tbl1Fld1])='" & UCase(tbl1fld1) & "' AND UCASE([Tbl2Fld2])='" & UCase(tbl2fld2) & "' ) "
            'Else
            '    sqls = "SELECT * FROM OURReportSQLquery WHERE (Param1 LIKE '%" & repdb & "%' AND ReportId = '" & rep & "' AND DOING = 'JOIN' AND Tbl1='" & tbl1 & "' AND Tbl2='" & tbl2 & "' AND Tbl1Fld1='" & tbl1fld1 & "' AND Tbl2Fld2='" & tbl2fld2 & "' ) "
            'End If
            If Not HasRecords(sqls) Then
                'check if join reverted exist
                'If IsCacheDatabase() Then
                sqls = "SELECT * FROM OURReportSQLquery WHERE (Param1 LIKE '%" & repdb.Trim.Replace(" ", "%") & "%' AND ReportId = '" & rep & "' AND DOING = 'JOIN' AND UCASE([Tbl1])='" & UCase(tbl2) & "' AND UCASE([Tbl2])='" & UCase(tbl1) & "' AND UCASE([Tbl1Fld1])='" & UCase(tbl2fld2) & "' AND UCASE([Tbl2Fld2])='" & UCase(tbl1fld1) & "' ) "
                'Else
                '    sqls = "SELECT * FROM OURReportSQLquery WHERE (Param1 LIKE '%" & repdb & "%' AND ReportId = '" & rep & "' AND DOING = 'JOIN' AND Tbl1='" & tbl2 & "' AND Tbl2='" & tbl1 & "' AND Tbl1Fld1='" & tbl2fld2 & "' AND Tbl2Fld2='" & tbl1fld1 & "' ) "
                'End If
                If Not HasRecords(sqls) Then
                    'insert join
                    insSQL = "INSERT INTO OURReportSQLquery (ReportID, Doing, Tbl1, Tbl1Fld1, Tbl2, Tbl2Fld2, comments,RecOrder,Param1,Param2,Logical) VALUES('" & rep & "','JOIN','" & tbl1 & "','" & tbl1fld1 & "','" & tbl2 & "','" & tbl2fld2 & "','" & jointype & "'," & recodr & ",'" & repdb & "','" & relatn & "','" & commnt & "')"
                    ret = ExequteSQLquery(insSQL)
                End If
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function AddField(ByVal rep As String, ByVal tbl As String, ByVal fld As String, ByVal comment As String) As String
        Dim ret As String = String.Empty
        Dim sqls As String = String.Empty
        Try
            'add requested field
            If IsCacheDatabase() Then
                sqls = "SELECT * FROM OURReportSQLquery WHERE (ReportId = '" & rep & "' AND DOING = 'SELECT' AND UCASE(Tbl1)='" & UCase(tbl) & "' AND UCASE(Tbl1Fld1)='" & UCase(fld) & "' ) "
            Else
                sqls = "SELECT * FROM OURReportSQLquery WHERE (ReportId = '" & rep & "' AND DOING = 'SELECT' AND Tbl1='" & tbl & "' AND Tbl1Fld1='" & fld & "') "
            End If
            If Not HasRecords(sqls) Then
                sqls = "INSERT INTO OURReportSQLquery (ReportID, Doing, Tbl1, Tbl1Fld1, Param1) VALUES('" & rep & "','SELECT','" & tbl & "','" & fld & "','" & comment & "')"
                ret = ExequteSQLquery(sqls)
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function IsCacheDatabase(Optional ByVal myprovider As String = "") As Boolean
        Dim ret As Boolean = False
        Try
            If myprovider = "" Then myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            If myprovider.StartsWith("InterSystems.Data.") Then
                ret = True
            End If
        Catch ex As Exception
        End Try
        Return ret
    End Function
    Public Function IsOracleDatabase(Optional ByVal myprovider As String = "") As Boolean
        Dim ret As Boolean = False
        Try
            If myprovider = "" Then myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            If myprovider = "Oracle.ManagedDataAccess.Client" Then
                ret = True
            End If
        Catch ex As Exception
        End Try
        Return ret
    End Function
    Public Function IsPostgreDatabase(Optional ByVal myprovider As String = "") As Boolean
        Dim ret As Boolean = False
        Try
            If myprovider = "" Then myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            If myprovider.StartsWith("Npgsql") Then
                ret = True
            End If
        Catch ex As Exception
        End Try
        Return ret
    End Function
    Public Function FixReservedWords(ByVal fldname As String, Optional dbtype As String = "", Optional connstr As String = "") As String
        'This function is used for table names and field names as well.
        Dim ret As String = String.Empty
        Dim dbcase = String.Empty
        If dbtype = "" OrElse connstr.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString.ToUpper Then
            dbtype = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
        ElseIf System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing AndAlso connstr.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString.ToUpper Then
            dbtype = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ProviderName.ToString
            dbcase = ConfigurationManager.AppSettings("csvdbcase").ToString
        Else
            dbcase = userdbcase
        End If
        If dbtype.StartsWith("InterSystems.Data.") AndAlso fldname.StartsWith("User.") Then
            fldname = fldname.Replace("User.", "")
        End If
        'If dbtype.StartsWith("InterSystems.Data.") AndAlso fldname.StartsWith("UserData.") Then
        '    fldname = fldname.Replace("UserData.", "")
        'End If
        If dbtype = "Npgsql" AndAlso dbcase = "doublequoted" Then
            ret = fldname
            If Not fldname.StartsWith("""") Then
                ret = """" & ret
            End If
            If Not fldname.EndsWith("""") Then
                ret = ret & """"
            End If
            Return ret
        End If
        Try
            Dim sCheck As String = String.Empty
            'The SQL query will be created in OUR db provider syntax, and will be corrected for User if needed in Correct* and Convert* functions
            If dbtype = "" AndAlso IsCacheDatabase() Then    'checks OUR db provider
                'TODO check where we need this
                dbtype = "InterSystems.Data."
            ElseIf dbtype = "" Then
                dbtype = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            End If

            'reserved words in Cache 
            Dim CacheReservedWords As String = "| ABSOLUTE | ADD | ALL | ALLOCATE | ALTER | AND | ANY | ARE | AS | "
            CacheReservedWords = CacheReservedWords & "ASC | ASSERTION | AT | AUTHORIZATION | AVG | BEGIN | BETWEEN | "
            CacheReservedWords = CacheReservedWords & "BIT | BIT_LENGTH | BOTH | BY | CASCADE | CASE | CAST | "
            CacheReservedWords = CacheReservedWords & "CHAR | CHARACTER | CHARACTER_LENGTH | CHAR_LENGTH | "
            CacheReservedWords = CacheReservedWords & "CHECK | CLOSE | COALESCE | COLLATE | COMMIT | CONNECT | "
            CacheReservedWords = CacheReservedWords & "CONNECTION | CONSTRAINT | CONSTRAINTS | CONTINUE | CONVERT | "
            CacheReservedWords = CacheReservedWords & "CORRESPONDING | COUNT | CREATE | CROSS | CURRENT | "
            CacheReservedWords = CacheReservedWords & "CURRENT_DATE | CURRENT_TIME | CURRENT_TIMESTAMP | "
            CacheReservedWords = CacheReservedWords & "CURRENT_USER | CURSOR | DATE | DEALLOCATE | DEC | DECIMAL | "
            CacheReservedWords = CacheReservedWords & "Declare | DEFAULT | DEFERRABLE | DEFERRED | DELETE | DESC | "
            CacheReservedWords = CacheReservedWords & "DESCRIBE | DESCRIPTOR | DIAGNOSTICS | DISCONNECT | DISTINCT | "
            CacheReservedWords = CacheReservedWords & "DOMAIN | Double | DROP | Else | End | ENDEXEC | ESCAPE | EXCEPT | "
            CacheReservedWords = CacheReservedWords & "EXCEPTION | EXEC | EXECUTE | EXISTS | EXTERNAL | EXTRACT | "
            CacheReservedWords = CacheReservedWords & "FALSE | FETCH | FIRST | FLOAT | FOR | FOREIGN | FOUND | FROM | FULL | "
            CacheReservedWords = CacheReservedWords & "Get | Global | GO | GoTo | GRANT | GROUP | HAVING | HOUR | "
            CacheReservedWords = CacheReservedWords & "IDENTITY | IMMEDIATE | IN | INDICATOR | INITIALLY | "
            CacheReservedWords = CacheReservedWords & "INNER | INPUT | INSENSITIVE | INSERT | INT | Integer | INTERSECT | "
            CacheReservedWords = CacheReservedWords & "INTERVAL | INTO | Is | ISOLATION | JOIN | LANGUAGE | LAST | "
            CacheReservedWords = CacheReservedWords & "LEADING | LEFT | LEVEL | Like | LOCAL | LOWER | MATCH | MAX | MIN | "
            CacheReservedWords = CacheReservedWords & "MINUTE | MODULE | NAMES | NATIONAL | NATURAL | NCHAR | "
            CacheReservedWords = CacheReservedWords & "Next | NO | Not | NULL | NULLIF | NUMERIC | OCTET_LENGTH | OF | ON | "
            CacheReservedWords = CacheReservedWords & "ONLY | OPEN | OPTION | Or | OUTER | OUTPUT | OVERLAPS | "
            CacheReservedWords = CacheReservedWords & "PAD | PARTIAL | PREPARE | PRESERVE | PRIMARY | PRIOR | PRIVILEGES | "
            CacheReservedWords = CacheReservedWords & "PROCEDURE | PUBLIC | READ | REAL | REFERENCES | RELATIVE | "
            CacheReservedWords = CacheReservedWords & "RESTRICT | REVOKE | RIGHT | ROLE | ROLLBACK | ROWS | "
            CacheReservedWords = CacheReservedWords & "SCHEMA | SCROLL | SECOND | SECTION | SELECT | SESSION_USER | "
            CacheReservedWords = CacheReservedWords & "Set | SMALLINT | SOME | SPACE | SQLERROR | SQLSTATE | STATISTICS | "
            CacheReservedWords = CacheReservedWords & "SUBSTRING | SUM | SYSDATE | SYSTEM_USER | TABLE | TEMPORARY | "
            CacheReservedWords = CacheReservedWords & "THEN | TIME | TIMEZONE_HOUR | TIMEZONE_MINUTE | TO | TOP | "
            CacheReservedWords = CacheReservedWords & "TRAILING | TRANSACTION | TRIM | TRUE | UNION | UNIQUE | "
            CacheReservedWords = CacheReservedWords & "UPDATE | UPPER | USER | USING | VALUES | VARCHAR | VARYING | WHEN | "
            CacheReservedWords = CacheReservedWords & "WHENEVER | WHERE | WITH | WORK | WRITE |"
            CacheReservedWords = CacheReservedWords.ToUpper

            'reserved words in SQL Server and MySql
            Dim SqlReservedWords As String = ",ADD,ALL,ALTER,AD,ANY,AS,ASC,AUTHORIZATION"
            SqlReservedWords = SqlReservedWords & ",BACKUP,BEGIN,BETWEEN,BREAK,BROWSE,BULK,BY"
            SqlReservedWords = SqlReservedWords & ",CASCADE,CASE,CHECK,CHECKPOINT,CLOSE,CLUSTERED,COALESCE"
            SqlReservedWords = SqlReservedWords & ",COLLATE,COLUMN,COMMIT,COMPUTE,CONSTRAINT,CONTAINS"
            SqlReservedWords = SqlReservedWords & ",CONTAINSTABLE,CONTINUE,CONVERT,CREATE,CROSS,CURRENT"
            SqlReservedWords = SqlReservedWords & ",CURRENT_DATE,CURRENT_TIME,CURRENT_TIMESTAMP,CURRENT_USER"
            SqlReservedWords = SqlReservedWords & ",CURSOR,DATABASE,DBCC,DEALLOCATE,DECLARE,DEFAULT,DELETE"
            SqlReservedWords = SqlReservedWords & ",DENY,DESC,DISK,DISTINCT,DISTRIBUTED,DOUBLE,DROP,DUMMY"
            SqlReservedWords = SqlReservedWords & ",DUMMY,DUMP,ELSE,END,ERRLVL,ESCAPE,EXCEPT,EXEC,EXECUTE"
            SqlReservedWords = SqlReservedWords & ",EXISTS,EXIT,FETCH,FILE,FILLFACTOR,FOR,FOREIGN,FREETEXT"
            SqlReservedWords = SqlReservedWords & ",FREETEXTTABLE,FROM,FULL,FUNCTION,GOTO,GRANT,GROUP,HAVING"
            SqlReservedWords = SqlReservedWords & ",HOLDLOCK,IDENTITY,IDENTITY_INSERT,IDENTITYCOL,IF,IN,INDEX"
            SqlReservedWords = SqlReservedWords & ",INNER,INSERT,INTERSECT,INTO,IS,JOIN,KEY,KILL,LEFT,LIKE"
            SqlReservedWords = SqlReservedWords & ",LINENO,LOAD,NATIONAL,NOCHECK,NONCLUSTERED,NOT,NULL,NULLIF"
            SqlReservedWords = SqlReservedWords & ",OF,OFF,OFFSETS,ON,OPEN,OPENDATASOURCE,OPENQUERY,OPENROWSET"
            SqlReservedWords = SqlReservedWords & ",OPENXML,OPTION,OR,ORDER,OUTER,OVER,PERCENT,PLAN,PRECISION"
            SqlReservedWords = SqlReservedWords & ",PRIMARY,PRINT,PROC,PROCEDURE,PUBLIC,RANGE,RAISEERROR,READ"
            SqlReservedWords = SqlReservedWords & ",READTEXT,RECONFIGURE,REFERENCES,REPLICATION,RESTORE"
            SqlReservedWords = SqlReservedWords & ",RESTRICT,RETURN,REVOKE,RIGHT,ROLLBACK,ROWCOUNT,ROWGUIDCOL"
            SqlReservedWords = SqlReservedWords & ",RULE,SAVE,SCHEMA,SELECT,SESSION_USER,SET,SETUSER,SHUTDOWN"
            SqlReservedWords = SqlReservedWords & ",SOME,STATISTICS,SYSTEM_USER,TABLE,TEXTSIZE,THEN,TO,TOP"
            SqlReservedWords = SqlReservedWords & ",TRAN,TRANSACTION,TRIGGER,TRUNCATE,TSEQUAL,UNION,UNIQUE"
            SqlReservedWords = SqlReservedWords & ",UPDATE,UPDATETEXT,USE,USER,VALUES,VARYING,VIEW,WAITFOR"
            SqlReservedWords = SqlReservedWords & ",WHEN,WHERE,WHILE,WITH,WRITETEXT,ABSOLUTE,ACTION,ADMIN"
            SqlReservedWords = SqlReservedWords & ",AFTER,AGGREGATE,ALIAS,ALLOCATE,ARE,ARRAY,ASSERTION,AT"
            SqlReservedWords = SqlReservedWords & ",BEFORE,BINARY,BIT,BLOB,BOOLEAN,BOTH,BREADTH,CALL,CASCADED"
            SqlReservedWords = SqlReservedWords & ",CAST,CATALOG,CHAR,CHARACTER,CLASS,CLOB,COLLATION,COMPLETION"
            SqlReservedWords = SqlReservedWords & ",CONNECT,CONNECTION,CONTRAINTS,CONSTRUCTOR,CORRESPONDING"
            SqlReservedWords = SqlReservedWords & ",CUBE,CURRENT_PATH,CURRENT_ROLE,CYCLE,DATA,DATE,DAY,DEC"
            SqlReservedWords = SqlReservedWords & ",DECIMAL,DEFERRABLE,DEFFERRED,DEPTH,DEREF,DESCRIBE,DESCRIPTOR"
            SqlReservedWords = SqlReservedWords & ",DESTROY,DESTRUCTOR,DETERMINISTIC,DICTIONARY,DIAGNOSTICS"
            SqlReservedWords = SqlReservedWords & ",DISCONNECT,DOMAIN,DYNAMIC,EACH,END-EXEC,EQUALS,EVERY"
            SqlReservedWords = SqlReservedWords & ",EXCEPTION,EXTERNAL,FALSE,FIRST,FLOAT,FOUND,FREE,GENERAL"
            SqlReservedWords = SqlReservedWords & ",GET,GLOBAL,GO,GROUPING,HOST,HOUR,IGNORE,IMMEDIATE,INDICATOR"
            SqlReservedWords = SqlReservedWords & ",INITIALIZE,INITIALLY,INOUT,INPUT,INT,INTEGER,INTERVAL"
            SqlReservedWords = SqlReservedWords & ",ISOLATION,ITERATE,LANGUAGE,LARGE,LAST,LATERAL,LEADING,LESS"
            SqlReservedWords = SqlReservedWords & ",LEVEL,LIMIT,LOCAL,LOCALTIME,LOCALTIMESTAMP,LOCATIOR,MAP"
            SqlReservedWords = SqlReservedWords & ",MATCH,MINUTE,MODIFIES,MODIFY,MODULE,MONTH,NAMES,NATURAL"
            SqlReservedWords = SqlReservedWords & ",NCLOB,NEW,NEXT,NO,NONE,NUMERIC,OBJECT,OLD,ONLY,OPERATION"
            SqlReservedWords = SqlReservedWords & ",ORDINALITY,OUT,OUTPUT,PAD,PARAMETER,PARAMETERS,PARTIAL"
            SqlReservedWords = SqlReservedWords & ",PATH,POSTFIX,PREFIX,PREORDER,PREPARE,PRESERVE,PRIOR"
            SqlReservedWords = SqlReservedWords & ",PRIVILEGES,READS,REAL,RECURSIVE,REF,REFERENCING,RELATIVE"
            SqlReservedWords = SqlReservedWords & ",RESULT,RETURNS,ROLE,ROLLUP,ROUTINE,ROW,ROWS,SAVEPOINT,SCROLL"
            SqlReservedWords = SqlReservedWords & ",SCROLL,SCOPE,SEARCH,SECOND,SECTION,SEQUENCE,SESSION,SETS"
            SqlReservedWords = SqlReservedWords & ",SIZE,SHOW,SMALLINT,SPACE,SPECIFIC,SPECIFICTYPE,SQL,SQLEXCEPTION"
            SqlReservedWords = SqlReservedWords & ",SQLSTATE,SQLWARNING,START,STATE,STATEMENT,STATIC,STRUCTURE"
            SqlReservedWords = SqlReservedWords & ",TEMPORARY,TERMINATE,THAN,TIME,TIMESTAMP,TIMEZONE_HOUR"
            SqlReservedWords = SqlReservedWords & ",TIMEZONE_MINUTE,TRAILING,TRANSLATION,TREAT,TRUE,UNDER"
            SqlReservedWords = SqlReservedWords & ",UNKNOWN,UNNEST,USAGE,USING,VALUE,VARCHAR,VARIABLE,WHENEVER"
            SqlReservedWords = SqlReservedWords & ",WITHOUT,WORK,WRITE,YEAR,ZONE,"

            Dim OracleReservedWords As String = ",AGGREGATE,AGGREGATES,ALL,ALLOW,ANALYZE,ANCESTOR,AND"
            OracleReservedWords &= ",ANY,AS,AS,AT,AVG,BETWEEN,BINARY_DOUBLE,BINARY_FLOAT,BLOB"
            OracleReservedWords &= ",BRANCH,BUILD,BY,BYTE,CASE,CAST,CHAR,CHILD,CLEAR,CLOB,COMMIT"
            OracleReservedWords &= ",COMPILE,CONSIDER,COUNT,DATATYPE,DATE,DATE_MEASURE,DAY"
            OracleReservedWords &= ",DECIMAL,DELETE,DESC,DESCENDANT,DIMENSION,DISALLOW,DIVISION"
            OracleReservedWords &= ",DML,ELSE,END,ESCAPE,EXECUTE,FIRST,FLOAT,FOR,FROM,HIERARCHIES"
            OracleReservedWords &= ",HIERARCHY,HOUR,IGNORE,IN,INFINITE,INSERT,INTEGER,INTERVAL"
            OracleReservedWords &= ",INTO,IS,LAST,LEAF_DESCENDANT,LEAVES,LEVEL,LEVELS,LIKE"
            OracleReservedWords &= ",LIKEC,LIKE2,LIKE4,LOAD,LOCAL,LOG_SPEC,LONG,MAINTAIN,MAX"
            OracleReservedWords &= ",MEASURE,MEASURES,MEMBER,MEMBERS,MERGE,MLSLABEL,MIN,MINUTE"
            OracleReservedWords &= ",MODEL,MONTH,NAN,NCHAR,NCLOB,NO,NONE,NOT,NULL,NULLS,NUMBER"
            OracleReservedWords &= ",NVARCHAR2,OF,OLAP,OLAP_DML_EXPRESSION,ON,ONLY,OPERATOR,OR"
            OracleReservedWords &= ",ORDER,OVER,OVERFLOW,PARALLEL,PARENT,PLSQL,PRUNE,RAW"
            OracleReservedWords &= ",RELATIVE,ROOT_ANCESTOR,ROWID,SCN,SECOND,SELF,SERIAL,SET"
            OracleReservedWords &= ",SOLVE,SOME,SORT,SPEC,SUM,SYNCH,TEXT_MEASURE,THEN,TIME"
            OracleReservedWords &= ",TIMESTAMP,TO,UNBRANCH,UPDATE,USING,VALIDATE,VALUES,VARCHAR2"
            OracleReservedWords &= ",WHEN,WHERE,WITHIN,WITH,YEAR,ZERO,ZONE,ACCESS,ADD,ALTER"
            OracleReservedWords &= ",ASC,AUDIT,CHECK,CLUSTER,COLUMN,COLUMN_VALUE,COMMENT,COMPRESS"
            OracleReservedWords &= ",CONNECT,CREATE,CURRENT,DEFAULT,DISTINCT,DROP,EXCLUSIVE"
            OracleReservedWords &= ",EXISTS,FILE,GRANT,GROUP,HAVING,IDENTIFIED,IMMEDIATE,INCREMENT"
            OracleReservedWords &= ",INDEX,INITIAL,INTERSECT,LOCK,MAXEXTENTS,MINUS,MODE,MODIFY"
            OracleReservedWords &= ",NESTED_TABLE_ID,NOAUDIT,NOCOMPRESS,NOWAIT,OFFLINE,ONLINE"
            OracleReservedWords &= ",OPTION,PCTFREE,PRIOR,PUBLIC,RANGE,RENAME,RESOURCE,REVOKE,ROW"
            OracleReservedWords &= ",ROWNUM,ROWS,SELECT,SESSION,SHARE,SIZE,SMALLINT,START"
            OracleReservedWords &= ",SUCCESSFUL,SYNONYM,SYSDATE,TABLE,TRIGGER,UID,UNION,UNIQUE"
            OracleReservedWords &= ",USER,VARCHAR,VIEW,WHENEVER,"

            'check if ret is reserved word in Cache, SQL Server, Oracle, or MySql 
            'And fix it putting [] or "" around depending of dbtype

            'Dim i As Integer
            'Dim fldparts() As String = fldname.Split(".")
            'For i = 0 To fldparts.Length - 1
            '    sCheck = "| " & fldparts(i).ToUpper & " |"
            '    If CacheReservedWords.Contains(sCheck) Then
            '        If dbtype.StartsWith("InterSystems.Data.") Then
            '            fldparts(i) = """" & fldparts(i) & """"
            '        Else
            '            fldparts(i) = "[" & fldparts(i) & "]"
            '        End If
            '    Else
            '        sCheck = "," & fldparts(i).ToUpper & ","
            '        If SqlReservedWords.Contains(sCheck) Then
            '            If dbtype.StartsWith("InterSystems.Data.") Then
            '                fldparts(i) = """" & fldparts(i) & """"
            '            Else
            '                fldparts(i) = "[" & fldparts(i) & "]"
            '            End If
            '        ElseIf OracleReservedWords.Contains(sCheck) Then
            '            If dbtype.StartsWith("InterSystems.Data.") Then
            '                fldparts(i) = """" & fldparts(i) & """"
            '            Else
            '                fldparts(i) = "[" & fldparts(i) & "]"
            '            End If
            '        End If

            '    End If
            'Next

            Dim i As Integer
            Dim fldparts() As String = fldname.Split(".")
            For i = 0 To fldparts.Length - 1
                sCheck = "," & fldparts(i).ToUpper & ","
                If dbtype.StartsWith("InterSystems.Data.") Then
                    sCheck = "| " & fldparts(i).ToUpper & " |"
                    If CacheReservedWords.Contains(sCheck) Then
                        fldparts(i) = """" & fldparts(i) & """"
                    End If
                ElseIf dbtype = "Oracle.ManagedDataAccess.Client" Then
                    If OracleReservedWords.Contains(sCheck) Then
                        fldparts(i) = """" & fldparts(i) & """"
                    End If
                ElseIf dbtype = "System.Data.Odbc" Then
                    'for ODBC we use Sql Server syntax
                    If SqlReservedWords.Contains(sCheck) OrElse CacheReservedWords.Contains(sCheck) OrElse OracleReservedWords.Contains(sCheck) Then
                        fldparts(i) = "[" & fldparts(i) & "]"
                    End If
                ElseIf dbtype = "System.Data.OleDb" Then
                    'for OleDb we use Sql Server syntax
                    If SqlReservedWords.Contains(sCheck) OrElse CacheReservedWords.Contains(sCheck) OrElse OracleReservedWords.Contains(sCheck) Then
                        fldparts(i) = "[" & fldparts(i) & "]"
                    End If
                Else
                    If SqlReservedWords.Contains(sCheck) Then
                        'TODO other providers ?
                        If dbtype.StartsWith("InterSystems.Data.") Then
                            fldparts(i) = """" & fldparts(i) & """"
                        ElseIf dbtype.Contains("MySql") Then
                            fldparts(i) = "`" & fldparts(i) & "`"
                        ElseIf dbtype.Contains("Npgsql") Then
                            fldparts(i) = """" & fldparts(i) & """"
                        Else 'not Cache
                            fldparts(i) = "[" & fldparts(i) & "]"
                        End If
                    End If
                End If
            Next
            ret = ""
            For i = 0 To fldparts.Length - 1
                If i > 0 Then ret = ret & "."
                ret = ret & fldparts(i)
            Next
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function FixDoubleFieldNames(ByVal rep As String, ByVal sqlst As String, Optional userconnprv As String = "") As String
        'corrects tables and fields names with dots and reserved words
        Dim ret As String = String.Empty
        Try
            'Fix sqlst renaming double fields
            Dim i, j As Integer
            Dim dvt, dvf As DataView
            Dim fldName As String = String.Empty
            Dim tblName As String = String.Empty
            Dim fldNamefixed As String = String.Empty
            Dim sctSQL As String = String.Empty
            'fixing fields in sqlst query with *, replacing * for list of fields
            If sqlst.IndexOf(".*") > 0 OrElse sqlst.IndexOf(" * ") > 0 Then
                sqlst = "SELECT "
                Dim dtf As DataTable
                Dim dt As DataTable = GetReportTables(rep)
                Dim tbl As String = String.Empty
                If dt.Rows.Count > 0 Then
                    'loop for tables
                    For i = 0 To dt.Rows.Count - 1
                        tbl = dt.Rows(i)("Tbl1").ToString
                        dtf = GetSQLTableFields(rep, tbl)
                        If dtf.Rows(0)("Comments").ToString.ToUpper = "TRUE" Then sqlst = sqlst & " DISTINCT "
                        For j = 0 To dtf.Rows.Count - 1
                            If j > 0 OrElse i > 0 Then sqlst = sqlst & ", "
                            sqlst = sqlst & FixReservedWords(CorrectTableNameWithDots(dtf.Rows(j)("Tbl1").ToString, userconnprv), userconnprv) & "." & FixReservedWords(dtf.Rows(j)("Tbl1Fld1"), userconnprv).ToString
                        Next
                    Next
                End If
            End If
            If Not HasSQLData(repid) Then
                Dim htFld As New Hashtable
                Dim fld As String = String.Empty
                Dim flds As String() = sqlst.Split(",")
                Dim n As Integer = 0

                For i = 0 To flds.Length - 1
                    If flds(i) <> String.Empty Then
                        n = Pieces(flds(i), ".")
                        fld = Piece(flds(i).Trim, ".", n)
                        If n > 1 Then
                            tblName = Piece(flds(i).Trim, ".", 1, n - 1)
                        Else
                            tblName = String.Empty
                        End If
                        If htFld(fld) IsNot Nothing Then
                            j = CInt(htFld(fld)) + 1
                            htFld(fld) = CStr(j)
                            sqlst = sqlst.Replace(tblName & "." & fld, tblName & "." & fld & " AS " & fld & j.ToString)
                        Else
                            htFld.Add(fld, "1")
                        End If
                    End If
                Next
            Else
                'get list of report sql query fields, in Cache return in upper case
                sctSQL = "SELECT DISTINCT Tbl1Fld1 FROM OURReportSQLquery WHERE (ReportID='" & rep & "' AND Doing='SELECT') ORDER BY Tbl1Fld1"
                dvf = mRecords(sctSQL) 'OUR database
                'loop for all fields in report sql
                If Not dvf Is Nothing AndAlso dvf.Count > 0 AndAlso dvf.Table.Rows.Count > 0 Then
                    For i = 0 To dvf.Table.Rows.Count - 1
                        fldName = dvf.Table.Rows(i)("Tbl1Fld1").ToString

                        'check if the field with the same name selected from other tables
                        If IsCacheDatabase() Then
                            'Ucase for Cache
                            sctSQL = "SELECT * FROM OURReportSQLquery WHERE (ReportID='" & rep & "' AND Doing='SELECT' AND UCASE(Tbl1Fld1)='" & fldName & "') ORDER BY Tbl1"
                        Else
                            sctSQL = "SELECT * FROM OURReportSQLquery WHERE (ReportID='" & rep & "' AND Doing='SELECT' AND Tbl1Fld1='" & fldName & "') ORDER BY Tbl1"
                        End If
                        dvt = mRecords(sctSQL) 'OUR database
                        If Not dvt Is Nothing AndAlso dvt.Count > 0 AndAlso dvt.Table.Rows.Count > 0 Then
                            For j = 0 To dvt.Table.Rows.Count - 1
                                If j > 0 Then
                                    tblName = FixReservedWords(CorrectTableNameWithDots(dvt.Table.Rows(j)("Tbl1").ToString, userconnprv), userconnprv)
                                    fldNamefixed = FixReservedWords(fldName)
                                    If dvt.Table.Rows(j)("Tbl1Fld1").ToString = fldName Then
                                        sqlst = sqlst.Replace(tblName & "." & fldNamefixed, tblName & "." & fldNamefixed & " AS " & fldName & j.ToString)
                                    End If
                                End If
                            Next
                        End If
                    Next
                End If
            End If

            ret = sqlst
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function AddTableNameToField(ByVal repid As String, ByVal fld As String, ByVal sql As String, Optional ByVal err As String = "", Optional userconnprv As String = "") As String
        Dim ret As String = ""
        Dim k As Integer
        Try
            sql = FixDoubleFieldNames(repid, sql, userconnprv).Replace(",", " ") & " "  'corrected tables and fields names with dots and reserved words
            k = sql.IndexOf("." & fld & " ")
            If k > 0 Then
                sql = sql.Substring(0, k)
                ret = sql.Substring(sql.LastIndexOf(" ")).Trim
                If ret <> "" Then
                    ret = ret & "." & fld
                End If
            Else
                ret = fld
            End If
        Catch ex As Exception
            err = ex.Message
        End Try
        Return ret
    End Function
    'Public Function UpdateParametersOld(ByVal repid As String, ByVal sqlquerytext As String) As String
    '    'NOT IN USE
    '    Dim err As String = String.Empty
    '    Try
    '        'update parameters with new sql query - sqlquerytext
    '        Dim i, n, k As Integer
    '        Dim ddsql As String = String.Empty
    '        Dim newddsql As String = String.Empty
    '        Dim updsql As String = String.Empty
    '        Dim sqltextFrom As String = String.Empty
    '        Dim sqltextSelect As String = String.Empty
    '        Dim er As String = String.Empty
    '        'OURReportShow has parameters(drop-downs in Report Show page) definition
    '        Dim dv2 As DataView = mRecords("SELECT * FROM OURReportShow WHERE (ReportID='" & repid & "') ORDER BY Indx")  'from OUR db
    '        n = dv2.Table.Rows.Count   'how many parameters (drop-downs)
    '        If n > 0 Then
    '            'SELECT part of sql query
    '            If sqlquerytext.ToUpper.IndexOf(" ORDER BY ") > 0 Then
    '                sqlquerytext = sqlquerytext.Substring(0, sqlquerytext.ToUpper.IndexOf(" ORDER BY ") + 1)
    '            End If
    '            sqltextSelect = sqlquerytext.Substring(0, sqlquerytext.ToUpper.IndexOf(" FROM ") + 1)
    '            'FROM part and the rest of sql query
    '            sqltextFrom = sqlquerytext.Substring(sqlquerytext.ToUpper.IndexOf(" FROM "))
    '            If IsCacheDatabase() Then
    '                'Remove TOP n, because Cache working strange if included sql has TOP n , it return subset of distinct values
    '                k = sqltextSelect.ToUpper.IndexOf(" TOP ")
    '                If k > 0 Then
    '                    sqltextSelect = sqltextSelect.Substring(k + 5).Trim
    '                    sqltextSelect = sqltextSelect.Substring(sqltextSelect.IndexOf(" "))
    '                    sqltextSelect = " SELECT " & sqltextSelect
    '                End If
    '            End If
    '            'FixDoubleFieldNames return only SELECT part of query
    '            sqltextSelect = FixDoubleFieldNames(repid, sqltextSelect).Trim
    '            'add the rest of query
    '            sqlquerytext = sqltextSelect & " " & sqltextFrom
    '        End If
    '        'loop on parameters
    '        For i = 0 To dv2.Table.Rows.Count - 1   'draw drop-down start
    '            ddsql = String.Empty
    '            newddsql = String.Empty
    '            updsql = String.Empty
    '            ddsql = dv2.Table.Rows(i)("DropDownSQL") & " "
    '            newddsql = ""
    '            n = ddsql.IndexOf("SELECT DISTINCT sub.")
    '            If n = 0 Then
    '                k = ddsql.IndexOf(" sub ")
    '                newddsql = ddsql.Substring(0, ddsql.IndexOf("(") + 1) & sqlquerytext & ")" & ddsql.Substring(k)
    '            Else  'ddsql does not based on sub
    '                newddsql = "SELECT DISTINCT sub." & dv2.Table.Rows(i)("DropDownFieldName") & " FROM (" & sqlquerytext & ") sub"
    '            End If
    '            If IsCacheDatabase() Then
    '                'might need a coreection with double quotes(?)
    '                updsql = "UPDATE OURReportShow SET DropDownSQL='" & newddsql.Replace("'", """") & "' WHERE (ReportID='" & repid & "' AND Indx=" & dv2.Table.Rows(i)("Indx") & ")"
    '            Else
    '                updsql = "UPDATE OURReportShow SET DropDownSQL='" & newddsql.Replace("'", """") & "' WHERE (ReportID='" & repid & "' AND Indx=" & dv2.Table.Rows(i)("Indx") & ")"
    '            End If
    '            er = ExequteSQLquery(updsql)
    '            If er.Trim <> "Query executed fine." Then
    '                err = err & "  " & err
    '            End If
    '        Next
    '    Catch ex As Exception
    '        err = err & "  " & ex.Message
    '    End Try
    '    Return err.Trim
    'End Function
    Public Function SwitchKeyVal(htToSwitch As Hashtable) As Hashtable
        Dim htSwitched As New Hashtable

        For Each de As DictionaryEntry In htToSwitch
            htSwitched.Add(de.Value, de.Key)
        Next
        Return htSwitched
    End Function
    Public Function GetXSDFieldHashtable(repid As String) As Hashtable
        Dim htFields As New Hashtable
        Dim htDuplicate As New Hashtable
        Dim tbl As String = String.Empty
        Dim fld As String = String.Empty
        Dim tblfld As String = String.Empty
        Dim j As Integer = 0
        Dim dr As DataRow

        Dim dt As DataTable = GetSQLFields(repid)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                dr = dt.Rows(i)
                tbl = dr("Tbl1").ToString.ToLower
                fld = dr("Tbl1Fld1")
                tblfld = tbl & "." & fld
                If htFields(fld) IsNot Nothing Then
                    If htDuplicate(fld) IsNot Nothing Then
                        j = CInt(htDuplicate(fld))
                        Dim f As String = fld
                        fld = fld & j.ToString
                        j += 1
                        htDuplicate(f) = j.ToString
                    Else
                        htDuplicate.Add(fld, "2")
                        fld = fld & "1"
                    End If
                End If
                htFields.Add(fld, tblfld)
            Next
        End If

        Dim HasExpression As Boolean = False
        Dim IsCombined As Boolean = False
        dt = GetReportFields(repid)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                dr = dt.Rows(i)
                fld = dr("Val").ToString
                If htFields(fld) = "" Then
                    HasExpression = IIf(Not IsDBNull(dr("Prop2")) AndAlso dr("Prop2").ToString <> String.Empty, True, False)
                    IsCombined = IIf(Not IsDBNull(dr("Prop3")) AndAlso dr("Prop3").ToString <> String.Empty, True, False)
                    If HasExpression Then
                        tblfld = fld & ".Expression"
                        htFields.Add(fld, tblfld)
                    ElseIf IsCombined Then
                        tblfld = fld & ".Combined"
                        htFields.Add(fld, tblfld)
                    End If
                End If
            Next
        End If
        Return htFields
    End Function

    Public Function UpdateParameters(ByVal repid As String, ByVal sqlquerytext As String) As String
        Dim err As String = String.Empty
        Try
            'update parameters with new sql query - sqlquerytext
            Dim i, n As Integer
            Dim ddsql As String = String.Empty
            Dim ddComment As String = String.Empty
            Dim newddsql As String = String.Empty
            Dim updsql As String = String.Empty
            Dim sqltextFrom As String = String.Empty
            Dim sqltextSelect As String = String.Empty
            Dim field As String = String.Empty
            Dim sFld As String = String.Empty
            Dim table As String = String.Empty
            Dim tblfld As String = String.Empty
            Dim er As String = String.Empty
            'OURReportShow has parameters(drop-downs in Report Show page) definition
            Dim dv2 As DataView = mRecords("SELECT * FROM OURReportShow WHERE (ReportID='" & repid & "') ORDER BY Indx")  'from OUR db

            If dv2 IsNot Nothing AndAlso dv2.Table IsNot Nothing Then
                n = dv2.Table.Rows.Count   'how many parameters (drop-downs)
                If n > 0 Then
                    For i = 0 To dv2.Table.Rows.Count - 1   'draw drop-down start
                        ddsql = String.Empty
                        newddsql = String.Empty
                        updsql = String.Empty
                        ddsql = dv2.Table.Rows(i)("DropDownSQL")
                        ddComment = dv2.Table.Rows(i)("comments")
                        newddsql = ""
                        n = ddsql.IndexOf("SELECT DISTINCT sub.")
                        'Parameter is old style and checked. Convert to new style
                        If n = 0 AndAlso ddComment = "checked" Then
                            field = dv2.Table.Rows(i)("DropDownFieldName")
                            updsql = ddsql.Substring(ddsql.IndexOf("(") + 1).Trim
                            sqltextSelect = updsql.Substring(6)
                            sqltextSelect = sqltextSelect.Substring(0, sqltextSelect.ToUpper.IndexOf(" FROM "))
                            Dim sFields As String() = Split(sqltextSelect, ",")
                            For ii As Integer = 0 To sFields.Count - 1
                                n = Pieces(sFields(ii), ".")
                                sFld = Piece(sFields(ii).Trim, ".", n)
                                table = Piece(sFields(ii).Trim, ".", 1, n - 1)
                                table = table.Replace("`", "")
                                If sFld.Contains(" AS ") Then sFld = Piece(sFld, " AS ", 2)
                                sFld = sFld.Replace("`", "")
                                If field = sFld Then
                                    field = Piece(Piece(sFields(ii).Trim, ".", n), " AS ", 1)
                                    field = field.Replace("`", "")
                                    tblfld = table & "." & field
                                    newddsql = "SELECT DISTINCT " & FixReservedWords(field) & " FROM " & FixReservedWords(table) & " ORDER BY " & FixReservedWords(field)
                                    Exit For
                                End If
                            Next

                        Else  'ddsql not based on sub so don't change'
                            Continue For
                        End If
                        updsql = "UPDATE OURReportShow "
                        updsql &= "SET DropDownID='" & tblfld & "',"
                        updsql &= "DropDownName='" & sFld & "',"
                        updsql &= "DropDownFieldName='" & field & "',"
                        updsql &= "DropDownSQL='" & newddsql.Replace("'", """") & "'"
                        updsql &= " WHERE (ReportID='" & repid & "' AND Indx=" & dv2.Table.Rows(i)("Indx") & ")"
                        er = ExequteSQLquery(updsql)
                        If er.Trim <> "Query executed fine." Then
                            err = err & "  " & err
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            err = err & "  " & ex.Message
        End Try
        Return err.Trim
    End Function
    Public Function RegisterUser(ByVal accss As String, ByVal permit As String, ByVal logon As String, ByVal pass As String, ByVal name As String, ByVal org As String, ByVal appl As String, ByVal rol As String, ByVal grp1 As String, ByVal grp2 As String, ByVal grp3 As String, ByVal email As String, ByVal commts As String, ByVal userconnstr As String, ByVal userconnprv As String, ByVal StartDate As String, ByVal EndDate As String, Optional ByVal phone As String = "", Optional ByVal ourconnstr As String = "", Optional ByVal ourconnprv As String = "") As String
        Dim sqlq As String
        Dim ret As String = String.Empty
        Dim pagettl As String = ConfigurationManager.AppSettings("pagettl").ToString
        Try
            Dim adminemail = ConfigurationManager.AppSettings("supportemail").ToString
            Dim emailtxt As String = "User " & logon & " has been registered at " & pagettl & ". "
            If logon = email AndAlso logon = pass Then
                emailtxt = emailtxt & "Use your email as logon and as first time password. You will be asked to change it."
            End If
            'sqlq = "INSERT INTO [OURPermits] (" & FixReservedWords("Access") & ",[PERMIT],[NetId],[localpass],[Name],[Unit],[Application],[RoleApp],[Group1],[Group2],[Group3],[Email],[Comments],[ConnStr],[ConnPrv],[StartDate],[EndDate]) "
            sqlq = "INSERT INTO [OURPermits] (" & FixReservedWords("Access") & ",[PERMIT],[NetId],[localpass]," & FixReservedWords("Name") & ",[Unit]," & FixReservedWords("Application") & ",[RoleApp],[Group1],[Group2],[Group3],[Email],[Comments],[ConnStr],[ConnPrv],[StartDate],[EndDate]) "
            sqlq = sqlq & " VALUES ('" & accss & "','" & permit & "','" & logon & "','" & pass & "','" & name & "','" & org & "','" & appl & "','" & rol & "','" & grp1 & "','" & grp2 & "','" & grp3 & "','" & email & "','" & commts & "','" & userconnstr & "','" & userconnprv & "','"   '& StartDate & "','" & EndDate & "')"

            If userconnprv = "Oracle.ManagedDataAccess.Client" Then
                sqlq = sqlq & DateToStringFormat(CDate(StartDate), userconnprv, "dd-MMM-yy") & "','" & DateToStringFormat(CDate(EndDate), userconnprv, "dd-MMM-yy") & "')"
            Else
                sqlq = sqlq & StartDate & "','" & EndDate & "')"
            End If

            ret = ExequteSQLquery(sqlq, ourconnstr, ourconnprv)
            If ret = "Query executed fine." Then
                ret = "User has been registered."
                ret = WriteToAccessLog(logon, "Registered for userdb: " & userconnstr & "," & userconnprv, 1)
                ret = SendHTMLEmail("", "User " & logon & " has been registered at " & pagettl, emailtxt, email, adminemail)
                If phone.Trim <> "" Then WriteToAccessLog(logon, "Site Admin, phone " & phone & ", registered in " & ourconnstr & " for unitdb " & userconnstr & "," & userconnprv & " with result:  " & ret, 1)
                ret = "User has been registered."
            Else
                WriteToAccessLog(logon, "Register user crashed: " & ret, 2)
            End If
        Catch ex As Exception
            ret = ex.Message
            WriteToAccessLog(logon, "Registered user or email to him crashed: " & ret, 2)
        End Try
        Return ret
    End Function
    Public Function AddCustomJoinsToInitialOnes(ByVal repid As String, ByVal dbname As String, Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "", Optional er As String = "", Optional redo As Boolean = False) As String
        If redo = False Then
            Return "Not redo custom joins"
        End If
        Dim ret As String = String.Empty
        Dim msql As String = String.Empty
        Dim n As Integer = 0
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim dv As DataView
        Dim tbl1 As String = String.Empty
        Dim fld1 As String = String.Empty
        Dim tbl2 As String = String.Empty
        Dim fld2 As String = String.Empty
        Try
            Dim myconstring As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            Dim myconnprv As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            'Dim repdb As String = userconnstr.Substring(0, userconnstr.IndexOf("User ID")).Trim
            Dim repdb As String = userconnstr
            'If repdb.ToUpper.IndexOf("USER ID") > 0 Then repdb = repdb.Substring(0, repdb.ToUpper.IndexOf("USER ID")).Trim
            If userconnstr.ToUpper.IndexOf("USER ID") > 0 AndAlso userconnprv <> "Oracle.ManagedDataAccess.Client" Then
                repdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("USER ID")).Trim
            ElseIf userconnstr.ToUpper.IndexOf("PASSWORD") > 0 AndAlso userconnprv = "Oracle.ManagedDataAccess.Client" Then
                repdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("PASSWORD")).Trim
            End If
            'not initial, not datadriven
            'msql = "SELECT Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2 FROM OURReportSQLquery INNER JOIN OURReportInfo ON OURReportSQLquery.ReportId=OURReportInfo.ReportId WHERE (NOT OURReportSQLquery.ReportId='" & repid & "') AND OURReportSQLquery.Doing='JOIN' AND OURReportInfo.ReportDB LIKE '%" & repdb & "%' AND NOT Param2='datadriven'"
            msql = "SELECT Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2 FROM OURReportSQLquery  WHERE (NOT OURReportSQLquery.ReportId='" & repid & "') AND OURReportSQLquery.Doing='JOIN' AND Param1 LIKE '%" & repdb.Trim.Replace(" ", "%") & "%' AND NOT Param2='datadriven' AND NOT Param2='initial'"

            dv = mRecords(msql)
            If dv Is Nothing OrElse dv.Count = 0 OrElse dv.Table.Rows.Count = 0 Then
                ret = "no custom joins found"
            Else
                For i = 0 To dv.Table.Rows.Count - 1
                    tbl1 = dv.Table.Rows(i)("Tbl1")
                    tbl2 = dv.Table.Rows(i)("Tbl2")
                    fld1 = dv.Table.Rows(i)("Tbl1Fld1")
                    fld2 = dv.Table.Rows(i)("Tbl2Fld2")
                    ret = AddJoin(repid, tbl1, tbl2, fld1, fld2, "INNERJOIN", repdb, "custom")
                Next
            End If

        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function IsRelation(ByVal repdb As String, ByVal dbname As String, ByVal tbl As String, ByVal fld As String, Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "", Optional er As String = "") As Boolean
        Dim ret As String = String.Empty
        Dim sqls As String = String.Empty
        Dim brelation As Boolean = False
        Dim rep = dbname & "_joins"
        'find join for tbl and fld
        Try
            If IsCacheDatabase() Then
                sqls = "SELECT * FROM OURReportSQLquery WHERE (ReportId LIKE '" & rep & "%' AND DOING = 'JOIN' AND UCASE(Tbl1)='" & UCase(tbl) & "' AND UCASE(Tbl1Fld1)='" & UCase(fld) & "' AND Param1 LIKE  '%" & repdb & "%') "
            Else
                sqls = "Select * FROM OURReportSQLquery WHERE (ReportId LIKE '" & rep & "%' AND DOING = 'JOIN' AND Tbl1='" & tbl & "'  AND Tbl1Fld1='" & fld & "' AND Param1 LIKE  '%" & repdb & "%') "
            End If
            If HasRecords(sqls) Then
                Return True
            Else
                'check if join reverted exist
                If IsCacheDatabase() Then
                    sqls = "SELECT * FROM OURReportSQLquery WHERE (ReportId LIKE '" & rep & "%' AND DOING = 'JOIN' AND UCASE(Tbl2)='" & UCase(tbl) & "' AND UCASE(Tbl2Fld2)='" & UCase(fld) & "' AND Param1 LIKE  '%" & repdb & "%') "
                Else
                    sqls = "SELECT * FROM OURReportSQLquery WHERE (ReportId LIKE '" & rep & "&' AND DOING = 'JOIN' AND Tbl2='" & tbl & "' AND Tbl2Fld2='" & fld & "' AND Param1 LIKE  '%" & repdb & "%') "
                End If
                If HasRecords(sqls) Then
                    Return True
                Else
                    Return False
                End If
            End If
        Catch ex As Exception
            ret = ex.Message
            Return False
        End Try
    End Function
    Public Function MakeInitialJoins(ByVal logon As String, ByVal dvusertbls As DataTable, ByVal repid As String, ByVal dbname As String, Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "", Optional er As String = "", Optional redo As Boolean = False, Optional systables As Boolean = False) As String
        Dim ret As String = String.Empty
        Dim msql As String = String.Empty
        Dim n As Integer = 0
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim dv As DataView
        Dim tbl1 As String = String.Empty
        Dim fld1 As String = String.Empty
        Dim tbl2 As String = String.Empty
        Dim fld2 As String = String.Empty
        Dim repdb As String = String.Empty
        Try
            Dim myconstring As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            Dim myconnprv As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            'Dim ourrepdb As String = myconstring.Substring(0, myconstring.IndexOf("User ID")).Trim
            Dim ourrepdb As String = myconstring
            'If ourrepdb.ToUpper.IndexOf("USER ID") > 0 AndAlso userconnprv <> "Oracle.ManagedDataAccess.Client" Then
            '    ourrepdb = ourrepdb.Substring(0, ourrepdb.ToUpper.IndexOf("USER ID")).Trim
            'End If
            'If userconnstr.ToUpper.IndexOf("USER ID") > 0 AndAlso userconnprv <> "Oracle.ManagedDataAccess.Client" Then
            '    repdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("USER ID")).Trim
            'End If
            'If userconnstr.IndexOf("UID") > 0 AndAlso userconnprv <> "Oracle.ManagedDataAccess.Client" Then
            '    repdb = userconnstr.Substring(0, userconnstr.IndexOf("UID")).Trim
            'End If
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

            'CSV User
            'Dim csv As String = String.Empty
            If IsCSVuser(userconnstr, "", er) = "yes" Then
                'csv = "yes"
                Dim tbl As String = String.Empty
                If Not dvusertbls Is Nothing AndAlso dvusertbls.Rows.Count > 0 Then
                    If redo Then
                        For i = 0 To dvusertbls.Rows.Count - 1
                            For j = 0 To dvusertbls.Rows.Count - 1
                                tbl1 = dvusertbls.Rows(i)("Tbl1").ToString
                                tbl2 = dvusertbls.Rows(j)("Tbl2").ToString
                                'clearing initial Joins in repdb only for user tables
                                msql = "DELETE FROM OURReportSQLquery WHERE (Param2 = 'initial' AND Doing='JOIN' AND Param1 LIKE '%" & repdb.Trim.Replace(" ", "%") & "%'  AND Tbl1='" & tbl1 & "' AND Tbl2='" & tbl2 & "') "
                                er = ExequteSQLquery(msql)
                            Next
                        Next
                    End If
                    Dim dvc1 As DataView
                    Dim dvc2 As DataView
                    Try
                        For i = 0 To dvusertbls.Rows.Count - 1
                            For j = 0 To i  'dvusertbls.Rows.Count - 1
                                tbl1 = dvusertbls.Rows(i)("Tbl1").ToString
                                tbl2 = dvusertbls.Rows(j)("Tbl2").ToString
                                dvc1 = GetListOfTableFields(tbl1, userconnstr, userconnprv, er)
                                dvc2 = GetListOfTableFields(tbl2, userconnstr, userconnprv, er)

                                If dvc1 Is Nothing OrElse dvc1.Table Is Nothing OrElse dvc1.Table.Rows.Count = 0 OrElse er = "Not data in the table " & tbl1 Then
                                    ret = "no rows in table1"
                                    Return ret
                                End If
                                If dvc2 Is Nothing OrElse dvc2.Table Is Nothing OrElse dvc2.Table.Rows.Count = 0 OrElse er = "Not data in the table " & tbl2 Then
                                    ret = "no rows in table2"
                                    Return ret
                                End If
                                For m = 0 To dvc1.Table.Rows.Count - 1
                                    fld1 = dvc1.Table.Rows(m)("COLUMN_NAME")
                                    For n = 0 To dvc2.Table.Rows.Count - 1
                                        'check if both numeric, datetime, or text !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                        fld2 = dvc2.Table.Rows(n)("COLUMN_NAME")
                                        If tbl1.ToUpper = tbl2.ToUpper AndAlso fld1.ToUpper = fld2.ToUpper Then
                                            Continue For
                                        End If
                                        If dvc1.Table.Rows(m)("DATA_TYPE") <> dvc2.Table.Rows(n)("DATA_TYPE") Then
                                            Continue For
                                        End If
                                        ' If GetFieldDataType(tbl1, fld1, userconnstr, userconnprv).Trim = GetFieldDataType(tbl2, fld2, userconnstr, userconnprv).Trim Then
                                        ret = AddJoin(repid, tbl1, tbl2, fld1, fld2, "INNERJOIN", repdb, "initial")
                                        'End If
                                    Next
                                Next
                            Next
                        Next
                    Catch ex As Exception
                        ret = "ERROR!! " & ret
                    End Try
                    Return ret
                End If
                Return ret
            End If

            'NOT CSV User
            Dim sqlsys As String
            If redo Then
                'clearing initial Joins in repdb 
                msql = "DELETE FROM OURReportSQLquery WHERE (Param2 = 'initial' AND Doing='JOIN' AND Param1 LIKE '%" & repdb.Trim.Replace(" ", "%") & "%' )"
                er = ExequteSQLquery(msql)
            End If
            'create initial Joins
            If userconnprv.StartsWith("InterSystems.Data.") Then
                'create initial Joins from relashionships depending of allowing system tables or not
                If systables Then 'persistent classes included
                    'dv = mRecords("SELECT * FROM %Dictionary.PropertyDefinition WHERE (NOT ID %STARTSWITH '%') AND (NOT ID %STARTSWITH 'OUR.') AND (NOT Type %STARTSWITH '%') ORDER BY ID", er, userconnstr, userconnprv)
                    sqlsys = "SELECT %Dictionary.ClassDefinition.ClassType, %Dictionary.PropertyDefinition.* FROM %Dictionary.PropertyDefinition INNER JOIN %Dictionary.ClassDefinition On (%Dictionary.PropertyDefinition.parent=%Dictionary.ClassDefinition.ID)  WHERE ( %Dictionary.ClassDefinition.ClassType = 'persistent')  AND (NOT %Dictionary.ClassDefinition.ID %STARTSWITH 'OUR.') "
                    dv = mRecords(sqlsys, er, userconnstr, userconnprv)
                Else
                    dv = mRecords("SELECT * FROM %Dictionary.PropertyDefinition WHERE (NOT ID %STARTSWITH '%') AND (NOT ID %STARTSWITH 'INFORMATION.SCHEMA') AND (NOT ID %STARTSWITH 'OUR.') AND (NOT Type %STARTSWITH '%') ORDER BY ID", er, userconnstr, userconnprv)
                End If
                If Not dv Is Nothing AndAlso dv.Count > 0 AndAlso dv.Table.Rows.Count > 0 Then
                    For i = 0 To dv.Table.Rows.Count - 1
                        Try
                            If dv.Table.Rows(i)("Cardinality").ToString = "children" OrElse dv.Table.Rows(i)("Cardinality").ToString = "many" Then
                                tbl1 = dv.Table.Rows(i)("parent").ToString
                                tbl2 = dv.Table.Rows(i)("Type").ToString
                                'fld1 = dv.Table.Rows(i)("Name")
                                fld1 = "ID"
                                fld2 = dv.Table.Rows(i)("Inverse").ToString
                                If IsCacheClassPersistant(tbl1, userconnstr, userconnprv) AndAlso IsCacheClassPersistant(tbl2, userconnstr, userconnprv) Then
                                    ret = AddJoin(repid, tbl1, tbl2, fld1, fld2, "INNERJOIN", repdb, "initial")
                                End If
                            ElseIf dv.Table.Rows(i)("Cardinality").ToString = "" Then
                                tbl1 = dv.Table.Rows(i)("parent").ToString
                                tbl2 = dv.Table.Rows(i)("Type").ToString
                                fld1 = dv.Table.Rows(i)("Name").ToString
                                fld2 = "ID"
                                If IsCacheClassPersistant(tbl1, userconnstr, userconnprv) AndAlso IsCacheClassPersistant(tbl2, userconnstr, userconnprv) Then
                                    ret = AddJoin(repid, tbl1, tbl2, fld1, fld2, "INNERJOIN", repdb, "initial")
                                End If
                            End If
                        Catch ex As Exception
                            ret = ex.Message
                        End Try
                    Next
                End If
            ElseIf userconnprv = "System.Data.SqlClient" Then
                'create initial Joins from forein indexes
                Dim sqlrefcons As String = " Select * From INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS"
                Dim dvrc As DataView = mRecords(sqlrefcons, er, userconnstr, userconnprv)
                If dvrc Is Nothing OrElse dvrc.Count = 0 OrElse dvrc.Table.Rows.Count = 0 Then
                    Return ret
                End If
                Dim sqlcolusg As String = "Select * From INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE"
                Dim fk, pk As String
                er = ""
                Dim dvtc As DataView
                For i = 0 To dvrc.Table.Rows.Count - 1
                    fk = dvrc.Table.Rows(i)("CONSTRAINT_NAME")
                    pk = dvrc.Table.Rows(i)("UNIQUE_CONSTRAINT_NAME")
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

                    'check if join exists go next, if not to add
                    ret = AddJoin(repid, tbl1, tbl2, fld1, fld2, "INNERJOIN", repdb, "initial")
                Next
            ElseIf userconnprv = "Npgsql" Then  'PostgreSQL  Npgsql
                'create initial Joins from forein indexes
                Dim sqlkeycol As String = " Select * From INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA ='public' AND LOWER(TABLE_CATALOG) ='" & dbname.ToLower & "' AND CONSTRAINT_SCHEMA ='public' AND CONSTRAINT_CATALOG =='" & dbname & "' ORDER BY TABLE_NAME, ORDINAL_POSITION"
                er = ""
                Dim dvkc As DataView = mRecords(sqlkeycol, er, userconnstr, userconnprv)
                If dvkc Is Nothing OrElse dvkc.Count = 0 OrElse dvkc.Table.Rows.Count = 0 Then
                    Return ret
                End If
                For i = 0 To dvkc.Table.Rows.Count - 1
                    tbl1 = dvkc.Table.Rows(i)("TABLE_NAME")
                    fld1 = dvkc.Table.Rows(i)("COLUMN_NAME")

                    'TODO ASAP for PostgreSQL Npgsql for forein indexes !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    'tbl2 = dvkc.Table.Rows(i)("REFERENCED_TABLE_NAME")
                    'fld2 = dvkc.Table.Rows(i)("REFERENCED_COLUMN_NAME")

                    'check if join exists go next, if not - to add
                    If fld2.Trim <> "" Then ret = AddJoin(repid, tbl1, tbl2, fld1, fld2, "INNERJOIN", repdb, "initial")
                Next
            ElseIf userconnprv = "System.Data.Odbc" Then
                'not in use for ODBC and for OleDb for now - does not work properly !! 
                Dim dtx As DataTable
                Dim dtx2 As DataTable
                Dim dv2 As DataView
                Dim indxname As String = String.Empty
                Dim spnames(2) As String
                Dim k, l As Integer
                Dim dtt As DataTable
                Dim dvt As DataView = GetListOfUserTables(False, userconnstr, userconnprv)
                dtt = dvt.ToTable
                If dtt IsNot Nothing AndAlso dtt.Rows.Count > 0 Then
                    For i = 0 To dtt.Rows.Count - 1
                        Try
                            tbl1 = dtt.Rows(i)("TABLE_NAME")
                            spnames(2) = tbl1
                            dtx = GetListByODBC(System.Data.Odbc.OdbcMetaDataCollectionNames.Indexes, userconnstr, userconnprv, spnames)
                            dv = dtx.DefaultView
                            dtx = dv.ToTable
                            If dtx.Rows.Count > 0 Then
                                For j = 0 To dtx.Rows.Count - 1
                                    fld1 = dtx.Rows(j)("COLUMN_NAME")
                                    'indxname = dtx.Rows(j)("INDEX_NAME")
                                    For k = 0 To dtt.Rows.Count - 1
                                        If k = i Then Continue For
                                        tbl2 = dtt.Rows(k)("TABLE_NAME")
                                        spnames(2) = tbl2
                                        dtx2 = GetListByODBC(System.Data.Odbc.OdbcMetaDataCollectionNames.Indexes, userconnstr, userconnprv, spnames)
                                        dv2 = dtx2.DefaultView
                                        dtx2 = dv2.ToTable
                                        For l = 0 To dtx2.Rows.Count - 1
                                            If dtx.Rows(j)("INDEX_NAME") = dtx2.Rows(l)("INDEX_NAME") Then
                                                fld2 = dtx2.Rows(l)("COLUMN_NAME")
                                                ret = AddJoin(repid, tbl1, tbl2, fld1, fld2, "INNERJOIN", repdb, "initial")
                                            End If
                                        Next
                                    Next
                                Next
                            End If
                        Catch ex As Exception
                            ret = ex.Message
                        End Try
                    Next
                End If
            ElseIf userconnprv = "MySql.Data.MySqlClient" Then
                'create initial Joins from forein indexes
                Dim sqlkeycol As String = " Select * From INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA='" & dbname & "' AND REFERENCED_TABLE_SCHEMA='" & dbname & "' ORDER BY TABLE_NAME, REFERENCED_TABLE_NAME, ORDINAL_POSITION"
                er = ""
                Dim dvkc As DataView = mRecords(sqlkeycol, er, userconnstr, userconnprv)
                If dvkc Is Nothing OrElse dvkc.Count = 0 OrElse dvkc.Table.Rows.Count = 0 Then
                    Return ret
                End If
                For i = 0 To dvkc.Table.Rows.Count - 1
                    tbl1 = dvkc.Table.Rows(i)("TABLE_NAME")
                    fld1 = dvkc.Table.Rows(i)("COLUMN_NAME")
                    tbl2 = dvkc.Table.Rows(i)("REFERENCED_TABLE_NAME")
                    fld2 = dvkc.Table.Rows(i)("REFERENCED_COLUMN_NAME")

                    'check if join exists go next, if not - to add
                    ret = AddJoin(repid, tbl1, tbl2, fld1, fld2, "INNERJOIN", repdb, "initial")
                Next
            ElseIf userconnprv = "Oracle.ManagedDataAccess.Client" Then
                'for Oracle.ManagedDataAccess.Client use r-constraints
                er = ""
                Dim sqlconsts As String = "select * from user_constraints WHERE constraint_type='R' order by table_name "
                Dim dvrc As DataView = mRecords(sqlconsts, er, userconnstr, userconnprv)
                If dvrc Is Nothing OrElse dvrc.Count = 0 OrElse dvrc.Table.Rows.Count = 0 Then
                    Return ret
                End If
                Dim sqlcolusg As String = "select * from user_cons_columns "
                Dim fk, pk As String
                er = ""
                Dim dvtc As DataView
                For i = 0 To dvrc.Table.Rows.Count - 1
                    'tbl = dvrc.Table.Rows(i)("TABLE_NAME")
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

                    'check if join exists go next, if not to add
                    ret = AddJoin(repid, tbl1, tbl2, fld1, fld2, "INNERJOIN", repdb, "initial")
                Next
            End If

            'create report with repid like dbname & "_joins" to show all joins existing for repdb
            msql = "SELECT * FROM OURReportInfo WHERE ReportId = '" & repid & "' And ReportDB Like '%" & ourrepdb & "%' And ReportName Like '%" & repdb & "%'"
            If Not HasRecords(msql) Then  'initial joins' report does not exist with repid like dbname & "_joins" 
                'create report on ourdb (!) with repid like dbname & "_joins" to show all joins existing for userrepdb, or if already exists add permissions
                msql = "SELECT * FROM OURReportSQLquery WHERE (ReportId=""" & repid & """ AND Doing=""JOIN"")"
                ret = MakeNewStanardReport(logon, repid, "Joins from relationships", ourrepdb, msql, dbname, "support@yanbor.com", DateToString(CDate(DateAndTime.DateAdd(DateInterval.Day, 3000, Now()))), userconnstr, myconnprv, er, False)
            End If

        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function

    Public Function MakeInitialReports(ByVal logon As String, ByVal useremail As String, ByVal userenddate As String, Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "", Optional er As String = "", Optional redo As Boolean = False, Optional systables As Boolean = False) As String
        Dim ret As String = String.Empty
        Dim n As Integer = 0
        Dim m As Integer = 0
        Dim i As Integer = 0
        Dim csvuser As String = String.Empty
        Dim msql As String = String.Empty
        Try
            If userconnstr.Trim = "" Then
                userconnstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
                userconnprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            End If
            Dim repdb As String = userconnstr
            If userconnstr.ToUpper.IndexOf("USER ID") > 0 AndAlso userconnprv <> "Oracle.ManagedDataAccess.Client" Then
                repdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("USER ID")).Trim
            ElseIf userconnstr.ToUpper.IndexOf("PASSWORD") > 0 AndAlso userconnprv = "Oracle.ManagedDataAccess.Client" Then
                repdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("PASSWORD")).Trim
            End If
            If userconnstr.IndexOf("UID") > 0 Then repdb = userconnstr.Substring(0, userconnstr.IndexOf("UID")).Trim
            Dim dbname As String = GetDataBase(userconnstr, userconnprv).Replace(" ", "").Replace("#", "")
            If System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing AndAlso dbname = GetDataBase(System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString, System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ProviderName.ToString) Then
                csvuser = "yes"
            End If
            If Not redo AndAlso csvuser = "yes" Then
                'fixing existing initial reports
                msql = "SELECT * FROM OURReportInfo WHERE ReportId LIKE '" & dbname.Replace(" ", "").Replace("#", "") & "_INIT_%' AND ReportDB LIKE '%" & repdb.Trim.Replace(" ", "%") & "%'"
                Dim dti As DataView = mRecords(msql, ret)
                If dti Is Nothing OrElse dti.Table Is Nothing OrElse dti.Table.Rows.Count = 0 Then
                    Return "No initial reports created for OURcsv"
                    Exit Function
                End If
                For i = 0 To dti.Table.Rows.Count - 1
                    ret = UpdateXSDandRDL(dti.Table.Rows(i)("ReportId"), userconnstr, userconnprv)
                    ret = CreateCleanReportColumnsFieldsItems(dti.Table.Rows(i)("ReportId"), dbname, userconnstr, userconnprv)
                Next
                Return "Initial reports updated"
                Exit Function
            End If
            ''inside GetDataBase already
            'If userconnprv = "Oracle.ManagedDataAccess.Client" Then _
            '    dbname = Piece(dbname, "/", 2).Replace(";", "")
            Dim repid As String = String.Empty

            If redo Then

                'Get list of user reports
                Dim dv1 As DataView = Nothing
                dv1 = GetListOfUserReports(logon, repdb, ret)

                If dv1 Is Nothing OrElse dv1.Count = 0 OrElse dv1.Table.Rows.Count = 0 Then
                    ret = "No user reports found"
                Else
                    For i = 0 To dv1.Table.Rows.Count - 1
                        If dv1.Table.Rows(i)("ReportId").ToString.Contains(dbname.Replace("#", "").Replace(" ", "") & "_INIT_") Then
                            ret = DeleteReport(dv1.Table.Rows(i)("ReportId"))
                        End If
                    Next
                End If
                'delete all user initial reports
                ''delete Join Reports
                'msql = "DELETE FROM OURReportInfo WHERE ReportId Like '" & dbname.Replace(" ", "") & "_INIT_%' AND ReportDB LIKE '%" & repdb & "%'"
                'ret = ExequteSQLquery(msql)
                ''delete Join Permissions
                'msql = "DELETE FROM OURPermissions WHERE Param1 LIKE '" & dbname.Replace(" ", "") & "_INIT_%' AND NetId='" & logon & "' "
                'ret = ExequteSQLquery(msql)
                ''delete sql query fields
                'msql = "DELETE FROM OURReportSQLquery WHERE ReportId LIKE '" & dbname.Replace(" ", "") & "_INIT_%'  AND (Param1 LIKE '%" & repdb & "%') "
                'ret = ExequteSQLquery(msql)
            End If

            'create initial reports
            Dim dvp As DataView
            If systables Then
                'all persistent classes
                dvp = GetListOfUserTables(True, userconnstr, userconnprv, ret, logon, csvuser) 'password is needed!!!
            Else
                dvp = GetListOfUserTables(False, userconnstr, userconnprv, ret, logon, csvuser) 'password is needed!!! GetListOfUserTables(False, userconstr, userconprv, err)
            End If

            If Not dvp Is Nothing AndAlso Not dvp.Table Is Nothing AndAlso dvp.Table.Rows.Count > 0 AndAlso er = "" Then
                m = dvp.Table.Rows.Count
                For i = 0 To m - 1
                    Try
                        'make report for each table
                        Dim tbl As String = dvp.Table.Rows(i)("TABLE_NAME")

                        repid = dbname.Replace(" ", "").Replace("#", "") & "_INIT_" & i.ToString & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
                        repid = repid.Replace(" ", "")
                        Dim sqltxt As String = String.Empty
                        If userconnprv.StartsWith("InterSystems.Data.") Then
                            sqltxt = "SELECT * FROM " & FixReservedWords(CorrectTableNameWithDots(tbl, userconnprv), userconnprv)

                        Else
                            'TODO Is it needed to do CorrectTableNameWithDots(tbl) for SQL Server and MySql?
                            sqltxt = "SELECT * FROM " & FixReservedWords(CorrectTableNameWithDots(tbl, userconnprv), userconnprv)
                        End If

                        ret = MakeNewStanardReport(logon, repid, tbl, repdb, sqltxt, dbname, useremail, userenddate, userconnstr, userconnprv, er)

                        If ret.Contains("ERROR!!") Then
                            WriteToAccessLog(logon, "ERROR!!  Initial Report created with errors: " & ret, 6)
                        Else
                            'WriteToAccessLog(logon, "Initial Report created:" & repid, 6)
                            n = n + 1
                        End If

                        'moved inside MakeNewStanardReport:
                        'ret = ret & " Analytics: " & MakeInitialReportAnalytics(repid, sqltxt, userconnstr, userconnprv, er) 
                    Catch ex As Exception
                        ret = ret & "  ERROR!! creating new initial reports: " & ret & " " & ex.Message
                        WriteToAccessLog(logon, ret, 6)
                    End Try
                Next
                WriteToAccessLog(logon, "Initial Reports created for " & n.ToString & " tables from total " & m.ToString & " tables.", 6)

                If Not userconnstr.ToUpper.StartsWith("DSN") Then
                    ret = ret & " Make initial reports with Joins: " & MakeInitialReportsWithJoins(logon, dbname, useremail, userenddate, userconnstr, userconnprv, er, redo, systables)
                End If
                WriteToAccessLog(logon, ret, 6)

            Else
                ret = "ERROR!! connecting to database (check User ID and Password in the connection string): " & ret & " " & er
                WriteToAccessLog(logon, ret, 6)
            End If
        Catch ex As Exception
            ret = "ERROR!! creating new reports: " & ret & " " & ex.Message
            WriteToAccessLog(logon, ret, 6)
        End Try
        Return ret
    End Function
    Public Function MakeUserInitialReports(ByVal logon As String, ByVal useremail As String, ByVal userenddate As String, Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "", Optional er As String = "") As String
        'NOT IN USE for now
        Dim ret As String = String.Empty
        Dim n As Integer = 0
        Dim m As Integer = 0
        Dim i As Integer = 0
        Dim csvuser As String = String.Empty
        Try
            If userconnstr.Trim = "" Then
                userconnstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
                userconnprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            End If
            If IsCSVuser(userconnstr, userconnprv, er) = "yes" Then
                'If csvuser = "yes" Then
                Return "No initial reports created for OURcsv"
                Exit Function
            End If
            'Dim repdb As String = userconnstr.Substring(0, userconnstr.IndexOf("User ID")).Trim
            Dim repdb As String = userconnstr
            'If userconnstr.ToUpper.IndexOf("USER ID") > 0 Then repdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("USER ID")).Trim
            If userconnstr.ToUpper.IndexOf("USER ID") > 0 AndAlso userconnprv <> "Oracle.ManagedDataAccess.Client" Then
                repdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("USER ID")).Trim
            ElseIf userconnstr.ToUpper.IndexOf("PASSWORD") > 0 AndAlso userconnprv = "Oracle.ManagedDataAccess.Client" Then
                repdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("PASSWORD")).Trim
            End If
            If userconnstr.IndexOf("UID") > 0 Then repdb = userconnstr.Substring(0, userconnstr.IndexOf("UID")).Trim
            Dim dbname As String = GetDataBase(userconnstr, userconnprv)   'repdb.Substring(repdb.LastIndexOf("=")).Replace("=", "").Replace(";", "").Trim
            'If userconnprv.StartsWith("InterSystems.Data") AndAlso dbname = GetNamespaceFromConnectionString(System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString) Then
            '    'dbname=OURcsv - Cache
            '    csvuser = "yes"
            'ElseIf dbname = GetDatabaseFromConnectionString(System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString) Then
            '    'dbname=OURcsv - MySql or Sql server
            '    csvuser = "yes"
            'End If

            Dim dtu As DataTable = mRecords("SELECT * FROM OURPermits WHERE NetId='" & logon & "' AND  ConnStr LIKE '%" & repdb.Trim.Replace(" ", "%") & "%'").Table
            If dtu Is Nothing OrElse dtu.Rows.Count = 0 Then
                Return ret
            End If
            Dim logonindx As String = dtu.Rows(0)("Indx")
            If userconnprv = "Oracle.ManagedDataAccess.Client" Then
                dbname = Piece(dbname, "/", 2).Replace(";", "")
            End If
            Dim repid As String = String.Empty
            Dim sqltxt As String = String.Empty
            'get list of all user reports if they exist
            If Not HasRecords("SELECT * FROM OURPermissions INNER JOIN OURReportInfo ON (OURPermissions.Param1=OURReportInfo.ReportId) WHERE ReportDB LIKE '%" & repdb.Trim.Replace(" ", "%") & "%' And OURPermissions.NetId='" & logon & "'") Then
                Dim dvp As DataView = GetListOfUserTables(False, userconnstr, userconnprv) 'password is needed!!!
                If Not dvp Is Nothing AndAlso Not dvp.Table Is Nothing AndAlso dvp.Table.Rows.Count > 0 AndAlso er = "" Then
                    'fill out provider dropdown
                    m = dvp.Table.Rows.Count
                    For i = 0 To m - 1
                        'make report for each table
                        Dim tbl As String = dvp.Table.Rows(i)("TABLE_NAME")
                        repid = logon & logonindx & "_" & dbname & "_INIT_" & i.ToString & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
                        If userconnprv.StartsWith("InterSystems.Data.") Then
                            sqltxt = "SELECT * FROM " & FixReservedWords(CorrectTableNameWithDots(tbl, userconnprv), userconnprv)
                        Else
                            'TODO Is it needed to do CorrectTableNameWithDots(tbl) for SQL Server and MySql?
                            sqltxt = "SELECT * FROM " & FixReservedWords(CorrectTableNameWithDots(tbl, userconnprv), userconnprv)
                        End If
                        Dim sqlq As String = "INSERT INTO OURReportInfo (ReportID, ReportName,ReportTtl,ReportType,ReportAttributes,SQLquerytext,Param7type,Param9type,ReportDB) VALUES ('" & repid & "','" & tbl & "','" & tbl & "','rdl','sql','" & sqltxt & "','standard','landscape','" & repdb & "')"
                        er = ExequteSQLquery(sqlq)
                        If er = "Query executed fine." Then
                            n = n + 1
                            ret = ret & " Report " & repid & " for table " & tbl & " created. <br>"
                        Else
                            ret = ret & " Report " & repid & "  for table " & tbl & " crashed. <br>"
                        End If
                        'add permissions
                        sqlq = "INSERT INTO OURPermissions (NetID,Application,Param1,Param2,Param3,AccessLevel,OpenFrom,OpenTo,COMMENTS) VALUES('" & logon & "','InteractiveReporting','" & repid & "','" & tbl & "','" & useremail & "','admin','" & DateToString(Now) & "','" & userenddate & "','initial')"
                        Dim retr As String = ExequteSQLquery(sqlq)
                        If retr = "Query executed fine." Then
                            ret = ret & " Permissions for report " & repid & " created. <br>"
                        Else
                            ret = ret & " Permissions for report " & repid & " crashed. <br>"
                        End If

                        'make xsd and rdl
                        ret = ret & UpdateXSDandRDL(repid, userconnstr, userconnprv)
                        ret = ret & ", " & CreateCleanReportColumnsFieldsItems(repid, dbname, userconnstr, userconnprv)
                        ret = ret & ", Analytics: " & Analytics(repid, sqltxt, userconnstr, userconnprv, er)
                        If Not userconnstr.ToUpper.StartsWith("DSN") Then
                            ret = ret & " Make initial reports with Joins: " & MakeInitialReportsWithJoins(logon, dbname, useremail, userenddate, userconnstr, userconnprv, er)
                        End If
                    Next
                    ret = ret & " - " & n.ToString & " new reports created from total " & m.ToString & " tables."
                    WriteToAccessLog(logon, "Initial Reports created for " & n.ToString & " tables from total " & m.ToString & " tables.", 6)
                    'ret = ret & " Joins: " & MakeInitialJoins(logon, dbname, useremail, userenddate, userconnstr, userconnprv, er) 'already created by admin
                Else
                    ret = "ERROR!! connecting to database (check User ID and Password in the connection string): " & ret & " " & er
                    WriteToAccessLog(logon, ret, 6)
                End If
            End If
            Return ret
        Catch ex As Exception
            ret = "ERROR!! creating new reports: " & ret & " " & ex.Message
        End Try
        Return ret
    End Function
    Public Function GetListOfUserReports(ByVal logon As String, ByVal userdb As String, Optional ByRef er As String = "", Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "") As DataView
        'Get list of user reports 
        If userconnstr.Trim = "" Then
            userconnstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            userconnprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        End If
        Dim msql As String = "SELECT DISTINCT OURReportInfo.ReportId, OURReportInfo.SQLquerytext, OURReportInfo.ReportDB FROM OURReportInfo INNER JOIN OURPermissions  ON (OURPermissions.Param1 = OURReportInfo.ReportId) WHERE NetId='" & logon & "' AND ReportDB LIKE '%" & userdb.Trim.Replace(" ", "%") & "%'"
        Dim dv1 As DataView = mRecords(msql, er)
        Return dv1
    End Function
    Public Function Analytics(ByVal rep As String, ByVal mySQL As String, Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "", Optional ByRef er As String = "", Optional tbl As String = "", Optional redo As Boolean = False) As String
        Dim ret As String = String.Empty
        Dim ddtv As DataView = mRecords("SELECT Tbl1,Tbl1Fld1,Tbl2,Tbl2Fld2,Indx FROM OURReportSQLquery WHERE ReportId='" & rep & "' AND Doing='GROUP BY' ORDER BY Tbl1Fld1,Tbl2Fld2")
        If Not ddtv Is Nothing AndAlso ddtv.Count > 0 AndAlso ddtv.Table.Rows.Count > 0 Then
            If redo Then
                ret = ExequteSQLquery("DELETE FROM OURReportSQLquery WHERE ReportId='" & rep & "' AND Doing='GROUP BY' AND comments='initial'")
            Else
                Return ret
            End If
        End If

        Dim dv3 As DataView = mRecords(mySQL, er, userconnstr, userconnprv)
        If dv3 Is Nothing OrElse dv3.Count = 0 OrElse dv3.Table.Rows.Count = 0 Then
            Return ret
        End If

        Dim dtt As New DataTable
        dtt = dv3.ToTable

        'Dim er As String = String.Empty
        Dim i As Integer
        Dim j As Integer
        Dim k As Integer = 0
        Dim dtv As DataView = Nothing
        Dim dtb As DataTable = CreateStatsAnalyticsTable()
        Try
            For i = 0 To dv3.Table.Columns.Count - 1
                If dv3.Table.Columns(i).DataType.Name = "String" OrElse dv3.Table.Columns(i).DataType.Name = "DateTime" Then
                    If dv3.Table.Columns(i).Caption <> "ID" AndAlso dv3.Table.Columns(i).Caption <> "Indx" Then
                        Dim Row As DataRow = dtb.NewRow()
                        Row("Field") = dv3.Table.Columns(i).Caption
                        dtb.Rows.Add(Row)
                    End If
                End If
                k = k + 1
            Next
            If k = 0 Then
                Return "No groups identified."
            End If
            Dim fldnames(2) As String
            fldnames(0) = "Field"
            fldnames(1) = "Count"
            fldnames(2) = "Count Distinct"
            dtb = dtb.DefaultView.ToTable(1, fldnames)

            'calc count and distinct count
            dtb = CalcStatsAnalytics(rep, dtt, dtb, er)
            dtb = dtb.DefaultView.ToTable(1, fldnames)

            'calc analytics
            Dim dta As DataTable = CreateAnalyticsTable()
            For i = 0 To dtb.Rows.Count - 1
                For j = 0 To dtb.Rows.Count - 1
                    If j >= i AndAlso dtb.Rows(i)("Field") <> dtb.Rows(j)("Field") AndAlso CInt(dtb.Rows(i)("Count Distinct")) < 0.75 * CInt(dtb.Rows(i)("Count")) AndAlso CInt(dtb.Rows(j)("Count Distinct")) < 0.75 * CInt(dtb.Rows(j)("Count")) Then
                        'potential group
                        Dim Row As DataRow = dta.NewRow()
                        Row("Category1") = dtb.Rows(i)("Field")
                        Row("Category2") = dtb.Rows(j)("Field")
                        Row("Count1") = CInt(dtb.Rows(i)("Count"))
                        Row("Count2") = CInt(dtb.Rows(j)("Count"))
                        Row("CountDistinct1") = CInt(dtb.Rows(i)("Count Distinct"))
                        Row("CountDistinct2") = CInt(dtb.Rows(j)("Count Distinct"))
                        dta.Rows.Add(Row)
                    End If
                Next
            Next
            'update analytics
            Dim sqlst As String = String.Empty
            For i = 0 To dta.Rows.Count - 1
                If Not HasRecords("SELECT * FROM OURReportSQLquery WHERE ReportId='" & rep & "' AND Doing='GROUP BY' AND Tbl1Fld1='" & dta.Rows(i)("Category1") & "'  AND Tbl2Fld2='" & dta.Rows(i)("Category2") & "' ") Then
                    'add record to OURReportSQLquery
                    Dim tbl1 As String = FindTableToTheField(rep, dta.Rows(i)("Category1"), userconnstr, userconnprv, er)
                    Dim tbl2 As String = FindTableToTheField(rep, dta.Rows(i)("Category2"), userconnstr, userconnprv, er)
                    'sqlst = "INSERT INTO OURReportSQLquery SET ReportId='" & rep & "',Doing='GROUP BY',comments='initial',Tbl1Fld1='" & dta.Rows(i)("Category1") & "',Tbl2Fld2='" & dta.Rows(i)("Category2") & "',Tbl1='" & tbl1 & "',Tbl2='" & tbl2 & "'"
                    sqlst = "INSERT INTO OURReportSQLquery (ReportId,Doing,comments,Tbl1Fld1,Tbl2Fld2,Tbl1,Tbl2) "
                    sqlst &= "VALUES('" & rep & "','GROUP BY','initial','" & dta.Rows(i)("Category1") & "','" & dta.Rows(i)("Category2") & "','" & tbl1 & "','" & tbl2 & "')"
                    ret = ExequteSQLquery(sqlst)
                End If
            Next
            Return ret
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Public Function AddGroupBy(ByVal rep As String, ByVal cat1 As String, ByVal cat2 As String, ByVal groupbytype As String, Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "", Optional er As String = "") As String
        If cat1.Trim = "" Then
            Return ""
        End If
        'if cat1=cat2 it will add custom group by with tbl1=tbl2 and fld1=fld2
        Dim ret As String = String.Empty
        Dim sqlst As String = String.Empty
        Try
            Dim tbl1 As String = FindTableToTheField(rep, cat1, userconnstr, userconnprv, er)
            Dim tbl2 As String = FindTableToTheField(rep, cat2, userconnstr, userconnprv, er)
            'find fields names
            Dim tbl1fld1 As String = cat1
            If cat1.LastIndexOf(".") > 0 Then tbl1fld1 = cat1.Substring(cat1.LastIndexOf(".")).Replace(".", "")
            Dim tbl2fld2 As String = cat2
            If cat2.LastIndexOf(".") > 0 Then tbl2fld2 = cat2.Substring(cat2.LastIndexOf(".")).Replace(".", "")

            'check if group by already exists
            If IsCacheDatabase() Then
                sqlst = "SELECT * FROM OURReportSQLquery WHERE (ReportId = '" & rep & "' AND DOING = 'GROUP BY' AND UCASE(Tbl1)='" & UCase(tbl1) & "' AND UCASE(Tbl2)='" & UCase(tbl2) & "' AND UCASE(Tbl1Fld1)='" & UCase(tbl1fld1) & "' AND UCASE(Tbl2Fld2)='" & UCase(tbl2fld2) & "' ) "
            Else
                sqlst = "SELECT * FROM OURReportSQLquery WHERE (ReportId = '" & rep & "' AND DOING = 'GROUP BY' AND Tbl1='" & tbl1 & "' AND Tbl2='" & tbl2 & "' AND Tbl1Fld1='" & tbl1fld1 & "' AND Tbl2Fld2='" & tbl2fld2 & "' ) "
            End If
            If Not HasRecords(sqlst) Then
                'check if group by reversed order exist
                If IsCacheDatabase() Then
                    sqlst = "SELECT * FROM OURReportSQLquery WHERE (ReportId = '" & rep & "' AND DOING = 'GROUP BY' AND UCASE(Tbl1)='" & UCase(tbl2) & "' AND UCASE(Tbl2)='" & UCase(tbl1) & "' AND UCASE(Tbl1Fld1)='" & UCase(tbl2fld2) & "' AND UCASE(Tbl2Fld2)='" & UCase(tbl1fld1) & "' ) "
                Else
                    sqlst = "SELECT * FROM OURReportSQLquery WHERE (ReportId = '" & rep & "' AND DOING = 'GROUP BY' AND Tbl1='" & tbl2 & "' AND Tbl2='" & tbl1 & "' AND Tbl1Fld1='" & tbl2fld2 & "' AND Tbl2Fld2='" & tbl1fld1 & "' ) "
                End If
                If Not HasRecords(sqlst) Then
                    'insert group by
                    sqlst = "INSERT INTO OURReportSQLquery (ReportID, Doing, Tbl1, Tbl1Fld1, Tbl2, Tbl2Fld2, comments) VALUES('" & rep & "','GROUP BY','" & tbl1 & "','" & tbl1fld1 & "','" & tbl2 & "','" & tbl2fld2 & "','" & groupbytype & "')"
                    ret = ExequteSQLquery(sqlst)
                End If
            End If

        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    'Public Function MakeInitialReportAnalytics(ByVal rep As String, ByVal mySQL As String, Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "", Optional er As String = "", Optional tbl As String = "", Optional redo As Boolean = False) As String
    '    'NOT IN USE, replaced with function Analytics 

    '    Dim ret As String = String.Empty
    '    Dim dv3 As DataView = mRecords(mySQL, er, userconnstr, userconnprv)
    '    If dv3 Is Nothing OrElse dv3.Count = 0 OrElse dv3.Table.Rows.Count = 0 Then
    '        Return ret
    '    End If

    '    Dim i As Integer
    '    Dim j As Integer
    '    Dim k As Integer = 0
    '    Dim dtb As DataTable = CreateStatsAnalyticsTable()
    '    Try
    '        If mySQL.ToUpper.Contains(" JOIN ") Then
    '            'loop for tables ??

    '        Else
    '            For i = 0 To dv3.Table.Columns.Count - 1
    '                If dv3.Table.Columns(i).DataType.Name = "String" OrElse dv3.Table.Columns(i).DataType.Name = "DateTime" Then
    '                    If dv3.Table.Columns(i).Caption <> "ID" AndAlso dv3.Table.Columns(i).Caption <> "Indx" Then
    '                        'group field
    '                        Dim Row As DataRow = dtb.NewRow()
    '                        Row("Field") = dv3.Table.Columns(i).Caption
    '                        dtb.Rows.Add(Row)
    '                        k = k + 1
    '                    End If
    '                End If
    '            Next
    '            If k = 0 Then
    '                Return "No groups identified."
    '            End If
    '            dtb = CalcStatsAnalytics(rep, dv3.Table, dtb, er)
    '            Dim fldnames(2) As String
    '            fldnames(0) = "Field"
    '            fldnames(1) = "Count"
    '            fldnames(2) = "Count Distinct"
    '            dtb = dtb.DefaultView.ToTable(1, fldnames)
    '        End If

    '        'calc analytics
    '        Dim dta As DataTable = CreateAnalyticsTable()
    '        For i = 0 To dtb.Rows.Count - 1
    '            For j = 0 To dtb.Rows.Count - 1
    '                If j <= i AndAlso CInt(dtb.Rows(i)("Count Distinct")) < 0.75 * CInt(dtb.Rows(i)("Count")) AndAlso CInt(dtb.Rows(j)("Count Distinct")) < 0.75 * CInt(dtb.Rows(j)("Count")) Then
    '                    'potential group
    '                    Dim Row As DataRow = dta.NewRow()
    '                    Row("Category1") = dtb.Rows(i)("Field")
    '                    Row("Category2") = dtb.Rows(j)("Field")
    '                    Row("Count1") = CInt(dtb.Rows(i)("Count"))
    '                    Row("Count2") = CInt(dtb.Rows(j)("Count"))
    '                    Row("CountDistinct1") = CInt(dtb.Rows(i)("Count Distinct"))
    '                    Row("CountDistinct2") = CInt(dtb.Rows(j)("Count Distinct"))
    '                    dta.Rows.Add(Row)
    '                End If
    '            Next
    '        Next
    '        'update analytics
    '        Dim sqlst As String = String.Empty
    '        If redo Then
    '            ret = ExequteSQLquery("DELETE FROM OURReportSQLquery WHERE ReportId='" & rep & "' AND Doing='GROUP BY'")
    '        End If
    '        For i = 0 To dta.Rows.Count - 1
    '            If Not HasRecords("Select * FROM OURReportSQLquery WHERE ReportId='" & rep & "' AND Doing='GROUP BY' AND Tbl1Fld1='" & dta.Rows(i)("Category1") & "'  AND Tbl2Fld2='" & dta.Rows(i)("Category2") & "' ") Then
    '                'add record to OURReportSQLquery
    '                Dim tbl1 As String
    '                Dim tbl2 As String
    '                If tbl = "" Then
    '                    tbl1 = FindTableToTheField(rep, dta.Rows(i)("Category1"), userconnstr, userconnprv, er)
    '                    tbl2 = FindTableToTheField(rep, dta.Rows(i)("Category2"), userconnstr, userconnprv, er)
    '                Else
    '                    tbl1 = tbl
    '                    tbl2 = tbl
    '                End If
    '                'sqlst = "INSERT INTO OURReportSQLquery SET ReportId='" & rep & "',Doing='GROUP BY',Tbl1Fld1='" & dta.Rows(i)("Category1") & "',Tbl2Fld2='" & dta.Rows(i)("Category2") & "',Tbl1='" & tbl1 & "',Tbl2='" & tbl2 & "'"
    '                sqlst = "INSERT INTO OURReportSQLquery "
    '                sqlst &= "(ReportId,Doing,Tbl1Fld1,Tbl2Fld2,Tbl1,Tbl2) "
    '                sqlst &= "VALUES('" & rep & "',GROUP BY','" & dta.Rows(i)("Category1") & "','" & dta.Rows(i)("Category2") & "','" & tbl1 & "','" & tbl2 & "')"
    '                ret = ExequteSQLquery(sqlst)
    '            End If
    '        Next
    '        Return ret

    '    Catch ex As Exception
    '        ret = "ERROR!! " & ex.Message
    '    End Try
    '    Return ret
    'End Function
    'Public Function MakeInitialJoinsWithCustomAnddatadriven(ByVal logon As String, ByVal dbname As String, ByVal useremail As String, ByVal userenddate As String, Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "", Optional er As String = "", Optional redo As Boolean = False, Optional systables As Boolean = False) As String
    '    Dim ret As String = String.Empty
    '    Dim msql As String = String.Empty
    '    Dim sqlm As String = String.Empty
    '    Dim sqlt1t2 As String = String.Empty
    '    Dim n As Integer = 0
    '    Dim i As Integer = 0
    '    Dim j As Integer = 0
    '    Dim dv As DataView
    '    Dim tbl1 As String = String.Empty
    '    Dim fld1 As String = String.Empty
    '    Dim tbl2 As String = String.Empty
    '    Dim fld2 As String = String.Empty
    '    Dim rep As String = String.Empty
    '    Try
    '        Dim myconstring As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
    '        Dim myconnprv As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
    '        ' Dim ourrepdb As String = myconstring.Substring(0, myconstring.IndexOf("User ID")).Trim
    '        Dim ourrepdb As String = myconstring
    '        If ourrepdb.ToUpper.IndexOf("USER ID") > 0 Then ourrepdb = ourrepdb.Substring(0, userconnstr.ToUpper.IndexOf("USER ID")).Trim
    '        'If ourrepdb.IndexOf("UID") > 0 Then ourrepdb = ourrepdb.Substring(0, userconnstr.IndexOf("UID")).Trim
    '        Dim repdb As String = userconnstr
    '        If userconnstr.ToUpper.IndexOf("USER ID") > 0 Then repdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("USER ID")).Trim
    '        If userconnstr.IndexOf("UID") > 0 Then repdb = userconnstr.Substring(0, userconnstr.IndexOf("UID")).Trim

    '        Dim tmst As String = Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
    '        Dim repid As String = dbname & "_joins"

    '        'create initial joins
    '        ret = ret & " Joins: " & MakeInitialJoins(repid, dbname, userconnstr, userconnprv, er, redo, systables)

    '        'add custom joins to initial ones
    '        ret = ret & " Joins: " & AddCustomJoinsToInitialOnes(repid, dbname, userconnstr, userconnprv, er, redo)

    '        'datadriven joins added to initial ones when created
    '    Catch ex As Exception
    '        ret = ex.Message
    '    End Try
    '    Return ret
    'End Function
    Public Function MakeInitialReportsWithJoins(ByVal logon As String, ByVal dbname As String, ByVal useremail As String, ByVal userenddate As String, Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "", Optional er As String = "", Optional redo As Boolean = False, Optional systables As Boolean = False) As String
        Dim ret As String = String.Empty
        Dim msql As String = String.Empty
        Dim sqlm As String = String.Empty
        Dim sqlt1t2 As String = String.Empty
        Dim n As Integer = 0
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim dv As DataView
        Dim tbl1 As String = String.Empty
        Dim fld1 As String = String.Empty
        Dim tbl2 As String = String.Empty
        Dim fld2 As String = String.Empty
        Dim rep As String = String.Empty
        Try
            Dim myconstring As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            Dim myconnprv As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            ' Dim ourrepdb As String = myconstring.Substring(0, myconstring.IndexOf("User ID")).Trim
            Dim ourrepdb As String = myconstring
            If ourrepdb.ToUpper.IndexOf("USER ID") > 0 AndAlso myconnprv <> "Oracle.ManagedDataAccess.Client" Then
                ourrepdb = ourrepdb.Substring(0, myconstring.ToUpper.IndexOf("USER ID")).Trim
            ElseIf ourrepdb.ToUpper.IndexOf("PASSWORD") > 0 AndAlso myconnprv = "Oracle.ManagedDataAccess.Client" Then
                ourrepdb = ourrepdb.Substring(0, myconstring.ToUpper.IndexOf("PASSWORD")).Trim
            End If
            'If ourrepdb.IndexOf("UID") > 0 Then ourrepdb = ourrepdb.Substring(0, userconnstr.IndexOf("UID")).Trim
            Dim repdb As String = userconnstr
            If userconnstr.ToUpper.IndexOf("USER ID") > 0 AndAlso userconnprv <> "Oracle.ManagedDataAccess.Client" Then
                repdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("USER ID")).Trim
            ElseIf userconnstr.ToUpper.IndexOf("PASSWORD") > 0 AndAlso userconnprv = "Oracle.ManagedDataAccess.Client" Then
                repdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("PASSWORD")).Trim
            End If
            If userconnstr.IndexOf("UID") > 0 AndAlso userconnprv <> "Oracle.ManagedDataAccess.Client" Then
                repdb = userconnstr.Substring(0, userconnstr.IndexOf("UID")).Trim
            End If


            Dim tmst As String = Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
            Dim repid As String = dbname & "_joins"
            Dim csv As String = String.Empty
            If IsCSVuser(userconnstr, "", er) = "yes" Then
                csv = "yes"
            End If
            ''create initial joins
            dv = GetListOfUserTables(systables, userconnstr, userconnprv, ret, logon, csv)
            If dv Is Nothing OrElse dv.Table Is Nothing Then
                Return "ERROR!! There are no user tables found for this user."
            End If
            ret = ret & " Joins:  " & MakeInitialJoins(logon, dv.Table, repid, dbname, userconnstr, userconnprv, er, redo, systables)

            ''add custom joins to initial ones
            ret = ret & " Joins: " & AddCustomJoinsToInitialOnes(repid, dbname, userconnstr, userconnprv, er, redo)

            ''datadriven joins added to initial ones when created


            'make reports based on joins with reportid=dbname & "_" & tbl1 & "JOIN" & tbl2 and title tbl1 & " JOIN " & tbl2
            'get all initial joins in repdb
            msql = "SELECT * FROM OURReportSQLquery WHERE (ReportId Like '" & dbname & "_joins_" & "'  AND Doing='JOIN' AND (Param1 LIKE '%" & repdb & "%' OR Comments LIKE '%" & repdb.Trim.Replace(" ", "%") & "%'))  ORDER BY Tbl1, Tbl2, Tbl1Fld1, Tbl2Fld2"
            dv = mRecords(msql, er)  'all possible joins in repdb

            If dv Is Nothing OrElse dv.Count = 0 OrElse dv.Table.Rows.Count = 0 Then
                ret = " No initial join reports found."
                Return ret
            End If

            Dim m As Integer = 0
            Dim fld As String = String.Empty
            Dim tbl As String = String.Empty
            ' Dim ddf As DataTable

            For i = 0 To dv.Table.Rows.Count - 1
                If tbl1 <> dv.Table.Rows(i)("Tbl1") OrElse tbl2 <> dv.Table.Rows(i)("Tbl2") Then
                    'new report
                    tbl1 = dv.Table.Rows(i)("Tbl1")
                    tbl2 = dv.Table.Rows(i)("Tbl2")
                    fld1 = dv.Table.Rows(i)("Tbl1Fld1")
                    fld2 = dv.Table.Rows(i)("Tbl2Fld2")

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
                    'Else
                    '    'add join to the current report
                    '    'sqlm = "INSERT INTO OURReportSQLquery (ReportID, Doing, Tbl1, Tbl1Fld1, Tbl2, Tbl2Fld2, Comments) VALUES('" & rep & "','JOIN','" & tbl1 & "','" & fld1 & "','" & tbl2 & "','" & fld2 & "','" & repdb & "')"
                    '    'er = ExequteSQLquery(sqlm)
                    '    ret = AddJoin(rep, tbl1, tbl2, fld1, fld2, "INNERJOIN", repdb, "initial")
                    '    'Save report
                    '    'ret = UpdateJoins(rep, userconnstr, userconnprv, er)
                    '    Dim sqlquerytext As String = MakeSQLQueryFromDB(rep, userconnstr, userconnprv)
                    '    ret = SaveSQLquery(rep, sqlquerytext)
                    '    er = UpdateXSDandRDL(rep, userconnstr, userconnprv)
                End If
            Next


        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function MakeNewStanardReport(ByVal logon As String, ByVal repid As String, ByVal repttl As String, ByVal repdb As String, ByVal msql As String, ByVal dbname As String, ByVal useremail As String, ByVal userenddate As String, Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "", Optional er As String = "", Optional doanalytics As Boolean = True, Optional pgftr As String = "", Optional origin As String = "") As String
        Dim ret As String = String.Empty
        Dim sqlq As String = String.Empty
        Dim stdrtype As String = String.Empty
        If repid.Contains("_INIT_") Then
            stdrtype = ("_INIT_")
        ElseIf repid.Contains("_JOIN_") Then
            stdrtype = ("_JOIN_")
        ElseIf repid.StartsWith(dbname & "_joins") Then
            stdrtype = ("_joins")
        End If
        Dim userrepdb As String = userconnstr
        'If userconnstr.ToUpper.IndexOf("USER ID") > 0 Then userrepdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("USER ID")).Trim
        If userconnstr.ToUpper.IndexOf("USER ID") > 0 AndAlso userconnprv <> "Oracle.ManagedDataAccess.Client" Then
            userrepdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("USER ID")).Trim
        ElseIf userconnstr.ToUpper.IndexOf("PASSWORD") > 0 AndAlso userconnprv = "Oracle.ManagedDataAccess.Client" Then
            userrepdb = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("PASSWORD")).Trim
        End If

        If userconnstr.IndexOf("UID") > 0 Then userrepdb = userconnstr.Substring(0, userconnstr.IndexOf("UID")).Trim
        Try
            'check if report exists and if yes, than add permissions and exit
            If repid.StartsWith(dbname & "_joins") Then
                'dbname_joins report, in this case repdb is ourdb
                sqlq = "SELECT * FROM OURReportInfo WHERE ReportId LIKE '" & dbname.Replace(" ", "") & stdrtype & "%' And ReportTtl = '" & repttl & "' And ReportDB Like '%" & repdb & "%' And ReportName Like '%" & userrepdb & "%'"
            Else
                'INIT or JOIN
                sqlq = "SELECT * FROM OURReportInfo WHERE ReportId LIKE '" & dbname.Replace(" ", "") & stdrtype & "%' And ReportTtl = '" & repttl & "' And ReportDB Like '%" & repdb & "%'"
            End If
            Dim dv As DataView = mRecords(sqlq)
            If Not dv Is Nothing AndAlso dv.Count > 0 AndAlso dv.Table.Rows.Count > 0 Then
                repid = dv.Table.Rows(0)("ReportId").ToString
                'add permissions for logon to the report if needed 
                sqlq = "SELECT * FROM OURPermissions WHERE NetId='" & logon & "' AND Param1='" & repid & "'"
                If HasRecords(sqlq) Then
                    ret = ret & " Permissions for report " & repid & " existed. <br>"
                    Return ret
                End If
                'new permission required
                sqlq = "INSERT INTO OURPermissions (NetID,Application,Param1,Param2,Param3,AccessLevel,OpenFrom,OpenTo,COMMENTS) VALUES('" & logon & "','InteractiveReporting','" & repid & "','" & repttl & "','" & useremail & "','admin','" & DateToString(Now) & "','" & userenddate & "','" & origin & "')"
                Dim retr As String = ExequteSQLquery(sqlq)
                If retr = "Query executed fine." Then
                    ret = ret & " Permissions for report " & repid & " created. <br>"
                Else
                    ret = ret & " Permissions for report " & repid & " crashed. <br>"
                End If
                Return ret
                Exit Function
            End If
            'new report
            If repid.StartsWith(dbname & "_joins") Then
                sqlq = "INSERT INTO OURReportInfo (ReportID, ReportName,ReportTtl,ReportType,ReportAttributes,SQLquerytext,Param7type,Param9type,ReportDB,comments,Param4type) VALUES ('" & repid & "','" & userrepdb & "','" & repttl & "','rdl','sql','" & msql & "','standard','portrait','" & repdb & "','" & pgftr & "','" & origin & "')"
            Else
                sqlq = "INSERT INTO OURReportInfo (ReportID, ReportName,ReportTtl,ReportType,ReportAttributes,SQLquerytext,Param7type,Param9type,ReportDB,comments,Param4type) VALUES ('" & repid & "','" & repid & "','" & repttl & "','rdl','sql','" & msql & "','standard','portrait','" & repdb & "','" & pgftr & "','" & origin & "')"
            End If
            er = ExequteSQLquery(sqlq)
            If er = "Query executed fine." Then
                ret = ret & " Report " & repid & " created. <br>"
                'add permissions to the report "Joins from relationships" with repid like dbname & "_joins" if needed
                sqlq = "SELECT * FROM OURPermissions WHERE NetId='" & logon & "' AND Param1='" & repid & "'"
                If HasRecords(sqlq) Then
                    ret = ret & " Permissions for report " & repid & " existed. <br>"
                    Return ret
                End If
                sqlq = "INSERT INTO OURPermissions (NetID,Application,Param1,Param2,Param3,AccessLevel,OpenFrom,OpenTo,COMMENTS) VALUES('" & logon & "','InteractiveReporting','" & repid & "','" & repttl & "','" & useremail & "','admin','" & DateToString(Now) & "','" & userenddate & "','" & origin & "')"
                Dim retr As String = ExequteSQLquery(sqlq)
                If retr = "Query executed fine." Then
                    ret = ret & " Permissions for report " & repid & " created. <br>"
                Else
                    ret = ret & " Permissions for report " & repid & " crashed. <br>"
                End If
                'do not make RDL and XSD initially
                'ret = ret & UpdateXSDandRDL(repid, userconnstr, userconnprv)
                'ret = ret & ", " & CreateCleanReportColumnsFieldsItems(repid, dbname, userconnstr, userconnprv)
                ''WriteToAccessLog(logon, "Initial Report " & repid & "  created.", 6)

                If doanalytics Then
                    'calc analytics for new report
                    ret = ret & " Analytics: " & Analytics(repid, msql, userconnstr, userconnprv, er)
                End If

            Else
                ret = ret & " ERROR!! Initial Report " & repid & " has not been created: " & er & "<br>"
                'WriteToAccessLog(logon, "Initial Report " & repid & " has not been created.", 6)
                Return ret
            End If

        Catch ex As Exception
            ret = ret & " ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Public Function MakeNewUserReport(ByVal logon As String, ByVal repid As String, ByVal repttl As String, ByVal repdb As String, ByVal msql As String, ByVal dbname As String, ByVal useremail As String, ByVal userenddate As String, Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "", Optional er As String = "", Optional doanalytics As Boolean = False, Optional pgftr As String = "") As String
        Dim ret As String = String.Empty
        Dim sqlq As String
        Try
            'check if report exists in the same db with the same title and the same reportid and if yes, than add permissions and exit
            sqlq = "SELECT * FROM OURReportInfo WHERE ReportId ='" & repid & "' And (ReportTtl ='" & repttl & "' OR ReportTtl LIKE '%(" & repttl & ")%') And ReportDB Like '%" & repdb.Trim.Replace(" ", "%") & "%'"
            Dim dv As DataView = mRecords(sqlq)
            If Not dv Is Nothing AndAlso dv.Count > 0 AndAlso dv.Table.Rows.Count > 0 Then
                repid = dv.Table.Rows(0)("ReportId").ToString
                'add permissions for logon to the report if needed 
                sqlq = "SELECT * FROM OURPermissions WHERE NetId='" & logon & "' AND Param1='" & repid & "'"
                If HasRecords(sqlq) Then
                    ret = ret & " Permissions for report " & repid & " existed. <br>"
                    Return ret
                End If
                'new permission required
                sqlq = "INSERT INTO OURPermissions (NetID,Application,Param1,Param2,Param3,AccessLevel,OpenFrom,OpenTo,COMMENTS) VALUES('" & logon & "','InteractiveReporting','" & repid & "','" & repttl & "','" & useremail & "','admin','" & DateToString(Now) & "','" & userenddate & "','initial')"
                Dim retr As String = ExequteSQLquery(sqlq)
                If retr = "Query executed fine." Then
                    ret = ret & " Permissions for report " & repid & " created. <br>"
                Else
                    ret = ret & " Permissions for report " & repid & " crashed. <br>"
                End If
                Return ret
                Exit Function
            End If
            'check if report exists in the same db with another title and the same reportid and if yes, than add title and permissions and exit
            sqlq = "SELECT * FROM OURReportInfo WHERE ReportId ='" & repid & "'  And ReportDB Like '%" & repdb & "%'"
            dv = mRecords(sqlq)
            If Not dv Is Nothing AndAlso dv.Count > 0 AndAlso dv.Table.Rows.Count > 0 Then
                repid = dv.Table.Rows(0)("ReportId").ToString
                'update the title
                sqlq = "UPDATE ReportTtl FROM OURReportInfo SET ReportTtl='" & dv.Table.Rows(0)("ReportTtl").ToString & " (" & repttl & ")' WHERE ReportId ='" & repid & "%'  And ReportDB Like '%" & repdb & "%'"
                ret = ExequteSQLquery(sqlq)
                'add permissions for logon to the report if needed 
                sqlq = "SELECT * FROM OURPermissions WHERE NetId='" & logon & "' AND Param1='" & repid & "'"
                If HasRecords(sqlq) Then
                    ret = ret & " Permissions for report " & repid & " existed. <br>"
                    Return ret
                End If
                'new permission required
                sqlq = "INSERT INTO OURPermissions (NetID,Application,Param1,Param2,Param3,AccessLevel,OpenFrom,OpenTo,COMMENTS) VALUES('" & logon & "','InteractiveReporting','" & repid & "','" & repttl & "','" & useremail & "','admin','" & DateToString(Now) & "','" & userenddate & "','initial')"
                Dim retr As String = ExequteSQLquery(sqlq)
                If retr = "Query executed fine." Then
                    ret = ret & " Permissions for report " & repid & " created. <br>"
                Else
                    ret = ret & " Permissions for report " & repid & " crashed. <br>"
                End If
                Return ret
                Exit Function
            End If
            'new report
            sqlq = "INSERT INTO OURReportInfo (ReportID, ReportName,ReportTtl,ReportType,ReportAttributes,SQLquerytext,Param7type,Param9type,ReportDB,comments) VALUES ('" & repid & "','" & repid & "','" & repttl & "','rdl','sql','" & msql & "','user','portrait','" & repdb & "','" & pgftr & "')"
            er = ExequteSQLquery(sqlq)
            If er = "Query executed fine." Then
                ret = ret & " Report for Table " & repid & " created. <br>"
                sqlq = "INSERT INTO OURPermissions (NetID,Application,Param1,Param2,Param3,AccessLevel,OpenFrom,OpenTo,COMMENTS) VALUES('" & logon & "','InteractiveReporting','" & repid & "','" & repttl & "','" & useremail & "','admin','" & DateToString(Now) & "','" & userenddate & "','user')"
                Dim retr As String = ExequteSQLquery(sqlq)
                If retr = "Query executed fine." Then
                    ret = ret & " Permissions for report " & repid & " created. <br>"
                Else
                    ret = ret & " Permissions for report " & repid & " crashed. <br>"
                End If
                ret = ret & UpdateXSDandRDL(repid, userconnstr, userconnprv)
                ret = ret & ", " & CreateCleanReportColumnsFieldsItems(repid, dbname, userconnstr, userconnprv)
                'WriteToAccessLog(logon, "User Report " & repid & "  created.", 6)

                If doanalytics Then
                    'calc analytics for new report
                    ret = ret & " Analytics: " & Analytics(repid, msql, userconnstr, userconnprv, er)
                End If

            Else
                ret = ret & " ERROR!! User Report " & repid & " has not been created. <br>"
                'WriteToAccessLog(logon, "User Report " & repid & " has not been created.", 6)
                Return ret
            End If

        Catch ex As Exception
            ret = ret & " ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Public Function WriteToAccessLog(ByVal logon As String, ByVal actn As String, ByVal cnt As Integer) As String
        Dim ret As String = String.Empty
        actn = cleanText(actn)
        Dim ipaddr As String = GetIPAddress()
        If ipaddr.Trim <> "" AndAlso ipaddr <> "::1" Then logon = logon & " at " & ipaddr
        Try
            Dim myprovider As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            Dim EventDate As String
            If myprovider = "Oracle.ManagedDataAccess.Client" Then
                EventDate = DateToString(DateTime.Now, myprovider, True)
            Else
                EventDate = DateToString(DateTime.Now, "", False)
            End If

            If IsCacheDatabase() Then
                ret = ExequteSQLquery("INSERT INTO OURAccessLog (EventDate,Logon,Action,""Count"") VALUES('" & EventDate & "','" & logon & "','" & actn & "'," & cnt.ToString & ")")
                'ElseIf IsOracleDatabase() Then
                '    ret = ExequteSQLquery("INSERT INTO OURAccessLog (EventDate,LOGON,Action,[""Count""]) VALUES('" & DateToStringFormat(EventDate, "", "dd-MMM-yy") & "','" & logon & "','" & actn & "'," & cnt.ToString & ")")
            Else
                ret = ExequteSQLquery("INSERT INTO OURAccessLog (EventDate,Logon,Action,[Count]) VALUES('" & EventDate & "','" & logon & "','" & actn & "'," & cnt.ToString & ")")
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function WriteToAccessLogComment(ByVal logon As String, ByVal actn As String, ByVal cnt As Integer, comment As String) As String
        If comment = String.Empty Then WriteToAccessLog(logon, actn, cnt)

        Dim ret As String = String.Empty
        actn = cleanText(actn)
        Dim ipaddr As String = GetIPAddress()
        If ipaddr.Trim <> "" AndAlso ipaddr <> "::1" Then logon = logon & " at " & ipaddr
        Try
            Dim EventDate As String = DateToString(DateTime.Now, "", False)
            If IsCacheDatabase() Then
                ret = ExequteSQLquery("INSERT INTO OURAccessLog (EventDate,Logon,Action,""Count"",Comments) VALUES('" & EventDate & "','" & logon & "','" & actn & "'," & cnt.ToString & ",'" & comment & "')")
            Else
                ret = ExequteSQLquery("INSERT INTO OURAccessLog (EventDate,Logon,Action,[Count],Comments) VALUES('" & EventDate & "','" & logon & "','" & actn & "'," & cnt.ToString & ",'" & comment & "')")
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function

    Public Function ReportExpired(repid As String, logon As String) As Boolean
        Dim sSql As String = "SELECT * FROM OURPermissions Where NetId='" & logon & "' AND Param1='" & repid & "'"
        Dim dt As DataTable = mRecords(sSql).ToTable()

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Dim dr As DataRow = dt.Rows(0)
            Dim opento As String = IIf(Not IsDBNull(dr("OpenTo")), dr("OpenTo").ToString, DateToString(Now()))
            Return ServiceExpired(DateToString(opento))
        End If
        Return False
    End Function
    Public Function ServiceExpired(ByVal enddate As String, Optional ByRef err As String = "") As Boolean
        Dim ret As String = String.Empty
        Dim b As Boolean = False
        Try
            If enddate = "" Then
                b = False
                Return b
            End If
            'if enddate<curdate then b=true - expired
            If CDate(enddate) < CDate(Now()) Then
                b = True
            End If

            'If enddate < DateToString(Now()) Then
            '    b = True
            'End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        err = ret
        Return b
    End Function
    Public Function ProcessPayment(ByVal lgn As String, ByVal amnt As Integer) As String
        'process payment in OUR database
        Dim sqlst As String = String.Empty
        Dim ret As String = WriteToAccessLog(lgn, "Request to process payment", amnt)
        Dim retn As String = String.Empty
        Dim hasrec As DataView
        Dim paid As Integer
        Dim enddate As String = String.Empty
        Dim startdate As String = String.Empty
        Try
            'update OURPermits for user EndDate field - !!! Pay is only for OUR web site, not OnSite installation, and NetId is unique on OUR db.
            hasrec = mRecords("SELECT * FROM OURPERMITS WHERE (NetId='" & lgn & "') AND (Application='InteractiveReporting')")
            If hasrec Is Nothing OrElse hasrec.Table Is Nothing OrElse hasrec.Table.Rows.Count = 0 Then
                ret = "User " & lgn & " has not been found. Payment cannot be applied. Contact Support"
                Return ret
            Else
                If IsDBNull(hasrec.Table.Rows(0)("paid")) Then
                    paid = 0
                Else
                    paid = hasrec.Table.Rows(0)("paid")
                End If
                If IsDBNull(hasrec.Table.Rows(0)("StartDate")) OrElse hasrec.Table.Rows(0)("StartDate") = "0000-00-00 00:00:00" Then
                    'update StartDate
                    sqlst = "UPDATE OURPERMITS SET StartDate='" & DateToString(Now) & "' WHERE [Application]='InteractiveReporting' AND NetID='" & lgn & "'"
                    ret = ExequteSQLquery(sqlst)
                    If ret = "Query executed fine." Then
                        ret = ""
                    Else
                        retn = retn & ",  " & ret
                    End If
                End If
                If IsDBNull(hasrec.Table.Rows(0)("EndDate")) OrElse hasrec.Table.Rows(0)("EndDate") = "0000-00-00 00:00:00" Then
                    'update EndDate
                    enddate = DateToString(DateAndTime.DateAdd(DateInterval.Day, amnt, Now()))
                    sqlst = "UPDATE OURPERMITS SET EndDate='" & enddate & "' WHERE [Application]='InteractiveReporting' AND NetID='" & lgn & "'"
                    ret = ExequteSQLquery(sqlst)
                    If ret = "Query executed fine." Then
                        ret = ""
                    Else
                        retn = retn & ",  " & ret
                    End If
                Else
                    'update EndDate
                    enddate = hasrec.Table.Rows(0)("EndDate").ToString
                    enddate = DateToString(DateAndTime.DateAdd(DateInterval.Day, amnt, CDate(enddate)))
                    sqlst = "UPDATE OURPERMITS SET EndDate='" & enddate & "' WHERE [Application]='InteractiveReporting' AND NetID='" & lgn & "'"
                    ret = ExequteSQLquery(sqlst)
                    If ret = "Query executed fine." Then
                        ret = WriteToAccessLog(lgn, "Payment processed.", amnt)
                        ret = ""
                    Else
                        retn = retn & ",  " & ret
                    End If
                End If
                amnt = paid + amnt
                sqlst = "UPDATE OURPERMITS SET paid=" & amnt & " WHERE [Application]='InteractiveReporting' AND NetID='" & lgn & "'"
                ret = ExequteSQLquery(sqlst)
                If ret = "Query executed fine." Then
                    ret = ""
                Else
                    retn = retn & ",  " & ret
                End If
            End If

            'update OURPermissions for reports OpenTo field
            hasrec = mRecords("SELECT * FROM OURPermissions WHERE (NetId='" & lgn & "') AND (Application='InteractiveReporting') AND (AccessLevel='admin')")
            If hasrec Is Nothing OrElse hasrec.Table Is Nothing OrElse hasrec.Table.Rows.Count = 0 Then
                ret = "User " & lgn & " reports with admin permissions have not been found."
                Return ret
            Else
                For i = 0 To hasrec.Table.Rows.Count - 1
                    If IsDBNull(hasrec.Table.Rows(i)("OpenFrom")) OrElse hasrec.Table.Rows(i)("OpenFrom") = "0000-00-00 00:00:00" Then
                        'update null OpenFrom
                        sqlst = "UPDATE OURPermissions SET OpenFrom='" & DateToString(Now) & "' WHERE [Application]='InteractiveReporting' AND NetID='" & lgn & "' AND Indx=" & hasrec.Table.Rows(i)("Indx").ToString
                        ret = ExequteSQLquery(sqlst)
                        If ret = "Query executed fine." Then
                            ret = ""
                        Else
                            retn = retn & ",  " & ret
                        End If
                    End If

                    'update OpenTo
                    sqlst = "UPDATE OURPermissions SET OpenTo='" & enddate & "' WHERE [Application]='InteractiveReporting' AND NetID='" & lgn & "' AND Indx=" & hasrec.Table.Rows(i)("Indx").ToString
                    ret = ExequteSQLquery(sqlst)
                    If ret = "Query executed fine." Then
                        ret = WriteToAccessLog(lgn, "Payment processed.", amnt)
                        ret = ""
                    Else
                        retn = retn & ",  " & ret
                    End If
                    'End If
                Next
                If ret = "Query executed fine." Then
                    ret = ""
                Else
                    retn = retn & ",  " & ret
                End If
            End If

            If retn.Trim <> "" AndAlso retn.Replace(",", "").Trim <> "" Then
                WriteToAccessLog(lgn, "Payment processed with errors: " & retn, amnt)
            Else
                retn = "Payment processed"
            End If
        Catch ex As Exception
            WriteToAccessLog(lgn, "Payment processed with errors: " & ex.Message, amnt)
            retn = "Payment processed with errors: " & retn & "  " & ex.Message
        End Try
        Return retn
    End Function
    Public Function SearchTable(ByVal myTableName As String, ByVal mySearchString As String, ByVal myGroupName As String) As DataTable

        If IsDBNull(mySearchString) = True Then
            mySearchString = ""
            Return Nothing
        End If

        Dim myConnection As SqlConnection
        Dim myCommand As New SqlClient.SqlCommand
        Dim myAdapter As SqlClient.SqlDataAdapter
        Dim myRecords As DataTable

        Dim myconstring As String
        myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
        myConnection = New SqlConnection(myconstring)
        myCommand.Connection = myConnection
        myCommand.CommandType = CommandType.StoredProcedure

        myCommand.CommandText = "xp_Search_" & myTableName

        'parameters to store-procedure
        myCommand.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SearchString", System.Data.SqlDbType.NVarChar))
        myCommand.Parameters.Add(New System.Data.SqlClient.SqlParameter("@GroupName", System.Data.SqlDbType.NVarChar))
        myCommand.Parameters.Item(0).Value = mySearchString
        myCommand.Parameters.Item(1).Value = myGroupName

        If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
        myAdapter = New SqlClient.SqlDataAdapter(myCommand)
        myRecords = New DataTable
        myAdapter.Fill(myRecords)

        myAdapter.Dispose()
        myCommand.Connection.Close()
        myCommand.Dispose()

        Return myRecords
    End Function
    Public Function UserRole(ByVal logon As String, ByRef Optional err As String = "") As String
        Dim userrol As String = String.Empty
        Try
            Dim mrec As DataView
            mrec = mRecords("SELECT * FROM OURPERMITS WHERE (NetId='" & logon & "') AND (Application='InteractiveReporting')")
            If mrec Is Nothing OrElse mrec.Table Is Nothing OrElse mrec.Table.Rows.Count = 0 Then
                Return ""
            Else
                userrol = mrec.Table.Rows(0)("RoleApp").ToString
                Return userrol
            End If
        Catch ex As Exception
            err = ex.Message
            Return ""
        End Try
    End Function
    Public Function FixDatetimeInOURUnits() As String
        Dim ret As String = String.Empty
        'fix DateTime fields
        If Not IsCacheDatabase() Then
            ret = ExequteSQLquery("UPDATE OURUnits SET StartDate=NULL WHERE StartDate=0")
            ret = ret & ExequteSQLquery("UPDATE OURUnits SET EndDate=NULL WHERE EndDate=0")
        End If
        Return ret.Replace("Query executed fine.", "").Trim
    End Function
    Public Function FixDatetimeInOURPermissions() As String
        Dim ret As String = String.Empty
        'fix DateTime fields
        If Not IsCacheDatabase() Then
            ret = ExequteSQLquery("UPDATE OURPermissions SET OpenFrom=NULL WHERE OpenFrom='0'")
            ret = ret & ExequteSQLquery("UPDATE OURPermissions SET OpenTo=NULL WHERE OpenTo='0'")
        End If
        Return ret.Replace("Query executed fine.", "").Trim
    End Function
    Public Function FixDatetimeInOURPermits() As String
        Dim ret As String = String.Empty
        'fix DateTime fields
        If Not IsCacheDatabase() AndAlso Not IsOracleDatabase() AndAlso Not IsPostgreDatabase() Then
            ret = ExequteSQLquery("UPDATE OURPERMITS SET StartDate=NULL WHERE StartDate=0")
            ret = ret & ExequteSQLquery("UPDATE OURPERMITS SET EndDate=NULL WHERE EndDate=0")
        End If
        Return ret.Replace("Query executed fine.", "").Trim
    End Function

    Public Function CreateDbTableForCSV(ByRef tblname As String, ByVal dstr As String, ByRef cols() As String, ByVal coltypes() As String, ByVal collens() As String, ByVal userconnstr As String, ByVal userconnprv As String, Optional ByRef er As String = "") As String
        'create new dbtable with text fields
        Dim ret As String = String.Empty
        Dim i, n As Integer
        Dim sqlq As String = String.Empty
        Dim qt As String = """"
        Try
            'get db 
            Dim db As String
            db = GetDataBase(userconnstr, userconnprv).ToLower
            'End If
            n = cols.Length
            If userconnprv = "MySql.Data.MySqlClient" Then
                sqlq = "Select * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME='" & db & "'"
                If Not HasRecords(sqlq, userconnstr, userconnprv) Then
                    sqlq = "CREATE DATABASE `" & db & "`"
                    ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
                    If ret = "Query executed fine." Then
                        ret = ""
                    Else
                        ret = ret & "<br/> " & db & " not created: " & ret & " "
                        Return ret
                    End If
                End If
                'End If
                'create table
                'Dim cols() As String = csvstr.Split(dstr)
                'n = cols.Length
                'If userconnprv = "MySql.Data.MySqlClient" Then
                'create table tblname
                sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" & db & "' AND TABLE_NAME='" & tblname & "'"
                If Not HasRecords(sqlq, userconnstr, userconnprv) Then
                    sqlq = "CREATE TABLE `" & db & "`.`" & tblname.ToLower & "`( "
                    For i = 0 To n - 1
                        sqlq = sqlq & " `" & cols(i).Trim & "` TEXT NULL" ' DEFAULT NULL"
                        If i < n - 1 Then sqlq = sqlq & ","
                    Next
                    sqlq = sqlq & " );"
                    ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
                    If ret = "Query executed fine." Then
                        ret = tblname & " created. "
                    Else
                        ret = "ERROR!! " & tblname & " creation crashed: " & ret & " "
                    End If
                Else
                    ret = tblname & " already exists. "
                End If
            ElseIf userconnprv = "Oracle.ManagedDataAccess.Client" Then
                'for Oracle.ManagedDataAccess.Client
                sqlq = "SELECT TABLE_NAME FROM user_tables WHERE TABLE_NAME='" & tblname & "'"
                If Not HasRecords(sqlq, userconnstr, userconnprv) Then
                    sqlq = "CREATE TABLE " & tblname & " ( "
                    'sqlq &= "Indx NUMBER GENERATED ALWAYS AS IDENTITY,"
                    For i = 0 To n - 1
                        'cols(i) = cols(i).Replace("_", " ").Replace(",", " ").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace("__", "_").Replace("""", "") 'fix column name
                        'cols(i) = FixReservedWords(cols(i), userconnprv)
                        sqlq &= cols(i).Trim & " VARCHAR2(4000 CHAR) DEFAULT NULL"
                        If i < n - 1 Then sqlq = sqlq & ","
                    Next
                    'sqlq &= "CONSTRAINT " & tblname & "_PK PRIMARY KEY(Indx)"
                    sqlq &= " )"

                    ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
                    If ret = "Query executed fine." Then
                        ret = tblname & " created. "
                    Else
                        ret = "ERROR!! " & tblname & " creation crashed: " & ret & " "
                    End If
                Else
                    ret = tblname & " already exists. "
                End If
            ElseIf userconnprv = "System.Data.Odbc" Then
                If Not TableExists(tblname, userconnstr, userconnprv) Then
                    sqlq = "CREATE TABLE [" & tblname & "]("
                    For i = 0 To n - 1
                        'cols(i) = cols(i).Replace("_", " ").Replace(",", " ").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace("__", "_").Replace("""", "") 'fix column name
                        'cols(i) = FixReservedWords(cols(i), userconnprv)
                        sqlq &= " " & cols(i).Trim & " [nvarchar](4000) NULL"
                        If i < n - 1 Then sqlq = sqlq & ","
                    Next
                    sqlq = sqlq & ")"

                    ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
                    If ret = "Query executed fine." Then
                        ret = tblname & " created. "
                    Else
                        ret = "ERROR!! " & tblname & " creation crashed: " & ret & " "
                    End If
                End If
                Return ret.Replace("Query executed fine.", "")
            ElseIf userconnprv = "System.Data.OleDb" Then
                If Not TableExists(tblname, userconnstr, userconnprv) Then
                    sqlq = "CREATE TABLE [" & tblname & "]("
                    For i = 0 To n - 1
                        'cols(i) = cols(i).Replace("_", " ").Replace(",", " ").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace("__", "_").Replace("""", "") 'fix column name
                        'cols(i) = FixReservedWords(cols(i), userconnprv)
                        sqlq &= " " & cols(i).Trim & " [nvarchar](4000) NULL"
                        If i < n - 1 Then sqlq = sqlq & ","
                    Next
                    sqlq = sqlq & ")"

                    ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
                    If ret = "Query executed fine." Then
                        ret = tblname & " created. "
                    Else
                        ret = "ERROR!! " & tblname & " creation crashed: " & ret & " "
                    End If
                End If
                Return ret.Replace("Query executed fine.", "")

            ElseIf userconnprv = "Npgsql" Then  'PostgreSQL  Npgsql
                If Not TableExists(tblname, userconnstr, userconnprv) Then
                    sqlq = "CREATE TABLE [" & tblname & "]("
                    For i = 0 To n - 1
                        'cols(i) = cols(i).Replace("_", " ").Replace(",", " ").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace("__", "_").Replace("""", "") 'fix column name
                        'cols(i) = FixReservedWords(cols(i), userconnprv)
                        sqlq &= " `" & cols(i).Trim & "` text NULL"
                        If i < n - 1 Then sqlq = sqlq & ","
                    Next
                    sqlq = sqlq & ")"
                    sqlq = ConvertSQLSyntaxFromOURdbToUserDB(sqlq, userconnprv, ret)
                    ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
                    If ret = "Query executed fine." Then
                        ret = tblname & " created. "
                    Else
                        ret = tblname & " creation crashed: " & ret & " "
                    End If
                End If

            ElseIf userconnprv.StartsWith("InterSystems.Data.") Then
                If tblname.IndexOf(".") < 0 Then
                    tblname = "UserData." & tblname
                End If
                If Not TableExists(tblname, userconnstr, userconnprv) Then
                    sqlq = "CREATE TABLE [" & tblname & "]("
                    For i = 0 To n - 1
                        'cols(i) = cols(i).Replace("_", " ").Replace(",", " ").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace("__", "_").Replace("""", "") 'fix column name
                        'cols(i) = FixReservedWords(cols(i), userconnprv)
                        sqlq &= " [" & cols(i).Trim & "] nvarchar(4000) NULL"
                        If i < n - 1 Then sqlq = sqlq & ","
                    Next
                    sqlq = sqlq & ")"
                    sqlq = ConvertSQLSyntaxFromOURdbToUserDB(sqlq, userconnprv, ret)
                    ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
                    If ret = "Query executed fine." Then
                        ret = tblname & " created. "
                    Else
                        ret = tblname & " creation crashed: " & ret & " "
                    End If
                End If

            ElseIf userconnprv = "Sqlite" Then
                If Not TableExists(tblname, userconnstr, userconnprv) Then
                    sqlq = "CREATE TABLE [" & tblname & "]("
                    For i = 0 To n - 1
                        If cols(i).Trim = "Indx" Then cols(i) = "IndxOrg"
                        sqlq &= " [" & cols(i).Trim & "]" & " [nvarchar](3000) NULL"
                        If i < n - 1 Then sqlq = sqlq & ","
                    Next
                    sqlq = sqlq & ", [Indx] Integer PRIMARY KEY AUTOINCREMENT"
                    sqlq = sqlq & ")"
                    sqlq = ConvertSQLSyntaxFromOURdbToUserDB(sqlq, userconnprv, ret)
                    ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
                    If ret = "Query executed fine." Then
                        ret = tblname & " created. "
                    Else
                        ret = tblname & " creation crashed: " & ret & " "
                    End If
                End If


            Else 'SQL Server 
                If Not TableExists(tblname, userconnstr, userconnprv) Then
                    sqlq = "CREATE TABLE [" & tblname & "]("
                    For i = 0 To n - 1
                        'cols(i) = cols(i).Replace("_", " ").Replace(",", " ").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace("__", "_").Replace("""", "") 'fix column name
                        'cols(i) = FixReservedWords(cols(i), userconnprv)
                        'sqlq &= " " & cols(i).Trim & " [nvarchar](MAX) NULL"
                        'sqlq &= " [" & cols(i).Trim & "] TEXT NULL"
                        sqlq &= " [" & cols(i).Trim & "]" & " [nvarchar](MAX) NULL"
                        If i < n - 1 Then sqlq = sqlq & ","
                    Next
                    sqlq = sqlq & ")"
                    sqlq = ConvertSQLSyntaxFromOURdbToUserDB(sqlq, userconnprv, ret)
                    ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
                    If ret = "Query executed fine." Then
                        ret = tblname & " created. "
                    Else
                        ret = tblname & " creation crashed: " & ret & " "
                    End If
                End If
            End If
            Return ret.Replace("Query executed fine.", "")
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function IsCSVuser(Optional ByVal userconnstr As String = "", Optional ByVal userconnprv As String = "", Optional ByRef er As String = "") As String
        Try
            If userconnstr.Trim = "" Then
                userconnstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
                userconnprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            End If
            If System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing Then
                Dim sCSVconnection As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString
                Dim sCSVconnprv As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ProviderName.ToString
                If userconnprv <> "Oracle.ManagedDataAccess.Client" Then
                    If GetDataBase(userconnstr, userconnprv) = GetDataBase(sCSVconnection, sCSVconnprv) Then Return "yes"
                Else
                    Dim userdb As String = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("PASSWORD")).Trim
                    Dim CSVdb As String = sCSVconnection.Substring(0, sCSVconnection.ToUpper.IndexOf("PASSWORD")).Trim
                    If userdb = CSVdb Then Return "yes"
                End If
            End If

            Return ""
        Catch ex As Exception
            er = ex.Message
            Return ""
        End Try
    End Function
    Public Function InsertRowIntoTable(ByVal tbl As String, ByVal mRow As DataRow, ByVal dt As DataTable, ByVal connstr1 As String, ByVal connprv1 As String, ByVal connstr2 As String, ByVal connprv2 As String) As String
        Dim ret As String = String.Empty
        Dim sqlstmt As String = ""
        Try
            If dt Is Nothing Then
                Dim dv As DataView = mRecords("SELECT * FROM " & tbl.ToLower, ret, connstr1, connprv1)
                If dv Is Nothing Then
                    Return ret
                Else
                    dt = dv.Table
                End If
            End If

            Dim sFields As String = String.Empty
            Dim sValues As String = String.Empty
            Dim fld As String = String.Empty

            For j As Integer = 0 To dt.Columns.Count - 1
                If dt.Columns(j).Caption.ToUpper <> "INDX" AndAlso dt.Columns(j).Caption.ToUpper <> "ID" Then
                    fld = FixReservedWords(dt.Columns(j).Caption, connprv2)
                    If sFields <> String.Empty Then
                        sFields &= "," & fld
                    Else
                        sFields = fld
                    End If
                    If TblFieldIsNumeric(tbl, dt.Columns(j).Caption, connstr2, connprv2) Then
                        If mRow(j).ToString.Trim = "" OrElse mRow(j).ToString.Trim = "NULL" Then
                            mRow(j) = "0"
                        End If
                        If sValues = String.Empty Then
                            sValues = mRow(j).ToString
                        Else
                            sValues &= "," & mRow(j).ToString
                        End If
                    ElseIf TblFieldIsDateTime(tbl, dt.Columns(j).Caption, connstr2, connprv2) Then
                        If mRow(j).ToString.Trim = "" OrElse mRow(j).ToString.Trim = "0" OrElse mRow(j).ToString.Trim = "NULL" Then
                            mRow(j) = "NULL"
                            If sValues = String.Empty Then
                                sValues = mRow(j)
                            Else
                                sValues &= "," & mRow(j)
                            End If
                        Else
                            If sValues = String.Empty Then
                                sValues = "'" & DateToString(CDate(mRow(j)), connprv2) & "'"
                            Else
                                sValues &= ",'" & DateToString(CDate(mRow(j)), connprv2) & "'"
                            End If
                        End If
                    Else
                        If mRow(j).ToString.Trim = "NULL" Then
                            If sValues = String.Empty Then
                                sValues = mRow(j).ToString
                            Else
                                sValues &= "," & mRow(j).ToString
                            End If
                        Else
                            If sValues = String.Empty Then
                                sValues = "'" & mRow(j).ToString & "'"
                            Else
                                sValues &= ",'" & mRow(j).ToString & "'"
                            End If
                        End If
                    End If
                End If
            Next
            sqlstmt = "INSERT INTO " & tbl & " (" & sFields & ") VALUES (" & sValues & ")"

            'sqlstmt = "INSERT INTO " & tbl & " SET "
            'For j = 0 To dt.Columns.Count - 1
            '    If dt.Columns(j).Caption <> "Indx" AndAlso dt.Columns(j).Caption <> "ID" Then
            '        sqlstmt = sqlstmt & FixReservedWords(dt.Columns(j).Caption, connprv2) & "="
            '        If TblFieldIsNumeric(tbl, dt.Columns(j).Caption, connstr2, connprv2) Then
            '            If mRow(j).ToString.Trim = "" OrElse mRow(j).ToString.Trim = "NULL" Then
            '                mRow(j) = "0"
            '            End If
            '            sqlstmt = sqlstmt & mRow(j)
            '        ElseIf TblFieldIsDateTime(tbl, dt.Columns(j).Caption, connstr2, connprv2) Then
            '            If mRow(j).ToString.Trim = "" OrElse mRow(j).ToString.Trim = "0" OrElse mRow(j).ToString.Trim = "NULL" Then
            '                mRow(j) = "NULL"
            '                sqlstmt = sqlstmt & mRow(j)
            '            Else
            '                sqlstmt = sqlstmt & "'" & mRow(j) & "'"
            '            End If

            '        Else
            '            sqlstmt = sqlstmt & "'" & mRow(j) & "'"
            '        End If
            '        If j < dt.Columns.Count - 1 Then
            '            sqlstmt = sqlstmt & ","
            '        End If
            '    End If
            'Next
            'If sqlstmt.EndsWith(",") Then
            '    sqlstmt = sqlstmt.Substring(0, sqlstmt.Length - 1)
            'End If
            ret = ExequteSQLquery(sqlstmt, connstr2, connprv2)
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function

    Public Function ImportExcelFileIntoCSVstring(ByVal filepath As String, ByVal userconnprv As String, Optional ByRef mtbl As DataTable = Nothing, Optional ByRef er As String = "", Optional ByRef rdltype As String = "") As String
        Dim ret As String = String.Empty
        Try
            Dim myConn As New OleDbConnection
            If filepath.ToUpper.EndsWith(".XLS") Then    'AndAlso Environment.Is64BitOperatingSystem = False 
                'myConn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filepath + ";" + "Extended Properties='Excel 8.0;HDR=Yes';"
                myConn.ConnectionString = ConfigurationManager.AppSettings("ACEOLEDBversion").ToString & "Data Source=" + filepath + ";" + "Extended Properties='Excel 8.0;HDR=Yes';"
            ElseIf filepath.ToUpper.EndsWith(".XLSX") Then
                myConn.ConnectionString = ConfigurationManager.AppSettings("ACEOLEDBversion").ToString & "Data Source=" + filepath + ";" + "Extended Properties='Excel 12.0;HDR=Yes;IMEX = 2';"
            ElseIf filepath.ToUpper.EndsWith(".MDB") Or filepath.ToUpper.EndsWith(".ACCDB") Then
                myConn.ConnectionString = ConfigurationManager.AppSettings("ACEOLEDBversion").ToString & "Data Source=" + filepath + ";" + "Persist Security Info=True;"
            End If
            myConn.Open()

            Dim scm As DataTable = Nothing
            Dim nam As String = String.Empty
            If filepath.ToUpper.EndsWith(".XLS") OrElse filepath.ToUpper.EndsWith(".XLSX") Then
                scm = myConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, New Object() {Nothing, Nothing, Nothing, "TABLE"})
                nam = "Sheet1$"
                nam = scm.Rows(0)("TABLE_NAME").ToString
            End If

            Dim myCommand As New OleDbCommand
            myCommand.Connection = myConn
            myCommand.CommandType = CommandType.Text
            If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
            myCommand.CommandText = "Select * FROM [" & nam & "]"
            Dim xlAdapter As OleDbDataAdapter = New OleDbDataAdapter
            xlAdapter.SelectCommand = myCommand
            Dim ds As DataSet = New DataSet
            xlAdapter.Fill(ds)
            myConn.Close()
            'make columns names CLS-compliant
            'Dim er As String = String.Empty

            Dim dts As DataTable = ds.Tables(0)   ' MakeDTColumnsNamesCLScompliant(ds.Tables(0), userconnprv, er)

            If rdltype = "matrix" Then

                dts.Rows(0).Delete()
                dts.Rows(1).Delete()
                dts.Rows(2).Delete()
                dts.Rows(3).Delete()
                'first 2 columns
                dts.Columns.Remove(dts.Columns(0).ColumnName)
                dts.Columns.Remove(dts.Columns(0).ColumnName)
                dts.Columns.Remove(dts.Columns(1).ColumnName)
            End If


            dts = MakeDTColumnsNamesCLScompliant(ds.Tables(0), userconnprv, er)


            mtbl = dts

            ret = ExportToCSVtext(dts, ",", "", "")

            Return ret
            Exit Function
        Catch ex As Exception
            er = "ERROR!! " & ex.Message
        End Try

        Return Nothing
    End Function
    Public Function ImportExcelIntoDataTable(ByVal filepath As String, ByVal userconnprv As String, Optional ByRef er As String = "") As DataTable
        ' Dim mSQL As String = String.Empty
        'Dim ret As String = String.Empty
        Try
            'TODO change for work with worksheet
            Dim myConn As New OleDbConnection
            If filepath.ToUpper.EndsWith(".XLS") Then    'AndAlso Environment.Is64BitOperatingSystem = False 
                'myConn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filepath + ";" + "Extended Properties='Excel 8.0;HDR=Yes';"
                myConn.ConnectionString = ConfigurationManager.AppSettings("ACEOLEDBversion").ToString & "Data Source=" + filepath + ";" + "Extended Properties='Excel 8.0;HDR=Yes';"
            ElseIf filepath.ToUpper.EndsWith(".XLSX") Then
                myConn.ConnectionString = ConfigurationManager.AppSettings("ACEOLEDBversion").ToString & "Data Source=" + filepath + ";" + "Extended Properties='Excel 12.0;HDR=Yes;IMEX = 2';"
            ElseIf filepath.ToUpper.EndsWith(".MDB") Or filepath.ToUpper.EndsWith(".ACCDB") Then
                myConn.ConnectionString = ConfigurationManager.AppSettings("ACEOLEDBversion").ToString & "Data Source=" + filepath + ";" + "Persist Security Info=True;"
            End If
            myConn.Open()

            Dim scm As DataTable = Nothing
            Dim nam As String = String.Empty
            If filepath.ToUpper.EndsWith(".XLS") OrElse filepath.ToUpper.EndsWith(".XLSX") Then
                scm = myConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, New Object() {Nothing, Nothing, Nothing, "TABLE"})
                nam = "Sheet1$"
                nam = scm.Rows(0)("TABLE_NAME").ToString
            End If

            Dim myCommand As New OleDbCommand
            myCommand.Connection = myConn
            myCommand.CommandType = CommandType.Text
            If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
            myCommand.CommandText = "Select * FROM [" & nam & "]"
            Dim xlAdapter As OleDbDataAdapter = New OleDbDataAdapter
            xlAdapter.SelectCommand = myCommand
            Dim ds As DataSet = New DataSet
            xlAdapter.Fill(ds)

            'make columns names CLS-compliant
            Dim dts As DataTable = MakeDTColumnsNamesCLScompliant(ds.Tables(0), userconnprv, er)

            myConn.Close()
            Return dts
            Exit Function
        Catch ex As Exception
            er = "ERROR!! " & ex.Message
        End Try

        Return Nothing
    End Function
    Public Function ImportExcelIntoDbTable(ByVal tblname As String, ByVal filepath As String, ByVal userconnstr As String, ByVal userconnprv As String, ByVal exst As Boolean) As String
        ' Dim mSQL As String = String.Empty
        Dim ret As String = String.Empty
        Try
            'TODO change for work with worksheet
            Dim myConn As New OleDbConnection
            If filepath.ToUpper.EndsWith(".XLS") Then    'AndAlso Environment.Is64BitOperatingSystem = False 
                'myConn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filepath + ";" + "Extended Properties='Excel 8.0;HDR=Yes';"
                myConn.ConnectionString = ConfigurationManager.AppSettings("ACEOLEDBversion").ToString & "Data Source=" + filepath + ";" + "Extended Properties='Excel 8.0;HDR=Yes';"
            ElseIf filepath.ToUpper.EndsWith(".XLSX") Then
                myConn.ConnectionString = ConfigurationManager.AppSettings("ACEOLEDBversion").ToString & "Data Source=" + filepath + ";" + "Extended Properties='Excel 12.0;HDR=Yes;IMEX = 2';"
            ElseIf filepath.ToUpper.EndsWith(".MDB") Or filepath.ToUpper.EndsWith(".ACCDB") Then
                myConn.ConnectionString = ConfigurationManager.AppSettings("ACEOLEDBversion").ToString & "Data Source=" + filepath + ";" + "Persist Security Info=True;"
            End If
            myConn.Open()

            Dim scm As DataTable = Nothing
            Dim nam As String = String.Empty
            If filepath.ToUpper.EndsWith(".XLS") OrElse filepath.ToUpper.EndsWith(".XLSX") Then
                scm = myConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, New Object() {Nothing, Nothing, Nothing, "TABLE"})
                nam = "Sheet1$"
                nam = scm.Rows(0)("TABLE_NAME").ToString
            ElseIf filepath.ToUpper.EndsWith(".MDB") OrElse filepath.ToUpper.EndsWith(".ACCDB") Then
                nam = tblname
                If nam.IndexOf("_") > 0 Then
                    nam = nam.Substring(0, nam.IndexOf("_")).Replace("_", "")
                End If
            End If

            Dim myCommand As New OleDbCommand
            myCommand.Connection = myConn
            myCommand.CommandType = CommandType.Text
            If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
            myCommand.CommandText = "Select * FROM [" & nam & "]"
            Dim xlAdapter As OleDbDataAdapter = New OleDbDataAdapter
            xlAdapter.SelectCommand = myCommand
            Dim ds As DataSet = New DataSet
            xlAdapter.Fill(ds)

            'make columns names CLS-compliant
            Dim er As String = String.Empty

            Dim dts As DataTable = MakeDTColumnsNamesCLScompliant(ds.Tables(0), userconnprv, er)

            ret = ImportDataTableIntoDb(dts, tblname, userconnstr, userconnprv, ret)
            myConn.Close()
            Return ret
            Exit Function
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try

        'DO NOT DELETE - working if Microsoft.Office.Interop.Excel is available !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        'Try
        '    'Dim exl As New Excel.ApplicationClass
        '    'Dim xlbook As Excel.Workbook
        '    'Dim xlsheet As Excel.Worksheet
        '    'xlbook = exl.Workbooks.Open(filepath)
        '    'xlsheet = xlbook.Worksheets(1)
        '    'Dim nam As String = xlsheet.Name
        '    'exl.Application.Workbooks.Close()

        '    Dim app As Microsoft.Office.Interop.Excel.Application = New Microsoft.Office.Interop.Excel.Application
        '    'Dim workBook As Microsoft.Office.Interop.Excel.Workbook = app.Workbooks.Open(filepath, 0, True, 5, "", "", True, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", False, False, 0, True, 1, 0)
        '    'Dim workBook As New Microsoft.Office.Interop.Excel.Workbook
        '    'app.Workbooks.OpenText(filepath, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, 1, Microsoft.Office.Interop.Excel.XlTextParsingType.xlDelimited, Microsoft.Office.Interop.Excel.Constants.xlDoubleQuote, False, True, False, True, False, False, "", Microsoft.Office.Interop.Excel.XlColumnDataType.xlTextFormat)
        '    'Dim workBook As Microsoft.Office.Interop.Excel.Workbook = app.Workbooks.Open(filepath, 0, True, 5, "", "", True, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, ",", False, False, 0, True, 1, 0)
        '    Dim workBook As Microsoft.Office.Interop.Excel.Workbook = app.Workbooks.Open(filepath, 0, True, 6, "", "", True, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, Chr(9), False, False, 0, True, 1, 0)

        '    Dim workSheet As Microsoft.Office.Interop.Excel.Worksheet = workBook.ActiveSheet
        '    Dim i, j, k, n As Integer
        '    Dim dt As New DataTable
        '    'For j = 0 To workSheet.Cells.Count - 1
        '    '    dt.Columns.Add(workSheet.Cells(0, j).ToString)
        '    'Next
        '    'For i = 1 To workSheet.Rows.Count - 1
        '    '    For j = 0 To workSheet.Cells.Count - 1
        '    '        dt.Rows(i - 1)(j) = workSheet.Cells(i, j).ToString
        '    '    Next
        '    'Next
        '    i = 0
        '    j = 0
        '    k = 0
        '    n = 0
        '    Dim row As Integer = 1
        '    Dim col As Integer = 1
        '    'Dim lbl As Label
        '    'Dim lst As ListBox
        '    Dim range As Microsoft.Office.Interop.Excel.Range
        '    Dim last_cell As Microsoft.Office.Interop.Excel.Range
        '    Dim first_cell As Microsoft.Office.Interop.Excel.Range
        '    Dim value_range As Microsoft.Office.Interop.Excel.Range
        '    Dim num_items As Integer
        '    ' Set the title.
        '    range = workSheet.Cells(1, 1)
        '    Dim cvalues() As String = range.Value2.ToString.Split(Chr(9))
        '    For j = 0 To cvalues.Count - 1
        '        If cvalues(j).Trim <> "" Then
        '            dt.Columns.Add(cvalues(j))
        '        End If
        '    Next
        '    n = dt.Columns.Count
        '    ' Get the values.
        '    ' Find the last cell in the column.value_range
        '    range = workSheet.Columns(col)
        '    last_cell = range.End(Microsoft.Office.Interop.Excel.XlDirection.xlDown)

        '    ' Get a Range holding the values.
        '    first_cell = workSheet.Cells(row + 1, col)
        '    value_range = workSheet.Range(first_cell, last_cell)

        '    ' Get the values.
        '    'range_values = value_range.Value

        '    ' Convert this into a 1-dimensional array.
        '    ' Note that the Range's array has lower bounds 1.
        '    num_items = value_range.Rows.Count
        '    For i = 1 To num_items
        '        k = 0
        '        cvalues = value_range.Rows(i).Value2.ToString.Split(Chr(9))
        '        Dim dtRow As DataRow
        '        dtRow = dt.NewRow
        '        For j = 0 To cvalues.Count - 1
        '            If cvalues(j).Trim <> "" Then
        '                If k < n Then
        '                    dtRow(k) = cvalues(j)
        '                Else
        '                    ret = "check values with spaces"
        '                End If
        '                k = k + 1
        '            End If
        '            If k = n Then Exit For
        '        Next
        '        If k < n Then
        '            For j = k To n - 1
        '                dtRow(k) = ""
        '            Next
        '        End If
        '        Try
        '            dt.Rows.Add(dtRow)
        '        Catch ex As Exception
        '            ret = "ERROR!! " & ex.Message
        '        End Try
        '    Next
        '    ret = ImportDataTableIntoDb(dt, tblname, userconnstr, userconnprv, "")
        '    Return ret


        'Catch ex As Exception
        '    ret = "ERROR!! " & ex.Message

        'End Try
        Return ret
    End Function

    Public Function ImportCSVintoDbTable(ByVal logon As String, ByRef tblname As String, ByVal csvpath As String, ByVal d As String, ByVal userconnstr As String, ByVal userconnprv As String, ByVal exst As Boolean) As String
        Dim ret As String = String.Empty
        Dim clnms As Boolean = False
        Dim sr1 As StreamReader = New StreamReader(csvpath)
        Dim csvstr1 As String = String.Empty
        Dim csvstr As String = String.Empty
        Dim msql As String = String.Empty
        Dim k As Integer = 0
        While Not sr1.EndOfStream
            csvstr = sr1.ReadLine
            k = k + 1
            If csvstr.Replace(d, "").Trim = "" Then  'empty row
                Continue While
            End If
            If csvstr.Replace(d, "").Trim <> "" Then
                csvstr1 = cleanText(csvstr)  'supposedly column names in first not empty row
                clnms = True
                Exit While
            End If
        End While
        sr1.Close()
        sr1.Dispose()
        Dim m As Integer = 0
        Dim n As Integer = 0
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim colnames(0) As String
        Dim bq As Boolean = False
        'Dim db As String

        'db = GetDataBase(userconnstr).ToLower

        'make array with column names with double quotes
        colnames = MakeArrayFromStringWithQuotes(csvstr1, d)
        'fix col names
        For i = 0 To colnames.Length - 1
            'fix column name
            colnames(i) = colnames(i).Replace("%", "Percent ").Replace(",", "_").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace("(", "_").Replace(")", "_").Replace("__", "_").Replace("""", "").Replace("_", "").Replace(".", "")
            colnames(i) = colnames(i).Replace("[", "").Replace("]", "").Trim
            colnames(i) = FixReservedWords(colnames(i), userconnprv)
            colnames(i) = Regex.Replace(colnames(i), "[^a-zA-Z0-9_]", "")  'CLS-compliant
            colnames(i) = cleanText(FixReservedWords(colnames(i), userconnprv))
            If userconnprv.StartsWith("InterSystems.Data.") Then
                colnames(i) = colnames(i).Replace("_", "")
            End If
            colnames(i) = colnames(i).Replace("""", "")
            If userconnprv.StartsWith("Oracle.") Then
                colnames(i) = FixReservedWords(colnames(i), userconnprv)
            End If

            If colnames(i).Trim = "" Then
                colnames(i) = "cName" & i.ToString
            ElseIf IsNumeric(colnames(i).Substring(0, 1)) Then
                colnames(i) = "c" & colnames(i).ToString
            End If
        Next
        'fix reserved words if column names for odbc
        If userconnprv = "System.Data.Odbc" Then
            colnames = FixReservedWordsColumnNamesODBC(colnames)
        ElseIf userconnprv = "System.Data.OleDb" Then
            colnames = FixReservedWordsColumnNamesODBC(colnames)
        End If

        n = colnames.Length
        Dim coltypes(n - 1) As String
        Dim collens(n - 1) As String
        Dim indxcol As String = String.Empty
        Try
            If exst = False Then
                'create table with columns names
                ret = CreateDbTableForCSV(tblname, d, colnames, coltypes, collens, userconnstr, userconnprv, ret)
                WriteToAccessLog(logon, "Create table " & tblname & " " & ret, 111)
                If ret.StartsWith("ERROR!!") Then
                    Return ret
                End If
            Else
                'check if tblname has the same fields colnames
                For i = 0 To n - 1
                    'colnames(i) = colnames(i).Replace("_", " ").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace(",", "_").Replace("(", "_").Replace(")", "_").Trim 'fix column name
                    colnames(i) = colnames(i).Replace("%", "Percent ").Replace(",", "_").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace("(", "_").Replace(")", "_").Replace("__", "_").Replace("""", "").Replace("_", "").Replace(".", "") 'fix column name
                    colnames(i) = cleanText(FixReservedWords(colnames(i), userconnprv))
                    colnames(i) = Regex.Replace(colnames(i), "[^a-zA-Z0-9_]", "")  'CLS-compliant
                    If userconnprv.StartsWith("InterSystems.Data.") Then
                        colnames(i) = colnames(i).Replace("_", "")
                    End If
                    If Not IsColumnFromTable(tblname, colnames(i), userconnstr, userconnprv, ret) Then
                        WriteToAccessLog(logon, "Existing table " & tblname & " does Not have a column " & colnames(i) & " - " & ret, 111)
                        Return "ERROR!! Existing table " & tblname & " does Not have a column " & colnames(i)
                    End If
                Next
                ret = ""
                indxcol = IndexFieldInTable(tblname, GetDataBase(userconnstr, userconnprv), userconnstr, userconnprv, ret)

            End If

            Dim er As String = String.Empty
            Dim db As String = GetDataBase(userconnstr, userconnprv)

            If userconnprv = "Oracle.ManagedDataAccess.Client" Then
                msql = "SELECT * FROM " & db & "." & tblname
            Else
                msql = "SELECT * FROM [" & tblname & "]"
            End If
            Dim dtd As DataTable = mRecords(msql, er, userconnstr, userconnprv).Table
            'Dim dtd As DataTable = mRecords("SELECT * FROM " & tblname, er, userconnstr, userconnprv).Table

            'load data from csv file
            If userconnprv = "MySql.Data.MySqlClient" Then
                msql = "SET NAMES utf8mb4;"
                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                'WriteToAccessLog(tblname, "SET NAMES: " & ret, 111)
            End If
            'msql = "LOAD Data INFILE  '" & csvpath & "' INTO Table " & tblname & " CHARACTER SET latin1 FIELDS TERMINATED BY '" & d & "' OPTIONALLY ENCLOSED BY '""' LINES TERMINATED BY '\r\n' IGNORE " & k.ToString & " LINES "
            'ret = ExequteSQLquery(msql, userconnstr, userconnprv)
            'WriteToAccessLog(tblname, "Load into table: " & ret, 111)

            'If ret <> "Query executed fine." OrElse Not HasRecords("SELECT * FROM " & tblname, userconnstr, userconnprv) Then
            'try long way
            Dim csvfields As String = String.Empty
            For i = 0 To colnames.Length - 1
                csvfields = csvfields & FixReservedWords(colnames(i), userconnprv)
                If i < colnames.Length - 1 Then
                    csvfields = csvfields & ","
                End If
            Next
            clnms = False
            Dim sr As StreamReader = New StreamReader(csvpath)
            Dim vals() As String
            Dim colname As String = String.Empty

            While Not sr.EndOfStream
                csvstr = cleanText(sr.ReadLine)
                k = k + 1
                If csvstr.Replace(d, "").Trim = "" Then  'empty rows
                    Continue While
                ElseIf csvstr.Replace(d, "").Trim <> "" AndAlso clnms = False Then
                    csvstr1 = csvstr  'supposedly column names in first not empty row
                    clnms = True
                    Continue While
                ElseIf csvstr.Replace(d, "").Trim <> "" AndAlso clnms = True Then
                    vals = MakeArrayFromStringWithQuotes(csvstr, d)
                    'If userconnprv = "Oracle.ManagedDataAccess.Client" OrElse userconnprv = "System.Data.SqlClient" Then
                    Dim sVals As String = String.Empty
                    Dim sVal As String = String.Empty

                    For i = 0 To vals.Length - 1
                        If exst AndAlso colnames(i) = indxcol Then
                            Continue For
                        End If
                        If vals(i) Is Nothing OrElse IsDBNull(vals(i)) OrElse vals(i).ToString = "" Then
                            If exst AndAlso ColumnTypeIsNumeric(dtd.Columns(colnames(i))) Then
                                sVals &= "0"
                            Else
                                sVals &= "''"
                            End If
                        Else
                            sVal = vals(i).Replace("""", "").Replace("'", "")
                            If exst AndAlso ColumnTypeIsNumeric(dtd.Columns(colnames(i))) Then
                                sVals &= vals(i).Replace("""", "").Replace("'", "")
                            Else
                                Dim dtReturn As New DateTime

                                If IsDateTimeValue(sVal, dtReturn) Then
                                    Dim bTimeStamp As Boolean = False

                                    If userconnprv = "Oracle.ManagedDataAccess.Client" Then bTimeStamp = True
                                    sVal = DateToString(dtReturn, userconnprv, bTimeStamp)
                                    sVals &= "'" & sVal & "'"
                                Else
                                    sVals &= "'" & vals(i).Replace("""", "").Replace("'", "") & "'"
                                End If

                            End If
                        End If
                        If i < vals.Length - 1 Then _
                                sVals &= ","
                    Next
                    msql = "INSERT INTO [" & tblname & "] (" & csvfields & ") VALUES (" & sVals & ")"

                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    m = m + 1
                End If
            End While
            sr.Close()
            sr.Dispose()
            'End If
            WriteToAccessLog(logon, tblname & " - rows inserted: " & m.ToString & " - with result: " & ret, 111)

            If exst = False Then
                ret = CorrectFieldTypesInTable(tblname, userconnstr, userconnprv, exst)
            End If

            Return ret

        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function ImportCSVintoDbTableFromStream(ByVal logon As String, ByRef tblname As String, ByRef datastr As Stream, ByVal d As String, ByVal userconnstr As String, ByVal userconnprv As String, ByVal exst As Boolean) As String
        Dim ret As String = String.Empty
        Dim clnms As Boolean = False
        Dim sr1 As StreamReader = New StreamReader(datastr)
        Dim csvstr1 As String = String.Empty
        Dim csvstr As String = String.Empty
        Dim msql As String = String.Empty
        Dim k As Integer = 0
        While Not sr1.EndOfStream
            csvstr = sr1.ReadLine
            k = k + 1
            If csvstr.Replace(d, "").Trim = "" Then  'empty row
                Continue While
            End If
            If csvstr.Replace(d, "").Trim <> "" Then
                csvstr1 = cleanText(csvstr)  'supposedly column names in first not empty row
                clnms = True
                Exit While
            End If
        End While
        'sr1.Close()
        'sr1.Dispose()
        Dim m As Integer = 0
        Dim n As Integer = 0
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim colnames(0) As String
        Dim bq As Boolean = False
        'Dim db As String

        'db = GetDataBase(userconnstr).ToLower

        'make array with column names with double quotes
        colnames = MakeArrayFromStringWithQuotes(csvstr1, d)
        'fix col names
        For i = 0 To colnames.Length - 1
            'fix column name
            colnames(i) = colnames(i).Replace("%", "Percent ").Replace(",", "_").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace("(", "_").Replace(")", "_").Replace("__", "_").Replace("""", "").Replace("_", "").Replace(".", "")
            colnames(i) = colnames(i).Replace("[", "").Replace("]", "").Trim
            colnames(i) = FixReservedWords(colnames(i), userconnprv)
            colnames(i) = Regex.Replace(colnames(i), "[^a-zA-Z0-9_]", "")  'CLS-compliant
            colnames(i) = cleanText(FixReservedWords(colnames(i), userconnprv))
            If userconnprv.StartsWith("InterSystems.Data.") Then
                colnames(i) = colnames(i).Replace("_", "")
            End If
            colnames(i) = colnames(i).Replace("""", "")
            If userconnprv.StartsWith("Oracle.") Then
                colnames(i) = FixReservedWords(colnames(i), userconnprv)
            End If

            If colnames(i).Trim = "" Then
                colnames(i) = "cName" & i.ToString
            ElseIf IsNumeric(colnames(i).Substring(0, 1)) Then
                colnames(i) = "c" & colnames(i).ToString
            End If
        Next
        'fix reserved words if column names for odbc
        If userconnprv = "System.Data.Odbc" Then
            colnames = FixReservedWordsColumnNamesODBC(colnames)
        ElseIf userconnprv = "System.Data.OleDb" Then
            colnames = FixReservedWordsColumnNamesODBC(colnames)
        End If

        n = colnames.Length
        Dim coltypes(n - 1) As String
        Dim collens(n - 1) As String
        Dim indxcol As String = String.Empty
        Try
            If exst = False Then
                'create table with columns names
                ret = CreateDbTableForCSV(tblname, d, colnames, coltypes, collens, userconnstr, userconnprv, ret)
                WriteToAccessLog(logon, "Create table " & tblname & " " & ret, 111)
                If ret.StartsWith("ERROR!!") Then
                    Return ret
                End If
            Else
                'check if tblname has the same fields colnames
                For i = 0 To n - 1
                    'colnames(i) = colnames(i).Replace("_", " ").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace(",", "_").Replace("(", "_").Replace(")", "_").Trim 'fix column name
                    colnames(i) = colnames(i).Replace("%", "Percent ").Replace(",", "_").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace("(", "_").Replace(")", "_").Replace("__", "_").Replace("""", "").Replace("_", "").Replace(".", "") 'fix column name
                    colnames(i) = cleanText(FixReservedWords(colnames(i), userconnprv))
                    colnames(i) = Regex.Replace(colnames(i), "[^a-zA-Z0-9_]", "")  'CLS-compliant
                    If userconnprv.StartsWith("InterSystems.Data.") Then
                        colnames(i) = colnames(i).Replace("_", "")
                    End If
                    If Not IsColumnFromTable(tblname, colnames(i), userconnstr, userconnprv, ret) Then
                        WriteToAccessLog(logon, "Existing table " & tblname & " does Not have a column " & colnames(i) & " - " & ret, 111)
                        Return "ERROR!! Existing table " & tblname & " does Not have a column " & colnames(i)
                    End If
                Next
                ret = ""
                indxcol = IndexFieldInTable(tblname, GetDataBase(userconnstr, userconnprv), userconnstr, userconnprv, ret)

            End If

            Dim er As String = String.Empty
            Dim db As String = GetDataBase(userconnstr, userconnprv)

            If userconnprv = "Oracle.ManagedDataAccess.Client" Then
                msql = "SELECT * FROM " & db & "." & tblname
            Else
                msql = "SELECT * FROM [" & tblname & "]"
            End If
            Dim dtd As DataTable = mRecords(msql, er, userconnstr, userconnprv).Table
            'Dim dtd As DataTable = mRecords("SELECT * FROM " & tblname, er, userconnstr, userconnprv).Table

            'load data from csv file
            If userconnprv = "MySql.Data.MySqlClient" Then
                msql = "SET NAMES utf8mb4;"
                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                'WriteToAccessLog(tblname, "SET NAMES: " & ret, 111)
            End If
            'msql = "LOAD Data INFILE  '" & csvpath & "' INTO Table " & tblname & " CHARACTER SET latin1 FIELDS TERMINATED BY '" & d & "' OPTIONALLY ENCLOSED BY '""' LINES TERMINATED BY '\r\n' IGNORE " & k.ToString & " LINES "
            'ret = ExequteSQLquery(msql, userconnstr, userconnprv)
            'WriteToAccessLog(tblname, "Load into table: " & ret, 111)

            'If ret <> "Query executed fine." OrElse Not HasRecords("SELECT * FROM " & tblname, userconnstr, userconnprv) Then
            'try long way
            Dim csvfields As String = String.Empty
            For i = 0 To colnames.Length - 1
                csvfields = csvfields & FixReservedWords(colnames(i), userconnprv)
                If i < colnames.Length - 1 Then
                    csvfields = csvfields & ","
                End If
            Next
            clnms = False
            'Dim sr As StreamReader = New StreamReader(datastr)
            Dim vals() As String
            Dim colname As String = String.Empty

            While Not sr1.EndOfStream
                csvstr = cleanText(sr1.ReadLine)
                k = k + 1
                If csvstr.Replace(d, "").Trim = "" Then  'empty rows
                    Continue While
                ElseIf csvstr.Replace(d, "").Trim <> "" AndAlso clnms = False Then
                    csvstr1 = csvstr  'supposedly column names in first not empty row
                    clnms = True
                    Continue While
                ElseIf csvstr.Replace(d, "").Trim <> "" AndAlso clnms = True Then
                    vals = MakeArrayFromStringWithQuotes(csvstr, d)
                    'If userconnprv = "Oracle.ManagedDataAccess.Client" OrElse userconnprv = "System.Data.SqlClient" Then
                    Dim sVals As String = String.Empty
                    Dim sVal As String = String.Empty

                    For i = 0 To vals.Length - 1
                        If exst AndAlso colnames(i) = indxcol Then
                            Continue For
                        End If
                        If vals(i) Is Nothing OrElse IsDBNull(vals(i)) OrElse vals(i).ToString = "" Then
                            If exst AndAlso ColumnTypeIsNumeric(dtd.Columns(colnames(i))) Then
                                sVals &= "0"
                            Else
                                sVals &= "''"
                            End If
                        Else
                            sVal = vals(i).Replace("""", "").Replace("'", "")
                            If exst AndAlso ColumnTypeIsNumeric(dtd.Columns(colnames(i))) Then
                                sVals &= vals(i).Replace("""", "").Replace("'", "")
                            Else
                                Dim dtReturn As New DateTime

                                If IsDateTimeValue(sVal, dtReturn) Then
                                    Dim bTimeStamp As Boolean = False

                                    If userconnprv = "Oracle.ManagedDataAccess.Client" Then bTimeStamp = True
                                    sVal = DateToString(dtReturn, userconnprv, bTimeStamp)
                                    sVals &= "'" & sVal & "'"
                                Else
                                    sVals &= "'" & vals(i).Replace("""", "").Replace("'", "") & "'"
                                End If

                            End If
                        End If
                        If i < vals.Length - 1 Then _
                                sVals &= ","
                    Next
                    msql = "INSERT INTO [" & tblname & "] (" & csvfields & ") VALUES (" & sVals & ")"

                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    m = m + 1
                End If
            End While
            sr1.Close()
            sr1.Dispose()
            'End If
            WriteToAccessLog(logon, tblname & " - rows inserted: " & m.ToString & " - with result: " & ret, 111)

            If exst = False Then
                ret = CorrectFieldTypesInTable(tblname, userconnstr, userconnprv, exst)
            End If

            Return ret

        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function ImportCSVintoDbTableByLine(ByVal logon As String, ByRef tblname As String, ByVal csvpath As String, ByVal d As String, ByVal userconnstr As String, ByVal userconnprv As String, ByVal exst As Boolean) As String
        Dim ret As String = String.Empty
        Dim clnms As Boolean = False
        Dim client = New WebClient
        Dim Stream As Stream = client.OpenRead(csvpath)
        Dim sr As StreamReader = New StreamReader(Stream)
        Dim csvstr1 As String = String.Empty
        Dim csvstr As String = String.Empty
        Dim msql As String = String.Empty
        Dim k As Integer = 0
        While Not sr.EndOfStream
            csvstr = cleanText(sr.ReadLine)
            k = k + 1
            If csvstr.Replace(d, "").Trim = "" Then  'empty row
                Continue While
            End If
            If csvstr.Replace(d, "").Trim <> "" Then
                csvstr1 = cleanText(csvstr)  'supposedly column names in first not empty row
                'TODO make Field names be CLS-compliant identifiers !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                clnms = True
                Exit While
            End If
        End While
        'sr1.Close()
        'sr1.Dispose()
        Dim m As Integer = 0
        Dim n As Integer = 0
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim colnames(0) As String
        Dim bq As Boolean = False
        'Dim db As String

        'db = GetDataBase(userconnstr).ToLower

        'make array with column names with double quotes
        colnames = MakeArrayFromStringWithQuotes(csvstr1, d)
        'fix col names
        For i = 0 To colnames.Length - 1
            'fix column name
            colnames(i) = colnames(i).Replace("%", "Percent ").Replace(",", "_").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace("(", "_").Replace(")", "_").Replace("__", "_").Replace("""", "").Replace("_", "").Replace(".", "")
            colnames(i) = colnames(i).Replace("[", "").Replace("]", "").Trim
            colnames(i) = FixReservedWords(colnames(i), userconnprv)
            colnames(i) = Regex.Replace(colnames(i), "[^a-zA-Z0-9_]", "")
            colnames(i) = cleanText(FixReservedWords(colnames(i), userconnprv))
            If userconnprv.StartsWith("InterSystems.Data.") Then
                colnames(i) = colnames(i).Replace("_", "")
            End If
            colnames(i) = colnames(i).Replace("""", "")
            If colnames(i).Trim = "" Then
                colnames(i) = "cName" & i.ToString
            ElseIf IsNumeric(colnames(i).Substring(0, 1)) Then
                colnames(i) = "c" & colnames(i).ToString
            End If
        Next
        'fix reserved words if column names for odbc
        If userconnprv = "System.Data.Odbc" Then
            colnames = FixReservedWordsColumnNamesODBC(colnames)
        ElseIf userconnprv = "System.Data.OleDb" Then
            colnames = FixReservedWordsColumnNamesODBC(colnames)
        End If

        n = colnames.Length
        Dim coltypes(n - 1) As String
        Dim collens(n - 1) As String
        Dim indxcol As String = String.Empty
        Try
            If exst = False Then
                'create table with columns names
                ret = CreateDbTableForCSV(tblname, d, colnames, coltypes, collens, userconnstr, userconnprv, ret)
                WriteToAccessLog(logon, "Create table: " & tblname & " " & ret, 111)
                If ret.StartsWith("ERROR!!") Then
                    Return ret
                End If
            Else
                'check if tblname has the same fields colnames
                For i = 0 To n - 1
                    'colnames(i) = colnames(i).Replace("_", " ").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace(",", "_").Replace("(", "_").Replace(")", "_").Trim 'fix column name
                    colnames(i) = colnames(i).Replace("%", "Percent ").Replace(",", "_").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace("(", "_").Replace(")", "_").Replace("__", "_").Replace("""", "").Replace("_", "").Replace(".", "") 'fix column name
                    colnames(i) = FixReservedWords(colnames(i), userconnprv)
                    colnames(i) = Regex.Replace(colnames(i), "[^a-zA-Z0-9_]", "")
                    colnames(i) = cleanText(FixReservedWords(colnames(i), userconnprv))
                    If userconnprv.StartsWith("InterSystems.Data.") Then
                        colnames(i) = colnames(i).Replace("_", "")
                    End If
                    If Not IsColumnFromTable(tblname, colnames(i), userconnstr, userconnprv, ret) Then
                        WriteToAccessLog(logon, "Existing table: " & tblname & " does not have a column " & colnames(i) & " - " & ret, 111)
                        Return "ERROR!! Existing table: " & tblname & " does not have a column " & colnames(i)
                    End If
                Next
                ret = ""
                indxcol = IndexFieldInTable(tblname, GetDataBase(userconnstr, userconnprv), userconnstr, userconnprv, ret)

            End If
            Dim er As String = String.Empty
            Dim dtd As DataTable = mRecords("SELECT * FROM " & tblname, er, userconnstr, userconnprv).Table
            'load data from csv file
            If userconnprv = "MySql.Data.MySqlClient" Then
                msql = "SET NAMES utf8mb4;"
                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                'WriteToAccessLog(tblname, "SET NAMES: " & ret, 111)
            End If
            'msql = "LOAD Data INFILE  '" & csvpath & "' INTO Table " & tblname & " CHARACTER SET latin1 FIELDS TERMINATED BY '" & d & "' OPTIONALLY ENCLOSED BY '""' LINES TERMINATED BY '\r\n' IGNORE " & k.ToString & " LINES "
            'ret = ExequteSQLquery(msql, userconnstr, userconnprv)
            'WriteToAccessLog(tblname, "Load into table: " & ret, 111)

            'If ret <> "Query executed fine." OrElse Not HasRecords("SELECT * FROM " & tblname, userconnstr, userconnprv) Then
            'try long way
            Dim csvfields As String = String.Empty
            For i = 0 To colnames.Length - 1
                csvfields = csvfields & FixReservedWords(colnames(i), userconnprv)
                If i < colnames.Length - 1 Then
                    csvfields = csvfields & ","
                End If
            Next
            clnms = True
            'Dim sr As StreamReader = New StreamReader(csvpath)
            Dim vals() As String
            Dim colname As String = String.Empty

            While Not sr.EndOfStream
                csvstr = cleanText(sr.ReadLine)
                k = k + 1
                If csvstr.Replace(d, "").Trim = "" Then  'empty rows
                    Continue While
                ElseIf csvstr.Replace(d, "").Trim <> "" AndAlso clnms = False Then
                    csvstr1 = csvstr  'supposedly column names in first not empty row
                    clnms = True
                    Continue While
                ElseIf csvstr.Replace(d, "").Trim <> "" AndAlso clnms = True Then
                    vals = MakeArrayFromStringWithQuotes(csvstr, d)
                    'If userconnprv = "Oracle.ManagedDataAccess.Client" OrElse userconnprv = "System.Data.SqlClient" Then
                    Dim sVals As String = String.Empty
                    For i = 0 To vals.Length - 1
                        If exst AndAlso colnames(i) = indxcol Then
                            Continue For
                        End If
                        If vals(i) Is Nothing OrElse IsDBNull(vals(i)) OrElse vals(i).ToString = "" Then
                            If exst AndAlso ColumnTypeIsNumeric(dtd.Columns(colnames(i))) Then
                                sVals &= "0"
                            Else
                                sVals &= "''"
                            End If
                        Else
                            If exst AndAlso ColumnTypeIsNumeric(dtd.Columns(colnames(i))) Then
                                sVals &= vals(i).Replace("""", "").Replace("'", "")
                            Else
                                sVals &= "'" & vals(i).Replace("""", "").Replace("'", "") & "'"
                            End If
                        End If
                        If i < vals.Length - 1 Then _
                                sVals &= ","
                    Next
                    msql = "INSERT INTO [" & tblname & "] (" & csvfields & ") VALUES (" & sVals & ")"

                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    m = m + 1
                End If
            End While
            sr.Close()
            sr.Dispose()
            'End If
            WriteToAccessLog(logon, tblname & " - rows inserted: " & m.ToString & " - with result: " & ret, 111)

            If exst = False Then
                ret = CorrectFieldTypesInTable(tblname, userconnstr, userconnprv, exst)
            End If

            Return ret

        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function FixReservedWordsColumnNamesODBC(ByVal colnames() As String) As String()
        Dim i As Integer
        For i = 0 To colnames.Length - 1
            If colnames(i) <> FixReservedWords(colnames(i)).Replace("""", "") Then
                'colnames(i) is reserved word
                colnames(i) = colnames(i) & "_1"
            End If
            'FixDoubleNames
        Next
        Return colnames
    End Function
    Public Function RetypeAndReplaceColumnODBC(tblname As String, colname As String, newtype As String, connstr As String, connprv As String, Optional ByRef er As String = "", Optional ByRef userODBCdriver As String = "", Optional ByRef userODBCdatabase As String = "", Optional ByRef userODBCdatasource As String = "") As String
        'connprv - odbc
        Dim ret As String = ""
        Dim NewName As String = colname & "_" & newtype
        Dim msql As String = String.Empty

        If newtype = "num" Then
            If userODBCdriver.ToUpper.StartsWith("ORACLE") Then
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " FLOAT(126) Default 0 NOT NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE " & tblname & " SET " & NewName & "=" & colname
                msql &= " WHERE " & colname & " IS NOT NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Drop the old column
                msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Rename new column
                msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & NewName & " To " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
            ElseIf userODBCdriver.ToUpper.StartsWith("SQL") Then
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " FLOAT NOT NULL "
                msql &= "CONSTRAINT DF_" & tblname & "_" & NewName & " DEFAULT(0)"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE " & tblname & " SET " & NewName & "=" & colname
                msql &= " WHERE " & colname & " IS NOT NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Drop the old column
                msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Rename new column
                msql = "sp_rename '" & tblname & "." & NewName & "', '" & colname & "', 'COLUMN'"
                ret = ExequteSQLquery(msql, connstr, connprv)
            ElseIf userODBCdriver.ToUpper.StartsWith("CACHE") OrElse userODBCdriver.ToUpper.StartsWith("IRIS") Then
                'NOT IN USE
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " DECIMAL NOT NULL "
                msql &= "CONSTRAINT DF_" & tblname & "_" & NewName & " DEFAULT(0)"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE " & tblname & " SET " & NewName & "=" & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Drop the old column
                msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Rename new column
                msql = "ALTER TABLE " & tblname & " MODIFY " & NewName & " RENAME " & colname & ""
                ret = ExequteSQLquery(msql, connstr, connprv)
            ElseIf userODBCdriver.ToUpper.StartsWith("PSQL") Then  'PostgreSQL  Npgsql
                'Dim db As String = GetDataBase(connstr)
                'Add new column
                msql = " ALTER TABLE `" & tblname & "` ADD `" & NewName & "` decimal(10,4) NOT NULL DEFAULT 0"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE `" & tblname & "` SET `" & NewName & "`=`" & colname & "`"
                msql &= " WHERE `" & colname & "` IS NOT NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Drop the old column
                msql = "ALTER TABLE `" & tblname & "` DROP COLUMN `" & colname & "`"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Rename new column
                msql = "ALTER TABLE `" & tblname & "` RENAME COLUMN '" & NewName & "' TO `" & colname & "`"
                ret = ExequteSQLquery(msql, connstr, connprv)
            End If

        ElseIf newtype = "date" Then
            If userODBCdriver.ToUpper.StartsWith("ORACLE") Then
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " DATE"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE " & tblname & " SET " & NewName & "=" & colname
                msql &= " WHERE " & colname & " IS NOT NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Drop the old column
                msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Rename new column
                msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & NewName & " To " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
            ElseIf userODBCdriver.ToUpper.StartsWith("SQL") Then
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " DATE"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE " & tblname & " SET " & NewName & "=" & colname
                msql &= " WHERE " & colname & " IS NOT NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Drop the old column
                msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Rename new column
                msql = "sp_rename '" & tblname & "." & NewName & "', '" & colname & "', 'COLUMN'"
                ret = ExequteSQLquery(msql, connstr, connprv)
            ElseIf userODBCdriver.ToUpper.StartsWith("CACHE") OrElse userODBCdriver.ToUpper.StartsWith("IRIS") Then
                'NOT IN USE
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " DATE NOT NULL "
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE " & tblname & " SET " & NewName & "=" & colname
                msql &= " WHERE " & colname & " IS NOT NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Drop the old column
                msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Rename new column
                msql = "ALTER TABLE " & tblname & " MODIFY " & NewName & " RENAME " & colname & ""
                ret = ExequteSQLquery(msql, connstr, connprv)

            ElseIf userODBCdriver.ToUpper.StartsWith("PSQL") Then  'PostgreSQL  Npgsql
                Dim db As String = GetDataBase(connstr, "Npgsql")
                'Add new column
                msql = " ALTER TABLE `" & tblname & "` ADD `" & NewName & "` DATE "
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE `" & tblname & "` SET `" & NewName & "`=`" & colname & "`"
                msql &= " WHERE `" & colname & "` IS NOT NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Drop the old column
                msql = "ALTER TABLE `" & tblname & "` DROP COLUMN `" & colname & "`"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Rename new column
                msql = "ALTER TABLE `" & tblname & "` RENAME COLUMN '" & NewName & "' TO `" & colname & "`"
                ret = ExequteSQLquery(msql, connstr, connprv)
            End If
        End If
        Return ret
    End Function
    Public Function RetypeAndReplaceColumn(tblname As String, colname As String, newtype As String, connstr As String, connprv As String) As String
        'ONLY FOR PostgreSQL, ORACLE and SQL Server
        Dim ret As String = ""
        Dim res As String = ""
        Dim NewName As String = colname & "new"   '& newtype
        Dim msql As String = String.Empty

        If connprv = "Oracle.ManagedDataAccess.Client" Then NewName = NewName.Replace("""", "")
        '========================================================NUMERIC======================================================
        If newtype = "num" Then
            If connprv = "Oracle.ManagedDataAccess.Client" Then
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " FLOAT(126) Default 0 Not NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE " & tblname & " Set " & NewName & "=" & colname
                msql &= " WHERE " & colname & " Is Not NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Drop the old column
                msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Rename new column
                msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & NewName & " To " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
            ElseIf connprv = "System.Data.SqlClient" Then
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " FLOAT Not NULL "
                msql &= "CONSTRAINT DF_" & tblname & "_" & NewName & " Default(0)"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE " & tblname & " Set " & NewName & "=" & colname
                msql &= " WHERE " & colname & " Is Not NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Drop the old column
                msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Rename new column
                msql = "sp_rename '" & tblname & "." & NewName & "', '" & colname & "', 'COLUMN'"
                ret = ExequteSQLquery(msql, connstr, connprv)
            ElseIf connprv = "Sqlite" Then
                'if primary key continue
                If colname = IndexFieldInTable(tblname, GetDataBase(connstr, connprv), connstr, connprv, ret) OrElse colname.StartsWith("ID") OrElse colname = "Indx" OrElse colname = "idx" Then
                    Return ""
                End If
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " FLOAT  NOT NULL DEFAULT '0'"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE " & tblname & " Set " & NewName & "=" & colname
                msql &= " WHERE " & colname & " Is Not NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Drop the old column
                msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Rename new column
                msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & NewName & " TO " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
            ElseIf connprv.StartsWith("InterSystems.Data") Then
                'NOT IN USE
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " DECIMAL NOT NULL "
                msql &= "CONSTRAINT DF_" & tblname & "_" & NewName & " DEFAULT(0)"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE " & tblname & " SET " & NewName & "=" & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Drop the old column
                msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Rename new column
                'ALTER TABLE tablename ALTER COLUMN oldname RENAME newname
                msql = "ALTER TABLE " & tblname & " MODIFY " & NewName & " RENAME " & colname & ""
                ret = ExequteSQLquery(msql, connstr, connprv)
            ElseIf connprv = "Npgsql" Then  'PostgreSQL  Npgsql
                'Dim db As String = "public" 'GetDataBase(connstr)
                'Add new column
                msql = " ALTER TABLE `" & tblname & "` ADD `" & NewName & "` decimal(20,4) NOT NULL DEFAULT 0"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE `" & tblname & "` SET `" & NewName & "`=CAST(`" & colname & "` AS DECIMAL)"
                msql &= " WHERE `" & colname & "` IS NOT NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Drop the old column
                msql = "ALTER TABLE `" & tblname & "` DROP COLUMN `" & colname & "`"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Rename new column
                msql = "ALTER TABLE `" & tblname & "` RENAME COLUMN `" & NewName & "` TO `" & colname & "`"
                ret = ExequteSQLquery(msql, connstr, connprv)
            End If

            '========================================================DATE======================================================
        ElseIf newtype = "date" Then
            If connprv = "Oracle.ManagedDataAccess.Client" Then
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " TIMESTAMP"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE " & tblname & " SET " & NewName & "=" & colname
                msql &= " WHERE " & colname & " IS NOT NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Drop the old column
                msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Rename new column
                msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & NewName & " To " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
            ElseIf connprv = "System.Data.SqlClient" Then
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " DATE NOT NULL "
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE " & tblname & " SET " & NewName & "=" & colname
                msql &= " WHERE " & colname & " IS NOT NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Drop the old column
                msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Rename new column
                msql = "sp_rename '" & tblname & "." & NewName & "', '" & colname & "', 'COLUMN'"
                ret = ExequteSQLquery(msql, connstr, connprv)
            ElseIf connprv = "Sqlite" Then
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " DATE"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE " & tblname & " Set " & NewName & "=" & colname
                msql &= " WHERE " & colname & " Is Not NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Drop the old column
                msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Rename new column
                msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & NewName & " TO " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
            ElseIf connprv.StartsWith("InterSystems.Data") Then
                'NOT IN USE
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " DATETIME NOT NULL "
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE " & tblname & " SET " & NewName & "=" & colname
                msql &= " WHERE " & colname & " IS NOT NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Drop the old column
                msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Rename new column
                msql = "ALTER TABLE " & tblname & " MODIFY " & NewName & " RENAME " & colname & ""
                ret = ExequteSQLquery(msql, connstr, connprv)
            ElseIf connprv = "Npgsql" Then  'PostgreSQL  Npgsql
                ' Dim db As String = "public" 'GetDataBase(connstr)
                'Add new column
                msql = " ALTER TABLE `" & tblname & "` ADD `" & NewName & "` DATE "
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE `" & tblname & "` SET `" & NewName & "`=CAST(`" & colname & "` AS DATE) "
                msql &= " WHERE `" & colname & "` IS NOT NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Drop the old column
                msql = "ALTER TABLE `" & tblname & "` DROP COLUMN `" & colname & "`"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Rename new column
                msql = "ALTER TABLE `" & tblname & "` RENAME COLUMN `" & NewName & "` TO `" & colname & "`"
                ret = ExequteSQLquery(msql, connstr, connprv)
            End If

            '========================================================STRING======================================================
        ElseIf newtype = "str" Then
            'MySql  varchar(4000) DEFAULT NULL
            Dim collen As String = "4000"
            If connprv = "Oracle.ManagedDataAccess.Client" Then
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " VARCHAR2(" & collen & " CHAR) DEFAULT NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE " & tblname & " SET " & NewName & "=" & colname
                msql &= " WHERE " & colname & " IS NOT NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                If ret = "Query executed fine." Then
                    'Drop the old column
                    msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                    ret = ExequteSQLquery(msql, connstr, connprv)
                    'Rename new column
                    msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & NewName & " To " & colname
                    ret = ExequteSQLquery(msql, connstr, connprv)
                Else
                    'Drop the new column
                    res = ret
                    msql = "ALTER TABLE " & tblname & " DROP COLUMN " & NewName
                    ret = ExequteSQLquery(msql, connstr, connprv)
                    Return "ERROR!! " & res
                End If
            ElseIf connprv = "System.Data.SqlClient" Then
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " TEXT NULL "
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE " & tblname & " SET " & NewName & "=" & colname
                msql &= " WHERE " & colname & " IS NOT NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                If ret = "Query executed fine." Then
                    'Drop the old column
                    msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                    ret = ExequteSQLquery(msql, connstr, connprv)
                    'Rename new column
                    msql = "sp_rename '" & tblname & "." & NewName & "', '" & colname & "', 'COLUMN'"
                    ret = ExequteSQLquery(msql, connstr, connprv)
                Else
                    'Drop the new column
                    res = ret
                    msql = "ALTER TABLE " & tblname & " DROP COLUMN " & NewName
                    ret = ExequteSQLquery(msql, connstr, connprv)
                    Return "ERROR!! " & res
                End If
            ElseIf connprv = "Sqlite" Then
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " TEXT NULL "
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE " & tblname & " Set " & NewName & "=" & colname
                msql &= " WHERE " & colname & " Is Not NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Drop the old column
                msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Rename new column
                msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & NewName & " TO " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
            ElseIf connprv.StartsWith("InterSystems.Data") Then
                'NOT IN USE
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & "  NVARCHAR(" & collen & ") NULL  "
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE " & tblname & " SET " & NewName & "=" & colname
                msql &= " WHERE " & colname & " IS NOT NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                If ret = "Query executed fine." Then
                    'Drop the old column
                    msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                    ret = ExequteSQLquery(msql, connstr, connprv)
                    'Rename new column
                    msql = "ALTER TABLE " & tblname & " MODIFY " & NewName & " RENAME " & colname & ""
                    ret = ExequteSQLquery(msql, connstr, connprv)
                Else
                    'Drop the new column
                    res = ret
                    msql = "ALTER TABLE " & tblname & " DROP COLUMN " & NewName
                    ret = ExequteSQLquery(msql, connstr, connprv)
                    Return "ERROR!! " & res
                End If
            ElseIf connprv = "Npgsql" Then  'PostgreSQL  Npgsql
                ' Dim db As String = "public" 'GetDataBase(connstr)
                'Add new column
                msql = " ALTER TABLE `" & tblname & "` ADD `" & NewName & "` character varying(" & collen & ") NULL "
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Populate new column
                msql = "UPDATE `" & tblname & "` SET `" & NewName & "`=CAST(`" & colname & "` AS character varying) "
                msql &= " WHERE `" & colname & "` IS NOT NULL"
                ret = ExequteSQLquery(msql, connstr, connprv)
                If ret = "Query executed fine." Then
                    'Drop the old column
                    msql = "ALTER TABLE `" & tblname & "` DROP COLUMN `" & colname & "`"
                    ret = ExequteSQLquery(msql, connstr, connprv)
                    'Rename new column
                    msql = "ALTER TABLE `" & tblname & "` RENAME COLUMN `" & NewName & "` TO `" & colname & "`"
                    ret = ExequteSQLquery(msql, connstr, connprv)
                Else
                    'Drop the new column
                    res = ret
                    msql = "ALTER TABLE " & tblname & " DROP COLUMN " & NewName
                    ret = ExequteSQLquery(msql, connstr, connprv)
                    Return "ERROR!! " & res
                End If
            End If

            '====================================================================================================
        ElseIf newtype = "pk" Then  'primary key
            If connprv = "Oracle.ManagedDataAccess.Client" Then
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " INTEGER GENERATED ALWAYS AS IDENTITY"
                ret = ExequteSQLquery(msql, connstr, connprv)
                ''Populate new column
                'msql = "UPDATE " & tblname & " SET " & NewName & "=" & colname
                'msql &= " WHERE " & colname & " IS NOT NULL"
                'ret = ExequteSQLquery(msql, connstr, connprv)
                If ret = "Query executed fine." Then
                    'Drop the old column
                    msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                    ret = ExequteSQLquery(msql, connstr, connprv)
                    'Rename new column
                    msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & NewName & " To " & colname
                    ret = ExequteSQLquery(msql, connstr, connprv)
                Else
                    'Drop the new column
                    res = ret
                    msql = "ALTER TABLE " & tblname & " DROP COLUMN " & NewName
                    ret = ExequteSQLquery(msql, connstr, connprv)
                    Return "ERROR!! " & res
                End If
            ElseIf connprv = "System.Data.SqlClient" Then
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " [int] IDENTITY(1,1) NOT NULL "
                ret = ExequteSQLquery(msql, connstr, connprv)
                ''Populate new column
                'msql = "UPDATE " & tblname & " SET " & NewName & "=" & colname
                'msql &= " WHERE " & colname & " IS NOT NULL"
                'ret = ExequteSQLquery(msql, connstr, connprv)
                If ret = "Query executed fine." Then
                    'Drop the old column
                    msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                    ret = ExequteSQLquery(msql, connstr, connprv)
                    'Rename new column
                    msql = "sp_rename '" & tblname & "." & NewName & "', '" & colname & "', 'COLUMN'"
                    ret = ExequteSQLquery(msql, connstr, connprv)
                Else
                    'Drop the new column
                    res = ret
                    msql = "ALTER TABLE " & tblname & " DROP COLUMN " & NewName
                    ret = ExequteSQLquery(msql, connstr, connprv)
                    Return "ERROR!! " & res
                End If
            ElseIf connprv = "Sqlite" Then
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " INTEGER PRIMARY KEY AUTOINCREMENT "
                ret = ExequteSQLquery(msql, connstr, connprv)
                ''Populate new column
                'msql = "UPDATE " & tblname & " Set " & NewName & "=" & colname
                'msql &= " WHERE " & colname & " Is Not NULL"
                'ret = ExequteSQLquery(msql, connstr, connprv)
                'Drop the old column
                msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
                'Rename new column
                msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & NewName & " TO " & colname
                ret = ExequteSQLquery(msql, connstr, connprv)
            ElseIf connprv.StartsWith("InterSystems.Data") Then
                'NOT IN USE
                'Add new column
                msql = " ALTER TABLE " & tblname & " ADD " & NewName & " [int] IDENTITY(1,1) NOT NULL "
                ret = ExequteSQLquery(msql, connstr, connprv)
                ''Populate new column
                'msql = "UPDATE " & tblname & " SET " & NewName & "=" & colname
                'msql &= " WHERE " & colname & " IS NOT NULL"
                'ret = ExequteSQLquery(msql, connstr, connprv)
                If ret = "Query executed fine." Then
                    'Drop the old column
                    msql = "ALTER TABLE " & tblname & " DROP COLUMN " & colname
                    ret = ExequteSQLquery(msql, connstr, connprv)
                    'Rename new column
                    msql = "ALTER TABLE " & tblname & " MODIFY " & NewName & " RENAME " & colname & ""
                    ret = ExequteSQLquery(msql, connstr, connprv)
                Else
                    'Drop the new column
                    res = ret
                    msql = "ALTER TABLE " & tblname & " DROP COLUMN " & NewName
                    ret = ExequteSQLquery(msql, connstr, connprv)
                    Return "ERROR!! " & res
                End If
            ElseIf connprv = "Npgsql" Then  'PostgreSQL  Npgsql
                ' Dim db As String = "public" 'GetDataBase(connstr)
                'Add new column
                msql = " ALTER TABLE `" & tblname & "` ADD `" & NewName & "`  integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ) "
                ret = ExequteSQLquery(msql, connstr, connprv)
                ''Populate new column
                'msql = "UPDATE `" & tblname & "` SET `" & NewName & "`=CAST(`" & colname & "` AS character varying) "
                'msql &= " WHERE `" & colname & "` IS NOT NULL"
                'ret = ExequteSQLquery(msql, connstr, connprv)
                If ret = "Query executed fine." Then
                    'Drop the old column
                    msql = "ALTER TABLE `" & tblname & "` DROP COLUMN `" & colname & "`"
                    ret = ExequteSQLquery(msql, connstr, connprv)
                    'Rename new column
                    msql = "ALTER TABLE `" & tblname & "` RENAME COLUMN `" & NewName & "` TO `" & colname & "`"
                    ret = ExequteSQLquery(msql, connstr, connprv)
                Else
                    'Drop the new column
                    res = ret
                    msql = "ALTER TABLE " & tblname & " DROP COLUMN " & NewName
                    ret = ExequteSQLquery(msql, connstr, connprv)
                    Return "ERROR!! " & res
                End If
            End If

        End If
        Return ret
    End Function

    Public Function ChangeColumnLength(ByVal tblname As String, ByVal colname As String, ByVal collen As String, ByVal userconnstr As String, ByVal userconnprv As String, Optional ByVal exst As Boolean = False) As String
        'TODO how to change only length not other setting, default, etc...
        Dim ret As String = String.Empty
        Dim msql As String = String.Empty
        Dim rer As String = String.Empty
        Dim db As String = GetDataBase(userconnstr, userconnprv).ToLower
        Dim userODBCdriver As String = String.Empty
        Dim userODBCdatabase As String = String.Empty
        Dim userODBCdatasource As String = String.Empty
        If userconnprv.StartsWith("System.Data.Odbc") Then
            userconnstr = userconnstr.Replace("Password", "Pwd").Replace("User ID", "UID")
            Dim bConnect As Boolean = DatabaseConnected(userconnstr, userconnprv, rer, userODBCdriver, userODBCdatabase, userODBCdatasource)
            If Not bConnect Then
                rer = "ERROR!! Database not connected. " & rer
                Return rer
            End If
        ElseIf userconnprv.StartsWith("System.Data.OleDb") Then
            'OleDb
            Dim bConnect As Boolean = DatabaseConnected(userconnstr, userconnprv, rer, userODBCdriver, userODBCdatabase, userODBCdatasource)
            If Not bConnect Then
                rer = "ERROR!! Database not connected. " & rer
                Return rer
            End If
        End If

        If userconnprv = "MySql.Data.MySqlClient" Then
            msql = "ALTER TABLE `" & db & "`.`" & tblname & "` CHANGE COLUMN `" & colname & "` `" & colname & "` VARCHAR(" & collen & ")  ;" 'NULL DEFAULT NULL ???

        ElseIf userconnprv = "Oracle.ManagedDataAccess.Client" Then
            msql = "ALTER TABLE " & tblname & " MODIFY " & colname & " VARCHAR2(" & collen & " CHAR)"

        ElseIf userconnprv.StartsWith("InterSystems.Data.") Then
            msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & FixReservedWords(colname, "InterSystems.Data.") & " VARCHAR(" & collen & ")"

        ElseIf userconnprv.StartsWith("System.Data.Odbc") Then
            If userODBCdriver.ToUpper.StartsWith("CACHE") OrElse userODBCdriver.StartsWith("IRIS") Then
                msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & colname & " VARCHAR(" & collen & ")"
            ElseIf userODBCdriver.ToUpper.StartsWith("SQL") Then
                msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & colname & " VARCHAR(" & collen & ")  NULL"
            ElseIf userODBCdriver.ToUpper.StartsWith("MYSQL") Then
                msql = "ALTER TABLE `" & db & "`.`" & tblname & "` CHANGE COLUMN `" & colname & "` `" & colname & "` VARCHAR(" & collen & ")  ;"  'NULL DEFAULT NULL
                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
            ElseIf userODBCdriver.ToUpper.StartsWith("ORACLE") Then
                msql = "ALTER TABLE " & tblname & " MODIFY " & colname & " VARCHAR2(" & collen & " CHAR)"
            ElseIf userODBCdriver.ToUpper.StartsWith("PSQL") Then  'PostgreSQL  Npgsql
                msql = "ALTER TABLE `" & tblname & "` ALTER COLUMN `" & colname & "` TYPE character varying(" & collen & ")  NULL"
            End If

        ElseIf userconnprv.StartsWith("System.Data.OleDb") Then
            If userODBCdriver.ToUpper.StartsWith("CACHE") OrElse userODBCdriver.StartsWith("IRIS") Then
                msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & colname & " VARCHAR(" & collen & ")"
            ElseIf userODBCdriver.ToUpper.StartsWith("SQL") Then
                msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & colname & " VARCHAR(" & collen & ")  NULL"
            ElseIf userODBCdriver.ToUpper.StartsWith("MYSQL") Then
                msql = "ALTER TABLE `" & db & "`.`" & tblname & "` CHANGE COLUMN `" & colname & "` `" & colname & "` VARCHAR(" & collen & ")  ;"  'NULL DEFAULT NULL
                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
            ElseIf userODBCdriver.ToUpper.StartsWith("ORACLE") Then
                msql = "ALTER TABLE " & tblname & " MODIFY " & colname & " VARCHAR2(" & collen & " CHAR)"
            ElseIf userODBCdriver.ToUpper.StartsWith("PSQL") Then  'PostgreSQL  Npgsql
                msql = "ALTER TABLE `" & tblname & "` ALTER COLUMN `" & colname & "` TYPE character varying(" & collen & ")  NULL"
            End If

        ElseIf userconnprv = "Npgsql" Then  'PostgreSQL  Npgsql
            msql = "ALTER TABLE `" & tblname & "` ALTER COLUMN `" & colname & "` TYPE character varying(" & collen & ")  NULL"

        Else 'sql server
            msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & colname & " VARCHAR(" & collen & ")  NULL"
        End If
        ret = ExequteSQLquery(msql, userconnstr, userconnprv)
        ' WriteToAccessLog(tblname, "column changed: " & msql & " - with result: " & ret, 111)
        Return ret
    End Function
    Public Function CorrectFieldsInTable(ByVal tblname As String, ByVal userconnstr As String, ByVal userconnprv As String, Optional ByVal exst As Boolean = False) As String
        'ON HOLD
        Dim ret As String = String.Empty
        Dim er As String = String.Empty
        Dim colname As String = String.Empty
        Dim datatype As String = String.Empty
        Dim i As Integer
        Dim dtf As New DataTable
        Try
            dtf = GetListOfTableFields(tblname, userconnstr, userconnprv, er).Table

            For i = 0 To dtf.Rows.Count - 1
                colname = dtf.Rows(i)("COLUMN_NAME").ToString
                datatype = dtf.Rows(i)("DATA_TYPE").ToString
                If Not (colname.ToUpper = "ID" Or colname.ToUpper = "INDX") Then
                    ret = MakeTableColumnNameTypeCLScompliant(tblname, colname, datatype, userconnstr, userconnprv, er)
                End If
            Next

            ret = CorrectFieldTypesInTable(tblname, userconnstr, userconnprv, True)

        Catch ex As Exception
            Return "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function

    Public Function CorrectFieldTypesInTable(ByVal tblname As String, ByVal userconnstr As String, ByVal userconnprv As String, Optional ByVal exst As Boolean = False) As String
        Dim ret As String = String.Empty
        Dim msql As String = String.Empty
        Dim n As Integer = 0
        Dim i As Integer
        Dim j As Integer
        Dim dvcsv As DataView
        Try
            'find field types
            Dim dv As DataView = mRecords("Select * FROM [" & tblname & "]", ret, userconnstr, userconnprv)
            If dv Is Nothing OrElse dv.Count = 0 OrElse dv.Table.Rows.Count = 0 Then
                Return "ERROR!! " & ret
            End If
            dvcsv = MakeDTColumnsNamesCLScompliant(dv.Table, userconnprv, ret).DefaultView

            Dim db As String
            'If userconnprv.StartsWith("InterSystems.Data.") Then
            '    'not needed here - tblname comes with namespace attached already...
            '    db = GetNamespaceFromConnectionString(userconnstr)
            'Else
            db = GetDataBase(userconnstr, userconnprv)
            If userconnprv = "MySql.Data.MySqlClient" Then
                db = db.ToLower
            End If
            n = dvcsv.Table.Columns.Count
            Dim coltypes(n - 1) As String
            Dim coldates(n - 1) As String
            Dim collens(n - 1) As String
            'initial types
            For i = 0 To n - 1
                collens(i) = "12"
                coltypes(i) = "num"
                coldates(i) = "date"
            Next
            'Loop through all rows
            Dim m As Integer = dvcsv.Table.Rows.Count
            For i = 0 To n - 1
                For j = 0 To m - 1
                    If Not dvcsv.Table.Rows(j)(i) Is Nothing AndAlso dvcsv.Table.Rows(j)(i).ToString.Length > CInt(collens(i)) Then
                        collens(i) = dvcsv.Table.Rows(j)(i).ToString.Length.ToString
                    End If

                    If Not dvcsv.Table.Rows(j)(i) Is Nothing AndAlso dvcsv.Table.Rows(j)(i).ToString <> "" Then
                        If dvcsv.Table.Rows(j)(i).ToString.EndsWith("K") AndAlso IsNumeric(dvcsv.Table.Rows(j)(i).ToString.Replace("K", "")) Then
                            Continue For
                        ElseIf dvcsv.Table.Rows(j)(i).ToString.EndsWith("M") AndAlso IsNumeric(dvcsv.Table.Rows(j)(i).ToString.Replace("M", "")) Then
                            Continue For
                        End If

                        If Not IsNumeric(dvcsv.Table.Rows(j)(i).ToString) Then
                            coltypes(i) = "str"
                        ElseIf IsNumeric(dvcsv.Table.Rows(j)(i).ToString) AndAlso dvcsv.Table.Rows(j)(i).ToString <> "0" AndAlso dvcsv.Table.Rows(j)(i).ToString.StartsWith("0") AndAlso Not dvcsv.Table.Rows(j)(i).ToString.StartsWith("0.") Then
                            coltypes(i) = "str"
                        ElseIf IsNumeric(dvcsv.Table.Rows(j)(i).ToString) AndAlso Not dvcsv.Table.Rows(j)(i).ToString.StartsWith("0") Then
                            Continue For
                        End If

                        If IsDate(dvcsv.Table.Rows(j)(i).ToString) AndAlso Not dvcsv.Table.Columns(i).ColumnName.ToUpper.Contains("ID") AndAlso Not dvcsv.Table.Columns(i).ColumnName = "Indx" Then
                            '(dvcsv.Table.Columns(i).ColumnName.ToUpper.Contains("DATE") OrElse dvcsv.Table.Columns(i).ColumnName.ToUpper.Contains("TIME")) AndAlso OrElse (IsNumeric(dvcsv.Table.Rows(j)(i).ToString)
                            Dim dtReturn As New DateTime
                            If Not IsDateTimeValue(dvcsv.Table.Rows(j)(i).ToString, dtReturn) Then
                                coldates(i) = ""
                            End If
                            'coltypes(i) = "date"
                        End If

                        If dvcsv.Table.Rows(j)(i).ToString.Trim <> "" AndAlso Not IsDate(dvcsv.Table.Rows(j)(i).ToString) Then 'AndAlso Not IsNumeric(dvcsv.Table.Rows(j)(i).ToString) Then
                            coldates(i) = ""
                        End If
                    End If
                Next
            Next

            ''!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            'Dim db As String = GetDataBase(userconnstr).ToLower
            ''Allow Sql Server to automatically create statistics
            'msql = "ALTER DATABASE " & db & " Set AUTO_CREATE_STATISTICS On "
            'ret = ExequteSQLquery(msql, userconnstr, userconnprv)
            'msql = "ALTER DATABASE " & db & " Set AUTO_UPDATE_STATISTICS On "
            'ret = ExequteSQLquery(msql, userconnstr, userconnprv)
            ''!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            'Loop through all columns to correct fields null values, at this point all fields are still text type
            For i = 0 To n - 1
                If coltypes(i) = "str" Then
                    If userconnprv = "MySql.Data.MySqlClient" Then
                        msql = "UPDATE `" & db & "`.`" & tblname & "` Set `" & dvcsv.Table.Columns(i).Caption & "`='' WHERE `" & dvcsv.Table.Columns(i).Caption & "` IS NULL "
                    ElseIf userconnprv = "Npgsql" Then
                        msql = "UPDATE `" & tblname & "` Set `" & dvcsv.Table.Columns(i).Caption & "`='' WHERE `" & dvcsv.Table.Columns(i).Caption & "` IS NULL "
                    Else
                        msql = "UPDATE " & tblname & " SET " & dvcsv.Table.Columns(i).Caption & "='' WHERE " & dvcsv.Table.Columns(i).Caption & " IS NULL "
                    End If
                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                ElseIf coltypes(i) = "num" Then
                    If userconnprv = "MySql.Data.MySqlClient" Then
                        msql = "UPDATE `" & db & "`.`" & tblname & "` SET `" & dvcsv.Table.Columns(i).Caption & "`=0 WHERE `" & dvcsv.Table.Columns(i).Caption & "` IS NULL OR `" & dvcsv.Table.Columns(i).Caption & "`=''"
                    ElseIf userconnprv = "Npgsql" Then
                        msql = "UPDATE `" & tblname & "` SET `" & dvcsv.Table.Columns(i).Caption & "`='0' WHERE `" & dvcsv.Table.Columns(i).Caption & "` IS NULL OR `" & dvcsv.Table.Columns(i).Caption & "`=''"
                    Else
                        msql = "UPDATE " & tblname & " SET " & dvcsv.Table.Columns(i).Caption & "=0 WHERE " & dvcsv.Table.Columns(i).Caption & " IS NULL OR " & dvcsv.Table.Columns(i).Caption & "=''"
                    End If
                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)

                    If userconnprv = "MySql.Data.MySqlClient" Then
                        msql = "UPDATE `" & db & "`.`" & tblname & "` SET `" & dvcsv.Table.Columns(i).Caption & "`=REPLACE(`" & dvcsv.Table.Columns(i).Caption & "`,',','') WHERE `" & dvcsv.Table.Columns(i).Caption & "` LIKE '%,%'"
                    ElseIf userconnprv = "Npgsql" Then
                        msql = "UPDATE `" & tblname & "` SET `" & dvcsv.Table.Columns(i).Caption & "`=REPLACE(`" & dvcsv.Table.Columns(i).Caption & "`,',','') WHERE `" & dvcsv.Table.Columns(i).Caption & "` LIKE '%,%'"
                    Else
                        msql = "UPDATE " & tblname & " SET " & dvcsv.Table.Columns(i).Caption & "=REPLACE(" & dvcsv.Table.Columns(i).Caption & ",',','') WHERE " & dvcsv.Table.Columns(i).Caption & " LIKE '%,%'"
                    End If
                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)

                    If userconnprv = "MySql.Data.MySqlClient" Then
                        msql = "UPDATE `" & db & "`.`" & tblname & "` SET `" & dvcsv.Table.Columns(i).Caption & "`=REPLACE(`" & dvcsv.Table.Columns(i).Caption & "`,'$','') WHERE `" & dvcsv.Table.Columns(i).Caption & "` LIKE '%$%'"
                    ElseIf userconnprv = "Npgsql" Then
                        msql = "UPDATE `" & tblname & "` SET `" & dvcsv.Table.Columns(i).Caption & "`=REPLACE(`" & dvcsv.Table.Columns(i).Caption & "`,'$','') WHERE `" & dvcsv.Table.Columns(i).Caption & "` LIKE '%$%'"
                    Else
                        msql = "UPDATE " & tblname & " SET " & dvcsv.Table.Columns(i).Caption & "=REPLACE(" & dvcsv.Table.Columns(i).Caption & ",'$','') WHERE " & dvcsv.Table.Columns(i).Caption & " LIKE '%$%'"
                    End If
                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)

                    If userconnprv = "MySql.Data.MySqlClient" Then
                        msql = "UPDATE `" & db & "`.`" & tblname & "` SET `" & dvcsv.Table.Columns(i).Caption & "`=TRIM(`" & dvcsv.Table.Columns(i).Caption & "`)"
                    ElseIf userconnprv = "Npgsql" Then
                        msql = "UPDATE `" & tblname & "` SET [" & dvcsv.Table.Columns(i).Caption & "]=TRIM([" & dvcsv.Table.Columns(i).Caption & "])"
                    Else
                        msql = "UPDATE " & tblname & " SET " & dvcsv.Table.Columns(i).Caption & "=TRIM(" & dvcsv.Table.Columns(i).Caption & ")"
                    End If
                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)

                    If userconnprv = "MySql.Data.MySqlClient" Then
                        msql = "UPDATE `" & db & "`.`" & tblname & "` SET `" & dvcsv.Table.Columns(i).Caption & "`=1000*REPLACE(`" & dvcsv.Table.Columns(i).Caption & "`,'K','') WHERE `" & dvcsv.Table.Columns(i).Caption & "` LIKE '%K'"

                    ElseIf userconnprv = "Npgsql" Then
                        msql = "UPDATE `" & tblname & "` SET `" & dvcsv.Table.Columns(i).Caption & "`=REPLACE(`" & dvcsv.Table.Columns(i).Caption & "`,'K','*1000') WHERE `" & dvcsv.Table.Columns(i).Caption & "` LIKE '%K'"

                    Else
                        msql = "UPDATE " & tblname & " SET " & dvcsv.Table.Columns(i).Caption & "=1000*REPLACE(" & dvcsv.Table.Columns(i).Caption & ",'K','') WHERE " & dvcsv.Table.Columns(i).Caption & " LIKE '%K'"
                    End If
                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)

                    If userconnprv = "MySql.Data.MySqlClient" Then
                        msql = "UPDATE `" & db & "`.`" & tblname & "` SET `" & dvcsv.Table.Columns(i).Caption & "`=1000000*REPLACE(`" & dvcsv.Table.Columns(i).Caption & "`,'M','') WHERE `" & dvcsv.Table.Columns(i).Caption & "` LIKE '%M'"
                    ElseIf userconnprv = "Npgsql" Then
                        msql = "UPDATE `" & tblname & "` SET `" & dvcsv.Table.Columns(i).Caption & "`=REPLACE(`" & dvcsv.Table.Columns(i).Caption & "`,'M','*1000000') WHERE `" & dvcsv.Table.Columns(i).Caption & "` LIKE '%M'"

                    Else
                        msql = "UPDATE " & tblname & " SET " & dvcsv.Table.Columns(i).Caption & "=1000000*REPLACE(" & dvcsv.Table.Columns(i).Caption & ",'M','') WHERE " & dvcsv.Table.Columns(i).Caption & " LIKE '%M'"
                    End If
                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)

                    If userconnprv = "MySql.Data.MySqlClient" Then
                        msql = "UPDATE `" & db & "`.`" & tblname & "` SET `" & dvcsv.Table.Columns(i).Caption & "`=REPLACE(`" & dvcsv.Table.Columns(i).Caption & "`,',','')"
                    ElseIf userconnprv = "Npgsql" Then
                        msql = "UPDATE `" & tblname & "` SET `" & dvcsv.Table.Columns(i).Caption & "`=REPLACE(`" & dvcsv.Table.Columns(i).Caption & "`,',','')"
                    Else
                        msql = "UPDATE `" & tblname & "` SET `" & dvcsv.Table.Columns(i).Caption & "`=REPLACE(`" & dvcsv.Table.Columns(i).Caption & "`,',','')"
                    End If
                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)

                End If
            Next

            'Loop through all columns to correct field types
            Dim col As String = String.Empty
            Dim colname As String = String.Empty

            Dim rer As String = String.Empty
            Dim userODBCdriver As String = String.Empty
            Dim userODBCdatabase As String = String.Empty
            Dim userODBCdatasource As String = String.Empty
            If userconnprv.StartsWith("System.Data.Odbc") Then
                userconnstr = userconnstr.Replace("Password", "Pwd").Replace("User ID", "UID")
                Dim bConnect As Boolean = DatabaseConnected(userconnstr, userconnprv, rer, userODBCdriver, userODBCdatabase, userODBCdatasource)
                If Not bConnect Then
                    rer = "ERROR!! Database not connected. " & rer
                    Return rer
                End If
            ElseIf userconnprv.StartsWith("System.Data.OleDb") Then
                'OleDb
                Dim bConnect As Boolean = DatabaseConnected(userconnstr, userconnprv, rer, userODBCdriver, userODBCdatabase, userODBCdatasource)
                If Not bConnect Then
                    rer = "ERROR!! Database not connected. " & rer
                    Return rer
                End If
            End If

            'Columns types
            For i = 0 To n - 1
                colname = dvcsv.Table.Columns(i).Caption
                'col = colname.Replace("_", " ").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace(",", "_").Trim 'fix column name
                col = Regex.Replace(colname, "[^a-zA-Z0-9_]", "")  'CLS-compliant
                col = cleanText(FixReservedWords(col, userconnprv))
                If userconnprv.StartsWith("InterSystems.Data.") Then
                    col = col.Replace("_", "")
                End If

                If coltypes(i) = "num" Then
                    If colname = "Indx" OrElse colname.ToUpper = "ID" OrElse colname.ToUpper = "PARENTID" OrElse colname.ToUpper = "IDX" Then
                        Continue For
                    End If


                    If userconnprv = "MySql.Data.MySqlClient" Then
                        msql = "ALTER TABLE `" & db & "`.`" & tblname & "` CHANGE COLUMN `" & colname & "` `" & col & "` DOUBLE DEFAULT 0 ;"
                        ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        'WriteToAccessLog(tblname, "column changed: " & msql & " - with result: " & ret, 111)
                        Continue For
                    ElseIf userconnprv = "Oracle.ManagedDataAccess.Client" Then
                        If col <> colname Then
                            msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        End If
                        ret = RetypeAndReplaceColumn(tblname, col, "num", userconnstr, userconnprv)
                        Continue For
                        ' msql = "ALTER TABLE " & tblname & " MODIFY " & col & " FLOAT(126) DEFAULT 0 NOT NULL"
                    ElseIf userconnprv.StartsWith("InterSystems.Data.") Then
                        If col <> colname Then
                            msql = "UPDATE %Dictionary.PropertyDefinition SET Name='" & col & "' "
                            msql &= "WHERE parent='" & tblname & "' AND Name='" & colname & "'"
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        End If
                        msql = "ALTER TABLE " & tblname & " MODIFY " & col & " DECIMAL DEFAULT(0) "
                        ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        'RetypeAndReplaceColumn(tblname, col, "num", userconnstr, userconnprv)
                        Continue For
                    ElseIf userconnprv.StartsWith("System.Data.Odbc") Then
                        If col <> colname Then
                            msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            If ret = "Query executed fine." Then
                                colname = col
                            End If
                        End If
                        If userODBCdriver.ToUpper.StartsWith("CACHE") OrElse userODBCdriver.StartsWith("IRIS") Then
                            If col <> colname Then
                                msql = "UPDATE %Dictionary.PropertyDefinition SET Name='" & FixReservedWords(col, "InterSystems.Data.") & "' "
                                msql &= "WHERE parent='" & tblname & "' AND Name='" & colname & "'"
                                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            End If
                            msql = "ALTER TABLE " & tblname & " MODIFY " & col & " DECIMAL DEFAULT(0) "
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            'RetypeAndReplaceColumnODBC(tblname, col, "num", userconnstr, userconnprv)
                        ElseIf userODBCdriver.ToUpper.StartsWith("SQL") Then
                            If col <> colname Then
                                msql = "sp_rename '" & tblname & "." & colname & "', '" & col & "', 'COLUMN' "
                                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            End If
                            RetypeAndReplaceColumnODBC(tblname, col, "num", userconnstr, userconnprv, ret, userODBCdriver)
                        ElseIf userODBCdriver.ToUpper.StartsWith("MYSQL") Then
                            msql = "ALTER TABLE `" & db & "`.`" & tblname & "` CHANGE COLUMN `" & colname & "` `" & col & "` FLOAT NOT NULL DEFAULT 0 ;"
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        ElseIf userODBCdriver.ToUpper.StartsWith("ORACLE") Then
                            If col <> colname Then
                                msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            End If
                            RetypeAndReplaceColumnODBC(tblname, col, "num", userconnstr, userconnprv, ret, userODBCdriver)

                        ElseIf userODBCdriver.ToUpper.StartsWith("PSQL") Then
                            If colname <> col Then
                                msql = "ALTER TABLE `" & tblname & "` RENAME COLUMN `" & colname & "` TO `" & col & "`;"
                                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            End If
                            RetypeAndReplaceColumnODBC(tblname, col, "num", userconnstr, userconnprv)

                        Else
                            If col <> colname Then
                                msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            End If
                        End If
                        Continue For
                    ElseIf userconnprv.StartsWith("System.Data.OleDb") Then
                        If col <> colname Then
                            msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            If ret = "Query executed fine." Then
                                colname = col
                            End If
                        End If
                        If userODBCdriver.ToUpper.StartsWith("CACHE") OrElse userODBCdriver.StartsWith("IRIS") Then
                            If col <> colname Then
                                msql = "UPDATE %Dictionary.PropertyDefinition SET Name='" & FixReservedWords(col, "InterSystems.Data.") & "' "
                                msql &= "WHERE parent='" & tblname & "' AND Name='" & colname & "'"
                                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            End If
                            msql = "ALTER TABLE " & tblname & " MODIFY " & col & " DECIMAL DEFAULT(0) "
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        ElseIf userODBCdriver.ToUpper.StartsWith("SQL") Then
                            If col <> colname Then
                                msql = "sp_rename '" & tblname & "." & colname & "', '" & col & "', 'COLUMN' "
                                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            End If
                            RetypeAndReplaceColumnODBC(tblname, col, "num", userconnstr, userconnprv, ret, userODBCdriver)
                        ElseIf userODBCdriver.ToUpper.StartsWith("MYSQL") Then
                            msql = "ALTER TABLE `" & db & "`.`" & tblname & "` CHANGE COLUMN `" & colname & "` `" & col & "` FLOAT NOT NULL DEFAULT 0 ;"
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        ElseIf userODBCdriver.ToUpper.StartsWith("ORACLE") Then
                            If col <> colname Then
                                msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            End If
                            RetypeAndReplaceColumnODBC(tblname, col, "num", userconnstr, userconnprv, ret, userODBCdriver)

                        ElseIf userODBCdriver.ToUpper.StartsWith("PSQL") Then
                            If colname <> col Then
                                msql = "ALTER TABLE `" & tblname & "` RENAME COLUMN `" & colname & "` TO `" & col & "`;"
                                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            End If
                            RetypeAndReplaceColumnODBC(tblname, col, "num", userconnstr, userconnprv)

                        Else
                            If col <> colname Then
                                msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            End If
                        End If
                        Continue For

                    ElseIf userconnprv = "Npgsql" Then  'PostgreSQL  Npgsql
                        If colname <> col Then
                            msql = "ALTER TABLE `" & tblname & "` RENAME COLUMN `" & colname & "` TO `" & col & "`;"
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        End If
                        ret = RetypeAndReplaceColumn(tblname, col, "num", userconnstr, userconnprv)
                        Continue For

                    ElseIf userconnprv = "Sqlite" Then
                        If colname <> col Then
                            msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col & ";"
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        End If
                        ret = RetypeAndReplaceColumn(tblname, col, "num", userconnstr, userconnprv)
                        Continue For

                    Else  'sql server
                        If col <> colname Then
                            msql = "sp_rename '" & tblname & "." & colname & "', '" & col & "', 'COLUMN'"
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        End If
                        ret = RetypeAndReplaceColumn(tblname, col, "num", userconnstr, userconnprv)
                        Continue For
                    End If

                ElseIf coldates(i) = "date" Then
                    'If userconnprv = "Sqlite" Then
                    '    msql = "ALTER TABLE " & tblname & " CHANGE COLUMN " & colname & " " & col & " DATETIME NOT NULL Default 0 ;"
                    '    ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '    ' WriteToAccessLog(tblname, "column changed: " & msql & " - with result: " & ret, 111)
                    '    Continue For
                    'Else
                    If userconnprv = "MySql.Data.MySqlClient" Then
                        msql = "ALTER TABLE `" & db & "`.`" & tblname & "` CHANGE COLUMN `" & colname & "` `" & col & "` DATETIME NOT NULL DEFAULT 0 ;"
                        ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        ' WriteToAccessLog(tblname, "column changed: " & msql & " - with result: " & ret, 111)
                        Continue For
                    ElseIf userconnprv = "Oracle.ManagedDataAccess.Client" Then
                        If col <> colname Then
                            msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        End If
                        ret = RetypeAndReplaceColumn(tblname, col, "date", userconnstr, userconnprv)
                        Continue For
                        'msql = "ALTER TABLE " & tblname & " MODIFY " & col & " DATE NOT NULL;"
                    ElseIf userconnprv.StartsWith("InterSystems.Data.") Then
                        If col <> colname Then
                            msql = "UPDATE %Dictionary.PropertyDefinition SET Name='" & col & "' "
                            msql &= "WHERE parent='" & tblname & "' AND Name='" & colname & "'"
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        End If
                        msql = "ALTER TABLE " & tblname & " MODIFY " & col & " DATETIME"
                        ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        Continue For
                    ElseIf userconnprv.StartsWith("System.Data.Odbc") Then
                        If col <> colname Then
                            msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            If ret = "Query executed fine." Then
                                colname = col
                            End If
                        End If
                        If userODBCdriver.ToUpper.StartsWith("CACHE") OrElse userODBCdriver.StartsWith("IRIS") Then
                            If col <> colname Then
                                msql = "UPDATE %Dictionary.PropertyDefinition SET Name='" & FixReservedWords(col, "InterSystems.Data.") & "' "
                                msql &= "WHERE parent='" & tblname & "' AND Name='" & colname & "'"
                                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            End If
                            msql = "ALTER TABLE " & tblname & " MODIFY " & col & " DATETIME"
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        ElseIf userODBCdriver.ToUpper.StartsWith("SQL") Then
                            If col <> colname Then
                                msql = "sp_rename '" & tblname & "." & colname & "', '" & col & "', 'COLUMN' "
                                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            End If
                            RetypeAndReplaceColumnODBC(tblname, col, "date", userconnstr, userconnprv, ret, userODBCdriver)
                        ElseIf userODBCdriver.ToUpper.StartsWith("MYSQL") Then
                            msql = "ALTER TABLE `" & db & "`.`" & tblname & "` CHANGE COLUMN `" & colname & "` `" & col & "` DATETIME NOT NULL DEFAULT 0 ;"
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        ElseIf userODBCdriver.ToUpper.StartsWith("ORACLE") Then
                            If col <> colname Then
                                msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            End If
                            RetypeAndReplaceColumnODBC(tblname, col, "date", userconnstr, userconnprv, ret, userODBCdriver)

                        ElseIf userODBCdriver.ToUpper.StartsWith("PSQL") Then
                            If colname <> col Then
                                msql = "ALTER TABLE `" & tblname & "` RENAME COLUMN `" & colname & "` TO `" & col & "`;"
                                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            End If
                            RetypeAndReplaceColumnODBC(tblname, col, "date", userconnstr, userconnprv)

                        Else
                            If col <> colname Then
                                msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            End If
                        End If
                        Continue For
                    ElseIf userconnprv.StartsWith("System.Data.OleDb") Then
                        If col <> colname Then
                            msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            If ret = "Query executed fine." Then
                                colname = col
                            End If
                        End If
                        If userODBCdriver.ToUpper.StartsWith("CACHE") OrElse userODBCdriver.StartsWith("IRIS") Then
                            If col <> colname Then
                                msql = "UPDATE %Dictionary.PropertyDefinition SET Name='" & FixReservedWords(col, "InterSystems.Data.") & "' "
                                msql &= "WHERE parent='" & tblname & "' AND Name='" & colname & "'"
                                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            End If
                            'msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & FixReservedWords(col, "InterSystems.Data.") & " DATETIME"
                            msql = "ALTER TABLE " & tblname & " MODIFY " & col & " DATETIME"
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        ElseIf userODBCdriver.ToUpper.StartsWith("SQL") Then
                            If col <> colname Then
                                msql = "sp_rename '" & tblname & "." & colname & "', '" & col & "', 'COLUMN' "
                                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            End If
                            RetypeAndReplaceColumnODBC(tblname, col, "date", userconnstr, userconnprv, ret, userODBCdriver)
                        ElseIf userODBCdriver.ToUpper.StartsWith("MYSQL") Then
                            msql = "ALTER TABLE `" & db & "`.`" & tblname & "` CHANGE COLUMN `" & colname & "` `" & col & "` DATETIME NOT NULL DEFAULT 0 ;"
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        ElseIf userODBCdriver.ToUpper.StartsWith("ORACLE") Then
                            If col <> colname Then
                                msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            End If
                            RetypeAndReplaceColumnODBC(tblname, col, "date", userconnstr, userconnprv, ret, userODBCdriver)

                        ElseIf userODBCdriver.ToUpper.StartsWith("PSQL") Then
                            If colname <> col Then
                                msql = "ALTER TABLE `" & tblname & "` RENAME COLUMN `" & colname & "` TO `" & col & "`;"
                                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            End If
                            RetypeAndReplaceColumnODBC(tblname, col, "date", userconnstr, userconnprv)

                        Else
                            If col <> colname Then
                                msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                            End If
                        End If
                        Continue For

                    ElseIf userconnprv = "Npgsql" Then  'PostgreSQL  Npgsql
                        If colname <> col Then
                            msql = "ALTER TABLE `" & tblname & "` RENAME COLUMN `" & colname & "` TO `" & col & "`;"
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        End If
                        ret = RetypeAndReplaceColumn(tblname, col, "date", userconnstr, userconnprv)
                        Continue For

                    ElseIf userconnprv = "Sqlite" Then
                        If colname <> col Then
                            msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col & ";"
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        End If
                        ret = RetypeAndReplaceColumn(tblname, col, "date", userconnstr, userconnprv)
                        Continue For

                    Else 'sql server
                        If col <> colname Then
                            msql = "sp_rename '" & tblname & "." & colname & "', '" & col & "', 'COLUMN' "
                            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        End If
                        ret = RetypeAndReplaceColumn(tblname, col, "date", userconnstr, userconnprv)
                        Continue For
                    End If
                Else

                    'text -  NOT NEEDED to adjust the length of the field !!!!!!!!!!!!!!!!!
                    '    If userconnprv.StartsWith("InterSystems.Data.") Then
                    '      If col <> colname Then
                    '        msql = "UPDATE %Dictionary.PropertyDefinition SET Name='" & col & "' "
                    '        msql &= "WHERE parent='" & tblname & "' AND Name='" & colname & "'"
                    '        ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '      End If
                    '      If collens(i) > 4000 Then
                    '        msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & col & " NVARCHAR(" & collens(i) & ") DEFAULT NULL"
                    '        ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '       End If
                    '    End If
                    '    If userconnprv = "MySql.Data.MySqlClient" Then
                    '        msql = "ALTER TABLE `" & db & "`.`" & tblname & "` CHANGE COLUMN `" & colname & "` `" & col & "` VARCHAR(" & collens(i) & ") NULL DEFAULT NULL ;"
                    '    ElseIf userconnprv = "Oracle.ManagedDataAccess.Client" Then
                    '        If col <> colname Then
                    '            msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                    '            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '        End If
                    '        msql = "ALTER TABLE " & tblname & " MODIFY " & col & " VARCHAR2(" & collens(i) & " CHAR)"
                    '    ElseIf userconnprv.StartsWith("InterSystems.Data.") Then
                    '        If col <> colname Then
                    '            msql = "UPDATE %Dictionary.PropertyDefinition SET Name='" & FixReservedWords(col, "InterSystems.Data.") & "' "
                    '            msql &= "WHERE parent='" & tblname & "' AND Name='" & colname & "'"
                    '            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '        End If
                    '        msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & FixReservedWords(col, "InterSystems.Data.") & " VARCHAR(" & collens(i) & ")"
                    '    ElseIf userconnprv.StartsWith("System.Data.Odbc") Then
                    '        If col <> colname Then
                    '            msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                    '            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '            If ret = "Query executed fine." Then
                    '                colname = col
                    '            End If
                    '        End If
                    '        If userODBCdriver.ToUpper.StartsWith("CACHE") OrElse userODBCdriver.StartsWith("IRIS") Then
                    '            If col <> colname Then
                    '                msql = "UPDATE %Dictionary.PropertyDefinition SET Name='" & FixReservedWords(col, "InterSystems.Data.") & "' "
                    '                msql &= "WHERE parent='" & tblname & "' AND Name='" & colname & "'"
                    '                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '            End If
                    '            msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & FixReservedWords(col, "InterSystems.Data.") & " VARCHAR(" & collens(i) & ")"
                    '        ElseIf userODBCdriver.ToUpper.StartsWith("SQL") Then
                    '            If col <> colname Then
                    '                msql = "sp_rename '" & tblname & "." & colname & "', '" & col & "', 'COLUMN' "
                    '                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '            End If
                    '            msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & col & " VARCHAR(" & collens(i) & ")  NULL"
                    '        ElseIf userODBCdriver.ToUpper.StartsWith("MYSQL") Then
                    '            msql = "ALTER TABLE `" & db & "`.`" & tblname & "` CHANGE COLUMN `" & colname & "` `" & col & "` VARCHAR(" & collens(i) & ") NULL DEFAULT NULL ;"
                    '            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '        ElseIf userODBCdriver.ToUpper.StartsWith("ORACLE") Then
                    '            If col <> colname Then
                    '                msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                    '                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '            End If
                    '            msql = "ALTER TABLE " & tblname & " MODIFY " & col & " VARCHAR2(" & collens(i) & " CHAR)"

                    '        ElseIf userODBCdriver.ToUpper.StartsWith("PSQL") Then  'PostgreSQL  Npgsql
                    '            If col <> colname Then
                    '                msql = "ALTER TABLE `" & tblname & "` RENAME COLUMN `" & colname & "` TO `" & col & "`;"
                    '                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '            End If
                    '            msql = "ALTER TABLE `" & tblname & "` ALTER COLUMN `" & col & "` TYPE character varying(" & collens(i) & ")  NULL"

                    '        Else
                    '            If col <> colname Then
                    '                msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                    '                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '            End If
                    '        End If
                    '        Continue For
                    '    ElseIf userconnprv.StartsWith("System.Data.OleDb") Then
                    '        If col <> colname Then
                    '            msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                    '            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '            If ret = "Query executed fine." Then
                    '                colname = col
                    '            End If
                    '        End If
                    '        If userODBCdriver.ToUpper.StartsWith("CACHE") OrElse userODBCdriver.StartsWith("IRIS") Then
                    '            If col <> colname Then
                    '                msql = "UPDATE %Dictionary.PropertyDefinition SET Name='" & FixReservedWords(col, "InterSystems.Data.") & "' "
                    '                msql &= "WHERE parent='" & tblname & "' AND Name='" & colname & "'"
                    '                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '            End If
                    '            msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & FixReservedWords(col, "InterSystems.Data.") & " VARCHAR(" & collens(i) & ")"
                    '        ElseIf userODBCdriver.ToUpper.StartsWith("SQL") Then
                    '            If col <> colname Then
                    '                msql = "sp_rename '" & tblname & "." & colname & "', '" & col & "', 'COLUMN' "
                    '                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '            End If
                    '            msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & col & " VARCHAR(" & collens(i) & ")  NULL"
                    '        ElseIf userODBCdriver.ToUpper.StartsWith("MYSQL") Then
                    '            msql = "ALTER TABLE `" & db & "`.`" & tblname & "` CHANGE COLUMN `" & colname & "` `" & col & "` VARCHAR(" & collens(i) & ") NULL DEFAULT NULL ;"
                    '            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '        ElseIf userODBCdriver.ToUpper.StartsWith("ORACLE") Then
                    '            If col <> colname Then
                    '                msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                    '                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '            End If
                    '            msql = "ALTER TABLE " & tblname & " MODIFY " & col & " VARCHAR2(" & collens(i) & " CHAR)"

                    '        ElseIf userODBCdriver.ToUpper.StartsWith("PSQL") Then  'PostgreSQL  Npgsql
                    '            If col <> colname Then
                    '                msql = "ALTER TABLE `" & tblname & "` RENAME COLUMN `" & colname & "` TO `" & col & "`;"
                    '                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '            End If
                    '            msql = "ALTER TABLE `" & tblname & "` ALTER COLUMN `" & col & "` TYPE character varying(" & collens(i) & ")  NULL"

                    '        Else
                    '            If col <> colname Then
                    '                msql = "ALTER TABLE " & tblname & " RENAME COLUMN " & colname & " TO " & col
                    '                ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '            End If
                    '        End If
                    '        Continue For

                    '    ElseIf userconnprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    '        If col <> colname Then
                    '            msql = "ALTER TABLE `" & tblname & "` RENAME COLUMN `" & colname & "` TO `" & col & "`;"
                    '            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '        End If
                    '        msql = "ALTER TABLE `" & tblname & "` ALTER COLUMN `" & col & "` TYPE character varying(" & collens(i) & ") "

                    '    Else 'sql server
                    '        If col <> colname Then
                    '            msql = "sp_rename '" & tblname & "." & colname & "', '" & col & "', 'COLUMN'"
                    '            ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '        End If
                    '        msql = "ALTER TABLE " & tblname & " ALTER COLUMN " & col & " VARCHAR(" & collens(i) & ")  NULL"
                    '    End If
                    '    ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    '    ' WriteToAccessLog(tblname, "column changed: " & msql & " - with result: " & ret, 111)
                End If
            Next

            Dim er As String = String.Empty
            Dim indxcol As String = IndexFieldInTable(tblname, db, userconnstr, userconnprv, er).Trim

            If indxcol.StartsWith("ERROR!!") Then
                Return ret
            End If

            If exst = False Then
                If indxcol = String.Empty Then
                    If ColumnExists(tblname, "Indx", userconnstr, userconnprv, er) Then
                        For i = 0 To 100
                            indxcol = "Indx" & i.ToString
                            If Not ColumnExists(tblname, indxcol, userconnstr, userconnprv, er) Then
                                Exit For
                            End If
                        Next
                    Else
                        indxcol = "Indx"
                    End If

                    If userconnprv = "MySql.Data.MySqlClient" Then
                        msql = "ALTER TABLE `" & db & "`.`" & tblname & "` ADD COLUMN `" & indxcol & "` int(11) Not NULL AUTO_INCREMENT PRIMARY KEY;"
                    ElseIf userconnprv = "Oracle.ManagedDataAccess.Client" Then
                        msql = "ALTER TABLE " & tblname & " ADD " & indxcol & " NUMBER GENERATED ALWAYS AS IDENTITY"
                        ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        msql = "ALTER TABLE " & tblname & " ADD CONSTRAINT " & tblname & "_pk PRIMARY KEY(" & indxcol & ")"
                    ElseIf userconnprv.StartsWith("InterSystems.Data.") Then
                        msql = "ALTER TABLE " & tblname & " ADD " & indxcol & " int IDENTITY(1,1) PRIMARY KEY"
                    ElseIf userconnprv.StartsWith("System.Data.Odbc") Then
                        msql = "ALTER TABLE " & tblname & " ADD " & indxcol & " int IDENTITY(1,1) PRIMARY KEY"
                    ElseIf userconnprv.StartsWith("System.Data.OleDb") Then
                        msql = "ALTER TABLE " & tblname & " ADD " & indxcol & " int IDENTITY(1,1) PRIMARY KEY"
                    ElseIf userconnprv = "Npgsql" Then  'PostgreSQL  Npgsql
                        msql = "ALTER TABLE `" & tblname & "` ADD COLUMN `" & indxcol & "` bigint NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 9223372036854775807 CACHE 1 );"
                        ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                        msql = "ALTER TABLE `" & tblname & "` ADD CONSTRAINT `" & tblname & "_pkey` PRIMARY KEY (`Indx`))"
                    ElseIf userconnprv = "Sqlite" Then
                        ret = RetypeAndReplaceColumn(tblname, indxcol, "pk", userconnstr, userconnprv)
                    Else 'sql server
                        msql = "ALTER TABLE " & tblname & " ADD " & indxcol & " int IDENTITY(1,1) Not NULL PRIMARY KEY"
                    End If

                    ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    ' reset dvcsv data view
                    Dim err As String = String.Empty
                    If userconnprv = "Oracle.ManagedDataAccess.Client" Then
                        msql = "SELECT * FROM " & db & "." & tblname
                    Else
                        msql = "SELECT * FROM [" & tblname & "]"
                    End If
                    dvcsv = mRecords(msql, err, userconnstr, userconnprv)
                    'dvcsv = mRecords("Select * FROM [" & tblname & "]", err, userconnstr, userconnprv)

                    If dvcsv Is Nothing OrElse dvcsv.Count = 0 OrElse dvcsv.Table.Rows.Count = 0 Then
                        Return err
                    End If

                    n = dvcsv.Table.Columns.Count
                    ReDim Preserve collens(n - 1)
                    ReDim Preserve coltypes(n - 1)
                    ReDim Preserve coldates(n - 1)

                    collens(n - 1) = "12"
                    coltypes(n - 1) = "num"
                    coldates(n - 1) = ""
                    ' WriteToAccessLog(tblname, "column added: " & indxcol & " - with result: " & ret, 111)
                End If
            End If
            Dim ret0 As String = ret
            ''find Indx field
            'indxcol = IndexFieldInTable(tblname, db, userconnstr, userconnprv, er)
            'If indxcol.Trim = "" OrElse indxcol.StartsWith("ERROR!!") Then
            '    Return ret
            'End If
            'fix datatime format
            Dim ind As Integer = 0
            Dim dat As String = String.Empty
            For i = 0 To n - 1
                If coldates(i) = "date" Then
                    'make DateToString
                    For j = 0 To m - 1
                        'If dvcsv.table.rows(j)(i).ToString = String.Empty Then Continue For
                        Dim dtReturn As New DateTime
                        If Not IsDateTimeValue(dvcsv.Table.Rows(j)(i).ToString, dtReturn) Then Continue For
                        'If Not IsDateTime(dvcsv.Table.Rows(j)(i).ToString) Then Continue For
                        dat = DateToString(dvcsv.Table.Rows(j)(i).ToString, userconnprv)
                        If userconnprv = "MySql.Data.MySqlClient" Then
                            msql = "UPDATE `" & db & "`.`" & tblname & "` SET `" & dvcsv.Table.Columns(i).Caption & "`='" & dat & "' WHERE `" & indxcol & "`=" & dvcsv.Table.Rows(j)(indxcol)
                        ElseIf userconnprv = "Npgsql" Then  'PostgreSQL  Npgsql
                            msql = "UPDATE `" & tblname & "` SET `" & dvcsv.Table.Columns(i).Caption & "`='" & dat & "' WHERE `" & indxcol & "`=" & dvcsv.Table.Rows(j)(indxcol)
                        Else
                            msql = "UPDATE " & tblname & " SET " & dvcsv.Table.Columns(i).Caption & "='" & dat & "' WHERE " & indxcol & "=" & dvcsv.Table.Rows(j)(indxcol)
                        End If
                        ret = ExequteSQLquery(msql, userconnstr, userconnprv)
                    Next
                End If
            Next
            Return ret0
        Catch ex As Exception
            ret = ex.Message
            Return ret
        End Try
        Return ret
    End Function
    Public Function IndexFieldInTable(ByVal tblname As String, ByVal dbschema As String, ByVal userconnstr As String, ByVal userconnprv As String, Optional ByVal er As String = "") As String
        'find the index field in the table
        Dim ret As String = String.Empty
        Dim dt As DataTable = Nothing
        Dim msql As String = String.Empty
        Dim constraint As String = String.Empty

        Dim dv As DataView
        'TODO other providers
        If userconnprv = "Sqlite" Then
            Dim dvi As DataView
            Dim dti As DataTable
            dv = GetListOfTableFields(tblname, userconnstr, userconnprv)
            dvi = mRecords("PRAGMA table_info('" & tblname & "')")
            If dvi Is Nothing OrElse dvi.Table Is Nothing OrElse dvi.Table.Columns.Count = 0 OrElse dvi.Table.Rows.Count = 0 Then
                Return ""
            Else
                dvi.RowFilter = "pk=1"
                dti = dvi.ToTable
                If dti.Rows.Count = 0 Then
                    Return ""
                Else
                    Return dti.Rows(0)("name")
                End If
            End If
            Return ""
        ElseIf userconnprv = "MySql.Data.MySqlClient" Then
            'msql = "SELECT DISTINCT TABLE_SCHEMA,TABLE_NAME,COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA='" & dbschema & "' AND TABLE_NAME='" & tblname & "' AND CONSTRAINT_NAME='PRIMARY'"
            msql = "SELECT DISTINCT TABLE_SCHEMA,TABLE_NAME,COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='" & dbschema.ToLower & "' AND TABLE_NAME='" & tblname & "' AND COLUMN_KEY='PRI' AND EXTRA LIKE '%auto_increment%';"

            'msql = "SELECT DISTINCT TABLE_SCHEMA,TABLE_NAME,COLUMN_NAME FROM  `INFORMATION_SCHEMA.COLUMNS` WHERE TABLE_SCHEMA='" & dbschema & "' AND TABLE_NAME='" & tblname & "';"

        ElseIf userconnprv = "Npgsql" Then  'PostgreSQL  Npgsql
            ' msql = "SELECT DISTINCT TABLE_SCHEMA,TABLE_NAME,COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='" & dbschema & "' AND TABLE_NAME='" & tblname & "' AND COLUMN_KEY='PRI' AND EXTRA LIKE '%auto_increment%';"
            msql = "Select CONSTRAINT_NAME From INFORMATION_SCHEMA.TABLE_CONSTRAINTS  WHERE TABLE_SCHEMA ='public' AND LOWER(TABLE_CATALOG) ='" & dbschema.ToLower & "' AND TABLE_NAME='" & tblname & "' And CONSTRAINT_TYPE='PRIMARY KEY'"
            dt = mRecords(msql, ret, userconnstr, userconnprv).Table
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                constraint = dt.Rows(0)("CONSTRAINT_NAME").ToString
                msql = "Select * From INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE CONSTRAINT_NAME='" & constraint & "' AND TABLE_SCHEMA ='public' AND LOWER(TABLE_CATALOG) ='" & dbschema.ToLower & "' AND TABLE_NAME='" & tblname & "'"
            Else
                Return ""
            End If

        ElseIf userconnprv = "System.Data.SqlClient" Then
            msql = "Select CONSTRAINT_NAME From INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE TABLE_NAME='" & tblname & "' And CONSTRAINT_TYPE='PRIMARY KEY'"
            dt = mRecords(msql, ret, userconnstr, userconnprv).Table
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                constraint = dt.Rows(0)("CONSTRAINT_NAME").ToString
                msql = "Select * From INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE CONSTRAINT_NAME='" & constraint & "' AND TABLE_NAME='" & tblname & "'"
            Else
                Return ""
            End If

        ElseIf userconnprv = "Oracle.ManagedDataAccess.Client" Then
            msql = "Select CONSTRAINT_NAME FROM user_constraints WHERE constraint_type='P' AND TABLE_NAME='" & tblname.ToUpper & "'"
            dt = mRecords(msql, ret, userconnstr, userconnprv).Table
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                constraint = dt.Rows(0)("CONSTRAINT_NAME").ToString
                msql = "SELECT * FROM user_cons_columns WHERE CONSTRAINT_NAME='" & constraint & "' AND TABLE_NAME='" & tblname.ToUpper & "'"
            Else
                Return ""
            End If

        ElseIf userconnprv = "System.Data.Odbc" Then
            'ODBC GetSchema("indexes")
            Dim spnames(2) As String
            spnames(2) = tblname
            dt = GetListByODBC(System.Data.Odbc.OdbcMetaDataCollectionNames.Indexes, userconnstr, userconnprv, spnames)
            dv = dt.DefaultView
            dv.RowFilter = "TABLE_NAME='" & tblname & "'"
            dt = dv.ToTable

        ElseIf userconnprv = "System.Data.OleDb" Then
            'ODBC GetSchema("indexes")
            Dim spnames(2) As String
            spnames(2) = tblname
            dt = GetListByODBC(System.Data.OleDb.OleDbMetaDataCollectionNames.Indexes, userconnstr, userconnprv, spnames)
            dv = dt.DefaultView
            dv.RowFilter = "TABLE_NAME='" & tblname & "'"
            dt = dv.ToTable

        ElseIf userconnprv.StartsWith("InterSystems.Data.") Then
            Dim sConstraintSchema As String = String.Empty
            Dim tbl As String = String.Empty
            If tblname.ToUpper.StartsWith("OUR") Then _
                tblname = CorrectSQLforCache(tblname)

            tblname = CorrectTableNameWithDots(tblname, userconnprv)
            sConstraintSchema = Piece(tblname, ".", 1)
            tbl = Piece(tblname, ".", 2)
            msql = "SELECT * from INFORMATION_SCHEMA.KEY_COLUMN_USAGE "
            msql &= "WHERE CONSTRAINT_SCHEMA = '" & sConstraintSchema & "' And TABLE_NAME='" & tbl & "' "
            msql &= "And CONSTRAINT_TYPE='PRIMARY KEY'"
        End If

        Try
            If userconnprv <> "System.Data.Odbc" AndAlso userconnprv <> "System.Data.OleDb" Then
                dt = mRecords(msql, ret, userconnstr, userconnprv).Table
            End If

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                ret = dt.Rows(0)("COLUMN_NAME").ToString
            Else
                ret = ""
            End If
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Public Function IsPrimaryKey(ColumnName As String, TableName As String, ConnectionString As String, ConnectionProvider As String, ByRef Result As String) As Boolean
        'NOT IN USE ???
        Dim dt As DataTable = Nothing
        Dim ret As String = String.Empty
        Dim sql As String = String.Empty
        Dim db As String = String.Empty
        Dim bRet As Boolean = False

        If ConnectionProvider = "MySql.Data.MySqlClient" Then
            db = GetDataBase(ConnectionString, ConnectionProvider)
            sql = "Select * From INFORMATION_SCHEMA.KEY_COLUMN_USAGE "
            sql &= "Where TABLE_SCHEMA='" & db.ToLower & "' AND TABLE_NAME='" & TableName & "' And COLUMN_NAME='" & ColumnName & "' And CONSTRAINT_NAME='PRIMARY'"

        ElseIf ConnectionProvider = "Npgsql" Then  'PostgreSQL  Npgsql
            db = GetDataBase(ConnectionString, ConnectionProvider)
            sql = "Select * From INFORMATION_SCHEMA.KEY_COLUMN_USAGE "
            sql &= "Where TABLE_SCHEMA ='public' AND LOWER(TABLE_CATALOG) ='" & db.ToLower & "' AND TABLE_NAME='" & TableName & "' And COLUMN_NAME='" & ColumnName & "' And CONSTRAINT_NAME LIKE '%_pkey'"

        ElseIf ConnectionProvider = "System.Data.SqlClient" Then
            sql = "Select CONSTRAINT_NAME From INFORMATION_SCHEMA.KEY_COLUMN_USAGE "
            sql &= "Where TABLE_NAME='" & TableName & "' AND COLUMN_NAME='" & ColumnName & "'"
            dt = mRecords(sql, Result, ConnectionString, ConnectionProvider).Table
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Dim sConstraint As String = dt.Rows(0)("CONSTRAINT_NAME")
                sql = "Select * From INFORMATION_SCHEMA.TABLE_CONSTRAINTS "
                sql &= "Where CONSTRAINT_NAME='" & sConstraint & "' And TABLE_NAME='" & TableName & "' And CONSTRAINT_TYPE='PRIMARY KEY'"
            Else
                Return False
            End If
        ElseIf ConnectionProvider.StartsWith("InterSystems.Data.") Then
            Dim sConstraintSchema As String = String.Empty
            Dim tbl As String = String.Empty
            If TableName.ToUpper.StartsWith("OUR") Then _
                TableName = CorrectSQLforCache(TableName)

            TableName = CorrectTableNameWithDots(TableName, ConnectionProvider)
            sConstraintSchema = Piece(TableName, ".", 1)
            tbl = Piece(TableName, ".", 2)
            sql = "SELECT * from INFORMATION_SCHEMA.KEY_COLUMN_USAGE "
            sql &= "WHERE CONSTRAINT_SCHEMA = '" & sConstraintSchema & "' And TABLE_NAME='" & tbl & "' "
            sql &= "And COLUMN_NAME='" & ColumnName & "' And CONSTRAINT_TYPE='PRIMARY KEY'"

        ElseIf ConnectionProvider = "Oracle.ManagedDataAccess.Client" Then
            sql = "Select CONSTRAINT_NAME From user_constraints "
            sql &= "Where CONSTRAINT_TYPE='P' AND UPPER(TABLE_NAME)=UPPER('" & TableName & "')"
            dt = mRecords(sql, Result, ConnectionString, ConnectionProvider).Table
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Dim sConstraint As String = dt.Rows(0)("CONSTRAINT_NAME")
                sql = "Select * From user_cons_columns "
                sql &= "Where CONSTRAINT_NAME='" & sConstraint & "' And UPPER(TABLE_NAME)=UPPER('" & TableName & "') And UPPER(COLUMN_NAME)=UPPER('" & ColumnName & "')"
            Else
                Return False
            End If
        ElseIf ConnectionProvider = "System.Data.Odbc" Then
            'ODBC GetSchema("indexes")
            Dim spnames(2) As String
            spnames(2) = TableName
            dt = GetListByODBC(System.Data.Odbc.OdbcMetaDataCollectionNames.Indexes, ConnectionString, ConnectionProvider, spnames)
            Dim dv As DataView = dt.DefaultView
            dv.RowFilter = "NOT_UNQUE=0)"  'Is Primary?
            dt = dv.ToTable
        ElseIf ConnectionProvider = "System.Data.OleDb" Then
            'OleDb GetSchema("indexes")
            Dim spnames(2) As String
            spnames(2) = TableName
            dt = GetListByODBC(System.Data.Odbc.OdbcMetaDataCollectionNames.Indexes, ConnectionString, ConnectionProvider, spnames)
            Dim dv As DataView = dt.DefaultView
            dv.RowFilter = "NOT_UNQUE=0)"  'Is Primary?
            dt = dv.ToTable
        End If

        Try
            If ConnectionProvider <> "System.Data.Odbc" AndAlso ConnectionProvider <> "System.Data.OleDb" Then
                dt = mRecords(sql, Result, ConnectionString, ConnectionProvider).Table
            End If

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                bRet = True
                Result = ColumnName
            Else
                bRet = False
            End If
        Catch ex As Exception
            Result = "ERROR!! " & ex.Message
        End Try
        Return bRet
    End Function

    Public Function CreateDbTablesForDataSet(ByRef tblname() As String, ByVal dset As DataSet, ByVal userconnstr As String, ByVal userconnprv As String, Optional ByRef er As String = "") As String
        Dim ret As String = String.Empty
        Dim j, m As Integer
        Dim sqlq As String = String.Empty
        Try
            'create db 
            Dim db As String = GetDataBase(userconnstr, userconnprv).ToLower
            'only for MySql
            If userconnprv = "MySql.Data.MySqlClient" Then
                sqlq = "Select * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME='" & db & "'"
                If Not HasRecords(sqlq, userconnstr, userconnprv) Then
                    sqlq = "CREATE DATABASE `" & db & "`"
                    ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
                    If ret = "Query executed fine." Then
                        ret = "" ' ret & "<br/> " & db & " created. "
                    Else
                        ret = ret & "<br/> " & db & " not created: " & ret & " "
                        Return ret
                    End If
                End If
            End If
            m = dset.Tables.Count
            ReDim Preserve tblname(m - 1)
            If m = 1 Then
                ret = ImportDataTableIntoDb(dset.Tables(0), tblname(0), userconnstr, userconnprv)
            Else
                For j = 0 To m - 1
                    'create table
                    tblname(j) = tblname(0) & j.ToString
                    ret = ImportDataTableIntoDb(dset.Tables(j), tblname(j), userconnstr, userconnprv)
                Next
            End If
            'TODO make relations

            Return ret.Replace("Query executed fine.", "")
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function

    Public Function ImportDataTableIntoDb(ByVal dt As DataTable, ByVal tablename As String, ByVal userconnstr As String, ByVal userconnprv As String, Optional ByRef er As String = "") As String
        'For new import.New table is not a copy of old one  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        Dim ret As String = String.Empty
        Dim mSQL, sqlq, col, typ As String
        Dim i, n As Integer
        Dim exst As Boolean
        Dim dbcase As String = String.Empty
        If userconnprv = "Npgsql" Then
            If userconnstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("MySqlConnection").ToString.Trim.ToUpper Then
                dbcase = ConfigurationManager.AppSettings("ourdbcase").ToString
            ElseIf System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing AndAlso userconnstr.Trim.ToUpper = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString.Trim.ToUpper Then
                dbcase = ConfigurationManager.AppSettings("csvdbcase").ToString
            Else 'userdbcase
                dbcase = userdbcase
            End If
        End If

        Dim db As String = GetDataBase(userconnstr, userconnprv)
        If userconnprv = "MySql.Data.MySqlClient" Then
            db = db.ToLower
        End If

        Dim ds As New DataSet()

        n = dt.Columns.Count
        'make columns names CLS-compliant
        dt = MakeDTColumnsNamesCLScompliant(dt, userconnprv, er)

        Try
            If Not TableExists(tablename, userconnstr, userconnprv, er) Then
                exst = False
                If userconnprv = "MySql.Data.MySqlClient" Then
                    sqlq = "CREATE TABLE `" & db & "`.`" & tablename.ToLower & "`( "
                    For i = 0 To n - 1
                        'done to be CLS-compliant above
                        'col = cleanText(FixReservedWords(dt.Columns(i).Caption, userconnprv))
                        'col = col.Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace(",", "_").Replace("%", " Percent ").Replace("_", " ").Trim 'fix column name
                        col = dt.Columns(i).Caption
                        sqlq = sqlq & "  " & col & "  "
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

                ElseIf userconnprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE " & tablename & " ( "
                    For i = 0 To n - 1
                        col = cleanText(FixReservedWords(dt.Columns(i).Caption, userconnprv))
                        col = col.Replace("_", " ").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace(",", "_").Trim 'fix column name
                        sqlq = sqlq & "  " & col & "  "
                        typ = dt.Columns(i).DataType.FullName
                        If ColumnTypeIsNumeric(dt.Columns(i)) Then
                            sqlq = sqlq & " FLOAT(126) DEFAULT 0 NOT NULL"
                        ElseIf ColumnTypeIsDateTime(dt.Columns(i)) Then
                            sqlq = sqlq & "  VARCHAR2(4000 CHAR) DEFAULT NULL"
                        ElseIf ColumnTypeIsString(dt.Columns(i)) Then
                            sqlq = sqlq & "  VARCHAR2(4000 CHAR) DEFAULT NULL"
                        Else
                            sqlq = sqlq & "  VARCHAR2(4000 CHAR) DEFAULT NULL"
                        End If
                        If i < n - 1 Then sqlq = sqlq & ","
                    Next

                ElseIf userconnprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    tablename = FixReservedWords(tablename, userconnprv)
                    sqlq = "CREATE Table `" & tablename & "`("
                    For i = 0 To n - 1
                        col = cleanText(dt.Columns(i).Caption)
                        col = col.Replace("_", " ").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace(",", "_").Trim 'fix column name
                        col = FixReservedWords(col, userconnprv)
                        sqlq = sqlq & "  `" & col & "`  "
                        typ = dt.Columns(i).DataType.FullName
                        If ColumnTypeIsNumeric(dt.Columns(i)) Then
                            sqlq = sqlq & " decimal(10,4) NOT NULL DEFAULT 0"
                        ElseIf ColumnTypeIsDateTime(dt.Columns(i)) Then
                            sqlq = sqlq & "  character varying(4000) NULL"
                        ElseIf ColumnTypeIsString(dt.Columns(i)) Then
                            sqlq = sqlq & "  text NULL"
                        Else
                            sqlq = sqlq & "  text NULL"
                        End If
                        If i < n - 1 Then sqlq = sqlq & ","
                    Next
                ElseIf userconnprv.StartsWith("InterSystems.Data.") Then
                    sqlq = "CREATE TABLE " & tablename & "( "
                    For i = 0 To n - 1
                        col = cleanText(dt.Columns(i).ColumnName)
                        col = col.Replace("_", " ").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace(",", "_").Trim 'fix column name
                        col = FixReservedWords(col, userconnprv)
                        sqlq = sqlq & "  " & col & "  "
                        typ = dt.Columns(i).DataType.FullName
                        If ColumnTypeIsNumeric(dt.Columns(i)) Then
                            sqlq = sqlq & " FLOAT NOT NULL DEFAULT 0"
                        ElseIf ColumnTypeIsDateTime(dt.Columns(i)) Then
                            sqlq = sqlq & "  [nvarchar](4000) NULL"
                        ElseIf ColumnTypeIsString(dt.Columns(i)) Then
                            sqlq = sqlq & "  [nvarchar](4000)  NULL"
                        Else
                            sqlq = sqlq & "  [nvarchar](4000)  NULL"
                        End If
                        If i < n - 1 Then sqlq = sqlq & ","
                    Next
                ElseIf userconnprv = "Sqlite" Then
                    sqlq = "CREATE TABLE " & tablename & "( "
                    For i = 0 To n - 1
                        col = cleanText(dt.Columns(i).Caption)
                        col = col.Replace("_", " ").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace(",", "_").Trim 'fix column name
                        col = FixReservedWords(col, userconnprv)
                        sqlq = sqlq & "  " & col & "  "
                        typ = dt.Columns(i).DataType.FullName
                        If ColumnTypeIsNumeric(dt.Columns(i)) Then
                            sqlq = sqlq & " FLOAT NOT NULL DEFAULT 0"
                        ElseIf ColumnTypeIsDateTime(dt.Columns(i)) Then
                            sqlq = sqlq & "  [nvarchar](4000) NULL"
                        ElseIf ColumnTypeIsString(dt.Columns(i)) Then
                            sqlq = sqlq & "  TEXT NULL"
                        Else
                            sqlq = sqlq & "  TEXT NULL"
                        End If
                        If i < n - 1 Then sqlq = sqlq & ","

                    Next
                    sqlq = sqlq & ", [Indx] Integer PRIMARY KEY AUTOINCREMENT"

                Else  'SQL Server
                    sqlq = "CREATE TABLE " & tablename & "( "
                    For i = 0 To n - 1
                        col = cleanText(dt.Columns(i).Caption)
                        col = col.Replace("_", " ").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace(",", "_").Trim 'fix column name
                        col = FixReservedWords(col, userconnprv)
                        sqlq = sqlq & "  " & col & "  "
                        typ = dt.Columns(i).DataType.FullName
                        If ColumnTypeIsNumeric(dt.Columns(i)) Then
                            sqlq = sqlq & " FLOAT NOT NULL DEFAULT 0"
                        ElseIf ColumnTypeIsDateTime(dt.Columns(i)) Then
                            sqlq = sqlq & "  [nvarchar](4000) NULL"
                        ElseIf ColumnTypeIsString(dt.Columns(i)) Then
                            sqlq = sqlq & "  TEXT NULL"
                        Else
                            sqlq = sqlq & "  TEXT NULL"
                        End If
                        If i < n - 1 Then sqlq = sqlq & ","
                    Next
                End If

                'sqlq = sqlq & " `Indx` int(11) NOT NULL AUTO_INCREMENT PRIMARY KEY "
                sqlq = sqlq & " )"
                sqlq = ConvertSQLSyntaxFromOURdbToUserDB(sqlq, userconnprv, ret)
                ret = ExequteSQLquery(sqlq, userconnstr, userconnprv)
                If ret = "Query executed fine." Then
                Else
                    ret = "ERROR!! " & tablename & " creation crashed: " & ret & " "
                    Return ret
                End If
            Else  'table exists
                exst = True
                'check if tablename has the same column names
                For i = 0 To n - 1
                    dt.Columns(i).Caption = dt.Columns(i).Caption.Replace("_", " ").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Replace(",", "_").Trim 'fix column name
                    If Not IsColumnFromTable(tablename, dt.Columns(i).Caption, userconnstr, userconnprv, ret) Then
                        ' WriteToAccessLog(tablename, "Existing table: " & tablename & " does not have a column " & dt.Columns(i).Caption & " - " & ret, 111)
                        Return "ERROR!! Existing table: " & tablename & " does not have a column " & dt.Columns(i).Caption
                    End If
                Next

            End If

            'load data from dt
            If userconnprv = "MySql.Data.MySqlClient" Then
                mSQL = "SET NAMES 'utf8mb4';"
                ret = ExequteSQLquery(mSQL, userconnstr, userconnprv)
                'WriteToAccessLog(tablename, "SET NAMES: " & ret, 111)
            End If

            ret = tablename & " created. "

            'bulk import
            If userconnprv = "MySql.Data.MySqlClient" Then
                Dim myConnection As MySqlConnection
                Dim myCommand As New MySqlCommand
                Dim myAdapter As MySqlDataAdapter
                myConnection = New MySqlConnection(userconnstr)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = "SELECT * FROM `" & db & "`.`" & tablename.ToLower & "` "
                'myCommand.CommandText = "SELECT * FROM " & tablename
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New MySqlDataAdapter(myCommand)
                Dim cb As MySqlCommandBuilder = New MySqlCommandBuilder(myAdapter)
                Try
                    myAdapter.Update(dt)
                    myAdapter.Dispose()
                    myCommand.Connection.Close()
                    myCommand.Dispose()
                Catch ex As Exception
                    ret = "ERROR!! " & ex.Message
                End Try
            ElseIf userconnprv = "Oracle.ManagedDataAccess.Client" Then
                ret = ImportDataTableIntoDb_Oracle(dt, tablename, userconnstr)
            ElseIf userconnprv = "InterSystems.Data.CacheClient" Then
                ret = ImportDataTableIntoDb_Cache(dt, tablename, userconnstr)
            ElseIf userconnprv = "InterSystems.Data.IRISClient" Then
                ret = ImportDataTableIntoDb_IRIS(dt, tablename, userconnstr)
            ElseIf userconnprv = "System.Data.Odbc" Then
                userconnstr = userconnstr.Replace("Password", "Pwd").Replace("User ID", "UID")
                Dim ODBCBuilder As System.Data.Odbc.OdbcCommandBuilder
                Dim dataODBCConnection As New System.Data.Odbc.OdbcConnection
                Dim dataCommand As New System.Data.Odbc.OdbcCommand
                dataODBCConnection = New System.Data.Odbc.OdbcConnection(userconnstr)
                If dataODBCConnection.State = ConnectionState.Closed Then dataODBCConnection.Open()
                dataCommand.Connection = dataODBCConnection
                dataCommand.CommandType = CommandType.Text
                dataCommand.CommandTimeout = 300000
                dataCommand.CommandText = "Select * FROM " & tablename
                Dim dataAdapter As New System.Data.Odbc.OdbcDataAdapter(dataCommand)
                ODBCBuilder = New System.Data.Odbc.OdbcCommandBuilder(dataAdapter)
                Try
                    dataAdapter.Update(dt)
                    dataAdapter.Dispose()
                    dataCommand.Connection.Close()
                    dataCommand.Dispose()
                Catch ex As Exception
                    ret = "ERROR!! " & ex.Message
                End Try
            ElseIf userconnprv = "System.Data.OleDb" Then
                userconnstr = ConfigurationManager.AppSettings("ACEOLEDBversion").ToString & "Persist Security Info=True;" & userconnstr
                Dim OleDbBuilder As System.Data.OleDb.OleDbCommandBuilder
                Dim dataOleDbConnection As New System.Data.OleDb.OleDbConnection
                Dim dataCommand As New System.Data.OleDb.OleDbCommand
                dataOleDbConnection = New System.Data.OleDb.OleDbConnection(userconnstr)
                If dataOleDbConnection.State = ConnectionState.Closed Then dataOleDbConnection.Open()
                dataCommand.Connection = dataOleDbConnection
                dataCommand.CommandType = CommandType.Text
                dataCommand.CommandTimeout = 300000
                dataCommand.CommandText = "Select * FROM " & tablename
                Dim dataAdapter As New System.Data.OleDb.OleDbDataAdapter(dataCommand)
                OleDbBuilder = New System.Data.OleDb.OleDbCommandBuilder(dataAdapter)
                Try
                    dataAdapter.Update(dt)
                    dataAdapter.Dispose()
                    dataCommand.Connection.Close()
                    dataCommand.Dispose()
                Catch ex As Exception
                    ret = "ERROR!! " & ex.Message
                End Try
            ElseIf userconnprv = "Npgsql" Then
                userconnstr = CorrectConnstringForPostgres(userconnstr, dbcase)
                mSQL = "Select * FROM [" & tablename & "]"
                mSQL = ConvertFromSqlServerToPostgres(mSQL, dbcase, userconnstr, userconnprv)
                Dim cmdBuilder As Npgsql.NpgsqlCommandBuilder
                Dim myConnection As Npgsql.NpgsqlConnection
                Dim myCommand As New Npgsql.NpgsqlCommand
                Dim myAdapter As Npgsql.NpgsqlDataAdapter
                myConnection = New Npgsql.NpgsqlConnection(userconnstr)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = mSQL
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New Npgsql.NpgsqlDataAdapter(myCommand)
                cmdBuilder = New Npgsql.NpgsqlCommandBuilder(myAdapter)
                Try
                    myAdapter.Update(dt)
                    myAdapter.Dispose()
                    myCommand.Connection.Close()
                    myCommand.Dispose()
                Catch ex As Exception
                    ret = "ERROR!! " & ex.Message
                End Try
            ElseIf userconnprv = "Sqlite" Then
                Try
                    Dim myCommand As New SqliteCommand
                    myCommand.Connection = sqliteconn
                    myCommand.CommandType = CommandType.Text
                    myCommand.CommandTimeout = 300000

                    Dim columnNames As String = String.Empty
                    For i = 0 To dt.Columns.Count - 1
                        columnNames = columnNames & "[" & dt.Columns(i).ColumnName & "]"
                        If i < dt.Columns.Count - 1 Then
                            columnNames = columnNames & ","
                        End If
                    Next
                    'Insert each row
                    Dim insertSql As String = String.Empty
                    Dim columnValues As String = String.Empty
                    For Each row As DataRow In dt.Rows
                        columnValues = ""
                        For i = 0 To dt.Columns.Count - 1
                            If ColumnTypeIsNumeric(dt.Columns(i)) Then
                                columnValues = columnValues & row(dt.Columns(i).ColumnName).ToString
                            Else
                                columnValues = columnValues & "'" & row(dt.Columns(i).ColumnName).ToString & "'"
                            End If
                            If i < dt.Columns.Count - 1 Then
                                columnValues = columnValues & ","
                            End If
                        Next
                        insertSql = "INSERT INTO [" & tablename & "] (" & columnNames & ") VALUES (" & columnValues & ")"
                        myCommand.CommandText = insertSql
                        myCommand.ExecuteNonQuery()
                    Next
                Catch ex As Exception
                    ret = "ERROR!! " & ex.Message
                End Try
            Else 'SQL Server client
                Dim cmdBuilder As SqlCommandBuilder
                Dim myConnection As SqlConnection
                Dim myCommand As New SqlClient.SqlCommand
                Dim myAdapter As SqlClient.SqlDataAdapter
                myConnection = New SqlConnection(userconnstr)
                myCommand.Connection = myConnection
                myCommand.CommandType = CommandType.Text
                myCommand.CommandTimeout = 300000
                myCommand.CommandText = "Select * FROM " & tablename
                If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
                myAdapter = New SqlClient.SqlDataAdapter(myCommand)
                cmdBuilder = New SqlCommandBuilder(myAdapter)
                Try
                    myAdapter.Update(dt)
                    myAdapter.Dispose()
                    myCommand.Connection.Close()
                    myCommand.Dispose()
                Catch ex As Exception
                    ret = "ERROR!! " & ex.Message
                End Try

            End If
            If userconnprv = "MySql.Data.MySqlClient" Then
                mSQL = "Select * FROM `" & db & "`.`" & tablename & "`"
            Else
                mSQL = "SELECT * FROM " & tablename
            End If

            If Not HasRecords(mSQL, userconnstr, userconnprv) Then
                ret = ImportDataTableRowsIntoTable(dt, tablename, userconnstr, userconnprv, ret)
            End If
            'Dim caption As String = String.Empty
            ''correct column names in the table... CHECK IF IT IS NEEDED!   !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1 NOT NEEDED
            'For i = 0 To n - 1
            '    caption = dt.Columns(i).Caption
            '    col = caption.Replace("_", " ").Trim.Replace(" ", "_").Replace("-", "_").Replace(":", "_").Replace("*", "_").Replace("^", "_").Replace("/", "_").Replace("\", "_").Trim 'fix column name
            '    col = FixReservedWords(col, userconnprv)
            '    If col <> caption Then
            '        If userconnprv = "MySql.Data.MySqlClient" Then
            '            If ColumnTypeIsNumeric(dt.Columns(i)) Then
            '                mSQL = "ALTER TABLE `" & db & "`.`" & tablename.ToLower & "` CHANGE COLUMN `" & caption & "` `" & col & "` FLOAT NOT NULL DEFAULT 0 ;"
            '                ret = ExequteSQLquery(mSQL, userconnstr, userconnprv)
            '                'WriteToAccessLog(tablename.ToLower, "column changed: " & mSQL & " - with result: " & ret, 111)
            '            Else
            '                mSQL = "ALTER TABLE `" & db & "`.`" & tablename.ToLower & "` CHANGE COLUMN `" & caption & "` `" & col & "` TEXT NULL DEFAULT NULL ;"
            '                ret = ExequteSQLquery(mSQL, userconnstr, userconnprv)
            '                'WriteToAccessLog(tablename.ToLower, "column changed: " & mSQL & " - with result: " & ret, 111)
            '            End If
            '        ElseIf userconnprv.StartsWith("InterSystems.Data.") Then
            '            mSQL = "UPDATE %Dictionary.PropertyDefinition SET Name='" & FixReservedWords(col, "InterSystems.Data.") & "' "
            '            mSQL &= "WHERE parent='" & tablename & "' AND Name='" & caption & "'"
            '            ret = ExequteSQLquery(mSQL, userconnstr, userconnprv)
            '            'WriteToAccessLog(tablename, "column changed: " & mSQL & " - with result: " & ret, 111)
            '        ElseIf userconnprv = "Oracle.ManagedDataAccess.Client" Then
            '            mSQL = "ALTER TABLE " & tablename.ToUpper & " RENAME COLUMN " & caption & " TO " & col
            '            ret = ExequteSQLquery(mSQL, userconnstr, userconnprv)
            '            'WriteToAccessLog(tablename, "column changed: " & mSQL & " - with result: " & ret, 111)
            '        ElseIf userconnprv = "System.Data.Odbc" Then
            '            mSQL = "ALTER TABLE " & tablename.ToUpper & " RENAME COLUMN " & caption & " TO " & col
            '            ret = ExequteSQLquery(mSQL, userconnstr, userconnprv)
            '            ' WriteToAccessLog(tablename, "column changed: " & mSQL & " - with result: " & ret, 111)
            '        ElseIf userconnprv = "System.Data.OleDb" Then
            '            mSQL = "ALTER TABLE " & tablename.ToUpper & " RENAME COLUMN " & caption & " TO " & col
            '            ret = ExequteSQLquery(mSQL, userconnstr, userconnprv)
            '            ' WriteToAccessLog(tablename, "column changed: " & mSQL & " - with result: " & ret, 111)

            '        ElseIf userconnprv = "Npgsql" Then  'PostgreSQL  Npgsql
            '            mSQL = "ALTER TABLE `" & tablename & "` RENAME COLUMN '" & caption & "' TO '" & col & "`"
            '            ret = ExequteSQLquery(mSQL, userconnstr, userconnprv)

            '        Else 'SQL server
            '            mSQL = "sp_rename '" & tablename & "." & caption & "', '" & col & "', 'COLUMN'"
            '            ret = ExequteSQLquery(mSQL, userconnstr, userconnprv)
            '            ' WriteToAccessLog(tablename, "column changed: " & mSQL & " - with result: " & ret, 111)
            '        End If
            '    End If
            'Next

            ret = CorrectFieldTypesInTable(tablename, userconnstr, userconnprv, exst)

        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function ImportDataTableRowsIntoTable(ByVal dv As DataTable, ByVal tablename As String, ByVal userconnstr As String, ByVal userconnprv As String, Optional ByRef er As String = "") As String
        'tablename table has proper structure
        Dim ret As String = String.Empty
        Dim re As String = String.Empty
        Dim n, i, j As Integer
        Dim m2SQL, mfields, fld As String
        Dim Values, val As String

        mfields = ""
        For j = 0 To dv.Columns.Count - 1
            fld = dv.Columns.Item(j).Caption
            If fld.ToUpper <> "INDX" AndAlso fld.ToUpper <> "ID" Then
                fld = FixReservedWords(fld, userconnprv)
                If mfields = "" Then
                    mfields = fld
                Else
                    mfields &= "," & fld
                End If
            End If
            'mfields = mfields & " [" & dv.Columns.Item(j).Caption & "]"
            'If j < dv.Columns.Count - 1 Then
            '    mfields = mfields & ", "
            'End If
        Next
        Try
            n = dv.Rows.Count
            For i = 0 To n - 1
                Values = ""
                For j = 0 To dv.Columns.Count - 1
                    fld = dv.Columns(j).Caption
                    If fld.ToUpper <> "INDX" AndAlso fld.ToUpper <> "ID" Then
                        val = cleanText(dv.Rows(i)(j).ToString.Trim)
                        If ColumnTypeIsNumeric(dv.Columns(j)) Then
                            If val = "" Then val = 0
                        ElseIf ColumnTypeIsDateTime(dv.Columns(j)) Then
                            If val = "0" OrElse val = "NULL" OrElse val = "" Then
                                val = "NULL"
                            Else
                                val = "'" & DateToString(CDate(val), userconnprv) & "'"
                            End If

                        ElseIf dv.Columns(j).DataType.Name = "String" Then
                            If val = "" OrElse val = "NULL" Then
                                val = "' '"
                            Else
                                val = "'" & val & "'"
                            End If
                        Else
                            If val = "" OrElse val = "NULL" Then
                                val = "NULL"
                            Else
                                val = "'" & val & "'"
                            End If
                        End If
                        If Values = "" Then
                            Values = val
                        Else
                            Values = Values & "," & val
                        End If
                    End If
                Next
                m2SQL = "INSERT INTO " & tablename & " (" & mfields & ") VALUES (" & Values & ")"
                re = ExequteSQLquery(m2SQL, userconnstr, userconnprv)
                If re <> "Query executed fine." Then
                    ret = ret & " ERROR!! " & re
                End If
            Next
            'For i = 0 To n - 1
            '    m2SQL = ""
            '    m2SQL = "INSERT INTO [" & tablename & "](" & mfields & ") VALUES ("
            '    For j = 0 To dv.Columns.Count - 1
            '        If ColumnTypeIsNumeric(dv.Columns(j)) Then
            '            m2SQL = m2SQL & " " & dv.Rows(i)(j).ToString & ""
            '        Else
            '            m2SQL = m2SQL & " '" & dv.Rows(i)(j).ToString & "'"
            '        End If
            '        If j < dv.Columns.Count - 1 Then
            '            m2SQL = m2SQL & ", "
            '        ElseIf j = dv.Columns.Count - 1 Then
            '            m2SQL = m2SQL & ") "
            '        End If
            '    Next
            're = ExequteSQLquery(m2SQL, userconnstr, userconnprv)
            '    If re <> "Query executed fine." Then
            '        ret = ret & " ERROR!! " & re
            '    End If
            'Next
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Public Function GetListByODBC(ByVal collectionName As String, ByVal userconstr As String, ByVal userconprv As String, Optional ByVal restrictions() As String = Nothing) As DataTable
        'ODBC and OlDb GetSchema
        Dim dt As DataTable = Nothing
        If userconprv = "System.Data.Odbc" Then
            userconstr = userconstr.Replace("Password", "Pwd").Replace("User ID", "UID")
            Dim dataConnection As New System.Data.Odbc.OdbcConnection(userconstr)
            Try
                If dataConnection.State = ConnectionState.Closed Then dataConnection.Open()
                If restrictions Is Nothing Then
                    dt = dataConnection.GetSchema(collectionName)
                Else
                    dt = dataConnection.GetSchema(collectionName, restrictions)
                End If
            Catch ex As Exception
                dataConnection.Close()
                dataConnection.Dispose()
                Return Nothing
            End Try

        ElseIf userconprv = "System.Data.OleDb" Then
            'OleDb
            userconstr = ConfigurationManager.AppSettings("ACEOLEDBversion").ToString & "Persist Security Info=True;" & userconstr
            Dim dataConnection As New System.Data.OleDb.OleDbConnection(userconstr)
            Try
                If dataConnection.State = ConnectionState.Closed Then dataConnection.Open()
                If restrictions Is Nothing Then
                    dt = dataConnection.GetSchema(collectionName)
                Else
                    dt = dataConnection.GetSchema(collectionName, restrictions)
                End If
            Catch ex As Exception
                dataConnection.Close()
                dataConnection.Dispose()
                Return Nothing
            End Try
        End If
        Return dt
    End Function
    Public Function ExportDataTableToExcel(ByVal dt As DataTable, ByVal Expdir As String, ByVal Expfile As String, Optional ByVal hdr As String = "", Optional ByVal ftr As String = "", Optional ByVal dlm As String = "") As String
        Dim txtline As String = String.Empty
        Dim ret As String = String.Empty
        Dim m, i, j As Integer
        If dt Is Nothing Then
            Return ret
            Exit Function
        End If

        If dlm = "" Then dlm = Chr(9)
        Dim MyFile As StreamWriter = New StreamWriter(Expdir & Expfile)
        Try
            txtline = ""
            MyFile.WriteLine(hdr)
            txtline = ""
            MyFile.WriteLine(txtline)
            m = dt.Columns.Count
            For i = 0 To m - 1
                txtline = txtline & dt.Columns(i).ColumnName
                If i < m - 1 Then txtline = txtline & dlm
            Next
            MyFile.WriteLine(txtline)
            txtline = ""
            For j = 0 To dt.Rows.Count - 1
                txtline = ""
                For i = 0 To m - 1
                    txtline = txtline & """" & cleanText(dt.Rows(j).Item(i).ToString) & """"
                    If i < m - 1 Then txtline = txtline & dlm
                Next
                If Trim(txtline) <> "" Then
                    MyFile.WriteLine(txtline)
                End If
            Next
            txtline = ""
            MyFile.WriteLine(txtline)
            MyFile.WriteLine(ftr)
            MyFile.Flush()
            MyFile.Close()
            MyFile = Nothing
            ret = Expdir & Expfile
        Catch ex As Exception
            ret = "Error creating Excel file" & ex.Message
            MyFile.Close()
            MyFile = Nothing
        End Try
        Return ret
    End Function
    Public Function IsFilterDefined(ipaddress As String) As Boolean
        Dim parts As Integer = Pieces(ipaddress, ".")
        Dim sCheck As String = String.Empty
        Dim sql As String = String.Empty

        If parts > 1 AndAlso parts < 5 Then
            sCheck = ":" & ipaddress 'Piece(ipaddress, ".", 1, 2) & "."
            sql = "SELECT * FROM ouraccesslog Where [Action] LIKE '%" & sCheck & "%' AND Comments = 'ipfilter'"
            If HasRecords(sql) Then Return True
        End If
        Return False
    End Function
    Public Function SaveImageFromDataUrl(dataUrl As String, filePath As String) As String
        Dim ret As String = String.Empty

        ' Extract base64 data and image type from the Data URL
        Dim match As Match = Regex.Match(dataUrl, "data:image/(?<type>.+?);base64,(?<data>.+)")
        If match.Success Then
            Dim imageType As String = match.Groups("type").Value
            Dim base64Data As String = match.Groups("data").Value

            Try
            ' Convert base64 data to a byte array
            Dim binData As Byte() = Convert.FromBase64String(base64Data)
            ' Create a MemoryStream from the byte array
            Using ms As New MemoryStream(binData)
                ' Create an Image object from the MemoryStream
                Using img As Image = Image.FromStream(ms)
                    ' Determine the ImageFormat based on the extracted type
                    Dim format As ImageFormat
                    Select Case imageType.ToLower()
                        Case "png"
                            format = ImageFormat.Png
                        Case "jpeg", "jpg"
                            format = ImageFormat.Jpeg
                        Case "gif"
                            format = ImageFormat.Gif
                        Case "bmp"
                            format = ImageFormat.Bmp
                        Case "icon"
                            format = ImageFormat.Icon
                        Case Else
                            ' Default to PNG if format is unknown or unsupported
                            format = ImageFormat.Png
                    End Select

                    ' Save the image to the specified file path
                    img.Save(filePath, format)
                End Using
            End Using
            Catch ex As Exception
                ret = ex.Message
            End Try
        End If
        Return ret
    End Function

End Module


