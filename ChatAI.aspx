<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ChatAI.aspx.vb" Inherits="ChatAI" %>

<script type="text/javascript" src="Controls/Javascripts/OUR.js"></script>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Chat with AI</title>
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
    </style>
</head>
<body>
    <form id="form1" runat="server">
        
            <table style="vertical-align: top; text-align: left;" width="100%">
              <tr>
                  <td colspan="3" style="font-size:x-large; font-style:normal; font-weight:bold; background-color: #e5e5e5; vertical-align:middle; text-align: left; height: 40px;">
                      <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label>
                  </td>
              </tr> 
            </table>       
        <br />
               <asp:HyperLink ID="HyperLinkLogOff" runat="server" NavigateUrl="Default.aspx" CssClass="NodeStyle" Font-Names="Arial">Log off</asp:HyperLink> 
               &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
               <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/ListOfReports.aspx" onclick="redirect(event); return false;" CssClass="NodeStyle" Font-Names="Arial">List of Reports</asp:HyperLink> 

               &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
              <asp:HyperLink ID="HyperLinkData" runat="server" NavigateUrl="~/ShowReport.aspx?srd=0"  onclick="redirect(event); return false;" ToolTip="Explore Report Data" CssClass="NodeStyle" Font-Names="Arial">Data</asp:HyperLink> 
        
               &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
               <asp:HyperLink ID="HyperLinkDataAI1" runat="server" NavigateUrl="~/DataAI.aspx?pg=expl&srd=0" onclick="redirect(event); return false;" CssClass="NodeStyle" Font-Names="Arial" Visible="True" ToolTip="DataAI analytics" Font-Bold="True">DataAI</asp:HyperLink>
   
        <br /><br />   
             
        <b>Question to AI:</b>&nbsp;&nbsp;&nbsp;<%=chatrequest%>
         <br /> <br />
       
        <b>AI Answer:</b>
         <br /> <br />
        <div>
            <%=chatresponse%>
            &nbsp;&nbsp;&nbsp; <%--&nbsp; &nbsp; &nbsp;--%> 
            <asp:HyperLink ID="HyperLinkDataAI" runat="server" NavigateUrl="~/DataAI.aspx?pg=expl&srd=0" onclick="redirect(event); return false;"  CssClass="NodeStyle" Font-Names="Arial" Visible="True" ToolTip="DataAI analytics" Font-Bold="True">DataAI</asp:HyperLink>
               &nbsp;&nbsp;&nbsp; <%--&nbsp; &nbsp; &nbsp;--%> 
           <asp:HyperLink ID="HyperLinkOpenAIcreds" runat="server" NavigateUrl="~/DataAIsqlite.aspx?frm=chatai" onclick="redirect(event); return false;"  CssClass="NodeStyle" Font-Names="Arial" Visible="True" ToolTip="OpenAI credentials" Font-Bold="True">OpenAI setting</asp:HyperLink>
        
        </div>
        
        <br />
        &nbsp;<asp:Label ID="Label1" runat="server" Font-Bold="False" Font-Names="Arial" Font-Size="XX-Small" ForeColor="Gray" Text="Use AI with caution. It is not perfect yet..." Font-Italic="True"></asp:Label> 
        
        <br /><br />
        <table runat="server" itemid="qs" width="900px">
            <tr><td>
            <b>Question to AI:</b>
            <br /> <br />
            &nbsp;<asp:TextBox ID="txtQuestion" runat="server" Enabled="True" Height="60px" TextMode="MultiLine" Width="80%"  Wrap="True" BorderColor="Black" > </asp:TextBox>
            <br />
            &nbsp;<asp:Button ID="btnQuestion" runat="server" CssClass="ticketbutton" Text="Ask" ToolTip="Ask AI" AutoPostBack="true" Width="80px" /> 
                        
            </td></tr>        
        </table>  
        <div id="spinner" class="modal" style="display:none;">
                <div id="divSpinner" style="text-align: center; width: 130px; display: inline-block; clip: rect(auto, auto, auto, 50%); position: fixed; margin-top: 300px; margin-right: auto; padding-top: 10px; background-color: #f8f8d3; z-index: 2147483647; left: 50%; top: -5%;">
                    <img id="imgSpinner" src="Controls/Images/WaitImage2.gif" style="width: 100px; height: 100px" />
                    <br />
                      Please Wait...
                </div>
      </div>

    </form>
</body>
</html>
