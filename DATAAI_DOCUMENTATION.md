# DataAI Documentation

This document describes the DataAI web application pages and the main workflows they support. It is written as a working guide for users who build reports, explore imported or database data, run analytics, create charts, schedule outputs, and review administration pages.

## Application Overview

DataAI is an ASP.NET Web Forms application for data reporting and analysis. It can work with SQL query results, imported files, report definitions, charts, dashboards, maps, scheduling pages, and analytical tools. The application is organized around report creation, report execution, data exploration, analytics, AI-assisted interpretation, and administration.

Typical workflow:

1. Sign in to the application.
2. Create or select a report.
3. Run the report in Data Explorer.
4. Apply filters or parameters.
5. Open analytics such as Analytics Dashboard, Analytics, See Data Overall Statistics, Export Overall Statistics to Excel, See Groups Statistics, See Fields Correlation, Correlation, Correlation Threshold, Pivot, Variance, Comparison Reports, Profiling, Data Quality, Ranking, Regression, Trends, Time-Based Summaries, Time Series, Outlier Flagging, Chart Recommendation Helpers, Map Readiness, Advanced Analytics, or Matrix Balancing.
6. Export results when the selected page supports Excel, PDF, Word, CSV, charts, report definitions, or package output.

## Getting Started Pages

### Default.aspx

The default entry page is normally used to start the application or sign in.

Sample:

- Open DataAI.
- Enter user credentials.
- Continue to the report list or dashboard.

### Registration.aspx

Used for user registration when registration is enabled.

Sample:

- Enter user information.
- Submit registration.
- Wait for confirmation or administrator approval, depending on configuration.

### confirm.aspx

Used for confirmation actions, such as confirming registration or another application operation.

Sample:

- Follow a confirmation link.
- Review the confirmation result.
- Continue to sign in.

### QuickStart.aspx

Provides a quick-start path for learning the application.

Sample:

- Review the first steps.
- Open the report list.
- Run an existing report.
- Try a chart or analytics page.

### AboutUs.aspx

Displays information about the DataAI application or organization.

Sample:

- Open About Us from the menu.
- Review general product or organization information.

### ContactUs.aspx

Displays contact information or a contact form.

Sample:

- Open Contact Us.
- Review support contact details.
- Send a message if the form is enabled.

### HelpDesk.aspx

Provides help desk or support information.

Sample:

- Open Help Desk.
- Review support instructions.
- Submit or follow support guidance.

### UnderConstruction.aspx

Placeholder page for features or sections that are not active yet.

Sample:

- Open a feature that is still pending.
- Review the under-construction notice.

## Installation And Product Information Pages

### InstallDataAI.aspx

Provides DataAI installation guidance.

Sample:

- Open the installation page.
- Review prerequisites.
- Follow installation steps for the configured environment.

### InstallIt.aspx

Supports installation-related instructions or actions.

Sample:

- Open Install It.
- Review deployment steps.
- Continue to the application setup pages.

### IndexSoftware.aspx

Displays software or product information.

Sample:

- Open the software index.
- Review available DataAI software modules.

### index.aspx, index1.aspx, Index3.aspx

Index pages can be used as entry, landing, or informational pages depending on the deployment.

Sample:

- Open the configured index page.
- Navigate to sign in, documentation, or main application pages.

## Report List And Report Definition Pages

### ListOfReports.aspx

Shows available reports.

Sample:

- Open the report list.
- Select a report such as Sales by Department.
- Open the report in Data Explorer.

### ReportEdit.aspx

Used to create or edit report definitions.

Sample:

- Select an existing report.
- Change selected fields, display names, filters, or options.
- Save the report.
- Open it in ShowReport.aspx.

### ReportCopy.aspx

Copies an existing report to a new report definition.

Sample:

- Choose a report named Sales by Month.
- Copy it to Sales by Month - Regional.
- Edit the copy without changing the original report.

### SQLquery.aspx

Used to define or run SQL query based reports.

Sample:

```sql
SELECT Department, EmployeeName, SalesAmount, SaleDate
FROM Sales
WHERE SaleDate >= '2026-01-01'
```

The query result can be used as the report dataset.

### RDLformat.aspx

Supports RDL-style report formatting.

Sample:

- Select a report.
- Review or edit report layout options.
- Save the report format.

### ReportDesigner.aspx

Used for report design tasks.

Sample:

- Select data fields.
- Set report structure.
- Save the design.
- Run the report.

### ListOfTables.aspx

Shows database tables available for report creation.

Sample:

- Open table list.
- Review available tables such as Customers, Orders, Sales, or Departments.
- Select tables for a report definition.

### ListOfJoins.aspx

Shows or manages joins between tables.

Sample:

- Join Orders.CustomerID to Customers.CustomerID.
- Save the join.
- Use joined fields in a report.

### FriendlyNames.aspx

Allows technical field names to be shown with user-friendly labels.

Sample:

- Change `CustNm` to `Customer Name`.
- Change `Tbl1Fld1` to `Category/Group 1`.
- Save friendly names for report display.

### Parameters.aspx

Defines report parameters.

Sample:

- Add a Start Date parameter.
- Add an End Date parameter.
- Use both parameters in a report filter.

### AddParameters.aspx

Adds parameters to a report or query.

Sample:

- Select a report.
- Add Region as a parameter.
- Run the report and choose a region value.

## Data Import Pages

### DataImport.aspx

Imports external data files for reporting and analysis.

Sample:

- Browse for a file.
- Import the file.
- Review imported columns.
- Open the imported dataset in Data Explorer or analytics pages.

### DataAIsqlite.aspx

Supports SQLite or lightweight local database style data access, depending on configuration.

Sample:

- Select a SQLite data source.
- Open tables or query results.
- Create reports from the available data.

### ScheduledImports.aspx

Defines scheduled data import jobs.

Sample:

- Create a daily import.
- Select the import source.
- Set the schedule time.
- Save the schedule.

### ScheduledImportsCalendar.aspx

Shows scheduled imports on a calendar.

Sample:

- Open the imports calendar.
- Review upcoming import dates.
- Select an import event to review details.

### RunScheduledImports.aspx

Runs scheduled imports.

Sample:

- Open scheduled import runner.
- Execute due imports.
- Review success or error messages.

## Data Explorer And Report Output Pages

### ShowReport.aspx

The main Data Explorer page for viewing report records. Many analytics pages link back to ShowReport.aspx with filters so the user can inspect source records.

Sample:

- Open Sales by Department.
- Filter Department = Finance.
- Review matching rows.
- Export the report if the report output supports export.

### ReportViews.aspx

Manages or displays saved report views.

Sample:

- Save a filtered report view.
- Reopen the same view later.
- Share or reuse the view if permissions allow.

### ShowCrystalReport.aspx

Displays Crystal Report output when available.

Sample:

- Select a Crystal-style report.
- Open formatted report output.
- Print or export if enabled.

### ChartGoogle.aspx

Creates Google chart style visualizations.

Sample:

- Select Category as the X axis.
- Select SalesAmount as the Y axis.
- Choose bar chart.
- Show the chart.

### ChartGoogleOne.aspx

Displays a single chart using selected chart parameters.

Sample:

- Open a chart link from Chart Recommendation Helpers.
- Review the chart for selected X and Y fields.
- Change chart options and click Show if needed.

### ChartGoogleOne.js.aspx

Provides JavaScript support for chart rendering.

Sample:

- ChartGoogleOne.aspx loads chart logic.
- The chart script renders the selected visualization.

### Dashboard.aspx

Displays dashboard output.

Sample:

- Open a dashboard.
- Review key charts, tables, and metrics.
- Navigate to detailed reports.

### ListOfDashboards.aspx

Shows available dashboards.

Sample:

- Open dashboard list.
- Select Executive Sales Dashboard.
- Review dashboard tiles and charts.

## Analytics And Data Science Pages

### DataAdmin.aspx

Analytics Dashboard page showing analytics tools with sample grids from the corresponding pages.

Sample:

- Open Data Admin.
- Review tiles for Analytics, Overall Statistics, Groups Statistics, Correlation, DataAI, Pivot, Variance, Comparison Reports, Profiling, Data Quality, Ranking, Regression, Time Based Summaries, Time Series, Outlier Flagging, and Matrix Balancing.
- Click Open to go to the selected analytics page.

### Analytics.aspx

General analytics page for report-based summaries and visual analysis options.

Sample:

- Select a report.
- Choose category/group fields.
- Choose value fields.
- Review summary table and chart options.

### AdvancedAnalytics.aspx

Advanced analytics entry point.

Sample:

- Open Advanced Analytics.
- Select a report or dataset.
- Continue to the specialized analytical method required.

### Pivot.aspx

Creates pivot-style cross-tab reports with row fields, column fields, value fields, and aggregation options.

Sample:

- Row Field: Department.
- Column Field: Month.
- Value Field: SalesAmount.
- Aggregation: Sum.
- Result: a cross-tab showing sales by department and month.

### Variance.aspx

Provides percentage-change analysis, variance analysis, and contribution-to-total analysis.

Sample:

- Base field: SalesAmount for January.
- Compare field: SalesAmount for February.
- Group field: Department.
- Result: difference, percent change, and contribution to total.

### ComparisonReports.aspx

Compares two periods, two groups, two locations, two SQL queries, or two imported files.

Sample for two periods:

- Comparison Type: Two Periods.
- Period 1: January 2026.
- Period 2: February 2026.
- Value Field: SalesAmount.
- Result: side-by-side totals, difference, and percent change.

Sample for two SQL queries:

```sql
-- Query 1
SELECT Department, SUM(SalesAmount) AS Sales
FROM Sales
WHERE SaleDate >= '2026-01-01' AND SaleDate < '2026-02-01'
GROUP BY Department
```

```sql
-- Query 2
SELECT Department, SUM(SalesAmount) AS Sales
FROM Sales
WHERE SaleDate >= '2026-02-01' AND SaleDate < '2026-03-01'
GROUP BY Department
```

The page compares matching fields from both query results.

Sample for two imported files:

- Browse for File 1.
- Browse for File 2.
- Select matching key and value fields.
- Compare records, totals, differences, and missing rows.

### Profiling.aspx

Automatically profiles every field in a report or imported dataset.

Sample:

- Select Sales by Department.
- Run profiling.
- Review each field for data type, count, blanks, distinct values, minimum, maximum, average, and standard deviation where applicable.

### DataQuality.aspx

Checks data quality problems such as missing values, duplicate records, invalid dates, out-of-range numbers, inconsistent categories, and suspicious text values.

Sample:

- Select a customer dataset.
- Run data quality checks.
- Review rows for Blank Email, Invalid Date, Duplicate Customer, or Suspicious Text.
- Click the Records link to open the matching records in ShowReport.aspx.

### Ranking.aspx

Performs top, bottom, and average ranking analysis for categories, customers, products, departments, locations, or other dimensions and report groups.

Sample:

- Category Field: Product.
- Value Field: SalesAmount.
- Rank Type: Top.
- Records: 10.
- Result: top 10 products by sales, with a link to matching records.

Sample with group:

- Category Field: Product.
- Group Field: Region.
- Rank Type: Average.
- Result: average sales per product inside each region.

### Regression.aspx

Analyzes how one value changes when another value changes, and can estimate a predicted Y value when X is entered.

Supported equation options include:

- Best Fit.
- Linear.
- Quadratic.
- Cubic.
- Exponential.
- Logarithmic.
- Power.
- Logistic Probability for yes/no style outcomes.

Sample:

- X Field: AdvertisingSpend.
- Y Field: SalesAmount.
- Equation Type: Best Fit.
- Predict Y when X is: 25000.
- Result: equation, R squared, predicted Y, and links to Trends.aspx.

Sample logistic probability:

- X Field: Balance.
- Y Field: RenewedSubscription.
- Equation Type: Logistic Probability.
- Result: probability that RenewedSubscription is yes when Balance has the selected value.

### Trends.aspx

Displays an equation chart and lets the user evaluate Y for a selected X value. The chart supports clicking inside the chart area to update the active X value, zooming in and out, resetting zoom, and moving through X/Y ranges with scroll bars.

Sample:

```text
Y = 10 + 2 * X * X
X Value = 4
```

Result:

- The chart draws the curve.
- The selected point `(X,Y)` is highlighted.
- Excel export captures the chart image into an Excel workbook.
- PDF export opens the chart for browser print/PDF output.

Supported examples:

```text
Y = 4 + 2 * exp(X)
Y = 4.851 * pow(X, 0.8333)
Y = 10 + ln(X)
Y = 12 + log(X)
```

### TimeBasedSummaries.aspx

Creates time-based summaries by day, week, month, quarter, or year when date fields exist.

Sample:

- Date Field: SaleDate.
- Date Aggregation: Month.
- Value Field: SalesAmount.
- Aggregation: Sum.
- Result: monthly sales summary, with record links back to ShowReport.aspx.

### TimeSeries.aspx

Creates moving averages and rolling totals for time-series style reports.

Sample:

- Date Field: SaleDate.
- Date Aggregation: Quarter.
- Value Field: SalesAmount.
- Number of time periods: 3.
- Result: three-period moving average and rolling total, with links to source records.

### OutlierFlagging.aspx

Flags outliers using standard deviation, percentage difference, or configurable business rules.

Sample:

- Row Field: Customer.
- Value Field: OrderAmount.
- Method: Standard Deviation.
- Threshold: 2.
- Result: customers with unusually high or low order amounts, with links to the matching records.

### ChartRecommendationHelpers.aspx

Recommends chart types based on selected fields.

Sample:

- Select Category field(s), Value field(s), and optionally a Date field.
- Run recommendations.
- Review recommended chart such as Line Chart, Multi-Line Chart, Area Chart, Stepped Area Chart, Bar Chart, Column Chart, Pie Chart, Histogram, Scatter Chart, Combo Chart, Bubble Chart, Sankey Chart, Gauge, or Report and Charts.
- Click open chart or open data to open the suggested output.

Notes:

- Category field(s) is a multi-select control and can combine several fields for the chart axis.
- Value field(s) is a multi-select control limited to numeric fields.
- Selected Category field(s), Value field(s), and Date field are remembered in Session for the same report.
- Report and Charts is the fallback recommendation when the current field selection is better reviewed as report output.

### ExportPackages.aspx

Creates export packages that can include Report PDF, CSV or Excel data, report definitions, RDL, charts, AI analysis, and notes.

Sample:

- Select a report.
- Choose Report, Report Definition, Charts, AI analysis, and Notes as needed.
- Choose Data in format: CSV or Excel.
- Enter Notes if needed.
- Click Export.
- The page builds a temporary package folder, writes the selected files, zips the folder, downloads the zip, and then removes the temporary files.

The report definition section includes `ReportDefinitions.txt` from the report definition textboxes and an RDL file when an RDL definition is available. The Charts section creates chart-ready CSV files, visible SVG charts, and an HTML summary when suitable category, numeric, or date fields exist. The AI analysis section stores the generated AI output text, not the raw report data sent to AI.

### Correlation.aspx

Shows field correlation analysis.

Sample:

- Select numeric fields such as SalesAmount, DiscountAmount, Quantity, and Profit.
- Run correlation.
- Review fields with positive or negative relationships.

### CorrelationThreshold.aspx

Filters correlation results by threshold and provides specialized correlation views.

Sample:

- Select a report.
- Set threshold to 0.75.
- Show only strong positive or negative correlations.

### MultidimensionalBalancing.aspx

Supports multidimensional or matrix balancing analysis.

Sample:

- Select Scenario.
- Choose balancing dimensions.
- Review matrix totals and differences.
- Adjust or review balancing output.

## Map Pages

### MapReport.aspx

Creates map-oriented reports.

Sample:

- Select a report with latitude and longitude.
- Choose location label fields.
- Open map output.

### MapReadines.aspx

Checks map readiness for latitude/longitude quality, missing coordinates, duplicate locations, invalid coordinate ranges, and KML-ready data.

Sample:

- Open Map Readiness from MapReport.aspx.
- Select Latitude and Longitude fields.
- Run readiness checks.
- Review missing coordinates, invalid coordinate ranges, duplicates, and KML-ready rows.

### GoogleMap.aspx

Displays Google map output.

Sample:

- Open a report with coordinates.
- Display records as points on a map.
- Click points to review location details.

### MapGoogle.aspx

Supports Google map style rendering.

Sample:

- Select location fields.
- Show markers on the map.
- Review geographic distribution.

## AI Pages

### DataAI.aspx

AI-assisted data analysis page.

Sample:

- Open DataAI from a report or analytics page.
- Ask for a summary of trends, outliers, or data quality results.
- Review AI-generated interpretation.

### DataAIaddons.aspx

Provides additional AI-related tools or add-ons.

Sample:

- Open DataAI Addons.
- Review available AI helper features.
- Use an add-on with report data if enabled.

### ChatAI.aspx

Chat-style AI interface.

Sample:

- Ask: "Which departments have the largest sales variance?"
- Review the answer based on available report context.

### DataAIHelp.aspx

Help page for DataAI functionality.

Sample:

- Open DataAI Help.
- Review instructions for AI questions and report analysis.

## Scheduling Pages

### ScheduledReports.aspx

Defines scheduled report runs.

Sample:

- Select a report.
- Set daily or weekly schedule.
- Choose output format.
- Save the scheduled report.

### ScheduleReportsCalendar.aspx

Shows scheduled reports on a calendar.

Sample:

- Open report calendar.
- Review upcoming report runs.
- Select a calendar item to inspect schedule details.

### RunScheduledReports.aspx

Runs scheduled report jobs.

Sample:

- Open scheduled report runner.
- Execute due reports.
- Review run status.

### SendEmailsForScheduledReports.aspx

Sends scheduled report emails.

Sample:

- Run scheduled report email delivery.
- Review email status.
- Confirm recipients received report output.

### ScheduledDownloads.aspx

Defines scheduled downloads.

Sample:

- Select a report or export.
- Set download schedule.
- Save the scheduled download.

### ScheduledDownloadsCalendar.aspx

Shows scheduled downloads on a calendar.

Sample:

- Open downloads calendar.
- Review upcoming downloads.
- Select a download event.

### RunScheduledItems.aspx

Runs scheduled items.

Sample:

- Open scheduled item runner.
- Execute pending scheduled jobs.
- Review status messages.

## Administration And Configuration Pages

### SiteAdmin.aspx

Main site administration page.

Sample:

- Open Site Admin.
- Review application settings.
- Navigate to user, unit, or configuration pages.

### UserDefinition.aspx

Manages user definitions.

Sample:

- Create a user.
- Assign user settings.
- Save changes.

### UnitDefinition.aspx

Defines organizational units.

Sample:

- Create a unit such as Finance or Operations.
- Save unit details.

### UnitRegistration.aspx

Registers a unit.

Sample:

- Enter unit registration information.
- Submit registration.

### UnitsAdmin.aspx

Administers units.

Sample:

- Open Units Admin.
- Review existing units.
- Edit or manage unit settings.

### UnitWebOnServer.aspx

Configures or displays unit web deployment information.

Sample:

- Open unit web server settings.
- Review deployment configuration.

### ClassExplorer.aspx

Explores classes or application objects.

Sample:

- Open Class Explorer.
- Review available classes or object definitions.

### Delete.aspx

Handles delete actions.

Sample:

- Select an item to delete.
- Confirm deletion.
- Return to the previous list.

## Task Pages

### TaskList.aspx

Shows task list items.

Sample:

- Open Task List.
- Review assigned tasks.
- Open a task for details.

### TaskListCalendar.aspx

Shows tasks in calendar form.

Sample:

- Open task calendar.
- Review due dates.
- Select a task event.

### TaskListSetting.aspx

Configures task list settings.

Sample:

- Open task settings.
- Change task display or notification options.
- Save settings.

### TaskListTimeLine.aspx

Shows tasks on a timeline.

Sample:

- Open task timeline.
- Review task progress over time.

## Full Page Reference

| Page | Purpose | Sample use |
| --- | --- | --- |
| AboutUs.aspx | Application or organization information. | Review product information. |
| AddParameters.aspx | Add report parameters. | Add Region as a report parameter. |
| AdvancedAnalytics.aspx | Entry point for advanced analytics. | Open advanced analysis tools for a dataset. |
| Analytics.aspx | General report analytics. | Summarize values by category/group fields. |
| ChartGoogle.aspx | Build Google-style charts. | Create a bar chart from Category and SalesAmount. |
| ChartGoogleOne.aspx | Display one selected chart. | Open a recommended chart from analytics. |
| ChartGoogleOne.js.aspx | Chart JavaScript support. | Render the selected chart. |
| ChartRecommendationHelpers.aspx | Recommend charts from selected category, value, and date fields. | Open a recommended chart or Report and Charts fallback. |
| ChatAI.aspx | AI chat interface. | Ask for an explanation of report trends. |
| ClassExplorer.aspx | Explore application classes or objects. | Review class/object definitions. |
| ComparisonReports.aspx | Compare periods, groups, locations, queries, or imported files. | Compare January sales to February sales. |
| confirm.aspx | Confirmation page. | Confirm registration or an action. |
| ContactUs.aspx | Contact page. | Review support contact details. |
| Correlation.aspx | Field correlation analysis. | Find numeric fields related to Profit. |
| CorrelationThreshold.aspx | Filter correlations by threshold. | Show correlations above 0.75. |
| Dashboard.aspx | Dashboard display. | Review charts and summary metrics. |
| DataAdmin.aspx | Analytics Dashboard. | Open tiles for Analytics, Pivot, Regression, Time Series, Outlier Flagging, and more. |
| DataAI.aspx | AI-assisted analysis. | Ask AI to summarize data quality findings. |
| DataAIaddons.aspx | AI add-ons. | Review additional AI helper tools. |
| DataAIHelp.aspx | DataAI help. | Learn how to ask analysis questions. |
| DataAIsqlite.aspx | SQLite/lightweight data source support. | Create reports from a SQLite source. |
| DataImport.aspx | Import data files. | Import a spreadsheet or delimited file for reporting. |
| DataQuality.aspx | Data quality checks. | Find missing values, duplicates, and invalid dates. |
| Default.aspx | Application start or sign in. | Sign in to DataAI. |
| Delete.aspx | Delete handler. | Confirm deletion of a selected item. |
| ExportPackages.aspx | Export packages. | Package report PDF, CSV or Excel data, definitions, RDL, charts, AI analysis, and notes. |
| FriendlyNames.aspx | User-friendly field names. | Rename technical fields for display. |
| GoogleMap.aspx | Google map output. | Display report rows as map points. |
| HelpDesk.aspx | Support/help desk. | Review support instructions. |
| index.aspx | Entry or information page. | Navigate to the application. |
| index1.aspx | Entry or information page. | Navigate to application content. |
| Index3.aspx | Entry or information page. | Open the configured landing page. |
| IndexSoftware.aspx | Software information. | Review DataAI software modules. |
| InstallDataAI.aspx | Installation guide. | Review DataAI installation steps. |
| InstallIt.aspx | Installation support page. | Continue setup instructions. |
| ListOfDashboards.aspx | Dashboard list. | Open a selected dashboard. |
| ListOfJoins.aspx | Table joins. | Join Orders to Customers. |
| ListOfReports.aspx | Report catalog. | Open Sales by Department. |
| ListOfTables.aspx | Database table list. | Select tables for a new report. |
| MapGoogle.aspx | Google map rendering. | Show selected coordinates on a map. |
| MapReadines.aspx | Map readiness checks. | Validate latitude and longitude quality. |
| MapReport.aspx | Map report setup. | Build a location report. |
| MultidimensionalBalancing.aspx | Matrix or multidimensional balancing. | Select Scenario and review balancing results. |
| OutlierFlagging.aspx | Outlier detection. | Flag order amounts above standard deviation threshold. |
| Parameters.aspx | Report parameters. | Define Start Date and End Date. |
| Pivot.aspx | Pivot/cross-tab reports. | Sum SalesAmount by Department and Month. |
| Profiling.aspx | Automatic field profiling. | Review blanks, distinct values, min, max, average, and standard deviation. |
| QuickStart.aspx | First-step guide. | Learn how to open and run a report. |
| Ranking.aspx | Top, bottom, and average ranking. | Show top 10 products by sales. |
| RDLformat.aspx | RDL-style report format. | Edit report layout options. |
| Registration.aspx | User registration. | Register a new user. |
| Regression.aspx | Regression and prediction. | Predict SalesAmount when AdvertisingSpend is 25000. |
| ReportCopy.aspx | Copy reports. | Copy Sales by Month for a new region. |
| ReportDesigner.aspx | Design reports. | Select fields and save report design. |
| ReportEdit.aspx | Edit reports. | Change fields, filters, or labels. |
| ReportViews.aspx | Saved report views. | Reopen a filtered report view. |
| RunScheduledImports.aspx | Run import jobs. | Execute due imports. |
| RunScheduledItems.aspx | Run scheduled jobs. | Execute pending scheduled items. |
| RunScheduledReports.aspx | Run report jobs. | Execute due reports. |
| ScheduledDownloads.aspx | Scheduled download setup. | Schedule a weekly export. |
| ScheduledDownloadsCalendar.aspx | Download calendar. | Review upcoming downloads. |
| ScheduledImports.aspx | Scheduled import setup. | Schedule a daily file import. |
| ScheduledImportsCalendar.aspx | Import calendar. | Review upcoming imports. |
| ScheduledReports.aspx | Scheduled report setup. | Schedule a monthly report. |
| ScheduleReportsCalendar.aspx | Report schedule calendar. | Review upcoming report runs. |
| SendEmailsForScheduledReports.aspx | Email scheduled reports. | Send report output to recipients. |
| ShowBusinessProposal.aspx | Business proposal display. | Review business proposal information. |
| ShowCrystalReport.aspx | Crystal report output. | Open formatted Crystal report output. |
| ShowReport.aspx | Data Explorer. | Filter rows and inspect source records. |
| SiteAdmin.aspx | Site administration. | Manage application settings. |
| SQLquery.aspx | SQL query reports. | Create a report from a SQL SELECT statement. |
| TaskList.aspx | Task list. | Review assigned tasks. |
| TaskListCalendar.aspx | Task calendar. | Review tasks by due date. |
| TaskListSetting.aspx | Task settings. | Configure task list options. |
| TaskListTimeLine.aspx | Task timeline. | Review task progress over time. |
| TimeBasedSummaries.aspx | Time-based summaries. | Summarize SalesAmount by month, quarter, or year. |
| TimeSeries.aspx | Moving averages and rolling totals. | Calculate a three-period moving average by quarter. |
| Trends.aspx | Equation chart and prediction point. | Chart `Y = 10 + 2 * X * X` and select X. |
| UnderConstruction.aspx | Placeholder page. | Show a pending feature notice. |
| UnitDefinition.aspx | Unit definition. | Create an Operations unit. |
| UnitRegistration.aspx | Unit registration. | Register a new unit. |
| UnitsAdmin.aspx | Unit administration. | Edit existing unit settings. |
| UnitWebOnServer.aspx | Unit web/server configuration. | Review unit deployment settings. |
| UserDefinition.aspx | User administration. | Create or edit a user. |
| Variance.aspx | Variance and percent-change analysis. | Compare February sales to January sales. |

## Sample End-To-End Workflows

### Sample 1: Create A SQL Report And Analyze It

1. Open SQLquery.aspx.
2. Enter a SQL query that returns Department, SaleDate, and SalesAmount.
3. Save the report.
4. Open ShowReport.aspx to review records.
5. Open Pivot.aspx and summarize SalesAmount by Department and Month.
6. Open Variance.aspx to compare two periods.
7. Export the result if the page supports the desired output format.

### Sample 2: Import A File And Check Data Quality

1. Open DataImport.aspx.
2. Import a file.
3. Open Profiling.aspx to review field types, blanks, distinct values, min, max, average, and standard deviation.
4. Open DataQuality.aspx.
5. Check missing values, duplicates, invalid dates, inconsistent categories, and suspicious text.
6. Use the Records links to inspect problem rows in ShowReport.aspx.

### Sample 3: Build A Ranking Report

1. Open Ranking.aspx.
2. Select Product as the category field.
3. Select SalesAmount as the value field.
4. Choose Top, Bottom, or Average.
5. Review ranked products.
6. Click Records to inspect the source rows.

### Sample 4: Predict A Value With Regression

1. Open Regression.aspx.
2. Select X Field and Y Field.
3. Select Best Fit or a specific equation type.
4. Enter a value in Predict Y when X is.
5. Review the equation and prediction.
6. Open Trends.aspx from the Trends and Predictions link to view the equation chart.

### Sample 5: Prepare Map Data

1. Open MapReport.aspx.
2. Open MapReadines.aspx.
3. Select latitude and longitude fields.
4. Run readiness checks.
5. Fix missing or invalid coordinates.
6. Return to MapReport.aspx or GoogleMap.aspx to display the map.

### Sample 6: Package Report Outputs

1. Open ExportPackages.aspx.
2. Select Report, Report Definition, Charts, AI analysis, and Notes as needed.
3. Choose Data in format: CSV or Excel.
4. Enter package notes if needed.
5. Click Export to download the zip package.
6. Use the package for delivery, backup, or review.
