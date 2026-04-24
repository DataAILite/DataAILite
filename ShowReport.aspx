<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ShowReport.aspx.vb" Inherits="ShowReport" %>

<script type="text/javascript" src="Controls/Javascripts/ShowReport.js"></script>
<script type="text/javascript" src="Controls/Javascripts/OUR.js"></script>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Explore Report Data</title>
    <style type="text/css">
        .auto-style1 {
            height: 27px;
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
<body bgcolor="WhiteSmoke">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <asp:UpdatePanel ID="udpShowReport" runat ="server">
       <ContentTemplate> 
           <div>
           <table>
      <tr>
        <td colspan="3" style="font-size: x-large; font-style: normal; font-weight: bold; background-color: #e5e5e5; vertical-align: middle; height: 40px;">
            <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label>
          </td>
      </tr> 
        <tr>
                 <td style="font-size: x-small; font-style: normal; font-weight: normal; background-color: #e5e5e5; vertical-align: top; text-align: left; width: 15%;">
                        <div id="tree" style="font-size: x-small; font-weight: normal; font-style: normal;">
                        <%--<br /><br />--%>
    <asp:TreeView ID="TreeView1"  runat="server" Width="100%" NodeIndent="10" Font-Names="Times New Roman"  EnableTheming="True" ImageSet="BulletedList">
          <Nodes>  
<%--            <asp:TreeNode Text="Back"  Value="back" Expanded="True" NavigateUrl="ListOfReports.aspx" >
              <asp:TreeNode Text="Report Definition" Value="repinfo" Expanded="True" NavigateUrl="ReportEdit.aspx?tne=2" ></asp:TreeNode>
              <asp:TreeNode Text="Report Data Query"  Value="repdata" Expanded="True" NavigateUrl="SQLquery.aspx?tnq=0" ></asp:TreeNode>
              <asp:TreeNode Text="Report Format Definition"  Value="repformat" Expanded="True" NavigateUrl="RDLformat.aspx?tnf=0" ></asp:TreeNode>
              <asp:TreeNode Text="List of Reports"  Value="listrep" Expanded="True" NavigateUrl="ListOfReports.aspx" ></asp:TreeNode>
              <asp:TreeNode Text="Log Off"  Value="logoff" Expanded="True" NavigateUrl="Default.aspx" ></asp:TreeNode>
            </asp:TreeNode>--%>

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
            <td id="MainSection" style="vertical-align: top; text-align: left; width: 85%">  
    <div style="text-align: left;width:100%">
 <%--<asp:Button ID="ButtonReset" runat="server" CssClass="ticketbutton" Text="Reset" ToolTip="Remove restrictions and show original report" AutoPostBack="true" Width="80px" /> --%>
  
 <asp:CheckBox ID="CheckBoxHideDuplicates" runat="server" Checked="True" Text="hide duplicate records" Font-Names="Arial" Font-Size="X-Small" AutoPostBack="True" ToolTip="It does not apply for big data..." Font-Italic="True" />
        &nbsp; &nbsp; &nbsp; &nbsp; 
        <asp:Label ID="Label1" runat="server" Text="Export delimiter:" Font-Italic="True" ForeColor="Black" Font-Size="Small"></asp:Label>&nbsp;&nbsp;
        <asp:TextBox ID="TextBoxDelimeter" runat="server" Width="16px" AutoPostBack="True">,</asp:TextBox>
        &nbsp; &nbsp; &nbsp; &nbsp;<asp:Label ID="LabelAddWhere" runat="server" Text=" " Font-Italic="True" ForeColor="Black" Font-Size="Small"><=></asp:Label>
        &nbsp;&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;
         <asp:LinkButton ID="ButtonReset" runat="server" Text="Reset" ToolTip="Remove restrictions and show original report" AutoPostBack="true" CssClass="NodeStyle" Font-Names="Arial" Font-Size="Small"/>
      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
    <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/ClassExplorer.aspx?FromData=yes" CssClass="NodeStyle" Font-Names="Arial" Font-Size="12px" Visible="True" ToolTip="More details about first report table with relationships if any">Classes</asp:HyperLink>
         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
          <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/ShowReport.aspx?srd=8" CssClass="NodeStyle" Font-Names="Arial" Font-Size="12px" Visible="True" >Show Data Statistics</asp:HyperLink>
        
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:LinkButton  ID="lnkDataAI" runat="server" Font-Size="Small" Visible="False">DataAI</asp:LinkButton>       
         &nbsp;&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; 
        <asp:HyperLink ID="HyperLinkDataAI" runat="server" NavigateUrl="~/DataAI.aspx?pg=expl&srd=0" CssClass="NodeStyle" Font-Names="Arial" Font-Size="12px" Visible="True" ToolTip="DataAI analytics" Font-Bold="True">DataAI</asp:HyperLink>
   
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    
         <asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx?hilt=Explore_Report_Data" Target="_blank" CssClass="NodeStyle" Font-Names="Arial" Font-Size="12px">Help</asp:HyperLink>&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
        <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/Default.aspx" CssClass="NodeStyle" Font-Names="Arial" Font-Size="12px" >Log off</asp:HyperLink>
          &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
    
        <asp:label runat="server" ID="LabelError" Font-Bold="True" ForeColor="Red" Font-Names="Arial"></asp:label>
    <table id="main" runat="server" >
  
    <tr id="old" visible="false" style="height:30px" >
        <td align="left">
        
        <asp:Button ID="ExportToExcel" runat="server" CssClass="ticketbutton" Text="Export Report Data into Excel" AutoPostBack="true" Width="220px" Visible="False"/>  &nbsp;&nbsp;&nbsp;
        <asp:Label ID="LabelExport" runat="server" Text=" " Font-Italic="true" ForeColor="black"></asp:Label>&nbsp;
        <asp:HyperLink ID="HyperLinkExport" runat="server" EnableTheming="False" ForeColor="Blue" Visible="False">[HyperLinkExport]</asp:HyperLink>&nbsp;&nbsp;&nbsp;
         <asp:Button ID="ButtonDownloadFile" runat="server" CssClass="ticketbutton" Text="Download Report" Width="175px" valign="bottom" ToolTip="Download report file to local directory in PDF format" UseSubmitBehavior="False" Visible="False"/>&nbsp;&nbsp;&nbsp;
        <%--<asp:Button ID="ShowRDL" runat="server" Text="Show Report" ToolTip="Show report for records below" AutoPostBack="true" Width="149px"  OnClientClick="target='_blank'" Enabled="False" Visible="False"/> &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;--%>
        <asp:Button ID="ShowRDL" runat="server" CssClass="ticketbutton" Text="Show Report" ToolTip="Show report for records below" AutoPostBack="true" Width="149px"  OnClientClick="target='_blank'" Visible="False" /> &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;
     
        <asp:Button ID="ButtonShowStats" runat="server" CssClass="ticketbutton" Text="Show Statistics" ToolTip="Show report for statistics below" AutoPostBack="true" Width="140px" Visible="False"/> &nbsp;&nbsp;&nbsp;        
        <asp:Button ID="ExportStatsToExcel" runat="server" CssClass="ticketbutton" Text="Export Statistics into Excel" AutoPostBack="true" Width="200px" Visible="False"/>  &nbsp;&nbsp;&nbsp; 
        
                         
        </td>

    </tr>
        
<%--        <tr style="border-color:#ffffff" >
            <td bgcolor="white" style="border-color:#ffffff; font-weight: bold; font-size: medium; color: Gray;" colspan="2" align="left" font-bold="True"></td>
            
        </tr>--%>
       <tr ID="trStatLabel" runat="server"> 
         <td style="border:1px solid black" align="left" bgcolor="lightgrey" > 
           <asp:Label ID="lblStatistics" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Larger" Text="Statistics" ForeColor="Gray"></asp:Label>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
           <asp:LinkButton ID="lnkExportGrid2" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Export to CSV file. May take a long time..." PostBackUrl="ShowReport.aspx?export=GridData&srd=8">Export to Excel</asp:LinkButton>  
              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             <asp:HyperLink ID="HyperLinkChatAI2" runat="server" NavigateUrl="~/ChatAI.aspx?pg=expl&srd=8" Font-Size="Small" ToolTip="Export to CSV file for DataAI analysis. May take a long time..." Font-Bold="True">AI</asp:HyperLink> 
   
             
             <br />
         </td>
       </tr>       
       <tr id="trReportStats" runat="server" >
         <td bgcolor="white">
            <asp:GridView ID="GridView2" runat="server" BackColor="WhiteSmoke" Font-Names="Arial" Font-Size="Small" PageSize="10">
            <AlternatingRowStyle BackColor="WhiteSmoke" />
            <RowStyle BackColor="White" />
            </asp:GridView><br />
        </td>
       </tr>
       <tr id="repttl"> 
           <td style="border:0px solid black" align="left">  
              <asp:Label ID="LabelReportTitle" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size ="Larger" Text="REPORT" ForeColor="Gray"></asp:Label>
                 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
              <asp:Label ID="LabelSearch" runat="server" Text="Search: " Font-Italic="True" ForeColor="Black" ></asp:Label>        
              <asp:DropDownList ID="DropDownColumns" runat="server" Width="150px" >
              </asp:DropDownList>&nbsp; 
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
            </td>
        </tr>
         <tr ID="trParameters" runat="server" style="border-color:#ffffff" >
            <td bgcolor="white" style="border-color:#ffffff; font-weight: bold; font-size: medium; color: Gray;" align="left" font-bold="True">
                <%--<asp:TextBox ID="txtAddWhere" runat="server" Text="" Enabled="False"></asp:TextBox>--%>
            </td>
        </tr>
       
         <tr><td bgcolor="lightgrey" align="left">&nbsp; 
             <asp:label runat="server" ID="LabelRowCount" Font-Bold="True" ForeColor="White" ToolTip="Row Count"></asp:label>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             <asp:LinkButton ID="lnkExportGrid1" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Export to CSV file. May take a long time..." PostBackUrl="ShowReport.aspx?export=GridData">Export to Excel</asp:LinkButton>
             
            <%-- &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             <asp:LinkButton ID="lnkGridAI" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Export to CSV file for DataAI analysis. May take a long time..." >AI</asp:LinkButton>
             --%> 
             
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             <asp:HyperLink ID="HyperLinkChatAI" runat="server" NavigateUrl="~/ChatAI.aspx?pg=expl&srd=0" Font-Size="Small" ToolTip="Export to CSV file for DataAI analysis. May take a long time..." Font-Bold="True">AI</asp:HyperLink> 
   
         
         </td></tr>
        
        <tr><td bgcolor="white">
        <asp:GridView ID="GridView1" runat="server" AllowSorting="True" BackColor="WhiteSmoke" Font-Names="Arial" Font-Size="Small" AllowPaging="True" PageSize="30">
            <AlternatingRowStyle BackColor="WhiteSmoke" />
            <RowStyle BackColor="White" />
        </asp:GridView>
        </td></tr>
        <tr><td bgcolor="lightgrey" >&nbsp;             
        </td></tr>
        <tr><td bgcolor="white" align="left" class="auto-style1">&nbsp;
            <asp:Button ID="ButtonExportIntoCSV" runat="server" CssClass="ticketbutton" Text="Export into delimiter separated CSV" AutoPostBack="true" Height="29px" Width="333px" Visible="False"/>
        <asp:Label ID="LabelExportExcel" runat="server" Text=" " Font-Italic="true" ForeColor="black"></asp:Label>
        <asp:HyperLink ID="HyperLinkToCSVFile" runat="server" EnableTheming="False" ForeColor="Blue" Visible="False">[HyperLinkToCSVFile]</asp:HyperLink>
       &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; 
            <asp:Button ID="ShowCrystal" runat="server" CssClass="ticketbutton" Text="Show Crystal Report" AutoPostBack="true" Width="168px" Visible="False"/> &nbsp;&nbsp;&nbsp;      
         </td>
        </tr>  
        <tr> <td bgcolor="white" style="border-color:#ffffff; font-weight: bold; font-size: medium; color: red;" align="left" font-bold="True">
            <asp:label runat="server" ID="LabelError1"></asp:label></td>
        </tr>
        <tr><td bgcolor="white" align="left">&nbsp; 
            <asp:Label ID="LabelPageFtr" runat="server" Text=" " Font-Italic="true" ForeColor="black"></asp:Label> 
        </td></tr>
         <tr><td bgcolor="white" align="left">&nbsp; 
            <asp:Label ID="LabelSQL" runat="server" Text=" " ForeColor="black"></asp:Label>  
        </td></tr>
        <tr align="left">
            <td>
                <asp:Label ID="LabelExportToNewTable" runat="server" Font-Bold="False" Font-Names="Arial" Font-Size="Small" ForeColor="Black" Text="Export data into new table:"></asp:Label>       
                <asp:TextBox ID="TextBoxExportTableName" runat="server" Width="150px"></asp:TextBox>
            <asp:Button ID="btnExportToTable" runat="server" CssClass="ticketbutton" Text="Export" ToolTip="Export data to new table" AutoPostBack="true" Width="80px" /> 
            <asp:LinkButton ID="btnOpenReport" runat="server" Text="data in new table" ToolTip="Open Data exported into new table"  AutoPostBack="true" ForeColor="Blue" Visible="False" ></asp:LinkButton>
            
            </td>
        </tr>
    </table>
        <asp:Label ID="LabelCrystalLink" runat="server" Text=" " Font-Italic="true" ForeColor="red"></asp:Label><br />
       
        </div>
                  </td>
        </tr>
        <tr align="left">
            <td>
                <asp:Label ID="LabelReportID" runat="server" Font-Bold="False" Font-Names="Arial" Font-Size="XX-Small" ForeColor="Black" Text=" ">...</asp:Label>       
            </td>
        </tr>
        
    </table>
    <ucmsgbox:msgbox id="MessageBox" runat ="server" > </ucmsgbox:msgbox>
          </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ShowRDL" />
            
           <%-- <asp:PostBackTrigger ControlID="ButtonExportIntoCSV" />
            <asp:PostBackTrigger ControlID="Submit1" />          
            <asp:PostBackTrigger ControlID="ButtonShowStats" />
            <asp:PostBackTrigger ControlID="ExportStatsToExcel" />
            <asp:PostBackTrigger ControlID="ExportToExcel" />--%>
        </Triggers>
      </asp:UpdatePanel>
   <div id="spinner" class="modal" style="display:none;">
       <div id="divSpinner" style="text-align: center; width: 130px; display: inline-block; clip: rect(auto, auto, auto, 50%); position: fixed; margin-top: 300px; margin-right: auto; padding-top: 10px; background-color: #f8f8d3; z-index: 2147483647; left: 50%; top: -5%;">
           <img id="imgSpinner" src="Controls/Images/WaitImage2.gif" style="width: 100px; height: 100px" />
           <br />
           Please Wait...
       </div>
    </div>   
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpShowReport" DisplayAfter="500" Visible="True">
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

