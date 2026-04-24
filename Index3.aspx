<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Index3.aspx.vb" Inherits="Index3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>OUReports Service</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link href="https://fonts.googleapis.com/css2?family=DM+Sans:ital,opsz,wght@0,9..40,300;0,9..40,500;0,9..40,700;1,9..40,400&family=Playfair+Display:wght@600;700;800&display=swap" rel="stylesheet" />

    <style type="text/css">
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

        * { margin: 0; padding: 0; box-sizing: border-box; }

        body {
            font-family: 'DM Sans', sans-serif;
            background-color: var(--bg-primary);
            color: var(--text-primary);
            min-height: 100vh;
            overflow-x: hidden;
        }

        /* ── Subtle background texture ── */
        body::before {
            content: '';
            position: fixed;
            inset: 0;
            background:
                radial-gradient(ellipse at 15% 20%, rgba(27,107,74,0.04) 0%, transparent 50%),
                radial-gradient(ellipse at 85% 80%, rgba(27,107,74,0.03) 0%, transparent 50%);
            pointer-events: none;
            z-index: 0;
        }

        #form1 {
            position: relative;
            z-index: 1;
        }

        /* ── Top bar ── */
        .top-bar {
            max-width: 1100px;
            margin: 0 auto;
            padding: 1rem 2rem;
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .top-bar a,
        .top-bar a:visited {
            font-family: 'DM Sans', sans-serif;
            font-size: 0.88rem;
            font-weight: 600;
            color: var(--accent);
            text-decoration: none;
            padding: 0.45rem 1rem;
            border-radius: var(--radius-sm);
            transition: all var(--transition);
            display: inline-flex;
            align-items: center;
            gap: 0.4rem;
        }

        .top-bar a:hover {
            background: var(--accent-light);
            color: var(--accent-hover);
        }

        .top-bar a::before {
            content: '\2190';
            font-size: 1.1rem;
        }

        /* ── Page header ── */
        .page-header {
            text-align: center;
            padding: 2.5rem 2rem 1rem;
            max-width: 800px;
            margin: 0 auto;
        }

        .page-header h1 {
            font-family: 'Playfair Display', serif;
            font-size: clamp(1.8rem, 4vw, 2.6rem);
            font-weight: 700;
            color: var(--text-primary);
            letter-spacing: -1px;
            line-height: 1.2;
            margin-bottom: 0.6rem;
        }

        .page-header h1 em {
            font-style: italic;
            color: var(--accent);
        }

        .page-header .subtitle {
            font-size: 1.05rem;
            color: var(--text-secondary);
            line-height: 1.6;
        }

        /* ── Registration cards ── */
        .reg-grid {
            max-width: 1000px;
            margin: 2.5rem auto 3rem;
            padding: 0 2rem;
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 2rem;
        }

        .reg-card {
            background: var(--bg-card);
            border: 1px solid var(--border);
            border-radius: var(--radius);
            padding: 2.5rem 2rem 2rem;
            box-shadow: var(--shadow-sm);
            position: relative;
            overflow: hidden;
            transition: all var(--transition);
            display: flex;
            flex-direction: column;
            align-items: center;
            text-align: center;
        }

        .reg-card::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            height: 4px;
            background: var(--accent);
            transform: scaleX(0);
            transition: transform var(--transition);
        }

        .reg-card:hover {
            transform: translateY(-4px);
            box-shadow: var(--shadow-lg);
            border-color: var(--accent);
        }

        .reg-card:hover::before {
            transform: scaleX(1);
        }

        .card-icon {
            width: 64px;
            height: 64px;
            border-radius: 16px;
            background: var(--accent-light);
            display: flex;
            align-items: center;
            justify-content: center;
            margin-bottom: 1.25rem;
        }

        .card-icon svg {
            width: 28px;
            height: 28px;
            stroke: var(--accent);
            fill: none;
            stroke-width: 1.8;
        }

        .card-badge {
            display: inline-flex;
            align-items: center;
            gap: 0.4rem;
            background: var(--accent-light);
            color: var(--accent);
            font-size: 0.72rem;
            font-weight: 700;
            letter-spacing: 0.06em;
            text-transform: uppercase;
            padding: 0.3rem 0.85rem;
            border-radius: 999px;
            margin-bottom: 1rem;
        }

        .card-badge::before {
            content: '';
            width: 6px;
            height: 6px;
            background: var(--accent);
            border-radius: 50%;
        }

        .card-title {
            font-family: 'Playfair Display', serif;
            font-size: 1.35rem;
            font-weight: 700;
            color: var(--text-primary);
            margin-bottom: 0.5rem;
            letter-spacing: -0.5px;
        }

        .card-desc {
            font-size: 0.92rem;
            color: var(--text-secondary);
            line-height: 1.6;
            margin-bottom: 1.5rem;
            max-width: 340px;
        }

        /* ── ASP Buttons styling ── */
        .reg-card .btn-asp {
            background: var(--accent);
            color: #FFFFFF;
            font-family: 'DM Sans', sans-serif;
            font-weight: 600;
            font-size: 1rem;
            padding: 0.85rem 2rem;
            border: none;
            border-radius: var(--radius);
            cursor: pointer;
            transition: all var(--transition);
            width: 100%;
            max-width: 320px;
            letter-spacing: 0.01em;
        }

        .reg-card .btn-asp:hover {
            background: var(--accent-hover);
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(27,107,74,0.25);
        }

        /* ── Video links ── */
        .video-links {
            margin-top: 1.25rem;
            display: flex;
            flex-direction: column;
            gap: 0.45rem;
        }

        .video-links a,
        .video-links a:visited {
            font-family: 'DM Sans', sans-serif;
            font-size: 0.82rem;
            color: var(--text-muted);
            text-decoration: none;
            transition: color var(--transition);
            display: inline-flex;
            align-items: center;
            gap: 0.35rem;
        }

        .video-links a:hover {
            color: var(--accent);
        }

        .video-links a::before {
            content: '';
            display: inline-block;
            width: 0;
            height: 0;
            border-style: solid;
            border-width: 4px 0 4px 7px;
            border-color: transparent transparent transparent currentColor;
        }

        /* ── Footer area ── */
        .page-footer {
            text-align: center;
            padding: 0 2rem 3rem;
            max-width: 800px;
            margin: 0 auto;
        }

        .page-footer .tagline {
            font-family: 'DM Sans', sans-serif;
            font-size: 0.95rem;
            font-weight: 500;
            font-style: italic;
            color: var(--text-muted);
            margin-bottom: 1.5rem;
            letter-spacing: 0.01em;
        }

        .seal-wrap {
            display: inline-block;
            opacity: 0.7;
            transition: opacity var(--transition);
        }

        .seal-wrap:hover {
            opacity: 1;
        }

        /* ── Fade in animation ── */
        .fade-up {
            opacity: 0;
            transform: translateY(24px);
            animation: fadeUp 0.7s ease forwards;
        }

        .fade-up-d1 { animation-delay: 0.1s; }
        .fade-up-d2 { animation-delay: 0.2s; }
        .fade-up-d3 { animation-delay: 0.3s; }

        @keyframes fadeUp {
            to { opacity: 1; transform: translateY(0); }
        }

        /* ── Responsive ── */
        @media (max-width: 768px) {
            .reg-grid {
                grid-template-columns: 1fr;
                gap: 1.5rem;
            }

            .reg-card {
                padding: 2rem 1.5rem;
            }

            .page-header {
                padding: 2rem 1.5rem 0.5rem;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <!-- ── Top bar ── -->
        <div class="top-bar fade-up">
            <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/index1.aspx" Target="_top">Home</asp:HyperLink>
        </div>

        <!-- ── Page header ── -->
        <div class="page-header fade-up fade-up-d1">
            <h1><em><asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reports"></asp:Label></em></h1>
            <p class="subtitle">Choose the registration type that best fits your needs. Individual accounts are completely free for personal use and development.</p>
        </div>

        <!-- ── Registration cards ── -->
        <div class="reg-grid">

            <!-- Individual User Card -->
            <div class="reg-card fade-up fade-up-d2">
                <div class="card-icon">
                    <svg viewBox="0 0 24 24" stroke-linecap="round" stroke-linejoin="round">
                        <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/>
                        <circle cx="12" cy="7" r="4"/>
                    </svg>
                </div>
                <div class="card-badge">Free</div>
                <div class="card-title">Individual User</div>
                <p class="card-desc">Free for individual users and developers. Connect to your own database or explore with ours.</p>
                <asp:Button ID="Button2" runat="server" Text="Individual Registration" CssClass="btn-asp" />
                <div class="video-links">
                    <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/Videos/UserRegistrationVideo.mp4">Register to connect to your database</asp:HyperLink>
                    <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/Videos/RegOurDb.mp4">Register to use our database</asp:HyperLink>
                </div>
            </div>

            <!-- Company / Unit Card -->
            <div class="reg-card fade-up fade-up-d3">
                <div class="card-icon">
                    <svg viewBox="0 0 24 24" stroke-linecap="round" stroke-linejoin="round">
                        <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
                        <circle cx="9" cy="7" r="4"/>
                        <path d="M23 21v-2a4 4 0 0 0-3-3.87"/>
                        <path d="M16 3.13a4 4 0 0 1 0 7.75"/>
                    </svg>
                </div>
                <div class="card-badge">Teams</div>
                <div class="card-title">Company / Unit</div>
                <p class="card-desc">Multiple users sharing one database. Ideal for teams, departments, and organizations.</p>
                <asp:Button ID="Button1" runat="server" Text="Company Registration" CssClass="btn-asp" />
                <div class="video-links">
                    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Videos/UnitRegistrationVideo.mp4">Company/Unit registration walkthrough</asp:HyperLink>
                </div>
            </div>

        </div>

        <!-- ── Footer ── -->
        <div class="page-footer fade-up fade-up-d3">
            <asp:Label runat="server" CssClass="tagline">Secure cloud-based report designer and data analysis</asp:Label>
            <br /><br />
            <span id="siteseal" class="seal-wrap">
                <script async type="text/javascript" src="https://seal.godaddy.com/getSeal?sealID=SYhDKXg2IT7QXHzPEW7z7tmavANkr8vMDCiRmZvbKczmKBJ5Wj8eKl1EX00B"></script>
            </span>
        </div>

    </form>

    <!-- ── Fade-in observer ── -->
    <script>
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
    </script>
</body>
</html>
