<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >

  
<head id="Head1" runat="server">
    <title>Online User Reporting</title> 
    <style type="text/css">
        .auto-style1 {
            height: 30px;
        }
        .auto-style2 {
            height: 30px;
            width: 320px;
        }
        .auto-style5 {
            width: 100%;
        }
        .auto-style6 {
            margin-bottom: 0px;
        }
        .auto-style7 {
            width: 160px;
        }
        .auto-style8 {
            height: 30px;
            width: 250px;
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
.demo {
  background-color: transparent; 
  color:blue;
  font-family: Arial; 
  font-size: 20px; 
  font-style: normal; 
  font-variant: normal; 
  font-weight: 400; 
  letter-spacing: normal; 
  orphans: 2; 
  text-align: left; 
  text-decoration: underline; 
  text-indent: 0px; 
  text-transform: none; 
  -webkit-text-stroke-width: 0px; 
  white-space: normal;
  word-spacing: 0px;  
}

        .auto-style9 {
            position: fixed;
            z-index: 2147483647;
            height: 100%;
            width: 100%;
            top: -253px;
            opacity: 0.8;
            left: -3px;
            background-color: #f8f8d3;
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

 <div>
       
         <form id="frmLogon" runat="server">
       
            <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />      
        <asp:UpdatePanel ID="udpDefault" runat ="server" >
            <ContentTemplate>               
                 <script type="text/javascript" src="Controls/Javascripts/OUR.js"></script>
                        <div>
           <table>
      <tr>
           <td colspan="3" style="font-size:x-large; font-style:normal; font-weight:bold; background-color: #e5e5e5; vertical-align:middle; text-align: left; height: 40px;">
              <asp:Label ID="LabelPageTtl" runat="server" Text=""></asp:Label>
          </td>
      </tr> 
        <tr>
            <td style="font-size: x-small; font-style: normal; font-weight: normal; background-color: #e5e5e5; vertical-align: top; text-align: left; width: 15%;">
                    <div id="tree" style="font-size: x-small; font-weight: normal; font-style: normal">
          
        <asp:TreeView ID="TreeView1"  runat="server" Width="100%" NodeIndent="10" Font-Names="Times New Roman"  EnableTheming="True" ImageSet="BulletedList">
          <Nodes>  
            <asp:TreeNode Text="&lt;b&gt;Home&lt;/b&gt;"  Value="~/index1.aspx" Expanded="True" > </asp:TreeNode>

            <asp:TreeNode Text="&lt;b&gt;Contact us&lt;/b&gt;" Expanded="True" Value="ContactUs.aspx"></asp:TreeNode>
            <asp:TreeNode Text="&lt;b&gt;Documentation&lt;/b&gt;"  Value="DataAIHelp.aspx" Expanded="False" Target="_blank">
                 <asp:TreeNode Text="Demo"  Value="https://oureports.net/OUReports/Default.aspx?logon=demo&pass=demo" Expanded="True" Target="_blank"> </asp:TreeNode>                 
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
       <RootNodeStyle HorizontalPadding="2px" Font-Bold="True" />
       <NodeStyle CssClass="NodeStyle" />
       <ParentNodeStyle Font-Bold="True" />
     </asp:TreeView>
     
    </div>
            </td>
            <td width="5px"></td>
   <td id="main" style="width: 85%; text-align: left; vertical-align: top"> 
    <div style="text-align: center;width:100%">
                     <br />
                     <asp:Label ID="Label1" runat="server" Font-Italic="True" ForeColor="#999999" Height="22px" Text="It is time to put the Internet to work making the creation and processing of custom reports convenient, simple, and accessible for end users and administrators." ToolTip="It is time to put the Internet to work making the creation and processing of custom reports convenient, simple, and accessible for end users and administrators.
At OUReports, we can serve organizations from our cloud Web server or by installing our Web application on their own Web server. 
Our system requires only restricted access(reading permissions) to the database of the organization we are serving. Our application automatically analyzes data structure, generates a set of preliminary reports, and provides a simple interface for creating ad hoc reports and conducting statistical research. 
Any organization storing data in SQL Server, MySQL, or Cache Intersystems databases can use our system to quickly and easily generate  fast highly informational and statistical reports." Font-Bold="True" Font-Names="Tahoma" Font-Size="14px" Visible="False"></asp:Label>
                    
               <span style ="font-family:Tahoma;font-size:14px;font-weight:bold;color:Red" >
                    <asp:Label ID="LblInvalid0" runat="server"  Height="25px" Width="800px" ForeColor="#CC3300"></asp:Label>
              </span>                                 
                <br />
                    
              <%-- <span style ="font-family:Tahoma;font-size:14px;font-weight:bold;color:Red" >
                    <span style="display: inline !important; float: none; background-color: rgb(255, 255, 153); color: rgb(153, 0, 0); font-family: Arial; font-size: 13.33px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;">Please read </span>
                    <a href="disclaimer.htm" tabindex="-1" style="background-color: transparent; color: rgb(0, 102, 204); font-family: Arial; font-size: 13.33px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: underline; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><font color="#0000ff" style="background-color: transparent; color: rgb(0, 0, 255); font-size: 13.33px; text-align: left;">Disclaimer</font></a>
                   <span style="display: inline !important; float: none; background-color: rgb(255, 255, 153); color: rgb(153, 0, 0); font-family: Arial; font-size: 13.33px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> 
                and </span>
                <a href="PrivacyPolicy.htm" tabindex="-1" style="background-color: transparent; color: rgb(0, 102, 204); font-family: Arial; font-size: 13.33px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: underline; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"><font color="#0000ff" style="background-color: transparent; color: rgb(0, 0, 255); font-size: 13.33px; text-align: left;">Privacy Policy</font></a><span style="display: inline !important; float: none; background-color: rgb(255, 255, 153); color: rgb(153, 0, 0); font-family: Arial; font-size: 13.33px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;"> first. 
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
        <asp:HyperLink ID="HyperLinkTermsAndCond" runat="server" NavigateUrl="~/TermsAndConditions.pdf" Target="_blank" Font-Names="Arial" Font-Size="Small">Terms and Conditions</asp:HyperLink></span>--%>   
                   <br/>
                <br />
                <asp:Label ID="LblInvalid" runat="server"  Height="25px" Width="800px" ForeColor="#CC3300"></asp:Label>&nbsp;
                 <br />
                 <br />
              </span>  
             <asp:LinkButton ID="lnkDemo" runat="server" TabIndex="-1" ToolTip="Demonstration Reports" Text="DEMO" CssClass="demo" Visible="false"></asp:LinkButton> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;    
             <asp:LinkButton ID="lnkPDF" runat="server" TabIndex="-1" ToolTip="Instruction, Documentation" Text="Introduction and Documentation" CssClass="demo" Visible="false"></asp:LinkButton> 
         <table border="0" cellpadding="1" cellspacing="0" width="100%">
              <tr>
                    <td align="center">
                        <h3 style="font-family: Arial">Please enter your logon and password:&nbsp; </h3>
                    </td>
              </tr>
              
              <tr>
                   <td align="center">                                      
                        <table border="0" cellpadding="0" cellspacing="0" class="auto-style5">
                             <tr style="border-color:#ffffff">
                                  <td  align="right" class="auto-style8">
                                      <asp:Label ID="lblLogOn" runat="server" Font-Bold="True" Font-Names="Tahoma" Font-Size="12px" ForeColor="#CC3300" Text="Logon*:&nbsp;"></asp:Label>
                                  </td>
                                  <td class="auto-style1">
                                      <asp:TextBox ID="Logon" runat="server" TabIndex="0" CssClass="auto-style6" Width="161px"></asp:TextBox>
                                      &nbsp;&nbsp;
                                      <asp:Linkbutton ID="btUserConnection" runat="server" tabindex="-1" Tooltip= "Define connection to user's database." text="Define User Connection" CssClass="NodeStyle" Font-Names="Tahoma" Font-Size="12px" Enabled="False" Visible="False" />
                                  </td>
                             </tr>                    
                       
                             <tr style="border-color:#ffffff">
                                  <td align="right"class="auto-style8">
                                       <span style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: #CC3300;">&nbsp;Password*:&nbsp;</span></td>
                                  <td class="auto-style1">
                                       <input  name="Pass" id="Pass" runat="server" type="password" class="auto-style7"/> &nbsp;&nbsp;
                                     
                                  </td>
                             </tr>
                             <tr id="trProvider" runat="server" visible="false" style="border-color:#ffffff;">
                                  <td align="right" class="auto-style1">
                                       <span style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: #CC3300;">User Database provider*:&nbsp;</span>
                                  </td>
                                  <td align ="left" class="auto-style8">
                                       <asp:DropDownList runat="server" Font-Size="Smaller" ID="dropdownDatabases" Width="165px" >
                                           <asp:ListItem Selected="True" Value="System.Data.SqlClient">SQL Server</asp:ListItem>
                                           <asp:ListItem Value="InterSystems.Data.CacheClient">Intersystems Cache</asp:ListItem>
                                           <asp:ListItem Value="MySql.Data.MySqlClient">MySQL</asp:ListItem>
                                           <asp:ListItem Value="Oracle.ManagedDataAccess.Client">Oracle</asp:ListItem>
                                           <asp:ListItem Value="InterSystems.Data.IRISClient">Intersystems IRIS</asp:ListItem>
                                           <asp:ListItem Value="System.Data.Odbc">ODBC</asp:ListItem>
                                           <asp:ListItem Value="System.Data.OleDb">OleDb</asp:ListItem>
                                           <asp:ListItem Value="Npgsql">PostgreSQL</asp:ListItem>
                                       </asp:DropDownList>
                                  </td>
                             </tr>
                             <tr id="trConnection" runat="server" visible="false" style="border-color:#ffffff; ">
                                  <td align="right" class="auto-style8" width="100%" >
                                        <span style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: #CC3300;">&nbsp;User Connection String*:&nbsp;</span>
                                  </td>
                                  <td align ="left" class="auto-style1" >
                                       <asp:TextBox ID="ConnStr" runat="server" Width="600px"></asp:TextBox> &nbsp;
                                       <asp:CheckBox ID="chkUserDBcase" runat="server" Font-Size="Smaller" Text="use double quote syntax for PostGreSQL db" Visible="False" />
                                  </td>
                             </tr>
                            <tr id="trDBPassword" runat="server" visible="false" style="border-color:#ffffff;">
                                   <td align ="right" class="auto-style8">
                                        <span style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: #CC3300;">&nbsp;Database Password*:&nbsp;</span>
                                   </td>
                                  <td align ="left" class="auto-style1">
                                        <asp:TextBox ID="txtDBpass" runat="server" Width="100px" TextMode="Password" ></asp:TextBox>
                                  </td>
                             </tr>
                           <tr id="trSaveConnection" runat="server" visible="false" style="border-color:#ffffff;">
                                  <td align="right" class="auto-style8">
                                       <span style ="font-family:Tahoma;font-size:12px;">&nbsp;&nbsp;</span>
                                  </td>
                                  <td align ="left" class="auto-style1">
                                          <asp:CheckBox ID="CheckBox1" runat="server" Font-Size="Smaller" Text="Save Connection Info for future use (password will not be saved for security reason)" />
                                  </td>
                             </tr>
                            <tr id="trLogIn" runat="server" style="border-color:#ffffff;visibility:visible">
                                <td class="auto-style8"> </td>
                               <td class="auto-style2">
                                <span style ="font-family:Tahoma;font-size:12px;"> 
                                 <font color="#930000" face="Arial" size="2">
                               <asp:Button ID="btLogin" runat="server" CssClass="ticketbutton" text="Login" />
                                </font>&nbsp;&nbsp;</span>
                                <span style ="font-family:Tahoma;font-size:12px;"><font color="#930000" face="Arial" size="2">
                               <asp:Button ID="btRegister" runat="server" CssClass="ticketbutton" text="Register" />
                                  </td>  
                            </tr>
                            <tr>
                                
                                    <td align="center" colspan="2"> 
                                        <br />
                                     <asp:LinkButton ID="btForgot" runat="server" TabIndex="-1" ToolTip ="Sends password reminder by Email. Logon must be entered." CssClass="NodeStyle" Font-Names="Tahoma" Font-Size="12px">Forgot Password ?</asp:LinkButton>
                                     
                                          &nbsp;
                                          <asp:LinkButton ID="btChange" runat="server" CssClass="NodeStyle" Font-Names="Tahoma" Font-Size="12px" TabIndex="-1" ToolTip="Change Password/Registration. Logon must be entered.">Change Password/Registration</asp:LinkButton>
                                    
                              </td>
                              
                            </tr>                   
 
                        </table> 
                        <font
                               color="#800000" face="Arial" size="2"><b><font color="#930000" face="Arial">
                        <br />
                        </font></b></font> 
                       <br /><br />
                        <font color="#800000" face="Arial" size="2"><b><font color="#930000" face="Arial"><span style="font-family:Tahoma;font-size:14px;font-weight:bold;color:Red">
                        <asp:Label ID="Label2" runat="server" ForeColor="#999999" Height="22px" Text=" At OUReports, we can serve organizations from our cloud Web server, or by installing our Web application on their own Web server, or by deploying a OUReports Appliance. Our system requires only restricted access(reading permissions) to the database of the organization we are serving. Our application automatically analyzes data structure, generates a set of preliminary reports, and provides a simple interface for creating ad hoc reports and conducting statistical research. " ToolTip="It is time to put the Internet to work making the creation and processing of custom reports convenient, simple, and accessible for end users and administrators.
At OUReports, we can serve organizations from our cloud Web server, or by installing our Web application on their own Web server, or by deploying a OUReports Appliance. 
Our system requires only restricted access(reading permissions) to the database of the organization we are serving. Our application automatically analyzes data structure, generates a set of preliminary reports, and provides a simple interface for creating ad hoc reports and conducting statistical research. 
Any organization storing data in SQL Server, MySQL, or Cache Intersystems databases can use our system to quickly and easily generate  fast highly informational and statistical reports." Visible="False"></asp:Label>
                        </span></font></b></font>
                        <br />
                       <br />
                       <br />
                       <br />
                       <br />
                        <p>
                            <%--<a href="mailto:<%=Session("SupportEmail")%>" name="supportemail" tabindex ="-1"><font color="#0000ff" face="Arial Rounded MT Bold" size="1">Support team</font></a>--%>

                        </p>
                        <p align="left">
                            <asp:Label ID="LabelVersion" runat="server" Font-Bold="False" Font-Names="Arial" Font-Size="XX-Small" ForeColor="Black" Text="Version 4-00"></asp:Label>
                        </p>
                       <span id="siteseal"><script async type="text/javascript" src="https://seal.godaddy.com/getSeal?sealID=SYhDKXg2IT7QXHzPEW7z7tmavANkr8vMDCiRmZvbKczmKBJ5Wj8eKl1EX00B"></script></span>
         <br />
                        </font></td>
              </tr>
         </table>  
        </div>
                  </td>
        </tr>
    </table>        



            <ucMsgBox:Msgbox id="MessageBox" runat ="server" > </ucMsgBox:Msgbox> 
                       
        </ContentTemplate>
        <Triggers>
        <asp:PostBackTrigger ControlID="btLogin" />
        </Triggers>
        </asp:UpdatePanel>   
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpDefault">
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
   
   </div>   
     
 </body>
</html>
