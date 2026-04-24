<%@ Page Language="VB" AutoEventWireup="false" CodeFile="QuickStart.aspx.vb" Inherits="QuickStart" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Quick Start - DataAI</title>
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
            --red: #B71C1C;
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
            --red: #EF5350; --border: #2A2A2A;
            --shadow-sm: 0 1px 3px rgba(0,0,0,0.2); --shadow-md: 0 4px 20px rgba(0,0,0,0.3); --shadow-lg: 0 12px 40px rgba(0,0,0,0.4);
        }
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: 'DM Sans', sans-serif; background: var(--bg-primary); color: var(--text-primary); overflow-x: hidden; transition: background var(--transition), color var(--transition); }
        a { color: var(--accent); text-decoration: none; font-weight: 600; transition: color var(--transition); }
        a:hover { color: var(--accent-hover); }

        .theme-toggle { width: 42px; height: 24px; background: var(--border); border-radius: 999px; position: relative; cursor: pointer; border: none; transition: background var(--transition); }
        .theme-toggle::after { content: ''; width: 18px; height: 18px; background: var(--bg-card); border-radius: 50%; position: absolute; top: 3px; left: 3px; transition: transform var(--transition); box-shadow: 0 1px 3px rgba(0,0,0,0.15); }
        .dark-theme .theme-toggle { background: var(--accent); }
        .dark-theme .theme-toggle::after { transform: translateX(18px); }

        /* ── Hero ── */
        .hero { max-width: 720px; margin: 0 auto; padding: 2rem 2rem 1rem; text-align: center; }
        .hero h1 { font-family: 'Playfair Display', serif; font-size: clamp(1.8rem, 4vw, 2.6rem); font-weight: 700; color: var(--accent); margin-bottom: 0.5rem; }
        .hero-sub { font-size: 1rem; color: var(--accent); font-weight: 700; margin-bottom: 0.25rem; }
        .hero-lead { font-size: 0.95rem; line-height: 1.65; color: var(--text-secondary); max-width: 520px; margin: 0 auto; }

        /* ── Content ── */
        .content { max-width: 720px; margin: 0 auto; padding: 1rem 2rem 3rem; }

        /* ── Option Cards ── */
        .option-card { background: var(--bg-card); border: 1px solid var(--border); border-radius: var(--radius); padding: 2rem 2.5rem; margin-bottom: 1.25rem; box-shadow: var(--shadow-sm); transition: box-shadow var(--transition); position: relative; overflow: hidden; }
        .option-card::before { content: ''; position: absolute; top: 0; left: 0; right: 0; height: 3px; background: var(--accent); }
        .option-card:hover { box-shadow: var(--shadow-md); }
        .option-card h2 { font-family: 'Playfair Display', serif; font-size: 1.3rem; font-weight: 700; color: var(--accent); margin-bottom: 0.6rem; }
        .option-card p { font-size: 0.92rem; line-height: 1.7; color: var(--text-secondary); margin-bottom: 0.75rem; }
        .option-card .note { font-size: 0.84rem; color: var(--red); font-weight: 600; line-height: 1.5; }

        /* ── Email form row ── */
        .email-row { display: flex; align-items: center; gap: 0.75rem; margin: 1rem 0 0.5rem; flex-wrap: wrap; }
        .email-row label { font-size: 0.9rem; font-weight: 700; color: var(--accent); white-space: nowrap; }
        .email-row input[type="text"], .email-row input[type="email"] {
            flex: 1; min-width: 220px; padding: 0.6rem 1rem; border: 1px solid var(--border); border-radius: var(--radius-sm);
            font-family: 'DM Sans', sans-serif; font-size: 0.92rem; color: var(--text-primary);
            background: var(--bg-primary); transition: border-color var(--transition);
        }
        .email-row input:focus { outline: none; border-color: var(--accent); }
        .dark-theme .email-row input { background: var(--bg-secondary); color: var(--text-primary); }

        /* ── Buttons ── */
        .btn-accent { display: inline-flex; align-items: center; justify-content: center; gap: 0.4rem; background: var(--accent); color: #fff; padding: 0.6rem 1.75rem; border-radius: var(--radius-sm); font-weight: 600; font-size: 0.92rem; border: none; cursor: pointer; transition: all var(--transition); text-decoration: none; font-family: 'DM Sans', sans-serif; }
        .btn-accent:hover { background: var(--accent-hover); transform: translateY(-1px); box-shadow: 0 4px 14px rgba(27,107,74,0.2); color: #fff; }
        .dark-theme .btn-accent { color: #0F0F0F; }

        .btn-outline { display: inline-flex; align-items: center; gap: 0.4rem; background: transparent; color: var(--accent); padding: 0.55rem 1.5rem; border-radius: var(--radius-sm); font-weight: 600; font-size: 0.88rem; border: 2px solid var(--accent); cursor: pointer; transition: all var(--transition); text-decoration: none; font-family: 'DM Sans', sans-serif; }
        .btn-outline:hover { background: var(--accent-light); }

        .btn-link-styled { display: inline-flex; align-items: center; gap: 0.35rem; color: var(--accent); font-weight: 600; font-size: 0.9rem; transition: color var(--transition); }
        .btn-link-styled:hover { color: var(--accent-hover); }
        .btn-link-styled .arrow { transition: transform var(--transition); }
        .btn-link-styled:hover .arrow { transform: translateX(3px); }

        /* ── Disclaimer notice ── */
        .disclaimer-notice { background: #FFF8E1; border: 1px solid #FFE082; border-radius: var(--radius-sm); padding: 1rem 1.25rem; margin-top: 0.5rem; text-align: center; }
        .dark-theme .disclaimer-notice { background: #2A2518; border-color: #5C4B1F; }
        .disclaimer-notice span { font-size: 0.85rem; color: var(--text-secondary); }
        .disclaimer-notice a { font-size: 0.85rem; }

        /* ── Divider ── */
        .or-divider { display: flex; align-items: center; gap: 1rem; margin: 0.25rem 0; }
        .or-divider::before, .or-divider::after { content: ''; flex: 1; height: 1px; background: var(--border); }
        .or-divider span { font-size: 0.8rem; font-weight: 600; color: var(--text-muted); text-transform: uppercase; letter-spacing: 0.05em; }

        /* ── Footer area ── */
        .footer-area { text-align: center; padding: 1rem 0 0; }
        .footer-area .tagline { font-size: 0.9rem; font-weight: 600; font-style: italic; color: var(--text-muted); margin-bottom: 0.75rem; }
        .seal-wrap { display: inline-block; margin-top: 0.5rem; }

        /* ── Loading modal (UpdateProgress) ── */
        .modal-overlay { position: fixed; z-index: 2147483647; top: 0; left: 0; height: 100%; width: 100%; background: rgba(250,250,248,0.85); display: flex; align-items: center; justify-content: center; }
        .dark-theme .modal-overlay { background: rgba(15,15,15,0.85); }
        .modal-box { background: var(--bg-card); border: 1px solid var(--border); border-radius: var(--radius); padding: 2rem 2.5rem; box-shadow: var(--shadow-lg); text-align: center; font-size: 0.95rem; font-weight: 600; color: var(--text-secondary); }

        /* ── Hide LabelPageTtl ── */
        #LabelPageTtl { display: none; }

        /* ── ASP Button overrides ── */
        input[type="submit"][name*="btStart"] {
            display: inline-flex; align-items: center; justify-content: center;
            background: var(--accent); color: #fff; padding: 0.6rem 2.5rem; border-radius: var(--radius-sm);
            font-weight: 600; font-size: 0.95rem; border: none; cursor: pointer;
            font-family: 'DM Sans', sans-serif; transition: all var(--transition);
        }
        input[type="submit"][name*="btStart"]:hover { background: var(--accent-hover); transform: translateY(-1px); box-shadow: 0 4px 14px rgba(27,107,74,0.2); }
        .dark-theme input[type="submit"][name*="btStart"] { color: #0F0F0F; }

        input[type="submit"][name*="ButtonVideo"] {
            display: inline-flex; align-items: center; justify-content: center;
            background: transparent; color: var(--accent); padding: 0.55rem 1.5rem; border-radius: var(--radius-sm);
            font-weight: 600; font-size: 0.9rem; border: 2px solid var(--accent); cursor: pointer;
            font-family: 'DM Sans', sans-serif; transition: all var(--transition);
            width: auto !important; height: auto !important;
        }
        input[type="submit"][name*="ButtonVideo"]:hover { background: var(--accent-light); }

        /* ── asp:TextBox override ── */
        input[name*="txtEmail"] {
            flex: 1; min-width: 220px; padding: 0.6rem 1rem !important; border: 1px solid var(--border) !important;
            border-radius: var(--radius-sm) !important; font-family: 'DM Sans', sans-serif !important;
            font-size: 0.92rem !important; color: var(--text-primary) !important; background: var(--bg-primary) !important;
            transition: border-color var(--transition) !important; width: 100% !important; max-width: 360px;
        }
        input[name*="txtEmail"]:focus { outline: none !important; border-color: var(--accent) !important; }
        .dark-theme input[name*="txtEmail"] { background: var(--bg-secondary) !important; color: var(--text-primary) !important; }

        @media (max-width: 600px) {
            .option-card { padding: 1.5rem; }
            .hero { padding: 1.5rem 1.25rem 0.5rem; }
            .content { padding: 0.75rem 1.25rem 2rem; }
            .email-row { flex-direction: column; align-items: stretch; }
        }
    </style>
</head>
<body>
<div id="divreg">

<!-- ═══ TOGGLE ═══ -->
<div style="text-align:right; max-width:720px; margin:0.75rem auto 0; padding:0 2rem;">
    <button class="theme-toggle" id="themeToggle" aria-label="Toggle dark mode" title="Switch to dark mode"></button>
</div>

<!-- ═══ HERO ═══ -->
<section class="hero">
    <h1>Quick Access to DataAI</h1>
    <div class="hero-sub">Free for individual users and developers</div>
    <p class="hero-lead">Get started in seconds. Work with your data in memory or register with one click to use DataAI online.</p>
</section>

<!-- ═══ FORM ═══ -->
<form id="form1" runat="server">
<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
<asp:UpdatePanel ID="udpRegister" runat="server">
<ContentTemplate>

<!-- Hidden original title label (code-behind may reference it) -->
<div style="display:none;">
    <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting"></asp:Label>
</div>

<div class="content">

    <!-- ── Option 1: Work in Memory ── -->
    <div class="option-card">
        <h2>Work in Memory</h2>
        <p>Upload your data in JSON, XML, CSV, or XLS format. It will be analyzed in memory and will not be kept when DataAI pages are closed.</p>
        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/DataAIaddons.aspx" CssClass="btn-link-styled">
            Launch In-Memory Mode <span class="arrow">&#8594;</span>
        </asp:HyperLink>
        <div style="margin-top:0.5rem;">
            <asp:Label ID="Label2" runat="server" CssClass="note">Your data in JSON, XML, CSV, XLS format will be analyzed in memory and will not be kept when DataAI pages are closed.</asp:Label>
        </div>
    </div>

    <div class="or-divider"><span>or</span></div>

    <!-- ── Option 2: One-Click Registration ── -->
    <div class="option-card">
        <h2>One-Click Registration</h2>
        <p>Register with just your email to work with DataAI and our database online.</p>

        <table id="Registr" runat="server" border="0" cellpadding="0" cellspacing="0" style="width:100%;">
            <tr>
                <td>
                    <div class="email-row">
                        <label for="txtEmail">Email</label>
                        <asp:TextBox runat="server" ID="txtEmail" type="text" ValidateRequestMode="Enabled" TextMode="Email" />
                    </div>
                </td>
            </tr>
        </table>

        <div style="margin-top:1rem; text-align:center;">
            <asp:Button ID="btStart" runat="server" Text="Start" />
        </div>

        <div style="text-align:center; margin-top:0.75rem;">
            <asp:Label ID="Label1" runat="server" CssClass="note">First time password will be sent to your email. You will be asked to change it for the next log in.</asp:Label>
        </div>

        <div style="text-align:center; margin-top:0.25rem;">
            <asp:Label ID="LblInvalid" runat="server" CssClass="note"></asp:Label>
        </div>

        <div style="text-align:center; margin-top:1rem;">
            <span style="font-size:0.9rem; color:var(--text-secondary);">Already registered?</span>&nbsp;
            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx">Sign in</asp:HyperLink>
        </div>

        <div style="text-align:center; margin-top:1.25rem;">
            <asp:Button ID="ButtonVideo" runat="server" Text="Quick Start Video" OnClientClick="target='_blank'" ToolTip="Video Demo" />
        </div>
    </div>

    <!-- ── Disclaimer ── -->
    <div class="disclaimer-notice">
        <span>Please read <a href="disclaimer.htm">Disclaimer</a> and <a href="PrivacyPolicy.htm">Privacy Policy</a> first.</span>
    </div>

    <!-- ── Footer ── -->
    <div class="footer-area">
        <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="https://oureports.net/OURfront/index.html" CssClass="btn-link-styled">DataAI.link <span class="arrow">&#8594;</span></asp:HyperLink>
        <br /><br />
        <asp:Label runat="server" CssClass="tagline">Secure cloud-based report designer and data analysis</asp:Label>
        <br />
        <div class="seal-wrap">
            <span id="siteseal"><script async type="text/javascript" src="https://seal.godaddy.com/getSeal?sealID=SYhDKXg2IT7QXHzPEW7z7tmavANkr8vMDCiRmZvbKczmKBJ5Wj8eKl1EX00B"></script></span>
        </div>
    </div>

</div>

</ContentTemplate>
</asp:UpdatePanel>

<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpRegister">
    <ProgressTemplate>
        <div class="modal-overlay">
            <div class="modal-box">
                <asp:Image ID="imgProgress" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Controls/Images/WaitImage2.gif" />
                <br />Please wait...
            </div>
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>

</form>
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
    });
</script>
</body>
</html>
