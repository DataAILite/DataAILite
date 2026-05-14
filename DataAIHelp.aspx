<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DataAIHelp.aspx.vb" Inherits="DataAIHelp" %>

<!DOCTYPE html>
<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>DataAI Help</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha1/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha1/dist/js/bootstrap.bundle.min.js"></script>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link href="https://fonts.googleapis.com/css2?family=DM+Sans:ital,opsz,wght@0,9..40,300;0,9..40,500;0,9..40,700;1,9..40,400&family=Playfair+Display:wght@600;700;800&display=swap" rel="stylesheet" />

    <style>
        :root {
            --bg-primary: #F6F8FB;
            --bg-secondary: #E9EEF5;
            --bg-card: #FFFFFF;
            --accent: #155E75;
            --accent-light: #E8F5F7;
            --accent-soft: #F3FAFB;
            --accent-hover: #0F4A5D;
            --blue: #2563EB;
            --blue-soft: #EEF4FF;
            --text-primary: #1A1A1A;
            --text-secondary: #4B5563;
            --text-muted: #7B8794;
            --red: #B42318;
            --red-soft: #FFF1F0;
            --border: #DDE5EE;
            --shadow-sm: 0 1px 4px rgba(15, 23, 42, 0.06);
            --shadow-md: 0 10px 28px rgba(15, 23, 42, 0.10);
            --radius: 10px;
            --radius-sm: 6px;
            --transition: 0.3s cubic-bezier(0.4, 0, 0.2, 1);
        }
        .dark-theme {
            --bg-primary: #0B1117; --bg-secondary: #111827; --bg-card: #17202A;
            --accent: #67E8F9; --accent-light: #10313A; --accent-soft: #12242B; --accent-hover: #A5F3FC;
            --blue: #93C5FD; --blue-soft: #12213B;
            --link-color: #5B9FFF; --link-hover: #8CBCFF;
            --text-primary: #F3F6FA; --text-secondary: #C7D1DC; --text-muted: #8392A3;
            --red: #FDA29B; --red-soft: #351818; --border: #263544;
            --shadow-sm: 0 1px 3px rgba(0,0,0,0.25); --shadow-md: 0 12px 32px rgba(0,0,0,0.35);
        }
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: 'DM Sans', sans-serif; background: radial-gradient(circle at top left, var(--accent-light), transparent 34rem), linear-gradient(180deg, var(--bg-primary), var(--bg-secondary)); color: var(--text-primary); overflow-x: hidden; transition: background var(--transition), color var(--transition); font-size: 13px; min-height: 100vh; }
        a { color: var(--blue); text-decoration: none; font-weight: 500; transition: color var(--transition), background var(--transition), border-color var(--transition); }
        a:hover { color: var(--accent-hover); }

        .theme-toggle { width: 42px; height: 24px; background: var(--border); border-radius: 999px; position: relative; cursor: pointer; border: none; transition: background var(--transition); }
        .theme-toggle::after { content: ''; width: 18px; height: 18px; background: var(--bg-card); border-radius: 50%; position: absolute; top: 3px; left: 3px; transition: transform var(--transition); box-shadow: 0 1px 3px rgba(0,0,0,0.15); }
        .dark-theme .theme-toggle { background: var(--accent); }
        .dark-theme .theme-toggle::after { transform: translateX(18px); }

        .top-bar { display: flex; justify-content: space-between; align-items: center; width: 100%; margin: 0.5rem auto 0; padding: 0 1.5rem; }
        .top-bar-left { display: flex; align-items: center; gap: 1.25rem; }
        .home-link { font-size: 0.82rem; font-weight: 600; padding: 0.25rem 0.45rem; border-radius: var(--radius-sm); }
        .home-link:hover { background: var(--accent-light); }

        /* ── Hero (compact) ── */
        .hero { width: 100%; margin: 0 auto; padding: 1.0rem 1.5rem 0.45rem; text-align: center; }
        .hero h1 { font-family: 'Playfair Display', serif; font-size: clamp(1.65rem, 3.5vw, 2.25rem); font-weight: 800; color: var(--accent); margin-bottom: 0.15rem; letter-spacing: 0; }
        .hero-lead { font-size: 0.86rem; color: var(--text-secondary); margin-bottom: 0.75rem; }
        .highlight-note { display: none; width: fit-content; max-width: 92%; margin: -0.35rem auto 0.65rem; padding: 0.28rem 0.7rem; border: 2px dashed #C1121F; border-radius: 999px; background: #FFFE71; color: #111827; font-size: 0.78rem; font-weight: 800; box-shadow: 0 0 0 2px rgba(217, 119, 6, 0.18); background-image: linear-gradient(#FFFE71, #FFFE71), repeating-linear-gradient(90deg, #C1121F 0 9px, transparent 9px 18px), repeating-linear-gradient(180deg, #C1121F 0 9px, transparent 9px 18px), repeating-linear-gradient(270deg, #C1121F 0 9px, transparent 9px 18px), repeating-linear-gradient(0deg, #C1121F 0 9px, transparent 9px 18px); background-origin: padding-box, border-box, border-box, border-box, border-box; background-clip: padding-box, border-box, border-box, border-box, border-box; background-size: auto, 54px 2px, 2px 54px, 54px 2px, 2px 54px; background-position: 0 0, 0 0, 100% 0, 0 100%, 0 0; }
        .highlight-note.flash-on { display: inline-block; animation: hiltFlash 1.15s ease-in-out infinite, runDashes 7s linear infinite; }
        @keyframes hiltFlash {
            0%, 100% { opacity: 1; border-color: #C1121F; box-shadow: 0 0 0 2px rgba(217, 119, 6, 0.18); }
            50% { opacity: 0.55; border-color: #FF4D4D; box-shadow: 0 0 0 5px rgba(217, 119, 6, 0.08); }
        }
        @keyframes runDashes {
            0% { background-position: 0 0, 0 0, 100% 0, 0 100%, 0 0; }
            100% { background-position: 0 0, 54px 0, 100% 54px, -54px 100%, 0 -54px; }
        }

        /* ── Quick-Nav Row: cards + buttons inline ── */
        .nav-row { display: flex; gap: 0.42rem; justify-content: center; flex-wrap: wrap; margin: 0 auto 0.72rem; max-width: 92%; }
        .nav-btn { display: inline-flex; align-items: center; gap: 0.3rem; padding: 0.42rem 0.78rem; border-radius: 999px; font-size: 0.76rem; font-weight: 700; border: 1px solid var(--border); background: rgba(255, 255, 255, 0.78); color: var(--text-primary); transition: all var(--transition); text-decoration: none; box-shadow: var(--shadow-sm); }
        .dark-theme .nav-btn { background: rgba(23, 32, 42, 0.86); }
        .nav-btn:hover { border-color: var(--blue); color: var(--blue); box-shadow: var(--shadow-md); transform: translateY(-1px); }
        .nav-btn.primary { background: var(--accent); color: #fff; border-color: var(--accent); }
        .nav-btn.primary:hover { background: var(--accent-hover); border-color: var(--accent-hover); color: #fff; }

        /* ── Search (compact) ── */
        .search-wrap { max-width: 390px; margin: 0 auto 0.15rem; position: relative; }
        .search-wrap input { width: 100%; padding: 0.48rem 0.85rem 0.48rem 2rem; border-radius: 999px; border: 1px solid var(--border); background: var(--bg-card); font-family: 'DM Sans', sans-serif; font-size: 0.82rem; color: var(--text-primary); transition: border-color var(--transition), box-shadow var(--transition); box-shadow: var(--shadow-sm); }
        .search-wrap input:focus { outline: none; border-color: var(--blue); box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.12); }
        .dark-theme .search-wrap input { background: var(--bg-secondary); color: var(--text-primary); }
        .search-wrap .s-icon { position: absolute; left: 0.65rem; top: 50%; transform: translateY(-50%); font-size: 0.78rem; color: var(--text-muted); pointer-events: none; }

        /* ── Content ── */
        .content { width: 80%; margin: 0 auto; padding: 0.6rem 0.75rem 1.5rem; }

        /* ── 2-column layout for section blocks ── */
        .blocks-grid { display: grid; grid-template-columns: 1fr; gap: 0.72rem; }

        /* ── Section Block ── */
        .sec-card { display: grid; grid-template-columns: 165px 1fr; background: rgba(255, 255, 255, 0.92); border: 1px solid var(--border); border-radius: var(--radius); box-shadow: var(--shadow-sm); transition: box-shadow var(--transition), transform var(--transition), border-color var(--transition); overflow: hidden; }
        .dark-theme .sec-card { background: rgba(23, 32, 42, 0.94); }
        .sec-card:hover { box-shadow: var(--shadow-md); transform: translateY(-1px); border-color: rgba(21, 94, 117, 0.32); }
        .sec-hdr { padding: 0.62rem 0.85rem; border-right: 1px solid var(--border); background: linear-gradient(180deg, var(--accent-light), var(--accent-soft)); display: flex; align-items: center; }
        .sec-title { font-family: 'Playfair Display', serif; font-size: 0.92rem; font-weight: 800; color: var(--accent); line-height: 1.22; }

        /* ── TOC Grid inside blocks ── */
        .toc-grid { display: grid; grid-template-columns: repeat(3, minmax(0, 1fr)); }
        .toc-grid.c2 { grid-template-columns: repeat(2, minmax(0, 1fr)); }
        .toc-grid.c3 { grid-template-columns: repeat(3, minmax(0, 1fr)); }
        .toc-grid.c4 { grid-template-columns: repeat(4, minmax(0, 1fr)); }
        .toc-grid.c5 { grid-template-columns: repeat(5, minmax(0, 1fr)); }
        .toc-grid.c6 { grid-template-columns: repeat(6, minmax(0, 1fr)); }
        .toc-grid.c7 { grid-template-columns: repeat(7, minmax(0, 1fr)); }

        .toc-cell { padding: 0.52rem 0.58rem; border-right: 1px solid var(--border); border-bottom: 1px solid var(--border); min-width: 0; }
        .toc-grid > .toc-cell { border-right: 1px solid var(--border); border-bottom: none; }
        .toc-grid > .toc-cell:last-child { border-right: none; }

        .cell-title { font-size: 0.76rem; font-weight: 800; color: var(--text-primary); margin-bottom: 0.32rem; display: flex; align-items: center; gap: 0.35rem; }
        .cell-title .dot { width: 7px; height: 7px; border-radius: 50%; flex-shrink: 0; box-shadow: 0 0 0 3px rgba(21, 94, 117, 0.10); }
        .cell-title a { color: var(--text-primary); font-weight: 700; }
        .cell-title a:hover { color: #0066CC; }

        .toc-link { display: block; padding: 0.11rem 0 0.11rem 0.55rem; font-size: 0.74rem; color: var(--text-secondary); text-decoration: none; font-weight: 600; border-left: 2px solid transparent; border-radius: 0 var(--radius-sm) var(--radius-sm) 0; transition: all 0.2s ease; line-height: 1.42; }
        .toc-link:hover { color: var(--blue); border-left-color: var(--blue); background: var(--blue-soft); padding-left: 0.85rem; }
        .toc-link.feat { color: var(--red); font-weight: 800; background: transparent; }
        .toc-link.feat:hover { color: var(--red); background: var(--red-soft); border-left-color: var(--red); }
        .toc-link.sub { padding-left: 1.25rem; font-size: 0.72rem; color: var(--text-muted); }
        .toc-link.sub:hover { padding-left: 1.4rem; color: #0066CC; }

        /* ── Full-width block (spans both columns) ── */
        .full-width { grid-column: 1 / -1; }

        /* ── Footer ── */
        .site-footer { text-align: center; padding: 0.95rem 1rem; color: var(--text-muted); font-size: 0.72rem; border-top: 1px solid var(--border); margin-top: 0.4rem; background: rgba(255, 255, 255, 0.35); }
        .site-footer a { font-weight: 600; }

        .hilt-yellow {
            background: #FFFE71 !important;
            color: #111827 !important;
            border-left-color: #D97706 !important;
            border-radius: 3px;
            box-shadow: 0 0 0 2px rgba(217, 119, 6, 0.22);
            padding: 0.08rem 0.3rem !important;
        }
        .toc-link.feat.hilt-yellow,
        .toc-link.feat.hilt-yellow:hover,
        .toc-link.feat.search-yellow,
        .toc-link.feat.search-yellow:hover,
        a.hilt-yellow,
        a.hilt-yellow:hover,
        a.search-yellow,
        a.search-yellow:hover {
            background: #FFFE71 !important;
            color: #111827 !important;
        }
        .search-yellow {
            background: #FFFE71 !important;
            color: #111827 !important;
            border-left-color: #D97706 !important;
            border-radius: 3px;
            box-shadow: 0 0 0 2px rgba(217, 119, 6, 0.22);
            padding: 0.08rem 0.3rem !important;
        }

        @media (max-width: 900px) {
            .blocks-grid { grid-template-columns: 1fr; }
            .content { width: 96%; }
            .sec-card { grid-template-columns: 1fr; }
            .sec-hdr { border-right: none; border-bottom: 1px solid var(--border); }
            .toc-grid, .toc-grid.c2, .toc-grid.c3, .toc-grid.c4, .toc-grid.c5, .toc-grid.c6, .toc-grid.c7 { grid-template-columns: repeat(2, 1fr); }
            .toc-grid > .toc-cell { border-right: 1px solid var(--border); border-bottom: 1px solid var(--border); }
            .toc-grid > .toc-cell:nth-child(2n) { border-right: none; }
        }
        @media (max-width: 600px) {
            .content { width: 100%; padding-left: 0.5rem; padding-right: 0.5rem; }
            .toc-grid, .toc-grid.c2, .toc-grid.c3, .toc-grid.c4, .toc-grid.c5, .toc-grid.c6, .toc-grid.c7 { grid-template-columns: 1fr; }
            .toc-cell { border-right: none !important; }
            .top-bar, .hero { padding-left: 1rem; padding-right: 1rem; }
        }
    </style>
</head>
<body>

<!-- ═══ TOP BAR ═══ -->
<div class="top-bar">
    <div class="top-bar-left">
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx" CssClass="home-link">&#8592; Log off</asp:HyperLink>
        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="http://DataAI.link" CssClass="home-link" style="color:var(--text-secondary); font-weight:500;">DataAI.link</asp:HyperLink>
    </div>
    <button class="theme-toggle" id="themeToggle" aria-label="Toggle dark mode" title="Switch to dark mode"></button>
</div>

<!-- ═══ HERO ═══ -->
<section class="hero">
    <h1>DataAI Help</h1>
    <p class="hero-lead">Videos, samples, documentation, downloads &mdash; everything you need to master DataAI.</p>
    <p id="highlightNote" class="highlight-note">Click highlighted links below to open the particular documentation.</p>
    <div class="nav-row">
        <a class="nav-btn primary" href="http://DataAI.link">&#9654; Live Demos &amp; Downloads</a>
        <a class="nav-btn" href="https://oureports.net/oureports/OnlineUserReporting.pdf">&#128196; Full PDF Manual</a>
        <a class="nav-btn" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=7">&#127968; Landing Page</a>
        <%--<a class="nav-btn" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=7">&#128274; Sign In</a>--%>
        <a class="nav-btn" href="https://oureports.net/OUReports/AdvancedReportDesigner.pdf#page=4">&#128295; Advanced Report Designer</a>
        <a class="nav-btn" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=68">&#128202; Analytics</a>
        <a class="nav-btn" href="AnalyticsNew.pdf">&#128202; More Analytics</a>
        <a class="nav-btn" href="Market.pdf">&#128200; Market Models</a>
        <a class="nav-btn" href="https://oureports.net/oureports/GoogleChartsAndDashboards.pdf">&#128200; Google Charts &amp; Dashboards</a>
        <a class="nav-btn" href="https://oureports.net/oureports/DataImport.pdf">&#128229; Data Import</a>
        <a class="nav-btn" href="https://oureports.net/oureports/AIandDataAI.pdf">&#129302; AI &amp; DataAI</a>
        <a class="nav-btn" href="https://oureports.net/oureports/DataAILite.pdf">&#9889; DataAILite</a>
        <a class="nav-btn" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=42">&#127758; Map Definition</a>
        <a class="nav-btn" href="https://oureports.net/OUReports/MatrixBalancing.pdf">&#129518; Matrix Balancing</a>
    </div>
    <div class="search-wrap">
        <span class="s-icon">&#128269;</span>
        <input type="text" id="tocSearch" placeholder="Search help topics..." oninput="filterTopics(this.value)" />
    </div>
</section>

<!-- ═══ MAIN ═══ -->
<form id="form1" runat="server">
<div class="content">
    <div class="blocks-grid">

        <!-- ANALYTICS HELP -->
        <div class="sec-card full-width">
            <div class="sec-hdr"><span class="sec-title">Analytics</span></div>
            <div class="toc-grid c5">
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:var(--accent)"></span> Analytics</div>
                    <a class="toc-link feat" href="AnalyticsNew.pdf#page=2">Analytics Dashboard</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=68">Detail Analytics</a>
                    <a class="toc-link" href="AnalyticsNew.pdf#page=3">Pivot / Cross Tab</a>
                    <a class="toc-link" href="AnalyticsNew.pdf#page=4">Variance Analysis</a>
                    <a class="toc-link" href="AnalyticsNew.pdf#page=5">Comparison Reports</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#D97706"></span> Statistics</div>
                    <a class="toc-link feat" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=54">Overall Statistics</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=55">Export Statistics to Excel</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=68">Group Statistics</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#2563EB"></span> Data Review</div>
                    <a class="toc-link" href="AnalyticsNew.pdf#page=6">Data Profiling</a>
                    <a class="toc-link" href="AnalyticsNew.pdf#page=7">Data Quality</a>
                    <a class="toc-link" href="AnalyticsNew.pdf#page=8">Ranking Analysis</a>
                    <a class="toc-link" href="AnalyticsNew.pdf#page=16">Audit Summaries</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#7C3AED"></span> Models</div>
                    <a class="toc-link feat" href="AnalyticsNew.pdf#page=9">Regression Analysis</a>
                    <a class="toc-link feat" href="AnalyticsNew.pdf#page=10">Trends</a>
                    <a class="toc-link" href="AnalyticsNew.pdf#page=11">Time Based Summaries</a>
                    <a class="toc-link" href="AnalyticsNew.pdf#page=12">Time Series</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#D97706"></span> Specialized Views</div>
                    <a class="toc-link" href="AnalyticsNew.pdf#page=13">Outlier Flagging</a>
                    <a class="toc-link feat" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=73">Fields Correlation</a>
                    <a class="toc-link" href="AnalyticsNew.pdf#page=14">Correlation Threshold</a>
                </div>
            </div>
        </div>

        <!-- MARKET HELP -->
        <div class="sec-card full-width">
            <div class="sec-hdr"><span class="sec-title">Market</span></div>
            <div class="toc-grid c4">
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:var(--accent)"></span> Market Overview</div>
                    <a class="toc-link feat" href="Market.pdf#page=2">Market Dashboard</a>
                    <a class="toc-link" href="Market.pdf#page=3">Market Demand</a>
                    <a class="toc-link" href="Market.pdf#page=4">Market Pricing</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#2563EB"></span> Market Behavior</div>
                    <a class="toc-link" href="Market.pdf#page=5">Market Elasticity</a>
                    <a class="toc-link" href="Market.pdf#page=6">Market Basket</a>
                    <a class="toc-link" href="Market.pdf#page=7">Market Segments</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#7C3AED"></span> Risk and Operations</div>
                    <a class="toc-link" href="Market.pdf#page=8">Market Churn</a>
                    <a class="toc-link" href="Market.pdf#page=9">Market Risk</a>
                    <a class="toc-link" href="Market.pdf#page=10">Market Inventory</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#D97706"></span> Value Models</div>
                    <a class="toc-link" href="Market.pdf#page=11">Market Profit</a>
                    <a class="toc-link" href="Market.pdf#page=12">Market Scenario</a>
                </div>
            </div>
        </div>

        <!-- EXPLORE REPORT DATA -->
        <div class="sec-card">
            <div class="sec-hdr"><span class="sec-title">Explore Report Data</span></div>
            <div class="toc-grid c7">
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:var(--accent)"></span> Data Exploration</div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=44">Selecting Parameters</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=45">Field Search</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#2563EB"></span> Export Data</div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=46">Export to Excel</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=48">Export to CSV</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=50">Export to Delimiter File</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=52">Export to XML</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#7C3AED"></span> Class Table Explorer</div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=74">Link to Tables</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=75">Mouse Over Children</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=76">Parent Link</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=77">Sample</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#D97706"></span> Import and AI</div>
                    <a class="toc-link feat" href="https://oureports.net/oureports/DataImport.pdf">Data Import</a>
                    <a class="toc-link feat" href="https://oureports.net/oureports/AIandDataAI.pdf">AI and DataAI</a>
                    <a class="toc-link" href="https://oureports.net/OUReports/MatrixBalancing.pdf">Matrix Balancing</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#D97706"></span> Chart</div>
                    <a class="toc-link feat" href="https://oureports.net/oureports/GoogleChartsAndDashboards.pdf">Google Charts</a>
                    <a class="toc-link" href="AnalyticsNew.pdf#page=15">Chart Recommendations</a>
                    <a class="toc-link" href="https://oureports.net/oureports/GoogleChartsAndDashboards.pdf">Chart Dashboards</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:var(--red)"></span> Map</div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=42">Map Report</a>
                    <a class="toc-link" href="AnalyticsNew.pdf#page=17">Map Readiness</a>
                </div>
            </div>
        </div>

        <!-- ── REPORT VIEWS ── -->
        <div class="sec-card">
            <div class="sec-hdr"><span class="sec-title">Report Views</span></div>
            <div class="toc-grid">
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:var(--accent)"></span> Parameters</div>
                    <a class="toc-link feat" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=57">Generic Report</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=58">Select Parameters</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=58">Not Related Parameters</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=17">Related Parameters</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#2563EB"></span> Report Graphs</div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=60">Bar Report</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=60">PIE Report</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=61">Line Report</a>
                    <a class="toc-link feat" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=62">Matrix Report</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=68">DrillDown Groups</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#7C3AED"></span> Export Report</div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=62">Export to Excel</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=62">Report Viewer</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=63">Options Menu</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=64">Export to Word</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=66">Export to PDF</a>
                </div>
            </div>
        </div>

        <!-- ── REPORT FORMAT DEFINITION ── -->
        <div class="sec-card">
            <div class="sec-hdr"><span class="sec-title">Report Format Definition</span></div>
            <div class="toc-grid c4">
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:var(--accent)"></span> <a href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=32">Columns &amp; Expressions</a></div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=32">Add Report Column</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=32">Friendly Name</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=33">Define Expression</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=33">Add Column</a>
                    <a class="toc-link feat" href="https://oureports.net/OUReports/AdvancedReportDesigner.pdf#page=4">Advanced Report Designer</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:var(--accent)"></span> Column Actions</div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=33">Change Column Order</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=35">Edit Report Column</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=35">Delete Report Column</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=35">Update Report Format</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#2563EB"></span> <a href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=36">Groups &amp; Totals</a></div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=36">Add Group</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=37">Edit Group</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=37">Change Group Order</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=37">Delete Group</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=37">Change Group Totals</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#7C3AED"></span> <a href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=38">Combine Column Values</a></div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=38">Add Combined Column</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=40">Replace with Combined</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=41">Delete Combined Column</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=41">Update Combined Column</a>
                </div>
            </div>
        </div>

        <!-- ── REPORT DATA DEFINITION ── -->
        <div class="sec-card">
            <div class="sec-hdr"><span class="sec-title">Report Data Definition</span></div>
            <div class="toc-grid c5">
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:var(--accent)"></span> <a href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=24">Data Fields</a></div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=25">Add Data Field</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=25">Delete Data Field</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=25">Update Data Fields</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#2563EB"></span> <a href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=26">Join Tables</a></div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=26">Add Join</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=26">Add Manually</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=26">From Possible Joins</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=26">List of Added Joins</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#2563EB"></span> Join Actions</div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=27">Reverse Join</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=27">Change Order</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=27">Delete Join</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=27">Edit Join</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=27">Update Join</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#7C3AED"></span> <a href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=28">Filters</a></div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=28">Add Condition</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=29">Edit Condition</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=29">Delete Condition</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=29">Customizing Logic</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=29">Updating Filters</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#D97706"></span> <a href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=30">Sorting</a></div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=30">Add Sort</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=31">Change Sort Order</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=31">Delete Sort</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=31">Edit Sort</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=31">Update Sort</a>
                </div>
            </div>
        </div>

        <!-- ── REPORT MANAGEMENT ── -->
        <div class="sec-card">
            <div class="sec-hdr"><span class="sec-title">Report Management</span></div>
            <div class="toc-grid c7">
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:var(--accent)"></span> Getting Started</div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=8">From Start Page</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=9">From Sign In Page</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:var(--red)"></span> <a href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=10">List of Reports</a></div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=10">Showing Report</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=10">Copying Report</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=11">Creating Report</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:var(--red)"></span> Report Actions</div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=11">Deleting Report</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=12">Editing Report</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=13">Advanced User</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#7C3AED"></span> <a href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=13">Report Info</a></div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=13">Normal User View</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=13">Report Title</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=13">Report Orientation</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=13">Page Footer</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#7C3AED"></span> Advanced Info</div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=14">Advanced User View</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=14">Report ID</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=14">Data Source</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=15">Data Query Text</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=15">Report Files</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#D97706"></span> <a href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=17">Parameters</a></div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=17">Normal User</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=17">Related Parameters</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=18">Add Parameter</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=20">Edit Parameter</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=21">Delete Parameter</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#2563EB"></span> <a href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=22">Users</a></div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=22">Add User</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=23">Edit User</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=23">Delete User</a>
                </div>
            </div>
        </div>

    </div><!-- /blocks-grid -->
</div><!-- /content -->
</form>

<!-- ═══ FOOTER ═══ -->
<div class="site-footer">
    DataAI &mdash; AI-driven analytics, dashboards &amp; reporting &ensp;|&ensp;
    <a href="http://DataAI.link">DataAI.link</a> &ensp;|&ensp;
    <a href="https://oureports.net/oureports/DataAILite.pdf">DataAILite</a>
</div>

<!-- ═══ SCRIPTS ═══ -->
<script>
    var themeBtn = document.getElementById('themeToggle');
    var highlightNote = document.getElementById('highlightNote');
    function setHighlightNote(active) {
        if (!highlightNote) return;
        if (active) {
            highlightNote.classList.add('flash-on');
        } else {
            highlightNote.classList.remove('flash-on');
        }
    }

    themeBtn.addEventListener('click', function () {
        document.body.classList.toggle('dark-theme');
        var isDark = document.body.classList.contains('dark-theme');
        themeBtn.setAttribute('title', isDark ? 'Switch to light mode' : 'Switch to dark mode');
        themeBtn.setAttribute('aria-label', isDark ? 'Switch to light mode' : 'Switch to dark mode');
    });

    (function () {
        var urlParams = new URLSearchParams(window.location.search);
        var hilt = urlParams.get("hilt");
        if (!hilt) return;

        function norm(text) {
            return (text || '').toLowerCase()
                .replace(/&/g, ' and ')
                .replace(/[^a-z0-9]+/g, ' ')
                .replace(/\s+/g, ' ')
                .trim();
        }

        var target = norm(hilt);
        var firstMatch = null;
        document.querySelectorAll("a, .cell-title, .sec-title, .nav-btn").forEach(function (el) {
            var text = norm(el.textContent);
            if (text && (text.indexOf(target) >= 0 || target.indexOf(text) >= 0)) {
                el.classList.add("hilt-yellow");
                if (!firstMatch) firstMatch = el;
            }
        });
        if (firstMatch) {
            setHighlightNote(true);
            firstMatch.scrollIntoView({ behavior: "smooth", block: "center" });
            if (firstMatch.focus) firstMatch.focus();
        }
    })();

    function filterTopics(q) {
        var target = (q || '').toLowerCase().trim();
        document.querySelectorAll(".search-yellow").forEach(function (el) {
            el.classList.remove("search-yellow");
        });
        if (!target) {
            setHighlightNote(document.querySelectorAll(".hilt-yellow").length > 0);
            return;
        }

        var found = false;
        document.querySelectorAll("a").forEach(function (link) {
            if (link.textContent.toLowerCase().indexOf(target) !== -1) {
                link.classList.add("search-yellow");
                found = true;
            }
        });
        setHighlightNote(found || document.querySelectorAll(".hilt-yellow").length > 0);
    }
</script>
</body>
</html>
