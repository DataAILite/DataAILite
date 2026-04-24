<%@ Page Language="VB" AutoEventWireup="false" CodeFile="index.aspx.vb" Inherits="index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>YANBOR LLC</title>
    <style type="text/css">
        .auto-style2 {
            height: 800px;
        }
        .auto-style4 {
            width: 1334px;
            /*background:url('Controls/Images/backgr.gif')*/
        }
         
        .auto-style5 {
            width: 1334px;
        }
        .auto-style6 {
            width: 1334px;
        }
        .auto-style7 {
            height: 20px;
        }
        .auto-style8 {
            height: 30px;
        }
        .auto-style9 {
            height: 305px;
        }
        .auto-style10 {
            height: 700px;
        }
        .auto-style11 {
            height: 825px;
            /*background-image: url("Controls/Images/graph1.gif");*/
        }
        .auto-style12 {
            height: 23px;
        }
        .auto-style13 {
            width: 320px;
        }
        .auto-style14 {
            width: 449px;
        }
        .auto-style15 {
            width: 373px;
        }
        .auto-style16 {
            width: 18%;
        }
        .auto-style17 {
            width: 393px;
        }
        </style>
</head>
<body class="auto-style11" style="height: 700px" >
    <form id="form1" runat="server" class="auto-style10">
          <asp:Menu ID="Menu1" runat="server" Orientation="Horizontal" StaticEnableDefaultPopOutImage="False">
     <Items>
         <asp:MenuItem Text="OUReports:  &nbsp;&nbsp;&nbsp;&nbsp;    " Value="OUReports" NavigateUrl="~/index.aspx">
         </asp:MenuItem>
         <asp:MenuItem Text="Products" Value="Products">
             <asp:MenuItem Text="OUReports Service" Value="Service" NavigateUrl="~/IndexService.aspx"></asp:MenuItem>
             <asp:MenuItem Text="OUReports Software" Value="Software" NavigateUrl="~/IndexSoftware.aspx"></asp:MenuItem>
         </asp:MenuItem>
         <asp:MenuItem Text="Customers" Value="Customers">
             <asp:MenuItem Text="Individual User" Value="Individual" NavigateUrl="~/IndexIndividual.aspx"></asp:MenuItem>
             <asp:MenuItem Text="Company" Value="Company" NavigateUrl="~/IndexCompany.aspx"></asp:MenuItem>
             <asp:MenuItem Text="Sale agent" Value="Agent" NavigateUrl="~/IndexSaleAgent.aspx"></asp:MenuItem>
         </asp:MenuItem>
        <asp:MenuItem Text="Use cases" Value="Usecases">
                 <asp:MenuItem Text="HealthCare" Value="HealthCare" NavigateUrl="~/UnderConstruction.aspx"></asp:MenuItem>
                 <asp:MenuItem Text="Public data" Value="Public" NavigateUrl="~/UseCasePublic.aspx"></asp:MenuItem>
                 <asp:MenuItem Text="Explore data" Value="Explore" NavigateUrl="~/UnderConstruction.aspx"></asp:MenuItem>
             </asp:MenuItem>
         <asp:MenuItem Text="Study Room" Value="Study">             
             <asp:MenuItem Text="Sandbox" Value="Sandbox" NavigateUrl="~/Default.aspx?logon=demo&pass=demo"></asp:MenuItem>
             <asp:MenuItem Text="Documentation" Value="Documentation" NavigateUrl="~/DataAIHelp.aspx"></asp:MenuItem>
         </asp:MenuItem>
         <asp:MenuItem Text="Contact us" Value="Contact" NavigateUrl="~/ContactUs.aspx"></asp:MenuItem>
     </Items>
    </asp:Menu>
        <br />
    <div style="width:100%; height:20px; text-align:center; font-family: Arial; font-size:x-large; background-color:white;">
        Online User Reports
    </div>
    <div style="width:100%; height:85%; background-image: url('Controls/Images/graph17.png'); background-repeat:no-repeat; background-position:center; background-size: contain;">
        <table style="margin: 0px; padding: 0px; border:none; width:100%; height:100%; table-layout:fixed;">
            <tr>
                <td style="vertical-align: middle; text-align:center; padding: 0px 0px 0px 0px; margin: 0px 0px 0px 0px; ">
                    <asp:Button ID="Button1" runat="server" Text="Free Start" BackColor="#33CC33" BorderColor="#99FF99" BorderStyle="Solid" Font-Bold="True" Width="247px" Height="48px" Font-Size="X-Large" />
                </td>
            </tr>
        </table>
    </div>
    <%-- ***********************************old content ***************************--%>
    <div align="center" style="display:none;">
             <h2 class="auto-style8">&nbsp;Online User Reporting - connect to your database and see reports made for you by OUReports</h2>
            <%--<h4 class="auto-style3">Small businesses and individual users: OUReports are OPEN for you <asp:LinkButton ID="btnRegistration" runat="server" TabIndex="-1" ToolTip="Registration." style="font-size: medium; font-weight: bold">Sign up Now</asp:LinkButton>, the easy report builder,only $1 per day, unlimited user reports, free trial&nbsp;for 10 days.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             
            </h4>--%>
             <%--<p style="font-size: medium; font-weight: bold" valign="center">&nbsp;--%>
                <asp:LinkButton ID="lnkPDF" runat="server" TabIndex="-1" ToolTip="Instroduction, Documentation" Text="Product introduction and details" OnClientClick="target='_blank'"></asp:LinkButton>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="btnDemo" runat="server" TabIndex="-1" ToolTip="Try out demo to see how it works!">Try out DEMO to see how it works</asp:LinkButton>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:HyperLink ID="HyperLinkTermsAndCond" runat="server" NavigateUrl="https://oureports.net/oureports/TermsAndConditions.pdf" Target="_blank" Font-Names="Arial" Font-Size="Small">Terms and Conditions</asp:HyperLink>
                        <%--</p>--%>
            
        </div>
     <br /> 
       <div align="center" style="background-color: #E6E6E6; display:none;" >
        <table width="100%" border="1" bordercolor="white">
            <tr>
                
               <td width="30%">
           <table><tr><td align="left" class="auto-style6">
                <table>
                    <tr>
                        <td  rowspan=2 style="border: thin solid #C0C0C0; font-size: 20px; font-family: Arial, Helvetica, sans-serif; background-color: #FFFFFF;">&nbsp;&nbsp;V&nbsp;&nbsp;</td>
                        <td>
                            <table>
                                <tr>
                                    <td align="left" style="font-family: Arial, Helvetica, sans-serif; font-size: small; font-weight: bold;">&nbsp; Secure cloud based solution</td>                        
                                </tr>
                                <tr>
                                    <td  align="left" style="font-family: Arial, Helvetica, sans-serif; font-size: x-small">&nbsp;&nbsp;No instalation and maintenance hassles</td>                        
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
               <table>
                    <tr>
                        <td  rowspan=2 style="border: thin solid #C0C0C0; font-size: 20px; font-family: Arial, Helvetica, sans-serif; background-color: #FFFFFF;">&nbsp;&nbsp;V&nbsp;&nbsp;</td>
                        
                        <td>
                            <table>
                                <tr>
                                    <td align="left" style="font-family: Arial, Helvetica, sans-serif; font-size: small; font-weight: bold;" class="auto-style17">&nbsp;&nbsp; Analyzing the data structure and doing variety of initial reports for you</td>                        
                                </tr>
                                <tr>
                                    <td  align="left" style="font-family: Arial, Helvetica, sans-serif; font-size: x-small" class="auto-style17">&nbsp;&nbsp; Create stunning reports with ease</td>                        
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
       
           <table>
                    <tr>
                        <td  rowspan=2 style="border: thin solid #C0C0C0; font-size: 20px; font-family: Arial, Helvetica, sans-serif; background-color: #FFFFFF;">&nbsp;&nbsp;V&nbsp;&nbsp;</td>
                        
                        <td>

                            <table>
                                <tr>
                                    <td align="left" style="font-family: Arial, Helvetica, sans-serif; font-size: small; font-weight: bold;">&nbsp;&nbsp; Several supported databases</td>                        
                                </tr>
                                <tr>
                                    <td  align="left" style="font-family: Arial, Helvetica, sans-serif; font-size: x-small" class="auto-style7">&nbsp;&nbsp; MySQL, SQL Server, Intersystems Cache and IRIS, Oracle, xml and csv files, and more coming</td>                        
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                           
       
           <table>
                    <tr>
                        <td  rowspan=2 style="border: thin solid #C0C0C0; font-size: 20px; font-family: Arial, Helvetica, sans-serif; background-color: #FFFFFF;">&nbsp;&nbsp;V&nbsp;&nbsp;</td>
                        
                        <td>

                            <table>
                                <tr>
                                    <td align="left" style="font-family: Arial, Helvetica, sans-serif; font-size: small; font-weight: bold;">&nbsp;&nbsp; Import and Export options</td>                        
                                </tr>
                                <tr>
                                    <td  align="left" style="font-family: Arial, Helvetica, sans-serif; font-size: x-small">&nbsp;&nbsp; RDL, Excel, Word, PDF, CSV, and more</td>                        
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
               <table>
                    <tr>
                        <td  rowspan=2 style="border: thin solid #C0C0C0; font-size: 20px; font-family: Arial, Helvetica, sans-serif; background-color: #FFFFFF;">&nbsp;&nbsp;V&nbsp;&nbsp;</td>
                        
                        <td>
                            <table>
                                <tr>
                                    <td align="left" style="font-family: Arial, Helvetica, sans-serif; font-size: small; font-weight: bold;" class="auto-style12">&nbsp;&nbsp; Simple designer for non-programmers</td>                        
                                </tr>
                                <tr>
                                    <td  align="left" style="font-family: Arial, Helvetica, sans-serif; font-size: x-small">&nbsp;&nbsp; No SQL knowledge is required</td>                        
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
               <table>
                    <tr>
                        <td  rowspan=2 style="border: thin solid #C0C0C0; font-size: 20px; font-family: Arial, Helvetica, sans-serif; background-color: #FFFFFF;">&nbsp;&nbsp;V&nbsp;&nbsp;</td>
                        
                        <td class="auto-style13">
                            <table class="auto-style15">
                                <tr>
                                    <td align="left" style="font-family: Arial, Helvetica, sans-serif; font-size: small; font-weight: bold;" class="auto-style14">&nbsp;&nbsp; "Pay per use" model for individual users</td>                        
                                </tr>
                                <tr>
                                    <td  align="left" style="font-family: Arial, Helvetica, sans-serif; font-size: x-small" class="auto-style14">&nbsp;&nbsp; Cut down your payments</td>                        
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td></tr></table>
               </td>
                <td width="18%" align="center"><b>&nbsp;Small businesses 
                    <br />
                    and individual users:</b><br />
                    <br />
                     OUReports Service is <b>OPEN </b>for you:<br />
&nbsp;<asp:LinkButton ID="btnSignUp" runat="server" TabIndex="-1" ToolTip="Registration." style="font-size: medium; font-weight: bold">Sign up Now</asp:LinkButton>
                    <br /> An easy cloud based report builder
                    for
                    non-programmers.<br />
                    &nbsp;Unlimited user reports, graphics, and statistics.<br />
                    <br />
&nbsp;&nbsp;&nbsp;<strong>Free trial&nbsp;for 100 days&nbsp;<br /></strong>
                    Only $1 per day after free trial.<br />
                &nbsp;&nbsp;<asp:HyperLink ID="HyperLinkTermsAndCond0" runat="server" NavigateUrl="https://oureports.net/oureports/TermsAndConditions.pdf" Target="_blank" Font-Names="Arial" Font-Size="Small">Terms and Conditions</asp:HyperLink>
                         
                    <br />
                    
                </td>
                
                <td align="center" class="auto-style16"><b>&nbsp;Company:</b><br />
                    <br />
                    Interested in company dedicated OUReports Service?<br />
                    <br />
                    <asp:LinkButton ID="btnRegisterCompany" runat="server" TabIndex="-1" ToolTip="Big Company Registration." style="font-size: medium; font-weight: bold">Register</asp:LinkButton> &nbsp;to start the <strong>30 days free trial</strong> with no obligation to continue OUReports Service.<br />
                    <br />
                    We will install it for you on<br />
                    OUR web server and connect&nbsp;to your database on your
                    data server.<br />
                    <br />
                    <asp:HyperLink ID="HyperLinkClient" runat="server" NavigateUrl="~/Partners.pdf" Target="_top" Font-Names="Arial" Font-Size="Small">OUR Business Proposal</asp:HyperLink>
           
                    <br />
                </td>
                <td width="20%" align="center"><b>Vendors, resalers:</b><br />
                    <br />
                    Interested in OUReports Software?<br />
                    <br />
                    <asp:LinkButton ID="btnRegisterVendor" runat="server" TabIndex="-1" ToolTip="Vendor Registration." style="font-size: medium; font-weight: bold">Register</asp:LinkButton> &nbsp;to start the <strong>30 days free trial</strong> with no obligation to buy OUReports Software.
                    <br />
                    <br />
                    After trial we can install it for you on
                    your web server and connect&nbsp;to your database on your data server if requested and paid. <br />
                    <br />

            <asp:HyperLink ID="HyperLinkVendorProposal" runat="server" NavigateUrl="~/Partners.pdf" Target="_blank" Font-Names="Arial" Font-Size="Small">OUR Business Proposal</asp:HyperLink>
           
                </td>
                <td width="18%" align="center"><b>&nbsp;Sale Agents:            <br />
                    <br />
                    <asp:LinkButton ID="btnRegisterAgent" runat="server" TabIndex="-1" ToolTip="OUReports Sale Agent Registration." style="font-size: medium; font-weight: bold">Register</asp:LinkButton></b> &nbsp;to become the OUReports Sale Agent <br />
                    <br />
                    
                    Already OUReports Sale Agent?
                    <asp:LinkButton ID="btnRegisteredAgentLogin" runat="server" TabIndex="-1" ToolTip="OUReports Sale Agent log in." style="font-size: medium; font-weight: bold">Log in</asp:LinkButton> 
                    <br />
                    <br />
                     <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="https://oureports.net/oureports/OUReportsSaleAgentService.pdf" Target="_top" Font-Names="Arial" Font-Size="Small">OUReports Indepedent Contractor Agreement</asp:HyperLink>
           </td>

            </tr></table>
       </div>
       <%--<br />--%>
       <asp:Label ID="Label2" runat="server" Font-Italic="True" ForeColor="#999999" Height="39px" Text="At OUReports, we can serve organizations from our cloud Web server or by installing OUReports Web site on their own Web server. 
Our system requires only restricted access(reading permissions) to the database of the organization we are serving. Our application automatically analyzes data structure, generates a set of preliminary reports, and provides a simple interface for creating ad hoc reports and conducting statistical research. 
Any organization storing data in SQL Server, MySQL, Oracle, dilimited files, or Cache Intersystems databases can use OUReports to quickly and easily generate fast highly informational and statistical reports. If your company, hospital, etc... are using local ReporTrack Report system, you might want to consider our service to generate and process your reports online using OUReports. We can help with conversion." Font-Bold="True" Font-Names="Tahoma" Font-Size="12px"></asp:Label>
        <%--<br />--%>
        <div align="center" style="background-color: #E6E6E6" class="auto-style9">
            <h5>Questions? Special deals for multiple users? Vendors, investors? Feel free to contact us</h5>
            <asp:Label ID="Label1" runat="server" Text=" "></asp:Label>
            <table class="auto-style5">
                <tr><td align="left" class="auto-style4" style="font-size: small">Your full name</td></tr>
                <tr><td align="left" class="auto-style4"><asp:TextBox Width="100%" ID="txtName" runat="server"></asp:TextBox></td></tr>
                <tr><td align="left" class="auto-style4" style="font-size: small">Your email</td></tr>                
                <tr><td align="left" class="auto-style4"><asp:TextBox Width="100%" ID="txtEmail" runat="server"></asp:TextBox></td></tr>
                <tr><td align="left" class="auto-style4" style="font-size: small">Subject</td></tr>
                <tr><td align="left" class="auto-style4"><asp:TextBox Width="100%" ID="txtSubject" runat="server"></asp:TextBox></td></tr>
                <tr><td align="left" class="auto-style4" style="font-size: small">Message</td></tr>
                <tr><td align="left" class="auto-style4"><asp:TextBox Width="100%" ID="txtBody" runat="server" Height="40px" Rows="3" TextMode="MultiLine"></asp:TextBox></td></tr>
                <tr><td align="center" class="auto-style4"><asp:Button ID="btnSendEmail" runat="server" Text="Send" /></td></tr>
                
            </table>
            
        </div>
    </form>
</body>
</html>
