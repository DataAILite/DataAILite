<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CalendarDropDown.ascx.vb" Inherits="Controls_CalendarDropDown" %>
<script type="text/javascript" src="Controls/Javascripts/CalendarDropDown.js"></script>
<style type="text/css">
.ButtonStyle{
        width: 16px;
        height: 16px;
        font-size: 12px;
        border-left:hidden;
        border-top:1px solid dimgray;
        border-right:1px solid dimgray;
        border-bottom:1px solid dimgray;
        /*border-radius:8%;*/

        /*border:1px solid dimgray;*/
        /*padding:1px 1px 1px 1px;*/
        /*padding-left:3px;*/
        /*padding-right:2px;*/
        padding:0px 0px 0px 0px;
        margin:0px 0px 0px 0px;
        background-color:transparent ;
        background-image: url('Controls/Images/calendar5.gif');
        background-position:center;
        image-rendering:auto;
        background-repeat: no-repeat;
    }
    .TodayStyle {
       vertical-align: top; 
       text-align: right;
        background-color: white ;
        height: 24px;
        width:100%;
        padding-left:0px;
        padding-right:0px;
        margin-left:0px;
        margin-right:0px;
        position:relative ;
        z-index: 9999;

    }
    .TextBoxStyle {
        padding:0px 0px 0px 0px; 
        margin:0px 0px 0px 0px;
        border-left:1px solid dimgray;
        border-top:1px solid dimgray;
        border-right:1px solid dimgray;
        border-bottom:1px solid dimgray;
        border-top-left-radius:5%;
        border-bottom-left-radius:5%;
        width:100%;
        height:18px;    
    }
    .auto-style3 {
        width: 16px;
        height: 18px;
        border:hidden;
        font-size: 12px;
        border-radius: 10%;
        border-style :solid;
        border-color: dimgray;
        border-width: 1px;
        background-repeat: no-repeat;
        background-position:center;
        background-color:transparent;
        padding:0px 0px 0px 0px;
        margin:0px 0px 0px 0px;
        position:relative;
        z-index: 9999;
    }
    .CalendarPanel {
        width: 225px;
    }
    .CalendarDiv {
        left:3px;
        width:220px;
        position: relative;
        z-index: 9999;
        visibility:hidden;
    }
    .CalendarDiv a:link {
        text-decoration:none;
    }
    .CalendarDiv a:visited {
        text-decoration:none
    }


</style>
<asp:Panel ID="pnlCalendar"  CssClass="CalendarPanel" runat="server" Height="25px">

<asp:HiddenField ID="hdnOpened" runat="server"/>

<table border="0"  style="margin: 0px; padding: 0px; width:100%; table-layout:fixed;">
    <tr id="rowTextbox" style="height:18px" runat="server">
        <td style="vertical-align: top; padding: 0px 0px 0px 0px; margin: 0px 0px 0px 0px; height: 100%; width: 100%;" >
<%--            <asp:TextBox ID="txtDate" runat="server" style="padding:0px 0px 0px 0px; border-style:solid; border-width:1px; margin:0px 0px 0px 0px; border-color:dimgray; border-top-left-radius:5%; border-bottom-left-radius:5%" Width="100%" Height="18px" AutoPostBack="True"></asp:TextBox>--%>
                <asp:TextBox ID="txtDate" runat="server" AutoPostBack="True" CssClass="TextBoxStyle"></asp:TextBox>
        </td>
        <td id="tdButton" style="vertical-align: top ;left: auto;" class="ButtonStyle" runat="server">
            <%--<asp:Button ID="btnDate" runat="server" style="display:none;" />--%>
        </td>
    </tr>
</table>
    <div id="DivCalendar" class="CalendarDiv" runat="server">
        <asp:Calendar ID="Calendar1" runat="server" Font-Names="Tahoma" Font-Size="Medium" TabIndex="-1" BorderStyle="Solid" BorderWidth="1px" width="100%" BorderColor="dimgray" BackColor="White" CaptionAlign="Top" >
            <DayHeaderStyle Font-Names="Tahoma" Font-Size="Smaller" Font-Overline="False" Font-Bold="True"  />
            <DayStyle Font-Names="Tahoma" Font-Size="Smaller" />
            <TodayDayStyle ForeColor="Red" />
        </asp:Calendar>
    </div>
    <table border="0" style="margin: 0px 0px 0px 0px; padding: 0px 0px 0px 0px; width:100%">
    <tr id="TodayRow" runat="server" style="visibility:hidden;" >
        <td class="TodayStyle"> 
         <asp:Button ID="btnToday" runat="server" Width="22px" Height="18px" BackColor="White" BorderColor="#0066FF" BorderStyle="Solid" TabIndex="-1"/>
         &nbsp;<asp:Label ID="lblToday" runat="server" Text="Today: 5/3/2015" Font-Names="Tahoma" Font-Size="Small" ForeColor="Black" Height="20px"></asp:Label>
         &nbsp;
        </td>
    </tr>
    </table>
    </asp:Panel>
