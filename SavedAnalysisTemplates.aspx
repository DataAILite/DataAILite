<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SavedAnalysisTemplates.aspx.vb" Inherits="SavedAnalysisTemplates" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Saved Analysis Templates</title>
    <style type="text/css">
        .NodeStyle { color:#0066FF; font-size:12px; font-weight:normal; text-decoration:none; }
        .NodeStyle:hover { text-decoration:underline; color:darkblue; }
        .ticketbutton { width:90px; height:25px; font-size:12px; border-radius:5px; border-style:solid; border-color:#4e4747; color:black; border-width:1px; background-image:linear-gradient(to bottom, rgba(211,211,211,0),rgba(211,211,250,3)); padding:3px; margin:5px; }
        .aiLinkButton { display:inline-block; width:90px; height:25px; line-height:17px; padding:3px; margin:5px; border-style:solid; border-color:#4e4747; border-width:1px; border-radius:5px; background-image:linear-gradient(to bottom, rgba(211,211,211,0),rgba(211,211,250,3)); color:#0066FF; font-size:12px; font-weight:bold; text-align:center; text-decoration:none; box-sizing:border-box; vertical-align:middle; }
        .aiLinkButton:hover { color:#004DCC; text-decoration:none; }
        .analysisgrid { font-family:Arial; font-size:12px; border-collapse:collapse; background-color:white; }
        .analysisgrid th { background-color:#663300; color:white; border:1px solid white; padding:4px; white-space:nowrap; }
        .analysisgrid td { border:1px solid #d0d0d0; padding:4px; white-space:normal; }
        .controlpanel { background-color:#e5e5e5; border:medium double #FFFFFF; color:black; font-family:Arial; font-size:small; width:auto; min-width:1120px; max-width:1280px; margin-left:0; margin-right:auto; }
        .analysisSubtitle { display:block; font-family:Arial; font-size:small; color:#333333; padding-top:4px; padding-bottom:8px; }
        .analysisExplanation { font-family:Arial; font-size:small; color:#333333; background-color:#f5fbf4; border:1px solid #d8ead4; padding:8px; margin-top:8px; max-width:1180px; }
        .analysisExplanation span { display:block; padding-bottom:4px; }
        .analysisRecommendation { display:block; text-align:center; font-family:Arial; font-size:small; font-weight:bold; color:red; margin-left:0; }
        .analysisRecommendation a { font-weight:bold; }
        .templateHint { font-family:Arial; font-size:11px; color:#444444; padding-left:4px; }
        .templateInput { width:230px; }
        .templateWideInput { width:360px; }
        .modal { position:fixed; z-index:2147483647; height:100%; width:100%; top:0; background-color:#f8f8d3; opacity:0.8; }
        .center { z-index:2147483647; margin:300px auto; padding-left:25px; padding-top:10px; width:130px; background-color:#f8f8d3; border-radius:10px; }
        .center img { height:100px; width:100px; }
        .explanationBlock { margin:6px 0 8px 0; padding:6px 8px; background-color:#fbfff8; border-left:4px solid #9ccc65; }
        .explanationTitle { color:#2f4f1f; margin-bottom:3px; }
        .explanationBlock ul { margin:3px 0 0 18px; padding:0; }
        .explanationBlock li { margin:2px 0; white-space:normal; }
    </style>
</head>
<body>
<form id="form1" runat="server">
<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
<asp:UpdatePanel ID="udpTemplates" runat="server">
<ContentTemplate>
<table>
<tr><td colspan="3" style="font-size:x-large; font-weight:bold; background-color:#e5e5e5; height:40px;"><asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label></td></tr>
<tr>
<td style="font-size:x-small; background-color:#e5e5e5; vertical-align:top; width:15%;">
<asp:TreeView ID="TreeView1" runat="server" Width="100%" NodeIndent="10" Font-Names="Times New Roman" EnableTheming="True" ImageSet="BulletedList">
<Nodes>
<asp:TreeNode Text="&lt;b&gt;Log Off&lt;/b&gt;" Value="Default.aspx" Expanded="True"></asp:TreeNode>
<asp:TreeNode Text="&lt;b&gt;List of Reports&lt;/b&gt;" Value="ListOfReports.aspx" Expanded="True"></asp:TreeNode>
<asp:TreeNode Text="&lt;b&gt;Report Definition&lt;/b&gt;" Value="ReportEdit.aspx?tne=2" Expanded="False"><asp:TreeNode Text="Report Parameters" Value="ReportEdit.aspx?tne=3"></asp:TreeNode><asp:TreeNode Text="Share Report (Users)" Value="ReportEdit.aspx?tne=4"></asp:TreeNode></asp:TreeNode>
<asp:TreeNode Text="&lt;b&gt;Report Data Query&lt;/b&gt;" Value="SQLquery.aspx?tnq=0" Expanded="False"><asp:TreeNode Text="Data fields" Value="SQLquery.aspx?tnq=0"></asp:TreeNode><asp:TreeNode Text="Joins" Value="SQLquery.aspx?tnq=1"></asp:TreeNode><asp:TreeNode Text="Filters" Value="SQLquery.aspx?tnq=2"></asp:TreeNode><asp:TreeNode Text="Sorting" Value="SQLquery.aspx?tnq=3"></asp:TreeNode></asp:TreeNode>
<asp:TreeNode Text="&lt;b&gt;Report Format Definition&lt;/b&gt;" Value="RDLformat.aspx?tnf=0" Expanded="False"><asp:TreeNode Text="Columns, Expressions" Value="RDLformat.aspx?tnf=0"></asp:TreeNode><asp:TreeNode Text="Groups, Total" Value="RDLformat.aspx?tnf=1"></asp:TreeNode><asp:TreeNode Text="Combine Values" Value="RDLformat.aspx?tnf=2"></asp:TreeNode><asp:TreeNode Text="Advanced Report Designer" Value="ReportDesigner.aspx"></asp:TreeNode><asp:TreeNode Text="Map Definition" Value="MapReport.aspx"></asp:TreeNode></asp:TreeNode>
<asp:TreeNode Text="Explore Report Data" Value="ShowReport.aspx?srd=0" Expanded="False"><asp:TreeNode Text="Export Data to Excel" Value="datatoExcel" NavigateUrl="ShowReport.aspx?srd=1"></asp:TreeNode><asp:TreeNode Text="Export Data to CSV" Value="datatoCSV" NavigateUrl="ShowReport.aspx?srd=2"></asp:TreeNode><asp:TreeNode Text="Export Data to Delimited File" Value="ShowReport" NavigateUrl="ShowReport.aspx?srd=10"></asp:TreeNode><asp:TreeNode Text="Export Data to XML" Value="datatoXML" NavigateUrl="ShowReport.aspx?srd=14"></asp:TreeNode></asp:TreeNode>
<asp:TreeNode Text="&lt;b&gt;Data Readiness Scanner&lt;/b&gt;" Value="DataReadinessScanner.aspx" NavigateUrl="DataReadinessScanner.aspx" Expanded="True"></asp:TreeNode>
                                        <asp:TreeNode Text="&lt;b&gt;Data Quality Dashboard&lt;/b&gt;" Value="DataCheck.aspx" NavigateUrl="DataCheck.aspx" Expanded="True">
                                            <asp:TreeNode Text="Data Quality" Value="DataQuality.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Data Profiling" Value="Profiling.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Data Dictionary" Value="DataDictionary.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Data Drift" Value="DataDrift.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Anomaly Scoring" Value="AnomalyScoring.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Outlier Flagging" Value="OutlierFlagging.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Rule-Based Alerts" Value="RuleBasedAlerts.aspx"></asp:TreeNode>
                                            <asp:TreeNode Text="Map Readiness" Value="MapReadines.aspx"></asp:TreeNode>
                                        </asp:TreeNode>
<asp:TreeNode Text="Show Report" Value="ShowReport.aspx?srd=3" Expanded="True"><asp:TreeNode Text="Show Generic Report" Value="ReportViews.aspx?gen=yes"></asp:TreeNode><asp:TreeNode Text="Show Report Charts" Value="ShowReport.aspx?srd=17"></asp:TreeNode><asp:TreeNode Text="Chart Recommendations" Value="ChartRecommendationHelpers.aspx"></asp:TreeNode><asp:TreeNode Text="Map Report" Value="MapReport.aspx"></asp:TreeNode><asp:TreeNode Text="Map Readiness" Value="MapReadines.aspx"></asp:TreeNode><asp:TreeNode Text="Export Report to Excel" Value="reptoExcel" NavigateUrl="ShowReport.aspx?srd=4"></asp:TreeNode><asp:TreeNode Text="Export Report to Word" Value="reptoWord" NavigateUrl="ShowReport.aspx?srd=5"></asp:TreeNode><asp:TreeNode Text="Export Report to PDF" Value="reptoPDF" NavigateUrl="ShowReport.aspx?srd=6"></asp:TreeNode><asp:TreeNode Text="Export Packages" Value="ExportPackages.aspx"></asp:TreeNode></asp:TreeNode>
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
<asp:TreeNode Text="Market Dashboard" Value="MarketAdmin.aspx" NavigateUrl="MarketAdmin.aspx" Expanded="False"><asp:TreeNode Text="Market Demand" Value="MarketDemand.aspx" NavigateUrl="MarketDemand.aspx"></asp:TreeNode><asp:TreeNode Text="Market Pricing" Value="MarketPricing.aspx" NavigateUrl="MarketPricing.aspx"></asp:TreeNode><asp:TreeNode Text="Market Elasticity" Value="MarketElasticity.aspx"></asp:TreeNode><asp:TreeNode Text="Market Basket" Value="MarketBasket.aspx" NavigateUrl="MarketBasket.aspx"></asp:TreeNode><asp:TreeNode Text="Market Segments" Value="MarketSegments.aspx" NavigateUrl="MarketSegments.aspx"></asp:TreeNode><asp:TreeNode Text="Market Churn" Value="MarketChurn.aspx" NavigateUrl="MarketChurn.aspx"></asp:TreeNode><asp:TreeNode Text="Market Risk" Value="MarketRisk.aspx" NavigateUrl="MarketRisk.aspx"></asp:TreeNode><asp:TreeNode Text="Market Inventory" Value="MarketInventory.aspx" NavigateUrl="MarketInventory.aspx"></asp:TreeNode><asp:TreeNode Text="Market Profit" Value="MarketProfit.aspx" NavigateUrl="MarketProfit.aspx"></asp:TreeNode><asp:TreeNode Text="Market Scenario" Value="MarketScenario.aspx" NavigateUrl="MarketScenario.aspx"></asp:TreeNode></asp:TreeNode>
</Nodes>
<RootNodeStyle HorizontalPadding="2px" Font-Bold="True" Font-Underline="False" />
<NodeStyle CssClass="NodeStyle" />
<ParentNodeStyle Font-Bold="True" />
</asp:TreeView>
</td>
<td style="width:5px;"></td>
<td style="width:85%; text-align:left; vertical-align:top;">
<asp:HyperLink ID="HyperLinkAnalytics" runat="server" NavigateUrl="~/Analytics.aspx" CssClass="NodeStyle" Font-Names="Arial">Detail Analytics</asp:HyperLink>
&nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkReport" runat="server" NavigateUrl="~/ShowReport.aspx?srd=3" CssClass="NodeStyle" Font-Names="Arial">Report and Charts</asp:HyperLink>
&nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx?hilt=Saved%20Analysis%20Templates" Target="_blank" CssClass="NodeStyle" Font-Names="Arial">Help</asp:HyperLink>
&nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkLogOff" runat="server" NavigateUrl="~/Default.aspx" CssClass="NodeStyle" Font-Names="Arial">Log off</asp:HyperLink>
<br /><br />
<div style="text-align:center;"><asp:Label ID="lblHeader" runat="server" Font-Size="22px" Font-Names="Arial">Saved Analysis Templates</asp:Label><asp:Label ID="LabelAnalysisSubtitle" runat="server" CssClass="analysisSubtitle" Text="Save reusable analysis settings, fields, filters, thresholds, and notes for the current session so repeated analytical work can be reopened consistently."></asp:Label></div>
<span class="analysisRecommendation">Work Flow suggested: review saved templates in <a class="NodeStyle" href="DataReadinessScanner.aspx">Data Readiness Scanner</a>, <a class="NodeStyle" href="DataAdmin.aspx">Analytics Dashboard</a>, and <a class="NodeStyle" href="AuditSummaries.aspx">Audit Summaries</a>.</span>
<br />
<table class="controlpanel" cellpadding="4" cellspacing="0" style="min-width:1280px;">

<tr><td style="font-weight:bold;">Template Name:</td><td><asp:TextBox ID="txtTemplateName" runat="server" CssClass="templateInput" ToolTip="Name for the saved analysis template stored in the current session."></asp:TextBox></td><td style="font-weight:bold;">Analysis Page:</td><td><asp:DropDownList ID="DropDownAnalysisPage" runat="server" Width="260px" ToolTip="Analytics page or market page this template is intended to reopen or document.">
<asp:ListItem>Detail Analytics</asp:ListItem>
<asp:ListItem>Data Readiness Scanner</asp:ListItem>
<asp:ListItem>Data Overall Statistics</asp:ListItem>
<asp:ListItem>Groups Statistics</asp:ListItem>
<asp:ListItem>Fields Correlation</asp:ListItem>
<asp:ListItem>Correlation Threshold</asp:ListItem>
<asp:ListItem>Chart Recommendations</asp:ListItem>
<asp:ListItem>Map Readiness</asp:ListItem>
<asp:ListItem>Matrix Balancing</asp:ListItem>
<asp:ListItem>Pivot / Cross Tab</asp:ListItem>
<asp:ListItem>Variance Analysis</asp:ListItem>
<asp:ListItem>Comparison Reports</asp:ListItem>
<asp:ListItem>Data Profiling</asp:ListItem>
<asp:ListItem>Data Quality</asp:ListItem>
<asp:ListItem>Ranking Analysis</asp:ListItem>
<asp:ListItem>Regression Analysis</asp:ListItem>
<asp:ListItem>Time Based Summaries</asp:ListItem>
<asp:ListItem>Time Series</asp:ListItem>
<asp:ListItem>Outlier Flagging</asp:ListItem>
<asp:ListItem>Audit Summaries</asp:ListItem>
<asp:ListItem>Cohort Analysis</asp:ListItem>
<asp:ListItem>Funnel Analysis</asp:ListItem>
<asp:ListItem>ABC Pareto Analysis</asp:ListItem>
<asp:ListItem>Data Drift Analysis</asp:ListItem>
<asp:ListItem>KPI Builder</asp:ListItem>
<asp:ListItem>Data Dictionary</asp:ListItem>
<asp:ListItem>Anomaly Scoring</asp:ListItem>
<asp:ListItem>Rule-Based Alerts</asp:ListItem>
<asp:ListItem>Saved Analysis Templates</asp:ListItem>
<asp:ListItem>Automated Analysis Narratives</asp:ListItem>
<asp:ListItem>Cross-Report Comparison</asp:ListItem>
<asp:ListItem>Market Dashboard</asp:ListItem>
<asp:ListItem>Market Demand</asp:ListItem>
<asp:ListItem>Market Pricing</asp:ListItem>
<asp:ListItem>Market Elasticity</asp:ListItem>
<asp:ListItem>Market Basket</asp:ListItem>
<asp:ListItem>Market Segments</asp:ListItem>
<asp:ListItem>Market Churn</asp:ListItem>
<asp:ListItem>Market Risk</asp:ListItem>
<asp:ListItem>Market Inventory</asp:ListItem>
<asp:ListItem>Market Profit</asp:ListItem>
<asp:ListItem>Market Scenario</asp:ListItem>
</asp:DropDownList><span class="templateHint">Only relevant rows below stay visible.</span></td></tr>
<tr class="templateControl ctrl-primary"><td style="font-weight:bold;">Primary / Category field(s):</td><td><asp:TextBox ID="txtPrimaryField" runat="server" CssClass="templateInput" ToolTip="Primary category, product, customer, group, or X-axis field. Use commas for multiple fields where the target page supports them."></asp:TextBox></td><td style="font-weight:bold;">Secondary / Group field:</td><td><asp:TextBox ID="txtSecondaryField" runat="server" CssClass="templateInput" ToolTip="Second category, group, row/column, segment, or comparison field."></asp:TextBox></td></tr>
<tr class="templateControl ctrl-rowcol"><td style="font-weight:bold;">Row Field:</td><td><asp:TextBox ID="txtRowField" runat="server" CssClass="templateInput" ToolTip="Row field for pivot/cross-tab or matrix style analysis."></asp:TextBox></td><td style="font-weight:bold;">Column Field:</td><td><asp:TextBox ID="txtColumnField" runat="server" CssClass="templateInput" ToolTip="Column field for pivot/cross-tab or matrix style analysis."></asp:TextBox></td></tr>
<tr class="templateControl ctrl-value"><td style="font-weight:bold;">Value Field(s):</td><td><asp:TextBox ID="txtValueField" runat="server" CssClass="templateInput" ToolTip="Numeric value, measure, Y field, KPI field, or weighted field. Use commas for multiple values where supported."></asp:TextBox></td><td style="font-weight:bold;">Second Value / Field2:</td><td><asp:TextBox ID="txtSecondValueField" runat="server" CssClass="templateInput" ToolTip="Second value field, comparison value, regression Y field, or Field2 for advanced analytics."></asp:TextBox></td></tr>
<tr class="templateControl ctrl-date"><td style="font-weight:bold;">Date / Period Field:</td><td><asp:TextBox ID="txtDateField" runat="server" CssClass="templateInput" ToolTip="Date, period, cohort date, activity date, movement date, or time-series field."></asp:TextBox></td><td style="font-weight:bold;">Date Aggregation:</td><td><asp:DropDownList ID="DropDownDateAggregation" runat="server" Width="150px" ToolTip="Time period used by time-based summaries, time series, market demand, inventory movement, drift, and KPI templates."><asp:ListItem></asp:ListItem><asp:ListItem>Day</asp:ListItem><asp:ListItem>Week</asp:ListItem><asp:ListItem>Month</asp:ListItem><asp:ListItem>Quarter</asp:ListItem><asp:ListItem>Year</asp:ListItem></asp:DropDownList></td></tr>
<tr class="templateControl ctrl-keystage"><td style="font-weight:bold;">Key / Entity Field:</td><td><asp:TextBox ID="txtKeyField" runat="server" CssClass="templateInput" ToolTip="Customer, order, product, report key, or entity identifier used by cohort, funnel, basket, cross-report, and market pages."></asp:TextBox></td><td style="font-weight:bold;">Stage / Status Field:</td><td><asp:TextBox ID="txtStageField" runat="server" CssClass="templateInput" ToolTip="Stage, status, outcome, churn flag, risk status, funnel step, or rule status field."></asp:TextBox></td></tr>
<tr class="templateControl ctrl-compare"><td style="font-weight:bold;">Base Value:</td><td><asp:TextBox ID="txtBaseValue" runat="server" CssClass="templateInput" ToolTip="Base group, period, location, category, or value used for variance and comparisons."></asp:TextBox></td><td style="font-weight:bold;">Compare Value:</td><td><asp:TextBox ID="txtCompareValue" runat="server" CssClass="templateInput" ToolTip="Comparison group, period, location, category, or value used for variance and comparisons."></asp:TextBox></td></tr>
<tr class="templateControl ctrl-cross"><td style="font-weight:bold;">Compare Report ID:</td><td><asp:TextBox ID="txtCompareReport" runat="server" CssClass="templateInput" ToolTip="Second report ID for Cross-Report Comparison."></asp:TextBox></td><td style="font-weight:bold;">Field Set:</td><td><asp:TextBox ID="txtFieldSet" runat="server" CssClass="templateInput" ToolTip="Optional comma-separated complete field set. If blank, it is built from the visible field controls."></asp:TextBox></td></tr>
<tr class="templateControl ctrl-options"><td style="font-weight:bold;">Aggregation:</td><td><asp:DropDownList ID="DropDownAggregation" runat="server" Width="150px" ToolTip="Saved aggregation choice for repeated records."><asp:ListItem>Count</asp:ListItem><asp:ListItem>CountDistinct</asp:ListItem><asp:ListItem>Sum</asp:ListItem><asp:ListItem>Average</asp:ListItem><asp:ListItem>Avg</asp:ListItem><asp:ListItem>Min</asp:ListItem><asp:ListItem>Max</asp:ListItem><asp:ListItem>StDev</asp:ListItem><asp:ListItem>Value</asp:ListItem></asp:DropDownList></td><td style="font-weight:bold;">Model / Type:</td><td><asp:DropDownList ID="DropDownTemplateMode" runat="server" Width="190px" ToolTip="Model, rank type, equation type, narrative focus, comparison mode, or market scenario type."><asp:ListItem></asp:ListItem><asp:ListItem>Top</asp:ListItem><asp:ListItem>Bottom</asp:ListItem><asp:ListItem>Average</asp:ListItem><asp:ListItem>BestFit</asp:ListItem><asp:ListItem>Linear</asp:ListItem><asp:ListItem>Polynomial</asp:ListItem><asp:ListItem>Exponential</asp:ListItem><asp:ListItem>Power</asp:ListItem><asp:ListItem>Logistic Probability</asp:ListItem><asp:ListItem>Percentage Change</asp:ListItem><asp:ListItem>Variance</asp:ListItem><asp:ListItem>Contribution</asp:ListItem><asp:ListItem>Executive Summary</asp:ListItem><asp:ListItem>Detailed</asp:ListItem></asp:DropDownList></td></tr>
<tr class="templateControl ctrl-threshold"><td style="font-weight:bold;">Threshold / Score:</td><td><asp:TextBox ID="txtThresholds" runat="server" CssClass="templateInput" ToolTip="Thresholds, score limits, rule values, or comparison settings saved with the template."></asp:TextBox></td><td style="font-weight:bold;">Top N / Number:</td><td><asp:TextBox ID="txtTopN" runat="server" CssClass="templateInput" ToolTip="Top N, bottom N, number of records, or another numeric setting."></asp:TextBox></td></tr>
<tr class="templateControl ctrl-window"><td style="font-weight:bold;">Number of Time Periods:</td><td><asp:TextBox ID="txtWindowPeriods" runat="server" CssClass="templateInput" ToolTip="Moving average, rolling total, demand, inventory, or period window."></asp:TextBox></td><td style="font-weight:bold;">Assumption %:</td><td><asp:TextBox ID="txtAssumptionPercent" runat="server" CssClass="templateInput" ToolTip="Scenario, market, elasticity, pricing, inventory, profit, risk, or demand assumption percent."></asp:TextBox></td></tr>
<tr class="templateControl ctrl-filter"><td style="font-weight:bold;">Filters:</td><td><asp:TextBox ID="txtFilters" runat="server" CssClass="templateWideInput" ToolTip="Filter, segment, period, search, SQL WHERE note, or business restriction to reuse with the template."></asp:TextBox></td><td style="font-weight:bold;">Search:</td><td><asp:TextBox ID="txtSearch" runat="server" CssClass="templateInput" ToolTip="Filter saved templates by text."></asp:TextBox></td></tr>
<tr><td style="font-weight:bold;">Notes:</td><td colspan="3"><asp:TextBox ID="txtNotes" runat="server" Width="650px" TextMode="MultiLine" Rows="2" ToolTip="Optional explanation of why this template is useful."></asp:TextBox><asp:Button ID="ButtonBuild" runat="server" CssClass="ticketbutton" Text="Build" OnClientClick="return showWaitingPanel();" ToolTip="Build or refresh the page results using the selected controls. The Build action also saves an Excel snapshot of the results in the session temporary folder so it can be included later in Export Packages if needed." /><asp:Button ID="ButtonReset" runat="server" CssClass="ticketbutton" Text="Reset" ToolTip="Clear selected options and rebuild the default analysis." /><asp:Button ID="ButtonExportCSV" runat="server" CssClass="ticketbutton" Text="CSV" ToolTip="Export the analysis grid to CSV." /><asp:Button ID="ButtonExportExcel" runat="server" CssClass="ticketbutton" Text="Excel" ToolTip="Export the analysis grid to Excel." /><asp:LinkButton OnClientClick="return showWaitingPanel();" ID="lnkTemplatesAI" runat="server" CssClass="aiLinkButton" Font-Names="Arial" ToolTip="Ask AI to interpret saved templates and suggest which templates should be used first.">AI</asp:LinkButton></td></tr>
</table>
<asp:Label ID="LabelError" runat="server" ForeColor="Red" Font-Names="Arial" Font-Size="Medium"></asp:Label><br />
<asp:Label ID="LabelBuildExportHint" runat="server" ForeColor="DarkGreen" Font-Names="Arial" Font-Size="Small" Text="Click Build to save results in temporary folder for future Export Packages if needed."></asp:Label><br />
<asp:Label ID="LabelInfo" runat="server" ForeColor="Black" Font-Names="Arial" Font-Size="Small"></asp:Label><br /><br />
<div style="font-family:Arial; font-size:small; padding-bottom:6px;"><asp:LinkButton ID="LinkButtonPrevious" runat="server" Font-Size="Small" OnClick="LinkButtonPrevious_Click">Previous</asp:LinkButton>&nbsp;&nbsp;<asp:Label ID="LabelPageNumberCaption" runat="server" Font-Names="Arial" Font-Size="Small" Text="Page Number"></asp:Label><asp:TextBox ID="TextBoxPageNumber" runat="server" Width="35px" Font-Names="Arial" Font-Size="Small" AutoPostBack="True" OnTextChanged="TextBoxPageNumber_TextChanged"></asp:TextBox><asp:Label ID="LabelPageCount" runat="server" Font-Names="Arial" Font-Size="Small"></asp:Label>&nbsp;&nbsp;<asp:LinkButton ID="LinkButtonNext" runat="server" Font-Size="Small" OnClick="LinkButtonNext_Click">Next</asp:LinkButton></div>
<div style="overflow:auto; max-width:100%;"><asp:GridView ID="GridViewTemplates" runat="server" CssClass="analysisgrid" AutoGenerateColumns="True" GridLines="Both"></asp:GridView></div>
<div class="analysisExplanation"><asp:Label ID="LabelModelExplanation" runat="server"></asp:Label><asp:Label ID="LabelAlgorithmExplanation" runat="server"></asp:Label><asp:Label ID="LabelOutputExplanation" runat="server"></asp:Label><asp:Label ID="LabelReadinessWhyUseful" runat="server" Visible="False"></asp:Label><asp:Label ID="LabelReadinessSuggestedFields" runat="server" Visible="False"></asp:Label></div>
</td></tr></table>
</ContentTemplate>
<Triggers><asp:PostBackTrigger ControlID="ButtonExportCSV" /><asp:PostBackTrigger ControlID="ButtonExportExcel" /><asp:PostBackTrigger ControlID="lnkTemplatesAI" /></Triggers>
</asp:UpdatePanel>
<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpTemplates"><ProgressTemplate><div class="modal"><div class="center"><asp:Image ID="imgProgress" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />Please Wait...</div></div></ProgressTemplate></asp:UpdateProgress>
<div id="ManualWaitingPanel" class="modal" style="display:none;"><div class="center"><img src="Controls/Images/WaitImage2.gif" alt="Please Wait" />Please Wait...</div></div>
</form>
<script type="text/javascript">
function hideWaitingPanel() { var waitingPanel = document.getElementById('ManualWaitingPanel'); if (waitingPanel) { waitingPanel.style.display = 'none'; } }
function showWaitingPanel() { var waitingPanel = document.getElementById('ManualWaitingPanel'); if (waitingPanel) { waitingPanel.style.display = 'block'; } return true; }
window.addEventListener('pageshow', hideWaitingPanel);
if (typeof (Sys) !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () { hideWaitingPanel(); updateTemplateControls(); });
}
var templateControlMap = {
    "Detail Analytics": ["primary", "value", "options", "filter"],
    "Data Readiness Scanner": ["filter"],
    "Data Overall Statistics": ["primary", "value", "filter"],
    "Groups Statistics": ["primary", "value", "options", "filter"],
    "Fields Correlation": ["value", "threshold", "filter"],
    "Correlation Threshold": ["value", "threshold", "filter"],
    "Chart Recommendations": ["primary", "value", "date", "options", "filter"],
    "Map Readiness": ["primary", "keystage", "filter"],
    "Matrix Balancing": ["primary", "value", "options", "filter"],
    "Pivot / Cross Tab": ["rowcol", "value", "options", "filter"],
    "Variance Analysis": ["primary", "value", "compare", "options", "filter"],
    "Comparison Reports": ["primary", "value", "date", "compare", "cross", "options", "filter"],
    "Data Profiling": ["primary", "value", "filter"],
    "Data Quality": ["primary", "value", "date", "threshold", "filter"],
    "Ranking Analysis": ["primary", "value", "options", "threshold", "filter"],
    "Regression Analysis": ["primary", "value", "options", "filter"],
    "Time Based Summaries": ["value", "date", "options", "filter"],
    "Time Series": ["value", "date", "options", "window", "filter"],
    "Outlier Flagging": ["primary", "value", "threshold", "filter"],
    "Audit Summaries": ["primary", "value", "options", "threshold", "filter"],
    "Cohort Analysis": ["value", "date", "keystage", "filter"],
    "Funnel Analysis": ["date", "keystage", "filter"],
    "ABC Pareto Analysis": ["primary", "value", "options", "filter"],
    "Data Drift Analysis": ["primary", "value", "date", "compare", "threshold", "filter"],
    "KPI Builder": ["primary", "value", "date", "options", "threshold", "filter"],
    "Data Dictionary": ["primary", "value", "date", "filter"],
    "Anomaly Scoring": ["primary", "value", "date", "threshold", "filter"],
    "Rule-Based Alerts": ["primary", "value", "date", "keystage", "threshold", "filter"],
    "Saved Analysis Templates": ["primary", "value", "date", "options", "threshold", "filter"],
    "Automated Analysis Narratives": ["primary", "value", "options", "filter"],
    "Cross-Report Comparison": ["primary", "value", "cross", "options", "filter"],
    "Market Dashboard": ["primary", "value", "date", "keystage", "options", "threshold", "window", "filter"],
    "Market Demand": ["primary", "value", "date", "options", "window", "filter"],
    "Market Pricing": ["primary", "value", "options", "threshold", "filter"],
    "Market Elasticity": ["primary", "value", "options", "threshold", "filter"],
    "Market Basket": ["primary", "value", "keystage", "filter"],
    "Market Segments": ["primary", "value", "keystage", "filter"],
    "Market Churn": ["primary", "value", "date", "keystage", "threshold", "filter"],
    "Market Risk": ["primary", "value", "keystage", "threshold", "filter"],
    "Market Inventory": ["primary", "value", "date", "options", "window", "threshold", "filter"],
    "Market Profit": ["primary", "value", "options", "threshold", "filter"],
    "Market Scenario": ["primary", "value", "options", "window", "threshold", "filter"]
};
function updateTemplateControls() {
    var ddl = document.getElementById('<%= DropDownAnalysisPage.ClientID %>');
    if (!ddl) { return; }
    var pageName = ddl.options[ddl.selectedIndex].text;
    var visibleGroups = templateControlMap[pageName] || ["primary", "value", "date", "keystage", "rowcol", "compare", "cross", "options", "threshold", "window", "filter"];
    var rows = document.querySelectorAll('.templateControl');
    for (var i = 0; i < rows.length; i++) {
        var show = false;
        for (var j = 0; j < visibleGroups.length; j++) {
            if (rows[i].className.indexOf('ctrl-' + visibleGroups[j]) >= 0) { show = true; break; }
        }
        rows[i].style.display = show ? '' : 'none';
    }
}
document.addEventListener('DOMContentLoaded', function () {
    var ddl = document.getElementById('<%= DropDownAnalysisPage.ClientID %>');
    if (ddl) { ddl.onchange = updateTemplateControls; }
    updateTemplateControls();
});
</script>
</body>
</html>
