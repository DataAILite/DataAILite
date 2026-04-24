<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AboutUs.aspx.vb" Inherits="AboutUs" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>About Us - DataAI</title>
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
        }
        .dark-theme {
            --bg-primary: #0F0F0F; --bg-secondary: #1A1A1A; --bg-card: #1E1E1E;
            --accent: #3DD68C; --accent-light: #162B20; --accent-hover: #5AEAA5;
            --text-primary: #F0F0F0; --text-secondary: #A0A0A0; --text-muted: #666;
            --border: #2A2A2A;
            --shadow-sm: 0 1px 3px rgba(0,0,0,0.2); --shadow-md: 0 4px 20px rgba(0,0,0,0.3); --shadow-lg: 0 12px 40px rgba(0,0,0,0.4);
        }
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: 'DM Sans', sans-serif; background: var(--bg-primary); color: var(--text-primary); overflow-x: hidden; transition: background var(--transition), color var(--transition); }
        a { color: var(--accent); text-decoration: none; font-weight: 600; transition: color var(--transition); }
        a:hover { color: var(--accent-hover); }

        /* ── Navbar ── */
        .top-nav { position: sticky; top: 0; z-index: 1000; background: var(--bg-primary); backdrop-filter: blur(12px); transition: background var(--transition); }
        .dark-theme .top-nav { background: rgba(15,15,15,0.92); }
        .nav-inner { max-width: 1320px; margin: 0 auto; padding: 0.5rem 2rem; display: flex; align-items: center; flex-wrap: wrap; gap: 0.15rem 0; }
        .nav-brand { font-family: 'Playfair Display', serif; font-weight: 800; font-size: 1.45rem; color: var(--accent); text-decoration: none; letter-spacing: -0.5px; flex-shrink: 0; }
        .nav-links { display: flex; align-items: center; gap: 0.15rem; margin-left: 2.5rem; list-style: none; flex-wrap: wrap; }
        .nav-links a, .nav-links .nav-dropdown > a { text-decoration: none; color: var(--accent); font-size: 0.88rem; font-weight: 700; font-style: italic; padding: 0.45rem 0.85rem; border-radius: var(--radius-sm); transition: all var(--transition); white-space: nowrap; }
        .nav-links a:hover, .nav-links .nav-dropdown > a:hover { color: var(--accent-hover); background: var(--accent-light); }
        .nav-dropdown { position: relative; }
        .nav-dropdown .dd-menu { display: none; position: absolute; top: calc(100% + 6px); left: 0; min-width: 240px; background: var(--bg-card); border: 1px solid var(--border); border-radius: var(--radius); box-shadow: var(--shadow-lg); padding: 0.5rem; z-index: 100; }
        .nav-dropdown:hover .dd-menu, .nav-dropdown.open .dd-menu { display: block; }
        .dd-menu a { display: block; padding: 0.55rem 0.85rem; font-size: 0.84rem; color: var(--text-secondary); border-radius: var(--radius-sm); font-weight: 500; font-style: normal; }
        .dd-menu a:hover { background: var(--accent-light); color: var(--accent); }
        .theme-toggle { margin-left: auto; flex-shrink: 0; width: 42px; height: 24px; background: var(--border); border-radius: 999px; position: relative; cursor: pointer; border: none; transition: background var(--transition); }
        .theme-toggle::after { content: ''; width: 18px; height: 18px; background: var(--bg-card); border-radius: 50%; position: absolute; top: 3px; left: 3px; transition: transform var(--transition); box-shadow: 0 1px 3px rgba(0,0,0,0.15); }
        .dark-theme .theme-toggle { background: var(--accent); }
        .dark-theme .theme-toggle::after { transform: translateX(18px); }

        /* ── Hero ── */
        .hero { max-width: 860px; margin: 0 auto; padding: 3rem 2rem 1.5rem; text-align: center; }
        .hero h1 { font-family: 'Playfair Display', serif; font-size: clamp(2rem, 4vw, 2.8rem); font-weight: 700; color: var(--accent); margin-bottom: 0.75rem; }
        .hero-lead { font-size: 1.05rem; line-height: 1.75; color: var(--text-secondary); max-width: 680px; margin: 0 auto 2rem; }

        /* ── Content Sections ── */
        .content { max-width: 860px; margin: 0 auto; padding: 0 2rem 3rem; }
        .section-card { background: var(--bg-card); border: 1px solid var(--border); border-radius: var(--radius); padding: 2rem 2.5rem; margin-bottom: 1.5rem; box-shadow: var(--shadow-sm); transition: box-shadow var(--transition); }
        .section-card:hover { box-shadow: var(--shadow-md); }
        .section-card h2 { font-family: 'Playfair Display', serif; font-size: 1.5rem; font-weight: 700; color: var(--accent); margin-bottom: 1rem; }
        .section-card p { font-size: 0.95rem; line-height: 1.75; color: var(--text-secondary); margin-bottom: 0.75rem; }
        .section-card p:last-child { margin-bottom: 0; }

        /* ── Features Grid ── */
        .features-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; margin: 1.5rem 0; }
        .feature-item { background: var(--bg-secondary); border-radius: var(--radius-sm); padding: 1.25rem 1.5rem; }
        .feature-item .num { font-family: 'Playfair Display', serif; font-size: 1.6rem; font-weight: 800; color: var(--accent); line-height: 1; margin-bottom: 0.4rem; }
        .feature-item h3 { font-size: 0.95rem; font-weight: 700; color: var(--text-primary); margin-bottom: 0.3rem; }
        .feature-item p { font-size: 0.85rem; line-height: 1.6; color: var(--text-secondary); margin: 0; }

        /* ── CTA ── */
        .cta-section { text-align: center; background: var(--accent-light); border-radius: var(--radius); padding: 2.5rem 2rem; margin-bottom: 1.5rem; }
        .cta-section h2 { font-family: 'Playfair Display', serif; font-size: 1.5rem; font-weight: 700; color: var(--accent); margin-bottom: 0.5rem; }
        .cta-section p { font-size: 0.95rem; color: var(--text-secondary); margin-bottom: 1.25rem; max-width: 520px; margin-left: auto; margin-right: auto; }
        .cta-btn { display: inline-flex; align-items: center; gap: 0.5rem; background: var(--accent); color: #fff; padding: 0.75rem 2rem; border-radius: var(--radius); font-weight: 600; font-size: 0.95rem; transition: all var(--transition); text-decoration: none; }
        .cta-btn:hover { background: var(--accent-hover); transform: translateY(-2px); box-shadow: 0 6px 20px rgba(27,107,74,0.25); color: #fff; }
        .dark-theme .cta-btn { color: #0F0F0F; }

        /* ── Team ── */
        .team-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 1.25rem; margin-top: 1.25rem; }
        .team-card { background: var(--bg-secondary); border-radius: var(--radius-sm); padding: 1.5rem; }
        .team-card h3 { font-size: 1rem; font-weight: 700; color: var(--text-primary); margin-bottom: 0.15rem; }
        .team-card .role { font-size: 0.82rem; font-weight: 600; color: var(--accent); margin-bottom: 0.6rem; }
        .team-card p { font-size: 0.85rem; line-height: 1.6; color: var(--text-secondary); margin: 0; }

        /* ── Responsive ── */
        @media (max-width: 768px) {
            .features-grid, .team-grid { grid-template-columns: 1fr; }
            .section-card { padding: 1.5rem; }
            .hero { padding: 2rem 1.25rem 1rem; }
            .content { padding: 0 1.25rem 2rem; }
        }
        @media (max-width: 992px) {
            .nav-links { margin-left: 0; flex-basis: 100%; gap: 0.1rem; padding-top: 0.4rem; }
        }
        @media (max-width: 600px) {
            .nav-inner { padding: 0.4rem 0.75rem; }
            .nav-links a, .nav-links .nav-dropdown > a { font-size: 0.78rem; padding: 0.35rem 0.55rem; }
        }
    </style>
</head>
<body>

<!-- ═══ PAGE ═══ -->
<div style="text-align:right; max-width:860px; margin:0.75rem auto 0; padding:0 2rem;">
    <button class="theme-toggle" id="themeToggle" aria-label="Toggle dark mode" title="Switch to dark mode"></button>
</div>

<!-- ═══ HERO ═══ -->
<section class="hero">
    <h1>About DataAI</h1>
    <p class="hero-lead">
        DataAI is an online report generator with analytical and artificial intelligence.
        It automatically analyzes data in your existing database or file, and generates
        reports, charts, maps, and dashboards, making data analytics convenient, simple,
        and accessible for everyone.
    </p>
</section>

<!-- ═══ CONTENT ═══ -->
<form id="form1" runat="server">
<div class="content">

    <!-- Why DataAI -->
    <div class="section-card">
        <h2>Why Choose DataAI?</h2>
        <div class="features-grid">
            <div class="feature-item">
                <div class="num">1</div>
                <h3>Seamless Integration</h3>
                <p>Effortlessly connect with your existing databases. DataAI supports SQL Server, MySQL, Oracle, PostgreSQL, InterSystems, ODBC, OleDb, and more.</p>
            </div>
            <div class="feature-item">
                <div class="num">2</div>
                <h3>User-Friendly Interface</h3>
                <p>Designed for both technical and non-technical users. Create, customize, and visualize reports with just a few clicks.</p>
            </div>
            <div class="feature-item">
                <div class="num">3</div>
                <h3>AI-Powered Insights</h3>
                <p>Harness artificial intelligence to automatically analyze your data, identify trends, and generate insights that drive smarter decisions.</p>
            </div>
            <div class="feature-item">
                <div class="num">4</div>
                <h3>Real-Time Reporting</h3>
                <p>Stay ahead with real-time data analytics. The most up-to-date information is at your fingertips, whenever you need it.</p>
            </div>
            <div class="feature-item">
                <div class="num">5</div>
                <h3>Customizable Dashboards</h3>
                <p>Tailor dashboards to your unique business needs. Create visually stunning, interactive reports with a comprehensive view of your data.</p>
            </div>
            <div class="feature-item">
                <div class="num">6</div>
                <h3>Secure &amp; Reliable</h3>
                <p>Your data is always protected with robust security measures. High reliability and performance so you can focus on your business.</p>
            </div>
            <div class="feature-item">
                <div class="num">7</div>
                <h3>Cost-Effective</h3>
                <p>Competitive pricing plans designed for businesses of all sizes. Top-tier analytics without breaking the bank.</p>
            </div>
            <div class="feature-item">
                <div class="num">8</div>
                <h3>Maps &amp; Geo Analytics</h3>
                <p>Generate KML maps from geo-coordinate data. Visualize your information on Google Earth and Google Maps.</p>
            </div>
        </div>
    </div>

    <!-- CTA -->
    <div class="cta-section">
        <h2>Join the DataAI Revolution</h2>
        <p>Empower your team with the tools they need to make data-driven decisions. Start your free trial today.</p>
        <a href="http://DataAI.link/" class="cta-btn">Visit DataAI.link</a>
    </div>

    <!-- Our Team -->
    <div class="section-card">
        <h2>Our Team</h2>
        <p>We are Yanbor LLC, a software development company focused on data analytics and AI-powered reporting.</p>
        <div class="team-grid">
            <div class="team-card">
                <h3>Irina Yaroshevskaya</h3>
                <div class="role">CEO &amp; Founder</div>
                <p>PhD in Computer Science with more than 20 years of experience in database design, management, and .NET programming. Former Computer Manager Principal at the University of Arizona, followed by nine years at Xerox and Conduent specializing in Care Management reporting systems.</p>
            </div>
            <div class="team-card">
                <h3>Fred Lepker</h3>
                <div class="role">Vice President of Development</div>
                <p>25 years of experience in software development, 19 of which are directly applicable to Care Management reporting systems with InterSystems Cache database and .NET programming.</p>
            </div>
        </div>
    </div>

</div>
</form>

<!-- ═══ SCRIPTS ═══ -->
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
