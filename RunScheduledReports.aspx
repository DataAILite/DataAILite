<%@ Page Language="VB" AutoEventWireup="false" CodeFile="RunScheduledReports.aspx.vb" Inherits="RunScheduledReports" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <asp:Timer runat="server" ID="RunTimer"></asp:Timer>

     <table width="100%">
      <tr>
          <td colspan="3" style="font-size:x-large; font-style:normal; font-weight:bold; background-color: #e5e5e5; vertical-align:middle; text-align: left; height: 40px;">
              <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting" ToolTip="Title"></asp:Label>
          </td>
      </tr> 
      <tr>
          <td>
           <div style="text-align:center">
           
               <br /><br />
               <%--<h2>Under construction. We are using this page only for development and testing!</h2>--%>
            <br /><br /><br />
               <h2><asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="ScheduledReports.aspx" Target="_blank" ToolTip="It will open new window/tab">Go to the List of Scheduled Reports</asp:HyperLink></h2>
        
            <br /><br /><br />
            <h1>Keep this page open to run the scheduled reports</h1>
            <br /><br /><br /><br /><br /><br /><br />
            <asp:Label runat="server" ID="LabelNruns"  Text="Number of reports: " Font-Bold="True" Font-Names="Arial" Font-Size="Medium" ForeColor="Red" ToolTip="How many reports were downloaded into local computer, and how many times the page was refreshed." ></asp:Label>
          </div>
         </td>
      </tr>

     </table>
    </form>
</body>
</html>
