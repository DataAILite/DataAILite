Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Math
Imports System.Collections
Imports MySql.Data.MySqlClient
Imports Oracle.ManagedDataAccess.Client
Imports Microsoft.VisualBasic.DateAndTime
Public Module DatabaseUpdate
    Public Function CreateNewOURdbOnNewServer(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "", Optional ByVal dbcase As String = "lower") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Dim pss As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            Dim maindb As String = String.Empty
            If myconprv.StartsWith("InterSystems.Data.Cache") Then
                maindb = "%SYS"
            ElseIf myconprv.StartsWith("InterSystems.Data.IRIS") Then

            ElseIf myconprv = "MySql.Data.MySqlClient" Then
                maindb = "sys"
            ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                'maindb = "XE" 'XEPDB1
                db = GetUserIDFromConnectionString(myconstr)  'db.Substring(db.LastIndexOf("/") + 1)
                pss = GetPasswordFromConnectionString(myconstr)
                pss = Regex.Replace(pss, "[^a-zA-Z0-9]", "")  'CLS-compliant
            ElseIf myconprv = "System.Data.Odbc" Then
                maindb = db
            ElseIf myconprv = "Npgsql" Then
                maindb = "postgres" '"public"
            ElseIf myconprv = "Sqlite" Then 'in memory
                Return ""
            Else  'SQL Server
                maindb = "master"
            End If
            myconstr = myconstr.Replace(db, maindb)
            ret = "Creating " & db & ": <br/> "
            If myconprv.StartsWith("InterSystems.Data.Cache") Then
                'create namespace
                If Not DatabaseExist(db, myconstr, myconprv) Then
                    sqlq = "CREATE DATABASE " & db
                    ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                    If ret = "Query executed fine." Then
                        ret = ret & "<br/> " & " ourdb created"
                        ret = ret & "<br/> " & CreateInitialClass(myconstr, myconprv)
                    Else
                        ret = ret & "<br/> ERROR!!" & " ourdb not created: " & ret
                        'Return ret
                    End If
                Else
                    ret = ret & "<br/> " & " ourdb already exists"
                    ret = ret & "<br/> " & CreateInitialClass(myconstr, myconprv)
                End If
            ElseIf myconprv.StartsWith("InterSystems.Data.IRIS") Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("SystemSQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("SystemSQLconnection").ToString
                If DatabaseExist(db, myconstr, myconprv) Then
                    ret = ret & "<br/> " & " ourdb already exists"
                    Return ret
                End If
                sqlq = "CREATE DATABASE " & db
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = ret & "<br/> " & " ourdb created"
                    ret = ret & "<br/> " & CreateInitialClass(myconstr, myconprv)
                Else
                    ret = ret & "<br/> ERROR!!" & " ourdb not created: " & ret
                    'Return ret
                End If

            ElseIf myconprv = "System.Data.SqlClient" Then
                If Not DatabaseExist(db, myconstr, myconprv) Then
                    sqlq = "CREATE DATABASE " & db
                    ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                    If ret = "Query executed fine." Then
                        ret = ret & "<br/> " & " ourdb created"
                    Else
                        ret = ret & "<br/> ERROR!!" & " ourdb not created: " & ret
                        'Return ret
                    End If
                Else
                    ret = ret & "<br/> " & " ourdb already exists"
                End If

            ElseIf myconprv = "MySql.Data.MySqlClient" Then
                If Not DatabaseExist(db, myconstr, myconprv) Then
                    sqlq = "CREATE DATABASE `" & db.ToLower & "`"
                    ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                    If ret = "Query executed fine." Then
                        ret = ret & "<br/> " & " ourdb created"
                    Else
                        ret = ret & "<br/> ERROR!!" & " ourdb not created: " & ret
                        'Return ret
                    End If
                Else
                    ret = ret & "<br/> " & " ourdb already exists"
                End If

            ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                Try
                    myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("SystemSQLconnection").ProviderName.ToString
                    myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("SystemSQLconnection").ToString
                    Dim retr As String = String.Empty
                    If Not DatabaseExist(db, myconstr, myconprv) Then
                        sqlq = "alter session set ""_ORACLE_SCRIPT""=true"
                        retr = ExequteSQLquery(sqlq, myconstr, myconprv)
                        If retr = "Query executed fine." Then
                            ret = ret & "<br/> " & " alter session "
                        Else
                            ret = ret & "<br/> ERROR!!" & " alter session crashed: " & retr
                            'Exit Try
                        End If
                        sqlq = "CREATE USER " & db & " "
                        sqlq = sqlq & "IDENTIFIED BY " & pss & " DEFAULT TABLESPACE USERS TEMPORARY TABLESPACE TEMP"
                        retr = ExequteSQLquery(sqlq, myconstr, myconprv)
                        If retr = "Query executed fine." Then
                            ret = ret & "<br/> " & " ourdb created"
                        Else
                            ret = ret & "<br/> ERROR!!" & " ourdb not created: " & retr
                            'Exit Try
                        End If
                    End If
                    sqlq = "GRANT CONNECT,RESOURCE,CREATE SESSION,CREATE VIEW,CREATE MATERIALIZED VIEW, ALTER SESSION,CREATE DATABASE LINK,CREATE PROCEDURE,CREATE PUBLIC SYNONYM,CREATE ROLE,CREATE SEQUENCE,CREATE SYNONYM,CREATE TABLE,CREATE TRIGGER,CREATE TYPE,UNLIMITED TABLESPACE TO """ & db & """"
                    retr = ExequteSQLquery(sqlq, myconstr, myconprv)
                    If retr = "Query executed fine." Then
                        ret = ret & "<br/> " & " ourdb permissions granted"
                    Else
                        ret = ret & "<br/> ERROR!!" & " ourdb permissions not granted: " & retr
                        Exit Try
                    End If

                Catch ex As Exception
                    ret = ret & "<br/> ERROR!!" & " ourdb not created: " & ret
                End Try

            ElseIf myconprv = "System.Data.Odbc" Then
                Try
                    Dim er As String = String.Empty
                    Dim userODBCdriver As String = String.Empty
                    Dim userODBCdatabase As String = String.Empty
                    Dim userODBCdatasource As String = String.Empty
                    myconstr = myconstr.Replace("Password", "Pwd").Replace("User ID", "UID")
                    Dim bConnect As Boolean = DatabaseConnected(myconstr, myconprv, er, userODBCdriver, userODBCdatabase, userODBCdatasource)
                    If Not bConnect Then
                        er = "ERROR!! Database not connected. " & er
                        Return er
                    End If
                    Dim sqlf As String = String.Empty

                    'TODO !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Check why? For another button?

                    If userODBCdriver.ToUpper.StartsWith("PSQL") Then
                        sqlf = "CREATE SCHEMA """ & userODBCdatabase & """;"
                    End If
                    ret = ExequteSQLquery(sqlf, myconstr, myconprv)
                    If ret = "Query executed fine." Then
                        ret = ret & "<br/> " & " ourdb created"
                    Else
                        ret = ret & "<br/> ERROR!!" & " ourdb not created: " & ret
                        'Return ret
                    End If
                Catch ex As Exception
                    ret = ret & "<br/> ERROR!!" & " ourdb not created: " & ret
                End Try

            ElseIf myconprv = "Npgsql" Then
                If Not DatabaseExist(db, myconstr, myconprv) Then
                    Try
                        Dim sqlf As String = String.Empty
                        If dbcase = "doublequoted" Then
                            sqlf = "CREATE DATABASE """ & db & """;"
                        ElseIf dbcase = "lower" Then
                            sqlf = "CREATE DATABASE [" & db.ToLower & "];"
                        ElseIf dbcase = "upper" Then
                            sqlf = "CREATE DATABASE [" & db.ToUpper & "];"
                        Else
                            sqlf = "CREATE DATABASE [" & db.ToLower & "];"
                        End If
                        ret = ExequteSQLquery(sqlf, myconstr, myconprv)
                        'tables will be created in public schema in db database
                        'sqlf = "CREATE SCHEMA """ & db & """;"
                        'ret = ExequteSQLquery(sqlf, myconstr, myconprv)
                        If ret = "Query executed fine." Then
                            ret = ret & "<br/> " & " ourdb created"
                        Else
                            ret = ret & "<br/> ERROR!!" & " ourdb not created: " & ret
                            'Return ret
                        End If
                    Catch ex As Exception
                        ret = ret & "<br/> ERROR!!" & " ourdb not created: " & ret
                    End Try
                Else
                    ret = ret & "<br/> " & " ourdb already exists"
                End If

            Else  'SQL Server
                If Not DatabaseExist(db, myconstr, myconprv) Then
                    Try
                        sqlq = "CREATE DATABASE " & db
                        'sqlq = sqlq & " ON PRIMARY (NAME = " & db & "_Data, "
                        'sqlq = sqlq & "FILENAME = ""C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\" & db & ".mdf"", "
                        'sqlq = sqlq & "SIZE = 20MB, MAXSIZE = 100MB, FILEGROWTH = 10%) "
                        'sqlq = sqlq & "LOG ON (NAME = " & db & "_Log, "
                        'sqlq = sqlq & "FILENAME = ""C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\" & db & ".ldf"", "
                        'sqlq = sqlq & "SIZE = 1MB, "
                        'sqlq = sqlq & "MAXSIZE = 5MB, "
                        'sqlq = sqlq & "FILEGROWTH = 10%)"
                        ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                        If ret = "Query executed fine." Then
                            ret = ret & "<br/> " & " ourdb " & db & " created"
                        Else
                            ret = ret & "<br/> ERROR!!" & " ourdb " & db & " not created: " & ret
                            'Return ret
                        End If
                    Catch ex As Exception
                        ret = ret & "<br/> ERROR!!" & " ourdb " & db & " not created: " & ret
                    End Try

                End If
            End If
        Catch ex As Exception
            ret = "<br/> ERROR!!" & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURdbToCurrentVersion(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "", Optional ByVal clon As Boolean = True) As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            ret = "Updating " & db & ": <br/> "

            'call Create
            If Not DatabaseExist(db, myconstr, myconprv) AndAlso myconprv <> "Sqlite" Then
                Dim ourdbcase As String = ConfigurationManager.AppSettings("ourdbcase").ToString.Trim
                CreateNewOURdbOnNewServer(myconstr, myconprv, ourdbcase)
            End If


            ret = ret & "<br/>OURFriendlyNames: " & InstallOURFriendlyNames(myconstr, myconprv)
            ret = ret & "<br/>OURFiles: " & InstallOURFiles(myconstr, myconprv)
            ret = ret & "<br/>OURUnits: " & InstallOURUnits(myconstr, myconprv, clon)
            ret = ret & "<br/>OURHelpDesk: " & InstallOURHelpDesk(myconstr, myconprv)
            ret = ret & "<br/>OURAccessLog: " & InstallOURAccessLog(myconstr, myconprv)
            ret = ret & "<br/>OURPermits: " & InstallOURPermits(myconstr, myconprv)
            ret = ret & "<br/>OURPermissions: " & InstallOURPermissions(myconstr, myconprv)
            ret = ret & "<br/>OURReportInfo: " & InstallOURReportInfo(myconstr, myconprv)
            ret = ret & "<br/>OURReportItems: " & InstallOURReportItems(myconstr, myconprv)
            ret = ret & "<br/>OURReportShow: " & InstallOURReportShow(myconstr, myconprv)
            ret = ret & "<br/>OURReportFormat: " & InstallOURReportFormat(myconstr, myconprv)
            ret = ret & "<br/>OURReportGroups: " & InstallOURReportGroups(myconstr, myconprv)
            ret = ret & "<br/>OURReportLists: " & InstallOURReportLists(myconstr, myconprv)
            ret = ret & "<br/>OURReportSQLquery: " & InstallOURReportSQLquery(myconstr, myconprv)
            ret = ret & "<br/>OURUserTables: " & InstallOURUserTables(myconstr, myconprv)
            ret = ret & "<br/>OURReportView: " & InstallOURReportView(myconstr, myconprv)
            If myconprv <> "Sqlite" Then ret = ret & "<br/>OURAgents: " & InstallOURAgents(myconstr, myconprv)
            ret = ret & "<br/>OURActivity: " & InstallOURActivity(myconstr, myconprv)
            ret = ret & "<br/>OURDashboards: " & InstallOURDashboards(myconstr, myconprv)
            ret = ret & "<br/>ourkmlhistory: " & Installourkmlhistory(myconstr, myconprv)
            ret = ret & "<br/>ourtasklistsetting: " & InstallOURTaskListSetting(myconstr, myconprv)
            If myconprv <> "Sqlite" Then ret = ret & "<br/>ourcomparison: " & InstallOURComparison(myconstr, myconprv)
            ret = ret & "<br/>OURScheduledReports: " & InstallOURScheduledReports(myconstr, myconprv)
            ret = ret & "<br/>OURScheduledDownloads: " & InstallOURScheduledDownloads(myconstr, myconprv)
            ret = ret & "<br/>OURScheduledImports: " & InstallOURScheduledImports(myconstr, myconprv)
        Catch ex As Exception
            ret = "<br/> ERROR!!" & ex.Message
        End Try
        Return ret
    End Function
    Public Function InstallOURComparison(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            If TableExists("OURComparison", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURComparison: " & UpdateOURComparison(myconstr, myconprv)

                'If Not ColumnExists("OURComparison", "recorder", myconstr, myconprv) Then
                '    If myconprv = "MySql.Data.MySqlClient" Then
                '        db = db.ToLower
                '        sqlq = "ALTER TABLE `" & db & "`.`ourcomparison` ADD COLUMN `recorder` tinyint(2) DEFAULT '0';"
                '    ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                '        sqlq = "ALTER TABLE OURComparison ADD recorder NUMBER(2,0) DEFAULT 0"
                '    Else
                '        sqlq = "ALTER TABLE [OURComparison] ADD [recorder] smallint DEFAULT 0"
                '    End If
                '    ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                'End If
            Else

                'CREATE Table  `OUR****`.`ourcomparison` (
                ' `Feature` VARCHAR( 250 ) DEFAULT NULL ,
                ' `Comments` VARCHAR( 240 ) DEFAULT NULL ,
                ' `OUReports` TINYINT(1) DEFAULT 0 ,
                ' `MSPowerBI` TINYINT(1) DEFAULT 0 ,
                ' `GoogleReports` TINYINT(1) DEFAULT 0 ,
                ' `MidasPlus` TINYINT(1) DEFAULT 0 ,
                ' `OracleAnalytics` TINYINT(1) DEFAULT 0 ,
                ' `Indx` INT( 11 ) Not NULL AUTO_INCREMENT ,
                'PRIMARY KEY(  `Indx` )
                ')

                'INSERT INTO `ourdataapp`.`ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Automatic data analysis and statistics', 'Only reading permissions needed', '1', '0', '0', '0', '0', '1');
                'INSERT INTO `ourdataapp`.`ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Automatic report generator', 'SSRS Tabular, Graphics, Matrix, DrillDown, Google Charts and Maps', '1', '0', '0', '0', '0', '4');
                'INSERT INTO `ourdataapp`.`ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Built in statistics', 'Count, Average, StDev, CI, Correlation', '1', '0', '0', '0', '0', '3');
                'INSERT INTO `ourdataapp`.`ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('For developers: easy RDL and KML generator', 'Download generated RDL report definitions, as well as KML for Google Earth and Maps', '1', '0', '0', '0', '0', '13');
                'INSERT INTO `ourdataapp`.`ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Report and data export. Report scheduling and sharing', 'Excel, Word, PDF, CSV, ...', '1', '1', '1', '1', '1', '5');
                'INSERT INTO `ourdataapp`.`ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Multiple databases including InterSystems', 'MS SQL Server, InterSystems Cache and IRIS, MySQL,  Oracle, ODBC, csv,xml,json files', '1', '0', '0', '0', '0', '2');
                'INSERT INTO `ourdataapp`.`ourcomparison` (`Feature`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Secure cloud based solution', '1', '1', '1', '0', '1', '9');
                'INSERT INTO `ourdataapp`.`ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`,`recorder`) VALUES ('Easy to use interface', 'Ad hoc reports and Google Charts and Maps for non-programmers', '1', '1', '1', '1', '0', '8');
                'INSERT INTO `ourdataapp`.`ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`,`recorder`) VALUES ('Advanced interface features', 'Class/Table Explorer for sys admins and developers, upload your own RDL or Crystal RPT.', '1', '0', '0', '0', '0', '14');
                'INSERT INTO `ourdataapp`.`ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`,`recorder`) VALUES ('Free for individual users, free month trial for companies', 'We serve individual users from www.OUReports.com free of charge', '1', '1', '1', '0', '0', '10');
                'INSERT INTO `ourdataapp`.`ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Company subscription', 'Dedicated web site on our server', '1', '1', '1', '1', '1', '11');
                'INSERT INTO `ourdataapp`.`ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Installation on company local server ', 'Software provided', '1', '1', '0', '1', '1', '12');
                'INSERT INTO `ourdataapp`.`ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Generate Google Earth, Google Maps, Map and Geo Charts', 'KML:  pins, cicles, paths, polygons, tours', '1', '0', '0', '1', '0', '6');
                'INSERT INTO `ourdataapp`.`ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Easy inteface to create and share Dashboards', 'Add Google Charts, Map and Geo Charts to dashboards', '1', '0', '1', '0', '1', '7');


                'create table ourcomparison
                If myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "CREATE TABLE `" & db & "`.`ourcomparison` ("
                    sqlq = sqlq & " `Feature` VARCHAR( 250 ) DEFAULT NULL,"
                    sqlq = sqlq & " `Comments` VARCHAR( 240 ) DEFAULT NULL,"
                    sqlq = sqlq & " `OUReports` TINYINT(1) DEFAULT 0,"
                    sqlq = sqlq & " `MSPowerBI` TINYINT(1) DEFAULT 0,"
                    sqlq = sqlq & " `GoogleReports` TINYINT(1) DEFAULT 0,"
                    sqlq = sqlq & " `MidasPlus` TINYINT(1) DEFAULT 0,"
                    sqlq = sqlq & " `OracleAnalytics` TINYINT(1) DEFAULT 0,"
                    sqlq = sqlq & " `recorder` tinyint(2) DEFAULT '0',"
                    sqlq = sqlq & " `Indx` int(11) NOT NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " PRIMARY KEY (`Indx`)"
                    sqlq = sqlq & ");"

                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURCOMPARISON ("
                    sqlq = sqlq & "Feature VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Comments VARCHAR2(240 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "OUReports NUMBER(1,0) DEFAULT 0,"
                    sqlq = sqlq & "MSPowerBI NUMBER(1,0) DEFAULT 0,"
                    sqlq = sqlq & "GoogleReports NUMBER(1,0) DEFAULT 0,"
                    sqlq = sqlq & "MidasPlus NUMBER(1,0) DEFAULT 0,"
                    sqlq = sqlq & "OracleAnalytics NUMBER(1,0) DEFAULT 0,"
                    sqlq = sqlq & "recorder NUMBER(2,0) DEFAULT 0,"
                    sqlq = sqlq & "Indx INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURCOMPARISON_PK PRIMARY KEY (Indx)"
                    sqlq = sqlq & ")"

                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    'sqlq = "CREATE Table [" & db & "].[OURComparison]("
                    sqlq = "CREATE Table [OURComparison]("
                    sqlq = sqlq & "[Feature] character varying(250) NULL,"
                    sqlq = sqlq & "[Comments] character varying(240) NULL,"
                    sqlq = sqlq & "[OUReports] smallint DEFAULT 0,"
                    sqlq = sqlq & "[MSPowerBI] smallint DEFAULT 0,"
                    sqlq = sqlq & "[GoogleReports] smallint DEFAULT 0,"
                    sqlq = sqlq & "[MidasPlus] smallint DEFAULT 0,"
                    sqlq = sqlq & "[OracleAnalytics] smallint DEFAULT 0,"
                    sqlq = sqlq & "[recorder] smallint DEFAULT 0,"
                    sqlq = sqlq & "[Indx] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURComparison_pkey] PRIMARY KEY ([Indx]))"

                Else 'Sql Server or Cache
                    sqlq = "CREATE Table [OURComparison]("
                    sqlq = sqlq & "[Feature] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Comments] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[OUReports] [Int] DEFAULT 0,"
                    sqlq = sqlq & "[MSPowerBI] [Int] DEFAULT 0,"
                    sqlq = sqlq & "[GoogleReports] [Int] DEFAULT 0,"
                    sqlq = sqlq & "[MidasPlus] [Int] DEFAULT 0,"
                    sqlq = sqlq & "[OracleAnalytics] [Int] DEFAULT 0,"
                    sqlq = sqlq & "[recorder] [Int] DEFAULT 0,"
                    sqlq = sqlq & "[Indx] [Int] IDENTITY(1, 1) Not NULL"
                    sqlq = sqlq & ")"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "OURComparison created"
                Else
                    ret = "OURComparison creation crashed: " & ret
                End If
            End If
            'Update column changes or additions
            'If (ret = "Table exists" OrElse ret = "OURComparison created") Then
            sqlq = ""
            Dim retrn As String = ret

            If myconprv = "MySql.Data.MySqlClient" Then
                sqlq = "DELETE FROM `ourcomparison`"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO `ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Automatic data analysis and statistics', 'Only reading permissions needed', '1', '0', '0', '0', '0',1);"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO `ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Automatic report generator', 'SSRS Tabular, Graphics, Matrix, DrillDown, Google Charts and Maps', '1', '0', '0', '0', '0',4);"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO `ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Built in statistics', 'Count, Average, StDev, CI, Correlation', '1', '0', '0', '0', '0',3);"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO `ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('For developers: easy RDL and KML generator', 'Download generated RDL report definitions, as well as KML for Google Earth and Maps', '1', '0', '0', '0', '0',13);"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO `ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Report and data export. Report scheduling and sharing', 'Excel, Word, PDF, CSV, ...', '1', '1', '1', '1', '1',5);"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO `ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Multiple databases including InterSystems', 'MS SQL Server, InterSystems Cache and IRIS, MySQL,  Oracle, ODBC, csv,xml,json files', '1', '0', '0', '0', '0',2);"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO `ourcomparison` (`Feature`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Secure cloud based solution', '1', '1', '1', '0', '1',9);"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO `ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Easy to use interface', 'Ad hoc reports and Google Charts and Maps for non-programmers', '1', '1', '1', '1', '0',8);"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO `ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Advanced interface features', 'Class/Table Explorer for sys admins and developers, upload your own RDL or Crystal RPT.', '1', '0', '0', '0', '0',14);"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO `ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Free for individual users, free month trial for companies', 'We serve individual users from www.OUReports.com free of charge', '1', '1', '1', '0', '0',10);"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO `ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Company subscription', 'Dedicated web site on our server', '1', '1', '1', '1', '1',11);"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO `ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Installation on company local server ', 'Software provided', '1', '1', '0', '1', '1',11);"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO `ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Generate Google Earth, Google Maps, Map and Geo Charts', 'KML:  pins, cicles, paths, polygons, tours', '1', '0', '0', '1', '0',6);"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO `ourcomparison` (`Feature`, `Comments`, `OUReports`, `MSPowerBI`, `GoogleReports`, `MidasPlus`, `OracleAnalytics`, `recorder`) VALUES ('Easy inteface to create and share Dashboards', 'Add Google Charts, Map and Geo Charts to dashboards', '1', '0', '1', '0', '1',7);"
            ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                sqlq = "DELETE FROM ourcomparison"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO ourcomparison (Feature, Comments, OUReports, MSPowerBI, GoogleReports, MidasPlus,OracleAnalytics, recorder) VALUES ('Automatic data analysis and statistics', 'Only reading permissions needed', '1', '0', '0', '0', '0',1)"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO ourcomparison (Feature, Comments, OUReports, MSPowerBI, GoogleReports, MidasPlus, OracleAnalytics, recorder) VALUES ('Automatic report generator', 'SSRS Tabular, Graphics, Matrix, DrillDown, Google Charts and Maps', '1', '0', '0', '0', '0',4)"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO ourcomparison (Feature, Comments, OUReports, MSPowerBI, GoogleReports, MidasPlus, OracleAnalytics, recorder) VALUES ('Built in statistics', 'Count, Average, StDev, CI, Correlation', '1', '0', '0', '0', '0',3)"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO ourcomparison (Feature, Comments, OUReports, MSPowerBI, GoogleReports, MidasPlus, OracleAnalytics, recorder) VALUES ('For developers: easy RDL and KML generator', 'Download generated RDL report definitions, as well as KML for Google Earth and Maps', '1', '0', '0', '0', '0',13)"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO ourcomparison (Feature, Comments, OUReports, MSPowerBI, GoogleReports, MidasPlus, OracleAnalytics, recorder) VALUES ('Report and data export. Report scheduling and sharing', 'Excel, Word, PDF, CSV, ...', '1', '1', '1', '1', '1',5)"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO ourcomparison (Feature, Comments, OUReports, MSPowerBI, GoogleReports, MidasPlus, OracleAnalytics, recorder) VALUES ('Multiple databases including InterSystems', 'MS SQL Server, InterSystems Cache and IRIS, MySQL,  Oracle, ODBC, csv,xml,json files', '1', '0', '0', '0', '0',2)"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO ourcomparison (Feature, OUReports, MSPowerBI, GoogleReports, MidasPlus, OracleAnalytics, recorder) VALUES ('Secure cloud based solution', '1', '1', '1', '0', '1',9)"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO ourcomparison (Feature, Comments, OUReports, MSPowerBI, GoogleReports, MidasPlus, OracleAnalytics, recorder) VALUES ('Easy to use interface', 'Ad hoc reports and Google Charts and Maps for non-programmers', '1', '1', '1', '1', '0',8)"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO ourcomparison (Feature, Comments, OUReports, MSPowerBI, GoogleReports, MidasPlus, OracleAnalytics, recorder) VALUES ('Advanced interface features', 'Class/Table Explorer for sys admins and developers, upload your own RDL or Crystal RPT.', '1', '0', '0', '0', '0',14)"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO ourcomparison (Feature, Comments, OUReports, MSPowerBI, GoogleReports, MidasPlus, OracleAnalytics, recorder) VALUES ('Free for individual users, free month trial for companies', 'We serve individual users from www.OUReports.com free of charge', '1', '1', '1', '0', '0',10)"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO ourcomparison (Feature, Comments, OUReports, MSPowerBI, GoogleReports, MidasPlus, OracleAnalytics, recorder) VALUES ('Company subscription', 'Dedicated web site on our server', '1', '1', '1', '1', '1',11)"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO ourcomparison (Feature, Comments, OUReports, MSPowerBI, GoogleReports, MidasPlus, OracleAnalytics, recorder) VALUES ('Installation on company local server ', 'Software provided', '1', '1', '0', '1', '1',11)"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO ourcomparison (Feature, Comments, OUReports, MSPowerBI, GoogleReports, MidasPlus, OracleAnalytics, recorder) VALUES ('Generate Google Earth, Google Maps, Map and Geo Charts', 'KML:  pins, cicles, paths, polygons, tours', '1', '0', '0', '1', '0',6)"
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                sqlq = "INSERT INTO ourcomparison (Feature, Comments, OUReports, MSPowerBI, GoogleReports, MidasPlus, OracleAnalytics, recorder) VALUES ('Easy inteface to create and share Dashboards', 'Add Google Charts, Map and Geo Charts to dashboards', '1', '0', '1', '0', '1',7)"
            End If

            If sqlq <> String.Empty Then
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then ret = retrn
            End If

            'End If
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function InstallOURScheduledReports(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            If TableExists("OURScheduledReports", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURScheduledReports: " & UpdateOURScheduledReports(myconstr, myconprv)

            Else

                'CREATE Table  `OURtesting`.`ourscheduledreports` (
                ' `Start` VARCHAR( 50 ) DEFAULT NULL ,
                ' `UserId` VARCHAR( 240 ) DEFAULT NULL ,
                ' `ReportId` VARCHAR( 240 ) DEFAULT NULL ,
                ' `Name` VARCHAR( 2500 ) DEFAULT NULL ,
                ' `WhereText` VARCHAR( 2500 ) DEFAULT NULL ,
                ' `Deadline` VARCHAR( 50 ) DEFAULT NULL ,
                ' `Filters` VARCHAR( 2500 ) DEFAULT NULL ,
                ' `ToWhom` VARCHAR( 2500 ) DEFAULT NULL ,
                ' `ID` INT( 11 ) Not NULL AUTO_INCREMENT ,
                ' `Prop1` VARCHAR( 1200 ) DEFAULT NULL ,
                ' `Prop2` VARCHAR( 1200 ) DEFAULT NULL ,
                ' `Prop3` VARCHAR( 1200 ) DEFAULT NULL ,
                ' `Status` VARCHAR( 50 ) DEFAULT NULL ,
                'PRIMARY KEY(  `ID` )
                ')
                'create table ourscheduledreports
                Dim db As String = GetDataBase(myconstr, myconprv)
                If myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "CREATE TABLE `" & db & "`.`ourscheduledreports` ("
                    sqlq = sqlq & " `Start` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `UserId` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `ReportId` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `Name` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `WhereText` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Deadline` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `Filters` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `ToWhom` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Status` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop1` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop2` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop3` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop4` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop5` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop6` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `ID` int(11) NOT NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " PRIMARY KEY (`ID`)"
                    sqlq = sqlq & ");"

                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURSCHEDULEDREPORTS ("
                    sqlq = sqlq & """Start"" VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ReportId VARCHAR2(240 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "UserId VARCHAR2(240 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Name VARCHAR2(2500 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "WhereText VARCHAR2(2500 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Deadline VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Filters VARCHAR2(2500 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ToWhom VARCHAR2(2500 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Status VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop1 VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop2 VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop3 VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop4 VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop5 VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop6 VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ID INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURSCHEDULEDREPORTS_PK PRIMARY KEY (ID)"
                    sqlq = sqlq & ")"

                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURScheduledReports]("
                    sqlq = sqlq & "[Start] character varying(50) NULL,"
                    sqlq = sqlq & "[ReportId] character varying(240) NULL,"
                    sqlq = sqlq & "[UserId] character varying(240) NULL,"
                    sqlq = sqlq & "[Name] character varying(2500) NULL,"
                    sqlq = sqlq & "[WhereText] character varying(2500) NULL,"
                    sqlq = sqlq & "[Deadline] character varying(50) NULL,"
                    sqlq = sqlq & "[Filters] character varying(2500) NULL,"
                    sqlq = sqlq & "[ToWhom] character varying(2500) NULL,"
                    sqlq = sqlq & "[Status] character varying(50) NULL,"
                    sqlq = sqlq & "[Prop1] character varying(1200) NULL,"
                    sqlq = sqlq & "[Prop2] character varying(1200) NULL,"
                    sqlq = sqlq & "[Prop3] character varying(1200) NULL,"
                    sqlq = sqlq & "[Prop4] character varying(1200) NULL,"
                    sqlq = sqlq & "[Prop5] character varying(1200) NULL,"
                    sqlq = sqlq & "[Prop6] character varying(1200) NULL,"
                    sqlq = sqlq & "[ID] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURScheduledReports_pkey] PRIMARY KEY ([ID]))"

                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE Table [OURScheduledReports]("
                    sqlq = sqlq & "[Start] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[ReportId] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[UserId] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[Name] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[WhereText] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[Deadline] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Filters] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[ToWhom] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[Status] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Prop1] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop3] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop4] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop5] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop6] [nvarchar](1200) NULL,"
                    sqlq = sqlq & " [ID] INTEGER PRIMARY KEY AUTOINCREMENT)"

                Else 'Sql Server or Cache
                    sqlq = "CREATE Table [OURScheduledReports]("
                    sqlq = sqlq & "[Start] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[ReportId] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[UserId] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[Name] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[WhereText] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[Deadline] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Filters] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[ToWhom] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[Status] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Prop1] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop3] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop4] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop5] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop6] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[ID] [Int] IDENTITY(1, 1) Not NULL"
                    sqlq = sqlq & ")"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "OURScheduledReports created"
                Else
                    ret = "OURScheduledReports creation crashed: " & ret
                End If
                If myconprv = "MySql.Data.MySqlClient" Then  'if needed
                    sqlq = "ALTER Table `" & db & "`.`ourscheduledreports` ADD COLUMN `Prop4` VARCHAR(120) NULL DEFAULT NULL AFTER `Prop3`, ADD COLUMN `Prop5` VARCHAR(120) NULL DEFAULT NULL AFTER `Prop4`, ADD COLUMN `Prop6` VARCHAR(120) NULL DEFAULT NULL AFTER `Prop5`;"
                    ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                End If
                If ret = "Query executed fine." Then
                    ret = "OURScheduledReports created"
                Else
                    ret = "OURScheduledReports creation: " & ret
                End If
            End If



        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    'Public Function InstallOURScheduledDownloads(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
    '    Dim ret As String = String.Empty
    '    Dim err As String = String.Empty
    '    Dim sqlq As String = String.Empty
    '    Try
    '        If myconstr = "" Then
    '            myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
    '            myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
    '        End If
    '        If TableExists("OURScheduledDownloads", myconstr, myconprv, err) Then
    '            ret = "Table exists"
    '            'Update table structure
    '            ret = ret & "<br/>Update OURScheduledDownloads: " & UpdateOURScheduledDownloads(myconstr, myconprv)

    '        Else

    '            'create table ourscheduleddownloads
    '            Dim db As String = GetDataBase(myconstr, myconprv)
    '            If myconprv = "MySql.Data.MySqlClient" Then
    '                db = db.ToLower
    '                sqlq = "CREATE TABLE `" & db & "`.`ourscheduleddownloads` ("
    '                sqlq = sqlq & " `StartDate` varchar(50) DEFAULT NULL,"
    '                sqlq = sqlq & " `UserId` varchar(240) DEFAULT NULL,"
    '                sqlq = sqlq & " `URL` varchar(2500) DEFAULT NULL,"
    '                sqlq = sqlq & " `Deadline` varchar(50) DEFAULT NULL,"
    '                sqlq = sqlq & " `ToWhom` varchar(2500) DEFAULT NULL,"
    '                sqlq = sqlq & " `Status` varchar(50) DEFAULT NULL,"
    '                sqlq = sqlq & " `Prop1` varchar(1200) DEFAULT NULL,"
    '                sqlq = sqlq & " `Prop2` varchar(1200) DEFAULT NULL,"
    '                sqlq = sqlq & " `Prop3` varchar(1200) DEFAULT NULL,"
    '                sqlq = sqlq & " `ID` int(11) NOT NULL AUTO_INCREMENT,"
    '                sqlq = sqlq & " PRIMARY KEY (`ID`)"
    '                sqlq = sqlq & ");"

    '            ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
    '                sqlq = "CREATE TABLE OURSCHEDULEDDOWNLOADS ("
    '                sqlq = sqlq & "StartDate VARCHAR2(50 Char) Default NULL,"
    '                sqlq = sqlq & "UserId VARCHAR2(240 Char) Default NULL,"
    '                sqlq = sqlq & "URL VARCHAR2(2500 Char) Default NULL,"
    '                sqlq = sqlq & "Deadline VARCHAR2(50 Char) Default NULL,"
    '                sqlq = sqlq & "ToWhom VARCHAR2(2500 Char) Default NULL,"
    '                sqlq = sqlq & "Status VARCHAR2(50 Char) Default NULL,"
    '                sqlq = sqlq & "Prop1 VARCHAR2(1200 Char) Default NULL,"
    '                sqlq = sqlq & "Prop2 VARCHAR2(1200 Char) Default NULL,"
    '                sqlq = sqlq & "Prop3 VARCHAR2(1200 Char) Default NULL,"
    '                sqlq = sqlq & "ID Integer GENERATED ALWAYS As IDENTITY,"
    '                sqlq = sqlq & "CONSTRAINT OURREPORTLISTS_PK PRIMARY KEY (ID)"
    '                sqlq = sqlq & ")"

    '            ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
    '                sqlq = "CREATE Table [OURScheduledDownloads]("
    '                sqlq = sqlq & "[StartDate] character varying(50) NULL,"
    '                sqlq = sqlq & "[UserId] character varying(240) NULL,"
    '                sqlq = sqlq & "[URL] character varying(2500) NULL,"
    '                sqlq = sqlq & "[Deadline] character varying(50) NULL,"
    '                sqlq = sqlq & "[ToWhom] character varying(2500) NULL,"
    '                sqlq = sqlq & "[Status] character varying(50) NULL,"
    '                sqlq = sqlq & "[Prop1] character varying(1200) NULL,"
    '                sqlq = sqlq & "[Prop2] character varying(1200) NULL,"
    '                sqlq = sqlq & "[Prop3] character varying(1200) NULL,"
    '                sqlq = sqlq & "[ID] Integer Not NULL GENERATED ALWAYS As IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
    '                sqlq = sqlq & "CONSTRAINT [OURScheduledReports_pkey] PRIMARY KEY ([ID]))"

    '            Else 'Sql Server or Cache
    '                sqlq = "CREATE Table [OURScheduledDownloads]("
    '                sqlq = sqlq & "[StartDate] [nvarchar](50) NULL,"
    '                sqlq = sqlq & "[UserId] [nvarchar](240) NULL,"
    '                sqlq = sqlq & "[URL] [nvarchar](2500) NULL,"
    '                sqlq = sqlq & "[Deadline] [nvarchar](50) NULL,"
    '                sqlq = sqlq & "[ToWhom] [nvarchar](2500) NULL,"
    '                sqlq = sqlq & "[Status] [nvarchar](50) NULL,"
    '                sqlq = sqlq & "[Prop1] [nvarchar](1200) NULL,"
    '                sqlq = sqlq & "[Prop2] [nvarchar](1200) NULL,"
    '                sqlq = sqlq & "[Prop3] [nvarchar](1200) NULL,"
    '                sqlq = sqlq & "[ID] [Int] IDENTITY(1, 1) Not NULL"
    '                sqlq = sqlq & ")"
    '            End If
    '            ret = ExequteSQLquery(sqlq, myconstr, myconprv)
    '            If ret = "Query executed fine." Then
    '                ret = "OURScheduledDownloads created"
    '            Else
    '                ret = "OURScheduledDownloads creation crashed: " & ret
    '            End If
    '        End If

    '    Catch ex As Exception
    '        ret = "<br/> " & ex.Message
    '    End Try
    '    Return ret
    'End Function
    Public Function InstallOURReportLists(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            db = db.ToLower
            If TableExists("OURReportLists", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURReportLists: " & UpdateOURReportLists(myconstr, myconprv)

            Else
                'create table OURReportLists
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "CREATE TABLE `" & db & "`.`ourreportlists` ("
                    sqlq = sqlq & " `ReportId` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `ListFld` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `RecFld` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `comments` varchar(400) DEFAULT NULL,"
                    sqlq = sqlq & " `Indx` int(11) NOT NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " PRIMARY KEY (`Indx`)"
                    sqlq = sqlq & ");"

                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURREPORTLISTS ("
                    sqlq = sqlq & "ReportId VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ListFld VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "RecFld VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "comments VARCHAR2(400 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Indx INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURREPORTLISTS_PK PRIMARY KEY (Indx)"
                    sqlq = sqlq & ")"

                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURReportLists]("
                    sqlq = sqlq & "[ReportId] character varying(50) NULL,"
                    sqlq = sqlq & "[ListFld] character varying(50) NULL,"
                    sqlq = sqlq & "[RecFld] character varying(50) NULL,"
                    sqlq = sqlq & "[comments] character varying(400) NULL,"
                    sqlq = sqlq & "[Indx] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURReportLists_pkey] PRIMARY KEY ([Indx]))"

                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE Table [OURReportLists]("
                    sqlq = sqlq & "[ReportId] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[ListFld] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[RecFld] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[comments] [nvarchar](400) NULL,"
                    sqlq = sqlq & " [Indx] INTEGER PRIMARY KEY AUTOINCREMENT)"

                Else 'Sql Server or Cache
                    sqlq = "CREATE Table [OURReportLists]("
                    sqlq = sqlq & "[ReportId] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[ListFld] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[RecFld] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[comments] [nvarchar](400) NULL,"
                    sqlq = sqlq & "[Indx] [Int] IDENTITY(1, 1) Not NULL"
                    sqlq = sqlq & ")"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "OURReportLists created"
                Else
                    ret = "OURReportLists creation crashed: " & ret
                End If
            End If
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function InstallOURReportView(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty

        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            If TableExists("OURReportView", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURReportView: " & UpdateOURReportView(myconstr, myconprv)

            Else
                If myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "CREATE TABLE `" & db & "`.`ourreportview` ("
                    sqlq &= "`ReportID` varchar(50) NOT NULL,"
                    sqlq &= "`ReportTemplate` varchar(45) DEFAULT NULL,"
                    sqlq &= "`ReportTitle` varchar(250) DEFAULT NULL,"
                    sqlq &= "`Orientation` varchar(45) DEFAULT 'portrait',"
                    sqlq &= "`ReportFieldLayout` varchar(10) DEFAULT 'Block',"
                    sqlq &= "`HeaderHeight` varchar(15) DEFAULT '1',"
                    sqlq &= "`HeaderBackColor` varchar(45) DEFAULT 'white',"
                    sqlq &= "`HeaderBorderColor` varchar(45) DEFAULT 'lightgrey',"
                    sqlq &= "`HeaderBorderStyle` varchar(15) DEFAULT 'Solid',"
                    sqlq &= "`HeaderBorderWidth` varchar(5) DEFAULT '1',"

                    sqlq &= "`FooterHeight` varchar(15) DEFAULT '1',"
                    sqlq &= "`FooterBackColor` varchar(45) DEFAULT 'white',"
                    sqlq &= "`FooterBorderColor` varchar(45) DEFAULT 'lightgrey',"
                    sqlq &= "`FooterBorderStyle` varchar(15) DEFAULT 'Solid',"
                    sqlq &= "`FooterBorderWidth` varchar(5) DEFAULT '1',"

                    sqlq &= "`DataFontName` varchar(100) DEFAULT 'Tahoma',"
                    sqlq &= "`DataFontSize` varchar(15) DEFAULT '12px',"
                    sqlq &= "`DataForeColor` varchar(45) DEFAULT 'black',"
                    sqlq &= "`DataBackColor` varchar(45) DEFAULT 'white',"
                    sqlq &= "`DataBorderColor` varchar(45) DEFAULT 'lightgrey',"
                    sqlq &= "`DataBorderStyle` varchar(15) DEFAULT 'Solid',"
                    sqlq &= "`DataBorderWidth` varchar(5) DEFAULT '1',"
                    sqlq &= "`DataFontStyle` varchar(15) DEFAULT 'regular',"
                    sqlq &= "`DataUnderline` tinyint(1) DEFAULT '0',"
                    sqlq &= "`DataStrikeout` tinyint(1) DEFAULT '0',"
                    sqlq &= "`ReportDetailAlign` varchar(10) DEFAULT 'Left',"
                    sqlq &= "`LabelFontName` varchar(100) DEFAULT 'Tahoma',"
                    sqlq &= "`LabelFontSize` varchar(15) DEFAULT '12px',"
                    sqlq &= "`LabelForeColor` varchar(45) DEFAULT 'black',"
                    sqlq &= "`LabelBackColor` varchar(45) DEFAULT 'white',"
                    sqlq &= "`LabelBorderColor` varchar(45) DEFAULT 'lightgrey',"
                    sqlq &= "`LabelBorderStyle` varchar(15) DEFAULT 'Solid',"
                    sqlq &= "`LabelBorderWidth` varchar(5) DEFAULT '1',"
                    sqlq &= "`LabelFontStyle` varchar(15) DEFAULT 'regular',"
                    sqlq &= "`LabelUnderline` tinyint(1) DEFAULT '0',"
                    sqlq &= "`LabelStrikeout` tinyint(1) DEFAULT '0',"
                    sqlq &= "`ReportCaptionAlign` varchar(10) DEFAULT 'Left',"
                    sqlq &= "`MarginBottom` decimal(10,2) DEFAULT '0.50',"
                    sqlq &= "`MarginLeft` decimal(10,2) DEFAULT '0.25',"
                    sqlq &= "`MarginRight` decimal(10,2) DEFAULT '0.25',"
                    sqlq &= "`MarginTop` decimal(10,2) DEFAULT '0.50',"
                    sqlq &= "`CreatedBy` varchar(45) DEFAULT NULL,"
                    sqlq &= "`DateCreated` datetime DEFAULT NULL,"
                    sqlq &= "`UpdatedBy` varchar(45) DEFAULT NULL,"
                    sqlq &= "`LastUpdate` datetime DEFAULT NULL,"
                    sqlq &= "`idx` int(11) NOT NULL AUTO_INCREMENT,"
                    sqlq &= "PRIMARY KEY (`idx`));"
                    'sqlq &= "UNIQUE KEY `idx_UNIQUE` (`idx`));"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURREPORTVIEW ("
                    sqlq &= "ReportID VARCHAR2(50 CHAR) NOT NULL,"
                    sqlq &= "ReportTemplate VARCHAR2(45 CHAR) DEFAULT NULL,"
                    sqlq &= "ReportTitle VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq &= "Orientation VARCHAR2(45 CHAR) DEFAULT 'portrait',"
                    sqlq &= "ReportFieldLayout VARCHAR2(10 CHAR) DEFAULT 'Block',"
                    sqlq &= "HeaderHeight VARCHAR2(15 CHAR) DEFAULT '1',"
                    sqlq &= "HeaderBackColor VARCHAR2(45 CHAR) DEFAULT 'white',"
                    sqlq &= "HeaderBorderColor VARCHAR2(45 CHAR) DEFAULT 'lightgrey',"
                    sqlq &= "HeaderBorderStyle VARCHAR2(15 CHAR) DEFAULT 'Solid',"
                    sqlq &= "HeaderBorderWidth VARCHAR2(5 CHAR) DEFAULT '1',"

                    sqlq &= "FooterHeight VARCHAR2(15 CHAR) DEFAULT '1',"
                    sqlq &= "FooterBackColor VARCHAR2(45 CHAR) DEFAULT 'white',"
                    sqlq &= "FooterBorderColor VARCHAR2(45 CHAR) DEFAULT 'lightgrey',"
                    sqlq &= "FooterBorderStyle VARCHAR2(15 CHAR) DEFAULT 'Solid',"
                    sqlq &= "FooterBorderWidth VARCHAR2(5 CHAR) DEFAULT '1',"

                    sqlq &= "DataFontName VARCHAR2(100 CHAR) DEFAULT 'Tahoma',"
                    sqlq &= "DataFontSize VARCHAR2(15 CHAR) DEFAULT '12px',"
                    sqlq &= "DataForeColor VARCHAR2(45 CHAR) DEFAULT 'black',"
                    sqlq &= "DataBackColor VARCHAR2(45 CHAR) DEFAULT 'white',"
                    sqlq &= "DataBorderColor VARCHAR2(45 CHAR) DEFAULT 'lightgrey',"
                    sqlq &= "DataBorderStyle VARCHAR2(15 CHAR) DEFAULT 'Solid',"
                    sqlq &= "DataBorderWidth VARCHAR2(5 CHAR) DEFAULT '1',"
                    sqlq &= "DataFontStyle VARCHAR2(15 CHAR) DEFAULT 'regular',"
                    sqlq &= "DataUnderline NUMBER(1,0) DEFAULT 0 NOT NULL,"
                    sqlq &= "DataStrikeout NUMBER(1,0) DEFAULT 0 NOT NULL,"
                    sqlq &= "ReportDetailAlign VARCHAR2(10 CHAR) DEFAULT 'Left',"
                    sqlq &= "LabelFontName VARCHAR2(100 CHAR) DEFAULT 'Tahoma',"
                    sqlq &= "LabelFontSize VARCHAR2(15 CHAR) DEFAULT '12px',"
                    sqlq &= "LabelForeColor VARCHAR2(45 CHAR) DEFAULT 'black',"
                    sqlq &= "LabelBackColor VARCHAR2(45 CHAR) DEFAULT 'white',"
                    sqlq &= "LabelBorderColor VARCHAR2(45 CHAR) DEFAULT 'lightgrey',"
                    sqlq &= "LabelBorderStyle VARCHAR2(15 CHAR) DEFAULT 'Solid',"
                    sqlq &= "LabelBorderWidth VARCHAR2(5 CHAR) DEFAULT '1',"
                    sqlq &= "LabelFontStyle VARCHAR2(15 CHAR) DEFAULT 'regular',"
                    sqlq &= "LabelUnderline NUMBER(1,0) DEFAULT 0 NOT NULL,"
                    sqlq &= "LabelStrikeout NUMBER(1,0) DEFAULT 0 NOT NULL,"
                    sqlq &= "ReportCaptionAlign VARCHAR2(10 CHAR) DEFAULT 'Left',"
                    sqlq &= "MarginBottom NUMBER(10,2) DEFAULT '0.50' NOT NULL,"
                    sqlq &= "MarginLeft NUMBER(10,2) DEFAULT '0.25' NOT NULL,"
                    sqlq &= "MarginRight NUMBER(10,2) DEFAULT '0.25' NOT NULL,"
                    sqlq &= "MarginTop NUMBER(10,2) DEFAULT '0.50' NOT NULL,"
                    sqlq &= "CreatedBy VARCHAR2(45 CHAR) DEFAULT NULL,"
                    sqlq &= "DateCreated DATE DEFAULT NULL,"
                    sqlq &= "UpdatedBy VARCHAR2(45 CHAR) DEFAULT NULL,"
                    sqlq &= "LastUpdate DATE DEFAULT NULL,"
                    sqlq &= "idx INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq &= "CONSTRAINT OURREPORTVIEW_PK PRIMARY KEY (idx)"
                    sqlq &= ")"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURReportView]("
                    sqlq &= "[ReportID] character varying(50) NOT NULL,"
                    sqlq &= "[ReportTemplate] character varying(45) NULL,"
                    sqlq &= "[ReportTitle] character varying(250) NULL,"
                    sqlq &= "[Orientation] character varying(45) NOT NULL DEFAULT 'portrait',"
                    sqlq &= "[ReportFieldLayout] character varying(10) NOT NULL DEFAULT 'Block',"
                    sqlq &= "[DataFontName] character varying(100) NOT NULL DEFAULT 'Tahoma',"
                    sqlq &= "[DataFontSize] character varying(15) NOT NULL DEFAULT '12px',"
                    sqlq &= "[DataForeColor] character varying(45) NOT NULL DEFAULT 'black',"
                    sqlq &= "[DataBackColor] character varying(45) NOT NULL DEFAULT 'white',"
                    sqlq &= "[DataBorderColor] character varying(45) NOT NULL DEFAULT 'lightgrey',"
                    sqlq &= "[DataBorderStyle] character varying(15) NOT NULL DEFAULT 'Solid',"
                    sqlq &= "[DataBorderWidth] character varying(5) NOT NULL DEFAULT '1',"
                    sqlq &= "[DataFontStyle] character varying(15) NOT NULL DEFAULT 'regular',"
                    sqlq &= "[DataUnderline] smallint NOT NULL DEFAULT 0,"
                    sqlq &= "[DataStrikeout] smallint NOT NULL DEFAULT 0,"
                    sqlq &= "[ReportDetailAlign] character varying(10) NOT NULL DEFAULT 'Left',"
                    sqlq &= "[LabelFontName] character varying(100) NOT NULL DEFAULT 'Tahoma',"
                    sqlq &= "[LabelFontSize] character varying(15) NOT NULL DEFAULT '12px',"
                    sqlq &= "[LabelForeColor] character varying(45) NOT NULL DEFAULT 'black',"
                    sqlq &= "[LabelBackColor] character varying(45) NOT NULL DEFAULT 'white',"
                    sqlq &= "[LabelBorderColor] character varying(45) NOT NULL DEFAULT 'lightgrey',"
                    sqlq &= "[LabelBorderStyle] character varying(15) NOT NULL DEFAULT 'Solid',"
                    sqlq &= "[LabelBorderWidth] character varying(5) NOT NULL DEFAULT '1',"
                    sqlq &= "[LabelFontStyle] character varying(15) NOT NULL DEFAULT 'regular',"
                    sqlq &= "[LabelUnderline] smallint NOT NULL DEFAULT 0,"
                    sqlq &= "[LabelStrikeout] smallint NOT NULL DEFAULT 0,"
                    sqlq &= "[ReportCaptionAlign] character varying(10) NOT NULL DEFAULT 'Left',"
                    sqlq &= "[MarginBottom] decimal(10,2) NOT NULL DEFAULT '0.50',"
                    sqlq &= "[MarginLeft] decimal(10,2) NOT NULL DEFAULT '0.25',"
                    sqlq &= "[MarginRight] decimal(10,2) NOT NULL DEFAULT '0.25',"
                    sqlq &= "[MarginTop] decimal(10,2) NOT NULL DEFAULT '0.50',"
                    sqlq &= "CreatedBy character varying(45) NULL,"
                    sqlq &= "[DateCreated] date NULL,"
                    sqlq &= "[UpdatedBy] character varying(45) NULL,"
                    sqlq &= "[LastUpdate] date NULL,"
                    sqlq &= "[idx] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq &= "CONSTRAINT [OURReportView_pkey] PRIMARY KEY ([idx]))"

                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE Table [OURReportView]("
                    sqlq &= "[ReportID] [nvarchar](50) NOT NULL,"
                    sqlq &= "[ReportTemplate] [nvarchar](45) DEFAULT NULL,"
                    sqlq &= "[ReportTitle] [nvarchar](250) DEFAULT NULL,"
                    sqlq &= "[Orientation] [nvarchar](45) NOT NULL DEFAULT 'portrait',"
                    sqlq &= "[ReportFieldLayout] [nvarchar](10) NOT NULL DEFAULT 'Block',"

                    sqlq &= "[HeaderHeight] [nvarchar](15) NOT NULL DEFAULT '1',"
                    sqlq &= "[HeaderBackColor] [nvarchar](45) NOT NULL DEFAULT 'white',"
                    sqlq &= "[HeaderBorderColor] [nvarchar](45) NOT NULL DEFAULT 'lightgrey',"
                    sqlq &= "[HeaderBorderStyle] [nvarchar](15) NOT NULL DEFAULT 'Solid',"
                    sqlq &= "[HeaderBorderWidth] [nvarchar](5) NOT NULL DEFAULT '1',"

                    sqlq &= "[FooterHeight] [nvarchar](15) NOT NULL DEFAULT '1',"
                    sqlq &= "[FooterBackColor] [nvarchar](45) NOT NULL DEFAULT 'white',"
                    sqlq &= "[FooterBorderColor] [nvarchar](45) NOT NULL DEFAULT 'lightgrey',"
                    sqlq &= "[FooterBorderStyle] [nvarchar](15) NOT NULL DEFAULT 'Solid',"
                    sqlq &= "[FooterBorderWidth] [nvarchar](5) NOT NULL DEFAULT '1',"

                    sqlq &= "[DataFontName] [nvarchar](100) NOT NULL DEFAULT 'Tahoma',"
                    sqlq &= "[DataFontSize] [nvarchar](15) NOT NULL DEFAULT '12px',"
                    sqlq &= "[DataForeColor] [nvarchar](45) NOT NULL DEFAULT 'black',"
                    sqlq &= "[DataBackColor] [nvarchar](45) NOT NULL DEFAULT 'white',"
                    sqlq &= "[DataBorderColor] [nvarchar](45) NOT NULL DEFAULT 'lightgrey',"
                    sqlq &= "[DataBorderStyle] [nvarchar](15) NOT NULL DEFAULT 'Solid',"
                    sqlq &= "[DataBorderWidth] [nvarchar](5) NOT NULL DEFAULT '1',"
                    sqlq &= "[DataFontStyle] [nvarchar](15) NOT NULL DEFAULT 'regular',"
                    sqlq &= "[DataUnderline] [int] NOT NULL DEFAULT '0',"
                    sqlq &= "[DataStrikeout] [int] NOT NULL DEFAULT '0',"
                    sqlq &= "[ReportDetailAlign] [nvarchar](10) NOT NULL DEFAULT 'Left',"
                    sqlq &= "[LabelFontName] [nvarchar](100) NOT NULL DEFAULT 'Tahoma',"
                    sqlq &= "[LabelFontSize] [nvarchar](15) NOT NULL DEFAULT '12px',"
                    sqlq &= "[LabelForeColor] [nvarchar](45) NOT NULL DEFAULT 'black',"
                    sqlq &= "[LabelBackColor] [nvarchar](45) NOT NULL DEFAULT 'white',"
                    sqlq &= "[LabelBorderColor] [nvarchar](45) NOT NULL DEFAULT 'lightgrey',"
                    sqlq &= "[LabelBorderStyle] [nvarchar](15) NOT NULL DEFAULT 'Solid',"
                    sqlq &= "[LabelBorderWidth] [nvarchar](5) NOT NULL DEFAULT '1',"
                    sqlq &= "[LabelFontStyle] [nvarchar](15) NOT NULL DEFAULT 'regular',"
                    sqlq &= "[LabelUnderline] [int] NOT NULL DEFAULT '0',"
                    sqlq &= "[LabelStrikeout] [int] NOT NULL DEFAULT '0',"
                    sqlq &= "[ReportCaptionAlign] [nvarchar](10) NOT NULL DEFAULT 'Left',"
                    sqlq &= "[MarginBottom] [decimal](10,2) NOT NULL DEFAULT '0.50',"
                    sqlq &= "[MarginLeft] [decimal](10,2) NOT NULL DEFAULT '0.25',"
                    sqlq &= "[MarginRight] [decimal](10,2) NOT NULL DEFAULT '0.25',"
                    sqlq &= "[MarginTop] [decimal](10,2) NOT NULL DEFAULT '0.50',"
                    sqlq &= "CreatedBy [nvarchar](45) NULL,"
                    sqlq &= "[DateCreated] [smalldatetime] NULL,"
                    sqlq &= "[UpdatedBy] [nvarchar](45) NULL,"
                    sqlq &= "[LastUpdate] [smalldatetime] NULL,"
                    sqlq = sqlq & " [idx] INTEGER PRIMARY KEY AUTOINCREMENT)"

                Else 'Sql Server or Cache
                    sqlq = "CREATE Table [OURReportView]("
                    sqlq &= "[ReportID] [nvarchar](50) NOT NULL,"
                    sqlq &= "[ReportTemplate] [nvarchar](45) DEFAULT NULL,"
                    sqlq &= "[ReportTitle] [nvarchar](250) DEFAULT NULL,"
                    sqlq &= "[Orientation] [nvarchar](45) NOT NULL DEFAULT 'portrait',"
                    sqlq &= "[ReportFieldLayout] [nvarchar](10) NOT NULL DEFAULT 'Block',"

                    sqlq &= "[HeaderHeight] [nvarchar](15) NOT NULL DEFAULT '1',"
                    sqlq &= "[HeaderBackColor] [nvarchar](45) NOT NULL DEFAULT 'white',"
                    sqlq &= "[HeaderBorderColor] [nvarchar](45) NOT NULL DEFAULT 'lightgrey',"
                    sqlq &= "[HeaderBorderStyle] [nvarchar](15) NOT NULL DEFAULT 'Solid',"
                    sqlq &= "[HeaderBorderWidth] [nvarchar](5) NOT NULL DEFAULT '1',"

                    sqlq &= "[FooterHeight] [nvarchar](15) NOT NULL DEFAULT '1',"
                    sqlq &= "[FooterBackColor] [nvarchar](45) NOT NULL DEFAULT 'white',"
                    sqlq &= "[FooterBorderColor] [nvarchar](45) NOT NULL DEFAULT 'lightgrey',"
                    sqlq &= "[FooterBorderStyle] [nvarchar](15) NOT NULL DEFAULT 'Solid',"
                    sqlq &= "[FooterBorderWidth] [nvarchar](5) NOT NULL DEFAULT '1',"

                    sqlq &= "[DataFontName] [nvarchar](100) NOT NULL DEFAULT 'Tahoma',"
                    sqlq &= "[DataFontSize] [nvarchar](15) NOT NULL DEFAULT '12px',"
                    sqlq &= "[DataForeColor] [nvarchar](45) NOT NULL DEFAULT 'black',"
                    sqlq &= "[DataBackColor] [nvarchar](45) NOT NULL DEFAULT 'white',"
                    sqlq &= "[DataBorderColor] [nvarchar](45) NOT NULL DEFAULT 'lightgrey',"
                    sqlq &= "[DataBorderStyle] [nvarchar](15) NOT NULL DEFAULT 'Solid',"
                    sqlq &= "[DataBorderWidth] [nvarchar](5) NOT NULL DEFAULT '1',"
                    sqlq &= "[DataFontStyle] [nvarchar](15) NOT NULL DEFAULT 'regular',"
                    sqlq &= "[DataUnderline] [int] NOT NULL DEFAULT '0',"
                    sqlq &= "[DataStrikeout] [int] NOT NULL DEFAULT '0',"
                    sqlq &= "[ReportDetailAlign] [nvarchar](10) NOT NULL DEFAULT 'Left',"
                    sqlq &= "[LabelFontName] [nvarchar](100) NOT NULL DEFAULT 'Tahoma',"
                    sqlq &= "[LabelFontSize] [nvarchar](15) NOT NULL DEFAULT '12px',"
                    sqlq &= "[LabelForeColor] [nvarchar](45) NOT NULL DEFAULT 'black',"
                    sqlq &= "[LabelBackColor] [nvarchar](45) NOT NULL DEFAULT 'white',"
                    sqlq &= "[LabelBorderColor] [nvarchar](45) NOT NULL DEFAULT 'lightgrey',"
                    sqlq &= "[LabelBorderStyle] [nvarchar](15) NOT NULL DEFAULT 'Solid',"
                    sqlq &= "[LabelBorderWidth] [nvarchar](5) NOT NULL DEFAULT '1',"
                    sqlq &= "[LabelFontStyle] [nvarchar](15) NOT NULL DEFAULT 'regular',"
                    sqlq &= "[LabelUnderline] [int] NOT NULL DEFAULT '0',"
                    sqlq &= "[LabelStrikeout] [int] NOT NULL DEFAULT '0',"
                    sqlq &= "[ReportCaptionAlign] [nvarchar](10) NOT NULL DEFAULT 'Left',"
                    sqlq &= "[MarginBottom] [decimal](10,2) NOT NULL DEFAULT '0.50',"
                    sqlq &= "[MarginLeft] [decimal](10,2) NOT NULL DEFAULT '0.25',"
                    sqlq &= "[MarginRight] [decimal](10,2) NOT NULL DEFAULT '0.25',"
                    sqlq &= "[MarginTop] [decimal](10,2) NOT NULL DEFAULT '0.50',"
                    sqlq &= "CreatedBy [nvarchar](45) NULL,"
                    sqlq &= "[DateCreated] [smalldatetime] NULL,"
                    sqlq &= "[UpdatedBy] [nvarchar](45) NULL,"
                    sqlq &= "[LastUpdate] [smalldatetime] NULL,"
                    sqlq &= "[idx] [Int] IDENTITY(1, 1) NOT NULL"
                    sqlq &= ")"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "OURReportView created"
                Else
                    ret = "OURReportView creation crashed: " & ret
                End If

            End If
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function InstallOURReportItems(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty

        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            If TableExists("OURReportItems", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURReportItems: " & UpdateOURReportItems(myconstr, myconprv)

            Else
                If myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "CREATE TABLE `" & db & "`.`ourreportitems` ("
                    sqlq &= "`ReportID` varchar(50) NOT NULL,"
                    sqlq &= "`ItemID` varchar(50) DEFAULT NULL,"
                    sqlq &= "`TabularColumnWidth` varchar(15) DEFAULT NULL,"
                    sqlq &= "`Caption` varchar(50) DEFAULT NULL,"
                    sqlq &= "`CaptionHeight` varchar(15) DEFAULT NULL,"
                    sqlq &= "`CaptionWidth` varchar(15) DEFAULT NULL,"
                    sqlq &= "`CaptionX` int(4) DEFAULT NULL,"
                    sqlq &= "`CaptionY` int(4) DEFAULT NULL,"
                    sqlq &= "`CaptionFontName` varchar(100) DEFAULT NULL,"
                    sqlq &= "`CaptionFontSize` varchar(15) DEFAULT NULL,"
                    sqlq &= "`CaptionFontStyle` varchar(15) DEFAULT 'Regular',"
                    sqlq &= "`CaptionUnderline` tinyint(1) NOT NULL DEFAULT 0,"
                    sqlq &= "`CaptionStrikeout` tinyint(1) NOT NULL DEFAULT 0,"
                    sqlq &= "`CaptionTextAlign` varchar(10) DEFAULT 'Left',"
                    sqlq &= "`CaptionForeColor` varchar(45) DEFAULT 'black',"
                    sqlq &= "`CaptionBackColor` varchar(45) DEFAULT 'white',"
                    sqlq &= "`CaptionBorderColor` varchar(45) DEFAULT 'lightgrey',"
                    sqlq &= "`CaptionBorderStyle` varchar(15) DEFAULT 'Solid',"
                    sqlq &= "`CaptionBorderWidth` varchar(5) DEFAULT '1',"
                    sqlq &= "`ReportItemType` varchar(45) DEFAULT 'DataField',"
                    sqlq &= "`ImagePath` varchar(255) DEFAULT NULL,"
                    sqlq &= "`ImageHeight` varchar(5) DEFAULT NULL,"
                    sqlq &= "`ImageWidth` varchar(5) DEFAULT NULL,"
                    sqlq &= "`FieldLayout` varchar(15) DEFAULT 'Block',"
                    sqlq &= "`SQLDatabase` varchar(250) DEFAULT NULL,"
                    sqlq &= "`SQLTable` varchar(250) DEFAULT NULL,"
                    sqlq &= "`SQLField` varchar(250) DEFAULT NULL,"
                    sqlq &= "`SQLDataType` varchar(15) DEFAULT NULL,"
                    sqlq &= "`ItemOrder` int(4) NOT NULL DEFAULT '0',"
                    sqlq &= "`DataHeight` varchar(15) DEFAULT NULL,"
                    sqlq &= "`DataWidth` varchar(15) DEFAULT NULL,"
                    sqlq &= "`DataX` int(4) DEFAULT NULL,"
                    sqlq &= "`DataY` int(4) DEFAULT NULL,"
                    sqlq &= "`Height` varchar(15) DEFAULT NULL,"
                    sqlq &= "`Width` varchar(15) DEFAULT NULL,"
                    sqlq &= "`X` int(4) DEFAULT NULL,"
                    sqlq &= "`Y` int(4) DEFAULT NULL,"
                    sqlq &= "`FontName` varchar(100) DEFAULT NULL,"
                    sqlq &= "`FontSize` varchar(15) DEFAULT NULL,"
                    sqlq &= "`FontStyle` varchar(15) DEFAULT 'Regular',"
                    sqlq &= "`Underline` tinyint(1) NOT NULL DEFAULT 0,"
                    sqlq &= "`Strikeout` tinyint(1) NOT NULL DEFAULT 0, "
                    sqlq &= "`TextAlign` varchar(10) DEFAULT 'Left',"
                    sqlq &= "`ForeColor` varchar(45) DEFAULT 'black',"
                    sqlq &= "`BackColor` varchar(45) DEFAULT 'white', "
                    sqlq &= "`BorderColor` varchar(45) DEFAULT 'lightgrey',"
                    sqlq &= "`BorderStyle` varchar(15) DEFAULT 'Solid',"
                    sqlq &= "`BorderWidth` varchar(5) DEFAULT '1',"
                    sqlq &= "`Section` varchar(20) DEFAULT 'Details',"
                    sqlq &= "`idx` int(11) NOT NULL AUTO_INCREMENT,"
                    sqlq &= "PRIMARY KEY (`idx`));"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURREPORTITEMS ("
                    sqlq &= "ReportID VARCHAR2(50 CHAR) NOT NULL,"
                    sqlq &= "ItemID VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq &= "TabularColumnWidth VARCHAR2(15 CHAR) DEFAULT NULL,"
                    sqlq &= "Caption VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq &= "CaptionHeight VARCHAR2(15 CHAR) DEFAULT NULL,"
                    sqlq &= "CaptionWidth VARCHAR2(15 CHAR) DEFAULT NULL,"
                    sqlq &= "CaptionX NUMBER(4,0) DEFAULT NULL,"
                    sqlq &= "CaptionY NUMBER(4,0) DEFAULT NULL,"
                    sqlq &= "CaptionFontName VARCHAR2(100 CHAR) DEFAULT NULL,"
                    sqlq &= "CaptionFontSize VARCHAR2(15 CHAR) DEFAULT NULL,"
                    sqlq &= "CaptionFontStyle VARCHAR2(15 CHAR) DEFAULT 'Regular',"
                    sqlq &= "CaptionUnderline NUMBER(1,0) DEFAULT 0 NOT NULL,"
                    sqlq &= "CaptionStrikeout NUMBER(1,0) DEFAULT 0 NOT NULL,"
                    sqlq &= "CaptionTextAlign VARCHAR2(10 CHAR) DEFAULT 'Left',"
                    sqlq &= "CaptionForeColor VARCHAR2(45 CHAR) DEFAULT 'black',"
                    sqlq &= "CaptionBackColor VARCHAR2(45 CHAR) DEFAULT 'white',"
                    sqlq &= "CaptionBorderColor VARCHAR2(45 CHAR) DEFAULT 'lightgrey',"
                    sqlq &= "CaptionBorderStyle VARCHAR2(15 CHAR) DEFAULT 'Solid',"
                    sqlq &= "CaptionBorderWidth VARCHAR2(5 CHAR) DEFAULT '1',"
                    sqlq &= "ReportItemType VARCHAR2(45 CHAR) DEFAULT 'DataField',"
                    sqlq &= "ImagePath VARCHAR2(255 CHAR) DEFAULT NULL,"
                    sqlq &= "ImageHeight VARCHAR2(5 CHAR) DEFAULT NULL,"
                    sqlq &= "ImageWidth VARCHAR2(5 CHAR) DEFAULT NULL,"
                    sqlq &= "FieldLayout VARCHAR2(15 CHAR) DEFAULT 'Block',"
                    sqlq &= "SQLDatabase VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq &= "SQLTable VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq &= "SQLField VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq &= "SQLDataType VARCHAR2(15 CHAR) DEFAULT NULL,"
                    sqlq &= "ItemOrder NUMBER(4,0) DEFAULT 0 NOT NULL,"
                    sqlq &= "DataHeight VARCHAR2(15 CHAR) DEFAULT NULL,"
                    sqlq &= "DataWidth VARCHAR2(15 CHAR) DEFAULT NULL,"
                    sqlq &= "DataX NUMBER(4,0) DEFAULT NULL,"
                    sqlq &= "DataY NUMBER(4,0) DEFAULT NULL,"
                    sqlq &= "Height VARCHAR2(15 CHAR) DEFAULT NULL,"
                    sqlq &= "Width VARCHAR2(15 CHAR) DEFAULT NULL,"
                    sqlq &= "X NUMBER(4,0) DEFAULT NULL,"
                    sqlq &= "Y NUMBER(4,0) DEFAULT NULL,"
                    sqlq &= "FontName VARCHAR2(100 CHAR) DEFAULT NULL,"
                    sqlq &= "FontSize VARCHAR2(15 CHAR) DEFAULT NULL,"
                    sqlq &= "FontStyle VARCHAR2(15 CHAR) DEFAULT 'Regular',"
                    sqlq &= "Underline NUMBER(1,0) DEFAULT 0 NOT NULL,"
                    sqlq &= "Strikeout NUMBER(1,0) DEFAULT 0 NOT NULL,"
                    sqlq &= "TextAlign VARCHAR2(10 CHAR) DEFAULT 'Left',"
                    sqlq &= "ForeColor VARCHAR2(45 CHAR) DEFAULT 'black',"
                    sqlq &= "BackColor VARCHAR2(45 CHAR) DEFAULT 'white',"
                    sqlq &= "BorderColor VARCHAR2(45 CHAR) DEFAULT 'lightgrey',"
                    sqlq &= "BorderStyle VARCHAR2(15 CHAR) DEFAULT 'Solid',"
                    sqlq &= "BorderWidth VARCHAR2(5 CHAR) DEFAULT '1',"
                    sqlq &= "Section VARCHAR2(20 CHAR) DEFAULT 'Details',"
                    sqlq &= "idx INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq &= "CONSTRAINT OURREPORTITEMS_PK PRIMARY KEY (idx)"
                    sqlq &= ")"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURReportItems]("
                    sqlq &= "[ReportID] character varying(50) NOT NULL,"
                    sqlq &= "[ItemID] character varying(50) NULL,"
                    sqlq &= "[TabularColumnWidth] character varying(15) NULL,"
                    sqlq &= "[Caption] character varying(50) NULL,"
                    sqlq &= "[CaptionHeight] character varying(15) NULL,"
                    sqlq &= "[CaptionWidth] character varying(15) NULL,"
                    sqlq &= "[CaptionX] smallint NULL,"
                    sqlq &= "[CaptionY] smallint NULL,"
                    sqlq &= "[CaptionFontName] character varying(100) NULL,"
                    sqlq &= "[CaptionFontSize] character varying(15) NULL,"
                    sqlq &= "[CaptionFontStyle] character varying(15) NOT NULL DEFAULT 'Regular',"
                    sqlq &= "[CaptionUnderline] smallint NOT NULL DEFAULT 0,"
                    sqlq &= "[CaptionStrikeout] smallint NOT NULL DEFAULT 0,"
                    sqlq &= "[CaptionTextAlign] character varying(10) NOT NULL DEFAULT 'Left',"
                    sqlq &= "[CaptionForeColor] character varying(45) NOT NULL DEFAULT 'black',"
                    sqlq &= "[CaptionBackColor] character varying(45) NOT NULL DEFAULT 'white',"
                    sqlq &= "[CaptionBorderColor] character varying(45) NOT NULL DEFAULT 'lightgrey',"
                    sqlq &= "[CaptionBorderStyle] character varying(15) NOT NULL DEFAULT 'Solid',"
                    sqlq &= "[CaptionBorderWidth] character varying(5) NOT NULL DEFAULT '1',"
                    sqlq &= "[ReportItemType] character varying(45) NOT NULL DEFAULT 'DataField',"
                    sqlq &= "[ImagePath] character varying(255) NULL,"
                    sqlq &= "[ImageHeight] character varying(5) NULL,"
                    sqlq &= "[ImageWidth] character varying(5) NULL,"
                    sqlq &= "[FieldLayout] character varying(15) NOT NULL DEFAULT 'Block',"
                    sqlq &= "[SQLDatabase] character varying(250) NULL,"
                    sqlq &= "[SQLTable] character varying(250) NULL,"
                    sqlq &= "[SQLField] character varying(250) NULL,"
                    sqlq &= "[SQLDataType] character varying(15) NULL,"
                    sqlq &= "[ItemOrder] smallint NOT NULL DEFAULT 0,"
                    sqlq &= "[DataHeight] character varying(15) NULL,"
                    sqlq &= "[DataWidth] character varying(15) NULL,"
                    sqlq &= "[DataX] smallint NULL,"
                    sqlq &= "[DataY] smallint NULL,"
                    sqlq &= "[Height] character varying(15) NULL,"
                    sqlq &= "[Width] character varying(15) NULL,"
                    sqlq &= "[X] smallint NULL,"
                    sqlq &= "[Y] smallint NULL,"
                    sqlq &= "[FontName] character varying(100) NULL,"
                    sqlq &= "[FontSize] character varying(15) NULL,"
                    sqlq &= "[FontStyle] character varying(15) NOT NULL DEFAULT 'Regular',"
                    sqlq &= "[Underline] smallint NOT NULL DEFAULT 0,"
                    sqlq &= "[Strikeout] smallint NOT NULL DEFAULT 0,"
                    sqlq &= "[TextAlign] character varying(10) NOT NULL DEFAULT 'Left',"
                    sqlq &= "[ForeColor] character varying(45) NOT NULL DEFAULT 'black',"
                    sqlq &= "[BackColor] character varying(45) NOT NULL DEFAULT 'white',"
                    sqlq &= "[BorderColor] character varying(45) NOT NULL DEFAULT 'lightgrey',"
                    sqlq &= "[BorderStyle] character varying(15) NOT NULL DEFAULT 'Solid',"
                    sqlq &= "[BorderWidth] character varying(5) NOT NULL DEFAULT '1',"
                    sqlq &= "[Section] character varying(20) NOT NULL DEFAULT 'Details',"
                    sqlq &= "[idx] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq &= "CONSTRAINT [OURReportItems_pkey] PRIMARY KEY ([idx]))"
                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE Table [OURReportItems]("
                    sqlq &= "[ReportID] [nvarchar](50) NOT NULL,"
                    sqlq &= "[ItemID] [nvarchar](50) NULL,"
                    sqlq &= "[TabularColumnWidth] [nvarchar](15) NULL,"
                    sqlq &= "[Caption] [nvarchar](50) NULL,"
                    sqlq &= "[CaptionHeight] [nvarchar](15) NULL,"
                    sqlq &= "[CaptionWidth] [nvarchar](15) NULL,"
                    sqlq &= "[CaptionX] [int] NULL,"
                    sqlq &= "[CaptionY] [int] NULL,"
                    sqlq &= "[CaptionFontName] [nvarchar](100) NULL,"
                    sqlq &= "[CaptionFontSize] [nvarchar](15) NULL,"
                    sqlq &= "[CaptionFontStyle] [nvarchar](15) NOT NULL DEFAULT 'Regular',"
                    sqlq &= "[CaptionUnderline] [int] NOT NULL DEFAULT '0',"
                    sqlq &= "[CaptionStrikeout] [int] NOT NULL DEFAULT '0',"
                    sqlq &= "[CaptionTextAlign] [nvarchar](10) NOT NULL DEFAULT 'Left',"
                    sqlq &= "[CaptionForeColor] [nvarchar](45) NOT NULL DEFAULT 'black',"
                    sqlq &= "[CaptionBackColor] [nvarchar](45) NOT NULL DEFAULT 'white',"
                    sqlq &= "[CaptionBorderColor] [nvarchar](45) NOT NULL DEFAULT 'lightgrey',"
                    sqlq &= "[CaptionBorderStyle] [nvarchar](15) NOT NULL DEFAULT 'Solid',"
                    sqlq &= "[CaptionBorderWidth] [nvarchar](5) NOT NULL DEFAULT '1',"
                    sqlq &= "[ReportItemType] [nvarchar](45) NOT NULL DEFAULT 'DataField',"
                    sqlq &= "[ImagePath] [nvarchar](255) NULL,"
                    sqlq &= "[ImageHeight] [nvarchar](5) NULL,"
                    sqlq &= "[ImageWidth] [nvarchar](5) NULL,"
                    sqlq &= "[FieldLayout] [nvarchar](15) NOT NULL DEFAULT 'Block',"
                    sqlq &= "[SQLDatabase] [nvarchar](250) NULL,"
                    sqlq &= "[SQLTable] [nvarchar](250) NULL,"
                    sqlq &= "[SQLField] [nvarchar](250) NULL,"
                    sqlq &= "[SQLDataType] [nvarchar](15) NULL,"
                    sqlq &= "[ItemOrder] [int] NOT NULL DEFAULT '0',"
                    sqlq &= "[DataHeight] [nvarchar](15) NULL,"
                    sqlq &= "[DataWidth] [nvarchar](15) NULL,"
                    sqlq &= "[DataX] [int] NULL,"
                    sqlq &= "[DataY] [int] NULL,"
                    sqlq &= "[Height] [nvarchar](15) NULL,"
                    sqlq &= "[Width] [nvarchar](15) NULL,"
                    sqlq &= "[X] [int] NULL,"
                    sqlq &= "[Y] [int] NULL,"
                    sqlq &= "[FontName] [nvarchar](100) NULL,"
                    sqlq &= "[FontSize] [nvarchar](15) NULL,"
                    sqlq &= "[FontStyle] [nvarchar](15) NOT NULL DEFAULT 'Regular',"
                    sqlq &= "[Underline] [int] NOT NULL DEFAULT '0',"
                    sqlq &= "[Strikeout] [int] NOT NULL DEFAULT '0',"
                    sqlq &= "[TextAlign] [nvarchar](10) NOT NULL DEFAULT 'Left',"
                    sqlq &= "[ForeColor] [nvarchar](45) NOT NULL DEFAULT 'black',"
                    sqlq &= "[BackColor] [nvarchar](45) NOT NULL DEFAULT 'white',"
                    sqlq &= "[BorderColor] [nvarchar](45) NOT NULL DEFAULT 'lightgrey',"
                    sqlq &= "[BorderStyle] [nvarchar](15) NOT NULL DEFAULT 'Solid',"
                    sqlq &= "[BorderWidth] [nvarchar](5) NOT NULL DEFAULT '1',"
                    sqlq &= FixReservedWords("Section", myconprv) & " [nvarchar](20) Not NULL Default 'Details',"
                    sqlq = sqlq & " [idx] INTEGER PRIMARY KEY AUTOINCREMENT)"
                Else 'Sql Server or Cache
                    sqlq = "CREATE Table [OURReportItems]("
                    sqlq &= "[ReportID] [nvarchar](50) NOT NULL,"
                    sqlq &= "[ItemID] [nvarchar](50) NULL,"
                    sqlq &= "[TabularColumnWidth] [nvarchar](15) NULL,"
                    sqlq &= "[Caption] [nvarchar](50) NULL,"
                    sqlq &= "[CaptionHeight] [nvarchar](15) NULL,"
                    sqlq &= "[CaptionWidth] [nvarchar](15) NULL,"
                    sqlq &= "[CaptionX] [int] NULL,"
                    sqlq &= "[CaptionY] [int] NULL,"
                    sqlq &= "[CaptionFontName] [nvarchar](100) NULL,"
                    sqlq &= "[CaptionFontSize] [nvarchar](15) NULL,"
                    sqlq &= "[CaptionFontStyle] [nvarchar](15) NOT NULL DEFAULT 'Regular',"
                    sqlq &= "[CaptionUnderline] [int] NOT NULL DEFAULT '0',"
                    sqlq &= "[CaptionStrikeout] [int] NOT NULL DEFAULT '0',"
                    sqlq &= "[CaptionTextAlign] [nvarchar](10) NOT NULL DEFAULT 'Left',"
                    sqlq &= "[CaptionForeColor] [nvarchar](45) NOT NULL DEFAULT 'black',"
                    sqlq &= "[CaptionBackColor] [nvarchar](45) NOT NULL DEFAULT 'white',"
                    sqlq &= "[CaptionBorderColor] [nvarchar](45) NOT NULL DEFAULT 'lightgrey',"
                    sqlq &= "[CaptionBorderStyle] [nvarchar](15) NOT NULL DEFAULT 'Solid',"
                    sqlq &= "[CaptionBorderWidth] [nvarchar](5) NOT NULL DEFAULT '1',"
                    sqlq &= "[ReportItemType] [nvarchar](45) NOT NULL DEFAULT 'DataField',"
                    sqlq &= "[ImagePath] [nvarchar](255) NULL,"
                    sqlq &= "[ImageHeight] [nvarchar](5) NULL,"
                    sqlq &= "[ImageWidth] [nvarchar](5) NULL,"
                    sqlq &= "[FieldLayout] [nvarchar](15) NOT NULL DEFAULT 'Block',"
                    sqlq &= "[SQLDatabase] [nvarchar](250) NULL,"
                    sqlq &= "[SQLTable] [nvarchar](250) NULL,"
                    sqlq &= "[SQLField] [nvarchar](250) NULL,"
                    sqlq &= "[SQLDataType] [nvarchar](15) NULL,"
                    sqlq &= "[ItemOrder] [int] NOT NULL DEFAULT '0',"
                    sqlq &= "[DataHeight] [nvarchar](15) NULL,"
                    sqlq &= "[DataWidth] [nvarchar](15) NULL,"
                    sqlq &= "[DataX] [int] NULL,"
                    sqlq &= "[DataY] [int] NULL,"
                    sqlq &= "[Height] [nvarchar](15) NULL,"
                    sqlq &= "[Width] [nvarchar](15) NULL,"
                    sqlq &= "[X] [int] NULL,"
                    sqlq &= "[Y] [int] NULL,"
                    sqlq &= "[FontName] [nvarchar](100) NULL,"
                    sqlq &= "[FontSize] [nvarchar](15) NULL,"
                    sqlq &= "[FontStyle] [nvarchar](15) NOT NULL DEFAULT 'Regular',"
                    sqlq &= "[Underline] [int] NOT NULL DEFAULT '0',"
                    sqlq &= "[Strikeout] [int] NOT NULL DEFAULT '0',"
                    sqlq &= "[TextAlign] [nvarchar](10) NOT NULL DEFAULT 'Left',"
                    sqlq &= "[ForeColor] [nvarchar](45) NOT NULL DEFAULT 'black',"
                    sqlq &= "[BackColor] [nvarchar](45) NOT NULL DEFAULT 'white',"
                    sqlq &= "[BorderColor] [nvarchar](45) NOT NULL DEFAULT 'lightgrey',"
                    sqlq &= "[BorderStyle] [nvarchar](15) NOT NULL DEFAULT 'Solid',"
                    sqlq &= "[BorderWidth] [nvarchar](5) NOT NULL DEFAULT '1',"
                    sqlq &= FixReservedWords("Section", myconprv) & " [nvarchar](20) Not NULL Default 'Details',"
                    sqlq &= "[idx] [Int] IDENTITY(1, 1) NOT NULL"
                    sqlq &= ")"

                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "OURReportItems created"
                Else
                    ret = "OURReportItems creation crashed: " & ret
                End If
            End If


        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function InstallOURReportSQLquery(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            If TableExists("OURReportSQLquery", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURReportSQLquery: " & UpdateOURReportSQLquery(myconstr, myconprv)

            Else
                'create table OURReportSQLquery
                If myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "CREATE TABLE `" & db & "`.`ourreportsqlquery` ("
                    sqlq = sqlq & " `ReportId` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `Doing` char(10) DEFAULT NULL,"
                    sqlq = sqlq & " `Tbl1` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `Tbl1Fld1` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `Oper` char(20) DEFAULT NULL,"
                    sqlq = sqlq & " `Tbl2` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `Tbl2Fld2` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `Tbl3` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `Tbl3Fld3` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `Type` char(10) DEFAULT NULL,"
                    sqlq = sqlq & " `comments` varchar(1240) DEFAULT NULL,"
                    sqlq = sqlq & " `Friendly` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `RecOrder` int(4) DEFAULT NULL,"
                    sqlq = sqlq & " `Group` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `Logical` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `Param1` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `Param2` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `Param3` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `Indx` int(11) NOT NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " PRIMARY KEY (`Indx`)"
                    sqlq = sqlq & ");"

                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURREPORTSQLQUERY ("
                    sqlq = sqlq & "ReportId VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Doing CHAR(10) DEFAULT NULL,"
                    sqlq = sqlq & "Tbl1 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Tbl1Fld1 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Oper CHAR(20) DEFAULT NULL,"
                    sqlq = sqlq & "Tbl2 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Tbl2Fld2 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Tbl3 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Tbl3Fld3 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Type CHAR(10) DEFAULT NULL,"
                    sqlq = sqlq & "comments VARCHAR2(2240 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Friendly VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "RecOrder NUMBER(4,0) DEFAULT NULL,"
                    sqlq = sqlq & """Group"" VARCHAR2(220 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Logical VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param1 VARCHAR2(220 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param2 VARCHAR2(220 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param3 VARCHAR2(220 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Indx INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURREPORTSQLQUERY_PK PRIMARY KEY (Indx)"
                    sqlq = sqlq & ")"

                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURReportSQLquery]("
                    sqlq = sqlq & "[ReportId] character varying(50) NULL,"
                    sqlq = sqlq & "[Doing] character varying(10) NULL,"
                    sqlq = sqlq & "[Tbl1] character varying(250) NULL,"
                    sqlq = sqlq & "[Tbl1Fld1] character varying(250) NULL,"
                    sqlq = sqlq & "[Oper] character varying(20) NULL,"
                    sqlq = sqlq & "[Tbl2] character varying(250) NULL,"
                    sqlq = sqlq & "[Tbl2Fld2] character varying(250) NULL,"
                    sqlq = sqlq & "[Tbl3] character varying(250) NULL,"
                    sqlq = sqlq & "[Tbl3Fld3] character varying(250) NULL,"
                    sqlq = sqlq & "[Type] character varying(10) NULL,"
                    sqlq = sqlq & "[comments] character varying(240) NULL,"
                    sqlq = sqlq & "[Friendly] character varying(50) NULL,"
                    sqlq = sqlq & "[RecOrder] smallint DEFAULT 0,"
                    sqlq = sqlq & """Group"" character varying(220) DEFAULT NULL,"
                    sqlq = sqlq & "[Logical] character varying(120) DEFAULT NULL,"
                    sqlq = sqlq & "[Param1] character varying(220) DEFAULT NULL,"
                    sqlq = sqlq & "[Param2] character varying(220) DEFAULT NULL,"
                    sqlq = sqlq & "[Param3] character varying(220) DEFAULT NULL,"
                    sqlq = sqlq & "[Indx] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURReportSQLquery_pkey] PRIMARY KEY ([Indx]))"

                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE Table [OURReportSQLquery]("
                    sqlq = sqlq & "[ReportId] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Doing] [nchar](10) NULL,"
                    sqlq = sqlq & "[Tbl1] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Tbl1Fld1] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Oper] [nchar](20) NULL,"
                    sqlq = sqlq & "[Tbl2] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Tbl2Fld2] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Tbl3] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Tbl3Fld3] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Type] [nchar](10) NULL,"
                    sqlq = sqlq & "[comments] [nvarchar](2240) NULL,"
                    sqlq = sqlq & "[Friendly] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[RecOrder] int DEFAULT NULL,"
                    sqlq = sqlq & FixReservedWords("Group", myconprv) & " varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & "[Logical] varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & "[Param1] varchar(220) DEFAULT NULL,"
                    sqlq = sqlq & "[Param2] varchar(220) DEFAULT NULL,"
                    sqlq = sqlq & "[Param3] varchar(220) DEFAULT NULL,"
                    sqlq = sqlq & " [Indx] INTEGER PRIMARY KEY AUTOINCREMENT)"

                Else 'Sql Server or Cache
                    sqlq = "CREATE Table [OURReportSQLquery]("
                    sqlq = sqlq & "[ReportId] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Doing] [nchar](10) NULL,"
                    sqlq = sqlq & "[Tbl1] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Tbl1Fld1] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Oper] [nchar](20) NULL,"
                    sqlq = sqlq & "[Tbl2] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Tbl2Fld2] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Tbl3] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Tbl3Fld3] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Type] [nchar](10) NULL,"
                    sqlq = sqlq & "[comments] [nvarchar](2240) NULL,"
                    sqlq = sqlq & "[Friendly] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[RecOrder] int DEFAULT NULL,"
                    sqlq = sqlq & FixReservedWords("Group", myconprv) & " varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & "[Logical] varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & "[Param1] varchar(220) DEFAULT NULL,"
                    sqlq = sqlq & "[Param2] varchar(220) DEFAULT NULL,"
                    sqlq = sqlq & "[Param3] varchar(220) DEFAULT NULL,"
                    sqlq = sqlq & "[Indx] [Int] IDENTITY(1, 1) Not NULL"
                    sqlq = sqlq & ")"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "OURReportSQLquery created"
                Else
                    ret = "OURReportSQLquery creation crashed: " & ret
                End If
            End If

        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function InstallOURReportGroups(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            If TableExists("OURReportGroups", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURReportGroups: " & UpdateOURReportGroups(myconstr, myconprv)

            Else
                'create table OURReportGroups
                Dim db As String = GetDataBase(myconstr, myconprv)
                If myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "CREATE TABLE `" & db & "`.`ourreportgroups` ("
                    sqlq = sqlq & " `ReportId` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `GroupField` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `CalcField` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `CntChk` int(11) NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " `SumChk` int(11) NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " `MaxChk` int(11) NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " `MinChk` int(11) NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " `AvgChk` int(11) NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " `StDevChk` int(11) NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " `CntDistChk` int(11) NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " `FirstChk` int(11) NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " `LastChk` int(11) NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " `comments` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `GrpOrder` int(11) NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " `PageBrk` int(11) NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " `Indx` int(11) NOT NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " PRIMARY KEY (`Indx`)"
                    sqlq = sqlq & ");"

                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURREPORTGROUPS ("
                    sqlq = sqlq & "ReportId VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "GroupField VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "CalcField VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "CntChk NUMBER(11,0) DEFAULT 0 NOT NULL,"
                    sqlq = sqlq & "SumChk NUMBER(11,0) DEFAULT 0 NOT NULL,"
                    sqlq = sqlq & "MaxChk NUMBER(11,0) DEFAULT 0 NOT NULL,"
                    sqlq = sqlq & "MinChk NUMBER(11,0) DEFAULT 0 NOT NULL,"
                    sqlq = sqlq & "AvgChk NUMBER(11,0) DEFAULT 0 NOT NULL,"
                    sqlq = sqlq & "StDevChk NUMBER(11,0) DEFAULT 0 NOT NULL,"
                    sqlq = sqlq & "CntDistChk NUMBER(11,0) DEFAULT 0 NOT NULL,"
                    sqlq = sqlq & "FirstChk NUMBER(11,0) DEFAULT 0 NOT NULL,"
                    sqlq = sqlq & "LastChk NUMBER(11,0) DEFAULT 0 NOT NULL,"
                    sqlq = sqlq & "comments VARCHAR2(240 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "GrpOrder NUMBER(11,0) DEFAULT 0 NOT NULL,"
                    sqlq = sqlq & "PageBrk NUMBER(11,0) DEFAULT 0 NOT NULL,"
                    sqlq = sqlq & "Indx Integer GENERATED ALWAYS As IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURREPORTGROUPS_PK PRIMARY KEY (Indx)"
                    sqlq = sqlq & ")"

                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURReportGroups]("
                    sqlq = sqlq & "[ReportId] character varying(50) NULL,"
                    sqlq = sqlq & "[GroupField] character varying(50) NULL,"
                    sqlq = sqlq & "[CalcField] character varying(50) NULL,"
                    sqlq = sqlq & "[CntChk] smallint Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[SumChk] smallint Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[MaxChk] smallint Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[MinChk] smallint Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[AvgChk] smallint Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[StDevChk] smallint Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[CntDistChk] smallint Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[FirstChk] smallint Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[LastChk] smallint Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[comments] character varying(240) NULL,"
                    sqlq = sqlq & "[GrpOrder] smallint Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[PageBrk] smallint Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[Indx] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURReportGroups_pkey] PRIMARY KEY ([Indx]))"

                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE Table [OURReportGroups]("
                    sqlq = sqlq & "[ReportId] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[GroupField] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[CalcField] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[CntChk] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[SumChk] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[MaxChk] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[MinChk] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[AvgChk] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[StDevChk] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[CntDistChk] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[FirstChk] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[LastChk] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[comments] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[GrpOrder] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[PageBrk] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & " [Indx] INTEGER PRIMARY KEY AUTOINCREMENT)"

                Else 'Sql Server or Cache
                    sqlq = "CREATE Table [OURReportGroups]("
                    sqlq = sqlq & "[ReportId] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[GroupField] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[CalcField] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[CntChk] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[SumChk] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[MaxChk] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[MinChk] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[AvgChk] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[StDevChk] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[CntDistChk] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[FirstChk] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[LastChk] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[comments] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[GrpOrder] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[PageBrk] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[Indx] [Int] IDENTITY(1, 1) Not NULL"
                    sqlq = sqlq & ")"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "OURReportGroups created"
                Else
                    ret = "OURReportGroups creation crashed: " & ret
                End If
            End If

        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function InstallOURReportFormat(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            If TableExists("OURReportFormat", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURReportFormat: " & UpdateOURReportFormat(myconstr, myconprv)

            Else
                'create table OURReportFormat
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "CREATE TABLE `" & db.ToLower & "`.`ourreportformat` ("
                    sqlq = sqlq & " `ReportId` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `Val` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `Order` int(11) NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " `Prop1` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop2` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop3` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop4` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop5` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop6` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop7` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Indx` int(11) NOT NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " PRIMARY KEY (`Indx`)"
                    sqlq = sqlq & ");"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURREPORTFORMAT ("
                    sqlq = sqlq & "ReportId VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Val VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & """Order"" NUMBER(11,0) Default 0 Not NULL,"
                    sqlq = sqlq & "Prop1 VARCHAR2(250 Char) Default NULL,"
                    sqlq = sqlq & "Prop2 VARCHAR2(250 Char) Default NULL,"
                    sqlq = sqlq & "Prop3 VARCHAR2(250 Char) Default NULL,"
                    sqlq = sqlq & "Prop4 VARCHAR2(250 Char) Default NULL,"
                    sqlq = sqlq & "Prop5 VARCHAR2(250 Char) Default NULL,"
                    sqlq = sqlq & "Prop6 VARCHAR2(250 Char) Default NULL,"
                    sqlq = sqlq & "Prop7 VARCHAR2(250 Char) Default NULL,"
                    sqlq = sqlq & "Indx Integer GENERATED ALWAYS As IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURREPORTFORMAT_PK PRIMARY KEY (Indx)"
                    sqlq = sqlq & ")"

                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURReportFormat]("
                    sqlq = sqlq & "[ReportId] character varying(50) NULL,"
                    sqlq = sqlq & "[Prop] character varying(50) NULL,"
                    sqlq = sqlq & "[Val] character varying(50) NULL,"
                    sqlq = sqlq & """Order"" smallint NOT NULL DEFAULT 0,"
                    sqlq = sqlq & "[Prop1] character varying(250) NULL,"
                    sqlq = sqlq & "[Prop2] character varying(250) NULL,"
                    sqlq = sqlq & "[Prop3] character varying(250) NULL,"
                    sqlq = sqlq & "[Prop4] character varying(250) NULL,"
                    sqlq = sqlq & "[Prop5] character varying(250) NULL,"
                    sqlq = sqlq & "[Prop6] character varying(250) NULL,"
                    sqlq = sqlq & "[Prop7] character varying(250) NULL,"
                    sqlq = sqlq & "[Indx] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURReportFormat_pkey] PRIMARY KEY ([Indx]))"

                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE Table [OURReportFormat]("
                    sqlq = sqlq & "[ReportId] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Prop] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Val] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Order] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[Prop1] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop3] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop4] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop5] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop6] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop7] [nvarchar](250) NULL,"
                    sqlq = sqlq & " [Indx] INTEGER PRIMARY KEY AUTOINCREMENT)"

                Else 'Sql Server or Cache
                    sqlq = "CREATE Table [OURReportFormat]("
                    sqlq = sqlq & "[ReportId] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Prop] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Val] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Order] [Int] Not NULL DEFAULT 0,"
                    sqlq = sqlq & "[Prop1] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop3] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop4] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop5] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop6] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop7] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Indx] [Int] IDENTITY(1, 1) Not NULL"
                    sqlq = sqlq & ")"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "OURReportFormat created"
                Else
                    ret = "OURReportFormat creation crashed: " & ret
                End If
            End If
            ''Update column changes or additions
            'If ret = "Table exists" OrElse ret = "OURReportFormat created" Then
            '    Dim bExists4 As Boolean = ColumnExists("OURReportFormat", "Prop4", myconstr, myconprv)
            '    If bExists4 Then
            '        If myconprv = "MySql.Data.MySqlClient" Then
            '            sqlq = "ALTER Table `" & db.ToLower & "`.`OURReportFormat` CHANGE COLUMN [Prop1] [Prop1] VARCHAR( 250 ) NULL Default NULL AFTER  [Order] ,"
            '            sqlq = sqlq & "CHANGE COLUMN [Prop2] [Prop2] VARCHAR( 250 )  Default NULL AFTER  [Prop1],"
            '            sqlq = sqlq & "CHANGE COLUMN [Prop3] [Prop3] VARCHAR( 250 )  Default NULL AFTER  [Prop2],"
            '            sqlq = sqlq & "CHANGE COLUMN [Prop4] [Prop4] VARCHAR( 250 )  Default NULL AFTER  [Prop3],"
            '            sqlq = sqlq & "CHANGE COLUMN [Prop5] [Prop5] VARCHAR( 250 )  Default NULL AFTER  [Prop4],"
            '            sqlq = sqlq & "CHANGE COLUMN [Prop6] [Prop6] VARCHAR( 250 )  Default NULL AFTER  [Prop5] ;"
            '            ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
            '        End If
            '    Else
            '        If myconprv = "MySql.Data.MySqlClient" Then
            '            sqlq = "ALTER Table `" & db.ToLower & "`.`OURReportFormat` "
            '            sqlq = sqlq & "ADD COLUMN [Prop4] VARCHAR( 250 ) NULL Default NULL AFTER  [Prop3],"
            '            sqlq = sqlq & "ADD COLUMN [Prop5] VARCHAR( 250 ) NULL Default NULL AFTER  [Prop4],"
            '            sqlq = sqlq & "ADD COLUMN [Prop6] VARCHAR( 250 ) NULL Default NULL AFTER  [Prop5];"
            '        ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
            '            sqlq = "ALTER TABLE OURREPORTFORMAT ADD ("
            '            sqlq &= " Prop4 VARCHAR2(250 CHAR) DEFAULT NULL,"
            '            sqlq &= " Prop5 VARCHAR2(250 CHAR) DEFAULT NULL,"
            '            sqlq &= " Prop6 VARCHAR2(250 CHAR) DEFAULT NULL"
            '            sqlq &= ")"
            '        Else
            '            sqlq = "ALTER TABLE OURReportFormat ADD "
            '            sqlq &= "[Prop4] [nvarchar](250) NULL,"
            '            sqlq &= "[Prop5] [nvarchar](250) NULL,"
            '            sqlq &= "[Prop6] [nvarchar](250) NULL"
            '        End If
            '        ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
            '    End If
            'End If
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function InstallOURReportShow(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            If TableExists("OURReportShow", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURReportShow: " & UpdateOURReportShow(myconstr, myconprv)

            Else
                'create table OURReportShow
                If myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "CREATE TABLE `" & db & "`.`ourreportshow` ("
                    sqlq = sqlq & " `ReportId` varchar(50) Default NULL,"
                    sqlq = sqlq & " `DropDownID` varchar(50) Default NULL,"
                    sqlq = sqlq & " `DropDownLabel` varchar(50) Default NULL,"
                    sqlq = sqlq & " `DropDownName` varchar(250) Default NULL,"
                    sqlq = sqlq & " `DropDownFieldName` varchar(250) Default NULL,"
                    sqlq = sqlq & " `DropDownFieldType` varchar(50) Default NULL,"
                    sqlq = sqlq & " `DropDownFieldValue` varchar(50) Default NULL,"
                    sqlq = sqlq & " `DropDownSQL` varchar(4000) Default NULL,"
                    sqlq = sqlq & " `comments` varchar(240) Default NULL,"
                    sqlq = sqlq & " `PrmOrder` int(3) Not NULL Default 0,"
                    sqlq = sqlq & " `Indx` int(11) Not NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " PRIMARY KEY (`Indx`)"
                    sqlq = sqlq & ");"

                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURREPORTSHOW ("
                    sqlq = sqlq & "ReportId VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "DropDownID VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "DropDownLabel VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "DropDownName VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "DropDownFieldName VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "DropDownFieldType VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "DropDownFieldValue VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "DropDownSQL VARCHAR2(4000 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "comments VARCHAR2(240 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "PrmOrder NUMBER(3,0) DEFAULT 0 NOT NULL,"
                    sqlq = sqlq & "Indx INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURREPORTSHOW_PK PRIMARY KEY (Indx)"
                    sqlq = sqlq & ")"

                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURReportShow]("
                    sqlq = sqlq & "[ReportID] character varying(50) NULL,"
                    sqlq = sqlq & "[DropDownID] character varying(50) NULL,"
                    sqlq = sqlq & "[DropDownLabel] character varying(50) NULL,"
                    sqlq = sqlq & "[DropDownName] character varying(250) NULL,"
                    sqlq = sqlq & "[DropDownFieldName] character varying(250) NULL,"
                    sqlq = sqlq & "[DropDownFieldType] character varying(50) NULL,"
                    sqlq = sqlq & "[DropDownFieldValue] character varying(50) NULL,"
                    sqlq = sqlq & "[DropDownSQL] character varying(4000) NULL,"
                    sqlq = sqlq & "[comments] character varying(240) NULL,"
                    sqlq = sqlq & "[PrmOrder] smallint Not NULL Default 0,"
                    sqlq = sqlq & "[Indx] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURReportShow_pkey] PRIMARY KEY ([Indx]))"

                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE Table [OURReportShow]("
                    sqlq = sqlq & "[ReportID] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[DropDownID] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[DropDownLabel] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[DropDownName] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[DropDownFieldName] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[DropDownFieldType] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[DropDownFieldValue] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[DropDownSQL] [nvarchar](4000) NULL,"
                    sqlq = sqlq & "[comments] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[PrmOrder] [Int] Not NULL Default 0,"
                    sqlq = sqlq & " [Indx] INTEGER PRIMARY KEY AUTOINCREMENT)"

                Else 'Sql Server or Cache
                    sqlq = "CREATE Table [OURReportShow]("
                    sqlq = sqlq & "[ReportID] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[DropDownID] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[DropDownLabel] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[DropDownName] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[DropDownFieldName] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[DropDownFieldType] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[DropDownFieldValue] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[DropDownSQL] [nvarchar](4000) NULL,"
                    sqlq = sqlq & "[comments] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[PrmOrder] [Int] Not NULL Default 0,"
                    sqlq = sqlq & "[Indx] [Int] IDENTITY(1, 1) Not NULL"
                    sqlq = sqlq & ")"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "OURReportShow created"
                Else
                    ret = "OURReportShow creation crashed: " & ret
                End If
            End If

        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function InstallOURReportInfo(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Dim dt As DataTable = Nothing

        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            Dim nCharCount As Integer = 250

            If TableExists("OURReportInfo", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURReportInfo: " & UpdateOURReportInfo(myconstr, myconprv)

            Else
                'create table OURReportInfo
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "CREATE TABLE `" & db.ToLower & "`.`ourreportinfo` ("
                    sqlq = sqlq & " `ReportId` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `ReportName` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `ReportType` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `ReportAttributes` varchar(80) DEFAULT NULL,"
                    sqlq = sqlq & " `ConnectionId` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `SQLquerytext` varchar(4000) DEFAULT NULL,"
                    sqlq = sqlq & " `ParamQuantity` char(2) DEFAULT NULL,"
                    sqlq = sqlq & " `ParamNumbers` char(10) DEFAULT NULL,"
                    sqlq = sqlq & " `ReportFile` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `Param0id` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Param0type` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Param1id` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Param1type` varchar(250) Default NULL,"
                    sqlq = sqlq & " `Param2id` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Param2type` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Param3id` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Param3type` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Param4id` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Param4type` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Param5id` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Param5type` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Param6id` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Param6type` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Param7id` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Param7type` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Param8id` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Param8type` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Param9id` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Param9type` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `ReportTtl` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & "`ReportDB` varchar(250)DEFAULT NULL,"
                    sqlq = sqlq & " `comments` varchar(2400) DEFAULT NULL,"
                    sqlq = sqlq & " `Indx` int(11) NOT NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " PRIMARY KEY (`Indx`)"
                    sqlq = sqlq & ");"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURREPORTINFO ("
                    sqlq = sqlq & "ReportId VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ReportName VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ReportType VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ReportAttributes VARCHAR2(80 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ConnectionId VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "SQLquerytext VARCHAR2(4000 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ParamQuantity CHAR(2) DEFAULT NULL,"
                    sqlq = sqlq & "ParamNumbers CHAR(10) DEFAULT NULL,"
                    sqlq = sqlq & "ReportFile VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param0id VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param0type VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param1id VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param1type VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param2id VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param2type VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param3id VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param3type VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param4id VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param4type VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param5id VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param5type VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param6id VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param6type VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param7id VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param7type VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param8id VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param8type VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param9id VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param9type VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ReportTtl VARCHAR2(240 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ReportDB VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "comments VARCHAR2(2400 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Indx INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURREPORTINFO_PK PRIMARY KEY (Indx)"
                    sqlq = sqlq & ")"

                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE TABLE [OURReportInfo]("
                    sqlq = sqlq & "[ReportId] character varying(50) NULL,"
                    sqlq = sqlq & "[ReportName] character varying(250) NULL,"
                    sqlq = sqlq & "[ReportType] character varying(50) NULL,"
                    sqlq = sqlq & "[ReportAttributes] character varying(80) NULL,"
                    sqlq = sqlq & "[ConnectionId] character varying(50) NULL,"
                    sqlq = sqlq & "[SQLquerytext] character varying(4000) NULL,"
                    sqlq = sqlq & "[ParamQuantity] character varying(2) NULL,"
                    sqlq = sqlq & "[ParamNumbers] character varying(10) NULL,"
                    sqlq = sqlq & "[ReportFile] character varying(50) NULL,"
                    sqlq = sqlq & "[Param0id] character varying(250) NULL,"
                    sqlq = sqlq & "[Param0type] character varying(250) NULL,"
                    sqlq = sqlq & "[Param1id] character varying(250) NULL,"
                    sqlq = sqlq & "[Param1type] character varying(250) NULL,"
                    sqlq = sqlq & "[Param2id] character varying(250) NULL,"
                    sqlq = sqlq & "[Param2type] character varying(250) NULL,"
                    sqlq = sqlq & "[Param3id] character varying(250) NULL,"
                    sqlq = sqlq & "[Param3type] character varying(250) NULL,"
                    sqlq = sqlq & "[Param4id] character varying(250) NULL,"
                    sqlq = sqlq & "[Param4type] character varying(250) NULL,"
                    sqlq = sqlq & "[Param5id] character varying(250) NULL,"
                    sqlq = sqlq & "[Param5type] character varying(250) NULL,"
                    sqlq = sqlq & "[Param6id] character varying(250) NULL,"
                    sqlq = sqlq & "[Param6type] character varying(250) NULL,"
                    sqlq = sqlq & "[Param7id] character varying(250) NULL,"
                    sqlq = sqlq & "[Param7type] character varying(250) NULL,"
                    sqlq = sqlq & "[Param8id] character varying(250) NULL,"
                    sqlq = sqlq & "[Param8type] character varying(250) NULL,"
                    sqlq = sqlq & "[Param9id] character varying(250) NULL,"
                    sqlq = sqlq & "[Param9type] character varying(250) NULL,"
                    sqlq = sqlq & "[ReportTtl] character varying(240) NULL,"
                    sqlq = sqlq & "[ReportDB] character varying(250) NULL,"
                    sqlq = sqlq & "[comments] character varying(2400) NULL,"
                    sqlq = sqlq & "[Indx] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURReportInfo_pkey] PRIMARY KEY ([Indx]))"
                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE Table [OURReportInfo]("
                    sqlq = sqlq & "[ReportId] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[ReportName] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[ReportType] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[ReportAttributes] [nvarchar](80) NULL,"
                    sqlq = sqlq & "[ConnectionId] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[SQLquerytext] [nvarchar](4000) NULL,"
                    sqlq = sqlq & "[ParamQuantity] [nchar](2) NULL,"
                    sqlq = sqlq & "[ParamNumbers] [nchar](10) NULL,"
                    sqlq = sqlq & "[ReportFile] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Param0id] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param0type] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param1id] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param1type] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param2id] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param2type] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param3id] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param3type] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param4id] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param4type] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param5id] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param5type] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param6id] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param6type] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param7id] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param7type] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param8id] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param8type] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param9id] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param9type] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[ReportTtl] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[ReportDB] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[comments] [nvarchar](2400) NULL,"
                    sqlq = sqlq & " [Indx] INTEGER PRIMARY KEY AUTOINCREMENT)"
                Else 'Sql Server or Cache
                    sqlq = "CREATE Table [OURReportInfo]("
                    sqlq = sqlq & "[ReportId] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[ReportName] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[ReportType] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[ReportAttributes] [nvarchar](80) NULL,"
                    sqlq = sqlq & "[ConnectionId] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[SQLquerytext] [nvarchar](4000) NULL,"
                    sqlq = sqlq & "[ParamQuantity] [nchar](2) NULL,"
                    sqlq = sqlq & "[ParamNumbers] [nchar](10) NULL,"
                    sqlq = sqlq & "[ReportFile] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Param0id] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param0type] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param1id] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param1type] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param2id] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param2type] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param3id] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param3type] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param4id] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param4type] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param5id] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param5type] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param6id] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param6type] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param7id] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param7type] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param8id] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param8type] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param9id] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param9type] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[ReportTtl] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[ReportDB] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[comments] [nvarchar](2400) NULL,"
                    sqlq = sqlq & "[Indx] [Int] IDENTITY(1, 1) Not NULL"
                    sqlq = sqlq & ")"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "OURReportInfo created"
                Else
                    ret = "OURReportInfo creation crashed: " & ret
                End If
            End If

            ''Update column changes or additions
            'If ret = "Table exists" OrElse ret = "OURReportInfo created" Then
            '    If myconprv = "MySql.Data.MySqlClient" Then
            '        sqlq = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='" & db & "' And TABLE_NAME='ourreportinfo' And COLUMN_NAME='Param5id'"
            '        dt = mRecords(sqlq, err, myconstr, myconprv).Table
            '        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            '            nCharCount = CInt(dt.Rows(0)("CHARACTER_MAXIMUM_LENGTH"))
            '        End If
            '        sqlq = "ALTER TABLE `" & db & "`.`ourreportinfo` "
            '        sqlq = sqlq & "CHANGE COLUMN `Param0id` `Param0id` VARCHAR(250) NULL DEFAULT NULL,"
            '        sqlq = sqlq & "CHANGE COLUMN `Param0type` `Param0type` VARCHAR(250) NULL DEFAULT NULL,"
            '        sqlq = sqlq & "CHANGE COLUMN `Param1id` `Param1id` VARCHAR(250) NULL DEFAULT NULL,"
            '        sqlq = sqlq & "CHANGE COLUMN `Param1type` `Param1type` VARCHAR(250) NULL DEFAULT NULL,"
            '        sqlq = sqlq & "CHANGE COLUMN `Param2id` `Param2id` VARCHAR(250) NULL DEFAULT NULL,"
            '        sqlq = sqlq & "CHANGE COLUMN `Param2type` `Param2type` VARCHAR(250) NULL DEFAULT NULL,"
            '        sqlq = sqlq & "CHANGE COLUMN `Param3id` `Param3id` VARCHAR(250) NULL DEFAULT NULL,"
            '        sqlq = sqlq & "CHANGE COLUMN `Param3type` `Param3type` VARCHAR(250) NULL DEFAULT NULL,"
            '        sqlq = sqlq & "CHANGE COLUMN `Param4id` `Param4id` VARCHAR(250) NULL DEFAULT NULL,"
            '        sqlq = sqlq & "CHANGE COLUMN `Param4type` `Param4type` VARCHAR(250) NULL DEFAULT NULL,"
            '        sqlq = sqlq & "CHANGE COLUMN `Param5id` `Param5id` VARCHAR(250) NULL DEFAULT NULL,"
            '        sqlq = sqlq & "CHANGE COLUMN `Param5type` `Param5type` VARCHAR(250) NULL DEFAULT NULL,"
            '        sqlq = sqlq & "CHANGE COLUMN `Param6id` `Param6id` VARCHAR(250) NULL DEFAULT NULL,"
            '        sqlq = sqlq & "CHANGE COLUMN `Param6type` `Param6type` VARCHAR(250) NULL DEFAULT NULL,"
            '        sqlq = sqlq & "CHANGE COLUMN `Param7id` `Param7id` VARCHAR(250) NULL DEFAULT NULL,"
            '        sqlq = sqlq & "CHANGE COLUMN `Param7type` `Param7type` VARCHAR(250) NULL DEFAULT NULL,"
            '        sqlq = sqlq & "CHANGE COLUMN `Param8id` `Param8id` VARCHAR(250) NULL DEFAULT NULL,"
            '        sqlq = sqlq & "CHANGE COLUMN `Param8type` `Param8type` VARCHAR(250) NULL DEFAULT NULL,"
            '        sqlq = sqlq & "CHANGE COLUMN `Param9id` `Param9id` VARCHAR(250) NULL DEFAULT NULL,"
            '        sqlq = sqlq & "CHANGE COLUMN `Param9type` `Param9type` VARCHAR(250) NULL DEFAULT NULL;"
            '    ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
            '        sqlq = "Select * FROM all_tab_cols WHERE TABLE_NAME = 'OURREPORTINFO'  AND COLUMN_NAME = 'PARAM5ID'"
            '        dt = mRecords(sqlq, err, myconstr, myconprv).Table
            '        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            '            nCharCount = CInt(dt.Rows(0)("CHAR_LENGTH"))
            '        End If
            '        sqlq = "ALTER TABLE OURREPORTINFO "
            '        sqlq &= "MODIFY ( "
            '        sqlq &= "Param0id VARCHAR2(250 CHAR) DEFAULT NULL,"
            '        sqlq &= "Param0type VARCHAR2(250 CHAR) DEFAULT NULL,"
            '        sqlq &= "Param1id VARCHAR2(250 CHAR) DEFAULT NULL,"
            '        sqlq &= "Param1type VARCHAR2(250 CHAR) DEFAULT NULL,"
            '        sqlq &= "Param2id VARCHAR2(250 CHAR) DEFAULT NULL,"
            '        sqlq &= "Param2type VARCHAR2(250 CHAR) DEFAULT NULL,"
            '        sqlq &= "Param3id VARCHAR2(250 CHAR) DEFAULT NULL,"
            '        sqlq &= "Param3type VARCHAR2(250 CHAR) DEFAULT NULL,"
            '        sqlq &= "Param4id VARCHAR2(250 CHAR) DEFAULT NULL,"
            '        sqlq &= "Param4type VARCHAR2(250 CHAR) DEFAULT NULL,"
            '        sqlq &= "Param5id VARCHAR2(250 CHAR) DEFAULT NULL,"
            '        sqlq &= "Param5type VARCHAR2(250 CHAR) DEFAULT NULL,"
            '        sqlq &= "Param6id VARCHAR2(250 CHAR) DEFAULT NULL,"
            '        sqlq &= "Param6type VARCHAR2(250 CHAR) DEFAULT NULL,"
            '        sqlq &= "Param7id VARCHAR2(250 CHAR) DEFAULT NULL,"
            '        sqlq &= "Param7type VARCHAR2(250 CHAR) DEFAULT NULL,"
            '        sqlq &= "Param8id VARCHAR2(250 CHAR) DEFAULT NULL,"
            '        sqlq &= "Param8type VARCHAR2(250 CHAR) DEFAULT NULL,"
            '        sqlq &= "Param9id VARCHAR2(250 CHAR) DEFAULT NULL,"
            '        sqlq &= "Param9type VARCHAR2(250 CHAR) DEFAULT NULL,"
            '        sqlq &= ")"
            '    ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql

            '    Else
            '        If myconprv.StartsWith("InterSystems.Data.") Then
            '            sqlq = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='OUR' AND TABLE_NAME='ReportInfo' AND COLUMN_NAME='Param5id'"
            '        Else
            '            sqlq = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='OURReportInfo' AND COLUMN_NAME='Param5id'"
            '        End If
            '        dt = mRecords(sqlq, err, myconstr, myconprv).Table
            '        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            '            nCharCount = CInt(dt.Rows(0)("CHARACTER_MAXIMUM_LENGTH"))
            '        End If
            '        If nCharCount < 250 Then
            '            For i As Integer = 0 To 9
            '                sqlq = "ALTER TABLE OURReportInfo ALTER COLUMN Param" & i & "id nvarchar(250) NULL"
            '                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
            '                sqlq = "ALTER TABLE OURReportInfo ALTER COLUMN Param" & i & "type nvarchar(250) NULL"
            '                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
            '            Next
            '            nCharCount = 250
            '        End If
            '    End If
            '    If nCharCount < 250 Then
            '        ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
            '    End If
            'End If
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function InstallOURPermissions(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            If TableExists("OURPermissions", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURPermissions: " & UpdateOURPermissions(myconstr, myconprv)

            Else
                Dim db As String = GetDataBase(myconstr, myconprv)
                'create table OURPermissions
                If myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "CREATE TABLE `" & db & "`.`ourpermissions` ("
                    sqlq = sqlq & " `NetId` varchar(120) NOT NULL,"
                    sqlq = sqlq & " `Application` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `Param1` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Param2` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Param3` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `AccessLevel` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `OpenFrom` datetime DEFAULT NULL,"
                    sqlq = sqlq & " `OpenTo` datetime DEFAULT NULL,"
                    sqlq = sqlq & " `Comments` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `Indx` int(11) NOT NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " PRIMARY KEY (`Indx`)"
                    sqlq = sqlq & ");"

                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURPERMISSIONS ("
                    sqlq = sqlq & "NetId VARCHAR2(120 CHAR) NOT NULL,"
                    sqlq = sqlq & "Application VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param1 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param2 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Param3 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "AccessLevel VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "OpenFrom VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "OpenTo VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Comments VARCHAR2(240 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Indx INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURPERMISSIONS_PK PRIMARY KEY (Indx)"
                    sqlq = sqlq & ")"

                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURPermissions]("
                    sqlq = sqlq & "[NetId] character varying(120) Not NULL,"
                    sqlq = sqlq & "[Application] character varying(50) NULL,"
                    sqlq = sqlq & "[Param1] character varying(250) NULL,"
                    sqlq = sqlq & "[Param2] character varying(250) NULL,"
                    sqlq = sqlq & "[Param3] character varying(250) NULL,"
                    sqlq = sqlq & "[AccessLevel] character varying(50) NULL,"
                    sqlq = sqlq & "[OpenFrom] character varying(50) NULL,"
                    sqlq = sqlq & "[OpenTo] character varying(50) NULL,"
                    sqlq = sqlq & "[Comments] character varying(240) NULL,"
                    sqlq = sqlq & "[Indx] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURPermissions_pkey] PRIMARY KEY ([Indx]))"

                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE Table [OURPermissions]("
                    sqlq = sqlq & "[NetId] [nvarchar](120) Not NULL,"
                    sqlq = sqlq & "[Application] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Param1] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param2] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param3] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[AccessLevel] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[OpenFrom] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[OpenTo] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Comments] [nvarchar](240) NULL,"
                    sqlq = sqlq & " [Indx] INTEGER PRIMARY KEY AUTOINCREMENT)"

                Else 'Sql Server or Cache
                    sqlq = "CREATE Table [OURPermissions]("
                    sqlq = sqlq & "[NetId] [nvarchar](120) Not NULL,"
                    sqlq = sqlq & "[Application] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Param1] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param2] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Param3] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[AccessLevel] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[OpenFrom] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[OpenTo] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Comments] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[Indx] [Int] IDENTITY(1, 1) Not NULL PRIMARY KEY"
                    sqlq = sqlq & ")"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "OURPermissions created"
                Else
                    ret = "OURPermissions creation crashed: " & ret
                End If
            End If

        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function InstallOURPermits(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            If TableExists("OURPermits", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURPermits: " & UpdateOURPermits(myconstr, myconprv)

            Else
                'create table OURPermits
                If myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "CREATE TABLE `" & db & "`.`ourpermits` ("
                    sqlq = sqlq & " `Access` char(10) DEFAULT NULL,"
                    sqlq = sqlq & " `PERMIT` char(10) DEFAULT NULL,"
                    sqlq = sqlq & " `NetId` char(120)  DEFAULT NULL,"
                    sqlq = sqlq & " `localpass` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `Name` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `Unit` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `Application` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `RoleApp` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `Group1` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `Group2` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `Group3` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `Comments` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `ConnStr` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `ConnPrv` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `StartDate` datetime DEFAULT NULL,"
                    sqlq = sqlq & " `EndDate` datetime DEFAULT NULL,"
                    sqlq = sqlq & " `paid` int(11) DEFAULT NULL,"
                    sqlq = sqlq & " `Email` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Indx` int(11) Not NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " PRIMARY KEY (`Indx`)"
                    sqlq = sqlq & ");"

                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURPERMITS ("
                    sqlq = sqlq & """Access"" VARCHAR2(10 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "PERMIT VARCHAR2(10 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "NetId VARCHAR2(120 CHAR ) DEFAULT NULL,"
                    sqlq = sqlq & "localpass VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Name VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Unit VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Application VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "RoleApp VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Group1 VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Group2 VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Group3 VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Comments VARCHAR2(240 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ConnStr VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ConnPrv VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "StartDate DATE DEFAULT NULL,"
                    sqlq = sqlq & "EndDate DATE DEFAULT NULL,"
                    sqlq = sqlq & "Email VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Indx INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURPERMITS_PK PRIMARY KEY (Indx)"
                    sqlq = sqlq & ")"

                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURPermits]("
                    sqlq = sqlq & "[Access] character varying(10) NULL,"
                    sqlq = sqlq & "[PERMIT] character varying(10) NULL,"
                    sqlq = sqlq & "[NetId] character varying(120) NULL,"
                    sqlq = sqlq & "[localpass] character varying(50) NULL,"
                    sqlq = sqlq & "[Name] character varying(50) NULL,"
                    sqlq = sqlq & "[Unit] character varying(50) NULL,"
                    sqlq = sqlq & "[Application] character varying(50) NULL,"
                    sqlq = sqlq & "[RoleApp] character varying(50) NULL,"
                    sqlq = sqlq & "[Group1] character varying(250) NULL,"
                    sqlq = sqlq & "[Group2] character varying(250) NULL,"
                    sqlq = sqlq & "[Group3] character varying(250) NULL,"
                    sqlq = sqlq & "[Comments] character varying(240) NULL,"
                    sqlq = sqlq & "[ConnStr] character varying(250) NULL,"
                    sqlq = sqlq & "[ConnPrv] character varying(250) NULL,"
                    sqlq = sqlq & "[StartDate] date NULL,"
                    sqlq = sqlq & "[EndDate] date NULL,"
                    sqlq = sqlq & "[paid] integer NULL,"
                    sqlq = sqlq & "[Email] character varying(250) NULL,"
                    sqlq = sqlq & "[Indx] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURPermits_pkey] PRIMARY KEY ([Indx]))"

                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE TABLE OURPermits("
                    sqlq = sqlq & "[Access] [nchar](10) NULL,"
                    sqlq = sqlq & "[PERMIT] [nchar](10) NULL,"
                    sqlq = sqlq & "[NetId] [nchar](120) NULL,"
                    sqlq = sqlq & "[localpass] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Name] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Unit] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Application] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[RoleApp] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Group1] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Group2] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Group3] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Comments] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[ConnStr] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[ConnPrv] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[StartDate] [datetime] NULL,"
                    sqlq = sqlq & "[EndDate] [datetime] NULL,"
                    sqlq = sqlq & "[paid] [int] NULL,"
                    sqlq = sqlq & "[Email] [nvarchar](250) NULL,"
                    sqlq = sqlq & " [Indx] INTEGER PRIMARY KEY AUTOINCREMENT)"

                Else 'Sql Server or Cache
                    sqlq = "CREATE TABLE OURPermits("
                    sqlq = sqlq & "[Access] [nchar](10) NULL,"
                    sqlq = sqlq & "[PERMIT] [nchar](10) NULL,"
                    sqlq = sqlq & "[NetId] [nchar](120) NULL,"
                    sqlq = sqlq & "[localpass] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Name] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Unit] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Application] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[RoleApp] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Group1] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Group2] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Group3] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Comments] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[ConnStr] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[ConnPrv] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[StartDate] [datetime] NULL,"
                    sqlq = sqlq & "[EndDate] [datetime] NULL,"
                    sqlq = sqlq & "[paid] [int] NULL,"
                    sqlq = sqlq & "[Email] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Indx] [int] IDENTITY(1,1) NOT NULL)"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
            End If

            If ret = "Query executed fine." OrElse (TableExists("OURPermits", myconstr, myconprv, err)) Then
                ret = "OURPermits created"
                'insert first record for super user
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "SELECT * FROM `" & db & "`.`ourpermits`"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    'sqlq = "SELECT * FROM `" & db & "`.`OURPermits`"
                    sqlq = "SELECT * FROM `OURPermits`"
                Else
                    sqlq = "SELECT * FROM OURPermits"
                End If
                If CountOfRecords(sqlq, myconstr, myconprv) = "0" Then
                    Dim adminemail = ConfigurationManager.AppSettings("supportemail").ToString
                    Dim unit As String = ConfigurationManager.AppSettings("unit").ToString
                    Dim superpass As String = ConfigurationManager.AppSettings("superpass").ToString
                    If myconprv = "MySql.Data.MySqlClient" Then
                        sqlq = "INSERT INTO `" & db & "`.`ourpermits` ([Access],[PERMIT],[NetId],[localpass],[Name],[Unit],[Application],[RoleApp],[Group1],[Group2],[Group3],[Comments],[StartDate],[EndDate],[Email]) "
                        sqlq = sqlq & " VALUES ('DEV','friendly','super','" & superpass & "','super','" & unit & "','InteractiveReporting','super','','','','Only for install','" & Format(DateTime.Now, "yyyy-MM-dd HH:mm:00") & "','2100-12-31 23:59:00','" & adminemail & "')"

                    ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                        sqlq = "INSERT INTO OURPERMITS (""Access"",PERMIT,NetId,localpass,Name,Unit,Application,RoleApp,Group1,Group2,Group3,Comments,StartDate,EndDate,Email) "
                        sqlq = sqlq & " VALUES ('DEV','friendly','super','" & superpass & "','super','" & unit & "','InteractiveReporting','super','','','','Only for install','" & DateToStringFormat(Now, myconprv, "dd-MMM-yyyy") & "','" & DateToStringFormat(CDate(DateAndTime.DateAdd(DateInterval.Day, 3650, Now())), myconprv, "dd-MMM-yyyy") & "','" & adminemail & "')"

                    ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                        sqlq = "INSERT INTO `OURPermits` ([Access],[PERMIT],[NetId],[localpass],[Name],[Unit],[Application],[RoleApp],[Group1],[Group2],[Group3],[Comments],[StartDate],[EndDate],[Email]) "
                        sqlq = sqlq & " VALUES ('DEV','friendly','super','" & superpass & "','super','" & unit & "','InteractiveReporting','super','','','','Only for install','" & Format(DateTime.Now, "yyyy-MM-dd HH:mm:00") & "','2100-12-31 23:59:00','" & adminemail & "')"

                    Else
                        sqlq = "INSERT INTO [OURPERMITS] ([Access],[PERMIT],[NetId],[localpass],[Name],[Unit],[Application],[RoleApp],[Group1],[Group2],[Group3],[Comments],[StartDate],[EndDate],[Email]) "
                        sqlq = sqlq & " VALUES ('DEV','friendly','super','" & superpass & "','super','" & unit & "','InteractiveReporting','super','','','','Only for install','" & DateToString(Now) & "','" & DateToString(CDate(DateAndTime.DateAdd(DateInterval.Day, 33333, Now()))) & "','" & adminemail & "')"
                    End If
                    ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                    If ret = "Query executed fine." Then ret = "OURPermits created"
                Else
                    'ret = "OURPermits creation crashed: " & ret
                End If
            End If
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function InstallOURAccessLog(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            If TableExists("OURAccessLog", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURAccessLog: " & UpdateOURAccessLog(myconstr, myconprv)

            Else
                'create table OURAccessLog
                If myconprv.StartsWith("InterSystems.Data.") Then
                    sqlq = "CREATE Table [OURAccessLog]("
                    sqlq = sqlq & "[ID] [Int] IDENTITY(1, 1) Not NULL,"
                    sqlq = sqlq & "[EventDate] [smalldatetime] NULL,"
                    sqlq = sqlq & "[Logon] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[""Count""] [Int] Not NULL,"
                    sqlq = sqlq & "[Action] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[Comments] [nvarchar](2500) NULL"
                    sqlq = sqlq & ""
                    sqlq = sqlq & ")"
                ElseIf myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "CREATE TABLE `" & db & "`.`ouraccesslog` ("
                    sqlq = sqlq & " `ID` int(11) NOT NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " `EventDate` datetime DEFAULT NULL,"
                    sqlq = sqlq & " `Logon` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `Count` int(11) NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " `Action` varchar(2500) DEFAULT NULL,"
                    sqlq = sqlq & " `Comments` varchar(2500) DEFAULT NULL,"
                    sqlq = sqlq & " PRIMARY KEY(`ID`)"
                    sqlq = sqlq & ");"

                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURACCESSLOG ("
                    sqlq = sqlq & "ID INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "EventDate TIMESTAMP(0),"
                    sqlq = sqlq & "Logon VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & """Count"" NUMBER(11,0) NOT NULL,"
                    sqlq = sqlq & "Action VARCHAR2(2500 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Comments VARCHAR2(2500 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "CONSTRAINT OURACCESSLOG_PK PRIMARY KEY(ID)"
                    sqlq = sqlq & ")"

                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURAccessLog]("
                    sqlq = sqlq & "[EventDate] date NULL,"
                    sqlq = sqlq & "[Logon] character varying(50) NULL,"
                    sqlq = sqlq & "[Count] integer NULL,"
                    sqlq = sqlq & "[Action] character varying(2500) NULL,"
                    sqlq = sqlq & "[Comments] character varying(2500) NULL,"
                    sqlq = sqlq & "[ID] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURAccessLog_pkey] PRIMARY KEY ([ID]))"

                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE Table [OURAccessLog]("
                    sqlq = sqlq & "[EventDate] [smalldatetime] NULL,"
                    sqlq = sqlq & "[Logon] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Count] [Int] Not NULL,"
                    sqlq = sqlq & "[Action] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[Comments] [nvarchar](2500) NULL,"
                    sqlq = sqlq & " [ID] INTEGER PRIMARY KEY AUTOINCREMENT)"

                Else 'Sql Server
                    sqlq = "CREATE Table [OURAccessLog]("
                    sqlq = sqlq & "[ID] [Int] IDENTITY(1, 1) Not NULL,"
                    sqlq = sqlq & "[EventDate] [smalldatetime] NULL,"
                    sqlq = sqlq & "[Logon] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Count] [Int] Not NULL,"
                    sqlq = sqlq & "[Action] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[Comments] [nvarchar](2500) NULL"
                    sqlq = sqlq & ""
                    sqlq = sqlq & ")"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "OURAccessLog created"
                Else
                    ret = "OURAccessLog creation crashed: " & ret
                End If

            End If

        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function InstallOURHelpDesk(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            If TableExists("OURHelpDesk", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURHelpDesk: " & UpdateOURHelpDesk(myconstr, myconprv)

            Else    'create table
                'HelpDesk
                If myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "CREATE TABLE `" & db & "`.`ourhelpdesk` ("
                    sqlq = sqlq & "`Start` varchar(50) Default NULL,"
                    sqlq = sqlq & "`Name` varchar(50) Default NULL,"
                    sqlq = sqlq & "`Ticket` varchar(2500) Default NULL,"
                    sqlq = sqlq & "`Deadline` varchar(50) Default NULL,"
                    sqlq = sqlq & "`Status` varchar(50) Default NULL,"
                    sqlq = sqlq & "`comments` varchar(2500) Default NULL,"
                    sqlq = sqlq & "`ToWhom` varchar(50) Default NULL,"
                    sqlq = sqlq & "`Version` varchar(45) Default NULL,"
                    sqlq = sqlq & " `Prop1` varchar(200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop2` varchar(200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop3` varchar(200) DEFAULT NULL,"
                    sqlq = sqlq & " `ID` int(11) Not NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " PRIMARY KEY (`ID`)"
                    sqlq = sqlq & ");"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURHELPDESK ("
                    sqlq = sqlq & """Start"" VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Name VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Ticket VARCHAR2(2500 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Deadline VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Status VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "comments VARCHAR2(2500 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ToWhom VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Version VARCHAR2(45 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop1 VARCHAR2(200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop2 VARCHAR2(200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop3 VARCHAR2(200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ID INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURHELPDESK_PK PRIMARY KEY (ID)"
                    sqlq = sqlq & ")"

                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURHelpDesk]("
                    sqlq = sqlq & "[Start] character varying(50) NULL,"
                    sqlq = sqlq & "[Name] character varying(50) NULL,"
                    sqlq = sqlq & "[Ticket] character varying(10000) NULL,"
                    sqlq = sqlq & "[Deadline] character varying(50) NULL,"
                    sqlq = sqlq & "[Status] character varying(50) NULL,"
                    sqlq = sqlq & "[comments] character varying(10000) NULL,"
                    sqlq = sqlq & "[ToWhom] character varying(50) NULL,"
                    sqlq = sqlq & "[Version] character varying(45) NULL,"
                    sqlq = sqlq & "[Prop1] character varying(200) NULL,"
                    sqlq = sqlq & "[Prop2] character varying(200) NULL,"
                    sqlq = sqlq & "[Prop3] character varying(200) NULL,"
                    sqlq = sqlq & "[ID] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURHelpDesk_pkey] PRIMARY KEY ([ID]))"

                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE TABLE [OURHelpDesk]("
                    sqlq = sqlq & "[Start] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Name] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Ticket] [nvarchar](2000) NULL,"
                    sqlq = sqlq & "[Deadline] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Status] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[comments] [nvarchar](2000) NULL,"
                    sqlq = sqlq & "[ToWhom] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Version] [nvarchar](45) NULL,"
                    sqlq = sqlq & "[Prop1] [nvarchar](200) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](200) NULL,"
                    sqlq = sqlq & "[Prop3] [nvarchar](200) NULL,"
                    sqlq = sqlq & " [ID] INTEGER PRIMARY KEY AUTOINCREMENT)"

                Else 'SQL Server or Cache
                    sqlq = "CREATE TABLE [OURHelpDesk]("
                    sqlq = sqlq & "[Start] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Name] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Ticket] [nvarchar](MAX) NULL,"
                    sqlq = sqlq & "[Deadline] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Status] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[comments] [nvarchar](MAX) NULL,"
                    sqlq = sqlq & "[ToWhom] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Version] [nvarchar](45) NULL,"
                    sqlq = sqlq & "[Prop1] [nvarchar](200) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](200) NULL,"
                    sqlq = sqlq & "[Prop3] [nvarchar](200) NULL,"
                    sqlq = sqlq & "[ID] [Int] IDENTITY(1, 1) Not NULL"
                    sqlq = sqlq & ")"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "OURHelpDesk created"
                Else
                    ret = "OURHelpDesk creation crashed: " & ret
                End If
            End If

            ''Update column changes or additions
            'If ret = "Table exists" OrElse ret = "OURHelpDesk created" Then
            '    If Not ColumnExists("OURHelpDesk", "Prop1", myconstr, myconprv) Then
            '        If myconprv = "MySql.Data.MySqlClient" Then
            '            sqlq = "ALTER TABLE `" & db & "`.`OURHelpDesk` ADD COLUMN `Prop1` VARCHAR(200) NULL DEFAULT NULL AFTER `Version`;"
            '        ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
            '            sqlq = "ALTER TABLE OURHelpDesk ADD Prop1 VARCHAR2(200 CHAR) DEFAULT NULL"
            '        ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
            '        Else
            '            sqlq = "ALTER TABLE [OURHelpDesk] ADD [Prop1] NVARCHAR(200) NULL DEFAULT NULL"
            '        End If
            '        ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
            '    End If
            '    If Not ColumnExists("OURHelpDesk", "Prop2", myconstr, myconprv) Then
            '        If myconprv = "MySql.Data.MySqlClient" Then
            '            sqlq = "ALTER TABLE `" & db & "`.`OURHelpDesk` ADD COLUMN `Prop2` VARCHAR(200) NULL DEFAULT NULL AFTER `Prop1`;"
            '        ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
            '            sqlq = "ALTER TABLE OURHelpDesk ADD Prop2 VARCHAR2(200 CHAR) DEFAULT NULL"
            '        ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
            '        Else
            '            sqlq = "ALTER TABLE [OURHelpDesk] ADD [Prop2] NVARCHAR(200) NULL DEFAULT NULL"
            '        End If
            '        ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
            '    End If
            '    If Not ColumnExists("OURHelpDesk", "Prop3", myconstr, myconprv) Then
            '        If myconprv = "MySql.Data.MySqlClient" Then
            '            sqlq = "ALTER TABLE `" & db & "`.`OURHelpDesk` ADD COLUMN `Prop3` VARCHAR(200) NULL DEFAULT NULL AFTER `Prop2`;"
            '        ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
            '            sqlq = "ALTER TABLE OURHelpDesk ADD Prop3 VARCHAR2(200 CHAR) DEFAULT NULL"
            '        ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
            '        Else
            '            sqlq = "ALTER TABLE [OURHelpDesk] ADD [Prop3] NVARCHAR(200) NULL DEFAULT NULL"
            '        End If
            '        ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
            '    End If

            '    'ALTER Table `ourdata2`.`ourhelpdesk` 
            '    'ADD COLUMN `Status` VARCHAR(50) NULL AFTER `Deadline`;
            '    'UPDATE ourdata2.ourhelpdesk SET `Status`=`Deadline`;

            'End If

        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function InstallOURDashboards(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            If TableExists("OURDashboards", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURDashboards: " & UpdateOURDashboards(myconstr, myconprv)

            ElseIf err = String.Empty Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "CREATE TABLE `" & db & "`.`ourdashboards` ("
                    sqlq = sqlq & "`UserID` VARCHAR(120) NULL DEFAULT NULL,"
                    sqlq = sqlq & "`Dashboard` VARCHAR(250) NULL DEFAULT NULL,"
                    sqlq = sqlq & "`ReportID` VARCHAR(250) NOT NULL DEFAULT  '0',"
                    sqlq = sqlq & "`ChartType` VARCHAR(120) NOT NULL DEFAULT  'PieChart',"
                    sqlq = sqlq & "`MapName` VARCHAR(250) NULL DEFAULT NULL,"
                    sqlq = sqlq & "`x1` VARCHAR(250) NULL DEFAULT NULL,"
                    sqlq = sqlq & "`x2` VARCHAR( 250 ) NULL DEFAULT NULL ,"
                    sqlq = sqlq & "`y1` VARCHAR( 250 ) NULL DEFAULT NULL ,"
                    sqlq = sqlq & "`y2` VARCHAR( 250 ) NULL DEFAULT NULL ,"
                    sqlq = sqlq & "`fn1` VARCHAR( 120 ) NULL DEFAULT NULL, "
                    sqlq = sqlq & "`fn2` VARCHAR( 120 ) NULL DEFAULT NULL ,"
                    sqlq = sqlq & "`WhereStm` VARCHAR( 2500 ) NULL DEFAULT NULL ,"
                    sqlq = sqlq & "`GraphTitle` longtext NULL DEFAULT NULL ,"
                    sqlq = sqlq & "`MapYesNo` VARCHAR( 20 ) NULL DEFAULT NULL ,"
                    sqlq = sqlq & "`Prop1` VARCHAR( 250 ) NULL DEFAULT NULL ,"
                    sqlq = sqlq & "`Prop2` VARCHAR( 250 ) NULL DEFAULT NULL ,"
                    sqlq = sqlq & "`Prop3` longtext NULL DEFAULT NULL ,"
                    sqlq = sqlq & "`Prop4` VARCHAR( 250 ) NULL DEFAULT NULL ,"
                    sqlq = sqlq & "`Prop5` longtext NULL DEFAULT NULL ,"
                    sqlq = sqlq & "`Prop6` VARCHAR( 250 ) NULL DEFAULT NULL ,"
                    sqlq = sqlq & "`ARR` longtext NULL DEFAULT NULL ,"
                    sqlq = sqlq & "`Indx` INT( 11 ) NOT NULL AUTO_INCREMENT ,"
                    sqlq = sqlq & "PRIMARY KEY (  `Indx` )"
                    sqlq = sqlq & ")"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURDASHBOARDS ("
                    sqlq = sqlq & "UserID VARCHAR2(120 CHAR ) DEFAULT NULL,"
                    sqlq = sqlq & "Dashboard VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ReportID VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ChartType VARCHAR2(120 CHAR) DEFAULT 'PieChart' NOT NULL,"
                    sqlq = sqlq & "MapName VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "x1 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "x2 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "y1 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "y2 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "fn1 VARCHAR2(120 CHAR) DEFAULT NULL ,"
                    sqlq = sqlq & "fn2 VARCHAR2(120 CHAR) DEFAULT NULL ,"
                    sqlq = sqlq & "WhereStm VARCHAR2(2500 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "GraphTitle VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "MapYesNo VARCHAR2(20 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop1 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop2 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop3 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop4 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop5 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop6 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ARR CLOB DEFAULT NULL,"
                    sqlq = sqlq & "Indx INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURDASHBOARDS_PK PRIMARY KEY (Indx)"
                    sqlq = sqlq & ")"

                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURDashboards]("
                    sqlq = sqlq & "[UserID] character varying(120) NULL,"
                    sqlq = sqlq & "[Dashboard] character varying(250) NULL,"
                    sqlq = sqlq & "[ReportID] character varying(250) NULL,"
                    sqlq = sqlq & "[ChartType] character varying(120) NOT NULL DEFAULT 'PieChart',"
                    sqlq = sqlq & "[MapName] character varying(250) NULL,"
                    sqlq = sqlq & "[x1] character varying(250) NULL,"
                    sqlq = sqlq & "[x2] character varying(250) NULL,"
                    sqlq = sqlq & "[y1] character varying(250) NULL,"
                    sqlq = sqlq & "[y2] character varying(250) NULL,"
                    sqlq = sqlq & "[fn1] character varying(120) NULL,"
                    sqlq = sqlq & "[fn2] character varying(120) NULL,"
                    sqlq = sqlq & "[WhereStm] character varying(2500) NULL,"
                    sqlq = sqlq & "[GraphTitle] character varying(250) NULL,"
                    sqlq = sqlq & "[MapYesNo] character varying(20) NULL,"
                    sqlq = sqlq & "[Prop1] character varying(250) NULL,"
                    sqlq = sqlq & "[Prop2] character varying(250) NULL,"
                    sqlq = sqlq & "[Prop3] character varying(250) NULL,"
                    sqlq = sqlq & "[Prop4] character varying(250) NULL,"
                    sqlq = sqlq & "[Prop5] character varying(250) NULL,"
                    sqlq = sqlq & "[Prop6] character varying(250) NULL,"
                    sqlq = sqlq & "[ARR] text NULL,"
                    sqlq = sqlq & "[Indx] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURDashboards_pkey] PRIMARY KEY ([Indx]))"

                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE TABLE  OURDashboards ("
                    sqlq = sqlq & "[UserID] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Dashboard] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[ReportID] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[ChartType] [nvarchar](120) NOT NULL DEFAULT 'PieChart',"
                    sqlq = sqlq & "[MapName] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[x1] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[x2] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[y1] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[y2] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[fn1] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[fn2] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[WhereStm] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[GraphTitle] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[MapYesNo] [nvarchar](20) NULL,"
                    sqlq = sqlq & "[Prop1] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop3] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop4] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop5] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop6] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[ARR] [text] NULL,"
                    sqlq = sqlq & " [Indx] INTEGER PRIMARY KEY AUTOINCREMENT)"
                Else
                    sqlq = "CREATE TABLE  OURDashboards ("
                    sqlq = sqlq & "[UserID] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Dashboard] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[ReportID] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[ChartType] [nvarchar](120) NOT NULL DEFAULT 'PieChart',"
                    sqlq = sqlq & "[MapName] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[x1] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[x2] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[y1] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[y2] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[fn1] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[fn2] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[WhereStm] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[GraphTitle] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[MapYesNo] [nvarchar](20) NULL,"
                    sqlq = sqlq & "[Prop1] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop3] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop4] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop5] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop6] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[ARR] [text] NULL,"
                    sqlq = sqlq & "[Indx] [Int] IDENTITY(1, 1) NOT NULL"
                    sqlq = sqlq & ")"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "OURDashboards created"
                Else
                    ret = "OURDashboards creation crashed: " & ret
                End If
            Else
                ret = err
            End If

        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function Installourkmlhistory(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            If TableExists("ourkmlhistory", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURkmlhistory: " & UpdateOURkmlhistory(myconstr, myconprv)

            Else
                'create table OURReportFormat
                Dim db As String = GetDataBase(myconstr, myconprv)
                If myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "CREATE TABLE `" & db & "`.`ourkmlhistory` ("
                    sqlq = sqlq & " `ReportId` varchar(250) NOT NULL,"
                    sqlq = sqlq & " `MapName` varchar(250) NOT NULL,"
                    sqlq = sqlq & " `MapType` varchar(150) NOT NULL,"
                    sqlq = sqlq & " `ShowPins` tinyint(1) NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " `ShowCircles` tinyint(1) NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " `ShowLinks` tinyint(1) NOT NULL DEFAULT '0',"

                    sqlq = sqlq & " `PlacemarkName` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `PlacemarkLon` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `PlacemarkLat` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `PlacemarkLonEnd` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `PlacemarkLatEnd` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `TimeStart` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `TimeEnd` varchar(50) DEFAULT NULL,"

                    sqlq = sqlq & " `InitAltit` int(11) NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " `LineWidth` int(11) NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " `ColorDensField` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `ColDensMultBy` decimal(10,4) NOT NULL DEFAULT '1',"
                    sqlq = sqlq & " `HighDensColor` varchar(50) DEFAULT NULL,"

                    sqlq = sqlq & " `ExtrAltField` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `ExtrAltMultBy` decimal(10,4) NOT NULL DEFAULT '1',"
                    sqlq = sqlq & " `ExtrAltColorField` varchar(50) DEFAULT NULL,"

                    sqlq = sqlq & " `KeyFields` varchar(2500) DEFAULT NULL,"
                    sqlq = sqlq & " `AddPointsFields` varchar(2500) DEFAULT NULL,"
                    sqlq = sqlq & " `Descriptions` varchar(2500) DEFAULT NULL,"

                    sqlq = sqlq & " `Prop1` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop2` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop3` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop4` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop5` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop6` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop7` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop8` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop9` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop10` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop11` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop12` varchar(250) DEFAULT NULL,"

                    sqlq = sqlq & " `PrmOrder` int(11) NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " `Comments` varchar(2500) DEFAULT NULL,"
                    sqlq = sqlq & " `KML` longtext DEFAULT NULL,"
                    sqlq = sqlq & " `Saved` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `Indx` int(11) NOT NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " PRIMARY KEY (`Indx`)"
                    sqlq = sqlq & ");"

                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURKMLHISTORY ("
                    sqlq = sqlq & "ReportId VARCHAR2(250 CHAR) NOT NULL,"
                    sqlq = sqlq & "MapName VARCHAR2(250 CHAR) NOT NULL,"
                    sqlq = sqlq & "MapType VARCHAR2(150 CHAR) NOT NULL,"
                    sqlq = sqlq & "ShowPins NUMBER(1,0) DEFAULT 0 NOT NULL,"
                    sqlq = sqlq & "ShowCircles NUMBER(1,0) DEFAULT 0 NOT NULL,"
                    sqlq = sqlq & "ShowLinks NUMBER(1,0) DEFAULT 0 NOT NULL,"
                    sqlq = sqlq & "PlacemarkName VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "PlacemarkLon VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "PlacemarkLat VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "PlacemarkLonEnd VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "PlacemarkLatEnd VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "TimeStart VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "TimeEnd VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "InitAltit NUMBER(11,0) DEFAULT 0 NOT NULL,"
                    sqlq = sqlq & "LineWidth NUMBER(11,0) DEFAULT 0 NOT NULL,"
                    sqlq = sqlq & "ColorDensField VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ColDensMultBy NUMBER(10,4) DEFAULT 1 NOT NULL,"
                    sqlq = sqlq & "HighDensColor VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ExtrAltField VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ExtrAltMultBy NUMBER(10,4) DEFAULT 1 NOT NULL,"
                    sqlq = sqlq & "ExtrAltColorField VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "KeyFields VARCHAR2(2500 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "AddPointsFields VARCHAR2(2500 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Descriptions VARCHAR2(2500 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop1 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop2 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop3 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop4 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop5 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop6 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop7 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop8 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop9 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop10 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop11 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop12 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "PrmOrder NUMBER(11,0) DEFAULT 0 NOT NULL,"
                    sqlq = sqlq & "Comments VARCHAR2(2500 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "KML CLOB DEFAULT NULL,"
                    sqlq = sqlq & "Saved VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Indx INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURKMLHISTORY_PK PRIMARY KEY (Indx)"
                    sqlq = sqlq & ")"

                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURKMLHistory]("
                    sqlq = sqlq & " [ReportId] character varying(250) NOT NULL,"
                    sqlq = sqlq & " [MapName] character varying(250) NOT NULL,"
                    sqlq = sqlq & " [MapType] character varying(150) NOT NULL,"
                    sqlq = sqlq & " [ShowPins] smallint NOT NULL DEFAULT 0,"
                    sqlq = sqlq & " [ShowCircles] smallint NOT NULL DEFAULT 0,"
                    sqlq = sqlq & " [ShowLinks] smallint NOT NULL DEFAULT 0,"

                    sqlq = sqlq & " [PlacemarkName] character varying(250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [PlacemarkLon] character varying(50) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [PlacemarkLat] character varying(50) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [PlacemarkLonEnd] character varying(50) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [PlacemarkLatEnd] character varying(50) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [TimeStart] character varying(50) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [TimeEnd] character varying(50) DEFAULT NULL,"

                    sqlq = sqlq & " [InitAltit] smallint NOT NULL DEFAULT 0,"
                    sqlq = sqlq & " [LineWidth] smallint NOT NULL DEFAULT 0,"
                    sqlq = sqlq & " [ColorDensField] character varying(250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [ColDensMultBy] decimal(10,4) NOT NULL DEFAULT 1,"
                    sqlq = sqlq & " [HighDensColor] character varying(50) NULL DEFAULT NULL,"

                    sqlq = sqlq & " [ExtrAltField] character varying(250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [ExtrAltMultBy] decimal(10,4) NOT NULL DEFAULT 1,"
                    sqlq = sqlq & " [ExtrAltColorField] character varying(50) NULL DEFAULT NULL,"

                    sqlq = sqlq & " [KeyFields] character varying(2500) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [AddPointsFields] character varying(2500) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Descriptions] character varying(2500) NULL DEFAULT NULL,"

                    sqlq = sqlq & " [Prop1] character varying(250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop2] character varying(250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop3] character varying(250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop4] character varying(250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop5] character varying(250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop6] character varying(250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop7] character varying(250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop8] character varying(250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop9] character varying(250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop10] character varying(250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop11] character varying(250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop12] character varying(250) NULL DEFAULT NULL,"

                    sqlq = sqlq & " [PrmOrder] smallint NOT NULL DEFAULT 0,"
                    sqlq = sqlq & " [Comments] character varying(2500) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [KML] text NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Saved] character varying(50)  NULL DEFAULT NULL,"

                    sqlq = sqlq & "[Indx] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURKMLHistory_pkey] PRIMARY KEY ([Indx]))"

                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE Table [OURKMLHistory]("
                    sqlq = sqlq & " [ReportId] [nvarchar](250) NOT NULL,"
                    sqlq = sqlq & " [MapName] [nvarchar](250) NOT NULL,"
                    sqlq = sqlq & " [MapType] [nvarchar](150) NOT NULL,"
                    sqlq = sqlq & " [ShowPins] [int] NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " [ShowCircles] [int] NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " [ShowLinks] [int] NOT NULL DEFAULT '0',"

                    sqlq = sqlq & " [PlacemarkName] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [PlacemarkLon] [nvarchar](50) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [PlacemarkLat] [nvarchar](50) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [PlacemarkLonEnd] [nvarchar](50) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [PlacemarkLatEnd] [nvarchar](50) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [TimeStart] [nvarchar](50) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [TimeEnd] [nvarchar](50) DEFAULT NULL,"

                    sqlq = sqlq & " [InitAltit] [int] NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " [LineWidth] [int] NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " [ColorDensField] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [ColDensMultBy] [decimal](10,4) NOT NULL DEFAULT '1',"
                    sqlq = sqlq & " [HighDensColor] [nvarchar](50) NULL DEFAULT NULL,"

                    sqlq = sqlq & " [ExtrAltField] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [ExtrAltMultBy] [decimal](10,4) NOT NULL DEFAULT '1',"
                    sqlq = sqlq & " [ExtrAltColorField] [nvarchar](50) NULL DEFAULT NULL,"

                    sqlq = sqlq & " [KeyFields] [nvarchar](2500) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [AddPointsFields] [nvarchar](2500) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Descriptions] [nvarchar](2500) NULL DEFAULT NULL,"

                    sqlq = sqlq & " [Prop1] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop2] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop3] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop4] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop5] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop6] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop7] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop8] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop9] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop10] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop11] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop12] [nvarchar](250) NULL DEFAULT NULL,"

                    sqlq = sqlq & " [PrmOrder] [int] NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " [Comments] [nvarchar](2500) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [KML] [text] NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Saved] [nvarchar](50)  NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Indx] INTEGER PRIMARY KEY AUTOINCREMENT)"

                Else 'Sql Server or Cache
                    sqlq = "CREATE Table [OURKMLHistory]("
                    sqlq = sqlq & " [ReportId] [nvarchar](250) NOT NULL,"
                    sqlq = sqlq & " [MapName] [nvarchar](250) NOT NULL,"
                    sqlq = sqlq & " [MapType] [nvarchar](150) NOT NULL,"
                    sqlq = sqlq & " [ShowPins] [int] NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " [ShowCircles] [int] NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " [ShowLinks] [int] NOT NULL DEFAULT '0',"

                    sqlq = sqlq & " [PlacemarkName] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [PlacemarkLon] [nvarchar](50) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [PlacemarkLat] [nvarchar](50) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [PlacemarkLonEnd] [nvarchar](50) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [PlacemarkLatEnd] [nvarchar](50) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [TimeStart] [nvarchar](50) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [TimeEnd] [nvarchar](50) DEFAULT NULL,"

                    sqlq = sqlq & " [InitAltit] [int] NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " [LineWidth] [int] NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " [ColorDensField] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [ColDensMultBy] [decimal](10,4) NOT NULL DEFAULT '1',"
                    sqlq = sqlq & " [HighDensColor] [nvarchar](50) NULL DEFAULT NULL,"

                    sqlq = sqlq & " [ExtrAltField] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [ExtrAltMultBy] [decimal](10,4) NOT NULL DEFAULT '1',"
                    sqlq = sqlq & " [ExtrAltColorField] [nvarchar](50) NULL DEFAULT NULL,"

                    sqlq = sqlq & " [KeyFields] [nvarchar](2500) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [AddPointsFields] [nvarchar](2500) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Descriptions] [nvarchar](2500) NULL DEFAULT NULL,"

                    sqlq = sqlq & " [Prop1] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop2] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop3] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop4] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop5] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop6] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop7] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop8] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop9] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop10] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop11] [nvarchar](250) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Prop12] [nvarchar](250) NULL DEFAULT NULL,"

                    sqlq = sqlq & " [PrmOrder] [int] NOT NULL DEFAULT '0',"
                    sqlq = sqlq & " [Comments] [nvarchar](2500) NULL DEFAULT NULL,"
                    sqlq = sqlq & " [KML] [text] NULL DEFAULT NULL,"
                    sqlq = sqlq & " [Saved] [nvarchar](50)  NULL DEFAULT NULL,"
                    sqlq = sqlq & "[Indx] [Int] IDENTITY(1, 1) Not NULL"
                    sqlq = sqlq & ")"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "ourkmlhistory created"
                Else
                    ret = "ourkmlhistory creation crashed: " & ret
                End If
            End If

        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UninstallOURTable(ByVal tbl As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        If myconstr = "" Then
            myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
        End If
        Try
            If TableExists(tbl, myconstr, myconprv, err) Then
                If err = "" Then
                    'delete 
                    Dim db As String = GetDataBase(myconstr, myconprv)
                    If myconprv = "MySql.Data.MySqlClient" Then
                        db = db.ToLower
                        sqlq = "DROP TABLE `" & db & "`.`" & tbl.ToLower & "`;"
                    ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                        sqlq = "DROP Table " & tbl.ToUpper & " CASCADE CONSTAINTS"
                    ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                        sqlq = "DROP TABLE [" & tbl & "];" 'CASCADE???
                    Else
                        sqlq = "DROP Table [" & tbl.ToUpper & "]"
                    End If
                    ret = ret & "<br/> " & " " & sqlq & "  " & ExequteSQLquery(sqlq, myconstr, myconprv)
                Else
                    ret = err
                End If
            End If
        Catch ex As Exception
            ret = ret & "<br/> " & sqlq & "  " & ex.Message
        End Try
        Return ret
    End Function
    'Public Function UninstallOURFiles(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
    '    Dim ret As String = String.Empty
    '    Dim err As String = String.Empty
    '    Dim sqlq As String = String.Empty
    '    If myconstr = "" Then
    '        myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
    '        myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
    '    End If
    '    Dim db As String = GetDataBase(myconstr, myconprv)
    '    Dim myView As DataView
    '    Try
    '        'delete ourfiles
    '        If myconprv = "MySql.Data.MySqlClient" Then
    '            sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" & db.ToLower & "' And TABLE_NAME='ourfiles'"
    '            myView = mRecords(sqlq, err, myconstr, myconprv)  'from OUR db
    '            If myView IsNot Nothing AndAlso myView.Table.Rows.Count = 1 Then
    '                sqlq = "DROP TABLE `" & db & "`.`ourfiles`;"
    '                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
    '                If ret = "Query executed fine." Then
    '                    ret = "<br/> " & " Table ourfiles deleted"
    '                Else
    '                    ret = "<br/> " & " Table ourfiles NOT deleted: " & ret
    '                End If
    '            End If
    '        ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
    '            'TODO !!!!!! for Oracle.ManagedDataAccess.Client

    '        Else
    '            If myconprv.StartsWith("InterSystems.Data.") Then
    '                sqlq = "Select ID As TABLE_NAME FROM %Dictionary.ClassDefinition WHERE Not (ID %STARTSWITH '%') AND ID='OURFiles'"
    '            Else
    '                sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='OURFiles'"
    '            End If
    '            myView = mRecords(sqlq, err, myconstr, myconprv)
    '            If myView IsNot Nothing AndAlso myView.Table.Rows.Count = 1 Then
    '                sqlq = "DROP Table [OURFiles]"
    '            End If
    '            ret = ret & "<br/> " & " " & sqlq & "  " & ExequteSQLquery(sqlq, myconstr, myconprv)
    '        End If
    '    Catch ex As Exception
    '        ret = ret & "<br/> " & sqlq & "  " & ex.Message
    '    End Try
    '    Return ret
    'End Function
    Public Function InstallOURFiles(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            If TableExists("OURFiles", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURFiles: " & UpdateOURFiles(myconstr, myconprv)

            ElseIf err = String.Empty Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "CREATE TABLE `" & db & "`.`ourfiles` ("
                    sqlq = sqlq & " `ID` int(11) NOT NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " PRIMARY KEY (`ID`),"
                    sqlq = sqlq & " `ReportId` varchar(250) DEFAULT NULL,"
                    sqlq = sqlq & " `Type` varchar(5) DEFAULT NULL,"
                    sqlq = sqlq & " `FileText` longtext DEFAULT NULL,"
                    sqlq = sqlq & " `Comments` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop1` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop2` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop3` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `Path` varchar(1024) DEFAULT NULL,"
                    sqlq = sqlq & " `UserFile` longtext DEFAULT NULL"
                    sqlq = sqlq & ");"

                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURFILES ("
                    sqlq = sqlq & "ID INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "ReportId VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Type VARCHAR2(5 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "FileText CLOB DEFAULT NULL,"
                    sqlq = sqlq & "Comments VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop1 VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop2 VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop3 VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Path VARCHAR2(1024 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "UserFile CLOB DEFAULT NULL,"
                    sqlq = sqlq & "CONSTRAINT OURFILES_PK PRIMARY KEY (ID)"
                    sqlq = sqlq & ")"

                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURFiles]("
                    sqlq = sqlq & " [ReportId] character varying(250) DEFAULT NULL,"
                    sqlq = sqlq & " [Type] character varying(5) DEFAULT NULL,"
                    sqlq = sqlq & " [FileText] text DEFAULT NULL,"
                    sqlq = sqlq & " [Comments] character varying(1200) DEFAULT NULL,"
                    sqlq = sqlq & " [Prop1] character varying(50) DEFAULT NULL,"
                    sqlq = sqlq & " [Prop2] character varying(50) DEFAULT NULL,"
                    sqlq = sqlq & " [Prop3] character varying(120) DEFAULT NULL,"
                    sqlq = sqlq & " [Path] character varying(1024) DEFAULT NULL,"
                    sqlq = sqlq & " [UserFile] text DEFAULT NULL,"
                    sqlq = sqlq & "[ID] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURFiles_pkey] PRIMARY KEY ([ID]))"

                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE TABLE [OURFiles] ("
                    sqlq = sqlq & " [ReportId] [nvarchar](250) DEFAULT NULL,"
                    sqlq = sqlq & " [Type] [nvarchar](5) DEFAULT NULL,"
                    sqlq = sqlq & " [FileText] [text] DEFAULT NULL,"
                    sqlq = sqlq & " [Comments] [nvarchar](1200) DEFAULT NULL,"
                    sqlq = sqlq & " [Prop1] [nvarchar](50) DEFAULT NULL,"
                    sqlq = sqlq & " [Prop2] [nvarchar](50) DEFAULT NULL,"
                    sqlq = sqlq & " [Prop3] [nvarchar](120) DEFAULT NULL,"
                    sqlq = sqlq & " [Path] [nvarchar](1024) DEFAULT NULL,"
                    sqlq = sqlq & " [UserFile] [text] DEFAULT NULL,"
                    sqlq = sqlq & " [ID] INTEGER PRIMARY KEY AUTOINCREMENT)"

                Else
                    sqlq = "CREATE TABLE [OURFiles] ("
                    sqlq = sqlq & " [ID] [Int] IDENTITY(1, 1) Not NULL PRIMARY KEY,"
                    sqlq = sqlq & " [ReportId] [nvarchar](250) DEFAULT NULL,"
                    sqlq = sqlq & " [Type] [nvarchar](5) DEFAULT NULL,"
                    sqlq = sqlq & " [FileText] [text] DEFAULT NULL,"
                    sqlq = sqlq & " [Comments] [nvarchar](1200) DEFAULT NULL,"
                    sqlq = sqlq & " [Prop1] [nvarchar](50) DEFAULT NULL,"
                    sqlq = sqlq & " [Prop2] [nvarchar](50) DEFAULT NULL,"
                    sqlq = sqlq & " [Prop3] [nvarchar](120) DEFAULT NULL,"
                    sqlq = sqlq & " [Path] [nvarchar](1024) DEFAULT NULL,"
                    sqlq = sqlq & " [UserFile] [text] DEFAULT NULL"
                    sqlq = sqlq & ")"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "OURFiles created"
                Else
                    ret = "OURFiles creation crashed: " & ret
                End If
            Else
                ret = err
            End If

        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    'Public Function UninstallOURFriendlyNames(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
    '    Dim ret As String = String.Empty
    '    Dim err As String = String.Empty
    '    Dim sqlq As String = String.Empty
    '    If myconstr = "" Then
    '        myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
    '        myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
    '    End If
    '    Dim db As String = GetDataBase(myconstr, myconprv)
    '    Dim myView As DataView
    '    Try
    '        'delete ourfriendlynames
    '        If myconprv = "MySql.Data.MySqlClient" Then
    '            sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" & db & "' And TABLE_NAME='ourfriendlynames'"
    '            myView = mRecords(sqlq, err, myconstr, myconprv)  'from OUR db
    '            If myView IsNot Nothing AndAlso myView.Table.Rows.Count = 1 Then
    '                sqlq = "DROP TABLE `" & db & "`.`ourfriendlynames`;"
    '                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
    '                If ret = "Query executed fine." Then
    '                    ret = "<br/> " & " Table ourfriendlynames deleted"
    '                Else
    '                    ret = "<br/> " & " Table ourfriendlynames NOT deleted: " & ret
    '                End If
    '            End If
    '        Else
    '            'TODO !!!!!! for Oracle.ManagedDataAccess.Client

    '            If myconprv.StartsWith("InterSystems.Data.") Then
    '                sqlq = "Select ID As TABLE_NAME FROM %Dictionary.ClassDefinition WHERE Not (ID %STARTSWITH '%') AND ID='OURFriendlyNames'"
    '            Else
    '                sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='OURFriendlyNames'"
    '            End If
    '            myView = mRecords(sqlq, err, myconstr, myconprv)
    '            If myView IsNot Nothing AndAlso myView.Table.Rows.Count = 1 Then
    '                sqlq = "DROP Table [OURFriendlyNames]"
    '            End If
    '            ret = ret & "<br/> " & " " & sqlq & "  " & ExequteSQLquery(sqlq, myconstr, myconprv)
    '        End If
    '    Catch ex As Exception
    '        ret = ret & "<br/> " & sqlq & "  " & ex.Message
    '    End Try
    '    Return ret
    'End Function
    Public Function InstallOURFriendlyNames(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            If TableExists("OURFriendlyNames", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURFriendlyNames: " & UpdateOURFriendlyNames(myconstr, myconprv)

            ElseIf err = String.Empty Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "CREATE TABLE `" & db & "`.`ourfriendlynames` ("
                    sqlq = sqlq & " `Unit` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `UnitDB` varchar(1024) DEFAULT NULL,"
                    sqlq = sqlq & " `TableName` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `FieldName` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `Friendly` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop1` varchar(20) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop2` varchar(20) DEFAULT NULL,"
                    sqlq = sqlq & " `Comments` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Indx` int(11) NOT NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " PRIMARY KEY (`Indx`)"
                    sqlq = sqlq & ");"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURFRIENDLYNAMES ("
                    sqlq = sqlq & "Unit VARCHAR2(120 CHAR) DEFAULT NULL ,"
                    sqlq = sqlq & "UnitDB VARCHAR2(1024 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "TableName VARCHAR2(240 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "FieldName VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Friendly VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop1 VARCHAR2(20 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop2 VARCHAR2(20 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Comments VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Indx INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURFRIENDLYNAMES_PK PRIMARY KEY (Indx)"
                    sqlq = sqlq & ")"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURFriendlyNames]("
                    sqlq = sqlq & "[Unit] character varying(120) NULL,"
                    sqlq = sqlq & "[UnitDB] character varying(1024) NULL,"
                    sqlq = sqlq & "[TableName] character varying(240) NULL,"
                    sqlq = sqlq & "[FieldName] character varying(120) NULL,"
                    sqlq = sqlq & "[Friendly] character varying(120) NULL,"
                    sqlq = sqlq & "[Prop1] character varying(20) NULL,"
                    sqlq = sqlq & "[Prop2] character varying(20) NULL,"
                    sqlq = sqlq & "[Comments] character varying(1200) NULL,"
                    sqlq = sqlq & "[Indx] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURFriendlyNames_pkey] PRIMARY KEY ([Indx]))"

                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE Table [OURFriendlyNames]("
                    sqlq = sqlq & "[Unit] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[UnitDB] [nvarchar](1024) NULL,"
                    sqlq = sqlq & "[TableName] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[FieldName] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Friendly] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Prop1] [nvarchar](20) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](20) NULL,"
                    sqlq = sqlq & "[Comments] [nvarchar](1200) NULL,"
                    sqlq = sqlq & " [Indx] INTEGER PRIMARY KEY AUTOINCREMENT)"

                Else
                    sqlq = "CREATE Table [OURFriendlyNames]("
                    sqlq = sqlq & "[Unit] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[UnitDB] [nvarchar](1024) NULL,"
                    sqlq = sqlq & "[TableName] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[FieldName] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Friendly] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Prop1] [nvarchar](20) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](20) NULL,"
                    sqlq = sqlq & "[Comments] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Indx] [Int] IDENTITY(1, 1) Not NULL PRIMARY KEY"
                    sqlq = sqlq & ")"
                End If

                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "OURFriendlyNames created"
                Else
                    ret = "OURFriendlyNames creation crashed: " & ret
                End If
            Else
                ret = err
            End If

        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function

    Public Function InstallOURUnits(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "", Optional ByVal clon As Boolean = True) As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            If TableExists("OURUnits", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURUnits: " & UpdateOURUnits(myconstr, myconprv)

            ElseIf err = String.Empty Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "CREATE TABLE `" & db & "`.`ourunits` ("
                    sqlq = sqlq & " `Unit` varchar(120) DEFAULT 'OUR',"
                    sqlq = sqlq & " `DistrMode` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `UnitWeb` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `OURConnStr` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `OURConnPrv` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `UserConnStr` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `UserConnPrv` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `StartDate` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `EndDate` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `Comments` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Indx` int(11) NOT NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " PRIMARY KEY (`Indx`)"
                    sqlq = sqlq & ");"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURUNITS ("
                    sqlq = sqlq & "Unit VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "DistrMode VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "UnitWeb VARCHAR2(240 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "OURConnStr VARCHAR2(240 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "OURConnPrv VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "UserConnStr VARCHAR2(240 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "UserConnPrv VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "StartDate VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "EndDate VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Comments VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Indx INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURUNITS_PK PRIMARY KEY (Indx)"
                    sqlq = sqlq & ")"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURUnits]("
                    sqlq = sqlq & "[Unit] character varying(120) Not NULL DEFAULT 'OUR',"
                    sqlq = sqlq & "[DistrMode] character varying(50) NULL,"
                    sqlq = sqlq & "[UnitWeb] character varying(240) NULL,"
                    sqlq = sqlq & "[OURConnStr] character varying(240) NULL,"
                    sqlq = sqlq & "[OURConnPrv] character varying(120) NULL,"
                    sqlq = sqlq & "[UserConnStr] character varying(240) NULL,"
                    sqlq = sqlq & "[UserConnPrv] character varying(120) NULL,"
                    sqlq = sqlq & "[StartDate] character varying(120) NULL,"
                    sqlq = sqlq & "[EndDate] character varying(120) NULL,"
                    sqlq = sqlq & "[Comments] character varying(1200) NULL,"
                    sqlq = sqlq & "[Indx] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURUnits_pkey] PRIMARY KEY ([Indx]))"
                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE TABLE [OURUnits]("
                    sqlq = sqlq & "[Unit] [nvarchar](120) Not NULL DEFAULT 'OUR',"
                    sqlq = sqlq & "[DistrMode] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[UnitWeb] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[OURConnStr] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[OURConnPrv] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[UserConnStr] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[UserConnPrv] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[StartDate] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[EndDate] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Comments] [nvarchar](1200) NULL,"
                    sqlq = sqlq & " [Indx] INTEGER PRIMARY KEY AUTOINCREMENT)"
                Else
                    sqlq = "CREATE TABLE [OURUnits]("
                    sqlq = sqlq & "[Unit] [nvarchar](120) Not NULL DEFAULT 'OUR',"
                    sqlq = sqlq & "[DistrMode] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[UnitWeb] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[OURConnStr] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[OURConnPrv] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[UserConnStr] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[UserConnPrv] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[StartDate] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[EndDate] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Comments] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Indx] [Int] IDENTITY(1, 1) Not NULL"
                    sqlq = sqlq & ")"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "OURUnits created"
                Else
                    ret = "OURUnits creation crashed: " & ret
                End If
            Else
                ret = err
            End If
            'Update column changes or additions
            If ret.StartsWith("Table exists") OrElse ret = "OURUnits created" Then
            'for clons add initial unit OUR
            'insert first record for super user
            If myconprv = "MySql.Data.MySqlClient" Then
                sqlq = "SELECT * FROM `" & db & "`.`OURUnits`"
            ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                'sqlq = "SELECT * FROM `" & db & "`.`OURUnits`"
                sqlq = "SELECT * FROM `OURUnits`"
            Else
                sqlq = "SELECT * FROM OURUnits"
            End If
            If clon OrElse CountOfRecords(sqlq, myconstr, myconprv) = "0" Then
                If ret = "Query executed fine." OrElse (TableExists("OURUnits", myconstr, myconprv, err) AndAlso CountOfRecords(sqlq, myconstr, myconprv) = "0") Then
                    ret = "OURUnits created"
                    Dim adminemail As String = ConfigurationManager.AppSettings("supportemail").ToString
                    Dim unit As String = ConfigurationManager.AppSettings("unit").ToString
                    Dim DistrMode As String = ConfigurationManager.AppSettings("webinstall").ToString
                    Dim UnitWeb As String = ConfigurationManager.AppSettings("weboureports").ToString
                    Dim OURConnPrv As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                    Dim OURConnStr As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString

                    If myconprv = "MySql.Data.MySqlClient" Then
                        sqlq = "INSERT INTO `" & db & "`.`OURUnits` ([Unit],[DistrMode],[UnitWeb],[OURConnStr],[OURConnPrv],[UserConnStr],[UserConnPrv],[Comments],[StartDate],[EndDate],[Email]) "
                        sqlq = sqlq & " VALUES ('" & unit & "','" & DistrMode & "','" & UnitWeb & "','" & OURConnStr & "','" & OURConnPrv & "','" & myconstr & "','" & myconprv & "','Only for install','" & Format(DateTime.Now, "yyyy-MM-dd HH:mm:00") & "','2100-12-31 23:59:00','" & adminemail & "')"

                    ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                        sqlq = "INSERT INTO OURUNITS (Unit,DistrMode,UnitWeb,OURConnStr,OURConnPrv,UserConnStr,UserConnPrv,Comments,StartDate,EndDate,Email) "
                        sqlq = sqlq & " VALUES ('" & unit & "','" & DistrMode & "','" & UnitWeb & "','" & OURConnStr & "','" & OURConnPrv & "','" & myconstr & "','" & myconprv & "','Only for install','" & DateToString(Now) & "','" & DateToString(CDate(DateAndTime.DateAdd(DateInterval.Day, 3650, Now()))) & "','" & adminemail & "')"

                    ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                        sqlq = "INSERT INTO `OURUnits` ([Unit],[DistrMode],[UnitWeb],[OURConnStr],[OURConnPrv],[UserConnStr],[UserConnPrv],[Comments],[StartDate],[EndDate],[Email]) "
                        sqlq = sqlq & " VALUES ('" & unit & "','" & DistrMode & "','" & UnitWeb & "','" & OURConnStr & "','" & OURConnPrv & "','" & myconstr & "','" & myconprv & "','Only for install','" & Format(DateTime.Now, "yyyy-MM-dd HH:mm:00") & "','2100-12-31 23:59:00','" & adminemail & "')"

                    Else
                        sqlq = "INSERT INTO [OURUNITS] ([Unit],[DistrMode],[UnitWeb],[OURConnStr],[OURConnPrv],[UserConnStr],[UserConnPrv],[Comments],[StartDate],[EndDate],[Email]) "
                        sqlq = sqlq & " VALUES ('" & unit & "','" & DistrMode & "','" & UnitWeb & "','" & OURConnStr & "','" & OURConnPrv & "','" & myconstr & "','" & myconprv & "','Only for install','" & DateToString(Now) & "','" & DateToString(CDate(DateAndTime.DateAdd(DateInterval.Day, 33333, Now()))) & "','" & adminemail & "')"
                    End If
                    ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                    If ret = "Query executed fine." Then ret = "OURUnits created"
                End If
            End If
            End If

        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function InstallOURAgents(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            If TableExists("OURAgents", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURAgents: " & UpdateOURAgents(myconstr, myconprv)

            ElseIf err = String.Empty Then
                Dim db As String = GetDataBase(myconstr, myconprv)
                If myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "CREATE TABLE `" & db & "`.`ouragents` ("
                    sqlq = sqlq & " `Name` varchar(200) DEFAULT NULL,"
                    sqlq = sqlq & " `Phone` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `Email` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `Address` varchar(1000) DEFAULT NULL,"
                    sqlq = sqlq & " `Comments` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop1` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop2` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop3` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `Indx` int(11) NOT NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " PRIMARY KEY (`Indx`)"
                    sqlq = sqlq & ");"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURAGENTS ("
                    sqlq = sqlq & "Name VARCHAR2(200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Phone VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Email VARCHAR2(240 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Address VARCHAR2(1000 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Comments VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop1 VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop2 VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop3 VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Indx INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURAGENTS_PK PRIMARY KEY (Indx)"
                    sqlq = sqlq & ")"

                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURAgents]("
                    sqlq = sqlq & "[Name] character varying(1200) Not NULL DEFAULT 'OUR',"
                    sqlq = sqlq & "[Phone] character varying(50) NULL,"
                    sqlq = sqlq & "[Email] character varying(240) NULL,"
                    sqlq = sqlq & "[Address] character varying(10000) NULL,"
                    sqlq = sqlq & "[Indx] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURAgents_pkey] PRIMARY KEY ([Indx]))"

                Else
                    sqlq = "CREATE TABLE [OURAgents]("
                    sqlq = sqlq & "[Name] [nvarchar](1200) Not NULL DEFAULT 'OUR',"
                    sqlq = sqlq & "[Phone] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Email] [nvarchar](240) NULL,"
                    If myconprv.StartsWith("InterSystems.Data.") Then
                        sqlq = sqlq & "[Address] [nvarchar](10000) NULL,"
                    Else
                        'in sql server the maximum literal number you can enter is 8000
                        'MAX allows up to 2 gb of data
                        sqlq = sqlq & "[Address] [nvarchar](MAX) NULL,"
                    End If

                    sqlq = sqlq & "[Comments] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop1] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Prop3] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Indx] [Int] IDENTITY(1, 1) Not NULL"
                    sqlq = sqlq & ")"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "OURAgents created"
                Else
                    ret = "OURAgents creation crashed: " & ret
                End If
            Else
                ret = err
            End If

        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function InstallOURActivity(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            If TableExists("OURActivity", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURActivity: " & UpdateOURActivity(myconstr, myconprv)

            ElseIf err = String.Empty Then
                Dim db As String = GetDataBase(myconstr, myconprv)
                If myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "CREATE TABLE `" & db & "`.`ouractivity` ("
                    sqlq = sqlq & " `Unit` int(11) DEFAULT NULL,"
                    sqlq = sqlq & " `Ticket` int(11) DEFAULT NULL,"
                    sqlq = sqlq & " `Activity` varchar(1000) DEFAULT NULL,"
                    sqlq = sqlq & " `ActivityType` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `ConnType` varchar(40) DEFAULT NULL,"
                    sqlq = sqlq & " `ConnOpen` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `ConnClosed` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `ConnectedBy` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `ConnectedServer` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `ConnectedDB` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Agent` int(11) DEFAULT NULL,"
                    sqlq = sqlq & " `AgentLevel` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `AgentPaid` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `Comments` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop1` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop2` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop3` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `Indx` int(11) NOT NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " PRIMARY KEY (`Indx`)"
                    sqlq = sqlq & ");"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURACTIVITY ("
                    sqlq = sqlq & "Unit NUMBER(11,0) DEFAULT NULL,"
                    sqlq = sqlq & "Ticket NUMBER(11,0) DEFAULT NULL,"
                    sqlq = sqlq & "Activity VARCHAR2(1000 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ActivityType VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ConnType VARCHAR2(40 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ConnOpen VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ConnClosed VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ConnectedBy VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ConnectedServer VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ConnectedDB VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Agent NUMBER(11,0) DEFAULT NULL,"
                    sqlq = sqlq & "AgentLevel VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "AgentPaid VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Comments VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop1 VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop2 VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop3 VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Indx INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURACTIVITY_PK PRIMARY KEY (Indx)"
                    sqlq = sqlq & ")"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURActivity]("
                    sqlq = sqlq & "[Unit] smallint NULL,"
                    sqlq = sqlq & "[Ticket] smallint NULL,"
                    sqlq = sqlq & "[Activity] character varying(1000) NULL,"
                    sqlq = sqlq & "[ActivityType] character varying(120) NULL,"
                    sqlq = sqlq & "[ConnType] character varying(40) NULL,"
                    sqlq = sqlq & "[ConnOpen] character varying(120) NULL,"
                    sqlq = sqlq & "[ConnClosed] character varying(120) NULL,"
                    sqlq = sqlq & "[ConnectedBy] character varying(120) NULL,"
                    sqlq = sqlq & "[ConnectedServer] character varying(1200) NULL,"
                    sqlq = sqlq & "[ConnectedDB] character varying(1200) NULL,"
                    sqlq = sqlq & "[Agent] smallint NULL,"
                    sqlq = sqlq & "[AgentLevel] character varying(120) NULL,"
                    sqlq = sqlq & "[AgentPaid] character varying(120) NULL,"
                    sqlq = sqlq & "[Comments] character varying(1200) NULL,"
                    sqlq = sqlq & "[Prop1] character varying(120) NULL,"
                    sqlq = sqlq & "[Prop2] character varying(120) NULL,"
                    sqlq = sqlq & "[Prop3] character varying(120) NULL,"
                    sqlq = sqlq & "[Indx] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURActivity_pkey] PRIMARY KEY ([Indx]))"
                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE TABLE [OURActivity]("
                    sqlq = sqlq & "[Unit] [Int] NULL,"
                    sqlq = sqlq & "[Ticket] [Int] NULL,"
                    sqlq = sqlq & "[Activity] [nvarchar](1000) NULL,"
                    sqlq = sqlq & "[ActivityType] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[ConnType] [nvarchar](40) NULL,"
                    sqlq = sqlq & "[ConnOpen] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[ConnClosed] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[ConnectedBy] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[ConnectedServer] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[ConnectedDB] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Agent] [Int] NULL,"
                    sqlq = sqlq & "[AgentLevel] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[AgentPaid] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Comments] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop1] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Prop3] [nvarchar](120) NULL,"
                    sqlq = sqlq & " [Indx] INTEGER PRIMARY KEY AUTOINCREMENT)"
                Else
                    sqlq = "CREATE TABLE [OURActivity]("
                    sqlq = sqlq & "[Unit] [Int] NULL,"
                    sqlq = sqlq & "[Ticket] [Int] NULL,"
                    sqlq = sqlq & "[Activity] [nvarchar](1000) NULL,"
                    sqlq = sqlq & "[ActivityType] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[ConnType] [nvarchar](40) NULL,"
                    sqlq = sqlq & "[ConnOpen] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[ConnClosed] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[ConnectedBy] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[ConnectedServer] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[ConnectedDB] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Agent] [Int] NULL,"
                    sqlq = sqlq & "[AgentLevel] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[AgentPaid] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Comments] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop1] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Prop3] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Indx] [Int] IDENTITY(1, 1) not NULL"
                    sqlq = sqlq & ")"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "OURActivity created"
                Else
                    ret = "OURActivity creation crashed: " & ret
                End If
            Else
                ret = err
            End If

        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function InstallOURTaskListSetting(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            If TableExists("ourtasklistsetting", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURTaskListSetting: " & UpdateOURTaskListSetting(myconstr, myconprv)

            Else
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "CREATE TABLE `" & db.ToLower & "`.`ourtasklistsetting` ("
                    sqlq = sqlq & " `Unit` int(11) DEFAULT NULL,"
                    sqlq = sqlq & " `UnitName` VARCHAR( 250 ) NULL DEFAULT NULL,"
                    sqlq = sqlq & " `FldText` VARCHAR( 50 ) NOT NULL,"
                    sqlq = sqlq & " `FldOrder` INT( 3 ) NOT NULL DEFAULT 0,"
                    sqlq = sqlq & " `FldColor` VARCHAR( 20 ) NULL DEFAULT NULL ,"
                    sqlq = sqlq & " `User` VARCHAR( 250 ) NULL DEFAULT NULL ,"
                    sqlq = sqlq & " `Prop1` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop2` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop3` varchar(120) DEFAULT NULL,"
                    sqlq = sqlq & " `Indx` int(11) NOT NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " PRIMARY KEY (`Indx`)"
                    sqlq = sqlq & ");"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURTASKLISTSETTING ("
                    sqlq = sqlq & "Unit NUMBER(11,0) DEFAULT NULL,"
                    sqlq = sqlq & "UnitName VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "FldText VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "FldOrder NUMBER(3,0) DEFAULT 0 NOT NULL,"
                    sqlq = sqlq & "FldColor VARCHAR2(20 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & """User"" VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop1 VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop2 VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop3 VARCHAR2(120 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Indx INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURTASKLISTSETTING_PK PRIMARY KEY (Indx)"
                    sqlq = sqlq & ")"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [ourtasklistsetting]("
                    sqlq = sqlq & "[Unit] smallint NOT NULL DEFAULT 0,"
                    sqlq = sqlq & "[UnitName] character varying(250) NULL,"
                    sqlq = sqlq & "[FldText] character varying(50) NOT NULL,"
                    sqlq = sqlq & "[FldOrder] smallint NOT NULL DEFAULT 0,"
                    sqlq = sqlq & "[FldColor] character varying(20) NULL,"
                    sqlq = sqlq & """User"" character varying(250) NULL,"
                    sqlq = sqlq & "[Prop1] character varying(120) NULL,"
                    sqlq = sqlq & "[Prop2] character varying(120) NULL,"
                    sqlq = sqlq & "[Prop3] character varying(120) NULL,"
                    sqlq = sqlq & "[Indx] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [ourtasklistsetting_pkey] PRIMARY KEY ([Indx]))"
                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE TABLE [ourtasklistsetting]("
                    sqlq = sqlq & "[Unit] [Int] NULL,"
                    sqlq = sqlq & "[UnitName] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[FldText] [nvarchar](50) NOT NULL,"
                    sqlq = sqlq & "[FldOrder] [Int] NOT NULL DEFAULT 0,"
                    sqlq = sqlq & "[FldColor] [nvarchar](20) NULL,"
                    sqlq = sqlq & "[User] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop1] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Prop3] [nvarchar](120) NULL,"
                    sqlq = sqlq & " [Indx] INTEGER PRIMARY KEY AUTOINCREMENT)"
                Else 'sql server or cache
                    sqlq = "CREATE TABLE [ourtasklistsetting]("
                    sqlq = sqlq & "[Unit] [Int] NULL,"
                    sqlq = sqlq & "[UnitName] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[FldText] [nvarchar](50) NOT NULL,"
                    sqlq = sqlq & "[FldOrder] [Int] NOT NULL DEFAULT 0,"
                    sqlq = sqlq & "[FldColor] [nvarchar](20) NULL,"
                    If myconprv.StartsWith("InterSystems.Data.") Then
                        sqlq = sqlq & """User"" [nvarchar](250) NULL,"
                    Else
                        sqlq = sqlq & "[User] [nvarchar](250) NULL,"
                    End If
                    sqlq = sqlq & "[Prop1] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Prop3] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[Indx] [Int] IDENTITY(1, 1) not NULL"
                    sqlq = sqlq & ")"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "ourtasklistsetting created"
                Else
                    ret = "ourtasklistsetting creation crashed: " & ret
                End If

            End If

        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function InstallOURUserTables(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "", Optional ByRef er As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            If TableExists("OURUserTables", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURUserTables: " & UpdateOURUserTables(myconstr, myconprv)

            ElseIf err = String.Empty Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = " CREATE TABLE `" & db.ToLower & "`.`ourusertables` ("
                    sqlq = sqlq & " `Unit` varchar(120) DEFAULT 'OUR',"
                    sqlq = sqlq & " `UserID` varchar(120) DEFAULT 'test',"
                    sqlq = sqlq & " `TableName` varchar(250) DEFAULT 'test',"
                    sqlq = sqlq & " `Group` varchar(120) NULL,"
                    sqlq = sqlq & " `UserDB` varchar(240) NULL,"
                    sqlq = sqlq & " `Prop1` varchar(250) NULL,"
                    sqlq = sqlq & " `Prop2` varchar(250) NULL,"
                    sqlq = sqlq & " `Prop3` varchar(250) NULL,"
                    sqlq = sqlq & " `Comments` varchar(1200) NULL,"
                    sqlq = sqlq & " `Indx` bigint(11) Not NULL AUTO_INCREMENT, PRIMARY KEY(`Indx`)"
                    sqlq = sqlq & " );"

                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURUSERTABLES ("
                    sqlq = sqlq & "Unit VARCHAR2(120 CHAR ) DEFAULT 'OUR' NOT NULL,"
                    sqlq = sqlq & "UserID VARCHAR2(120 CHAR ) DEFAULT 'test' NOT NULL,"
                    sqlq = sqlq & "TableName VARCHAR2(250 CHAR ) DEFAULT 'test' NOT NULL,"
                    sqlq = sqlq & """Group"" VARCHAR2(120 CHAR ) DEFAULT NULL,"
                    sqlq = sqlq & "UserDB VARCHAR2(240 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop1 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop2 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop3 VARCHAR2(250 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Comments VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Indx INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURUSERTABLES_PK PRIMARY KEY (Indx)"
                    sqlq = sqlq & ")"

                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURUserTables]("
                    sqlq = sqlq & "[Unit] character varying(120) Not NULL DEFAULT 'OUR',"
                    sqlq = sqlq & "[UserID] character varying(120) Not NULL DEFAULT 'csvtest',"
                    sqlq = sqlq & "[TableName] character varying(250) Not NULL DEFAULT 'csvtest',"
                    sqlq = sqlq & """Group"" character varying(120) NULL,"
                    sqlq = sqlq & "[OURConnPrv] character varying(120) NULL,"
                    sqlq = sqlq & "[UserDB] character varying(240) NULL,"
                    sqlq = sqlq & "[Prop1] character varying(250) NULL,"
                    sqlq = sqlq & "[Prop2] character varying(250) NULL,"
                    sqlq = sqlq & "[Prop3] character varying(250) NULL,"
                    sqlq = sqlq & "[Comments] character varying(1200) NULL,"
                    sqlq = sqlq & "[Indx] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURUserTables_pkey] PRIMARY KEY ([Indx]))"

                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE TABLE [OURUserTables]("
                    sqlq = sqlq & "[Unit] [nvarchar](120) DEFAULT 'OUR',"
                    sqlq = sqlq & "[UserID] [nvarchar](120)  DEFAULT 'csvtest',"
                    sqlq = sqlq & "[TableName] [nvarchar](250) DEFAULT 'csvtest',"
                    sqlq = sqlq & FixReservedWords("Group", myconprv) & " [nvarchar](120) NULL,"
                    sqlq = sqlq & "[OURConnPrv] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[UserDB] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[Prop1] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop3] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Comments] [nvarchar](1200) NULL,"
                    sqlq = sqlq & " [Indx] INTEGER PRIMARY KEY AUTOINCREMENT)"

                Else
                    sqlq = "CREATE TABLE [OURUserTables]("
                    sqlq = sqlq & "[Unit] [nvarchar](120) Not NULL DEFAULT 'OUR',"
                    sqlq = sqlq & "[UserID] [nvarchar](120) Not NULL DEFAULT 'csvtest',"
                    sqlq = sqlq & "[TableName] [nvarchar](250) Not NULL DEFAULT 'csvtest',"
                    sqlq = sqlq & FixReservedWords("Group", myconprv) & " [nvarchar](120) NULL,"
                    sqlq = sqlq & "[OURConnPrv] [nvarchar](120) NULL,"
                    sqlq = sqlq & "[UserDB] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[Prop1] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Prop3] [nvarchar](250) NULL,"
                    sqlq = sqlq & "[Comments] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Indx] [Int] IDENTITY(1, 1) Not NULL"
                    sqlq = sqlq & ")"
                End If
                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "Query executed fine." Then
                    ret = "OURUserTables created"
                Else
                    ret = "OURUserTables creation crashed: " & ret
                End If
            Else
                ret = err
            End If

        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function

    Public Function InstallOURScheduledDownloads(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "", Optional ByRef er As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            If TableExists("OURScheduledDownloads", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURScheduledDownloads: " & UpdateOURScheduledDownloads(myconstr, myconprv)

            ElseIf err = String.Empty Then
                'create table ourscheduleddownloads
                If myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "CREATE TABLE `" & db & "`.`ourscheduleddownloads` ("
                    sqlq = sqlq & " `StartDate` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `UserId` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `URL` varchar(2500) DEFAULT NULL,"
                    sqlq = sqlq & " `Deadline` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `ToWhom` varchar(2500) DEFAULT NULL,"
                    sqlq = sqlq & " `Status` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop1` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop2` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop3` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop4` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop5` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop6` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `ID` int(11) NOT NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " PRIMARY KEY (`ID`)"
                    sqlq = sqlq & ");"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURSCHEDULEDDOWNLOADS ("
                    sqlq = sqlq & "StartDate VARCHAR2(50 CHAR ) DEFAULT NULL,"
                    sqlq = sqlq & "UserID VARCHAR2(240 CHAR ) DEFAULT NULL,"
                    sqlq = sqlq & "URL VARCHAR2(2500 CHAR ) DEFAULT NULL,"
                    sqlq = sqlq & "Deadline VARCHAR2(50 CHAR ) DEFAULT NULL,"
                    sqlq = sqlq & "ToWhom VARCHAR2(2500 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Status VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop1 VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop2 VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop3 VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop4 VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop5 VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop6 VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ID INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURSCHEDULEDDOWNLOADS_PK PRIMARY KEY (ID)"
                    sqlq = sqlq & ")"

                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURScheduledDownloads]("
                    sqlq = sqlq & "[StartDate] character varying(50) NULL,"
                    sqlq = sqlq & "[UserID] character varying(240) NULL,"
                    sqlq = sqlq & "[URL] character varying(2500) NULL,"
                    sqlq = sqlq & "[Deadline] character varying(50) NULL,"
                    sqlq = sqlq & "[ToWhom] character varying(2500) NULL,"
                    sqlq = sqlq & "[Status] character varying(50) NULL,"
                    sqlq = sqlq & "[Prop1] character varying(1200) NULL,"
                    sqlq = sqlq & "[Prop2] character varying(1200) NULL,"
                    sqlq = sqlq & "[Prop3] character varying(1200) NULL,"
                    sqlq = sqlq & "[Prop4] character varying(1200) NULL,"
                    sqlq = sqlq & "[Prop5] character varying(1200) NULL,"
                    sqlq = sqlq & "[Prop6] character varying(1200) NULL,"
                    sqlq = sqlq & "[ID] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURScheduledDownloads_pkey] PRIMARY KEY ([ID]))"

                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE TABLE [OURScheduledDownloads]("
                    sqlq = sqlq & "[StartDate] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[UserID] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[URL] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[Deadline] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[ToWhom] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[Status] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Prop1] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop3] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop4] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop5] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop6] [nvarchar](1200) NULL,"
                    sqlq = sqlq & " [ID] INTEGER PRIMARY KEY AUTOINCREMENT)"

                Else
                    sqlq = "CREATE TABLE [OURScheduledDownloads]("
                    sqlq = sqlq & "[StartDate] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[UserID] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[URL] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[Deadline] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[ToWhom] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[Status] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Prop1] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop3] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop4] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop5] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop6] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[ID] [Int] IDENTITY(1, 1) Not NULL"
                    sqlq = sqlq & ")"

                End If

                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "" OrElse ret = "Query executed fine." Then
                    ret = "OURScheduledDownloads created"
                Else
                    ret = "OURScheduledDownloads creation crashed: " & ret
                End If
            Else
                ret = err
            End If
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function

    Public Function InstallOURScheduledImports(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "", Optional ByRef er As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)
            If TableExists("OURScheduledImports", myconstr, myconprv, err) Then
                ret = "Table exists"
                'Update table structure
                ret = ret & "<br/>Update OURScheduledImports: " & UpdateOURScheduledImports(myconstr, myconprv)

            ElseIf err = String.Empty Then
                'create table ourscheduledimports
                If myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "CREATE TABLE `" & db & "`.`ourscheduledimports` ("
                    sqlq = sqlq & " `StartDate` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `UserId` varchar(240) DEFAULT NULL,"
                    sqlq = sqlq & " `URL` varchar(2500) DEFAULT NULL,"
                    sqlq = sqlq & " `TableName` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Deadline` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `ToWhom` varchar(2500) DEFAULT NULL,"
                    sqlq = sqlq & " `Status` varchar(50) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop1` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop2` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop3` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop4` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop5` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop6` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop7` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `Prop8` varchar(1200) DEFAULT NULL,"
                    sqlq = sqlq & " `ID` int(11) NOT NULL AUTO_INCREMENT,"
                    sqlq = sqlq & " PRIMARY KEY (`ID`)"
                    sqlq = sqlq & ");"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "CREATE TABLE OURSCHEDULEDIMPORTS ("
                    sqlq = sqlq & "StartDate VARCHAR2(50 CHAR ) DEFAULT NULL,"
                    sqlq = sqlq & "UserID VARCHAR2(240 CHAR ) DEFAULT NULL,"
                    sqlq = sqlq & "URL VARCHAR2(2500 CHAR ) DEFAULT NULL,"
                    sqlq = sqlq & "TableName VARCHAR2(1200 CHAR ) DEFAULT NULL,"
                    sqlq = sqlq & "Deadline VARCHAR2(50 CHAR ) DEFAULT NULL,"
                    sqlq = sqlq & "ToWhom VARCHAR2(2500 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Status VARCHAR2(50 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop1 VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop2 VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop3 VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop4 VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop5 VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop6 VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop7 VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "Prop8 VARCHAR2(1200 CHAR) DEFAULT NULL,"
                    sqlq = sqlq & "ID INTEGER GENERATED ALWAYS AS IDENTITY,"
                    sqlq = sqlq & "CONSTRAINT OURSCHEDULEDIMPORTS_PK PRIMARY KEY (ID)"
                    sqlq = sqlq & ")"

                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "CREATE Table [OURScheduledImports]("
                    sqlq = sqlq & "[StartDate] character varying(50) NULL,"
                    sqlq = sqlq & "[UserID] character varying(240) NULL,"
                    sqlq = sqlq & "[URL] character varying(2500) NULL,"
                    sqlq = sqlq & "[TableName] character varying(1200) NULL,"
                    sqlq = sqlq & "[Deadline] character varying(50) NULL,"
                    sqlq = sqlq & "[ToWhom] character varying(2500) NULL,"
                    sqlq = sqlq & "[Status] character varying(50) NULL,"
                    sqlq = sqlq & "[Prop1] character varying(1200) NULL,"
                    sqlq = sqlq & "[Prop2] character varying(1200) NULL,"
                    sqlq = sqlq & "[Prop3] character varying(1200) NULL,"
                    sqlq = sqlq & "[Prop4] character varying(1200) NULL,"
                    sqlq = sqlq & "[Prop5] character varying(1200) NULL,"
                    sqlq = sqlq & "[Prop6] character varying(1200) NULL,"
                    sqlq = sqlq & "[Prop7] character varying(1200) NULL,"
                    sqlq = sqlq & "[Prop8] character varying(1200) NULL,"
                    sqlq = sqlq & "[ID] integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),"
                    sqlq = sqlq & "CONSTRAINT [OURScheduledImports_pkey] PRIMARY KEY ([ID]))"

                ElseIf myconprv = "Sqlite" Then   'in memory
                    sqlq = "CREATE TABLE [OURScheduledImports]("
                    sqlq = sqlq & "[StartDate] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[UserID] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[URL] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[TableName] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Deadline] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[ToWhom] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[Status] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Prop1] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop3] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop4] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop5] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop6] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop7] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop8] [nvarchar](1200) NULL,"
                    sqlq = sqlq & " [ID] INTEGER PRIMARY KEY AUTOINCREMENT)"

                Else
                    sqlq = "CREATE TABLE [OURScheduledImports]("
                    sqlq = sqlq & "[StartDate] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[UserID] [nvarchar](240) NULL,"
                    sqlq = sqlq & "[URL] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[TableName] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Deadline] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[ToWhom] [nvarchar](2500) NULL,"
                    sqlq = sqlq & "[Status] [nvarchar](50) NULL,"
                    sqlq = sqlq & "[Prop1] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop2] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop3] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop4] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop5] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop6] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop7] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[Prop8] [nvarchar](1200) NULL,"
                    sqlq = sqlq & "[ID] [Int] IDENTITY(1, 1) Not NULL"
                    sqlq = sqlq & ")"

                End If

                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret = "" OrElse ret = "Query executed fine." Then
                    ret = "OURScheduledImports created"
                Else
                    ret = "OURScheduledImports creation crashed: " & ret
                End If
            Else
                ret = err
            End If
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function

    Public Function UpdateOURFriendlyNames(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURFiles(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURUnits(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If Not ColumnExists("OurUnits", "Official", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourunits` ADD COLUMN `Official` VARCHAR(200) NULL DEFAULT NULL AFTER `Comments`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURUNITS ADD Official VARCHAR2(200 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURUnits` ADD COLUMN `Official` character varying(200) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURUnits] ADD [Official] NVARCHAR(200) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'Official' added. "
            End If
            If Not ColumnExists("OurUnits", "Address", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourunits` ADD COLUMN `Address` VARCHAR(1000) NULL DEFAULT NULL AFTER `Official`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURUNITS ADD Address VARCHAR2(1000 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURUnits` ADD COLUMN `Address` character varying(1000) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURUnits] ADD [Address] NVARCHAR(1000) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'Address' added. "
            End If

            If Not ColumnExists("OurUnits", "Phone", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourunits` ADD COLUMN `Phone` VARCHAR(100) NULL DEFAULT NULL AFTER `Address`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURUNITS ADD Phone VARCHAR2(100 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURUnits` ADD COLUMN `Phone` character varying(100) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURUnits] ADD [Phone] NVARCHAR(100) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'Phone' added. "
            End If
            If Not ColumnExists("OurUnits", "Email", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourunits` ADD COLUMN `Email` VARCHAR(200) NULL DEFAULT NULL AFTER `Phone`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURUNITS ADD Email VARCHAR2(200 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURUnits` ADD COLUMN `Email` character varying(200) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURUnits] ADD [Email] NVARCHAR(200) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'Email' added. "
            End If
            If Not ColumnExists("OurUnits", "Agent", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourunits` ADD COLUMN `Agent` VARCHAR(200) NULL DEFAULT NULL AFTER `Email`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURUNITS ADD Agent VARCHAR2(200 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURUnits` ADD COLUMN `Agent` character varying(200) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURUnits] ADD [Agent] NVARCHAR(200) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'Agent' added. "
            End If
            If Not ColumnExists("OurUnits", "Prop1", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourunits` ADD COLUMN `Prop1` VARCHAR(200) NULL DEFAULT NULL AFTER `Agent`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURUNITS ADD Prop1 VARCHAR2(200 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURUnits` ADD COLUMN `Prop1` character varying(200) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURUnits] ADD [Prop1] NVARCHAR(200) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'Prop1' added. "
            End If
            If Not ColumnExists("OurUnits", "Prop2", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourunits` ADD COLUMN `Prop2` VARCHAR(200) NULL DEFAULT NULL AFTER `Prop1`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURUNITS ADD Prop2 VARCHAR2(200 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURUnits` ADD COLUMN `Prop2` character varying(200) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURUnits] ADD [Prop2] NVARCHAR(200) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'Prop2' added. "
            End If
            If Not ColumnExists("OurUnits", "Prop3", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourunits` ADD COLUMN `Prop3` VARCHAR(200) NULL DEFAULT NULL AFTER `Prop2`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURUNITS ADD Prop3 VARCHAR2(200 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURUnits` ADD COLUMN `Prop3` character varying(200) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURUnits] ADD [Prop3] NVARCHAR(200) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'Prop3' added."
            End If

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = " < br /> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURHelpDesk(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURAccessLog(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)


            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURPermits(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURPermissions(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURReportInfo(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURReportItems(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If Not ColumnExists("OurReportItems", "ImagePath", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportitems` ADD COLUMN `ImagePath` VARCHAR(255) NULL DEFAULT NULL AFTER `ReportItemType`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTITEMS ADD ImagePath VARCHAR2(255 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportItems` ADD COLUMN `ImagePath` character varying(255) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURReportItems] ADD [ImagePath] NVARCHAR(255) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'ImagePath' added. "
                End If
            End If

            If Not ColumnExists("OurReportItems", "ImageHeight", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportitems` ADD COLUMN `ImageHeight` VARCHAR(5) NULL DEFAULT NULL AFTER `ImagePath`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTITEMS ADD `ImageHeight` VARCHAR2(5 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportItems` ADD COLUMN `ImageHeight` character varying(5) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURReportItems] ADD [ImageHeight] NVARCHAR(5) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'ImageHeight' added. "
                End If
            End If
            If Not ColumnExists("OurReportItems", "ImageWidth", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportitems` ADD COLUMN `ImageWidth` VARCHAR(5) NULL DEFAULT NULL AFTER `ImageHeight`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTITEMS ADD `ImageWidth` VARCHAR2(5 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportItems` ADD COLUMN `ImageWidth` character varying(5) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURReportItems] ADD [ImageWidth] NVARCHAR(5) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv)
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'ImageWidth' added. "
                End If
            End If


            If Not ColumnExists("OurReportItems", "TabularColumnWidth", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportitems` ADD COLUMN `TabularColumnWidth` VARCHAR(15) NULL DEFAULT NULL AFTER `ItemID`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTITEMS ADD TabularColumnWidth VARCHAR2(15 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportItems` ADD COLUMN `TabularColumnWidth` character varying(15) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURReportItems] ADD [TabularColumnWidth] NVARCHAR(15) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'TabularColumnWidth' added. "
                End If
            End If
            If Not ColumnExists("OurReportItems", "CaptionHeight", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportitems` ADD COLUMN `CaptionHeight` VARCHAR(15) NULL DEFAULT NULL AFTER `Caption`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTITEMS ADD CaptionHeight VARCHAR2(15 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportItems` ADD COLUMN `CaptionHeight` character varying(15) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURReportItems] ADD [CaptionHeight] NVARCHAR(15) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'CaptionHeight' added. "
                End If
            End If
            If Not ColumnExists("OurReportItems", "CaptionWidth", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportitems` ADD COLUMN `CaptionWidth` VARCHAR(15) NULL DEFAULT NULL AFTER `CaptionHeight`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTITEMS ADD CaptionWidth VARCHAR2(15 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportItems` ADD COLUMN `CaptionWidth` character varying(15) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURReportItems] ADD [CaptionWidth] NVARCHAR(15) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'CaptionWidth' added. "
                End If
            End If
            If Not ColumnExists("OurReportItems", "CaptionX", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportitems` ADD COLUMN `CaptionX` INT(4) NULL DEFAULT NULL AFTER `CaptionWidth`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTITEMS ADD CaptionX NUMBER(4,0) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportItems` ADD COLUMN `CaptionX` smallint NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURReportItems] ADD [CaptionX] [int] NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'CaptionX' added. "
                End If
            End If
            If Not ColumnExists("OurReportItems", "CaptionY", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportitems` ADD COLUMN `CaptionY` INT(4) NULL DEFAULT NULL AFTER `CaptionX`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTITEMS ADD CaptionY NUMBER(4,0) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportItems` ADD COLUMN `CaptionY` smallint NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURReportItems] ADD [CaptionY] [int] NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'CaptionY' added. "
                End If
            End If

            If Not ColumnExists("OurReportItems", "CaptionTextAlign", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportitems` ADD COLUMN `CaptionTextAlign` VARCHAR(10) NOT NULL DEFAULT 'Left' AFTER `CaptionStrikeout`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTITEMS ADD CaptionTextAlign VARCHAR2(10 CHAR) DEFAULT 'Left'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportItems` ADD COLUMN `CaptionTextAlign` character varying(10) NOT NULL DEFAULT 'Left';"
                Else
                    sqlq = "ALTER TABLE [OURReportItems] ADD [CaptionTextAlign] NVARCHAR(10) NOT NULL DEFAULT 'Left'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'CaptionTextAlign' added. "
                End If
            End If

            If Not ColumnExists("OurReportItems", "DataHeight", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportitems` ADD COLUMN `DataHeight` VARCHAR(15) NULL DEFAULT NULL AFTER `ItemOrder`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTITEMS ADD DataHeight VARCHAR2(15 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportItems` ADD COLUMN `DataHeight` character varying(15) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURReportItems] ADD [DataHeight] NVARCHAR(15) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'DataHeight' added. "
                End If
            End If
            If Not ColumnExists("OurReportItems", "DataWidth", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportitems` ADD COLUMN `DataWidth` VARCHAR(15) NULL DEFAULT NULL AFTER `DataHeight`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTITEMS ADD DataWidth VARCHAR2(15 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportItems` ADD COLUMN `DataWidth` character varying(15) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURReportItems] ADD [DataWidth] NVARCHAR(15) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'DataWidth' added. "
                End If
            End If
            If Not ColumnExists("OurReportItems", "DataX", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportitems` ADD COLUMN `DataX` INT(4) NULL DEFAULT NULL AFTER `DataWidth`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTITEMS ADD DataX NUMBER(4,0) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportItems` ADD COLUMN `DataX` smallint NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURReportItems] ADD [DataX] [int] NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'DataX' added. "
                End If
            End If
            If Not ColumnExists("OurReportItems", "DataY", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportitems` ADD COLUMN `DataY` INT(4) NULL DEFAULT NULL AFTER `DataX`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTITEMS ADD DataY NUMBER(4,0) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportItems` ADD COLUMN `DataY` smallint NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURReportItems] ADD [DataY] [int] NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'DataY' added. "
                End If
            End If
            If Not ColumnExists("OurReportItems", "TextAlign", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportitems` ADD COLUMN `TextAlign` VARCHAR(10) NOT NULL DEFAULT 'Left' AFTER `Strikeout`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTITEMS ADD TextAlign VARCHAR2(10 CHAR) DEFAULT 'Left'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportItems` ADD COLUMN `TextAlign` character varying(10) NOT NULL DEFAULT 'Left';"
                Else
                    sqlq = "ALTER TABLE [OURReportItems] ADD [TextAlign] NVARCHAR(10) NOT NULL DEFAULT 'Left'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'TextAlign' added. "
                End If
            End If

            If Not ColumnExists("OurReportItems", "CaptionBorderColor", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportitems` ADD COLUMN `CaptionBorderColor` VARCHAR(45) NOT NULL DEFAULT 'lightgrey' AFTER `CaptionBackColor`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTITEMS ADD CaptionBorderColor VARCHAR2(45 CHAR) DEFAULT 'lightgrey'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportItems` ADD COLUMN `CaptionBorderColor` character varying(45) NOT NULL DEFAULT 'lightgrey';"
                Else
                    sqlq = "ALTER TABLE [OURReportItems] ADD [CaptionBorderColor] NVARCHAR(45) NOT NULL DEFAULT 'lightgrey'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'CaptionBorderColor' added. "
                End If
            End If
            If Not ColumnExists("OurReportItems", "CaptionBorderStyle", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportitems` ADD COLUMN `CaptionBorderStyle` VARCHAR(15) NOT NULL DEFAULT 'Solid' AFTER `CaptionBorderColor`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTITEMS ADD CaptionBorderStyle VARCHAR2(15 CHAR) DEFAULT 'Solid'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportItems` ADD COLUMN `CaptionBorderStyle` character varying(15) NOT NULL DEFAULT 'Solid';"
                Else
                    sqlq = "ALTER TABLE [OURReportItems] ADD [CaptionBorderStyle] NVARCHAR(15) NOT NULL DEFAULT 'Solid'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'CaptionBorderStyle' added. "
                End If
            End If
            If Not ColumnExists("OurReportItems", "CaptionBorderWidth", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportitems` ADD COLUMN `CaptionBorderWidth` VARCHAR(5) NOT NULL DEFAULT '1' AFTER `CaptionBorderStyle`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTITEMS ADD CaptionBorderWidth VARCHAR2(5 CHAR) DEFAULT '1'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportItems` ADD COLUMN `CaptionBorderWidth` character varying(5) NOT NULL DEFAULT '1';"
                Else
                    sqlq = "ALTER TABLE [OURReportItems] ADD [CaptionBorderWidth] NVARCHAR(5) NOT NULL DEFAULT '1'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'CaptionBorderWidth' added. "
                End If
            End If
            If Not ColumnExists("OurReportItems", "BorderColor", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportitems` ADD COLUMN `BorderColor` VARCHAR(45) NOT NULL DEFAULT 'lightgrey' AFTER `BackColor`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTITEMS ADD BorderColor VARCHAR2(45 CHAR) DEFAULT 'lightgrey'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportItems` ADD COLUMN `BorderColor` character varying(45) NOT NULL DEFAULT 'lightgrey';"
                Else
                    sqlq = "ALTER TABLE [OURReportItems] ADD [BorderColor] NVARCHAR(45) NOT NULL DEFAULT 'lightgrey'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'BorderColor' added. "
                End If
            End If
            If Not ColumnExists("OurReportItems", "BorderStyle", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportitems` ADD COLUMN `BorderStyle` VARCHAR(15) NOT NULL DEFAULT 'Solid' AFTER `BorderColor`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTITEMS ADD BorderStyle VARCHAR2(15 CHAR) DEFAULT 'Solid'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportItems` ADD COLUMN `BorderStyle` character varying(15) NOT NULL DEFAULT 'Solid';"
                Else
                    sqlq = "ALTER TABLE [OURReportItems] ADD [BorderStyle] NVARCHAR(15) NOT NULL DEFAULT 'Solid'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'BorderStyle' added. "
                End If
            End If
            If Not ColumnExists("OurReportItems", "BorderWidth", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportitems` ADD COLUMN `BorderWidth` VARCHAR(5) NOT NULL DEFAULT '1' AFTER `BorderStyle`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTITEMS ADD BorderWidth VARCHAR2(5 CHAR) DEFAULT '1'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportItems` ADD COLUMN `BorderWidth` character varying(5) NOT NULL DEFAULT '1';"
                Else
                    sqlq = "ALTER TABLE [OURReportItems] ADD [BorderWidth] NVARCHAR(5) NOT NULL DEFAULT '1'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'BorderWidth' added. "
                End If
            End If

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURReportShow(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURReportFormat(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURReportGroups(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURReportLists(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If Not ColumnExists("OURReportLists", "Prop1", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportlists` ADD COLUMN `Prop1` VARCHAR(250) NULL DEFAULT NULL AFTER `comments`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTLISTS ADD Prop1 VARCHAR2(250 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportLists` ADD COLUMN `Prop1` character varying(250) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURReportLists] ADD [Prop1] NVARCHAR(250) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'Prop1' added. "
                End If
            End If

            If Not ColumnExists("OURReportLists", "Prop2", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportlists` ADD COLUMN `Prop2` VARCHAR(250) NULL DEFAULT NULL AFTER `comments`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTLISTS ADD Prop2 VARCHAR2(250 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportLists` ADD COLUMN `Prop2` character varying(250) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURReportLists] ADD [Prop2] NVARCHAR(250) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'Prop2' added. "
                End If
            End If

            If Not ColumnExists("OURReportLists", "Prop3", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportlists` ADD COLUMN `Prop3` VARCHAR(250) NULL DEFAULT NULL AFTER `comments`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTLISTS ADD Prop3 VARCHAR2(250 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportLists` ADD COLUMN `Prop3` character varying(250) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURReportLists] ADD [Prop3] NVARCHAR(250) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'Prop3' added. "
                End If
            End If

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURReportSQLquery(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURScheduledReports(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURScheduledDownloads(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If Not ColumnExists("OURScheduledDownloads", "Prop4", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourscheduleddownloads` ADD COLUMN `Prop4` VARCHAR(1200) NULL DEFAULT NULL AFTER `Prop3`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURSCHEDULEDDOWNLOADS ADD Prop4 VARCHAR2(1200 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURScheduledDownloads` ADD COLUMN `Prop4` character varying(1200) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURScheduledDownloads] ADD [Prop4] NVARCHAR(1200) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'Prop4' added. "
            End If

            If Not ColumnExists("OURScheduledDownloads", "Prop5", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourscheduleddownloads` ADD COLUMN `Prop5` VARCHAR(1200) NULL DEFAULT NULL AFTER `Prop4`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURSCHEDULEDDOWNLOADS ADD Prop5 VARCHAR2(1200 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURScheduledDownloads` ADD COLUMN `Prop5` character varying(1200) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURScheduledDownloads] ADD [Prop5] NVARCHAR(1200) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'Prop5' added. "
            End If

            If Not ColumnExists("OURScheduledDownloads", "Prop6", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourscheduleddownloads` ADD COLUMN `Prop6` VARCHAR(1200) NULL DEFAULT NULL AFTER `Prop5`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURSCHEDULEDDOWNLOADS ADD Prop6 VARCHAR2(1200 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURScheduledDownloads` ADD COLUMN `Prop6` character varying(1200) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURScheduledDownloads] ADD [Prop6] NVARCHAR(1200) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'Prop6' added. "
            End If

            'If ret = "" OrElse ret = "Query executed fine." Then
            '    ret = "OURScheduledDownloads updated"
            'Else
            '    ret = "OURScheduledDownloads update crashed: " & ret
            'End If

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function

    Public Function UpdateOURScheduledImports(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If Not ColumnExists("OURScheduledImports", "TableName", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourscheduledimports` ADD COLUMN `TableName` VARCHAR(1200) NULL DEFAULT NULL AFTER `URL`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURSCHEDULEDIMPORTS ADD TableName VARCHAR2(1200 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURScheduledImports` ADD COLUMN `TableName` character varying(1200) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURScheduledImports] ADD [TableName] NVARCHAR(1200) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'TableName' added. "
            End If

            If Not ColumnExists("OURScheduledImports", "Prop7", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourscheduledimports` ADD COLUMN `Prop7` VARCHAR(1200) NULL DEFAULT NULL AFTER `Prop6`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURSCHEDULEDIMPORTS ADD Prop7 VARCHAR2(1200 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURScheduledImports` ADD COLUMN `Prop7` character varying(1200) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURScheduledImports] ADD [Prop7] NVARCHAR(1200) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'Prop7' added. "
            End If

            If Not ColumnExists("OURScheduledImports", "Prop8", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourscheduledimports` ADD COLUMN `Prop8` VARCHAR(1200) NULL DEFAULT NULL AFTER `Prop7`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURSCHEDULEDIMPORTS ADD Prop8 VARCHAR2(1200 CHAR) DEFAULT NULL"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURScheduledImports` ADD COLUMN `Prop8` character varying(1200) NULL DEFAULT NULL;"
                Else
                    sqlq = "ALTER TABLE [OURScheduledImports] ADD [Prop8] NVARCHAR(1200) NULL DEFAULT NULL"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'Prop8' added. "
            End If

            'If ret = "" OrElse ret = "Query executed fine." Then
            '    ret = "OURScheduledImports updated"
            'Else
            '    ret = "OURScheduledImports update crashed: " & ret
            'End If

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURUserTables(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURReportView(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If Not ColumnExists("OurReportView", "ReportCaptionAlign", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `ReportCaptionAlign` VARCHAR(10) NOT NULL DEFAULT 'Left' AFTER `LabelStrikeout`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD ReportCaptionAlign VARCHAR2(10 CHAR) DEFAULT 'Left'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `ReportCaptionAlign` character varying(10) NOT NULL DEFAULT 'Left';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [ReportCaptionAlign] NVARCHAR(10) NOT NULL DEFAULT 'Left'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'ReportCaptionAlign' added. "
                End If
            End If
            If Not ColumnExists("OurReportView", "ReportDetailAlign", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `ReportDetailAlign` VARCHAR(10) NOT NULL DEFAULT 'Left' AFTER `DataStrikeout`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD ReportDetailAlign VARCHAR2(10 CHAR) DEFAULT 'Left'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `ReportDetailAlign` character varying(10) NOT NULL DEFAULT 'Left';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [ReportDetailAlign] NVARCHAR(10) NOT NULL DEFAULT 'Left'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'ReportDetailAlign' added. "
            End If
            End If

            If Not ColumnExists("OurReportView", "DataBackColor", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `DataBackColor` VARCHAR(45) NOT NULL DEFAULT 'white' AFTER `DataForeColor`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD DataBackColor VARCHAR2(45 CHAR) DEFAULT 'white'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `DataBackColor` character varying(45) NOT NULL DEFAULT 'white';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [DataBackColor] NVARCHAR(45) NOT NULL DEFAULT 'white'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'DataBackColor' added. "
                End If
            End If
            If Not ColumnExists("OurReportView", "DataBorderColor", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `DataBorderColor` VARCHAR(45) NOT NULL DEFAULT 'lightgrey' AFTER `DataBackColor`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD DataBorderColor VARCHAR2(45 CHAR) DEFAULT 'lightgrey'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `DataBorderColor` character varying(45) NOT NULL DEFAULT 'lightgrey';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [DataBorderColor] NVARCHAR(45) NOT NULL DEFAULT 'lightgrey'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'DataBorderColor' added. "
                End If
            End If
            If Not ColumnExists("OurReportView", "DataBorderStyle", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `DataBorderStyle` VARCHAR(15) NOT NULL DEFAULT 'Solid' AFTER `DataBorderColor`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD DataBorderStyle VARCHAR2(15 CHAR) DEFAULT 'Solid'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `DataBorderStyle` character varying(15) NOT NULL DEFAULT 'Solid';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [DataBorderStyle] NVARCHAR(15) NOT NULL DEFAULT 'Solid'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'DataBorderStyle' added. "
                End If
            End If
            If Not ColumnExists("OurReportView", "DataBorderWidth", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `DataBorderWidth` VARCHAR(5) NOT NULL DEFAULT '1' AFTER `DataBorderStyle`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD DataBorderWidth VARCHAR2(5 CHAR) DEFAULT '1'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `DataBorderWidth` character varying(5) NOT NULL DEFAULT '1';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [DataBorderWidth] NVARCHAR(5) NOT NULL DEFAULT '1'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'DataBorderWidth' added. "
                End If
            End If
            If Not ColumnExists("OurReportView", "LabelBackColor", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `LabelBackColor` VARCHAR(45) NOT NULL DEFAULT 'white' AFTER `LabelForeColor`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD LabelBackColor VARCHAR2(45 CHAR) DEFAULT 'white'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `LabelBackColor` character varying(45) NOT NULL DEFAULT 'white';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [LabelBackColor] NVARCHAR(45) NOT NULL DEFAULT 'white'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'LabelBackColor' added. "
                End If
            End If
            If Not ColumnExists("OurReportView", "LabelBorderColor", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `LabelBorderColor` VARCHAR(45) NOT NULL DEFAULT 'lightgrey' AFTER `LabelBackColor`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD LabelBorderColor VARCHAR2(45 CHAR) DEFAULT 'lightgrey'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `LabelBorderColor` character varying(45) NOT NULL DEFAULT 'lightgrey';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [LabelBorderColor] NVARCHAR(45) NOT NULL DEFAULT 'lightgrey'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'LabelBorderColor' added. "
                End If
            End If
            If Not ColumnExists("OurReportView", "LabelBorderStyle", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `LabelBorderStyle` VARCHAR(15) NOT NULL DEFAULT 'Solid' AFTER `LabelBorderColor`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD LabelBorderStyle VARCHAR2(15 CHAR) DEFAULT 'Solid'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `LabelBorderStyle` character varying(15) NOT NULL DEFAULT 'Solid';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [LabelBorderStyle] NVARCHAR(15) NOT NULL DEFAULT 'Solid'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'LabelBorderStyle' added. "
                End If
            End If
            If Not ColumnExists("OurReportView", "LabelBorderWidth", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `LabelBorderWidth` VARCHAR(5) NOT NULL DEFAULT '1' AFTER `LabelBorderStyle`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD LabelBorderWidth VARCHAR2(5 CHAR) DEFAULT '1'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `LabelBorderWidth` character varying(5) NOT NULL DEFAULT '1';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [LabelBorderWidth] NVARCHAR(5) NOT NULL DEFAULT '1'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'LabelBorderWidth' added. "
                End If
            End If
            If Not ColumnExists("OurReportView", "ReportFieldLayout", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `ReportFieldLayout` VARCHAR(10) NOT NULL DEFAULT 'Block' AFTER `Orientation`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD ReportFieldLayout VARCHAR2(10 CHAR) DEFAULT 'Block'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `ReportFieldLayout` character varying(10) NOT NULL DEFAULT 'Block';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [ReportFieldLayout] NVARCHAR(10) NOT NULL DEFAULT 'Block'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'ReportFieldLayout' added. "
                End If
            End If

            If Not ColumnExists("OurReportView", "HeaderHeight", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `HeaderHeight` VARCHAR(15) NOT NULL DEFAULT '1' AFTER `ReportFieldLayout`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD HeaderHeight VARCHAR2(15 CHAR) DEFAULT '1'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `HeaderHeight` character varying(15) NOT NULL DEFAULT '1';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [HeaderHeight] NVARCHAR(5) NOT NULL DEFAULT '1'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'HeaderHeight' added. "
                End If
            End If
            If Not ColumnExists("OurReportView", "HeaderBackColor", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `HeaderBackColor` VARCHAR(45) NOT NULL DEFAULT 'white' AFTER `HeaderHeight`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD HeaderBackColor VARCHAR2(45 CHAR) DEFAULT 'white'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `HeaderBackColor` character varying(45) NOT NULL DEFAULT 'white';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [HeaderBackColor] NVARCHAR(45) NOT NULL DEFAULT 'white'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'HeaderBackColor' added. "
                End If
            End If
            If Not ColumnExists("OurReportView", "HeaderBorderColor", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `HeaderBorderColor` VARCHAR(45) NOT NULL DEFAULT 'lightgrey' AFTER `HeaderBackColor`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD HeaderBorderColor VARCHAR2(45 CHAR) DEFAULT 'lightgrey'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `HeaderBorderColor` character varying(45) NOT NULL DEFAULT 'lightgrey';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [HeaderBorderColor] NVARCHAR(45) NOT NULL DEFAULT 'lightgrey'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'HeaderBorderColor' added. "
                End If
            End If
            If Not ColumnExists("OurReportView", "HeaderBorderStyle", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `HeaderBorderStyle` VARCHAR(15) NOT NULL DEFAULT 'Solid' AFTER `HeaderBorderColor`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD HeaderBorderStyle VARCHAR2(15 CHAR) DEFAULT 'Solid'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `HeaderBorderStyle` character varying(15) NOT NULL DEFAULT 'Solid';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [HeaderBorderStyle] NVARCHAR(15) NOT NULL DEFAULT 'Solid'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'HeaderBorderStyle' added. "
                End If
            End If
            If Not ColumnExists("OurReportView", "HeaderBorderWidth", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `HeaderBorderWidth` VARCHAR(5) NOT NULL DEFAULT '1' AFTER `HeaderBorderStyle`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD HeaderBorderWidth VARCHAR2(5 CHAR) DEFAULT '1'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `HeaderBorderWidth` character varying(5) NOT NULL DEFAULT '1';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [HeaderBorderWidth] NVARCHAR(5) NOT NULL DEFAULT '1'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'HeaderBorderWidth' added. "
                End If
            End If
            If Not ColumnExists("OurReportView", "FooterHeight", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `FooterHeight` VARCHAR(15) NOT NULL DEFAULT '1' AFTER `HeaderBorderWidth`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD FooterHeight VARCHAR2(15 CHAR) DEFAULT '1'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `FooterHeight` character varying(15) NOT NULL DEFAULT '1';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [FooterHeight] NVARCHAR(5) NOT NULL DEFAULT '1'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'FooterHeight' added. "
                End If
            End If
            If Not ColumnExists("OurReportView", "FooterBackColor", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `FooterBackColor` VARCHAR(45) NOT NULL DEFAULT 'white' AFTER `FooterHeight`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD FooterBackColor VARCHAR2(45 CHAR) DEFAULT 'white'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `FooterBackColor` character varying(45) NOT NULL DEFAULT 'white';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [FooterBackColor] NVARCHAR(45) NOT NULL DEFAULT 'white'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'FooterBackColor' added. "
                End If
            End If
            If Not ColumnExists("OurReportView", "FooterBorderColor", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `FooterBorderColor` VARCHAR(45) NOT NULL DEFAULT 'lightgrey' AFTER `FooterBackColor`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD FooterBorderColor VARCHAR2(45 CHAR) DEFAULT 'lightgrey'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `FooterBorderColor` character varying(45) NOT NULL DEFAULT 'lightgrey';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [FooterBorderColor] NVARCHAR(45) NOT NULL DEFAULT 'lightgrey'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'FooterBorderColor' added. "
                End If
            End If
            If Not ColumnExists("OurReportView", "FooterBorderStyle", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `FooterBorderStyle` VARCHAR(15) NOT NULL DEFAULT 'Solid' AFTER `FooterBorderColor`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD FooterBorderStyle VARCHAR2(15 CHAR) DEFAULT 'Solid'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `FooterBorderStyle` character varying(15) NOT NULL DEFAULT 'Solid';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [FooterBorderStyle] NVARCHAR(15) NOT NULL DEFAULT 'Solid'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'FooterBorderStyle' added. "
                End If
            End If
            If Not ColumnExists("OurReportView", "FooterBorderWidth", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    sqlq = "ALTER TABLE `" & db & "`.`ourreportview` ADD COLUMN `FooterBorderWidth` VARCHAR(5) NOT NULL DEFAULT '1' AFTER `FooterBorderStyle`;"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURREPORTVIEW ADD FooterBorderWidth VARCHAR2(5 CHAR) DEFAULT '1'"
                ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlq = "ALTER TABLE `OURReportView` ADD COLUMN `FooterBorderWidth` character varying(5) NOT NULL DEFAULT '1';"
                Else
                    sqlq = "ALTER TABLE [OURReportView] ADD [FooterBorderWidth] NVARCHAR(5) NOT NULL DEFAULT '1'"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv) '.Replace("Query executed fine.", "")
                If ret.EndsWith("Query executed fine.") Then
                    ret = ret.Replace("Query executed fine.", "") & "<br/>&nbsp;&nbsp; Field 'FooterBorderWidth' added. "
                End If
            End If

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURAgents(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURActivity(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)


            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURDashboards(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURkmlhistory(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURTaskListSetting(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)

            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function UpdateOURComparison(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If
            Dim db As String = GetDataBase(myconstr, myconprv)


            If Not ColumnExists("OURComparison", "recorder", myconstr, myconprv) Then
                If myconprv = "MySql.Data.MySqlClient" Then
                    db = db.ToLower
                    sqlq = "ALTER TABLE `" & db & "`.`ourcomparison` ADD COLUMN `recorder` tinyint(2) DEFAULT '0';"
                ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                    sqlq = "ALTER TABLE OURComparison ADD recorder NUMBER(2,0) DEFAULT 0"
                Else
                    sqlq = "ALTER TABLE [OURComparison] ADD [recorder] smallint DEFAULT 0"
                End If
                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "") & " Field 'recorder' added."
            End If
            If ret = "" Then ret = "No Updates"
        Catch ex As Exception
            ret = "<br/> " & ex.Message
        End Try
        Return ret
    End Function
    'Public Function UpdateOURUnits(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
    '    Dim ret, sqlq, err As String
    '    err = ""
    '    ret = ""
    '    Try
    '        If myconstr = "" Then
    '            myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
    '            myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
    '        End If
    '        Dim db As String = GetDataBase(myconstr, myconprv)

    '    Catch ex As Exception
    '        ret = "<br/> " & ex.Message
    '    End Try
    '    Return ret
    'End Function
    'Public Function UpdateOURFriendlyNames(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
    '    Dim ret, sqlq, err As String
    '    err = ""
    '    ret = ""
    '    Try
    '        If myconstr = "" Then
    '            myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
    '            myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
    '        End If
    '        Dim db As String = GetDataBase(myconstr, myconprv)

    '    Catch ex As Exception
    '        ret = "<br/> " & ex.Message
    '    End Try
    '    Return ret
    'End Function
    'Public Function UpdateOURFiles(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
    '    Dim ret, sqlq, err As String
    '    err = ""
    '    ret = ""
    '    Try
    '        If myconstr = "" Then
    '            myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
    '            myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
    '        End If
    '        Dim db As String = GetDataBase(myconstr, myconprv)

    '    Catch ex As Exception
    '        ret = "<br/> " & ex.Message
    '    End Try
    '    Return ret
    'End Function
    'Public Function UpdateOURUnits(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
    '    Dim ret, sqlq, err As String
    '    err = ""
    '    ret = ""
    '    Try
    '        If myconstr = "" Then
    '            myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
    '            myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
    '        End If
    '        Dim db As String = GetDataBase(myconstr, myconprv)

    '    Catch ex As Exception
    '        ret = "<br/> " & ex.Message
    '    End Try
    '    Return ret
    'End Function
    'Public Function UpdateOURFriendlyNames(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
    '    Dim ret, sqlq, err As String
    '    err = ""
    '    ret = ""
    '    Try
    '        If myconstr = "" Then
    '            myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
    '            myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
    '        End If
    '        Dim db As String = GetDataBase(myconstr, myconprv)

    '    Catch ex As Exception
    '        ret = "<br/> " & ex.Message
    '    End Try
    '    Return ret
    'End Function
    'Public Function UpdateOURFiles(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
    '    Dim ret, sqlq, err As String
    '    err = ""
    '    ret = ""
    '    Try
    '        If myconstr = "" Then
    '            myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
    '            myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
    '        End If
    '        Dim db As String = GetDataBase(myconstr, myconprv)

    '    Catch ex As Exception
    '        ret = "<br/> " & ex.Message
    '    End Try
    '    Return ret
    'End Function
    'Public Function UpdateOURUnits(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
    '    Dim ret, sqlq, err As String
    '    err = ""
    '    ret = ""
    '    Try
    '        If myconstr = "" Then
    '            myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
    '            myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
    '        End If
    '        Dim db As String = GetDataBase(myconstr, myconprv)

    '    Catch ex As Exception
    '        ret = "<br/> " & ex.Message
    '    End Try
    '    Return ret
    'End Function
    'Public Function UninstallOURUserTables(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
    '    Dim ret As String = String.Empty
    '    Dim err As String = String.Empty
    '    Dim sqlq As String = String.Empty
    '    If myconstr = "" Then
    '        myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
    '        myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
    '    End If
    '    Try
    '        'delete OURUserTables
    '        If myconprv = "MySql.Data.MySqlClient" Then
    '            Dim db As String = GetDataBase(myconstr, myconprv)
    '            sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" & db & "' And TABLE_NAME='ourusertables'"
    '            If HasRecords(sqlq, myconstr, myconprv) Then
    '                sqlq = "DROP TABLE `" & db & "`.`ourusertables`;"
    '                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
    '                If ret = "Query executed fine." Then
    '                    ret = "<br/> " & " Table ourusertables deleted"
    '                Else
    '                    ret = "<br/> " & " Table ourusertables NOT deleted: " & ret
    '                End If
    '            End If
    '        Else
    '            'TODO !!!!!! for Oracle.ManagedDataAccess.Client

    '            If myconprv.StartsWith("InterSystems.Data.") Then
    '                sqlq = "Select ID As TABLE_NAME FROM %Dictionary.ClassDefinition WHERE Not (ID %STARTSWITH '%') AND ID='OURUserTables'"
    '            Else
    '                sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='OURUserTables'"
    '            End If
    '            If HasRecords(sqlq, myconstr, myconprv) Then
    '                sqlq = "DROP Table [OURUserTables]"
    '            End If
    '            ret = ret & "<br/> " & " " & sqlq & "  " & ExequteSQLquery(sqlq, myconstr, myconprv)
    '        End If
    '    Catch ex As Exception
    '        ret = ret & "<br/> " & sqlq & "  " & ex.Message
    '    End Try
    '    Return ret
    'End Function
    'Public Function UninstallOURUnits(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
    '    Dim ret As String = String.Empty
    '    Dim err As String = String.Empty
    '    Dim sqlq As String = String.Empty
    '    If myconstr = "" Then
    '        myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
    '        myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
    '    End If
    '    Dim db As String = GetDataBase(myconstr, myconprv)
    '    Dim myView As DataView
    '    Try
    '        'delete ourfriendlynames
    '        If myconprv = "MySql.Data.MySqlClient" Then
    '            sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" & db & "' And TABLE_NAME='ourunits'"
    '            myView = mRecords(sqlq, err, myconstr, myconprv)  'from OUR db
    '            If myView IsNot Nothing AndAlso myView.Table.Rows.Count = 1 Then
    '                sqlq = "DROP TABLE `" & db & "`.`ourunits`;"
    '                ret = ExequteSQLquery(sqlq, myconstr, myconprv)
    '                If ret = "Query executed fine." Then
    '                    ret = "<br/> " & " Table ourunits deleted"
    '                Else
    '                    ret = "<br/> " & " Table ourunits NOT deleted: " & ret
    '                End If
    '            End If
    '        Else
    '            'TODO !!!!!! for Oracle.ManagedDataAccess.Client

    '            If myconprv.StartsWith("InterSystems.Data.") Then
    '                sqlq = "Select ID As TABLE_NAME FROM %Dictionary.ClassDefinition WHERE Not (ID %STARTSWITH '%') AND ID='OURUnits'"
    '            Else
    '                sqlq = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='OURUnits'"
    '            End If
    '            myView = mRecords(sqlq, err, myconstr, myconprv)
    '            If myView IsNot Nothing AndAlso myView.Table.Rows.Count = 1 Then
    '                sqlq = "DROP Table [OURUnits]"
    '            End If
    '            ret = ret & "<br/> " & " " & sqlq & "  " & ExequteSQLquery(sqlq, myconstr, myconprv)
    '        End If
    '    Catch ex As Exception
    '        ret = ret & "<br/> " & sqlq & "  " & ex.Message
    '    End Try
    '    Return ret
    'End Function
    Public Function UninstallOURTablesClasses(Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim err As String = String.Empty
        Dim sqlq As String = String.Empty
        If myconstr = "" Then
            myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
        End If
        Dim db As String = GetDataBase(myconstr, myconprv)
        If db.StartsWith("OURdata") Then
            ret = "Uninstalling OUR tables from " & db & " is not allowed!"
            Return ret
            Exit Function
        End If
        ret = "Uninstalling OUR tables from " & db & ": <br/> "
        ret = ret & "<br/>" & UninstallOURTable("OURFriendlyNames", myconstr, myconprv)
        ret = ret & "<br/>" & UninstallOURTable("OURFiles", myconstr, myconprv)
        ret = ret & "<br/>" & UninstallOURTable("OURUnits", myconstr, myconprv)
        ret = ret & "<br/>" & UninstallOURTable("OURHelpDesk", myconstr, myconprv)
        ret = ret & "<br/>" & UninstallOURTable("OURAccessLog", myconstr, myconprv)
        ret = ret & "<br/>" & UninstallOURTable("OURDashboards", myconstr, myconprv)
        ret = ret & "<br/>" & UninstallOURTable("ourkmlhistory", myconstr, myconprv)
        ret = ret & "<br/>" & UninstallOURTable("ourtasklistsetting", myconstr, myconprv)
        ret = ret & "<br/>" & UninstallOURTable("OURPermits", myconstr, myconprv)
        ret = ret & "<br/>" & UninstallOURTable("OURPermissions", myconstr, myconprv)
        ret = ret & "<br/>" & UninstallOURTable("OURReportInfo", myconstr, myconprv)
        ret = ret & "<br/>" & UninstallOURTable("OURReportShow", myconstr, myconprv)
        ret = ret & "<br/>" & UninstallOURTable("OURReportFormat", myconstr, myconprv)
        ret = ret & "<br/>" & UninstallOURTable("OURReportGroups", myconstr, myconprv)
        ret = ret & "<br/>" & UninstallOURTable("OURReportLists", myconstr, myconprv)
        ret = ret & "<br/>" & UninstallOURTable("OURReportSQLquery", myconstr, myconprv)
        ret = ret & "<br/>" & UninstallOURTable("OURReportChildren", myconstr, myconprv)
        ret = ret & "<br/>" & UninstallOURTable("OURReportView", myconstr, myconprv)
        ret = ret & "<br/>" & UninstallOURTable("OURReportItems", myconstr, myconprv)
        If myconprv.StartsWith("InterSystems.Data.") Then
            'uninstall OUR.Init class
            sqlq = "DROP TABLE OUR.INIT"
            ret = ret & "<br/> " & sqlq & "  " & ExequteSQLquery(sqlq)
        End If
        Return ret
    End Function
    Public Function UpdateOURdbToVersion(ByVal versionFrom As String, ByVal versionTo As String, Optional ByVal myconstr As String = "", Optional ByVal myconprv As String = "") As String
        Dim ret, sqlq, err As String
        err = ""
        ret = ""
        Try
            If myconstr = "" Then
                myconprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                myconstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            End If

            '======================= TEMPLATE (!!!  DO NOT DELETE, ONLY COPY  !!!)==================================================

            If versionFrom = "XX-xx" AndAlso versionTo = "XX-xy" Then
                '------------------ OURFriendlyNames - see InstallOURFriendlyNames(myconstr, myconprv)

                '---------------------------------------------------------------------------

                '------------------ OURFiles - see InstallOURFiles(myconstr, myconprv)

                '---------------------------------------------------------------------------

                '------------------ OURUnits - see InstallOURUnits(myconstr, myconprv)

                '---------------------------------------------------------------------------

                '------------------ OURHelpDesk - see InstallOURHelpDesk(myconstr, myconprv)

                '---------------------------------------------------------------------------

                '------------------ OURAccessLog - see InstallOURAccessLog(myconstr, myconprv)

                '---------------------------------------------------------------------------

                '------------------ OURPermits - see InstallOURPermits(myconstr, myconprv)

                '---------------------------------------------------------------------------

                '------------------ OURPermissions - see InstallOURPermissions(myconstr, myconprv)

                '---------------------------------------------------------------------------

                '------------------ OURReportInfo - see InstallOURReportInfo(myconstr, myconprv)


                '------------------ OURReportShow - see InstallOURReportShow(myconstr, myconprv)

                '---------------------------------------------------------------------------

                '------------------ OURReportFormat - see InstallOURReportFormat(myconstr, myconprv)


                '---------------------------------------------------------------------------

                '------------------ OURReportGroups - see InstallOURReportGroups(myconstr, myconprv)

                '---------------------------------------------------------------------------

                '------------------ OURReportLists - see InstallOURReportLists(myconstr, myconprv)

                '---------------------------------------------------------------------------

                '------------------ OURReportSQLquery - see InstallOURReportSQLquery(myconstr, myconprv)

                '---------------------------------------------------------------------------

                '------------------ OURScheduledReports - see InstallOURScheduledReports(myconstr, myconprv)

                '---------------------------------------------------------------------------

                '------------------ OURUserTables - see InstallOURUserTables(myconstr, myconprv)
                '---------------------------------------------------------------------------

                '------------------ OURAgents - see InstallOURAgents(myconstr, myconprv)

                '---------------------------------------------------------------------------

                '------------------ OURActivity - see InstallOURActivity(myconstr, myconprv)

                '---------------------------------------------------------------------------

                '------------------ OURDashboards - see InstallOURDashboards(myconstr, myconprv)

                '---------------------------------------------------------------------------

                '------------------ ourkmlhistory - see Installourkmlhistory(myconstr, myconprv)
                '---------------------------------------------------------------------------

                '------------------ ourtasklistsetting - see InstallOURTaskListSetting(myconstr, myconprv)

                '---------------------------------------------------------------------------

                '------------------ ourcomparison - see InstallOURComparison(myconstr, myconprv)

                '---------------------------------------------------------------------------



            End If



            If versionFrom = "XX-xx" AndAlso versionTo = "XX-xy" Then
                '---------------------------------------------------------------------------
                '==================== SAMPLES  (!!!  DO NOT DELETE, ONLY COPY  !!!)==============================================
                'See also in DataModule:
                'RetypeAndReplaceColumn
                'RetypeAndReplaceColumnODBC

                '------------------ OURFiles - see InstallOURFiles(myconstr, myconprv)
                'Update column changes or additions
                '    If Not ColumnExists("OURFiles", "UserFile", myconstr, myconprv) Then
                '        If myconprv = "MySql.Data.MySqlClient" Then
                '            sqlq = "ALTER TABLE `" & db & "`.`OURFiles` ADD COLUMN `UserFile` longtext DEFAULT NULL;"
                '            ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                '        ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                '            sqlq = "ALTER TABLE OURFiles ADD UserFile CLOB DEFAULT NULL"
                '            ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                '        ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                '        Else
                '            sqlq = "ALTER TABLE OURFiles ADD [UserFile] [text] DEFAULT NULL"
                '            ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                '        End If
                '    End If
                '---------------------------------------------------------------------------

                '------------------ OURUnits - see InstallOURUnits(myconstr, myconprv)
                'If Not ColumnExists("OurUnits", "Official", myconstr, myconprv) Then
                '    If myconprv = "MySql.Data.MySqlClient" Then
                '        sqlq = "ALTER TABLE `" & db & "`.`ourunits` ADD COLUMN `Official` VARCHAR(200) NULL DEFAULT NULL AFTER `Comments`;"
                '    ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                '        sqlq = "ALTER TABLE OURUNITS ADD Official VARCHAR2(200 CHAR) DEFAULT NULL"
                '    ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                '        sqlq = "ALTER TABLE `OURUnits` ADD COLUMN `Official` character varying(200) NULL DEFAULT NULL;"
                '    Else
                '        sqlq = "ALTER TABLE [OURUnits] ADD [Official] NVARCHAR(200) NULL DEFAULT NULL"
                '    End If
                '    ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                'End If
                '---------------------------------------------------------------------------

                '------------------ OURHelpDesk - see InstallOURHelpDesk(myconstr, myconprv)
                '    If Not ColumnExists("OURHelpDesk", "Prop1", myconstr, myconprv) Then
                '        If myconprv = "MySql.Data.MySqlClient" Then
                '            sqlq = "ALTER TABLE `" & db & "`.`OURHelpDesk` ADD COLUMN `Prop1` VARCHAR(200) NULL DEFAULT NULL AFTER `Version`;"
                '        ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                '            sqlq = "ALTER TABLE OURHelpDesk ADD Prop1 VARCHAR2(200 CHAR) DEFAULT NULL"
                '        ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                '        Else
                '            sqlq = "ALTER TABLE [OURHelpDesk] ADD [Prop1] NVARCHAR(200) NULL DEFAULT NULL"
                '        End If
                '        ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                '    End If
                '---------------------------------------------------------------------------

                ''constraints
                'If myconprv.StartsWith("InterSystems.Data.") Then
                '    sqlq = "ALTER TABLE [OURAccessLog] ADD  CONSTRAINT [DF_OURAccessLog_Count]  DEFAULT ((0)) FOR [""Count""]"
                'ElseIf myconprv = "MySql.Data.MySqlClient" Then
                '    'TODO !!!!!! for Oracle.ManagedDataAccess.Client
                'Else 'Sql Server
                '    sqlq = "ALTER TABLE [OURAccessLog] ADD  CONSTRAINT [DF_OURAccessLog_Count]  DEFAULT ((0)) FOR [Count]"
                'End If
                'ret = ExequteSQLquery(sqlq, myconstr, myconprv)
                'If ret = "Query executed fine." Then
                '    ret = "OURAccessLog updated"
                'Else
                '    ret = "OURAccessLog updating crashed: " & ret
                'End If
                '---------------------------------------------------------------------------


                '------------------ OURReportInfo - see InstallOURReportInfo(myconstr, myconprv)
                '    If myconprv = "MySql.Data.MySqlClient" Then
                '        sqlq = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='" & db & "' And TABLE_NAME='ourreportinfo' And COLUMN_NAME='Param5id'"
                '        dt = mRecords(sqlq, err, myconstr, myconprv).Table
                '        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                '            nCharCount = CInt(dt.Rows(0)("CHARACTER_MAXIMUM_LENGTH"))
                '        End If
                '        sqlq = "ALTER TABLE `" & db & "`.`ourreportinfo` "
                '        sqlq = sqlq & "CHANGE COLUMN `Param0id` `Param0id` VARCHAR(250) NULL DEFAULT NULL,"
                '        sqlq = sqlq & "CHANGE COLUMN `Param0type` `Param0type` VARCHAR(250) NULL DEFAULT NULL,"
                '        sqlq = sqlq & "CHANGE COLUMN `Param1id` `Param1id` VARCHAR(250) NULL DEFAULT NULL,"
                '        sqlq = sqlq & "CHANGE COLUMN `Param1type` `Param1type` VARCHAR(250) NULL DEFAULT NULL,"
                '        sqlq = sqlq & "CHANGE COLUMN `Param2id` `Param2id` VARCHAR(250) NULL DEFAULT NULL,"
                '        sqlq = sqlq & "CHANGE COLUMN `Param2type` `Param2type` VARCHAR(250) NULL DEFAULT NULL,"
                '        sqlq = sqlq & "CHANGE COLUMN `Param3id` `Param3id` VARCHAR(250) NULL DEFAULT NULL,"
                '        sqlq = sqlq & "CHANGE COLUMN `Param3type` `Param3type` VARCHAR(250) NULL DEFAULT NULL,"
                '        sqlq = sqlq & "CHANGE COLUMN `Param4id` `Param4id` VARCHAR(250) NULL DEFAULT NULL,"
                '        sqlq = sqlq & "CHANGE COLUMN `Param4type` `Param4type` VARCHAR(250) NULL DEFAULT NULL,"
                '        sqlq = sqlq & "CHANGE COLUMN `Param5id` `Param5id` VARCHAR(250) NULL DEFAULT NULL,"
                '        sqlq = sqlq & "CHANGE COLUMN `Param5type` `Param5type` VARCHAR(250) NULL DEFAULT NULL,"
                '        sqlq = sqlq & "CHANGE COLUMN `Param6id` `Param6id` VARCHAR(250) NULL DEFAULT NULL,"
                '        sqlq = sqlq & "CHANGE COLUMN `Param6type` `Param6type` VARCHAR(250) NULL DEFAULT NULL,"
                '        sqlq = sqlq & "CHANGE COLUMN `Param7id` `Param7id` VARCHAR(250) NULL DEFAULT NULL,"
                '        sqlq = sqlq & "CHANGE COLUMN `Param7type` `Param7type` VARCHAR(250) NULL DEFAULT NULL,"
                '        sqlq = sqlq & "CHANGE COLUMN `Param8id` `Param8id` VARCHAR(250) NULL DEFAULT NULL,"
                '        sqlq = sqlq & "CHANGE COLUMN `Param8type` `Param8type` VARCHAR(250) NULL DEFAULT NULL,"
                '        sqlq = sqlq & "CHANGE COLUMN `Param9id` `Param9id` VARCHAR(250) NULL DEFAULT NULL,"
                '        sqlq = sqlq & "CHANGE COLUMN `Param9type` `Param9type` VARCHAR(250) NULL DEFAULT NULL;"
                '    ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                '        sqlq = "Select * FROM all_tab_cols WHERE TABLE_NAME = 'OURREPORTINFO'  AND COLUMN_NAME = 'PARAM5ID'"
                '        dt = mRecords(sqlq, err, myconstr, myconprv).Table
                '        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                '            nCharCount = CInt(dt.Rows(0)("CHAR_LENGTH"))
                '        End If
                '        sqlq = "ALTER TABLE OURREPORTINFO "
                '        sqlq &= "MODIFY ( "
                '        sqlq &= "Param0id VARCHAR2(250 CHAR) DEFAULT NULL,"
                '        sqlq &= "Param0type VARCHAR2(250 CHAR) DEFAULT NULL,"
                '        sqlq &= "Param1id VARCHAR2(250 CHAR) DEFAULT NULL,"
                '        sqlq &= "Param1type VARCHAR2(250 CHAR) DEFAULT NULL,"
                '        sqlq &= "Param2id VARCHAR2(250 CHAR) DEFAULT NULL,"
                '        sqlq &= "Param2type VARCHAR2(250 CHAR) DEFAULT NULL,"
                '        sqlq &= "Param3id VARCHAR2(250 CHAR) DEFAULT NULL,"
                '        sqlq &= "Param3type VARCHAR2(250 CHAR) DEFAULT NULL,"
                '        sqlq &= "Param4id VARCHAR2(250 CHAR) DEFAULT NULL,"
                '        sqlq &= "Param4type VARCHAR2(250 CHAR) DEFAULT NULL,"
                '        sqlq &= "Param5id VARCHAR2(250 CHAR) DEFAULT NULL,"
                '        sqlq &= "Param5type VARCHAR2(250 CHAR) DEFAULT NULL,"
                '        sqlq &= "Param6id VARCHAR2(250 CHAR) DEFAULT NULL,"
                '        sqlq &= "Param6type VARCHAR2(250 CHAR) DEFAULT NULL,"
                '        sqlq &= "Param7id VARCHAR2(250 CHAR) DEFAULT NULL,"
                '        sqlq &= "Param7type VARCHAR2(250 CHAR) DEFAULT NULL,"
                '        sqlq &= "Param8id VARCHAR2(250 CHAR) DEFAULT NULL,"
                '        sqlq &= "Param8type VARCHAR2(250 CHAR) DEFAULT NULL,"
                '        sqlq &= "Param9id VARCHAR2(250 CHAR) DEFAULT NULL,"
                '        sqlq &= "Param9type VARCHAR2(250 CHAR) DEFAULT NULL,"
                '        sqlq &= ")"
                '    ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                '    'TODO
                '    Else
                '        If myconprv.StartsWith("InterSystems.Data.") Then
                '            sqlq = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='OUR' AND TABLE_NAME='ReportInfo' AND COLUMN_NAME='Param5id'"
                '        Else
                '            sqlq = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='OURReportInfo' AND COLUMN_NAME='Param5id'"
                '        End If
                '        dt = mRecords(sqlq, err, myconstr, myconprv).Table
                '        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                '            nCharCount = CInt(dt.Rows(0)("CHARACTER_MAXIMUM_LENGTH"))
                '        End If
                '        If nCharCount < 250 Then
                '            For i As Integer = 0 To 9
                '                sqlq = "ALTER TABLE OURReportInfo ALTER COLUMN Param" & i & "id nvarchar(250) NULL"
                '                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                '                sqlq = "ALTER TABLE OURReportInfo ALTER COLUMN Param" & i & "type nvarchar(250) NULL"
                '                ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                '            Next
                '            nCharCount = 250
                '        End If
                '    End If
                '    If nCharCount < 250 Then
                '        ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                '    End If

                '------------------ OURReportShow - see InstallOURReportShow(myconstr, myconprv)
                '    If Not ColumnExists("OURReportShow", "PrmOrder", myconstr, myconprv) Then
                '        If myconprv = "MySql.Data.MySqlClient" Then
                '            db = db.ToLower
                '            sqlq = "ALTER TABLE `" & db & "`.`ourreportshow` ADD COLUMN `PrmOrder` INT(3) NOT NULL DEFAULT 0 AFTER `comments`;"
                '        ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                '            sqlq = "ALTER TABLE OURREPORTSHOW ADD PrmOrder NUMBER(3,0) DEFAULT 0 NOT NULL"
                '        ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql

                '        Else
                '            sqlq = "ALTER TABLE OURReportShow ADD [PrmOrder] INT NOT NULL DEFAULT 0"
                '        End If
                '        ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                '    End If
                '---------------------------------------------------------------------------

                '------------------ OURReportFormat - see InstallOURReportFormat(myconstr, myconprv)
                'If ret = "Table exists" OrElse ret = "OURReportFormat created" Then
                '    Dim bExists4 As Boolean = ColumnExists("OURReportFormat", "Prop4", myconstr, myconprv)
                '    If bExists4 Then
                '        If myconprv = "MySql.Data.MySqlClient" Then
                '            sqlq = "ALTER Table `" & db.ToLower & "`.`OURReportFormat` CHANGE COLUMN [Prop1] [Prop1] VARCHAR( 250 ) NULL Default NULL AFTER  [Order] ,"
                '            sqlq = sqlq & "CHANGE COLUMN [Prop2] [Prop2] VARCHAR( 250 )  Default NULL AFTER  [Prop1],"
                '            sqlq = sqlq & "CHANGE COLUMN [Prop3] [Prop3] VARCHAR( 250 )  Default NULL AFTER  [Prop2],"
                '            sqlq = sqlq & "CHANGE COLUMN [Prop4] [Prop4] VARCHAR( 250 )  Default NULL AFTER  [Prop3],"
                '            sqlq = sqlq & "CHANGE COLUMN [Prop5] [Prop5] VARCHAR( 250 )  Default NULL AFTER  [Prop4],"
                '            sqlq = sqlq & "CHANGE COLUMN [Prop6] [Prop6] VARCHAR( 250 )  Default NULL AFTER  [Prop5] ;"
                '            ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                '        End If
                '    Else
                '        If myconprv = "MySql.Data.MySqlClient" Then
                '            sqlq = "ALTER Table `" & db.ToLower & "`.`OURReportFormat` "
                '            sqlq = sqlq & "ADD COLUMN [Prop4] VARCHAR( 250 ) NULL Default NULL AFTER  [Prop3],"
                '            sqlq = sqlq & "ADD COLUMN [Prop5] VARCHAR( 250 ) NULL Default NULL AFTER  [Prop4],"
                '            sqlq = sqlq & "ADD COLUMN [Prop6] VARCHAR( 250 ) NULL Default NULL AFTER  [Prop5];"
                '        ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                '            sqlq = "ALTER TABLE OURREPORTFORMAT ADD ("
                '            sqlq &= " Prop4 VARCHAR2(250 CHAR) DEFAULT NULL,"
                '            sqlq &= " Prop5 VARCHAR2(250 CHAR) DEFAULT NULL,"
                '            sqlq &= " Prop6 VARCHAR2(250 CHAR) DEFAULT NULL"
                '            sqlq &= ")"
                '        Else
                '            sqlq = "ALTER TABLE OURReportFormat ADD "
                '            sqlq &= "[Prop4] [nvarchar](250) NULL,"
                '            sqlq &= "[Prop5] [nvarchar](250) NULL,"
                '            sqlq &= "[Prop6] [nvarchar](250) NULL"
                '        End If
                '        ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                '    End If

                '---------------------------------------------------------------------------


                '------------------ OURReportSQLquery - see InstallOURReportSQLquery(myconstr, myconprv)
                '    If Not ColumnExists("OURReportSQLquery", "Friendly", myconstr, myconprv) Then
                '        If myconprv = "MySql.Data.MySqlClient" Then
                '            db = db.ToLower
                '            sqlq = "ALTER TABLE `" & db & "`.`ourreportsqlquery` ADD COLUMN `Friendly` VARCHAR(50) NULL DEFAULT NULL AFTER `comments`;"
                '        ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                '            sqlq = "ALTER TABLE OURREPORTSQLQUERY ADD Friendly VARCHAR2(50 CHAR) DEFAULT NULL"
                '        Else
                '            sqlq = "ALTER TABLE OURReportSQLquery ADD [Friendly] VARCHAR(50) NULL DEFAULT NULL"
                '        End If
                '        ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                '    End If
                '    If Not ColumnExists("OURReportSQLquery", "RecOrder", myconstr, myconprv) Then
                '        If myconprv = "MySql.Data.MySqlClient" Then
                '            sqlq = "ALTER TABLE `" & db & "`.`ourreportsqlquery` ADD COLUMN `RecOrder` INT(4) NULL DEFAULT NULL AFTER `Friendly`;"
                '        ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                '            sqlq = "ALTER TABLE OURREPORTSQLQUERY ADD RecOrder NUMBER(4,0) DEFAULT NULL"
                '        Else
                '            sqlq = "ALTER TABLE OURReportSQLquery ADD [RecOrder] INT NULL DEFAULT NULL"
                '        End If
                '        ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                '    End If
                '    If Not ColumnExists("OURReportSQLquery", "Group", myconstr, myconprv) Then
                '        If myconprv = "MySql.Data.MySqlClient" Then
                '            sqlq = "ALTER TABLE `" & db & "`.`ourreportsqlquery` ADD COLUMN " & FixReservedWords("Group") & " VARCHAR(120) NULL DEFAULT NULL AFTER `RecOrder`;"
                '        ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                '            sqlq = "ALTER TABLE OURREPORTSQLQUERY ADD ""Group"" VARCHAR2(120 CHAR) DEFAULT NULL"
                '        Else
                '            sqlq = "ALTER TABLE OURReportSQLquery ADD " & FixReservedWords("Group") & " VARCHAR(120) NULL DEFAULT NULL"
                '        End If
                '        ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                '    End If
                '    If Not ColumnExists("OURReportSQLquery", "Logical", myconstr, myconprv) Then
                '        If myconprv = "MySql.Data.MySqlClient" Then
                '            sqlq = "ALTER TABLE `" & db & "`.`ourreportsqlquery` ADD COLUMN `Logical` VARCHAR(120) NULL DEFAULT NULL AFTER `Group`;"
                '        ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                '            sqlq = "ALTER TABLE OURREPORTSQLQUERY ADD Logical VARCHAR2(120 CHAR) DEFAULT NULL"
                '        Else
                '            sqlq = "ALTER TABLE OURReportSQLquery ADD [Logical] VARCHAR(120) NULL DEFAULT NULL"
                '        End If
                '        ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                '    End If
                '    If Not ColumnExists("OURReportSQLquery", "Param1", myconstr, myconprv) Then
                '        If myconprv = "MySql.Data.MySqlClient" Then
                '            sqlq = "ALTER TABLE `" & db & "`.`ourreportsqlquery` ADD COLUMN `Param1` VARCHAR(120) NULL DEFAULT NULL AFTER `Logical`;"
                '        ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                '            sqlq = "ALTER TABLE OURREPORTSQLQUERY ADD Param1 VARCHAR2(120 CHAR) DEFAULT NULL"
                '        Else
                '            sqlq = "ALTER TABLE OURReportSQLquery ADD [Param1] VARCHAR(120) NULL DEFAULT NULL"
                '        End If
                '        ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                '    End If
                '    If Not ColumnExists("OURReportSQLquery", "Param2", myconstr, myconprv) Then
                '        If myconprv = "MySql.Data.MySqlClient" Then
                '            sqlq = "ALTER TABLE `" & db & "`.`ourreportsqlquery` ADD COLUMN `Param2` VARCHAR(120) NULL DEFAULT NULL AFTER `Param1`;"
                '        ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                '            sqlq = "ALTER TABLE OURREPORTSQLQUERY ADD Param2 VARCHAR2(120 CHAR) DEFAULT NULL"
                '        Else
                '            sqlq = "ALTER TABLE OURReportSQLquery ADD [Param2] VARCHAR(120) NULL DEFAULT NULL"
                '        End If
                '        ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                '    End If
                '    If Not ColumnExists("OURReportSQLquery", "Param3", myconstr, myconprv) Then
                '        If myconprv = "MySql.Data.MySqlClient" Then
                '            sqlq = "ALTER TABLE `" & db & "`.`ourreportsqlquery` ADD COLUMN `Param3` VARCHAR(120) NULL DEFAULT NULL AFTER `Param2`;"
                '        ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                '            sqlq = "ALTER TABLE OURREPORTSQLQUERY ADD Param3 VARCHAR2(120 CHAR) DEFAULT NULL"
                '        Else
                '            sqlq = "ALTER TABLE OURReportSQLquery ADD [Param3] VARCHAR(120) NULL DEFAULT NULL"
                '        End If
                '        ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                '    End If
                '---------------------------------------------------------------------------


                '------------------ OURDashboards - see InstallOURDashboards(myconstr, myconprv)
                '    If Not ColumnExists("OURDashboards", "ARR", myconstr, myconprv) Then
                '        If myconprv = "MySql.Data.MySqlClient" Then
                '            db = db.ToLower
                '            sqlq = "ALTER TABLE `" & db & "`.`OURDashboards` ADD COLUMN `ARR` longtext DEFAULT NULL AFTER `Prop6`;"
                '            ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                '        ElseIf myconprv = "Oracle.ManagedDataAccess.Client" Then
                '            sqlq = "ALTER TABLE OURDASHBOARDS ADD ARR CLOB DEFAULT NULL"
                '            ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                '        ElseIf myconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                '        Else
                '            sqlq = "ALTER TABLE OURDashboards ADD [ARR] [text] DEFAULT NULL"
                '            ret = ret & ExequteSQLquery(sqlq, myconstr, myconprv).Replace("Query executed fine.", "")
                '        End If
                '    End If
                '---------------------------------------------------------------------------

            End If

        Catch ex As Exception
            ret = "<br/> ERROR!!" & ex.Message
        End Try
        Return ret
    End Function
End Module

