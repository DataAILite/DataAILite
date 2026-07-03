<%@ Page Language="VB" AutoEventWireup="false" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Sales - DataAI</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha1/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha1/dist/js/bootstrap.bundle.min.js"></script>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link href="https://fonts.googleapis.com/css2?family=DM+Sans:ital,opsz,wght@0,9..40,300;0,9..40,500;0,9..40,700;1,9..40,400&family=Playfair+Display:wght@600;700;800&display=swap" rel="stylesheet" />
    <style>
        :root {
            --bg-primary: #f7f7f4;
            --bg-secondary: #e9f1ef;
            --bg-card: #ffffff;
            --accent: #17624a;
            --accent-light: #e0f1ec;
            --accent-hover: #0f4936;
            --ink: #17201d;
            --text-secondary: #52605b;
            --text-muted: #7b8681;
            --border: #dce2df;
            --blue: #245f9d;
            --blue-soft: #e3edf7;
            --gold: #a06b12;
            --gold-soft: #f5ead6;
            --shadow-sm: 0 1px 3px rgba(0,0,0,0.04);
            --shadow-md: 0 8px 28px rgba(23,32,29,0.08);
            --radius: 8px;
            --transition: 0.3s cubic-bezier(0.4, 0, 0.2, 1);
        }

        .dark-theme {
            --bg-primary: #111513;
            --bg-secondary: #1b2421;
            --bg-card: #171c1a;
            --accent: #48d699;
            --accent-light: #183529;
            --accent-hover: #6ee3b1;
            --ink: #eef4f1;
            --text-secondary: #a9b7b1;
            --text-muted: #78847f;
            --border: #2c3834;
            --blue: #78aee8;
            --blue-soft: #16283a;
            --gold: #e4b45d;
            --gold-soft: #352915;
            --shadow-sm: 0 1px 3px rgba(0,0,0,0.2);
            --shadow-md: 0 8px 28px rgba(0,0,0,0.35);
        }

        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: 'DM Sans', sans-serif; background: var(--bg-primary); color: var(--ink); overflow-x: hidden; transition: background var(--transition), color var(--transition); }
        a { color: var(--accent); text-decoration: none; font-weight: 700; transition: color var(--transition), background var(--transition), transform var(--transition); }
        a:hover { color: var(--accent-hover); }

        .top-nav { position: sticky; top: 0; z-index: 1000; background: rgba(247,247,244,0.94); backdrop-filter: blur(12px); border-bottom: 1px solid var(--border); }
        .dark-theme .top-nav { background: rgba(17,21,19,0.94); }
        .nav-inner { max-width: 1320px; margin: 0 auto; padding: 0.5rem 2rem; display: flex; align-items: center; flex-wrap: wrap; gap: 0.15rem 0; }
        .nav-brand { font-family: 'Playfair Display', serif; font-weight: 800; font-size: 1.45rem; color: var(--accent); letter-spacing: -0.5px; flex-shrink: 0; }
        .nav-links { display: flex; align-items: center; gap: 0.15rem; margin-left: 2.5rem; list-style: none; flex-wrap: wrap; }
        .nav-links a, .nav-links .nav-dropdown > a { color: var(--accent); font-size: 0.88rem; font-weight: 700; font-style: italic; padding: 0.45rem 0.85rem; border-radius: var(--radius); white-space: nowrap; }
        .nav-links a:hover, .nav-links .nav-dropdown > a:hover { color: var(--accent-hover); background: var(--accent-light); }
        .nav-dropdown { position: relative; }
        .nav-dropdown .dd-menu { display: none; position: absolute; top: calc(100% + 6px); left: 0; min-width: 240px; background: var(--bg-card); border: 1px solid var(--border); border-radius: var(--radius); box-shadow: var(--shadow-md); padding: 0.5rem; z-index: 100; }
        .nav-dropdown:hover .dd-menu, .nav-dropdown.open .dd-menu { display: block; }
        .dd-menu a { display: block; padding: 0.55rem 0.85rem; font-size: 0.84rem; color: var(--text-secondary); border-radius: var(--radius); font-weight: 500; font-style: normal; }
        .dd-menu a:hover { background: var(--accent-light); color: var(--accent); }
        .theme-toggle { margin-left: auto; flex-shrink: 0; width: 42px; height: 24px; background: var(--border); border-radius: 999px; position: relative; cursor: pointer; border: none; }
        .theme-toggle::after { content: ''; width: 18px; height: 18px; background: var(--bg-card); border-radius: 50%; position: absolute; top: 3px; left: 3px; transition: transform var(--transition); box-shadow: 0 1px 3px rgba(0,0,0,0.15); }
        .dark-theme .theme-toggle { background: var(--accent); }
        .dark-theme .theme-toggle::after { transform: translateX(18px); }

        .hero { max-width: 1320px; margin: 0 auto; padding: 3rem 2rem 2rem; display: grid; grid-template-columns: minmax(0,1.08fr) minmax(320px,0.92fr); gap: 2.5rem; align-items: center; }
        .eyebrow { display: inline-flex; align-items: center; gap: 0.5rem; color: var(--blue); background: var(--blue-soft); border-radius: 999px; padding: 0.4rem 0.9rem; font-size: 0.78rem; font-weight: 800; letter-spacing: 0.03em; margin-bottom: 1.2rem; }
        .eyebrow::before { content: ''; width: 7px; height: 7px; border-radius: 50%; background: var(--blue); }
        h1 { font-family: 'Playfair Display', serif; font-size: clamp(2.35rem, 5vw, 4.1rem); line-height: 1.08; font-weight: 800; color: var(--accent); margin-bottom: 1rem; letter-spacing: 0; }
        h1 span { color: inherit; }
        .hero-lead { font-size: 1.06rem; color: var(--text-secondary); line-height: 1.75; max-width: 620px; margin-bottom: 1.5rem; }
        .hero-ctas { display: flex; gap: 0.85rem; flex-wrap: wrap; }
        .btn-main, .btn-secondary { display: inline-flex; align-items: center; justify-content: center; min-height: 44px; padding: 0.72rem 1.25rem; border-radius: var(--radius); font-weight: 800; }
        .btn-main { background: var(--accent); color: #fff; }
        .btn-main:hover { background: var(--accent-hover); color: #fff; transform: translateY(-2px); }
        .btn-secondary { background: var(--bg-card); color: var(--ink); border: 1px solid var(--border); }
        .btn-secondary:hover { color: var(--accent); border-color: var(--accent); transform: translateY(-2px); }
        .hero-panel { background: var(--bg-card); border: 1px solid var(--border); border-radius: var(--radius); box-shadow: var(--shadow-md); overflow: hidden; }
        .hero-panel-media { min-height: 330px; aspect-ratio: 4/3; background: linear-gradient(145deg, var(--accent-light), var(--bg-secondary)); display: flex; align-items: center; justify-content: center; position: relative; overflow: hidden; }
        .hero-panel-media::before { content: ''; position: absolute; inset: 0; background: radial-gradient(ellipse at 30% 20%, rgba(27,107,74,0.08), transparent 60%); }
        .hero-panel-media img { width: 88%; height: auto; max-height: 88%; object-fit: contain; filter: drop-shadow(0 4px 12px rgba(0,0,0,0.08)); position: relative; display: block; }
        .hero-panel-body { padding: 1.4rem; }
        .metric-row { display: grid; grid-template-columns: 1fr 1fr; gap: 0.8rem; margin-bottom: 1rem; }
        .metric { background: var(--bg-secondary); border-radius: var(--radius); padding: 0.9rem; }
        .metric strong { display: block; color: var(--accent); font-size: 1.25rem; line-height: 1.1; margin-bottom: 0.2rem; }
        .metric span { color: var(--text-secondary); font-size: 0.8rem; line-height: 1.35; display: block; }
        .panel-note { color: var(--text-secondary); line-height: 1.6; font-size: 0.92rem; }

        .content { max-width: 1320px; margin: 0 auto; padding: 1rem 2rem 3.5rem; }
        .section-title { max-width: 760px; margin-bottom: 1.2rem; }
        .section-title h2 { font-family: 'Playfair Display', serif; color: var(--accent); font-size: clamp(1.6rem, 3vw, 2.2rem); margin-bottom: 0.45rem; }
        .section-title p { color: var(--text-secondary); line-height: 1.7; }
        .grid-3 { display: grid; grid-template-columns: repeat(3, 1fr); gap: 1rem; margin-bottom: 2.5rem; }
        .info-card { background: var(--bg-card); border: 1px solid var(--border); border-radius: var(--radius); padding: 1.35rem; box-shadow: var(--shadow-sm); }
        .info-card h3 { font-size: 1rem; color: var(--ink); font-weight: 800; margin-bottom: 0.45rem; }
        .info-card p { color: var(--text-secondary); font-size: 0.9rem; line-height: 1.65; margin-bottom: 0; }
        .tag { display: inline-flex; color: var(--gold); background: var(--gold-soft); border-radius: 999px; padding: 0.25rem 0.65rem; font-size: 0.73rem; font-weight: 800; margin-bottom: 0.75rem; }
        .offer-band { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; margin-bottom: 2.5rem; }
        .offer { background: var(--bg-card); border: 1px solid var(--border); border-radius: var(--radius); padding: 1.4rem; display: grid; grid-template-columns: 96px 1fr; gap: 1rem; align-items: center; }
        .offer-icon { height: 96px; border-radius: var(--radius); background: var(--blue-soft); color: var(--blue); display: flex; align-items: center; justify-content: center; font-family: 'Playfair Display', serif; font-size: 2rem; font-weight: 800; }
        .offer h3 { font-size: 1rem; font-weight: 800; margin-bottom: 0.4rem; }
        .offer p { font-size: 0.9rem; color: var(--text-secondary); line-height: 1.6; margin: 0; }
        .cta-strip { background: var(--accent); color: #fff; border-radius: var(--radius); padding: 2rem; display: flex; align-items: center; justify-content: space-between; gap: 1.5rem; }
        .cta-strip h2 { font-family: 'Playfair Display', serif; font-size: 1.7rem; margin-bottom: 0.35rem; }
        .cta-strip p { margin: 0; color: rgba(255,255,255,0.86); line-height: 1.6; }
        .cta-strip a { background: #fff; color: var(--accent-hover); border-radius: var(--radius); padding: 0.75rem 1.2rem; white-space: nowrap; }
        .cta-strip a:hover { transform: translateY(-2px); color: var(--accent-hover); }
        .dark-theme .btn-main, .dark-theme .cta-strip { color: #07130e; }
        .dark-theme .cta-strip p { color: rgba(7,19,14,0.8); }

        @media (max-width: 992px) {
            .nav-links { margin-left: 0; flex-basis: 100%; gap: 0.1rem; padding-top: 0.4rem; }
            .hero { grid-template-columns: 1fr; padding: 2.5rem 1.5rem 1.5rem; }
            .grid-3, .offer-band { grid-template-columns: 1fr; }
            .content { padding-left: 1.5rem; padding-right: 1.5rem; }
        }
        @media (max-width: 640px) {
            .nav-inner { padding: 0.45rem 0.75rem; }
            .nav-links a, .nav-links .nav-dropdown > a { font-size: 0.78rem; padding: 0.35rem 0.55rem; }
            .hero { padding: 2rem 1rem 1rem; }
            .content { padding: 1rem 1rem 2.5rem; }
            .hero-panel-media { min-height: 240px; }
            .metric-row, .offer { grid-template-columns: 1fr; }
            .offer-icon { width: 96px; }
            .cta-strip { align-items: flex-start; flex-direction: column; padding: 1.4rem; }
            .cta-strip a { width: 100%; text-align: center; }
        }
    </style>
</head>
<body>
    <header class="top-nav">
        <div class="nav-inner">
            <a class="nav-brand" href="index.html">DataAI</a>
            <button class="theme-toggle" id="themeToggle" aria-label="Toggle dark mode" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Switch to dark mode"></button>
            <ul class="nav-links" id="navLinks">
                <li class="nav-dropdown">
                    <a href="#">About&ensp;&#9662;</a>
                    <div class="dd-menu">
                        <a href="DataAIOverview.html" target="_blank">DataAI Overview</a>
                        <a href="AboutUs.aspx" target="_blank">About Us</a>
                    </div>
                </li>
                <li><a href="Sales.aspx">Sales</a></li>
                <li><a href="Partners.pdf" target="_blank">Partners</a></li>
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
                    </div>
                </li>
                <li><a href="ContactUs.aspx">Contact Us</a></li>
                <li><a href="DataAIHelp.aspx" target="_blank">Guide</a></li>
                <li><a href="Default.aspx">Sign In</a></li>
            </ul>
        </div>
    </header>

    <form id="form1" runat="server">
        <section class="hero">
            <div>
                <div class="eyebrow">Sales enablement for DataAI</div>
                <h1>Sell data analytics that starts working <span>with the data customers already have.</span></h1>
                <p class="hero-lead">
                    DataAI gives organizations a practical path from database connection to reports, dashboards, charts, maps, statistical analysis, and AI-assisted summaries without building a custom reporting platform from scratch.
                </p>
                <div class="hero-ctas">
                    <a class="btn-main" href="UnitRegistration.aspx?org=company">Register a Company</a>
                    <a class="btn-secondary" href="ContactUs.aspx">Talk to Sales</a>
                    <a class="btn-secondary" href="ShowBusinessProposal.aspx" target="_blank">View Proposal</a>
                </div>
            </div>
            <aside class="hero-panel" aria-label="DataAI sales summary">
                <div class="hero-panel-media">
                    <img src="graph.PNG" alt="DataAI reporting and charting preview" />
                </div>
                <div class="hero-panel-body">
                    <div class="metric-row">
                        <div class="metric"><strong>1</strong><span>Connection string starts analysis.</span></div>
                        <div class="metric"><strong>Many</strong><span>Reports, charts, maps, dashboards, and models.</span></div>
                    </div>
                    <p class="panel-note">
                        Position DataAI for teams that need faster reporting, self-service analysis, and a deployable product they can run with their own SQL or non-SQL database.
                    </p>
                </div>
            </aside>
        </section>

        <main class="content">
            <section>
                <div class="section-title">
                    <h2>What Buyers Get</h2>
                    <p>DataAI is strongest when the buyer already has operational data but lacks a quick, repeatable way for staff to turn that data into useful reports and analytics.</p>
                </div>
                <div class="grid-3">
                    <div class="info-card">
                        <span class="tag">Speed</span>
                        <h3>Automatic report discovery</h3>
                        <p>DataAI analyzes database structure and creates initial reports and analytics, reducing the time between connection and useful output. DataAILite version works in memory only for more security.</p>
                    </div>
                    <div class="info-card">
                        <span class="tag">Adoption</span>
                        <h3>Tools for non-programmers</h3>
                        <p>End users can work with reports, charts, dashboards, and analytics without writing SQL or waiting on custom development.</p>
                    </div>
                    <div class="info-card">
                        <span class="tag">Control</span>
                        <h3>Deployment flexibility</h3>
                        <p>Customers can use DataAI as a hosted service or install the software on their own web server with source-code access. DataAI is open source and licensed under the GNU General Public License v3.0 (GPL v3). Windows 10/11 or Windows Server with IIS required.</p>
                    </div>
                </div>
            </section>

            <section>
                <div class="section-title">
                    <h2>Sales Paths</h2>
                    <p>Use these entry points depending on where the prospect is in the conversation.</p>
                </div>
                <div class="offer-band">
                    <div class="offer">
                        <div class="offer-icon">S</div>
                        <div>
                            <h3>Service conversation</h3>
                            <p>For teams that want DataAI configured and supported quickly. Start with the company registration path and validate database access, reporting goals, and users.</p>
                        </div>
                    </div>
                    <div class="offer">
                        <div class="offer-icon">P</div>
                        <div>
                            <h3>Software purchase conversation</h3>
                            <p>For organizations that need ownership, local hosting, integration, and source code. Lead with the formal proposal and software overview.</p>
                        </div>
                    </div>
                </div>
            </section>

            <section>
                <div class="section-title">
                    <h2>Qualifying Questions</h2>
                    <p>These help a sales conversation move quickly from interest to a useful next step.</p>
                </div>
                <div class="grid-3">
                    <div class="info-card">
                        <h3>Where is the data?</h3>
                        <p>Confirm whether the prospect uses SQL Server, MySQL, Oracle, PostgreSQL, InterSystems, ODBC, OleDb, CSV, Excel, or Access.</p>
                    </div>
                    <div class="info-card">
                        <h3>Who needs reports?</h3>
                        <p>Identify analysts, administrators, managers, and end users who need self-service reporting or repeatable dashboards.</p>
                    </div>
                    <div class="info-card">
                        <h3>What decision improves?</h3>
                        <p>Anchor the demo around a concrete outcome: faster reporting, better visibility, fewer manual extracts, or richer analytics.</p>
                    </div>
                </div>
            </section>

            <section class="cta-strip">
                <div>
                    <h2>Ready for a DataAI sales conversation?</h2>
                    <p>Register a company account, send a sales question, or open the proposal for a purchase-oriented discussion.</p>
                </div>
                <a href="ContactUs.aspx">Contact Sales</a>
            </section>
        </main>
    </form>

    <script>
        var themeBtn = document.getElementById('themeToggle');
        themeBtn.addEventListener('click', function () {
            document.body.classList.toggle('dark-theme');
            var isDark = document.body.classList.contains('dark-theme');
            var newTitle = isDark ? 'Switch to light mode' : 'Switch to dark mode';
            themeBtn.setAttribute('title', newTitle);
            themeBtn.setAttribute('aria-label', newTitle);
        });

        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (el) { return new bootstrap.Tooltip(el); });
    </script>
</body>
</html>
