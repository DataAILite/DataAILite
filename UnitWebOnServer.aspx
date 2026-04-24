<%@ Page Language="VB" AutoEventWireup="false" CodeFile="UnitWebOnServer.aspx.vb" Inherits="UnitWebOnServer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Unit Web</title>
</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />   
        <div style="font-size: x-large; font-style: normal; font-weight: bold; background-color: #e5e5e5; text-align:left; height: 40px; line-height:40px; width:100%;">
             <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label>
         </div>
         <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/index1.aspx" CssClass="NodeStyle" Font-Names="Arial">Home</asp:HyperLink>

        <br /> <br /> <br /> <br />
        <div align="center">
            <asp:Label ID="Label1" runat="server" Text="" Font-Bold="True" Font-Size="Larger" ForeColor="#CC0000"></asp:Label>
        </div>
    </form>
</body>
</html>
