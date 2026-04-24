<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Msgbox.ascx.vb" Inherits="Controls_Msgbox" %>
<%--<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>--%>
<style type="text/css">
.msgbody
{
	background-color: #e6eefa ;
	border-width: 1px;
	border-style: solid;
	border-color: Gray;
	padding: 3px;
}
.msgheader
{
	/*background-color: #6ee1e5;*/
    background-color: Gray;
	border-color: White;
	border-width: 1px;
	/*color:  darkslateblue;*/
    color:  white;
	font-weight: bold;
    text-align:left;
	width: 100%;
	height: 20px;
}
.msgbackground
{
	background-color: #3753fc;
	filter: alpha(opacity=30);
	opacity: 0.3;
}
.divClose
{
    /*height:30px;*/
    /*padding: 15px;*/
    text-align:center;
	width: 100%;
    /*margin:5px;*/
}
.msgboxbutton 
{
  width:80px;
  height: 25px;
  font-size: 12px;
  border-radius: 5px;
  border-style :solid;
  border-color: #4e4747 ;
  color: black;
  border-width: 1px;
  /*background-color: ButtonFace;*/
  background-image: linear-gradient(to bottom, rgba(158, 188, 250,0),rgba(158, 188, 250,1));
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
    opacity: 1;
}
.center img
{
    height: 100px;
    width: 100px;
}
</style>

<asp:UpdatePanel ID="udpMsgbox" runat="server" UpdateMode="Conditional" RenderMode="Inline">
   <ContentTemplate>
     <asp:Button ID="btnD" runat="server" Text="" Style="display: none" Width="0" Height="0" />
     <asp:Button ID="btnD2" runat="server" Text="" Style="display: none" Width="0" Height="0" />
    <asp:Panel ID="pnlMsg" runat="server" CssClass="msgbody" Style="display: inline " Width="550px">
    <asp:Panel ID="pnlHeader" runat="server" CssClass="msgheader">
        &nbsp
        <asp:Label ID="lblHeader" runat="server" Text="Message"></asp:Label>
    </asp:Panel>
    <asp:GridView ID="grdMsg" runat="server" ShowHeader="false" Width="100%" AutoGenerateColumns="false" BorderWidth="0" EnableTheming="True">
        <Columns>
          <asp:TemplateField>
            <ItemTemplate>
                <table >
                    <tr>
                        <td valign="top">
                         <asp:Image ID="imgErr" runat="server" ImageUrl="~/Controls/Images/picError.Image.png"
                Visible =' <%#IIf(CType(Container.DataItem, Message).Icon = MessageIcon.Error, True, False) %>' />
                         <asp:Image ID="imgWarning" runat="server" ImageUrl="~/Controls/Images/picExclaim.Image.png"
                Visible =' <%#IIf(CType(Container.DataItem, Message).Icon = MessageIcon.Warning, True, False) %>' />
                         <asp:Image ID="imgInfo" runat="server" ImageUrl="~/Controls/Images/picInfo.Image.png"
                Visible =' <%#IIf(CType(Container.DataItem, Message).Icon = MessageIcon.Information, True, False) %>' />
                         <asp:Image ID="imgQuestion" runat="server" ImageUrl="~/Controls/Images/picQuestion.Image.gif"
                Visible =' <%#IIf(CType(Container.DataItem, Message).Icon = MessageIcon.Question, True, False) %>' />
                         <asp:Image ID="imgStop" runat="server" ImageUrl="~/Controls/Images/Stop.bmp"
                Visible =' <%#IIf(CType(Container.DataItem, Message).Icon = MessageIcon.Stop, True, False) %>' />
                        </td>
                        <td style="font-family: sans-serif; font-size: medium">                        
                            <%# Eval("MessageText")%>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
          </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <div class="divClose"  >
                <asp:Button ID="btnOK" runat="server" class="msgboxbutton" Text="OK" CausesValidation="false" />
                <asp:Button ID="btnPostOK" runat="server" class="msgboxbutton" Text="OK" CausesValidation="false" OnClick="btnPostOK_Click"
                    Visible="false"  />
                <asp:Button ID="btnPostYes" runat="server" class="msgboxbutton" Text="Yes" CausesValidation="false"
                    OnClick="btnPostYes_Click" Visible="false" />
                <asp:Button ID="btnPostNo" runat="server" class="msgboxbutton" Text="No" CausesValidation="false"
                    OnClick="btnPostNo_Click" Visible="false" />
                <asp:Button ID="btnPostRetry" runat="server" class="msgboxbutton" Text="Retry" CausesValidation="false"
                    OnClick="btnPostRetry_Click" Visible="false" />
                <asp:Button ID="btnPostAbort" runat="server" class="msgboxbutton" Text="Abort" CausesValidation="false"
                    OnClick="btnPostAbort_Click" Visible="false" />
                <asp:Button ID="btnPostIgnore" runat="server" class="msgboxbutton" Text="Ignore" CausesValidation="false"
                    OnClick="btnPostIgnore_Click" Visible="false" />
                <asp:Button ID="btnPostOther1" runat="server" class="msgboxbutton" Style="width:auto" Text="" CausesValidation="false"
                    OnClick="btnPostOther1_Click" Visible="false" />
                <asp:Button ID="btnPostOther2" runat="server" class="msgboxbutton" Style="width:auto" Text="" CausesValidation="false"
                    OnClick="btnPostOther2_Click" Visible="false" />
                <asp:Button ID="btnPostCancel" runat="server" class="msgboxbutton" Text="Cancel" CausesValidation="false"
                    OnClick="btnPostCancel_Click" Visible="false" />
   </div>
   </asp:Panel>

   <ajaxToolkit:ModalPopupExtender ID="popMsg" runat="server" TargetControlID="btnD"
            PopupControlID="pnlMsg" PopupDragHandleControlID="pnlHeader" BackgroundCssClass="msgbackground"
            DropShadow="false" OkControlID="btnOK" >
   </ajaxToolkit:ModalPopupExtender>
 </ContentTemplate>
</asp:UpdatePanel>

<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpMsgbox">
    <ProgressTemplate >
    <div class="modal">
        <div class="center">
           <asp:Image ID="imgProgress" runat="server"  ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />
           Please Wait...
       </div>
    </div>

    </ProgressTemplate>
</asp:UpdateProgress>