<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Analytics.aspx.vb" Inherits="Analytics" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Analytics</title>
<style type="text/css">
        .auto-style1 {
            width: 107px;
            height: 26px;
        }
        .auto-style2 {
            width: 300px;
            height: 26px;
        }
        .auto-style3 {
            width: 247px;
            height: 26px;
        }
        .auto-style4 {
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
.ticketbutton 
{
  width: 80px;
  height: 25px;
  font-size: 12px;
  border-radius: 5px;
  border-style :solid;
  border-color: #4e4747 ;
  color: black;
  border-width: 1px;
  /*background-color: ButtonFace;*/
  background-image: linear-gradient(to bottom, rgba(211, 211, 211,0),rgba(211, 211, 250,3));  
  /*rgba(158, 188, 250,0),rgba(158, 188, 250,1));*/

  padding: 3px;
  margin:5px;
  z-index: 9999; 
}
        </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />      
    <asp:UpdatePanel ID="udpTablesList" runat ="server" >
       <ContentTemplate>
            <div>
           <table>
      <tr>
          <td colspan="3" style="font-size:x-large; font-style:normal; font-weight:bold; background-color: #e5e5e5; vertical-align:middle; text-align: left; height: 40px;">
              <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label>
          </td>
      </tr> 
        <tr>
            <td style="font-size: x-small; font-style: normal; font-weight: normal; background-color: #e5e5e5; vertical-align: top; text-align: left; width: 15%;">
                    <div id="tree" style="font-size: x-small; font-weight: normal; font-style: normal">
                        <%--<br /><br />--%>

     <asp:TreeView ID="TreeView1"  runat="server" Width="100%" NodeIndent="10" Font-Names="Times New Roman"  EnableTheming="True" ImageSet="BulletedList">
          <Nodes>  
             <asp:TreeNode Text="&lt;b&gt;Log Off&lt;/b&gt;"  Value="Default.aspx" Expanded="True" ></asp:TreeNode>

             <asp:TreeNode Text="&lt;b&gt;List of Reports&lt;/b&gt;"  Value="ListOfReports.aspx" Expanded="True" ></asp:TreeNode>                              
            
             <asp:TreeNode Text="&lt;b&gt;Report Definition&lt;/b&gt;" Value="ReportEdit.aspx?tne=2" Expanded="True" ></asp:TreeNode>

             <asp:TreeNode Text="&lt;b&gt;Report Data Query&lt;/b&gt;"  Value="SQLquery.aspx?tnq=0" Expanded="True" ></asp:TreeNode>
              
             <asp:TreeNode Text="&lt;b&gt;Report Format Definition&lt;/b&gt;"  Value="RDLformat.aspx?tnf=0" Expanded="True" ></asp:TreeNode>

             <%--<asp:TreeNode Text="&lt;b&gt;Advanced Report Designer&lt;/b&gt;"  Value="ReportDesigner.aspx" Expanded="True" ></asp:TreeNode>--%>

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
    <div style="text-align: center;width:100%;">
    <div style="text-align: center;">
      <%--  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
           <asp:HyperLink ID="HyperLinkListOfReports" runat="server" NavigateUrl="~/ListOfReports.aspx" Enabled="True" Visible="True" CssClass="NodeStyle" Font-Names="Arial">List of Reports</asp:HyperLink>
          --%>  
       <%--    <asp:LinkButton ID="LinkButtonRefresh" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="May take a long time...">Recalculate Analytics</asp:LinkButton> 
     --%>
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/ShowReport.aspx?srd=12" CssClass="NodeStyle" Font-Names="Arial">Correlation</asp:HyperLink>
        
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/ShowReport.aspx?srd=8" CssClass="NodeStyle" Font-Names="Arial">Data and Statistics</asp:HyperLink>

         &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLink5" runat="server" NavigateUrl="~/ShowReport.aspx?srd=3" CssClass="NodeStyle" Font-Names="Arial">Report and Charts</asp:HyperLink>
              
         &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLinkListOfDashboards" runat="server" NavigateUrl="~/ListOfDashboards.aspx" CssClass="NodeStyle" Font-Names="Arial">List of User Dashboards</asp:HyperLink>
        
         &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLinkAdvancedAnalytics" runat="server" NavigateUrl="~/AdvancedAnalytics.aspx" CssClass="NodeStyle" Font-Names="Arial">Matrix Balancing</asp:HyperLink>
      
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/FriendlyNames.aspx" CssClass="NodeStyle" Font-Names="Arial" Enabled="False" Visible="False">FriendlyNames</asp:HyperLink>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp; 
        <asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx" Target="_blank" CssClass="NodeStyle" Font-Names="Arial">Help</asp:HyperLink>&nbsp;&nbsp;
                 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
       <%-- <asp:LinkButton ID="LinkButtonHelpDesk" runat="server" OnClientClick="target='_blank'" CssClass="NodeStyle" Font-Names="Arial">Report a problem</asp:LinkButton> 
         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        !! DO NOT DELETE, NEXT LINE IS FOR TESTING ON SITE ONLY !! Comment it for production: --%>
        <%--<asp:HyperLink ID="HyperLinkTestHelp" runat="server" NavigateUrl="~/HelpDesk.aspx" visible="False" CssClass="NodeStyle" Font-Names="Arial">Test to report a problem </asp:HyperLink>--%>  
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx" CssClass="NodeStyle" Font-Names="Arial">Log off</asp:HyperLink>            
        
    <table border="0" cellpadding="1" cellspacing="0" width="100%">
     <%--<tr id="trDB" runat ="server" visible ="false">
       <td align="center" valign="top">
           <br />
           <asp:Label ID="LabelDB" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Smaller" ForeColor="Gray" ></asp:Label>
           <br />
           <asp:DropDownList ID="DropDownListConnStr" runat="server" AutoPostBack="True" Font-Names="Arial"> </asp:DropDownList>
       </td>
      </tr>--%>

      <tr id="trMessage" runat ="server" visible ="false" >
       <td align="center" valign="top">
         <asp:Label ID="LabelMessage" runat="server" Font-Size="Larger" ForeColor="Red" Font-Names="Arial"></asp:Label>
       </td>
      </tr>

     <tr>
       <td align="center" valign="top">
        <table border="0" cellpadding="0" cellspacing="5" width="50%">
            
        </table>
       </td>
     <tr>
       <td align="center" valign="top">
         
         <asp:Label ID="lblHeader" runat="server"  Font-Size="22px" Font-Names="Arial" >Analytics:</asp:Label>

       </td>
      </tr>
      
        <tr>
            <td align="center">
             <table id="Table2" runat="server" bgcolor="#e5e5e5" rules="rows" width="100%" style="border:medium double #FFFFFF; font-size: small;
                                        color: black; font-family: Arial; background-color: #e5e5e5; vertical-align: top;">
                                   <tr valign="top" runat ="server" colspan="3" border="3" style="color: black; font-family: Arial; border:medium double #FFFFFF; background-color: #e5e5e5;border-width: 2px;width:100%;" >
   
                                       <td align="right" colspan="3">
     
                                         &nbsp;&nbsp;<asp:Label ID="Label7" runat="server" Text=" Correlation: "  ToolTip="Correlation between field1(argument) and field2 if both are numeric" Font-Bold="True"></asp:Label>
     
                                       </td>
                                  </tr>
                                  <tr valign="top" runat ="server"  border="3" style="color: black; font-family: Arial; border:medium double #FFFFFF; background-color: #e5e5e5;border-width: 2px;width:100%;" >
                                    <td align="left" style="border:medium double #FFFFFF;" width="1%">
  
                                       
                                    </td>

                                    <td align="left" valign="top" style="border:medium double #FFFFFF; text-align: center;" width="74%">
                                        
                                        &nbsp;<asp:Label ID="Label11" runat="server" Text="(!!! ATTENTION !!!)    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;   SELECT: " ToolTip="Required (!) to calculate reports, statistics, analytics, and charts below." ForeColor="Red" Font-Bold="True" Font-Size="Medium"></asp:Label>
                                       <br /><br />
                                        &nbsp;<asp:Label ID="Label3" runat="server" Text="The argument: " ToolTip="Required (!) to calculate reports, statistics, analytics, and charts below (Axis Y in Charts)."  Font-Bold="True" Font-Size="Medium"></asp:Label>
                                        <asp:DropDownList ID="DropDownList3" runat="server" ToolTip="Axis Y" AutoPostBack="True"></asp:DropDownList> 
                                        
                                        <asp:Label ID="Label4" runat="server" Text="and the aggregation function "  ToolTip="Required! Function to aggregate the values of the argument field. For text field: Count and Count Distinct, for numeric field Sum, Max, Min, StDev, etc... as well. "   Font-Bold="True" Font-Size="Medium"></asp:Label>
                                        <asp:DropDownList ID="DropDownList4" runat="server"  ToolTip="Numeric or Text aggregation function"></asp:DropDownList>
                                        <br />
                                        <br />
                                        
                                        &nbsp;<asp:Label ID="Label12" runat="server" Text="Category/Group 1: " ToolTip="Required (!) to calculate reports, statistics, analytics, and charts below (Axis X in Charts)."   Font-Bold="True" Font-Size="Medium"></asp:Label>
                                        <asp:DropDownList ID="DropDownList2" runat="server" ToolTip="Category/Group1" AutoPostBack="True"></asp:DropDownList> 
                                        <%-- <br />--%>
                                         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                         &nbsp;<asp:Label ID="Label13" runat="server" Text="Category/Group 2: " ToolTip="Required (!) to calculate reports, statistics, analytics, and charts below (Axis X in Charts)."   Font-Bold="True" Font-Size="Medium"></asp:Label>
                                         <asp:DropDownList ID="DropDownList7" runat="server" ToolTip="Category/Group2" AutoPostBack="True"></asp:DropDownList>
                                    </td>
                                    
                                    <td align="left" style="border:medium double #FFFFFF;" width="25%">
                                      
                                       <%-- <br />--%>
                                        &nbsp;&nbsp;<asp:Label ID="Label8" runat="server" Text="Optional, used only for charts and matrix balancing if needed: " ToolTip="Optional - it needs only for checking correlation between fields and for matrix balancing."></asp:Label>
                                        <br /><%--<br />--%>
                                        &nbsp;&nbsp;<asp:Label ID="Label5" runat="server" Text="Select: the field2 " ToolTip="Optional - it needs only for checking correlation between fields and for matrix balancing."></asp:Label>
                                        <asp:DropDownList ID="DropDownList5" runat="server" ToolTip="For Matrix balancing or for checking correlation between field1 and field2" AutoPostBack="True"></asp:DropDownList>  
                                        <br />
                                        &nbsp;&nbsp;<asp:Label ID="Label6" runat="server" Text="and the aggregation function "  ToolTip="aggregation function to aggrigate the values of argument field2"></asp:Label>
                                        <asp:DropDownList ID="DropDownList6" runat="server"  ToolTip="Numeric or Text aggrigate functions"></asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                       
                                        <br />&nbsp; 
                                        <asp:Label ID="Label14" runat="server" Text="=================================================" ForeColor="White"></asp:Label>
                                        <br />
                                         &nbsp;&nbsp;<asp:Label ID="Label9" runat="server" Text="Optional, used to add Category/Group to Analytics: " ToolTip="Optional, used to add manually the Category/Group to Analytics if not included automatically"></asp:Label>
                                         <br /><%--<br />--%>
                                         &nbsp;&nbsp;<asp:Label ID="Label10" runat="server" Text="Select the Category/Group: " ToolTip="Optional, used to add manually the Category/Group to Analytics if not included automatically"></asp:Label>
                                         <asp:DropDownList ID="DropDownList1" runat="server" ToolTip="List of fields to add as Category/Group to Analytics" AutoPostBack="True"></asp:DropDownList>  
                                        <%-- <br /><br />--%>
                                         &nbsp;<asp:LinkButton ID="LinkButtonAddCategory" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Add custom Category/Group to Analytics">add</asp:LinkButton> 
                                         <%--<br />&nbsp;--%>
                                    </td>
                                  </tr>
                                  <tr valign="top" runat ="server" colspan="3" border="3" style="color: black; font-family: Arial; border:medium double #FFFFFF; background-color: lightgreen;border-width: 2px;width:100%;" >
   
                                     <td align="left" colspan="3">
     
                                       <%--<asp:Label ID="Label15" runat="server" Text=" Reports: "  ToolTip="Click to see reports" Font-Bold="True"></asp:Label>--%>
                                       <table runat="server" id="listshort"  border="0" style="font-size: 12px; font-family: Arial; height: 12px" >
                                               <tr  height="12px" width="80%" >
                                                   <%--<td class="auto-style3"  style="font-weight:bold">Reports:</td>                                                   
                                                   <td class="auto-style1" style="font-weight:bold"> </td>
                                                   <td class="auto-style1" style="font-weight:bold"> </td>                    
                                                   <td class="auto-style1" style="font-weight:bold"> </td>
                                                   <td class="auto-style1" style="font-weight:bold"> </td>                    
                                                   <td class="auto-style1" style="font-weight:bold"> </td>
                                                   <td class="auto-style1" style="font-weight:bold"> </td>
                                                   <td class="auto-style1" style="font-weight:bold"> </td>--%>
                                                    <td width="100px" style="font-weight:bold">Reports:</td>                                                   
                                                    <td width="50px" style="font-weight:bold"> </td>
                                                    <td width="30px" style="font-weight:bold"> </td>                    
                                                    <td width="30px" style="font-weight:bold"> </td>
                                                    <td width="30px" style="font-weight:bold"> </td>                    
                                                    <td width="80px" style="font-weight:bold"> </td>
                                                    <td width="100px" style="font-weight:bold"> </td>
                                                    <td width="25px" style="font-weight:bold"> </td>
                                                   
                                               </tr>           
                                        </table> 
                                     </td>
                                  </tr>

            </table>
          </td>      
             
        </tr>
        <tr>
            <td align="left" style="font-weight: normal; color: #ffffff; font-family: Arial; background-color: LightGray; font-size:small;" class="auto-style1">
                        <asp:Label ID="Label2" runat="server" ForeColor="Black" Text="Search:"></asp:Label>
                        <%-- &nbsp;--%>
                        <asp:TextBox ID="txtSearch" runat="server" Visible="true" width="200px"></asp:TextBox>
                        <asp:Button ID="ButtonSearch" runat="server" CssClass="ticketbutton" Text="Search" Visible="true" valign="center"/>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Label ID="lblRecordsCount" runat="server" Text=" " ForeColor="Black"></asp:Label>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
                        
                        <asp:LinkButton ID="lnkGridAI" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Ask AI to analyze the data. May take a long time..." Font-Bold="True" Visible="True" Font-Size="Medium">AI</asp:LinkButton>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:LinkButton ID="LinkButtonRefresh" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="May take a long time...">Recalculate Analytics</asp:LinkButton> 
     
            </td>
        </tr>
        <tr>
            <td align="center" width="80%" >
             <table runat="server" id="list"  border="0" style="font-size: 12px; font-family: Arial">
                <tr>
                    <td class="auto-style3"  style="font-weight:bold">Category/Group 1</td>
                    <td class="auto-style3"  style="font-weight:bold">Category/Group 2</td>
                    <td class="auto-style1" style="font-weight:bold">Matrix/Pivot</td>
                    <td class="auto-style1" style="font-weight:bold">Bar Chart</td>                    
                    <td class="auto-style1" style="font-weight:bold">Pie Chart</td>
                    <td class="auto-style1" style="font-weight:bold">Line Chart</td>                    
                    <td class="auto-style1" style="font-weight:bold">Data records</td>
                    <td class="auto-style1" style="font-weight:bold">Dashboard</td>
                    <td class="auto-style1" style="font-weight:bold">Charts</td>
                    <td class="auto-style1" style="font-weight:bold">Matrix Balancing</td>   
                </tr>           
             </table> 
            </td>
        </tr>
    </table>       
            
        <br />
         <div align="left" style="background-color: lightgray; font-family: Arial, Helvetica, sans-serif; font-size: medium; font-weight: bold; color: #FFFFFF;">
                     &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                 </div>
        
        <asp:Label ID="Label1" runat="server" Font-Size="Larger" ForeColor="Red"> </asp:Label>
        <br />
        <br />
        <br />
        <br />
        <br />
        </div>

 </div>
                  </td>
        </tr>
    </table>


        <ucMsgBox:Msgbox id="MessageBox" runat ="server" > </ucMsgBox:Msgbox>
        <ucDlgTextbox:DlgTextbox id="dlgTextbox" runat="server" />
      </ContentTemplate>
      </asp:UpdatePanel>   
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpTablesList">
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

