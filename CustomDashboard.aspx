<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CustomDashboard.aspx.vb" Inherits="CustomDashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Custom Analytics Dashboard</title>
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
            text-align: center;
        }
        .dashboardSubTitle {
            display: block;
            color: #666666;
            font-size: 12px;
            text-align: center;
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
        .tileGrid .analyticsTile:nth-child(1) { background-color: #f7fbff; }
        .tileGrid .analyticsTile:nth-child(2) { background-color: #f8fff7; }
        .tileGrid .analyticsTile:nth-child(3) { background-color: #fffaf2; }
        .tileGrid .analyticsTile:nth-child(4) { background-color: #fbf8ff; }
        .tileGrid .analyticsTile:nth-child(5) { background-color: #f6fcfb; }
        .tileGrid .analyticsTile:nth-child(6) { background-color: #fff7f8; }
        .tileGrid .analyticsTile:nth-child(7) { background-color: #f9fbf1; }
        .tileGrid .analyticsTile:nth-child(8) { background-color: #f4faff; }
        .tileGrid .analyticsTile:nth-child(9) { background-color: #fff8f3; }
        .tileGrid .analyticsTile:nth-child(10) { background-color: #f5fbf7; }
        .tileGrid .analyticsTile:nth-child(11) { background-color: #f9f7ff; }
        .tileGrid .analyticsTile:nth-child(12) { background-color: #f7fcff; }
        .tileCaption {
            display: block;
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
        }
        .tileBody {
            display: block;
            padding: 4px 10px 3px 10px;
            color: #333333;
            font-size: 12px;
            line-height: 17px;
        }
        .previewBox {
            display: block;
            margin: 5px 10px 2px 10px;
            border: 1px solid #cfcfcf;
            background-color: rgba(255, 255, 255, 0.82);
            max-height: 136px;
            overflow: hidden;
            color: #333333;
            font-size: 10px;
            line-height: 15px;
            padding: 5px;
            word-break: break-word;
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
        .openText {
            display: block;
            margin: 5px 10px 10px 10px;
            color: #0066FF;
            font-size: 12px;
            font-weight: bold;
        }
    </style>
    <script type="text/javascript">
        function customDashboardTilePageSize() {
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

        function setCustomDashboardTilePageSize() {
            var hidden = document.getElementById("<%= HiddenDashboardPageSize.ClientID %>");
            if (!hidden) {
                return;
            }

            var size = customDashboardTilePageSize();
            hidden.value = size;

            var url = new URL(window.location.href);
            var currentSize = parseInt(url.searchParams.get("ps") || "0", 10);
            if (currentSize !== size) {
                url.searchParams.set("ps", size);
                url.searchParams.set("page", "1");
                window.location.replace(url.toString());
            }
        }

        var customDashboardResizeTimer = null;
        function queueCustomDashboardTilePageSize() {
            window.clearTimeout(customDashboardResizeTimer);
            customDashboardResizeTimer = window.setTimeout(setCustomDashboardTilePageSize, 400);
        }

        if (window.Sys && Sys.Application) {
            Sys.Application.add_load(setCustomDashboardTilePageSize);
        } else if (window.addEventListener) {
            window.addEventListener("load", setCustomDashboardTilePageSize);
        }

        if (window.addEventListener) {
            window.addEventListener("resize", queueCustomDashboardTilePageSize);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <asp:HiddenField ID="HiddenDashboardPageSize" runat="server" />
        <asp:UpdatePanel ID="udpCustomDashboard" runat="server">
            <ContentTemplate>
                <table>
                    <tr>
                        <td colspan="3" style="font-size: x-large; font-style: normal; font-weight: bold; background-color: #e5e5e5; vertical-align: middle; text-align: left; height: 40px;">
                            <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="font-size: x-small; font-style: normal; font-weight: normal; background-color: #e5e5e5; vertical-align: top; text-align: left; width: 15%;">
                            <asp:TreeView ID="TreeView1" runat="server" Width="100%" NodeIndent="10" Font-Names="Times New Roman" EnableTheming="True" ImageSet="BulletedList">
                                <Nodes>
                                    <asp:TreeNode Text="&lt;b&gt;Log off&lt;/b&gt;" Value="Default.aspx" Expanded="True"></asp:TreeNode>
                                    <asp:TreeNode Text="&lt;b&gt;List of Reports&lt;/b&gt;" Value="ListOfReports.aspx" Expanded="True"></asp:TreeNode>
                                    <asp:TreeNode Text="&lt;b&gt;Data Readiness Scanner&lt;/b&gt;" Value="DataReadinessScanner.aspx" NavigateUrl="DataReadinessScanner.aspx" Expanded="True"></asp:TreeNode>
                                    <asp:TreeNode Text="Analytics Dashboard" Value="DataAdmin.aspx" NavigateUrl="DataAdmin.aspx" Expanded="False"></asp:TreeNode>
                                    <asp:TreeNode Text="Market Dashboard" Value="MarketAdmin.aspx" NavigateUrl="MarketAdmin.aspx" Expanded="False"></asp:TreeNode>
                                </Nodes>
                                <RootNodeStyle HorizontalPadding="2px" Font-Bold="True" Font-Underline="False" />
                                <NodeStyle CssClass="NodeStyle" />
                                <ParentNodeStyle Font-Bold="True" />
                            </asp:TreeView>
                        </td>
                        <td style="width: 5px"></td>
                        <td id="main" style="width: 85%; text-align: left; vertical-align: top">
                            <div style="text-align: center; width: 100%;">
                                <asp:HyperLink ID="HyperLinkListOfReports" runat="server" NavigateUrl="~/ListOfReports.aspx" CssClass="NodeStyle" Font-Names="Arial">List of Reports</asp:HyperLink>
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:HyperLink ID="HyperLinkListOfDashboards" runat="server" NavigateUrl="~/ListOfDashboards.aspx" CssClass="NodeStyle" Font-Names="Arial" ToolTip="Report subset of user dashboards">Dashboards</asp:HyperLink>
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx?hilt=Analytics%20Dashboard" Target="_blank" CssClass="NodeStyle" Font-Names="Arial">Help</asp:HyperLink>
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:HyperLink ID="HyperLinkLogOff" runat="server" NavigateUrl="~/Default.aspx" CssClass="NodeStyle" Font-Names="Arial">Log off</asp:HyperLink>

                                <div class="dashboard">
                                    <div class="dashboardHeader">
                                        <asp:Label ID="lblHeader" runat="server" CssClass="dashboardTitle" Text="Custom Analytics Dashboard"></asp:Label>
                                        <asp:Label ID="LabelDescription" runat="server" CssClass="dashboardSubTitle" Text="Saved analytical views from report pages, opened with their selected fields and options."></asp:Label>
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
                                        <asp:Literal ID="LiteralTiles" runat="server"></asp:Literal>
                                    </div>
                                    <asp:Label ID="LabelMessage" runat="server" Font-Size="Larger" ForeColor="Red" Font-Names="Arial"></asp:Label>
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpCustomDashboard">
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
