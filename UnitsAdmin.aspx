<%@ Page Language="VB" AutoEventWireup="false" CodeFile="UnitsAdmin.aspx.vb" Inherits="UnitsAdmin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Site administration</title>
     <style type="text/css">
        .style1
        {
            height: 23px;
            width: 557px;
        }
        .style2
        {
            height: 27px;
            width: 450px
            /*width: 557px;*/
        }
        .style3
        {
            height: 23px;
            width: 109px;
        }
        .style4
        {
            height: 27px;
            width: 200px;
        }
        .auto-style1 {
            height: 23px;
            width: 378px;
        }
.ticketbutton 
{
  width: 80px;
  height: 25px;
  font-size: 12px;
  border-radius: 5px;
  border-style :solid;
  border-color: #4e4747 ;
  color: black;
  border-width: 1px;
  /*background-color: ButtonFace;*/
  background-image: linear-gradient(to bottom, rgba(211, 211, 211,0),rgba(211, 211, 250,3));

  padding: 3px;
  margin:5px;
  z-index: 9999; 
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
        <div align="center">
             <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/ListOfReports.aspx">List of Reports</asp:HyperLink>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx">Log off</asp:HyperLink>
                        
            <br />
            <asp:Label ID="Label1" runat="server" Text="Units Administration" Font-Size="Larger" Font-Bold="true" ForeColor="Gray"></asp:Label>
        </div>
        <div >     
            <Table>
                <tr height="30px">
                    <td align="left" style="font-weight: bold; color: #ffffff; font-family: Arial; background-color: Gray; font-size:small;" class="auto-style1">
                        <asp:Label ID="Label2" runat="server" ForeColor="White" Text="Search:"></asp:Label>
                         &nbsp;&nbsp
                        <asp:TextBox ID="SearchText" runat="server" Visible="true" width="200px"></asp:TextBox>
                        <asp:Button ID="ButtonSearch" runat="server" CssClass="ticketbutton" Text="Search" Visible="true" valign="center"/>
                    </td>
                    <td Width="20%" align="center">
                        <asp:Label ID="Label3" runat="server" Text=" units"></asp:Label>
                        
                    </td>
                    <td Width="40%" align="center">                        
                       
                        <asp:LinkButton ID="btnRegistration" runat="server" TabIndex="-1" ToolTip="New unit or db Registration.">new unit or db registration</asp:LinkButton>
                    </td>
                    <td>
                        &nbsp;</td>
                </tr>
            </Table>
                     
        </div> 
        <asp:GridView ID="GridViewUnits" runat="server" AllowSorting="True" BackColor="WhiteSmoke" Font-Names="Arial" Font-Size="Small" AllowPaging="True" PageSize="10">
            <AlternatingRowStyle BackColor="#f0f0f0" />
            <RowStyle BackColor="White" />   
            <Columns>
                <%--<asp:BoundField DataField="Indx" HtmlEncode="False" DataFormatString="<a target='_blank' href='UserDefinition.aspx?indx={0}'>edit</a>" />--%>
                 <asp:BoundField DataField="Indx" HtmlEncode="False" DataFormatString="<a href='UnitDefinition.aspx?indx={0}'>edit</a>" />
            </Columns>
        </asp:GridView>
    </form>
</body>
</html>
