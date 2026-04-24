<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ReportCopy.aspx.vb" Inherits="ReportCopy" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Copy Report</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
          <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/ListOfReports.aspx">List of reports</asp:HyperLink>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:HyperLink ID="HyperLink6" runat="server" NavigateUrl="~/ShowReport.aspx"  Visible="False" Enabled="False" ToolTip="Show Report data">Show report data</asp:HyperLink>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/ReportEdit.aspx" ToolTip="Edit Report">Edit report</asp:HyperLink>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;
            <asp:Label ID="LabelAlert" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large" ForeColor="Gray" Text="Label" Visible="False"></asp:Label>
            <br />  <br />    
        <asp:Label ID="LabelAlert0" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large"
            ForeColor="Gray" Text="Copy Report"></asp:Label>&nbsp;&nbsp;</div>
        <p>
            New Report Id:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:TextBox ID="TextBoxReportID" runat="server" Width="13%" Enabled="False"></asp:TextBox>
                                </p>
        <p>
            New Report Title:&nbsp;&nbsp;
                                <asp:TextBox ID="TextBoxReportTitle" runat="server" Width="50%" CausesValidation="True"></asp:TextBox>
                            &nbsp;</p>
        <p>
                
        <asp:Label ID="LabelAlert1" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Medium"
            ForeColor="Gray" Text="Copy:"></asp:Label>:</p>
        <p>
                                <asp:Button ID="ButtonSubmit" runat="server" Text="Submit" Width="147px" />
                            </p>
    </form>
</body>
</html>
