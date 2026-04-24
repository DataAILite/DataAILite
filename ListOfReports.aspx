<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ListOfReports.aspx.vb" Inherits="ListOfReports" %>Param3type

<script type="text/javascript" src="Controls/Javascripts/OUR.js"></script>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>List Of Reports</title>
    <style type="text/css">
        .auto-style1 {
            /*width: 107px;*/
            width:6%;
            height: 26px;
        }
        .auto-style2 {
            /*width: 300px;*/
             width:30%;
            height: 26px;
        }
        .auto-style3 {
            /*width: 247px;*/
             width: 7%;
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
    <asp:UpdatePanel ID="udpReportList" runat ="server" >
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
            <asp:TreeNode Text="&lt;b&gt;Log off&lt;/b&gt;"  Value="~/Default.aspx" Expanded="True" >
                 
            </asp:TreeNode>            
           
            <asp:TreeNode Text="&lt;b&gt;Documentation&lt;/b&gt;"  Value="DataAIHelp.aspx" Expanded="False" Target="_blank">
                <asp:TreeNode Text="Reports Demo"  Value="https://oureports.net/OUReports/Default.aspx?logon=demo&pass=demo" Expanded="True" Target="_blank"> </asp:TreeNode>                 
                 <asp:TreeNode Text="General documentation " Value="Documentation" NavigateUrl="https://oureports.net/OUReports/OnlineUserReporting.pdf" Target="_blank"></asp:TreeNode>
                <asp:TreeNode Text="Advanced Report Designer " Value="AdvancedReportDesigner " NavigateUrl="https://oureports.net/OUReports/AdvancedReportDesigner.pdf#page=4" Target="_blank"></asp:TreeNode>
                <asp:TreeNode Text="Video: Advanced Report Designer - Tabular Reports " Value="VideoAdvancedReportDesigner" NavigateUrl="https://oureports.net/OUReports/Videos/AdvancedReportDesigner Tabular.mp4" Target="_blank"></asp:TreeNode>
                <asp:TreeNode Text="Video: Advanced Report Designer - HeaderFooter " Value="VideoAdvancedReportDesignerHeaderFooter" NavigateUrl="https://oureports.net/OUReports/Videos/AdvancedReportDesigner-HeaderFooter.mp4" Target="_blank"></asp:TreeNode>
                 <asp:TreeNode Text="Video: Advanced Report Designer - Free Form " Value="VideoAdvancedReportDesignerFreeForm" NavigateUrl="https://oureports.net/OUReports/Videos/AdvancedReportDesigner-FreeForm.mp4" Target="_blank"></asp:TreeNode>

                <asp:TreeNode Text="Charts and Dashboards " Value="ChartsDashboards" NavigateUrl="https://oureports.net/OUReports/GoogleChartsAndDashboards.pdf" Target="_blank"></asp:TreeNode>
             <asp:TreeNode Text="Video: DataAI - Data Analytics and Instant Reporting " Value="VideoDI" NavigateUrl="https://oureports.net/OUReports/Videos/DataImport.mp4" Target="_blank"></asp:TreeNode>
                <asp:TreeNode Text="Video: Charts, Maps, and Dashboards " Value="Video1" NavigateUrl="https://oureports.net/OUReports/Videos/zoom_2.mp4" Target="_blank"></asp:TreeNode>
             <asp:TreeNode Text="Video: Quick Start (only email needed) " Value="VideoQuickStart" NavigateUrl="https://oureports.net/OUReports/Videos/QuickStart.mp4" Target="_blank"></asp:TreeNode>
             <asp:TreeNode Text="Video: Individual Registration, user database " Value="Video1" NavigateUrl="https://oureports.net/OUReports/Videos/UserRegistrationVideo.mp4" Target="_blank"></asp:TreeNode>
             <asp:TreeNode Text="Video: Individual Registration, use our database " Value="Video1" NavigateUrl="https://oureports.net/OUReports/Videos/RegOurDb.mp4" Target="_blank"></asp:TreeNode>
             <asp:TreeNode Text="Video: Company Registration " Value="Video1" NavigateUrl="https://oureports.net/OUReports/Videos/UnitRegistrationVideo.mp4" Target="_blank"></asp:TreeNode>
             <asp:TreeNode Text="Video: Input from Access " Value="VideoQuickStart" NavigateUrl="https://oureports.net/OUReports/Videos/InputFromAccess.mp4" Target="_blank"></asp:TreeNode>
             <asp:TreeNode Text="Video: Matrix Balancing " Value="Video" NavigateUrl="https://oureports.net/OUReports/Videos/MatrixBalance.mp4" Target="_blank"></asp:TreeNode>
             <asp:TreeNode Text="Dashboards documentation" Value="HealthCare" NavigateUrl="https://oureports.net/OUReports/DashboardHelp.pdf" Target="_blank"></asp:TreeNode>
                
                <asp:TreeNode Text="Sample: Covid 2020 Dashboard" Value="Covid2020" NavigateUrl="https://oureports.net/OUReports/default.aspx?srd=30&dash=yes&lgn=d720202024346P906" Target="_blank"></asp:TreeNode>
                 <asp:TreeNode Text="Sample: Public data" Value="Public" NavigateUrl="https://oureports.net/OUReports/UseCasePublic.aspx" Target="_blank"></asp:TreeNode>
                 <asp:TreeNode Text="Explore data" Value="Explore" NavigateUrl="https://oureports.net/OUReports/ExploreData.pdf" Target="_blank"></asp:TreeNode>
                 <asp:TreeNode Text="Matrix Balancing" Value="Matrix" NavigateUrl="https://oureports.net/OUReports/MatrixBalancing.pdf#page=2" Target="_blank"></asp:TreeNode>
                 <asp:TreeNode Text="More Matrix Balancing Samples" Value="Matrix" NavigateUrl="https://oureports.net/OUReports/MatrixBalancingSamples.pdf" Target="_blank"></asp:TreeNode>
                <asp:TreeNode Text="Video: Matrix Balanceing Scenarios 1a and 1b" Value="Matrix" NavigateUrl="https://oureports.net/OUReports/Videos/MatrixBalance1a1b.mp4" Target="_blank"></asp:TreeNode>
                <asp:TreeNode Text="Video: Matrix Balanceing Scenarios 2a and 3a" Value="Matrix" NavigateUrl="https://oureports.net/OUReports/Videos/MatrixBalance2a3a.mp4" Target="_blank"></asp:TreeNode>
                <asp:TreeNode Text="Video: Matrix Balanceing Scenarios 2b and 2c" Value="Matrix" NavigateUrl="https://oureports.net/OUReports/Videos/MatrixBalance2b2c.mp4" Target="_blank"></asp:TreeNode>
                <asp:TreeNode Text="Video: Matrix Balanceing Scenarios 3b and 3c" Value="Matrix" NavigateUrl="https://oureports.net/OUReports/Videos/MatrixBalance3b3c.mp4" Target="_blank"></asp:TreeNode>
                <asp:TreeNode Text="Video: Matrix Balanceing Scenarios 4a, 4b, and 4c" Value="Matrix" NavigateUrl="https://oureports.net/OUReports/Videos/MatrixBalance4a4b4c.mp4" Target="_blank"></asp:TreeNode>
                <asp:TreeNode Text="Making Google Maps and Earth documentation " Value="Documentation" NavigateUrl="https://oureports.net/OUReports/MapDefinitionDocumentation.pdf" Target="_blank"></asp:TreeNode>
               <%-- <asp:TreeNode Text="KML generator Demo"  Value="Default.aspx?logon=csvdemo&pass=demo" Expanded="True" > </asp:TreeNode>     
               --%> <%--<asp:TreeNode Text="KML generator Help"  Value="MapDefinitionDocumentation.pdf" Expanded="True" ></asp:TreeNode>--%>
               <asp:TreeNode Text="Task List documentation " Value="TaskListDocumentation" NavigateUrl="https://oureports.net/OUReports/Tasklist.pdf" Target="_blank"></asp:TreeNode>
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
          <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/SiteAdmin.aspx" Enabled="False" Visible="False" CssClass="NodeStyle" Font-Names="Arial" Target="_blank">Site Admininistration</asp:HyperLink>
           &nbsp;&nbsp;
            <asp:HyperLink ID="HyperLinkTaskList" runat="server" NavigateUrl="~/TaskList.aspx" Enabled="False" Visible="False" CssClass="NodeStyle" Font-Names="Arial" Target="_blank">Task List</asp:HyperLink> 
        <%--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
        &nbsp;&nbsp;&nbsp;
        <%--<asp:LinkButton ID="btnListOfTables" runat="server" Text="Tables" CssClass="NodeStyle" Visible="False" Enabled="False" Font-Names="Arial"></asp:LinkButton>
        &nbsp;&nbsp;&nbsp;--%>
        <asp:LinkButton ID="btnListOfClasses" runat="server" Text="Tables/Classes" CssClass="NodeStyle" Visible="False" Enabled="False" Font-Names="Arial" ToolTip="Selecting table/class from dropdown will present the list of reports where it used. It has links to list of Joins and to the page to manage tables. "></asp:LinkButton>
        <%--&nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="btnListOfJoins" runat="server" Text="Joins" CssClass="NodeStyle" Visible="False" Enabled="False" Font-Names="Arial"></asp:LinkButton>
       --%>
        &nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLinkListOfDashboards" runat="server" NavigateUrl="~/ListOfDashboards.aspx" CssClass="NodeStyle" Font-Names="Arial" Target="_blank">Dashboards</asp:HyperLink>
          
        &nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLinkScheduledReports" runat="server" NavigateUrl="~/ScheduledReports.aspx" CssClass="NodeStyle" Font-Names="Arial">Scheduled Reports</asp:HyperLink>
       
          &nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLinkScheduledDownloads" runat="server" NavigateUrl="~/ScheduledDownloads.aspx" CssClass="NodeStyle" Font-Names="Arial" Target="_blank">Scheduled Downloads</asp:HyperLink>
      
           &nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLinkScheduledImports" runat="server" NavigateUrl="~/ScheduledImports.aspx" CssClass="NodeStyle" Font-Names="Arial" Target="_blank">Scheduled Imports</asp:HyperLink>
      
           &nbsp;&nbsp;&nbsp;
       <asp:LinkButton ID="btnFriendlyNames" runat="server" Text="Friendly Names" CssClass="NodeStyle" Font-Names="Arial" ></asp:LinkButton>
               <%-- &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp; 
        <asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx?hilt=List_of_Reports" Target="_blank" CssClass="NodeStyle" Font-Names="Arial">Help</asp:HyperLink>&nbsp;&nbsp;
                 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="LinkButtonHelpDesk" runat="server" CssClass="NodeStyle" Font-Names="Arial">Report a problem</asp:LinkButton> 
         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
       <%-- !! DO NOT DELETE, NEXT LINE IS FOR TESTING ON SITE ONLY !! Comment it for production: --%>
        <asp:HyperLink ID="HyperLinkTestHelp" runat="server" NavigateUrl="HelpDesk.aspx" visible="False" CssClass="NodeStyle" Font-Names="Arial">Test to report a problem </asp:HyperLink>  
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx" CssClass="NodeStyle" Font-Names="Arial">Log off</asp:HyperLink>            
        
    <table border="0" cellpadding="1" cellspacing="0" width="100%">
     <tr id="trDB" runat ="server" visible ="false">
         <td align="center" valign="top" width="100%">
             <table>
                 <tr>
                  <td align="center" valign="top" >
                       <br />
                       <asp:Label ID="LabelDB" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Smaller" ForeColor="Gray" ></asp:Label>
                       <br />
                       <asp:DropDownList ID="DropDownListConnStr" runat="server" AutoPostBack="True" Font-Names="Arial"> </asp:DropDownList>
                   </td>
                   <td align="left" valign="top" width="200px">
                       <br />
                       <asp:Label ID="LabelDBUserID" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Smaller" ForeColor="Gray" Visible="True" Text="DB User ID:"></asp:Label>
                       <br />
                       <asp:TextBox ID="txtDBUserID" runat="server" AutoPostBack="False" Font-Names="Arial" Visible="True" Enabled="True" width="200px" Text=" " ToolTip="Enter the User ID for the Database"> </asp:TextBox>
                   
                       
                   </td>
                   <td align="left" valign="top" width="400px">
                       <br />
                       <asp:Label ID="LabelDBUserPass" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Smaller" ForeColor="Gray" Visible="True" Text="DB Password:"></asp:Label>
                       <br />
                       <asp:TextBox ID="txtDBUserPass" runat="server" AutoPostBack="False" Font-Names="Arial" Visible="True" Enabled="True" Text=" " TextMode="Password" ToolTip="Enter the password for the User of the Database"> </asp:TextBox>
                       &nbsp;
                       <asp:Button runat="server" ID="btnShowList" Visible="true"  Text="show list" />
                   </td>
                 </tr>
             </table>
         </td>     

      </tr>

      <tr id="trMessage" runat ="server" visible ="false" >
       <td align="center" valign="top">
         <asp:Label ID="LabelMessage" runat="server" Font-Size="Larger" ForeColor="Red" Font-Names="Arial"></asp:Label>
       </td>
      </tr>

     <tr>
       <td align="center" valign="top">
        <table border="0" cellpadding="0" cellspacing="5" width="50%">
            <tr id ="trPay1" runat="server" visible="false">
                <td align="right    " >
                 <asp:LinkButton ID="HyperLinkPay1" runat="server" Font-Size="Larger" Postbackurl="~/Pay.aspx" CssClass="NodeStyle" Font-Names="Arial">Pay</asp:LinkButton> 
                </td>
                <td align ="right" >
                 <%--<asp:LinkButton ID="btnCreate" runat="server" Font-Size="Larger" CssClass="NodeStyle" Font-Names="Arial" >New Report</asp:LinkButton> --%>
                </td>
            </tr>
            <tr id ="trPay2" runat="server" visible ="false">
                <td colspan="2" align="center" >
                 <asp:LinkButton ID="HyperLinkPay2" runat="server" Font-Size="Larger" Postbackurl="~/Pay.aspx" CssClass="NodeStyle" Font-Names="Arial">Pay</asp:LinkButton> 
                </td>
             </tr>
            <tr id ="trNewRepHelp" runat="server" visible ="false   ">
                <td >
                  <br />
                  <asp:Label ID="LabelCreateReport" runat="server" Text="To create a new report first enter the new Report Title and click the button Create or click 'copy' from the list of existing reports." CssClass="auto-style4"></asp:Label>
                </td>               
            </tr>
           <tr id ="trReportTitle" runat="server" visible ="false">
                <td align="right" >
                   <asp:Label ID="lblTitle" runat="server" Text="Report Title:"></asp:Label>&nbsp;&nbsp;
                </td>
                <td align="left"  >
                  <asp:TextBox ID="TextBoxNewReportTtl" runat="server" 
                      AutoPostBack="True" Width="441px"></asp:TextBox>
                </td>
            </tr>
           <tr id ="trCreateButton" runat="server" visible ="false">
                <td colspan="2" align="center" >
                     <asp:Button ID="ButtonCreateReport" runat="server" CssClass="ticketbutton" Text="Create" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnCancelCreate" runat="server" CssClass="ticketbutton" Text="Cancel" />
                </td>
             </tr>
        </table>
       </td>
     <tr>
       <td align="left" valign="top">
        
         <asp:Label ID="lblHeader" runat="server" Font-Bold="True" Font-Size="22px" Font-Names="Arial" >Reports:</asp:Label>
           
         <%--  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
         <asp:LinkButton ID="btnCreate" runat="server" Font-Size="Medium" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Create new report by query existing tables" Font-Italic="True" Font-Underline="True">Create new report</asp:LinkButton> 
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;     
         <asp:LinkButton ID="btnDataImport" runat="server" Font-Size="Medium" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Import data into new or existing table in user database from the local file" Font-Italic="True" Font-Underline="True">Import data</asp:LinkButton> 
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
        
         <asp:CheckBox ID="chkAdvanced" runat="server" Text=" Advanced User"  AutoPostBack="True"  Font-Size="12px" />
       </td>
      </tr>
      
        <tr>
            <td align="left" style="font-weight: normal; color: #ffffff; font-family: Arial; background-color: lightgray; font-size:small;" class="auto-style1">
                        <asp:Label ID="Label2" runat="server" ForeColor="Black" Text="Search:"></asp:Label>
                         &nbsp;&nbsp
                        <asp:TextBox ID="txtSearch" runat="server" Visible="true" width="200px"></asp:TextBox>
                        <asp:Button ID="ButtonSearch" runat="server" CssClass="ticketbutton" Text="Search" Visible="true"/>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp
                        <asp:Label ID="lblReportsCount" runat="server" Text=" " ForeColor="Black"></asp:Label>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp
                        <asp:CheckBox ID="chkInitialReports" runat="server" Text="show generic reports" ForeColor="White" AutoPostBack="True" Checked="False" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:LinkButton ID="lnkRedoInitialReports" runat="server">Redo Initial Reports (it might take a long time)</asp:LinkButton>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:LinkButton ID="lnkHideBadReports" runat="server">Hide corrupted reports</asp:LinkButton>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:LinkButton ID="lnkUnHideBadReports" runat="server">Show corrupted reports as well</asp:LinkButton>
                    </td>
        </tr>
        <tr>
            <td align="center" width="80%" >
        <table runat="server" id="list"  border=0 style="font-size: 12px; font-family: Arial">
                <tr>
                    <td class="auto-style3"  style="font-weight:bold"></td>
                    <td class="auto-style3" style="font-weight:bold">Show Report:</td>
                    <td class="auto-style2" style="font-weight:bold"></td>
                    <td class="auto-style1" style="font-weight:bold"></td>
                    <td class="auto-style1" style="font-weight:bold"></td>
                    <td class="auto-style1" style="font-weight:bold"></td>
                    <td class="auto-style1" style="font-weight:bold"></td>
                    <td class="auto-style1" style="font-weight:bold"></td>
                    <td class="auto-style1" style="font-weight:bold"></td>
                    <td class="auto-style1" style="font-weight:bold"></td>
                    <td class="auto-style1" style="font-weight:bold"></td>
                    <td class="auto-style1" style="font-weight:bold"></td>
                </tr>           
        </table>
       
    
    
            </td>
        </tr>
    </table>       
            
        <br />
         <div align="left" backcolor="LightGray"  style="background-color: lightgray; font-family: Arial, Helvetica, sans-serif; font-size: medium; font-weight: bold; color: #FFFFFF;">
                     &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                 </div>
        <%--<table runat="server">
            <tr >
            <td align="left" style="font-weight: bold; color: #ffffff; font-family: Arial; background-color: Gray; font-size:small;" width="100%">
                     ...   
                    </td>
            </tr>
        </table>
        <br />
        <br /><asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/Default.aspx">Log off</asp:HyperLink>--%>
        
        <asp:Label ID="Label1" runat="server" Font-Size="Larger" ForeColor="Red"> </asp:Label>
        <br />
        <br />
        <%--<br />
        <br />
        <br />--%>
        </div>

 </div>
                  </td>
        </tr>
    </table>


        <ucMsgBox:Msgbox id="MessageBox" runat ="server" > </ucMsgBox:Msgbox>
        <ucDlgTextbox:DlgTextbox id="dlgTextbox" runat="server" />
      </ContentTemplate>
      </asp:UpdatePanel>   
      <div id="spinner" class="modal" style="display:none;">
            <div id="divSpinner" style="text-align: center; width: 130px; display: inline-block; clip: rect(auto, auto, auto, 50%); position: fixed; margin-top: 300px; margin-right: auto; padding-top: 10px; background-color: #f8f8d3; z-index: 2147483647; left: 50%; top: -5%;">
                <img id="imgSpinner" src="Controls/Images/WaitImage2.gif" style="width: 100px; height: 100px" />
                <br />
                      Please Wait...
            </div>
        </div>  

        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpReportList">
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
