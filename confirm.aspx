<%@ Page Language="VB" AutoEventWireup="false" CodeFile="confirm.aspx.vb" Inherits="confirm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script type="text/javascript" src="Controls/Javascripts/OUR.js"></script>

    <title>confirm</title>
<style type="text/css">
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
    opacity: 1;
}
 </style>   
</head>
<body>

    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />  
        
<asp:UpdatePanel ID="udpConfirm" runat ="server">
    <ContentTemplate>
<table border="0" cellpadding="0" cellspacing="0" width="100%">
  <tr>
    <td style=" text-align:left; font-size: x-large; font-style: normal; font-weight: bold; background-color: #e5e5e5; vertical-align: middle; height: 40px;">
        <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label>
         <%--<h1><asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label></h1>--%>
    </td>
  </tr>
  <tr>
     <td align="center" valign="top">
       <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx">Log on</asp:HyperLink>            
     </td>
 </tr>
  <tr id="trMsg" runat="server" visible="false">
    <td align="center" valign="top" height="25px">
       <span style ="font-family:Tahoma;font-size:14px;font-weight:bold;color:Red" >
          <asp:Label ID="LblMsg" runat="server"  Height="25px" Width="800px"></asp:Label>
       </span>
    </td>
 </tr>
  <tr>
    <td align="center" >
        <h3>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Change Password and/or Registration</h3>       
    </td>
  </tr>
 <tr>
     <td align="center" valign="top" style="height: 144px">
         <table ID="Regist" runat="server" border="0" cellpadding="2" cellspacing="0" width="80%">
           <tr >
             <td align="right" width="450px">
               <span style ="font-family:Tahoma;font-size:12px; font-weight: bold; ">
                   &nbsp;Logon:&nbsp;&nbsp;
               </span>
             </td>
             <td align ="left">                              
                <asp:TextBox runat="server" ID="txtLogin" class="auto-style1"/>
             </td>
          </tr>
          <tr >
             <td align="right" width="450px">
               <span style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: #FF0000;">
                   &nbsp;Current Password*:&nbsp;&nbsp;
               </span>
             </td>
             <td align ="left">                
               <asp:TextBox runat="server" ID="txtCurrent" class="auto-style1"/>                
             </td>
          </tr>
          <tr >
             <td align="right" width="450px">
               <span style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: #FF0000;">
                   &nbsp;New Password*:&nbsp;&nbsp;
               </span>
             </td>
             <td align ="left">               
               <asp:TextBox ID="txtNew" runat="server" type="password" required="required"  class="auto-style1"/>
             </td>
          </tr>
          <tr >
             <td align="right" width="450px">
               <span style ="font-family:Tahoma;font-size:12px; font-weight: bold; color: #FF0000;">
                   &nbsp;Repeat Password*:&nbsp;&nbsp;
               </span>
             </td>
             <td align ="left">                 
               <asp:TextBox ID="txtRepeat" runat="server" type="password" required="required"  class="auto-style1"/>
             </td>
          </tr>
          <tr style="border-color:#ffffff">
                                  <td align="right" class="auto-style2" style="font-family: tahoma; font-size:12px; font-weight: bold; color: #FF0000;">
                                      <%-- &nbsp;Organization*:&nbsp;&nbsp;--%>

                                  </td>
                                  
                                  <td align ="left" class="auto-style1">
                                      <asp:TextBox runat="server" ID="txtUnit" class="auto-style6"/>
                                  </td>
                             </tr>
                             <tr style="border-color:#ffffff">
                                  <td align="right" class="auto-style2" style="font-family: tahoma; font-size:12px; font-weight: bold; color: #FF0000;">
                                       Email*:&nbsp;&nbsp;
                                  </td>
                                  
                                  <td align ="left" class="auto-style1">
                                       <asp:TextBox runat="server" ID="txtEmail" style="width: 347px" required="required"/>
                                  </td>
                             </tr>
                            <tr style="border-color:#ffffff">
                                  <td align="right" class="auto-style2" style="font-family: tahoma; font-size:12px; font-weight: bold; color: #FF0000;">
                                       User Database provider*:&nbsp;&nbsp;
                                  </td>
                                 
                                  <td align ="left" class="auto-style1">
                                       <asp:DropDownList runat="server" Font-Size="Smaller" ID="dropdownDatabases" AutoPostBack="True" >
                                           <asp:ListItem Selected="True" Value="System.Data.SqlClient">SQL Server</asp:ListItem>                                           
                                           <asp:ListItem Value="MySql.Data.MySqlClient">MySQL</asp:ListItem>
                                           <asp:ListItem Value="Npgsql">PostgreSQL</asp:ListItem>
                                           <asp:ListItem Value="Oracle.ManagedDataAccess.Client">Oracle</asp:ListItem>
                                           <asp:ListItem Value="InterSystems.Data.CacheClient">Intersystems Cache</asp:ListItem>
                                           <asp:ListItem Value="InterSystems.Data.IRISClient">Intersystems IRIS</asp:ListItem>
                                       </asp:DropDownList>
                                  </td>
                             </tr>
                             <tr style="border-color:#ffffff">
                                  <td align="right" class="auto-style2" style="font-family: tahoma; font-size:12px; font-weight: bold; color: #FF0000;">
                                       User Connection String*:&nbsp;&nbsp;
                                  </td>
                                  
                                  <td align ="left" class="auto-style1">
                                       <asp:TextBox ID="txtConnStr" runat="server" Width="810px"></asp:TextBox>
                                  </td>
                             </tr>
                             
                            <tr style="border-color:#ffffff">
                                  <td align="right" class="auto-style2">
                                       <span style ="font-family:Tahoma;font-size:12px;">&nbsp;&nbsp;</span>
                                  </td>
                                  
                                  <td align ="left" class="auto-style1" style="font-family: Arial; font-size: x-small">
                                       
                                      <asp:CheckBox ID="CheckBox1" runat="server" Font-Size="Smaller" Text="Save Connetion Info for future use (password will not be saved for security reason and will be requested during login)" Checked="True" Visible="False" />
                                       
                                      &nbsp;<br />
                                      <asp:Label ID="lblConnection" runat="server" Text="Server=yourserver; Database=yourdatabase; User ID=youruserid; Password=yourpassword" Visible="False"></asp:Label>
                                      <br />
                                      <asp:Label ID="lblPassWord" runat="server" Text="(password to database will not be saved for security reasons and will be requested during login)"> </asp:Label> 
                                      <br />

                                  </td>
                             </tr>
          <tr >
             <td align="center" colspan="2">
                 <br />
                 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;                  
                  <asp:Button ID="btnSubmit" runat="server" Text="Change" Width="131px" />
             </td>

          </tr>
         </table> 
     </td>
 </tr>
</table>    
 
   <ucMsgBox:Msgbox id="MessageBox" runat ="server" > </ucMsgBox:Msgbox> 
</ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpConfirm">
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
