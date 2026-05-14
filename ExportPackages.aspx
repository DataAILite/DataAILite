<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ExportPackages.aspx.vb" Inherits="ExportPackages" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Export Packages</title>
    <style type="text/css">
        .NodeStyle { color:#0066FF; font-size:12px; font-weight:normal; text-decoration:none; }
        .NodeStyle:hover { text-decoration:underline; color:darkblue; }
        .ticketbutton { width:120px; height:25px; font-size:12px; border-radius:5px; border-style:solid; border-color:#4e4747; color:black; border-width:1px; background-image:linear-gradient(to bottom, rgba(211,211,211,0),rgba(211,211,250,3)); padding:3px; margin:5px; }
        .analysisgrid { font-family:Arial; font-size:12px; border-collapse:collapse; background-color:white; }
        .analysisgrid th { background-color:#663300; color:white; border:1px solid white; padding:4px; white-space:nowrap; }
        .analysisgrid td { border:1px solid #d0d0d0; padding:4px; white-space:nowrap; }
        .controlpanel { background-color:#e5e5e5; border:medium double #FFFFFF; color:black; font-family:Arial; font-size:small; width:auto; min-width:760px; max-width:980px; margin-left:0; margin-right:auto; }
        .modal { position:fixed; z-index:2147483647; height:100%; width:100%; top:0; background-color:#f8f8d3; opacity:0.8; }
        .center { z-index:2147483647; margin:300px auto; padding-left:25px; padding-top:10px; width:130px; background-color:#f8f8d3; border-radius:10px; }
        .center img { height:100px; width:100px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <asp:UpdatePanel ID="udpExportPackages" runat="server">
            <ContentTemplate>
                <table>
                    <tr><td colspan="3" style="font-size:x-large; font-weight:bold; background-color:#e5e5e5; height:40px;"><asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label></td></tr>
                    <tr>
                        <td style="font-size:x-small; background-color:#e5e5e5; vertical-align:top; width:15%;">
                            <asp:TreeView ID="TreeView1" runat="server" Width="100%" NodeIndent="10" Font-Names="Times New Roman" EnableTheming="True" ImageSet="BulletedList">
                                <Nodes>
                                        <asp:TreeNode Text="&lt;b&gt;Log Off&lt;/b&gt;" Value="Default.aspx" Expanded="True"></asp:TreeNode>
                                        <asp:TreeNode Text="&lt;b&gt;List of Reports&lt;/b&gt;" Value="ListOfReports.aspx" Expanded="True"></asp:TreeNode>
                                        <asp:TreeNode Text="&lt;b&gt;Report Definition&lt;/b&gt;" Value="ReportEdit.aspx?tne=2" Expanded="True"></asp:TreeNode>
                                        <asp:TreeNode Text="&lt;b&gt;Report Data Query&lt;/b&gt;" Value="SQLquery.aspx?tnq=0" Expanded="True"></asp:TreeNode>
                                        <asp:TreeNode Text="&lt;b&gt;Report Format Definition&lt;/b&gt;" Value="RDLformat.aspx?tnf=0" Expanded="True"></asp:TreeNode>
                                        <asp:TreeNode Text="Explore Report Data" Value="ShowReport.aspx?srd=0" Expanded="True">
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
                                <RootNodeStyle HorizontalPadding="2px" Font-Bold="True" Font-Underline="False" /><NodeStyle CssClass="NodeStyle" /><ParentNodeStyle Font-Bold="True" />
                            </asp:TreeView>
                        </td>
                        <td style="width:5px;"></td>
                        <td style="width:85%; text-align:left; vertical-align:top;">
                            <asp:HyperLink ID="HyperLinkAnalytics" runat="server" NavigateUrl="~/Analytics.aspx" CssClass="NodeStyle" Font-Names="Arial">Analytics</asp:HyperLink>
                            &nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkReport" runat="server" NavigateUrl="~/ShowReport.aspx?srd=3" CssClass="NodeStyle" Font-Names="Arial">Report and Charts</asp:HyperLink>
                            &nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx" Target="_blank" CssClass="NodeStyle" Font-Names="Arial">Help</asp:HyperLink>
                            &nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkLogOff" runat="server" NavigateUrl="~/Default.aspx" CssClass="NodeStyle" Font-Names="Arial">Log off</asp:HyperLink>
                            <br /><br />
                            <div style="text-align:center;"><asp:Label ID="lblHeader" runat="server" Font-Size="22px" Font-Names="Arial">Export Packages</asp:Label></div>
                            <br /><br />
                            <table class="controlpanel" cellpadding="4" cellspacing="0">
                                <tr>
                                    <td><asp:CheckBox ID="CheckReport" runat="server" Text="Report" Checked="True" /></td>
                                    <td><asp:CheckBox ID="CheckDefinitions" runat="server" Text="Report Definition" Checked="True" /></td>
                                    <td>
                                        <asp:Label ID="LabelDataFormat" runat="server" Text="Data in format:" AssociatedControlID="DropDownDataFormat"></asp:Label>
                                        <asp:DropDownList ID="DropDownDataFormat" runat="server">
                                            <asp:ListItem Text="CSV" Value="CSV" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="Excel" Value="Excel"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td><asp:CheckBox ID="CheckCharts" runat="server" Text="Charts" Checked="True" /></td>
                                    <td><asp:CheckBox ID="CheckAIAnalysis" runat="server" Text="AI analysis" Checked="True" /></td>
                                    <td><asp:CheckBox ID="CheckNotes" runat="server" Text="Notes" Checked="True" /></td>
                                </tr>
                                <tr><td colspan="3" style="font-weight:bold;">Notes:<br /><asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Rows="2" Width="680px"></asp:TextBox></td></tr>
                                <tr><td colspan="3"><asp:Button ID="ButtonBuild" runat="server" CssClass="ticketbutton" Text="Export" OnClientClick="showExportWaiting();" /></td></tr>
                            </table>
                            <asp:Label ID="LabelError" runat="server" ForeColor="Red" Font-Names="Arial" Font-Size="Medium"></asp:Label><br />
                            <asp:Label ID="LabelInfo" runat="server" ForeColor="Black" Font-Names="Arial" Font-Size="Small"></asp:Label><br /><br />
                            <div style="overflow:auto; max-width:100%;"><asp:GridView ID="GridViewPackage" runat="server" CssClass="analysisgrid" AutoGenerateColumns="True" GridLines="Both"></asp:GridView></div>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers><asp:PostBackTrigger ControlID="ButtonBuild" /></Triggers>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpExportPackages">
            <ProgressTemplate>
                <div class="modal">
                    <div class="center">
                        <asp:Image ID="imgProgress" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />
                        Please Wait...
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <div id="ExportWaitingPanel" class="modal" style="display:none;">
            <div class="center">
                <img src="Controls/Images/WaitImage2.gif" alt="Please Wait" />
                Please Wait...
            </div>
        </div>
    </form>
    <script type="text/javascript">
        var exportWaitingTimer = null;

        function showExportWaiting() {
            var waitingPanel = document.getElementById('ExportWaitingPanel');
            if (waitingPanel) {
                waitingPanel.style.display = 'block';
                if (exportWaitingTimer) {
                    window.clearTimeout(exportWaitingTimer);
                }
                exportWaitingTimer = window.setTimeout(hideExportWaiting, 60000);
            }
            return true;
        }

        function hideExportWaiting() {
            var waitingPanel = document.getElementById('ExportWaitingPanel');
            if (waitingPanel) {
                waitingPanel.style.display = 'none';
            }
            if (exportWaitingTimer) {
                window.clearTimeout(exportWaitingTimer);
                exportWaitingTimer = null;
            }
        }

        window.addEventListener('pageshow', hideExportWaiting);
    </script>
</body>
</html>

