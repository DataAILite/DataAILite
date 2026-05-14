<%@ Page Language="VB" AutoEventWireup="false" CodeFile="MarketAdmin.aspx.vb" Inherits="MarketAdmin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Market Dashboard</title>
    <style type="text/css">
        .NodeStyle { color:#0066FF; font-size:12px; font-weight:normal; text-decoration:none; }
        .NodeStyle:hover { text-decoration:underline; color:darkblue; }
        .modal { position:fixed; z-index:2147483647; height:100%; width:100%; top:0; background-color:#f8f8d3; opacity:0.8; }
        .center { z-index:2147483647; margin:300px auto; padding-left:25px; padding-top:10px; width:130px; background-color:#f8f8d3; border-radius:10px; }
        .center img { height:100px; width:100px; }
        .dashboard { font-family:Arial; margin:0; max-width:1180px; text-align:left; }
        .dashboardHeader { margin:14px 0 12px 0; }
        .dashboardTitle { display:block; color:#333333; font-size:22px; font-weight:normal; margin-bottom:4px; }
        .dashboardSubTitle { color:#666666; font-size:12px; }
        .tileGrid { display:grid; grid-template-columns:repeat(auto-fill, minmax(280px, 1fr)); gap:10px; align-items:stretch; }
        .marketTile { display:block; min-height:124px; border:1px solid #bfbfbf; border-radius:4px; background-color:#ffffff; color:#222222; text-decoration:none; box-shadow:0 1px 2px rgba(0,0,0,0.08); padding:0; }
        .marketTile:hover { border-color:#0066FF; box-shadow:0 2px 8px rgba(0,0,0,0.16); }
        .tileGrid .marketTile:nth-child(1) { background-color:#f7fbff; }
        .tileGrid .marketTile:nth-child(2) { background-color:#f8fff7; }
        .tileGrid .marketTile:nth-child(3) { background-color:#fffaf2; }
        .tileGrid .marketTile:nth-child(4) { background-color:#fbf8ff; }
        .tileGrid .marketTile:nth-child(5) { background-color:#f6fcfb; }
        .tileGrid .marketTile:nth-child(6) { background-color:#fff7f8; }
        .tileGrid .marketTile:nth-child(7) { background-color:#f9fbf1; }
        .tileGrid .marketTile:nth-child(8) { background-color:#f4faff; }
        .tileGrid .marketTile:nth-child(9) { background-color:#fff8f3; }
        .tileGrid .marketTile:nth-child(10) { background-color:#f7f6ff; }
        .tileCaption { display:block; padding:8px 10px 2px 10px; border-bottom:0; background-color:transparent; }
        .tileTitle { display:block; color:#222222; font-size:14px; font-weight:bold; line-height:18px; }
        .tileText { display:block; color:#555555; font-size:11px; line-height:15px; min-height:0; }
        .tileBody { display:block; padding:4px 10px 3px 10px; color:#333333; font-size:12px; line-height:17px; }
        .openText { display:block; margin:5px 10px 10px 10px; color:#0066FF; font-size:12px; font-weight:bold; }
        .previewBox { display:block; margin:5px 10px 2px 10px; border:1px solid #cfcfcf; background-color:rgba(255,255,255,0.82); max-height:136px; overflow:hidden; }
        .previewTable { width:100%; border-collapse:collapse; font-family:Arial; font-size:9px; table-layout:fixed; }
        .previewTable th { border:1px solid #d9d9d9; background-color:#f8f8f8; color:#333333; font-weight:bold; padding:1px; white-space:nowrap; overflow:hidden; text-overflow:ellipsis; }
        .previewTable td { border:1px solid #e1e1e1; color:#222222; padding:1px; white-space:nowrap; overflow:hidden; text-overflow:ellipsis; }
        .previewEmpty { display:block; color:#777777; font-size:11px; padding:8px; }
        .suitabilityBox { margin:8px 0 12px 0; max-width:1180px; overflow:auto; }
        .suitabilityTable { width:100%; border-collapse:collapse; font-family:Arial; font-size:11px; background-color:#f3fff3; }
        .suitabilityTable th { background-color:#663300; color:white; border:1px solid white; padding:4px; text-align:left; white-space:nowrap; }
        .suitabilityTable td { border:1px solid #d0d0d0; color:#222222; padding:4px; vertical-align:top; background-color:#f3fff3; }
        .statusGood { color:#006600; font-weight:bold; }
        .statusPartial { color:#996600; font-weight:bold; }
        .statusMissing { color:#990000; font-weight:bold; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <asp:UpdatePanel ID="udpMarketAdmin" runat="server">
            <ContentTemplate>
                <table>
                    <tr>
                        <td colspan="3" style="font-size:x-large; font-weight:bold; background-color:#e5e5e5; vertical-align:middle; text-align:left; height:40px;">
                            <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="font-size:x-small; background-color:#e5e5e5; vertical-align:top; text-align:left; width:15%;">
                            <asp:TreeView ID="TreeView1" runat="server" Width="100%" NodeIndent="10" Font-Names="Times New Roman" EnableTheming="True" ImageSet="BulletedList">
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
                                <RootNodeStyle HorizontalPadding="2px" Font-Bold="True" Font-Underline="False" />
                                <NodeStyle CssClass="NodeStyle" />
                                <ParentNodeStyle Font-Bold="True" />
                            </asp:TreeView>
                        </td>
                        <td style="width:5px;"></td>
                        <td style="width:85%; text-align:left; vertical-align:top;">
                            <asp:HyperLink ID="HyperLinkAnalytics" runat="server" NavigateUrl="~/Analytics.aspx" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Open the main Analytics page for this report.">Analytics</asp:HyperLink>
                            &nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkDataAdmin" runat="server" NavigateUrl="~/DataAdmin.aspx" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Open the Analytics Dashboard.">Analytics Dashboard</asp:HyperLink>
                            &nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkReport" runat="server" NavigateUrl="~/ShowReport.aspx?srd=3" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Open the report and chart views for this report.">Report and Charts</asp:HyperLink>
                            &nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx?hilt=Market%20Dashboard" Target="_blank" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Open help for market pages.">Help</asp:HyperLink>
                            &nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkLogOff" runat="server" NavigateUrl="~/Default.aspx" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Log off and clear the current session.">Log off</asp:HyperLink>

                            <div class="dashboard">
                                <div class="dashboardHeader">
                                    <asp:Label ID="lblHeader" runat="server" CssClass="dashboardTitle" Text="Market Dashboard" ToolTip="Open market and business model pages for the current report data."></asp:Label>
                                    <asp:Label ID="LabelDescription" runat="server" CssClass="dashboardSubTitle" Text="Market and business model pages for demand, pricing, elasticity, basket, segmentation, churn, risk, inventory, profit, and scenarios."></asp:Label>
                                </div>

                                <div class="suitabilityBox">
                                    <asp:Literal ID="litMarketSuitability" runat="server"></asp:Literal>
                                </div>

                                <div class="tileGrid">
                                    <a id="tileDemand" runat="server" class="marketTile" href="MarketDemand.aspx" title="Open Market Demand">
                                        <span class="tileCaption"><span class="tileTitle">Market Demand</span><span class="tileText">Demand values, share, records, and projected demand.</span></span>
                                        <span class="tileBody">Review categories, products, regions, or customers by demand value.</span>
                                        <span class="previewBox"><asp:Literal ID="litPreviewDemand" runat="server"></asp:Literal></span>
                                        <span class="openText">Open</span>
                                    </a>
                                    <a id="tilePricing" runat="server" class="marketTile" href="MarketPricing.aspx" title="Open Market Pricing">
                                        <span class="tileCaption"><span class="tileTitle">Market Pricing</span><span class="tileText">Price bands, average quantity, average revenue, and sensitivity notes.</span></span>
                                        <span class="tileBody">Check how price bands relate to quantity and revenue movement.</span>
                                        <span class="previewBox"><asp:Literal ID="litPreviewPricing" runat="server"></asp:Literal></span>
                                        <span class="openText">Open</span>
                                    </a>
                                    <a id="tileElasticity" runat="server" class="marketTile" href="MarketElasticity.aspx" title="Open Market Elasticity">
                                        <span class="tileCaption"><span class="tileTitle">Market Elasticity</span><span class="tileText">Price change, quantity change, elasticity, and demand notes.</span></span>
                                        <span class="tileBody">Compare quantity response across price bands by market group.</span>
                                        <span class="previewBox"><asp:Literal ID="litPreviewElasticity" runat="server"></asp:Literal></span>
                                        <span class="openText">Open</span>
                                    </a>
                                    <a id="tileBasket" runat="server" class="marketTile" href="MarketBasket.aspx" title="Open Market Basket">
                                        <span class="tileCaption"><span class="tileTitle">Market Basket</span><span class="tileText">Items ordered together, support, and cross-sell notes.</span></span>
                                        <span class="tileBody">Find co-occurring products or categories for bundle review.</span>
                                        <span class="previewBox"><asp:Literal ID="litPreviewBasket" runat="server"></asp:Literal></span>
                                        <span class="openText">Open</span>
                                    </a>
                                    <a id="tileSegments" runat="server" class="marketTile" href="MarketSegments.aspx" title="Open Market Segments">
                                        <span class="tileCaption"><span class="tileTitle">Market Segments</span><span class="tileText">Segments, records, values, averages, and segment notes.</span></span>
                                        <span class="tileBody">Compare market groups by value and average value.</span>
                                        <span class="previewBox"><asp:Literal ID="litPreviewSegments" runat="server"></asp:Literal></span>
                                        <span class="openText">Open</span>
                                    </a>
                                    <a id="tileChurn" runat="server" class="marketTile" href="MarketChurn.aspx" title="Open Market Churn">
                                        <span class="tileCaption"><span class="tileTitle">Market Churn</span><span class="tileText">Last activity, value, retention score, and churn notes.</span></span>
                                        <span class="tileBody">Review customers or segments for retention risk.</span>
                                        <span class="previewBox"><asp:Literal ID="litPreviewChurn" runat="server"></asp:Literal></span>
                                        <span class="openText">Open</span>
                                    </a>
                                    <a id="tileRisk" runat="server" class="marketTile" href="MarketRisk.aspx" title="Open Market Risk">
                                        <span class="tileCaption"><span class="tileTitle">Market Risk</span><span class="tileText">Value exposure, risk score, and risk notes.</span></span>
                                        <span class="tileBody">Score market groups by exposure and review high-risk areas.</span>
                                        <span class="previewBox"><asp:Literal ID="litPreviewRisk" runat="server"></asp:Literal></span>
                                        <span class="openText">Open</span>
                                    </a>
                                    <a id="tileInventory" runat="server" class="marketTile" href="MarketInventory.aspx" title="Open Market Inventory">
                                        <span class="tileCaption"><span class="tileTitle">Market Inventory</span><span class="tileText">Units, records, velocity, and inventory movement notes.</span></span>
                                        <span class="tileBody">Identify fast and slow movement by product or category.</span>
                                        <span class="previewBox"><asp:Literal ID="litPreviewInventory" runat="server"></asp:Literal></span>
                                        <span class="openText">Open</span>
                                    </a>
                                    <a id="tileProfit" runat="server" class="marketTile" href="MarketProfit.aspx" title="Open Market Profit">
                                        <span class="tileCaption"><span class="tileTitle">Market Profit</span><span class="tileText">Revenue, estimated cost, estimated profit, margin, and records.</span></span>
                                        <span class="tileBody">Review profitability drivers using selected market dimensions.</span>
                                        <span class="previewBox"><asp:Literal ID="litPreviewProfit" runat="server"></asp:Literal></span>
                                        <span class="openText">Open</span>
                                    </a>
                                    <a id="tileScenario" runat="server" class="marketTile" href="MarketScenario.aspx" title="Open Market Scenario">
                                        <span class="tileCaption"><span class="tileTitle">Market Scenario</span><span class="tileText">Current value, assumption change, scenario value, and difference.</span></span>
                                        <span class="tileBody">Model market assumptions and compare current and scenario values.</span>
                                        <span class="previewBox"><asp:Literal ID="litPreviewScenario" runat="server"></asp:Literal></span>
                                        <span class="openText">Open</span>
                                    </a>
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpMarketAdmin">
            <ProgressTemplate>
                <div class="modal"><div class="center"><asp:Image ID="imgProgress" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />Please Wait...</div></div>
            </ProgressTemplate>
        </asp:UpdateProgress>
    </form>
</body>
</html>
