<%@ Page Language="VB" AutoEventWireup="false" CodeFile="OpenTrends.aspx.vb" Inherits="OpenTrends" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Open Trends</title>
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
        .aiLinkButton {
            display: inline-block;
            width: 90px;
            height: 25px;
            line-height: 17px;
            padding: 3px;
            margin: 5px;
            border-style: solid;
            border-color: #4e4747;
            border-width: 1px;
            border-radius: 5px;
            background-image: linear-gradient(to bottom, rgba(211, 211, 211,0),rgba(211, 211, 250,3));
            color: #0066FF;
            font-size: 12px;
            font-weight: bold;
            text-align: center;
            text-decoration: none;
            box-sizing: border-box;
            vertical-align: middle;
        }
        .aiLinkButton:hover {
            color: #004DCC;
            text-decoration: none;
        }
        .ticketbutton {
            width: 90px;
            height: 25px;
            font-size: 12px;
            border-radius: 5px;
            border-style: solid;
            border-color: #4e4747;
            color: black;
            border-width: 1px;
            background-image: linear-gradient(to bottom, rgba(211, 211, 211,0),rgba(211, 211, 250,3));
            padding: 3px;
            margin: 5px;
            z-index: 9999;
        }
        .trendgrid {
            font-family: Arial;
            font-size: 12px;
            border-collapse: collapse;
            background-color: white;
        }
        .trendgrid th {
            background-color: #663300;
            color: white;
            border: 1px solid white;
            padding: 4px;
            white-space: nowrap;
        }
        .trendgrid td {
            border: 1px solid #d0d0d0;
            padding: 4px;
            white-space: nowrap;
        }
        .controlpanel {
            background-color: #e5e5e5;
            border: medium double #FFFFFF;
            color: black;
            font-family: Arial;
            font-size: small;
            width: auto;
            min-width: 760px;
            max-width: 980px;
            margin-left: 0;
            margin-right: auto;
        }
        .analysisSubtitle {
            display: block;
            font-family: Arial;
            font-size: small;
            color: #333333;
            padding-top: 4px;
            padding-bottom: 8px;
        }
        .analysisExplanation {
            font-family: Arial;
            font-size: small;
            color: #333333;
            background-color: #f5fbf4;
            border: 1px solid #d8ead4;
            padding: 8px;
            margin-top: 8px;
            max-width: 1180px;
        }
        .analysisExplanation span {
            display: block;
            padding-bottom: 4px;
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
    </style>
    <script type="text/javascript">
        function showWaitingPanel() {
            var p = document.getElementById('waitingPanel');
            if (p) { p.style.display = 'block'; }
            return true;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <asp:UpdatePanel ID="udpOpenTrends" runat="server">
            <ContentTemplate>
                <table style="width: 100%;">
                    <tr>
                        <td style="font-size: x-large; font-weight: bold; background-color: #e5e5e5; vertical-align: middle; text-align: left; height: 40px;">
                            <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align: top; text-align: left;">
                            <div style="text-align: center;">
                                <asp:Label ID="lblHeader" runat="server" Font-Size="22px" Font-Names="Arial" ToolTip="Build trend equations from the current Google Chart data.">Open Trends from Chart</asp:Label>
                                <asp:Label ID="LabelAnalysisSubtitle" runat="server" CssClass="analysisSubtitle" Text="Create trend equations from the current chart data and open each result in Trends and Predictions."></asp:Label>
                            </div>
                            <br />
                            <asp:Panel ID="PanelControls" runat="server" CssClass="controlpanel">
                                <span title="X value used for the Predicted Y column and for the highlighted point in Trends.">Predict Y when X is:</span>
                                <asp:TextBox ID="txtPredictX" runat="server" Width="90px" ToolTip="Enter the X value for prediction. If blank, the latest X value from the chart data is used."></asp:TextBox>
                                &nbsp;&nbsp;
                                <span title="Equation family used for fitting the chart data. Best Fit tests several models and keeps the one with the strongest R squared.">Equation Type:</span>
                                <asp:DropDownList ID="DropDownEquationType" runat="server" Width="130px" ToolTip="Choose Best Fit or a specific equation type for the trend rows.">
                                    <asp:ListItem Value="BestFit" Selected="True">Best Fit</asp:ListItem>
                                    <asp:ListItem Value="Linear">Linear</asp:ListItem>
                                    <asp:ListItem Value="Quadratic">Quadratic</asp:ListItem>
                                    <asp:ListItem Value="Cubic">Cubic</asp:ListItem>
                                    <asp:ListItem Value="Exponential">Exponential</asp:ListItem>
                                    <asp:ListItem Value="Logarithmic">Logarithmic</asp:ListItem>
                                    <asp:ListItem Value="Power">Power</asp:ListItem>
                                </asp:DropDownList>
                                &nbsp;&nbsp;
                                <asp:Button ID="ButtonBuild" runat="server" CssClass="ticketbutton" Text="Build" ToolTip="Build trend rows from the current chart data." />
                                <asp:Button ID="ButtonBack" runat="server" CssClass="ticketbutton" Text="Back" ToolTip="Return to the chart page." />
                                &nbsp;&nbsp;
                                <asp:LinkButton OnClientClick="return showWaitingPanel();" ID="lnkOpenTrendsAI" runat="server" CssClass="aiLinkButton" Font-Names="Arial" ToolTip="Ask AI to interpret trend equations created from the current chart.">AI</asp:LinkButton>
                            </asp:Panel>
                            <asp:Label ID="LabelError" runat="server" Font-Names="Arial" Font-Size="Small" ForeColor="Red"></asp:Label>
                            <br />
                            <asp:Label ID="LabelInfo" runat="server" Font-Names="Arial" Font-Size="Small" ForeColor="#333333"></asp:Label>
                            <br /><br />
                            <div style="font-family:Arial; font-size:small; font-weight:bold; color:#333333; padding-bottom:6px;">
                                <asp:Label ID="LabelChartTitle" runat="server"></asp:Label>
                                <br />
                                <asp:Label ID="LabelChartType" runat="server"></asp:Label>
                            </div>
                            <div style="font-family:Arial; font-size:small; padding-bottom:6px;">
                                <asp:LinkButton ID="LinkButtonPrevious" runat="server" Font-Size="Small" OnClick="LinkButtonPrevious_Click">Previous</asp:LinkButton>&nbsp;&nbsp;
                                <asp:Label ID="LabelPageNumberCaption" runat="server" Font-Names="Arial" Font-Size="Small" Text="Page Number"></asp:Label>
                                <asp:TextBox ID="TextBoxPageNumber" runat="server" Width="35px" Font-Names="Arial" Font-Size="Small" AutoPostBack="True" OnTextChanged="TextBoxPageNumber_TextChanged"></asp:TextBox>
                                <asp:Label ID="LabelPageCount" runat="server" Font-Names="Arial" Font-Size="Small"></asp:Label>&nbsp;&nbsp;
                                <asp:LinkButton ID="LinkButtonNext" runat="server" Font-Size="Small" OnClick="LinkButtonNext_Click">Next</asp:LinkButton>
                            </div>
                            <div style="overflow: auto; max-width: 100%;">
                                <asp:GridView ID="GridViewOpenTrends" runat="server" CssClass="trendgrid" AutoGenerateColumns="True" GridLines="Both"></asp:GridView>
                            </div>
                            <div class="analysisExplanation">
                                <asp:Label ID="LabelModelExplanation" runat="server"></asp:Label>
                                <asp:Label ID="LabelAlgorithmExplanation" runat="server"></asp:Label>
                                <asp:Label ID="LabelOutputExplanation" runat="server"></asp:Label>
                            </div>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="lnkOpenTrendsAI" />
            </Triggers>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpOpenTrends">
            <ProgressTemplate>
                <div class="modal">
                    <div class="center">
                        <asp:Image ID="imgProgress" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />
                        Please Wait...
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <div id="waitingPanel" class="modal" style="display:none;">
            <div class="center">
                <img src="Controls/Images/WaitImage2.gif" alt="Please Wait" />
                Please Wait...
            </div>
        </div>
    </form>
</body>
</html>
