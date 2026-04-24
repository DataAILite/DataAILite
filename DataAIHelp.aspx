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
            --bg-primary: #FAFAF8;
            --bg-secondary: #F0EFEB;
            --bg-card: #FFFFFF;
            --accent: #1B6B4A;
            --accent-light: #E8F5EE;
            --accent-hover: #145236;
            --text-primary: #1A1A1A;
            --text-secondary: #5A5A5A;
            --text-muted: #8A8A8A;
            --red: #CC0000;
            --border: #E2E0DB;
            --shadow-sm: 0 1px 3px rgba(0,0,0,0.04);
            --shadow-md: 0 4px 20px rgba(0,0,0,0.06);
            --radius: 12px;
            --radius-sm: 7px;
            --transition: 0.3s cubic-bezier(0.4, 0, 0.2, 1);
        }
        .dark-theme {
            --bg-primary: #0F0F0F; --bg-secondary: #1A1A1A; --bg-card: #1E1E1E;
            --accent: #3DD68C; --accent-light: #162B20; --accent-hover: #5AEAA5;
            --link-color: #5B9FFF; --link-hover: #8CBCFF;
            --text-primary: #F0F0F0; --text-secondary: #A0A0A0; --text-muted: #666;
            --red: #EF5350; --border: #2A2A2A;
            --shadow-sm: 0 1px 3px rgba(0,0,0,0.2); --shadow-md: 0 4px 20px rgba(0,0,0,0.3);
        }
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: 'DM Sans', sans-serif; background: var(--bg-primary); color: var(--text-primary); overflow-x: hidden; transition: background var(--transition), color var(--transition); font-size: 13px; }
        a { color: #0066CC; text-decoration: none; font-weight: 500; transition: color var(--transition); }
        a:hover { color: #004499; }

        .theme-toggle { width: 42px; height: 24px; background: var(--border); border-radius: 999px; position: relative; cursor: pointer; border: none; transition: background var(--transition); }
        .theme-toggle::after { content: ''; width: 18px; height: 18px; background: var(--bg-card); border-radius: 50%; position: absolute; top: 3px; left: 3px; transition: transform var(--transition); box-shadow: 0 1px 3px rgba(0,0,0,0.15); }
        .dark-theme .theme-toggle { background: var(--accent); }
        .dark-theme .theme-toggle::after { transform: translateX(18px); }

        .top-bar { display: flex; justify-content: space-between; align-items: center; max-width: 1280px; margin: 0.5rem auto 0; padding: 0 1.5rem; }
        .top-bar-left { display: flex; align-items: center; gap: 1.25rem; }
        .home-link { font-size: 0.82rem; font-weight: 600; }

        /* ── Hero (compact) ── */
        .hero { max-width: 1280px; margin: 0 auto; padding: 1.1rem 1.5rem 0.35rem; text-align: center; }
        .hero h1 { font-family: 'Playfair Display', serif; font-size: clamp(1.5rem, 3.5vw, 2rem); font-weight: 700; color: var(--accent); margin-bottom: 0.2rem; }
        .hero-lead { font-size: 0.85rem; color: var(--text-secondary); margin-bottom: 0.7rem; }

        /* ── Quick-Nav Row: cards + buttons inline ── */
        .nav-row { display: flex; gap: 0.5rem; justify-content: center; flex-wrap: wrap; margin-bottom: 0.65rem; }
        .nav-btn { display: inline-flex; align-items: center; gap: 0.3rem; padding: 0.38rem 0.85rem; border-radius: var(--radius-sm); font-size: 0.78rem; font-weight: 600; border: 1px solid var(--border); background: var(--bg-card); color: var(--text-primary); transition: all var(--transition); text-decoration: none; }
        .nav-btn:hover { border-color: #0066CC; color: #0066CC; box-shadow: var(--shadow-sm); transform: translateY(-1px); }
        .nav-btn.primary { background: var(--accent); color: #fff; border-color: var(--accent); }
        .nav-btn.primary:hover { background: var(--accent-hover); border-color: var(--accent-hover); color: #fff; }

        /* ── Search (compact) ── */
        .search-wrap { max-width: 360px; margin: 0 auto 0.15rem; position: relative; }
        .search-wrap input { width: 100%; padding: 0.42rem 0.85rem 0.42rem 2rem; border-radius: var(--radius-sm); border: 1px solid var(--border); background: var(--bg-card); font-family: 'DM Sans', sans-serif; font-size: 0.82rem; color: var(--text-primary); transition: border-color var(--transition); }
        .search-wrap input:focus { outline: none; border-color: #0066CC; }
        .dark-theme .search-wrap input { background: var(--bg-secondary); color: var(--text-primary); }
        .search-wrap .s-icon { position: absolute; left: 0.65rem; top: 50%; transform: translateY(-50%); font-size: 0.78rem; color: var(--text-muted); pointer-events: none; }

        /* ── Content ── */
        .content { max-width: 1280px; margin: 0 auto; padding: 0.6rem 1.5rem 1.5rem; }

        /* ── 2-column layout for section blocks ── */
        .blocks-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 0.6rem; }

        /* ── Section Block ── */
        .sec-card { background: var(--bg-card); border: 1px solid var(--border); border-radius: var(--radius); box-shadow: var(--shadow-sm); transition: box-shadow var(--transition); overflow: hidden; }
        .sec-card:hover { box-shadow: var(--shadow-md); }
        .sec-hdr { padding: 0.55rem 0.85rem; border-bottom: 1px solid var(--border); }
        .sec-title { font-family: 'Playfair Display', serif; font-size: 0.88rem; font-weight: 700; }

        /* ── TOC Grid inside blocks ── */
        .toc-grid { display: grid; grid-template-columns: repeat(3, 1fr); }
        .toc-grid.c2 { grid-template-columns: repeat(2, 1fr); }
        .toc-grid.c4 { grid-template-columns: repeat(4, 1fr); }

        .toc-cell { padding: 0.55rem 0.7rem; border-right: 1px solid var(--border); border-bottom: 1px solid var(--border); }
        .toc-grid > .toc-cell:nth-child(3n) { border-right: none; }
        .toc-grid.c2 > .toc-cell:nth-child(3n) { border-right: 1px solid var(--border); }
        .toc-grid.c2 > .toc-cell:nth-child(2n) { border-right: none; }
        .toc-grid.c4 > .toc-cell:nth-child(3n) { border-right: 1px solid var(--border); }
        .toc-grid.c4 > .toc-cell:nth-child(4n) { border-right: none; }
        .toc-grid > .toc-cell:last-child { border-bottom: none; }
        .toc-grid > .toc-cell:nth-last-child(-n+3) { border-bottom: none; }
        .toc-grid.c2 > .toc-cell:nth-last-child(-n+3) { border-bottom: 1px solid var(--border); }
        .toc-grid.c2 > .toc-cell:nth-last-child(-n+2) { border-bottom: none; }
        .toc-grid.c4 > .toc-cell:nth-last-child(-n+3) { border-bottom: 1px solid var(--border); }
        .toc-grid.c4 > .toc-cell:nth-last-child(-n+4) { border-bottom: none; }

        .cell-title { font-size: 0.76rem; font-weight: 700; color: var(--text-primary); margin-bottom: 0.3rem; display: flex; align-items: center; gap: 0.35rem; }
        .cell-title .dot { width: 6px; height: 6px; border-radius: 50%; flex-shrink: 0; }
        .cell-title a { color: var(--text-primary); font-weight: 700; }
        .cell-title a:hover { color: #0066CC; }

        .toc-link { display: block; padding: 0.1rem 0 0.1rem 0.7rem; font-size: 0.76rem; color: var(--text-secondary); text-decoration: none; font-weight: 500; border-left: 2px solid transparent; transition: all 0.2s ease; line-height: 1.5; }
        .toc-link:hover { color: #0066CC; border-left-color: #0066CC; background: #EBF2FF; padding-left: 0.85rem; }
        .toc-link.feat { color: var(--red); font-weight: 600; }
        .toc-link.feat:hover { color: #0066CC; }
        .toc-link.sub { padding-left: 1.25rem; font-size: 0.72rem; color: var(--text-muted); }
        .toc-link.sub:hover { padding-left: 1.4rem; color: #0066CC; }

        /* ── Full-width block (spans both columns) ── */
        .full-width { grid-column: 1 / -1; }

        /* ── Footer ── */
        .site-footer { text-align: center; padding: 0.85rem 1rem; color: var(--text-muted); font-size: 0.72rem; border-top: 1px solid var(--border); margin-top: 0.25rem; }
        .site-footer a { font-weight: 600; }

        .hilt-yellow { background-color: #FEF08A; border-radius: 3px; padding: 0 3px; }

        @media (max-width: 900px) {
            .blocks-grid { grid-template-columns: 1fr; }
            .toc-grid, .toc-grid.c4 { grid-template-columns: repeat(2, 1fr); }
            .toc-grid > .toc-cell:nth-child(3n) { border-right: 1px solid var(--border); }
            .toc-grid > .toc-cell:nth-child(2n) { border-right: none; }
        }
        @media (max-width: 600px) {
            .toc-grid, .toc-grid.c2, .toc-grid.c4 { grid-template-columns: 1fr; }
            .toc-cell { border-right: none !important; }
            .top-bar, .hero, .content { padding-left: 1rem; padding-right: 1rem; }
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
    <div class="nav-row">
        <a class="nav-btn primary" href="http://DataAI.link">&#9654; Live Demos &amp; Downloads</a>
        <a class="nav-btn" href="https://oureports.net/oureports/OnlineUserReporting.pdf">&#128196; Full PDF Manual</a>
        <a class="nav-btn" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=7">&#127968; Landing Page</a>
        <%--<a class="nav-btn" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=7">&#128274; Sign In</a>--%>
        <a class="nav-btn" href="https://oureports.net/OUReports/AdvancedReportDesigner.pdf#page=4">&#128295; Advanced Report Designer</a>
        <a class="nav-btn" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=42">&#127758; Map Definition</a>
        <a class="nav-btn" href="https://oureports.net/OUReports/MatrixBalancing.pdf">&#129518; Matrix Balancing</a>
        <a class="nav-btn" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=68">&#128202; Analytics</a>
        <a class="nav-btn" href="https://oureports.net/oureports/GoogleChartsAndDashboards.pdf">&#128200; Google Charts &amp; Dashboards</a>
        <a class="nav-btn" href="https://oureports.net/oureports/DataImport.pdf">&#128229; Data Import</a>
        <a class="nav-btn" href="https://oureports.net/oureports/AIandDataAI.pdf">&#129302; AI &amp; DataAI</a>
        <a class="nav-btn" href="https://oureports.net/oureports/DataAILite.pdf">&#9889; DataAILite</a>
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

        <!-- ── REPORT MANAGEMENT ── -->
        <div class="sec-card">
            <div class="sec-hdr"><span class="sec-title">Report Management</span></div>
            <div class="toc-grid">
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
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:var(--red)"></span> Special Topics</div>
                    <a class="toc-link feat" href="https://oureports.net/oureports/DataImport.pdf">Data Import</a>
                    <a class="toc-link feat" href="https://oureports.net/oureports/AIandDataAI.pdf">AI and DataAI</a>
                </div>
            </div>
        </div>

        <!-- ── REPORT DATA DEFINITION ── -->
        <div class="sec-card">
            <div class="sec-hdr"><span class="sec-title">Report Data Definition</span></div>
            <div class="toc-grid c2">
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

        <!-- ── REPORT FORMAT DEFINITION ── -->
        <div class="sec-card">
            <div class="sec-hdr"><span class="sec-title">Report Format Definition</span></div>
            <div class="toc-grid">
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:var(--accent)"></span> <a href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=32">Columns &amp; Expressions</a></div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=32">Add Report Column</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=32">Friendly Name</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=33">Define Expression</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=33">Add Column</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=33">Change Column Order</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=33">Up or Down Links</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=33">Column Order Dialog</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=35">Edit Report Column</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=35">Delete Report Column</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=35">Update Report Format</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#2563EB"></span> <a href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=36">Groups &amp; Totals</a></div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=36">Add Group</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=37">Edit Group</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=37">Friendly Group Name</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=37">Change Totals Column</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=37">Change Group Order</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=37">Delete Group</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=37">Update Group</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#7C3AED"></span> <a href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=38">Combine Column Values</a></div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=38">Add Combined Column</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=40">Replace with Combined</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=41">Delete Combined Column</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=41">Delete New Column</a>
                    <a class="toc-link sub" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=41">Delete Replaced Column</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=41">Update Combined Column</a>
                </div>
            </div>
        </div>

        <!-- ── EXPLORE REPORT DATA ── -->
        <div class="sec-card">
            <div class="sec-hdr"><span class="sec-title">Explore Report Data</span></div>
            <div class="toc-grid">
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
                    <div class="cell-title"><span class="dot" style="background:#D97706"></span> Statistics</div>
                    <a class="toc-link feat" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=55">Export Statistics to Excel</a>
                    <a class="toc-link feat" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=54">Overall Statistics</a>
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

        <!-- ── STATISTICS, CORRELATION & TABLE EXPLORER ── -->
        <div class="sec-card">
            <div class="sec-hdr"><span class="sec-title">Statistics, Correlation &amp; Table Explorer</span></div>
            <div class="toc-grid">
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:#D97706"></span> Groups Statistics</div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=68">View Groups Statistics</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:var(--red)"></span> Fields Correlation</div>
                    <a class="toc-link feat" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=73">Analyze Field Correlation</a>
                </div>
                <div class="toc-cell" data-topic>
                    <div class="cell-title"><span class="dot" style="background:var(--accent)"></span> <a href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=74">Class Table Explorer</a></div>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=74">Link to Tables</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=75">Mouse Over Children</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=76">Parent Link</a>
                    <a class="toc-link" href="https://oureports.net/oureports/OnlineUserReporting.pdf#page=77">Sample</a>
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
    themeBtn.addEventListener('click', function () {
        document.body.classList.toggle('dark-theme');
        var isDark = document.body.classList.contains('dark-theme');
        themeBtn.setAttribute('title', isDark ? 'Switch to light mode' : 'Switch to dark mode');
        themeBtn.setAttribute('aria-label', isDark ? 'Switch to light mode' : 'Switch to dark mode');
    });

    (function () {
        var urlParams = new URLSearchParams(window.location.search);
        var hilt = urlParams.get("hilt");
        if (hilt) {
            document.querySelectorAll("a").forEach(function (link) {
                if (link.textContent.indexOf(hilt) !== -1) {
                    link.classList.add("hilt-yellow");
                    link.scrollIntoView({ behavior: "smooth", block: "center" });
                    link.focus();
                }
            });
        }
    })();

    function filterTopics(q) {
        q = q.toLowerCase().trim();
        document.querySelectorAll("[data-topic]").forEach(function (cell) {
            cell.style.display = (!q || cell.textContent.toLowerCase().indexOf(q) !== -1) ? "" : "none";
        });
    }
</script>
</body>
</html>
