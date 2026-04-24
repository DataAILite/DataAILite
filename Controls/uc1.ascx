<%@ Control Language="VB" AutoEventWireup="false" CodeFile="uc1.ascx.vb" Inherits="Controls_uc1" %>
<script type="text/javascript" src="Controls/Javascripts/ChecklistDropDown.js"></script>
<style type="text/css">
    .ButtonStyle1 {
        width: 18px;
        height: 20px;
        font-size: 12px;
        border-radius: 3px;
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
        border-style :solid;
        border-color: dimgray;
        border-width: 1px;
        background-repeat: no-repeat;
        background-position:center;
        background-color:ButtonFace;
        padding:0px;
        margin:0px;
        z-index: 9999;
    }
    .ButtonDownStyle{
        width: 20px;
        /*height: 20px;*/
        /*border:none;*/
        font-size: 12px;
        padding:0px;
        margin:0px;
        /*border-radius: 3px;*/
        border-left:hidden;
        border-top:1px solid dimgray;
        border-right:1px solid dimgray;
        border-bottom:1px solid dimgray;
        /*border-style :solid;
        border-color: dimgray;
        border-width: 1px;*/
        background-color:ButtonFace;
        background-image: url('Controls/Images/arrow-down-black2.bmp');
        background-position:center;
        image-rendering:auto;
        background-repeat: no-repeat;
        z-index: 9999;
    }
    .ButtonUpStyle {
        width: 20px;
        height: 20px;
        font-size: 12px;
        padding:0px;
        margin:0px;
        border-left:hidden;
        border-top:1px solid dimgray;
        border-right:1px solid dimgray;
        border-bottom:1px solid dimgray;
        /*border-style :solid;
        border-color: dimgray;
        border-width: 1px;*/
        background-color:ButtonFace;
        background-image: url('Controls/Images/arrow-up-black.bmp');
        background-position:center;
        image-rendering:auto;
        background-repeat: no-repeat;
        z-index: 9999;
    }
    .tdBtnDropDown {
        width: 20px;
        /*height: 21px;*/
        padding:0px;
        margin:0px;
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
        /*display:inline;*/
        border: 1px solid ;
        border-color:darkgray;
        left: 3px;
        overflow: auto;
        position: relative;
        width:225px;
        z-index: 9999;
    }
    .DropDownClosed {
        visibility:hidden;
        display:none;
        border: 1px solid ;
        border-color:darkgray;
        left: 3px;
        overflow-y:scroll;
        position: relative;
        background-color:white;
        max-height:200px;
        z-index: auto;
        
    }
   .DropDownOpened {
        visibility:visible;
        display: block;
        border: 1px solid ;
        border-color:darkgray;
        left: 3px;
        overflow-y:scroll;
        position: relative;
        background-color:white;
        max-height:200px;
        /*width:auto;*/
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
<asp:HiddenField ID="hdnDropDown" runat="server" Value="closed"/>

<asp:HiddenField ID="hdnText" runat="server" Value=""/>
    <asp:HiddenField id="hdnSelectedData"  runat="server" Value=""/>
    <asp:HiddenField id="hdnScrollTop" runat="server" Value="0"/>

    <asp:HiddenField id="hdnAutoPostBack" runat="server" Value="0"/>

    <asp:HiddenField id="hdnPostBackType" runat="server" Value="1"/>

<table ID="tblDD" style="margin: 0px; padding: 0px; border:hidden; width:100%; table-layout:fixed; " >
    <tr id="rowDD" runat="server">
            <td style="vertical-align: top; padding: 0px; margin: 0px; width:90%;"   >
              <asp:TextBox ID="txtValue" runat="server" style="padding:0px; margin:0px;" width="100%" ReadOnly="True" BorderColor="DimGray" BorderWidth="1px" Height="21px"></asp:TextBox>
        </td>
        <td ID="tdBtnDropDown"  runat="server" class="tdBtnDropDown" >
            <asp:Button ID="btnDropDown" runat="server" TabIndex="-1" CssClass="ButtonDownStyle" />
        </td>
    
    </tr>
</table>

    <div id="DivChecklist" class="DropDownClosed" runat="server" >
        <asp:CheckBoxList ID="Checklist" runat="server" OnSelectedIndexChanged="Checklist_SelectedIndexChanged" Width="100%" AutoPostBack="False">
    </asp:CheckBoxList>
    </div>
</asp:Panel>