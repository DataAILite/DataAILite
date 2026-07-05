<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DataCheck.aspx.vb" Inherits="DataCheck" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Data Quality Dashboard</title>
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
        .qualityTile { display:block; min-height:124px; border:1px solid #bfbfbf; border-radius:4px; background-color:#ffffff; color:#222222; text-decoration:none; box-shadow:0 1px 2px rgba(0,0,0,0.08); padding:0; }
        .qualityTile:hover { border-color:#0066FF; box-shadow:0 2px 8px rgba(0,0,0,0.16); }
        .tileGrid .qualityTile:nth-child(1) { background-color:#f7fbff; }
        .tileGrid .qualityTile:nth-child(2) { background-color:#f8fff7; }
        .tileGrid .qualityTile:nth-child(3) { background-color:#fffaf2; }
        .tileGrid .qualityTile:nth-child(4) { background-color:#fbf8ff; }
        .tileGrid .qualityTile:nth-child(5) { background-color:#f6fcfb; }
        .tileGrid .qualityTile:nth-child(6) { background-color:#fff7f8; }
        .tileGrid .qualityTile:nth-child(7) { background-color:#f9fbf1; }
        .tileGrid .qualityTile:nth-child(8) { background-color:#f4faff; }
        .tileGrid .qualityTile:nth-child(9) { background-color:#fff8f3; }
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
        .qualityBox { margin:8px 0 12px 0; max-width:1180px; overflow:auto; }
        .qualityTable { width:100%; border-collapse:collapse; font-family:Arial; font-size:11px; background-color:#f3fff3; }
        .qualityTable th { background-color:#663300; color:white; border:1px solid white; padding:4px; text-align:left; white-space:nowrap; }
        .qualityTable td { border:1px solid #d0d0d0; color:#222222; padding:4px; vertical-align:top; background-color:#f3fff3; }
        .statusGood { color:#006600; font-weight:bold; }
        .statusPartial { color:#996600; font-weight:bold; }
        .statusMissing { color:#990000; font-weight:bold; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <asp:UpdatePanel ID="udpDataCheck" runat="server">
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
                                        <asp:TreeNode Text="&lt;b&gt;Data Readiness Scanner&lt;/b&gt;" Value="DataReadinessScanner.aspx" NavigateUrl="DataReadinessScanner.aspx" Expanded="True"></asp:TreeNode>
                                        <asp:TreeNode Text="&lt;b&gt;Data Quality Dashboard&lt;/b&gt;" Value="DataCheck.aspx" NavigateUrl="DataCheck.aspx" Expanded="False">
                                            <asp:TreeNode Text="Data Quality" Value="DataQuality.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Data Profiling" Value="Profiling.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Data Dictionary" Value="DataDictionary.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Data Drift" Value="DataDrift.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Anomaly Scoring" Value="AnomalyScoring.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Outlier Flagging" Value="OutlierFlagging.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Rule-Based Alerts" Value="RuleBasedAlerts.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Map Readiness" Value="MapReadines.aspx"></asp:TreeNode>
                                        </asp:TreeNode>
                                        <asp:TreeNode Text="Reports and Charts" Value="ShowReport.aspx?srd=3" Expanded="False">
                                            <asp:TreeNode Text="Show formatted report" Value="ShowReport.aspx?srd=3"></asp:TreeNode>
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
                                        <asp:TreeNode Text="Analytics Dashboard" Value="DataAdmin.aspx" NavigateUrl="DataAdmin.aspx" Expanded="False">
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
                                            <asp:TreeNode Text="Ranking Analysis" Value="Ranking.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Regression Analysis" Value="Regression.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Time Based Summaries" Value="TimeBasedSummaries.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Time Series" Value="TimeSeries.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Audit Summaries" Value="AuditSummaries.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Cohort Analysis" Value="Cohort.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Funnel Analysis" Value="Funnel.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="ABC Pareto Analysis" Value="ABCPareto.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Automated Analysis Narratives" Value="AutomatedAnalysisNarratives.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Cross-Report Comparison" Value="CrossReportComparison.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="KPI Builder" Value="KPIBuilder.aspx"></asp:TreeNode>
                                        </asp:TreeNode>
                                        <asp:TreeNode Text="Market Dashboard" Value="MarketAdmin.aspx" NavigateUrl="MarketAdmin.aspx" Expanded="False">
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
                            <asp:HyperLink ID="HyperLinkData" runat="server" NavigateUrl="~/ShowReport.aspx?srd=0" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Open the report data explorer.">Data</asp:HyperLink>
                            &nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkReadiness" runat="server" NavigateUrl="~/DataReadinessScanner.aspx" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Open Data Readiness Scanner.">Data Readiness</asp:HyperLink>
                            &nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkAnalytics" runat="server" NavigateUrl="~/DataAdmin.aspx" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Open Analytics Dashboard.">Analytics Dashboard</asp:HyperLink>
                            &nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx?hilt=Data%20Quality" Target="_blank" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Open help for data quality pages.">Help</asp:HyperLink>
                            &nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkLogOff" runat="server" NavigateUrl="~/Default.aspx" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Log off and clear the current session.">Log off</asp:HyperLink>

                            <div class="dashboard">
                                <div class="dashboardHeader">
                                    <asp:Label ID="lblHeader" runat="server" CssClass="dashboardTitle" Text="Data Quality Dashboard" ToolTip="Open data quality and readiness checks for the current report data."></asp:Label>
                                    <asp:Label ID="LabelDescription" runat="server" CssClass="dashboardSubTitle" Text="Data Quality asks: &quot;Can this data be trusted, what is missing or suspicious, and what should be fixed before using analytics?&quot;"></asp:Label>
                                </div>

                                <div class="qualityBox">
                                    <asp:Literal ID="litQualitySuitability" runat="server"></asp:Literal>
                                </div>

                                <div class="tileGrid">
                                    <a id="tileDataReadiness" runat="server" class="qualityTile" href="DataReadinessScanner.aspx" title="Open Data Readiness Scanner">
                                        <span class="tileCaption"><span class="tileTitle">Data Readiness Scanner</span><span class="tileText">Scores useful analytics and suggests fields.</span></span>
                                        <span class="tileBody">Start here to decide which checks and analytics are ready for this data.</span>
                                        <span class="previewBox"><asp:Literal ID="litPreviewDataReadiness" runat="server"></asp:Literal></span>
                                        <span class="openText">Open</span>
                                    </a>
                                    <a id="tileDataQuality" runat="server" class="qualityTile" href="DataQuality.aspx" title="Open Data Quality">
                                        <span class="tileCaption"><span class="tileTitle">Data Quality</span><span class="tileText">Missing values, duplicates, dates, ranges, and suspicious text.</span></span>
                                        <span class="tileBody">Find direct quality problems that should be fixed or reviewed.</span>
                                        <span class="previewBox"><asp:Literal ID="litPreviewDataQuality" runat="server"></asp:Literal></span>
                                        <span class="openText">Open</span>
                                    </a>
                                    <a id="tileProfiling" runat="server" class="qualityTile" href="Profiling.aspx" title="Open Data Profiling">
                                        <span class="tileCaption"><span class="tileTitle">Data Profiling</span><span class="tileText">Field type, count, blanks, distinct, min, max, average, and stdev.</span></span>
                                        <span class="tileBody">Understand each field before using it in reports or models.</span>
                                        <span class="previewBox"><asp:Literal ID="litPreviewProfiling" runat="server"></asp:Literal></span>
                                        <span class="openText">Open</span>
                                    </a>
                                    <a id="tileDataDictionary" runat="server" class="qualityTile" href="DataDictionary.aspx" title="Open Data Dictionary">
                                        <span class="tileCaption"><span class="tileTitle">Data Dictionary</span><span class="tileText">Field meanings, types, blanks, distinct values, and usage hints.</span></span>
                                        <span class="tileBody">Document what each field appears to mean and how it can be used.</span>
                                        <span class="previewBox"><asp:Literal ID="litPreviewDataDictionary" runat="server"></asp:Literal></span>
                                        <span class="openText">Open</span>
                                    </a>
                                    <a id="tileDataDrift" runat="server" class="qualityTile" href="DataDrift.aspx" title="Open Data Drift Analysis">
                                        <span class="tileCaption"><span class="tileTitle">Data Drift Analysis</span><span class="tileText">Distribution changes across periods or comparison groups.</span></span>
                                        <span class="tileBody">Check whether field behavior changed enough to affect analytics.</span>
                                        <span class="previewBox"><asp:Literal ID="litPreviewDataDrift" runat="server"></asp:Literal></span>
                                        <span class="openText">Open</span>
                                    </a>
                                    <a id="tileAnomalyScoring" runat="server" class="qualityTile" href="AnomalyScoring.aspx" title="Open Anomaly Scoring">
                                        <span class="tileCaption"><span class="tileTitle">Anomaly Scoring</span><span class="tileText">Unusual values, combinations, period movements, and patterns.</span></span>
                                        <span class="tileBody">Score suspicious behavior that broader checks may miss.</span>
                                        <span class="previewBox"><asp:Literal ID="litPreviewAnomalyScoring" runat="server"></asp:Literal></span>
                                        <span class="openText">Open</span>
                                    </a>
                                    <a id="tileOutliers" runat="server" class="qualityTile" href="OutlierFlagging.aspx" title="Open Outlier Flagging">
                                        <span class="tileCaption"><span class="tileTitle">Outlier Flagging</span><span class="tileText">Numeric outliers by standard deviation, percent difference, or rules.</span></span>
                                        <span class="tileBody">Find values that may represent errors, special cases, or extreme events.</span>
                                        <span class="previewBox"><asp:Literal ID="litPreviewOutliers" runat="server"></asp:Literal></span>
                                        <span class="openText">Open</span>
                                    </a>
                                    <a id="tileRuleBasedAlerts" runat="server" class="qualityTile" href="RuleBasedAlerts.aspx" title="Open Rule-Based Alerts">
                                        <span class="tileCaption"><span class="tileTitle">Rule-Based Alerts</span><span class="tileText">Missing values, variance, correlation, outliers, map readiness, and risk thresholds.</span></span>
                                        <span class="tileBody">Apply business thresholds and turn quality concerns into alerts.</span>
                                        <span class="previewBox"><asp:Literal ID="litPreviewRuleBasedAlerts" runat="server"></asp:Literal></span>
                                        <span class="openText">Open</span>
                                    </a>
                                    <a id="tileMapReadiness" runat="server" class="qualityTile" href="MapReadines.aspx" title="Open Map Readiness">
                                        <span class="tileCaption"><span class="tileTitle">Map Readiness</span><span class="tileText">Latitude, longitude, missing coordinates, duplicates, and KML readiness.</span></span>
                                        <span class="tileBody">Check whether report data can safely support map views.</span>
                                        <span class="previewBox"><asp:Literal ID="litPreviewMapReadiness" runat="server"></asp:Literal></span>
                                        <span class="openText">Open</span>
                                    </a>
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpDataCheck">
            <ProgressTemplate>
                <div class="modal"><div class="center"><asp:Image ID="imgProgress" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />Please Wait...</div></div>
            </ProgressTemplate>
        </asp:UpdateProgress>
    </form>
</body>
</html>
