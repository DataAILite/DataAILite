<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TextboxDlg.ascx.vb" Inherits="Controls_TextboxDlg" %>

<%@ Register src="~/Controls/Msgbox.ascx" tagname="Msgbox" tagprefix="uc4" %>

<style type="text/css">
.dlgbody
{
	background-color: #e6eefa ;
	border-width: 1px;
	border-style: solid;
	border-color: darkGray;
	padding: 3px;
    width:550px;
}
.dlgheader
{
    background-color: Gray;
	border-color: White;
	border-width: 1px;
	color:  white;
    font-family:Tahoma;
    font-size :14px;
	font-weight: bold;
	width: 100%;
	height: 20px;
    text-align:center;
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
.divButtons
{
    text-align:center;
	width: 100%;
}
.dlgbackground
{
	background-color: #3753fc;
	filter: alpha(opacity=30);
	opacity: 0.3;
}
.td1 {
    width: 25%;
    text-align:right;
    vertical-align: middle;
    padding-right :3px;
    height: 26px;
}
.td2 {
    width:75%;
    text-align:left;
    height: 26px;
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
<asp:UpdatePanel ID="udpTextboxDlg" runat="server" UpdateMode="Conditional" RenderMode="Inline">
    <ContentTemplate>
     <asp:Button ID="btnD" runat="server" Text="" Style="display: none" Width="0" Height="0" />
     <asp:Button ID="btnD2" runat="server" Text="" Style="display: none" Width="0" Height="0" />
        <asp:Panel ID="pnlBody" runat="server" CssClass="dlgbody" Style="display: none;" >
            <asp:Panel ID="pnlHeader" runat="server" CssClass="dlgheader">
                <asp:Label ID="lblHeader" runat="server" Text="Dialog Text"></asp:Label>
            </asp:Panel>
            <div id="divStartLabel" runat="server" style="vertical-align: middle; padding: 5px 15px 5px 15px; height: auto;">
                 <asp:Label id="lblMain" runat ="server" Text="To create a new report first enter the new Report Title and click the Create button." > </asp:Label>
            </div>
            <div id="divInput" align="center" runat="server" >
                <table id="tblInput" style="width: 98%; " >
                     <tr id="trText1" runat="server" >
                        <td class="td1" >
                            <asp:Label ID="lblText1" runat="server" Text="Report Title:"></asp:Label>
                        </td>
                        <td class="td2">
                            <asp:TextBox ID="txtText1" runat="server" Width="90%" ></asp:TextBox>
                        </td>
                    </tr>
                   <tr id="trText2" runat ="server" >
                        <td class="td1">
                            <asp:Label ID="lblText2" runat="server" Text="Text2:"></asp:Label>
                        </td>
                        <td class="td2">
                            <asp:TextBox ID="txtText2" runat="server" Width="90%"></asp:TextBox>
                        </td>
                    </tr>
                   <tr id="trText3" runat ="server" >
                        <td class="td1">
                            <asp:Label ID="lblText3" runat="server" Text="Text3:"></asp:Label>
                        </td>
                        <td class="td2">
                            <asp:TextBox ID="txtText3" runat="server" Width="90%"></asp:TextBox>
                        </td>
                    </tr>
                           </table>
            </div>
            <div class="divButtons" >
               <asp:Button ID="btnOK" runat="server" class="dlgboxbutton" Text="OK" CausesValidation="false" />
               <asp:Button ID="btnCancel" runat="server" class="dlgboxbutton" Text="Cancel" CausesValidation="false" />
            </div> 
        </asp:Panel>
        <uc4:Msgbox id="MessageBox" runat ="server" />
        <ajaxToolkit:ModalPopupExtender ID="popDlg" runat="server" TargetControlID="btnD"
            PopupControlID="pnlBody" PopupDragHandleControlID="pnlHeader" BackgroundCssClass="dlgbackground"
            DropShadow="false" OkControlID="btnD2" >
        </ajaxToolkit:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpTextboxDlg">
    <ProgressTemplate >
    <div class="modal">
        <div class="center">
           <asp:Image ID="imgProgress" runat="server"  ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />
           Please Wait...
       </div>
    </div>

    </ProgressTemplate>
</asp:UpdateProgress>