<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TaskListCalendar.aspx.vb" Inherits="TaskListCalendar" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Calendar</title>
</head>
<body style="height: 800px">
    <form id="form1" runat="server">
        <div>
            <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="http://OUReports.com">Home</asp:HyperLink>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <a href="TaskList.aspx">Task List</a>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:HyperLink ID="HyperLink5" runat="server" NavigateUrl="~/TaskListTimeLine.aspx">Time Line</asp:HyperLink>
        </div>
        <div>
            <asp:Calendar ID = "Calendar1" runat = "server" Caption="Task List Calendar" CellPadding="3" CellSpacing="30" Height="100%" NextPrevFormat="FullMonth" ShowGridLines="True" Width="100%" DayStyle-ForeColor="Black" SelectedDayStyle-ForeColor="Black">
                <DayHeaderStyle Height="30px" />
                <DayStyle Height="80px" HorizontalAlign="Center" VerticalAlign="Middle" />
                <OtherMonthDayStyle BackColor="#EBEBEB" Height="80px" />
                <SelectorStyle BackColor="#FFCC66" />
                <TitleStyle Height="30px" />
                <TodayDayStyle Height="80px" />
            </asp:Calendar>
        </div>
    </form>
</body>
</html>
