<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ScheduledReports.aspx.vb" Inherits="ScheduledReports" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Scheduled Reports</title>
    
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
        <asp:UpdatePanel ID="udpSchedReps" runat ="server">
          <ContentTemplate>

    <div style="text-align: center">
        <table id="tblLinks" runat="server" border="0" width="100%">
            <tr>
                <td align="left" width="10%">
                    <a href="ScheduledReports.aspx">Refresh</a>
                </td>
                <td align="left" width="15%">
                    <a href="RunScheduledReports.aspx">Run Scheduled Reports</a>
                </td>
                <td width="10%"> <strong>  <asp:Label ID="Label1" runat="server" Text=""></asp:Label>  </strong></td>
                <td width="40%">
                    &nbsp;&nbsp;&nbsp;&nbsp
                     <asp:HyperLink ID="HyperLinkPrev" runat="server" NavigateUrl="~/ScheduledReports.aspx?mnth=prev" BackColor="#99CCFF"><-previous</asp:HyperLink>
                    &nbsp;&nbsp                 
                    
                    <strong><strong><asp:Label ID="Label3" runat="server" Text="" Width="250px">Scheduled Reports</asp:Label> </strong></strong>
                     &nbsp;&nbsp                     
                     <asp:HyperLink ID="HyperLinkNext" runat="server" NavigateUrl="~/ScheduledReports.aspx?mnth=next" BackColor="#99CCFF">next-></asp:HyperLink>
                     &nbsp;&nbsp;&nbsp;&nbsp

                </td>
                <td width="15%">
                    <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/ListOfReports.aspx">List of Reports</asp:HyperLink>
                </td>
                <td align="right" width="10%" style="padding-right: 15px">
                    <a href="Default.aspx">Log Off</a> &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;  <%--<a href="TaskListSetting.aspx">Setting</a>--%>
                </td>
            </tr>
        </table>
        <%--<br />--%>
        <table id="SchedReps" runat="server" border="1"  width="100%" rules="rows" style="font-size: x-small;
            color: black; font-family: Arial; background-color: #ffffff;"  bgcolor="#666633 ">             
    
            <tr style= "display: none">
                <td colspan="11" style="font-weight: bold; color: #ffffff; font-family: Arial; height: 10px;
                    background-color: #666633">
                </td>
            </tr>
           <tr height="30px">
                <td align="left" colspan="4" style="font-weight: bold; color: #ffffff; font-family: Arial; height: 30px;
                    background-color: Gray; font-size:small;">
                    <div >
                      Search: 
                      <asp:Label ID="Label2" runat="server" ForeColor="white"></asp:Label>
                      &nbsp;&nbsp
                     <asp:TextBox ID="FirstLetters" runat="server" Visible="true" width="200px"></asp:TextBox>
                     <asp:Button ID="ButtonSearch" runat="server" CssClass="ticketbutton" Text="Search" Visible="true" />
                    </div>    
                </td>
               
                <td align="left" colspan="3" style="font-weight: bold; color: #ffffff; font-family: Arial; height: 12px;
                    background-color: Gray">&nbsp;
                     
                      Scheduled: 
                      <asp:Label ID="MessageLabel" runat="server" ForeColor="white"></asp:Label>
                      &nbsp;&nbsp
                    
                    <asp:CheckBox id="chkHowTo" runat="server" Text="Knowledge base" AutoPostBack="True" Enabled="False" Visible="False" />&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:CheckBox id="ckNotDoneOnly" runat="server" Text="Not Done Only" AutoPostBack="True"  Enabled="False" Visible="False"/>
                    <asp:Button id="btnAddTicket" runat="server" CssClass="ticketbutton" Text="Add Ticket" Enabled="False" Visible="False"/>
                    &nbsp;                         
                </td>
                <td align="right"  colspan="4"  style="font-weight: bold; color: #ffffff; font-family: Arial; height: 30px;
                    background-color: Gray; font-size:small;">
                      &nbsp;&nbsp
                     
                </td>
            </tr>
             <tr style= "display: none">
                <td colspan="11" style="font-weight: bold; color: #ffffff; font-family: Arial; height: 10px;
                    background-color: #666633">
                </td>
            </tr>
            <tr id="trHeader2" style="font-weight: bold; font-size: small; color: white;
                    font-family: Arial; height: 27px; background-color: #666633" >
                <td align="left" valign="top" nowrap="nowrap" style="font-weight: bold; font-size: small; color: white;
                    font-family: Arial; height: 27px; background-color: #666633" width="20px">
                    #
                </td>
                <td align="left" valign="top" width="80px" style="background-color: #666633" >
                    Report Id</td>
                <td align="left" valign="top" style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                    height: 27px; background-color: #666633; width: 40px;">
                    Run on date</td>
                
                <td align="left"  valign="top" Width="200px" style="height: 27px; background-color: #666633; width: 120px;">
                    Report</td>
                <td align="left"  valign="top" style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                    background-color: #666633; " Width="80px">
                    Status</td>
                <td align="left"  valign="top" style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                    background-color: #666633" Width="100px">
                    Parameters and Filters</td>
                <td align="left" valign="top" Width="70px" style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                    height: 27px; background-color: #666633">
                    Emal to:</td>
                <td align="left" valign="top" Width="70px" style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                    height: 27px; background-color: #666633">
                    Delete:</td>
                <td align="left" valign="top" Width="70px" style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                    height: 27px; background-color: #666633">
                    Delete all:</td>
                <td align="center" valign="top" Width="70px" style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                    height: 27px; background-color: #666633">
                    Reccurence Id:</td>
                <td align="left" valign="top" Width="70px" style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                    height: 27px; background-color: #666633">
                    Download:</td>
            </tr>
            
           
        </table>
    </div>
    <%--<asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Size="Larger" ForeColor="Red"
            Text="_" Width="415px"></asp:Label>--%>
            <TABLE CELLSPACING=0 CELLPADDING=0 BORDER=0 WIDTH=100%>
        <TR>
        
        <td style="height: 19px">&nbsp; &nbsp;&nbsp; &nbsp; &nbsp;</td>
        <td style="height: 19px"><% If Session("admin") = "admin" Then%>
        &nbsp;&nbsp; 
        <%End if%></td>
        <td align="right" style="height: 19px">
        &nbsp;
        </td>
        </TR>
        </TABLE>
    <ucmsgbox:msgbox id="MessageBox" runat ="server" > </ucmsgbox:msgbox>
    <ucDlgTicket:DlgTicket id="dlgTicket" runat="server" FontName="Tahoma" FontSize="12px" />
   </ContentTemplate>
  </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpSchedReps">
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
