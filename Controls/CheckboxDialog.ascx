<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CheckboxDialog.ascx.vb" Inherits="Controls_CheckboxDialog" %>
<%@ Register tagPrefix="ucMBox" src="~/Controls/msgbox.ascx" tagName="MsgBox" %>
<style type="text/css">
.dlgbody
{
	background-color: #e6eefa ;
	border-width: 1px;
	border-style: solid;
	border-color: darkGray;
	padding: 3px;
    width:550px;
    /*height:400px;*/
}
.dlgheader
{
	/*background-color: #a01919;*/
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
.divCheckboxList {
    border: 1px solid ;
    display: block;
    border-color:darkgray;
    overflow-y:scroll;
    position: relative;
    background-color:white;
    max-height:250px;
    width:100%;
    /*height:250px;*/
    z-index: auto;
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
  /*background-color: ButtonFace;*/
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
<asp:UpdatePanel ID="udpCheckboxDlg" runat="server" UpdateMode="Conditional" RenderMode="Inline">
    <ContentTemplate>
     <asp:Button ID="btnD" runat="server" Text="" Style="display: none" Width="0" Height="0" />
     <asp:Button ID="btnD2" runat="server" Text="" Style="display: none" Width="0" Height="0" />
        <asp:Panel ID="pnlBody" runat="server" CssClass="dlgbody" Style="display: none " >
            <asp:Panel ID="pnlHeader" runat="server" CssClass="dlgheader">
                <asp:Label ID="lblHeader" runat="server" Text="Dialog Text"></asp:Label>
            </asp:Panel>
            <div id="divPrompt" runat ="server" style="padding: 6px 6px 6px 3px; font-family: Tahoma; font-size: 12px; font-weight: normal;" >
                <asp:Label id="lblPrompt" runat="server" Text="List Prompt>"></asp:Label>
            </div>
            <div id="DivChecklist" class=divCheckboxList runat="server" style="font-family: Tahoma; font-size: 12px">
                <asp:CheckBoxList ID="Checklist" runat="server" >
                </asp:CheckBoxList>
            </div>
            <div class="divButtons" >
               <asp:Button ID="btnOK" runat="server" class="dlgboxbutton" Text="OK" CausesValidation="false" />
               <asp:Button ID="btnCancel" runat="server" class="dlgboxbutton" Text="Cancel" CausesValidation="false" />
            </div> 
        </asp:Panel>
        <ucMBox:Msgbox id="Msgbox" runat ="server" />
        <ajaxToolkit:ModalPopupExtender ID="popDlg" runat="server" TargetControlID="btnD"
            PopupControlID="pnlBody" PopupDragHandleControlID="pnlHeader" BackgroundCssClass="dlgbackground"
            DropShadow="false" OkControlID="btnD2" >
        </ajaxToolkit:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpCheckboxDlg">
    <ProgressTemplate >
    <div class="modal">
        <div class="center">
           <asp:Image ID="imgProgress" runat="server"  ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />
           Please Wait...
       </div>
    </div>

    </ProgressTemplate>
</asp:UpdateProgress>