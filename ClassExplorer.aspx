<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ClassExplorer.aspx.vb" Inherits="ClassExplorer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Class/Table Explorer</title>
  <style type="text/css">
        .auto-style1 {
            height: 27px;
          width: 1447px;
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
      .auto-style2 {
          font-family: Arial;
          font-weight: bold;
          font-size: larger;
          color: #808080;
      }
      .auto-style3 {
          width: 1447px;
      }
      .auto-style4 {
          width: 1447px;
          height: 159px;
      }
    </style>
</head>
<body bgcolor="WhiteSmoke">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <asp:UpdatePanel ID="udpShowClass" runat ="server">
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
                       
    <asp:TreeView ID="TreeView1"  runat="server" Width="100%" NodeIndent="10" Font-Names="Times New Roman"  EnableTheming="True" ImageSet="BulletedList">
          <Nodes>  

              <asp:TreeNode Text="&lt;b&gt;Log off&lt;/b&gt;"  Value="Default.aspx" Expanded="True" >      
            </asp:TreeNode>

              <asp:TreeNode Text="&lt;b&gt;List of Reports&lt;/b&gt;"  Value="ListOfReports.aspx" Expanded="True" ></asp:TreeNode> 
            
            <asp:TreeNode Text="&lt;b&gt;Demo&lt;/b&gt;"  Value="~/Default.aspx?logon=demo&pass=demo" Expanded="True" >                 
            </asp:TreeNode>
           
            <asp:TreeNode Text="&lt;b&gt;Documentation&lt;/b&gt;"  Value="DataAIHelp.aspx?hilt=Class_Table_Explorer" Expanded="True" >                 
            </asp:TreeNode>

         <%--   <asp:TreeNode Text="&lt;b&gt;Report a problem&lt;/b&gt;"  Value="ListOfReports.aspx?repprbl=yes" Expanded="True" >                 
            </asp:TreeNode>

            <asp:TreeNode Text="&lt;b&gt;Contact us&lt;/b&gt;"  Value="ContactUs.aspx" Expanded="True" >                 
            </asp:TreeNode>--%>
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

        &nbsp; &nbsp; &nbsp;<asp:CheckBox ID="CheckBoxSysTables" runat="server" AutoPostBack="True" Text="show system tables" />
        &nbsp;&nbsp; &nbsp; &nbsp;

 <asp:CheckBox ID="CheckBoxHideDuplicates" runat="server" Checked="True" Text="hide duplicate records" Font-Names="Arial" Font-Size="X-Small" Visible="False" AutoPostBack="True" />
       
        
        &nbsp; &nbsp; &nbsp;
        <asp:LinkButton ID="btnListOfTables" runat="server" Text="List Of Tables" CssClass="NodeStyle" Visible="True" Enabled="True" Font-Names="Arial" ToolTip="Present tables and reports. You can correct the table fields. You can delete tables with no reports."></asp:LinkButton>
       &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="btnListOfJoins" runat="server" Text="List Of Joins" CssClass="NodeStyle" Visible="True" Enabled="True" Font-Names="Arial" ToolTip="Present the list of possible Joins between tables."></asp:LinkButton>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;       
         <asp:CheckBox ID="CheckBoxRelations" runat="server" Checked="True" Text="show relationships" Font-Names="Arial" Font-Size="X-Small" Visible="True" AutoPostBack="True" />
        
        &nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;
         <asp:LinkButton ID="btnCorrectFields" runat="server" Text="correct field types" CssClass="NodeStyle" Font-Names="Arial"></asp:LinkButton>
        
        &nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;
          <asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx?hilt=Class_Table_Explorer" Target="_blank" CssClass="NodeStyle" Font-Names="Arial" Font-Size="12px">Help</asp:HyperLink>&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
       &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;
       <asp:Label ID="Label1" runat="server" Text="Export delimiter:" Font-Italic="True" ForeColor="Black" Font-Size="Small" Visible="False"></asp:Label>&nbsp;&nbsp;
        <asp:TextBox ID="TextBoxDelimeter" runat="server" Width="16px" AutoPostBack="True" Visible="False"></asp:TextBox>
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;<asp:Label ID="LabelAddWhere" runat="server" Text=" " Font-Italic="True" ForeColor="Black" Font-Size="Small"><=></asp:Label>&nbsp;&nbsp;&nbsp;
        <asp:Button ID="ExportToExcel" runat="server" AutoPostBack="true" Text="Export into delimited file" Width="220px" Visible="False" />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;<asp:HyperLink ID="HyperLinkExport" runat="server" EnableTheming="False" ForeColor="Blue" Visible="False">[HyperLinkExport]</asp:HyperLink>&nbsp;&nbsp;&nbsp;
       
    <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/ListOfReports.aspx" CssClass="NodeStyle" Font-Names="Arial" Font-Size="12px" Visible="False">List of reports</asp:HyperLink>
         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
        <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/Default.aspx" CssClass="NodeStyle" Font-Names="Arial" Font-Size="12px">Log off</asp:HyperLink>
          &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
    
        <asp:label runat="server" ID="LabelError" Font-Bold="True" ForeColor="Red" Font-Names="Arial"></asp:label>
    <table id="main" runat="server" >
  
    <tr id="old" visible="false" style="height:30px" >
        <td align="left" class="auto-style3">
        
            &nbsp;&nbsp;&nbsp;
        <asp:Label ID="LabelExport" runat="server" Text=" " Font-Italic="True" ForeColor="Black" Visible="False"></asp:Label>&nbsp;
          <asp:Button ID="ButtonDownloadFile" runat="server" Text="Download Report" Width="175px" valign="bottom" ToolTip="Download report file to local directory in PDF format" UseSubmitBehavior="False" Visible="False"/>&nbsp;&nbsp;&nbsp;
        <%--<asp:Button ID="ShowRDL" runat="server" Text="Show Report" ToolTip="Show report for records below" AutoPostBack="true" Width="149px"  OnClientClick="target='_blank'" Enabled="False" Visible="False"/> &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;--%>
        <asp:Button ID="ShowRDL" runat="server" Text="Show Report" ToolTip="Show report for records below" AutoPostBack="true" Width="149px"  OnClientClick="target='_blank'" Visible="False" /> &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;
     
        <asp:Button ID="ButtonShowStats" runat="server" Text="Show Statistics" ToolTip="Show report for statistics below" AutoPostBack="true" Width="140px" Visible="False"/> &nbsp;&nbsp;&nbsp;        
        <asp:Button ID="ExportStatsToExcel" runat="server" Text="Export Statistics into Excel" AutoPostBack="true" Width="200px" Visible="False"/>  &nbsp;&nbsp;&nbsp; 
        
                         
        </td>

    </tr>
        
<%--        <tr style="border-color:#ffffff" >
            <td bgcolor="white" style="border-color:#ffffff; font-weight: bold; font-size: medium; color: Gray;" colspan="2" align="left" font-bold="True"></td>
            
        </tr>--%>
       <tr ID="trStatLabel" runat="server"> 
         <td style="border:1px solid black" align="left" class="auto-style3"> 
           <asp:Label ID="lblStatistics" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Larger" Text="Statistics" ForeColor="Gray"></asp:Label><br />
         </td>
       </tr>       
       <tr id="trReportStats" runat="server" >
         <td bgcolor="white" class="auto-style4">
            <asp:GridView ID="GridView2" runat="server" BackColor="WhiteSmoke" Font-Names="Arial" Font-Size="Small" PageSize="10">
            <AlternatingRowStyle BackColor="WhiteSmoke" />
            <RowStyle BackColor="White" />
            </asp:GridView><br />
        </td>
       </tr>
       <tr id="repttl"> 
        <td style="border:0px solid black" align="left" class="auto-style3"> 
          
          <table><tr><td valign="top">
              <asp:Label ID="LabelSelect" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Larger" Text="Select " ForeColor="Gray"></asp:Label>
               <span class="auto-style2">TABLE/CLASS:</span>&nbsp;&nbsp;&nbsp;<asp:DropDownList ID="DropDownTables" runat="server" Width="600px" AutoPostBack="True" >
              </asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="LabelNRecords" runat="server" Text="View first " ForeColor="Black" Font-Italic="True" Visible="False"></asp:Label>
               &nbsp;&nbsp;<asp:TextBox ID="TextBoxNRecords" runat="server" Width="46px" Visible="False">1000</asp:TextBox>
               &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br />  
            </td>
            <td>
                &nbsp; &nbsp; <%--&nbsp;&nbsp; &nbsp; --%>
            </td>
            <td>
                 <%-- <br />&nbsp;&nbsp;&nbsp; <br />--%>
              <asp:Label ID="LabelReports" runat="server" Text="Reports:" ForeColor="Gray" class="auto-style2"></asp:Label>
              <br/>
               <table runat="server" id="list" style="border: 5px solid #FFFFFF; font-size: 12px; font-family: Arial">
                <tr runat="server" id="trheaders"  style="text-align: left; vertical-align: top" visible="false">
                    <td align="left" style="font-weight:bold"> Data </td>                
                    <td align="center"  style="font-weight:bold"> Analytics </td> 
                    <td align="center"  style="font-weight:bold"> Report </td>
                    <td align="center"  style="font-weight:bold"> DataAI </td>
                </tr>
                <tr runat="server" id="replist"  style="text-align: left; vertical-align: top" visible="false">
                    <td  style="font-weight:bold"> </td>
                    <td  style="font-weight:bold"> </td>                    
                    <td  style="font-weight:bold"> </td>
                    <td  style="font-weight:bold"> </td>
                </tr>           
               </table>
       
            

           </td></tr></table>




            </td>
        </tr>
         <tr ID="trParameters" runat="server" style="border-color:#ffffff" >
            <td bgcolor="white" style="border-color:#ffffff; font-weight: bold; font-size: medium; color: Gray;" align="left" font-bold="True" class="auto-style3">
                <%--<asp:TextBox ID="txtAddWhere" runat="server" Text="" Enabled="False"></asp:TextBox>--%>        

            
            </td>
        </tr>
       
         <tr ID="trData" runat="server" >
           <td bgcolor="#e5e5e5" align="left" class="auto-style3">&nbsp; 
             <asp:label runat="server" ID="LabelRowCount" Font-Bold="False" ForeColor="Black" ToolTip="Row Count"></asp:label> &nbsp;
             &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;&nbsp; &nbsp;
              <asp:Label ID="LabelSearch" runat="server" Text="Search: " ForeColor="Black" Font-Italic="True"></asp:Label>
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
            <asp:Button ID="ButtonSearch" runat="server" Text="Search" ToolTip="Show data selected" AutoPostBack="true" Width="80px" /> 

            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
            <asp:LinkButton ID="LinkButtonAnalytics" runat="server" Text="Data analytics" CssClass="NodeStyle" Font-Names="Arial"></asp:LinkButton>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:HyperLink ID="HyperLinkChatAI" runat="server" NavigateUrl="~/ChatAI.aspx?pg=expl&srd=0" Font-Size="Small" ToolTip="Export to CSV file for DataAI analysis. May take a long time..." Font-Bold="True">AI</asp:HyperLink> 
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <%--<asp:LinkButton ID="lnkExportGrid1" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Export to CSV file. May take a long time..." PostBackUrl="ShowReport.aspx?export=GridData">Export to Excel</asp:LinkButton>--%>
              <asp:LinkButton ID="lnkExportGrid1" runat="server" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Export to CSV file. May take a long time...">Export to Excel</asp:LinkButton>
         
        </td></tr>
        
        <tr><td bgcolor="white" class="auto-style3">
        <asp:GridView ID="GridView1" runat="server" AllowSorting="True" BackColor="WhiteSmoke" Font-Names="Arial" Font-Size="Small" AllowPaging="True" PageSize="40" AlternatingRowStyle-BackColor="#E6E6E6" CellPadding="0" PagerSettings-Mode="NumericFirstLast" PagerSettings-PageButtonCount="20">
            <AlternatingRowStyle BackColor="WhiteSmoke" />
            <RowStyle BackColor="White" />
        </asp:GridView>
        </td></tr>
        <tr><td bgcolor="#e5e5e5" class="auto-style3">&nbsp;             
        </td></tr>
        <tr><td bgcolor="white" align="left" class="auto-style1">&nbsp;
            <asp:Button ID="ButtonExportIntoCSV" runat="server" Text="Export into delimiter separated CSV" AutoPostBack="true" Height="29px" Width="333px" Visible="False"/>
        <asp:Label ID="LabelExportExcel" runat="server" Text=" " Font-Italic="true" ForeColor="black"></asp:Label>
        <asp:HyperLink ID="HyperLinkToCSVFile" runat="server" EnableTheming="False" ForeColor="Blue" Visible="False">[HyperLinkToCSVFile]</asp:HyperLink>
            &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;&nbsp; 
            <asp:Button ID="ShowCrystal" runat="server" Text="Show Crystal Report" AutoPostBack="true" Width="29px" Visible="False"/>        
        
        </td>
        </tr>  
        <tr> <td bgcolor="white" style="border-color:#ffffff; font-weight: bold; font-size: medium; color: red;" align="left" font-bold="True" class="auto-style3">
            <asp:label runat="server" ID="LabelError1"></asp:label></td>
        </tr>
        <tr><td bgcolor="white" align="left" class="auto-style3">&nbsp; 
            <asp:Label ID="LabelPageFtr" runat="server" Text=" " Font-Italic="true" ForeColor="black"></asp:Label> 
        </td></tr>
         <tr><td bgcolor="white" align="left" class="auto-style3">&nbsp; 
            <asp:Label ID="LabelSQL" runat="server" Text=" " ForeColor="black"></asp:Label>  
        </td></tr>
        <tr align="left">
            <td>
                <asp:Label ID="LabelExportToNewTable" runat="server" Font-Bold="False" Font-Names="Arial" Font-Size="Small" ForeColor="Black" Text="Export data into new table:"></asp:Label>       
                <asp:TextBox ID="TextBoxExportTableName" runat="server" Width="150px"></asp:TextBox>
            <asp:Button ID="btnExportToTable" runat="server" CssClass="ticketbutton" Text="Export" ToolTip="Export data to new table" AutoPostBack="true" Width="80px" /> 
            <%--<asp:LinkButton ID="btnOpenReport" runat="server" Text="data in new table" ToolTip="Open Data exported into new table"  AutoPostBack="true" ForeColor="Blue" Visible="False" ></asp:LinkButton>--%>
            
            </td>
        </tr>
    </table>
        <asp:Label ID="LabelResult" runat="server" Text=" " Font-Italic="true" ForeColor="maroon"></asp:Label><br />
       
        </div>
                  </td>
        </tr>
        <tr align="left"><td>
            <asp:Label ID="LabelReportID" runat="server" Font-Bold="False" Font-Names="Arial" Font-Size="XX-Small" ForeColor="Black" Text=" ">...</asp:Label>       
        </td></tr>
    </table>
               <ucmsgbox:msgbox id="MessageBox" runat ="server" > </ucmsgbox:msgbox>
          </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ButtonSearch" />
            <asp:PostBackTrigger ControlID="btnExportToTable" />
         </Triggers>
      </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpShowClass" DisplayAfter="500" Visible="True">
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

