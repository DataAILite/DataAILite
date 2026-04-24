<%@ Page Language="VB" AutoEventWireup="false" CodeFile="RDLformat.aspx.vb" Inherits="RDLformat" %>

<script type="text/javascript" src="Controls/Javascripts/OUR.js"></script>
<script type="text/javascript" src="Controls/Javascripts/ReportDesignMenu.js"></script>
<script type="text/javascript" src="Controls/Javascripts/FieldOrder.js"></script>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>RDL format</title>
    <link rel="stylesheet" type="text/css" href="Controls/css/ReportViewMenu.css"/>
    <link rel="stylesheet" type="text/css" href="Controls/css/Dialog.css"/>

    <style type="text/css">
        .auto-style3 {
            width: 219px;
            height: 30px;
        }
        .auto-style7 {
            width: 100%;
            height: 40px;
        }
        .auto-style9 {
            height: 40px;
        }
        .auto-style10 {
            height: 23px;
            width: 167px;
        }
        .auto-style12 {
            margin-top: 0px;
        }
        .ColumnTitle{
            width: 100%;
            height: 30px;
            font-weight: bold;
            color: White; 
            font-family: Arial;
            background-color: #C0C0C0;
            font-size: medium;
        }
        .vertical-center {
          display: flex;
          /*justify-content: center;*/
          align-items: center;
        }

        .ColumnMenu {
           display:inline-block;
           width:24px;
           height:24px;
           padding-top:3px;
           padding-left:3px;
           margin-left:5px;
           background-color: #c0c0c0;
           border-radius:50%;
         }
        .ColumnMenu:hover {
            background-color: white;
            cursor: default;
        }
        .MenuLine {
            width: 15px;
            height: 2px;
            background-color: blue;
            margin:3px;
        }
        .divCircle {
            width:25px;
            height:25px;
            border-radius:50%;
            background-color: white;
        }

        .ColumnHeader {
             white-space: nowrap;
             text-align: left;
             height:24px;
             font-weight: bold;
             font-size: small;
             color: white;
             font-family: Arial; 
             background-color: #C0C0C0;
        }
        .auto-style20 {
            height: 23px;
            width: 112px;
        }
        .auto-style21 {
            width: 220px;
            margin-left: 0px;
        }
        .auto-style22 {
            width: 250px;
        }
        .auto-style26 {
            width: 225px;
            height: 4px;
        }
        .auto-style27 {
            width: 250px;
            height: 38px;
        }
        .auto-style32 {
            width: 87px;
            height: 4px;
        }
        .auto-style34 {
            text-decoration: underline;
        }
        .auto-style35 {
            width: 283px;
            height: 4px;
        }
        .auto-style45 {
            height: 4px;
            width: 203px;
        }
        .auto-style46 {
            width: 250px;
            /*width: 651px;*/
            height: 29px;
        }
        .auto-style47 {
            /*width: 798px;*/
            width: 250px;
        }
        .auto-style74 {
            width: 250px;
        }
        .auto-style75 {
            width: 250px;
        }
        .auto-style76 {
            width: 900px;
        }
        .auto-style79 {
            height: 29px;
        }
        .auto-style80 {
            width: 600px;
            height: 29px;
        }
        .auto-style83 {
            margin-left: 0px;
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

.auto-style85 
{
    width: 407px;
}
.auto-style86 
{
    width: 100%;
    height: 23px;
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
        <asp:UpdatePanel ID="udpRDLformat" runat ="server">
            <ContentTemplate>

             <asp:HiddenField ID ="hdnFieldOrder" runat ="server" />
 <table>
      <tr>
          <td colspan="3" style="font-size:x-large; font-style:normal; font-weight:bold; background-color: #e5e5e5; vertical-align:middle; text-align: left; height: 40px;">
              <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label>
          </td>
      </tr> 
        <tr>
        <td style="font-size: x-small; font-style: normal; font-weight: normal; background-color: #e5e5e5; vertical-align: top; text-align: left; width: 15%;">
           <div id="tree">
             <asp:TreeView ID="TreeView1"  runat="server" Width="100%" NodeIndent="10" Font-Names="Times New Roman"  EnableTheming="True" ImageSet="BulletedList">
                    <Nodes>  
              <asp:TreeNode Text="&lt;b&gt;Log Off;&lt;/b&gt;"  Value="Default.aspx" Expanded="True" ></asp:TreeNode>

              <asp:TreeNode Text="&lt;b&gt;List of Reports&lt;/b&gt;"  Value="ListOfReports.aspx" Expanded="True" ></asp:TreeNode>                              
                    
              <asp:TreeNode Text="&lt;b&gt;Report Definition&lt;/b&gt;"  Value="ReportEdit.aspx?tne=2" Expanded="True" ></asp:TreeNode>

              <asp:TreeNode Text="&lt;b&gt;Report Parameters&lt;/b&gt;"  Value="ReportEdit.aspx?tne=3" Expanded="True" ></asp:TreeNode>

              <%--<asp:TreeNode Text="&lt;b&gt;Share Report (Users)&lt;/b&gt;"  Value="ReportEdit.aspx?tne=4" Expanded="True" ></asp:TreeNode>--%>
 
            <asp:TreeNode Text="Report Data Query"  Value="SQLquery.aspx?tnq=0" Expanded="True" >
                 <asp:TreeNode Text="Data fields" Value="SQLquery.aspx?tnq=0" > </asp:TreeNode>
                 <asp:TreeNode Text="Joins" Value="SQLquery.aspx?tnq=1"> </asp:TreeNode>
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
                 <asp:TreeNode Text="Export Data to Excel"  Value="datatoExcel" NavigateUrl="ShowReport.aspx?srd=1" > </asp:TreeNode>
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
       <RootNodeStyle HorizontalPadding="2px" Font-Bold="True" />
       <NodeStyle CssClass="NodeStyle" />
       <ParentNodeStyle Font-Bold="True" />
     </asp:TreeView>
    
    </div>
    </td>
  <td style="width: 5px"></td>
  <td id="main" style="width: 85%; text-align: left; vertical-align: top"> 
                <table id="tblTitle" style="width:100%; font-family: Arial;">
                  <tr>
                    <td style="width:61%;text-align:right">
                       <asp:Label ID="LabelAlert0" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large" ForeColor="Gray" Text="Report Format Definition - "></asp:Label>
                       <asp:Label ID="LabelAlert" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large" ForeColor="Gray" Text="Label"></asp:Label>
                    </td>
                    <td style="width:26%;text-align:center;">
                       <asp:Label ID="lblView" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Medium" ForeColor="#000099" Text="Column Order, Expressions"></asp:Label>
                       <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/ListOfReports.aspx" Visible="false" CssClass="NodeStyle" Font-Size="12px">List of reports</asp:HyperLink>
                    </td>
<%--                    <td style="width:13%;text-align:center;">
                      <asp:HyperLink ID="HyperLink6" runat="server" NavigateUrl="~/ShowReport.aspx?srd=3" ToolTip="Run Report" CssClass="NodeStyle" Font-Size="12px" Visible="False" Enabled="False">Show report data</asp:HyperLink>    
                    </td>--%>
                    <td style="width:13%;text-align:center;">
                      <asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx?hilt=Report_Format_Definition" Target="_blank" CssClass="NodeStyle" Font-Size="12px">Help</asp:HyperLink>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:HyperLink ID="HyperLinkLogOff" runat="server" NavigateUrl="~/Default.aspx" CssClass="NodeStyle" Font-Names="Arial">Log off</asp:HyperLink> 
                    </td>                                          
                  </tr>
                </table> 

 <%--               <div style="width:100%; font-family: Arial; font-size: xx-small; text-align:center">
                       <asp:Label ID="lblView" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Medium" ForeColor="#000099" Text="Column Order, Expressions"></asp:Label>
                </div> --%>
        <asp:HyperLink ID="HyperLink1" runat="server"  NavigateUrl="~/ReportEdit.aspx" ToolTip="Edit Report" Enabled="False" Visible="False">Edit report</asp:HyperLink> 
        <div>
        <asp:Menu ID="MenuMain" Visible="false" Width="600px" runat="server" Orientation="Horizontal"  OnMenuItemClick="MenuMain_MenuItemClick" StaticSelectedStyle-BackColor="LightGray" DynamicSelectedStyle-BackColor="LightGray"    StaticSelectedStyle-Font-Bold="true"  StaticMenuItemStyle-BackColor="#999999" BorderWidth ="0px" Height="20px">
    <Items>
        <asp:MenuItem Text="Report Data" Value="0" ToolTip="Report Query to get data from db">
            <asp:MenuItem Text=" Data Fields" ToolTip="SELECT statement" Value="10"></asp:MenuItem>                    
            <asp:MenuItem Text=" Join Tables" ToolTip="JOIN statement" Value="11"></asp:MenuItem>
            <asp:MenuItem Text=" Filters " ToolTip="Filter data by defined conditions" Value="12"></asp:MenuItem>
            <asp:MenuItem Text=" Sorting" ToolTip="ORDER BY statement" Value="13"></asp:MenuItem>
        </asp:MenuItem>
        <asp:MenuItem Text=" Report format " Value="1" ToolTip="Report RDL Format" >
            <asp:MenuItem Text=" Column Order, Expressions" ToolTip="Columns Order, functions" Value="20"></asp:MenuItem>
            <asp:MenuItem Text=" Groups and Totals" ToolTip="Grouping and Totals" Value="21"></asp:MenuItem>
            <asp:MenuItem Text=" Combine column values " ToolTip="Listing of column values" Value="22"></asp:MenuItem>
        </asp:MenuItem>

        <asp:MenuItem Text=" Report Info" Value="2" ToolTip="Report Info and Report Files" ></asp:MenuItem>        
        <asp:MenuItem Text=" Parameters " Value="3" ToolTip="Report Parameters" ></asp:MenuItem>
        <asp:MenuItem Text=" Users " Value="4" ToolTip="Report Users" ></asp:MenuItem>        
    </Items>
</asp:Menu>
            
             <asp:Menu ID="Menu1" runat="server" BorderWidth="0px" DynamicSelectedStyle-BackColor="LightGray" OnMenuItemClick="Menu1_MenuItemClick" Orientation="Horizontal" StaticMenuItemStyle-BackColor="#e5e5e5" StaticSelectedStyle-BackColor="LightGray" StaticSelectedStyle-Font-Bold="true" Width="900px">
                <Items>
                    <asp:MenuItem Text=" Column Order, Expressions" ToolTip="Columns Order, functions" Value="0"></asp:MenuItem>
                    <asp:MenuItem Text=" Groups and Totals" ToolTip="Grouping and Totals" Value="1"></asp:MenuItem>
                    <asp:MenuItem Text=" Combine column values " ToolTip="Listing of column values" Value="2"></asp:MenuItem>
                    <asp:MenuItem Text=" Advanced report designer " ToolTip="Advanced report designer" Value="3"></asp:MenuItem> 
                    <asp:MenuItem Text=" Map definition " ToolTip="Map designer" Value="4"></asp:MenuItem> 
                </Items>
            </asp:Menu>
        <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">

<asp:View ID="Tab1" runat="server">
                    <table bgcolor="LightGray" border="1" cellpadding="0" cellspacing="0" width="100%">
                        <tr valign="top">
                            <td class="auto-style7">
                                <table id="Table7" runat="server" bgcolor="LightGray" border="0" class="auto-style9">
                                    <tr>
                                        <td align="left" bgcolor="LightGray" class="auto-style76" style="color: black; font-family: Arial; letter-spacing: normal; ">
                                            &nbsp;Expressions, Functions, Friendly Names, and Column Order in Report&nbsp;&nbsp;&nbsp;<asp:Label ID="LabelNoGroups2" runat="server" Text=" "></asp:Label>
                                        </td>
                                        <td align="left"  style="color: black; font-family: Arial; letter-spacing: normal;">
                                            <asp:Label ID="Label20" runat="server" Font-Underline="True" Text=" "></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td align="left" width="100%">
                                <table id="RDLadds2" runat="server" bgcolor="#e5e5e5" border="1" rules="rows" style=" font-size: small;
                color: black; font-family: Arial; background-color: #e5e5e5;border:medium double #FFFFFF;" width="40%">
                                    <tr valign="top">
                                       
                                        <td align="left" colspan=2 class="auto-style46" style="font-weight: bold; font-size: small; color: black; font-family: Arial; background-color: #e5e5e5;border:medium double #FFFFFF;" valign="top">
                                           
                                            <br />&nbsp;&nbsp;Column:&nbsp;&nbsp;&nbsp;
                                            
                                            <asp:DropDownList ID="DropDownRepFields" runat="server" AutoPostBack="True" Width="300px"> </asp:DropDownList>
                                            <br />&nbsp;
                                            <br />&nbsp;&nbsp;Friendly Name:&nbsp;&nbsp; 
                                            
                                            <asp:TextBox ID="TextBoxFieldFriendly" runat="server" Height="20px" Width="250px"></asp:TextBox>
                                            <br />&nbsp; 
                                        </td>
                                    </tr>
                                    
                                    <tr valign="top">
                                       
                                        <%--<td align="left"  class="auto-style46" style="font-weight: bold; font-size: small; color: white; font-family: Arial; background-color: #999999" valign="top">--%>
                                        <td align="left"  style=" font-weight: bold; font-size: small; color:black; font-family: Arial; background-color: #e5e5e5;border:medium double #FFFFFF;" valign="top">
                                            <br />&nbsp;
                                            &nbsp;Function type:
                                            &nbsp;&nbsp;
                                            <asp:DropDownList ID="DropDownFunctionsType" runat="server" AutoPostBack="True" Width="100px">
                                                <asp:ListItem>Text</asp:ListItem>
                                                <asp:ListItem>Math</asp:ListItem>
                                                <asp:ListItem>DateTime</asp:ListItem>
                                                <asp:ListItem>Financial</asp:ListItem>
                                                <asp:ListItem>Conversion</asp:ListItem>
                                                <%--<asp:ListItem>Statistics</asp:ListItem>--%>
                                            </asp:DropDownList>
                                       <%-- </td>
                                       <td align="left"  style="width:450px; font-weight: bold; font-size: small; color: white;  font-family: Arial; background-color: #999999" valign="top"> 
                                       --%>  
                                           <%--<br />--%> 
                                           &nbsp;Function:&nbsp;&nbsp;
                                            
                                             <asp:DropDownList ID="DropDownFunctions" runat="server" AutoPostBack="True" Width="150px">
                                            </asp:DropDownList>
                                        <%--</td>
                                        <td align="left" colspan=2 style="font-weight: bold; font-size: small; color: white; font-family: Arial; background-color: #999999; " valign="top" class="auto-style47">
                                        --%>   
                                            <br /> <br />&nbsp;
                                            &nbsp;Expression:&nbsp;
                                            
                                            <asp:TextBox ID="TextBoxExpression" runat="server" Height="20px" Width="600px" TextMode="SingleLine"></asp:TextBox>&nbsp;&nbsp;&nbsp;
                                            <br />&nbsp;
                                        </td>                                        
                                    </tr>

                                    <tr>
                                        <td align="left"  nowrap="nowrap" style="font-weight: bold; font-size: small; color: black; font-family: Arial; background-color: #e5e5e5;border:medium double #FFFFFF;" valign="top"> 
                                            <br />&nbsp;
                                            <%--<asp:Label ID="Label21" runat="server" Text=" "></asp:Label>--%>
                                             &nbsp;<asp:CheckBox ID="chkReplaceColValue"  runat="server" Text="replace column value" ToolTip="Replace Column Value with expression or add new column with value = expression." Font-Size="X-Small" Font-Names="Arial" Font-Italic="True" ForeColor="#3333CC" />
                                             
                                            &nbsp;&nbsp;
                                            <asp:Button ID="ButtonAddField" runat="server" Text="Add/Update Column" Width="171px"  CssClass="ticketbutton" />
                                           <br />&nbsp;
                                            <%-- <asp:Button ID="ButtonUpdateField" runat="server" Text="Update Column" Width="171px" CssClass="auto-style83" />--%>
                                        </td>
                                    </tr>
                                    
                                </table>
                            </td>
                        </tr>
                        <tr class="ColumnTitle">
                                        <td>
                                            <table>
                                                <tr>
                                                    <td>
                                                      <div class="vertical-center">
                                                        Report Columns
                                                      </div>                                                                                                    
                                                    </td>
                                                    <td>
                                                      <div id="divColumnMenu" runat="server"  class="ColumnMenu"  title="Set Column Order">
                                                          <div style="font-size:20px;padding-left:3px;padding-bottom:3px;color:blue;">
                                                              &#8645
                                                          </div>
<%--                                                      <div id="divMenuLine1" class="MenuLine" ></div>
                                                          <div id="divMenuLine2" class="MenuLine"  ></div>
                                                          <div id="divMenuLine3" class="MenuLine"  ></div>--%>
                                                       </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td colspan="3"></td>
                        </tr>
                        <tr>
                            <td style="white-space: nowrap; text-align: left; vertical-align: text-top; height:27px; width:100%; font-size: small; color: white; font-family: Arial; background-color: #C0C0C0; ">
                                <table id="RDLrepfields" runat="server" bgcolor="#663300" border="1" rules="rows" style=" font-size: small; color: black; font-family: Arial; background-color: #ffffff" width="100%"> 
                                                                        
                                    <tr>                                        
                                        <td class="ColumnHeader" style="width:20%;">
                                            Column
                                        </td>                
                                        <td class="ColumnHeader" style="width:5%;">
                                            Delete
                                        </td>
                                        <td class="ColumnHeader" style="width:3%;">
                                            Up
                                        </td>
                                        <td class="ColumnHeader" style="width:5%;">
                                            Down
                                        </td>
                                        <td class="ColumnHeader" style="width:5%;">
                                            Order
                                        </td>
                                        <td class="ColumnHeader" style="width:32%;">
                                            Expression
                                        </td>
                                        <td class="ColumnHeader" style="width:30%;">
                                            Friendly name</td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td class="auto-style7">
                                <table id="Table8" runat="server" bgcolor="LightGray" border="0" >
                                    <tr>
                                        <td align="left" bgcolor="LightGray" style="color: black; font-family: Arial; letter-spacing: normal; ">
                                            <asp:Label ID="Label22" runat="server" Text="Update report:"></asp:Label>
                                            &nbsp;&nbsp;&nbsp;
                                       <%-- </td>
                                        <td align="left"  style="color: #ffffff; font-family: Arial; letter-spacing: normal;">--%>
                                            <asp:Button ID="ButtonSubmit2" runat="server"  BackColor="#7CDC99" CssClass="ticketbutton" Text="Submit" Width="131px" ToolTip="Submit list of columns and update XSD and RDL" />
                                        <asp:Label ID="Label23" runat="server" Text=" "></asp:Label>&nbsp;&nbsp;&nbsp;
                                        
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:View>



        <asp:View ID="Tab2" runat="server">
            <table cellpadding=0 cellspacing=0 bgcolor="LightGray" border="1" width="100%" >
                <tr valign="top">
                    <td class="auto-style86">
                        <table id="Table2" runat="server" bgcolor="LightGray" border="0" class="auto-style9"  width="100%">
                            <tr>
                                <td align="left" style="color: black; font-family: Arial; letter-spacing: normal; " bgcolor="LightGray" class="auto-style3">
                                    <asp:Label ID="Label8" runat="server" Text="Add Group" ></asp:Label>
                                    &nbsp;&nbsp;&nbsp;<asp:Label ID="LabelNoGroups" runat="server" Text=" "></asp:Label></td>
                                <td align="left" style="color: black; font-family: Arial; letter-spacing: normal;" >
                                   <asp:Label ID="Label3" runat="server" Text="   " Font-Underline="True"></asp:Label> </td>                                
                            </tr>
                         </table>
                    </td>
                </tr>
                <tr valign="top">
        <td align="left" width="100%">
            <table id="RDLadds" runat="server" bgcolor="#e5e5e5" border="1" rules="rows" style=" font-size: small;
                color: black; font-family: Arial; background-color: #e5e5e5;border:medium double #FFFFFF;" width="30%">              
                
                <tr valign="top">
                    <td align="left" valign="top" nowrap="nowrap" style="font-weight: bold; font-size: small; color: black;background-color: #e5e5e5;border:medium double #FFFFFF;
                        font-family: Arial; " >
                        <br />
                        &nbsp;Group by the field:&nbsp;&nbsp;
                        <asp:DropDownList ID="DropDownGroupFields" runat="server" Width="290px" CssClass="auto-style12" AutoPostBack="True"></asp:DropDownList>
                        <br />&nbsp;
                        </td>    
                </tr>
                
                <tr>
                     <td align="left"  style="font-weight: bold; font-size: small;font-family: Arial; color: black;background-color: #e5e5e5;border:medium double #FFFFFF; "
                       valign="top" class="auto-style75" >
                        &nbsp;Friendly Group Name:
                        <asp:TextBox ID="TextCommentsGroup" runat="server" Height="20px"
                            Width="280px"></asp:TextBox>
                        <br />&nbsp;
                     </td>
                </tr>

                <tr>    
                    <td align="left" style="font-weight: bold; font-size: small; color: black;background-color: #e5e5e5;border:medium double #FFFFFF;
                        font-family: Arial;" valign="top">
                        &nbsp;Totals for Column:
                        <asp:DropDownList ID="DropDownCalcFields" runat="server" Width="300px" AutoPostBack="True"></asp:DropDownList>
                        <br />&nbsp;

                    </td>
                </tr>

                <tr>
                    <td align="left" valign="top"  nowrap="nowrap" style="font-weight: bold; font-size: small; color: black;background-color: #e5e5e5;border:medium double #FFFFFF;
                        font-family: Arial; " >
                       <br />&nbsp; 
                       &nbsp;&nbsp;&nbsp; <asp:Button ID="ButtonAddGroup" runat="server" Text="Add/Update Group" Width="130px"  CssClass="ticketbutton" /> 
                        
                         <br />
                         &nbsp;<asp:Label ID="LabelSelected" runat="server" Text="Selected" Visible="False"></asp:Label>
                    </td>
                </tr>
                
                </table>
            </td>
            </tr>
            <tr>
                    <td colspan="4" class="ColumnTitle" >
                        Groups and Totals</td>
            </tr>
            <tr><td>
            <table id="RDLgroups" runat="server" bgcolor="#663300" border="1" rules="rows" style=" font-size: small; color: black; font-family: Arial; background-color: #ffffff" width="100%"> 
                
                <tr>
                    <td class="ColumnHeader" >
                        Group By &nbsp;</td>
                    <td class="ColumnHeader" >
                        Totals for Column </td>
                    <td class="ColumnHeader" >
                        Group Name</td>
                    <td class="ColumnHeader" >
                        Count</td>
                    <td class="ColumnHeader" >
                        Sum</td>
                    <td class="ColumnHeader"  >
                        Max</td>
                    <td class="ColumnHeader" >
                        Min</td>                   
                    <td class="ColumnHeader" >
                        Average</td>
                    <td class="ColumnHeader" >
                        Std Dev</td>
                    <td class="ColumnHeader" >
                        Count Distinct &nbsp;</td>
                    <td class="ColumnHeader" >
                        First</td>
                    <td class="ColumnHeader" >
                        Last</td>
                    <td class="ColumnHeader" >
                        Page Break </td>
                     <td class="ColumnHeader" >
                        Order</td>
                   <td class="ColumnHeader" >
                        Up </td>
                    <td class="ColumnHeader" >
                        Down </td>
                    <td class="ColumnHeader" >
                        Delete </td>
                </tr>
               
            </table>
        </td>
        </tr>
                <tr valign="top">
                    <td class="auto-style7">
                        <table id="Table1" runat="server" bgcolor="LightGray" border="0" class="auto-style9">
                            <tr>
                                <td align="left" style="color: black; font-family: Arial; letter-spacing: normal; " bgcolor="LightGray" class="auto-style74">
                                    <asp:Label ID="Label1" runat="server" Text="Save Groups and update report:"></asp:Label>
                                    &nbsp;&nbsp;&nbsp;<asp:Label ID="Label2" runat="server" Text=" "></asp:Label></td>
                                <td align="left" style="color: #ffffff; font-family: Arial; letter-spacing: normal;">
                                    <asp:Button ID="ButtonSubmit" runat="server" Text="Submit" Width="131px" ToolTip="Submit Groups and update XSD and RDL"  BackColor="#7CDC99" CssClass="ticketbutton" /></td>                                
                            </tr>
                         </table>
                    </td>
                </tr>
        </table>
</asp:View>



  <asp:View ID="Tab3" runat="server">
                    <table bgcolor="LightGray" border="1" cellpadding="0" cellspacing="0" width="100%">
                        <tr valign="top">
                            <td class="auto-style7">
                                <table id="Table5" runat="server" bgcolor="LightGray" border="0" class="auto-style9">
                                    <tr>
                                        <td align="left" bgcolor="LightGray" style="color: black; font-family: Arial; letter-spacing: normal; ">
                                            <asp:Label ID="Label14" runat="server" Text="Combining all values in the column:"></asp:Label>
                                            &nbsp;&nbsp;&nbsp;<asp:Label ID="LabelNoGroups1" runat="server" Text=" "></asp:Label>
                                        </td>
                                        <td align="left" style="color: #ffffff; font-family: Arial; letter-spacing: normal;">
                                            <asp:Label ID="Label15" runat="server" Font-Underline="True" Text=" "></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td align="left" width="100%">
                                <table id="RDLadds1" runat="server" bgcolor="#e5e5e5" border="1" rules="rows" style=" font-size: small;
                                       color: black; font-family: Arial; background-color: #e5e5e5;border:medium double #FFFFFF;" width="30%">
                                    <tr valign="top">
                                        <td align="left"  style="font-weight: bold; font-size: small; color: black;font-family: Arial;background-color: #e5e5e5;border:medium double #FFFFFF;">
                                            <br />&nbsp;
                                            &nbsp;For the set of columns defined by:&nbsp;&nbsp;
                                            <asp:DropDownList ID="DropDownListRecFields" runat="server" AutoPostBack="False" Width="250px"></asp:DropDownList>                                            
                                        </td> 
                                    </tr>
                                    <tr>
                                        <td align="left"  style="font-weight: bold; font-size: small; color: black;font-family: Arial;background-color: #e5e5e5;border:medium double #FFFFFF;">
                                            <br />&nbsp;
                                            &nbsp;Combine all values in the Column:&nbsp;&nbsp;
                                            <asp:DropDownList ID="DropDownListFields" runat="server" Width="250px">
                                            </asp:DropDownList>
                                            <br />&nbsp; &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left"  style="font-weight: bold; font-size: small; color: black;font-family: Arial;background-color: #e5e5e5;border:medium double #FFFFFF;">
                                            <br />&nbsp;
                                            &nbsp;Comments:&nbsp; &nbsp;
                                          <asp:TextBox ID="TextCommentsList" runat="server" Height="20px" Width="390px"></asp:TextBox>
                                          <br />
                                      </td>
                                    </tr>
                                    <tr>
                                        <td align="left"  style="font-weight: bold; font-size: small; color: black;font-family: Arial;background-color: #e5e5e5;border:medium double #FFFFFF;">
                                            <br />&nbsp;
                                            &nbsp;Friendly Name:&nbsp; &nbsp; 
                                            
                                            <asp:TextBox ID="TextBoxFriendlyNameField" runat="server" Height="20px" Width="370px" AutoPostBack="False"></asp:TextBox>
                                           <br /> 
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left"  style="font-weight: bold; font-size: small; color: black;font-family: Arial;background-color: #e5e5e5;border:medium double #FFFFFF;">
                                            <br />&nbsp;
                                            &nbsp;<asp:CheckBox ID="chkOldColumn"  runat="server" Text="replace column value" ToolTip="Replace Column value with combined values or add new column." Font-Names="Arial" Font-Italic="True" Font-Size="X-Small" ForeColor="#3333CC" />
                                            
                                            &nbsp; &nbsp;&nbsp; &nbsp;<asp:Button ID="ButtonAddList" runat="server" Text="Add/Update Column" Width="140px" CssClass="ticketbutton" />
                                           <br />
                                        </td>
                                    </tr>
                                    
                                </table>
                            </td>
                        </tr>
                        <tr>
                                        <td class="ColumnTitle" colspan ="5">Combined values in Columns:</td>
                        </tr>
                        <tr>
                            <td align="left" bgcolor="#999999" class="auto-style19" nowrap="nowrap" style=" font-size: small; color: white; font-family: Arial; background-color: #C0C0C0; " valign="top">
                                <table id="RDLlists" runat="server" bgcolor="#663300" border="1" rules="rows" style=" font-size: small; color: black; font-family: Arial; background-color: #ffffff" width="100%"> 
                                    <tr>
                                        <td class="ColumnHeader" style="width:20%;">
                                            Column Name</td>
                                        <td class="ColumnHeader" style="width:20%;">
                                            For the set of columns defined by</td>
                                        <td class="ColumnHeader" style="width:20%;">
                                            combine all values in the Column &nbsp;</td> 
                                        <td class="ColumnHeader" style="width:15%;">
                                            Friendly Name &nbsp;</td>      
                                        <td class="ColumnHeader" style="width:25%;">
                                            Delete</td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td class="auto-style7">
                                <table id="Table6" runat="server" bgcolor="LightGray" border="0" class="auto-style9">
                                    <tr>
                                        <td align="left" bgcolor="LightGray" class="auto-style27" style="color: black; font-family: Arial; letter-spacing: normal; ">
                                            <asp:Label ID="Label17" runat="server" Text="Update report:"></asp:Label>
                                            &nbsp;&nbsp;&nbsp;<asp:Label ID="Label18" runat="server" Text=" "></asp:Label>
                                        </td>
                                        <td align="left" style="color: #ffffff; font-family: Arial; letter-spacing: normal;">
                                            <asp:Button ID="ButtonSubmit3" runat="server" Text="Submit" Width="131px" ToolTip="Submit Combining Values and update XSD and RDL"  BackColor="#7CDC99" CssClass="ticketbutton"  />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:View>
  <asp:View ID="Tab4" runat="server">
        <table width="100%"cellpadding=0 cellspacing=0 border="0" >
            <tr valign="top">
                <td class="TabArea" style="width: 100%"/>             
            </tr>
        </table>
    </asp:View>
    <asp:View ID="Tab5" runat="server">
        <table width="100%" cellpadding=0 cellspacing=0 border="0" >
            <tr valign="top">
                <td class="TabArea" style="width: 100%"/>             
            </tr>
        </table>
    </asp:View>
       &nbsp;              
            </asp:MultiView>
        <asp:Label ID="LabelAlert1" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large"
            ForeColor="Gray" Text="_________________________________________________________________________"></asp:Label>
            
        </div>

</td>
        </tr>
    </table> 

        <div>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Label ID="LabelReprtID" runat="server" Font-Bold="False" Font-Names="Arial" Font-Size="XX-Small" ForeColor="Black" Text=" "></asp:Label>
        </div>
        <%-- ************************************ Design Menu ********************************** --%>
        <div id="divDesignMenu"  style="display:none;" class="skin0" onmouseover="highlight(event)" onmouseout="lowlight(event)" onclick="doOption(event)"  >
          <div id="PositionFields" class="menuitems">Set Column Order</div>
          <div id="ReportDesigner" class="menuitems">Report Designer</div>
        </div>
        <%-- ************************************ Column Order Dialog ********************************** --%>
        <div id="divColumnOrderDlgBackground" runat="server" class="dlgbackground" style="display:none; height:100%;width:100%;position:absolute;top:0px;left:0px;">
            <div id="divColumnOrderDlg" runat="server"  class="ColumnOrderDlg relative-middle">
                  <div id="divColumnOrderDlgHeading" style=" font-size: small; text-align: center; line-height: 22px; background-color: gray; width: 100%; height: 22px; color: white; ">
                      <asp:Label ID="lblColumnOrderDlgHeading" runat="server" Text="Set Column Order"></asp:Label>
                      <div id="divColumnOrderDlgX" runat ="server" class="close" title="close dialog">&times;</div>
                  </div>
                  <div id="divColumnOrderDlgBody" class ="clearfix" >
                      <table>
                          <tr>
                             <td id="tdColumnList">
                                 <div id="divColumnList" style="height:300px; width: 170px; margin-left: 10px;">
                                    <WC:DragList id="lstColumns" runat="server"  BackColor="White" BorderStyle="Solid" Font-Names="Tahoma" Font-Size="Small" DoPostBack="False" DropOK="True" ToolTip="Drag and drop column to set order" Text="Columns" HeadingBackColor="#000066" HeadingForeColor="White" BorderColor="#666666" BorderWidth="1px" ForeColor="Black"></WC:DragList>
                                 </div>
                             </td>
                             <td id="tdArrowButtons">
                                 <div id="divArrowButtons" style=" display:inline-block; margin:0px; " >
                                     <asp:Button ID="btnOrderUp" runat="server" style="background-image:url(Controls/Images/arrow-up-black.png)" CausesValidation="False" UseSubmitBehavior="False" Text="" ToolTip="Move Up" CssClass="smallimagebutton" />
                                     <br />
                                     <asp:Button ID="btnOrderDown" runat="server" style="background-image:url(Controls/Images/arrow-down-black.png)" CausesValidation="False" UseSubmitBehavior="False" Text="" ToolTip="Move Down" CssClass="smallimagebutton" />
                                 </div>
                             </td>
                          </tr>
                      </table>
                      <div id="divColumnOrderDlgButtons" style="margin-top: 25px; margin-left: 8px;">
                           <asp:Button ID="btnColumnOrderDlgSave" runat="server" CssClass="dlgboxbutton" Text="Save" CausesValidation="false" UseSubmitBehavior="False" />
                           <asp:Button ID="btnColumnOrderDlgCancel" runat="server" CssClass="dlgboxbutton" Text="Cancel" CausesValidation="false" UseSubmitBehavior="False" />
                      </div>
                  </div>
            </div>
        </div>
        <ucMsgBox:Msgbox id="MessageBox" runat ="server" > </ucMsgBox:Msgbox>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpRDLformat">
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
