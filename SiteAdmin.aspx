<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SiteAdmin.aspx.vb" Inherits="SiteAdmin" %>
<%@ Register TagPrefix="uc1" TagName="DropDownColumns" Src="Controls/uc1.ascx" %>

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
         .auto-style3 {
             width: 61%;
             height: 50px;
         }
         .auto-style4 {
             width: 100%;
         }
         .auto-style7 {
             width: 481px;
         }
         .auto-style8 {
             height: 30px;
             width: 53%;
         }
    </style>
   
</head>
<body>
    <form id="form1" runat="server">
         <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" /> 
         <asp:UpdatePanel ID="udpSiteAdmin" runat ="server" >
        <ContentTemplate>
<div align="left">
     <table width="100%">
      <tr>
           <td width="100%" style="font-size:x-large; font-style:normal; font-weight:bold; background-color: #e5e5e5; vertical-align:middle; text-align: left; height: 40px;">
              <asp:Label ID="LabelPageTtl" runat="server" Text=""></asp:Label>
          </td>
      </tr> 
         </table>
</div>

<%--
        <div align="center">
              
          </div>--%>
         <div  align="left">              
            <br />
            <asp:Label ID="Label1" runat="server" Text="Site Administration" Font-Size="Larger" Font-Bold="true" ForeColor="Gray"></asp:Label>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
           <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/ListOfReports.aspx">List of Reports</asp:HyperLink>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:HyperLink ID="HyperLinkTaskList" runat="server" NavigateUrl="~/TaskList.aspx" >Task List</asp:HyperLink> 
        
            
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/Default.aspx">Log off</asp:HyperLink>
            <table class="auto-style3">
            <tr >
                <td align="left"  valign="top" style="font-weight: bold; color: #ffffff; font-family: Arial; background-color: Gray; font-size:small;" class="auto-style8">
                    <div class="auto-style4" >   
                      Groups:&nbsp; 
                      <asp:DropDownList id="ddGroups" runat="server" ForeColor="black" AutoPostBack="True">
                          <asp:ListItem>All</asp:ListItem>
                        </asp:DropDownList>
                      &nbsp;&nbsp;   
                        Tables in group:&nbsp;&nbsp; &nbsp;&nbsp; <asp:LinkButton ID="btTables" runat="server" TabIndex="-1" ToolTip="list of tables in group">save tables in group</asp:LinkButton> 
                        
                       <br /> <uc1:DropDownColumns ID="DropDownGroupTables" runat="server" Width="250px" ClientIDMode="Predictable" FontName="arial" FontSize="Small" BorderColor="Red" ForeColor="Black" BorderWidth="1" DropDownHeight="190px" DropDownButtonHeight="22px" TextBoxHeight="22px" />
                        <%--<asp:LinkButton ID="btUsers" runat="server" TabIndex="-1" ToolTip="refresh list of users" Enabled="False" Visible="False">users in group</asp:LinkButton>--%>
                          
                    </div>    
                    </td>
                    <td align="left"  valign="top" Width="50%" style="font-weight: bold; color: #ffffff; font-family: Arial; height: 30px;
                    background-color: Gray; font-size:small;">
                    <div class="auto-style4" > 
                        <asp:Label ID="lbNewGroup" runat="server" ForeColor="White" Text="New group:"></asp:Label>
                        &nbsp;   <asp:TextBox ID="txtGroup" runat="server" Visible="true" width="200px"></asp:TextBox>
                      &nbsp;&nbsp;          
                        <asp:LinkButton ID="btAddGroup" runat="server" TabIndex="-1" ToolTip="Add new group in unit">add</asp:LinkButton>
                    </div>
                    </td>
            </tr>
        </table>
        </div>
        <div >     
            <Table>
                <tr height="30px">
                    <td align="left" style="font-weight: bold; color: #ffffff; font-family: Arial; background-color: Gray; font-size:small;" class="auto-style7" >
                        <asp:Label ID="Label2" runat="server" ForeColor="White" Text="Search:"></asp:Label>
                         &nbsp;&nbsp
                        <asp:TextBox ID="SearchText" runat="server" Visible="true" width="259px"></asp:TextBox>
                        <asp:Button ID="ButtonSearch" runat="server" CssClass="ticketbutton" Text="Search" Visible="true" valign="center"/>
                    </td>
                    <td  align="center">                        
                       <strong>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Users in group:</strong> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;  
                        <asp:LinkButton ID="btnRegistration" runat="server" TabIndex="-1" ToolTip="New user Registration.">new user registration</asp:LinkButton>
                    </td>
                   
                </tr>
            </Table>
                     
        </div> 
      
        <asp:GridView ID="GridView1" runat="server" AllowSorting="True" BackColor="WhiteSmoke" Font-Names="Arial" Font-Size="Small" AllowPaging="True" PageSize="30">
            <AlternatingRowStyle BackColor="#f0f0f0" />
            <RowStyle BackColor="White" />   
            <Columns>
                <%--<asp:BoundField DataField="Indx" HtmlEncode="False" DataFormatString="<a target='_blank' href='UserDefinition.aspx?indx={0}'>edit</a>" />--%>
                 <asp:BoundField DataField="Indx" HtmlEncode="False" DataFormatString="<a href='UserDefinition.aspx?indx={0}'>edit</a>" />
            </Columns>
        </asp:GridView>

            <div align="left">
                  <table class="auto-style3">
            <tr >
                <td align="left"  valign="top" style="font-weight: bold; color: #ffffff; font-family: Arial; background-color: Gray; font-size:small;" class="auto-style8">
                    <div class="auto-style4" >   
                     Site Banner:&nbsp; 
                         <asp:TextBox ID="txtSiteBanner" runat="server" Visible="true" width="400px"></asp:TextBox>
                        &nbsp;&nbsp;&nbsp;&nbsp;
                         <asp:TextBox ID="txtDescript" runat="server" Visible="true" width="400px"></asp:TextBox>
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="ButtonSubmit" runat="server" Text="Submit" />
                    </div>    
                    </td>
            </tr>
        </table>
            </div>
            <div align="left" BackColor="#f0f0f0" style="background-color: #800000; font-family: Arial, Helvetica, sans-serif; font-size: medium; font-weight: bold; color: #FFFFFF;">
                &nbsp;&nbsp;&nbsp; For Advanced Admins: &nbsp;&nbsp; Update Unit2 from Unit1&nbsp;&nbsp;
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
                                 <asp:Label ID="Label3" runat="server" Font-Bold="True" Font-Overline="True" Font-Size="Medium" ForeColor="#990000"></asp:Label>
                             </p>
                
            </p>

                       <ucMsgBox:Msgbox id="MessageBox" runat ="server" > </ucMsgBox:Msgbox>
             </ContentTemplate>
      </asp:UpdatePanel>   
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpSiteAdmin">
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
