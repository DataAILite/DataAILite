<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ProductionCustomer.aspx.vb" Inherits="ProductionCustomer" %>

<script type="text/javascript" src="Controls/Javascripts/OUR.js"></script>

<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>DataAI ETL — Production License Registration</title>
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
        body { font-family: 'DM Sans', sans-serif; background-color: var(--bg-primary); color: var(--text-primary); overflow-x: hidden; transition: background-color var(--transition), color var(--transition); }

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
        .page-header h1 { font-family: 'Playfair Display', serif; font-size: clamp(1.5rem, 3vw, 2rem); font-weight: 700; color: var(--accent); margin-bottom: 0.4rem; }
        .page-header .subtitle { font-size: 0.95rem; font-weight: 600; color: var(--text-secondary); margin-bottom: 0.5rem; }
        .disclaimer-notice { display: inline-block; background: #FFF9C4; border: 1px solid #F9E04B; border-radius: var(--radius-sm); padding: 0.35rem 1rem; font-size: 0.8rem; color: #6D4C00; margin-bottom: 0.25rem; }
        .dark-theme .disclaimer-notice { background: #3E2723; border-color: #5D4037; color: #FFCC80; }
        .disclaimer-notice a { color: #1565C0; font-weight: 600; text-decoration: underline; }
        .dark-theme .disclaimer-notice a { color: #64B5F6; }

        /* ── Registration Card ── */
        .reg-card { max-width: 720px; margin: 0.75rem auto 2rem; background: var(--bg-card); border: 1px solid var(--border); border-radius: var(--radius); padding: 1.5rem 2rem 1.25rem; box-shadow: var(--shadow-md); }
        .reg-card .error-msg { text-align: center; margin-bottom: 0.75rem; }

        /* Form rows */
        .form-row { display: flex; align-items: flex-start; gap: 0.75rem; margin-bottom: 0.65rem; }
        .form-row .label-col { width: 210px; flex-shrink: 0; text-align: right; font-size: 0.82rem; font-weight: 600; padding-top: 0.45rem; }
        .form-row .label-col.required { color: var(--red); }
        .form-row .label-col.optional { color: var(--text-muted); }
        .form-row .input-col { flex: 1; }
        .asp-input { width: 100%; padding: 0.4rem 0.7rem; font-family: 'DM Sans', sans-serif; font-size: 0.85rem; background: var(--bg-primary); color: var(--text-primary); border: 1px solid var(--border); border-radius: var(--radius-sm); transition: border-color var(--transition), box-shadow var(--transition); outline: none; }
        .asp-input:focus { border-color: var(--accent); box-shadow: 0 0 0 3px var(--accent-light); }
        .asp-textarea { resize: vertical; min-height: 80px; }

        .captcha-row { display: flex; align-items: center; gap: 0.5rem; margin-bottom: 1rem; padding: 0.75rem 1rem; background: var(--bg-secondary); border-radius: var(--radius-sm); }

        /* Submit button */
        .reg-submit { text-align: center; margin-top: 1rem; }
        .asp-btn-submit { padding: 0.6rem 2.2rem; font-family: 'DM Sans', sans-serif; font-size: 0.92rem; font-weight: 600; background: var(--accent); color: #fff; border: none; border-radius: var(--radius); cursor: pointer; transition: all var(--transition); box-shadow: 0 4px 14px rgba(27,107,74,0.2); }
        .asp-btn-submit:hover { background: var(--accent-hover); transform: translateY(-2px); box-shadow: 0 6px 20px rgba(27,107,74,0.3); }
        .dark-theme .asp-btn-submit { color: #0F0F0F; }

        /* Success panel */
        .success-panel { max-width: 720px; margin: 1rem auto 2rem; background: var(--bg-card); border: 1px solid var(--border); border-radius: var(--radius); padding: 2rem; box-shadow: var(--shadow-md); text-align: center; }
        .success-panel h2 { color: var(--accent); font-family: 'Playfair Display', serif; margin-bottom: 0.75rem; }
        .success-panel p { color: var(--text-secondary); font-size: 0.95rem; }

        /* ── Loading Modal ── */
        .modal-overlay { position: fixed; z-index: 9999; inset: 0; background-color: rgba(0,0,0,0.4); backdrop-filter: blur(4px); }
        .modal-center { position: absolute; top: 50%; left: 50%; transform: translate(-50%,-50%); background: var(--bg-card); border-radius: var(--radius); padding: 2rem 2.5rem; text-align: center; box-shadow: var(--shadow-lg); font-weight: 600; color: var(--text-primary); }

        /* ── Responsive ── */
        @media (max-width: 992px) { .nav-inner { padding: 0.6rem 1.5rem; } .nav-links { margin-left: 0; flex-basis: 100%; gap: 0.1rem; padding-top: 0.4rem; } }
        @media (max-width: 768px) { .form-row { flex-direction: column; align-items: flex-start; gap: 0.3rem; } .form-row .label-col { width: auto; text-align: left; padding-top: 0; } .form-check-row { padding-left: 0; } .reg-card { padding: 1.25rem 1rem; margin: 1rem 1rem 2rem; } }
    </style>
</head>
<body>

<!-- ═══ NAVBAR ═══ -->
<header class="top-nav">
    <div class="nav-inner">
        <a class="nav-brand" href="index1.aspx">DataAI</a>
        <button class="theme-toggle" id="themeToggle" aria-label="Toggle dark mode"></button>
        <ul class="nav-links">
            <li class="nav-dropdown">
                <a href="#">About&ensp;&#9662;</a>
                <div class="dd-menu">
                    <a href="https://oureports.net/OUReports/DataAIOverview.html" target="_blank">DataAI Overview</a>
                    <a href="AboutUs.aspx" target="_blank">About Us</a>
                </div>
            </li>
            <li><a href="https://oureports.net/OUReports/Partners.pdf" target="_blank">Partners</a></li>
            <li class="nav-dropdown">
                <a href="#">Products&ensp;&#9662;</a>
                <div class="dd-menu">
                    <a href="Index3.aspx">Services</a>
                    <a href="IndexSoftware.aspx">Software</a>
                    <a href="DataAIETLmarkets.html">DataAI ETL</a>
                </div>
            </li>
            <li class="nav-dropdown">
                <a href="#">Customers&ensp;&#9662;</a>
                <div class="dd-menu">
                    <a href="Registration.aspx">Individual</a>
                    <a href="UnitRegistration.aspx?org=company">Company</a>
                    <a href="ProductionCustomer.aspx">DataAI ETL Production</a>
                </div>
            </li>
            <li><a href="ContactUs.aspx">Contact Us</a></li>
            <li><a href="Default.aspx">Sign In</a></li>
        </ul>
    </div>
</header>

<!-- ═══ PAGE CONTENT ═══ -->
<div id="divPage">
    <form id="frmProdCust" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <asp:UpdatePanel ID="udpProdCust" runat="server">
            <ContentTemplate>

                <!-- Page header -->
                <div class="page-header">
                    <h1>DataAI ETL &mdash; Production License Registration</h1>
                    <p class="subtitle">Register your organization for a production DataAI ETL commercial agreement</p>
                    <div class="disclaimer-notice">
                        Please read <a href="disclaimer.htm">Disclaimer</a>
                        and <a href="PrivacyPolicy.htm">Privacy Policy</a> first.
                    </div>
                </div>

                <!-- Success panel (shown after submit) -->
                <asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="success-panel">
                    <h2>Thank you!</h2>
                    <p>Your production license registration has been received. A Yanbor LLC representative will contact you shortly.</p>
                    <p style="margin-top:1rem;">
                        <asp:HyperLink ID="lnkLicense" runat="server" Target="_blank" style="font-size:0.95rem;">
                            View your DataAI ETL commercial license sample &rarr;
                        </asp:HyperLink>
                    </p>
                    <p style="margin-top:0.5rem;"><a href="DataAIETLmarkets.html">Return to DataAI ETL markets</a></p>
                </asp:Panel>

                <!-- Registration form card -->
                <asp:Panel ID="pnlForm" runat="server" Visible="true">
                <div class="reg-card">

                    <div class="error-msg">
                        <asp:Label ID="LblMessage" runat="server" Text="" style="font-weight:600; font-size:0.9rem;"></asp:Label>
                    </div>

                    <!-- I'm not a robot -->
                    <div class="captcha-row">
                        <asp:CheckBox ID="chkme" runat="server" AutoPostBack="True" />
                        <asp:Label ID="Label2" runat="server" Text="I'm " Font-Italic="True" Font-Bold="True" Font-Size="Medium" ForeColor="#CC0000"></asp:Label>
                        <asp:Label ID="Label3" runat="server" Text=" not" Font-Size="Large" Font-Bold="True" Font-Underline="True" ForeColor="#66FF33"></asp:Label>
                        <asp:Label ID="Label4" runat="server" Text=" a robot" Font-Size="Medium" Font-Bold="True" ForeColor="#0066FF" Font-Italic="True" Font-Names="Arial Rounded MT Bold"></asp:Label>
                    </div>

                    <!-- Marketplace product -->
                    <div class="form-row">
                        <div class="label-col required">Marketplace product*:</div>
                        <div class="input-col">
                            <asp:DropDownList ID="ddProduct" runat="server" CssClass="asp-input" style="width:auto;">
                                <asp:ListItem Value="">Select a product</asp:ListItem>
                                <asp:ListItem Value="AWS">AWS (Glue / EMR)</asp:ListItem>
                                <asp:ListItem Value="Alteryx">Alteryx</asp:ListItem>
                                <asp:ListItem Value="Databricks">Databricks</asp:ListItem>
                                <asp:ListItem Value="DotNet">.NET pipelines</asp:ListItem>
                                <asp:ListItem Value="Google">Google Cloud (Dataproc)</asp:ListItem>
                                <asp:ListItem Value="IRIS">InterSystems IRIS</asp:ListItem>
                                <asp:ListItem Value="Oracle">Oracle AIDP Spark</asp:ListItem>
                                <asp:ListItem Value="PowerBI">Power BI</asp:ListItem>
                                <asp:ListItem Value="SSIS">SSIS</asp:ListItem>
                                <asp:ListItem Value="Tableau">Tableau</asp:ListItem>
                                <asp:ListItem Value="Talend">Talend / MuleSoft</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <!-- Company name -->
                    <div class="form-row">
                        <div class="label-col required">Company name*:</div>
                        <div class="input-col">
                            <asp:TextBox ID="txtUnit" runat="server" CssClass="asp-input" MaxLength="200" />
                        </div>
                    </div>

                    <!-- Contact name & title -->
                    <div class="form-row">
                        <div class="label-col required">Contact name &amp; title*:</div>
                        <div class="input-col">
                            <asp:TextBox ID="txtOfficial" runat="server" CssClass="asp-input" MaxLength="200" />
                        </div>
                    </div>

                    <!-- Email -->
                    <div class="form-row">
                        <div class="label-col required">Email*:</div>
                        <div class="input-col">
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="asp-input" MaxLength="200" />
                        </div>
                    </div>

                    <!-- Phone -->
                    <div class="form-row">
                        <div class="label-col required">Phone*:</div>
                        <div class="input-col">
                            <asp:TextBox ID="txtPhone" runat="server" CssClass="asp-input" MaxLength="100" />
                        </div>
                    </div>

                    <!-- Address -->
                    <div class="form-row">
                        <div class="label-col required">Address*:</div>
                        <div class="input-col">
                            <asp:TextBox ID="txtAddress" runat="server" CssClass="asp-input" MaxLength="1000" />
                        </div>
                    </div>

                    <!-- Company website -->
                    <div class="form-row">
                        <div class="label-col optional">Company website:</div>
                        <div class="input-col">
                            <asp:TextBox ID="txtUnitWeb" runat="server" CssClass="asp-input" MaxLength="500" />
                        </div>
                    </div>

                    <!-- Notes -->
                    <div class="form-row">
                        <div class="label-col optional">Notes / message:</div>
                        <div class="input-col">
                            <asp:TextBox ID="txtComments" runat="server" CssClass="asp-input asp-textarea"
                                TextMode="MultiLine" Rows="4" MaxLength="2000" />
                        </div>
                    </div>

                    <!-- Submit -->
                    <div class="reg-submit">
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit Registration"
                            CssClass="asp-btn-submit" Enabled="false" Visible="false" />
                    </div>

                </div>
                </asp:Panel>

            </ContentTemplate>
        </asp:UpdatePanel>

        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpProdCust">
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
</div>

<!-- ═══ SCRIPTS ═══ -->
<script>
    // Theme toggle
    var themeBtn = document.getElementById('themeToggle');
    themeBtn.addEventListener('click', function () {
        document.body.classList.toggle('dark-theme');
    });

    // Touch-friendly dropdown toggles
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
</script>
</body>
</html>
