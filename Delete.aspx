<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Delete.aspx.vb" Inherits="Delete" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div align="center">
        <br />
        <br />
        <br />
        <asp:Label ID="LabelSure" runat="server" Font-Size="X-Large" ForeColor="#C00000"
            Text="Are you sure you want to delete the report? " Width="445px"></asp:Label>
        <br />
        <br /><asp:Button ID="ButtonYes" runat="server" Text="Yes" Width="64px" />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;<asp:Button
            ID="ButtonNo" runat="server" Text="No" Width="58px" />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;
        
        </div>
    </form>
</body>
</html>
