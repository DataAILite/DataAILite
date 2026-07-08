# DataAISandbox Full Documentation



**Version:** May 14, 2026



**Project:** DataAI open source ASP.NET Web Forms reporting, analytics, market modeling, charting, mapping, dashboard, export, and AI-assisted analysis application.



## Executive Summary

DataAISandbox is the development and demonstration workspace for DataAI. It brings together report design, database exploration, imported file analysis, dashboards, statistical analytics, market models, maps, exports, and AI-assisted interpretation in one ASP.NET Web Forms application.



## What DataAISandbox Does

- Connects to existing databases or imported datasets and turns them into reportable, analyzable, exportable views.

- Combines report design with profiling, data quality checks, pivoting, variance, ranking, regression, time summaries, outliers, correlations, maps, and market models.

- Provides dashboards for analytics, charts, and market models.

- Adds AI-assisted interpretation for selected reports, grids, charts, and analytical outputs.

- Adds readiness-first guidance so users can see which analytics, market models, chart pages, map checks, or data-quality pages fit the current report before choosing a workflow.

- Supports chart dashboards, custom analytics dashboards, and mixed dashboards that can combine chart tiles with analytical/report-view tiles.



## Main Workflow Examples



### Build a Report From a Database

1. Open ListOfReports.aspx and choose or create a report.

2. Use ReportEdit.aspx to define title, parameters, and sharing.

3. Use SQLquery.aspx to choose fields, joins, filters, and sorting.

4. Use RDLformat.aspx or ReportDesigner.aspx to choose layout, expressions, groups, totals, and advanced design.

5. Open ShowReport.aspx or ReportViews.aspx to inspect records and export results.



### Analyze a Report

1. Open DataReadinessScanner.aspx to review recommended analytics and suggested fields for the current report.

2. Open DataAdmin.aspx to see the Analytics Dashboard preview or DataCheck.aspx to focus on data trust/readiness checks.

3. Open Analytics.aspx for grouped detail analytics.

4. Use Pivot.aspx, Variance.aspx, DataQuality.aspx, Profiling.aspx, Regression.aspx, DataDrift.aspx, AnomalyScoring.aspx, RuleBasedAlerts.aspx, and related pages for targeted analysis.

5. Export to Excel/CSV/PDF/Word where available or send the visible grid to DataAI.aspx for AI interpretation.



### Create Chart Dashboards

1. Open ChartRecommendationHelpers.aspx from the Show Report menu.

2. Restrict category/date/value fields if needed or let the page build recommendations.

3. Select eligible charts with Add to Dashboard and click Create Dashboard.

4. Open Dashboard.aspx to browse dashboard pages with Previous/Page/Next navigation.

5. Use ChartGoogleOne.aspx to open individual charts, add them to dashboards, or download chart data when chart-ready data exists.



### Create Custom or Mixed Dashboards

1. Use Add to Dashboard from report, chart, analytics, or market pages.

2. Save the current URL and report context into a user dashboard.

3. Open CustomDashboard.aspx for analytical/report-view tiles, Dashboard.aspx for chart-only dashboards, or MixDashboard.aspx when a dashboard contains both chart and analytical tiles.

4. Use dashboard export controls to package dashboard notes and available chart/report/analytical outputs.



### Review Market Models

1. Open MarketAdmin.aspx for a dashboard overview.

2. Open a Market page such as Demand, Pricing, Elasticity, Basket, Risk, Inventory, Profit, or Scenario.

3. Select fields, optional period aggregation, and assumptions where applicable.

4. Review the grid, drill into Records links, export results, or send output to AI.



### Prepare Map Reports

1. Open MapReadines.aspx to confirm coordinate readiness.

2. Review missing/invalid/duplicate coordinate checks and suggested latitude/longitude fields.

3. Open MapReport.aspx to configure map fields and output.

4. Use map links or KML-ready output for location-based review.



## Page Documentation by Functional Area



### Entry, Download, Installation, and Public Pages



| Page | Purpose | Typical Use |

|---|---|---|

| `index.aspx` | Legacy/default landing entry used by the site root. | Routes visitors into the public DataAI experience. |

| `index1.aspx` | Main DataAI home page for open source messaging, demos, documentation, downloads, and navigation. | Use when a visitor needs to understand DataAI, open demos, download software, or reach GitHub. |

| `index.html` | Static public landing page version for DataAI marketing and links. | Use when a non-Web Forms static entry page is needed. |

| `Index3.aspx` | Service-oriented overview page with modern green visual styling. | Introduces DataAI service value and the platform story. |

| `IndexSoftware.aspx` | Software-oriented entry page. | Directs users toward downloadable software and related product pages. |

| `DownloadDataAI.aspx` | Download page for DataAI, DataAILite, and TaskListAI. | Collects email, shows product-specific download text, links GitHub and license text, and routes download/payment where applicable. |

| `InstallDataAI.aspx` | Installation page for DataAI. | Provides installation guidance or installer-oriented workflow. |

| `InstallIt.aspx` | Generic installer/support page. | Supports installation flow handled through the web project. |

| `InstallTaskList.aspx` | TaskList installer/download support page. | Supports TaskListAI installation or installer information. |

| `QuickStart.aspx` | Quick start entry. | Gives first-time users the shortest path to try reports. |

| `Registration.aspx` | Individual registration page. | Creates or initiates individual user access. |

| `UnitRegistration.aspx` | Company/unit registration page. | Creates organizational units or company registration requests. |

| `confirm.aspx` | Confirmation page. | Used after registration, account, or request flows. |

| `AboutUs.aspx` | About page. | Explains the organization and product background. |

| `ContactUs.aspx` | Contact page. | Lets users find support/contact routes. |

| `HelpDesk.aspx` | Support/helpdesk entry. | Links to help or ticket-style support workflows. |

| `ShowBusinessProposal.aspx` | Business proposal display page. | Presents proposal content when included in deployment. |

| `UnderConstruction.aspx` | Placeholder page. | Used for features or routes not yet active. |



### Security, Users, Units, and Administration



| Page | Purpose | Typical Use |

|---|---|---|

| `Default.aspx` | Sign-in and authenticated entry page. | Starts report sessions, demo sessions, and authenticated user work. |

| `SiteAdmin.aspx` | Site administration page. | Administrative maintenance for the application. |

| `UnitsAdmin.aspx` | Unit administration page. | Manages organizational units. |

| `UnitDefinition.aspx` | Unit definition page. | Defines or edits organizational unit metadata. |

| `UnitWebOnServer.aspx` | Unit web/server configuration page. | Supports unit-level deployment settings. |

| `UserDefinition.aspx` | User definition page. | Creates or edits user records and permissions. |

| `Delete.aspx` | Delete action page. | Supports removal workflows where application data or definitions must be deleted. |



### Report Management and Report Lists



| Page | Purpose | Typical Use |

|---|---|---|

| `ListOfReports.aspx` | Main report list and launch page. | Users select reports, open dashboards, edit definitions, or manage report records. |

| `ListOfDashboards.aspx` | List of saved chart dashboards. | Users open or delete selected dashboards. |

| `Dashboard.aspx` | Chart dashboard page with page navigation. | Displays saved chart tiles and lets users move between dashboard pages. |

| `ReportCopy.aspx` | Report copy support page. | Duplicates report definitions where supported. |

| `FriendlyNames.aspx` | Friendly names page. | Assigns readable labels to fields or groups. |

| `AddParameters.aspx` | Report parameter helper page. | Adds or configures report parameters. |

| `Parameters.aspx` | Parameter handling page. | Supports user selection and parameter passing into reports. |



### Report Design: Data, Query, and Format



| Page | Purpose | Typical Use |

|---|---|---|

| `ReportEdit.aspx` | Report definition page. | Sets report identity, title, metadata, parameters, sharing, and definition settings. |

| `SQLquery.aspx` | Report data query designer. | Defines data fields, joins, filters, sorting, SQL, and report data retrieval. |

| `RDLformat.aspx` | Report format definition page. | Controls column order, expressions, groups, totals, combined values, formatting, designer links, and map definitions. |

| `ReportDesigner.aspx` | Advanced report designer. | Provides advanced layout/design operations for report presentation. |

| `ListOfTables.aspx` | Table explorer/list page. | Shows available source tables for report building. |

| `ListOfJoins.aspx` | Join definition list page. | Supports relationship and join configuration between report tables. |

| `ClassExplorer.aspx` | Class/table explorer. | Inspects table/class structures and fields. |



### Report Viewing, Data Exploration, and Export



| Page | Purpose | Typical Use |

|---|---|---|

| `ShowReport.aspx` | Main report/data explorer page. | Shows records, filters, exports, analytics links, charts, maps, and drill-down filtered results. |

| `ReportViews.aspx` | Generic and formatted report views. | Shows report outputs and exports to PDF, Word, Excel, or other report formats where supported. |

| `ShowCrystalReport.aspx` | Crystal report display page. | Displays legacy Crystal-style report output where configured. |

| `DataImport.aspx` | Data import page. | Imports CSV, TXT, XML, JSON, Excel, Access, and similar data into reportable datasets. |

| `ExportPackages.aspx` | Export package builder. | Creates zipped packages with selected report output, data formats, charts, definitions, AI analysis, and notes. |

| `DataAIHelp.aspx` | Documentation/help index. | Organizes PDF/help links by topics and highlights searched help terms. |



### Analytics Dashboard and Analytical Pages



| Page | Purpose | Typical Use |

|---|---|---|

| `DataAdmin.aspx` | Analytics Dashboard. | Tile previews for Analytics, Overall Statistics, Group Statistics, Correlation, DataAI, Pivot, Variance, Comparison, Profiling, Data Quality, Ranking, Regression, Time Summaries, Time Series, Outliers, and Matrix Balancing. |

| `DataCheck.aspx` | Data Quality Dashboard. | Groups data trust/readiness pages such as profiling, quality, dictionary, drift, anomaly scoring, outliers, alerts, and map readiness. |

| `DataReadinessScanner.aspx` | Data readiness scanner. | Scores which analytics, market models, charts, map checks, and quality pages fit the selected report. |

| `Analytics.aspx` | Detail Analytics. | Grouped analytics from category/group fields and value fields, including count, sum, min, max, average, standard deviation, distinct counts, charts, and AI interpretation. |

| `AdvancedAnalytics.aspx` | Advanced analytics and matrix balancing entry. | Supports advanced matrix balancing and related analytical workflows. |

| `MultidimensionalBalancing.aspx` | Multidimensional balancing. | Balances values across multiple fields/dimensions for allocation, weighting, or reconciliation. |

| `Pivot.aspx` | Pivot / cross-tab analysis. | Creates row-by-column cross-tab reports with selected row, column, value, and aggregation. |

| `Variance.aspx` | Variance, percentage change, and contribution analysis. | Compares base and comparison values, variance, percent change, and contribution-to-total. |

| `ComparisonReports.aspx` | Comparison reports. | Compares two periods, groups, locations, queries, or imported files and links to base/compare records. |

| `Profiling.aspx` | Automatic field profiling. | Profiles every field with detected type, blanks, distinct values, min/max, average, and standard deviation where applicable. |

| `DataQuality.aspx` | Data quality checks. | Finds missing values, duplicate records, invalid dates, out-of-range values, inconsistent categories, and suspicious text. |

| `Ranking.aspx` | Ranking analysis. | Ranks categories, customers, products, locations, groups, or other dimensions by top, bottom, or average values. |

| `Regression.aspx` | Regression and prediction. | Fits linear, polynomial, exponential, logarithmic, power, and logistic probability models and links to Trends. |

| `Trends.aspx` | Trend chart and equation explorer. | Displays equation-based charts, interactive X/Y selection, zooming/panning, and Excel/PDF chart export. |

| `TimeBasedSummaries.aspx` | Time-based summaries. | Aggregates data by day, week, month, quarter, or year where date fields exist. |

| `TimeSeries.aspx` | Moving averages and rolling totals. | Calculates rolling totals and moving averages over selected time periods. |

| `OutlierFlagging.aspx` | Outlier flagging. | Flags records based on standard deviation, percentage difference, or business-rule thresholds. |

| `AnomalyScoring.aspx` | Anomaly scoring. | Scores unusual combinations, unusual values inside groups, unusual period movement, and suspicious category/value patterns. |

| `DataDrift.aspx` | Data drift analysis. | Compares distributions between groups or periods to identify shifting data behavior. |

| `RuleBasedAlerts.aspx` | Rule-based alerts. | Shows alerts for missing-value rates, variance, correlation, outlier, map-readiness, and churn-style thresholds. |

| `ABCPareto.aspx` | ABC Pareto analysis. | Finds the smaller set of categories/items that account for the largest value share. |

| `Cohort.aspx` | Cohort analysis. | Compares groups with the same start period or starting event. |

| `Funnel.aspx` | Funnel analysis. | Measures conversion/drop-off across selected stages. |

| `KPIBuilder.aspx` | KPI builder. | Builds KPI-style metrics from selected fields, groups, targets, and thresholds. |

| `DataDictionary.aspx` | Data dictionary. | Documents field names, types, examples, detected roles, and suggested usage. |

| `AutomatedAnalysisNarratives.aspx` | Automated analysis narratives. | Creates narrative summaries from selected analytical outputs. |

| `CrossReportComparison.aspx` | Cross-report comparison. | Compares selected values between the current report and another saved report. |

| `Correlation.aspx` | Field correlation analysis. | Calculates correlations between numeric fields and links to threshold/specialized views. |

| `CorrelationThreshold.aspx` | Correlation threshold view. | Filters correlation results by selected thresholds and prepares correlation output for export/AI review. |

| `AuditSummaries.aspx` | Audit-style analytical summaries. | Shows which fields, filters, thresholds, and aggregation choices produced analytical results. |



### Charts, Dashboards, and Chart Recommendation Pages



| Page | Purpose | Typical Use |

|---|---|---|

| `ChartGoogle.aspx` | Google chart page. | Displays report data as selected Google chart visualizations. |

| `ChartGoogleOne.aspx` | Single/multi-chart Google chart renderer. | Receives chart parameters and renders chart output. |

| `ChartGoogleOne.js.aspx` | JavaScript-backed chart helper page. | Provides dynamic chart script output where used by chart pages. |

| `ChartRecommendationHelpers.aspx` | Chart recommendation engine. | Builds recommended charts from category/date/value fields, supports paging, validation, dashboard selection, and Create Dashboard. |

| `Dashboard.aspx` | Saved chart dashboard viewer. | Shows saved chart dashboards with navigation controls, chart tiles, and page selection. |

| `CustomDashboard.aspx` | Custom analytics dashboard viewer. | Shows saved analytical/report-view dashboard tiles with preview grids and open/delete links. |

| `MixDashboard.aspx` | Mixed dashboard viewer. | Shows dashboards that contain both chart tiles and analytical tiles. |



### Map and Geographic Pages



| Page | Purpose | Typical Use |

|---|---|---|

| `MapReport.aspx` | Map report definition and output. | Configures latitude/longitude, placemark, description, color, and KML/map outputs. |

| `MapReadines.aspx` | Map readiness checks. | Checks coordinate fields, missing coordinates, invalid ranges, duplicates, and KML-ready records. |

| `MapGoogle.aspx` | Google map display page. | Displays map output where Google map rendering is configured. |

| `GoogleMap.aspx` | Google map helper/display page. | Supports map visualization in deployments that use this route. |



### Market and Business Model Pages



| Page | Purpose | Typical Use |

|---|---|---|

| `MarketAdmin.aspx` | Market Dashboard. | Tile dashboard for all Market pages with preview grids and report title in the header. |

| `MarketDemand.aspx` | Demand model. | Models demand by category/product/customer/location or combined primary fields, with optional period aggregation. |

| `MarketPricing.aspx` | Pricing sensitivity model. | Groups by price bands and optional primary dimension; compares average quantity and revenue by band. |

| `MarketElasticity.aspx` | Price elasticity model. | Estimates elasticity from price and quantity bands and projects revenue impact under assumption changes. |

| `MarketBasket.aspx` | Market-basket co-occurrence. | Finds items appearing together in transactions and calculates support and weighted basket value. |

| `MarketSegments.aspx` | Customer/product/market segmentation. | Groups selected dimensions and compares segment value, average value, and notes. |

| `MarketChurn.aspx` | Churn/retention scoring. | Scores customer or segment recency and retention risk using date/activity and value fields. |

| `MarketRisk.aspx` | Risk scoring. | Scores grouped exposure/value from 0 to 100 and classifies risk levels. |

| `MarketInventory.aspx` | Inventory movement and reorder analysis. | Calculates movement, velocity, current inventory, supply periods, reorder point, and reorder-needed flag. |

| `MarketProfit.aspx` | Profitability driver model. | Calculates revenue, cost source, estimated cost, estimated profit, margin, and profit contribution. |

| `MarketScenario.aspx` | Scenario model. | Builds downside/base/upside values from an assumption percentage and selected value field. |



### AI and Assistant Pages



| Page | Purpose | Typical Use |

|---|---|---|

| `DataAI.aspx` | AI-assisted analysis page. | Receives selected data or analytical output and prepares AI interpretation workflow. |

| `ChatAI.aspx` | AI chat interface. | Lets users ask follow-up questions and receive narrative analysis about selected data. |

| `DataAIaddons.aspx` | DataAI add-ons / DataAILite workflow page. | Supports in-memory or add-on style data analysis experiences. |

| `DataAIsqlite.aspx` | SQLite-oriented DataAI page. | Supports SQLite-style or lightweight data workflows when configured. |



### Scheduling and Automation Pages



| Page | Purpose | Typical Use |

|---|---|---|

| `ScheduledReports.aspx` | Scheduled reports setup. | Configures recurring report delivery or report runs. |

| `ScheduleReportsCalendar.aspx` | Scheduled reports calendar. | Calendar view for scheduled reports. |

| `RunScheduledReports.aspx` | Scheduled report runner. | Executes scheduled report jobs. |

| `ScheduledImports.aspx` | Scheduled imports setup. | Configures recurring data imports. |

| `ScheduledImportsCalendar.aspx` | Scheduled imports calendar. | Calendar view for scheduled import jobs. |

| `RunScheduledImports.aspx` | Scheduled import runner. | Executes scheduled import jobs. |

| `ScheduledDownloads.aspx` | Scheduled downloads setup. | Configures scheduled download/export operations. |

| `ScheduledDownloadsCalendar.aspx` | Scheduled downloads calendar. | Calendar view for scheduled downloads. |

| `RunScheduledItems.aspx` | General scheduled item runner. | Runs configured scheduled tasks. |

| `SendEmailsForScheduledReports.aspx` | Scheduled email sender. | Sends scheduled report emails when configured. |



### TaskList and Companion Pages



| Page | Purpose | Typical Use |

|---|---|---|

| `TaskList.aspx` | TaskList main page. | AI/project manager companion task list page. |

| `TaskListCalendar.aspx` | Task calendar. | Calendar view of tasks. |

| `TaskListSetting.aspx` | TaskList settings. | Configures TaskList behavior. |

| `TaskListTimeLine.aspx` | Task timeline. | Timeline view of task history or planning. |



## Detailed Notes



### Analytics Dashboard

DataAdmin.aspx is the analytical command center. Its tiles preview the most important analytics pages and help the user choose the next workflow. It is especially useful immediately after a report is opened because it shows whether the report has enough category, date, and numeric fields for deeper analysis.



### Market Dashboard

MarketAdmin.aspx is the equivalent command center for business model pages. It shows market model tiles and uses current report data or sample market data where needed. Each tile opens a focused model page such as pricing, elasticity, demand, inventory, profit, churn, risk, basket, segment, or scenario.



### Chart Dashboards

ChartRecommendationHelpers.aspx, ChartGoogleOne.aspx, Dashboard.aspx, CustomDashboard.aspx, and MixDashboard.aspx work together. The recommendation page reviews available category, date, and numeric fields and proposes charts. Users can select recommended rows and create a dashboard. Dashboard.aspx displays chart-only dashboards, CustomDashboard.aspx displays analytical/report-view tiles, and MixDashboard.aspx displays dashboards that combine chart and analytical tiles. ChartGoogleOne.aspx keeps add-to-dashboard and download actions visible when chart-ready data exists and the user has the required rights.



### Export Packages

ExportPackages.aspx can combine selected report output, report definitions, data formats, chart references, AI analysis, user notes, dashboard chart PNGs, generated report PDFs, and dashboard item references into a package. It is useful when a report or dashboard needs to travel with enough context for review or audit. When Export as PDF document(s) is used, the main PDF is downloaded alone if no separate PDFs are involved; otherwise the main PDF and referenced PDF files are returned in a ZIP.



### AI Interpretation

AI buttons prepare the visible analytical grid or report output as session data, then open the DataAI/ChatAI workflow so the AI explains the result the user is actually seeing rather than unrelated raw data.



### Privacy and Configuration

Private deployment values such as connection strings, support email, payment settings, file paths, and OpenAI credentials belong in configuration and are not part of source-controlled documentation. Analytical outputs are generally calculated from the current report data and shown in memory or exported only when requested.



## Glossary

- **Report:** A saved definition describing where data comes from, how it is queried, and how it is displayed.

- **RDL format:** The project report-format layer used for columns, groups, totals, expressions, and layout behavior.

- **Dashboard tile:** A compact preview or chart that opens a deeper page or saved dashboard view.

- **AI interpretation:** A narrative explanation generated from the current visible report or analytical grid.

- **Market model:** A transparent business calculation such as demand, pricing, basket, churn, risk, inventory, profit, elasticity, or scenario analysis.

- **Record link:** A grid value that opens ShowReport.aspx with a filter for the records used to calculate that row.
