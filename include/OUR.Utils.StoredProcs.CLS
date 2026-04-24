<?xml version="1.0" encoding="UTF-8"?>
<Export generator="Cache" version="25" zv="Cache for Windows (x86-64) 2017.2 (Build 741U_SU)" ts="2019-07-01 09:50:16">
<Class name = "OUR.Utils.StoredProcs" >
<IncludeCode>OUR.GenASys.SQL</IncludeCode>
<Super>%RegisteredObject,%XML.Adaptor</Super>
<TimeChanged>65195,33318.240171</TimeChanged>
<TimeCreated>65186,30240.317729</TimeCreated>

<Method name="NamespaceOK">
<ClassMethod>1</ClassMethod>
<FormalSpec>Namespace:%String</FormalSpec>
<ReturnType>%Boolean</ReturnType>
<Implementation><![CDATA[
	s ret=0
	q:Namespace="" 0
	s n=$zu(90,0) f i=1:1:n s ck=$zu(90,2,0,i) i ck=Namespace s ret=1 q
	q ret
]]></Implementation>
</Method>

<Method name="CurrentNamespaceGet">
<ClassMethod>1</ClassMethod>
<ReturnType>%String</ReturnType>
<Implementation><![CDATA[	q $znspace
]]></Implementation>
</Method>

<Method name="ConvertTimeStamp">
<ClassMethod>1</ClassMethod>
<FormalSpec>ts:%TimeStamp</FormalSpec>
<ReturnType>%String</ReturnType>
<Implementation><![CDATA[
	s zdh=$zdth(ts,3,1,2)
	q $zdt(zdh,1,3,2)
	;
]]></Implementation>
</Method>

<Method name="PropertyExists">
<ClassMethod>1</ClassMethod>
<FormalSpec>ClassName:%String,PropertyName:%String</FormalSpec>
<ReturnType>%Boolean</ReturnType>
<Implementation><![CDATA[
	&Sql(Select ID Into :id
	     From %Dictionary.PropertyDefinition
	     Where Parent=:ClassName And Name = :PropertyName
	    )
	q (SQLCODE=0)
]]></Implementation>
</Method>

<Method name="ClassExists">
<ClassMethod>1</ClassMethod>
<FormalSpec>class:%String</FormalSpec>
<ReturnType>%Boolean</ReturnType>
<Implementation><![CDATA[
	q ##class(%Dictionary.ClassDefinition).%ExistsId(class)
	;
]]></Implementation>
</Method>

<Method name="ClassPropertyExists">
<ClassMethod>1</ClassMethod>
<FormalSpec>ClassProperty:%String</FormalSpec>
<ReturnType>%Boolean</ReturnType>
<Implementation><![CDATA[
	s class=$p(ClassProperty,".",1,$l(ClassProperty,".")-1)
	s property=$p(ClassProperty,".",$l(ClassProperty,"."))
	&Sql(Select ID Into :id From %Dictionary.CompiledProperty
	     Where Name = :property And Parent = :class
	    )

	q 'SQLCODE
	;
]]></Implementation>
</Method>

<Method name="ObjectErrorMessage">
<ClassMethod>1</ClassMethod>
<FormalSpec>sc</FormalSpec>
<ReturnType>%String</ReturnType>
<Implementation><![CDATA[
	s ErrMsg=""
	d $system.Status.DecomposeStatus(sc,.err)
	i $d(err) {
		s i=0 f {
			s i=$o(err(i)) q:'i
			s ErrMsg=$s(ErrMsg="":err(i),1:ErrMsg_$c(13,10)_err(i))
		}
	}
	q ErrMsg
]]></Implementation>
</Method>

<Method name="FormatQueryClass">
<ClassMethod>1</ClassMethod>
<FormalSpec>class:%String</FormalSpec>
<ReturnType>%String</ReturnType>
<Implementation><![CDATA[
	i $l(class,".")=2 q class
	q $tr($p(class,".",1,$l(class,".")-1),".","_")_"."_$p(class,".",$l(class,"."))
	;
]]></Implementation>
</Method>

<Method name="FixReservedWord">
<ClassMethod>1</ClassMethod>
<FormalSpec>Field:%String</FormalSpec>
<ReturnType>%String</ReturnType>
<Implementation><![CDATA[
   s ret=Field
	
   s CacheReservedWords = "| ABSOLUTE | ADD | ALL | ALLOCATE | ALTER | AND | ANY | ARE | AS | "
   s CacheReservedWords = CacheReservedWords_"| ASC | ASSERTION | AT | AUTHORIZATION | AVG | BEGIN | BETWEEN | "
   s CacheReservedWords = CacheReservedWords_"BIT | BIT_LENGTH | BOTH | BY | CASCADE | CASE | CAST | "
   s CacheReservedWords = CacheReservedWords_"CHAR | CHARACTER | CHARACTER_LENGTH | CHAR_LENGTH | "
   s CacheReservedWords = CacheReservedWords_"CHECK | CLOSE | COALESCE | COLLATE | COMMIT | CONNECT | "
   s CacheReservedWords =  CacheReservedWords_"CONNECTION | CONSTRAINT | CONSTRAINTS | CONTINUE | CONVERT | "
   s CacheReservedWords =  CacheReservedWords_"CORRESPONDING | COUNT | CREATE | CROSS | CURRENT | "
   s CacheReservedWords =  CacheReservedWords_"CURRENT_DATE | CURRENT_TIME | CURRENT_TIMESTAMP | "
   s CacheReservedWords =  CacheReservedWords_"CURRENT_USER | CURSOR | DATE | DEALLOCATE | DEC | DECIMAL | "
   s CacheReservedWords =  CacheReservedWords_"Declare | DEFAULT | DEFERRABLE | DEFERRED | DELETE | DESC | "
   s CacheReservedWords =  CacheReservedWords_"DESCRIBE | DESCRIPTOR | DIAGNOSTICS | DISCONNECT | DISTINCT | "
   s CacheReservedWords =  CacheReservedWords_"DOMAIN | Double | DROP | Else | End | ENDEXEC | ESCAPE | EXCEPT | "
   s CacheReservedWords =  CacheReservedWords_"EXCEPTION | EXEC | EXECUTE | EXISTS | EXTERNAL | EXTRACT | "
   s CacheReservedWords =  CacheReservedWords_"FALSE | FETCH | FIRST | FLOAT | FOR | FOREIGN | FOUND | FROM | FULL | "
   s CacheReservedWords =  CacheReservedWords_"Get | Global | GO | GoTo | GRANT | GROUP | HAVING | HOUR | "
   s CacheReservedWords =  CacheReservedWords_"IDENTITY | IMMEDIATE | IN | INDICATOR | INITIALLY | "
   s CacheReservedWords =  CacheReservedWords_"INNER | INPUT | INSENSITIVE | INSERT | INT | Integer | INTERSECT | "
   s CacheReservedWords =  CacheReservedWords_"INTERVAL | INTO | Is | ISOLATION | JOIN | LANGUAGE | LAST | "
   s CacheReservedWords =  CacheReservedWords_"LEADING | LEFT | LEVEL | Like | LOCAL | LOWER | MATCH | MAX | MIN | "
   s CacheReservedWords =  CacheReservedWords_"MINUTE | MODULE | NAMES | NATIONAL | NATURAL | NCHAR | "
   s CacheReservedWords =  CacheReservedWords_"Next | NO | Not | NULL | NULLIF | NUMERIC | OCTET_LENGTH | OF | ON | "
   s CacheReservedWords =  CacheReservedWords_"ONLY | OPEN | OPTION | Or | OUTER | OUTPUT | OVERLAPS | "
   s CacheReservedWords =  CacheReservedWords_"PAD | PARTIAL | PREPARE | PRESERVE | PRIMARY | PRIOR | PRIVILEGES | "
   s CacheReservedWords =  CacheReservedWords_"PROCEDURE | PUBLIC | READ | REAL | REFERENCES | RELATIVE | "
   s CacheReservedWords =  CacheReservedWords_"RESTRICT | REVOKE | RIGHT | ROLE | ROLLBACK | ROWS | "
   s CacheReservedWords =  CacheReservedWords_"SCHEMA | SCROLL | SECOND | SECTION | SELECT | SESSION_USER | "
   s CacheReservedWords =  CacheReservedWords_"Set | SMALLINT | SOME | SPACE | SQLERROR | SQLSTATE | STATISTICS | "
   s CacheReservedWords =  CacheReservedWords_"SUBSTRING | SUM | SYSDATE | SYSTEM_USER | TABLE | TEMPORARY | "
   s CacheReservedWords =  CacheReservedWords_"THEN | TIME | TIMEZONE_HOUR | TIMEZONE_MINUTE | TO | TOP | "
   s CacheReservedWords =  CacheReservedWords_"TRAILING | TRANSACTION | TRIM | TRUE | UNION | UNIQUE | "
   s CacheReservedWords =  CacheReservedWords_"UPDATE | UPPER | USER | USING | VALUES | VARCHAR | VARYING | WHEN | "
   s CacheReservedWords =  CacheReservedWords_"WHENEVER | WHERE | WITH | WORK | WRITE |"
   s CacheReservedWords = $$$ucase(CacheReservedWords)
   
   s ck=$$$ucase("| "_Field_" |")
   if CacheReservedWords[ck s ret=""""_Field_""""
   q ret
]]></Implementation>
</Method>

<Method name="GetProperties">
<ClassMethod>1</ClassMethod>
<FormalSpec><![CDATA[class:%String,&props]]></FormalSpec>
<ReturnType>%Status</ReturnType>
<Implementation><![CDATA[
	; internal called by ExportClassData
	;
	s sc=1
	s objClass=##class(%Dictionary.CompiledClass).%OpenId(class)
	i $IsObject(objClass) {
		s props=$g(props)
		s ck=$lb("many","children")
	}
	s i="" f {
	    s objProp=objClass.Properties.GetNext(.i) q:i=""
	    s PropName=objProp.Name,TypeName=objProp.Type,cardinality=objProp.Cardinality
	    s required=objProp.Required
	    continue:objProp.Collection="array"
	    continue:$lf(ck,cardinality)
	    ;only include storable properties
	    continue:'objProp.Storable
		;i 'IncludeAudit,objProp.Origin="MidasPlus.Base.audit" continue
		s ok=1
		s:ok props=props+1,props(props)=PropName_"^"_TypeName_"^"_objProp.Identity_"^"_objProp.Required
	}
	q sc
	;
]]></Implementation>
</Method>

<Method name="ExportClassData">
<ClassMethod>1</ClassMethod>
<FormalSpec>UCI:%String,Classes:%String,dir:%String="",verbose:%Boolean=0</FormalSpec>
<ReturnType>%Status</ReturnType>
<Implementation><![CDATA[
	; Function:
	;	Exports class data in tsv format
	; Inputs:
	;	UCI - Namespace from which to get classes.
	;	Classes - List of classes delimited by a comma
    ;	dir - directory to save tsv files 
	;   verbose - if 1 then messages (information and errors) are shown
	; Returns:
	;	%Status
	;####
	
	i '..NamespaceOK(UCI) {
		w:verbose !,UCI_" does not exist."
		q $$$ERROR($$$GeneralError,"ExportClassData: "_UCI_" does not exist.")
	}
	s sc=1
	s errs=""
	s:$g(dir)="" dir="c:\temp\OURClassData\"
	s direct=$s($e(dir,$l(dir),$l(dir))'="\":dir_"\",1:dir)
	i '##class(%File).DirectoryExists(direct) {
		w:verbose !,"ExportSmarTrackData: ",direct," doesn't exist."
		q $$$ERROR($$$GeneralError,"ExportSmarTrackData: " _direct_" doesn't exist.")
	}	
	s CurrentUCI=$ZNSPACE
	s $znspace=UCI
	f c=1:1:$l(Classes,",") {
		s IsError=0,msg=""
		s class=$p(Classes,",",c)
		s objClass=##class(%Dictionary.CompiledClass).%OpenId(class)
		i '$IsObject(objClass) {
			s IsError=1,msg=class_" does not exist in "_UCI_" namespace."
		}
		ElseIf objClass.ClassType'="persistent" {
			s IsError=1,msg=class_" is not a persistent class."
		}
		ElseIf objClass.Abstract {
			s IsError=1,msg=class_" is an abstract class. It cannot be exported."
		}
		i IsError {
			s errs=$s(errs="":msg,1:errs_$c(13,10)_msg)
			continue
		}
		i ..HasStreamProperty(class) {
				s sc=##class(OUR.Utils.StoredProcs).ExportClassGlobals(class,UCI,direct,0)
				if sc {
					s fname=direct_class_"_data.xml"
					w:verbose !,class_" global data saved to "_fname
				}
				else {
					s err=..ObjectErrorMessage(sc)
					s errs=$s(errs="":err,1:errs_$c(13,10)_err)
				}
				continue
		}
		k props,line,lines,p,TimeStamp,prp
		s nLine=0,line="",props=""
		d ..GetProperties(class,.p)
		i p {
			f i=1:1:p {
				s prop=$p(p(i),"^",1),TypeName=$p(p(i),"^",2),IsIdentity=$p(p(i),"^",3),required=$p(p(i),"^",4)
                
				s:IsIdentity IdProp=prop
				i ((TypeName["TimeStamp" )|| (TypeName="%Library.DateTime") || (TypeName="%DateTime")) {
					 s TimeStamp(prop)=""
				}
				
				i (TypeName["%Date")||(TypeName["%Library.Date") s Date(prop)=""
				
				i 'IsIdentity {
					s prp(i)=prop
					s line=$s(line="":prop,1:line_$c(9)_prop)
					s fld=..FixReservedWord(prop)
					/*
					if $d(Stream(prop)) {
						s fld="CONVERT(VARCHAR(1000000),"_fld_") As "_prop_"_Text"
                        s prp(i)=prop_"_Text"
                        k Stream(prop)
                        s Stream(prop_"_Text")=""
					}
					*/
					s props=$s(props="":fld,1:props_","_fld)
				}
			}
			i $g(IdProp)'="" {
				i IdProp="ID" {
					s last=$o(prp(""),-1)
					s prp(last+1)=IdProp
					
				}
				else {
					s prp(0)=IdProp
				}
				s line=$s(IdProp="ID":line_$c(9)_IdProp,1:IdProp_$c(9)_line)
				s props=$s(IdProp'="ID":IdProp_","_props,1:props_","_IdProp)
			}
			; Heading
			s nLine=nLine+1,lines(nLine)=line
			; Get values
			s cls=..FormatQueryClass(class)
			s sql="Select "_props_" From "_cls
			s rs=##class(%ResultSet).%New()
			s sc=rs.Prepare(sql)
			i 'sc {
				s err=..ObjectErrorMessage(sc)
				s errs=$s(errs="":err,1:errs_$c(13,10)_err)
				continue
			}
			s sc=rs.Execute()
			i 'sc {
				s err=..ObjectErrorMessage(sc)
				s errs=$s(errs="":err,1:errs_$c(13,10)_err)
				continue
			}
			while rs.%Next() {
				s line=""
				s i="" f {
					s i=$o(prp(i)) q:i=""
					s prop=prp(i)
					s val=rs.Data(prop)
					i val'="" {
						; make date and time values more readable
						i $d(TimeStamp(prop)) {s val=..ConvertTimeStamp(val)}
						ElseIf $d(Date(prop)),val?1N.N {s val=$zdt(val)}
					}
					s:val[$c(9) val=$tr(val,$c(9)," ")
					i val[$c(13,10) {
						s:val[$c(13,10) val=$tr(val,$c(13,10)," "_$c(16))
					}
					s line=$s(line="":val,1:line_$c(9)_val)
				}
				s nLine=nLine+1,lines(nLine)=line
			}
			d rs.Close()

			s fname=direct_$tr(cls,".","_")_".tsv"
			d ##class(%File).Delete(fname)
			s f=##class(%File).%New(fname)
			d f.Open("WSN")
			s line="" f {
				s line=$o(lines(line)) q:line=""
				d f.WriteLine(lines(line))
			}
			d f.Close()
			w:verbose !,class_" data saved to "_fname
		}
	}
	s $znspace=CurrentUCI
	i errs'="" {
		w:verbose !,errs
		q $$$ERROR($$$GeneralError,"ExportClassData: "_$c(13,10)_errs)
	}			
	q sc
]]></Implementation>
</Method>

<Method name="GetClassStorage">
<ClassMethod>1</ClassMethod>
<FormalSpec><![CDATA[class:%String,&globals,StorageType:%String]]></FormalSpec>
<ReturnType>%Status</ReturnType>
<Implementation><![CDATA[
	s sc=1
	i class'="" {
		s StorageType=$g(StorageType)
		s objClass=##class(%Dictionary.ClassDefinition).%OpenId(class)
		i $IsObject(objClass) {s super=objClass.Super}
		i StorageType'="" {
			s stype=$$$ucase(StorageType)
			f i=1:1:$l(stype,",") {
				s t=$p(stype,",",i)
				s type(t)=""
			}
		}
		s sql="Select DataLocation,IndexLocation,IdLocation,StreamLocation From %Dictionary.StorageDefinition "
		s sql=sql_" Where Parent = '"_class_"'"
		s rs=##class(%ResultSet).%New()
		s sc=rs.Prepare(sql) 
		s sc=rs.Execute() 
		s n=0		
		while rs.%Next() {
			s IdLoc=rs.Data("IdLocation")
			s v=rs.Data("DataLocation")
			i (StorageType="")||($d(type("D"))),v'="" {
				i v'["%%PARENT" {s globals(class,v)="",n=n+1}
				Else {s:IdLoc'="" globals(class,IdLoc)=""}
			}
			s v=rs.Data("IndexLocation")
			i (StorageType="")||($d(type("I"))),v'="" s globals(class,v)="",n=n+1
			s v=rs.Data("StreamLocation")
			i (StorageType="")||($d(type("S"))),v'="" s globals(class,v)="",n=n+1
		}
		d rs.Close()
		i 'n,super'="" {
			f i=1:1:$l(super,",") {
				s cls=$p(super,",",i)
			 	i $e(cls,1)'="%" d ..GetClassStorage(cls,.globals,StorageType)
			}
		}
	}	
	q sc
]]></Implementation>
</Method>

<Method name="GetGlobals">
<ClassMethod>1</ClassMethod>
<FormalSpec><![CDATA[ClassMask:%String,&globals,StorageType:%String]]></FormalSpec>
<ReturnType>%Status</ReturnType>
<Implementation><![CDATA[
	s sc=1
	s StorageType=$g(StorageType)
	i ClassMask'="" {
		f i=1:1:$l(ClassMask,",") {
			s msk=$p(ClassMask,",",i)
			i $d(^oddDEF(msk)) {
				d ..GetClassStorage(msk,.globals,StorageType)
			}
			Else {
				s cls=msk f {
					s cls=$o(^oddDEF(cls)) 
					i cls=""||($e(cls,1,$l(msk))'=msk) q
					d ..GetClassStorage(cls,.globals,StorageType)
				}
			}
		}		
	}	
	q sc
]]></Implementation>
</Method>

<Method name="ImportClassGlobals">
<ClassMethod>1</ClassMethod>
<FormalSpec>ClassMask:%String,UCI:%String="",dir:%String="",verbose:%Boolean=0</FormalSpec>
<ReturnType>%Status</ReturnType>
<Implementation><![CDATA[
	s sc=1
	i $g(ClassMask)'="" {
		s:$g(UCI)="" UCI=$znspace
		i '..NamespaceOK(UCI) {
			w:verbose !,UCI_" does not exist."
			q $$$ERROR($$$GeneralError,"ImportClassGlobals: "_UCI_" does not exist.")
		}
		s:$g(dir)="" dir="c:\temp\OURClassGlobalData\"
		s direct=$s($e(dir,$l(dir),$l(dir))'="\":dir_"\",1:dir)
		i '##class(%File).DirectoryExists(direct) {
			w:verbose !,"ImportClassGlobals: ",direct," doesn't exist."
			q $$$ERROR($$$GeneralError,"ImportClassGlobals: " _direct_" doesn't exist.")
		}
		s CurrentUCI=$znspace
		s $znspace=UCI
		s errs=""
		s opt=$s(verbose:"d",1:"-d")
		f i=1:1:$l(ClassMask,",") {
			s msk=$p(ClassMask,",",i)
			i $d(^oddDEF(msk)) {
				s FileName=direct_msk_"_Data.xml"
				i '##class(%File).Exists(FileName) {
					w:verbose !,"ImportClassGlobals: "_FileName_" doesn't exist."
					s msg="ImportClassGlobals: "_FileName_" doesn't exist."
					s errs=$s(errs="":msg,1:errs_$c(13,10)_msg)		
                    continue
				}
				d $system.OBJ.Load(FileName,opt)
			}
			Else {
				s cls=msk f {
					s err=0
					s cls=$o(^oddDEF(cls)) 
					i cls=""||($e(cls,1,$l(msk))'=msk) q
					s FileName=direct_cls_"_Data.xml"
					i '##class(%File).Exists(FileName) {
						w:verbose !,"ImportClassGlobals: "_FileName_" doesn't exist."
						s msg="ImportClassGlobals: "_FileName_" doesn't exist."
						s errs=$s(errs="":msg,1:errs_$c(13,10)_msg)			
                        continue
					}
					d $system.OBJ.Load(FileName,opt)
				}
			}
		}		
		s $znspace=CurrentUCI
		
		i errs'="" {
			q $$$ERROR($$$GeneralError,"ExportClassData: "_$c(13,10)_errs)
		}			
			
	}
	else {
			w:verbose !,"ImportClassGlobals: No class mask was specified."
			q $$$ERROR($$$GeneralError,"ImportClassGlobals: No class mask was specified.")
	}
	
	q sc
]]></Implementation>
</Method>

<Method name="ExportClassGlobals">
<ClassMethod>1</ClassMethod>
<FormalSpec>ClassMask:%String,UCI:%String="",dir:%String="",verbose:%Boolean=0</FormalSpec>
<ReturnType>%Status</ReturnType>
<Implementation><![CDATA[
	s sc=1
	i $g(ClassMask)'="" {
		s:$g(UCI)="" UCI=$znspace
		i '..NamespaceOK(UCI) {
			w:verbose !,UCI_" does not exist."
			q $$$ERROR($$$GeneralError,"ExportClassGlobals: "_UCI_" does not exist.")
		}
		s:$g(dir)="" dir="c:\temp\OURClassGlobalData\"
		s direct=$s($e(dir,$l(dir),$l(dir))'="\":dir_"\",1:dir)
		i '##class(%File).DirectoryExists(direct) {
			w:verbose !,"ExportClassGlobals: ",direct," doesn't exist."
			q $$$ERROR($$$GeneralError,"ExportClassGlobals: " _direct_" doesn't exist.")
		}	
		s CurrentUCI=$znspace
		s $znspace=UCI
		d ..GetGlobals(ClassMask,.globals)
		i $d(globals) {
			s opt=$s(verbose:"d",1:"-d")
			s cls="" f {
				s cls=$o(globals(cls)) q:cls=""
				s GloList=""
				s FileName=direct_cls_"_Data.xml"
				s g="" f {
					s g=$o(globals(cls,g)) q:g=""
					s GloList=$s(GloList="":g_".GBL",1:GloList_","_g_".GBL")
				}
				s sc=$system.OBJ.Export(GloList,FileName,opt)
			}
		}
		else {
			s $znspace=CurrentUCI
		}
		s $znspace=CurrentUCI
		
	}
	else {
			w:verbose !,"ExportClassGlobals: No class mask was specified."
			q $$$ERROR($$$GeneralError,"ExportClassGlobals: No class mask was specified.")
	}
	q sc
]]></Implementation>
</Method>

<Method name="KillClassGlobals">
<ClassMethod>1</ClassMethod>
<FormalSpec>ClassMask:%String</FormalSpec>
<ReturnType>%Status</ReturnType>
<Implementation><![CDATA[
	s sc=1
	i ClassMask'="" {
		d ..GetGlobals(ClassMask,.globals)
		i $d(globals) {
			s dir="c:\temp\OURClassGlobalData\"
			s cls="" f {
				s cls=$o(globals(cls)) q:cls=""
				s GloList=""
				s FileName=dir_cls_"_Data.xml"
				s g="" f {
					s g=$o(globals(cls,g)) q:g=""
					s GloList=$s(GloList="":g_".GBL",1:GloList_","_g_".GBL")
				}
				s sc=$system.OBJ.Export(GloList,FileName,"-d")
			}
			s cls="" f {
				s cls=$o(globals(cls)) q:cls=""
				s glo="" f {
					s glo=$o(globals(cls,glo)) q:glo=""
					k @glo
				}
			}
		}
	}	
	q sc
]]></Implementation>
</Method>

<Method name="HasStreamProperty">
<ClassMethod>1</ClassMethod>
<FormalSpec>class:%String</FormalSpec>
<ReturnType>%Boolean</ReturnType>
<Implementation><![CDATA[
	s ret=0
	i ..ClassExists(class) {

		d ..GetProperties(class,.p)
		if p {
			f i=1:1:p {
				s TypeName=$p(p(i),"^",2)
				i TypeName["%Stream" {
					s ret=1
					q
				}	
			}
		}
	}
	q ret
]]></Implementation>
</Method>

<Method name="ImportClassData">
<ClassMethod>1</ClassMethod>
<FormalSpec>UCI:%String,Classes:%String,dir:%String="",verbose:%Boolean=0</FormalSpec>
<ReturnType>%Status</ReturnType>
<Implementation><![CDATA[
	i '..NamespaceOK(UCI) {
		w:verbose !,UCI_" does not exist."
		q $$$ERROR($$$GeneralError,"ExportClassData: "_UCI_" does not exist.")
	}
	s sc=1,errs=""
	s:$g(dir)="" dir="c:\temp\OURClassData\"
	s direct=$s($e(dir,$l(dir),$l(dir))'="\":dir_"\",1:dir)
	i '##class(%File).DirectoryExists(direct) {
		w:verbose !,"ImportClassData: ",direct," doesn't exist."
		q $$$ERROR($$$GeneralError,"ImportClassData: " _direct_" doesn't exist.")
	}	
	s CurrentUCI=$ZNSPACE
	s $znspace=UCI
	s IdentityInsert=$system.SQL.GetIdentityInsert()
	d $system.SQL.SetIdentityInsert(1)
	d $system.SQL.SetFilerRefIntegrity(0,.OldRefIntFlag)
		
	f c=1:1:$l(Classes,",") {
		s IsError=0,msg=""
		k props,line,lines,p,stream
		s nLine=0,line="",props=""
		s class=$p(Classes,",",c)
		i '..ClassExists(class) {
			s msg=class_" does not exist."
			s errs=$s(errs="":errs=msg,1:errs_$c(13,10)_msg)
			continue
		}
		if ..HasStreamProperty(class) {
			s sc=##class(OUR.Utils.StoredProcs).ImportClassGlobals(class,UCI,direct,0)
			i 'sc {
				s msg=..ObjectErrorMessage(sc)
				s errs=$s(errs="":errs=msg,1:errs_$c(13,10)_msg)
			}
			continue
		}
		s fname=direct_$tr(class,".","_")_".tsv"
		i '##class(%File).Exists(fname) {
			;w:verbose !,"ImportClassData: "_fname_" doesn't exist."
			s msg="ImportClassData: "_fname_" doesn't exist."
			s errs=$s(errs="":msg,1:errs_$c(13,10)_msg)			
			continue
		}
		s objClass=##class(%Dictionary.CompiledClass).%OpenId(class)
		i '$IsObject(objClass) {
			s IsError=1,msg=class_" does not exist in "_UCI_" namespace."
		}
		ElseIf objClass.ClassType'="persistent" {
			s IsError=1,msg=class_" is not a persistent class."
		}
		ElseIf objClass.Abstract {
			s IsError=1,msg=class_" is an abstract class. It cannot be exported."
		}
		i IsError {
			s errs=$s(errs="":errs=msg,1:errs_$c(13,10)_msg)
			continue
		}
		d ..GetProperties(class,.p)
		k prop,type,Id,TimeStamp
		if p {
			f i=1:1:p {
				s prop=$p(p(i),"^",1),TypeName=$p(p(i),"^",2),IsIdentity=$p(p(i),"^",3),required=$p(p(i),"^",4)
				s:IsIdentity Id(prop)=""
				s:required Required(prop)=""
				i (TypeName["TimeStamp" )|| (TypeName="%Library.DateTime") || (TypeName="%DateTime") {
					 s TimeStamp(prop)=""
				}
				i TypeName["%Stream" s stream(prop)=""
				s prop(prop)="",type(prop)=TypeName
			}
			s fstream=##class(%FileCharacterStream).%New()
			s fstream.Filename=fname,n=0
			while 'fstream.AtEnd {
				s line=fstream.ReadLine()
				i 'n s header=line
				s:n lines(n)=line
				s n=n+1
			}
			d fstream.%Close()
			k fstream
			f i=1:1:$l(header,$c(9)) {
				s col(i)=$p(header,$c(9),i)
			}
			d $system.SQL.PurgeForTable(class)
			s qclass=$s($l(class,".")>2:$tr($p(class,".",1,$l(class,".")-1),".","_")_"."_$p(class,".",$l(class,".")),1:class)
			d ..KillClassGlobals(class)
			s n=0
			k vals
			f {
				s n=$o(lines(n)) q:n=""
				s line=lines(n)
				f i=1:1:$l(line,$c(9)) {
					s prop=col(i)
					continue:'$d(prop(prop))
					s val=$p(line,$c(9),i)
					i val'="" {
						i $d(TimeStamp(prop)) {
							s val=$zdatetimeh(val,5)
							s val=$zdt(val,3,1)
						}
						ElseIf (type(prop)["%Date")||(type(prop)["%Library.Date") {
							s val=$zdh(val)
						}
						ElseIf $d(stream(prop)) {
							s val=$replace(val,$c(16),$c(13,10))
						}
					}
					ElseIf val="",$d(Required(prop)) {
						s type=$g(type(prop))
						i type["String" {
							s val=" "
						}
						ElseIf type["Integer" {
							s val=-1
						}
						ElseIf (type="%Library.Date")||(type="%Date") {
							s val=+$h
						}
						ElseIf type["Boolean" {
							s val=$s(prop["Active":1,1:0)
						}
						ElseIf $d(TimeStamp(prop)) {
							s val=$zdt($zts,3,1,2)
						}
					}
					;take care of any quotes in value
					
					;take care of any single quotes in value
					i val["'" {
						s nv=$l(val,"'")
						f ii=1:1:nv {
							s v(ii)=$p(val,"'",ii)
						}
						s v=""
						f ii=1:1:nv-1 {
							s v=v_v(ii)_"''"
						}
						s val=v_v(nv)
					}
					s vals(prop)=val
				}
				; check for required fields that are not included in the file
				s prop="" f {
					s prop=$o(Required(prop)) q:prop=""
					i '$d(vals(prop)) {
						s val=""
						s type=$g(type(prop))
						i type["String" {
							s val=" "
						}
						ElseIf type["Integer" {
							s val=-1
						}
						ElseIf (type="%Library.Date")||(type="%Date") {
							s val=+$h
						}
						ElseIf type["Boolean" {
							s val=$s(prop["Active":1,1:0)
						}
						ElseIf $d(TimeStamp(prop)) {
							s val=$zdt($zts,3,1,2)
						}
						s vals(prop)=val
					}
				}
				;				
				s sql="Insert Into "_qclass_" set "
				; Create set value string for sql statement
				s vallist=""
				s prop="" f {
					s prop=$o(vals(prop)) q:prop=""
					i $d(prop(prop)),vals(prop)'="" {
					  s fld=..FixReservedWord(prop)
					  s var=fld_"='"_vals(prop)_"'"
					  s vallist=$s(vallist="":var,1:vallist_", "_var)	
					}
				}
				; save the record
				i vallist'="" {
					s sql=sql_vallist
					s rs=##class(%ResultSet).%New()
					s sc=rs.Prepare(sql)
					i 'sc {
						s msg="ImportClassData Prepare: "_..ObjectErrorMessage(sc)
						s errs=$s(errs="":msg,1:errs_$c(13,10)_msg)			
						continue
					} 
					s sc=rs.Execute()
					i 'sc {
						s msg="ImportClassData Execute: "_..ObjectErrorMessage(sc)
						s errs=$s(errs="":msg,1:errs_$c(13,10)_msg)			
						continue
					}
					d rs.Close()
					k rs
				}				
			}
		}
	}
	d $system.SQL.SetIdentityInsert(IdentityInsert)
	d $system.SQL.SetFilerRefIntegrity(1)
	
	s $znspace=CurrentUCI
	i errs'="" {
		w:verbose !,errs
		q $$$ERROR($$$GeneralError,"ExportClassData: "_$c(13,10)_errs)
	}
	q sc
]]></Implementation>
</Method>
</Class>
</Export>
