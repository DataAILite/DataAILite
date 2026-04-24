<%@ Page Language="VB" AutoEventWireup="false" CodeFile="InstallIt.aspx.vb" Inherits="InstallIt" %>
<%@ Register TagPrefix="uc1" TagName="DropDownColumns" Src="Controls/uc1.ascx" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Units Installation and Update</title>
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
       <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />      
       <asp:UpdatePanel ID="udpInstall" runat="server">
           <ContentTemplate>
       <%-- <p>
            
 &nbsp; &nbsp;<asp:Image ID="Image1" runat="server" ImageUrl="/Controls/Images/graph5small.png" Height="70px" Width="120px" />
            <br />
        </p>--%>
        <div align="center" style="font-size: small; font-family: Arial">
            <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/ListOfReports.aspx">List of Reports</asp:HyperLink>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:HyperLink ID="HyperLink5" runat="server" NavigateUrl="~/ClassExplorer.aspx" Visible="False">Class Explorer</asp:HyperLink>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:HyperLink ID="HyperLinkTaskList" runat="server" NavigateUrl="~/TaskList.aspx">Task List</asp:HyperLink>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:HyperLink ID="HyperLinkHelpDesk" runat="server" NavigateUrl="~/HelpDesk.aspx">Help Desk</asp:HyperLink>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/Default.aspx">Log off</asp:HyperLink>
            <br />
 <br /><asp:Label ID="Label1" runat="server" Font-Size="Small" ForeColor="Maroon"></asp:Label>
            <br />
            <br />
            <asp:Label ID="Label5" runat="server" Font-Size="Small" ForeColor="Maroon"></asp:Label>
            <br />
            <br />
             <br />
            <br />
            <div align="center">
                <asp:Label ID="Label10" runat="server" Font-Bold="True" Font-Size="Larger" ForeColor="Gray" Text="Server Remote Connections"></asp:Label>
            </div>  
            <br /> 
            &nbsp;Ticket 
            <asp:DropDownList ID="ddTickets" runat="server">
                <asp:ListItem Selected="True">Undefined</asp:ListItem>
            </asp:DropDownList>
            &nbsp;Server
            <asp:TextBox ID="txtIP" runat="server"></asp:TextBox>&nbsp;
            <asp:Button ID="btnRemoteConnect" runat="server" Text="Remote Connect" />
            
            &nbsp;
            <asp:Button ID="btnRemoteDisconnect" runat="server" Text="Remote Disconnect" />
            
            <br />
            <asp:Label ID="Label11" runat="server" Font-Size="Medium" ForeColor="Red" Font-Bold="True"></asp:Label>
            
            
            
            <br />
            <asp:GridView ID="GridViewConnections" runat="server"  AutoGenerateColumns="true" AllowPaging="True" AllowSorting="True" BackColor="WhiteSmoke" Font-Names="Arial" Font-Size="Small" PageSize="3">
                <AlternatingRowStyle BackColor="#f0f0f0" />
                <RowStyle BackColor="White" />
               <%-- <Columns>
                  <asp:BoundField DataField="Indx" DataFormatString="<a href='InstallIt.aspx?email=send&indx={0}'>email</a>" HtmlEncode="False" />
                </Columns>--%>
            </asp:GridView>
            <br />
            <div align="center">
                <asp:Label ID="Label6" runat="server" Font-Bold="True" Font-Size="Larger" ForeColor="Gray" Text="Unit Administration"></asp:Label>
            </div>
            <div align="left">
                <table>
                    <tr height="30px">
                        <td Width="30%" align="left" style="font-weight: bold; color: #ffffff; font-family: Arial; background-color: maroon; font-size:small;">
                            <asp:Label ID="Label7" runat="server" ForeColor="White" Text="Search:"></asp:Label>
                            <asp:TextBox ID="SearchText" runat="server" width="150px"></asp:TextBox>
                            <asp:Button ID="ButtonSearch" runat="server" CssClass="ticketbutton" Text="Search" valign="center" Visible="true" />
                        </td>
                        
                        <td align="center" width="20%">
                            <asp:LinkButton ID="btnUnits" runat="server" TabIndex="-1" ToolTip="Units" NavigateUrl="~/UnitsAdmin.aspx">Units</asp:LinkButton>
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Label ID="Label8" runat="server" Text=" units"></asp:Label>
                        </td>
                       <%-- <td Width="15%" align="center">                         
                       
                        </td>--%>
                        <td align="center" width="20%">
                            <asp:LinkButton ID="btnUnitRegistration" runat="server" TabIndex="-1" ToolTip="New Unit Registration.">new unit and/or db registration</asp:LinkButton>
                        </td>
                        <td align="center" width="15%">
                           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/SiteAdmin.aspx" Enabled="False" Visible="False">all users</asp:HyperLink>
                        </td>
                        <td align="center" width="20%">
                            <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/UserDefinition.aspx?crsuper=yes" Enabled="False" Visible="False">new super user</asp:HyperLink>
                        </td>
                    </tr>
                </table>
            </div>
            <asp:GridView ID="GridViewUnits" runat="server"  AutoGenerateColumns="true" AllowPaging="True" AllowSorting="True" BackColor="WhiteSmoke" Font-Names="Arial" Font-Size="Small" PageSize="3" Enabled="False" Visible="False">
                <AlternatingRowStyle BackColor="#f0f0f0" />
                <RowStyle BackColor="White" />
                <Columns>
                <%--<asp:BoundField DataField="Indx" HtmlEncode="False" DataFormatString="<a target='_blank' href='UserDefinition.aspx?indx={0}'>edit</a>" />--%>
                    <asp:BoundField DataField="Indx" DataFormatString="<a href='UnitDefinition.aspx?indx={0}'>edit</a>" HtmlEncode="False" />
                </Columns>
            </asp:GridView>
            <br />
                                     <div align="left" backcolor="#f0f0f0" style="background-color: #800000; font-family: Arial, Helvetica, sans-serif; font-size: medium; font-weight: bold; color: #FFFFFF;">
                                         &nbsp;&nbsp;&nbsp; For Advanced Super Admins: &nbsp;&nbsp;Change number of user paid days&nbsp;&nbsp;<%--&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
                                     </div>
                                          <p>
                                          </p>
                                          <asp:Label ID="Label13" runat="server" Font-Bold="False" Font-Names="Arial" Font-Size="X-Small" ForeColor="Black" Text="To add paid time run: paid.aspx?amt=3000&amp;cm=demo&amp;tx=super&amp;st=Completed"></asp:Label>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                     <div id="divSpam" runat="server" style="display:;">
                                      <div align="left" backcolor="#f0f0f0" style="background-color: #800000; font-family: Arial, Helvetica, sans-serif; font-size: medium; font-weight: bold; color: #FFFFFF;">
                                         &nbsp;&nbsp;&nbsp; Spam removal: &nbsp;&nbsp;Move email ip adresses to spam&nbsp;&nbsp;
                                     </div>
                                          <p>
                                               <asp:HyperLink ID="HyperLink6" runat="server" NavigateUrl="~/ourspam.aspx" Enabled="True" Visible="True" Font-Size="Medium">open list of emails to move to spam or undo it</asp:HyperLink>
                                          </p>
                                     </div>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <div align="left" backcolor="#f0f0f0" style="background-color: #800000; font-family: Arial, Helvetica, sans-serif; font-size: medium; font-weight: bold; color: #FFFFFF;">
                                              &nbsp;&nbsp;&nbsp; For Advanced Super Admins: &nbsp;&nbsp;Create the clone to whole system: OUReports and OURdbs&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                          </div>
                                          <p align="left">
                                              Old site folder in wwwroot:
                                              <asp:TextBox ID="TextBoxSourceFolder" runat="server" Width="140px"></asp:TextBox>
                                              &nbsp; &nbsp; New site abbreviation:
                                              <asp:TextBox ID="TextBoxSiteAbr" runat="server" Width="40px"></asp:TextBox>
                                              &nbsp; &nbsp; New site for:
                                              <asp:TextBox ID="TextBoxSiteFor" runat="server" Width="140px"></asp:TextBox>
                                              &nbsp; &nbsp; New site admin email:
                                              <asp:TextBox ID="txtCloneEmail" runat="server" Width="140px"></asp:TextBox>
                                              &nbsp; &nbsp;<asp:Button ID="ButtonClone" runat="server" Text="Clone to [OUReports &amp; new abbreviation]" Width="300px" />
                                              <br />
                                              <asp:Label ID="Label25" runat="server" Font-Size="Medium" ForeColor="#CC0000" Text="Result:"></asp:Label>
                                              <br />
                                          </p>
                                     <div align="left" backcolor="#f0f0f0" style="background-color: #800000; font-family: Arial, Helvetica, sans-serif; font-size: medium; font-weight: bold; color: #FFFFFF;">
                                         &nbsp;&nbsp;&nbsp; For Advanced Super Admins: &nbsp;&nbsp; Install/Update OURdb&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                     </div>
                                     <br />
                                     Create new OURdb in the same server or update existing one:
                                     <p align="left" style="font-size: small">
                             OURdb provider:&nbsp;
                             <asp:DropDownList ID="DropDownListDBProv" runat="server" Font-Size="Smaller" AutoPostBack="True">
                                 <asp:ListItem></asp:ListItem>
                                 <asp:ListItem Value="MySql.Data.MySqlClient">MySQL</asp:ListItem>
                                 <asp:ListItem Value="System.Data.SqlClient">SQL Server</asp:ListItem>
                                 <asp:ListItem Value="Npgsql">PostgreSQL</asp:ListItem>
                                 <asp:ListItem Value="InterSystems.Data.CacheClient">Intersystems Cache</asp:ListItem>
                                 <asp:ListItem Value="InterSystems.Data.IRISClient">Intersystems IRIS</asp:ListItem>                          
                                 <asp:ListItem Value="Oracle.ManagedDataAccess.Client">Oracle</asp:ListItem>
                             </asp:DropDownList>
                             &nbsp;&nbsp;&nbsp;&nbsp;OURdb Connection string:&nbsp;&nbsp;
                             <asp:TextBox ID="TextBoxDBConn" runat="server" Width="40%" AutoPostBack="True"></asp:TextBox>
                                          &nbsp; <asp:CheckBox ID="chkUserDBcase" runat="server" Font-Size="Smaller" Text="use double quote syntax for PostGreSQL db" Visible="False" />
                                         &nbsp;<asp:Button ID="btnUpdateOURdb" runat="server" Text="Update OURdb to current version" Width="300px" />
                                 &nbsp;  
                                         &nbsp;<asp:Button ID="btnCreateOURdbOnNewserver" runat="server" Text="Create very first empty OURdb on new Server" Width="300px" />
                                         &nbsp; or click &nbsp; 

                                          <asp:HyperLink ID="HyperLink7" runat="server" NavigateUrl="~/Default.aspx?payourdbst=Completed" Enabled="True" Visible="True" Font-Size="Medium">create or update current ourdb Default.aspx?payourdbst=Completed</asp:HyperLink>


                                 <br /><br />
                                         Update OURdb above from version:<asp:TextBox ID="TextBoxFromVersion" runat="server" Width="40px" AutoPostBack="True"></asp:TextBox>
                                         &nbsp;to new version:<asp:TextBox ID="TextBoxToVersion" runat="server" Width="40px" AutoPostBack="True"></asp:TextBox>:
                                         &nbsp;<asp:Button ID="btnUpdateToVersion" runat="server" Text="Update OURdb to new version" Width="300px" />

                            </p>
                                     <br />
                                     <asp:Label ID="Label19" runat="server" Font-Bold="True" Font-Overline="True" Font-Size="Medium" ForeColor="#990000"></asp:Label>
                                     <br />
                                       <div align="left" backcolor="#f0f0f0" style="background-color: #800000; font-family: Arial, Helvetica, sans-serif; font-size: medium; font-weight: bold; color: #FFFFFF;">
                                         &nbsp;&nbsp;&nbsp; For Advanced Super Admins: &nbsp;&nbsp; Clean up the user database, Assign Forein Keys&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                     </div>

                                     <p align="left" style="font-size: small">
                             OurDB provider:&nbsp;
                             <asp:DropDownList ID="DropDownListOurdb" runat="server" Font-Size="Smaller">
                                 <asp:ListItem></asp:ListItem>
                                 <asp:ListItem Value="MySql.Data.MySqlClient">MySQL</asp:ListItem>
                                 <asp:ListItem Value="System.Data.SqlClient">SQL Server</asp:ListItem>
                                 <asp:ListItem Value="Npgsql">PostgreSQL</asp:ListItem>
                                 <asp:ListItem Value="InterSystems.Data.CacheClient">Intersystems Cache</asp:ListItem>
                                 <asp:ListItem Value="InterSystems.Data.IRISClient">Intersystems IRIS</asp:ListItem>                          
                                 <asp:ListItem Value="Oracle.ManagedDataAccess.Client">Oracle</asp:ListItem>
                             </asp:DropDownList>
                             &nbsp;&nbsp;&nbsp;&nbsp;OurDB Connection string:&nbsp;&nbsp;
                             <asp:TextBox ID="TextBoxOurdbConnStr" runat="server" Width="66%"></asp:TextBox>

                                      <p align="left" style="font-size: small">
                             UserDB provider:&nbsp;
                             <asp:DropDownList ID="DropDownListUserDB3" runat="server" Font-Size="Smaller">
                                 <asp:ListItem></asp:ListItem>
                                 <asp:ListItem Value="MySql.Data.MySqlClient">MySQL</asp:ListItem>
                                 <asp:ListItem Value="System.Data.SqlClient">SQL Server</asp:ListItem>
                                 <asp:ListItem Value="Npgsql">PostgreSQL</asp:ListItem>
                                 <asp:ListItem Value="InterSystems.Data.CacheClient">Intersystems Cache</asp:ListItem>
                                 <asp:ListItem Value="InterSystems.Data.IRISClient">Intersystems IRIS</asp:ListItem>                          
                                 <asp:ListItem Value="Oracle.ManagedDataAccess.Client">Oracle</asp:ListItem>
                             </asp:DropDownList>
                             &nbsp;&nbsp;&nbsp;&nbsp;User Connection string:&nbsp;&nbsp;
                             <asp:TextBox ID="TextBoxUserConnStr" runat="server" Width="66%"></asp:TextBox>
                                          
                             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;                               
                                                   
                                     <p align="left">
                                          &nbsp;<asp:Button ID="ButtonCleanOURDB" runat="server" Text="Clean OUR Database" Width="200px" ToolTip="Clean ourusertables and reports without tables" /> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                         <asp:Label ID="Label12" runat="server" Font-Size="Small" ForeColor="Black">Report (ID or Title):</asp:Label>&nbsp; 
                                         <asp:TextBox ID="TextBoxRep" runat="server" Width="400px" ToolTip="If ReportId LIKE text, put % around the value"></asp:TextBox>&nbsp;&nbsp;                                          
                                         <asp:Button ID="ButtonDeleteReports" runat="server" Text="Delete Report(s) with such ReportId" Width="300px" />&nbsp;&nbsp;&nbsp;
                                         <asp:Button ID="ButtonDeleteReportsTtl" runat="server" Text="Delete Report(s) with such Report Title" Width="300px" />&nbsp;&nbsp;&nbsp;
                                         <asp:Button ID="btnForeinKeys" runat="server" Text="Update Forein Keys" Width="200px" />
                                       </p>
                                     <p align="left">
                                         <%--<asp:CheckBox ID="CheckBoxCSV" runat="server" Checked="True" />--%>
                                          &nbsp;<asp:Label ID="Label14" runat="server" Font-Size="Small" ForeColor="Black">User:</asp:Label>&nbsp;                                          
                                        <asp:TextBox ID="TextBoxLogon" runat="server" Width="400px" ToolTip="If Table Name LIKE text, put % around the value"></asp:TextBox>&nbsp;&nbsp;                                     
                                         &nbsp;<asp:Label ID="Label9" runat="server" Font-Size="Small" ForeColor="Black">Table Name:</asp:Label>&nbsp;                                          
                                        <asp:TextBox ID="TextBoxTableName" runat="server" Width="400px" ToolTip="If Table Name LIKE text, put % around the value"></asp:TextBox>&nbsp;&nbsp;                                          
                                        <asp:Button ID="ButtonDeleteTables" runat="server" Text="Clean User Database - deleting tables" Width="300px" /> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;                                      
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>



             <div align="left" BackColor="#f0f0f0" style="background-color: #800000; font-family: Arial, Helvetica, sans-serif; font-size: medium; font-weight: bold; color: #FFFFFF;">
                &nbsp;&nbsp;&nbsp; For Advanced Super Admins: &nbsp;&nbsp; Update Unit2 from Unit1&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            </div>
            <p align="left" style="font-size: small" >
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;
                 <br />
                 Copy from Unit1:&nbsp; &nbsp;&nbsp;&nbsp;&nbsp; Unit1:&nbsp;&nbsp;
                <asp:DropDownList ID="ddUnit1" runat="server" AutoPostBack="True" Font-Size="Smaller" />
                &nbsp;&nbsp;&nbsp;&nbsp;
                                     
                 <p align="left" style="font-size: small">
                     Unit1 OurDB provider:&nbsp;&nbsp;
                     <asp:DropDownList ID="ddOURConnPrv1" runat="server" Font-Size="Smaller" >
                             <asp:ListItem></asp:ListItem>
                             <asp:ListItem Value="MySql.Data.MySqlClient">MySQL</asp:ListItem>
                             <asp:ListItem Value="System.Data.SqlClient">SQL Server</asp:ListItem>
                             <asp:ListItem Value="Npgsql">PostgreSQL</asp:ListItem>
                             <asp:ListItem Value="InterSystems.Data.CacheClient">Intersystems Cache</asp:ListItem>
                             <asp:ListItem Value="InterSystems.Data.IRISClient">Intersystems IRIS</asp:ListItem>                          
                             <asp:ListItem Value="Oracle.ManagedDataAccess.Client">Oracle</asp:ListItem>
                     </asp:DropDownList>
                     &nbsp;&nbsp;&nbsp;&nbsp; Connection string:&nbsp;&nbsp;
                     <asp:TextBox ID="txtOURdb1" runat="server" Width="65%"></asp:TextBox>
                     &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                     <p align="left" style="font-size: small">
                        Unit1 DataDB provider:&nbsp;
                         <asp:DropDownList ID="ddUserConnPrv1" runat="server" Font-Size="Smaller">
                             <asp:ListItem></asp:ListItem>
                             <asp:ListItem Value="MySql.Data.MySqlClient">MySQL</asp:ListItem>
                             <asp:ListItem Value="System.Data.SqlClient">SQL Server</asp:ListItem>
                             <asp:ListItem Value="Npgsql">PostgreSQL</asp:ListItem>
                             <asp:ListItem Value="InterSystems.Data.CacheClient">Intersystems Cache</asp:ListItem>
                             <asp:ListItem Value="InterSystems.Data.IRISClient">Intersystems IRIS</asp:ListItem>                          
                             <asp:ListItem Value="Oracle.ManagedDataAccess.Client">Oracle</asp:ListItem>
                         </asp:DropDownList>
                         &nbsp;&nbsp;&nbsp;&nbsp; Connection string:&nbsp;&nbsp;
                         <asp:TextBox ID="txtUserdb1" runat="server" Width="65%"></asp:TextBox>
                         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                         <p align="left" style="font-size: small">
                             To another Unit2:&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Unit2:&nbsp;&nbsp;
                             <asp:DropDownList ID="ddUnit2" runat="server" AutoPostBack="True" Font-Size="Smaller" />
                             &nbsp;&nbsp;&nbsp;&nbsp;
                         </p>
                         <p align="left" style="font-size: small">
                            Unit2 OurDB provider:&nbsp;&nbsp;
                             <asp:DropDownList ID="ddOURConnPrv2" runat="server" Font-Size="Smaller">
                                 <asp:ListItem></asp:ListItem>
                                 <asp:ListItem Value="MySql.Data.MySqlClient">MySQL</asp:ListItem>
                                 <asp:ListItem Value="System.Data.SqlClient">SQL Server</asp:ListItem>
                                 <asp:ListItem Value="Npgsql">PostgreSQL</asp:ListItem>
                                 <asp:ListItem Value="InterSystems.Data.CacheClient">Intersystems Cache</asp:ListItem>
                                 <asp:ListItem Value="InterSystems.Data.IRISClient">Intersystems IRIS</asp:ListItem>                          
                                 <asp:ListItem Value="Oracle.ManagedDataAccess.Client">Oracle</asp:ListItem>
                             </asp:DropDownList>
                             &nbsp;&nbsp;&nbsp;&nbsp; Connection string:&nbsp;&nbsp;
                             <asp:TextBox ID="txtOURdb2" runat="server" Width="66%"></asp:TextBox>
                             <br />
                            
                         </p>
                         <p align="left" style="font-size: small">
                            Unit2 DataDB provider:&nbsp;
                             <asp:DropDownList ID="ddUserConnPrv2" runat="server" Font-Size="Smaller">
                                 <asp:ListItem></asp:ListItem>
                                 <asp:ListItem Value="MySql.Data.MySqlClient">MySQL</asp:ListItem>
                                 <asp:ListItem Value="System.Data.SqlClient">SQL Server</asp:ListItem>
                                 <asp:ListItem Value="Npgsql">PostgreSQL</asp:ListItem>
                                 <asp:ListItem Value="InterSystems.Data.CacheClient">Intersystems Cache</asp:ListItem>
                                 <asp:ListItem Value="InterSystems.Data.IRISClient">Intersystems IRIS</asp:ListItem>                          
                                 <asp:ListItem Value="Oracle.ManagedDataAccess.Client">Oracle</asp:ListItem>
                             </asp:DropDownList>
                             &nbsp;&nbsp;&nbsp;&nbsp; Connection string:&nbsp;&nbsp;
                             <asp:TextBox ID="txtUserdb2" runat="server" Width="66%"></asp:TextBox>
                             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                             <p align="left" style="font-size: small">
                                 &nbsp;<asp:Button ID="btnCopyReports" runat="server" Text="Copy reports from OURdb1 to OURdb2" Width="321px" />
                                 &nbsp;&nbsp;&nbsp;
                                 &nbsp;<asp:Button ID="btnCopyDashboards" runat="server" Text="Copy dashboards from OURdb1 to OURdb2" Width="321px" />
                             </p>
                             <p align="left" style="font-size: small">
                                 &nbsp;&nbsp;&nbsp;&nbsp;
                                 <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Overline="True" Font-Size="Medium" ForeColor="#990000"></asp:Label>
                             </p>
                             <p align="left" style="font-size: small">
                                 &nbsp;</p>
                             <p align="left" style="font-size: small">
                                 <table align="left">
                                     <tr align="left" >
                                         <td align="left" >
                                             OUR selected tables:&nbsp;&nbsp; <%--<br />--%> 
                                             <asp:CheckBox ID="CheckBoxSelectAllOURTables" runat="server" AutoPostBack="True" Font-Size="Small" Text="select all OUR Tables" />
                                             &nbsp;&nbsp;
                                             <asp:CheckBox ID="CheckBoxUnselectAllOURTables" runat="server" AutoPostBack="True" Font-Size="Small" Text="unselect all OUR Tables" />
                                 
                                         </td>
                                         
                                     </tr>
                                     <tr align="left" >
                                         <td align="left" >
                                             <uc1:DropDownColumns ID="ddOURTables" runat="server" BorderColor="Silver" BorderWidth="1" ClientIDMode="Predictable" DropDownHeight="190px" FontName="arial" FontSize="Small" ForeColor="Black" tabindex="0" Width="250px" />
                                             <br /><br /><br />
                                             <asp:Button ID="btnUpdateOURTables" runat="server" Text="Update data in OUR selected tables from Unit1 OURdb to Unit2 OURdb" Width="473px" />
                                         </td>
                                         
                                     </tr>
                                 </table> 
                                     
                                 </p>
                                
                              <p align="left" style="font-size: small">
                                 &nbsp;&nbsp;&nbsp;&nbsp;
                                 <asp:Label ID="Label16" runat="server" Font-Bold="True" Font-Overline="True" Font-Size="Medium" ForeColor="#990000"></asp:Label>
                             </p>
                             <p align="left" style="font-size: small">
                                 &nbsp;</p>
                              <p align="left" style="font-size: small">
                                 &nbsp;&nbsp;&nbsp;&nbsp;
                                 <asp:Label ID="Label17" runat="server" Font-Bold="True" Font-Overline="True" Font-Size="Medium" ForeColor="#990000"></asp:Label>
                             </p>
                             <p align="left" style="font-size: small">
                                 &nbsp;</p>
                             
                             
                             <p align="left" style="font-size: small">
                                 &nbsp;</p>
                                     <br />
                                     <div align="left" backcolor="#f0f0f0" style="background-color: #800000; font-family: Arial, Helvetica, sans-serif; font-size: medium; font-weight: bold; color: #FFFFFF;">
                                         &nbsp;&nbsp;&nbsp; For Advanced Super Admins: &nbsp;&nbsp; Run SQL in OURdb&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                     </div>
                                     <p align="left">
                                         SQL statement (only for OUR database):
                                         <br />
                                         <asp:TextBox ID="TextBoxSQL" runat="server" Height="60px" TextMode="MultiLine" Width="1196px"></asp:TextBox>
                                         &nbsp;<br /> &nbsp;<asp:Button ID="ButtonPrepareSQL" runat="server" Text="Prepare" Width="172px" />
                                     </p>
                                     <p align="left">
                                         <asp:Label ID="Label3" runat="server" Font-Size="Medium" ForeColor="Black">Records affected:</asp:Label>
                                         <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AllowSorting="True" BackColor="WhiteSmoke" EnableSortingAndPagingCallbacks="True" Font-Names="Arial" Font-Size="Small" PageSize="30">
                                             <AlternatingRowStyle BackColor="WhiteSmoke" />
                                             <RowStyle BackColor="White" />
                                         </asp:GridView>
                                     </p>
                                     <p align="left">
                                         <asp:Button ID="ButtonRunSQL" runat="server" Text="Run SQL" Width="172px" />
                                         <br />
                                         <asp:Label ID="Label4" runat="server" Font-Size="Larger" ForeColor="Red"> </asp:Label>
                                     </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                          <p>
                                          </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                     <p>
                                     </p>
                                 </p>
                             </p>
                         </p>
                     </p>
                </p>
                                     
             </p>                         
            </div>
             <asp:Label ID="LabelReportID" runat="server" Font-Bold="False" Font-Names="Arial" Font-Size="XX-Small" ForeColor="Black" Text=" "></asp:Label>
           </ContentTemplate>
       </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpInstall">
                <ProgressTemplate >
                <div class="modal">
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
