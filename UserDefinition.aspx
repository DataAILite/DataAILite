<%@ Page Language="VB" AutoEventWireup="false" CodeFile="UserDefinition.aspx.vb" Inherits="UserDefinition" %>
<%@ Register TagPrefix="uc1" TagName="DropDownColumns" Src="Controls/uc1.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Definition</title>
    <style type="text/css">
        .auto-style4 {
            width:60%;
            height:auto;
        }

     .dlgboxbutton 
    {
      width: 80px;
      height: 25px;
      font-size: 12px;
      border-radius: 5px;
      border-style :solid;
      border-color: #4e4747 ;
      color: black;
      border-width: 1px;
      background-image: linear-gradient(to bottom, rgba(158, 188, 250,0),rgba(158, 188, 250,1));
      padding: 3px;
      margin:5px;
      z-index: 9999; 
    }

    .dlgboxbutton:hover {
        background-image:none;
        background-color:lightblue;
        color:black;
    }

    .dlgboxbutton:active { /*mouse down*/
        border-color:blue;
        color:white;
       background-color:darkgray;
    }

    .dlgboxbutton:focus {
       outline-color:blue;
    }

    .tdstyle1 {
        width: 25%;
        text-align: right;
    }

    .tdstyle2 {
        width: 75%;
        text-align: left;
    }

    .body {
        width:100%;
        height:95%;
        }
        
    </style>
</head>
<body>
    <form id="form1" runat="server" >
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />      
        <asp:UpdatePanel ID="udpUserDefinition" runat="server">
            <ContentTemplate>
                <div  style="text-align: center">
                    <h1>&nbsp;User Definition</h1>
                </div>
                <div class="body" align="center">
                     <table id="tblInput" class="auto-style4" >
                        <tr id="trText1" runat="server"  >
                        <td class="tdstyle1">
                            <asp:Label ID="lblText1" runat="server" Text="Logon:"></asp:Label>
                        </td>
                        <td class=" tdstyle2">
                            <asp:TextBox ID="txtLogon" runat="server" Width="90%" Enabled="False" ReadOnly="True" ></asp:TextBox>
                        </td>
                    </tr>
                   <tr id="trText2" runat ="server"  >
                        <td class="tdstyle1">
                            <asp:Label ID="lblText2" runat="server" Text="Name:"></asp:Label>
                        </td>
                        <td class="tdstyle2" >
                            <asp:TextBox ID="txtName" runat="server" Width="90%"></asp:TextBox>
                        </td>
                    </tr>
                   <tr id="trText3" runat ="server" >
                            <td class="tdstyle1">
                            <asp:Label ID="lblText3" runat="server" Text="Unit:"></asp:Label>
                        </td>
                            <td class="tdstyle2" >
                            <asp:TextBox ID="txtUnit" runat="server" Width="90%" Enabled="False"></asp:TextBox>
                        </td>
                    </tr>
                <tr id="tr1" runat ="server"  >
                            <td class="tdstyle1">
                            <asp:Label ID="Label1" runat="server" Text="Email:"></asp:Label>
                        </td>
                            <td class="tdstyle2" >
                            <asp:TextBox ID="txtEmail" runat="server" Width="90%"></asp:TextBox>
                        </td>
                    </tr>

                   <tr id="tr2" runat ="server" >
                            <td class="tdstyle1">
                            <asp:Label ID="Label2" runat="server" Text="Role:"></asp:Label>
                        </td>
                            <td class="tdstyle2"  >
                            <asp:DropDownList ID="ddRoles" runat="server">
                                <asp:ListItem>user</asp:ListItem>
                                <asp:ListItem Value="SITEADMIN">Site Admin</asp:ListItem>
                                <asp:ListItem Value="SUPPORT">Support</asp:ListItem>
                                <asp:ListItem Value="TESTER">Tester</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
               
                 
                    
                     <tr id="tr6" runat="server" >
                            <td class="tdstyle1"  >
                            <asp:Label ID="Label6" runat="server" Text="User database provider:"></asp:Label>
                        </td>
                            <td class="tdstyle2">
                                       <asp:DropDownList runat="server" Font-Size="Smaller" ID="ddConnPrv" AutoPostBack="True" Enabled="False" >
                                           <asp:ListItem Value="System.Data.SqlClient">SQL Server</asp:ListItem>
                                           <asp:ListItem Value="InterSystems.Data.CacheClient">Intersystems Cache</asp:ListItem>
                                           <asp:ListItem Value="InterSystems.Data.IRISClient">Intersystems IRIS</asp:ListItem>
                                           <asp:ListItem Value="MySql.Data.MySqlClient">MySQL</asp:ListItem>
                                           <asp:ListItem Value="Npgsql">PostgreSQL</asp:ListItem>
                                           <asp:ListItem Value="Oracle.ManagedDataAccess.Client">Oracle</asp:ListItem>
                                           <asp:ListItem Value="System.Data.Odbc">ODBC</asp:ListItem>
                                           <asp:ListItem Value="System.Data.OleDb">OleDb</asp:ListItem>
                                           <asp:ListItem Value="CSVfiles">Use our database</asp:ListItem>
                                       </asp:DropDownList>
                        </td>
                    </tr>
                    <tr id="tr5" runat="server">
                            <td class="tdstyle1" >
                            <asp:Label ID="Label5" runat="server" Text="User database connection:"></asp:Label>
                        </td>
                            <td class="tdstyle2">
                                <asp:TextBox ID="txtConnStr" runat="server" Enabled="False" ReadOnly="True" Height="22px" Width="90%"></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trDbPassword" runat="server">
                            <td class="tdstyle1">
                                    <asp:Label ID="LabelDBpassword" runat="server" Text="Database password: " style ="font-family:Tahoma;font-size:12px; font-weight:bold; color: darkgray;" ToolTip="Requested to make initial reports and analytics immediately. You can start making them afterwards in corresponding pages. " Visible="True" />
                            </td>
                            <td class="tdstyle2">
                                    <asp:TextBox ID="txtDBpass" runat="server" Width="100px" TextMode="Password" Visible="True"></asp:TextBox>
                        </td>
                    </tr>

                      <tr id="tr4" runat ="server" >
                            <td class="tdstyle1" >
                            <asp:Label ID="Label4" runat="server" Text="Manage reports:"></asp:Label>
                        </td>
                            <td class="tdstyle2">
                            <asp:DropDownList ID="ddRights" runat="server" Height="26px">
                                <asp:ListItem Value="user">read reports only</asp:ListItem>
                                <asp:ListItem Value="admin">create and edit reports</asp:ListItem>
                            </asp:DropDownList>

                                <asp:CheckBox ID="chkFriendlyNames" runat="server" TextAlign="Left" Text="friendly names" Visible="True" Width="200px" />
                            </td>
                        </tr>
                        <tr id="tr10" runat ="server" >
                                <td class="tdstyle1" >
                            <asp:Label ID="Label10" runat="server" Text="Groups:"></asp:Label>
                        </td>
                            <td class="tdstyle2" style="padding-bottom: 25px;" >
                                    <uc1:DropDownColumns ID="DropDownGroups" runat="server" Width="250px" ClientIDMode="Predictable" FontName="arial" FontSize="Small" BorderColor="Red" ForeColor="Black" BorderWidth="1" DropDownHeight="190px" TextBoxHeight="22px" DropDownButtonHeight="22px" PostBackType="OnClose" />
                            </td>
                        </tr>
                        <tr id="tr7" runat ="server" >
                            <td class="tdstyle1">
                                <asp:Label ID="Label7" runat="server" Text="Comments:"></asp:Label>
                            </td>
                            <td class="tdstyle2">
                                <asp:TextBox ID="txtComments" runat="server" Width="90%"></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="tr8" runat ="server" >
                            <td class="tdstyle1">
                            <asp:Label ID="Label8" runat="server" Text="Start Date:"></asp:Label>
                        </td>
                            <td class="tdstyle2">
                            <asp:TextBox ID="txtStartDate" runat="server" Width="25%" Enabled="False"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="tr9" runat ="server">
                            <td class="tdstyle1">
                            <asp:Label ID="Label9" runat="server" Text="End Date:"></asp:Label>
                        </td>
                            <td class="tdstyle2">
                            <asp:TextBox ID="txtEndDate" runat="server" Width="25%" Enabled="False"></asp:TextBox>
                        </td>
                    </tr>

                    </table>

                   <table id="tblButtons" align="center" >
                      <tr>
                        <td  style="width: 25%; height: auto; text-align: center;">
                           <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="dlgboxbutton" />
                        </td>
                        <td style="width: 25%; height: auto; text-align: center;" >
                            <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="dlgboxbutton"/>
                        </td>
                        <td style="width: 25%; height: auto; text-align: center;" >
                            <asp:Button ID="btnDeleteUser" runat="server" Text="Disable User" CssClass="dlgboxbutton" />
                        </td>
                        <td style="width: 25%; height: auto; text-align: center;" >
                            <asp:Button ID="btnDelete" runat="server" Text="Delete User" CssClass="dlgboxbutton" />
                        </td>
                    </tr>
              </table>
        <ucMsgBox:Msgbox id="MessageBox" runat ="server" > </ucMsgBox:Msgbox> 
        <br />
    <p>
       <asp:Label ID="Label11" runat="server" Text=" " Font-Bold="True" ForeColor="#CC0000"></asp:Label>
                     &nbsp;
                  </p>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>     
    </form>
    
</body>
</html>
