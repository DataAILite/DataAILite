<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AddAnalyticsDashboard.aspx.vb" Inherits="AddAnalyticsDashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add To Dashboard</title>
    <style type="text/css">
        body { font-family: Arial; font-size: 12px; background-color: #ffffff; }
        .NodeStyle { color: #0066FF; font-size: 12px; font-weight: normal; text-decoration: none; }
        .NodeStyle:hover { text-decoration: underline; color: darkblue; }
        .box { width: 520px; margin: 80px auto; background-color: #e6eefa; border: 1px solid #222222; }
        .heading { background-color: gray; color: white; height: 24px; line-height: 24px; text-align: center; font-size: small; }
        .content { padding: 12px; }
        .listHeader { background-color: darkgray; border: 1px solid #808080; height: 20px; line-height: 20px; padding-left: 8px; color: white; }
        .listBox { border-style: none solid solid solid; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; border-right-color: #808080; border-bottom-color: #808080; border-left-color: #808080; height: 225px; overflow-y: scroll; background-color: white; }
        .dlgboxbutton {
            width: 80px;
            height: 25px;
            font-size: 12px;
            border-radius: 5px;
            border-style: solid;
            border-color: #4e4747;
            color: black;
            border-width: 1px;
            background-image: linear-gradient(to bottom, rgba(158, 188, 250,0),rgba(158, 188, 250,1));
            padding: 3px;
            margin: 5px;
        }
        .message { color: #990000; font-weight: bold; }
        .summary { color: #333333; line-height: 18px; margin-bottom: 8px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="box">
            <div class="heading">Add To Dashboard</div>
            <div class="content">
                <div class="summary">
                    <asp:Label ID="LabelTileTitle" runat="server"></asp:Label><br />
                    <asp:Label ID="LabelTileUrl" runat="server"></asp:Label>
                </div>
                Name:
                <asp:TextBox ID="TextBoxDashboardName" runat="server" Width="250px" ToolTip="Type a new dashboard name, or check existing dashboards below."></asp:TextBox>
                <asp:Button ID="ButtonFind" runat="server" CssClass="dlgboxbutton" Text="Find" ToolTip="Filter dashboards by entered text." />
                <div class="listHeader">Dashboards</div>
                <div class="listBox">
                    <asp:CheckBoxList ID="CheckBoxListDashboards" runat="server" Width="100%" BorderStyle="None"></asp:CheckBoxList>
                </div>
                <div style="text-align: right; margin-top: 8px;">
                    <asp:Button ID="ButtonAdd" runat="server" CssClass="dlgboxbutton" Text="Add" />
                    <asp:Button ID="ButtonCancel" runat="server" CssClass="dlgboxbutton" Text="Cancel" CausesValidation="false" />
                </div>
                <asp:Label ID="LabelMessage" runat="server" CssClass="message"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>
