<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ConditionDlg.ascx.vb" Inherits="Controls_ConditionDlg" %>

<%@ Register src="Msgbox.ascx" tagname="Msgbox" tagprefix="uc4" %>

<script type="text/javascript" src="Controls/Javascripts/ConditionDlg.js"></script>

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
.lblCal {
    height: 26px;
    padding-top=15px;
}
.td1 {
    width:8%;
    text-align:right;
    /*vertical-align: top;*/
    padding-right :3px;
    height: 26px;
}
.td2 {
    width:40%;
    text-align:left;
    height: 26px;
}
.td3 {
    width:52%;
    text-align:left;
    height: 26px;
}
#divInput {
    font-family: Arial; 
    font-size: 14px; 
    width: 100%;
}
.txtComments {
    height:75px;
    width:100%;
}
.inline {
        display: inline-block;
    }
.ckRelative {
    padding:0px;
}
.padleft {
    padding-left:5px;
}
 .auto-style4 {
        width: 98%;
    }
 .PleaseWait {
            /*position:center;*/
            position:absolute;
            z-index: 2147483647;
            opacity: 0.8;
            overflow: hidden;
            text-align: center;
            /*height: 100%;*/
            /*width: 100%;*/
            height: 128px;
            width: 200px;
            line-height: 32px;
            left: 500px;
            /*top: 80px;*/
            top: 300px;
            /*background-color: #FAFAFA;*/
            background-color: white;
    }


.modal
{
    position: fixed;
    z-index: 2147483647;
    height: 100%;
    width: 100%;
    top: 0;
    background-color: #f8f8d3;
    /*filter: alpha(opacity=60);*/
    opacity: 0.8;
    /*-moz-opacity: 0.8;*/
}
.center
{
    z-index: 2147483647;
    margin: 300px auto;
    padding-left:25px;
    padding-top:10px;
    /*padding: 20px 10px 10px 10px;*/
    width: 130px;
    background-color:#f8f8d3;
    /*background-color: #e6eefa ;*/
    border-radius: 10px;
    /*filter: alpha(opacity=100);*/
    opacity: 1;
}
.center img
{
    height: 100px;
    width: 100px;
}
</style>
<asp:UpdatePanel ID="udpCondDlg" runat="server" UpdateMode="Conditional" RenderMode="Inline">
    <ContentTemplate>
     <asp:Button ID="btnD" runat="server" Text="" Style="display: none" Width="0" Height="0" />
     <asp:Button ID="btnD2" runat="server" Text="" Style="display: none" Width="0" Height="0" />
        <asp:Panel ID="pnlBody" runat="server" CssClass="dlgbody" Style="display: none; " >
            <asp:Panel ID="pnlHeader" runat="server" CssClass="dlgheader">
                <asp:Label ID="lblHeader" runat="server" Text="Dialog Text"></asp:Label>
            </asp:Panel>
            <asp:HiddenField id="hdnMode" runat="server" value="None"/>
            <asp:HiddenField id="hdnBtnText1" runat="server" value="Fields"/>
            <asp:HiddenField id="hdnBtnText2" runat="server" value="Fields"/>
            <asp:HiddenField id="hdnBtnCal1" runat="server" value="Fields"/>
            <asp:HiddenField id="hdnBtnCal2" runat="server" value="Fields"/>
            <asp:HiddenField id="hdnFieldIndex" runat="server" value="0"/>
            <asp:HiddenField id="hdnNext" runat="server"/>
            <asp:HiddenField id="hdnPrev" runat="server"/>
            <asp:HiddenField id="hdnOperator" runat="server" Value="="/>
            <asp:HiddenField id="hdnDateField1" runat="server"/>
            <asp:HiddenField id="hdnDateField2" runat="server"/>
            <asp:HiddenField id="hdnValField1" runat="server"/>
            <asp:HiddenField id="hdnValField2" runat="server"/>
            <%--put inputs here--%>
            <div id="divInput" align="center" runat="server" >
                <table id="tblInput" class="auto-style4" >
                    <tr>
                        <td class="td1" style="padding-top: 3px">
                            <asp:Label ID="lblField" runat="server" Text="Field:"></asp:Label>
                        </td>
                        <td class="td2">
                            <div id="divDDField" class="padleft">
                             <asp:DropDownList ID="ddField" runat="server" Width="195px" ></asp:DropDownList>
                            </div>
                            
                        </td>
                        <td class="td3"> </td>
                    </tr>
                    <tr>
                        <td class="td1" style="padding-top: 3px">
                            <asp:Label ID="lblOperator" runat="server" Text="Operator:"></asp:Label>
                        </td>
                        <td class="td2">
                            <div id="divDDOperator" class="padleft" ">
                               <asp:checkbox ID="ckNot" runat ="server"  Text="Not" ToolTip="Not Operator" TabIndex="-1" TextAlign="Right" />
                               &nbsp;
                               <asp:DropDownList ID="ddOperator" runat="server">
                                    <asp:ListItem>=</asp:ListItem>
                                    <asp:ListItem>&gt;</asp:ListItem>
                                    <asp:ListItem>&lt;</asp:ListItem>
                                    <asp:ListItem>&lt;=</asp:ListItem>
                                    <asp:ListItem>&gt;=</asp:ListItem>
                                    <asp:ListItem>&lt;&gt;</asp:ListItem>
                                    <asp:ListItem>In</asp:ListItem>
                                    <asp:ListItem>between</asp:ListItem>
                                    <asp:ListItem>Contains</asp:ListItem>
                                    <asp:ListItem>StartsWith</asp:ListItem>
                                    <asp:ListItem>EndsWith</asp:ListItem>
                                    <asp:ListItem>IS NULL</asp:ListItem>
                                </asp:DropDownList>
                            </div>
 
                        </td>
                        <td class="td3"> </td>
                    </tr>
                   <tr id="trText" runat="server" height="18px" >
                        <td class="td1">
                            <div id="divValLabels" style="padding-bottom: 10px">
                              <asp:Label ID="lblValue1" runat="server" Text="Value:" ></asp:Label>
                              <asp:Label ID="lblValue2" runat="server" Text="Values:" style="display:none" ></asp:Label>
                            </div>
                        </td>
                        <td class="td2">
                          <div class="inline" id="divValue1" runat="server" >
                             <asp:TextBox ID="txtValue1" runat="server" width="195px" ></asp:TextBox>
                             <asp:DropDownList ID="ddValField1" runat="server" Width="200px" style="display:none;"></asp:DropDownList>
                              <%--&nbsp;--%>
                            <div class="inline" id="divBtnText1" runat="server"  style="padding-bottom: 5px">
                                <asp:Button id="btnText1" class="dlgboxbutton" runat="server" Text="Fields" tabindex="-1" Style="width:50px;" ToolTip="Choose field"/>
                             </div>
                          </div>
                        </td>
                        <td class="td3">
                          <div class="inline" id="divValue2" runat="server" style="display:none" >                             
                            <div id="divAnd1" class="inline" runat="server" style="height: 23px" >
                               <asp:Label ID="lblAnd1" runat="server" >And&nbsp;&nbsp </asp:Label>
                            </div>
                            <asp:TextBox ID="txtValue2" runat="server" width="195px" ></asp:TextBox>
                            <asp:DropDownList ID="ddValField2" runat="server" width="200px" style="display:none"></asp:DropDownList>
                            &nbsp;
                            <div class="inline" id="divBtnText2" runat="server"  style="padding-bottom: 5px">
                                <asp:Button id="btnText2" class="dlgboxbutton" runat="server" Text="Fields" TabIndex="-1" OnClientClick="return false" Style="width:50px;" ToolTip="Choose field"/>
                             </div>
                         </div>
                        </td>
                    </tr>
                   <tr id="trCalendar" runat="server" style="display:none">
                        <td class="td1">
                            <div id="divCalLabels" class="inline" style="padding-top: 10px" >
                               <asp:Label ID="lblCal1" runat="server" Text="Value:"></asp:Label>
                               <asp:Label ID="lblCal2" runat="server" Text="Values:" style="display:none;" ></asp:Label>
                            </div>
                        </td>
                        <td class="td2">
                               <div id="divCalendar1" class="inline" runat="server" align="left" >
                                 <div id="divRelative1" runat ="server" align="left">
                                   <asp:checkbox ID="ckRelative1" runat ="server"  Text="Relative Date" ToolTip="Make date relative" TabIndex="-1" TextAlign="Right" />
                                   <br />
                                 </div>
                                 <div id="divDate" class="inline" runat="server" align="left" style="width: 195px; padding-left: 5px;" >
                                      <asp:TextBox id="Date1" runat="server" Width="100%" ></asp:TextBox>
                                      <ajaxToolkit:CalendarExtender ID="ceDate1" runat ="server"  TargetControlID="Date1" Format="M/d/yyyy" TodaysDateFormat="M/d/yyyy" />
                                 </div>
                               </div>
                               <div id="divDateField1" class="inline" runat="server" style="width:195px;padding-left: 5px;display :none;">
                                   <br />
                                 <asp:DropDownList ID="ddDateField1" runat="server" width="100%"></asp:DropDownList>
                               </div>
                               &nbsp;
                              <div class="inline" id="divBtnCal1" runat="server"  >
                                 <br />
                                <asp:Button id="btnCal1" class="dlgboxbutton" runat="server" Text="Fields" TabIndex="-1" OnClientClick="return false" Style="width:50px;" ToolTip="Choose field"/>
                              </div>
                        </td>
                       <td class="td3">
                         <div id="divCal2" class="inline" runat ="server" style="display:none;" >
                             <div id="divAnd2" class="inline" runat="server" style="height: 18px" >
                               <asp:Label ID="lblAnd2" runat="server" >And&nbsp; </asp:Label>
                             </div>
                             <div id="divCalendar2" class="inline" runat="server" align="left" >
                                 <div id="divRelative2" runat ="server" align="left" >
                                  <asp:checkbox ID="ckRelative2" runat ="server"  Text="Relative Date" ToolTip="Make date relative" TabIndex="-1" TextAlign="Right" />
                                  <br />
                                </div>
                                <div id="divDate2" class="inline" runat="server" align="left" style="width: 195px; padding-left:5px;" >
                                   <asp:TextBox id="Date2" runat  ="server" Width="100%" ></asp:TextBox>
                                   <ajaxToolkit:CalendarExtender ID="ceDate2" runat ="server"  TargetControlID="Date2" Format="M/d/yyyy" TodaysDateFormat="M/d/yyyy" />
                                </div>
                             </div>
                             <div id="divDateField2" class="inline" runat="server" style="width:195px;display:none;">
                               <br />
                               <asp:DropDownList ID="ddDateField2" runat="server" width="100%" ></asp:DropDownList>
                             </div>
                             &nbsp;
                            <div id="divBtnCal2" class="inline" runat="server" >
                              <br />
                              <asp:Button id="btnCal2" class="dlgboxbutton" runat="server" Text="Fields" TabIndex="-1" OnClientClick="return false" Style="width:50px;" ToolTip="Choose field"/>
                            </div>
                         </div>
                           
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
<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpCondDlg">
    <ProgressTemplate >
    <div class="modal">
        <div class="center">
           <asp:Image ID="imgProgress" runat="server"  ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />
           Please Wait...
       </div>
    </div>

    </ProgressTemplate>
</asp:UpdateProgress>

