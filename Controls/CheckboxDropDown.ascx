<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CheckboxDropDown.ascx.vb" Inherits="Controls_CheckboxDropDown" %>
<script type="text/javascript" src="Scripts/ChecklistDropDown.js"></script>

<style type="text/css">
    .ButtonStyle1 {
        width: 18px;
        height: 18px;
        font-size: 12px;
        border-radius: 10%;
        border-style :solid;
        border-color: dimgray;
        border-width: 1px;
        background-repeat: no-repeat;
        background-position:center;
        background-color:ButtonFace;
        padding:0px;
        margin:0px;
        z-index: 9999;
        /*background-image: url("Images\DDImageDown.bmp");*/
        /*padding: 5px 4px 0px 5px*/
     }
    .SaveButtonStyle {
        height:21px;
        width:70px;
        border: 1px solid dimgray;
        border-radius: 10%;
        padding:0px;
        margin:0px;
    }
    .ButtonDownStyle{
        width: 18px;
        height: 18px;
        font-size: 12px;
        border:hidden;
        padding:0px;
        margin:0px;
        background-color:ButtonFace;
        background-image: url('Controls/Images/arrow-down-black2.bmp');
        background-position:center;
        image-rendering:auto;
        background-repeat: no-repeat;
    }
    .ButtonUpStyle {
        width: 18px;
        height: 18px;
        font-size: 12px;
        border:hidden;
        padding:0px;
        margin:0px;
        background-color:ButtonFace;
        background-image: url('Controls/Images/arrow-up-black.bmp');
        background-position:center;
        image-rendering:auto;
        background-repeat: no-repeat;

    }
    .DropDownPanel {
    }

    .CheckboxListClosed {
        visibility:hidden;
        border: 1px solid;
        border-color:darkgray;
        left: 3px;
        overflow: auto;
        position: relative;
        width: 225px;
        z-index: 9999;
    }
    .CheckboxListOpen {
        visibility:visible;
        border: 1px solid ;
        border-color:darkgray;
        left: 3px;
        overflow: auto;
        position: relative;
        width:225px;
        z-index: 9999;
    }
    .CheckboxList {
        /*border:darkgray 1px solid;*/
        display: none;
        /*border-color:darkgray;*/
        left: 3px;
        overflow-y:scroll;
        position: relative;
        color:black;
        background-color:white;
        /*max-height:100px;*/
        width:100%;
        /*height:400px;*/
        z-index: auto;
    }
    .auto-style1 {
        width: 100%;
        height: 100%;
    }
    .ActionButtonsStyle {
        display:inline-block;
        padding-top:5px;
        padding-left:0px;
        padding-right:0px;
        padding-bottom:0px;
        /*padding:0px;*/
        margin: 0px;
        height: 26px;
        width:100%;
        /*left:3px;*/
        overflow: auto;
        position: relative;
        border:1px solid ;
        border-left:hidden;
        border-right:hidden;
        border-bottom:hidden;
        border-color:darkgray;
        background-color:white;
        z-index: 9999;
    }
</style>

<asp:Panel ID="pnlDropDownCheckList" runat="server"  CssClass="DropDownPanel" height="0px" BackColor="White" HorizontalAlign="Left" Width="300px" ScrollBars="None" >
<table ID="tblDD" style="margin: 0px; padding: 0px; border:hidden; width:100%; table-layout:fixed; " >
    <tr id="rowDD" style="height: 18px" runat="server">
        <td style="vertical-align: top; padding: 0px; margin: 0px; " class="auto-style1"  >
            <asp:TextBox ID="txtValue" runat="server" width=100% style="padding:0px; margin:0px;" Height="18px" AutoPostBack="True" ReadOnly="True"></asp:TextBox>
        </td>
<%--        <td ID="btnDD" class="ButtonStyle1" runat="server" style="vertical-align: top ;margin: 0px;padding: 0px; " background="Images\DDImageDown.bmp" >

        </td>--%>
        <td style="vertical-align: top;margin: 0px;padding: 0px; " class=ButtonStyle1 runat="server"  >
            <asp:Button ID="btnDropDown" runat="server" TabIndex="-1" Font-Bold="True" CssClass="ButtonDownStyle" />
        </td>
    
    </tr>
</table>

<div id="DivChecklist" class=CheckboxList style="border-width:1px; border-style:solid;"  runat="server" >
    <asp:CheckBoxList ID="Checklist" runat= "server" >
    </asp:CheckBoxList>
<%--<table style="margin: 0px; padding: 0px; border:hidden; width:100%" >
<tr id="ActionButtons" runat="server">
   <td class="ActionButtonsStyle" align="center">
        <asp:Button ID="btnCancel" runat="server" CssClass="SaveButtonStyle" Text="Cancel" Width="73px" />
    
   </td>

</tr>
</table>--%>
</div>
<%--<div id="DivAction" class="ActionButtonsStyle" runat="server" visible="false">

    <asp:Button ID="btnCancel" runat="server" CssClass="SaveButtonStyle" Text="Cancel" />

</div>--%>
<%--<table style="margin: 0px; padding: 0px;">
<tr id="ActionButtons" runat="server" visible="true">
   <td class="ActionButtonsStyle">
        <asp:Button ID="btnCancel" runat="server" CssClass="SaveButtonStyle" Text="Cancel" Width="73px" />
    
   </td>

</tr>
</table>--%>
</asp:Panel>