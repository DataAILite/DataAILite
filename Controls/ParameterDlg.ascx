<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ParameterDlg.ascx.vb" Inherits="Controls_ParameterDlg" %>
<%@ Register src="~/Controls/msgbox.ascx" tagname="Msgbox" tagprefix="uc4" %>

<script type="text/javascript" src="Controls/Javascripts/ParameterDlg.js"></script>

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
.td1 {
    width:10%;
    text-align:right;
    vertical-align: top;
    padding-right :3px;
    height: 26px;
}
.td2 {
    width:90%;
    text-align:left;
    height: 26px;
}
#divInput {
    font-family: Arial; 
    font-size: 14px; 
    width: 100%;
}
.txtComments {
    height:150px;
    width:100%;
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
<asp:UpdatePanel ID="udpParamDlg" runat="server" UpdateMode="Conditional" RenderMode="Inline">
    <ContentTemplate>
     <asp:Button ID="btnD" runat="server" Text="" Style="display: none" Width="0" Height="0" />
     <asp:Button ID="btnD2" runat="server" Text="" Style="display: none" Width="0" Height="0" />
        <asp:Panel ID="pnlBody" runat="server" CssClass="dlgbody" Style="display: none " >
            <asp:Panel ID="pnlHeader" runat="server" CssClass="dlgheader">
                <asp:Label ID="lblHeader" runat="server" Text="Dialog Text"></asp:Label>
            </asp:Panel>
            <%--put inputs here--%>
            <asp:HiddenField ID="hdnField" runat="server"/>
            <asp:HiddenField ID="hdnSQL" runat="server"/>
            <asp:HiddenField ID="hdnIsCacheDB" value="0" runat="server"/>
            <asp:HiddenField ID="hdnIsOracleDB" Value="0" runat="server"/>
            <div id="divInput" align="center" runat="server" >
                <table id="tblInput" style="width: 98%" >
                    <tr>
                        <td class="td1">
                            <asp:Label ID="lblField" runat="server" Text="Field:"></asp:Label>
                        </td>
                        <td class="td2">
                            <asp:DropDownList ID="ddField" runat="server" Width="100%" ></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="td1">
                            <asp:Label ID="lblLabel" runat="server" Text="Label:"></asp:Label>
                        </td>
                        <td class="td2">
                            <asp:TextBox ID="txtLabel" runat="server" Width="50%"></asp:TextBox>
                        </td>
                    </tr>
                   <tr>
                        <td class="td1">
                            <asp:Label ID="lblParameter" runat="server" Text="Parameter"></asp:Label>
                        </td>
                        <td class="td2">
                            <asp:TextBox ID="txtParameter" runat="server" Width="50%"></asp:TextBox>
                        </td>
                    </tr>
                   <tr id="trType" runat="server" style="display:inline;">
                        <td class="td1">
                            <asp:Label ID="lblType" runat="server" Text="Type:"></asp:Label>
                        </td>
                        <td class="td2">
                            <asp:DropDownList ID="ddType" runat="server" Width="50%"></asp:DropDownList>
                        </td>
                    </tr>
                   <tr id="trSQL" runat="server" style="display:inline;">
                        <td class="td1">
                            <asp:Label ID="lblSQL" runat="server" Text="SQL:"></asp:Label>
                        </td>
                        <td >
                            <asp:TextBox ID="txtSQL" runat="server" CssClass="txtComments" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                   <tr style="display:none;">
                        <td class="td1" >
                            <asp:Label ID="lblComments" runat="server" Text="Comments:"></asp:Label>
                        </td>
                        <td >
                            <asp:TextBox ID="txtComments" runat="server" TextMode="MultiLine" CssClass="txtComments"></asp:TextBox>
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

<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpParamDlg">
    <ProgressTemplate >
    <div class="modal">
        <div class="center">
           <asp:Image ID="imgProgress" runat="server"  ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />
           Please Wait...
       </div>
    </div>

    </ProgressTemplate>
</asp:UpdateProgress>