<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TaskList.aspx.vb" Inherits="TaskList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Task List</title>
    
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
            width: 152px;
        }
        .auto-style2 {
            height: 27px;
            width: 165px;
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
        <asp:UpdatePanel ID="udpHelpDesk" runat ="server">
          <ContentTemplate>

    <div style="text-align: center">
        <table id="tblLinks" runat="server" border="0" width="100%">
            <tr>
                <td width="7%" aligh="left">
                    <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/default.aspx">Home</asp:HyperLink></td>
                <td width="7%" aligh="left">
                    <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="https://OUReports.net/OUReports/TaskList.pdf" Target="_blank">Help</asp:HyperLink></td>
                <td align="center" width="7%">
                  <%--  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
                    <asp:LinkButton ID="btTopic" runat="server" CssClass="NodeStyle" Font-Names="Tahoma" Font-Size="12px" TabIndex="-1" ToolTip="Register new topic for the team. Only for Team Admin." Enabled="False" Visible="False">Register new topic</asp:LinkButton>
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/TaskListCalendar.aspx" Enabled="True" Visible="True">Calendar</asp:HyperLink>
                </td>
                <td width="7%" aligh="left">
                    <asp:HyperLink ID="HyperLink5" runat="server" NavigateUrl="~/TaskListTimeLine.aspx">Time Line</asp:HyperLink></td>
                <td width="10%"> <strong>  <asp:Label ID="Label1" runat="server" Text=""></asp:Label>  </strong></td>
                <td width="20%"> <strong><asp:Label ID="Label3" runat="server" Text=""></asp:Label> </strong></td>
                
                <td width="20%">
                     &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
           <asp:HyperLink ID="HyperLink6" runat="server" NavigateUrl="~/ListOfReports.aspx">List of Reports</asp:HyperLink>
                    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/TeamAdmin.aspx" Enabled="False" Visible="False">Team Members and Topics</asp:HyperLink>
                </td>
                <td align="right" width="10%" style="padding-right: 15px">
                    <asp:HyperLink ID="LogOff" runat="server" NavigateUrl="Default.aspx">Log Off</asp:HyperLink>
                    &nbsp; &nbsp;&nbsp; &nbsp; &nbsp;  <a href="TaskListSetting.aspx">Setting</a>
                </td>
            </tr>
        </table>
        <%--<br />--%>
        <table id="HelpDesk" runat="server" border="1"  width="100%" rules="rows" style="font-size: x-small;
            color: black; font-family: Arial; background-color: #ffffff;"  bgcolor="#666633 ">
             
             <tr id="trInput" style= "display: none">
                <td align="left" valign="top" nowrap="nowrap" style="font-weight: bold; font-size: small; color: white;
                    font-family: Arial; height: 23px; background-color: #666633" >
                    #
                </td>
                <td align="left" style="font-weight: bold; font-size: small; color: white;
                    font-family: Arial; background-color: #666633" valign="top" >
                    Date:<br /><asp:TextBox ID="TextDate" runat="server" ></asp:TextBox></td>
                <td align="left"  style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                    height: 23px; background-color: #666633; width: 100px;" valign="top">
                    From:<br /><asp:DropDownList ID="DropDownListWho" runat="server" >
                    </asp:DropDownList>&nbsp;
                    <asp:Button ID="ButtonAddWho" runat="server" Text="add" />
                    <asp:Label ID="LabelWho" runat="server" Height="30px" Width="10px"></asp:Label><br>
                    </td>                    
                <td align="left" style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                    height: 23px; background-color: #666633; " valign="top" >
                    Problem / bug:<br /><asp:TextBox ID="TextTopics" runat="server" Height="53px" 
                        TextMode="MultiLine" ></asp:TextBox></td> 
                <td align="left" style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                    background-color: #666633; " valign="top" >
                    Status:<br /><asp:TextBox ID="TextDecisions" runat="server" Height="13px" ToolTip="asap, !, done, soon, eventually"></asp:TextBox></td>
                <td align="left"  style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                    background-color: #666633" valign="top" >
                    Comments:<br /><asp:TextBox ID="TextComments" runat="server" Height="53px" 
                        TextMode="MultiLine" ></asp:TextBox><br />
                    Attach:<br />
                    <input id="FileO" type="file"  runat="server" 
                        style="width: 480px"/> <asp:Button ID="ButtonAttach" runat="server" Text="Attach" /></td>     
                <td align="left"  style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                    height: 23px; background-color: #666633" valign="top" >
                    To:<br /><asp:DropDownList ID="DropDownListWhom" runat="server" >
                    </asp:DropDownList>&nbsp;
                    <asp:Button ID="ButtonAddWhom" runat="server" Text="add to email list" /><br />
                    <asp:Label ID="LabelWhom" runat="server" Height="53px"></asp:Label><br>
                    <asp:Button ID="ButtonAddAssignment" runat="server" Text="Submit Ticket" 
                        Font-Bold="True" Font-Size="Medium" ForeColor="#990000"  />
                </td>
                 <td></td>
            </tr>

           <tr height="30px" >
               <td align="left" colspan="4" style="font-weight: bold; color: #ffffff; font-family: Arial; height: 30px;
                    background-color: Gray; font-size:small;">
                    <div >
                      Tasks: 
                      <asp:Label ID="MessageLabel" runat="server" ForeColor="white"></asp:Label>
                      &nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp&nbsp;&nbsp
                   
                      Topic: 
                      <asp:DropDownList id="ddTopics" runat="server" ForeColor="black" AutoPostBack="True"></asp:DropDownList>
                      &nbsp;&nbsp
                    </div>    
                </td>
               <td align="left" colspan="2" style="font-weight: bold; color: #ffffff; font-family: Arial; height: 10px;
                    background-color: Gray">&nbsp;
                   <asp:LinkButton ID="btnDown" runat="server" CssClass="ticketbutton" Text="Download" Visible="true" />
               </td>
                <td align="right" colspan="2" style="font-weight: bold; color: #ffffff; font-family: Arial; height: 10px;
                    background-color: Gray">
                     <div >
                      Search: 
                      <asp:Label ID="Label2" runat="server" ForeColor="white"></asp:Label>
                      &nbsp;&nbsp
                     <asp:TextBox ID="FirstLetters" runat="server" Visible="true" width="200px" AutoPostBack="True"></asp:TextBox>
                     <asp:Button ID="ButtonSearch" runat="server" CssClass="ticketbutton" Text="Search" Visible="true" />
                     &nbsp;&nbsp
                    <asp:CheckBox id="chkHowTo" runat="server" Text="Knowledge base" AutoPostBack="True" />&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:CheckBox id="ckNotDoneOnly" runat="server" Text="Not Done Only" AutoPostBack="True" />
                    <asp:Button id="btnAddTicket" runat="server" CssClass="ticketbutton" Text="Add Task"/>
                    </div>
                    <%--&nbsp;--%>                         
                </td>
            </tr>
            <tr id="trHeader2" style="font-weight: bold; font-size: small; color: white;
                    font-family: Arial; height: 27px; background-color: #666633" >
                <td align="left" valign="top" nowrap="nowrap" style="font-weight: bold; font-size: small; color: white;
                    font-family: Arial; height: 27px; background-color: #666633" width="20px">
                    #
                </td>
                <td align="left" valign="top" width="20px" style="font-weight: bold; font-size: small; color: white;
                    font-family: Arial; background-color: #666633" >
                    Version</td>
                <td align="left" valign="top" style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                    height: 27px; background-color: #666633; width: 20px;">
                    Start</td>
                <td align="left" valign="top" Width="35px" style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                    height: 27px; background-color: #666633">
                    Deadline</td>
                <td align="left"  valign="top" Width="300px" style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                    height: 27px; background-color: #666633; width: 300px;">
                    <asp:Label ID="Label4" runat="server" ForeColor="white" Text="Problem" ToolTip="Task"></asp:Label></td>
                <td align="left"  valign="top" style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                    background-color: #666633; " Width="50px">
                    Status</td>
                <td align="left"  valign="top" style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                    background-color: #666633" Width="400px">
                    Comments</td>
                <td align="left" valign="top" Width="40px" style="font-weight: bold; font-size: small; color: white; font-family: Arial;
                    height: 27px; background-color: #666633">
                    Email to</td>
                
                
            </tr>
            <tr style= "display: none">
                <td colspan="8" style="font-weight: bold; color: #ffffff; font-family: Arial; height: 10px;
                    background-color: #666633">
                </td>
            </tr>
        </table>
    </div>

    <ucmsgbox:msgbox id="MessageBox" runat ="server" > </ucmsgbox:msgbox>
    <ucDlgTicket:DlgTicket id="dlgTicket" runat="server" FontName="Tahoma" FontSize="12px" />
   </ContentTemplate>
  </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpHelpDesk">
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
