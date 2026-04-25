# Enabling InterSystems and Oracle Support in DataModuleProprietary.vb

If you have a valid license for **Oracle Database** or **InterSystems IRIS / Caché**, you can enable native database connectivity by uncommenting the corresponding code blocks in `App_Code/DataModuleProprietary.vb`.

**License requirement:** Oracle Database requires a valid commercial license from Oracle Corporation. InterSystems IRIS and Caché require a valid commercial license from InterSystems Corporation. These are proprietary products and are not provided or distributed with DataAI.

---

## How Uncommenting Works

Every function in `DataModuleProprietary.vb` has its implementation commented out with single-quote (`'`) line comments. The function signatures (the `Public Function ... End Function` lines) are already active — only the internal logic is commented out.

To enable a function, remove the `'` at the beginning of each line inside the function body, between `Dim ret As String = String.Empty` and `Return ret`.

**Before (commented out):**
```vb
Public Function DatabaseConnected_Oracle(ByVal myconstring As String, ByRef er As String) As Boolean
    Dim r As Boolean = False
    'Try
    '    Dim myConnection As Oracle.ManagedDataAccess.Client.OracleConnection
    '    myConnection = New Oracle.ManagedDataAccess.Client.OracleConnection(myconstring)
    '    ...
    'End Try
    Return r
End Function
```

**After (uncommented):**
```vb
Public Function DatabaseConnected_Oracle(ByVal myconstring As String, ByRef er As String) As Boolean
    Dim r As Boolean = False
    Try
        Dim myConnection As Oracle.ManagedDataAccess.Client.OracleConnection
        myConnection = New Oracle.ManagedDataAccess.Client.OracleConnection(myconstring)
        ...
    End Try
    Return r
End Function
```

**Tip:** In Visual Studio, select all commented lines inside a function and press **Ctrl+K, Ctrl+U** to uncomment them all at once.

---

## For Oracle Users

Uncomment the internal code in all functions ending with `_Oracle`:

| # | Function Name | Region | Lines |
|---|--------------|--------|-------|
| 1 | `AddRowIntoTable_Oracle` | AddRowIntoTable | 72–95 |
| 2 | `LoadDataTableIntoDbTable_Oracle` | LoadDataTableIntoDbTable | 170–196 |
| 3 | `HasRecords_Oracle` | HasRecords | 280–299 |
| 4 | `CountOfRecords_Oracle` | CountOfRecords | 378–396 |
| 5 | `mRecords_Oracle` | mRecords | 563–586 |
| 6 | `mRecordsFromSP_Oracle` | mRecordsFromSP | 664–720 |
| 7 | `RunSP_Oracle` | RunSP | 788–816 |
| 8 | `ExequteSQLquery_Oracle` | ExequteSQLquery | 884–908 |
| 9 | `ImportDataTableIntoDb_Oracle` | ImportDataTableIntoDb | 914–938 |
| 10 | `DatabaseConnected_Oracle` | DatabaseConnected | 1052–1069 |

**Also required:**

- Make sure line 2 (`Imports Oracle.ManagedDataAccess.Client`) is present and uncommented — it already is in the default file.
- Place the `Oracle.ManagedDataAccess.dll` NuGet package in your application's `Bin` folder.
- Configure Oracle connection strings in `web.config` (see the WebConfigGuide).
- Set `<add key="Oracle" value="OK" />` in `web.config` `<appSettings>`.

---

## For InterSystems IRIS Users

Uncomment the internal code in all functions ending with `_IRIS`:

| # | Function Name | Region | Lines |
|---|--------------|--------|-------|
| 1 | `AddRowIntoTable_IRIS` | AddRowIntoTable | 40–66 |
| 2 | `LoadDataTableIntoDbTable_IRIS` | LoadDataTableIntoDbTable | 137–165 |
| 3 | `HasRecords_IRIS` | HasRecords | 203–236 |
| 4 | `CountOfRecords_IRIS` | CountOfRecords | 306–337 |
| 5 | `GetGlobalNodeValue_IRIS` | GetGlobalNodeValue | 403–427 |
| 6 | `mRecords_IRIS` | mRecords | 466–507 |
| 7 | `mRecordsFromSP_IRIS` | mRecordsFromSP | 593–622 |
| 8 | `RunSP_IRIS` | RunSP | 727–750 |
| 9 | `ExequteSQLquery_IRIS` | ExequteSQLquery | 823–847 |
| 10 | `ImportDataTableIntoDb_IRIS` | ImportDataTableIntoDb | 973–996 |
| 11 | `DatabaseConnected_IRIS` | DatabaseConnected | 1003–1021 |

**Also required:**

- Add a reference to `InterSystems.Data.IRISClient.dll` in your project and place it in the `Bin` folder.
- Configure IRIS connection strings in `web.config` (see the WebConfigGuide).
- Set `<add key="IRISProv" value="OK" />` in `web.config` `<appSettings>`.

---

## For InterSystems Caché Users

Uncomment the internal code in all functions ending with `_Cache`:

| # | Function Name | Region | Lines |
|---|--------------|--------|-------|
| 1 | `AddRowIntoTable_Cache` | AddRowIntoTable | 8–34 |
| 2 | `LoadDataTableIntoDbTable_Cache` | LoadDataTableIntoDbTable | 102–132 |
| 3 | `HasRecords_Cache` | HasRecords | 241–274 |
| 4 | `CountOfRecords_Cache` | CountOfRecords | 342–373 |
| 5 | `GetGlobalNodeValue_Cache` | GetGlobalNodeValue | 432–459 |
| 6 | `mRecords_Cache` | mRecords | 512–558 |
| 7 | `mRecordsFromSP_Cache` | mRecordsFromSP | 627–659 |
| 8 | `RunSP_Cache` | RunSP | 756–783 |
| 9 | `ExequteSQLquery_Cache` | ExequteSQLquery | 852–879 |
| 10 | `ImportDataTableIntoDb_Cache` | ImportDataTableIntoDb | 944–968 |
| 11 | `DatabaseConnected_Cache` | DatabaseConnected | 1026–1047 |

**Also required:**

- Add a reference to `InterSystems.Data.CacheClient.dll` in your project and place it in the `Bin` folder.
- Configure Caché connection strings in `web.config` (see the WebConfigGuide).
- Set `<add key="CacheProv" value="OK" />` in `web.config` `<appSettings>`.

---

## Summary Checklist

### Oracle
- [ ] Uncomment the body of all 10 `_Oracle` functions in `DataModuleProprietary.vb`
- [ ] Ensure `Imports Oracle.ManagedDataAccess.Client` is present at the top of the file
- [ ] Place `Oracle.ManagedDataAccess.dll` in the `Bin` folder
- [ ] Set Oracle connection strings in `web.config` `<connectionStrings>`
- [ ] Set `<add key="Oracle" value="OK" />` in `web.config` `<appSettings>`
- [ ] Set `<add key="ourdbcase" value="upper" />` in `web.config`

### InterSystems IRIS
- [ ] Uncomment the body of all 11 `_IRIS` functions in `DataModuleProprietary.vb`
- [ ] Place `InterSystems.Data.IRISClient.dll` in the `Bin` folder
- [ ] Set IRIS connection strings in `web.config` `<connectionStrings>`
- [ ] Set `<add key="IRISProv" value="OK" />` in `web.config` `<appSettings>`
- [ ] Set `<add key="ourdbcase" value="upper" />` in `web.config`

### InterSystems Caché
- [ ] Uncomment the body of all 11 `_Cache` functions in `DataModuleProprietary.vb`
- [ ] Place `InterSystems.Data.CacheClient.dll` in the `Bin` folder
- [ ] Set Caché connection strings in `web.config` `<connectionStrings>`
- [ ] Set `<add key="CacheProv" value="OK" />` in `web.config` `<appSettings>`
- [ ] Set `<add key="ourdbcase" value="upper" />` in `web.config`
