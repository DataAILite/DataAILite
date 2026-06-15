# Data Analysis Capabilities in This Project

This project can be used as a web-based data analysis, reporting, visualization, and AI-assisted insight platform. It is built around connecting to existing databases or imported files, turning data into reports and dashboards, and then applying statistical, charting, mapping, matrix-balancing, and OpenAI-assisted interpretation workflows in ASP.NET Web Forms and VB.NET.

## Data Sources That Can Be Analyzed

The project can analyze data from several kinds of sources:

- Existing relational databases through configured connection strings.
- SQL Server, MySQL, PostgreSQL, SQLite, ODBC, and OleDb-style data sources.
- Oracle, InterSystems IRIS, and InterSystems Cache when the proprietary provider code and licensed client libraries are enabled.
- Uploaded local or web files, including CSV, TXT, XML, JSON, XLS, XLSX, MDB, and ACCDB.
- Imported tables that are converted into reportable database tables.

After a connection or import is available, the application can inspect tables and fields, build SQL queries, register reports, and create reusable dashboards and analytical views.

## Core Analysis Workflows

### 1. Data Exploration

Users can browse tables, inspect fields, run SQL-based reports, filter result sets, and export data. The project supports table exploration, custom SQL queries, report views, report designer workflows, and saved report definitions.

Possible analysis:

- Review raw records returned by a report or SQL query.
- Filter records interactively.
- Explore table structures and available fields.
- Build reusable reports from imported or connected data.
- Export results to CSV or Excel for external analysis.

### 2. Descriptive Statistics

The analytics features calculate statistics for numeric fields and grouped report data.

Possible analysis:

- Count records.
- Calculate sums.
- Calculate minimum and maximum values.
- Calculate averages.
- Calculate standard deviation.
- Compare statistics across categories.
- Produce overall totals and grouped subtotals.
- Generate detail reports with category-level and overall statistics.

This is useful for operational reporting, quality checks, financial summaries, survey results, utilization reports, and any dataset where grouped numeric summaries are important.

### 3. Grouped, Cross-Tab, and Variance Analytics

The application can summarize values by one or more category fields, create matrices from report data, compare measures between selected groups or periods, and calculate contribution to total.

Possible analysis:

- Group data by category fields.
- Compare aggregated numeric values across categories.
- Build matrix-style reports where one category is rows and another category is columns.
- Analyze sums, averages, minimums, maximums, and standard deviations by group.
- Compare matrix cells and totals.
- Calculate variance between a base value and a comparison value.
- Calculate percentage change from base to comparison.
- Calculate each group contribution to the overall total.

This supports use cases such as regional comparisons, time/category summaries, department-by-service analysis, demographic cross-tabulation, budget-versus-actual review, period-over-period comparison, and category contribution analysis.

### 4. Correlation Analysis

The project includes a dedicated correlation workflow for numeric report fields.

Possible analysis:

- Identify numeric fields in a report.
- Calculate field-level statistics used for correlation.
- Calculate correlation coefficients between pairs of numeric fields.
- Display correlated field pairs.
- Export correlation results.
- Send correlation data to the AI interpretation workflow.

This is useful for finding relationships between measures, such as cost and usage, volume and wait time, revenue and staffing, or survey metrics.

### 5. Charts and Dashboards

The project uses Google Charts-style visualizations for reports, analytics, correlations, maps, and matrix outputs.

Supported chart-style analysis includes:

- Pie charts.
- Bar charts.
- Column charts.
- Line charts.
- Area charts.
- Stepped area charts.
- Scatter charts.
- Combo charts.
- Bubble charts.
- Histograms.
- Gauges.
- Sankey charts.
- Matrix charts.
- Dashboard statistics views.

These visualizations can be generated from selected report fields and aggregation functions, making the project suitable for both exploratory analysis and recurring dashboards.

### 6. Geographic and Map-Based Analysis

The project includes map report and Google Maps / Google Earth style workflows. It supports latitude/longitude fields, placemark fields, descriptions, time fields, color fields, and extruded map values.

Possible analysis:

- Plot records as map points.
- Build GeoChart and MapChart views.
- Create KML-style geographic outputs.
- Use placemark names and descriptions.
- Use longitude and latitude start/end coordinates.
- Represent routes or movements when start and end coordinates exist.
- Color or extrude map features based on selected data fields.

This is useful for location-based reporting, service-area analysis, route/movement analysis, regional performance, and any dataset with geographic coordinates.

### 7. Matrix Balancing and Multidimensional Balancing

The advanced analytics area focuses heavily on matrix balancing. It can compare a starting matrix with target totals or target matrices and calculate balanced values and balancing coefficients. It can also be used to compare two complete matrices against each other, such as one year against another year, one period against several periods, or a starting matrix against a target scenario where every cell value may be different.

Possible analysis:

- Balance a matrix to requested row and column sums.
- Balance one matrix against another target matrix.
- Compare starting, target, and balanced matrices.
- Compare two matrices cell by cell and identify which cell-level changes truly affect the final output.
- Evaluate how complex data develops over time, across scenarios, or between target and actual structures.
- Find which rows, columns, categories, or detailed cells drive the largest movement between two matrices.
- Calculate balancing coefficients.
- Measure maximum differences between balanced and target values.
- Perform partial balancing on selected parts of a matrix.
- Expand balancing by additional fields.
- Perform multidimensional balancing across multiple selected fields.

This is useful for survey weighting, demographic adjustment, allocation models, proportional fitting, reconciliation of row/column totals, and other workflows where a table of values must be adjusted to known control totals. It is also useful when two matrices are both valid but different, because matrix balancing gives users a metric-based way to see what changed inside the matrix, which cells are driving the output, and which comparisons are meaningful enough to support business or analytical conclusions.

### 8. AI-Assisted Interpretation

The project includes OpenAI-powered workflows that send selected data, analytics, maps, correlations, or matrix outputs to an AI chat page for interpretation.

Possible analysis:

- Ask the AI to interpret report data.
- Ask for meaningful analytical observations from a table.
- Interpret chart data.
- Interpret map analytics.
- Interpret correlation results.
- Interpret matrix balancing results.
- Explain which matrix cells, rows, or columns most strongly affect the balanced output.
- Summarize complex matrix development between periods, years, or scenarios in business language.
- Ask follow-up natural-language questions about the current dataset.

The AI layer is best understood as an interpretation and narrative-assistance feature. The statistical, matrix-balancing, and reporting calculations are performed by the application, and the AI can help explain patterns, summarize results, describe which differences matter, and suggest insights.

## Practical Use Cases

This project can support analysis for:

- Business reporting and operational dashboards.
- Imported spreadsheet or CSV analysis.
- Database exploration without writing custom application code.
- Statistical summaries by category.
- Relationship discovery through correlation analysis.
- Geographic and location-based reporting.
- Matrix/cross-tab analysis and balancing.
- Survey, population, or allocation weighting workflows.
- AI-generated explanations of reports and analytical outputs.
- Scheduled or repeatable reporting workflows.

## Existing ASP.NET Features

Several analysis and reporting features are already present in the project.

**`SQLquery.aspx`** already provides a report SQL query designer with:

- SQL data field selection.
- Join definition.
- Filter definition.
- Sorting.
- Report parameters.
- Query saving.
- Query-based report updates.
- Links into report data, charts, analytics, exports, and matrix balancing.

**`RDLformat.aspx`** already provides report formatting and output features with:

- Column order and expressions.
- Friendly names and formatting functions.
- Groups and totals.
- Combined column values.
- Advanced report designer navigation.
- Map definition navigation.
- Data export to Excel, CSV, delimited file, and XML.
- Report export to Excel, Word, and PDF.
- Generic report display.
- Report charts.
- Overall statistics.
- Group statistics.
- Field correlation.
- Matrix balancing.

**`Analytics.aspx`** already provides report analytics features with:

- Report data retrieval for analytics.
- Automatic analytics recalculation.
- Category/group field selection.
- Value field selection.
- Count and count-distinct calculations.
- Sum, maximum, minimum, average, standard deviation, and value calculations for numeric fields.
- Generated group analytics records.
- Correlation display for selected fields.
- Matrix graph links.
- Bar, pie, and line graph links.
- Detail reports with category totals and statistics.
- Statistics dashboard links.
- Google chart links.
- Advanced analytics and matrix-balancing navigation.
- AI interpretation link for analytical output.

**`Pivot.aspx`** provides pivot-style cross-tab analysis with:

- Row field selection.
- Column field selection.
- Value field selection.
- Aggregation options including count, count distinct, sum, minimum, maximum, average, standard deviation, and value.
- Search filtering across the current report result.
- Row totals, column totals, and grand total.
- CSV and Excel export.
- AI interpretation link for pivot output.

Model: Pivot cross-tab summary. Inputs are the current report records after search filtering, the selected Row field, Column field, Value field, and Aggregate option.

Algorithm: Each record is assigned to a row group and column group, then the value field is accumulated by the selected aggregate such as Sum, Count, Average, Minimum, or Maximum. The grid is rebuilt from grouped buckets so row/column intersections show comparable totals.

Output: The first columns identify row-field values, each generated pivot column represents a column-field value, and cells show the calculated aggregate for that intersection. Blank or zero cells mean no matching records or no usable numeric value for that intersection.

**`Variance.aspx`** provides percentage-change, variance, and contribution-to-total analysis with:

- Analysis type selection for variance, percentage change, and contribution to total.
- Group field selection.
- Compare field selection.
- Base value and comparison value selection.
- Value field selection.
- Aggregation options including sum, average, and count.
- Search filtering across the current report result.
- Base value, comparison value, variance, and percent-change output.
- Group contribution-to-total output with total row.
- CSV and Excel export.
- AI interpretation link for variance and contribution output.

Model: Variance, percent-change, and contribution-to-total analysis. Inputs are grouping fields, base value field, comparison value field, and any search restriction on the current report data.

Algorithm: Records are grouped by the selected dimension, base and comparison values are aggregated separately, variance is calculated as Compare minus Base, percent change is calculated from Base where possible, and contribution shows the row share of total comparison amount.

Output: The grid shows each dimension/group, base value, comparison value, variance, percent change, contribution to total, and record counts. Positive variance means the compare value is higher than base; negative variance means it is lower.

**`Profiling.aspx`** provides automatic profiling for every field in the current report or imported dataset with:

- Source data type and detected data type.
- Nonblank count and blank count.
- Distinct value count.
- Minimum and maximum values where applicable.
- Average and standard deviation for numeric values.
- Search filtering across the current report result before profiling.
- CSV and Excel export.
- AI interpretation link for profiling output.

Model: Automatic field profiling for the current report or imported dataset. Inputs are all available columns and records matching the current search text.

Algorithm: The page scans every field, detects usable data type patterns, counts total and blank values, counts distinct values, and calculates numeric or date statistics where the column values support those calculations.

Output: The grid lists one row per field with detected type, count, blanks, distinct values, minimum, maximum, average, standard deviation, and notes. Numeric statistics are populated only for fields that can be interpreted as numbers.

**`DataQuality.aspx`** provides data quality checks for the current report or imported dataset with:

- Missing value checks for every field.
- Duplicate full-record checks.
- Invalid date checks for date-like text values.
- Out-of-range numeric checks based on configurable standard-deviation limits.
- Inconsistent category checks for case, spacing, or punctuation variants.
- Suspicious text checks for leading/trailing spaces, control characters, very long text, markup-like text, and repeated character patterns.
- Search filtering across the current report result before checking quality.
- CSV and Excel export.
- AI interpretation link for data quality output.

Model: Data quality rule checks for the current report data. Inputs are all fields, current search text, and the configured numeric standard-deviation threshold.

Algorithm: The page evaluates missing values, duplicate rows, invalid date-like values, out-of-range or suspicious numeric values, inconsistent category spelling/casing, and suspicious text patterns. Each issue is registered with a filter that can reopen affected records.

Output: The grid shows issue type, field, affected record count, and issue description. Record-count links open the exact rows in Data Explorer so questionable values can be reviewed in context.

**`Ranking.aspx`** provides ranking and top/bottom analysis for categories, customers, products, departments, locations, report groups, or other dimensions with:

- Rank field selection for the item or dimension being ranked.
- Optional within-group field selection for ranking separately inside report groups.
- Value field selection.
- Aggregation options including sum, average, minimum, maximum, count, and count distinct.
- Top, bottom, or average-nearest ranking mode.
- Top Value, Bottom Value, or Average Value output depending on the selected ranking mode.
- Group Top Value, Group Bottom Value, or Group Average Value output when within-group ranking is selected.
- Drill-down links from Records values to Data Explorer for the matching records.
- Configurable number of ranked rows.
- Search filtering across the current report result before ranking.
- CSV and Excel export.
- AI interpretation link for ranking output.

Model: Ranking and top/bottom/average analysis for categories or other dimensions. Inputs are the selected rank field, optional group field, value field, rank type, top count, and search text.

Algorithm: Records are grouped by rank field and optional group, the selected value field is aggregated, then rows are sorted for Top, Bottom, or Average ranking. Group value columns show the selected rank result inside each group when a group field is used.

Output: The grid shows the ranked dimension, optional group, rank type, calculated top/bottom/average value, group value when applicable, and record count. Record links open the rows used to calculate each ranked result.

**`ComparisonReports.aspx`** provides comparison reports between two periods, two groups, two locations, two queries, or two imported files with:

- Comparison type selection for periods, groups, locations, queries, or imported files.
- Row field selection for grouping the comparison output, with an all-records option.
- Compare field selection for choosing the field that identifies the two values being compared.
- Base value and compare value selection.
- Value field and aggregation options including sum, average, minimum, maximum, count, and count distinct.
- Output columns for comparison type, row/group, base value, compare value, variance, percent change, base records, and compare records.
- Search filtering across the current report result before comparison.
- CSV and Excel export.
- AI interpretation link for comparison output.

Model: Two-source comparison analysis. Inputs can be two periods, two groups, two locations, two SQL queries, or two imported files, plus the row field, value field, aggregate option, base value, compare value, and search text.

Algorithm: The page builds base and compare datasets, groups both sides by the selected row dimension, aggregates the selected value field, matches groups by row value, and calculates Compare minus Base plus percent change from Base where Base is not zero.

Output: The grid shows comparison type, row dimension, Base value, Compare value, Variance, Percent Change, Base Records, and Compare Records. Base Records and Compare Records link to the exact rows used for each side of the comparison.

When `Two Imported Files` is selected, the page shows two browse controls: one for the base file and one for the compare file. After the user selects both files and clicks Build, the files are read into memory, marked internally as `Base` and `Compare`, and combined into one temporary comparison dataset. The selected row field, value field, and aggregation are then used to calculate base value, compare value, variance, percent change, base records, and compare records. For example, two files with `Department` and `Sales` columns can be compared by department using Sum of Sales. The direct file comparison supports delimited CSV, TSV, and TXT files, and the analytical file data is not stored permanently.

**`Regression.aspx`** provides regression analysis to understand and predict how one numeric column changes when another numeric column changes with:

- Selection of an independent X value field and a dependent Y field to predict.
- Optional group field selection to calculate separate regression lines by category.
- Equation type selection for best fit, linear, quadratic, cubic, exponential, logarithmic, power, and logistic probability models.
- Logistic regression for binary yes/no style outcome fields, returning probability from 0 to 1.
- Optional prediction input for estimating Y when X has a selected value.
- Regression output including records, equation type, equation, slope/intercept where applicable, coefficients, correlation, R squared, average X, average Y, min X, max X, and predicted Y.
- Trends and Predictions link from each regression row to an interactive trend chart.
- Search filtering across the current report result before regression.
- CSV and Excel export.
- AI interpretation link for regression output.

Model: Regression and prediction analysis for numeric relationships. Inputs are selected X field, Y field, optional group field, equation type, and optional prediction X value.

Algorithm: The page collects numeric X/Y pairs, optionally separates them by group, fits the selected model type, calculates coefficients and prediction values, and creates a trend link with the equation and selected fields.

Output: The grid shows group, X field, Y field, equation, coefficient details, predicted Y where requested, record count, and a Trends link. Record links open the rows used to fit each equation.

The `Predict Y when X is` value is optional. It does not change the regression calculation itself; coefficients, correlation, and R squared are calculated from the report data. When the user enters an X value, the page substitutes that value into the selected regression equation, including linear, polynomial, exponential, logarithmic, power, or logistic probability equations, and displays the estimated result in the `Predicted Y` column.

**`Trends.aspx`** provides interactive trend and prediction charting from a regression equation with:

- Equation textbox and X Value textbox populated from **`Regression.aspx`**.
- Support for equations using arithmetic, powers, `pow`, `exp`, `ln`, `log`, square root, trigonometric functions, constants `E` and `PI`, and polynomial forms.
- X and Y axes with automatically adjusted ranges.
- A highlighted `(X,Y)` point on the trend line for the selected X value.
- Clickable chart area that updates the active X value and moves the selected point to the clicked X coordinate.
- Zoom In, Zoom Out, Reset Zoom, and X/Y range scroll bars that change the visible range while keeping the chart area size stable.
- Excel export that captures the chart image into an Excel workbook.
- PDF export through the browser print/PDF workflow.

**`TimeBasedSummaries.aspx`** provides time-based summaries when date fields exist with:

- Date field selection.
- Period selection by day, week, month, quarter, or year.
- Value field and aggregation selection.
- Output by period with period start, record count, and calculated value.
- Records links from each period row back to the matching source records in Data Explorer.
- CSV and Excel export.

Model: Period-based summary analysis for reports with date fields. Inputs are selected date field, aggregation period, value field, aggregate option, and search text.

Algorithm: Dates are normalized into day, week, month, quarter, or year buckets. Values inside each bucket are aggregated using the selected calculation, and each period receives a filter back to contributing records.

Output: The grid shows time period, record count, selected aggregate value, and record links. Links open records in the selected period so totals can be traced back to report data.

**`TimeSeries.aspx`** provides time-series style rolling calculations with:

- Date field and value field selection.
- Date aggregation by day, month, quarter, or year.
- Configurable number of time periods for the moving calculation window.
- Moving average and rolling total output.
- Records links from each period row back to the matching source records in Data Explorer.
- CSV and Excel export.

Model: Time-series rolling analysis. Inputs are date field, value field, date aggregation period, number of time periods, calculation type, and optional search text.

Algorithm: Records are grouped into ordered time periods, period totals are calculated, and the selected rolling window is applied to produce moving averages or rolling totals across consecutive periods.

Output: The grid shows each period, records, period total, moving average or rolling total, and record links. Number of time periods controls how many prior periods are included in each rolling calculation.

**`OutlierFlagging.aspx`** provides outlier checks with:

- Numeric field selection.
- Standard-deviation, percentage-difference, or business-rule method selection.
- Configurable standard-deviation threshold, percent threshold, business minimum, and business maximum.
- Output showing flagged rows, value, method, reason, average, and standard deviation.
- Row field links back to the matching source records in Data Explorer.
- CSV and Excel export.

Model: Outlier detection for numeric values. Inputs are selected row field, value field, rule type, threshold settings, and search text.

Algorithm: The page calculates baseline statistics such as average and standard deviation or applies percent/business-rule thresholds, then flags records whose values are outside selected rule limits.

Output: The grid shows row value, numeric value, rule, threshold, difference, average, standard deviation, and a reason explaining why the row was flagged. Row links open flagged records in Data Explorer.

**`AuditSummaries.aspx`** provides audit-style summaries showing which settings produced an analytical result with:

- Analysis type selection.
- Result name and selected report fields.
- Filter/search text, thresholds, aggregation options, and notes.
- Paged grid output and CSV/Excel export.
- AI interpretation link for the audit summary.

Model: Audit summary for analytical traceability. Inputs are selected report fields, filters, thresholds, aggregation choices, search text, and current report context.

Algorithm: The page records the analytical settings that produced the result, including field selections, filter text, threshold values, aggregation options, and result counts so the analysis can be reviewed later.

Output: The grid shows analysis type, selected fields, filters, thresholds, aggregation options, records/results affected, and audit notes. It explains which choices produced each analytical result.

**`ChartRecommendationHelpers.aspx`** provides chart recommendation helpers based on selected fields with:

- Category field(s), date field, and numeric Value field(s) selection.
- Multi-select category and value controls for combined category axes and multiple numeric Y fields.
- Session-based restoration of selected Category field(s), Value field(s), and Date field for the same report.
- Recommendations for line, area, stepped area, bar, column, pie, histogram, scatter, combo, bubble, Sankey, gauge, and Report and Charts output.
- Automatic recommendation generation across available Category, Date, and Value fields when no field restriction is selected.
- Restriction of generated recommendation output to 1000 rows for performance, while preserving a balanced sample of category and value combinations.
- Priority assignment, including Highest priority rows for unique field combinations with evenly distributed dashboard-safe chart types.
- Category combinations up to two fields and Value combinations up to three numeric fields for Highest-priority dashboard suggestions.
- Multi-value chart support for chart types that support multiple Y values, such as Area, Stepped Area, Line, and Column.
- Bubble, Sankey, and Gauge recommendations are available to open, but they are not assigned Highest priority and their dashboard checkboxes are disabled.
- Index/ID-like numeric fields are demoted so they are not promoted into Highest-priority recommendations.
- Add to Dashboard checkboxes for choosing which recommended charts should be added to a dashboard.
- Create Dashboard button that creates a new dashboard from checked rows on the current grid page; when no rows are checked, it uses dashboard-safe Highest-priority recommendations from the current page.
- Dashboard validation that skips broken chart tiles or recommendations that do not produce usable dashboard data.
- Explanation of why each chart type is suitable.
- Open Chart links that send selected chart type and fields to **`ChartGoogleOne.aspx`**.
- Report and Charts fallback link for returning to report/chart output when the selected fields are better reviewed as report data.
- CSV and Excel export.

**`ExportPackages.aspx`** provides export package support with:

- Checkboxes for Report, Report Definition, Charts, AI analysis, and Notes.
- Data in format selection for CSV or Excel.
- User-entered Notes textbox.
- Export button that creates a temporary package folder in `Temp`, writes selected package files, zips the folder, downloads the zip file, and then removes the temporary folder and zip.
- Report PDF export using the current report definition and current report data.
- Report definition export as both `ReportDefinitions.txt` and an RDL file when available.
- CSV or Excel data export for the current report data.
- Chart package output with chart-ready CSV files, visible SVG charts, and an HTML chart summary for bar, pie, and time-based chart data when suitable fields exist.
- AI analysis output saved as real generated AI text when OpenAI settings are configured, without including the raw data sent to AI in the final AI analysis file.

**`CorrelationThreshold.aspx`** provides correlation threshold filters and specialized correlation views with:

- Minimum absolute correlation threshold.
- Positive-only, negative-only, or combined correlation views.
- Search filtering by field name.
- Stored correlation results when available, with live numeric-field correlation fallback.
- CSV and Excel export.

Model: Correlation threshold analysis for numeric field relationships. Inputs are eligible numeric fields, threshold direction, threshold value, and search text.

Algorithm: The page calculates pairwise correlation values between numeric fields, compares each pair to the selected threshold, and classifies relationship direction and strength.

Output: The grid shows field pair, correlation value, threshold match, direction, and interpretation. Values close to 1 indicate strong positive movement together; values close to -1 indicate strong opposite movement.

**`MapReadines.aspx`** provides map readiness checks before map report output with:

- Automatic scan of report fields for possible latitude and longitude candidates.
- Exclusion of index and ID-like fields from coordinate suggestions.
- Suggested coordinate fields and map suitability messages.
- Latitude, longitude, and name field selection.
- Checks for missing coordinates, invalid coordinate ranges, duplicate locations, and KML-ready records.
- Record-count links back to affected rows in Data Explorer.

Model: Map readiness and coordinate quality review for the current report data.

Algorithm: The page scans fields for likely coordinate names and numeric coordinate patterns, checks selected coordinate fields for missing values, out-of-range values, duplicate latitude/longitude pairs, and KML-ready rows, and creates filters for each issue category.

Output: The green readiness panel explains whether the dataset appears map-ready and suggests possible coordinate fields. The grid shows each map-readiness check, affected count, and linked records where applicable.

**`DataAdmin.aspx`** provides an analytics dashboard overview for the current report with:

- Dashboard tiles for Analytics, Data Overall Statistics, Groups Statistics, Correlation, DataAI, Pivot / Cross Tab, Variance Analysis, Comparison Reports, Data Profiling, Data Quality, Ranking Analysis, Regression Analysis, Time Based Summaries, Time Series, Outlier Flagging, and Matrix Balancing.
- Left-menu navigation under Analytics Dashboard, with Detail Analytics linking to **`Analytics.aspx`**.
- Small live preview grids generated from the current report data in memory.
- Analytics tile preview based on the same analytics groups table used by **`Analytics.aspx`**.
- Data Overall Statistics tile preview based on the same statistics table used by the top statistics grid in **`ShowReport.aspx?srd=8`**.
- DataAI tile preview showing a five-row by five-column sample from the current report data.
- Variance tile preview showing real base and compare values, variance, percent change, and records from the current report data where grouping fields are available.
- Pivot and Matrix preview grids shaped as compact cross-tab summaries.
- Data Quality, Profiling, Ranking, Regression, Time Based Summaries, Time Series, Outlier Flagging, Correlation, and Groups preview grids shaped to match the purpose of their corresponding analysis pages.
- Open links from each tile to the full analysis page.

### Market and Business Model Pages

The project now includes ASP.NET Market pages for business-model style analysis. These pages use the current report data when a report is selected; when no report data is available, they can use the sample market retail dataset in `SampleData/MarketRetailSales.csv`. The Market pages share common behavior: field dropdowns are populated from the current dataset, Search filters the in-memory source records before calculations, Records values link back to matching records in Data Explorer, CSV and Excel exports are available, and the AI button sends the calculated market output for interpretation.

Market pages now also support richer field grouping where the model benefits from it. Demand, Segments, Risk, Inventory, Profit, and Scenario can use a multi-selected Primary Field list. When more than one Primary Field is selected, the selected field values are combined into one grouped key using ` | `, so a result can be grouped by combinations such as region plus product, customer plus segment, or location plus category. Basket, Pricing, and Churn keep a single Primary Field because their current algorithms depend on one item, price, or customer/segment key.

**`MarketAdmin.aspx`** provides the Market Dashboard with:

- One dashboard tile for each Market page.
- Live preview grids generated from current report data or sample market data.
- Open links to Demand, Pricing, Elasticity, Basket, Segments, Churn, Risk, Inventory, Profit, and Scenario pages.
- Left-menu navigation using the same Analytics and Market menu structure propagated from **`DataAdmin.aspx`**.

**`MarketDemand.aspx`** provides demand modeling with:

- Inputs: multi-select Primary Field for the market dimension combination, Value Field for the demand measure, optional Date Field for period-based demand, Date Aggregation for Day, Week, Month, Quarter, or Year, Assumption % for projected demand, and Search text.
- Model: grouped demand model by category, product, customer, location, or combined selected dimensions.
- Algorithm: records are filtered by Search, grouped by the selected Primary Field value or combined Primary Field values, and the Value Field is summed for each group. The page calculates record count, share of total demand, and projected demand as `Demand Value * (1 + Assumption %)`. When Date Field is selected, records are grouped by Primary Field combination and selected Date Aggregation so demand can be reviewed by day, week, month, quarter, or year.
- Output: Dimension, optional Period, Demand Value, Records, Share %, Projected Demand, and hidden FilterId for Data Explorer links.

**`MarketPricing.aspx`** provides pricing sensitivity analysis with:

- Inputs: Value Field as the price field, Secondary Field as quantity/units, optional Primary Field for market context, and Search text. Primary Field can be `(None)` to calculate by price band only. Date Field and Assumption % are hidden because they are not used by this calculation.
- Model: price-band sensitivity model comparing volume and revenue behavior across price ranges, optionally by a selected market dimension.
- Algorithm: records are filtered by Search and grouped into price bands from the selected Value Field. If Primary Field is `(None)`, results are grouped by price band only. If a Primary Field is selected, results are grouped by Dimension plus Price Band. The selected Secondary Field is treated as quantity or units, and the page calculates record count, average quantity, average revenue, and a sensitivity note based on relative volume.
- Output: Dimension, when shown, is the selected Primary Field value used to split the pricing result. Price Band is the calculated range for the selected Value Field. Records is the count of matching source rows and links to those records. Average Quantity is the average selected Secondary Field quantity or units in the band. Average Revenue is average calculated revenue, price times quantity where quantity is available. Sensitivity Note flags whether that band has higher or lower unit volume compared with the built-in volume threshold.

**`MarketElasticity.aspx`** provides pricing elasticity analysis with:

- Inputs: Value Field as price, Secondary Field as quantity/units, optional Primary Field for market context, Assumption %, and Search text. Date Field is hidden because it is not used by this calculation.
- Model: price elasticity model comparing price movement with quantity or volume movement.
- Algorithm: records are filtered by Search, grouped by price band and optional Primary Field context, and summarized for average price, quantity sold, revenue, and elasticity behavior. The page estimates elasticity by comparing quantity change to price change between price bands. Assumption % is treated as a possible price change and is used to project price, quantity, revenue, and revenue impact based on the calculated elasticity.
- Output: Dimension, Price Band, Average Price, Quantity Sold, Revenue, Price Change %, Quantity Change %, Elasticity, Assumption Price Change %, Projected Price, Projected Quantity, Projected Revenue, Revenue Impact, Elasticity Note, Records, and hidden FilterId for Data Explorer links.

**`MarketBasket.aspx`** provides market-basket co-occurrence analysis with:

- Inputs: Primary Field as the item/product/category, Secondary Field as order, transaction, or invoice ID, optional Value Field for weighted basket value, and Search text. Assumption % is hidden because it is not used by this calculation.
- Model: co-occurrence model for finding items that appear together in the same transaction.
- Algorithm: records are filtered by Search, grouped into transactions using Secondary Field, and unique Primary Field item values are collected per transaction. The page counts every item pair that appears together, calculates support as `Orders Together / Total Orders`, and sums Weighted Basket Value from the selected Value Field for matching pair records.
- Output: Item A and Item B are the two Primary Field values found together in the same transaction, order, or invoice. Records is the number of transactions containing the pair and links to matching rows. Support % is the share of checked transactions that contain the pair. Weighted Basket Value is the sum of the selected Value Field for matching pair rows. Basket Note identifies the pair as a candidate for bundle, cross-sell, or co-occurrence review.

**`MarketSegments.aspx`** provides customer, product, or market segmentation summaries with:

- Inputs: multi-select Primary Field for the segment combination, Value Field for the segment measure, and Search text. Date Field, Secondary Field, and Assumption % are hidden because they are not used by this calculation.
- Model: grouped segmentation model comparing market, customer, product, or location segments by value concentration and average value.
- Algorithm: records are filtered by Search, grouped by one or more selected Primary Fields, and the Value Field is summed and averaged for each segment. Each segment is compared with the overall average to assign a segment note.
- Output: Segment is the combined selected Primary Field value. Records is the count of source rows in that segment and links to those records. Value is the sum of the selected Value Field for the segment. Average Value is Value divided by Records. Segment Note compares the segment average with the overall average and labels the segment as above or below average.

**`MarketChurn.aspx`** provides churn and retention scoring with:

- Inputs: Primary Field as customer or segment, Value Field as customer/segment value, Date Field as activity date, Date Aggregation for Day, Week, Month, Quarter, or Year, and Search text. Secondary Field and Assumption % are hidden because they are not used by this calculation.
- Model: recency-based retention and churn review model.
- Algorithm: records are filtered by Search and grouped by customer or segment. The page keeps each group's latest activity date, sums the Value Field, compares latest activity with the latest date in the data, and calculates a retention score. When Date Field is selected, results can also be grouped by selected Date Aggregation.
- Output: Customer / Segment is the selected Primary Field value being scored. Period appears when Date Field is selected and shows the selected day, week, month, quarter, or year bucket. Records is the number of matching rows and links to those records. Last Activity is the latest date found for the group or period. Value is the sum of the selected Value Field. Retention Score is a recency score where more recent activity scores higher. Churn Note flags recently active groups versus groups that should be reviewed for churn risk.

**`MarketRisk.aspx`** provides market risk scoring with:

- Inputs: multi-select Primary Field for the risk dimension combination, Value Field as exposure/value, Assumption %, and Search text. Date Field and Secondary Field are hidden because they are not used by this calculation.
- Model: relative exposure risk model.
- Algorithm: records are filtered by Search, grouped by the selected Primary Field value or combined Primary Field values, and the Value Field is summed as exposure. Each group's exposure is compared with the maximum group exposure, producing a Risk Score from 0 to 100. Notes classify groups as high, medium, or lower exposure.
- Output: Dimension, Records, Value, Risk Score, Risk Note, and hidden FilterId for Data Explorer links.

**`MarketInventory.aspx`** provides inventory movement analysis with:

- Inputs: multi-select Primary Field as item/product/category or combined inventory dimension, Value Field as units or movement value, optional Date Field for period-based movement, Date Aggregation for Day, Week, Month, Quarter, or Year, Assumption %, and Search text. Secondary Field is hidden because it is not used by this calculation.
- Model: grouped inventory movement and velocity model.
- Algorithm: records are filtered by Search, grouped by the selected Primary Field value or combined Primary Field values, and the Value Field is summed as Units / Movement. Velocity is calculated as movement divided by record count. The page uses the selected Current Inventory field, or auto-detects inventory, stock, on-hand, available, or balance fields when present. Assumption % is treated as safety stock and is added to velocity to calculate Reorder Point. Supply Periods are calculated as Current Inventory divided by Velocity, and Reorder Needed is Yes when Current Inventory is less than or equal to Reorder Point. When Date Field is selected, records are grouped by Primary Field combination and selected Date Aggregation so movement can be reviewed by day, week, month, quarter, or year.
- Output: Item, optional Period, Units / Movement, Records, Velocity, Inventory Field, Current Inventory, Supply Periods, Safety Stock %, Reorder Point, Reorder Needed, Inventory Note, and hidden FilterId for Data Explorer links.

**`MarketProfit.aspx`** provides profitability driver analysis with:

- Inputs: multi-select Primary Field for the profitability driver combination, Value Field as revenue/value, Assumption % as estimated cost rate, and Search text. Date Field and Secondary Field are hidden because they are not used by this calculation.
- Model: simple profitability driver model using revenue, estimated cost, estimated profit, and margin.
- Algorithm: records are filtered by Search, grouped by the selected Primary Field value or combined Primary Field values, and the Value Field is summed as revenue. The page first searches for total cost, extended cost, cost amount, direct cost, or expense fields. If no total cost field is available, it searches for unit cost and quantity fields and calculates direct cost as unit cost times quantity. If no usable direct cost exists, Estimated Cost is calculated as `Revenue * Assumption %`; when the assumption is zero, the model defaults to a 65% cost rate. Estimated Profit is `Revenue - Estimated Cost`, Margin % is calculated from profit divided by revenue, and Profit Contribution % shows each group's share of total profit.
- Output: Driver, Revenue, Direct Cost, Cost Source, Cost Rate %, Estimated Cost, Estimated Profit, Margin %, Profit Contribution %, Profit Note, Records, and hidden FilterId for Data Explorer links.

**`MarketScenario.aspx`** provides scenario modeling for market assumptions with:

- Inputs: multi-select Primary Field for the scenario dimension combination, Value Field as current value, Assumption % as the scenario change, and Search text. Date Field and Secondary Field are hidden because they are not used by this calculation.
- Model: what-if scenario model for changing current values by a selected assumption percentage.
- Algorithm: records are filtered by Search, grouped by the selected Primary Field value or combined Primary Field values, and the Value Field is summed as Current Value. Assumption % creates a downside value as `Current Value * (1 - Abs(Assumption %))`, a base value as the current value, and an upside value as `Current Value * (1 + Abs(Assumption %))`. The page calculates downside difference, upside difference, scenario range, and a range note.
- Output: Dimension, Current Value, Downside Value, Base Value, Upside Value, Downside Difference, Upside Difference, Scenario Range, Assumption %, Scenario Note, Records, and hidden FilterId for Data Explorer links.

Other existing pages also cover major capabilities:

- **`Correlation.aspx`** provides dedicated field-correlation analysis and export.
- **`AdvancedAnalytics.aspx`** provides matrix balancing and advanced matrix workflows.
- **`MultidimensionalBalancing.aspx`** provides multidimensional balancing workflows.
- **`MapReport.aspx`** provides map report definition and geographic output workflows.
- **`MapReadines.aspx`** provides map readiness checks for latitude/longitude quality, missing coordinates, duplicate locations, invalid coordinate ranges, and KML-ready data.
- **`DataImport.aspx`** provides import workflows for CSV, Excel, XML, JSON, Access, and related report creation.
- **`ScheduledImports.aspx`**, **`ScheduledReports.aspx`**, and related calendar/run pages provide scheduled imports and scheduled reporting workflows.

## ASP.NET Analysis Features That Can Still Be Programmed

The current ASP.NET project can still be extended with additional analysis screens, report actions, and reusable VB.NET helper functions. Many earlier ideas have now been implemented as ASP.NET pages and are no longer pending items, including pivot/cross-tab reports, variance analysis, comparison reports, profiling, data quality checks, ranking, regression, trends and predictions, time-based summaries, time-series rolling calculations, outlier flagging, chart recommendation helpers, export packages, correlation threshold views, map readiness checks, audit summaries, analytics dashboard tiles, dashboard navigation, and Market/business model pages.

Practical remaining extensions include:

- Reusable analysis templates so common field selections, thresholds, grouping choices, chart choices, and export options can be saved and reused per report or per user.
- Configurable default dashboard layouts for **`DataAdmin.aspx`**, including user-selected tile order, visible tiles, preferred preview style, and saved dashboard presets.
- Cross-report analysis packages that compare selected metrics across several saved reports, departments, locations, or periods.
- Shared Word and PDF export options for analysis pages that currently export only CSV or Excel.
- More complete analysis notes and report narratives that combine selected charts, data-quality warnings, regression/trend output, and AI interpretation into one review package.
- Rule-based alerting for analytical thresholds, such as outliers above a selected limit, missing-value rates, strong correlations, large variances, or map-readiness failures.
- More advanced what-if extensions beyond **`MarketScenario.aspx`**, such as saved multi-assumption sets, comparison of several named scenarios side by side, and scenario export packages that include inputs, calculations, charts, and notes.
- Optional machine-learning style extensions where they fit the project, such as classification for yes/no outcomes, clustering for similar records or customers, anomaly scoring, simple recommendation scoring, and train/test evaluation summaries. These should be implemented as transparent ASP.NET workflows with visible fields, filters, model settings, and exportable results.
- Forecasting extensions for business time series, such as trend projection, seasonality summaries, moving-average forecasts, confidence bands, and forecast-versus-actual review.
- Additional future Market model refinements where the available data supports them, such as seasonal elasticity, vendor lead-time based reorder points, richer profit allocation rules, and saved multi-assumption scenario sets.
- Model governance and audit views for any future machine-learning or advanced market-model pages, showing selected fields, filters, training period, validation period, assumptions, coefficients or feature importance where available, result counts, and exportable model notes.

## Best Fit

The best fit for this ASP.NET project is practical business data analysis: descriptive statistics, report exploration, dashboards, correlation, data quality checks, geographic analysis, matrix/cross-tab analysis, matrix balancing, scheduled reporting, and AI-assisted explanation.

The quality of analysis depends on the connected data source, the fields selected for a report, the configured database provider, and whether OpenAI credentials and optional map/provider settings are configured.
