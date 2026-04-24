<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ContactUs.aspx.vb" Inherits="ContactUs" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Contact Us - DataAI</title>
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
            --red: #CC0000;
            --border: #E2E0DB;
            --shadow-sm: 0 1px 3px rgba(0,0,0,0.04);
            --shadow-md: 0 4px 20px rgba(0,0,0,0.06);
            --radius: 14px;
            --radius-sm: 8px;
            --transition: 0.3s cubic-bezier(0.4, 0, 0.2, 1);
        }
        .dark-theme {
            --bg-primary: #0F0F0F; --bg-secondary: #1A1A1A; --bg-card: #1E1E1E;
            --accent: #3DD68C; --accent-light: #162B20; --accent-hover: #5AEAA5;
            --text-primary: #F0F0F0; --text-secondary: #A0A0A0; --text-muted: #666;
            --red: #EF5350; --border: #2A2A2A;
            --shadow-sm: 0 1px 3px rgba(0,0,0,0.2); --shadow-md: 0 4px 20px rgba(0,0,0,0.3);
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
        .hero { max-width: 580px; margin: 0 auto; padding: 2rem 2rem 0.75rem; text-align: center; }
        .hero h1 { font-family: 'Playfair Display', serif; font-size: clamp(1.8rem, 4vw, 2.4rem); font-weight: 700; color: var(--accent); margin-bottom: 0.4rem; }
        .hero-lead { font-size: 0.95rem; color: var(--text-secondary); }

        /* ── Form Card ── */
        .content { max-width: 580px; margin: 0 auto; padding: 1rem 2rem 2.5rem; }
        .form-card { background: var(--bg-card); border: 1px solid var(--border); border-radius: var(--radius); padding: 2rem 2.5rem; box-shadow: var(--shadow-sm); transition: box-shadow var(--transition); }
        .form-card:hover { box-shadow: var(--shadow-md); }

        /* ── Captcha row ── */
        .captcha-row { display: flex; align-items: center; gap: 0.5rem; margin-bottom: 1.25rem; padding: 0.75rem 1rem; background: var(--bg-secondary); border-radius: var(--radius-sm); }

        /* ── Form fields ── */
        .field-group { margin-bottom: 1rem; }
        .field-group label { display: block; font-size: 0.82rem; font-weight: 600; color: var(--text-secondary); margin-bottom: 0.3rem; text-transform: uppercase; letter-spacing: 0.04em; }
        .field-group label .req { color: var(--red); }

        /* asp:TextBox / asp:DropDownList overrides */
        .field-group input[type="text"],
        .field-group input[type="email"],
        .field-group textarea,
        .field-group select {
            width: 100% !important; padding: 0.6rem 1rem !important; border: 1px solid var(--border) !important;
            border-radius: var(--radius-sm) !important; font-family: 'DM Sans', sans-serif !important;
            font-size: 0.92rem !important; color: var(--text-primary) !important;
            background: var(--bg-primary) !important; transition: border-color var(--transition) !important;
            appearance: auto;
        }
        .field-group input:focus, .field-group textarea:focus, .field-group select:focus { outline: none !important; border-color: var(--accent) !important; }
        .dark-theme .field-group input, .dark-theme .field-group textarea, .dark-theme .field-group select { background: var(--bg-secondary) !important; color: var(--text-primary) !important; }
        .field-group textarea { min-height: 120px; resize: vertical; }

        /* asp:Button override */
        input[type="submit"][name*="btnSendEmail"] {
            display: inline-flex; align-items: center; justify-content: center;
            background: var(--accent); color: #fff; padding: 0.65rem 3rem; border-radius: var(--radius-sm);
            font-weight: 600; font-size: 0.95rem; border: none; cursor: pointer;
            font-family: 'DM Sans', sans-serif; transition: all var(--transition); width: 100%;
        }
        input[type="submit"][name*="btnSendEmail"]:hover { background: var(--accent-hover); transform: translateY(-1px); box-shadow: 0 4px 14px rgba(27,107,74,0.2); }
        .dark-theme input[type="submit"][name*="btnSendEmail"] { color: #0F0F0F; }

        /* ── Validation label ── */
        .validation-msg { text-align: center; margin-bottom: 0.75rem; min-height: 1.25rem; }

        /* ── Footer ── */
        .footer-area { text-align: center; margin-top: 1.5rem; }
        .seal-wrap { display: inline-block; }

        /* ── Home link ── */
        .home-link { font-size: 0.88rem; }

        @media (max-width: 600px) {
            .form-card { padding: 1.5rem; }
            .hero { padding: 1.5rem 1.25rem 0.5rem; }
            .content { padding: 0.75rem 1.25rem 2rem; }
        }
    </style>
</head>
<body>

<!-- ═══ TOP BAR ═══ -->
<div style="display:flex; justify-content:space-between; align-items:center; max-width:580px; margin:0.75rem auto 0; padding:0 2rem;">
    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/index1.aspx" Target="_top" CssClass="home-link">&#8592; Home</asp:HyperLink>
    <button class="theme-toggle" id="themeToggle" aria-label="Toggle dark mode" title="Switch to dark mode"></button>
</div>

<!-- ═══ HERO ═══ -->
<section class="hero">
    <h1>Contact Us</h1>
    <p class="hero-lead">Questions? Feel free to reach out. We'd love to hear from you.</p>
    <h1><asp:Label ID="LabelPageTtl" runat="server" Text="" style="font-size:0;"></asp:Label></h1>
</section>

<!-- ═══ FORM ═══ -->
<form id="form1" runat="server">
<div class="content">
    <div class="form-card">

        <!-- Validation message -->
        <div class="validation-msg">
            <asp:Label ID="Label1" runat="server" Text=" " ForeColor="#CC0000"></asp:Label>
        </div>

        <!-- Captcha -->
        <div class="captcha-row">
            <asp:CheckBox ID="chkme" runat="server" AutoPostBack="True" />
            <asp:Label ID="Label2" runat="server" Text="I'm " Font-Italic="True" Font-Bold="True" Font-Size="Medium" ForeColor="#CC0000"></asp:Label>
            <asp:Label ID="Label3" runat="server" Text=" not" Font-Size="Large" Font-Bold="True" Font-Underline="True" ForeColor="#66FF33"></asp:Label>
            <asp:Label ID="Label4" runat="server" Text=" a robot" Font-Size="Medium" Font-Bold="True" ForeColor="#0066FF" Font-Italic="True" Font-Names="Arial Rounded MT Bold"></asp:Label>
        </div>

        <!-- Topic -->
        <div class="field-group">
            <label>Topic <span class="req">*</span></label>
            <asp:DropDownList ID="ddTopics" runat="server" Width="100%">
                <asp:ListItem></asp:ListItem>
                <asp:ListItem Value="Question">Question</asp:ListItem>
                <asp:ListItem Value="Suggestion">Suggestion</asp:ListItem>
                <asp:ListItem Value="Proposal">Proposal</asp:ListItem>
            </asp:DropDownList>
        </div>

        <!-- Full Name -->
        <div class="field-group">
            <label>Your full name <span class="req">*</span></label>
            <asp:TextBox ID="txtName" runat="server" Width="100%"></asp:TextBox>
        </div>

        <!-- Email -->
        <div class="field-group">
            <label>Your email <span class="req">*</span></label>
            <asp:TextBox ID="txtEmail" runat="server" Width="100%"></asp:TextBox>
        </div>

        <!-- Subject -->
        <div class="field-group">
            <label>Subject <span class="req">*</span></label>
            <asp:TextBox ID="txtSubject" runat="server" Width="100%"></asp:TextBox>
        </div>

        <!-- Message -->
        <div class="field-group">
            <label>Message <span class="req">*</span></label>
            <asp:TextBox ID="txtBody" runat="server" Rows="3" TextMode="MultiLine" Width="100%"></asp:TextBox>
        </div>

        <!-- Send -->
        <div style="margin-top:1.25rem;">
            <asp:Button ID="btnSendEmail" runat="server" Text="Send" />
        </div>

    </div>

    <!-- GoDaddy Seal -->
    <div class="footer-area">
        <div class="seal-wrap">
            <span id="siteseal"><script async type="text/javascript" src="https://seal.godaddy.com/getSeal?sealID=SYhDKXg2IT7QXHzPEW7z7tmavANkr8vMDCiRmZvbKczmKBJ5Wj8eKl1EX00B"></script></span>
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
</script>
</body>
</html>
