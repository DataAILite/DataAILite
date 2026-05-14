<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DataAdmin.aspx.vb" Inherits="DataAdmin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Data Analytics Dashboard</title>
    <style type="text/css">
        .NodeStyle {
            color: #0066FF;
            font-size: 12px;
            font-weight: normal;
            text-decoration: none;
        }
        .NodeStyle:hover {
            text-decoration: underline;
            color: darkblue;
        }
        .modal {
            position: fixed;
            z-index: 2147483647;
            height: 100%;
            width: 100%;
            top: 0;
            background-color: #f8f8d3;
            opacity: 0.8;
        }
        .center {
            z-index: 2147483647;
            margin: 300px auto;
            padding-left: 25px;
            padding-top: 10px;
            width: 130px;
            background-color: #f8f8d3;
            border-radius: 10px;
        }
        .center img {
            height: 100px;
            width: 100px;
        }
        .dashboard {
            font-family: Arial;
            margin: 0;
            width: 100%;
            text-align: left;
        }
        .dashboardHeader {
            margin: 14px 0 12px 0;
        }
        .dashboardTitle {
            display: block;
            color: #333333;
            font-size: 22px;
            font-weight: normal;
            margin-bottom: 4px;
        }
        .dashboardSubTitle {
            color: #666666;
            font-size: 12px;
        }
        .dashboardPager {
            font-family: Arial;
            font-size: 12px;
            text-align: right;
            margin: 0 0 8px 0;
        }
        .tileGrid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
            gap: 10px;
            align-items: stretch;
        }
        .analyticsTile {
            display: block;
            min-height: 124px;
            border: 1px solid #bfbfbf;
            border-radius: 4px;
            background-color: #ffffff;
            color: #222222;
            text-decoration: none;
            box-shadow: 0 1px 2px rgba(0, 0, 0, 0.08);
            padding: 0;
        }
        .analyticsTile:hover {
            border-color: #0066FF;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.16);
        }
        .tileGrid .analyticsTile:nth-child(1) {
            background-color: #f7fbff;
        }
        .tileGrid .analyticsTile:nth-child(2) {
            background-color: #f8fff7;
        }
        .tileGrid .analyticsTile:nth-child(3) {
            background-color: #fffaf2;
        }
        .tileGrid .analyticsTile:nth-child(4) {
            background-color: #fbf8ff;
        }
        .tileGrid .analyticsTile:nth-child(5) {
            background-color: #f6fcfb;
        }
        .tileGrid .analyticsTile:nth-child(6) {
            background-color: #fff7f8;
        }
        .tileGrid .analyticsTile:nth-child(7) {
            background-color: #f9fbf1;
        }
        .tileGrid .analyticsTile:nth-child(8) {
            background-color: #f4faff;
        }
        .tileGrid .analyticsTile:nth-child(9) {
            background-color: #fff8f3;
        }
        .tileGrid .analyticsTile:nth-child(10) {
            background-color: #f5fbf7;
        }
        .tileGrid .analyticsTile:nth-child(11) {
            background-color: #f9f7ff;
        }
        .tileGrid .analyticsTile:nth-child(12) {
            background-color: #f7fcff;
        }
        .tileGrid .analyticsTile:nth-child(13) {
            background-color: #fffdf3;
        }
        .tileGrid .analyticsTile:nth-child(14) {
            background-color: #f8f6f2;
        }
        .tileGrid .analyticsTile:nth-child(15) {
            background-color: #f3fbff;
        }
        .tileGrid .analyticsTile:nth-child(16) {
            background-color: #fff6fb;
        }
        .tileGrid .analyticsTile:nth-child(17) {
            background-color: #f6fff3;
        }
        .tileCaption {
            padding: 8px 10px 2px 10px;
            border-bottom: 0;
            background-color: transparent;
        }
        .tileTitle {
            display: block;
            color: #222222;
            font-size: 14px;
            font-weight: bold;
            line-height: 18px;
        }
        .tileText {
            display: block;
            color: #555555;
            font-size: 11px;
            line-height: 15px;
            min-height: 0;
        }
        .tileBody {
            display: block;
            padding: 4px 10px 3px 10px;
            color: #333333;
            font-size: 12px;
            line-height: 17px;
        }
        .openText {
            display: block;
            margin: 5px 10px 10px 10px;
            color: #0066FF;
            font-size: 12px;
            font-weight: bold;
        }
        .previewBox {
            display: block;
            margin: 5px 10px 2px 10px;
            border: 1px solid #cfcfcf;
            background-color: rgba(255, 255, 255, 0.82);
            max-height: 136px;
            overflow: hidden;
        }
        .previewTable {
            width: 100%;
            border-collapse: collapse;
            font-family: Arial;
            font-size: 9px;
            table-layout: fixed;
        }
        .previewTable th {
            border: 1px solid #d9d9d9;
            background-color: #f8f8f8;
            color: #333333;
            font-weight: bold;
            padding: 1px;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }
        .previewTable td {
            border: 1px solid #e1e1e1;
            color: #222222;
            padding: 1px;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }
        .previewEmpty {
            display: block;
            color: #777777;
            font-size: 11px;
            padding: 8px;
        }
    </style>
    <script type="text/javascript">
        function dataAdminTilePageSize() {
            var grid = document.querySelector(".tileGrid");
            if (!grid) {
                return 12;
            }

            var gridRect = grid.getBoundingClientRect();
            var availableWidth = Math.max(grid.clientWidth || gridRect.width, 280);
            var availableHeight = Math.max(window.innerHeight - gridRect.top - 20, 210);
            var columns = Math.max(1, Math.floor((availableWidth + 10) / 290));
            var rows = Math.max(1, Math.floor((availableHeight + 10) / 210));
            return Math.max(4, Math.min(36, columns * rows));
        }

        function setDataAdminTilePageSize() {
            var hidden = document.getElementById("<%= HiddenDashboardPageSize.ClientID %>");
            if (!hidden) {
                return;
            }

            var size = dataAdminTilePageSize();
            hidden.value = size;

            var url = new URL(window.location.href);
            var currentSize = parseInt(url.searchParams.get("ps") || "0", 10);
            if (currentSize !== size) {
                url.searchParams.set("ps", size);
                url.searchParams.set("page", "1");
                window.location.replace(url.toString());
            }
        }

        var dataAdminResizeTimer = null;
        function queueDataAdminTilePageSize() {
            window.clearTimeout(dataAdminResizeTimer);
            dataAdminResizeTimer = window.setTimeout(setDataAdminTilePageSize, 400);
        }

        if (window.Sys && Sys.Application) {
            Sys.Application.add_load(setDataAdminTilePageSize);
        } else if (window.addEventListener) {
            window.addEventListener("load", setDataAdminTilePageSize);
        }

        if (window.addEventListener) {
            window.addEventListener("resize", queueDataAdminTilePageSize);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <asp:HiddenField ID="HiddenDashboardPageSize" runat="server" />
        <asp:UpdatePanel ID="udpDataAdmin" runat="server">
            <ContentTemplate>
                <table>
                    <tr>
                        <td colspan="3" style="font-size: x-large; font-weight: bold; background-color: #e5e5e5; vertical-align: middle; text-align: left; height: 40px;">
                            <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="font-size: x-small; background-color: #e5e5e5; vertical-align: top; text-align: left; width: 15%;">
                            <div id="tree" style="font-size: x-small;">
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
                            </div>
                        </td>
                        <td style="width: 5px;"></td>
                        <td id="main" style="width: 85%; text-align: left; vertical-align: top;">
                            <div style="text-align: left; width: 100%;">
                                <asp:HyperLink ID="HyperLinkAnalytics" runat="server" NavigateUrl="~/Analytics.aspx" CssClass="NodeStyle" Font-Names="Arial">Analytics</asp:HyperLink>
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:HyperLink ID="HyperLinkDataExplorer" runat="server" NavigateUrl="~/ShowReport.aspx?srd=0" CssClass="NodeStyle" Font-Names="Arial">Data Explorer</asp:HyperLink>
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:HyperLink ID="HyperLinkReport" runat="server" NavigateUrl="~/ShowReport.aspx?srd=3" CssClass="NodeStyle" Font-Names="Arial">Report and Charts</asp:HyperLink>
                                &nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkChartDashboards" runat="server" NavigateUrl="~/ListOfDashboards.aspx" CssClass="NodeStyle" Font-Names="Arial">Chart Dashboards</asp:HyperLink>
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx?hilt=Analytics%20Dashboard" Target="_blank" CssClass="NodeStyle" Font-Names="Arial">Help</asp:HyperLink>
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:HyperLink ID="HyperLinkLogOff" runat="server" NavigateUrl="~/Default.aspx" CssClass="NodeStyle" Font-Names="Arial">Log off</asp:HyperLink>

                                <div class="dashboard">
                                    <div class="dashboardHeader">
                                        <asp:Label ID="lblHeader" runat="server" CssClass="dashboardTitle" Text="Data Analytics Dashboard"></asp:Label>
                                        <asp:Label ID="LabelDescription" runat="server" CssClass="dashboardSubTitle" Text="Analytics reports included for the current report data."></asp:Label>
                                    </div>
                                    <div class="dashboardPager">
                                        <asp:LinkButton ID="LinkButtonPrevious" runat="server" Font-Size="Small">Previous</asp:LinkButton>
                                        &nbsp;&nbsp;
                                        <asp:Label ID="LabelPageNumberCaption" runat="server" Font-Names="Arial" Font-Size="Small" Text="Page Number"></asp:Label>
                                        <asp:TextBox ID="TextBoxPageNumber" runat="server" Width="35px" Font-Names="Arial" Font-Size="Small" AutoPostBack="True"></asp:TextBox>
                                        <asp:Label ID="LabelPageCount" runat="server" Font-Names="Arial" Font-Size="Small"></asp:Label>
                                        &nbsp;&nbsp;
                                        <asp:LinkButton ID="LinkButtonNext" runat="server" Font-Size="Small">Next</asp:LinkButton>
                                    </div>

                                    <div class="tileGrid">
                                        <a id="tileAnalytics" runat="server" class="analyticsTile" href="Analytics.aspx" title="Open Analytics">
                                            <span class="tileCaption">
                                                <span class="tileTitle">Analytics</span>
                                                <span class="tileText">Grouped summaries, report analytics, charts, and selected field comparisons.</span>
                                            </span>
                                            <span class="tileBody">Use this as the main analytics page for selected report fields and groups.</span>
                                            <span class="previewBox"><asp:Literal ID="litPreviewAnalytics" runat="server"></asp:Literal></span>
                                            <span class="openText">Open</span>
                                        </a>

                                        <a id="tileStatistics" runat="server" class="analyticsTile" href="ShowReport.aspx?srd=8" title="Open Data Overall Statistics">
                                            <span class="tileCaption">
                                                <span class="tileTitle">Data Overall Statistics</span>
                                                <span class="tileText">Counts, totals, averages, minimums, maximums, and standard deviation.</span>
                                            </span>
                                            <span class="tileBody">Overall statistics for every field in the current report result.</span>
                                            <span class="previewBox"><asp:Literal ID="litPreviewStatistics" runat="server"></asp:Literal></span>
                                            <span class="openText">Open</span>
                                        </a>

                                        <a id="tileGroups" runat="server" class="analyticsTile" href="ReportViews.aspx?grpstats=yes" title="Open Groups Statistics">
                                            <span class="tileCaption">
                                                <span class="tileTitle">Groups Statistics</span>
                                                <span class="tileText">Statistics calculated separately by category or report group.</span>
                                            </span>
                                            <span class="tileBody">Grouped statistics by report category, department, customer, product, or other selected group.</span>
                                            <span class="previewBox"><asp:Literal ID="litPreviewGroups" runat="server"></asp:Literal></span>
                                            <span class="openText">Open</span>
                                        </a>

                                        <a id="tileChartRecommendations" runat="server" class="analyticsTile" href="ChartRecommendationHelpers.aspx" title="Open Chart Recommendations">
                                            <span class="tileCaption">
                                                <span class="tileTitle">Chart Recommendations</span>
                                                <span class="tileText">Suggested charts from available category, date, and numeric value fields.</span>
                                            </span>
                                            <span class="tileBody">Review chart ideas, open chart previews, and send selected charts to a dashboard.</span>
                                            <span class="previewBox"><asp:Literal ID="litPreviewChartRecommendations" runat="server"></asp:Literal></span>
                                            <span class="openText">Open</span>
                                        </a>

                                        <a id="tileDataAI" runat="server" class="analyticsTile" href="DataAI.aspx?pg=expl&amp;srd=0" title="Open DataAI">
                                            <span class="tileCaption">
                                                <span class="tileTitle">DataAI</span>
                                                <span class="tileText">Send selected report data and analytics results for AI-assisted interpretation.</span>
                                            </span>
                                            <span class="tileBody">AI-assisted interpretation for report data and generated analytics outputs.</span>
                                            <span class="previewBox"><asp:Literal ID="litPreviewDataAI" runat="server"></asp:Literal></span>
                                            <span class="openText">Open</span>
                                        </a>

                                        <a id="tileRegression" runat="server" class="analyticsTile" href="Regression.aspx" title="Open Regression Analysis">
                                            <span class="tileCaption">
                                                <span class="tileTitle">Regression Analysis</span>
                                                <span class="tileText">Understand and predict how one numeric column changes when another changes.</span>
                                            </span>
                                            <span class="tileBody">Simple linear regression with equation, slope, intercept, correlation, R squared, and predicted value.</span>
                                            <span class="previewBox"><asp:Literal ID="litPreviewRegression" runat="server"></asp:Literal></span>
                                            <span class="openText">Open</span>
                                        </a>

                                        <a id="tilePivot" runat="server" class="analyticsTile" href="Pivot.aspx" title="Open Pivot / Cross Tab">
                                            <span class="tileCaption">
                                                <span class="tileTitle">Pivot / Cross Tab</span>
                                                <span class="tileText">Row fields, column fields, values, aggregation, totals, and export.</span>
                                            </span>
                                            <span class="tileBody">Pivot-style cross-tab report with row fields, column fields, value fields, and aggregation options.</span>
                                            <span class="previewBox"><asp:Literal ID="litPreviewPivot" runat="server"></asp:Literal></span>
                                            <span class="openText">Open</span>
                                        </a>

                                        <a id="tileVariance" runat="server" class="analyticsTile" href="Variance.aspx" title="Open Variance Analysis">
                                            <span class="tileCaption">
                                                <span class="tileTitle">Variance Analysis</span>
                                                <span class="tileText">Percentage change, variance, and contribution-to-total analysis.</span>
                                            </span>
                                            <span class="tileBody">Compare base and comparison values, percentage changes, variances, and contribution totals.</span>
                                            <span class="previewBox"><asp:Literal ID="litPreviewVariance" runat="server"></asp:Literal></span>
                                            <span class="openText">Open</span>
                                        </a>

                                        <a id="tileCorrelation" runat="server" class="analyticsTile" href="ShowReport.aspx?srd=12" title="Open Correlation">
                                            <span class="tileCaption">
                                                <span class="tileTitle">Correlation</span>
                                                <span class="tileText">Relationships between numeric fields and selected measures.</span>
                                            </span>
                                            <span class="tileBody">Correlation view for comparing numeric fields in the current report data.</span>
                                            <span class="previewBox"><asp:Literal ID="litPreviewCorrelation" runat="server"></asp:Literal></span>
                                            <span class="openText">Open</span>
                                        </a>

                                        <a id="tileComparison" runat="server" class="analyticsTile" href="ComparisonReports.aspx" title="Open Comparison Reports">
                                            <span class="tileCaption">
                                                <span class="tileTitle">Comparison Reports</span>
                                                <span class="tileText">Compare two periods, groups, locations, queries, or imported files.</span>
                                            </span>
                                            <span class="tileBody">Build base versus compare reports with variance, percent change, and record counts.</span>
                                            <span class="previewBox"><asp:Literal ID="litPreviewComparison" runat="server"></asp:Literal></span>
                                            <span class="openText">Open</span>
                                        </a>

                                        <a id="tileProfiling" runat="server" class="analyticsTile" href="Profiling.aspx" title="Open Data Profiling">
                                            <span class="tileCaption">
                                                <span class="tileTitle">Data Profiling</span>
                                                <span class="tileText">Field type, count, blanks, distinct values, min, max, average, and deviation.</span>
                                            </span>
                                            <span class="tileBody">Automatic profiling of fields in the report or imported dataset.</span>
                                            <span class="previewBox"><asp:Literal ID="litPreviewProfiling" runat="server"></asp:Literal></span>
                                            <span class="openText">Open</span>
                                        </a>

                                        <a id="tileQuality" runat="server" class="analyticsTile" href="DataQuality.aspx" title="Open Data Quality">
                                            <span class="tileCaption">
                                                <span class="tileTitle">Data Quality</span>
                                                <span class="tileText">Missing values, duplicate records, invalid dates, ranges, categories, and text checks.</span>
                                            </span>
                                            <span class="tileBody">Data quality checks with links from findings to matching records in Data Explorer.</span>
                                            <span class="previewBox"><asp:Literal ID="litPreviewQuality" runat="server"></asp:Literal></span>
                                            <span class="openText">Open</span>
                                        </a>

                                        <a id="tileRanking" runat="server" class="analyticsTile" href="Ranking.aspx" title="Open Ranking Analysis">
                                            <span class="tileCaption">
                                                <span class="tileTitle">Ranking Analysis</span>
                                                <span class="tileText">Top, bottom, and average ranking with grouped values and record drill-down.</span>
                                            </span>
                                            <span class="tileBody">Top, bottom, and average-nearest ranking for categories, customers, products, departments, or locations.</span>
                                            <span class="previewBox"><asp:Literal ID="litPreviewRanking" runat="server"></asp:Literal></span>
                                            <span class="openText">Open</span>
                                        </a>

                                        <a id="tileTimeBased" runat="server" class="analyticsTile" href="TimeBasedSummaries.aspx" title="Open Time Based Summaries">
                                            <span class="tileCaption">
                                                <span class="tileTitle">Time Based Summaries</span>
                                                <span class="tileText">Summaries by day, week, month, quarter, or year when date fields exist.</span>
                                            </span>
                                            <span class="tileBody">Grouped totals, counts, averages, minimums, and maximums over selected time periods.</span>
                                            <span class="previewBox"><asp:Literal ID="litPreviewTimeBased" runat="server"></asp:Literal></span>
                                            <span class="openText">Open</span>
                                        </a>

                                        <a id="tileTimeSeries" runat="server" class="analyticsTile" href="TimeSeries.aspx" title="Open Time Series">
                                            <span class="tileCaption">
                                                <span class="tileTitle">Time Series</span>
                                                <span class="tileText">Moving averages and rolling totals for time-series style reports.</span>
                                            </span>
                                            <span class="tileBody">Rolling calculations by date period with period totals and moving averages.</span>
                                            <span class="previewBox"><asp:Literal ID="litPreviewTimeSeries" runat="server"></asp:Literal></span>
                                            <span class="openText">Open</span>
                                        </a>

                                        <a id="tileOutliers" runat="server" class="analyticsTile" href="OutlierFlagging.aspx" title="Open Outlier Flagging">
                                            <span class="tileCaption">
                                                <span class="tileTitle">Outlier Flagging</span>
                                                <span class="tileText">Standard deviation, percentage difference, and business-rule outlier checks.</span>
                                            </span>
                                            <span class="tileBody">Find unusual values and review the source records behind flagged rows.</span>
                                            <span class="previewBox"><asp:Literal ID="litPreviewOutliers" runat="server"></asp:Literal></span>
                                            <span class="openText">Open</span>
                                        </a>

                                        <a id="tileMapReadiness" runat="server" class="analyticsTile" href="MapReadines.aspx" title="Open Map Readiness">
                                            <span class="tileCaption">
                                                <span class="tileTitle">Map Readiness</span>
                                                <span class="tileText">Latitude, longitude, missing coordinates, duplicate locations, and KML-ready checks.</span>
                                            </span>
                                            <span class="tileBody">Review whether report data is ready for map reports and location exports.</span>
                                            <span class="previewBox"><asp:Literal ID="litPreviewMapReadiness" runat="server"></asp:Literal></span>
                                            <span class="openText">Open</span>
                                        </a>

                                        <a id="tileMatrix" runat="server" class="analyticsTile" href="ShowReport.aspx?srd=13" title="Open Matrix Balancing">
                                            <span class="tileCaption">
                                                <span class="tileTitle">Matrix Balancing</span>
                                                <span class="tileText">Matrix view for comparing rows, columns, and totals.</span>
                                            </span>
                                            <span class="tileBody">Matrix-style balancing and comparison using selected row and column dimensions. Select Scenario.</span>
                                            <span class="previewBox"><asp:Literal ID="litPreviewMatrix" runat="server"></asp:Literal></span>
                                            <span class="openText">Open</span>
                                        </a>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpDataAdmin">
            <ProgressTemplate>
                <div class="modal">
                    <div class="center">
                        <asp:Image ID="imgProgress" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />
                        Please Wait...
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
    </form>
</body>
</html>


