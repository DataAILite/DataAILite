<%@ Page Language="VB" AutoEventWireup="false" CodeFile="UnitRegistration.aspx.vb" Inherits="UnitRegistration" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>DataAI - Company/Unit Registration</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha1/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha1/dist/js/bootstrap.bundle.min.js"></script>
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link href="https://fonts.googleapis.com/css2?family=DM+Sans:ital,opsz,wght@0,9..40,300;0,9..40,500;0,9..40,700;1,9..40,400&family=Playfair+Display:wght@600;700;800&display=swap" rel="stylesheet">

    <style>
        :root {
            --bg-primary: #FAFAF8;
            --bg-secondary: #F0EFEB;
            --bg-card: #FFFFFF;
            --accent: #1B6B4A;
            --accent-light: #E8F5EE;
            --accent-hover: #145236;
            --text-primary: #1A1A1A;
            --text-secondary: #5A5A5A;
            --text-muted: #8A8A8A;
            --border: #E2E0DB;
            --shadow-sm: 0 1px 3px rgba(0,0,0,0.04);
            --shadow-md: 0 4px 20px rgba(0,0,0,0.06);
            --shadow-lg: 0 12px 40px rgba(0,0,0,0.08);
            --radius: 14px;
            --radius-sm: 8px;
            --transition: 0.3s cubic-bezier(0.4, 0, 0.2, 1);
            --red: #C62828;
        }
        .dark-theme {
            --bg-primary: #0F0F0F;
            --bg-secondary: #1A1A1A;
            --bg-card: #1E1E1E;
            --accent: #3DD68C;
            --accent-light: #162B20;
            --accent-hover: #5AEAA5;
            --text-primary: #F0F0F0;
            --text-secondary: #A0A0A0;
            --text-muted: #666;
            --border: #2A2A2A;
            --shadow-sm: 0 1px 3px rgba(0,0,0,0.2);
            --shadow-md: 0 4px 20px rgba(0,0,0,0.3);
            --shadow-lg: 0 12px 40px rgba(0,0,0,0.4);
            --red: #EF5350;
        }
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body {
            font-family: 'DM Sans', sans-serif;
            background-color: var(--bg-primary);
            color: var(--text-primary);
            overflow-x: hidden;
            transition: background-color var(--transition), color var(--transition);
        }

        /* ── Navbar ── */
        .top-nav { position: sticky; top: 0; z-index: 1000; background: var(--bg-primary); backdrop-filter: blur(12px); transition: background var(--transition); }
        .dark-theme .top-nav { background: rgba(15,15,15,0.92); }
        .nav-inner { max-width: 1320px; margin: 0 auto; padding: 0.4rem 2rem; display: flex; align-items: center; flex-wrap: wrap; gap: 0.15rem 0; }
        .nav-brand { font-family: 'Playfair Display', serif; font-weight: 800; font-size: 1.45rem; color: var(--accent); text-decoration: none; letter-spacing: -0.5px; flex-shrink: 0; }
        .nav-links { display: flex; align-items: center; gap: 0.15rem; margin-left: 2.5rem; list-style: none; flex-wrap: wrap; }
        .nav-links a, .nav-links .nav-dropdown > a { text-decoration: none; color: var(--accent); font-size: 0.88rem; font-weight: 700; font-style: italic; padding: 0.45rem 0.85rem; border-radius: var(--radius-sm); transition: all var(--transition); white-space: nowrap; }
        .nav-links a:hover, .nav-links .nav-dropdown > a:hover { color: var(--accent-hover); background: var(--accent-light); }
        .nav-dropdown { position: relative; }
        .nav-dropdown .dd-menu { display: none; position: absolute; top: calc(100% + 6px); left: 0; min-width: 240px; background: var(--bg-card); border: 1px solid var(--border); border-radius: var(--radius); box-shadow: var(--shadow-lg); padding: 0.5rem; z-index: 100; }
        .nav-dropdown:hover .dd-menu, .nav-dropdown.open .dd-menu { display: block; }
        .dd-menu a { display: block; padding: 0.55rem 0.85rem; font-size: 0.84rem; color: var(--text-secondary); border-radius: var(--radius-sm); }
        .dd-menu a:hover { background: var(--accent-light); color: var(--accent); }
        .theme-toggle { margin-left: auto; flex-shrink: 0; width: 42px; height: 24px; background: var(--border); border-radius: 999px; position: relative; cursor: pointer; border: none; transition: background var(--transition); }
        .theme-toggle::after { content: ''; width: 18px; height: 18px; background: var(--bg-card); border-radius: 50%; position: absolute; top: 3px; left: 3px; transition: transform var(--transition); box-shadow: 0 1px 3px rgba(0,0,0,0.15); }
        .dark-theme .theme-toggle { background: var(--accent); }
        .dark-theme .theme-toggle::after { transform: translateX(18px); }

        /* ── Page Header ── */
        .page-header { max-width: 1320px; margin: 0 auto; padding: 1rem 2rem 0; text-align: center; }
        .page-header h1 { font-family: 'Playfair Display', serif; font-size: clamp(1.5rem, 3vw, 2rem); font-weight: 700; color: var(--accent); margin-bottom: 0.3rem; }
        .page-header .subtitle { font-size: 0.92rem; color: var(--text-secondary); margin-bottom: 0.2rem; }
        .page-header .subtitle-sm { font-size: 0.85rem; color: var(--text-muted); margin-bottom: 0.4rem; }
        .page-header-links { margin: 0.4rem 0; font-size: 0.85rem; }
        .page-header-links a { color: var(--accent); font-weight: 600; text-decoration: none; margin: 0 0.75rem; }
        .page-header-links a:hover { color: var(--accent-hover); }
        .seal-wrap { margin: 0.25rem 0; }
        .page-desc { font-size: 0.85rem; color: var(--text-muted); font-style: italic; margin-bottom: 0.25rem; }

        /* ── Registration Card ── */
        .reg-card { max-width: 1280px; margin: 0.75rem auto 1.5rem; background: var(--bg-card); border: 1px solid var(--border); border-radius: var(--radius); padding: 1.25rem 2rem 1rem; box-shadow: var(--shadow-md); }
        .reg-card .error-msg { text-align: center; margin-bottom: 0.5rem; }

        /* Form rows */
        .form-row { display: flex; align-items: center; gap: 0.75rem; margin-bottom: 0.6rem; }
        .form-row .label-col { width: 300px; flex-shrink: 0; text-align: right; font-size: 0.82rem; font-weight: 600; white-space: nowrap; }
        .form-row .label-col.required { color: var(--red); }
        .form-row .label-col.optional { color: var(--text-muted); }
        .form-row .input-col { flex: 1; font-size: 0.85rem; }

        /* Styled inputs */
        .form-row input[type="text"],
        .form-row input[type="password"],
        .form-row textarea {
            width: 100%; padding: 0.4rem 0.7rem;
            font-family: 'DM Sans', sans-serif; font-size: 0.85rem;
            background: var(--bg-primary); color: var(--text-primary);
            border: 1px solid var(--border); border-radius: var(--radius-sm);
            transition: border-color var(--transition), box-shadow var(--transition);
            outline: none;
        }
        .form-row input:focus, .form-row textarea:focus { border-color: var(--accent); box-shadow: 0 0 0 3px var(--accent-light); }

        .reg-card .asp-input {
            width: 100%; padding: 0.4rem 0.7rem;
            font-family: 'DM Sans', sans-serif; font-size: 0.85rem;
            background: var(--bg-primary); color: var(--text-primary);
            border: 1px solid var(--border) !important; border-radius: var(--radius-sm);
            transition: border-color var(--transition), box-shadow var(--transition); outline: none;
        }
        .reg-card .asp-input:focus { border-color: var(--accent) !important; box-shadow: 0 0 0 3px var(--accent-light); }
        .reg-card .asp-input-sm { width: 200px; }

        .reg-card select { -webkit-appearance: menulist; -moz-appearance: menulist; appearance: menulist; overflow: visible; }

        .form-check-row { display: flex; align-items: flex-start; gap: 0.5rem; margin-bottom: 0.6rem; padding-left: 300px; font-size: 0.82rem; }
        .form-hint { font-size: 0.75rem; color: var(--text-muted); margin-top: 0.15rem; }
        .inline-fields { display: flex; flex-wrap: nowrap; gap: 0.4rem; align-items: center; font-size: 0.82rem; }
        .inline-fields label { color: var(--text-muted); font-weight: 500; white-space: nowrap; }

        /* Buttons */
        .reg-submit { text-align: center; margin-top: 1rem; display: flex; justify-content: center; gap: 0.75rem; flex-wrap: wrap; align-items: center; }
        .reg-card .asp-btn {
            padding: 0.6rem 2rem; font-family: 'DM Sans', sans-serif; font-size: 0.92rem; font-weight: 600;
            background: var(--accent); color: #fff; border: none; border-radius: var(--radius); cursor: pointer;
            transition: all var(--transition); box-shadow: 0 4px 14px rgba(27,107,74,0.2);
        }
        .reg-card .asp-btn:hover { background: var(--accent-hover); transform: translateY(-2px); box-shadow: 0 6px 20px rgba(27,107,74,0.3); }
        .reg-card .asp-btn-secondary {
            padding: 0.5rem 1.5rem; font-family: 'DM Sans', sans-serif; font-size: 0.85rem; font-weight: 600;
            background: var(--bg-secondary); color: var(--text-primary); border: 1px solid var(--border); border-radius: var(--radius-sm); cursor: pointer;
            transition: all var(--transition);
        }
        .reg-card .asp-btn-secondary:hover { border-color: var(--accent); color: var(--accent); }
        .dark-theme .asp-btn { color: #0F0F0F; }

        .bottom-links { text-align: center; margin-top: 0.75rem; font-size: 0.82rem; display: flex; justify-content: center; align-items: center; gap: 0.75rem; flex-wrap: wrap; }
        .bottom-links a { color: var(--accent); text-decoration: none; margin: 0 0.5rem; }
        .bottom-links a:hover { color: var(--accent-hover); }
        .bottom-info { text-align: center; margin-top: 0.5rem; font-size: 0.8rem; color: var(--text-muted); }

        /* ── Loading Modal ── */
        .modal-overlay { position: fixed; z-index: 9999; inset: 0; background-color: rgba(0,0,0,0.4); backdrop-filter: blur(4px); }
        .modal-center { position: absolute; top: 50%; left: 50%; transform: translate(-50%,-50%); background: var(--bg-card); border-radius: var(--radius); padding: 2rem 2.5rem; text-align: center; box-shadow: var(--shadow-lg); font-weight: 600; color: var(--text-primary); }

        /* ── Responsive ── */
        @media (max-width: 992px) {
            .nav-inner { padding: 0.4rem 1.5rem; }
            .nav-links { margin-left: 0; flex-basis: 100%; gap: 0.1rem; padding-top: 0.4rem; }
        }
        @media (max-width: 768px) {
            .form-row { flex-direction: column; align-items: flex-start; gap: 0.3rem; }
            .form-row .label-col { width: auto; text-align: left; }
            .form-check-row { padding-left: 0; }
            .reg-card { padding: 1rem 1rem; margin: 0.75rem 0.5rem 1.5rem; }
            .inline-fields { flex-direction: column; align-items: flex-start; }
        }
        @media (max-width: 600px) {
            .nav-inner { padding: 0.4rem 0.75rem; }
            .nav-links a, .nav-links .nav-dropdown > a { font-size: 0.78rem; padding: 0.35rem 0.55rem; }
            .page-header { padding: 0.75rem 1rem 0; }
        }
    </style>
</head>
<body>

 <!-- ═══ NAVBAR ═══ -->
 <header class="top-nav">
     <div class="nav-inner">
         <a class="nav-brand" href="#">DataAI</a>
         <button class="theme-toggle" id="themeToggle" aria-label="Toggle dark mode" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Switch to dark mode"></button>

         <%--<span id="siteseal"><script async type="text/javascript" src="https://seal.godaddy.com/getSeal?sealID=SYhDKXg2IT7QXHzPEW7z7tmavANkr8vMDCiRmZvbKczmKBJ5Wj8eKl1EX00B"></script></span>--%>

         <ul class="nav-links" id="navLinks">
             <li class="nav-dropdown">
                 <a href="#">About&ensp;&#9662;</a>
                 <div class="dd-menu">
                     <a href="https://oureports.net/OUReports/DataAIOverview.html" target="_blank">DataAI Overview</a>
                     <%--<a href="comparison.aspx" target="_blank">Comparison</a>--%>
                     <a href="AboutUs.aspx" target="_blank">About Us</a>
                 </div>
             </li>
             <li><a href="https://oureports.net/OUReports/Partners.pdf" target="_blank">Partners</a></li>
             <li class="nav-dropdown">
                 <a href="#">Products&ensp;&#9662;</a>
                 <div class="dd-menu">
                     <a href="https://oureports.net/OUReports/TestingSiteAIProposal.pdf" target="_blank">Testing Site</a>
                     <a href="Index3.aspx">Services</a>
                     <a href="IndexSoftware.aspx">Software</a>
                     <a href="https://oureports.net/HelpDesk/Default.aspx">Project Manager - Free</a>
                 </div>
             </li>
             <li class="nav-dropdown">
                 <a href="#">Customers&ensp;&#9662;</a>
                 <div class="dd-menu">
                     <a href="Registration.aspx">Individual</a>
                     <a href="UnitRegistration.aspx?org=company">Company</a>
                     <%--<a href="OUReportsAgents.aspx">Sales Agent</a>--%>
                 </div>
             </li>
             <li class="nav-dropdown">
                 <a href="#">Use Cases&ensp;&#9662;</a>
                 <div class="dd-menu">
                     <a href="https://oureports.net/OUReports/default.aspx?srd=30&dash=yes&lgn=d720202024346P906" target="_blank">Covid 2020 Dashboard</a>
                     <a href="https://oureports.net/OUReports/DashboardHelp.pdf" target="_blank">Health Care</a>
                     <a href="https://oureports.net/OUReports/UseCasePublic.aspx" target="_blank">Public Data</a>
                     <a href="https://oureports.net/OUReports/ExploreData.pdf" target="_blank">Explore Data</a>
                     <a href="https://oureports.net/OUReports/MapDefinitionDocumentation.pdf" target="_blank">Google Earth &amp; Map Generator</a>
                     <a href="https://oureports.net/OUReports/Tornadoes.pdf" target="_blank">Google Earth: Tornadoes</a>
                     <a href="https://oureports.net/OUReports/ExploreDataAndDataAnalytics.pdf" target="_blank">Explore Data &amp; Analytics</a>
                     <a href="https://oureports.net/OUReports/MatrixBalancingYouthTobaccoUsage.pdf" target="_blank">Matrix Balancing - Youth Tobacco</a>
                     <a href="https://oureports.net/OUReports/WorldHappinessScoresIn2019.pdf" target="_blank">World Happiness Scores 2019</a>
                 </div>
             </li>
             <li class="nav-dropdown">
                 <a href="#">Docs&ensp;&#9662;</a>
                 <div class="dd-menu">
                     <a href="https://oureports.net/OUReports/DataAIHelp.aspx" target="_blank">DataAI Help</a>
                     <a href="https://oureports.net/OUReports/OnlineUserReporting.pdf" target="_blank">General Documentation</a>
                     <a href="https://oureports.net/OUReports/AIandDataAI.pdf" target="_blank">AI and DataAI (analytical &amp; artificial intelligence)</a>
                     <a href="https://oureports.net/OUReports/DataImport.pdf" target="_blank">Data Import</a>
                     <a href="https://oureports.net/OUReports/AdvancedReportDesigner.pdf#page=4" target="_blank">Advanced Report Designer</a>
                     <a href="https://oureports.net/OUReports/GoogleChartsAndDashboards.pdf" target="_blank">Charts and Dashboards</a>
                     <a href="https://oureports.net/OUReports/MatrixBalancing.pdf" target="_blank">Matrix Balancing</a>
                     <a href="https://oureports.net/OUReports/MatrixBalancingSamples.pdf" target="_blank">More Matrix Balancing Samples</a>
                     <a href="https://oureports.net/OUReports/MapDefinitionDocumentation.pdf" target="_blank">Google Maps &amp; Earth Reports</a>
                 </div>
             </li>
             <li class="nav-dropdown">
                 <a href="#">Videos&ensp;&#9662;</a>
                 <div class="dd-menu">
                     <a href="https://oureports.net/OUReports/Videos/videoAIandDataAI.mp4" target="_blank">DataAI and AI for Data Analytics</a>
                     <a href="https://oureports.net/OUReports/Videos/videoDataAI.mp4" target="_blank">More DataAI and AI for Data Analytics</a>
                     <a href="https://oureports.net/OUReports/Videos/DataImport.mp4" target="_blank">Data Import, Analytics, Instant Reporting</a>
                     <a href="https://oureports.net/OUReports/Videos/zoom_2.mp4" target="_blank">Charts, Maps, and Dashboards</a>
                     <a href="https://oureports.net/OUReports/Videos/QuickStart.mp4" target="_blank">Quick Start (only email needed)</a>
                     <a href="https://oureports.net/OUReports/Videos/UserRegistrationVideo.mp4" target="_blank">Individual Registration, user database</a>
                     <a href="https://oureports.net/OUReports/Videos/RegOurDb.mp4" target="_blank">Individual Registration, use our database</a>
                     <a href="https://oureports.net/OUReports/Videos/UnitRegistrationVideo.mp4" target="_blank">Company Registration</a>
                     <a href="https://oureports.net/OUReports/Videos/AdvancedReportDesigner Tabular.mp4" target="_blank">Advanced Report Designer - Tabular</a>
                     <a href="https://oureports.net/OUReports/Videos/AdvancedReportDesigner-HeaderFooter.mp4" target="_blank">Advanced Report Designer - Header Footer</a>
                     <a href="https://oureports.net/OUReports/Videos/AdvancedReportDesigner-FreeForm.mp4" target="_blank">Advanced Report Designer - Free Form</a>
                     <a href="https://oureports.net/OUReports/Videos/InputFromAccess.mp4" target="_blank">Input from Access</a>
                     <a href="https://oureports.net/OUReports/Videos/MatrixBalance.mp4" target="_blank">Matrix Balancing</a>
                     <a href="https://oureports.net/OUReports/Videos/MatrixBalance1a1b.mp4" target="_blank">Matrix Balancing Scenarios 1a, 1b</a>
                     <a href="https://oureports.net/OUReports/Videos/MatrixBalance2a3a.mp4" target="_blank">Matrix Balancing Scenarios 2a, 3a</a>
                     <a href="https://oureports.net/OUReports/Videos/MatrixBalance2b2c.mp4" target="_blank">Matrix Balancing Scenarios 2b, 2c</a>
                     <a href="https://oureports.net/OUReports/Videos/MatrixBalance3b3c.mp4" target="_blank">Matrix Balancing Scenarios 3b, 3c</a>
                     <a href="https://oureports.net/OUReports/Videos/MatrixBalance4a4b4c.mp4" target="_blank">Matrix Balancing Scenarios 4a, 4b, 4c</a>
                 </div>
             </li>
             <li><a href="https://oureports.net/OUReports/DataAITraining.pdf" target="_blank">Self Training</a></li>
             <li><a href="ContactUs.aspx">Contact Us</a></li>
             <li><a href="https://oureports.net/OUReports/DataAIHelp.aspx" target="_blank">Guide</a></li>
             <li><a href="Default.aspx">Sign In</a></li>
         </ul>
     </div>
 </header>

<!-- ═══ PAGE CONTENT ═══ -->
<form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
    <asp:UpdatePanel ID="udpunitreg" runat="server">
        <ContentTemplate>

            <div class="page-header">
                <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting" Visible="false"></asp:Label>
                <h1>Company / Unit Registration</h1>
                <asp:Label ID="Label29" runat="server" Text="" Visible="false"></asp:Label>
                <p class="subtitle-sm">Multiple users - one database</p>
                <%-- Links moved to bottom of page --%>
                <asp:HyperLink ID="HyperLinkClient" runat="server" NavigateUrl="~/Partners.pdf" Target="_top" Visible="false">OUR Business Proposal</asp:HyperLink>
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/ContactUs.aspx" Target="_top" Visible="false">Contact Us</asp:HyperLink>
                <%-- Seal moved to bottom --%>
                <p class="page-desc"><asp:Label ID="Label31" runat="server" Text="Analyzing the data structure, doing data mining and designing variety of initial reports."></asp:Label></p>
                <%-- Hidden links for code-behind --%>
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/SiteAdmin.aspx?unitdb=yes" Enabled="False" Visible="False">Users</asp:HyperLink>
                <asp:HyperLink ID="HyperLinkHelp" runat="server" NavigateUrl="DataAIHelp.aspx" Target="_blank" Enabled="False" Visible="False">Help</asp:HyperLink>
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/index.aspx" Enabled="False" Visible="False">Back</asp:HyperLink>
            </div>

            <div class="reg-card">

                <!-- Terms & Conditions + Use own DB checkbox -->
                <div id="tr1" runat="server" class="form-check-row">
                    <asp:Label ID="Label30" runat="server" Text="Read*:" style="color:var(--red); font-weight:600; font-size:0.82rem; margin-right:0.3rem;"></asp:Label>
                    <asp:CheckBox ID="chkTermsAndCond" runat="server" Text=" " AutoPostBack="True" BorderColor="#FF3300" />
                    <asp:HyperLink ID="HyperLinkTermsAndCond" runat="server" NavigateUrl="https://oureports.net/OUReports/TermsAndConditions.pdf" Target="_blank" Font-Size="Small">I have read and agreed to Terms and Conditions</asp:HyperLink>
                    &nbsp;&nbsp;
                    <asp:CheckBox ID="chkOURdb" runat="server" Text="Use a database at Unit own data server as Unit DataAI Setting database" AutoPostBack="True" BorderColor="#FF3300" ToolTip="If not checked the OUReports will create the MySql database on the OUReports data server for the Unit's report definitions." Font-Size="Smaller" />
                </div>

                <!-- Unit Name -->
                <div id="trText3" runat="server" class="form-row">
                    <div class="label-col required"><asp:Label ID="lblText3" runat="server" Text="Unit name*:"></asp:Label></div>
                    <div class="input-col"><asp:TextBox ID="txtUnit" runat="server" CssClass="asp-input"></asp:TextBox></div>
                </div>

                <!-- Unit Contact Info -->
                <div id="tr12" runat="server" class="form-row">
                    <div class="label-col optional"><asp:Label ID="Label23" runat="server" Text="Unit contact info:"></asp:Label></div>
                    <div class="input-col">
                        <div class="inline-fields">
                            <label><asp:Label ID="Label25" runat="server" Text="phone:"></asp:Label></label>
                            <asp:TextBox ID="txtUnitPhone" runat="server" CssClass="asp-input" style="width:140px;"></asp:TextBox>
                            <label><asp:Label ID="Label26" runat="server" Text="email:"></asp:Label></label>
                            <asp:TextBox ID="txtUnitEmail" runat="server" CssClass="asp-input" style="width:200px;"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <!-- Unit Official -->
                <div id="tr14" runat="server" class="form-row">
                    <div class="label-col optional"><asp:Label ID="Label27" runat="server" Text="Unit official making decisions title and name:"></asp:Label></div>
                    <div class="input-col">
                        <asp:TextBox ID="txtUnitBossName" runat="server" CssClass="asp-input"></asp:TextBox>
                        <asp:Label ID="Label28" runat="server" Visible="false" Text="title and name:"></asp:Label>
                    </div>
                </div>

                <!-- Unit Address -->
                <div id="tr13" runat="server" class="form-row">
                    <div class="label-col optional"><asp:Label ID="Label24" runat="server" Text="Unit address:"></asp:Label></div>
                    <div class="input-col"><asp:TextBox ID="txtUnitAddress" runat="server" CssClass="asp-input"></asp:TextBox></div>
                </div>

                <!-- DataAI Setting DB Provider -->
                <div id="trOURprv" runat="server" class="form-row">
                    <div class="label-col required"><asp:Label ID="Label2" runat="server" Text="Unit DataAI Setting DB Provider*:" ToolTip="Unit Database for Unit DataAI Setting"></asp:Label></div>
                    <div class="input-col">
                        <asp:DropDownList runat="server" Font-Size="Smaller" ID="ddOURConnPrv" AutoPostBack="True" Width="250px">
                            <asp:ListItem Value="System.Data.SqlClient">SQL Server</asp:ListItem>
                            <asp:ListItem Value="Npgsql">PostgreSQL</asp:ListItem>
                            <asp:ListItem Value="MySql.Data.MySqlClient">MySQL</asp:ListItem>
                            <asp:ListItem Value="Oracle.ManagedDataAccess.Client">Oracle</asp:ListItem>
                            <asp:ListItem Value="InterSystems.Data.CacheClient">Intersystems Cache</asp:ListItem>
                            <asp:ListItem Value="InterSystems.Data.IRISClient">Intersystems IRIS</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <!-- DataAI Setting DB Connection String -->
                <div id="trOURdb" runat="server" class="form-row">
                    <div class="label-col required"><asp:Label ID="Label1" runat="server" Text="Unit DataAI Setting DB Connection String*:" ToolTip="Unit Database for Unit DataAI Setting"></asp:Label></div>
                    <div class="input-col"><asp:TextBox ID="txtOURdb" runat="server" CssClass="asp-input" ToolTip="Unit Database for Unit DataAI Setting with writing access for User ID"></asp:TextBox></div>
                </div>

                <!-- DataAI Setting DB Password -->
                <div id="trOURdbPass" runat="server" class="form-row">
                    <div class="label-col required"><asp:Label ID="Label32" runat="server" Text="Unit DataAI Setting DB Password*:" ToolTip="Unit Database password for Unit DataAI Setting"></asp:Label></div>
                    <div class="input-col"><asp:TextBox ID="txtOURdbPass" runat="server" CssClass="asp-input" style="width:250px;" ToolTip="Unit Database password for Unit DataAI Setting with writing access for User ID" TextMode="Password"></asp:TextBox></div>
                </div>

                <!-- Data DB Provider -->
                <div id="tr6" runat="server" class="form-row">
                    <div class="label-col required"><asp:Label ID="Label6" runat="server" Text="Unit Data DB Connection Provider*:" ToolTip="Unit Database with Unit Data"></asp:Label></div>
                    <div class="input-col">
                        <asp:DropDownList runat="server" Font-Size="Smaller" ID="ddUserConnPrv" AutoPostBack="True" Width="250px">
                            <asp:ListItem Value="System.Data.SqlClient">SQL Server</asp:ListItem>
                            <asp:ListItem Value="Npgsql">PostgreSQL</asp:ListItem>
                            <asp:ListItem Value="MySql.Data.MySqlClient">MySQL</asp:ListItem>
                            <asp:ListItem Value="Oracle.ManagedDataAccess.Client">Oracle</asp:ListItem>
                            <asp:ListItem Value="InterSystems.Data.CacheClient">Intersystems Cache</asp:ListItem>
                            <asp:ListItem Value="InterSystems.Data.IRISClient">Intersystems IRIS</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <!-- Data DB Connection String + Password -->
                <div id="tr5" runat="server" class="form-row">
                    <div class="label-col required"><asp:Label ID="Label5" runat="server" Text="Unit Data DB Connection String*:" ToolTip="Unit Database with Unit Data"></asp:Label></div>
                    <div class="input-col">
                        <asp:TextBox ID="txtUserConnStr" runat="server" CssClass="asp-input" ToolTip="Unit Database for Unit Data with reading access for User ID"></asp:TextBox>
                        <div class="form-hint" style="margin-top:0.4rem;">
                            <asp:Label ID="LabelDBpassword" runat="server" Text="Database password*:" style="color:var(--red); font-size:0.82rem;"></asp:Label>
                            <asp:TextBox ID="txtDBpass" runat="server" CssClass="asp-input asp-input-sm" TextMode="Password" style="display:inline-block; width:150px; margin-left:0.3rem;"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <!-- Start Date & End Date -->
                <div id="tr4" runat="server" class="form-row">
                    <div class="label-col optional"><asp:Label ID="Label4" runat="server" Text="Start Date:"></asp:Label></div>
                    <div class="input-col">
                        <div class="inline-fields">
                            <div id="divDate" class="inline" runat="server" style="width:160px;">
                                <asp:TextBox id="Date1" runat="server" CssClass="asp-input" Width="100%"></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="ceDate1" runat="server" TargetControlID="Date1" Format="M/d/yyyy" TodaysDateFormat="M/d/yyyy" />
                            </div>
                            <span id="tr8" runat="server" style="display:inline-flex; align-items:center; gap:0.4rem;">
                                <label style="font-weight:600; font-size:0.82rem; color:var(--text-muted);"><asp:Label ID="Label8" runat="server" Text="End Date:"></asp:Label></label>
                                <div id="divDate2" class="inline" runat="server" style="width:160px;">
                                    <asp:TextBox id="Date2" runat="server" CssClass="asp-input" Width="100%"></asp:TextBox>
                                    <ajaxToolkit:CalendarExtender ID="ceDate2" runat="server" TargetControlID="Date2" Format="M/d/yyyy" TodaysDateFormat="M/d/yyyy" />
                                </div>
                            </span>
                        </div>
                    </div>
                </div>

                <!-- OUReports Agent -->
                <div id="tr9" runat="server" class="form-row">
                    <div class="label-col optional"><asp:Label ID="Label11" runat="server" Text="Did OUReports Agent contact you?"></asp:Label></div>
                    <div class="input-col">
                        <div class="inline-fields">
                            <asp:CheckBox ID="chkOURAgent" runat="server" AutoPostBack="True" Checked="True" Text="yes" /> /no&nbsp;&nbsp;
                            <label><asp:Label ID="Label13" runat="server" Text="agent name:"></asp:Label></label>
                            <asp:TextBox id="txtAgentName" runat="server" CssClass="asp-input" style="width:150px;"></asp:TextBox>
                            <label><asp:Label ID="Label14" runat="server" Text="phone:"></asp:Label></label>
                            <asp:TextBox id="txtAgentPhone" runat="server" CssClass="asp-input" style="width:110px;"></asp:TextBox>
                            <label><asp:Label ID="Label15" runat="server" Text="email:"></asp:Label></label>
                            <asp:TextBox id="txtAgentEmail" runat="server" CssClass="asp-input" style="width:180px;"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <!-- Agent help -->
                <div id="tr10" runat="server" class="form-row">
                    <div class="label-col optional"><asp:Label ID="Label12" runat="server" Text="How did OUReports Agent help you?:"></asp:Label></div>
                    <div class="input-col">
                        <asp:DropDownList ID="ddAgentHelps" runat="server" Font-Size="Smaller" Width="350px">
                            <asp:ListItem Value="web">provided the OUReports.com web address to you</asp:ListItem>
                            <asp:ListItem Value="demo">+supported you through Demo</asp:ListItem>
                            <asp:ListItem Value="sign">+supported you signing the agreement</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <!-- Unit OUReports URL -->
                <div id="trUnitWeb" runat="server" class="form-row">
                    <div class="label-col optional"><asp:Label ID="lblText2" runat="server" Text="Unit OUReports URL:"></asp:Label></div>
                    <div class="input-col"><asp:TextBox ID="txtUnitWeb" runat="server" CssClass="asp-input"></asp:TextBox></div>
                </div>

                <!-- Distribution Model -->
                <div id="trModels" runat="server" class="form-row">
                    <div class="label-col optional"><asp:Label ID="Label3" runat="server" Text="Distribution Model:"></asp:Label></div>
                    <div class="input-col">
                        <asp:DropDownList ID="ddModels" runat="server" Font-Size="Smaller" Width="400px">
                            <asp:ListItem Value="UnitOWeb-OURdb">Unit OUReports and OURdb on OUR server (Direct customer)</asp:ListItem>
                            <asp:ListItem Value="UnitOWeb-UnitOdb">Unit OUReports on OUR server, OURdb on Unit data server</asp:ListItem>
                            <asp:ListItem Value="UnitWeb-UnitOdb">Unit OUReports on UNIT server (Vendor)</asp:ListItem>
                            <asp:ListItem Value="OURweb-OURdb">OURweb-OURdb (individual users)</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <!-- Site Admin Contact -->
                <div id="tr11" runat="server" class="form-row">
                    <div class="label-col required"><asp:Label ID="Label16" runat="server" Text="Unit site admin contact info*:"></asp:Label></div>
                    <div class="input-col">
                        <div class="inline-fields" style="width:100%;">
                            <label><asp:Label ID="Label17" runat="server" Text="name:"></asp:Label></label>
                            <asp:TextBox id="txtName" runat="server" CssClass="asp-input" style="flex:1; min-width:0;"></asp:TextBox>
                            <label><asp:Label ID="Label18" runat="server" Text="phone:"></asp:Label></label>
                            <asp:TextBox id="txtPhone" runat="server" CssClass="asp-input" style="flex:1; min-width:0;"></asp:TextBox>
                            <label><asp:Label ID="Label19" runat="server" Text="email:"></asp:Label></label>
                            <asp:TextBox id="txtEmail" runat="server" CssClass="asp-input" style="flex:1; min-width:0; border-color:var(--red) !important;"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <!-- Site Admin Logon -->
                <div id="trText1" runat="server" class="form-row">
                    <div class="label-col required"><asp:Label ID="lblText1" runat="server" Text="Unit site admin logon and password*:"></asp:Label></div>
                    <div class="input-col">
                        <div class="inline-fields" style="width:100%;">
                            <label><asp:Label ID="Label20" runat="server" Text="logon:"></asp:Label></label>
                            <asp:TextBox ID="txtLogon" runat="server" CssClass="asp-input" style="flex:1; min-width:0;" ToolTip="Logon"></asp:TextBox>
                            <label><asp:Label ID="Label21" runat="server" Text="password:"></asp:Label></label>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="asp-input" style="flex:1; min-width:0; border-color:var(--red) !important;" TextMode="Password" ToolTip="password"></asp:TextBox>
                            <label><asp:Label ID="Label22" runat="server" Text="repeat:"></asp:Label></label>
                            <asp:TextBox ID="txtRepeat" runat="server" CssClass="asp-input" style="flex:1; min-width:0; border-color:var(--red) !important;" TextMode="Password" ToolTip="Repeat password"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <!-- Comments -->
                <div id="tr7" runat="server" class="form-row">
                    <div class="label-col optional"><asp:Label ID="Label7" runat="server" Text="Comments:"></asp:Label></div>
                    <div class="input-col"><asp:TextBox ID="txtComments" runat="server" CssClass="asp-input"></asp:TextBox></div>
                </div>

                <ucMsgBox:Msgbox id="MessageBox" runat="server"></ucMsgBox:Msgbox>

                <!-- Buttons -->
                <div class="reg-submit">
                    <asp:Button ID="btnCancel" runat="server" Text="Back to Units" CssClass="asp-btn-secondary" Enabled="False" Visible="False" />
                    <asp:Button ID="btnSave" runat="server" Text="Submit" CssClass="asp-btn" Enabled="False" ToolTip="Register Company" />
                    <asp:Button ID="btnUpdate" runat="server" Text="Update to current version" CssClass="asp-btn-secondary" />
                </div>

                <div class="bottom-links">
                    <a href="Partners.pdf" target="_top">OUR Business Proposal</a>
                    <a href="ContactUs.aspx" target="_top">Contact Us</a>
                    <asp:HyperLink ID="HyperLinkUnitOURWeb" runat="server" NavigateUrl="DataAIHelp.aspx" Target="_blank">Unit OUReports</asp:HyperLink>
                    <asp:HyperLink ID="HyperLinkSeeProposal" runat="server" NavigateUrl="DataAIHelp.aspx" Target="_blank">See OUR Business Proposal</asp:HyperLink>
                </div>

                <%-- Hidden labels kept for code-behind --%>
                <asp:Label ID="Label9" runat="server" ForeColor="Gray" Visible="false"></asp:Label>
                <asp:Label ID="Label10" runat="server" Font-Italic="True" Font-Size="X-Small" Text="Unit index #" Visible="false"></asp:Label>

            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpunitreg">
        <ProgressTemplate>
            <div class="modal-overlay">
                <div class="modal-center">
                    <asp:Image ID="imgProgress" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />
                    <br />Please Wait...
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</form>

<div style="text-align:center; margin:-0.5rem 0 1rem;">
    <span id="siteseal"><script async type="text/javascript" src="https://seal.godaddy.com/getSeal?sealID=SYhDKXg2IT7QXHzPEW7z7tmavANkr8vMDCiRmZvbKczmKBJ5Wj8eKl1EX00B"></script></span>
</div>

<!-- ═══ SCRIPTS ═══ -->
<script>
    var themeBtn = document.getElementById('themeToggle');
    themeBtn.addEventListener('click', function () {
        document.body.classList.toggle('dark-theme');
        var isDark = document.body.classList.contains('dark-theme');
        var newTitle = isDark ? 'Switch to light mode' : 'Switch to dark mode';
        themeBtn.setAttribute('title', newTitle);
        themeBtn.setAttribute('aria-label', newTitle);
        var tip = bootstrap.Tooltip.getInstance(themeBtn);
        if (tip) tip.setContent({ '.tooltip-inner': newTitle });
    });

    document.querySelectorAll('.nav-dropdown > a').forEach(function (toggle) {
        toggle.addEventListener('click', function (e) {
            if ('ontouchstart' in window || window.innerWidth <= 992) {
                e.preventDefault();
                var parent = toggle.closest('.nav-dropdown');
                var wasOpen = parent.classList.contains('open');
                document.querySelectorAll('.nav-dropdown.open').forEach(function (dd) { dd.classList.remove('open'); });
                if (!wasOpen) parent.classList.add('open');
            }
        });
    });
    document.addEventListener('click', function (e) {
        if (!e.target.closest('.nav-dropdown')) {
            document.querySelectorAll('.nav-dropdown.open').forEach(function (dd) { dd.classList.remove('open'); });
        }
    });

    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (el) { return new bootstrap.Tooltip(el); });

    // Fix: re-enable all dropdown options after page load and UpdatePanel postbacks
    function fixFormControls() {
        document.querySelectorAll('.reg-card select').forEach(function(sel) {
            sel.disabled = false;
            for (var i = 0; i < sel.options.length; i++) {
                sel.options[i].disabled = false;
            }
        });
    }
    fixFormControls();
    if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(fixFormControls);
    }
</script>
</body>
</html>
