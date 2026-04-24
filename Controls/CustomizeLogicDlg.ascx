<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CustomizeLogicDlg.ascx.vb" Inherits="Controls_CustomizeLogicDlg" %>
<%@ Register src="~/Controls/msgbox.ascx" tagname="Msgbox" tagprefix="uc4" %>

<link href="Controls/css/treeview.css" rel="stylesheet" type="text/css" />
<link href="Controls/css/ContextMenu.css" rel="stylesheet" type="text/css" />
<link href="Controls/css/Dialog.css" rel="stylesheet" type="text/css" />

<script src="Controls/Javascripts/json2.js" type="text/javascript"></script>
<script src="Controls/Javascripts/TreeView.js" type="text/javascript"></script>
<script src="Controls/Javascripts/ContextMenu.js" type="text/javascript"></script>
<script src="Controls/Javascripts/CustomizeLogicDlg.js" type="text/javascript"></script>

<style type="text/css">
#tblInput {
    width:98%;
}
.td {
    width:32%
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

<asp:UpdatePanel ID="udpCustDlg" runat="server" UpdateMode="Conditional" RenderMode="Inline">
    <ContentTemplate>
     <asp:HiddenField ID="hdnJson" runat="server"/>
     <asp:HiddenField ID="hdnGroupCount" runat="server" Value="0" />
     <asp:HiddenField ID="hdnConditionCount" runat="server" Value="0" />
     <asp:HiddenField ID="hdnDeletedData" runat="server" Value="" />

     <asp:Button ID="btnD" runat="server" Text="" Style="display: none" Width="0" Height="0" />
     <asp:Button ID="btnD2" runat="server" Text="" Style="display: none" Width="0" Height="0" />
        <asp:Panel ID="pnlBody" runat="server" CssClass="dlgbody" Style="display: none ; font-family:Tahoma; font-size:14px;" >
            <asp:Panel ID="pnlHeader" runat="server" CssClass="dlgheader">
                <asp:Label ID="lblHeader" runat="server" Text="Dialog Text"></asp:Label>
            </asp:Panel>
            <%--put inputs here--%>
             <div id="divInput" align="center" runat="server" >
                <table id="tblInput" >
                    <tr >
                        <td id="td1" style="padding-top: 3px; " >
                          <div id="divTreeView" style="border: solid 1px gray; height:350px; padding-left:5px; background-color:white; overflow:scroll; width:450px;" tabindex="-1" oncontextmenu="return showMenu(event)" >
                             Report Conditions <br />
                          </div>                            
                        </td>
                        <td id="td2" style="padding-top: 3px; vertical-align:top;">
                            <table >
                                <tr>
                                    <td style="text-align:left; vertical-align:top;">
                                        <fieldset style="border: thin solid #808080; width: 130px;">
                                            <legend>Logical Operators</legend>
                                            <div style="text-align:center">
                                                <asp:RadioButton ID="rbAnd" runat="server" Text="And" GroupName="logical" Checked="true" />
                                                <asp:RadioButton ID="rbOr" runat="server" Text="Or" GroupName="logical" />
                                            </div>
                                        </fieldset>
                                    </td>
                                    <td >
                                        &nbsp;                           
                                    </td>
                                    <td >
                                        <asp:Button ID="btnAddGroup" runat="server" CssClass="dlgboxbutton" Text="Add Group" CausesValidation="false" />
                                        <br />
                                        <asp:Button ID="btnUngroup" runat="server" CssClass="dlgboxbutton" Text="Ungroup" CausesValidation="false" />
                                    </td>
                                </tr>
                                <tr >
                                    <td style="text-align:left;">
                                       <asp:Button ID="btnUp" style= "width:22px;height:21px; padding:0px; margin-left:3px; margin-bottom:2px; background-position:center center; background-image:url(Controls/Images/arrow-up-gray.png)" runat="server" CssClass="dlgboxbutton" Text="" CausesValidation="false" ToolTip="Move node up one row" />
                                        <br />
                                       <asp:Button ID="btnDown" style= "width:22px; height:21px; padding:0px; margin-left:3px; margin-top:0px; background-position:center center; background-image:url(Controls/Images/arrow-down-gray.png)" runat="server" CssClass="dlgboxbutton" Text="" CausesValidation="false" ToolTip="Move field down one row" />
                                    </td>
                                    <td >&nbsp</td>
                                    <td >
                                        &nbsp
                                    </td>
                                </tr>
                           </table>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="pnlMenu"  style="display:none" class="skin0" onMouseover="highlight(event)" onMouseout="lowlight(event)" onClick="jumpto(event)"  >
                <div id="RenameGroup" class="menuitems">Rename Group</div>
                <div id="Ungroup" class="menuitems">Ungroup</div>
            </div>
            <div class="divButtons" >
               <asp:Button ID="btnOK" runat="server" CssClass="dlgboxbutton" Text="OK" CausesValidation="false" />
               <asp:Button ID="btnCancel" runat="server" CssClass="dlgboxbutton" Text="Cancel" CausesValidation="false" />
            </div> 
        </asp:Panel>
        <uc4:Msgbox id="MessageBox" runat ="server" />
        <ajaxToolkit:ModalPopupExtender ID="popDlg" runat="server" TargetControlID="btnD"
            PopupControlID="pnlBody" PopupDragHandleControlID="pnlHeader" BackgroundCssClass="dlgbackground"
            DropShadow="false" OkControlID="btnD2" >
        </ajaxToolkit:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>

<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpCustDlg">
    <ProgressTemplate >
      <div class="modal" >
        <div class="center">
           <asp:Image ID="imgProgress" runat="server"  ImageAlign="AbsMiddle" ImageUrl="~Controls/Images/WaitImage2.gif" />
           Please Wait...
       </div>
    </div>
    </ProgressTemplate>
</asp:UpdateProgress>

<script type="text/javascript">
    var prefix="dlgCustomizeLogic_"
    var isLoaded = false;
    var TreeView1 = new TreeView();
    var deletedGroups = { Groups: [] }

    //TreeView1.SelectedNodeChanged = function (event) {
    //    selectedNodeChanged(event);
    //}
    //TreeView1.BeforeLabelEdit = function (event) {
    //    return true;
    //}

    //TreeView1.AfterLabelEdit = function (event) {
    //    TreeView1.EditLabel = false;
    //    var node = TreeView1.SelectedNode;
    //    if (node != null) {
    //        var cond = node.ConditionData;
    //        if (cond.GroupName != "") {
    //            cond.GroupName = node.Text;
    //            if (node.ChildNodes.length > 0) {
    //                for (var i = 0; i < node.ChildNodes.length; i++) {
    //                    cond = node.ChildNodes[i].ConditionData;
    //                    if (cond != null)
    //                        cond.ContainedBy = node.Text;
    //                }
    //            }
    //        }
    //    }
        

    //    return true;
    //}

    function populateTreeView(data) {
        if (data != null && !isLoaded) {
            data = data.replace(/\\/g, "");
           TreeView.ParseJSON(JSON.parse(data), TreeView1);
           isLoaded = true;
           TreeView1.ShowLines(true);
        }
   }
</script>
