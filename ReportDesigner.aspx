<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ReportDesigner.aspx.vb" Inherits="ReportDesigner" %>

<script type="text/javascript" src="Controls/Javascripts/ReportView.js"></script>
<script type="text/javascript" src="Controls/Javascripts/ReportViewMenu.js" ></script>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="Controls/css/ReportView.css"/>
    <link rel="stylesheet" type="text/css" href="Controls/css/ReportViewMenu.css"/>
    <style type="text/css">
    </style>

</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <asp:UpdatePanel ID="updReportDesigner" runat="server" >
          <ContentTemplate>
            <asp:HiddenField ID ="hdnReportView" runat ="server" />
            <asp:HiddenField ID ="hdnReportID" runat ="server" />
            <asp:HiddenField ID ="hdnReportTitle" runat ="server" />
            <asp:HiddenField ID ="hdnDataBase" runat ="server" />
            <asp:HiddenField ID ="hdnFontData" runat="server"/>
            <asp:HiddenField ID ="hdnFontColor" runat="server" value="#000000" />
            <asp:HiddenField ID ="hdnSection" runat="server" value="Details" />
            <asp:HiddenField ID="hfSizeLimit" runat="Server" Value="4096" />

             <div class="title" >
                <asp:label ID="lblPageTitle" runat="server" Text="Online User Reporting"></asp:label>
            </div>
            <div id="divMain" runat="server" class="clearfix wholepage">
                <div id="divTreeMenu" class="column menu" runat="server">
                    <asp:TreeView ID ="TreeView1" runat ="server" NodeIndent="10" Font-Names="Times New Roman" BorderColor="#333333" CssClass="overflowy" Height="100%" Width="100%" BackColor="#CCCCCC" ImageSet="BulletedList">
                        <Nodes>
                            <asp:TreeNode Text="&lt;b&gt;Log Off;&lt;/b&gt;"  Value="Default.aspx" Expanded="True" ></asp:TreeNode>

                            <asp:TreeNode Text="&lt;b&gt;List of Reports&lt;/b&gt;"  Value="ListOfReports.aspx" Expanded="True" ></asp:TreeNode>                              

                            <asp:TreeNode Text="&lt;b&gt;Report Definition&lt;/b&gt;"  Value="ReportEdit.aspx?tne=2" Expanded="True" ></asp:TreeNode>

                            <asp:TreeNode Text="&lt;b&gt;Report Parameters&lt;/b&gt;"  Value="ReportEdit.aspx?tne=3" Expanded="True" ></asp:TreeNode>

                            <%--<asp:TreeNode Text="&lt;b&gt;Share Report (Users)&lt;/b&gt;"  Value="ReportEdit.aspx?tne=4" Expanded="True" ></asp:TreeNode>--%>

                            <asp:TreeNode Text="Report Data Query"  Value="SQLquery.aspx?tnq=0" Expanded="True" >
                                 <asp:TreeNode Text="Data fields" Value="SQLquery.aspx?tnq=0" > </asp:TreeNode>
                                 <asp:TreeNode Text="Joins" Value="SQLquery.aspx?tnq=1"> </asp:TreeNode>
                                 <asp:TreeNode Text="Filters"     Value="SQLquery.aspx?tnq=2" > </asp:TreeNode>
                                 <asp:TreeNode Text="Sorting"     Value="SQLquery.aspx?tnq=3" > </asp:TreeNode>                 
                            </asp:TreeNode>

                            <asp:TreeNode Text="Report Format Definition"  Value="RDLformat.aspx?tnf=0" Expanded="True" >
                                 <asp:TreeNode Text="Columns, Expressions"  Value="RDLformat.aspx?tnf=0" > </asp:TreeNode>
                                 <asp:TreeNode Text="Groups, Total"          Value="RDLformat.aspx?tnf=1" > </asp:TreeNode>
                                 <asp:TreeNode Text="Combine Values"          Value="RDLformat.aspx?tnf=2" > </asp:TreeNode>
                                 <asp:TreeNode Text="Map Definition"          Value="MapReport.aspx" > </asp:TreeNode>
                            </asp:TreeNode>

                            <asp:TreeNode Text="Explore Report Data"  Value="ShowReport.aspx?srd=0" Expanded="True" >
                                 <asp:TreeNode Text="Export Data to Excel"  Value="datatoExcel" NavigateUrl="ShowReport.aspx?srd=1" > </asp:TreeNode>
                                 <asp:TreeNode Text="Export Data to CSV"    Value="datatoCSV" NavigateUrl="ShowReport.aspx?srd=2" > </asp:TreeNode>
                                 <asp:TreeNode Text="Export Data to Delimited File"  value="ShowReport" Navigateurl="ShowReport.aspx?srd=10" > </asp:TreeNode>
                                <asp:TreeNode Text="Export Data to XML"    Value="datatoXML" NavigateUrl="ShowReport.aspx?srd=14" > </asp:TreeNode> 
                            </asp:TreeNode>

                            <asp:TreeNode Text="Show Report"  Value="ShowReport.aspx?srd=3" Expanded="True" >
                                 <asp:TreeNode Text="Show Generic Report"  Value="ReportViews.aspx?gen=yes" > </asp:TreeNode>
                                  <asp:TreeNode Text="Show Report Charts"  Value="ShowReport.aspx?srd=17" > </asp:TreeNode>
                                 <asp:TreeNode Text="Export Report to Excel"  Value="reptoExcel" NavigateUrl="ShowReport.aspx?srd=4" > </asp:TreeNode>
                                 <asp:TreeNode Text="Export Report to Word"    Value="reptoWord" NavigateUrl="ShowReport.aspx?srd=5" > </asp:TreeNode>
                                 <asp:TreeNode Text="Export Report to PDF"  Value="reptoPDF" NavigateUrl="ShowReport.aspx?srd=6" > </asp:TreeNode>
                                 <asp:TreeNode Text="See Crystal Report"    Value="ShowReport.aspx?srd=7"> </asp:TreeNode>
                            </asp:TreeNode>

                            <asp:TreeNode Text="Show Analytics"  Value="ShowReport.aspx?srd=11" Expanded="True" >
                                <asp:TreeNode Text="See Data Overall Statistics"  Value="ShowReport.aspx?srd=8" > </asp:TreeNode>
                                <asp:TreeNode Text="Export Overall Statistics to Excel"  Value="reptoExcel" NavigateUrl="ShowReport.aspx?srd=9" > </asp:TreeNode>
                                <asp:TreeNode Text="See Groups Statistics"  Value="ReportViews.aspx?grpstats=yes" > </asp:TreeNode>
                                <asp:TreeNode Text="See Fields Correlation"  Value="ShowReport.aspx?srd=12" > </asp:TreeNode>
                                <asp:TreeNode Text="Matrix Balancing"  Value="ShowReport.aspx?srd=13" > </asp:TreeNode>
                            </asp:TreeNode>
                        </Nodes>
                       <RootNodeStyle HorizontalPadding="2px" Font-Bold="True" />
                       <NodeStyle CssClass="NodeStyle" />
                       <ParentNodeStyle Font-Bold="True" />
                    </asp:TreeView>
                </div>
                <div>
                    <table>
                        <tr colspan="3">
                           <div class="header">
                              <asp:label ID="lblHeader" runat="server" Text="Report Designer"></asp:label>
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                              <asp:label ID="lblSection" runat="server" ForeColor="#000099" Font-Size="Medium"></asp:label>
                            </div>
                        </tr>
                        <tr colspan="3">
                <div id="divContent" runat="server" class="column content" style="display:;" >
                   <div id="divSpecialFieldList" class="column columnmargin" style="width:20%; height:240px; margin-left: 5px; margin-top: 1px; border-style: none solid solid solid; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; border-right-color: #808080; border-bottom-color: #808080; border-left-color: #808080; display:none; ">
                       <WC:DragList id="lstSpecialFields" runat="server" Font-Names="Tahoma" BackColor="White" BorderStyle="None" Font-Size="Small" Text="Special Fields" Height="100%" Width="100%" HeadingAlignment="Center" HeadingForeColor="ActiveCaptionText" BorderWidth="1px" Draggable="True" DoPostBack="False" />
                   </div>
                   <div id="divGroupList" runat="server"  class ="column columnmargin" style="width:20%; height:240px; margin-left: 5px; margin-top: 1px; border-style: none solid solid solid; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; border-right-color: #808080; border-bottom-color: #808080; border-left-color: #808080; display:none; ">
                       <WC:DragList id="lstGroups" runat="server" Font-Names="Tahoma" BackColor="White" BorderStyle="None" Font-Size="Small" Text="Groups" Height="100%" Width="100%" HeadingAlignment="Center" HeadingForeColor="ActiveCaptionText" BorderWidth="1px" Draggable="False" DoPostBack="False" />
                   </div>
                   <div id="divFieldList" class="column" style="width:25%; height:240px; margin-left: 5px; margin-top: 1px; border-style: none solid solid solid; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; border-right-color: #808080; border-bottom-color: #808080; border-left-color: #808080;">
                       <WC:DragList id="lstFields" runat="server" Font-Names="Tahoma" BackColor="White" BorderStyle="None" Font-Size="Small" Text="Fields" Height="100%" Width="100%" HeadingAlignment="Center" HeadingForeColor="ActiveCaptionText" BorderWidth="1px" DoPostBack="False" Draggable="True" />
                   </div>
                   <div id="divSectionList" runat="server" class="column" style=" width:20%; height:240px; margin-left: 1px; margin-top: 1px; margin-right:38%; border-style: none solid solid solid; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; border-right-color: #808080; border-bottom-color: #808080; border-left-color: #808080; ">
                       <WC:DragList id="lstSections" runat="server" Font-Names="Tahoma" BackColor="White" BorderStyle="None" Font-Size="Small" Text="Available Sections" Height="100%" Width="100%" HeadingAlignment="Center" HeadingForeColor="ActiveCaptionText" BorderWidth="1px" DoPostBack="False" Draggable="False" />
                   </div>
                   <div id="divButtons" style="float: right;">
                        <div style="display:inline-block; margin-right:3px;">
                          <asp:Button ID="btnHelp" runat="server" CssClass="roundbutton" Text="&#10067;" CausesValidation="false" UseSubmitBehavior="False" ToolTip="Report Designer Help" />
                        </div>
                       <div style="display:inline">
                          <asp:Button ID="btnDesignerMenu" runat="server" CssClass="roundbutton" Text="&#9776;" CausesValidation="false" UseSubmitBehavior="False" ToolTip="Report Designer Options" Visible="True" />
                       </div>
                       <div style="display:inline;">
                          <asp:Button ID="btnSubmit" runat="server" CssClass="roundbutton" Text="&#128190;" CausesValidation="false" UseSubmitBehavior="False" ToolTip="Save and Show Report" Visible="True" />
                       </div>
                        <div style="display:inline; margin-right:10px;">
                            <asp:Button ID="btnClose" runat="server" CssClass="roundbutton" Text="&#10006;" CausesValidation="false" UseSubmitBehavior="False" ToolTip="Close Designer" />
                        </div>
                  </div>
                    <br />
                    <%-- ******************************************* Action Buttons ************************************************ --%>
                  <div id="divActionButtons" style="float: right;">
                          <asp:Button ID="btnShow" runat="server" style="outline: hidden;" Text="" CausesValidation="false" UseSubmitBehavior="False"  BorderStyle="None" Height="1px" Width="1px" BackColor="White" TabIndex="-1" />
                          <asp:Button ID="btnReturn" runat="server" style="outline: hidden;" Text="" CausesValidation="false" UseSubmitBehavior="False"  BorderStyle="None" Height="1px" Width="1px" BackColor="White" TabIndex="-1" />
                          <asp:Button ID="btnExit" runat="server" style="outline: hidden;" Text="" CausesValidation="false" UseSubmitBehavior="False"  BorderStyle="None" Height="1px" Width="1px" BackColor="White" TabIndex="-1" />
                  </div>
<%--                  <div id="divDrop" runat="server" class ="columnmargin" style="background-color: #FFFFFF; border: 1px solid #000000; width: 83%; height: 54%; position: absolute; overflow-y:auto; overflow-x: scroll; white-space: nowrap; font-size:10px; top: 327px;" oncontextmenu="return showDropMenu(event)">
                      <div id="DropLine" runat="server" style="position:absolute; width:2px; height:100%; border-right:2px dotted navy; top:0px; left:8in; z-index: 999;"></div>
                  </div>--%>
                  <div id="divDrop" runat="server" class ="columnmargin" style="background-color: #FFFFFF; border: 1px solid #000000; width: 83%; height: 54%; position: absolute; overflow-y:auto; overflow-x: scroll; white-space: nowrap; font-size:10px; top: 327px;" oncontextmenu="return false;">
                      <div id="DropLine" runat="server" style="position:absolute; width:2px; height:100%; border-right:2px dotted navy; top:0px; left:8in; z-index: 999;"></div>
                  </div>
                  <div id="divHeaderDisplay" runat="server" class ="columnmargin" style="display:none; background-color: #FFFFFF; border: 1px solid #000000; width: 83%; height: 54%; position: absolute; overflow-y: hidden; overflow-x: hidden; white-space: nowrap; font-size:10px; top: 327px;" oncontextmenu="return false;">
                      <div id="HeadVerticalLine" runat="server" style="display: none; position:absolute; width:2px; height:1in; border-right:2px dotted navy; top:0px; left:8in; z-index: 999;"></div>
                      <div id="HeadHorizontalLine" runat="server" style=" display:none; position:absolute; width:8in; height:2px; border-top:2px dotted navy; top:1in; left:0px;z-index: 999; "></div>
                      <div id="divHeader" runat="server" style="position:absolute; width:8in; height:1in; border-right: 1px solid darkgrey; border-bottom:1px solid darkgrey; top:0px; left:2px; background-color: white;"></div>
                      <div id="divHeaderSettings" runat="server" class="roundbutton" style="display:none; position:absolute; top:0px; left:97%; z-index:999;padding-left:3px; padding-top:3px;">
                            <asp:ImageButton ID="btnHeaderSettings" runat="server" ImageUrl="~/Controls/Images/Settings2.ico" CausesValidation="false" UseSubmitBehavior="False" ToolTip="Header Settings" ImageAlign="Middle" Height="28" Width="28" />
                      </div>
                  </div>
                  <div id="divFooterDisplay" runat="server" class ="columnmargin" style="display:none; background-color: #FFFFFF; border: 1px solid #000000; width: 83%; height: 54%; position: absolute; overflow-y: hidden; overflow-x: hidden; white-space: nowrap; font-size:10px; top: 327px;" oncontextmenu="return false;">
                      <div id="FootVerticalLine" runat="server" style="display:none; position:absolute; width:2px; height:1in; border-right:2px dotted navy; top:0px; left:8in; z-index: 999;"></div>
                      <div id="FootHorizontalLine" runat="server" style=" display:none; position:absolute; width:8in; height:2px; border-top:2px dotted navy; top:1in; left:0px;z-index: 999; "></div>
                      <div id="divFooter" runat="server" style="position:absolute; width:8in; height:1in; border-right: 1px solid darkgrey; border-bottom:1px solid darkgrey; top:0px; left:2px; background-color: white;"></div>

                       <div id="divFooterSettings" runat="server" class="roundbutton" style="display:none; position:absolute; top:0px; left:97%; z-index:999;padding-left:3px; padding-top:3px;">
                            <asp:ImageButton ID="btnFooterSettings" runat="server" ImageUrl="~/Controls/Images/Settings2.ico" CausesValidation="false" UseSubmitBehavior="False" ToolTip="Footer Settings" ImageAlign="Middle" Height="28" Width="28" />
                      </div>
                  </div>
                  <div id="divGroup" runat="server" class ="columnmargin" style="display:none; background-color: #FFFFFF; border: 1px solid #000000; width: 83%; height: 54%; position: absolute; overflow-y: hidden; overflow-x: hidden; white-space: nowrap; font-size:10px; top: 327px;" oncontextmenu="return false;">
                  </div>


                    <%--****************************************** Menus ****************************************--%>
                  <div id="pnlMenu"  style="display:none;" class="skin0" onmouseover="highlight(event)" onmouseout="lowlight(event)" onclick="doOption(event)"  >
                    <div id="DeleteField" class="menuitems">Delete Field</div>
                    <div id="EditCaption" class="menuitems">Edit Caption</div>
                    <div id="MoveWithArrowKeys" class="menuitems">Move Using Arrow Keys </div>
                    <div id="CaptionTextAlign" class="menuitems">Caption Text Align &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&gt</div>
                    <div id="DetailTextAlign" class="menuitems">Detail Text Align &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&gt</div>
                    <div id="CaptionFieldLayout" class="menuitems">Field Layout &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&gt</div>
                    <div id="ResizeCaption" class="menuitems">Resize Caption &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&gt; </div>
                    <div id="ResizeDetail" class="menuitems">Resize Detail &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&gt; </div>
                  </div>
                  <div id="divLabelMenu" style="display:none; width:175px;" class="skin0" onmouseover="highlight(event)" onmouseout="lowlight(event)" onclick="doOption(event)">
                    <div id="DeleteLabel" class="menuitems">Delete Label</div>
                    <div id="EditLabel" class="menuitems">Edit Label</div>
                    <div id="LabelSettings" class="menuitems"> Label Settings...</div>
                    <div id="LabelTextAlign" class="menuitems">Label Text Align &nbsp;&nbsp;&nbsp;&nbsp;&gt; </div>
                    <div id="ResizeLabel" class="menuitems">Resize Label &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&gt; </div>
                    <div id="MoveWithArrows" class="menuitems">Move Using Arrow Keys </div>
                  </div>
                  <div id="divSpecialFieldMenu" style="display:none; width:175px;" class="skin0" onmouseover="highlight(event)" onmouseout="lowlight(event)" onclick="doOption(event)">
                    <div id="DeleteSpecialField" class="menuitems">Delete Field</div>
                    <div id="SpecialFieldSettings" class="menuitems"> Field Settings...</div>
                    <div id="SpecialFieldTextAlign" class="menuitems">Field Text Align &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&gt; </div>
                    <div id="ResizeSpecialField" class="menuitems">Resize Field &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&gt; </div>
                    <div id="MoveSpecialFieldWithArrows" class="menuitems">Move Using Arrow Keys </div>
                  </div>
                   <div id ="divGroupFieldMenu" style="display:none; width:235px;" class="skin0" onmouseover="highlight(event)" onmouseout="lowlight(event)" onclick="doOption(event)">
                      <div id="GroupFieldSettings" class="menuitems"> Group Field Settings...</div>
                     <div id="ResizeGroupFieldHeight" class="menuitems">Resize Group Field Height</div>
                     <div id="MoveGroupVertically" class="menuitems"> Move Group Vertically With Arrows</div>
                   </div>
                  <div id="divImageFieldMenu" style="display:none; width:185px;" class="skin0" onmouseover="highlight(event)" onmouseout="lowlight(event)" onclick="doOption(event)">
                    <div id="DeleteImageField" class="menuitems">Delete Field</div>
                    <div id="ImageFieldSettings" class="menuitems"> Image Field Settings...</div>
                    <div id="ResizeImageField" class="menuitems">Resize Field &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&gt; </div>
                    <div id="MoveImageFieldWithArrows" class="menuitems">Move Using Arrow Keys </div>
                 </div>
                  <div id="divDropMenu"  style="display:none; width:200px;" class="skin0" onmouseover="highlight(event)" onmouseout="lowlight(event)" onclick="doOption(event)"  >
                          <div id="SaveAndShow" style="padding-left:5px;padding-right:5px;" class="menuitems">Save And Show Report</div>
                          <div id="SaveAndReturn" style="padding-left:5px;padding-right:5px;" class="menuitems">Save And Return To Designer</div>
                          <div id="SaveAndClose" style="padding-left:5px;padding-right:5px;" class="menuitems">Save And Close Designer</div>
                          <hr id="DropLine1" style="border: thin solid #E1F0FF; width: 100%;  margin-top: 1px; margin-bottom: 1px; height: auto; " />
                          <div id="AddLabel" style="padding-left:5px;padding-right:5px; display:none;" class="menuitems">Add Text Label</div>
                          <div id="ReportTemplate" style="padding-left:5px;padding-right:5px;" class="menuitems">Report Template &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&gt; </div>
                          <div id="ReportOrientation" style="padding-left:5px;padding-right:5px;" class="menuitems">Report Orientation &nbsp;&nbsp;&nbsp;&nbsp;&gt; </div>
                          <div id="ReportFieldLayout" style="display:none; padding-left:5px;padding-right:5px;" class="menuitems">Report Field Layout &nbsp;&nbsp;&nbsp;&gt;</div>
                         <div id="TabularColWidths" style="display:none; padding-left:5px;padding-right:5px;" class="menuitems">Tabular Column Widths...</div>
                         <div id="ClearFields" style="padding-left:5px;padding-right:5px;" class="menuitems">Clear Fields</div>
                         <hr id="DropLine2" style="border: thin solid #E1F0FF; width: 100%;  margin-top: 1px; margin-bottom: 1px; height: auto; " />
                         <div id="CaptionSettings" style="padding-left:5px;padding-right:5px;" class="menuitems">Report Caption Settings...</div>
                         <div id="DetailSettings" style="padding-left:5px;padding-right:5px;" class="menuitems">Report Detail Settings...</div>
                         <%--<div id="GroupSettings" style="padding-left:5px;padding-right:5px;" class="menuitems">Report Group Settings...</div>--%>
                        <div id="ReportCaptionAlign" style="padding-left:5px;padding-right:5px;" class="menuitems">Report Caption Align &nbsp;&gt; </div>
                        <div id="ReportDetailAlign" style="padding-left:5px;padding-right:5px;" class="menuitems">Report Detail Align &nbsp;&nbsp;&nbsp;&gt; </div>
                        <hr id="DropLine3" style="border: thin solid #E1F0FF; width: 100%;  margin-top: 1px; margin-bottom: 1px; height: auto; " />
                        <div id="HeaderSettings" style="padding-left:5px;padding-right:5px;" class="menuitems">Header Settings...</div>
                        <div id="HeaderFieldSettings" style="padding-left:5px;padding-right:5px;" class="menuitems">Header Field Settings...</div>
                        <div id="FooterSettings" style="padding-left:5px;padding-right:5px;" class="menuitems">Footer Settings...</div>
                        <div id="FooterFieldSettings" style="padding-left:5px;padding-right:5px;" class="menuitems">Footer Field Settings...</div>
                        <hr id="DropLine4" style="border: thin solid #E1F0FF; width: 100%;  margin-top: 1px; margin-bottom: 1px; height: auto; " />
                        <div id="CloseDesigner" style="padding-left:5px;padding-right:5px;" class="menuitems">Close Designer</div>
                  </div>
                    <%-- *********************************************** Sub Menus **************************************************** --%>
                  <div id="divOrientationMenu" style="display:none;" class="skin0" onmouseover="highlight(event)" onmouseout="lowlight(event)" onclick="doOption(event)">
                        <div id="Portrait" class="menuitems" style="display:inline-block;width:100%;">
                           <div id ="ckPortrait" class="checkmark">&#10004</div>
                           Portrait
                        </div>
                        <div id="Landscape" class="menuitems" style="display:inline-block;width:100%;">
                            <div id ="ckLandscape" class="checkmark" >&nbsp;&nbsp;</div>
                            Landscape
                        </div>
                  </div>
                  <div id="divTemplateMenu" style="display:none;" class="skin0" onmouseover="highlight(event)" onmouseout="lowlight(event)" onclick="doOption(event)">
                        <div id="Tabular" class="menuitems" style="display:inline-block;width:100%;">
                           <div id ="ckTabular" class="checkmark">&#10004</div>
                           Tabular
                        </div>
                        <div id="FreeForm" class="menuitems" style="display:inline-block;width:100%;">
                            <div id ="ckFreeForm" class="checkmark" >&nbsp;&nbsp;</div>
                            Free Form
                        </div>
                  </div>
                  <div id="divFieldLayoutMenu" style="display:none;" class="skin0" onmouseover="highlight(event)" onmouseout="lowlight(event)" onclick="doOption(event)">
                        <div id="Inline" class="menuitems" style="display:inline-block;width:100%;">
                           <div id ="ckInline" class="checkmark">&#10004</div>
                           In Line
                        </div>
                        <div id="Block" class="menuitems" style="display:inline-block;width:100%;">
                            <div id ="ckBlock" class="checkmark" >&nbsp;&nbsp;</div>
                            Block
                        </div>
                  </div>
                  <div id="divTextAlignMenu" style="display:none;" class="skin0" onmouseover="highlight(event)" onmouseout="lowlight(event)" onclick="doOption(event)">
                        <div id="Left" class="menuitems" style="display:inline-block;width:100%;">
                           <div id ="ckLeft" class="checkmark">&#10004</div>
                           Left
                        </div>
                        <div id="Center" class="menuitems" style="display:inline-block;width:100%;">
                            <div id ="ckCenter" class="checkmark" >&nbsp;&nbsp;</div>
                           Center
                        </div>
                        <div id="Right" class="menuitems" style="display:inline-block;width:100%;">
                            <div id ="ckRight" class="checkmark" >&nbsp;&nbsp;</div>
                           Right
                        </div>
                       <div id="Auto" class="menuitems" style="display:none;width:100%;">
                            <div id ="ckAuto" class="checkmark" >&nbsp;&nbsp;</div>
                           Auto
                        </div>
                  </div>
                  <div id="divResizeMenu" style="display:none;" class="skin0" onmouseover="highlight(event)" onmouseout="lowlight(event)" onclick="doOption(event)">
                        <div id="horizontal" class="menuitems" style="display:inline-block;width:100%;">
                           <div id ="ckWidth" class="checkmark">&#10004</div>
                          Width
                        </div>
                        <div id="vertical" class="menuitems" style="display:inline-block;width:100%;">
                            <div id ="ckHeight" class="checkmark" >&nbsp;&nbsp;</div>
                           Height
                        </div>
                        <div id="both" class="menuitems" style="display:inline-block;width:100%;">
                            <div id ="ckBoth" class="checkmark" >&nbsp;&nbsp;</div>
                           Both Height and Width
                        </div>
                  </div>
                </div>
                        </tr>
                  </table>
                </div>

               <%-- ******************************** HeaderFooter Dialog Box ************************--%>
                <div id="divHeaderFooterDlgBackground" runat="server" class="PopupBackground" style="display:none; height:100%;width:100%;position:absolute;top:0px;left:0px;" >
                      <div id="divHeaderFooterDlg" runat="server" class="HeaderFooterDlg relative-middle" >
                            <div id="divHeaderFooterDlgHeading" style=" font-size: small; text-align: center; line-height: 22px; background-color: gray; width: 100%; height: 22px; color: white; " >
                                 <asp:Label ID="lblHeaderFooterDlgHeading" runat="server" Text="Header Settings"></asp:Label>
                                 <div id="divHeaderFooterDlgX" runat ="server" class="close" title="close dialog">&times;</div>
                            </div>
                            <div id="divHeaderFooterDlgBody" class="clearfix" style="margin:5px; font-family: Tahoma; font-size: small;">
                                 <div id="divHeaderFooterHeight" style="display:block; margin:5px;">
                                      <label for="txtHeaderFooterHeight" >Height:</label>
                                      <br />
                                      <input type="number" id="txtHeaderFooterHeight" name="txtHeaderFooterHeight" runat="server" min="1" value="1" style="border: 1px solid #0099ff; width:75px; margin-top: 2px;" />
                                        inch
                                  </div>
                                <table>
                                     <tr>
                                          <td id="tdHeaderFooterSettings"">
                                                <div id ="divHeaderFooterBackgroundSettings" runat="server" style="display:block; margin-left: 10px; margin-top: 10px;  font-size: small;" >
                                                       <asp:Label ID="lblHeaderFooterBackgroundSettings" runat="server" Text="Background Settings" ></asp:Label>
                                                       <br />
                                                       <div id="divHeaderFooterBackgroundSettingsEntry" style=" display: inline-block; width: 220px; height: 100px; border-style:solid; border-color:#0099FF; border-width: 1px; padding-top: 5px;">
                                                            <div id="divHeaderFooterBackColor">
                                                                 <div id="divlblHeaderFooterBackColor" style="margin-left: 5px">Background Color:</div>
                                                                 <div id="divHeaderFooterChooseBackColor" style="border: 1px solid #666666; width: 170px; height: 26px; line-height: 26px; margin-left: 5px; border-radius:3px;" title="Choose background color from a list">
                                                                    <table>
                                                                       <tr>
                                                                          <td >
                                                                             <div id="divHeaderFooterShowBackColor" style="border: none; width: 22px; height: 18px; border-radius:3px; background-color: white; margin-bottom:3px;"></div>
                                                                          </td>  
                                                                          <td >
                                                                             <div id="divHeaderFooterBackColorName"  style="font-size:smaller; border: none; height: 18px; width:125px; text-align:left; margin-bottom:3px;">white</div>
                                                                          </td>
                                                                       </tr>
                                                                    </table>
                                                                 </div>
                                                             </div>
                                                             <div id="divHeaderFooterCustomBackColor" >
                                                                <input id="btnHeaderFooterBackColor" type="color" value="black" style=" height:1px; width:1px; border:none; "></input>
                                                                <input id="btnHeaderFooterBackColorCustomize" type="button" value="Custom Background Color" title="Customize fill color" style ="margin-left: 10px;"  />
                                                             </div>
                                                           <br />
                                                        </div>
                                                </div>
                                                <div id="divHeaderFooterBorderSettings" runat="server" style="display:block; margin-left: 10px; margin-top: 10px; font-size: small;" >
                                                    <asp:Label ID="lblHeaderFooterBorderSettings" runat="server" Text="Border Settings" ></asp:Label>
                                                    <br />
                                                    <div id="divHeaderFooterBorderSettingsEntry" style=" display: inline-block; width: 220px; height: 200px; border-style:solid; border-color:#0099FF; border-width: 1px;">
                                                        <div id="divHeaderFooterBorderStyle" style="display:block; margin:5px;">
                                                            <asp:Label ID="lblHeaderFooterBorderStyle" runat="server" Text="Border Style:" Font-Size="Small"></asp:Label>
                                                            <br />
                                                            <select id="lstHeaderFooterBorderStyles" size="1" style="width: 173px; border-color: #0099ff; outline:none;" >
                                                                <option value="None">None</option>
                                                                <option value="Solid">Solid</option>
                                                                <option value="Dotted">Dotted</option>
                                                                <option value="Dashed">Dashed</option>
                                                                <option value="Double">Double</option>
                                                            </select>    
                                                        </div>
                                                        <div id="divHeaderFooterBorderWidth"  style="display:block; margin:5px;">
                                                            <label for="txtHeaderFooterBorderWidth">Border Width:</label>
                                                            <br />
                                                            <input type="number" id="txtHeaderFooterBorderWidth" name="txtHeaderFooterBorderWidth" min="1" value="1" style="border: 1px solid #0099ff;width:60px;" />
                                                            px
                                                        </div>
                                                        <%--<br />--%>
                                                        <div id="divHeaderFooterBorderColor">
                                                            <div id="divlblHeaderFooterBorderColor" style="margin-left: 5px">Border Color:</div>
                                                            <div id="divHeaderFooterChooseBorderColor" style="border: 1px solid #666666; width: 170px; height: 26px; line-height: 26px; margin-left: 5px; border-radius:3px;" title="Choose border color from a list">
                                                                <table>
                                                                    <tr>
                                                                       <td>
                                                                          <div id="divHeaderFooterShowBorderColor" style="border: none; width: 22px; height: 18px; border-radius:3px; background-color: black; margin-bottom:3px;"></div>
                                                                       </td>
                                                                       <td>
                                                                          <div id="divHeaderFooterBorderColorName"  style="font-size:smaller; border: none; height: 18px; width:125px; text-align:left; margin-bottom:3px;">black</div>
                                                                       </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </div>
                                                        <div id="divHeaderFooterBorderCustomColor" >
                                                            <input id="btnHeaderFooterBorderColor" type="color" value="black" style="height:1px; width:1px; border:none; "></input>
                                                            <input id="btnHeaderFooterBorderColorCustomize" type="button" value="Custom Border Color" title="Customize border color" />
                                                        </div>
                                                        <br />
                                                    </div>
                                                </div>
                                         </td>
                                         <td id="tdHeaderFooterFieldSettings">
                                                    <div id="divFieldSettingsEntry" style="display: inline-block; width: 255px; height: 350px;  margin-left:35px; font-size: small;">
                                                            <asp:Label ID="lblFieldSettings" runat="server" Text ="Field Settings"></asp:Label>
                                                           <div id="divFieldSettingsDisplay" style="display: inline-block; width: 255px; height:165px; border-style:solid; border-color:#0099FF; border-width: 1px; ">
                                                                 <div id="divHeaderFooterFillFields" style="margin-top: 3px;">
                                                                       <asp:CheckBox id ="ckHeaderFooterFillFields" runat="server" Text="Fill Fields With Back Color" Checked="True" />
                                                                   </div>
                                                                <div id="divHeaderFooterRemoveBorders" style="margin-top: 0px;">
                                                                       <asp:CheckBox id = "ckRemoveBorders" runat="server" Text="Remove Field Borders" Checked="True" />
                                                                </div>
                                                                <div id="divHeaderFooterSetTextColors" style="margin-top: 0px;">
                                                                       <asp:CheckBox id = "ckSetTextColors" runat="server" Text="Set Field Text Colors" Checked="True" />
                                                                </div>
                                                               <div id="divFieldTextColorEntry">
                                                                       <div id="divFieldTextColor" style="margin-top:5px;">
                                                                            <div id="divlblFieldTextColor" style="margin-left: 5px">Text Color:</div>
                                                                            <div id="divChooseFieldTextColor" style="border: 1px solid #666666; width: 170px; height: 26px; line-height: 26px; margin-left: 5px; border-radius:3px;" title="Choose text color from a list">
                                                                                <table>
                                                                                    <tr>
                                                                                        <td >
                                                                                            <div id="divShowFieldTextColor" style="border: none; width: 22px; height: 18px; border-radius:3px; background-color: black; margin-bottom:3px;"></div>
                                                                                        </td>
                                                                                        <td >
                                                                                            <div id="divFieldTextColorName"  style="font-size:smaller; border: none; height: 18px; width:125px; text-align:left; margin-bottom:3px;">black</div>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                           </div>
                                                                        </div>
                                                                       <div id="divCustomFieldTextColor" >
                                                                              <input id="btnFieldTextColor" type="color" value="black" style="height:1px; width:1px; border:none; "></input>
                                                                              <input id="btnCustomizeFieldTextColor" type="button" value="Custom Field Text Color" title="Customize field text color" style="margin-left: 80px;" />
                                                                       </div>
                                                               </div>
                                                           </div>
                                                    </div>
                                         </td>
                                     </tr>
                                </table>
                                <br />
                                 <div id="divHeaderFooterDlgBoxButtons" style="text-align:center;">
                                     <asp:Button ID="btnHeaderFooterOK" runat="server" CssClass="dlgboxbutton" Text="OK" CausesValidation="false" UseSubmitBehavior="False" />
                                     <asp:Button ID="btnHeaderFooterCancel" runat="server" CssClass="dlgboxbutton btnCancel" Text="Cancel" CausesValidation="false" UseSubmitBehavior="False" />
                               </div>
                            </div>
                      </div>
                </div>
               <%-- ************************************ Font Dialog Box ***************************--%>
               <div id="divFontDlgBackground" runat="server" class="PopupBackground" style="display:none; height:100%;width:100%;position:absolute;top:0px;left:0px;">
                    <div id="divFontDlg" runat="server" class="FontDlg relative-middle">
                        <div id="divFontDlgHeading" style=" font-size: small; text-align: center; line-height: 22px; background-color: gray; width: 100%; height: 22px; color: white; ">
                           <asp:Label ID="lblFontDlgHeading" runat="server" Text="Fonts"></asp:Label>
                           <div id="divFontDlgX" runat ="server" class="close" title="close dialog">&times;</div>
                        </div>
                        <div id="divFontDlgBody" class="clearfix" style="margin:5px;">
                            <table>
                                <tr>
                                    <td id="tdFont">
                                        <div id="divFont" style="display:block; margin: 5px;">
                                            <asp:Label ID="lblFont" runat="server" Text="Font:" Font-Size="small"></asp:Label>
                                            <br />
<%--                                            <input id="txtFont" runat="server" type="text" style="border: 1px solid #0099FF; outline:none; padding: 3px; margin: 0px; height: 22px; width: 150px;" />
                                            <br />--%>
                                            <select id="lstFonts" size="1" style="width: 150px; border-color: #0099ff; outline:none;" >
                                                <option value="Arial">Arial</option>
                                                <option value="Calibri">Calibri</option>
                                                <option value="Cambria">Cambria</option>
                                                <option value="Cambria Math">Cambria Math</option>
                                                <option value="Candara">Candara</option>
                                                <option value="Comic Sans MS">Comic Sans MS</option>
                                                <option value="Courier">Courier</option>
                                                <option value="Courier New">Courier New</option>
                                            </select>    
                                        </div>
                                    </td>
                                    <td id="tdFontStyle">
                                    <div id="divFontStyle" style="display:block; margin:5px;">
                                            <asp:Label ID="lblFontStyle" runat="server" Text="Font style:" Font-Size="Small"></asp:Label>
                                            <br />
<%--                                            <input id="txtFontStyle" runat="server" type="text" style="border: 1px solid #0099FF; padding: 3px; outline:none; margin: 0px; height: 22px; width: 90px;" />
                                            <br />--%>
                                            <select id="lstFontStyles" size="1" style="width: 90px; border-color: #0099ff; outline:none;" >
                                                <option value="Regular">Regular</option>
                                                <option value="Italic">Italic</option>
                                                <option value="Bold">Bold</option>
                                                <option value="Bold Italic">Bold Italic</option>
                                            </select>    
                                        </div>
                                    </td>
                                    <td id="tdFontSize">
                                      <div id="divFontSize" style="display:block; margin:5px">
                                            <asp:Label ID="lblFontSize" runat="server" Text="Size:" Font-Size="Small"></asp:Label>
                                            <br />
<%--                                            <input id="txtFontSize" runat="server" type="text" style="border: 1px solid #0099FF; padding: 3px; outline:none; margin: 0px; height: 22px; width: 50px;" />
                                            <br />--%>
                                            <select id="lstFontSize" size="1" style="width: 50px; border-color: #0099ff; outline:none;" >
                                                <option value="8">8</option>
                                                <option value="9">9</option>
                                                <option value="10">10</option>
                                                <option value="11">11</option>
                                                <option value="12">12</option>
                                                <option value="14">14</option>
                                                <option value="16">16</option>
                                                <option value="18">18</option>
                                                <option value="20">20</option>
                                                <option value="22">22</option>
                                                <option value="24">24</option>
                                                <option value="26">26</option>
                                                <option value="28">28</option>
                                                <option value="32">32</option>
                                                <option value="36">36</option>
                                                <option value="40">40</option>
                                                <option value="48">48</option>
                                                <option value="72">72</option>
                                            </select>    
                                        </div>
                                    </td>
                                </tr>
                            </table>

                            <table>
                                <tr>
                                    <td id="tdEffects">
                                        <div id="divEffects" runat="server" style="display:block; margin-left: 5px; height: 300px; font-size: small;">
                                            <asp:Label ID="lblEffects" runat="server" Text="Effects" ></asp:Label>
                                            <br />
                                            <div id="divEffectsEntry" style=" display: inline-block; width: 225px; height: 225px; border-style:solid; border-color:#0099FF; border-width: 1px;">
                                                <div id="divStrikeout" runat="server" style="margin-bottom:10px;margin-top:5px;">
                                                    <input id="ckStrikeout" type="checkbox" value="Strikeout" />
                                                    <label id="lblStrikeout">Strikeout</label>
                                                </div>
                                                <div id="divUnderline" runat="server" style="margin-bottom:10px;" >
                                                    <input id="ckUnderline" type="checkbox" value="Underline" />
                                                    <label id="lblUnderline">Underline</label>
                                                </div>
                                                <div id="divColor">
                                                    <div id="divlblColor" style="margin-left: 5px">Text Color:</div>
                                                    <div id="divChooseColor" style="border: 1px solid #666666; width: 170px; height: 26px; line-height: 26px; margin-left: 5px; border-radius:3px;" title="Choose text color from a list">
                                                        <table>
                                                            <tr>
                                                                <td >
                                                                    <div id="divShowColor" style="border: none; width: 22px; height: 18px; border-radius:3px; background-color: black; margin-bottom:3px;"></div>
                                                                </td>
                                                                <td >
                                                                    <div id="divColorName"  style="font-size:smaller; border: none; height: 18px; width:125px; text-align:left; margin-bottom:3px;">black</div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                   </div>
                                                </div>
                                                <div id="divCustomColor" >
                                                  <input id="btnColor" type="color" value="black" style="height:1px; width:1px; border:none; "></input>
                                                  <input id="btnCustomize" type="button" value="Custom Text Color" title="Customize text color" />
                                               </div>
                                              <br />
                                              <div id="divBackColor">
                                                 <div id="divlblBackColor" style="margin-left: 5px">Background Color:</div>
                                                     <div id="divChooseBackColor" style="border: 1px solid #666666; width: 170px; height: 26px; line-height: 26px; margin-left: 5px; border-radius:3px;" title="Choose background color from a list">
                                                        <table>
                                                            <tr>
                                                                <td >
                                                                    <div id="divShowBackColor" style="border: none; width: 22px; height: 18px; border-radius:3px; background-color: white; margin-bottom:3px;"></div>
                                                                </td>
                                                                <td >
                                                                    <div id="divBackColorName"  style="font-size:smaller; border: none; height: 18px; width:125px; text-align:left; margin-bottom:3px;">white</div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </div>
                                              <div id="divCustomBackColor" >
                                                  <input id="btnBackColor" type="color" value="black" style="height:1px; width:1px; border:none; "></input>
                                                  <input id="btnBackColorCustomize" type="button" value="Custom Background Color" title="Customize fill color" />
                                               </div>
                                            </div>
                                        </div>
                                    </td>
                                    <td id="tdSampleAndBorder">
                                        <div id ="divSampleAndBorderEntry" style=" display: inline-block; width: 225px; height: 305px;">
                                            <div id="divSample" runat="server" style="display:block; margin-left: 25px; font-size: small;">
                                                <asp:Label ID="lblSample" runat="server" Text="Sample" ></asp:Label>
                                               <br />
                                              <div id="divSampleDisplay" style=" display: inline-block; width: 200px; height: 65px; border-style:solid; border-color:#0099FF; border-width: 1px; overflow-x:auto; overflow-y:hidden;" >
                                                <div id="divSampleText" class="relative-middle" style="text-align: center; ">
                                                    AaBbCcXxYyZz
                                                </div>
                                            </div>
                                           </div>
                                            <div id="divBorderSettings" runat="server" style="display:block; margin-left: 25px; font-size: small;" >
                                                <asp:Label ID="lblBorderSettings" runat="server" Text="Border Settings" ></asp:Label>
                                                <br />
                                                <div id="divBorderSettingsEntry" style=" display: inline-block; width: 200px; height: 185px; border-style:solid; border-color:#0099FF; border-width: 1px;">
                                                    <div id="divBorderStyle" style="display:block; margin:5px;">
                                                     <asp:Label ID="Label3" runat="server" Text="Border Style:" Font-Size="Small"></asp:Label>
                                                     <br />
                                                     <select id="lstBorderStyles" size="1" style="width: 173px; border-color: #0099ff; outline:none;" >
                                                        <option value="None">None</option>
                                                        <option value="Solid">Solid</option>
                                                        <option value="Dotted">Dotted</option>
                                                        <option value="Dashed">Dashed</option>
                                                        <option value="Double">Double</option>
                                                    </select>    
                                                </div>
                                                    <div id="divBorderWidth"  style="display:block; margin:5px;">
                                                    <label for="txtBorderWidth">Border Width:</label>
                                                    <br />
                                                    <input type="number" id="txtBorderWidth" name="txtBorderWidth" min="1" value="1" style="border: 1px solid #0099ff;width:60px;" />
                                                    px
                                                </div>
                                                    <div id="divBorderColor">
                                                         <div id="divlblBorderColor" style="margin-left: 5px">Border Color:</div>
                                                         <div id="divChooseBorderColor" style="border: 1px solid #666666; width: 170px; height: 26px; line-height: 26px; margin-left: 5px; border-radius:3px;" title="Choose border color from a list">
                                                        <table>
                                                            <tr>
                                                                <td >
                                                                    <div id="divShowBorderColor" style="border: none; width: 22px; height: 18px; border-radius:3px; background-color: black; margin-bottom:3px;"></div>
                                                                </td>
                                                                <td >
                                                                    <div id="divBorderColorName"  style="font-size:smaller; border: none; height: 18px; width:125px; text-align:left; margin-bottom:3px;">black</div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                   </div>
                                                </div>
                                                    <div id="divBorderCustomColor" >
                                                  <input id="btnBorderColor" type="color" value="black" style="height:1px; width:1px; border:none; "></input>
                                                  <input id="btnBorderColorCustomize" type="button" value="Custom Border Color" title="Customize border color" style="margin-left: 65px" />
                                               </div>
                                            </div>
                                        </div>
                                        </div>
                                     </td>
                                </tr>
                             </table>

                            <div id="divDlgBoxButtons" style="float:right;">
                                 <asp:Button ID="btnDlgBoxOK" runat="server" CssClass="dlgboxbutton" Text="OK" CausesValidation="false" UseSubmitBehavior="False" />
                                 <asp:Button ID="btnDlgBoxCancel" runat="server" CssClass="dlgboxbutton btnCancel" Text="Cancel" CausesValidation="false" UseSubmitBehavior="False" />
                            </div>
                        </div>
                    </div>
               </div>
               <%--***************************** Color lists ****************--%>
               <div id="divColorList" tabindex="0" class="ColorList overflowy" style="display:none; ">
                   <table id="tblColorList" style="width: 100%;">
                   
                   </table>
               </div>
               <div id="divBackColorList" tabindex="0" class="ColorList overflowy" style="display:none; ">
                   <table id="tblBackColorList" style="width: 100%;">
                   
                   </table>
               </div>
               <div id="divBorderColorList" tabindex="0" class="ColorList overflowy" style="display:none; ">
                   <table id="tblBorderColorList" style="width: 100%;">
                   
                   </table>
               </div>
               <%--************************************ Message Box Dialog ************************--%>
               <div id="divMsgBoxBackground" runat="server" class="PopupBackground" style="display: none; height:100%;width:100%;position:absolute;top:0px;left:0px;z-index: 2147483700">
                   <div id="divMsgBox" runat="server" class="Popup relative-middle">
                       <div id="divMsgBoxHeading" style=" font-size: small; text-align: center; line-height: 22px; background-color: gray; width: 100%; height: 22px; color: white; ">
                         <asp:Label ID="lblCaption" runat="server" Text="Message"></asp:Label>
                          <div id="divX" runat ="server" class="close" title="close dialog">&times;</div>
                       </div>
                       <div id="divMsgBoxBody"  class="clearfix"; style="margin:5px;" >
                           <table>
                               <tr>
                                   <td id="tdImage" style="vertical-align: top;">
                                       <asp:Image ID="imgInfo" runat="server" ImageUrl="Controls/Images/picInfo.Image.png"/>
                                       <asp:Image ID="imgError" runat="server" ImageUrl="Controls/Images/picError.Image.png"/>
                                       <asp:Image ID="imgWarning" runat="server" ImageUrl="Controls/Images/picExclaim.Image.png"/>
                                   </td>
                                   <td id="tdMessage" style="font-family: Tahoma; font-size: medium">
                                      <div id="divMsgBoxMessage" >
                                       No Message Sent
                                      </div>
                                   </td>
                               </tr>
                           </table>
                       </div>
                       <div id="divButton" class="divButton">
                           <asp:Button ID="btnOK" runat="server" CssClass="dlgboxbutton" Text="OK" CausesValidation="false"/>
<%--                           <asp:Button ID="btnCancel" runat="server" CssClass="dlgboxbutton" Text="Cancel" CausesValidation="False" Visible="False" UseSubmitBehavior="False" />
                           <asp:Button ID="Button1" runat="server" CssClass="dlgboxbutton" Text="Show Report" CausesValidation="False" Visible="False" UseSubmitBehavior="False" />
                           <asp:Button ID="Button2" runat="server" CssClass="dlgboxbutton" Text="Return to Designer" CausesValidation="False" Visible="False" UseSubmitBehavior="False" />--%>
                       </div>
                    </div>
              </div>
               <%--************************************ Tabular Column Width Dialog ************************--%>
               <div id="TabularWidthBackground" runat="server" class="PopupBackground" style="display: none; height:100%;width:100%;position:absolute;top:0px;left:0px;">
                    <div id="divTabularWidthDlg" runat="server" class="TabularWidthDlg relative-middle">
                       <div id="divTabularWidthHeading" style=" font-size: small; text-align: center; line-height: 24px; background-color: white; border-bottom: 1px solid #222222;width: 100%; height: 24px; color: #222222; ">
                         <asp:Label ID="lblTabularWidthHeading" runat="server" Text="Resize Tabular Column Width"></asp:Label>
                          <div id="divTabularWidthX" runat ="server" class="CloseColWidth" title="close dialog">&times;</div>
                       </div>

                       <div id="divTabularWidthBody"  style=" margin:5px; border: 1px solid #000000; display: inline-block; background-color: #FFFFFF; width: 99%; height: 100px; overflow-y:auto; overflow-x: scroll; background-color: #FFFFFF; white-space:nowrap" >
                       </div>
                       
                       <div id="divTip" class="tip">
                           Test
                       </div>

                        <div id="divSizeTip" class="sizetip">

                        </div>

                       <div id="divTabularWidthButtons" style="text-align:center;">
                         <asp:Button ID="btnSaveColWidths" runat="server" CssClass="dlgboxbutton" Text="Apply Changes" CausesValidation="false" UseSubmitBehavior="False" Width="90px" />
                         <asp:Button ID="btnCancelColWidths" runat="server" CssClass="dlgboxbutton btnCancel" Text="Cancel" CausesValidation="false" UseSubmitBehavior="False" />
                       </div>

                    </div>


               </div>
                <%-- ************************************ Image Field Settings Dialog ************************ --%>
                <div id="divImageFieldSettingsBackground" runat="server" class="PopupBackground" style="display:none; height:100%;width:100%;position:absolute;top:0px;left:0px;" >
                         <div id="divImageFieldSettingsDlg" runat="server" class="ImageFieldDlg relative-middle" style="position: relative; z-index: inherit">
                                <div id="divImageFieldDlgHeading" style=" font-size: small; text-align: center; line-height: 22px; background-color: gray; width: 100%; height: 22px; color: white; ">
                                      <asp:Label ID="lblImageFieldDlgHeading" runat="server" Text="Image Field Settings"></asp:Label>
                                     <div id="divImageFieldDlgHeadingX" runat ="server" class="close" title="close dialog">&times;</div>
                                </div>
                                <div id="divImageFieldDlgBody" class="clearfix" style="margin:5px; font-family: Tahoma; font-size: smaller;">
                                      <div id ="divImageSize" runat="server" >
                                            <div id="divImageSizeLabel" style="margin-left: 7px">
                                                     <asp:Label ID="lblImageSize" runat="server" Text="Image Size"></asp:Label>
                                             </div>
                                            <div id ="divSelectSize" runat="server" style="border: thin solid #0099ff; margin: 7px; width: 725px; height: 125px;">
                                                 <table id="tblImageSizes">
                                                     <tr>
                                                         <td id="td16x16">
                                                             <div id="div16x16" style="margin: 5px; width: 105px; height: auto; text-align: center; vertical-align: middle;">
                                                                 <asp:RadioButton ID="rb16x16" runat="server" Text="16 x 16" Font-Size="Smaller" GroupName="size" />
                                                             </div>
                                                         </td>
                                                         <td id="td24x24">
                                                             <div id="div24x24" style="margin: 5px; width: 105px; height: auto; text-align: center; vertical-align: middle;">
                                                                 <asp:RadioButton ID="rb24x24" runat="server" Text="24 x 24" Font-Size="Smaller" GroupName="size" />
                                                             </div>
                                                         </td>
                                                         <td id="td32x32">
                                                             <div id="div32x32" style="margin: 5px; width: 105px; height: auto; text-align: center; vertical-align: middle;">
                                                                 <asp:RadioButton ID="rb32x32" runat="server" Text="32 x 32" Font-Size="Smaller" GroupName="size" />
                                                             </div>
                                                         </td>
                                                         <td id="td64x64">
                                                             <div id="div64x64" style="margin: 5px; width: 105px; height: auto; text-align: center; vertical-align: middle;">
                                                                 <asp:RadioButton ID="rb64x64" runat="server" Text="64 x 64" Font-Size="Smaller" GroupName="size" />
                                                             </div>
                                                         </td>
                                                         <td id="td128x128">
                                                             <div id="div128x128" style="margin: 5px; width: 105px; height: auto; text-align: center; vertical-align: middle;">
                                                                 <asp:RadioButton ID="rb128x128" runat="server" Text="128 x 128" Font-Size="Smaller" GroupName="size" />
                                                             </div>
                                                         </td>
                                                         <td id="td256x256">
                                                             <div id="div256x256" style="margin: 5px; width: 105px; height: auto; text-align: center; vertical-align: middle;">
                                                                 <asp:RadioButton ID="rb256x256" runat="server" Text="256 x 256" Font-Size="Smaller" GroupName="size" />
                                                             </div>
                                                         </td>
                                                     </tr>
                                                 </table>
                                                 <div id="divCustomImageSize" style="margin-left: 22px">
                                                       <table>
                                                           <tr>
                                                               <td>
                                                                      <asp:RadioButton ID="rbCustom" runat="server" Text="Custom" Font-Size="Small" GroupName="size" />       
                                                               </td>
                                                               <td>
                                                                    <div id ="divSizeOptions" style="margin-left: 50px">
                                                                           <div id="SizeOptionsLabel" style="font-size: small; margin-bottom: 3px;">Size Options:</div>
                                                                           <select id="lstSizeOptions" size="1" style="width: 185px; border-color: #0099ff; outline: none;" tabindex="0">
                                                                               <option value="Square">Square</option>
                                                                               <option value="KeepAspectRatio">Original Aspect Ratio</option>
                                                                                <option value="FreeForm">Free Form</option>
                                                                          </select>    

                                                                           <%--<asp:CheckBox id="ckbSquare" runat="server" Text="Square" Checked="True" Enabled="False" />--%>
                                                                   </div>
                                                               </td>
                                                               <td>
                                                                     <div id="CustomSizeEntry " style="margin-left: 42px; font-size: small; margin-top: 5px;">
                                                                        <table>
                                                                            <tr>
                                                                                <td>
                                                                                    <div id="divImageWidth" style="margin-bottom: 5px;" >
                                                                                           <asp:Label ID="lblImageWidth" runat ="server" Text="Width:"> </asp:Label>
                                                                                    </div>
                                                                                </td>
                                                                                <td>
                                                                                     <input type="number" id="txtImageWidth" name="txtImageWidth" min="16" value="" style="border: 1px solid #0099ff;width:60px;" disabled="disabled" tabindex="0" />
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <div id="divImageHeight" style="margin-bottom: 5px"; >
                                                                                           <asp:Label ID="lblImageHeight" runat ="server" Text="Height:"> </asp:Label>
                                                                                    </div>
                                                                                </td>
                                                                                <td>
                                                                                     <input type="number" id="txtImageHeight" name="txtImageHeight" min="16" value="" style="border: 1px solid #0099ff;width:60px;" disabled="disabled" tabindex="0" />
                                                                                </td>
                                                                            </tr>
                                                                        </table>

                                                                     </div>

                                                               </td>
                                                           </tr>
                                                       </table>
                                                 </div>
                                          </div>
                                      </div>
                                      <div id="divSelectImageFile" style="text-align: center; vertical-align: middle">
                                            <asp:Button ID="btnSelectImageFile" runat="server" CssClass="imagefilebutton" Text="Select Image File" CausesValidation="false" UseSubmitBehavior="False" />
                                      </div>
                                      <div id="divChosenFile" style="text-align: center; vertical-align: middle; font-size: 14px; height: 28px; margin-top: 10px;">

                                      </div>
                                      <div id="divUploadImageFile" style="text-align: center; vertical-align: middle; display:none;">
                                            <asp:Button ID="btnUploadImageFile" runat="server" CssClass="imagefilebutton" Text="Upload Image File" CausesValidation="false" UseSubmitBehavior="False" />
                                      </div>
                                      <div id="divSampleImage" >
                                          <div id="divSampleImageText" style="float:left;"> 
                                              <br /><br />
                                               <div id="divText">&nbsp;&nbsp;&nbsp; Sample</div>
                                          </div>
                                          <div id="divRefresh" style="float: right; text-align: center; margin-right: 10px; display:none;">
                                                <asp:Image id="imgRefresh" runat="server"  ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/ReportDesignerImages/Refresh.ico" Height="32px" Width="32px" />
                                                        <%--<img id ="imgRefresh" runat="server"  src="~/Controls/Images/ReportDesignerImages/Refresh.ico"  alt="Refresh Image" height="32" width="32" />--%>
                                                <br />
                                                Refresh Image
                                                   <%--<img id="imgRefresh" src="~/Controls/Images/WaitImage2.gif" style="width: 32px; height: 32px" />--%>
                                          </div> 
                                          <hr id="hrImageLine1" style="border: thin solid #626262; width: 100%; height: auto; margin: 1px 3px 1px 3px" />
                                          <div id="divShowImage" style="height: 325px; text-align: center; vertical-align: middle;">
                                                <div id="divImageSample" class="relative-middle" style="padding: 4px; border-style: none; border-color: black; border-width: 1px; width: 74px; height:74px;">
                                                   <%--<asp:Image id="imgSample" runat="server"  ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/ReportDesignerImages/Weather256x256.ico" Height="64px" Width="64px" />--%>
                                                           <img id="imgSample" runat="server"  src="~/Controls/Images/ReportDesignerImages/Weather256x256.ico"  alt="Sample Image" width="64" height="64" />

                                                </div>
                                          </div>
                                          <hr id="hrImageLine2" style="border: thin solid #626262; width: 100%; height: auto; margin: 1px 3px 1px 3px" />
                                      </div>
                                     <div id="divImageFieldBorderSettings" runat="server" style="display: inline-block; margin-left: 10px; margin-top:10px;font-size:small;">
                                           <asp:Label ID="lblImageFieldBorderSettings" runat="server" Text="Border Settings" ></asp:Label>
                                           <br />
                                           <div id="divImageFieldBorderSettingsEntry" style="display: inline-block; height: 70px; border-style:solid; border-color:#0099FF; border-width: 1px; width: 720px;">
                                                  <table>
                                                      <tr>
                                                          <td>
                                                                 <div id="divImageFieldBorderStyle" style="display: block;  margin-top: 5px; margin-left: 55px; margin-right: 10px;" >
                                                                     <div id="ImageFieldBorderStyleLabel" style="font-size: small; margin-bottom: 3px;">Border Style:</div>
                                                                    <select id="lstImageFieldBorderStyles" size="1" style="width: 100px; border-color: #0099ff; outline: none;" tabindex="0">
                                                                       <option value="None">None</option>
                                                                       <option value="Solid">Solid</option>
                                                                        <option value="Dotted">Dotted</option>
                                                                        <option value="Dashed">Dashed</option>
                                                                        <option value="Double">Double</option>
                                                                   </select>    
                                                                 </div>
                                                          </td>
                                                          <td>
                                                                  <div id="divImageFieldBorderWidth"  style="display: block; margin-top: 5px;margin-right: 10px; ">
                                                                      <div id="ImageFieldBorderWidthLabel" style="font-size: small">Border Width:</div>
                                                                      <input type="number" id="txtImageFieldBorderWidth" name="txtImageFieldBorderWidth" min="1" value="1" style="border: 1px solid #0099ff;width:100px;" tabindex="0" />
                                                                      px
                                                                  </div>
                                                          </td>
                                                          <td>
                                                                   <div id="divImageFieldBorderColor" style="display:block;margin-top: 5px;">
                                                                         <div id="divlblImageFieldBorderColor" style="margin-left: 5px; font-size: small;">Border Color:</div>
                                                                         <div id="divImageFieldChooseBorderColor" style="border: 1px solid #0099ff; width: 170px; height: 26px; line-height: 26px; margin-left: 5px; border-radius:3px; font-size: small;" title="Choose border color from a list" tabindex="0">
                                                                       <table>
                                                                            <tr>
                                                                                   <td>
                                                                                      <div id="divImageFieldShowBorderColor" style="border: none; width: 22px; height: 18px; border-radius:3px; background-color: black; margin-bottom:3px;"></div>
                                                                                 </td>
                                                                                <td>
                                                                                    <div id="divImageFieldBorderColorName"  style="font-size:smaller; border: none; height: 18px; width:125px; text-align:left; margin-bottom:3px;">black</div>
                                                                               </td>
                                                                          </tr>
                                                                     </table>
                                                                     </div>
                                                               </div>
                                                          </td>
                                                          <td>
                                                                <div id="divImageFieldBorderCustomColor">
                                                                       <input id="btnImageFieldBorderColor" type="color" style="height:1px; width:1px; border:none; " value="#000000" tabindex="-1"></input>
                                                                       <input id="btnImageFieldBorderColorCustomize" type="button" value="Custom Border Color" title="Customize border color" />
                                                               </div>
                                                          </td>
                                                      </tr>
                                                  </table>
                                           </div>


                                     
                                        </div>
                                     <div id="divImageFieldDlgBoxButtons" style="float:right;">
                                           <asp:Button ID="btnImageFieldDlgBoxOK" runat="server" CssClass="dlgboxbutton" Text="OK" CausesValidation="false" UseSubmitBehavior="False" />
                                           <asp:Button ID="btnImageFieldDlgBoxCancel" runat="server" CssClass="dlgboxbutton btnCancel" Text="Cancel" CausesValidation="false" UseSubmitBehavior="False" />
                                     </div>

                                </div>
                          </div>;

                </div>

               <ucMsgBox:Msgbox id="MessageBox" runat ="server" > </ucMsgBox:Msgbox>
            </div>

          </ContentTemplate>

         <Triggers> 
            <asp:PostBackTrigger ControlID="btnUploadImageFile"/>            
        </Triggers>

        </asp:UpdatePanel>

         <asp:FileUpload id="FileRDL" runat ="server" AllowMultiple="false" accept =".bmp,.jpg,.jpeg,.png,.gif,.ico,.svg" style="display:none;" />

        <%--<input  type="file" id="FileRDL" name="FileRDL" runat="server" accept =".ico,.bmp,.jpg,.jpeg,.png,.gif,.svg" style="display:none;"/>--%>

        <div id="spinner" class="modal" style="display:none;">
             <div id="divSpinner" style="text-align: center; width: 130px; display: inline-block; clip: rect(auto, auto, auto, 50%); position: fixed; margin-top: 300px; margin-right: auto; padding-top: 10px; background-color: #f8f8d3; z-index: 2147483650; left: 50%; top: -5%;">
                  <img id="imgSpinner" src="Controls/Images/WaitImage2.gif" style="width: 100px; height: 100px" />
                    <br />
                      Please Wait...
             </div>
        </div>

        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="updReportDesigner">
            <ProgressTemplate >
            <div class="modal">
                <div class="center">
                    <asp:Image ID="imgProgress" runat="server"  ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />
                    Please Wait...
                </div>
            </div>
            </ProgressTemplate>
        </asp:UpdateProgress>

    <script type="text/javascript">
        var isLoaded = false;
        var fontData = new FontData();
        var reportView = new ReportView()
        ;
        reportView.ReportTemplate = enumReportTemplate.Tabular;
        function populateReportView(data) {
            if (data != null && !isLoaded) {
                data = data.replace(/\\/g, "");
                ReportView.ParseJSON(JSON.parse(data), reportView);
                //isLoaded = true;
            }
        }
    </script>

    </form>
</body>
</html>
