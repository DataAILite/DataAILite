<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Correlation.aspx.vb" Inherits="Correlation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Correlation</title>
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
           <asp:LinkButton ID="LinkButtonRefresh" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="May take a long time...">Recalculate Correlations</asp:LinkButton> 
     
        &nbsp;&nbsp;<%--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
        <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/FriendlyNames.aspx" CssClass="NodeStyle" Font-Names="Arial" Enabled="False" Visible="False">FriendlyNames</asp:HyperLink>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        
        &nbsp;&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; 
        <asp:HyperLink ID="HyperLinkDataAI" runat="server" NavigateUrl="~/DataAI.aspx?pg=cor" CssClass="NodeStyle" Font-Names="Arial" Font-Size="12px" Visible="True" ToolTip="DataAI analytics" Font-Bold="False">DataAI</asp:HyperLink>
    
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:LinkButton  ID="lnkDataAI" runat="server" Font-Size="Small" Visible="False">DataAI</asp:LinkButton>       
        
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp; 
        <asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx?hilt=Fields_Correlation" Target="_blank" CssClass="NodeStyle" Font-Names="Arial">Help</asp:HyperLink>&nbsp;&nbsp;
                 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
       <%-- <asp:LinkButton ID="LinkButtonHelpDesk" runat="server" OnClientClick="target='_blank'" CssClass="NodeStyle" Font-Names="Arial">Report a problem</asp:LinkButton> 
         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
       <%-- !! DO NOT DELETE, NEXT LINE IS FOR TESTING ON SITE ONLY !! Comment it for production: --%>
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
            <td align="left" style="font-weight: normal; color: #ffffff; font-family: Arial; background-color: LightGray; font-size:small;" class="auto-style1">
                        <asp:Label ID="Label2" runat="server" ForeColor="Black" Text="Search:"></asp:Label>
                         &nbsp;&nbsp
                        <asp:TextBox ID="txtSearch" runat="server" Visible="true" width="200px"></asp:TextBox>
                        <asp:Button ID="ButtonSearch" runat="server" CssClass="ticketbutton" Text="Search" Visible="true" valign="center"/>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp
                        <asp:CheckBox ID="chkCorrelate" runat="server" Text="show all" ForeColor="Black" AutoPostBack="True" Checked="False" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp
                        <asp:Label ID="lblRecordsCount" runat="server" Text=" " ForeColor="Black"></asp:Label>

                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp
                        &nbsp;&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lnkExport" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Export to CSV file. May take a long time...">Export to Excel</asp:LinkButton></td> 

             </td>
        </tr>
        <tr>
            <td align="center" width="80%" >
             <table runat="server" id="list"  border=0 style="font-size: 12px; font-family: Arial">
                <tr>
                    <td class="auto-style3"  style="font-weight:bold">Field 1</td>
                    <td class="auto-style3"  style="font-weight:bold">Field 2</td>
                    <td class="auto-style1" style="font-weight:bold">Correlation Coefficient</td>
                    <td class="auto-style1" style="font-weight:bold">RDL</td>
                    <td class="auto-style1" style="font-weight:bold">Charts</td>   
                    <td class="auto-style1" style="font-weight:bold">Dashboard</td> 
                    <%--<td class="auto-style1" style="font-weight:bold">Pie Chart</td>--%>
                    <%--<td class="auto-style1" style="font-weight:bold">Line Chart</td>      --%>              
                    <%--<td class="auto-style1" style="font-weight:bold">Data records</td>--%>
                    <%--<td class="auto-style2" style="font-weight:bold">Delete </td>   --%>
                </tr>           
             </table> 
            </td>
        </tr>
    </table>       
            
        <br />
         <div align="left" backcolor="Gray"  style="background-color: lightgray; font-family: Arial, Helvetica, sans-serif; font-size: medium; font-weight: bold; color: #FFFFFF;">
                     &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                 </div>
        
        <asp:Label ID="Label1" runat="server" Font-Size="Larger" ForeColor="Maroon"> </asp:Label>
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

