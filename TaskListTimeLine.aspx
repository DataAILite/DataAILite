<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TaskListTimeLine.aspx.vb" Inherits="TaskListTimeLine" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Time Line</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="http://OUReports.com">Home</asp:HyperLink>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <a href="TaskList.aspx">Task List</a> 
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/TaskListCalendar.aspx" Enabled="True" Visible="True">Calendar</asp:HyperLink>
           
        </div>
        <h2> Time Line</h2>
        <div>
            <asp:GridView ID="GridViewTML" runat="server">
            </asp:GridView>
        </div>
    </form>
</body>
</html>
