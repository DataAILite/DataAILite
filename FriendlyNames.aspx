<%@ Page Language="VB" AutoEventWireup="false" CodeFile="FriendlyNames.aspx.vb" Inherits="FriendlyNames" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Friendly Names:</title>
    <style type="text/css">
        .auto-style1 {
            height: 78px;
        }
.modal
{
    position: fixed;
    z-index: 2147483647;
    height: 100%;
    width: 100%;
    top: 0;
    background-color: #f8f8d3;
    opacity: 0.8;
}
.center
{
    z-index: 2147483647;
    margin: 300px auto;
    padding-left:25px;
    padding-top:10px;
    width: 130px;
    background-color:#f8f8d3;
    border-radius: 10px;
}
.center img
{
    height: 100px;
    width: 100px;
}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <asp:UpdatePanel ID="udpFriendlyNames" runat ="server">
        <ContentTemplate>     
            <div width="100%">
             <asp:Label ID="LabelAlert0" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Large"
            ForeColor="Gray" Text="Friendly Names"></asp:Label>&nbsp;
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        &nbsp;<asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/ListOfReports.aspx">List of Reports</asp:HyperLink>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
        &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; <a href="Default.aspx">Log Off</a>
            <br />
             <table id="SQLselect" runat="server" bgcolor="#663300" border="1" rules="rows" style=" font-size: small;
                                        color: black; font-family: Arial; background-color: #ffffff" width="100%">
                                    <tr valign="top">
                                        <td align="left" bgcolor="#999999" nowrap="nowrap" style="font-weight: normal; font-size: small; color: white;
                                                 font-family: Arial; background-color: #999999; " valign="top" class="auto-style1">Table:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            <br />
                                            <asp:DropDownList ID="DropDownTables" runat="server" AutoPostBack="True">
                                            </asp:DropDownList>
                                        </td>
                                  <td align="left" style="font-size: small; color: white;
                                                 background-color: #999999; font-weight: normal;" valign="top" class="auto-style1" >Field:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <br />
                                             <asp:DropDownList ID="DropDownFields" runat="server" AutoPostBack="True">
                                            </asp:DropDownList>
                                        </td>
                                     <td align="left"  style="font-weight: bold; font-size: small; color: white; font-family: Arial;background-color: #999999; " valign="top" class="auto-style1"  >
                                            Friendly Name: <br /> 
                                            <asp:TextBox ID="TextBoxFieldFriendly" runat="server" Height="16px" Width="544px"></asp:TextBox><br />
                                            <asp:CheckBox ID="CheckBoxAllTableFields" runat="server" Font-Italic="True" Font-Size="X-Small" Text="for fields in tables" />
&nbsp;
                                            <asp:CheckBox ID="CheckBoxAllDataFields" runat="server" Font-Italic="True" Font-Size="X-Small" Text="for data fields" />
&nbsp;
                                            <asp:CheckBox ID="CheckBoxAllReportColumns" runat="server" Font-Italic="True" Font-Size="X-Small" Text="for report columns" />
&nbsp;
                                            <asp:CheckBox ID="CheckBoxAllGroupFields" runat="server" Font-Italic="True" Font-Size="X-Small" Text="for report groups" />
                                          &nbsp;
                                            <asp:CheckBox ID="CheckBoxAllParameters" runat="server" Font-Italic="True" Font-Size="X-Small" Text="for report params" />
                                          </td>
                                        
                                        <td align="left"  nowrap="nowrap" style="font-weight: bold; font-size: small; color: white;
                                                font-family: Arial; background-color: #999999; " valign="top" class="auto-style1">
                                            <asp:Label ID="Label1" runat="server" Text="Selected" Visible="False" Enabled="False"></asp:Label>
                                            <br />
                                            <asp:Button ID="ButtonAssignFriendlyName" runat="server" Text="Assign/Update Friendly Name to the Field(s)" Width="340px" />
                                        </td>
                                    </tr>                                    
                                </table>
            <br />  <asp:Label ID="LabelAlert1" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Medium"
            ForeColor="Gray" Text="Friendly Field Names:" Visible="False"></asp:Label>&nbsp;<br />

                        <asp:GridView ID="GridView1" runat="server" BackColor="White" Font-Names="Arial" Font-Size="Small" AutoGenerateColumns="False">
                <AlternatingRowStyle BackColor="WhiteSmoke" />
                <Columns>
                    <asp:BoundField HeaderText="TABLE_NAME" DataField="TABLE_NAME"  />
                    <asp:BoundField HeaderText="COLUMN_NAME" DataField="COLUMN_NAME"  />
                    <asp:BoundField HeaderText="GlobalFriendlyName" DataField="GlobalFriendlyName"  />
                    <asp:BoundField HeaderText="Reports" HtmlEncode="False" DataField="Reports" />
                    <asp:BoundField HeaderText="ReportDataFieldFriendlyName" HtmlEncode="False" DataField="ReportDataFieldFriendlyName" />
                    <asp:BoundField HeaderText="ReportColumnFriendlyName" HtmlEncode="False" DataField="ReportColumnFriendlyName" />
                    <asp:BoundField HeaderText="ReportGroupFriendlyName" HtmlEncode="False" DataField="ReportGroupFriendlyName" />
                    <asp:BoundField HeaderText="ReportParameterFriendlyName" HtmlEncode="False" DataField="ReportParameterFriendlyName" />
                </Columns>
                <RowStyle BackColor="White" />
            </asp:GridView>
            <br />
           </div>
          </ContentTemplate>        
      </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpFriendlyNames">
            <ProgressTemplate >
            <div class="modal" >
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
