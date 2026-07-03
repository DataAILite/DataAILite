<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ShowBusinessProposal.aspx.vb" Inherits="ShowBusinessProposal" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>DataAI Business Proposal</title>
    <style type="text/css">
        :root {
            --ink: #1d2522;
            --muted: #5b6762;
            --line: #d8dfdc;
            --panel: #ffffff;
            --soft: #eef5f2;
            --accent: #17624a;
            --accent2: #245f9d;
            --gold: #a06b12;
        }

        * { box-sizing: border-box; }
        body {
            margin: 0;
            padding: 0;
            background: #f7f7f4;
            color: var(--ink);
            font-family: Arial, Helvetica, sans-serif;
            font-size: 14px;
            line-height: 1.55;
        }
        a { color: var(--accent2); font-weight: bold; }
        .page {
            width: min(980px, calc(100% - 32px));
            margin: 24px auto 40px;
            background: var(--panel);
            border: 1px solid var(--line);
            box-shadow: 0 8px 30px rgba(0,0,0,0.06);
        }
        .header {
            border-left: 8px solid var(--accent);
            padding: 28px 34px 24px;
            background: linear-gradient(135deg, #ffffff 0%, var(--soft) 100%);
        }
        .kicker {
            color: var(--accent2);
            font-size: 12px;
            font-weight: bold;
            letter-spacing: 0.08em;
            text-transform: uppercase;
            margin-bottom: 8px;
        }
        h1, h2, h3 { margin: 0; line-height: 1.2; }
        h1 {
            color: var(--accent);
            font-size: 30px;
        }
        .subtitle {
            color: var(--muted);
            font-size: 16px;
            font-weight: bold;
            margin-top: 8px;
        }
        .content { padding: 28px 34px 34px; }
        section { margin-bottom: 26px; }
        h2 {
            color: var(--accent);
            font-size: 20px;
            margin-bottom: 10px;
            text-transform: uppercase;
        }
        h3 {
            color: var(--ink);
            font-size: 16px;
            margin-bottom: 6px;
        }
        p { margin: 0 0 10px; }
        ul { margin: 8px 0 0 20px; padding: 0; }
        li { margin-bottom: 6px; }
        .notice {
            background: var(--soft);
            border-left: 4px solid var(--accent);
            padding: 14px 16px;
            margin: 14px 0;
        }
        .grid {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 12px;
        }
        .card {
            border: 1px solid var(--line);
            background: #fff;
            padding: 14px;
        }
        .tag {
            display: inline-block;
            color: var(--gold);
            background: #f5ead6;
            border-radius: 999px;
            padding: 3px 9px;
            font-size: 11px;
            font-weight: bold;
            margin-bottom: 8px;
        }
        table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 10px;
            font-size: 13px;
        }
        th, td {
            border: 1px solid var(--line);
            padding: 10px;
            vertical-align: top;
            text-align: left;
        }
        th {
            background: var(--accent);
            color: #fff;
            font-weight: bold;
        }
        tr.total td {
            background: var(--soft);
            font-weight: bold;
        }
        .price {
            white-space: nowrap;
            font-weight: bold;
            color: var(--accent);
        }
        .small {
            color: var(--muted);
            font-size: 12px;
        }
        .signature {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 24px;
            margin-top: 18px;
        }
        .sig-line {
            border-top: 1px solid var(--line);
            padding-top: 8px;
            min-height: 46px;
            color: var(--muted);
        }
        @media print {
            body { background: #fff; }
            .page { width: auto; margin: 0; box-shadow: none; border: none; }
        }
        @media (max-width: 760px) {
            .grid, .signature { grid-template-columns: 1fr; }
            .header, .content { padding-left: 18px; padding-right: 18px; }
            table { font-size: 12px; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="page">
            <div class="header">
                <div class="kicker">Yanbor LLC</div>
                <h1>DataAI Business Proposal</h1>
                <div class="subtitle">
                    Prepared for <asp:Label ID="Label1" runat="server" Text="Client Company"></asp:Label>
                </div>
            </div>

            <div class="content">
                <section>
                    <h2>Overview</h2>
                    <p>
                        Yanbor LLC proposes DataAI as a practical reporting and analytics solution for organizations that need faster access to useful information from existing databases and files.
                        DataAI helps users create reports, charts, maps, dashboards, statistical summaries, market analysis, and AI-assisted explanations without building a custom analytics platform from the beginning.
                    </p>
                    <div class="notice">
                        <strong>Open-source position:</strong> DataAI is open source and licensed under the GNU General Public License v3.0 (GPL v3).
                        The software license fee is therefore proposed as $0. Client may use, study, modify, and redistribute the software subject to the GPL v3 license terms.
                        Yanbor LLC pricing in this proposal is for installation, configuration, hosting, training, support, and custom development services.
                    </div>
                </section>

                <section>
                    <h2>Business Need</h2>
                    <div class="grid">
                        <div class="card">
                            <span class="tag">Speed</span>
                            <h3>Initial reports and analytics</h3>
                            <p>DataAI analyzes database structure and creates initial reports and analytics so Client can begin reviewing useful outputs quickly.</p>
                        </div>
                        <div class="card">
                            <span class="tag">Access</span>
                            <h3>Self-service reporting</h3>
                            <p>End users and administrators can work with reports, dashboards, charts, maps, and statistics without routine programming support.</p>
                        </div>
                        <div class="card">
                            <span class="tag">Security</span>
                            <h3>DataAILite option</h3>
                            <p>DataAILite version works in memory only for more security and is useful when Client wants analysis without storing working data in the application database.</p>
                        </div>
                    </div>
                </section>

                <section>
                    <h2>Recommended Scope</h2>
                    <ul>
                        <li>Install DataAI on Windows 10/11 or Windows Server with IIS, or configure a hosted DataAI service for the Client.</li>
                        <li>Connect DataAI to the Client database or file source using read-only credentials when possible.</li>
                        <li>Generate and review initial reports, charts, maps, dashboards, statistical summaries, and market-analysis outputs.</li>
                        <li>Configure security, user access, report categories, sample dashboards, and export options.</li>
                        <li>Train Client administrators and selected users on report creation, analytics review, and maintenance.</li>
                    </ul>
                    <p class="small">
                        Windows 10/11 or Windows Server with IIS is required for self-hosted DataAI. Database engines may include SQL Server, MySQL, Oracle, PostgreSQL, InterSystems Cache/IRIS, ODBC, OleDb, CSV, Excel, or Access depending on the selected DataAI configuration.
                    </p>
                </section>

                <section>
                    <h2>Pricing Basis</h2>
                    <p>
                        Current BI and analytics products commonly use per-user or annual subscription pricing. For example, Microsoft lists Power BI Pro at $14 per user/month and Premium Per User at $24 per user/month; Tableau lists Standard at $15 per user/month and Enterprise at $35 per user/month, with Creator licenses higher; Metabase lists an open-source plan as free and enterprise pricing starting at $20,000 per year.
                    </p>
                    <p>
                        Because DataAI is GPL v3 open-source software, the recommended DataAI pricing below avoids a software license charge and instead prices professional services. This makes the proposal easier to explain: Client pays for expert help, not for permission to use GPL v3 software.
                    </p>
                </section>

                <section>
                    <h2>Suggested Pricing</h2>
                    <table summary="Suggested DataAI pricing">
                        <tr>
                            <th>Item</th>
                            <th>Scope</th>
                            <th>Suggested Price</th>
                        </tr>
                        <tr>
                            <td>DataAI open-source software license</td>
                            <td>Use of DataAI source code under GNU General Public License v3.0 (GPL v3).</td>
                            <td class="price">$0</td>
                        </tr>
                        <tr>
                            <td>DataAILite secure in-memory pilot</td>
                            <td>Remote setup of DataAILite, one sample data source, basic walkthrough, and validation that in-memory analysis workflow is working.</td>
                            <td class="price">$2,500</td>
                        </tr>
                        <tr>
                            <td>Quick Start implementation</td>
                            <td>DataAI installation, one database connection, initial report and analytics generation, and one administrator training session.</td>
                            <td class="price">$4,500</td>
                        </tr>
                        <tr>
                            <td>Standard implementation</td>
                            <td>Production setup, up to three data sources, initial dashboards, report categories, security setup, exports, and two training sessions.</td>
                            <td class="price">$9,500</td>
                        </tr>
                        <tr>
                            <td>Enterprise implementation</td>
                            <td>Dedicated server setup, multiple environments, up to eight data sources, dashboard/report package, security review, documentation, and four training sessions.</td>
                            <td class="price">$18,500</td>
                        </tr>
                        <tr>
                            <td>Custom development and integration</td>
                            <td>New connectors, custom reports, embedded workflows, application integration, branding, import/export automation, or special analytics.</td>
                            <td class="price">$125/hour or $5,000 per 40-hour block</td>
                        </tr>
                        <tr>
                            <td>Training</td>
                            <td>Remote training for administrators, report designers, or business users.</td>
                            <td class="price">$1,500 half-day / $2,800 full-day</td>
                        </tr>
                        <tr>
                            <td>Standard support</td>
                            <td>Email support, minor configuration help, and scheduled maintenance guidance.</td>
                            <td class="price">$750/month or $7,500/year</td>
                        </tr>
                        <tr>
                            <td>Priority support</td>
                            <td>Faster response, monthly review call, bug triage, update assistance, and up to four support hours per month.</td>
                            <td class="price">$1,500/month or $15,000/year</td>
                        </tr>
                        <tr>
                            <td>Managed hosting and maintenance</td>
                            <td>Yanbor-managed DataAI web hosting, backups, application updates, uptime review, and operational monitoring. Cloud or server infrastructure costs are billed separately if applicable.</td>
                            <td class="price">$1,200/month plus infrastructure</td>
                        </tr>
                        <tr class="total">
                            <td>Recommended first-year budget</td>
                            <td>Standard implementation plus annual standard support. Custom development and hosting are optional.</td>
                            <td class="price">$17,000</td>
                        </tr>
                    </table>
                    <p class="small">
                        Prices are suggested estimates for proposal planning. Final pricing should be confirmed after Client data sources, security requirements, hosting model, number of users, and custom-development scope are reviewed.
                    </p>
                </section>

                <section>
                    <h2>Project Timeline</h2>
                    <table summary="DataAI implementation timeline">
                        <tr>
                            <th>Phase</th>
                            <th>Activities</th>
                            <th>Typical Duration</th>
                        </tr>
                        <tr>
                            <td>Discovery</td>
                            <td>Confirm data sources, users, security requirements, hosting model, and sample reports.</td>
                            <td>1 week</td>
                        </tr>
                        <tr>
                            <td>Installation</td>
                            <td>Install DataAI or DataAILite, configure IIS, establish database/file access, and validate connection security.</td>
                            <td>1-2 weeks</td>
                        </tr>
                        <tr>
                            <td>Initial analytics</td>
                            <td>Generate initial reports, dashboards, statistics, maps, and market-analysis outputs; review with Client stakeholders.</td>
                            <td>1-2 weeks</td>
                        </tr>
                        <tr>
                            <td>Training and handoff</td>
                            <td>Train administrators and users, document configuration, and define support process.</td>
                            <td>1 week</td>
                        </tr>
                    </table>
                </section>

                <section>
                    <h2>Client Responsibilities</h2>
                    <ul>
                        <li>Provide a Windows 10/11 or Windows Server with IIS environment for self-hosted installation, unless managed hosting is selected.</li>
                        <li>Provide database or file access, preferably read-only for analytical sources.</li>
                        <li>Identify initial users, administrators, and sample reports or decisions that should guide configuration.</li>
                        <li>Review GPL v3 obligations with Client legal counsel when modifying or redistributing DataAI.</li>
                    </ul>
                </section>

                <section>
                    <h2>Assumptions and Notes</h2>
                    <ul>
                        <li>No proprietary software license fee is included for DataAI itself because DataAI is open source under GPL v3.</li>
                        <li>Third-party database licenses, Windows Server licenses, cloud infrastructure, domain names, SSL certificates, OpenAI/API usage, and other external costs are not included unless specifically added to a final agreement.</li>
                        <li>This proposal is not legal advice. GPL v3 compliance questions should be reviewed by Client legal counsel.</li>
                        <li>Estimates are valid for 60 days and may change if the requested scope, data volume, hosting model, or integration requirements change.</li>
                    </ul>
                </section>

                <section>
                    <h2>Acceptance</h2>
                    <p>
                        If Client would like to proceed, Yanbor LLC recommends selecting a service package, confirming the hosting model, and scheduling a discovery meeting.
                        Questions may be sent through the <a href="ContactUs.aspx">Contact page</a>.
                    </p>
                    <div class="signature">
                        <div class="sig-line">Yanbor LLC representative / date</div>
                        <div class="sig-line">Client representative / date</div>
                    </div>
                </section>
            </div>
        </div>
    </form>
</body>
</html>
