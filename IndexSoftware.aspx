<%@ Page Language="VB" AutoEventWireup="false" CodeFile="IndexSoftware.aspx.vb" Inherits="IndexSoftware" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>DataAI Software</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha1/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha1/dist/js/bootstrap.bundle.min.js"></script>
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link href="https://fonts.googleapis.com/css2?family=DM+Sans:ital,opsz,wght@0,9..40,300;0,9..40,500;0,9..40,700;1,9..40,400&family=Playfair+Display:wght@600;700;800&display=swap" rel="stylesheet">

    <!-- Google Analytics -->
    <script async src="https://www.googletagmanager.com/gtag/js?id=G-K4GE50T22P"></script>
    <script>
        window.dataLayer = window.dataLayer || [];
        function gtag() { dataLayer.push(arguments); }
        gtag("js", new Date());
        gtag("config", "G-K4GE50T22P");
    </script>

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
        .top-nav {
            position: sticky;
            top: 0;
            z-index: 1000;
            background: var(--bg-primary);
            backdrop-filter: blur(12px);
            -webkit-backdrop-filter: blur(12px);
            transition: background var(--transition), border var(--transition);
        }
        .dark-theme .top-nav { background: rgba(15,15,15,0.92); }

        .nav-inner {
            max-width: 1320px;
            margin: 0 auto;
            padding: 0.75rem 2rem;
            display: flex;
            align-items: center;
            flex-wrap: wrap;
            gap: 0.25rem 0;
        }

        .nav-brand {
            font-family: 'Playfair Display', serif;
            font-weight: 800;
            font-size: 1.45rem;
            color: var(--accent);
            text-decoration: none;
            letter-spacing: -0.5px;
            flex-shrink: 0;
        }

        .nav-links {
            display: flex;
            align-items: center;
            gap: 0.15rem;
            margin-left: 2.5rem;
            list-style: none;
            flex-wrap: wrap;
        }

        .nav-links a,
        .nav-links .nav-dropdown > a {
            text-decoration: none;
            color: var(--accent);
            font-size: 0.88rem;
            font-weight: 700;
            font-style: italic;
            padding: 0.45rem 0.85rem;
            border-radius: var(--radius-sm);
            transition: all var(--transition);
            white-space: nowrap;
        }
        .nav-links a:hover,
        .nav-links .nav-dropdown > a:hover {
            color: var(--accent-hover);
            background: var(--accent-light);
        }

        .nav-dropdown { position: relative; }
        .nav-dropdown .dd-menu {
            display: none;
            position: absolute;
            top: calc(100% + 6px);
            left: 0;
            min-width: 240px;
            background: var(--bg-card);
            border: 1px solid var(--border);
            border-radius: var(--radius);
            box-shadow: var(--shadow-lg);
            padding: 0.5rem;
            z-index: 100;
        }
        .nav-dropdown:hover .dd-menu,
        .nav-dropdown.open .dd-menu { display: block; }
        .dd-menu a {
            display: block;
            padding: 0.55rem 0.85rem;
            font-size: 0.84rem;
            color: var(--text-secondary);
            border-radius: var(--radius-sm);
        }
        .dd-menu a:hover {
            background: var(--accent-light);
            color: var(--accent);
        }

        /* Theme toggle */
        .theme-toggle {
            margin-left: auto;
            flex-shrink: 0;
            width: 42px; height: 24px;
            background: var(--border);
            border-radius: 999px;
            position: relative;
            cursor: pointer;
            border: none;
            transition: background var(--transition);
        }
        .theme-toggle::after {
            content: '';
            width: 18px; height: 18px;
            background: var(--bg-card);
            border-radius: 50%;
            position: absolute;
            top: 3px; left: 3px;
            transition: transform var(--transition);
            box-shadow: 0 1px 3px rgba(0,0,0,0.15);
        }
        .dark-theme .theme-toggle { background: var(--accent); }
        .dark-theme .theme-toggle::after { transform: translateX(18px); }

        .mobile-toggle { display: none !important; }

        /* ── Hero ── */
        .hero {
            max-width: 1320px;
            margin: 0 auto;
            padding: 4rem 2rem 2rem;
            text-align: center;
        }
        .hero h1 {
            font-family: 'Playfair Display', serif;
            font-size: clamp(2rem, 4.5vw, 3.2rem);
            font-weight: 700;
            line-height: 1.15;
            letter-spacing: -1.5px;
            color: var(--text-primary);
            margin-bottom: 1.5rem;
        }
        .hero h1 em {
            font-style: italic;
            color: var(--accent);
        }
        .hero-desc {
            font-size: 1.05rem;
            line-height: 1.75;
            color: var(--text-secondary);
            max-width: 720px;
            margin: 0 auto 2.5rem;
        }

        /* ── Proposal Link ── */
        .proposal-section {
            max-width: 1320px;
            margin: 0 auto;
            padding: 0 2rem 3rem;
            text-align: center;
        }
        .proposal-link {
            display: inline-flex;
            align-items: center;
            gap: 0.5rem;
            background: var(--accent);
            color: #fff;
            padding: 0.85rem 1.8rem;
            border-radius: var(--radius);
            text-decoration: none;
            font-weight: 600;
            font-size: 0.95rem;
            transition: all var(--transition);
            border: none;
        }
        .proposal-link:hover {
            background: var(--accent-hover);
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(27,107,74,0.25);
            color: #fff;
        }
        .dark-theme .proposal-link { color: #0F0F0F; }
        .dark-theme .proposal-link:hover { color: #0F0F0F; box-shadow: 0 6px 20px rgba(61,214,140,0.25); }

        /* ── Download Cards ── */
        .download-section {
            max-width: 1320px;
            margin: 0 auto;
            padding: 0 2rem 4rem;
        }
        .download-section-title {
            font-family: 'Playfair Display', serif;
            font-size: 2rem;
            font-weight: 700;
            text-align: center;
            margin-bottom: 0.5rem;
            color: var(--text-primary);
        }
        .download-section-desc {
            text-align: center;
            color: var(--text-secondary);
            font-size: 0.95rem;
            margin-bottom: 2.5rem;
        }
        .download-grid {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 1.5rem;
            max-width: 960px;
            margin: 0 auto;
        }
        .dl-card {
            background: var(--bg-card);
            border: 1px solid var(--border);
            border-radius: var(--radius);
            padding: 2rem 1.5rem;
            text-align: center;
            transition: all var(--transition);
            box-shadow: var(--shadow-sm);
            position: relative;
            overflow: hidden;
        }
        .dl-card::before {
            content: '';
            position: absolute;
            top: 0; left: 0; right: 0;
            height: 3px;
            background: var(--accent);
            transform: scaleX(0);
            transition: transform var(--transition);
        }
        .dl-card:hover {
            transform: translateY(-4px);
            box-shadow: var(--shadow-lg);
            border-color: var(--accent);
        }
        .dl-card:hover::before { transform: scaleX(1); }

        .dl-card-icon {
            width: 64px; height: 64px;
            border-radius: 16px;
            background: var(--accent-light);
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 1rem;
            font-size: 1.6rem;
        }
        .dl-card h3 {
            font-family: 'Playfair Display', serif;
            font-size: 1.2rem;
            font-weight: 700;
            margin-bottom: 0.4rem;
            color: var(--text-primary);
        }
        .dl-card p {
            font-size: 0.85rem;
            color: var(--text-secondary);
            line-height: 1.6;
            margin-bottom: 1.25rem;
        }
        .dl-card .dl-btn {
            display: inline-flex;
            align-items: center;
            gap: 0.4rem;
            background: var(--accent);
            color: #fff;
            padding: 0.6rem 1.4rem;
            border-radius: var(--radius-sm);
            text-decoration: none;
            font-weight: 600;
            font-size: 0.85rem;
            transition: all var(--transition);
        }
        .dl-card .dl-btn:hover {
            background: var(--accent-hover);
            transform: translateY(-1px);
            color: #fff;
        }
        .dark-theme .dl-card .dl-btn { color: #0F0F0F; }
        .dark-theme .dl-card .dl-btn:hover { color: #0F0F0F; }
        .dl-card .dl-doc {
            display: block;
            margin-top: 0.75rem;
            font-size: 0.78rem;
            color: var(--text-muted);
            text-decoration: none;
            transition: color var(--transition);
        }
        .dl-card .dl-doc:hover { color: var(--accent); }

        /* ── Animations ── */
        .fade-up {
            opacity: 0;
            transform: translateY(30px);
            animation: fadeUp 0.7s ease forwards;
        }
        .fade-up-d1 { animation-delay: 0.1s; }
        .fade-up-d2 { animation-delay: 0.2s; }
        .fade-up-d3 { animation-delay: 0.3s; }

        @keyframes fadeUp {
            to { opacity: 1; transform: translateY(0); }
        }

        /* ── Responsive ── */
        @media (max-width: 992px) {
            .nav-inner {
                padding: 0.6rem 1.5rem;
            }
            .nav-links {
                margin-left: 0;
                flex-basis: 100%;
                gap: 0.1rem;
                padding-top: 0.4rem;
            }
            .nav-dropdown .dd-menu {
                left: 0;
                min-width: 220px;
            }
            .download-grid {
                grid-template-columns: 1fr;
                max-width: 400px;
            }
        }

        @media (max-width: 600px) {
            .nav-inner { padding: 0.5rem 0.75rem; }
            .nav-links a,
            .nav-links .nav-dropdown > a {
                font-size: 0.78rem;
                padding: 0.35rem 0.55rem;
            }
            .hero { padding: 3rem 1rem 1.5rem; }
            .hero h1 { font-size: 1.8rem; }
            .download-section { padding-left: 1rem; padding-right: 1rem; }
        }
    </style>
</head>
<body>

    <!-- ═══ NAVBAR ═══ -->
    <header class="top-nav">
        <div class="nav-inner">
            <a class="nav-brand" href="index1.aspx">DataAI</a>
            <button class="theme-toggle" id="themeToggle" aria-label="Toggle dark mode" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Switch to dark mode"></button>

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
                        <a href="UseCasePublic.aspx" target="_blank">Public Data</a>
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

    <!-- ═══ HERO ═══ -->
    <section class="hero fade-up">
        <h1><em>DataAI Software</em></h1>
        <form id="form1" runat="server">
            <p class="hero-desc">
                <asp:Label ID="Label1" runat="server" Text="DataAI automatically analyzes data structure, generates a set of preliminary reports, and provides a simple interface for creating ad hoc reports and conducting statistical research with AI."></asp:Label>
            </p>
            <div class="proposal-section">
                <asp:HyperLink ID="HyperLinkClient" runat="server" NavigateUrl="~/Partners.pdf" Target="_top" CssClass="proposal-link">
                    <svg width="18" height="18" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path d="M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z"/><polyline points="14 2 14 8 20 8"/><line x1="16" y1="13" x2="8" y2="13"/><line x1="16" y1="17" x2="8" y2="17"/><polyline points="10 9 9 9 8 9"/></svg>
                    OUR Business Proposal
                </asp:HyperLink>
            </div>

            <%-- Preserve hidden controls for code-behind compatibility --%>
            <div style="display:none">
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/index1.aspx" Target="_top">Home</asp:HyperLink>
                <%--<asp:Label ID="Label2" runat="server" Text=""></asp:Label>--%>
            </div>
        </form>
    </section>

    <!-- ═══ DOWNLOAD SOFTWARE ═══ -->
    <section class="download-section">
        <h2 class="download-section-title fade-up">Download Software</h2>
        <p class="download-section-desc fade-up fade-up-d1">Install on your own Windows 10/11 or Windows Server with IIS. Use as-is.</p>
        <div class="download-grid">
            <div class="dl-card fade-up fade-up-d1">
                <div class="dl-card-icon">
                    <svg width="28" height="28" fill="none" stroke="var(--accent)" stroke-width="2" viewBox="0 0 24 24"><path d="M21 15v4a2 2 0 01-2 2H5a2 2 0 01-2-2v-4"/><polyline points="7 10 12 15 17 10"/><line x1="12" y1="15" x2="12" y2="3"/></svg>
                </div>
                <h3>DataAI</h3>
                <p>Full version (~90 MB). Installer or manual install available.</p>
                <a href="DownloadDataAI.aspx" class="dl-btn">Download DataAI</a>
                <a href="https://oureports.net/OUReports/AIandDataAI.pdf" target="_blank" class="dl-doc">How to use DataAI</a>
            </div>
            <div class="dl-card fade-up fade-up-d2">
                <div class="dl-card-icon">
                    <svg width="28" height="28" fill="none" stroke="var(--accent)" stroke-width="2" viewBox="0 0 24 24"><path d="M21 15v4a2 2 0 01-2 2H5a2 2 0 01-2-2v-4"/><polyline points="7 10 12 15 17 10"/><line x1="12" y1="15" x2="12" y2="3"/></svg>
                </div>
                <h3>DataAI Lite</h3>
                <p>In-memory version (~90 MB). Analyze private data locally.</p>
                <a href="DownloadDataAI.aspx?DataAILitedown=yes" class="dl-btn">Download DataAI Lite</a>
                <a href="https://oureports.net/OUReports/ReadMeInstallDataAILite.pdf" target="_blank" class="dl-doc">Install - setup guide</a>
            </div>
            <div class="dl-card fade-up fade-up-d3">
                <div class="dl-card-icon">
                    <svg width="28" height="28" fill="none" stroke="var(--accent)" stroke-width="2" viewBox="0 0 24 24"><path d="M21 15v4a2 2 0 01-2 2H5a2 2 0 01-2-2v-4"/><polyline points="7 10 12 15 17 10"/><line x1="12" y1="15" x2="12" y2="3"/></svg>
                </div>
                <h3>TaskList AI</h3>
                <p>AI-powered project manager (~20 MB). Lightweight task management.</p>
                <a href="DownloadDataAI.aspx?tasklistdown=yes" class="dl-btn">Download TaskList AI</a>
                <a href="https://oureports.net/HelpDesk/Default.aspx" target="_blank" class="dl-doc">Use online project manager</a>
            </div>
        </div>
    </section>

    <!-- ═══ SCRIPTS ═══ -->
    <script>
        // Theme toggle
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

        // Touch-friendly dropdown toggles
        document.querySelectorAll('.nav-dropdown > a').forEach(function(toggle) {
            toggle.addEventListener('click', function(e) {
                if ('ontouchstart' in window || window.innerWidth <= 992) {
                    e.preventDefault();
                    var parent = toggle.closest('.nav-dropdown');
                    var wasOpen = parent.classList.contains('open');
                    document.querySelectorAll('.nav-dropdown.open').forEach(function(dd) {
                        dd.classList.remove('open');
                    });
                    if (!wasOpen) parent.classList.add('open');
                }
            });
        });
        document.addEventListener('click', function(e) {
            if (!e.target.closest('.nav-dropdown')) {
                document.querySelectorAll('.nav-dropdown.open').forEach(function(dd) {
                    dd.classList.remove('open');
                });
            }
        });

        // Scroll-triggered fade-in
        var observer = new IntersectionObserver(function(entries) {
            entries.forEach(function(entry) {
                if (entry.isIntersecting) {
                    entry.target.style.animationPlayState = 'running';
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.1 });

        document.querySelectorAll('.fade-up').forEach(function(el) {
            el.style.animationPlayState = 'paused';
            observer.observe(el);
        });

        // Initialize Bootstrap tooltips
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (el) { return new bootstrap.Tooltip(el); });
    </script>
</body>
</html>
