<%@ Page Language="VB" AutoEventWireup="false" CodeFile="UnitDefinition.aspx.vb" Inherits="UnitDefinition" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Unit Definition</title>
    <style type="text/css">
        .auto-style1 {
            width: 531px;
            width: 20%;
           
        }
        .auto-style3 {
            height: 29px;
        }
        .auto-style4 {
            width: 86%;
        }
        .auto-style5 {
            width: 531px;
            width: 20%;
            height: 29px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />      
        <div align="center">
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:HyperLink ID="HyperLinkUsers" runat="server" NavigateUrl="~/SiteAdmin.aspx?unitdb=yes" >Users</asp:HyperLink>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp; 
        <asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx" Target="_blank">Help</asp:HyperLink>&nbsp;&nbsp;
                 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
               &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx">Log off</asp:HyperLink>            
         <br />
            <h1>Unit/db Definition</h1>
        </div>
        <div align="center">
            <table id="tblInput" class="auto-style4" >
                     <tr id="trText1" runat="server"  align="left">
                        <td class="auto-style1" >
                            <asp:Label ID="lblText1" runat="server" Text="Edit by:"></asp:Label>
                        </td>
                        <td class="auto-style3">
                            <asp:TextBox ID="txtLogon" runat="server" Width="98%" Enabled="False" ReadOnly="True" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="trText3" runat ="server"  align="left">
                        <td class="auto-style1">
                            <asp:Label ID="lblText3" runat="server" Text="Unit:"></asp:Label>
                        </td>
                        <td class="td2">
                            <asp:TextBox ID="txtUnit" runat="server" Width="98%"></asp:TextBox>
                        </td>
                    </tr>
                   <tr id="trText2" runat ="server"  align="left" >
                        <td class="auto-style1">
                            <asp:Label ID="lblText2" runat="server" Text="Unit Web:"></asp:Label>
                        </td>
                        <td class="td2">
                            <asp:TextBox ID="txtUnitWeb" runat="server" Width="98%"></asp:TextBox>
                        </td>
                    </tr>
                   
                    <tr id="tr1" runat="server"  align="left">
                        <td class="auto-style5" >
                            <asp:Label ID="Label1" runat="server" Text="OURdb Connection String:"></asp:Label>
                        </td>
                        <td class="auto-style3">
                            <asp:TextBox ID="txtOURdb" runat="server" Width="98%" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="tr2" runat="server"  align="left">
                        <td class="auto-style5" >
                            <asp:Label ID="Label2" runat="server" Text="OURdb Connection Provider:"></asp:Label>
                        </td>
                        <td class="auto-style3">
                                       <asp:DropDownList runat="server" Font-Size="Smaller" ID="ddOURConnPrv" AutoPostBack="True" >
                                           <asp:ListItem Value="System.Data.SqlClient">SQL Server</asp:ListItem>
                                           <asp:ListItem Value="InterSystems.Data.CacheClient">Intersystems Cache</asp:ListItem>
                                           <asp:ListItem Value="InterSystems.Data.IRISClient">Intersystems IRIS</asp:ListItem>
                                           <asp:ListItem Value="MySql.Data.MySqlClient">MySQL</asp:ListItem>
                                           <asp:ListItem Value="Oracle.ManagedDataAccess.Client">Oracle</asp:ListItem>
                                           <asp:ListItem Value="Npgsql">PostgreSQL</asp:ListItem>
                                       </asp:DropDownList>
                        </td>
                    </tr>
                    <tr id="tr5" runat="server"  align="left">
                        <td class="auto-style1" >
                            <asp:Label ID="Label5" runat="server" Text="User Connection String:"></asp:Label>
                        </td>
                        <td class="auto-style3">
                            <asp:TextBox ID="txtUserConnStr" runat="server" Width="98%" ></asp:TextBox>
                        </td>
                    </tr>
                     <tr id="tr6" runat="server"  align="left">
                        <td class="auto-style1" >
                            <asp:Label ID="Label6" runat="server" Text="User Connection Provider:"></asp:Label>
                        </td>
                        <td class="auto-style3">
                                       <asp:DropDownList runat="server" Font-Size="Smaller" ID="ddUserConnPrv" AutoPostBack="True" >
                                           <asp:ListItem Value="System.Data.SqlClient">SQL Server</asp:ListItem>
                                           <asp:ListItem Value="InterSystems.Data.CacheClient">Intersystems Cache</asp:ListItem>
                                           <asp:ListItem Value="InterSystems.Data.IRISClient">Intersystems IRIS</asp:ListItem>
                                           <asp:ListItem Value="MySql.Data.MySqlClient">MySQL</asp:ListItem>
                                           <asp:ListItem Value="Oracle.ManagedDataAccess.Client">Oracle</asp:ListItem>
                                       </asp:DropDownList>
                        </td>
                    </tr>
                    <tr id="tr7" runat ="server"  align="left">
                        <td class="auto-style1">
                            <asp:Label ID="Label7" runat="server" Text="Comments:"></asp:Label>
                        </td>
                        <td class="td2">
                            <asp:TextBox ID="txtComments" runat="server" Width="98%"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="tr3" runat ="server"  align="left">
                        <td class="auto-style1">
                            <asp:Label ID="Label3" runat="server" Text="Distribution Model:"></asp:Label>
                        </td>
                        <td class="td2">
                             <asp:DropDownList ID="ddModels" runat="server">
                                 <%--<asp:ListItem>UNITOURwebOnOUR-UNITOURdbOnOUR</asp:ListItem>
                                 <asp:ListItem>UNITOURwebOnOUR-UNITOURdbOnUNIT</asp:ListItem>
                                 <asp:ListItem>UNITOURwebOnUNIT-UNITOURdbOnOUR</asp:ListItem>                                 
                                 <asp:ListItem>UNITOURwebOnUNIT-UNITOURdbOnUNIT</asp:ListItem>--%>
                                 <asp:ListItem>UNIT OUReports on OUR server</asp:ListItem>                                                             
                                 <asp:ListItem>UNIT OUReports on UNIT server</asp:ListItem>
                                 <asp:ListItem>OURweb-OURdb</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr id="tr4" runat ="server"  align="left">
                        <td class="auto-style1">
                            <asp:Label ID="Label4" runat="server" Text="Start Date:"></asp:Label>
                        </td>
                        <td class="td2">
                             <div id="divDate" class="inline" runat="server" align="left" style="width: 195px; padding-left: 5px;" >
                                      <asp:TextBox id="Date1" runat="server" Width="100%" ></asp:TextBox>
                                      <ajaxToolkit:CalendarExtender ID="ceDate1" runat ="server"  TargetControlID="Date1" Format="M/d/yyyy" TodaysDateFormat="M/d/yyyy" />
                             </div>
                        </td>
                    </tr>
                    <tr id="tr8" runat ="server"  align="left">
                        <td class="auto-style1">
                            <asp:Label ID="Label8" runat="server" Text="End Date:"></asp:Label>
                        </td>
                        <td class="td2">
                            <div id="divDate2" class="inline" runat="server" align="left" style="width: 195px; padding-left:5px;" >
                                   <asp:TextBox id="Date2" runat  ="server" Width="100%" ></asp:TextBox>
                                   <ajaxToolkit:CalendarExtender ID="ceDate2" runat ="server"  TargetControlID="Date2" Format="M/d/yyyy" TodaysDateFormat="M/d/yyyy" />
                                </div>
                        </td>
                    </tr>
              </table>
        <ucMsgBox:Msgbox id="MessageBox" runat ="server" > </ucMsgBox:Msgbox> 
        <br />
        &nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnSave" runat="server" Text="Save" />
   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnSeeProposal" runat="server" Text="See Business Proposal" />
            &nbsp;&nbsp;&nbsp;&nbsp;<br />
            <br />
            &nbsp;&nbsp;&nbsp;<asp:Button ID="btnInstall" runat="server" Text="Install new unit" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnUpdate" runat="server" Text="Update to current version" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnUninstall" runat="server" Text="Uninstall OURdb tables" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnDeleteUnit" runat="server" Text="Disable Unit" />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
      </div>

    <p>
        <asp:Label ID="Label9" runat="server" ForeColor="Gray"></asp:Label>
        </p>

        <p>
            &nbsp;</p>
        <p>
            <asp:Label ID="Label10" runat="server" Font-Italic="True" Font-Size="X-Small" Text="Unit index #"></asp:Label>
        </p>

    </form>
    </body>
</html>
