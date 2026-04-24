<%@ Page Language="VB" AutoEventWireup="false" CodeFile="RunScheduledItems.aspx.vb" Inherits="RunScheduledItems" %>

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
               <h2><asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="ScheduledDownloads.aspx" Target="_blank" ToolTip="It will open new window/tab">Go to the List of Scheduled Downloads</asp:HyperLink></h2>
        
            <br /><br /><br />
            <h1>Keep this page open to run the scheduled downloads</h1>
            <br /><br /><br /><br /><br /><br /><br />
            <asp:Label runat="server" ID="LabelNruns"  Text="Number of downloads: " Font-Bold="True" Font-Names="Arial" Font-Size="Medium" ForeColor="Red" ToolTip="How many files were downloaded into local computer, and how many times the page was refreshed." ></asp:Label>
          </div>
         </td>
      </tr>

     </table>
    </form>
</body>
</html>
