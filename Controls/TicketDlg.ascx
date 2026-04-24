<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TicketDlg.ascx.vb" Inherits="Controls_TicketDlg" %>

<%@ Register src="Msgbox.ascx" tagname="Msgbox" tagprefix="uc4" %>
<%@ Register TagPrefix="uc1" TagName="DropDownColumns" Src="uc1.ascx" %>

<script type="text/javascript" src="Controls/Javascripts/TicketDlg.js"></script>

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
    line-height:20px;
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
.lblCal {
    height: 26px;
    padding-top=15px;
}
.td1 {
    width:12%;
    text-align:right;
    /*vertical-align: top;*/
    padding-right :3px;
    height: 26px;
}
.td2 {
    width:88%;
    text-align:left;
    height: 26px;
}

#divInput {
    font-family: Arial; 
    font-size: 14px; 
    width: 100%;
}
.PopupBackground {
    background-color: rgba(158, 188, 250,0.8);
}
.Popup {
    position:absolute;
    width:90%;
    height:320px;
    background-color:#e6eefa ;
    font-family:Arial;
    z-index: 2147483650;
    border:1px solid #222222;
}

.inline {
        display: inline-block;
    }
.txtDescription {
    height:75px;
    width:95%;
}
.txtComments {
    height:100px;
    width:95%;
}
.txtPrevComments {
    font-family: Tahoma; 
    font-size: 10px;
    height:100px;
    width:98%;
}
.OtherText {
    display:none;
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
 .LookupTextBoxStyle {
        padding:0px 0px 0px 0px; 
        margin:0px 0px 0px 0px;
        border-left:1px solid dimgray;
        border-top:1px solid dimgray;
        border-right:1px solid dimgray;
        border-bottom:1px solid dimgray;
        border-top-left-radius:3px;
        border-bottom-left-radius:3px;
        /*width:100%;*/
        height:20px;    
    }
    .LookupButtonStyle{
        width: 22px;
        height: 22px;
        padding:0px;
        margin:0px;
        border-left:hidden;
        border-top:1px solid dimgray;
        border-right:1px solid dimgray;
        border-bottom:1px solid dimgray;
        border-top-right-radius:10%;
        border-bottom-right-radius:10%;
        background-color:ButtonFace;
        z-index: 9999;
    }
    /* The Close Button */
.close {
    color: white;
    float: right;
    font-size: 20px;
    font-weight: bold;
    padding-right:10px;
}

.close:hover,
.close:focus {
    color: blue;
    text-decoration: none;
    cursor: pointer;
}
.clearfix::after {
  content: "";
  clear: both;
  display: table;
}
.column {
  float: left;
  padding: 0px;
}

.buttons {
  width: 19%;
  height:120px;
  text-align:center;
}

.btnClose {
position:absolute;
top:75px;
left:10px;
}

.content {
  width: 80%;
  height:400px;
}

</style>
<asp:UpdatePanel ID="udpTicketDlg" runat="server" UpdateMode="Conditional" RenderMode="Inline">
    <ContentTemplate>
     <asp:Button ID="btnD" runat="server" Text="" Style="display: none" Width="0" Height="0" />
     <asp:Button ID="btnD2" runat="server" Text="" Style="display: none" Width="0" Height="0" />
        <asp:Panel ID="pnlBody" runat="server" CssClass="dlgbody" Style="display: none " >
            <asp:Panel ID="pnlHeader" runat="server" CssClass="dlgheader">
                <asp:Label ID="lblHeader" runat="server" Text="Dialog Text"></asp:Label>
            </asp:Panel>
            <%--put inputs here--%>
            <div id="divInput" align="center" runat="server" >
                <table id="tblInput"  style="border-width: 0px; width: 98%">
                 <tr>
                     <td class="td1">
                         <asp:Label ID="lblTicketNo" runat="server" Text="Ticket No:"></asp:Label>
                     </td>
                    <td class="td2">
                        <asp:Label ID="lblTicketNoVal" runat="server" Text="1"></asp:Label>
                    </td>
                  </tr>
                 <tr>
                     <td class="td1">
                         <asp:Label ID="lblDateTime" runat="server" width="100%" Text="Date Time:"></asp:Label>
                     </td>
                    <td class="td2">
                        <asp:Label ID="lblDateTimeVal" runat="server" Text="2/8/2019 15:05:00"></asp:Label>
                    </td>
                 </tr>
                 <tr>
                     <td class="td1">
                         <asp:Label ID="lblFrom" runat="server" Text="Initiated:"></asp:Label>
                     </td>
                    <td class="td2" >
                           <%-- <asp:DropDownList ID="ddFrom" runat="server" Width="40%" Enabled="False">
                                <asp:ListItem>user</asp:ListItem>
                            </asp:DropDownList>--%>
                        <asp:Label ID="ddFrom" runat="server" Text="user"></asp:Label>
                    </td>
                  </tr>
                     <tr>
                     <td class="td1">
                         <asp:Label ID="lblVersion" runat="server" Text="Version:"></asp:Label>
                     </td>
                    <td class="td2" >
                            <asp:DropDownList ID="ddVersion" runat="server" Width="40%">
                                <asp:ListItem>current</asp:ListItem>
                                <asp:ListItem>next</asp:ListItem>
                                <asp:ListItem>undefined</asp:ListItem>                                
                                <asp:ListItem>all</asp:ListItem>
                                <asp:ListItem>old</asp:ListItem>
                            </asp:DropDownList>
                        &nbsp;Deadline:&nbsp;
                        <input type="date" name="txtDeadline" value="<%=Data.Deadline %>" id="idDdeadline">
                    </td>
                  </tr>
                 <tr>
                     <td class="td1">
                         <asp:Label ID="lblDescription" runat="server" Text="Description:"></asp:Label>
                     </td>
                    <td class="td2">
                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" CssClass="txtDescription"></asp:TextBox>
                    </td>
                 </tr>
                 <tr>
                     <td class="td1">
                         <asp:Label ID="lblStatus" runat="server" Text="Status:"></asp:Label>
                     </td>
                    <td class="td2" >
                        <div id="divStatus" runat="server"  class="inline" style="width: 100%">
                           <asp:DropDownList ID="ddStatus" runat="server" Width="40%" >
                               <asp:ListItem>problem</asp:ListItem>
                               <asp:ListItem>asap</asp:ListItem>
                               <asp:ListItem>bug</asp:ListItem>
                               <asp:ListItem>done</asp:ListItem>
                               <asp:ListItem>deleted</asp:ListItem>
                               <asp:ListItem>dismissed</asp:ListItem>
                               <asp:ListItem>eventually</asp:ListItem>
                               <asp:ListItem>in progress</asp:ListItem>
                               <asp:ListItem>how to</asp:ListItem>
                               <asp:ListItem>knowledge</asp:ListItem>
                               <asp:ListItem>documentation</asp:ListItem>
                               <asp:ListItem>meeting</asp:ListItem>
                               <asp:ListItem>planning</asp:ListItem>
                               <asp:ListItem>redesign</asp:ListItem>
                               <asp:ListItem>known bug</asp:ListItem>
                               <asp:ListItem>testing</asp:ListItem> 
                               <asp:ListItem>soon</asp:ListItem>
                               <asp:ListItem>other</asp:ListItem>
                            </asp:DropDownList>
                            &nbsp;
                            <div id="divOtherStatus" runat="server" class="inline" style="width:50%; padding-bottom: 3px; display: none;">
                              <asp:TextBox ID="txtOtherStatus" runat="server" width="100%"></asp:TextBox>
                            </div>
                        </div>
                    </td>
                  </tr>
                 <tr id="trPreviousComments" runat="server" visible="false">
                    <td class="td1" colspan="2">
                      <div id="divPrevComments" runat="server" style="width: 100%;text-align: center;">
                       <asp:Label ID="lblPrevComments" runat="server" Text=" Previous Comments:" Font-Size="10px" Font-Names="tahoma" Font-Bold="True"></asp:Label>
                       <asp:TextBox ID="txtPrevComments" runat="server" TextMode="MultiLine" CssClass="txtPrevComments" ReadOnly="True" Enabled="True" TabIndex="-1" BorderStyle="None" BackColor="#EEEEEE"></asp:TextBox>
                      </div>
                    </td>
                    <td></td>
                 </tr>
                <tr>
                    <td class="td1">
                            <asp:Label ID="lblComments" runat="server" Text="Comments:"></asp:Label>
                    </td>
                    <td class="td2" >
                        <asp:TextBox ID="txtComments" runat="server" TextMode="MultiLine" CssClass="txtComments"></asp:TextBox>
                    </td>
                 </tr>
                 <tr>
                     <td class="td1">
                         <div id="divLblBreak" runat="server" style="display: none">
                             <br />
                         </div>
                         <asp:Label ID="lblTo" runat="server" Text="Email to:"></asp:Label>
                     </td>
                    <td id="tdSendto" runat ="server"  class="td2" style="padding-bottom: 25px; padding-top: 0px; ">
                        <div id="divNoEmail" runat="server" style="display: none">
                          <asp:CheckBox id="ckNoEmail" runat="server" style="padding:0px;margin:0px;" Text="Don't Email me" Width="100%" TabIndex="-1" ToolTip="Don't Email the logged on user" />
                          <br />
                        </div>
                        <div id="divDDSendTo" runat="server" style="width:200px; display:inline-block;">
                            <uc1:DropDownColumns id="ddSendTo" runat="server" width="200px" DropDownHeight="150px"/>
                        </div>

                            <%--******************************Lookup Textbox**************************--%>
                        <div id="divLUSendto" runat="server" style="width:250px; height:25px; display:none;">
                            <table style="margin: 0px; padding: 0px; border:none; width:100%; table-layout:fixed;">
                              <tr id="trTextbox" runat="server" >
                                <td id="tdTextBox" style="vertical-align: top; padding: 0px 0px 0px 0px; margin: 0px 0px 0px 0px; width: 90%; ">
                                   <asp:TextBox ID="txtLookup" runat="server" CssClass="LookupTextBoxStyle" Width="100%" ></asp:TextBox>
                                </td>
                                <td id="tdBtnLookup" style="vertical-align: top;margin: 0px;padding: 0px; width:10%" runat="server" >
                                    <asp:Button id="btnLookup" runat="server"  CssClass="LookupButtonStyle" Text="..." Font-Bold="True" Font-Size="Medium" TabIndex="-1" ToolTip="Do Lookup" />
                                </td>
                              </tr>
                            </table>
                        </div>
                        <%--*******************************Lookup Dialog******************************--%>
                        <div id="divPopupBackground" runat="server" class="PopupBackground" style="display:none;height:100%;width:100%;position:absolute;top:0px;left:0px;">
                            <div id="divPopup" runat="server" class="Popup"  >
                            <div id="divHeading" style="font-size: small; text-align: center; background-color: gray; width: 100%; height: 22px; line-height:22px; color: white">
                                Send To
                                <span id="spnClose" runat ="server" class="close" title="close dialog">&times;</span>
                            </div>
                            <div id="divPopupBody " class="clearfix" >
                                <div id="divContent" class="column content" runat="server" style="margin-left: 5px;">
                                    <div id="divSearch" runat ="server" style=" display:inline; width: 100%; height: 30px; padding-top: 5px; padding-bottom: 5px; ">
                                        Search:
                                        <asp:Textbox ID="txtSearch" runat="server" Wrap="False" Width="200px"></asp:Textbox>
                                        <asp:Button ID="btnFind" runat="server" CssClass="dlgboxbutton" Text="Find" CausesValidation="false" UseSubmitBehavior="True" />
                                    </div>
                                    <div id="divListHeader" runat="server" style="background-color: darkgray; border: 1px solid #808080; height: 20px; line-height:20px; padding-left: 8px; color: #FFFFFF;">
                                        <asp:label ID="lblListHeader" runat="server" Text="Users"></asp:label>
                                    </div> 
                                    <div id="divList" runat="server" style="border-style: none solid solid solid; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; border-right-color: #808080; border-bottom-color: #808080; border-left-color: #808080; height:225px; overflow-y:scroll;">
                                        <asp:CheckBoxList ID="lstItems" runat="server" Width="100%" BorderStyle="None">
                                            <asp:ListItem></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </div>                                  
                                </div>
                                <div id="divPopupButtons" class="column buttons" >
                                    <asp:Button ID="btnSubmit" runat="server" CssClass="dlgboxbutton" Text="OK" CausesValidation="false" />
                                    <br />
                                    <asp:Button ID="btnClose" runat="server" CssClass="dlgboxbutton btnCancel" Text="Cancel" CausesValidation="false" />
                                </div>
                            </div>
                        </div>
                        </div>
                     </td>
                 </tr>
                 <tr>
                     <td class="td1">
                         <asp:Label ID="lblAttach" runat="server" Text="Attach:"></asp:Label>
                     </td>
                    <td class="td2" style="display: inline">
                      <asp:FileUpload id="FileUpload" runat ="server" CssClass="OtherText" />
                      <asp:Button ID="btnBrowse" runat="server" class="dlgboxbutton" Text="Browse..." CausesValidation="false" width="15%" Height="21px" />
                      <asp:Label ID="lblAttached" runat="server" Text="No file selected. " ForeColor="Blue" width="60%"></asp:Label>
                      <asp:Button ID="btnAttach" runat="server" class="dlgboxbutton" Text="Upload" CausesValidation="false" Width="15%" Height="21px" />
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
    <Triggers><asp:PostBackTrigger ControlID="btnAttach"/></Triggers>
</asp:UpdatePanel>
<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpTicketDlg">
    <ProgressTemplate >
    <div class="modal" >
        <div class="center" >
           <asp:Image ID="imgProgress" runat="server"  ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />
           Please Wait...
       </div>
    </div>
    </ProgressTemplate>
</asp:UpdateProgress>
