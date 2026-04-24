Imports System.Data
Imports Oracle.ManagedDataAccess.Client
Public Module DataModuleProprietary

#Region "AddRowIntoTable - Oracle and InterSystems"
    Public Function AddRowIntoTable_Cache(ByVal mRow As DataRow, ByVal Tbl As String, ByVal myconstring As String) As String
        Dim ret As String = String.Empty
        'Dim myRecords As DataTable = New DataTable
        'Try
        '    Dim cacheBuilder As InterSystems.Data.CacheClient.CacheCommandBuilder                  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '    Dim dataCacheConnectionString As String = String.Empty
        '    Dim dataCacheConnection As New InterSystems.Data.CacheClient.CacheConnection
        '    Dim dataCommand As New InterSystems.Data.CacheClient.CacheCommand
        '    Dim ds As New System.Data.DataSet
        '    dataCacheConnection = New InterSystems.Data.CacheClient.CacheConnection(myconstring)
        '    If dataCacheConnection.State = ConnectionState.Closed Then dataCacheConnection.Open()
        '    dataCommand.Connection = dataCacheConnection
        '    dataCommand.CommandType = CommandType.Text
        '    dataCommand.CommandTimeout = 300000
        '    dataCommand.CommandText = "Select * FROM " & Tbl
        '    Dim dataAdapter As New InterSystems.Data.CacheClient.CacheDataAdapter(dataCommand)
        '    cacheBuilder = New InterSystems.Data.CacheClient.CacheCommandBuilder(dataAdapter)     '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '    dataAdapter.Fill(ds)
        '    If ds.Tables.Count > 0 Then
        '        myRecords = ds.Tables(0)
        '        'add row
        '        myRecords.ImportRow(mRow)                                                             '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '        dataAdapter.Update(myRecords)
        '    End If
        '    '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '    dataCacheConnection.Close()
        'Catch ex As Exception
        '    ret = ex.Message
        'End Try
        Return ret
    End Function

    Public Function AddRowIntoTable_IRIS(ByVal mRow As DataRow, ByVal Tbl As String, ByVal myconstring As String) As String
        Dim ret As String = String.Empty
        'Dim myRecords As DataTable = New DataTable
        'Try
        '    Dim IRISBuilder As InterSystems.Data.IRISClient.IRISCommandBuilder                 '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '    Dim dataIRISConnection As New InterSystems.Data.IRISClient.IRISConnection
        '    Dim dataCommand As New InterSystems.Data.IRISClient.IRISCommand
        '    Dim ds As New System.Data.DataSet
        '    dataIRISConnection = New InterSystems.Data.IRISClient.IRISConnection(myconstring)
        '    If dataIRISConnection.State = ConnectionState.Closed Then dataIRISConnection.Open()
        '    dataCommand.Connection = dataIRISConnection
        '    dataCommand.CommandType = CommandType.Text
        '    dataCommand.CommandTimeout = 300000
        '    dataCommand.CommandText = "Select * FROM " & Tbl
        '    Dim dataAdapter As New InterSystems.Data.IRISClient.IRISDataAdapter(dataCommand)
        '    IRISBuilder = New InterSystems.Data.IRISClient.IRISCommandBuilder(dataAdapter)     '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '    dataAdapter.Fill(ds)
        '    If ds.Tables.Count > 0 Then
        '        myRecords = ds.Tables(0)
        '        'add row
        '        myRecords.ImportRow(mRow)                                                             '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '        dataAdapter.Update(myRecords)
        '    End If
        '    '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '    dataIRISConnection.Close()
        'Catch ex As Exception
        '    ret = ex.Message
        'End Try
        Return ret
    End Function

    Public Function AddRowIntoTable_Oracle(ByVal mRow As DataRow, ByVal Tbl As String, ByVal myconstring As String) As String
        Dim ret As String = String.Empty
        Dim myRecords As DataTable = New DataTable
        'Try
        '    Dim cmdBuilder As Oracle.ManagedDataAccess.Client.OracleCommandBuilder
        '    Dim myConnection As Oracle.ManagedDataAccess.Client.OracleConnection
        '    Dim myCommand As New Oracle.ManagedDataAccess.Client.OracleCommand
        '    Dim myAdapter As Oracle.ManagedDataAccess.Client.OracleDataAdapter
        '    myConnection = New Oracle.ManagedDataAccess.Client.OracleConnection(myconstring)
        '    myCommand.Connection = myConnection
        '    myCommand.CommandType = CommandType.Text
        '    myCommand.CommandTimeout = 300000
        '    myCommand.CommandText = "Select * FROM " & Tbl
        '    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
        '    myAdapter = New Oracle.ManagedDataAccess.Client.OracleDataAdapter(myCommand)
        '    cmdBuilder = New Oracle.ManagedDataAccess.Client.OracleCommandBuilder(myAdapter)     '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '    myAdapter.Fill(myRecords)
        '    'add row
        '    myRecords.ImportRow(mRow)                         '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '    myAdapter.Update(myRecords)                       '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '    myAdapter.Dispose()
        '    myCommand.Connection.Close()
        '    myCommand.Dispose()
        'Catch ex As Exception
        '    ret = ex.Message
        'End Try
        Return ret
    End Function
#End Region

#Region "LoadDataTableIntoDbTable - Oracle and InterSystems"
    Public Function LoadDataTableIntoDbTable_Cache(ByVal dtt As DataTable, ByVal Tbl As String, ByVal myconstring As String) As String
        Dim ret As String = String.Empty
        'Dim myRecords As DataTable = New DataTable
        'Try
        '    Dim cacheBuilder As InterSystems.Data.CacheClient.CacheCommandBuilder                  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '    Dim dataCacheConnectionString As String = String.Empty
        '    Dim dataCacheConnection As New InterSystems.Data.CacheClient.CacheConnection
        '    Dim dataCommand As New InterSystems.Data.CacheClient.CacheCommand
        '    Dim ds As New System.Data.DataSet
        '    dataCacheConnection = New InterSystems.Data.CacheClient.CacheConnection(myconstring)
        '    If dataCacheConnection.State = ConnectionState.Closed Then dataCacheConnection.Open()
        '    dataCommand.Connection = dataCacheConnection
        '    dataCommand.CommandType = CommandType.Text
        '    dataCommand.CommandTimeout = 300000
        '    dataCommand.CommandText = "Select * FROM " & Tbl
        '    Dim dataAdapter As New InterSystems.Data.CacheClient.CacheDataAdapter(dataCommand)
        '    cacheBuilder = New InterSystems.Data.CacheClient.CacheCommandBuilder(dataAdapter)     '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '    dataAdapter.Fill(ds)
        '    If ds.Tables.Count > 0 Then
        '        myRecords = ds.Tables(0)
        '        'add row
        '        For i As Integer = 0 To dtt.Rows.Count - 1
        '            myRecords.ImportRow(dtt.Rows(i))
        '        Next
        '        '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '        dataAdapter.Update(myRecords)
        '    End If
        '    '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '    dataCacheConnection.Close()
        'Catch ex As Exception
        '    ret = ex.Message
        'End Try
        Return ret
    End Function

    Public Function LoadDataTableIntoDbTable_IRIS(ByVal dtt As DataTable, ByVal Tbl As String, ByVal myconstring As String) As String
        Dim ret As String = String.Empty
        'Dim myRecords As DataTable = New DataTable
        'Try
        '    Dim IRISBuilder As InterSystems.Data.IRISClient.IRISCommandBuilder                 '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '    Dim dataIRISConnection As New InterSystems.Data.IRISClient.IRISConnection
        '    Dim dataCommand As New InterSystems.Data.IRISClient.IRISCommand
        '    Dim ds As New System.Data.DataSet
        '    dataIRISConnection = New InterSystems.Data.IRISClient.IRISConnection(myconstring)
        '    If dataIRISConnection.State = ConnectionState.Closed Then dataIRISConnection.Open()
        '    dataCommand.Connection = dataIRISConnection
        '    dataCommand.CommandType = CommandType.Text
        '    dataCommand.CommandTimeout = 300000
        '    dataCommand.CommandText = "Select * FROM " & Tbl
        '    Dim dataAdapter As New InterSystems.Data.IRISClient.IRISDataAdapter(dataCommand)
        '    IRISBuilder = New InterSystems.Data.IRISClient.IRISCommandBuilder(dataAdapter)     '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '    dataAdapter.Fill(ds)
        '    If ds.Tables.Count > 0 Then
        '        myRecords = ds.Tables(0)
        '        'add row
        '        For i As Integer = 0 To dtt.Rows.Count - 1
        '            myRecords.ImportRow(dtt.Rows(i))
        '        Next                                                          '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '        dataAdapter.Update(myRecords)
        '    End If
        '    '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '    dataIRISConnection.Close()
        'Catch ex As Exception
        '    ret = ex.Message
        'End Try
        Return ret
    End Function

    Public Function LoadDataTableIntoDbTable_Oracle(ByVal dtt As DataTable, ByVal Tbl As String, ByVal myconstring As String) As String
        Dim ret As String = String.Empty
        'Dim myRecords As DataTable = New DataTable
        'Try
        '    Dim cmdBuilder As Oracle.ManagedDataAccess.Client.OracleCommandBuilder
        '    Dim myConnection As Oracle.ManagedDataAccess.Client.OracleConnection
        '    Dim myCommand As New Oracle.ManagedDataAccess.Client.OracleCommand
        '    Dim myAdapter As Oracle.ManagedDataAccess.Client.OracleDataAdapter
        '    myConnection = New Oracle.ManagedDataAccess.Client.OracleConnection(myconstring)
        '    myCommand.Connection = myConnection
        '    myCommand.CommandType = CommandType.Text
        '    myCommand.CommandTimeout = 300000
        '    myCommand.CommandText = "Select * FROM " & Tbl
        '    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
        '    myAdapter = New Oracle.ManagedDataAccess.Client.OracleDataAdapter(myCommand)
        '    cmdBuilder = New Oracle.ManagedDataAccess.Client.OracleCommandBuilder(myAdapter)     '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '    myAdapter.Fill(myRecords)
        '    'add row
        '    For i As Integer = 0 To dtt.Rows.Count - 1
        '        myRecords.ImportRow(dtt.Rows(i))
        '    Next
        '    myAdapter.Update(myRecords)                       '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        '    myAdapter.Dispose()
        '    myCommand.Connection.Close()
        '    myCommand.Dispose()
        'Catch ex As Exception
        '    ret = ex.Message
        'End Try
        Return ret
    End Function
#End Region

#Region "HasRecords - Oracle and InterSystems"
    Public Function HasRecords_IRIS(ByVal mySQL As String, ByVal myconstring As String, ByRef myRecords As DataTable) As String
        Dim ret As String = String.Empty
        Dim hasrec As Boolean
        'Try
        '    Dim dataIRISConnection As New InterSystems.Data.IRISClient.IRISConnection
        '    Dim dataCommand As New InterSystems.Data.IRISClient.IRISCommand
        '    Dim ds As New System.Data.DataSet
        '    dataIRISConnection = New InterSystems.Data.IRISClient.IRISConnection(myconstring)

        '    If dataIRISConnection.State = ConnectionState.Closed Then dataIRISConnection.Open()
        '    dataCommand.Connection = dataIRISConnection
        '    dataCommand.CommandType = CommandType.Text
        '    dataCommand.CommandTimeout = 300000
        '    dataCommand.CommandText = mySQL
        '    Dim dataAdapter As New InterSystems.Data.IRISClient.IRISDataAdapter(dataCommand)
        '    ' catch "Incorrect list format" error and ignore it. It is an internal Cache
        '    ' error and does not affect getting the data into the dataset.
        '    ' All other errors are returned.
        '    Try
        '        dataAdapter.Fill(ds)
        '    Catch exc As Exception
        '        If Not exc.Message.ToUpper.StartsWith("INCORRECT LIST FORMAT:") Then
        '            ret = "RETURN_FALSE"
        '            Return ret
        '        End If
        '    End Try
        '    'dataAdapter.Fill(ds)
        '    If ds.Tables.Count > 0 Then myRecords = ds.Tables(0)
        '    dataAdapter.Dispose()
        '    dataCommand.Dispose()
        '    dataIRISConnection.Close()
        'Catch ex As Exception
        '    ret = ex.Message
        '     Return False
        'End Try
        Return hasrec
    End Function

    Public Function HasRecords_Cache(ByVal mySQL As String, ByRef myconstring As String, ByRef myRecords As DataTable) As String
        Dim ret As String = String.Empty
        Dim hasrec As Boolean
        'Try
        '    Dim dataCacheConnectionString As String = String.Empty
        '    Dim dataCacheConnection As New InterSystems.Data.CacheClient.CacheConnection
        '    Dim dataCommand As New InterSystems.Data.CacheClient.CacheCommand
        '    Dim ds As New System.Data.DataSet
        '    If myconstring = String.Empty Then
        '        myconstring = InterSystems.Data.CacheClient.CacheConnection.ConnectDlg()
        '    End If
        '    dataCacheConnection = New InterSystems.Data.CacheClient.CacheConnection(myconstring)
        '    If dataCacheConnection.State = ConnectionState.Closed Then dataCacheConnection.Open()
        '    dataCommand.Connection = dataCacheConnection
        '    dataCommand.CommandType = CommandType.Text
        '    dataCommand.CommandTimeout = 300000
        '    dataCommand.CommandText = mySQL
        '    Dim dataAdapter As New InterSystems.Data.CacheClient.CacheDataAdapter(dataCommand)
        '    ' catch "Incorrect list format" error and ignore it. It is an internal Cache
        '    ' error and does not affect getting the data into the dataset.
        '    Try
        '        dataAdapter.Fill(ds)
        '    Catch exc As Exception
        '        If Not exc.Message.ToUpper.StartsWith("INCORRECT LIST FORMAT:") Then
        '            ret = "RETURN_FALSE"
        '            Return ret
        '        End If
        '    End Try
        '    'dataAdapter.Fill(ds)
        '    If ds.Tables.Count > 0 Then myRecords = ds.Tables(0)
        '    dataCacheConnection.Close()
        'Catch ex As Exception
        '    ret = ex.Message
        '     Return False
        'End Try
        Return hasrec
    End Function

    Public Function HasRecords_Oracle(ByVal mySQL As String, ByVal myconstring As String, ByRef myRecords As DataTable) As String
        Dim ret As String = String.Empty
        Dim hasrec As Boolean
        'Try
        '    Dim myConnection As Oracle.ManagedDataAccess.Client.OracleConnection
        '    Dim myCommand As New Oracle.ManagedDataAccess.Client.OracleCommand
        '    Dim myAdapter As Oracle.ManagedDataAccess.Client.OracleDataAdapter
        '    myConnection = New Oracle.ManagedDataAccess.Client.OracleConnection(myconstring)
        '    myCommand.Connection = myConnection
        '    myCommand.CommandType = CommandType.Text
        '    myCommand.CommandTimeout = 300000
        '    myCommand.CommandText = mySQL
        '    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
        '    myAdapter = New Oracle.ManagedDataAccess.Client.OracleDataAdapter(myCommand)
        '    myAdapter.Fill(myRecords)
        '    myAdapter.Dispose()
        '    myCommand.Connection.Close()
        '    myCommand.Dispose()
        'Catch ex As Exception
        '    ret = ex.Message
        '     Return False
        'End Try
        Return hasrec
    End Function
#End Region

#Region "CountOfRecords - Oracle and InterSystems"
    Public Function CountOfRecords_IRIS(ByVal mySQL As String, ByVal myconstring As String, ByRef myRecords As DataTable) As String
        Dim ret As String = String.Empty
        'Try
        '    Dim dataIRISConnection As New InterSystems.Data.IRISClient.IRISConnection
        '    Dim dataCommand As New InterSystems.Data.IRISClient.IRISCommand
        '    Dim ds As New System.Data.DataSet
        '    dataIRISConnection = New InterSystems.Data.IRISClient.IRISConnection(myconstring)

        '    If dataIRISConnection.State = ConnectionState.Closed Then dataIRISConnection.Open()
        '    dataCommand.Connection = dataIRISConnection
        '    dataCommand.CommandType = CommandType.Text
        '    dataCommand.CommandTimeout = 300000
        '    dataCommand.CommandText = mySQL
        '    Dim dataAdapter As New InterSystems.Data.IRISClient.IRISDataAdapter(dataCommand)
        '    ' catch "Incorrect list format" error and ignore it. It is an internal Cache
        '    ' error and does not affect getting the data into the dataset.
        '    ' All other errors are returned.
        '    Try
        '        dataAdapter.Fill(ds)
        '    Catch exc As Exception
        '        If Not exc.Message.ToUpper.StartsWith("INCORRECT LIST FORMAT:") Then
        '            ret = "ERROR!! " & exc.Message
        '            Return ret
        '        End If
        '    End Try
        '    'dataAdapter.Fill(ds)
        '    If ds.Tables.Count > 0 Then myRecords = ds.Tables(0)
        '    dataAdapter.Dispose()
        '    dataCommand.Dispose()
        '    dataIRISConnection.Close()
        'Catch ex As Exception
        '    ret = ex.Message
        'End Try
        Return ret
    End Function

    Public Function CountOfRecords_Cache(ByVal mySQL As String, ByRef myconstring As String, ByRef myRecords As DataTable) As String
        Dim ret As String = String.Empty
        'Try
        '    Dim dataCacheConnectionString As String = String.Empty
        '    Dim dataCacheConnection As New InterSystems.Data.CacheClient.CacheConnection
        '    Dim dataCommand As New InterSystems.Data.CacheClient.CacheCommand
        '    Dim ds As New System.Data.DataSet
        '    If myconstring = String.Empty Then
        '        myconstring = InterSystems.Data.CacheClient.CacheConnection.ConnectDlg()
        '    End If
        '    dataCacheConnection = New InterSystems.Data.CacheClient.CacheConnection(myconstring)
        '    If dataCacheConnection.State = ConnectionState.Closed Then dataCacheConnection.Open()
        '    dataCommand.Connection = dataCacheConnection
        '    dataCommand.CommandType = CommandType.Text
        '    dataCommand.CommandTimeout = 300000
        '    dataCommand.CommandText = mySQL
        '    Dim dataAdapter As New InterSystems.Data.CacheClient.CacheDataAdapter(dataCommand)
        '    ' catch "Incorrect list format" error and ignore it. It is an internal Cache
        '    ' error and does not affect getting the data into the dataset.
        '    Try
        '        dataAdapter.Fill(ds)
        '    Catch exc As Exception
        '        If Not exc.Message.ToUpper.StartsWith("INCORRECT LIST FORMAT:") Then
        '            ret = "ERROR!! " & exc.Message
        '            Return ret
        '        End If
        '    End Try
        '    'dataAdapter.Fill(ds)
        '    If ds.Tables.Count > 0 Then myRecords = ds.Tables(0)
        '    dataCacheConnection.Close()
        'Catch ex As Exception
        '    ret = ex.Message
        'End Try
        Return ret
    End Function

    Public Function CountOfRecords_Oracle(ByVal mySQL As String, ByVal myconstring As String, ByRef myRecords As DataTable) As String
        Dim ret As String = String.Empty
        'Try
        '    Dim myConnection As Oracle.ManagedDataAccess.Client.OracleConnection
        '    Dim myCommand As New Oracle.ManagedDataAccess.Client.OracleCommand
        '    Dim myAdapter As Oracle.ManagedDataAccess.Client.OracleDataAdapter
        '    myConnection = New Oracle.ManagedDataAccess.Client.OracleConnection(myconstring)
        '    myCommand.Connection = myConnection
        '    myCommand.CommandType = CommandType.Text
        '    myCommand.CommandTimeout = 300000
        '    myCommand.CommandText = mySQL
        '    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
        '    myAdapter = New Oracle.ManagedDataAccess.Client.OracleDataAdapter(myCommand)
        '    myAdapter.Fill(myRecords)
        '    myAdapter.Dispose()
        '    myCommand.Connection.Close()
        '    myCommand.Dispose()
        'Catch ex As Exception
        '    ret = ex.Message
        'End Try
        Return ret
    End Function
#End Region

#Region "GetGlobalNodeValue - InterSystems"
    Public Function GetGlobalNodeValue_IRIS(ByVal glb As String, ByVal myconstring As String) As String
        Dim ret As String = String.Empty
        'Try
        '    Dim dataCacheConnection As New InterSystems.Data.IRISClient.IRISConnection
        '    Dim dataCommand As New InterSystems.Data.IRISClient.IRISCommand
        '    dataCacheConnection = New InterSystems.Data.IRISClient.IRISConnection(myconstring)
        '    If dataCacheConnection.State = ConnectionState.Closed Then dataCacheConnection.Open()
        '    dataCommand.Connection = dataCacheConnection
        '    dataCommand.CommandType = CommandType.StoredProcedure
        '    dataCommand.CommandTimeout = 300000
        '    dataCommand.CommandText = "OUR.GETGLOBALNODEDATA"
        '    dataCommand.Parameters.Add("glb", glb)
        '    Dim dataAdapter As New InterSystems.Data.IRISClient.IRISDataAdapter(dataCommand)
        '    Dim ds As New System.Data.DataSet
        '    dataAdapter.Fill(ds)
        '    If ds Is Nothing OrElse ds.Tables Is Nothing Then
        '        ret = "No data..."
        '        Return ret
        '    End If
        '    ret = ds.Tables(0).Rows(0).Item("VALUE").ToString
        '    dataAdapter.Dispose()
        '    dataCommand.Dispose()
        '    dataCacheConnection.Close()
        'Catch ex As Exception
        '    ret = ex.Message
        'End Try
        Return ret
    End Function

    Public Function GetGlobalNodeValue_Cache(ByVal glb As String, ByRef myconstring As String) As String
        Dim ret As String = String.Empty
        'Try
        '    Dim dataCacheConnection As New InterSystems.Data.CacheClient.CacheConnection
        '    Dim dataCommand As New InterSystems.Data.CacheClient.CacheCommand
        '    If myconstring = String.Empty Then
        '        myconstring = InterSystems.Data.CacheClient.CacheConnection.ConnectDlg()
        '    End If
        '    dataCacheConnection = New InterSystems.Data.CacheClient.CacheConnection(myconstring)
        '    If dataCacheConnection.State = ConnectionState.Closed Then dataCacheConnection.Open()
        '    dataCommand.Connection = dataCacheConnection
        '    dataCommand.CommandType = CommandType.StoredProcedure
        '    dataCommand.CommandTimeout = 300000
        '    dataCommand.CommandText = "OUR.GETGLOBALNODEDATA"
        '    dataCommand.Parameters.Add("glb", glb)
        '    Dim dataAdapter As New InterSystems.Data.CacheClient.CacheDataAdapter(dataCommand)
        '    Dim ds As New System.Data.DataSet
        '    dataAdapter.Fill(ds)
        '    If ds Is Nothing OrElse ds.Tables Is Nothing Then
        '        ret = "No data..."
        '        Return ret
        '    End If
        '    ret = ds.Tables(0).Rows(0).Item("VALUE").ToString
        '    dataAdapter.Dispose()
        '    dataCommand.Dispose()
        '    dataCacheConnection.Close()
        'Catch ex As Exception
        '    ret = ex.Message
        'End Try
        Return ret
    End Function
#End Region

#Region "mRecords - Oracle and InterSystems"
    Public Function mRecords_IRIS(ByVal mySQL As String, ByRef er As String, ByVal myconstring As String, ByRef myRecords As DataTable) As String
        Dim ret As String = String.Empty
        'Try
        '    If mySQL.ToUpper.IndexOf(" DISTINCT ") > 0 Then
        '        ret = "USE_DISTINCT"
        '        Return ret
        '    Else
        '        Dim dataIRISConnection As New InterSystems.Data.IRISClient.IRISConnection
        '        Dim dataCommand As New InterSystems.Data.IRISClient.IRISCommand
        '        Try
        '            Dim ds As New System.Data.DataSet
        '            dataIRISConnection = New InterSystems.Data.IRISClient.IRISConnection(myconstring)
        '            If dataIRISConnection.State = ConnectionState.Closed Then dataIRISConnection.Open()
        '            dataCommand.Connection = dataIRISConnection
        '            dataCommand.CommandType = CommandType.Text
        '            dataCommand.CommandTimeout = 300000
        '            dataCommand.CommandText = mySQL
        '            Dim dataAdapter As New InterSystems.Data.IRISClient.IRISDataAdapter(dataCommand)
        '            ' catch "Incorrect list format" error and ignore it. It is an internal Cache
        '            ' error and does not affect getting the data into the dataset.
        '            ' All other errors are returned.
        '            Try
        '                dataAdapter.Fill(ds)
        '            Catch exc As Exception
        '                If Not exc.Message.ToUpper.StartsWith("INCORRECT LIST FORMAT:") Then
        '                    er = "ERROR!! " & exc.Message
        '                    ret = "RETURN_VIEW"
        '                    Return ret
        '                End If
        '            End Try
        '            'dataAdapter.Fill(ds)
        '            If ds.Tables.Count > 0 Then myRecords = ds.Tables(0)
        '            dataAdapter.Dispose()
        '        Catch ex As Exception
        '            'assign error er
        '            er = "ERROR!! " & ex.Message
        '        End Try
        '        dataCommand.Dispose()
        '        dataIRISConnection.Close()
        '    End If
        'Catch ex As Exception
        '    ret = ex.Message
        'End Try
        Return ret
    End Function

    Public Function mRecords_Cache(ByVal mySQL As String, ByRef er As String, ByRef myconstring As String, ByRef myRecords As DataTable) As String
        Dim ret As String = String.Empty
        'Try
        '    If mySQL.ToUpper.IndexOf(" DISTINCT ") > 0 Then
        '        ret = "USE_DISTINCT"
        '        Return ret
        '    Else
        '        Dim dataCacheConnectionString As String = String.Empty
        '        Dim dataCacheConnection As New InterSystems.Data.CacheClient.CacheConnection
        '        Dim dataCommand As New InterSystems.Data.CacheClient.CacheCommand
        '        Try
        '            Dim ds As New System.Data.DataSet
        '            If myconstring = String.Empty Then
        '                myconstring = InterSystems.Data.CacheClient.CacheConnection.ConnectDlg()
        '            End If
        '            dataCacheConnection = New InterSystems.Data.CacheClient.CacheConnection(myconstring)
        '            If dataCacheConnection.State = ConnectionState.Closed Then dataCacheConnection.Open()
        '            dataCommand.Connection = dataCacheConnection
        '            dataCommand.CommandType = CommandType.Text
        '            dataCommand.CommandTimeout = 300000
        '            dataCommand.CommandText = mySQL
        '            Dim dataAdapter As New InterSystems.Data.CacheClient.CacheDataAdapter(dataCommand)
        '            ' catch "Incorrect list format" error and ignore it. It is an internal Cache
        '            ' error and does not affect getting the data into the dataset.
        '            ' All other errors are returned.
        '            Try
        '                dataAdapter.Fill(ds)
        '            Catch exc As Exception
        '                If Not exc.Message.ToUpper.StartsWith("INCORRECT LIST FORMAT:") Then
        '                    er = "ERROR!! " & exc.Message
        '                    ret = "RETURN_VIEW"
        '                    Return ret
        '                End If
        '            End Try
        '            'dataAdapter.Fill(ds)
        '            If ds.Tables.Count > 0 Then myRecords = ds.Tables(0)
        '            dataAdapter.Dispose()

        '        Catch ex As Exception
        '            'assign error er
        '            er = "ERROR!! " & ex.Message
        '        End Try
        '        dataCommand.Dispose()
        '        dataCacheConnection.Close()
        '    End If
        'Catch ex As Exception
        '    ret = ex.Message
        'End Try
        Return ret
    End Function

    Public Function mRecords_Oracle(ByVal mySQL As String, ByRef er As String, ByVal myconstring As String, ByRef myRecords As DataTable) As String
        Dim ret As String = String.Empty
        'Try
        '    Dim myConnection As Oracle.ManagedDataAccess.Client.OracleConnection
        '    Dim myCommand As New Oracle.ManagedDataAccess.Client.OracleCommand
        '    Dim myAdapter As Oracle.ManagedDataAccess.Client.OracleDataAdapter
        '    myConnection = New Oracle.ManagedDataAccess.Client.OracleConnection(myconstring)
        '    myCommand.Connection = myConnection
        '    myCommand.CommandType = CommandType.Text
        '    myCommand.CommandTimeout = 300000
        '    myCommand.CommandText = mySQL
        '    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
        '    myAdapter = New Oracle.ManagedDataAccess.Client.OracleDataAdapter(myCommand)
        '    Try
        '        myAdapter.Fill(myRecords)
        '    Catch ex As Exception
        '        'assign error er
        '        'er = "ERROR!! " & ex.Message
        '    End Try
        '    myAdapter.Dispose()
        '    myCommand.Connection.Close()
        '    myCommand.Dispose()
        'Catch ex As Exception
        '    ret = ex.Message
        'End Try
        Return ret
    End Function
#End Region

#Region "mRecordsFromSP - Oracle and InterSystems"
    Public Function mRecordsFromSP_IRIS(ByVal mySP As String, ByVal Nparameters As Integer, ByVal ParamName As Array, ByVal ParamType As Array, ByVal ParamValue As Array, ByVal myconstring As String, ByRef myRecords As DataTable) As String
        Dim ret As String = String.Empty
        'Try
        '    Dim n As Integer = Nparameters
        '    Dim dataCacheConnectionString As String = String.Empty
        '    Dim dataCacheConnection As New InterSystems.Data.IRISClient.IRISConnection
        '    Dim dataCommand As New InterSystems.Data.IRISClient.IRISCommand
        '    dataCacheConnection = New InterSystems.Data.IRISClient.IRISConnection(myconstring)
        '    If dataCacheConnection.State = ConnectionState.Closed Then dataCacheConnection.Open()
        '    dataCommand.Connection = dataCacheConnection
        '    dataCommand.CommandType = CommandType.StoredProcedure
        '    dataCommand.CommandText = mySP
        '    dataCommand.CommandTimeout = 300000
        '    Dim i As Integer
        '    If Nparameters > 0 Then
        '        For i = 0 To n - 1
        '            If Not ParamValue(i) Is Nothing Then
        '                dataCommand.Parameters.Add(ParamName(i).ToString, ParamValue(i))  'working !!!
        '            End If
        '        Next
        '    End If
        '    Dim dataAdapter As New InterSystems.Data.IRISClient.IRISDataAdapter(dataCommand)
        '    Dim ds As New System.Data.DataSet
        '    dataAdapter.Fill(ds)
        '    If ds.Tables.Count > 0 Then myRecords = ds.Tables(0)
        '    dataAdapter.Dispose()
        '    dataCommand.Dispose()
        '    dataCacheConnection.Close()
        'Catch ex As Exception
        '    ret = ex.Message
        'End Try
        Return ret
    End Function

    Public Function mRecordsFromSP_Cache(ByVal mySP As String, ByVal Nparameters As Integer, ByVal ParamName As Array, ByVal ParamType As Array, ByVal ParamValue As Array, ByRef myconstring As String, ByRef myRecords As DataTable) As String
        Dim ret As String = String.Empty
        'Try
        '    Dim n As Integer = Nparameters
        '    Dim dataCacheConnectionString As String = String.Empty
        '    Dim dataCacheConnection As New InterSystems.Data.CacheClient.CacheConnection
        '    Dim dataCommand As New InterSystems.Data.CacheClient.CacheCommand
        '    Dim ds As New System.Data.DataSet
        '    If myconstring = String.Empty Then
        '        myconstring = InterSystems.Data.CacheClient.CacheConnection.ConnectDlg()
        '    End If
        '    dataCacheConnection = New InterSystems.Data.CacheClient.CacheConnection(myconstring)
        '    If dataCacheConnection.State = ConnectionState.Closed Then dataCacheConnection.Open()
        '    dataCommand.Connection = dataCacheConnection
        '    dataCommand.CommandType = CommandType.StoredProcedure
        '    dataCommand.CommandText = mySP
        '    dataCommand.CommandTimeout = 300000
        '    Dim i As Integer
        '    If Nparameters > 0 Then
        '        For i = 0 To n - 1
        '            If Not ParamValue(i) Is Nothing Then
        '                dataCommand.Parameters.Add(ParamName(i).ToString, ParamValue(i))  'working !!!
        '            End If
        '        Next
        '    End If
        '    Dim dataAdapter As New InterSystems.Data.CacheClient.CacheDataAdapter(dataCommand)
        '    dataAdapter.Fill(ds)
        '    If ds.Tables.Count > 0 Then myRecords = ds.Tables(0)
        '    dataAdapter.Dispose()
        '    dataCommand.Dispose()
        '    dataCacheConnection.Close()
        'Catch ex As Exception
        '    ret = ex.Message
        'End Try
        Return ret
    End Function

    Public Function mRecordsFromSP_Oracle(ByVal mySP As String, ByVal Nparameters As Integer, ByVal ParamName As Array, ByVal ParamType As Array, ByVal ParamValue As Array, ByVal myconstring As String, ByVal myconstr As String, ByVal myconprv As String, ByRef myRecords As DataTable) As String
        Dim ret As String = String.Empty
        'Try
        '    Dim myConnection As Oracle.ManagedDataAccess.Client.OracleConnection
        '    Dim myCommand As New Oracle.ManagedDataAccess.Client.OracleCommand
        '    Dim myAdapter As Oracle.ManagedDataAccess.Client.OracleDataAdapter
        '    myConnection = New Oracle.ManagedDataAccess.Client.OracleConnection(myconstring)
        '    myCommand.Connection = myConnection
        '    myCommand.CommandType = CommandType.StoredProcedure
        '    myCommand.CommandTimeout = 300000
        '    myCommand.CommandText = mySP
        '    If Nparameters > 0 Then
        '        Dim param(Nparameters - 1) As Oracle.ManagedDataAccess.Client.OracleParameter
        '        For i = 0 To Nparameters - 1
        '            If ParamType(i) = "nvarchar" Then
        '                param(i) = New Oracle.ManagedDataAccess.Client.OracleParameter(ParamName(i).ToString, OracleDbType.Varchar2, 255, ParameterDirection.Input)
        '            ElseIf ParamType(i) = "datetime" Then
        '                param(i) = New Oracle.ManagedDataAccess.Client.OracleParameter(ParamName(i).ToString, OracleDbType.TimeStamp)
        '                param(i).Direction = ParameterDirection.Input
        '            Else
        '                param(i) = New Oracle.ManagedDataAccess.Client.OracleParameter(ParamName(i).ToString, OracleDbType.Single)
        '                param(i).Direction = ParameterDirection.Input
        '            End If
        '            param(i).Value = ParamValue(i)
        '            myCommand.Parameters.Add(param(i))
        '        Next
        '    End If
        '    Dim OutParams As ListItemCollection = GetListOfSPOutParams(mySP, myconstr, myconprv)

        '    If OutParams.Count > 0 Then
        '        Dim li As ListItem
        '        Dim OutParam As Oracle.ManagedDataAccess.Client.OracleParameter
        '        For i = 0 To OutParams.Count - 1
        '            li = OutParams.Item(i)
        '            If li.Value = "nvarchar" Then
        '                OutParam = New Oracle.ManagedDataAccess.Client.OracleParameter(li.Text, OracleDbType.Varchar2, 255, ParameterDirection.Output)
        '            ElseIf li.Value = "datetime" Then
        '                OutParam = New Oracle.ManagedDataAccess.Client.OracleParameter(li.Text, OracleDbType.TimeStamp)
        '                OutParam.Direction = ParameterDirection.Output
        '            ElseIf li.Value = "cursor" Then
        '                OutParam = New Oracle.ManagedDataAccess.Client.OracleParameter(li.Text, OracleDbType.RefCursor)
        '                OutParam.Direction = ParameterDirection.Output
        '            Else
        '                OutParam = New Oracle.ManagedDataAccess.Client.OracleParameter(li.Text, OracleDbType.Single)
        '                OutParam.Direction = ParameterDirection.Output
        '            End If
        '            myCommand.Parameters.Add(OutParam)
        '        Next
        '    End If
        '    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
        '    myAdapter = New Oracle.ManagedDataAccess.Client.OracleDataAdapter(myCommand)
        '    myAdapter.Fill(myRecords)
        '    myAdapter.Dispose()
        '    myCommand.Connection.Close()
        '    myCommand.Dispose()
        'Catch ex As Exception
        '    ret = ex.Message
        'End Try
        Return ret
    End Function
#End Region

#Region "RunSP - Oracle and InterSystems"
    Public Function RunSP_IRIS(ByVal mySP As String, ByVal Nparameters As Integer, ByVal ParamName As Array, ByVal ParamType As Array, ByVal ParamValue As Array, ByVal myconstring As String) As String
        Dim ret As String = String.Empty
        'Try
        '    Dim dataCacheConnectionString As String = String.Empty
        '    Dim dataCacheConnection As New InterSystems.Data.IRISClient.IRISConnection
        '    Dim dataCommand As New InterSystems.Data.IRISClient.IRISCommand
        '    dataCacheConnection = New InterSystems.Data.IRISClient.IRISConnection(myconstring)
        '    If dataCacheConnection.State = ConnectionState.Closed Then dataCacheConnection.Open()
        '    dataCommand.Connection = dataCacheConnection
        '    dataCommand.CommandType = CommandType.StoredProcedure
        '    dataCommand.CommandText = mySP
        '    dataCommand.CommandTimeout = 300000
        '    Dim i As Integer
        '    If Nparameters > 0 Then
        '        For i = 0 To Nparameters - 1
        '            If Not ParamValue(i) Is Nothing Then
        '                dataCommand.Parameters.Add(ParamName(i).ToString, ParamValue(i))  'working !!!
        '            End If
        '        Next
        '    End If
        '    dataCommand.ExecuteNonQuery()
        '    dataCommand.Dispose()
        '    dataCacheConnection.Close()
        'Catch ex As Exception
        '    ret = ex.Message
        'End Try
        Return ret
    End Function

    Public Function RunSP_Cache(ByVal mySP As String, ByVal Nparameters As Integer, ByVal ParamName As Array, ByVal ParamType As Array, ByVal ParamValue As Array, ByRef myconstring As String) As String
        Dim ret As String = String.Empty
        'Try
        '    Dim dataCacheConnectionString As String = String.Empty
        '    Dim dataCacheConnection As New InterSystems.Data.CacheClient.CacheConnection
        '    Dim dataCommand As New InterSystems.Data.CacheClient.CacheCommand
        '    Dim ds As New System.Data.DataSet
        '    If myconstring = String.Empty Then
        '        myconstring = InterSystems.Data.CacheClient.CacheConnection.ConnectDlg()
        '    End If
        '    dataCacheConnection = New InterSystems.Data.CacheClient.CacheConnection(myconstring)
        '    If dataCacheConnection.State = ConnectionState.Closed Then dataCacheConnection.Open()
        '    dataCommand.Connection = dataCacheConnection
        '    dataCommand.CommandType = CommandType.StoredProcedure
        '    dataCommand.CommandText = mySP
        '    dataCommand.CommandTimeout = 300000
        '    If Nparameters > 0 Then
        '        For i = 0 To Nparameters - 1
        '            If Not ParamValue(i) Is Nothing Then
        '                dataCommand.Parameters.Add(ParamName(i).ToString, ParamValue(i))  'working !!!
        '            End If
        '        Next
        '    End If
        '    dataCommand.ExecuteNonQuery()
        '    dataCommand.Dispose()
        '    dataCacheConnection.Close()
        'Catch ex As Exception
        '    ret = ex.Message
        'End Try
        Return ret
    End Function

    Public Function RunSP_Oracle(ByVal mySP As String, ByVal Nparameters As Integer, ByVal ParamName As Array, ByVal ParamType As Array, ByVal ParamValue As Array, ByVal myconstring As String) As String
        Dim ret As String = String.Empty
        'Try
        '    Dim myConnection As Oracle.ManagedDataAccess.Client.OracleConnection
        '    Dim myCommand As New Oracle.ManagedDataAccess.Client.OracleCommand
        '    myConnection = New Oracle.ManagedDataAccess.Client.OracleConnection(myconstring)
        '    myCommand.Connection = myConnection
        '    myCommand.CommandType = CommandType.StoredProcedure
        '    myCommand.CommandTimeout = 300000
        '    myCommand.CommandText = mySP
        '    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
        '    Dim param(Nparameters) As MySql.Data.MySqlClient.MySqlParameter
        '    For i = 0 To Nparameters - 1
        '        If ParamType(i) = "nvarchar" Then
        '            param(i) = New MySql.Data.MySqlClient.MySqlParameter("@" + ParamName(i), MySql.Data.MySqlClient.MySqlDbType.VarChar, 255, ParameterDirection.Input)
        '        ElseIf ParamType(i) = "datetime" Then
        '            param(i) = New MySql.Data.MySqlClient.MySqlParameter("@" + ParamName(i), MySql.Data.MySqlClient.MySqlDbType.DateTime, 255, ParameterDirection.Input)
        '        Else
        '            param(i) = New MySql.Data.MySqlClient.MySqlParameter("@" + ParamName(i), MySql.Data.MySqlClient.MySqlDbType.Int16, 255, ParameterDirection.Input)
        '        End If
        '        param(i).Value = ParamValue(i)
        '        myCommand.Parameters.Add(param(i))
        '    Next
        '    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
        '    myCommand.ExecuteNonQuery()
        '    myCommand.Connection.Close()
        '    myCommand.Dispose()
        'Catch ex As Exception
        '    ret = ex.Message
        'End Try
        Return ret
    End Function
#End Region

#Region "ExequteSQLquery - Oracle and InterSystems"
    Public Function ExequteSQLquery_IRIS(ByVal SQLq As String, ByVal myconstring As String) As String
        Dim r As String = "Query executed fine."
        'Try
        '    Dim dataIRISConnection As New InterSystems.Data.IRISClient.IRISConnection
        '    Dim dataCommand As New InterSystems.Data.IRISClient.IRISCommand
        '    Dim ds As New System.Data.DataSet
        '    dataIRISConnection = New InterSystems.Data.IRISClient.IRISConnection(myconstring)
        '    Try
        '        If dataIRISConnection.State = ConnectionState.Closed Then dataIRISConnection.Open()
        '        dataCommand.Connection = dataIRISConnection
        '        dataCommand.CommandType = CommandType.Text
        '        dataCommand.CommandTimeout = 300000
        '        dataCommand.CommandText = SQLq
        '        dataCommand.ExecuteNonQuery()
        '        dataCommand.Dispose()
        '        dataIRISConnection.Close()
        '    Catch ex As Exception
        '        'dataCommand.Connection.Close()
        '        dataCommand.Dispose()
        '        dataIRISConnection.Dispose()
        '        r = ex.Message
        '        Return r
        '    End Try
        'Catch ex As Exception
        '    r = ex.Message
        'End Try
        Return r
    End Function

    Public Function ExequteSQLquery_Cache(ByVal SQLq As String, ByRef myconstring As String) As String
        Dim r As String = "Query executed fine."
        'Try
        '    Dim dataCacheConnection As New InterSystems.Data.CacheClient.CacheConnection
        '    Dim dataCommand As New InterSystems.Data.CacheClient.CacheCommand
        '    Dim ds As New System.Data.DataSet
        '    If myconstring = String.Empty Then
        '        myconstring = InterSystems.Data.CacheClient.CacheConnection.ConnectDlg()
        '    End If
        '    dataCacheConnection = New InterSystems.Data.CacheClient.CacheConnection(myconstring)
        '    Try
        '        If dataCacheConnection.State = ConnectionState.Closed Then dataCacheConnection.Open()
        '        dataCommand.Connection = dataCacheConnection
        '        dataCommand.CommandType = CommandType.Text
        '        dataCommand.CommandTimeout = 300000
        '        dataCommand.CommandText = SQLq
        '        dataCommand.ExecuteNonQuery()
        '        dataCommand.Dispose()
        '        dataCacheConnection.Close()
        '    Catch ex As Exception
        '        'dataCommand.Connection.Close()
        '        dataCommand.Dispose()
        '        dataCacheConnection.Dispose()
        '        r = ex.Message
        '        Return r
        '    End Try
        'Catch ex As Exception
        '    r = ex.Message
        'End Try
        Return r
    End Function

    Public Function ExequteSQLquery_Oracle(ByVal SQLq As String, ByVal myconstring As String) As String
        Dim r As String = "Query executed fine."
        'Try
        '    Dim myConnection As Oracle.ManagedDataAccess.Client.OracleConnection
        '    Dim myCommand As New Oracle.ManagedDataAccess.Client.OracleCommand
        '    myConnection = New Oracle.ManagedDataAccess.Client.OracleConnection(myconstring)
        '    Try
        '        myCommand.Connection = myConnection
        '        myCommand.CommandType = CommandType.Text
        '        myCommand.CommandTimeout = 300000
        '        myCommand.CommandText = SQLq
        '        If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
        '        myCommand.ExecuteNonQuery()
        '        myCommand.Connection.Close()
        '        myCommand.Dispose()
        '        myConnection.Dispose()
        '    Catch ex As Exception
        '        myCommand.Connection.Close()
        '        myCommand.Dispose()
        '        myConnection.Dispose()
        '        r = ex.Message
        '        Return r
        '    End Try
        'Catch ex As Exception
        '    r = ex.Message
        'End Try
        Return r
    End Function
#End Region

#Region "ImportDataTableIntoDb - Oracle and InterSystems"
    Public Function ImportDataTableIntoDb_Oracle(ByVal dt As DataTable, ByVal tablename As String, ByVal userconnstr As String) As String
        Dim ret As String = String.Empty
        'Try
        '    Dim cmdBuilder As Oracle.ManagedDataAccess.Client.OracleCommandBuilder
        '    Dim myConnection As Oracle.ManagedDataAccess.Client.OracleConnection
        '    Dim myCommand As New Oracle.ManagedDataAccess.Client.OracleCommand
        '    Dim myAdapter As Oracle.ManagedDataAccess.Client.OracleDataAdapter
        '    myConnection = New Oracle.ManagedDataAccess.Client.OracleConnection(userconnstr)
        '    myCommand.Connection = myConnection
        '    myCommand.CommandType = CommandType.Text
        '    myCommand.CommandTimeout = 300000
        '    myCommand.CommandText = "Select * FROM " & tablename.ToUpper
        '    If myCommand.Connection.State = ConnectionState.Closed Then myCommand.Connection.Open()
        '    myAdapter = New Oracle.ManagedDataAccess.Client.OracleDataAdapter(myCommand)
        '    cmdBuilder = New Oracle.ManagedDataAccess.Client.OracleCommandBuilder(myAdapter)
        '    Try
        '        myAdapter.Update(dt.DataSet)
        '        myAdapter.Dispose()
        '        myCommand.Connection.Close()
        '        myCommand.Dispose()
        '    Catch ex As Exception
        '        ret = "ERROR!! " & ex.Message
        '    End Try
        'Catch ex As Exception
        '    ret = "ERROR!! " & ex.Message
        'End Try
        Return ret
    End Function

    Public Function ImportDataTableIntoDb_Cache(ByVal dt As DataTable, ByVal tablename As String, ByVal userconnstr As String) As String
        Dim ret As String = String.Empty
        'Try
        '    Dim cacheBuilder As InterSystems.Data.CacheClient.CacheCommandBuilder
        '    Dim dataCacheConnectionString As String = String.Empty
        '    Dim dataCacheConnection As New InterSystems.Data.CacheClient.CacheConnection
        '    Dim dataCommand As New InterSystems.Data.CacheClient.CacheCommand
        '    dataCacheConnection = New InterSystems.Data.CacheClient.CacheConnection(userconnstr)
        '    If dataCacheConnection.State = ConnectionState.Closed Then dataCacheConnection.Open()
        '    dataCommand.Connection = dataCacheConnection
        '    dataCommand.CommandType = CommandType.Text
        '    dataCommand.CommandTimeout = 300000
        '    dataCommand.CommandText = "Select * FROM " & tablename
        '    Dim dataAdapter As New InterSystems.Data.CacheClient.CacheDataAdapter(dataCommand)
        '    cacheBuilder = New InterSystems.Data.CacheClient.CacheCommandBuilder(dataAdapter)
        '    Try
        '        dataAdapter.Update(dt)
        '        dataAdapter.Dispose()
        '        'dataCommand.Connection.Close()
        '        dataCommand.Dispose()
        '    Catch ex As Exception
        '        ret = "ERROR!! " & ex.Message
        '    End Try
        'Catch ex As Exception
        '    ret = "ERROR!! " & ex.Message
        'End Try
        Return ret
    End Function

    Public Function ImportDataTableIntoDb_IRIS(ByVal dt As DataTable, ByVal tablename As String, ByVal userconnstr As String) As String
        Dim ret As String = String.Empty
        'Try
        '    Dim IRISBuilder As InterSystems.Data.IRISClient.IRISCommandBuilder
        '    Dim dataIRISConnection As New InterSystems.Data.IRISClient.IRISConnection
        '    Dim dataCommand As New InterSystems.Data.IRISClient.IRISCommand
        '    dataIRISConnection = New InterSystems.Data.IRISClient.IRISConnection(userconnstr)
        '    If dataIRISConnection.State = ConnectionState.Closed Then dataIRISConnection.Open()
        '    dataCommand.Connection = dataIRISConnection
        '    dataCommand.CommandType = CommandType.Text
        '    dataCommand.CommandTimeout = 300000
        '    dataCommand.CommandText = "Select * FROM " & tablename
        '    Dim dataAdapter As New InterSystems.Data.IRISClient.IRISDataAdapter(dataCommand)
        '    IRISBuilder = New InterSystems.Data.IRISClient.IRISCommandBuilder(dataAdapter)
        '    Try
        '        dataAdapter.Update(dt)
        '        dataAdapter.Dispose()
        '        dataCommand.Connection.Close()
        '        dataCommand.Dispose()
        '    Catch ex As Exception
        '        ret = "ERROR!! " & ex.Message
        '    End Try
        'Catch ex As Exception
        '    ret = "ERROR!! " & ex.Message
        'End Try
        Return ret
    End Function
#End Region

#Region "DatabaseConnected - Oracle and InterSystems"
    Public Function DatabaseConnected_IRIS(ByVal myconstring As String, ByRef er As String) As Boolean
        Dim r As Boolean = False
        'Try
        '    Dim dataCacheConnection As New InterSystems.Data.IRISClient.IRISConnection
        '    Dim dataCommand As New InterSystems.Data.IRISClient.IRISCommand
        '    Dim ds As New System.Data.DataSet
        '    dataCacheConnection = New InterSystems.Data.IRISClient.IRISConnection(myconstring)
        '    Try
        '        If dataCacheConnection.State = ConnectionState.Closed Then dataCacheConnection.Open()
        '        If dataCacheConnection.State = ConnectionState.Open Then r = True
        '        dataCacheConnection.Close()
        '    Catch ex As Exception
        '        er = ex.Message
        '        dataCommand.Dispose()
        '        dataCacheConnection.Dispose()
        '        r = False
        '    End Try
        'Catch ex As Exception
        '    er = ex.Message
        'End Try
        Return r
    End Function

    Public Function DatabaseConnected_Cache(ByRef myconstring As String, ByRef er As String) As Boolean
        Dim r As Boolean = False
        'Try
        '    Dim dataCacheConnection As New InterSystems.Data.CacheClient.CacheConnection
        '    Dim dataCommand As New InterSystems.Data.CacheClient.CacheCommand
        '    Dim ds As New System.Data.DataSet
        '    If myconstring = String.Empty Then
        '        myconstring = InterSystems.Data.CacheClient.CacheConnection.ConnectDlg()
        '    End If
        '    dataCacheConnection = New InterSystems.Data.CacheClient.CacheConnection(myconstring)
        '    Try
        '        If dataCacheConnection.State = ConnectionState.Closed Then dataCacheConnection.Open()
        '        If dataCacheConnection.State = ConnectionState.Open Then r = True
        '        dataCacheConnection.Close()
        '    Catch ex As Exception
        '        er = ex.Message
        '        dataCommand.Dispose()
        '        dataCacheConnection.Dispose()
        '        r = False
        '    End Try
        'Catch ex As Exception
        '    er = ex.Message
        'End Try
        Return r
    End Function

    Public Function DatabaseConnected_Oracle(ByVal myconstring As String, ByRef er As String) As Boolean
        Dim r As Boolean = False
        'Try
        '    Dim myConnection As Oracle.ManagedDataAccess.Client.OracleConnection
        '    myConnection = New Oracle.ManagedDataAccess.Client.OracleConnection(myconstring)
        '    Try
        '        If myConnection.State = ConnectionState.Closed Then myConnection.Open()
        '        If myConnection.State = ConnectionState.Open Then r = True
        '        myConnection.Close()
        '        myConnection.Dispose()
        '    Catch ex As Exception
        '        myConnection.Close()
        '        myConnection.Dispose()
        '        er = ex.Message
        '        r = False
        '    End Try
        'Catch ex As Exception
        '    er = ex.Message
        'End Try
        Return r
    End Function
#End Region

End Module
