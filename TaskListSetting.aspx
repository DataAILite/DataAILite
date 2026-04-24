<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TaskListSetting.aspx.vb" Inherits="TaskListSetting" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style2 {
            width: 18%;
        }
        .auto-style3 {
            width: 569px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:HyperLink ID="HyperLinkBack" runat="server" NavigateUrl="HelpDesk.aspx">Back</asp:HyperLink>
        </div>
        <div>

            <h1>Setting of Task List and the Ticket Dialog:</h1>
                <p> 
                </p>
            <strong><asp:Label ID="Label3" runat="server" Text="Headers:"></asp:Label></strong>
                <table width="100%" id="tblHeaders" runat="server" rules="rows" visible="true">
                   <tr style="background-color:darkgray">
                       <td width="30%">
                           <strong>Header</strong>
                       </td>
                        <td class="auto-style2">                           
                           Color
                       </td>
                       <td class="auto-style3">
                           Text</td>
                       <td>
                            </td>
                   </tr>
                 
                   <tr>
                       <td>
                           header1

                       </td>
                       <td class="auto-style2">                           
                           <input type="color" name="h1color" value="#ff0000" id="clrHeader1"></td> 
                       
                       <td class="auto-style3">
                           <asp:TextBox ID="txtHeader1" runat="server" Width="529px" Text="Task List">Task List</asp:TextBox>
                       </td>
                       <td>
                           &nbsp;<%--<asp:LinkButton ID="h1submit" runat="server">update</asp:LinkButton> --%>
                       </td>
                   </tr>
                   <tr>
                       <td>
                           header2

                       </td>
                       <td class="auto-style2">                           
                           <input type="color" name="h2color" value="#ff0000" id="clrHeader2"></td> 
                       
                       <td class="auto-style3">
                           <asp:TextBox ID="txtHeader2" runat="server" Width="526px" Text="Task List"></asp:TextBox>
                       </td>
                       <td>
                            &nbsp;<%--<asp:LinkButton ID="h2submit" runat="server">update</asp:LinkButton>--%>
                       </td>
                   </tr>
                  
               </table>
            <p>
                </p>
               <strong><asp:Label ID="Label1" runat="server" Text="Version Dropdown:"></asp:Label></strong>
                <table width="100%" id="tblVersion" runat="server" rules="rows" visible="true">
                   <tr style="background-color:darkgray">
                       <td width="30%">
                           <strong>Text</strong>
                       </td>
                        <td class="auto-style2">
                           <strong>Color</strong>
                       </td>
                       <td>
                           &nbsp;</td>
                       <td>
                           &nbsp;</td>
                       <td>
                           &nbsp;</td>
                       <td>
                           &nbsp;</td>
                       <td>
                           &nbsp;</td>
                   </tr>
                    <tr>
                       <td>
                         
                           <asp:TextBox ID="txtNewVersion" runat="server" Width="233px"></asp:TextBox>
                           
                       </td> 
                        <td class="auto-style2">
                           <input type="color" name="addclrVersion" value="#ff0000"></td> 
                       
                       <td colspan="4">
                           &nbsp;<asp:LinkButton ID="lnkbtnAddVersion" runat="server">add</asp:LinkButton>
                       </td>
                   </tr>
                  
               </table>
           
           <p>
               </p>
               <strong><asp:Label ID="Label2" runat="server" Text="Status Dropdown:"></asp:Label></strong>
               <table width="100%" id="tblStatus" runat="server" rules="rows" visible="true">
                   <tr style="background-color:darkgray">
                       <td width="30%">
                           <strong>Text</strong>
                       </td>
                       <td class="auto-style2">
                           <strong>Color</strong>
                       </td>
                       <td>
                           &nbsp;</td>
                       <td>
                           &nbsp;</td>
                       <td>
                           &nbsp;</td>
                       <td>
                           &nbsp;</td>
                       <td>
                           &nbsp;</td>
                   </tr>
                    <tr>
                       <td>
                           
                           <asp:TextBox ID="txtNewStatus" runat="server" Width="233px"></asp:TextBox>
                           
                       </td>
                       <td class="auto-style2">                           
                           <input type="color" name="addclrStatus" value="#ff0000" id="addclrStatus"></td> 
                       <td colspan="4">
                           <asp:LinkButton ID="lnkbtnAddStatus" runat="server">add</asp:LinkButton>
                       </td>
                   </tr>
                               
               </table>
           
        </div>
    </form>
</body>
</html>
