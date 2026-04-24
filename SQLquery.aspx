<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SQLquery.aspx.vb" Inherits="SQLquery" %>
<%@ Register TagPrefix="uc1" TagName="DropDownColumns" Src="Controls/uc1.ascx" %>
<%@ Register src="Controls/CalendarDropDown.ascx" tagname="CalendarDropDown" tagprefix="uc2" %>

<script type="text/javascript" src="Controls/Javascripts/SQLquery.js"></script>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SQL Query Designer</title>
    <style type="text/css">
        .auto-style7 {
            width: 100%;
            height: 40px;
        }
        .auto-style9 {
            height: 40px;
        }
        .auto-style6 {
            width: 900px;
            height: 30px;
        }
        .auto-style19 {
            height: 27px;
            width: 900px;
            background-color:#C0C0C0;
            color:white;
            font-family:Arial;
            font-size:small;
            text-wrap:none;
        }
        .auto-style21 {
            margin-left: 101px;
        }
        .auto-style22 {
            width: 375px;
        }
        .auto-style40 {
            width: 21%;
            height: 50px;
        }
        .auto-style41 {
            width: 39%;
            height: 20px;
        }
        .tr {
        background-color: #C0C0C0;
        border-bottom: 1px solid black;
    }
   .inlinebottom {
           display:inline-block;
           vertical-align:bottom;
       }
   .inline {
           display:inline-block;
       }
   .conditions {
       width:100%;
       border:1px solid black;
       background-color:white;
       color:black;
       font-family:Arial;
       font-size :small;
   }
   .columns {
       height: 23px;
       border-width: 0px;
       padding: 0px;
       margin: 0px;
       font-family: Arial;
       font-size: small;
       font-weight: bold;
       color: white;
       background-color: #C0C0C0;
       table-layout:fixed;
       overflow:hidden;
   }
    .ButtonStyleSubmit {
        width: 100px;
        height: 30px;
        font-size: 12px;
        border-radius: 6px;
        border-style :solid;
        border-color: ButtonFace;
        color: black;
        border-width: 1px;
        /*background-repeat: no-repeat;
        background-position:center;*/
        background-color: ButtonFace ;
        padding:0px;
        margin:0px;
        z-index: 9999;
        /*background-image: url("Images\DDImageDown.bmp");*/
        /*padding: 5px 4px 0px 5px*/
     }
    .divSearch {
        display:inline-block;
        margin-bottom:3px;
        /*margin-right: 8px;*/
        padding: 3px;
        height:18px;
        width:18px;
        outline-style: none;
    }
    .btnSearch {
        width: 22px;
        height: 22px;
        /*font-size: 16px;*/
        border-style :none;
        outline-style: none;
        background-color: white;
        margin-bottom: 3px;
        /*margin-right: 8px;*/
        padding: 0px;
    }
    .imgSearch {
        outline-style: none;
    }

    
  .imgSearch:focus {
     outline-style: dashed;
     outline-color: gray;

 }
    
     .ButtonStyle2 {
        height: 25px;
        font-size: 12px;
        border-radius: 5px;
        border-style :solid;
        border-color: ButtonFace ;
        color: black;
        border-width: 1px;
        /*background-repeat: no-repeat;
        background-position:center;*/
        background-color: ButtonFace;
        padding: 3px;
        margin:0px;
        z-index: 9999;
        /*background-image: url("Images\DDImageDown.bmp");*/
        /*padding: 5px 4px 0px 5px*/
     }
        p.MsoListParagraph
	{margin-top:0in;
	margin-right:0in;
	margin-bottom:8.0pt;
	margin-left:.5in;
	line-height:107%;
	font-size:11.0pt;
	font-family:"Calibri",sans-serif;
	}
        .auto-style84 {
            width: 509px;
            height: 21px;
        }
        .auto-style86 {
            width: 900px;
        }
        .auto-style87 {
            height: 27px;
            width: 900px;
        }
        .auto-style88 {
            width: 342px;
        }
        .auto-style94 {
            width: 900px;
            height: 44px;
        }
        .auto-style96 {
            width: 900px;
        }
        .auto-style104 {
            height: 37px;
        }
        .auto-style105 {
            width: 13%;
            height: 50px;
        }
        .auto-style106 {
            height: 27px;
        }
        .auto-style107 {
            width: 20%;
            height: 50px;
        }
        .auto-style113 {
            width: 273px;
            height: 21px;
        }
        .auto-style114 {
            height: 21px;
        }
        .auto-style115 {
            width: 409px;
            height: 21px;
        }
        .auto-style116 {
            height: 52px;
        }
        .auto-style117 {
            width: 268px;
            height: 50px;
        }
        .auto-style118 {
            height: 50px;
        }
 .PleaseWait {
    position: fixed;
    background-color: #FAFAFA;
    z-index: 2147483647 !important;
    opacity: 0.8;
    overflow: hidden;
    text-align: center;
    /*height: 100%;*/
    /*width: 100%;*/
    padding-top:20%;
        /*height: 64px;*/
        /*width: 100%;*/
        /*background-image: url(Controls/Images/WaitImage2.gif );*/
        /*background-repeat: no-repeat;*/
        /*padding-left: 70px;*/
        /*line-height :32px;*/
    }
        .auto-style119 {
            height: 71px;
            width: 270px;
        }
        .auto-style120 {
            height: 10px;
        }
        .auto-style121 {
            width: 900px;
            height: 70px;
        }
        .auto-style122 {
            width: 17%;
            height: 22px;
        }
        .auto-style123 {
            border: 1px solid ButtonFace;
            font-size: 12px;
            border-radius: 6px;
            color: black;
/*background-repeat: no-repeat;
        background-position:center;*/background-color: ButtonFace;
            padding: 0px;
            margin: 0px;
            z-index: 9999;
        /*background-image: url("Images\DDImageDown.bmp");*/
        /*padding: 5px 4px 0px 5px*/
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
.DataButtonEnabled
{
  width: 80px;
  height: 18px;
  font-size: small;
  border-radius: 5px;
  border-style :solid;
  border-color: #4e4747 ;
  color: black;
  border-width: 1px;
  /*background-image: linear-gradient(to bottom, rgba(158, 188, 250,0),rgba(158, 188, 250,1));*/
  background-image: linear-gradient(to bottom, white,rgb(230,236,255),rgb(189,206,255));
  padding: 0px;
  margin-top:12px;
  margin-bottom:3px;
  margin-left:5px;
  margin-right:5px;
  z-index: 9999; 
}

.DataButtonEnabled:hover {
    background-image: linear-gradient(to bottom, white,rgb(253,236,138),rgb(252,233,118));
}
.DataButtonEnabled:active { /* Mouse Down */
    background-image: linear-gradient(to bottom, rgb(189,206,255),rgb(230,236,255),white);
    border-color:black;
}

.DataButtonDisabled
{
  width: 80px;
  height: 18px;
  font-size: small;
  border-radius: 5px;
  border-style :solid;
  border-color: gray ;
  color:gray;
  border-width: 1px;
  /*background-image: linear-gradient(to bottom, rgba(158, 188, 250,0),rgba(158, 188, 250,1));*/
  background-image: linear-gradient(to bottom, white,rgb(239,242,246),rgb(189,206,255));
  padding: 0px;
  margin-top:12px;
  margin-bottom:3px;
  margin-left:5px;
  margin-right:5px;
  z-index: 9999; 
}

        .auto-style124 {
            height: 71px;
            width: 333px;
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
    <form id="form1" runat="server" >
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />      
        <asp:UpdatePanel ID="udpSqlQuery" runat ="server" >
            <ContentTemplate>
         <asp:HiddenField ID ="hdnAllTables" runat ="server" />
         <asp:HiddenField ID ="hdnSearchText" runat ="server" />
         <asp:HiddenField ID ="hdnReportTables" runat ="server" />

                 <table>
      <tr>
          <td colspan="3" style="text-align:left; font-size: x-large; font-style: normal; font-weight: bold; background-color: #e5e5e5; vertical-align: middle; height: 40px;">
                          <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label>
          </td>
          </tr> 
        <tr>
                    <td style="font-size: x-small; font-style: normal; font-weight: normal; background-color: #e5e5e5; vertical-align: top; text-align: left; width: 15%;">
                        <div id="tree" style="font-size: x-small; font-weight: normal; font-style: normal;">

                          <asp:TreeView ID="TreeView1"  runat="server" Width="100%" NodeIndent="10" Font-Names="Times New Roman"  EnableTheming="True" ImageSet="BulletedList">
                   <Nodes>  
                               <asp:TreeNode Text="&lt;b&gt;Log Off;&lt;/b&gt;"  Value="Default.aspx" Expanded="True" ></asp:TreeNode>

                              <asp:TreeNode Text="&lt;b&gt;List of Reports&lt;/b&gt;"  Value="ListOfReports.aspx" Expanded="True" ></asp:TreeNode>                              

                              <asp:TreeNode Text="&lt;b&gt;Report Definition&lt;/b&gt;"  Value="ReportEdit.aspx?tne=2" Expanded="True" ></asp:TreeNode>

                              <asp:TreeNode Text="&lt;b&gt;Report Parameters&lt;/b&gt;"  Value="ReportEdit.aspx?tne=3" Expanded="True" ></asp:TreeNode>

                              <%--<asp:TreeNode Text="&lt;b&gt;Share Report (Users)&lt;/b&gt;"  Value="ReportEdit.aspx?tne=4" Expanded="True" ></asp:TreeNode>--%>
 
                              <asp:TreeNode Text="Report Data Query"  Value="SQLquery.aspx?tnq=0" Expanded="True" >
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
                            <RootNodeStyle HorizontalPadding="2px" Font-Bold="True" Font-Underline="False" />
                            <NodeStyle CssClass="NodeStyle" />
                            <ParentNodeStyle Font-Bold="True" />
     </asp:TreeView>
     
    </div>
  </td>

            <td width="5px"></td>
                    <td id="main" style="width: 85%; text-align: left; vertical-align: top">  
        <div>
                <table id="tblTitle" style="width:100%; font-family: Arial;">
                  <tr>
                    <td style="width:61%;text-align:right">
                       <asp:Label ID="LabelAlert0" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large" ForeColor="Gray" Text="Report Data Query - "></asp:Label>
                       <asp:Label ID="LabelAlert" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large" ForeColor="Gray" Text="Label"></asp:Label>
                    </td>
                    <td style="width:26%;text-align:center;">
                       <asp:Label ID="lblView" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Medium" ForeColor="#000099" Text="Data Fields"></asp:Label>
                       <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/ListOfReports.aspx" CssClass="NodeStyle" Visible="false" Font-Size="12px">List of reports</asp:HyperLink>
                    </td>
 <%--                   <td style="width:13%;text-align:center;">
                      <asp:HyperLink ID="HyperLink6" runat="server"  NavigateUrl="~/ShowReport.aspx?srd=3"  Visible="False" Enabled="False" ToolTip="Show Report data" CssClass="NodeStyle" Font-Size="12px">Show report data</asp:HyperLink>    
                    </td>--%>
                    <td style="width:13%;text-align:center;">
                      <asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx?hilt=Report_Data_Definition" Target="_blank" CssClass="NodeStyle" Font-Size="12px">Help</asp:HyperLink>
                         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:HyperLink ID="HyperLinkLogOff" runat="server" NavigateUrl="~/Default.aspx" CssClass="NodeStyle" Font-Names="Arial">Log off</asp:HyperLink> 
                    </td>                                          
                  </tr>
                </table> 

<%--            <div style="width:100%; font-family: Arial; font-size: xx-small; text-align:center">
                       <asp:Label ID="lblView" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Medium" ForeColor="#000099" Text="Data Fields"></asp:Label>
            </div> --%>            

            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/ReportEdit.aspx" ToolTip="Edit Report" Enabled="False" Visible="False">Edit report</asp:HyperLink>
<asp:Menu ID="MenuMain"  Visible="false" Width="600px" runat="server" Orientation="Horizontal"  OnMenuItemClick="MenuMain_MenuItemClick" StaticSelectedStyle-BackColor="LightGray" DynamicSelectedStyle-BackColor="LightGray"    StaticSelectedStyle-Font-Bold="true"  StaticMenuItemStyle-BackColor="#e5e5e5" BorderWidth ="0px" Height="20px">
    <Items>
        <asp:MenuItem Text="Report Data " Value="0" ToolTip="Report Query to get data from db">
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
            <asp:Menu ID="Menu1" runat="server" BorderWidth="0px" OnMenuItemClick="Menu1_MenuItemClick" Orientation="Horizontal" DynamicSelectedStyle-BackColor="LightGray" StaticMenuItemStyle-BackColor="#e5e5e5" StaticSelectedStyle-BackColor="LightGray" StaticSelectedStyle-Font-Bold="true" Width="600px">
                <Items>
                    <asp:MenuItem Text=" Data Fields" ToolTip="SELECT statement" Value="0"></asp:MenuItem>                    
                    <asp:MenuItem Text=" Join Tables" ToolTip="JOIN statement" Value="1"></asp:MenuItem>
                    <asp:MenuItem Text=" Filters " ToolTip="Filter data by defined conditions" Value="2"></asp:MenuItem>
                    <asp:MenuItem Text=" Sorting" ToolTip="ORDER BY statement" Value="3"></asp:MenuItem>
                </Items>
            </asp:Menu>
            <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                <asp:View ID="Tab1" runat="server">
                    <table style="border-style:solid;border-width: 1px;padding:0px;margin:0px;background-color:lightgray;width:100%">
                         
                        <tr valign="top">
                            <td align="left" class="auto-style121">
                                <table id="SQLselect" runat="server" bgcolor="#e5e5e5" rules="rows" style=" font-size: small;border:medium double #FFFFFF;
                                        color: black; font-family: Arial; background-color: #e5e5e5" width="480px">
                                    <tr>
                                        <td align="left" bgcolor=" #e5e5e5"  style="color: black; font-family: Arial; letter-spacing: normal;border:medium double #FFFFFF; ">
                                            <asp:Label ID="Label10" runat="server" Font-Underline="False" Text="SELECT " ForeColor="Black"></asp:Label>
                                            &nbsp;&nbsp;&nbsp;<asp:Label ID="LabelNoGroups0" runat="server" Text=" "></asp:Label>
                                        
                                            &nbsp;&nbsp;&nbsp;<asp:CheckBox ID="CheckBoxDistinct" runat="server" Text="DISTINCT"  ForeColor="Black"/>
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td  colspan="3" style=" background-color:#e5e5e5; text-align: left; text-wrap:none; font-family:Tahoma; font-size: small; color:black; border:medium double #FFFFFF;"  >
                                            Table:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            <div style="display:inline-block; border: 1px solid #C0C0C0; background-color: #FFFFFF; width: 240px; height: 20px;">
                                              <input id="txtSearch" runat="server"  type="search" placeholder="Search tables..." style="border-width: 1px; border-color: #C0C0C0; padding: 0px 0px 0px 3px; margin: 3px 0px 0px 3px; font-family: Tahoma; font-size: small; border-style: none solid none none; height:18px; width:200px; outline-style: none;" />
                                                <div id="divSearch" class="divSearch"   title="Do Search">
                                                       <asp:ImageButton ID="imgSearch" runat="server"  tabindex="0"  ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/SearchIcon3.ico" Height="18" Width="18" BorderStyle="None" BorderWidth="1px" BorderColor="Transparent" BackColor="Transparent"  CssClass="imgSearch" Visible="True" />
                                                      <%--<asp:Button  ID="btnSearch" runat ="server" Text="&#128269;" CausesValidation="False" UseSubmitBehavior="False" ToolTip="Do Search" CssClass="btnSearch" Visible="false" />--%>
                                                </div>
                                            </div>
                                            <asp:CheckBox ID="CheckBoxSysTables" runat="server" AutoPostBack="True" Text="show system tables" />
                                            <%--<img src="~controls/images/inf.png" width="24" height="24"/>--%>
                                            <asp:Label ID="lblTableAlert" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Medium" ForeColor="red" Text=" "></asp:Label>
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td align="left" nowrap="nowrap" style="font-weight: normal; font-size: small; color: white;border:medium double #FFFFFF;
                                          font-family: Arial; background-color: #e5e5e5; " valign="top" class="auto-style119">
                                             
                                            <asp:ListBox ID="DropDownTables" runat="server" AutoPostBack="False" Width="540px">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:ListBox>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="font-size: small; color: black; background-color: #e5e5e5;border:medium double #FFFFFF; font-weight: normal;" valign="top" class="auto-style124" >
                                              Fields: 
                                              <asp:Button id="btnSelectAll" runat="server" CssClass="DataButtonEnabled" Text="Select All"  />
                                              <asp:Button id="btnUnselectAll" runat="server" CssClass="DataButtonEnabled" Text="UnselectAll"  />
                                              <asp:CheckBox ID="CheckBoxSelectAllFields" runat="server" AutoPostBack="True" Text="select all" Visible="False" />
                                              &nbsp;&nbsp;&nbsp;&nbsp;
                                              <asp:CheckBox ID="CheckBoxUnselectAllFields" runat="server" AutoPostBack="True" Text="unselect all" Visible="False" />
                                      <br />
                                            <uc1:DropDownColumns ID="DropDownColumns" runat="server" Width="580px" ClientIDMode="Predictable" FontName="arial" FontSize="Small" BorderColor="Silver" ForeColor="Black" BorderWidth="1" DropDownHeight="190px" FontBold="False" DropDownButtonHeight="22px" TextBoxHeight="22px" PostBackType="None" />
                                            <asp:Label ID="lblColumnsAlert" runat="server" Text=" " Visible="False" Enabled="False" Width="300px" Height="22px" BorderWidth="1px" BorderStyle="Solid" BorderColor="Silver" BackColor="White" ForeColor="Silver"></asp:Label>
                                        </td>
                                    </tr>
                                    
                                </table>
                                            
                                            <%--<br />--%>
                                <asp:Button ID="ButtonSelectFields" runat="server" Text="Select Fields to the Query" Width="200px" CssClass="ticketbutton"/>
                                <br /><asp:Label ID="Label1" runat="server" Text="Selected" Visible="False" Enabled="False"></asp:Label><br />
                            </td>
                        </tr>
                        <tr>
                            <td align="left" bgcolor="#999999" class="auto-style87" nowrap="nowrap" style=" font-size: small; color: white;
                                    font-family: Arial; background-color: #C0C0C0; " valign="top">
                                <table id="SQLfields" runat="server" bgcolor="#663300" border="1" rules="rows" style=" font-size: small; color: black; font-family: Arial; background-color: #ffffff" width="100%"> 
                                    <tr>                                        
                                        <td align="left" nowrap="nowrap" style="font-weight: bold; font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; " class="auto-style113">
                                            Tables selected:&nbsp;
                                        </td>                
                                        <td align="left" nowrap="nowrap" style="font-weight: bold; font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; " class="auto-style84"  >
                                            Fields selected:&nbsp;
                                        </td> 
                                        <td align="left"  nowrap="nowrap" style="font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; font-weight: bold;" class="auto-style114" >
                                            Del&nbsp;
                                        </td>  
                                        <td align="left"  nowrap="nowrap" style="font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; font-weight: bold;" class="auto-style114" >
                                            Friendly Names&nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                      
                        <tr>
                            <td align="left" bgcolor="#999999" style="font-weight: bold; font-size: small; color: white;
                                font-family: Arial; background-color: #C0C0C0; " valign="top" class="auto-style86">
                                <asp:Label ID="LabelSQL" runat="server" Text="SQL query:" ForeColor="Black"></asp:Label>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td class="auto-style104" >
                                <table id="Table4" runat="server" bgcolor="LightGray" border="0" >
                                    <tr>
                                        <td align="left" bgcolor="LightGray" style="color: black; font-family: Arial; letter-spacing: normal; " class="auto-style88">
                                            <asp:Label ID="Label12" runat="server" Text="Update report:"></asp:Label>
                                            &nbsp;&nbsp;&nbsp;<asp:Label ID="Label13" runat="server" Text=" "></asp:Label>&nbsp;&nbsp;&nbsp;
                                        <%--</td>
                                        <td align="left" style="color: #ffffff; font-family: Arial; letter-spacing: normal;">--%>
                                            <asp:Button ID="ButtonSubmitSELECT" runat="server" Text="Submit" Width="131px" ToolTip="Submit SQL query" BackColor="#7CDC99" CssClass="ticketbutton" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:View>
                
                <asp:View ID="Tab2" runat="server">
                    <table style="border-style:solid;border-width: 1px;padding:0px;margin:0px;background-color:lightgray;width:100%">
                        <tr valign="top">
                            <td width="100%">
                                <table id="Table7" runat="server" bgcolor="LightGray" border="0" width="100%">
                                    <tr>
                                        <td align="left" bgcolor="LightGray" style="color: black; font-family: Arial; letter-spacing: normal; " class="auto-style41">
                                            <asp:Label ID="Label19" runat="server" Font-Underline="False" Text="Add Join manually:"></asp:Label>
                                            &nbsp;<asp:Label ID="LabelNoGroups2" runat="server" Text=" "></asp:Label>
                                        </td>
                                        <td align="left" width="60%" style="color: #ffffff; font-family: Arial; letter-spacing: normal;" class="auto-style106">
                                            &nbsp;</td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td align="left" width="100%">
                                <table id="Joins" runat="server" bgcolor="#e5e5e5" border="1" rules="rows" style=" font-size: small;
                                            color: black; font-family: Arial; background-color: black;border:medium double #FFFFFF;" width="40%">
                                 
                                    <tr valign="top">
                                        <td align="left" bgcolor="#e5e5e5" nowrap="nowrap" style="font-weight: bold; font-size: small; color: black;
                                                    font-family: Arial; background-color: #e5e5e5;border:medium double #FFFFFF; " valign="top" class="auto-style117" >
                                            <br />
                                            &nbsp;<asp:CheckBox ID="CheckBoxAllTables1" runat="server" AutoPostBack="True" Text="show all tables" Font-Italic="True" Font-Names="Arial" Font-Size="X-Small" ForeColor="#3333CC" />
                                            &nbsp;Table1:
                                            &nbsp;&nbsp;
                                            <asp:DropDownList ID="DropDownTableJ1" runat="server" AutoPostBack="True">
                                            </asp:DropDownList>
                                             
                                        <%--</td>
                                        <td align="left"  style="font-weight: bold; font-size: small; color: white;
                                                font-family: Arial; background-color: #999999" valign="top" class="auto-style107" >--%>
                                            &nbsp;Field1:&nbsp;&nbsp;
                                             
                                            <asp:DropDownList ID="DropDownFieldJ1" runat="server" AutoPostBack="True">
                                            </asp:DropDownList> 
                                            
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left"  style="font-weight: bold; font-size: small; color: black;
                                                font-family: Arial; background-color: #e5e5e5;border:medium double #FFFFFF;" class="auto-style105" >
                                            &nbsp;Join Type:&nbsp;&nbsp;&nbsp;<%--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
                                            <%--<br />--%>
                                            <asp:DropDownList ID="DropDownJoinType" runat="server" AutoPostBack="True">
                                                <asp:ListItem Value="INNER JOIN">Join</asp:ListItem>
                                                <asp:ListItem Value="LEFT JOIN">Left Join</asp:ListItem>
                                            </asp:DropDownList>
                                             
                                                                                      
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td align="left" width="20%"  bgcolor="#e5e5e5"  nowrap="nowrap" style="font-weight: bold; font-size: small; color: black;
                                                    font-family: Arial; background-color: #e5e5e5;border:medium double #FFFFFF; " class="auto-style118" >
                                            <br />
                                            &nbsp;<asp:CheckBox ID="CheckBoxAllTables2" runat="server" AutoPostBack="True" Text="show all tables"  Font-Italic="True" Font-Names="Arial" Font-Size="X-Small" ForeColor="#3333CC"  />
                                            &nbsp;Table2:&nbsp;&nbsp;                                            
                                            
                                            <asp:DropDownList ID="DropDownTableJ2" runat="server" AutoPostBack="True">
                                            </asp:DropDownList>
                                        
                                            
                                            &nbsp;Field2:&nbsp;&nbsp;
                                             
                                            <asp:DropDownList ID="DropDownFieldJ2" runat="server" AutoPostBack="True">
                                            </asp:DropDownList> 

                                           <br />&nbsp;&nbsp;
                                        </td></tr>
                                    <tr valign="top"> 
                                        <td align="left" nowrap="nowrap"  style="font-weight: bold; font-size: small; color: white;
                                                font-family: Arial; background-color: #e5e5e5;border:medium double #FFFFFF; " valign="top" class="auto-style40">
                                            <%--&nbsp;<asp:Label ID="Label21" runat="server" Text="Add Join"></asp:Label>--%>
                                            <br />&nbsp;<asp:Button ID="ButtonAddJoin" runat="server" Text="Add Join" Width="131px" CssClass="ticketbutton"/>
                                        </td>
                                    </tr>                                    
                                </table>
                            </td>
                        </tr>



                        <tr valign="top">
                            <td align="left" width="100%">
                                <table id="ListOfJoins" runat="server" bgcolor="#663300" border="1" rules="rows" style=" font-size: small;
                                            color: black; font-family: Arial; background-color: #ffffff" width="100%">
                                    <tr valign="top">
                                        <td  align="left" bgcolor="LightGray" style="color:black; font-family: Arial; letter-spacing: normal; " class="auto-style41" >Or add from the list of possible Joins for report tables if any:<br />                                            
                                        </td>  
                                        <td  align="left" bgcolor="LightGray" style="color: black; font-family: Arial; letter-spacing: normal; " class="auto-style41" ><br />                                            
                                        </td> 
                                    </tr> 
                                    
                                </table>
                            </td>
                        </tr>
                        <tr valign="top">
                                        <td align="left" bgcolor="LightGray" style="color: black; font-family: Arial; letter-spacing: normal; " class="auto-style41"  > Joins added so far:<br />                                            
                                        </td>  
                                        <td></td>
                                        
                        </tr> 
                        <tr>
                            <td align="left" bgcolor="#999999" class="auto-style96" nowrap="nowrap" style=" font-size: small; color: white;
                        font-family: Arial; background-color: #C0C0C0; " valign="top">
                                <table id="SQLJoins" runat="server" bgcolor="#663300" border="1" rules="rows" style=" font-size: small; color: black; font-family: Arial; background-color: #ffffff" width="100%"> 
                                    <tr>                                        
                                        <td align="left" nowrap="nowrap" style="font-weight: bold; font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; width:15%; "  >
                                            Table1:&nbsp;</td>                
                    
                                        <td align="left"  nowrap="nowrap" style="font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; font-weight: bold; width:12%;"  >
                                            Field1&nbsp; </td>
                                        <td align="left"  nowrap="nowrap" style="font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; font-weight: bold; width:10%;" >
                                            Join Type&nbsp; </td>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold; font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; width:15%; "   >
                                            Table2:&nbsp;</td>  
                                        <td align="left"  nowrap="nowrap" style="font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; font-weight: bold; width:12%;" >
                                            Field2&nbsp; </td>               
                    
 <%--                                       <td align="left"  nowrap="nowrap" style="font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; font-weight: bold;" >
                                             Up&nbsp;&nbsp;&nbsp; Down&nbsp;&nbsp;&nbsp; Del&nbsp;&nbsp;&nbsp; Reverse&nbsp;&nbsp;&nbsp;
                                        </td>
--%>
                                       <td align="left"  nowrap="nowrap" style="font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; font-weight: bold; width:5%;" >
                                            Up &nbsp; 
                                       </td>
                                        <td align="left"  nowrap="nowrap" style="font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; font-weight: bold; width:5%;" >
                                            Down &nbsp; 
                                       </td>
                                        <td align="left"  nowrap="nowrap" style="font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; font-weight: bold; width:8%;" >
                                            Reverse &nbsp; 
                                       </td>   
                                        <td align="left"  nowrap="nowrap" style="font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; font-weight: bold; width:18%;" >
                                            Delete &nbsp; 
                                       </td>                  
                    
                                    </tr>
                                </table>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                                        <td class="auto-style19" style="font-weight: bold; color: Gray; font-family: Arial; background-color: #C0C0C0;
                                            font-size: medium; text-decoration: underline;">
                                        </td>
                                       <td></td>
                        </tr>
                        <tr>
                            <td align="left" bgcolor="LightGray" width="100%" style="font-weight: bold; font-size: small; color: black;
                                font-family: Arial; background-color: #C0C0C0; " valign="top">
                                <asp:Label ID="LabelSQL1" runat="server" Text="SQL query:" ForeColor="Black"></asp:Label>
                            </td>
                            <td></td>
                        </tr>
                        <tr valign="top">
                            <td class="auto-style94">
                                <table id="Table8" runat="server" bgcolor="LightGray" border="0" class="auto-style9">
                                    <tr>
                                        <td align="left" bgcolor="LightGray" class="auto-style22" style="color: black; font-family: Arial; letter-spacing: normal; ">
                                            <asp:Label ID="Label22" runat="server" Text="Update report:"></asp:Label>
                                            &nbsp;&nbsp;&nbsp;<asp:Label ID="Label23" runat="server" Text=" "></asp:Label>
                                        </td>
                                        <td align="left" class="auto-style6" style="color: #ffffff; font-family: Arial; letter-spacing: normal;">
                                            <asp:Button ID="ButtonSubmitJoins" runat="server"  BackColor="#7CDC99" CssClass="ticketbutton" Text="Submit" Width="131px" ToolTip="Submit SQL query" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                </asp:View>



                <asp:View ID="Tab3" runat="server">
                  <table style="border-style:solid;border-width: 1px;padding:0px;margin:0px;background-color:gray;width:100%" >
                     <tr valign="top" id="trConditionEntry" runat="server" visible="false" >
                            <td class="auto-style7" style="border-width: 0px">
                                <table id="Table5" runat="server" border="0" class="auto-style7" style="background-color: lightgray; color: #FFFFFF;">
                                   
                                    <tr align="center" border="0">
                                      <td style="font-size: x-large; border-width: 0px; font-family: Arial; color:black;">
                                        <asp:Label ID="lblConditionEntry" runat ="server" Text="New Condition" > </asp:Label>
                                        </td>
                                    </tr>

                                    <tr>
                                         <td align="left" style="color: #FFFFFF; font-family: Arial; letter-spacing: normal">
                                             <asp:DropDownList ID="DropDownTableW1" runat="server" AutoPostBack="True" Visible ="false">
                                            </asp:DropDownList>
                                            &nbsp;&nbsp;&nbsp; Field:&nbsp;
                                            <asp:DropDownList ID="DropDownFieldW1" runat="server" AutoPostBack="True">
                                            </asp:DropDownList>
                                            &nbsp;&nbsp;&nbsp; Operator:&nbsp;
                                            <asp:DropDownList ID="DropDownOperator" runat="server" AutoPostBack="True">
                                                <asp:ListItem>=</asp:ListItem>
                                                <asp:ListItem>&gt;</asp:ListItem>
                                                <asp:ListItem>&lt;</asp:ListItem>
                                                <asp:ListItem>&lt;=</asp:ListItem>
                                                <asp:ListItem>&gt;=</asp:ListItem>
                                                <asp:ListItem>&lt;&gt;</asp:ListItem>
                                                <asp:ListItem>In</asp:ListItem>
                                                <asp:ListItem>between</asp:ListItem>
                                               <asp:ListItem>Contains</asp:ListItem>
                                               <asp:ListItem>StartsWith</asp:ListItem>
                                               <asp:ListItem>EndsWith</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:Label ID="lblValue" runat="server" Visible ="true"  >&nbsp;Value:&nbsp</asp:Label>
                                            <asp:Label ID="lblValue1" runat="server" visible="false" >&nbsp;&nbsp;Values:&nbsp</asp:Label>
                                            <div class="inline" id="divTextBlock" runat="server" visible="true">
                                                <div class="inline" id="divText1" runat="server" visible="true"><asp:TextBox ID="TextBoxStatic" runat="server" AutoPostBack="True" Width="185px" ></asp:TextBox> </div>
                                                <div class="inline" id="divField1" runat="server" visible="false" ><asp:DropDownList ID="DropDownFieldW2" runat="server" AutoPostBack="True"></asp:DropDownList> </div>
                                                <div class="inline" id="divTxtBtn1" runat="server" visible="true">&nbsp<asp:Button ID="btnTxt1" runat="server" Text="Fields" ToolTip="Choose field " CssClass="ButtonStyle2" TabIndex="-1" /> </div>
                                                <div class="inline" id="divAnd1" runat="server" visible="false"><asp:Label ID="lblAnd" runat="server">&nbsp;&nbsp;And&nbsp;&nbsp</asp:Label> </div>
                                                <div class="inline" id="divText2" runat="server" visible="false"><asp:TextBox ID="TextBoxStatic2" runat="server" AutoPostBack="True" Width="185px" ></asp:TextBox> </div>
                                                <div class="inline" id="divField2" runat="server" visible="false"><asp:DropDownList ID="DropDownFieldW3" runat="server" AutoPostBack="True"></asp:DropDownList> </div>
                                                <div class="inline" id="divTxtBtn2" runat="server" visible="false"><asp:Button ID="btnTxt2" runat="server" Text="Fields" ToolTip="Choose field " CssClass="ButtonStyle2" TabIndex="-1" /> </div>
                                            </div>

                                            <div class="inlinebottom" id="divCalendarBlock" runat="server" visible="false">
                                              <div class="inlinebottom" id="divCalendar1" runat="server"  visible="true"><uc2:CalendarDropDown ID="ddDate1" runat="server" FontBold="False" FontName="Arial" FontSize="small" ForeColor="Black" /> </div>
                                              <div class="inlinebottom" id="divField3" runat="server" visible="false"><asp:DropDownList ID="DropDownFieldW4" runat="server" AutoPostBack="True"></asp:DropDownList> </div>
                                              <div class="inlinebottom" id="divCalBtn1" runat="server" visible="true">&nbsp<asp:Button ID="btnCal1" runat="server" Text="Fields" ToolTip="Choose field " CssClass="ButtonStyle2" TabIndex="-1" /> </div>
                                              <div class="inlinebottom" id="divAnd2" runat="server" visible="true"><asp:Label ID="lblAnd2" runat="server">&nbsp;&nbsp;And&nbsp;&nbsp</asp:Label> </div>
                                              <div class="inlinebottom" id="divcalendar2" runat="server" visible="true"><uc2:CalendarDropDown ID="ddDate2" runat="server" FontName="Arial" FontSize="Medium" ForeColor="Black" Visible="true"  /> </div>
                                              <div class="inlinebottom" id="divField4" runat="server" visible="false"><asp:DropDownList ID="DropDownFieldW5" runat="server" AutoPostBack="True"></asp:DropDownList> </div>
                                              <div class="inlinebottom" id="divCalBtn2" runat="server" visible="true">&nbsp<asp:Button ID="btnCal2" runat="server" Text="Fields" ToolTip="Choose field " CssClass="ButtonStyle2" TabIndex="-1" /> </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" width="100%">
                                          <br />
                                          <asp:Button ID="ButtonAddCondition" runat="server" Text="Add Condition" CssClass="ButtonStyleSubmit"   />
                                            &nbsp;&nbsp;&nbsp
                                          <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="ButtonStyleSubmit"   />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                      <tr id="trConditionList" runat="server" align="left" valign="top" visible="true">
                          <td class="auto-style19" style="border-width: 0px">
                              <div style="padding: 0px; font-family: Arial; font-size: x-large; color: black; background-color: lightgray; border-width: 0px;  width: 100%" >
                                  <div valign="center" align="left" style="padding: 2px 0px 2px 0px; border-width: 0px; display: inline-block; font-family: Arial, Helvetica, sans-serif; font-size: medium;" class="auto-style122">
                                      <%--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
                                      Conditions:
                                  </div>
                                  <div valign="center" align="left" style="padding: 3px 0px 4px 0px; border-width: 0px; display: inline-block; width: 40%; height: 30px;">
                                      <asp:Button ID="btnNewCondition" runat="server" CssClass="ButtonStyle2" text="New Condition" ToolTip="Define a new condition" />
                                      <asp:Button ID="btnCustomizeLogic" runat="server" CssClass="ButtonStyle2" text="Customize Logic" ToolTip="Customize condition logic" />
                                  </div>
                              </div>
                              <table id="SQLConditions" runat="server" class="conditions" rules="rows" style="width:100%;border-width :0px" visible="true">
                                  <tr class="tr" style="border-width: 0px;">
                                      <td class="columns" style="width:15%;">Table&nbsp; </td>
                                      <td class="columns" style="width: 10%;">Field&nbsp; </td>
                                      <td class="columns" style="width: 10%;">Operator&nbsp;</td>
                                      <td class="columns" style="width: 10%;">Value 1 </td>
                                      <td class="columns" style="width: 10%;">Value 2 </td>
                                      <td class="columns" style="width: 8%;">Logical Op </td>
                                      <td class="columns" style="width: 8%;">Group </td>
                                      <td class="columns" style="width: 8%;">Rec Order </td>
                                      <td class="columns" style="width: 10%;">&nbsp;&nbsp </td>
                                      <td class="columns" style="width: 10%;">&nbsp;&nbsp;</td>
                                    </tr>
                             </table>
                          </td>
                                  </tr>
                        <tr>
                            <td align="left" width="100%" style="border-width: 0px; background-color:lightgray ; font-weight: bold; font-size: 12px; 
                        font-family: Arial;  " valign="top">
                                <div style="border-width: 0px; font-weight: bold; color: black; font-family: Arial; background-color:lightgray ;font-size: medium; ">
                                   
                                </div>
                                <asp:Label ID="LabelSQL2" runat="server" Text="SQL query:" ForeColor="Black"></asp:Label>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td style="border-width: 0px" bgcolor="lightgray">
                                <table id="Table6" runat="server" bgcolor="lightgray" border="0" class="auto-style9">
                                    <tr>
                                        <td align="center" class="auto-style6" width="100%" style="border-width: 0px; color: black; font-family: Arial; letter-spacing: normal;">
                                            <asp:Button ID="ButtonSubmitWHERE" runat="server"  BackColor="#7CDC99" CssClass="ticketbutton" Text="Save Conditions" Width="131px" ToolTip="Update report" Height="25px" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:View>


                  
                <asp:View ID="Tab4" runat="server">
                    <table style="border-style:solid;border-width: 1px;padding:0px;margin:0px;background-color:lightgray;width:100%">
                        <tr valign="top">
                            <td >
                                <table id="Table1" runat="server" bgcolor="LightGray" border="0" >
                                    
                                </table>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td align="left" class="auto-style94">
                                <table id="Table2" runat="server" bgcolor="#e5e5e5" border="1" rules="rows" style=" font-size: small;
                                        color: black; font-family: Arial;border:medium double #FFFFFF; background-color: #ffffff;" width="30%">
                                    
                                    <tr>
                                        <td align="left" bgcolor="#e5e5e5"  style="color: black; font-family: Arial; letter-spacing: normal; background-color: #e5e5e5;border:medium double #FFFFFF;  ">
                                            <asp:Label ID="Label2" runat="server" Font-Underline="False" Text="SORT BY:"></asp:Label>
                                            &nbsp;&nbsp;&nbsp;<asp:Label ID="Label3" runat="server" Text=" "></asp:Label>
                                       <%-- </td>
                                        <td align="left"  style="color: black; font-family: Arial; letter-spacing: normal;">--%>
                                            &nbsp;&nbsp;<asp:DropDownList ID="DropDownSortType" runat="server">
                                                <asp:ListItem>ASC</asp:ListItem>
                                                <asp:ListItem>DESC</asp:ListItem>
                                            </asp:DropDownList>
                                            &nbsp;</td>
                                    </tr>
                                    
                                    <tr valign="top">
                                        <td align="left" bgcolor="#e5e5e5" nowrap="nowrap" style="font-weight: normal; font-size: small; color: black;
                                                 font-family: Arial; background-color: #e5e5e5;border:medium double #FFFFFF; " valign="center" class="auto-style116">
                                             <br />
                                            Tables:&nbsp;&nbsp;<%--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
                                          
                                            <asp:DropDownList ID="DropDownTableS1" runat="server" AutoPostBack="True">
                                            </asp:DropDownList>
                                        <%--</td>
                                       <td align="left" style="font-size: small; color: white;
                                                 background-color: #999999; font-weight: normal;" valign="top" class="auto-style116" >--%>
                                            <%--&nbsp;&nbsp;&nbsp; &nbsp;--%>
                                            &nbsp;&nbsp;&nbsp;Fields:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                     
                                            <asp:DropDownList ID="DropDownFieldS1" runat="server" AutoPostBack="True">
                                            </asp:DropDownList>
                                           <%-- <br />--%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left"  nowrap="nowrap" style="font-weight: bold; font-size: small; color: black;
                                                font-family: Arial; background-color: #e5e5e5;border:medium double #FFFFFF; " valign="top" class="auto-style116">
                                            
                                            <asp:Button ID="ButtonAddSort" runat="server" Text="Add Field to Sorting" Width="211px" CssClass="ticketbutton"/>

                                           <br />
                                           <asp:Label ID="Label4" runat="server" Text="Selected" Visible="False" Enabled="False"></asp:Label>
                                            
                                        </td>
                                    </tr>                                    
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="left" bgcolor="#999999" class="auto-style87" nowrap="nowrap" style=" font-size: small; color: white;
                                    font-family: Arial; background-color: #C0C0C0; " valign="top">
                                <table id="SQLSort" runat="server" bgcolor="#663300" border="1" rules="rows" style=" font-size: small; color: black; font-family: Arial; background-color: #ffffff" width="100%"> 
                                    <tr>                                        
                                        <td align="left" nowrap="nowrap" style="font-weight: bold; font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; width: 28%; " class="auto-style113">
                                            Tables selected:&nbsp;
                                        </td>                
                                        <td align="left" nowrap="nowrap" style="font-weight: bold; font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; width: 28%; " class="auto-style115"  >
                                            Sort by fields:&nbsp;
                                        </td> 
<%--                                        <td align="left"  nowrap="nowrap" style="font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; font-weight: bold;" class="auto-style114" >
                                            Up&nbsp;&nbsp; Down&nbsp;&nbsp;&nbsp;&nbsp; Del&nbsp;
                                        </td>--%>
                                        <td align="left"  nowrap="nowrap" style="font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; font-weight: bold; width: 5%;" class="auto-style114" >
                                            Up
                                        </td> 
                                        <td align="left"  nowrap="nowrap" style="font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; font-weight: bold; width: 5%" class="auto-style114" >
                                            Down
                                        </td>
                                        <td align="left"  nowrap="nowrap" style="font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; font-weight: bold; width: 5%;" class="auto-style114" >
                                            Delete
                                        </td>                                                                                 
                                        <td align="left"  nowrap="nowrap" style="font-size: small; color: white;
                                            font-family: Arial; background-color: #C0C0C0; font-weight: bold; width: 29%;" class="auto-style114" >
                                            Friendly Names&nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <%--<tr>
                            <td class="auto-style87" style="font-weight: bold; font-size: medium; color: Gray;
                                font-family: Arial; background-color: #C0C0C0; text-decoration: underline;">
                               
                            </td>
                        </tr>--%>
                        <tr>
                            <td align="left" bgcolor="#999999" style="font-weight: bold; font-size: small; color: white;
                                font-family: Arial; background-color: #C0C0C0; " valign="top" class="auto-style86">
                                <asp:Label ID="LabelSQLsort" runat="server" Text="SQL query:" ForeColor="Black"></asp:Label>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td class="auto-style104" >
                                <table id="Table10" runat="server" bgcolor="LightGray" border="0" >
                                    <tr>
                                        <td align="left" bgcolor="LightGray" style="color: black; font-family: Arial; letter-spacing: normal; " class="auto-style88">
                                            <asp:Label ID="Label6" runat="server" Text="Update report:"></asp:Label>
                                           
                                        <%--</td>
                                        <td align="left" style="color: #ffffff; font-family: Arial; letter-spacing: normal;">--%>
                                            <asp:Button ID="ButtonSubmitSorting" runat="server"  BackColor="#7CDC99" CssClass="ticketbutton" Text="Submit" Width="131px" ToolTip="Submit SQL query" />
                                         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="Label7" runat="server" Text=" "></asp:Label>
                                        
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:View>
            
            </asp:MultiView>
<%--            <asp:Label ID="LabelAlert1" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large" ForeColor="Gray" Text=" _______________________________________________________"></asp:Label>--%>
                <asp:Label ID="LabelAlert1" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large" ForeColor="Gray" Text=" "></asp:Label>

            <%--<br />--%>  
            
           </div>
          </td>
        </tr>
       </table>
              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
              <asp:Label ID="LabelReportID" runat="server" Font-Bold="False" Font-Names="Arial" Font-Size="XX-Small" ForeColor="Black" Text=" "></asp:Label>                 

        <ucDlgCustomizeLogic:DlgCustomizeLogic id="dlgCustomizeLogic" runat="server" Width="750px" />
        <ucMsgBox:Msgbox id="MessageBox" runat ="server" > </ucMsgBox:Msgbox>
        <ucDlgCondition:DlgCondition id="dlgCondition" runat="server" FontName="Tahoma" FontSize="14px" Width="715px"/>
        </ContentTemplate>
        </asp:UpdatePanel>
            <div id="spinner" class="modal" style="display:none;">
                <div id="divSpinner" style="text-align: center; width: 130px; display: inline-block; clip: rect(auto, auto, auto, 50%); position: fixed; margin-top: 300px; margin-right: auto; padding-top: 10px; background-color: #f8f8d3; z-index: 2147483647; left: 50%; top: -5%;">
                  <img id="imgSpinner" src="Controls/Images/WaitImage2.gif" style="width: 100px; height: 100px" />
                    <br />
                      Please Wait...
                </div>
            </div>

        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpSqlQuery">
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
