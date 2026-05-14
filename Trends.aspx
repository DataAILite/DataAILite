<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Trends.aspx.vb" Inherits="Trends" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Trends and Predictions</title>
    <style type="text/css">
        .NodeStyle { color:#0066FF; font-size:12px; font-weight:normal; text-decoration:none; }
        .NodeStyle:hover { text-decoration:underline; color:darkblue; }
        .ticketbutton { width:90px; height:25px; font-size:12px; border-radius:5px; border-style:solid; border-color:#4e4747; color:black; border-width:1px; background-image:linear-gradient(to bottom, rgba(211,211,211,0),rgba(211,211,250,3)); padding:3px; margin:5px; }
        a.ticketbutton, a.ticketbutton:visited, a.ticketbutton:hover { text-decoration:none; display:inline-flex; align-items:center; justify-content:center; box-sizing:border-box; vertical-align:middle; }
        .controlpanel { background-color:#e5e5e5; border:medium double #FFFFFF; color:black; font-family:Arial; font-size:small; width:auto; min-width:760px; max-width:980px; margin-left:0; margin-right:auto; }
        .chartwrap { font-family:Arial; margin-top:14px; width:1120px; max-width:100%; }
        .charttitle { text-align:center; width:1104px; max-width:100%; margin-bottom:8px; }
        .charttools { font-family:Arial; font-size:12px; margin-bottom:6px; }
        .chartviewport { width:100%; max-width:1160px; background-color:#f8f8f8; }
        .chartrow { display:flex; align-items:flex-start; }
        .chartcanvasbox { border:1px solid #d0d0d0; background-color:#ffffff; width:1104px; height:654px; overflow:hidden; }
        #trendCanvas { border:1px solid #b8b8b8; background-color:#ffffff; cursor:crosshair; display:block; }
        .rangebar { width:948px; vertical-align:middle; }
        .rangeLabel { display:inline-block; width:78px; font-weight:bold; }
        .xpanrow { margin-top:6px; padding-left:108px; white-space:nowrap; }
        .ypanbox { width:42px; height:650px; margin-left:8px; position:relative; }
        .ypanbox input[type=range] { position:absolute; left:-304px; top:304px; width:650px; transform:rotate(-90deg); transform-origin:center; }
        .ypanlabel { font-family:Arial; font-size:11px; color:#333333; position:absolute; left:0; top:0; writing-mode:vertical-rl; transform:rotate(180deg); height:650px; text-align:center; }
        .statusline { font-family:Arial; font-size:12px; color:#333333; margin-top:8px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <table>
            <tr><td colspan="3" style="font-size:x-large; font-weight:bold; background-color:#e5e5e5; height:40px;"><asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label></td></tr>
            <tr>
                <td style="font-size:x-small; background-color:#e5e5e5; vertical-align:top; width:15%;">
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
                        <RootNodeStyle HorizontalPadding="2px" Font-Bold="True" Font-Underline="False" /><NodeStyle CssClass="NodeStyle" /><ParentNodeStyle Font-Bold="True" />
                    </asp:TreeView>
                </td>
                <td style="width:5px;"></td>
                <td style="width:85%; text-align:left; vertical-align:top;">
                    <asp:HyperLink ID="HyperLinkRegression" runat="server" NavigateUrl="~/Regression.aspx" CssClass="NodeStyle" Font-Names="Arial">Regression Analysis</asp:HyperLink>
                    &nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkAnalytics" runat="server" NavigateUrl="~/Analytics.aspx" CssClass="NodeStyle" Font-Names="Arial">Analytics</asp:HyperLink>
                    &nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx?hilt=Trends" Target="_blank" CssClass="NodeStyle" Font-Names="Arial">Help</asp:HyperLink>
                    &nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkLogOff" runat="server" NavigateUrl="~/Default.aspx" CssClass="NodeStyle" Font-Names="Arial">Log off</asp:HyperLink>
                    <br /><br />
                    <table class="controlpanel" cellpadding="4" cellspacing="0">
                        <tr>
                            <td style="font-weight:bold;">Equation:</td>
                            <td><asp:TextBox ID="txtEquation" runat="server" Width="520px"></asp:TextBox></td>
                            <td style="font-weight:bold;">X Value:</td>
                            <td><asp:TextBox ID="txtXValue" runat="server" Width="110px"></asp:TextBox></td>
                            <td><input type="button" class="ticketbutton" value="Draw" onclick="drawTrendChart();" /></td>
                            <td>
                                <asp:HiddenField ID="HiddenTrendEquation" runat="server" />
                                <asp:HiddenField ID="HiddenTrendXValue" runat="server" />
                                <asp:HiddenField ID="HiddenTrendImage" runat="server" />
                                <asp:LinkButton ID="ButtonExportExcel" runat="server" CssClass="ticketbutton" Style="width:110px;" OnClientClick="return prepareTrendExcelExport();">Export to Excel</asp:LinkButton>
                            </td>
                            <td><input type="button" class="ticketbutton" value="Export to PDF" style="width:110px;" onclick="exportTrendPdf();" /></td>
                        </tr>
                    </table>
                    <asp:Label ID="LabelError" runat="server" ForeColor="Red" Font-Names="Arial" Font-Size="Medium"></asp:Label>
                    <div class="chartwrap">
                        <div class="charttitle">
                            <asp:Label ID="lblHeader" runat="server" Font-Size="22px" Font-Names="Arial">Trends and Predictions</asp:Label><br />
                            <asp:Label ID="lblSubtitle" runat="server" Font-Size="12px" Font-Names="Arial" ForeColor="#333333"></asp:Label>
                        </div>
                        <div class="charttools">
                            <input type="button" class="ticketbutton" value="Zoom In" onclick="zoomTrendChart(1.25);" />
                            <input type="button" class="ticketbutton" value="Zoom Out" onclick="zoomTrendChart(0.8);" />
                            <input type="button" class="ticketbutton" value="Reset Zoom" style="width:100px;" onclick="resetTrendZoom();" />
                            <span id="trendZoomLabel">Zoom: 100%</span>
                        </div>
                        <div id="trendViewport" class="chartviewport">
                            <div class="chartrow">
                                <div class="chartcanvasbox">
                                    <canvas id="trendCanvas" width="1100" height="650"></canvas>
                                </div>
                                <div class="ypanbox">
                                    <input id="trendYPan" type="range" min="0" max="100" value="50" oninput="drawTrendChart();" />
                                    <span id="trendYPanLabel" class="ypanlabel">Y range</span>
                                </div>
                            </div>
                            <div class="xpanrow">
                                <input id="trendXPan" class="rangebar" type="range" min="0" max="100" value="50" oninput="drawTrendChart();" />
                                <span id="trendXPanLabel">X range</span>
                            </div>
                        </div>
                        <div id="trendStatus" class="statusline"></div>
                    </div>
                </td>
            </tr>
        </table>
    </form>
    <script type="text/javascript">
        var equationBoxId = '<%=txtEquation.ClientID%>';
        var xBoxId = '<%=txtXValue.ClientID%>';
        var trendZoom = 1;
        var trendBaseWidth = 1100;
        var trendBaseHeight = 650;
        var trendState = null;

        function parseNumber(text) {
            if (text === null || text === undefined) return NaN;
            return parseFloat(String(text).replace(/,/g, '').trim());
        }

        function compileEquation(text) {
            var clean = String(text || '').trim();
            var parts = clean.split('=');
            var expression = parts.length > 1 ? parts.slice(1).join('=') : clean;
            expression = expression.replace(/\s+/g, '');
            expression = expression.replace(/(\d),(?=\d{3}(\D|$))/g, '$1');
            expression = expression.replace(/Math\./gi, '');
            expression = expression.replace(/\bABS\b/gi, 'abs');
            expression = expression.replace(/\bACOS\b/gi, 'acos');
            expression = expression.replace(/\bASIN\b/gi, 'asin');
            expression = expression.replace(/\bATAN\b/gi, 'atan');
            expression = expression.replace(/\bCEIL\b/gi, 'ceil');
            expression = expression.replace(/\bCOS\b/gi, 'cos');
            expression = expression.replace(/\bEXP\b/gi, 'exp');
            expression = expression.replace(/\bFLOOR\b/gi, 'floor');
            expression = expression.replace(/\bLN\b/gi, 'log');
            expression = expression.replace(/\bLOG\b/gi, 'log');
            expression = expression.replace(/\bMAX\b/gi, 'max');
            expression = expression.replace(/\bMIN\b/gi, 'min');
            expression = expression.replace(/\bPOW\b/gi, 'pow');
            expression = expression.replace(/\bROUND\b/gi, 'round');
            expression = expression.replace(/\bSIN\b/gi, 'sin');
            expression = expression.replace(/\bSQRT\b/gi, 'sqrt');
            expression = expression.replace(/\bTAN\b/gi, 'tan');
            expression = expression.replace(/\bPI\b/gi, 'PI');
            expression = expression.replace(/\bE\b/gi, 'E');
            expression = expression.replace(/\bX\b/gi, 'X');
            expression = replacePowers(expression);

            var allowed = /^(X|PI|E|abs|acos|asin|atan|ceil|cos|exp|floor|log|max|min|pow|round|sin|sqrt|tan|\d|\+|\-|\*|\/|\.|\(|\)|,)*$/;
            if (!allowed.test(expression)) return null;

            try {
                var fn = new Function('X', 'var abs=Math.abs,acos=Math.acos,asin=Math.asin,atan=Math.atan,ceil=Math.ceil,cos=Math.cos,exp=Math.exp,floor=Math.floor,log=Math.log,max=Math.max,min=Math.min,pow=Math.pow,round=Math.round,sin=Math.sin,sqrt=Math.sqrt,tan=Math.tan,PI=Math.PI,E=Math.E; return ' + expression + ';');
                var testValue = fn(1);
                if (!isFinite(testValue)) return null;
                return fn;
            } catch (ex) {
                return null;
            }
        }

        function replacePowers(expression) {
            var token = '(?:X|E|PI|\\d+(?:\\.\\d+)?|[a-z]+\\([^()]*\\)|\\([^()]*\\))';
            var powerPattern = new RegExp('(' + token + ')\\^(' + token + ')', 'i');
            var previous = '';
            while (expression.indexOf('^') >= 0 && previous !== expression) {
                previous = expression;
                expression = expression.replace(powerPattern, 'pow($1,$2)');
            }
            return expression;
        }

        function niceRange(minValue, maxValue) {
            if (!isFinite(minValue) || !isFinite(maxValue)) return { min: -10, max: 10 };
            if (minValue === maxValue) {
                var spread = Math.max(1, Math.abs(minValue) * 0.25);
                minValue -= spread;
                maxValue += spread;
            }
            var padding = (maxValue - minValue) * 0.15;
            minValue -= padding;
            maxValue += padding;
            if (minValue > 0) minValue = Math.min(0, minValue);
            if (maxValue < 0) maxValue = Math.max(0, maxValue);
            return { min: minValue, max: maxValue };
        }

        function centeredRange(centerValue) {
            if (!isFinite(centerValue)) return { min: -10, max: 10 };
            var halfRange = Math.max(5, Math.abs(centerValue) * 0.5);
            return { min: centerValue - halfRange, max: centerValue + halfRange };
        }

        function updateTrendZoomLabel() {
            var label = document.getElementById('trendZoomLabel');
            if (label) {
                var zoomPercent = trendZoom * 100;
                label.innerHTML = 'Zoom: ' + (zoomPercent < 10 ? zoomPercent.toFixed(2) : Math.round(zoomPercent)) + '%';
            }
        }

        function setCanvasSize() {
            var canvas = document.getElementById('trendCanvas');
            if (canvas.width !== trendBaseWidth) canvas.width = trendBaseWidth;
            if (canvas.height !== trendBaseHeight) canvas.height = trendBaseHeight;
            updateTrendZoomLabel();
        }

        function readRangeBar(id) {
            var bar = document.getElementById(id);
            if (!bar) return 50;
            var value = parseFloat(bar.value);
            if (!isFinite(value)) return 50;
            return Math.max(0, Math.min(100, value));
        }

        function updateRangeLabels(xRange, yRange) {
            var xLabel = document.getElementById('trendXPanLabel');
            var yLabel = document.getElementById('trendYPanLabel');
            if (xLabel) xLabel.innerHTML = 'X: ' + xRange.min.toFixed(4) + ' to ' + xRange.max.toFixed(4);
            if (yLabel) yLabel.innerHTML = 'Y: ' + yRange.min.toFixed(4) + ' to ' + yRange.max.toFixed(4);
        }

        function expandRange(range, factor) {
            var center = (range.min + range.max) / 2;
            var half = (range.max - range.min) * factor / 2;
            if (!isFinite(half) || half <= 0) half = 10;
            return { min: center - half, max: center + half };
        }

        function visibleRange(fullRange, zoom, panPercent) {
            var fullSpan = fullRange.max - fullRange.min;
            if (!isFinite(fullSpan) || fullSpan <= 0) return fullRange;
            var visibleSpan = fullSpan / (zoom * 4);
            var navigationRange = navigationRangeForZoom(fullRange, visibleSpan);
            var travel = navigationRange.max - navigationRange.min - visibleSpan;
            if (travel < 0) travel = 0;
            var start = navigationRange.min + travel * (panPercent / 100);
            return { min: start, max: start + visibleSpan };
        }

        function navigationRangeForZoom(fullRange, visibleSpan) {
            var fullSpan = fullRange.max - fullRange.min;
            var center = (fullRange.min + fullRange.max) / 2;
            var navigationSpan = Math.max(fullSpan, visibleSpan * 2);
            return { min: center - navigationSpan / 2, max: center + navigationSpan / 2 };
        }

        function rangeCenter(fullRange, zoom, panPercent) {
            var visible = visibleRange(fullRange, zoom, panPercent);
            return (visible.min + visible.max) / 2;
        }

        function panForCenter(fullRange, zoom, centerValue) {
            var fullSpan = fullRange.max - fullRange.min;
            if (!isFinite(fullSpan) || fullSpan <= 0) return 50;
            var visibleSpan = fullSpan / (zoom * 4);
            var navigationRange = navigationRangeForZoom(fullRange, visibleSpan);
            var travel = navigationRange.max - navigationRange.min - visibleSpan;
            if (travel <= 0) return 50;
            return Math.max(0, Math.min(100, ((centerValue - visibleSpan / 2 - navigationRange.min) / travel) * 100));
        }

        function buildTrendState(equation, equationText, xValue, selectedY, selectedPointIsValid) {
            var baseXRange = expandRange(centeredRange(xValue), 4);
            var sampled = sampleEquation(equation, baseXRange, selectedY);
            var minY = sampled.minY;
            var maxY = sampled.maxY;
            if (!selectedPointIsValid && sampled.points.length > 0) selectedY = (minY + maxY) / 2;
            var baseYRange = niceRange(minY, maxY);
            if (selectedPointIsValid) {
                if (selectedY < baseYRange.min) baseYRange.min = selectedY;
                if (selectedY > baseYRange.max) baseYRange.max = selectedY;
            }

            return {
                equationText: equationText,
                xValue: xValue,
                yValue: selectedY,
                selectedPointIsValid: selectedPointIsValid,
                baseXRange: baseXRange,
                baseYRange: baseYRange
            };
        }

        function currentTrendState(equation, equationText, xValue, selectedY, selectedPointIsValid) {
            if (!trendState || trendState.equationText !== equationText || Math.abs(trendState.xValue - xValue) > 0.0000001) {
                trendZoom = 1;
                var xBar = document.getElementById('trendXPan');
                var yBar = document.getElementById('trendYPan');
                if (xBar) xBar.value = 50;
                if (yBar) yBar.value = 50;
                trendState = buildTrendState(equation, equationText, xValue, selectedY, selectedPointIsValid);
                if (xBar) xBar.value = panForCenter(trendState.baseXRange, trendZoom, trendState.xValue);
                if (yBar) yBar.value = panForCenter(trendState.baseYRange, trendZoom, trendState.yValue);
            }
            return trendState;
        }

        function zoomTrendChart(factor) {
            if (trendState) {
                var xBar = document.getElementById('trendXPan');
                var yBar = document.getElementById('trendYPan');
                var xCenter = rangeCenter(trendState.baseXRange, trendZoom, readRangeBar('trendXPan'));
                var yCenter = rangeCenter(trendState.baseYRange, trendZoom, readRangeBar('trendYPan'));
                var newZoom = Math.max(0.001, Math.min(8, trendZoom * factor));
                if (xBar) xBar.value = panForCenter(trendState.baseXRange, newZoom, xCenter);
                if (yBar) yBar.value = panForCenter(trendState.baseYRange, newZoom, yCenter);
                trendZoom = newZoom;
                drawTrendChart();
                return;
            }
            trendZoom = Math.max(0.001, Math.min(8, trendZoom * factor));
            drawTrendChart();
        }

        function resetTrendZoom() {
            trendZoom = 1;
            var xBar = document.getElementById('trendXPan');
            var yBar = document.getElementById('trendYPan');
            if (xBar) xBar.value = 50;
            if (yBar) yBar.value = 50;
            trendState = null;
            drawTrendChart();
        }

        function drawTrendChart() {
            setCanvasSize();
            var canvas = document.getElementById('trendCanvas');
            var ctx = canvas.getContext('2d');
            var status = document.getElementById('trendStatus');
            var equationText = document.getElementById(equationBoxId).value;
            var equation = compileEquation(equationText);
            var xValue = parseNumber(document.getElementById(xBoxId).value);

            ctx.clearRect(0, 0, canvas.width, canvas.height);
            if (!equation || !isFinite(xValue)) {
                trendState = null;
                status.innerHTML = 'Enter an equation like Y = 12.5 + 3.4 * X, Y = 10 + 2 * X * X, Y = 10 * exp(0.2 * X), or Y = 2 ^ X.';
                return;
            }

            var yValue = equation(xValue);
            var selectedPointIsValid = isFinite(yValue);
            if (!selectedPointIsValid) yValue = 0;
            var state = currentTrendState(equation, equationText, xValue, yValue, selectedPointIsValid);
            yValue = state.yValue;
            selectedPointIsValid = state.selectedPointIsValid;
            var xRange = visibleRange(state.baseXRange, trendZoom, readRangeBar('trendXPan'));
            var yRange = visibleRange(state.baseYRange, trendZoom, readRangeBar('trendYPan'));
            var sampled = sampleEquation(equation, xRange, yValue);
            var points = sampled.points;
            updateRangeLabels(xRange, yRange);
            var left = 108, right = 40, top = 40, bottom = 72;
            var plotW = canvas.width - left - right;
            var plotH = canvas.height - top - bottom;

            function px(x) { return left + ((x - xRange.min) / (xRange.max - xRange.min)) * plotW; }
            function py(y) { return top + plotH - ((y - yRange.min) / (yRange.max - yRange.min)) * plotH; }
            function dataX(pixelX) { return xRange.min + ((pixelX - left) / plotW) * (xRange.max - xRange.min); }
            function clamp(value, minValue, maxValue) { return Math.max(minValue, Math.min(maxValue, value)); }

            ctx.font = '12px Arial';
            ctx.strokeStyle = '#d6d6d6';
            ctx.lineWidth = 1;
            ctx.fillStyle = '#333';
            ctx.textAlign = 'left';
            ctx.textBaseline = 'alphabetic';
            for (var i = 0; i <= 8; i++) {
                var gx = left + i * plotW / 8;
                var gy = top + i * plotH / 8;
                ctx.beginPath(); ctx.moveTo(gx, top); ctx.lineTo(gx, top + plotH); ctx.stroke();
                ctx.beginPath(); ctx.moveTo(left, gy); ctx.lineTo(left + plotW, gy); ctx.stroke();
                var xv = xRange.min + i * (xRange.max - xRange.min) / 8;
                var yv = yRange.max - i * (yRange.max - yRange.min) / 8;
                ctx.fillText(xv.toFixed(2), gx - 18, top + plotH + 22);
                ctx.textAlign = 'right';
                ctx.fillText(yv.toFixed(2), left - 12, gy + 4);
                ctx.textAlign = 'left';
            }

            ctx.strokeStyle = '#b7b7b7';
            ctx.lineWidth = 1.5;
            ctx.beginPath(); ctx.rect(left, top, plotW, plotH); ctx.stroke();
            var hasYAxis = xRange.min <= 0 && xRange.max >= 0;
            var hasXAxis = yRange.min <= 0 && yRange.max >= 0;
            var zeroX = hasYAxis ? px(0) : left;
            var zeroY = hasXAxis ? py(0) : top + plotH;
            if (hasYAxis) {
                ctx.strokeStyle = '#8a8a8a';
                ctx.beginPath(); ctx.moveTo(zeroX, top); ctx.lineTo(zeroX, top + plotH); ctx.stroke();
                drawArrow(ctx, zeroX, top + 16, zeroX, top + 2);
            }
            if (hasXAxis) {
                ctx.strokeStyle = '#8a8a8a';
                ctx.beginPath(); ctx.moveTo(left, zeroY); ctx.lineTo(left + plotW, zeroY); ctx.stroke();
                drawArrow(ctx, left + plotW - 16, zeroY, left + plotW - 2, zeroY);
            }
            ctx.font = 'bold 12px Arial';
            if (hasYAxis && hasXAxis) {
                ctx.fillText('(0,0)', zeroX + 6, zeroY + 16);
            } else if (hasYAxis) {
                ctx.fillText('0', zeroX + 5, top + plotH + 38);
            } else if (hasXAxis) {
                ctx.font = 'bold 12px Arial';
                ctx.textAlign = 'right';
                ctx.fillText('0', left - 12, zeroY - 6);
                ctx.textAlign = 'left';
            }

            ctx.strokeStyle = '#0066cc';
            ctx.lineWidth = 3;
            ctx.beginPath();
            var started = false;
            for (var q = 0; q < points.length; q++) {
                var drawX = px(points[q].x);
                var drawY = py(points[q].y);
                if (!isFinite(drawY) || drawY < top - plotH || drawY > top + plotH * 2) {
                    started = false;
                    continue;
                }
                if (!started) {
                    ctx.moveTo(drawX, drawY);
                    started = true;
                } else {
                    ctx.lineTo(drawX, drawY);
                }
            }
            ctx.stroke();

            var sx = px(xValue);
            var sy = py(yValue);
            var axisY = clamp(py(0), top, top + plotH);
            var axisX = clamp(px(0), left, left + plotW);
            if (selectedPointIsValid) {
                ctx.strokeStyle = '#800000';
                ctx.fillStyle = '#800000';
                ctx.lineWidth = 2;
                ctx.setLineDash([4, 4]);
                ctx.beginPath(); ctx.moveTo(sx, axisY); ctx.lineTo(sx, sy); ctx.lineTo(axisX, sy); ctx.stroke();
                ctx.setLineDash([]);

                drawMarker(ctx, sx, sy, '(' + xValue.toFixed(4) + ', ' + yValue.toFixed(4) + ')', 12, 18);
            }

            ctx.fillStyle = '#333';
            ctx.font = 'bold 13px Arial';
            ctx.textAlign = 'center';
            ctx.fillText('X Axis', left + plotW / 2, canvas.height - 12);
            ctx.save();
            ctx.font = 'bold 13px Arial';
            ctx.fillStyle = '#333';
            ctx.textAlign = 'center';
            ctx.textBaseline = 'middle';
            ctx.translate(24, top + plotH / 2);
            ctx.rotate(-Math.PI / 2);
            ctx.fillText('Y Axis', 0, 0);
            ctx.restore();
            ctx.textAlign = 'left';
            ctx.textBaseline = 'alphabetic';

            if (selectedPointIsValid) {
                status.innerHTML = htmlEncode(equationText) + '; selected point Y = ' + yValue.toFixed(4);
            } else {
                status.innerHTML = 'The equation does not return a finite Y value for this X value. The chart grid remains visible for nearby valid values.';
            }
            canvas.onclick = function (evt) {
                var rect = canvas.getBoundingClientRect();
                var scaleX = canvas.width / rect.width;
                var clickedX = (evt.clientX - rect.left) * scaleX;
                if (clickedX < left) clickedX = left;
                if (clickedX > left + plotW) clickedX = left + plotW;
                document.getElementById(xBoxId).value = dataX(clickedX).toFixed(4);
                drawTrendChart();
            };
        }

        function sampleEquation(equation, startRange, selectedY) {
            var range = { min: startRange.min, max: startRange.max };
            var result = { xRange: { min: range.min, max: range.max }, points: [], minY: selectedY, maxY: selectedY };
            for (var p = 0; p <= 420; p++) {
                var pointX = range.min + p * (range.max - range.min) / 420;
                var pointY = equation(pointX);
                if (isFinite(pointY)) {
                    result.points.push({ x: pointX, y: pointY });
                    if (pointY < result.minY) result.minY = pointY;
                    if (pointY > result.maxY) result.maxY = pointY;
                }
            }
            return result;
        }

        function drawMarker(ctx, x, y, label, dx, dy) {
            drawCircle(ctx, x, y);
            ctx.font = 'bold 12px Arial';
            ctx.fillText(label, x + dx, y + dy);
        }

        function drawCircle(ctx, x, y) {
            ctx.beginPath();
            ctx.arc(x, y, 7, 0, Math.PI * 2);
            ctx.stroke();
        }

        function drawArrow(ctx, fromX, fromY, toX, toY) {
            var headLength = 10;
            var angle = Math.atan2(toY - fromY, toX - fromX);
            ctx.beginPath();
            ctx.moveTo(fromX, fromY);
            ctx.lineTo(toX, toY);
            ctx.lineTo(toX - headLength * Math.cos(angle - Math.PI / 6), toY - headLength * Math.sin(angle - Math.PI / 6));
            ctx.moveTo(toX, toY);
            ctx.lineTo(toX - headLength * Math.cos(angle + Math.PI / 6), toY - headLength * Math.sin(angle + Math.PI / 6));
            ctx.stroke();
        }

        function prepareTrendExcelExport() {
            drawTrendChart();
            var canvas = document.getElementById('trendCanvas');
            var equation = document.getElementById(equationBoxId).value;
            var xValue = document.getElementById(xBoxId).value;
            document.getElementById('<%=HiddenTrendEquation.ClientID%>').value = equation;
            document.getElementById('<%=HiddenTrendXValue.ClientID%>').value = xValue;
            document.getElementById('<%=HiddenTrendImage.ClientID%>').value = canvas.toDataURL('image/png');
            return true;
        }

        function exportTrendPdf() {
            drawTrendChart();
            var canvas = document.getElementById('trendCanvas');
            var equation = document.getElementById(equationBoxId).value;
            var xValue = document.getElementById(xBoxId).value;
            var popup = window.open('', '_blank');
            popup.document.write('<html><head><title>Trends and Predictions</title></head><body style="font-family:Arial;">');
            popup.document.write('<h3>Trends and Predictions</h3>');
            popup.document.write('<div><b>Equation:</b> ' + htmlEncode(equation) + '</div>');
            popup.document.write('<div><b>X Value:</b> ' + htmlEncode(xValue) + '</div><br />');
            popup.document.write('<img style="max-width:100%;" src="' + canvas.toDataURL('image/png') + '" />');
            popup.document.write('<script>window.onload=function(){window.print();};<\/script>');
            popup.document.write('</body></html>');
            popup.document.close();
        }

        function htmlEncode(text) {
            return String(text || '').replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
        }

        window.onload = drawTrendChart;
    </script>
</body>
</html>
