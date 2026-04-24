<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ReportViews.aspx.vb" Inherits="ReportViews" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<script type="text/javascript" src="Controls/Javascripts/OUR.js"></script>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Report</title>
<style type="text/css">
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
         <asp:HiddenField ID ="hdnReportDimensions" runat ="server" />
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <script type="text/javascript">
            var wasLoading = false;

            Sys.Application.add_load(function () {
                $find("viewer").add_propertyChanged(viewerPropertyChanged);
            });
            function viewerPropertyChanged(sender, e) {
                if (e.get_propertyName() === "isLoading") {
                    var viewer = $find("viewer");
                    if (viewer.get_isLoading()) {
                        wasLoading = true;
                    }
                    else if (wasLoading) {
                        wasLoading = false;
                        getReportDimensions('');
                        //viewer.remove_propertyChanged(viewerPropertyChanged);
                    }
                }
            }
        </script>

    <asp:UpdatePanel ID="udpViewReport" runat ="server">
       <ContentTemplate> 
           <div>               
    <table>
      <tr>
        <td colspan="3" style="font-size: x-large; font-style: normal; font-weight: bold; background-color: #e5e5e5; vertical-align: middle; height: 40px; text-align: left;">
            <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label>
          </td>
      </tr> 
      <tr>
            <td style="font-size: x-small; font-style: normal; font-weight: normal; background-color: #e5e5e5; vertical-align: top; text-align: left; width: 15%; ">
                    <div id="tree" style="font-size: x-small; font-weight: normal; font-style: normal">
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
             <%-- <asp:TreeNode Text="Show Report"  Value="ReportViews.aspx?see=yes&srd=3" Expanded="True" >--%>
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
            <td id="main" style="vertical-align: top; text-align: left; width: 85%"> 
                <table  id="maintable" runat="server" width="100%">
                    <tr ID="trReportTitle" runat="server" style="border-color:#ffffff" width="100%">
                        <td width="100%">
                          <table>
                            <tr>
                                <td bgcolor="white" width="75%" style="border-color:#ffffff; font-size: medium; color: Gray;" align="left" font-bold="True">
                                    <asp:Label ID ="lblReportFunction" runat ="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large" Text="Generic Report:" ForeColor="Gray"></asp:Label>
                                    <asp:Label ID="LabelReportTitle" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large" Text="REPORT" ForeColor="#000099"></asp:Label>
                                      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                      <%--<asp:HyperLink ID="HyperLinkChatAI" runat="server" NavigateUrl="~/ChatAI.aspx" Font-Size="Small" Target="_blank" ToolTip="Analyze resulting data with AI" Font-Bold="True" Visible="False">AI</asp:HyperLink>--%> 
                                      <asp:LinkButton  ID="lnkChatAI" runat="server" Font-Size="Medium" Visible="True" ToolTip="Analyze resulting data with AI" Font-Bold="True">AI</asp:LinkButton> 
                                      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;                                
 
                                    &nbsp; &nbsp; &nbsp; <asp:Label ID="LabelAddWhere" runat="server" Text=" " Font-Italic="True" ForeColor="Black" Font-Size="Small"><=></asp:Label>
                                    &nbsp; &nbsp; &nbsp;  &nbsp;  &nbsp;                                      
                                    <asp:LinkButton ID="ButtonReset" runat="server" Text="Reset" ToolTip="Remove restrictions and show original report" AutoPostBack="true" CssClass="NodeStyle" Font-Names="Arial" Font-Size="Small"/> 
                                      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
                                    <asp:HyperLink ID="HyperLinkSchedule" runat="server" NavigateUrl="~/ScheduleReportsCalendar.aspx" CssClass="NodeStyle" Font-Names="Arial" Font-Size="Small">Schedule report</asp:HyperLink>
                                   &nbsp; &nbsp; &nbsp; 
                                    <asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx?hilt=View_Reports" Target="_blank" CssClass="NodeStyle" Font-Names="Arial" Font-Size="Small">Help</asp:HyperLink>
                                    
                                    &nbsp; &nbsp; &nbsp; <asp:Label ID="LabelRunSched" runat="server" Text=" " Font-Italic="True" ForeColor="Black" Font-Size="Small" Visible="False"></asp:Label>
                                     &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:HyperLink ID="HyperLinkLogOff" runat="server" NavigateUrl="~/Default.aspx" CssClass="NodeStyle" Font-Names="Arial">Log off</asp:HyperLink>  
                                  
                                </td>
                            </tr>
                          </table>                              
                        </td>                    
                    </tr>
                     <tr ID="trParameters" runat="server" style="border-color:#ffffff" >
                        <td bgcolor="white" style="border-color:#ffffff; font-weight: bold; font-size: medium; color: Gray;" align="left" font-bold="True">
                         <%-- <asp:Button ID="ButtonShowReport" runat="server" Text="Show Report" />&nbsp;&nbsp;&nbsp; --%>
                            <%--<asp:TextBox ID="txtAddWhere" runat="server" Text="" Enabled="False"></asp:TextBox>--%>
                         </td>
                    </tr>
                    <tr ID="tr1" runat="server" style="border-color:#ffffff" >
                        <td bgcolor="white" style="border-color:#ffffff; font-weight: bold; font-size: medium; color: Gray;" align="left" font-bold="True">
                            <%--<asp:Button ID="ButtonShowReport" runat="server" Text="Show Report" />&nbsp;&nbsp;&nbsp;--%>
                            <asp:Label ID="Label5" runat="server" Text="Graphs: "></asp:Label>
                            <asp:Label ID="Label1" runat="server" Text="axis X" ToolTip="primary group - Axis X"></asp:Label>
                            <asp:DropDownList ID="DropDownList1" runat="server"  ToolTip="Axis X" AutoPostBack="False"></asp:DropDownList>
                            <asp:Label ID="Label2" runat="server" Text=" and "  ToolTip="secondary group - Axis X, optional"></asp:Label>
                            <asp:DropDownList ID="DropDownList2" runat="server"  ToolTip="Axis Y, optional" AutoPostBack="False"></asp:DropDownList>
                            <asp:Label ID="Label3" runat="server" Text=", axis Y" ToolTip="Axis Y"></asp:Label>
                            <asp:DropDownList ID="DropDownList3" runat="server" ToolTip="Axis Y" AutoPostBack="True"></asp:DropDownList>  
                            <asp:CheckBox ID="chkboxNumeric" runat="server" Checked="False" Text="numeric," Font-Names="Arial" Font-Size="X-Small"  AutoPostBack="True" Enabled="True" ToolTip="Axis Y field values are numeric. Do all statistics as for numeric field." />
                            <asp:Label ID="Label4" runat="server" Text=" aggregate "  ToolTip="aggegate function"></asp:Label>
                            <asp:DropDownList ID="DropDownList4" runat="server"  ToolTip="Numeric or Text aggrigate functions"></asp:DropDownList>&nbsp;                                                   
                            &nbsp; 
                            <asp:LinkButton ID="LinkButtonReverse" runat="server" CssClass="NodeStyle" Font-Names="Arial">reverse group order</asp:LinkButton>
                            
                         
                            
                            <br />
                            &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
                            <asp:Button ID="ButtonCharts" runat="server" CssClass="ticketbutton" Text="Google Charts ..." ToolTip="Show Google Charts" AutoPostBack="true" Width="100px"  OnClientClick="target='_blank'"/> 
                             <asp:Button ID="ButtonDashbords" runat="server" CssClass="ticketbutton" Text="Dashboard Statistics" ToolTip="Show Google Statistical Dashboard" AutoPostBack="true" Width="130px"  OnClientClick="target='_blank'"/> 
                            <%--&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;--%>
                            <%--&nbsp; &nbsp; &nbsp;  &nbsp; &nbsp;&nbsp;&nbsp; &nbsp; &nbsp; --%>&nbsp;&nbsp;&nbsp;&nbsp;
                            
                            <%--&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;--%>

                            <%--&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;&nbsp;--%>&nbsp;&nbsp;<asp:Label ID="Label6" runat="server" Text="SSRS reports:"  ToolTip="Advanced users can download RDL files"  CssClass="NodeStyle" Font-Names="Arial"></asp:Label>
                            <asp:Button ID="ButtonMatrix" runat="server" CssClass="ticketbutton" Text="Matrix" ToolTip="Matrix/Pivot table: group by Primary and Secondary fields the aggrigate value of field" Visible="True" />
                            <asp:Button ID="ButtonDynamicReport" runat="server" CssClass="ticketbutton" Text="DrillDown" ToolTip="Data by categories as groups and totals" Visible="True" />
                           
                            <asp:Button ID="ButtonShowGraph" runat="server" CssClass="ticketbutton" Text="Bar" ToolTip="RDL Bar Chart: Axis X - group by Primary and Secondary fields, Axis Y - aggrigate value of the field" />
                            <asp:Button ID="ButtonPie" runat="server" CssClass="ticketbutton" Text="Pie" ToolTip="RDL Pie Chart: Axis X - group by Primary and Secondary fields, Axis Y -  the aggrigate value of field" />
                            <asp:Button ID="ButtonLine" runat="server" CssClass="ticketbutton" Text="Line" ToolTip="RDL Line Chart: Axis X - group by Primary and Secondary fields, Axis Y -  the aggrigate value of field" />                        
                        </td>
                    </tr>
                    <tr>
                        <td bgcolor="Gray" align="left">&nbsp; 
                            <%-- <asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="~/OnlineUserReporting.pdf" Target="_blank" CssClass="NodeStyle" Font-Names="Arial" Font-Size="Small" Font-Italic="True" ForeColor="#33CCFF">Help</asp:HyperLink>
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>&nbsp;
                             <asp:CheckBox ID="CheckBoxHideDuplicates" runat="server" Checked="True" Text="hide duplicate records" Font-Names="Arial" Font-Size="Smaller" AutoPostBack="True" Enabled="False" ToolTip="It does not apply for big data..." />
                                &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; <%--&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;--%>
                        <asp:label runat="server" ID="LabelRowCount" Font-Bold="True" ForeColor="White" ToolTip="Row Count" Font-Size="Small"></asp:label> &nbsp;   
                             &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;<%-- &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;--%><asp:Label ID="LabelSearch" runat="server" Text="Search: " Font-Italic="True" ForeColor="White" ></asp:Label>        
                                    <asp:DropDownList ID="DropDownColumns" runat="server" Width="150px" AutoPostBack="false" ></asp:DropDownList>&nbsp; 
                                    <asp:DropDownList ID="DropDownOperator" runat="server"  ToolTip="Numeric or Text operators">
                                        <asp:ListItem></asp:ListItem>
                                        <asp:ListItem>=</asp:ListItem>
                                        <asp:ListItem>&lt;&gt;</asp:ListItem>
                                        <asp:ListItem>&lt;</asp:ListItem>
                                        <asp:ListItem>&lt;=</asp:ListItem>
                                        <asp:ListItem>&gt;</asp:ListItem>
                                        <asp:ListItem>&gt;=</asp:ListItem>
                                        <asp:ListItem>IN</asp:ListItem>
                                        <asp:ListItem>Not IN</asp:ListItem>
                                        <asp:ListItem>Contains</asp:ListItem>
                                        <asp:ListItem>Not Contains</asp:ListItem>
                                        <asp:ListItem>StartsWith</asp:ListItem>
                                        <asp:ListItem>Not StartsWith</asp:ListItem>
                                        <asp:ListItem>EndsWith</asp:ListItem>
                                        <asp:ListItem>Not EndsWith</asp:ListItem>
                                    </asp:DropDownList>&nbsp;
                                    <asp:TextBox ID="TextBoxSearch" runat="server" Width="100px"></asp:TextBox>
                                    <asp:Button ID="ButtonSearch" runat="server" CssClass="ticketbutton" Text="Search" ToolTip="Show data selected" AutoPostBack="true" Width="80px" /> 

                                       &nbsp; 
                                    <asp:Label ID="lblDesignerCreated" runat ="server" Font-Bold="True" ForeColor="Blue" Font-Size="Medium" Text="Not Created by Designer" Font-Names="Tahoma"> </asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td  bgcolor="#e5e5e5" align="left">

                             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
                            <asp:LinkButton  ID="lnkImage" runat="server" Font-Size="Small" Visible="True" ToolTip="Export current page of report to image png file">Export page to Image</asp:LinkButton> 
                                    
                             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:LinkButton  ID="lnkWord" runat="server" Font-Size="Small" Visible="True" ToolTip="Export whole report to Word document">Export report to Word</asp:LinkButton> 
                            
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:LinkButton  ID="lnkPDF" runat="server" Font-Size="Small" Visible="True" ToolTip="Export  whole report to PDF file">Export report to PDF</asp:LinkButton> 
                            
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:LinkButton  ID="lnkExpandAll" runat="server" Font-Size="Small" Visible="True" ToolTip="Expand all details in report">Expand all</asp:LinkButton>  

                        </td>
                    </tr>
                   
                    <tr>
                        <td>
                            <div style="text-align: center;width:100%">     
                            <rsweb:ReportViewer ID="viewer" ShowRefreshButton="false" PageCountMode="Actual" runat="server" Width="100%" SizeToReportContent="true" ShowPrintButton="true" ShowExportControls="true" ShowPageNavigationControls="true" align="left" ShowBackButton="False" ShowParameterPrompts="True" AsyncRendering="False" InteractivityPostBackMode="AlwaysSynchronous"></rsweb:ReportViewer>
                            </div>
                           
                        </td>
                    </tr>
                    <tr><td bgcolor="white" align="left">&nbsp; 
                        
                       &nbsp; &nbsp; &nbsp; &nbsp;<br /><br /> 
                        <asp:Label ID="LabelShare" runat="server" Text=" Send report link to email address:" ForeColor="black" Font-Size="Small" ToolTip="Up To Date Report will be available from the link in email"></asp:Label>&nbsp;
                        <asp:TextBox ID="txtShareEmail" runat="server" ToolTip="Enter email address"></asp:TextBox>&nbsp;<asp:Button ID="btnShare" runat="server" CssClass="ticketbutton" Text="Share" Font-Size="X-Small" />
                        <br />
                         <%-- &nbsp; &nbsp;&nbsp;--%> 
                            <asp:LinkButton ID="LinkButtonDownRDL" runat="server" Text="Download Report Definition file"  CssClass="NodeStyle" Font-Names="Arial"></asp:LinkButton> 
                              
                            <asp:HyperLink ID="HyperLinkRDL" runat="server" NavigateUrl="~/RDLFILES/" Target="_blank" CssClass="NodeStyle" Font-Names="Arial">See Report Definition (RDL)</asp:HyperLink>&nbsp;&nbsp;
                       
                        <br />
                        <asp:Label ID="LabelSQL" runat="server" Text=" " ForeColor="Black" Font-Size="X-Small"></asp:Label>  
                        <br /><br />
                        <asp:Label ID="LabelError" runat="server" Font-Bold="True"></asp:Label>
                        <br /><br />
                        <%--&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; --%>
                        <asp:Label ID="LabelTicket" runat="server" Text=" Create a new ticket in HelpDesk with the report attached or add a comment to the existing ticket:" ForeColor="black" Font-Size="Small"></asp:Label>&nbsp;
                        <asp:DropDownList ID="DropDownTickets" runat="server"  ToolTip="Tickets in HelpDesk" AutoPostBack="False"></asp:DropDownList>&nbsp;<asp:Button ID="ButtonTicket" runat="server" CssClass="ticketbutton" Text="Open a Ticket" Font-Size="X-Small" />
                        
                     </td</tr>
                    
                </table>
       
                  </td>
        </tr>
        <tr><td>
            <asp:Label ID="LabelReportID" runat="server" Font-Bold="False" Font-Names="Arial" Font-Size="XX-Small" ForeColor="Black" Text=" ">...</asp:Label>       
        </td></tr>
    </table> 
                
               <ucmsgbox:msgbox id="MessageBox" runat ="server" > </ucmsgbox:msgbox>
         </ContentTemplate>
        <Triggers>
        <asp:PostBackTrigger ControlID="DropDownList3" />
            <%-- lnkImage needs to be here to get download to work --%>
            <asp:PostBackTrigger ControlID="lnkImage"/> 
            <asp:PostBackTrigger ControlID="lnkPdf"/> 
            <asp:PostBackTrigger ControlID="lnkWord"/> 
        </Triggers>
      </asp:UpdatePanel>
        <div id="spinner" class="modal" style="display:none;">
             <div id="divSpinner" style="text-align: center; width: 130px; display: inline-block; clip: rect(auto, auto, auto, 50%); position: fixed; margin-top: 300px; margin-right: auto; padding-top: 10px; background-color: #f8f8d3; z-index: 2147483647; left: 50%; top: -5%;">
                  <img id="imgSpinner" src="Controls/Images/WaitImage2.gif" style="width: 100px; height: 100px" />
                    <br />
                      Please Wait...
             </div>
        </div>

        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpViewReport">
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
