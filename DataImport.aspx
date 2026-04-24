<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DataImport.aspx.vb" Inherits="DataImport" %>

<script type="text/javascript" src="Controls/Javascripts/OUR.js"></script>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Data Import</title>
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
  width: 80px;
  height: 25px;
  font-size: 12px;
  border-radius: 5px;
  border-style :double;
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

    <asp:UpdatePanel ID="udpDataImport" runat ="server" >
       <ContentTemplate>
           <asp:HiddenField ID="hfSizeLimit" runat="Server" Value="4096" />
<table style="vertical-align: top; text-align: left;">
      <tr>
          <td colspan="4" style="font-size:x-large; font-style:normal; font-weight:bold; background-color: #e5e5e5; vertical-align:middle; text-align: left; height: 40px;">
              <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label>
          </td>
      </tr> 
        <tr>
            <td style="font-size: x-small; font-style: normal; font-weight: normal; background-color: #e5e5e5; vertical-align: top; text-align: left; width: 15%;">
                    <div id="tree" style="font-size: x-small; font-weight: normal; font-style: normal">
                        <%--<br /><br />--%>
       <asp:TreeView ID="TreeView1"  runat="server" Width="100%" NodeIndent="10" Font-Names="Times New Roman"  EnableTheming="True" ImageSet="BulletedList">
          <Nodes>  
             <asp:TreeNode Text="&lt;b&gt;Log Off;&lt;/b&gt;"  Value="https://OUReports.net/OUReports/Default.aspx" Expanded="True" ></asp:TreeNode>

             <asp:TreeNode Text="&lt;b&gt;List of Reports&lt;/b&gt;" Expanded="True" Value="~/ListOfReports.aspx"></asp:TreeNode>          
          
               <asp:TreeNode Text="&lt;b&gt;Contact us&lt;/b&gt;" Expanded="True" Value="https://OUReports.net/OUReports/ContactUs.aspx"></asp:TreeNode>
            <asp:TreeNode Text="&lt;b&gt;Documentation&lt;/b&gt;"  Value="https://OUReports.net/OUReports/OnlineUserReporting.pdf#page=5" Expanded="False" >
                <asp:TreeNode Text="Reports Demo"  Value="https://OUReports.net/OUReports/Default.aspx?logon=demo&pass=demo" Expanded="True" > </asp:TreeNode>                 
                 <asp:TreeNode Text="General documentation " Value="Documentation" NavigateUrl="DataAIHelp.aspx" Target="_blank"></asp:TreeNode>
                <asp:TreeNode Text="Advanced Report Designer " Value="AdvancedReportDesigner " NavigateUrl="https://oureports.net/OUReports/AdvancedReportDesigner.pdf#page=4" Target="_blank"></asp:TreeNode>
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
                 <asp:TreeNode Text="Sample: Public data" Value="Public" NavigateUrl="https://oureports.net/OUReports/UseCasePublic.aspx"></asp:TreeNode>
                 <asp:TreeNode Text="Explore data" Value="Explore" NavigateUrl="https://oureports.net/OUReports/ExploreData.pdf" Target="_blank"></asp:TreeNode>
                 <asp:TreeNode Text="Matrix Balancing" Value="Matrix" NavigateUrl="https://oureports.net/OUReports/MatrixBalancing.pdf#page=2" Target="_blank"></asp:TreeNode>
                 <asp:TreeNode Text="More Matrix Balancing Samples" Value="Matrix" NavigateUrl="https://oureports.net/OUReports/MatrixBalancingSamples.pdf" Target="_blank"></asp:TreeNode>
                <asp:TreeNode Text="Video: Matrix Balanceing Scenarios 1a and 1b" Value="Matrix" NavigateUrl="https://oureports.net/OUReports/Videos/MatrixBalance1a1b.mp4" Target="_blank"></asp:TreeNode>
                <asp:TreeNode Text="Video: Matrix Balanceing Scenarios 2a and 3a" Value="Matrix" NavigateUrl="https://oureports.net/OUReports/Videos/MatrixBalance2a3a.mp4" Target="_blank"></asp:TreeNode>
                <asp:TreeNode Text="Video: Matrix Balanceing Scenarios 2b and 2c" Value="Matrix" NavigateUrl="https://oureports.net/OUReports/Videos/MatrixBalance2b2c.mp4" Target="_blank"></asp:TreeNode>
                <asp:TreeNode Text="Video: Matrix Balanceing Scenarios 3b and 3c" Value="Matrix" NavigateUrl="https://oureports.net/OUReports/Videos/MatrixBalance3b3c.mp4" Target="_blank"></asp:TreeNode>
                <asp:TreeNode Text="Video: Matrix Balanceing Scenarios 4a, 4b, and 4c" Value="Matrix" NavigateUrl="https://oureports.net/OUReports/Videos/MatrixBalance4a4b4c.mp4" Target="_blank"></asp:TreeNode>
                <asp:TreeNode Text="Making Google Maps and Earth documentation " Value="Documentation" NavigateUrl="https://oureports.net/OUReports/MapDefinitionDocumentation.pdf" Target="_blank"></asp:TreeNode>
               <%-- <asp:TreeNode Text="KML generator Demo"  Value="https://OUReports.net/OUReports/Default.aspx?logon=csvdemo&pass=demo" Expanded="True" > </asp:TreeNode>     
               --%> <%--<asp:TreeNode Text="KML generator Help"  Value="https://OUReports.net/OUReports/MapDefinitionDocumentation.pdf" Expanded="True" ></asp:TreeNode>--%>
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
   <td id="main" style="width: 50%; text-align: left; vertical-align: top"> 

        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/ListOfReports.aspx" CssClass="NodeStyle" Font-Names="Arial"  Font-Size="12px">List of reports</asp:HyperLink>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="lnkHelp" runat="server" NavigateUrl="https://oureports.net/OUReports/DataImport.pdf" CssClass="NodeStyle" Font-Names="Arial" Font-Size="12px" Target="_blank">Help</asp:HyperLink> 
         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx" CssClass="NodeStyle" Font-Names="Arial" Font-Size="12px">Log off</asp:HyperLink> 
        <br />
       <h1>Data Import from the local file</h1>
       <%--<br />                
   
        <br/>--%>
        <div style="width: 70%px">
          <table id="Table2" runat="server" bgcolor="#e5e5e5" rules="rows" style="border:medium double #FFFFFF; font-size: small; color: black; font-family: Arial; background-color: #e5e5e5; vertical-align: top; width:100%">
            <tr valign="top" runat ="server"  border="3" style="color: black; font-family: Arial; border:medium double #FFFFFF; background-color: #e5e5e5;border-width: 2px;width:100%;" >                                    
            <td align="left">
                <br />
              &nbsp;&nbsp;<asp:Label ID="LabelTableName" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large" ForeColor="Gray" Text="Upload data into new table or check if table exists:"></asp:Label>
              <br /> &nbsp;&nbsp;
                <asp:Label ID="Label4" runat="server"  Font-Bold="True" Font-Names="Arial" Font-Size="Small"  Text="*Table name:"></asp:Label>
                <asp:TextBox ID="txtTableName" runat="server" AutoPostBack="True" Height="22px" Width="200px" BorderStyle="Double" BorderColor="#CC0000"> </asp:TextBox>
             
                         
             
              
                       <%-- &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>&nbsp;&nbsp;
                        <asp:Label ID="Label3" runat="server"  Font-Bold="False" Font-Names="Arial" Font-Size="Small" ForeColor="Gray" Text="Filter existing tables:"></asp:Label>                        
                        <asp:TextBox ID="txtSearch" runat="server" Visible="true" Height="20px" width="160px"></asp:TextBox>
                        <asp:Button ID="ButtonSearch" runat="server" CssClass="ticketbutton"  ForeColor="Gray" Text="Search" Visible="true" valign="center"/>
                &nbsp;&nbsp;<asp:Label ID="LabelTables" runat="server" Font-Bold="False" Font-Names="Arial" Font-Size="Small" ForeColor="Gray" Text="or select existing table to upload data:"></asp:Label>
               
                <br />
              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:DropDownList ID="DropDownTables" runat="server" AutoPostBack="True" Height="22px" Width="500px">
              </asp:DropDownList>

               <%-- <br />&nbsp;<br />&nbsp;--%>
                  <br />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                     
                <asp:CheckBox ID="chkboxClearTable" runat="server" AutoPostBack="True"  ForeColor="Gray" Text="delete all records from the existing table before upload" ToolTip="All records will be deteted from the selected table and saved in the table with DELETED attached to original table name." />
                    &nbsp;&nbsp;&nbsp;&nbsp;<asp:CheckBox ID="chkboxUniqueFields" runat="server" AutoPostBack="False" Checked="False" Enabled="False" Text="field names in upload file are unique" ToolTip="The field names in file to upload have unique names." Visible="False" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<asp:Label ID="LabelDelimiter" runat="server" Text="csv file delimiter:"  ForeColor="Gray"></asp:Label>
                    <asp:TextBox ID="TextboxDelimiter" runat="server" Height="18px" Width="16px">,</asp:TextBox>
                </td>
            </tr>

            <tr valign="top" runat ="server"  border="3" style="color: black; font-family: Arial; border:medium double #FFFFFF; background-color: #e5e5e5;border-width: 2px;width:100%;" >                                    
             <td align="left">
                <br /><%--&nbsp;  <br /> --%>         

             &nbsp;&nbsp;<asp:Label ID="lblSelectFile" runat="server" Text="*Files to upload:"  Font-Bold="True" Font-Names="Arial" Font-Size="Small" ></asp:Label>
              <%--&nbsp;&nbsp;--%>
              <asp:Button ID="btnBrowse" runat="server" Font-Bold="True" Text="Choose file(s)" ToolTip="Browse for CSV, XML, JSON, Excel, Access file(s) to input data from into the db table."  Width="150px" CssClass="ticketbutton"  BorderColor="#CC0000" BorderStyle="Double" />
                                
              &nbsp;&nbsp;<asp:Label ID="lblFileChosen" runat="server" Text=" " ToolTip=" "></asp:Label>&nbsp;&nbsp;                               
           
              <%--<br /> <br />--%>
              &nbsp;&nbsp;<asp:Label ID="Label1" runat="server" Text=" or upload from url: "  Font-Bold="False" Font-Names="Arial" Font-Size="Small" ForeColor="Gray" Enabled="True" Visible="True"></asp:Label>&nbsp;&nbsp;
              <asp:TextBox ID="txtURI" runat="server" Width="500px" AutoPostBack="True"  ForeColor="Gray" Enabled="True" Visible="True">https://</asp:TextBox>

               <%-- <br />&nbsp;--%>
              </td>
            </tr>


            <%-- <tr valign="top" runat ="server"  border="3" style="color: black; font-family: Arial; border:medium double #FFFFFF; background-color: #e5e5e5;border-width: 2px;width:100%;" >                                    
              <td align="left">
               <br />
              &nbsp;&nbsp;<asp:CheckBox ID="chkboxClearTable" runat="server" AutoPostBack="True" Text="delete all records from the table before upload" ToolTip="All records will be deteted from the selected table and saved in the table with DELETED attached to original table name." />
              &nbsp;&nbsp;&nbsp;&nbsp;<asp:CheckBox ID="chkboxUniqueFields" runat="server" AutoPostBack="False" Checked="False" Enabled="False" Text="field names in upload file are unique" ToolTip="The field names in file to upload have unique names." Visible="False" />
              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<asp:Label ID="LabelDelimiter" runat="server" Text="csv file delimiter:"></asp:Label>
              <asp:TextBox ID="TextboxDelimiter" runat="server" Height="18px" Width="16px">,</asp:TextBox>
              

                <br />&nbsp;
               </td>
            </tr>--%>

          </table>  
              
              <div id="divFileUpload" runat="server" style="margin:3px; display:flex">
                  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                  &nbsp;&nbsp;&nbsp;<asp:Button ID="ButtonUploadFile" runat="server" Text="Upload file(s) into table(s)." ToolTip="Upload file(s) CSV, XML, XLS, XLSX, JSON, TXT-JSON file or Access table. It might take a long time... Please be patient." Width="200px" CssClass="ticketbutton"  BorderColor="#CC0000" BorderStyle="Double" Font-Bold="True" Font-Size="Small" />
             <br />
               &nbsp;&nbsp;<asp:Label ID="LabelAlert1" runat="server" CssClass="LabelStyle" ForeColor="Red" Text=" "></asp:Label><br />
              </div>
              <br />
               &nbsp;<asp:Label ID="LabelAlert" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large" ForeColor="Red" Text=" "></asp:Label>  
              <br />
              <br />
              &nbsp;<asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large" ForeColor="Gray" Text="Result:"></asp:Label>
              <br />
              <br />
              &nbsp;<asp:Label ID="lblTitle" runat="server" Text="Report Title:"></asp:Label>
              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
              <asp:HyperLink ID="HyperLinkReportEdit" runat="server" NavigateUrl="~/ReportEdit.aspx" Enabled="False" Visible="False" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Edit Report title, etc..." Font-Italic="True" Font-Underline="True">edit title</asp:HyperLink>
               &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="hlkSeeImportedData" runat="server" NavigateUrl="~/ShowReport.aspx?srd=0" CssClass="NodeStyle" Font-Names="Arial" Font-Size="12px" Font-Italic="True" ToolTip="See data, reports, analytics, statistics, charts" Font-Underline="True">explore data</asp:HyperLink> 
            
            <br />
              &nbsp;<asp:TextBox ID="TextBoxReportTitle" runat="server" Enabled="False" Height="18px"  TextMode="MultiLine"  Width="100%" Wrap="True"></asp:TextBox>
              <br />
              <br />
              &nbsp;<asp:Label ID="lblSQL" runat="server" Text="Data Query Text:"></asp:Label>
              <br />
              &nbsp;<asp:TextBox ID="TextBoxSQL" runat="server" Enabled="False" Height="18px" TextMode="MultiLine" Width="100%" Wrap="True"></asp:TextBox>
              <br />
              <br />
              &nbsp;<asp:Label ID="lblpageFtr" runat="server" Text="Date and Time of Import:"></asp:Label>
              <br />
              &nbsp;<asp:TextBox ID="TextboxPageFtr" runat="server" Enabled="False" Height="36px" TextMode="MultiLine" Width="100%" Wrap="True"> </asp:TextBox>
              <br />
              <br /> 
              &nbsp;<asp:Label ID="lblRepId" runat="server" Text="Report ID:"></asp:Label>
              <br />
              &nbsp;<asp:TextBox ID="TextboxRepID" runat="server" Enabled="False" Height="18px" Width="60%"> </asp:TextBox>
              <br />
              <br />  
              <asp:Label ID="lblOrientation" runat="server" Visible="false" Text="Report Orientation:"></asp:Label>              
              <asp:TextBox ID="TextboxOrientation" runat="server" Visible="false"  Enabled="False" Height="18px">portrait</asp:TextBox>
             
            <div id="divSeeData" runat="server" style="margin:3px; display:flex">
                <%--&nbsp;&nbsp;&nbsp; <asp:HyperLink ID="hlkSeeImportedData" runat="server" NavigateUrl="~/ShowReport.aspx?srd=0" Target="_blank" CssClass="NodeStyle" Font-Names="Arial" Font-Size="12px" Font-Italic="True">data, reports, analytics.</asp:HyperLink> --%>
            </div>

        </div>


       </td>
       <td>
           &nbsp;

       </td>
       <td id="tdreports" runat="server"  style="width: 48%; text-align: left; vertical-align: top"   visible="false" >
          <br />&nbsp;&nbsp;&nbsp; <br />
           <h2 ><asp:Label ID="LabelReports" runat="server" Text="Reports:"></asp:Label></h2>
           <div>
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
       
           </div>
       </td>
     </tr>
     
    </table>


        <ucMsgBox:Msgbox id="MessageBox" runat ="server" > </ucMsgBox:Msgbox>
        <ucDlgTextbox:DlgTextbox id="dlgTextbox" runat="server" />
      </ContentTemplate>

        <Triggers> 
            <asp:PostBackTrigger ControlID="ButtonUploadFile"/>            
        </Triggers>

      </asp:UpdatePanel>   

        <asp:FileUpload id="FileRDL" runat ="server" AllowMultiple="true" style="display:none;" />
        <%--<asp:FileUpload id="FewFiles" runat ="server" AllowMultiple="true" Enabled="true" style="display:none;" />--%>

            <div id="spinner" class="modal" style="display:none;">
                <div id="divSpinner" style="text-align: center; width: 130px; display: inline-block; clip: rect(auto, auto, auto, 50%); position: fixed; margin-top: 300px; margin-right: auto; padding-top: 10px; background-color: #f8f8d3; z-index: 2147483647; left: 50%; top: -5%;">
                  <img id="imgSpinner" src="Controls/Images/WaitImage2.gif" style="width: 100px; height: 100px" />
                    <br />
                      Please Wait...
                </div>
            </div>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpDataImport">
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
