<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ScheduleReportsCalendar.aspx.vb" Inherits="ScheduleReportsCalendar" %>
<%@ Register TagPrefix="uc1" TagName="DropDownColumns" Src="Controls/uc1.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Calendar</title>
</head>
<body style="height: 800px">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
          <asp:UpdatePanel ID="udpSchedRep" runat ="server">
          <ContentTemplate>
 &nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
 <asp:HyperLink ID="HyperLink2" runat = "server" NavigateUrl="ReportViews.aspx?see=yes">back to report</asp:HyperLink>

 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
 <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="ScheduledReports.aspx?calndr=yes&tn=">List of Scheduled Reports</asp:HyperLink>
           <br />
        <div style="font-weight: bold; color: #000000; background-color: #C0C0C0;">
            <br />
            Select report recurrence*:&nbsp;
            <asp:DropDownList ID="DropDownRecurse" runat="server">
                <asp:ListItem>daily</asp:ListItem>
                <asp:ListItem>weekly</asp:ListItem>
                <asp:ListItem>monthly</asp:ListItem>
                <asp:ListItem>yearly</asp:ListItem>
            </asp:DropDownList>
           
            <br />
            How many times to repeat*: <asp:TextBox runat = "server" ID="ntimes" Width="31px">0</asp:TextBox>

            <br />
            Time*: <asp:TextBox runat = "server" ID="runtime" Width="200px" ToolTip="Time shows the time on our Server! Military time format, default time shows the time on the server Now. Keep this in mind while scheduling. ">HH:mm:00</asp:TextBox>
        
            <br />
            Send email notification to*:    <uc1:DropDownColumns ID="DropDownColumns" runat="server" Width="250px" ClientIDMode="Predictable" FontName="arial" FontSize="Small" BorderColor="Red" ForeColor="Black" BorderWidth="1" DropDownHeight="190px"/>
                                            
            &nbsp; 
            <br />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            Click on day number in the calendar below to schedule the report starting on the selected day:
            &nbsp; 
            <br />
        </div>
        <br />
        <div>
            <asp:Calendar ID = "Calendar1" runat = "server" Caption="Scheduled Reports Calendar" CellPadding="3" CellSpacing="30" Height="100%" NextPrevFormat="FullMonth" ShowGridLines="True" Width="100%" DayStyle-ForeColor="Black" SelectedDayStyle-ForeColor="Black">
                <DayHeaderStyle Height="30px" />
                <DayStyle Height="80px" HorizontalAlign="Center" VerticalAlign="Middle" />
                <OtherMonthDayStyle BackColor="#EBEBEB" Height="80px" />
                <SelectorStyle BackColor="#FFCC66" />
                <TitleStyle Height="30px" />
                <TodayDayStyle Height="80px" />
            </asp:Calendar>
        </div>
        <br />
        <ucmsgbox:msgbox id="MessageBox" runat ="server" > </ucmsgbox:msgbox>
               </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpSchedRep">
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
