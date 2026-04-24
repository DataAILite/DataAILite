<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ReportEdit.aspx.vb" Inherits="ReportEdit" %>

<script type="text/javascript" src="Controls/Javascripts/OUR.js"></script>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Create/Edit Report</title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
            height: 50px;
        }
        .auto-style2 {
            width: 101%;
        }
        .auto-style3 {
            width: 100%;
            height: 27px;
        }
        .auto-style9 {
            width: 10%;
        }
        .auto-style10 {
            width: 10%;
            height: 34px;
        }
        .auto-style11 {
            height: 34px;
        }
        .auto-style12 {
            width: 10%;
            height: 45px;
        }
        .auto-style13 {
            height: 45px;
            width: 1231px;
        }
        .auto-style15 {
            height: 138px;
        }
        .auto-style17 {
            height: 1px;            
        }
        .auto-style18 {
            height: 131px;
        }
        .auto-style19 {
            height: 10px;
        }
        .auto-style20 {
            height: 27px;
        }
        #tdParameterList {
            height: 27px;
            width: 1451px;
            border-width: 0px;
            background-color:#C0C0C0;
            color:white;
            font-family:Arial;
            font-size:small;
            text-wrap:none;
        }
#divParamList {
            padding: 0px; 
            font-family: Arial; 
            font-size: x-large; 
            color: #FFFFFF; 
            background-color: LightGray;
            border-width: 0px;  
            width: 100%;
        }
#divParamList1 {
            padding: 3px 0px 4px 0px; 
            border-width: 0px; 
            display: inline-block; 
            width: 89%; 
            /*width: 100%;*/
            height: 25px;
            border-width: 0px;
        }
#divParamList2 {
            padding: 0px; 
            border-width: 0px; 
            display: inline-block; 
            width: 10%; 
            height: 35px;
        }
.ParamButtonStyleSubmit {
        width: 100px;
        height: 30px;
        font-size: 12px;
        border-radius: 6px;
        border-style :solid;
        border-color: ButtonFace;
        color: black;
        border-width: 1px;
        background-color: ButtonFace ;
        padding:0px;
        margin:0px;
        z-index: 9999;
     }
 .ParamButtonStyle {
        height: 25px;
        font-size: 12px;
        border-radius: 5px;
        border-style :solid;
        border-color: ButtonFace ;
        color: black;
        border-width: 1px;
        background-color: ButtonFace;
        padding: 3px;
        margin:0px;
        z-index: 9999;
     }
.paramcolumns {
       height: 23px;
       border-width: 0px;
       padding: 0px;
       margin: 0px;
       font-family: Arial;
       font-size: small;
       font-weight: bold;
       color: white;
       background-color: #C0C0C0;
       text-wrap:normal;
       table-layout:fixed;
       overflow:hidden;
   }
#trParameters {
       border-width: 0px;
       background-color: #C0C0C0;
       border-bottom: 1px solid black;
    }
#tblParameters {
       width:100%;
       border:1px solid black;
       background-color:white;
       color:black;
       font-family:Arial;
       font-size :small;
       border-width :0px;
   }
#divMenu {
    width:100%;
    height:20px;
    background:Gray;
    border-width:0px;
    padding:0px;
}
.mnuDefine {
    width:120px;
    height:20px;
    background:Gray;
    border-width:0px;
    padding:20px;
}
        .auto-style23 {
            width: 19%;
            height: 60px;
        }
        .auto-style24 {
            height: 60px;
        }
        .auto-style25 {
            width: 10%;
            height: 101px;
        }
        .auto-style27 {
            margin-top: 0px;
        }
.modal
{
    position: fixed;
    z-index: 2147483647;
    height: 100%;
    width: 100%;
    top: 0;
    background-color: #f8f8d3;
    opacity: 0.8;

}
.center
{
    z-index: 2147483647;
    margin: 300px auto;
    padding-left:25px;
    padding-top:10px;
    width: 130px;
    background-color:#f8f8d3;
    border-radius: 10px;
    opacity: 1;
}
.center img
{
    height: 100px;
    width: 100px;
}

.NodeStyle
{
    color: #0066FF;
    font-size:12px;
    font-weight:normal;
    text-decoration:none;
}
.NodeStyle:hover
{
    text-decoration:underline;
    color:darkblue;
}
.LabelStyle {
    margin:5px;
}
        .auto-style28 {
            height: 101px;
        }
 .ticketbutton 
{
  width: 30px;
  height: 25px;
  font-size: 12px;
  border-radius: 5px;
  border-style :solid;
  border-color: #4e4747 ;
  color: black;
  border-width: 1px;
  /*background-color: ButtonFace;*/
  background-image: linear-gradient(to bottom, rgba(211, 211, 211,0),rgba(211, 211, 250,3));

  padding: 3px;
  margin:5px;
  z-index: 9999; 
}
        </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />      
        <asp:UpdatePanel ID="udpReportEdit" runat ="server">
          <ContentTemplate>
              <asp:HiddenField ID="hfSizeLimit" runat="Server" Value="4096" />
  <table>
      <tr>
         <td colspan="3" style="text-align:left; font-size: x-large; font-style: normal; font-weight: bold; background-color: #e5e5e5; vertical-align: middle; height: 40px;">
              <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label>
          </td>
      </tr> 
        <tr>
        <td style="font-size: x-small; font-style: normal; font-weight: normal; background-color: #e5e5e5; vertical-align: top; text-align: left; width: 15%;">
           <div id="tree" style="font-size: x-small; font-weight: normal; font-style: normal; padding-top: 8px;">
             <asp:TreeView ID="TreeView1"  runat="server" Width="100%" NodeIndent="10" Font-Names="Times New Roman"  EnableTheming="True" ImageSet="BulletedList">
          <Nodes>  
                  <asp:TreeNode Text="&lt;b&gt;Log Off;&lt;/b&gt;"  Value="Default.aspx" Expanded="True" ></asp:TreeNode>

                  <asp:TreeNode Text="&lt;b&gt;List of Reports&lt;/b&gt;" Expanded="True" Value="ListOfReports.aspx"></asp:TreeNode>
                 
                 <asp:TreeNode Text="&lt;b&gt;Report Definition&lt;/b&gt;"  Value="ReportEdit.aspx?tne=2" Expanded="True" ></asp:TreeNode>

                 <asp:TreeNode Text="&lt;b&gt;Report Parameters&lt;/b&gt;"  Value="ReportEdit.aspx?tne=3" Expanded="True" ></asp:TreeNode>

                 <%--<asp:TreeNode Text="&lt;b&gt;Share Report (Users)&lt;/b&gt;"  Value="ReportEdit.aspx?tne=4" Expanded="True" ></asp:TreeNode>--%>
                
                 <asp:TreeNode Text="Report Data Query"  Value="SQLquery.aspx?tnq=0" Expanded="True" SelectAction="Select">
                   <asp:TreeNode Text="Data fields" Value="SQLquery.aspx?tnq=0" > </asp:TreeNode>
                   <asp:TreeNode Text="Joins"       Value="SQLquery.aspx?tnq=1" > </asp:TreeNode>
                   <asp:TreeNode Text="Filters"     Value="SQLquery.aspx?tnq=2" > </asp:TreeNode>
                   <asp:TreeNode Text="Sorting"     Value="SQLquery.aspx?tnq=3" > </asp:TreeNode>                 
            </asp:TreeNode>
            
                 <asp:TreeNode Text="Report Format Definition"  Value="RDLformat.aspx?tnf=0" Expanded="True" >
                   <asp:TreeNode Text="Advanced Report Designer"  Value="ReportDesigner.aspx" > </asp:TreeNode>
                   <asp:TreeNode Text="Columns, Expressions"  Value="RDLformat.aspx?tnf=0" > </asp:TreeNode>
                   <asp:TreeNode Text="Groups, Total"          Value="RDLformat.aspx?tnf=1" > </asp:TreeNode>
                   <asp:TreeNode Text="Combine Values"          Value="RDLformat.aspx?tnf=2" > </asp:TreeNode>
                   <asp:TreeNode Text="Map Definition"          Value="MapReport.aspx" > </asp:TreeNode>
            </asp:TreeNode>
           
                 <asp:TreeNode Text="Explore Report Data"  Value="ShowReport.aspx?srd=0" Expanded="True" >
                   <asp:TreeNode Text="Export Data to Excel" Value="datatoExcel" NavigateUrl="ShowReport.aspx?srd=1"> </asp:TreeNode>
                   <asp:TreeNode Text="Export Data to CSV"    Value="datatoCSV" NavigateUrl="ShowReport.aspx?srd=2" > </asp:TreeNode>                 
                   <asp:TreeNode Text="Export Data to Delimited File"  value="ShowReport" Navigateurl="ShowReport.aspx?srd=10" > </asp:TreeNode>
                   <asp:TreeNode Text="Export Data to XML"    Value="datatoXML" NavigateUrl="ShowReport.aspx?srd=14" > </asp:TreeNode> 

                </asp:TreeNode>
                 <asp:TreeNode Text="Show Report"  Value="ShowReport.aspx?srd=3" Expanded="True" >
                   <asp:TreeNode Text="Show Generic Report"  Value="ReportViews.aspx?gen=yes" > </asp:TreeNode>
                    <asp:TreeNode Text="Show Report Charts"  Value="ShowReport.aspx?srd=17" > </asp:TreeNode>
                   <asp:TreeNode Text="Export Report to Excel"  Value="reptoExcel" NavigateUrl="ShowReport.aspx?srd=4" > </asp:TreeNode>
                   <asp:TreeNode Text="Export Report to Word"    Value="reptoWord" NavigateUrl="ShowReport.aspx?srd=5" > </asp:TreeNode>
                   <asp:TreeNode Text="Export Report to PDF"  Value="reptoPDF" NavigateUrl="ShowReport.aspx?srd=6" > </asp:TreeNode>
                   <asp:TreeNode Text="See Crystal Report"    Value="ShowReport.aspx?srd=7"> </asp:TreeNode>
                 </asp:TreeNode>

                 <asp:TreeNode Text="Show Analytics"  Value="ShowReport.aspx?srd=11" Expanded="True" >
                   <asp:TreeNode Text="See Data Overall Statistics"  Value="ShowReport.aspx?srd=8" > </asp:TreeNode>
                   <asp:TreeNode Text="Export Overall Statistics to Excel"  Value="reptoExcel" NavigateUrl="ShowReport.aspx?srd=9" > </asp:TreeNode>
                   <asp:TreeNode Text="See Groups Statistics"  Value="ReportViews.aspx?grpstats=yes" > </asp:TreeNode>                     
                   <asp:TreeNode Text="See Fields Correlation"  Value="ShowReport.aspx?srd=12" > </asp:TreeNode>
                   <asp:TreeNode Text="Matrix Balancing"  Value="ShowReport.aspx?srd=13" > </asp:TreeNode>
                 </asp:TreeNode>
               </Nodes>
               <RootNodeStyle HorizontalPadding="2px" Font-Bold="True" Font-Underline="False" />
               <NodeStyle CssClass="NodeStyle" />
               <ParentNodeStyle Font-Bold="True" />
            </asp:TreeView>
          </div>
        </td>
   
        <td style="width: 5px"></td>
        <td id="MainSection" style="vertical-align: top; text-align: left; width: 85%">  
            <table style="width:100%; font-family: Arial; font-size: xx-small;">
                <tr>
                <td style="width:15%;text-align:left;font-family: Arial; font-size: xx-small;">
                    <asp:CheckBox ID="chkAdvanced" runat="server" Text=" Advanced User"  AutoPostBack="True"  Font-Size="12px" />
                </td>
                <td style="width:40%;text-align:right">
                    <asp:Label ID="LabelAlert0" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large" ForeColor="Gray" Text="Report Definition - "></asp:Label>
                    <asp:Label ID="LabelAlert1" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large" ForeColor="Gray" Text="Label"></asp:Label>
                </td>
                <td style="width:30%;text-align:center;font-family: Arial; font-size: xx-small;">
                    <asp:Label ID="lblView" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Medium" ForeColor="#000099" Text="Report Info"></asp:Label>
                    <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/ListOfReports.aspx" Visible="false" CssClass="NodeStyle" Font-Size="12px">List of reports</asp:HyperLink>
                </td>
<%--                <td style="width:15%;text-align:center;font-family: Arial; font-size: xx-small;">
                    <asp:HyperLink ID="HyperLink1" runat="server"  NavigateUrl="~/ShowReport.aspx?srd=3"  Visible="False" Enabled="False" ToolTip="Show Report data" CssClass="NodeStyle" Font-Size="12px">Show report data</asp:HyperLink>    
                </td>--%>
                <td style="width:15%;text-align:center;font-family: Arial; font-size: xx-small;">
                    <asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx?hilt=Report_Info" Target="_blank" CssClass="NodeStyle" Font-Size="12px">Help</asp:HyperLink>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;

                    <asp:HyperLink ID="HyperLinkLogOff" runat="server" NavigateUrl="~/Default.aspx" CssClass="NodeStyle" Font-Names="Arial">Log off</asp:HyperLink> 
                </td>                                          
                </tr>
            </table>    
            <%--<br />--%>
<%--            <div style="width:100%; font-family: Arial; font-size: xx-small; text-align:center">
                       <asp:Label ID="lblView" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Medium" ForeColor="#000099" Text="Report Info"></asp:Label>
            </div>--%>
                                <br />
 <asp:Menu ID="MenuMain" Width="600px" runat="server" Orientation="Horizontal"  OnMenuItemClick="MenuMain_MenuItemClick" StaticSelectedStyle-BackColor="LightGray" DynamicSelectedStyle-BackColor="LightGray"    StaticSelectedStyle-Font-Bold="true"  StaticMenuItemStyle-BackColor="#e5e5e5" BorderWidth ="0px" Height="20px">
    <Items>
        <asp:MenuItem Text="Report Data Query" Value="0" ToolTip="Report Query to get data from db">
            <asp:MenuItem Text=" Data Fields" ToolTip="SELECT statement" Value="10"></asp:MenuItem>                    
            <asp:MenuItem Text=" Join Tables" ToolTip="JOIN statement" Value="11"></asp:MenuItem>
            <asp:MenuItem Text=" Filters " ToolTip="Filter data by defined conditions" Value="12"></asp:MenuItem>
            <asp:MenuItem Text=" Sorting" ToolTip="ORDER BY statement" Value="13"></asp:MenuItem>
        </asp:MenuItem>
        <asp:MenuItem Text=" Report Format " Value="1" ToolTip="Report RDL Format" >
            <asp:MenuItem Text=" Column Order, Expressions" ToolTip="Columns Order, functions" Value="20"></asp:MenuItem>
            <asp:MenuItem Text=" Groups and Totals" ToolTip="Grouping and Totals" Value="21"></asp:MenuItem>
            <asp:MenuItem Text=" Combine column values " ToolTip="Listing of column values" Value="22"></asp:MenuItem>
            <asp:MenuItem Text=" Advanced report designer " ToolTip="Advanced report designer" Value="23"></asp:MenuItem> 
            <asp:MenuItem Text=" Map definition " ToolTip="Map designer" Value="24"></asp:MenuItem>
        </asp:MenuItem>

        <asp:MenuItem Text=" Report Info" Value="2" ToolTip="Report Info and Report Files" ></asp:MenuItem>        
        <asp:MenuItem Text=" Parameters " Value="3" ToolTip="Report Parameters" ></asp:MenuItem>
        <asp:MenuItem Text=" Users " Value="4" ToolTip="Report Users" ></asp:MenuItem> 
       <%-- <asp:MenuItem Text=" Import Data " Value="5" ToolTip="Import Data to new or existing table in database" ></asp:MenuItem>--%>
    </Items>
</asp:Menu>

  <asp:MultiView ID="MultiView1" runat="server"  ActiveViewIndex="2"  >
       <asp:View ID="Tab1" runat="server">
        <table width="100%"cellpadding=0 cellspacing=0 border="0" >
            <tr valign="top">
                <td class="TabArea" style="width: 100%"/>             
            </tr>
        </table>
    </asp:View>
    <asp:View ID="Tab2" runat="server">
        <table width="100%" cellpadding=0 cellspacing=0 border="0" >
            <tr valign="top">
                <td class="TabArea" style="width: 100%"/>             
            </tr>
        </table>
    </asp:View>
       &nbsp;
   <asp:View ID="Tab3" runat="server"  >
        <table style="border-style:solid;border-width: 1px;padding:0px;margin:0px;background-color:#FFFFFF;width:100%" >
            <tr valign="top">
                <td style="width: 100%">
                    <table id="main" runat="server" style="border-style:none;border-width: 0px;padding:0px;margin:0px;background-color:#FFFFFF;width:100%">
                        <tr>
                            <td class="auto-style9" style="color: black; text-decoration: none; width:10%;">
                                <asp:Label ID="Label10" runat="server" Font-Names="Arial" Font-Size="24px" Text="Report Information: "></asp:Label></td>
                            <td align="left" style="color: white;">
                                <asp:Label ID="LabelEditOrNew" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr bgcolor="#e5e5e5" id="trEditRep0" runat="server" >
                            <td align="right" bgcolor="#e5e5e5" class="auto-style10" style="color: black; font-family: Arial; letter-spacing: normal;width:10%; ">
                                <asp:Label ID="Label1" runat="server" Text="Report ID: "></asp:Label>
                            </td>
                            <td align="left" width="90%" class="auto-style11">
                                <asp:TextBox ID="TextBoxReportID" runat="server" Width="60%"></asp:TextBox>
                                <asp:Label ID="LabelReportID" runat="server" ForeColor="black" Text="Label"></asp:Label>
                                
                            </td>
                        </tr>
                        <tr bgcolor="#e5e5e5">
                            <td align="right" bgcolor="#e5e5e5" class="auto-style9" style="color: black; font-family: Arial; letter-spacing: normal;width:10%; ">
                                <asp:Label ID="Label3" runat="server" Text="Report Title:"></asp:Label>
                            </td>
                            <td align="left" width="90%">
                                <asp:TextBox ID="TextBoxReportTitle" runat="server" Width="98%" CausesValidation="True" AutoPostBack="True" ValidateRequestMode="Enabled"></asp:TextBox>
                            </td>
                        </tr>
                        <tr bgcolor="#e5e5e5" id="trReportType" runat="server" visible="false" >
                            <td align="right" bgcolor="#e5e5e5" class="auto-style9" style="color: black; font-family: Arial; letter-spacing: normal;width:10%; ">
                                <asp:Label ID="lblReportType" runat="server" Text="Report Type:"></asp:Label>
                            </td>
                            <td align="left">
                                <asp:DropDownList ID="DropDownReportType" runat="server" Visible="False" Width="129px">
                                    <asp:ListItem>rdl</asp:ListItem>
                                    <asp:ListItem>crystal</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr bgcolor="#e5e5e5" id="trEditRep1" runat="server" >
                            <td align="right" bgcolor="#e5e5e5" class="auto-style9" style="color: black; font-family: Arial; letter-spacing: normal;width:10%; ">
                                <asp:Label ID="Label4" runat="server" Text="Report Orientation:"></asp:Label>
                            </td>
                            <td align="left" >
                                <asp:DropDownList ID="DropDownOrientation" runat="server" Width="129px">
                                    <asp:ListItem>portrait</asp:ListItem>
                                    <asp:ListItem>landscape</asp:ListItem>
                                </asp:DropDownList>
                         
                            </td>
                        </tr>
                        <tr bgcolor="#e5e5e5" id="trEditRep2" runat="server" >
                            <td align="right" bgcolor="#e5e5e5" class="auto-style9" style="color: black; font-family: Arial; letter-spacing: normal;width:10%; ">
                                <asp:Label ID="Label5" runat="server" Text="Data Source:"></asp:Label>
                            </td>
                            <td align="left" width="90%" style="color: black; font-family: Arial; letter-spacing: normal; height: 24px;" >
                                <asp:DropDownList ID="DropDownReportAttributes" runat="server" Width="129px" AutoPostBack="True">                                
                                       <asp:ListItem>sql</asp:ListItem>
                                       <asp:ListItem>sp</asp:ListItem>
                                     </asp:DropDownList>
                                &nbsp;<asp:Label ID="LabelSP" runat="server" ForeColor="black">Stored Procedure:</asp:Label>
                                <%--<asp:DropDownList ID="DropDownSPs" runat="server" Width="655px" OnSelectedIndexChanged="Page_Load" AutoPostBack="True"> </asp:DropDownList>--%>
                                <asp:DropDownList ID="DropDownSPs" runat="server" Width="655px" AutoPostBack="True"> </asp:DropDownList>                                
                                <asp:TextBox ID="TextBoxNewSPname" runat="server" Width="2%" Visible="False"></asp:TextBox>
                            </td>
                        </tr>
                        <tr bgcolor="#e5e5e5" id="trEditRep3" runat="server" >
                            <td align="right" class="auto-style9" height="100px" style="color: black; font-family: Arial; letter-spacing: normal;width:10%; ">
                                <asp:Label ID="Label6" runat="server" Text="Data Query Text:"></asp:Label>
                                <br />
                             &nbsp;</td>
                            <td align="left" class="auto-style2" height="100px" width="90%">
                                <asp:CheckBox ID="chkUseSQLText" runat="server" Text="Use entered text for Query" Font-Names="Arial" Font-Size="Small" ForeColor="black" />
                                <br />
                                <asp:TextBox ID="TextBoxSQLorSPtext" runat="server" Height="100px" TextMode="MultiLine" Width="98%" Wrap="True"></asp:TextBox>
                                <br />
                                 <asp:Label ID="lblSelectFile" runat="server" Text="User rdl file to upload if needed:"  ToolTip="Browse RDL file to upload for the report."   Font-Names="Arial" Font-Size="Small" ForeColor="black"></asp:Label>
                                &nbsp;&nbsp;<asp:Button ID="btnBrowse" runat="server" Text="Browse..." ToolTip="Browse RDL file to upload for the report." width="10%" CssClass="ticketbutton"  />
                                &nbsp;<asp:Label ID="lblFileChosen" runat="server" Text=" "  Font-Names="Arial" Font-Size="Small" ForeColor="black"></asp:Label>&nbsp;&nbsp;
                                    
                                &nbsp;&nbsp;&nbsp;<asp:Button ID="ButtonUploadRDL" runat="server" Text="Upload the report definition file" ToolTip="Upload user customized RDL file" Width="24%" CssClass="ticketbutton" />
                                    &nbsp;&nbsp;&nbsp;<asp:Button ID="ButtonDownloadFile" runat="server" Text="Download report definition files" Width="24%"  ToolTip="Download report files to local directory" Visible="False" Enabled="False" />
                                 &nbsp;&nbsp;
                            </td>
                        </tr>
                        <tr bgcolor="#e5e5e5" id="trEditRep4" runat="server" visible="false" >
                            <td align="right" class="auto-style25" style="color: black; font-family: Arial; letter-spacing: normal;">
                                <asp:Label ID="Label9" runat="server" Text="Report Files :"></asp:Label>
                            </td>
                            <td align="left" style="color: black; font-family: Arial; letter-spacing: normal; " width="90%" class="auto-style28" >
                                <div id="divRDL1" runat="server" style="display:none;">
                                  <asp:Button ID="ButtonXSD" runat="server" Text="Create/update XSD for new report" Width="9px" Visible="False" />
                                <asp:Label ID="LabelRdlFile" runat="server" Font-Bold="False" Font-Size="Medium"> </asp:Label>
                                  <asp:Label ID="LabelRPT" runat="server" ForeColor="White"></asp:Label>
                                  <asp:Button ID="ButtonRDL" runat="server" Text="Create/update RDL file" Visible="False" Width="9px" />
                                </div>
                                
                                <asp:TextBox ID="txtURI" runat="server" Width="400px" AutoPostBack="True">https://</asp:TextBox>
                                <br />

                                <asp:Label ID="LabelTableName" runat="server" Text="Insert into new table:" CssClass="LabelStyle"></asp:Label>
                                <asp:TextBox ID="txtTableName" runat="server" Width="200px" CssClass="LabelStyle" AutoPostBack="True"> </asp:TextBox>
                                &nbsp;&nbsp;<asp:Label ID="LabelTables" runat="server" ForeColor="White" Text="or into existing table:" CssClass="LabelStyle"></asp:Label>  
                                <asp:DropDownList ID="DropDownTables" runat="server" AutoPostBack="True" Height="16px">  </asp:DropDownList>
                                <br />
                                &nbsp;<asp:CheckBox ID="chkboxClearTable" runat="server" Text="delete all records before upload" ToolTip="All records will be deteted from the selected table and saved in the table with DELETED attached to original table name." AutoPostBack="True" />
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                &nbsp;<asp:CheckBox ID="chkboxUniqueFields" runat="server" Text="field names in upload file are unique" ToolTip="The field names in file to upload have unique names." AutoPostBack="False" Checked="False" Visible="False" Enabled="False" />
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="LabelDelimiter" runat="server" Text="csv file delimiter:" ></asp:Label>
                                <asp:TextBox ID="TextboxDelimiter" runat="server" Width="16px" Height="18px">,</asp:TextBox>
                                
                               <br />  
                                <div id="divFileUpload" runat="server" style="margin:3px; display:flex">
                                &nbsp;&nbsp;&nbsp;<asp:Button ID="ButtonUploadFile" runat="server" Text="Upload formatted file into table(s)." ToolTip="Upload CSV, XML, XLS, XLSX, JSON, TXT-JSON file or Access table. It might take a long time... Please be patient." Width="24%" />
                                  
                                </div>
                                 <div id="divRDL2" runat="server" style="display:none;">
                                    <asp:Button ID="ButtonCrystal" runat="server" Enabled="False" Text="Create new Crystal RPT file" Visible="False" Width="29px" />
                                    &nbsp;<asp:Label ID="LabelXSD" runat="server" ForeColor="White"></asp:Label>
                                    &nbsp;<asp:Label ID="LabelRDL" runat="server" ForeColor="White"> </asp:Label>
                                    &nbsp;&nbsp;
                                    <asp:HyperLink ID="HyperLink5" runat="server" Enabled="False" Font-Names="Arial" Font-Size="Medium" Font-Underline="True" ForeColor="Blue" NavigateUrl="~/ReportEdit.aspx?DELREP=yes" ToolTip="Delete report file" Visible="False">del</asp:HyperLink>
                                    &nbsp;<asp:Label ID="LabelRdlRpt" runat="server" ForeColor="White" Visible="False">Report file:</asp:Label>
                                    &nbsp;&nbsp;
                                    <asp:TextBox ID="TextBoxReportFile" runat="server" Enabled="False" Visible="False" Width="61px"></asp:TextBox>
                                    <asp:Label ID="LabelRptFile" runat="server" Font-Bold="False" Font-Size="Medium" Visible="False">Select:</asp:Label>
                                    <input id="FileRPT" runat="server" class="auto-style21" type="file" visible="False" />
                                    <asp:Button ID="ButtonUploadRPT" runat="server" Text="Upload RPT" Visible="False" Width="36px" />
                                </div>
                            </td>
                        </tr>
                        <tr bgcolor="#e5e5e5" id="trEditRep5" runat="server">
                            <td align="right" style="color: black; font-family: Arial; letter-spacing: normal; " bgcolor="#e5e5e5" class="auto-style23">
                                <asp:Label ID="LabelPageFtr" runat="server" Text="Page Footer:"></asp:Label></td>
                            <td align="left" class="auto-style24" ><asp:TextBox ID="TextBoxPageFtr" runat="server" Width="98%" Height="50px" MaxLength="240" Rows="4" TextMode="MultiLine" CausesValidation="True" CssClass="auto-style27"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td align="right" class="auto-style9" bgcolor="#e5e5e5">
                               &nbsp; 
                            </td>
                            <td align="left" bgcolor="#e5e5e5" style="color: black; font-family: Arial; height: 8px; background-color: #e5e5e5">
                                <asp:CheckBox ID="chkRemoveReportFormating" runat="server" Text="delete previous report format: expressions, groups, lists, advanced designed items, etc..." ToolTip="All report format will be cleaned: expressions, groups, lists, advanced designed items, etc..." AutoPostBack="True" Font-Names="Arial" Font-Size="Small" ForeColor="black"/>
                                <br />
                                <%--&nbsp;&nbsp;&nbsp;&nbsp;--%>
                                <asp:Button ID="ButtonSubmit" runat="server" Text="Save" ToolTip="Submit Report Info and update XSD and RDL. No dashboards with this report are updating." BackColor="#7CDC99" width="40px" CssClass="ticketbutton" />  
                                
                               <asp:Label ID="Label2" runat="server" Text="&lt;--- !!!&nbsp; First save the report information then add parameters and users if needed." Font-Names="Arial" Font-Size="Small" ForeColor="black" ></asp:Label>
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="ButtonUpdateDashboards" runat="server" Text="Update the report in dashboards" ToolTip="Update dashboards which include this report if needed (data or definition changed). It might take a long time... " Width="24%" CssClass="ticketbutton" />
                                
                                
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
     </asp:View>
    <asp:View ID="Tab4" runat="server">
      <table style="border-width: 0px;padding:0px;margin:0px;background-color:#e5e5e5;width:100%">
            <tr id="trParameterList" runat="server" align="left" valign="top" visible="true" bgcolor="#e5e5e5" >
                <td id="tdParameterList" runat ="server" bgcolor="#e5e5e5" >
                    <%--<div id="divParamList" style="vertical-align: middle; text-align: left; height:30px; font-family: Arial, Helvetica, sans-serif; font-size: small; line-height: normal; display: inline-block;">--%>
                       <div id="divParamList1" style="height: 30px; vertical-align: middle; text-align: left; width: 100%; vertical-align: middle; background-color:#e5e5e5; font-family: Arial, Helvetica, sans-serif; font-size: small; font-weight: normal; color: #000000;">
                           Parameters: &nbsp;<asp:Label ID="lblParamNo" runat="server" Text=" "></asp:Label>
                           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                           <asp:Button ID="btnDefine" runat="server" width="100px" CssClass="ticketbutton" text="New Parameter" ToolTip="Define new parameter(s)" />
                           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:CheckBox ID="chkRelatedParameters" runat="server" Text="related parameters" AutoPostBack="True" Visible="True"></asp:CheckBox>
                        </div>
                        
                       <%-- <div id="divParamList1" style="vertical-align: top; text-align: center; width: 75%; height:30px;" >
                          
                            &nbsp;&nbsp; 
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                             
                        </div>--%>
                        
                    <%--</div>--%>
                    <table id="tblParameters" runat="server" rules="rows" visible="true">
                      <tr id="trParameters">
                        <td class="paramcolumns" style="width: 10%;">Field&nbsp; </td>
                        <td class="paramcolumns" style="width: 10%;">Label&nbsp; </td>
                        <td class="paramcolumns" style="width: 10%;">Parameter&nbsp;</td>
                        <td class="paramcolumns" style="width: 10%;">Type</td>
                        <td class="paramcolumns" style="width: 35%;">SQL </td>
                        <td class="paramcolumns" style="width: 20%;">Comments </td>
                        <td class="paramcolumns" style="width: 3%;">&nbsp;</td>
                        <td class="paramcolumns" style="width: 8%;">&nbsp; </td>
                        <td class="paramcolumns" style="width: 3%;">&nbsp;</td>
                        <td class="paramcolumns" style="width: 3%;">&nbsp; </td>
                      </tr>
                    </table>
                </td>
            </tr>
            <tr id="trParam" runat="server" style="display:none"  >
                <td class="auto-style1">
                    <table id="Table1" runat="server" bgcolor="#e5e5e5" border="1">
                        <tr>
                            <td align="left" style="color: black; font-family: Arial; letter-spacing: normal; " bgcolor="#e5e5e5" class="auto-style12">
                                    <asp:Label ID="Label7" runat="server" Text="Number of Parameters:"></asp:Label>&nbsp;&nbsp;&nbsp;<asp:Label ID="LabelNofParams" runat="server" Text=" "></asp:Label>

                            </td>
                            <td align="left" style="color: black; font-family: Arial; letter-spacing: normal;" class="auto-style13" >
                                    Click (for sql only) <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/Parameters.aspx" Font-Names="Arial" Font-Size="Medium" Font-Underline="True" ForeColor="Blue" ToolTip="Open page to select report parameters">select report parameters</asp:HyperLink>&nbsp;or assign them manually below (for sp and sql):
                                    <asp:Label ID="LabelSPParams" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Medium" ForeColor="White" Text=" ..."></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
         <tr id="trParam1" runat="server" style="display: none">
        <td align="left" valign="top" width="100%" style="height: 177px">
            <table id="TableParams" runat="server" bgcolor="#e5e5e5" border="1" rules="rows" style="font-size: small;
                color: black; font-family: Arial; background-color: #ffffff" width="100%" class="auto-style15">
                
                
                <tr>
                    <td align="left" valign="top" nowrap="nowrap" style="font-weight: bold; font-size: small; color: white;
                        font-family: Arial; height: 23px; background-color: #e5e5e5" width="40">
                        Field:<br />
                        <asp:TextBox ID="TextBoxID" runat="server" Width="40px"></asp:TextBox></td>
                        
                    <td align="left" style="font-weight: bold; font-size: small; width: 70px; color: white;
                        font-family: Arial; height: 23px; background-color: #e5e5e5" valign="top">
                        Label:<br />
                        <asp:TextBox ID="TextBoxLabel" runat="server" Height="16px" Width="70px"></asp:TextBox></td>
                     
                    <td align="left" style="font-weight: bold; font-size: small; width: 70px; color: white;
                        font-family: Arial; height: 23px; background-color: #e5e5e5" valign="top">
                        Parameter:<br />
                        <asp:TextBox ID="TextBoxField" runat="server" Width="70px"></asp:TextBox></td>
                        
                    <td align="left" style="font-weight: bold; font-size: small; width: 50px; color: white;
                        font-family: Arial; height: 23px; background-color: #e5e5e5" valign="top">
                        Type:<br />
                        <asp:DropDownList ID="DropDownListType" runat="server" Width="50px">
                            <asp:ListItem>nvarchar</asp:ListItem>
                            <asp:ListItem>int</asp:ListItem>
                            <asp:ListItem>datetime</asp:ListItem>
                        </asp:DropDownList>&nbsp;</td>
                        
                    <td align="left" style="font-weight: bold; font-size: small; width: 400px; color: white;
                        font-family: Arial; height: 23px; background-color: #e5e5e5" valign="top">
                        SQL:<br />
                        <asp:TextBox ID="TextBoxSQL" runat="server" Height="53px"  TextMode="MultiLine" Width="400"></asp:TextBox></td>
                        
                    <td align="left" style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                        height: 23px; background-color: #e5e5e5; width: 206px;" valign="top">
                        Comments:<br />
                        <asp:TextBox ID="TextComments" runat="server" Height="53px" TextMode="MultiLine"
                            Width="204px"></asp:TextBox></td>
                            
                    <td align="left" style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                        height: 23px; background-color: #e5e5e5" valign="top">
                        <br />
                        <asp:Button ID="ButtonAddParameter" runat="server" Text="Add Parameter" /></td>
                </tr>
                <tr>
                    <td colspan="7" style="font-weight:bold; color: #e5e5e5; font-family:Arial; height:10px;background-color: #e5e5e5; font-size: medium;">
                        
                        Parameters:</td>
                </tr>
                <tr>
                    <td align="left" nowrap="nowrap" style="font-weight: bold; font-size: small; color: black;
                        font-family: Arial; height: 27px; background-color: #e5e5e5" width="40">
                        ID&nbsp;</td>
                    <td align="left"  nowrap="nowrap" style="font-weight: bold; font-size: small; width: 70px; color: black;
                        font-family: Arial; height: 27px; background-color: #e5e5e5">
                        Label</td>
                    <td align="left"  nowrap="nowrap" style="font-weight: bold; font-size: small; width: 70px; color: black;
                        font-family: Arial; height: 27px; background-color: #e5e5e5" width="70">
                        Parameter</td>
                    <td align="left"  nowrap="nowrap" style="font-weight: bold; font-size: small; width: 50px; color: black;
                        font-family: Arial; height: 27px; background-color: #e5e5e5" width="50">
                        Type</td>
                    <td align="left"  nowrap="nowrap" style="font-weight: bold; font-size: small; width: 400px; color: black;
                        font-family: Arial; height: 27px; background-color: #e5e5e5" width="400">
                        SQL for Parameter</td>
                    <td align="left"  nowrap="nowrap" style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                        height: 27px; background-color: #e5e5e5; width: 206px;">
                        Comments</td>
                    <td align="left"  nowrap="nowrap" style="font-weight: bold; font-size: small; color: black; font-family: Arial;
                        height: 27px; background-color: #e5e5e5">
                        Edit</td>
                </tr>
                <tr>
                    <td colspan="7" style="font-weight: bold; color: black; font-family: Arial; background-color: #e5e5e5" class="auto-style17">
                    </td>
                
                </tr>
            </table>
        </td>
    </tr>
    </table>
            
    </asp:View>
    <asp:View ID="Tab5" runat="server">
        <table width="100%" cellpadding=0 cellspacing=0 border="0">
            <tr valign="top" bgcolor="#e5e5e5" >
                <td class="TabArea" style="width: 100%">
             <table id="Table2" runat="server" bgcolor="LightGray" border="0" width="100%">
                <%--<tr bgcolor="#e5e5e5">
                   <td align="left" style="color: black; font-family: Arial; letter-spacing: normal; " bgcolor="LightGray" class="auto-style20" >
                       <asp:Label ID="Label8" runat="server" Text="Number of Users:"></asp:Label>
                       &nbsp;&nbsp;&nbsp;
                       <asp:Label ID="LabelNofUsers" runat="server" Text=" "></asp:Label>
                   </td>
                   <td align="left" style="color: black; font-family: Arial; letter-spacing: normal;" class="auto-style20">
                   </td>
                </tr>--%>
                <tr>
                   <td align="left" width="100%" colspan=2 class="auto-style18">
                       <table id="TableUsers" runat="server" bgcolor="#e5e5e5" border="1" rules="rows" style=" font-size: small; color: black; font-family: Arial; background-color: #ffffff" width="100%">
                         <tr>
                            <td align="left" valign="top" nowrap="nowrap" style="font-weight: bold; font-size: small; color: black;font-family: Arial; height: 23px; background-color: #e5e5e5; width: 90px;" bgcolor="#e5e5e5">
                        Logon:<br />
                              <asp:TextBox ID="TextBoxNetId" runat="server" Width="100px"></asp:TextBox>
                            </td>                       
                            <td align="left" style="font-weight: bold; font-size: small; width: 49px; color: black;font-family: Arial; height: 23px; background-color: #e5e5e5" valign="top">
                               Level:<br />
                               <asp:DropDownList ID="DropDownListAccessLevel" runat="server" Width="68px">
                            <asp:ListItem>user</asp:ListItem>
                            <asp:ListItem>admin</asp:ListItem>
                               </asp:DropDownList>
                            </td>
                           <td align="left" valign="top" nowrap="nowrap" style="font-weight: bold; font-size: small; color: black;font-family: Arial; height: 23px; background-color: #e5e5e5; width: 120px;">
                         Email:<br />
                            <asp:TextBox ID="TextBoxEmail" runat="server" Width="250px"></asp:TextBox>
                           </td>     
                           <td align="left" valign="top" nowrap="nowrap" style="font-weight: bold; font-size: small; color: black;font-family: Arial; height: 23px; background-color: #e5e5e5; width: 142px;">
                         From (mm/dd/yyyy):<br />
                             <asp:TextBox ID="TextBoxFrom" runat="server" Width="142px"></asp:TextBox>
                           </td> 
                           <td align="left" valign="top" nowrap="nowrap" style="font-weight: bold; font-size: small; color: black;font-family: Arial; height: 23px; background-color: #e5e5e5; width: 140px;">
                        To (mm/dd/yyyy):<br />
                             <asp:TextBox ID="TextBoxTo" runat="server" Width="140px"></asp:TextBox>
                           </td>    
                           <td align="left" style="font-weight: bold; font-size: small; color: black; font-family: Arial; height: 23px; background-color: #e5e5e5; width: 301px;" valign="top">
                             Comments:<br />
                             <asp:TextBox ID="TextBoxComm" runat="server" Height="26px" TextMode="MultiLine" Width="290px"></asp:TextBox>
                           </td>
                           <td align="left" style="font-weight: bold; font-size: small; color: black; font-family: Arial;height: 23px; background-color: #e5e5e5" valign="bottom">
                             <%--<br />--%>
                             <asp:Button ID="ButtonAddUser" runat="server" Text="Add Report User" Width="131px" CssClass="ticketbutton"  />
                           </td>
                         </tr>
                         <tr>
                           <td colspan="7" style="font-weight: bold; color: Black; font-family: Arial; background-color: #e5e5e5;font-size: medium;" class="auto-style19">
                            Users: <asp:Label ID="LabelNofUsers" runat="server" Text=" "></asp:Label>
                           </td>
                        </tr>
                        <tr>
                           <td align="left" nowrap="nowrap" style="font-weight: bold; font-size: small; color: white;font-family: Arial; height: 27px; background-color: #C0C0C0; width: 90px;" bgcolor="#C0C0C0">
                             Logon&nbsp;
                           </td>
                           <td align="left"  nowrap="nowrap" style="font-weight: bold; font-size: small; width: 49px; color: white;font-family: Arial; height: 27px; background-color: #C0C0C0" bgcolor="#663300">
                             Level
                           </td>
                           <td align="left"  nowrap="nowrap" style="font-weight: bold; font-size: small; width: 120px; color: white;font-family: Arial; height: 27px; background-color: #C0C0C0" bgcolor="#663300">
                             Email(for admin)
                           </td>
                           <td align="left"  nowrap="nowrap" style="font-weight: bold; font-size: small; width: 142px; color: white;font-family: Arial; height: 27px; background-color: #C0C0C0">
                             Report open from:

                           </td>
                           <td align="left"  nowrap="nowrap" style="font-weight: bold; font-size: small; width: 120px; color: white;font-family: Arial; height: 27px; background-color: #C0C0C0">
                             Report open to:
                           </td>
                           <td align="left"  nowrap="nowrap" style="font-weight: bold; font-size: small; color: white; font-family: Arial;height: 27px; background-color: #C0C0C0; width: 301px;">
                             Comments
                           </td>
                           <td align="left"  nowrap="nowrap" style="font-weight: bold; font-size: small; color: white; font-family: Arial;height: 27px; background-color: #C0C0C0">
                             Edit</td>
                         </tr>
                         <tr>
                           <td colspan="7" style="font-weight: bold; color: #ffffff; font-family: Arial; background-color: #e5e5e5; font-size: medium;" class="auto-style17">
                           </td>
                         </tr>
                       </table>
        </td>
                </tr>
               </table>
             </td>
                </tr>
            </table>
        </asp:View>
   
</asp:MultiView>
        <br/>   
     
         <asp:Label ID="LabelAlert" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large"
            ForeColor="Gray" Text="Label"></asp:Label>           
   
        &nbsp;
         <asp:HyperLink ID="HyperLinkImportData" runat="server" Enabled="False" Font-Names="Arial" Font-Size="Medium" ForeColor="Blue" NavigateUrl="~/DataImport.aspx" ToolTip="Import Report Data" Visible="False" Font-Italic="True">import data</asp:HyperLink>
        <br/>                           

            </td>
        </tr>
    </table> 
         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   
        <asp:Label ID="LabelRepID" runat="server" Font-Bold="False" Font-Names="Arial" Font-Size="XX-Small" ForeColor="Black" Text=" rrrrrr"></asp:Label>

        <ucmsgbox:msgbox id="MessageBox" runat ="server" > </ucmsgbox:msgbox>
        <ucdlgcheck:dlgcheck  ID="dlgChooseParams" runat ="server" ChecklistBackColor="#FFFFFB" FontName="Tahoma" PromptFontSize="14px" PromptForeColor="#660033" Width="600px" />
        <ucdlgparam:dlgparam  ID ="dlgEnterParams" runat="server" FontName="Tahoma" />
                </ContentTemplate>
            <Triggers> 
            <asp:PostBackTrigger ControlID="ButtonUploadFile"/>
            <asp:PostBackTrigger ControlID="ButtonUploadRDL"/>
            <asp:PostBackTrigger ControlID="ButtonDownloadFile"/>
            </Triggers>
        </asp:UpdatePanel>
            <asp:FileUpload id="FileRDL" runat ="server" style="display:none;" />
            <div id="spinner" class="modal" style="display:none;">
                <div id="divSpinner" style="text-align: center; width: 130px; display: inline-block; clip: rect(auto, auto, auto, 50%); position: fixed; margin-top: 300px; margin-right: auto; padding-top: 10px; background-color: #f8f8d3; z-index: 2147483647; left: 50%; top: -5%;">
                  <img id="imgSpinner" src="Controls/Images/WaitImage2.gif" style="width: 100px; height: 100px" />
                    <br />
                      Please Wait...
                </div>
            </div>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpReportEdit" DisplayAfter="200">
            <ProgressTemplate >
            <div class="modal">
                <div class="center">
                    <asp:Image ID="imgProgress" runat="server"  ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />
                    Please Wait...
                </div>
            </div>
            </ProgressTemplate>
        </asp:UpdateProgress> 
    </form>
</body>
</html>
