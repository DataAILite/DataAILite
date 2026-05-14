<%@ Page Language="VB" AutoEventWireup="false" CodeFile="MarketProfit.aspx.vb" Inherits="MarketProfit" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Market Profitability Model</title>
    <style type="text/css">
        .NodeStyle { color:#0066FF; font-size:12px; font-weight:normal; text-decoration:none; }
        .NodeStyle:hover { text-decoration:underline; color:darkblue; }
        .ticketbutton { width:90px; height:25px; font-size:12px; border-radius:5px; border-style:solid; border-color:#4e4747; color:black; border-width:1px; background-image:linear-gradient(to bottom, rgba(211,211,211,0),rgba(211,211,250,3)); padding:3px; margin:5px; }
        .aiLinkButton { display:inline-block; width:90px; height:25px; line-height:17px; padding:3px; margin:5px; border-style:solid; border-color:#4e4747; border-width:1px; border-radius:5px; background-image:linear-gradient(to bottom, rgba(211,211,211,0),rgba(211,211,250,3)); color:#0066FF; font-size:12px; font-weight:bold; text-align:center; text-decoration:none; box-sizing:border-box; vertical-align:middle; }
        .aiLinkButton:hover { color:#004DCC; text-decoration:none; }
        .modal { position:fixed; z-index:2147483647; height:100%; width:100%; top:0; background-color:#f8f8d3; opacity:0.8; }
        .center { z-index:2147483647; margin:300px auto; padding-left:25px; padding-top:10px; width:130px; background-color:#f8f8d3; border-radius:10px; }
        .center img { height:100px; width:100px; }
        .marketgrid { font-family:Arial; font-size:12px; border-collapse:collapse; background-color:white; }
        .marketgrid th { background-color:#663300; color:white; border:1px solid white; padding:4px; white-space:nowrap; }
        .marketgrid td { border:1px solid #d0d0d0; padding:4px; white-space:nowrap; }
        .controlpanel { background-color:#e5e5e5; border:medium double #FFFFFF; color:black; font-family:Arial; font-size:small; width:auto; min-width:1080px; max-width:1260px; margin-left:0; margin-right:auto; }
        .marketTitleBlock { width:auto; min-width:1080px; max-width:1260px; margin-left:0; margin-right:auto; text-align:center; font-family:Arial; }
        .marketSubtitle { display:block; color:#555555; font-size:12px; line-height:17px; margin-top:4px; }
        .fieldLabel { font-weight:bold; text-align:left; white-space:nowrap; width:115px; }
        .fieldControl { text-align:left; white-space:nowrap; width:210px; }
        .marketExplanation { margin-top:10px; width:auto; min-width:1080px; max-width:1260px; font-family:Arial; font-size:12px; color:#333333; line-height:17px; }
        .marketExplanation span { display:block; margin-bottom:4px; }
    </style>
</head>
<body>
<form id="form1" runat="server">
<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
<asp:UpdatePanel ID="udpMarket" runat="server">
<ContentTemplate>
<table>
<tr><td colspan="3" style="font-size:x-large; font-weight:bold; background-color:#e5e5e5; height:40px;"><asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label></td></tr>
<tr>
<td style="font-size:x-small; background-color:#e5e5e5; vertical-align:top; text-align:left; width:15%;">
<asp:TreeView ID="TreeView1" runat="server" Width="100%" NodeIndent="10" Font-Names="Times New Roman" EnableTheming="True" ImageSet="BulletedList" OnSelectedNodeChanged="TreeView1_SelectedNodeChanged">
          <Nodes>
                                        <asp:TreeNode Text="&lt;b&gt;Log Off&lt;/b&gt;" Value="Default.aspx" Expanded="True"></asp:TreeNode>
                                        <asp:TreeNode Text="&lt;b&gt;List of Reports&lt;/b&gt;" Value="ListOfReports.aspx" Expanded="True"></asp:TreeNode>
                                        <asp:TreeNode Text="&lt;b&gt;Report Definition&lt;/b&gt;" Value="ReportEdit.aspx?tne=2" Expanded="False">
                                            <asp:TreeNode Text="Report Parameters" Value="ReportEdit.aspx?tne=3"></asp:TreeNode>
                                            <asp:TreeNode Text="Share Report (Users)" Value="ReportEdit.aspx?tne=4"></asp:TreeNode>
                                        </asp:TreeNode>
                                        <asp:TreeNode Text="&lt;b&gt;Report Data Query&lt;/b&gt;" Value="SQLquery.aspx?tnq=0" Expanded="False">
                                            <asp:TreeNode Text="Data fields" Value="SQLquery.aspx?tnq=0"></asp:TreeNode>
                                            <asp:TreeNode Text="Joins" Value="SQLquery.aspx?tnq=1"></asp:TreeNode>
                                            <asp:TreeNode Text="Filters" Value="SQLquery.aspx?tnq=2"></asp:TreeNode>
                                            <asp:TreeNode Text="Sorting" Value="SQLquery.aspx?tnq=3"></asp:TreeNode>
                                        </asp:TreeNode>
                                        <asp:TreeNode Text="&lt;b&gt;Report Format Definition&lt;/b&gt;" Value="RDLformat.aspx?tnf=0" Expanded="False">
                                            <asp:TreeNode Text="Columns, Expressions" Value="RDLformat.aspx?tnf=0"></asp:TreeNode>
                                            <asp:TreeNode Text="Groups, Total" Value="RDLformat.aspx?tnf=1"></asp:TreeNode>
                                            <asp:TreeNode Text="Combine Values" Value="RDLformat.aspx?tnf=2"></asp:TreeNode>
                                            <asp:TreeNode Text="Advanced Report Designer" Value="ReportDesigner.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Map Definition" Value="MapReport.aspx"></asp:TreeNode>
                                        </asp:TreeNode>
                                        <asp:TreeNode Text="Explore Report Data" Value="ShowReport.aspx?srd=0" Expanded="False">
                                            <asp:TreeNode Text="Export Data to Excel" Value="datatoExcel" NavigateUrl="ShowReport.aspx?srd=1"></asp:TreeNode>
                                            <asp:TreeNode Text="Export Data to CSV" Value="datatoCSV" NavigateUrl="ShowReport.aspx?srd=2"></asp:TreeNode>
                                            <asp:TreeNode Text="Export Data to Delimited File" Value="ShowReport" NavigateUrl="ShowReport.aspx?srd=10"></asp:TreeNode>
                                            <asp:TreeNode Text="Export Data to XML" Value="datatoXML" NavigateUrl="ShowReport.aspx?srd=14"></asp:TreeNode>
                                        </asp:TreeNode>
                                        <asp:TreeNode Text="Show Report" Value="ShowReport.aspx?srd=3" Expanded="True">
                                            <asp:TreeNode Text="Show Generic Report" Value="ReportViews.aspx?gen=yes"></asp:TreeNode>
                                            <asp:TreeNode Text="Show Report Charts" Value="ShowReport.aspx?srd=17"></asp:TreeNode>
                                            <asp:TreeNode Text="Chart Recommendations" Value="ChartRecommendationHelpers.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Map Report" Value="MapReport.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Map Readiness" Value="MapReadines.aspx"></asp:TreeNode>

                                            <asp:TreeNode Text="Export Report to Excel" Value="reptoExcel" NavigateUrl="ShowReport.aspx?srd=4"></asp:TreeNode>
                                            <asp:TreeNode Text="Export Report to Word" Value="reptoWord" NavigateUrl="ShowReport.aspx?srd=5"></asp:TreeNode>
                                            <asp:TreeNode Text="Export Report to PDF" Value="reptoPDF" NavigateUrl="ShowReport.aspx?srd=6"></asp:TreeNode>
                                            <asp:TreeNode Text="Export Packages" Value="ExportPackages.aspx"></asp:TreeNode>
                                        </asp:TreeNode>
                                        <asp:TreeNode Text="Analytics Dashboard" Value="DataAdmin.aspx" NavigateUrl="DataAdmin.aspx" Expanded="True">
                                            <asp:TreeNode Text="Detail Analytics" Value="Analytics.aspx" NavigateUrl="Analytics.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="See Data Overall Statistics" Value="ShowReport.aspx?srd=8"></asp:TreeNode>
                                            <asp:TreeNode Text="Export Overall Statistics to Excel" Value="reptoExcel" NavigateUrl="ShowReport.aspx?srd=9"></asp:TreeNode>
                                            <asp:TreeNode Text="See Groups Statistics" Value="ReportViews.aspx?grpstats=yes"></asp:TreeNode>
                                            <asp:TreeNode Text="See Fields Correlation" Value="ShowReport.aspx?srd=12"></asp:TreeNode>
                                            <asp:TreeNode Text="Correlation Threshold" Value="CorrelationThreshold.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Matrix Balancing" Value="ShowReport.aspx?srd=13"></asp:TreeNode>
                                            <asp:TreeNode Text="Pivot / Cross Tab" Value="Pivot.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Variance Analysis" Value="Variance.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Comparison Reports" Value="ComparisonReports.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Data Profiling" Value="Profiling.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Data Quality" Value="DataQuality.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Ranking Analysis" Value="Ranking.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Regression Analysis" Value="Regression.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Time Based Summaries" Value="TimeBasedSummaries.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Time Series" Value="TimeSeries.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Outlier Flagging" Value="OutlierFlagging.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Audit Summaries" Value="AuditSummaries.aspx"></asp:TreeNode>
                                        </asp:TreeNode>
                                        <asp:TreeNode Text="Market Dashboard" Value="MarketAdmin.aspx" NavigateUrl="MarketAdmin.aspx" Expanded="True">

                                            <asp:TreeNode Text="Market Demand" Value="MarketDemand.aspx" NavigateUrl="MarketDemand.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Market Pricing" Value="MarketPricing.aspx" NavigateUrl="MarketPricing.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Market Elasticity" Value="MarketElasticity.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Market Basket" Value="MarketBasket.aspx" NavigateUrl="MarketBasket.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Market Segments" Value="MarketSegments.aspx" NavigateUrl="MarketSegments.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Market Churn" Value="MarketChurn.aspx" NavigateUrl="MarketChurn.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Market Risk" Value="MarketRisk.aspx" NavigateUrl="MarketRisk.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Market Inventory" Value="MarketInventory.aspx" NavigateUrl="MarketInventory.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Market Profit" Value="MarketProfit.aspx" NavigateUrl="MarketProfit.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Market Scenario" Value="MarketScenario.aspx" NavigateUrl="MarketScenario.aspx"></asp:TreeNode>
                                        </asp:TreeNode>
                                    </Nodes>
<RootNodeStyle HorizontalPadding="2px" Font-Bold="True" Font-Underline="False" /><NodeStyle CssClass="NodeStyle" /><ParentNodeStyle Font-Bold="True" />
</asp:TreeView>
</td>
<td style="width:5px;"></td>
<td style="width:85%; text-align:left; vertical-align:top;">
<asp:HyperLink ID="HyperLinkAnalytics" runat="server" NavigateUrl="~/Analytics.aspx" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Open the main Analytics page for this report.">Analytics</asp:HyperLink>
&nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkReport" runat="server" NavigateUrl="~/ShowReport.aspx?srd=3" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Open the report and chart views for this report.">Report and Charts</asp:HyperLink>
&nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx?hilt=Market%20Profit" Target="_blank" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Open help for market pages.">Help</asp:HyperLink>
&nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkLogOff" runat="server" NavigateUrl="~/Default.aspx" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Log off and clear the current session.">Log off</asp:HyperLink>
<br /><br />
<div class="marketTitleBlock"><asp:Label ID="lblHeader" runat="server" Font-Size="22px" Font-Names="Arial" ToolTip="Build market and business model analysis using the selected report fields.">Market Profitability Model</asp:Label><asp:Label ID="LabelMarketSubtitle" runat="server" CssClass="marketSubtitle" ToolTip="This text explains the market model used by this page." Text="Shows profitability drivers. Primary Field can combine multiple fields; Value Field is treated as revenue or value. Cost is calculated from available cost fields, unit cost and quantity fields, or Assumption % as fallback, and the grid shows cost source, cost rate, margin, contribution, and profit notes."></asp:Label></div>
<br />
<table class="controlpanel" cellpadding="4" cellspacing="0">
<tr><td class="fieldLabel" title="Select one or more category, product, customer, location, or other fields used to group the market model.">Primary field(s):</td><td class="fieldControl"><asp:Panel ID="pnlPrimarySingle" runat="server"><asp:DropDownList ID="DropDownDimension" runat="server" ToolTip="Product, category, region, customer, or other demand dimension."></asp:DropDownList></asp:Panel><asp:Panel ID="pnlPrimaryMulti" runat="server" Visible="False"><asp:ListBox ID="ListBoxDimension" runat="server" SelectionMode="Multiple" Rows="4" Width="210px" ToolTip="Select one or more fields to combine as the primary market grouping."></asp:ListBox></asp:Panel></td><td class="fieldLabel" title="Select the numeric field used for value, sales, quantity, revenue, profit, price, or score calculations.">Value field:</td><td class="fieldControl"><asp:DropDownList ID="DropDownValueField" runat="server" ToolTip="Quantity, sales, revenue, or other demand value."></asp:DropDownList></td></tr>
<tr><td class="fieldLabel"><asp:Panel ID="pnlDateLabel" runat="server"><span title="Optionally select the date field used for activity, churn, retention, or time-aware market calculations.">Date field:</span></asp:Panel></td><td class="fieldControl"><asp:Panel ID="pnlDateControl" runat="server"><asp:DropDownList ID="DropDownDateField" runat="server" ToolTip="Optional date field for models that use activity dates."></asp:DropDownList></asp:Panel></td><td class="fieldLabel"><asp:Panel ID="pnlSecondaryLabel" runat="server"><span title="Optionally select a second field such as order, customer, quantity, segment, or category.">Secondary field:</span></asp:Panel></td><td class="fieldControl"><asp:Panel ID="pnlSecondaryControl" runat="server"><asp:DropDownList ID="DropDownSecondaryField" runat="server" ToolTip="Optional second field such as order, customer, or quantity."></asp:DropDownList></asp:Panel></td></tr>
<tr><td class="fieldLabel" title="Enter a percentage assumption used by scenario or business model calculations.">Assumption %:</td><td class="fieldControl"><asp:TextBox ID="txtAssumption" runat="server" Width="70px" ToolTip="Percent change or business assumption used by scenario-style calculations.">10</asp:TextBox></td><td colspan="2"></td></tr>
<tr><td align="left" colspan="4" style="font-weight:bold;"><span title="Filter the market model result grid by text.">Search:</span> <asp:TextBox ID="txtSearch" runat="server" Width="260px" ToolTip="Type text to filter the market model results shown in the grid."></asp:TextBox><asp:Button ID="ButtonBuild" runat="server" CssClass="ticketbutton" Text="Build" OnClick="ButtonBuild_Click" ToolTip="Build the market model using the selected fields and assumptions." /><asp:Button ID="ButtonReset" runat="server" CssClass="ticketbutton" Text="Reset" OnClick="ButtonReset_Click" ToolTip="Clear search and restore default market model assumptions." /><asp:Button ID="ButtonExportCSV" runat="server" CssClass="ticketbutton" Text="CSV" OnClick="ButtonExportCSV_Click" ToolTip="Export the current market model results to CSV." /><asp:Button ID="ButtonExportExcel" runat="server" CssClass="ticketbutton" Text="Excel" OnClick="ButtonExportExcel_Click" ToolTip="Export the current market model results to Excel." /><asp:LinkButton OnClientClick="return showWaitingPanel();" ID="lnkMarketAI" runat="server" CssClass="aiLinkButton" Font-Names="Arial" OnClick="lnkMarketAI_Click" ToolTip="Ask AI to interpret the current market model results, opportunities, risks, and recommended actions.">AI</asp:LinkButton></td></tr>
</table>
<asp:Label ID="LabelError" runat="server" ForeColor="Red" Font-Names="Arial" Font-Size="Medium"></asp:Label><br />
<asp:Label ID="LabelInfo" runat="server" ForeColor="Black" Font-Names="Arial" Font-Size="Small"></asp:Label><br /><br />
<div style="font-family:Arial; font-size:small; padding-bottom:6px;"><asp:LinkButton ID="LinkButtonPrevious" runat="server" Font-Size="Small" OnClick="LinkButtonPrevious_Click">Previous</asp:LinkButton>&nbsp;&nbsp;<asp:Label ID="LabelPageNumberCaption" runat="server" Font-Names="Arial" Font-Size="Small" Text="Page Number"></asp:Label><asp:TextBox ID="TextBoxPageNumber" runat="server" Width="35px" Font-Names="Arial" Font-Size="Small" AutoPostBack="True" OnTextChanged="TextBoxPageNumber_TextChanged"></asp:TextBox><asp:Label ID="LabelPageCount" runat="server" Font-Names="Arial" Font-Size="Small"></asp:Label>&nbsp;&nbsp;<asp:LinkButton ID="LinkButtonNext" runat="server" Font-Size="Small" OnClick="LinkButtonNext_Click">Next</asp:LinkButton></div>
<div style="overflow:auto; max-width:100%;"><asp:GridView ID="GridViewMarket" runat="server" CssClass="marketgrid" AutoGenerateColumns="True" GridLines="Both" ToolTip="Market model results. Click record counts to open the corresponding records in Data Explorer."></asp:GridView></div>
<div class="marketExplanation">
<asp:Label ID="LabelModelExplanation" runat="server"></asp:Label>
<asp:Label ID="LabelAlgorithmExplanation" runat="server"></asp:Label>
<asp:Label ID="LabelOutputExplanation" runat="server"></asp:Label>
</div>
</td>
</tr>
</table>
</ContentTemplate>
<Triggers><asp:PostBackTrigger ControlID="ButtonExportCSV" /><asp:PostBackTrigger ControlID="ButtonExportExcel" /><asp:PostBackTrigger ControlID="lnkMarketAI" /></Triggers>
</asp:UpdatePanel>
<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpMarket"><ProgressTemplate><div class="modal"><div class="center"><asp:Image ID="imgProgress" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />Please Wait...</div></div></ProgressTemplate></asp:UpdateProgress>
        <div id="ManualWaitingPanel" class="modal" style="display:none;">
            <div class="center">
                <img src="Controls/Images/WaitImage2.gif" alt="Please Wait" />
                Please Wait...
            </div>
        </div>
</form>
    <script type="text/javascript">
        function showWaitingPanel() {
            var waitingPanel = document.getElementById('ManualWaitingPanel');
            if (waitingPanel) {
                waitingPanel.style.display = 'block';
            }
            return true;
        }

        window.addEventListener('pageshow', function () {
            var waitingPanel = document.getElementById('ManualWaitingPanel');
            if (waitingPanel) {
                waitingPanel.style.display = 'none';
            }
        });
    </script>
</body>
</html>
