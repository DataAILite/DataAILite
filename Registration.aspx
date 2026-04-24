<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Registration.aspx.vb" Inherits="Registration" %>

<script type="text/javascript" src="Controls/Javascripts/OUR.js"></script>

<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>DataAI - User Registration</title>
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
        .top-nav {
            position: sticky; top: 0; z-index: 1000;
            background: var(--bg-primary);
            backdrop-filter: blur(12px);
            transition: background var(--transition);
        }
        .dark-theme .top-nav { background: rgba(15,15,15,0.92); }

        .nav-inner {
            max-width: 1320px; margin: 0 auto;
            padding: 0.4rem 2rem;
            display: flex; align-items: center; flex-wrap: wrap; gap: 0.15rem 0;
        }
        .nav-brand {
            font-family: 'Playfair Display', serif; font-weight: 800; font-size: 1.45rem;
            color: var(--accent); text-decoration: none; letter-spacing: -0.5px; flex-shrink: 0;
        }
        .nav-links {
            display: flex; align-items: center; gap: 0.15rem;
            margin-left: 2.5rem; list-style: none; flex-wrap: wrap;
        }
        .nav-links a, .nav-links .nav-dropdown > a {
            text-decoration: none; color: var(--accent); font-size: 0.88rem;
            font-weight: 700; font-style: italic; padding: 0.45rem 0.85rem;
            border-radius: var(--radius-sm); transition: all var(--transition); white-space: nowrap;
        }
        .nav-links a:hover, .nav-links .nav-dropdown > a:hover {
            color: var(--accent-hover); background: var(--accent-light);
        }
        .nav-dropdown { position: relative; }
        .nav-dropdown .dd-menu {
            display: none; position: absolute; top: calc(100% + 6px); left: 0;
            min-width: 240px; background: var(--bg-card); border: 1px solid var(--border);
            border-radius: var(--radius); box-shadow: var(--shadow-lg); padding: 0.5rem; z-index: 100;
        }
        .nav-dropdown:hover .dd-menu, .nav-dropdown.open .dd-menu { display: block; }
        .dd-menu a {
            display: block; padding: 0.55rem 0.85rem; font-size: 0.84rem;
            color: var(--text-secondary); border-radius: var(--radius-sm);
        }
        .dd-menu a:hover { background: var(--accent-light); color: var(--accent); }

        .theme-toggle {
            margin-left: auto; flex-shrink: 0; width: 42px; height: 24px;
            background: var(--border); border-radius: 999px; position: relative;
            cursor: pointer; border: none; transition: background var(--transition);
        }
        .theme-toggle::after {
            content: ''; width: 18px; height: 18px; background: var(--bg-card);
            border-radius: 50%; position: absolute; top: 3px; left: 3px;
            transition: transform var(--transition); box-shadow: 0 1px 3px rgba(0,0,0,0.15);
        }
        .dark-theme .theme-toggle { background: var(--accent); }
        .dark-theme .theme-toggle::after { transform: translateX(18px); }

        /* ── Page Header ── */
        .page-header {
            max-width: 1320px; margin: 0 auto;
            padding: 1rem 2rem 0; text-align: center;
        }
        .page-header-bar {
            background: var(--bg-secondary); border-radius: var(--radius);
            padding: 0.4rem 1.2rem; display: inline-block; margin-bottom: 0.75rem;
        }
        .page-header-bar span { font-size: 0.9rem; font-weight: 700; color: var(--text-primary); }
        .page-header h1 {
            font-family: 'Playfair Display', serif; font-size: clamp(1.5rem, 3vw, 2rem);
            font-weight: 700; color: var(--accent); margin-bottom: 0.5rem;
        }
        .page-header .subtitle {
            font-size: 0.95rem; font-weight: 600; color: var(--text-secondary); margin-bottom: 0.4rem;
        }

        /* ── Disclaimer notice ── */
        .disclaimer-notice {
            display: inline-block; background: #FFF9C4; border: 1px solid #F9E04B;
            border-radius: var(--radius-sm); padding: 0.35rem 1rem;
            font-size: 0.8rem; color: #6D4C00; margin-bottom: 0.25rem;
        }
        .dark-theme .disclaimer-notice {
            background: #3E2723; border-color: #5D4037; color: #FFCC80;
        }
        .disclaimer-notice a { color: #1565C0; font-weight: 600; text-decoration: underline; }
        .dark-theme .disclaimer-notice a { color: #64B5F6; }

        .seal-wrap { margin: 0.25rem 0; }

        /* ── Registration Card ── */
        .reg-card {
            max-width: 720px; margin: 0.75rem auto 1.5rem;
            background: var(--bg-card); border: 1px solid var(--border);
            border-radius: var(--radius); padding: 1.25rem 2rem 1rem;
            box-shadow: var(--shadow-md);
        }
        .reg-card .login-link {
            text-align: center; margin-bottom: 0.75rem;
            font-size: 0.88rem; color: var(--text-secondary);
        }
        .reg-card .login-link a { color: var(--accent); font-weight: 600; }

        .reg-card .error-msg { text-align: center; margin-bottom: 0.5rem; }

        /* Form rows */
        .form-row {
            display: flex; align-items: center; gap: 0.75rem;
            margin-bottom: 0.6rem;
        }
        .form-row .label-col {
            width: 200px; flex-shrink: 0; text-align: right;
            font-size: 0.82rem; font-weight: 600;
        }
        .form-row .label-col.required { color: var(--red); }
        .form-row .label-col.optional { color: var(--text-muted); }
        .form-row .input-col { flex: 1; }

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
        .form-row input:focus, .form-row textarea:focus {
            border-color: var(--accent);
            box-shadow: 0 0 0 3px var(--accent-light);
        }

        /* Style ASP.NET TextBox and DropDown to match */
        .reg-card .asp-input {
            width: 100%; padding: 0.4rem 0.7rem;
            font-family: 'DM Sans', sans-serif; font-size: 0.85rem;
            background: var(--bg-primary); color: var(--text-primary);
            border: 1px solid var(--border); border-radius: var(--radius-sm);
            transition: border-color var(--transition), box-shadow var(--transition);
            outline: none;
        }
        .reg-card .asp-input:focus {
            border-color: var(--accent);
            box-shadow: 0 0 0 3px var(--accent-light);
        }
        .reg-card .asp-input-sm { width: 180px; }
        .reg-card .asp-input-conn { width: 100%; }

        .form-hint {
            font-size: 0.78rem; color: var(--text-muted);
            margin-top: 0.3rem; line-height: 1.5;
        }

        .form-check-row {
            display: flex; align-items: flex-start; gap: 0.5rem;
            margin-bottom: 0.6rem; padding-left: 200px;
        }
        .form-check-row a { font-size: 0.82rem; }

        /* Browse button */
        .reg-card .asp-btn-browse {
            padding: 0.5rem 1rem; font-family: 'DM Sans', sans-serif;
            font-size: 0.82rem; font-weight: 600;
            background: var(--bg-secondary); color: var(--text-primary);
            border: 1px solid var(--border); border-radius: var(--radius-sm);
            cursor: pointer; transition: all var(--transition);
        }
        .reg-card .asp-btn-browse:hover {
            background: var(--accent-light); border-color: var(--accent);
        }

        /* Register button */
        .reg-submit { text-align: center; margin-top: 1rem; }
        .reg-card .asp-btn-register {
            padding: 0.6rem 2rem;
            font-family: 'DM Sans', sans-serif; font-size: 0.92rem; font-weight: 600;
            background: var(--accent); color: #fff;
            border: none; border-radius: var(--radius); cursor: pointer;
            transition: all var(--transition);
            box-shadow: 0 4px 14px rgba(27,107,74,0.2);
        }
        .reg-card .asp-btn-register:hover {
            background: var(--accent-hover); transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(27,107,74,0.3);
        }
        .dark-theme .asp-btn-register { color: #0F0F0F; }

        /* ── Loading Modal ── */
        .modal-overlay {
            position: fixed; z-index: 9999; inset: 0;
            background-color: rgba(0,0,0,0.4);
            backdrop-filter: blur(4px);
        }
        .modal-center {
            position: absolute; top: 50%; left: 50%; transform: translate(-50%,-50%);
            background: var(--bg-card); border-radius: var(--radius);
            padding: 2rem 2.5rem; text-align: center;
            box-shadow: var(--shadow-lg); font-weight: 600; color: var(--text-primary);
        }

        /* ── Animations ── */
        .fade-up {
            opacity: 0; transform: translateY(30px);
            animation: fadeUp 0.7s ease forwards;
        }
        .fade-up-d1 { animation-delay: 0.1s; }
        .fade-up-d2 { animation-delay: 0.2s; }
        @keyframes fadeUp { to { opacity: 1; transform: translateY(0); } }

        /* ── Responsive ── */
        @media (max-width: 992px) {
            .nav-inner { padding: 0.6rem 1.5rem; }
            .nav-links { margin-left: 0; flex-basis: 100%; gap: 0.1rem; padding-top: 0.4rem; }
            .nav-dropdown .dd-menu { left: 0; min-width: 220px; }
        }
        @media (max-width: 768px) {
            .form-row { flex-direction: column; align-items: flex-start; gap: 0.3rem; }
            .form-row .label-col { width: auto; text-align: left; }
            .form-check-row { padding-left: 0; }
            .reg-card { padding: 1.5rem 1.25rem; margin: 1.5rem 1rem 2rem; }
        }
        @media (max-width: 600px) {
            .nav-inner { padding: 0.5rem 0.75rem; }
            .nav-links a, .nav-links .nav-dropdown > a { font-size: 0.78rem; padding: 0.35rem 0.55rem; }
            .page-header { padding: 2rem 1rem 0; }
            .page-header h1 { font-size: 1.8rem; }
        }
    </style>
</head>
<body>

<!-- ═══ NAVBAR ═══ -->
<header class="top-nav">
    <div class="nav-inner">
        <a class="nav-brand" href="index1.aspx">DataAI</a>
        <button class="theme-toggle" id="themeToggle" aria-label="Toggle dark mode" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Switch to dark mode"></button>
        <ul class="nav-links">
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
                    <a href="https://oureports.net/OUReports/AIandDataAI.pdf" target="_blank">AI and DataAI</a>
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
<div id="divreg">
    <form id="frmLogon" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <asp:UpdatePanel ID="udpRegister" runat="server">
            <ContentTemplate>

                <!-- Page header -->
                <div class="page-header">
                    <asp:Label ID="LabelPageTtl" runat="server" Text="Online User Reporting" Visible="false"></asp:Label>
                    <h1>User Registration</h1>
                    <p class="subtitle">Free for individual user and developer</p>

                    <div class="disclaimer-notice">
                        Please read <a href="disclaimer.htm">Disclaimer</a>
                        and <a href="PrivacyPolicy.htm">Privacy Policy</a> first.
                    </div>                  
                </div>

                <!-- Registration form card -->
                <div class="reg-card">

                    <div class="login-link">
                        Already registered? <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx">Log on</asp:HyperLink>
                    </div>

                    <div class="error-msg">
                        <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                        <asp:Label ID="LblInvalid" runat="server" style="color:var(--red); font-weight:600; font-size:0.9rem;"></asp:Label>
                    </div>

                    <!-- Terms & Conditions -->
                    <div id="Registr" runat="server">
                    <div id="tr1" runat="server" class="form-check-row">
                        <asp:Label ID="Label30" runat="server" Text="Read*:" style="color:var(--red); font-weight:600; font-size:0.85rem; margin-right:0.5rem;"></asp:Label>
                        <asp:CheckBox ID="chkTermsAndCond" runat="server" Text=" " AutoPostBack="True" BorderColor="#FF3300" />
                        <asp:HyperLink ID="HyperLinkTermsAndCond" runat="server" NavigateUrl="https://oureports.net/oureports/TermsAndConditions.pdf" Target="_blank" Font-Size="Small">I have read and agreed to Terms and Conditions</asp:HyperLink>
                    </div>

                    <!-- Database Provider -->
                    <div id="trprov" runat="server" class="form-row">
                        <div class="label-col required">
                            <asp:Label ID="LabelProv" runat="server" Text="User Database provider*:"></asp:Label>
                        </div>
                        <div class="input-col">
                            <asp:DropDownList runat="server" ID="dropdownDatabases" AutoPostBack="True" Font-Size="Smaller" Width="250px">
                                <asp:ListItem Value="System.Data.SqlClient">SQL Server</asp:ListItem>
                                <asp:ListItem Value="MySql.Data.MySqlClient">MySQL</asp:ListItem>
                                <asp:ListItem Value="Oracle.ManagedDataAccess.Client">Oracle</asp:ListItem>
                                <asp:ListItem Value="InterSystems.Data.CacheClient">Intersystems Cache</asp:ListItem>
                                <asp:ListItem Value="InterSystems.Data.IRISClient">Intersystems IRIS</asp:ListItem>
                                <asp:ListItem Value="System.Data.Odbc">ODBC</asp:ListItem>
                                <asp:ListItem Value="System.Data.OleDb">OleDb</asp:ListItem>
                                <asp:ListItem Value="Npgsql">PostgreSQL</asp:ListItem>
                                <asp:ListItem Value="CSVfiles">Use our database</asp:ListItem>
                            </asp:DropDownList>
                            &nbsp;<asp:Button ID="btnBrowse" runat="server" Text="Browse..." ToolTip="Browse for Access mdb or accdb file locally" CssClass="asp-btn-browse" UseSubmitBehavior="True" />
                            <asp:FileUpload id="FileOleDb" runat="server" style="display:none;" />
                            &nbsp;<asp:Label ID="lblFileChosen" runat="server" Text=""></asp:Label>
                        </div>
                    </div>

                    <!-- Connection String -->
                    <div id="trconnstr" runat="server" class="form-row">
                        <div class="label-col required">
                            <asp:Label ID="Label1" runat="server" Text="User Connection String*:"></asp:Label>
                        </div>
                        <div class="input-col">
                            <asp:TextBox ID="ConnStr" runat="server" CssClass="asp-input asp-input-conn"></asp:TextBox>
                        </div>
                    </div>

                    <!-- Database Password -->
                    <div id="trDBpass" runat="server" class="form-row">
                        <div class="label-col optional">
                            <asp:Label ID="LabelDBpassword" runat="server" Text="Database password:" ToolTip="Requested to make initial reports and analytics immediately. You can start making them afterwards in corresponding pages."></asp:Label>
                        </div>
                        <div class="input-col">
                            <asp:TextBox ID="DBpass" runat="server" TextMode="Password" CssClass="asp-input asp-input-sm"></asp:TextBox>
                        </div>
                    </div>

                    <!-- Connection hints -->
                    <div id="trlabels" runat="server" class="form-row">
                        <div class="label-col">&nbsp;</div>
                        <div class="input-col">
                            <asp:CheckBox ID="CheckBox1" runat="server" Font-Size="Smaller" Text="Save Connection Info for future use (password will not be saved for security reason and will be requested during login)" Checked="True" Visible="False" />
                            <div class="form-hint">
                                <asp:Label ID="lblConnection" runat="server" Text="Server=yourserver; Database=yourdatabase; User ID=youruserid;"></asp:Label>
                                <br />
                                <asp:Label ID="lblPassWord" runat="server" Text="(password to database will not be saved for security reasons and will be requested during login every time)"></asp:Label>
                            </div>
                        </div>
                    </div>

                    <!-- User Logon -->
                    <div class="form-row">
                        <div class="label-col required">User Logon*:</div>
                        <div class="input-col">
                            <input name="Logon" type="text" required="required" />
                        </div>
                    </div>

                    <!-- Name -->
                    <div class="form-row">
                        <div class="label-col optional">Name:</div>
                        <div class="input-col">
                            <input name="Name" type="text" />
                        </div>
                    </div>

                    <!-- User Password -->
                    <div class="form-row">
                        <div class="label-col required">User Password*:</div>
                        <div class="input-col">
                            <input name="Pass" type="password" required="required" />
                        </div>
                    </div>

                    <!-- Repeat Password -->
                    <div class="form-row">
                        <div class="label-col required">Repeat Password*:</div>
                        <div class="input-col">
                            <input name="RepeatPass" type="password" required="required" />
                        </div>
                    </div>

                    <!-- Organization -->
                    <div id="trunit" runat="server" class="form-row">
                        <div class="label-col required">
                            <asp:Label ID="LabelOrg" runat="server" Text="Organization*:"></asp:Label>
                        </div>
                        <div class="input-col">
                            <asp:TextBox runat="server" ID="Unit" CssClass="asp-input" Enabled="False" Visible="False" />
                        </div>
                    </div>

                    <!-- Email -->
                    <div class="form-row">
                        <div class="label-col required">Email*:</div>
                        <div class="input-col">
                            <asp:TextBox runat="server" ID="Email" type="text" CssClass="asp-input" required="required" />
                        </div>
                    </div>

                    </div><!-- /Registr -->

                    <!-- Register Button -->
                    <div class="reg-submit">
                        <asp:Button ID="btRegister" runat="server" Text="Register" CssClass="asp-btn-register" />
                    </div>

                </div>
                <div class="seal-wrap" align="center">
                    <span id="siteseal"><script async type="text/javascript" src="https://seal.godaddy.com/getSeal?sealID=SYhDKXg2IT7QXHzPEW7z7tmavANkr8vMDCiRmZvbKczmKBJ5Wj8eKl1EX00B"></script></span>
               </div>
            </ContentTemplate>
        </asp:UpdatePanel>

        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="udpRegister">
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
        var isDark = document.body.classList.contains('dark-theme');
        var newTitle = isDark ? 'Switch to light mode' : 'Switch to dark mode';
        themeBtn.setAttribute('title', newTitle);
        themeBtn.setAttribute('aria-label', newTitle);
        var tip = bootstrap.Tooltip.getInstance(themeBtn);
        if (tip) tip.setContent({ '.tooltip-inner': newTitle });
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

    // Scroll-triggered fade-in
    var observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                entry.target.style.animationPlayState = 'running';
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.1 });
    document.querySelectorAll('.fade-up').forEach(function (el) {
        el.style.animationPlayState = 'paused';
        observer.observe(el);
    });

    // Initialize Bootstrap tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (el) { return new bootstrap.Tooltip(el); });

    // Fix: re-enable the provider dropdown and connection string controls.
    // The code-behind Init/Page_Load may disable them based on Session state,
    // but we need them enabled and interactive on the client side.
    function fixFormControls() {
        // Re-enable all dropdown options
        var sel = document.querySelector('select[id$="dropdownDatabases"]');
        if (sel) {
            sel.disabled = false;
            for (var i = 0; i < sel.options.length; i++) {
                sel.options[i].disabled = false;
            }
        }
        // Re-enable connection string textbox if it exists and is visible
        var conn = document.querySelector('input[id$="ConnStr"]');
        if (conn && conn.offsetParent !== null) {
            conn.disabled = false;
            conn.readOnly = false;
        }
    }
    fixFormControls();
    // Re-run after UpdatePanel partial postbacks
    if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(fixFormControls);
    }
</script>
</body>
</html>
