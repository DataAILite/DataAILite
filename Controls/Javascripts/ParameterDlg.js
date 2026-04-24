function fixReservedWords(ctlId, field) {

    var isCache = document.getElementById(ctlId + "hdnIsCacheDB").value;
    var isOracle = document.getElementById(ctlId + "hdnIsOracleDB").value;

    var check = "";
    var ret = "";
    //reserved words in Cache 
    var CacheReservedWords = "| ABSOLUTE | ADD | ALL | ALLOCATE | ALTER | AND | ANY | ARE | AS | ";
    CacheReservedWords = CacheReservedWords + "ASC | ASSERTION | AT | AUTHORIZATION | AVG | BEGIN | BETWEEN | ";
    CacheReservedWords = CacheReservedWords + "BIT | BIT_LENGTH | BOTH | BY | CASCADE | CASE | CAST | ";
    CacheReservedWords = CacheReservedWords + "CHAR | CHARACTER | CHARACTER_LENGTH | CHAR_LENGTH | ";
    CacheReservedWords = CacheReservedWords + "CHECK | CLOSE | COALESCE | COLLATE | COMMIT | CONNECT | ";
    CacheReservedWords = CacheReservedWords + "CONNECTION | CONSTRAINT | CONSTRAINTS | CONTINUE | CONVERT | ";
    CacheReservedWords = CacheReservedWords + "CORRESPONDING | COUNT | CREATE | CROSS | CURRENT | ";
    CacheReservedWords = CacheReservedWords + "CURRENT_DATE | CURRENT_TIME | CURRENT_TIMESTAMP | ";
    CacheReservedWords = CacheReservedWords + "CURRENT_USER | CURSOR | DATE | DEALLOCATE | DEC | DECIMAL | ";
    CacheReservedWords = CacheReservedWords + "Declare | DEFAULT | DEFERRABLE | DEFERRED | DELETE | DESC | ";
    CacheReservedWords = CacheReservedWords + "DESCRIBE | DESCRIPTOR | DIAGNOSTICS | DISCONNECT | DISTINCT | ";
    CacheReservedWords = CacheReservedWords + "DOMAIN | Double | DROP | Else | End | ENDEXEC | ESCAPE | EXCEPT | ";
    CacheReservedWords = CacheReservedWords + "EXCEPTION | EXEC | EXECUTE | EXISTS | EXTERNAL | EXTRACT | "
    CacheReservedWords = CacheReservedWords + "FALSE | FETCH | FIRST | FLOAT | FOR | FOREIGN | FOUND | FROM | FULL | ";
    CacheReservedWords = CacheReservedWords + "Get | Global | GO | GoTo | GRANT | GROUP | HAVING | HOUR | ";
    CacheReservedWords = CacheReservedWords + "IDENTITY | IMMEDIATE | IN | INDICATOR | INITIALLY | ";
    CacheReservedWords = CacheReservedWords + "INNER | INPUT | INSENSITIVE | INSERT | INT | Integer | INTERSECT | ";
    CacheReservedWords = CacheReservedWords + "INTERVAL | INTO | Is | ISOLATION | JOIN | LANGUAGE | LAST | ";
    CacheReservedWords = CacheReservedWords + "LEADING | LEFT | LEVEL | Like | LOCAL | LOWER | MATCH | MAX | MIN | ";
    CacheReservedWords = CacheReservedWords + "MINUTE | MODULE | NAMES | NATIONAL | NATURAL | NCHAR | ";
    CacheReservedWords = CacheReservedWords + "Next | NO | Not | NULL | NULLIF | NUMERIC | OCTET_LENGTH | OF | ON | ";
    CacheReservedWords = CacheReservedWords + "ONLY | OPEN | OPTION | Or | OUTER | OUTPUT | OVERLAPS | ";
    CacheReservedWords = CacheReservedWords + "PAD | PARTIAL | PREPARE | PRESERVE | PRIMARY | PRIOR | PRIVILEGES | ";
    CacheReservedWords = CacheReservedWords + "PROCEDURE | PUBLIC | READ | REAL | REFERENCES | RELATIVE | ";
    CacheReservedWords = CacheReservedWords + "RESTRICT | REVOKE | RIGHT | ROLE | ROLLBACK | ROWS | ";
    CacheReservedWords = CacheReservedWords + "SCHEMA | SCROLL | SECOND | SECTION | SELECT | SESSION_USER | ";
    CacheReservedWords = CacheReservedWords + "Set | SMALLINT | SOME | SPACE | SQLERROR | SQLSTATE | STATISTICS | ";
    CacheReservedWords = CacheReservedWords + "SUBSTRING | SUM | SYSDATE | SYSTEM_USER | TABLE | TEMPORARY | ";
    CacheReservedWords = CacheReservedWords + "THEN | TIME | TIMEZONE_HOUR | TIMEZONE_MINUTE | TO | TOP | ";
    CacheReservedWords = CacheReservedWords + "TRAILING | TRANSACTION | TRIM | TRUE | UNION | UNIQUE | ";
    CacheReservedWords = CacheReservedWords + "UPDATE | UPPER | USER | USING | VALUES | VARCHAR | VARYING | WHEN | ";
    CacheReservedWords = CacheReservedWords + "WHENEVER | WHERE | WITH | WORK | WRITE |";
    CacheReservedWords = CacheReservedWords.toUpperCase()

    //reserved words in SQL Server and MySql
    var SqlReservedWords = ",ADD,ALL,ALTER,AD,ANY,AS,ASC,AUTHORIZATION";
    SqlReservedWords = SqlReservedWords + ",BACKUP,BEGIN,BETWEEN,BREAK,BROWSE,BULK,BY";
    SqlReservedWords = SqlReservedWords + ",CASCADE,CASE,CHECK,CHECKPOINT,CLOSE,CLUSTERED,COALESCE";
    SqlReservedWords = SqlReservedWords + ",COLLATE,COLUMN,COMMIT,COMPUTE,CONSTRAINT,CONTAINS";
    SqlReservedWords = SqlReservedWords + ",CONTAINSTABLE,CONTINUE,CONVERT,CREATE,CROSS,CURRENT";
    SqlReservedWords = SqlReservedWords + ",CURRENT_DATE,CURRENT_TIME,CURRENT_TIMESTAMP,CURRENT_USER";
    SqlReservedWords = SqlReservedWords + ",CURSOR,DATABASE,DBCC,DEALLOCATE,DECLARE,DEFAULT,DELETE";
    SqlReservedWords = SqlReservedWords + ",DENY,DESC,DISK,DISTINCT,DISTRIBUTED,DOUBLE,DROP,DUMMY";
    SqlReservedWords = SqlReservedWords + ",DUMMY,DUMP,ELSE,END,ERRLVL,ESCAPE,EXCEPT,EXEC,EXECUTE";
    SqlReservedWords = SqlReservedWords + ",EXISTS,EXIT,FETCH,FILE,FILLFACTOR,FOR,FOREIGN,FREETEXT";
    SqlReservedWords = SqlReservedWords + ",FREETEXTTABLE,FROM,FULL,FUNCTION,GOTO,GRANT,GROUP,HAVING";
    SqlReservedWords = SqlReservedWords + ",HOLDLOCK,IDENTITY,IDENTITY_INSERT,IDENTITYCOL,IF,IN,INDEX";
    SqlReservedWords = SqlReservedWords + ",INNER,INSERT,INTERSECT,INTO,IS,JOIN,KEY,KILL,LEFT,LIKE";
    SqlReservedWords = SqlReservedWords + ",LINENO,LOAD,NATIONAL,NOCHECK,NONCLUSTERED,NOT,NULL,NULLIF";
    SqlReservedWords = SqlReservedWords + ",OF,OFF,OFFSETS,ON,OPEN,OPENDATASOURCE,OPENQUERY,OPENROWSET";
    SqlReservedWords = SqlReservedWords + ",OPENXML,OPTION,OR,ORDER,OUTER,OVER,PERCENT,PLAN,PRECISION";
    SqlReservedWords = SqlReservedWords + ",PRIMARY,PRINT,PROC,PROCEDURE,PUBLIC,RAISEERROR,READ";
    SqlReservedWords = SqlReservedWords + ",READTEXT,RECONFIGURE,REFERENCES,REPLICATION,RESTORE";
    SqlReservedWords = SqlReservedWords + ",RESTRICT,RETURN,REVOKE,RIGHT,ROLLBACK,ROWCOUNT,ROWGUIDCOL";
    SqlReservedWords = SqlReservedWords + ",RULE,SAVE,SCHEMA,SELECT,SESSION_USER,SET,SETUSER,SHUTDOWN";
    SqlReservedWords = SqlReservedWords + ",SOME,STATISTICS,SYSTEM_USER,TABLE,TEXTSIZE,THEN,TO,TOP";
    SqlReservedWords = SqlReservedWords + ",TRAN,TRANSACTION,TRIGGER,TRUNCATE,TSEQUAL,UNION,UNIQUE";
    SqlReservedWords = SqlReservedWords + ",UPDATE,UPDATETEXT,USE,USER,VALUES,VARYING,VIEW,WAITFOR";
    SqlReservedWords = SqlReservedWords + ",WHEN,WHERE,WHILE,WITH,WRITETEXT,ABSOLUTE,ACTION,ADMIN";
    SqlReservedWords = SqlReservedWords + ",AFTER,AGGREGATE,ALIAS,ALLOCATE,ARE,ARRAY,ASSERTION,AT";
    SqlReservedWords = SqlReservedWords + ",BEFORE,BINARY,BIT,BLOB,BOOLEAN,BOTH,BREADTH,CALL,CASCADED";
    SqlReservedWords = SqlReservedWords + ",CAST,CATALOG,CHAR,CHARACTER,CLASS,CLOB,COLLATION,COMPLETION";
    SqlReservedWords = SqlReservedWords + ",CONNECT,CONNECTION,CONTRAINTS,CONSTRUCTOR,CORRESPONDING";
    SqlReservedWords = SqlReservedWords + ",CUBE,CURRENT_PATH,CURRENT_ROLE,CYCLE,DATA,DATE,DAY,DEC";
    SqlReservedWords = SqlReservedWords + ",DECIMAL,DEFERRABLE,DEFFERRED,DEPTH,DEREF,DESCRIBE,DESCRIPTOR";
    SqlReservedWords = SqlReservedWords + ",DESTROY,DESTRUCTOR,DETERMINISTIC,DICTIONARY,DIAGNOSTICS";
    SqlReservedWords = SqlReservedWords + ",DISCONNECT,DOMAIN,DYNAMIC,EACH,END-EXEC,EQUALS,EVERY";
    SqlReservedWords = SqlReservedWords + ",EXCEPTION,EXTERNAL,FALSE,FIRST,FLOAT,FOUND,FREE,GENERAL";
    SqlReservedWords = SqlReservedWords + ",GET,GLOBAL,GO,GROUPING,HOST,HOUR,IGNORE,IMMEDIATE,INDICATOR";
    SqlReservedWords = SqlReservedWords + ",INITIALIZE,INITIALLY,INOUT,INPUT,INT,INTEGER,INTERVAL";
    SqlReservedWords = SqlReservedWords + ",ISOLATION,ITERATE,LANGUAGE,LARGE,LAST,LATERAL,LEADING,LESS";
    SqlReservedWords = SqlReservedWords + ",LEVEL,LIMIT,LOCAL,LOCALTIME,LOCALTIMESTAMP,LOCATIOR,MAP";
    SqlReservedWords = SqlReservedWords + ",MATCH,MINUTE,MODIFIES,MODIFY,MODULE,MONTH,NAMES,NATURAL";
    SqlReservedWords = SqlReservedWords + ",NCLOB,NEW,NEXT,NO,NONE,NUMERIC,OBJECT,OLD,ONLY,OPERATION";
    SqlReservedWords = SqlReservedWords + ",ORDINALITY,OUT,OUTPUT,PAD,PARAMETER,PARAMETERS,PARTIAL";
    SqlReservedWords = SqlReservedWords + ",PATH,POSTFIX,PREFIX,PREORDER,PREPARE,PRESERVE,PRIOR";
    SqlReservedWords = SqlReservedWords + ",PRIVILEGES,READS,REAL,RECURSIVE,REF,REFERENCING,RELATIVE";
    SqlReservedWords = SqlReservedWords + ",RESULT,RETURNS,ROLE,ROLLUP,ROUTINE,ROW,ROWS,SAVEPOINT,SCROLL";
    SqlReservedWords = SqlReservedWords + ",SCROLL,SCOPE,SEARCH,SECOND,SECTION,SEQUENCE,SESSION,SETS";
    SqlReservedWords = SqlReservedWords + ",SIZE,SHOW,SMALLINT,SPACE,SPECIFIC,SPECIFICTYPE,SQL,SQLEXCEPTION";
    SqlReservedWords = SqlReservedWords + ",SQLSTATE,SQLWARNING,START,STATE,STATEMENT,STATIC,STRUCTURE";
    SqlReservedWords = SqlReservedWords + ",TEMPORARY,TERMINATE,THAN,TIME,TIMESTAMP,TIMEZONE_HOUR";
    SqlReservedWords = SqlReservedWords + ",TIMEZONE_MINUTE,TRAILING,TRANSLATION,TREAT,TRUE,UNDER";
    SqlReservedWords = SqlReservedWords + ",UNKNOWN,UNNEST,USAGE,USING,VALUE,VARCHAR,VARIABLE,WHENEVER";
    SqlReservedWords = SqlReservedWords + ",WITHOUT,WORK,WRITE,YEAR,ZONE,";

    var OracleReservedWords = ",AGGREGATE,AGGREGATES,ALL,ALLOW,ANALYZE,ANCESTOR,AND";
    OracleReservedWords = OracleReservedWords + ",ANY,AS,AS,AT,AVG,BETWEEN,BINARY_DOUBLE,BINARY_FLOAT,BLOB";
    OracleReservedWords = OracleReservedWords + ",BRANCH,BUILD,BY,BYTE,CASE,CAST,CHAR,CHILD,CLEAR,CLOB,COMMIT";
    OracleReservedWords = OracleReservedWords + ",COMPILE,CONSIDER,COUNT,DATATYPE,DATE,DATE_MEASURE,DAY";
    OracleReservedWords = OracleReservedWords + ",DECIMAL,DELETE,DESC,DESCENDANT,DIMENSION,DISALLOW,DIVISION";
    OracleReservedWords = OracleReservedWords + ",DML,ELSE,END,ESCAPE,EXECUTE,FIRST,FLOAT,FOR,FROM,HIERARCHIES";
    OracleReservedWords = OracleReservedWords + ",HIERARCHY,HOUR,IGNORE,IN,INFINITE,INSERT,INTEGER,INTERVAL";
    OracleReservedWords = OracleReservedWords + ",INTO,IS,LAST,LEAF_DESCENDANT,LEAVES,LEVEL,LEVELS,LIKE";
    OracleReservedWords = OracleReservedWords + ",LIKEC,LIKE2,LIKE4,LOAD,LOCAL,LOG_SPEC,LONG,MAINTAIN,MAX";
    OracleReservedWords = OracleReservedWords + ",MEASURE,MEASURES,MEMBER,MEMBERS,MERGE,MLSLABEL,MIN,MINUTE";
    OracleReservedWords = OracleReservedWords + ",MODEL,MONTH,NAN,NCHAR,NCLOB,NO,NONE,NOT,NULL,NULLS,NUMBER";
    OracleReservedWords = OracleReservedWords + ",NVARCHAR2,OF,OLAP,OLAP_DML_EXPRESSION,ON,ONLY,OPERATOR,OR";
    OracleReservedWords = OracleReservedWords + ",ORDER,OVER,OVERFLOW,PARALLEL,PARENT,PLSQL,PRUNE,RAW";
    OracleReservedWords = OracleReservedWords + ",RELATIVE,ROOT_ANCESTOR,ROWID,SCN,SECOND,SELF,SERIAL,SET";
    OracleReservedWords = OracleReservedWords + ",SOLVE,SOME,SORT,SPEC,SUM,SYNCH,TEXT_MEASURE,THEN,TIME";
    OracleReservedWords = OracleReservedWords + ",TIMESTAMP,TO,UNBRANCH,UPDATE,USING,VALIDATE,VALUES,VARCHAR2";
    OracleReservedWords = OracleReservedWords + ",WHEN,WHERE,WITHIN,WITH,YEAR,ZERO,ZONE,ACCESS,ADD,ALTER";
    OracleReservedWords = OracleReservedWords + ",ASC,AUDIT,CHECK,CLUSTER,COLUMN,COLUMN_VALUE,COMMENT,COMPRESS";
    OracleReservedWords = OracleReservedWords + ",CONNECT,CREATE,CURRENT,DEFAULT,DISTINCT,DROP,EXCLUSIVE";
    OracleReservedWords = OracleReservedWords + ",EXISTS,FILE,GRANT,GROUP,HAVING,IDENTIFIED,IMMEDIATE,INCREMENT";
    OracleReservedWords = OracleReservedWords + ",INDEX,INITIAL,INTERSECT,LOCK,MAXEXTENTS,MINUS,MODE,MODIFY";
    OracleReservedWords = OracleReservedWords + ",NESTED_TABLE_ID,NOAUDIT,NOCOMPRESS,NOWAIT,OFFLINE,ONLINE";
    OracleReservedWords = OracleReservedWords + ",OPTION,PCTFREE,PRIOR,PUBLIC,RENAME,RESOURCE,REVOKE,ROW";
    OracleReservedWords = OracleReservedWords + ",ROWNUM,ROWS,SELECT,SESSION,SHARE,SIZE,SMALLINT,START";
    OracleReservedWords = OracleReservedWords + ",SUCCESSFUL,SYNONYM,SYSDATE,TABLE,TRIGGER,UID,UNION,UNIQUE";
    OracleReservedWords = OracleReservedWords + ",USER,VARCHAR,VIEW,WHENEVER,";

    var fldparts = field.split(".");
    for (var i = 0; i < fldparts.length; i++) {
        if (isCache == "1") {
            check = "| " + fldparts[i].toUpperCase() + " |";
            if (CacheReservedWords.indexOf(check) > -1)
                fldparts[i] = '"' + fldparts[i] + '"';
        }
        else if (isOracle == "1") {
            check = "," + fldparts[i].toUpperCase() + ",";
            if (OracleReservedWords.indexOf(check) > -1)
                fldparts[i] = '"' + fldparts[i] + '"';
        }
        else {
            check = "," + fldparts[i].toUpperCase() + ",";
            if (SqlReservedWords.indexOf(check) > -1)
                fldparts[i] = '[' + fldparts[i] + ']';
        }
        //check = "| " + fldparts[i].toUpperCase() + " |";
        //if (CacheReservedWords.indexOf(check) > -1) {
        //    if (isCache == "1") {
        //        fldparts[i] = '"' + fldparts[i] + '"'           
        //    }
        //    else {
        //        fldparts[i] = '[' + fldparts[i] + ']'
        //    };
        // }
        //else {
        //    check = "," + fldparts[i].toUpperCase() + ",";
        //    if (SqlReservedWords.indexOf(check) > -1) {
        //        if (isCache == "1") {
        //            fldparts[i] = '"' + fldparts[i] + '"'
        //        }
        //        else {
        //            fldparts[i] = '[' + fldparts[i] + ']'
        //        };
        //    };
        //};

    };
    for (var i = 0; i < fldparts.length; i++) {
        if (i > 0) ret = ret + ".";
        ret = ret + fldparts[i];
    }

    return ret;
}
function SetOtherFields(ctlId) {
    var ddField = document.getElementById(ctlId + "ddField");
    var txtLabel = document.getElementById(ctlId + "txtLabel");
    var txtParam = document.getElementById(ctlId + "txtParameter");
    var txtSQL = document.getElementById(ctlId + "txtSQL");
    var ddType = document.getElementById(ctlId + "ddType");
    var selectedText = ddField.options[ddField.selectedIndex].innerHTML;
    var selectedValue = ddField.value;
    var hdnField = document.getElementById(ctlId + "hdnField");
    //var hdnSQLValue = document.getElementById(ctlId + "hdnSQL").value;

    var pieces;
    var selectedField = selectedText;

    var selectedTable = "";
    var param = "";

    if (selectedText.indexOf(" AS ") > -1) {
        pieces = selectedText.split(" AS ");
        param = pieces[1];
        selectedText = pieces[0];
    }
    else if (selectedText.indexOf(".") > -1) {
        pieces = selectedText.split(".");
        param = pieces[pieces.length - 1];
    }
    //selectedText = selectedField;
    hdnField.value = selectedText;

    pieces = selectedText.split(".");
    selectedField = pieces[pieces.length - 1];
    for (var i = 0; i < pieces.length - 1; i++) {
        if (selectedTable == "")
            selectedTable = pieces[i];
        else
            selectedTable = selectedTable + "." + pieces[i];
    }
    txtSQL.value = "SELECT DISTINCT " + fixReservedWords(ctlId,selectedField) + " FROM " + fixReservedWords(ctlId,selectedTable) + " ORDER BY " + fixReservedWords(ctlId,selectedField);

    if (selectedValue == "System.String") {
        selectedValue = "nvarchar";
    }
    else {
        var val = selectedValue.toUpperCase();
        if (val.indexOf("DATETIME") > -1)
            selectedValue = "datetime";
        else
         selectedValue = "int";
    }
    // set ddType value
    for (var i = 0; i < ddType.options.length; i++) {
        if (ddType.options[i].text.toLowerCase() == selectedValue.toLowerCase()) {
            ddType.options[i].selected = true;
        }
        else {
            ddType.options[i].selected = false;
        }
    }
    txtLabel.value = param;
    txtParam.value = param;

//    if (hdnSQLValue != null && hdnSQLValue != "")
//    {

//        txtSQL.value = "SELECT DISTINCT sub." + selectedText + " FROM (" + hdnSQLValue + ") sub";
//}
            

    //alert("Selected Text: " + selectedText + " Value: " + selectedValue);
}