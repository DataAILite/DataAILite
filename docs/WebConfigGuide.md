# DataAILite web.config Configuration Guide

This guide walks you through configuring the `web.config` file for your DataAILite installation. The file is located in the root of your DataAILite web application directory.

## Before You Begin

Make a backup copy of `web.config` before making any changes:

```
copy web.config web.config.backup
```

Open the file in any text editor (Notepad, VS Code, Notepad++). All values you need to change are shown as `[placeholder text]` in the default file.

---

## Step 1: Database Connection Strings

The `<connectionStrings>` section defines three connections. Uncomment the block that matches your database engine and fill in your server details.

**MySqlConnection** — DataAILite's operational database (stores reports, users, settings).
**UserSqlConnection** — Your data database (the data you want to analyze).
**CSVconnection** — Used for CSV file import operations (usually points to the same database as UserSqlConnection).

### MySQL (recommended)

```xml
<add name="MySqlConnection"
     connectionString="Server=YOUR_SERVER; Database=YOUR_DB_NAME; User ID=YOUR_USER; Password=YOUR_PASSWORD;"
     providerName="MySql.Data.MySqlClient" />
<add name="UserSqlConnection"
     connectionString="Server=YOUR_DATA_SERVER; Database=YOUR_DATA_DB; User ID=YOUR_USER; Password=YOUR_PASSWORD;"
     providerName="MySql.Data.MySqlClient" />
<add name="CSVconnection"
     connectionString="Server=YOUR_DATA_SERVER; Database=YOUR_DATA_DB; User ID=YOUR_USER; Password=YOUR_PASSWORD;"
     providerName="MySql.Data.MySqlClient" />
```

### SQL Server

```xml
<add name="MySqlConnection"
     connectionString="Server=YOUR_SERVER; Database=YOUR_DB_NAME; User ID=YOUR_USER; Password=YOUR_PASSWORD; Trusted_Connection=True"
     providerName="System.Data.SqlClient" />
<add name="UserSqlConnection"
     connectionString="Server=YOUR_DATA_SERVER; Database=YOUR_DATA_DB; User ID=YOUR_USER; Password=YOUR_PASSWORD; Trusted_Connection=True"
     providerName="System.Data.SqlClient" />
<add name="CSVconnection"
     connectionString="Server=YOUR_DATA_SERVER; Database=YOUR_DATA_DB; User ID=YOUR_USER; Password=YOUR_PASSWORD; Trusted_Connection=True"
     providerName="System.Data.SqlClient" />
```

### PostgreSQL

```xml
<add name="MySqlConnection"
     connectionString="Server=YOUR_SERVER; Port=5432; Database=YOUR_DB_NAME; User ID=YOUR_USER; Password=YOUR_PASSWORD;"
     providerName="Npgsql" />
<add name="UserSqlConnection"
     connectionString="Server=YOUR_DATA_SERVER; Port=5432; Database=YOUR_DATA_DB; User ID=YOUR_USER; Password=YOUR_PASSWORD;"
     providerName="Npgsql" />
<add name="CSVconnection"
     connectionString="Server=YOUR_DATA_SERVER; Port=5432; Database=YOUR_DATA_DB; User ID=YOUR_USER; Password=YOUR_PASSWORD;"
     providerName="Npgsql" />
```

### Oracle

Oracle also requires a fourth `SystemSqlConnection` line.

```xml
<add name="SystemSqlConnection"
     connectionString="Data Source=YOUR_DATA_SOURCE; User ID=SYS; Password=YOUR_SYS_PASSWORD; DBA Privilege=SYSDBA"
     providerName="Oracle.ManagedData.Client" />
<add name="MySqlConnection"
     connectionString="Data Source=YOUR_DATA_SOURCE; User ID=YOUR_USER; Password=YOUR_PASSWORD;"
     providerName="Oracle.ManagedData.Client" />
<add name="UserSqlConnection"
     connectionString="Data Source=YOUR_DATA_SOURCE; User ID=YOUR_USER; Password=YOUR_PASSWORD;"
     providerName="Oracle.ManagedData.Client" />
<add name="CSVconnection"
     connectionString="Data Source=YOUR_DATA_SOURCE; User ID=YOUR_USER; Password=YOUR_PASSWORD;"
     providerName="Oracle.ManagedData.Client" />
```
**License required:** Oracle Database requires a valid commercial license from Oracle Corporation. DataAI does not provide or distribute Oracle software. Visit https://www.oracle.com for licensing information.

### InterSystems IRIS

Default port is 1972 or 51773. IRIS also requires a `SystemSqlConnection` line.

```xml
<add name="SystemSqlConnection"
     connectionString="Server=YOUR_SERVER; Port=1972; Namespace=%SYS; User ID=_SYSTEM; Password=YOUR_SYS_PASSWORD"
     providerName="InterSystems.Data.IRISClient" />
<add name="MySqlConnection"
     connectionString="Server=YOUR_SERVER; Port=1972; Namespace=YOUR_NAMESPACE; User ID=YOUR_USER; Password=YOUR_PASSWORD"
     providerName="InterSystems.Data.IRISClient" />
<add name="UserSqlConnection"
     connectionString="Server=YOUR_DATA_SERVER; Port=1972; Namespace=YOUR_NAMESPACE; User ID=YOUR_USER; Password=YOUR_PASSWORD"
     providerName="InterSystems.Data.IRISClient" />
<add name="CSVconnection"
     connectionString="Server=YOUR_DATA_SERVER; Port=1972; Namespace=YOUR_NAMESPACE; User ID=YOUR_USER; Password=YOUR_PASSWORD"
     providerName="InterSystems.Data.IRISClient" />
```

**License required:** InterSystems IRIS requires a valid commercial license from InterSystems Corporation. DataAI does not provide or distribute InterSystems software. Visit https://www.intersystems.com for licensing information.

### InterSystems Caché

Use the same format as IRIS above, but change the provider to `InterSystems.Data.CacheClient`.

**License required:** InterSystems Caché requires a valid commercial license from InterSystems Corporation.

### SQLite (default / standalone)

The default configuration ships with SQLite for zero-setup local use:

```xml
<add name="MySqlConnection" connectionString="Sqlite" providerName="Sqlite" />
<add name="UserSqlConnection" connectionString="Sqlite" providerName="Sqlite" />
<add name="CSVconnection" connectionString="Sqlite" providerName="Sqlite" />
```

**Important:** When switching from SQLite to another database, remove or comment out the SQLite lines and uncomment your chosen database block. Only one set of connection strings should be active at a time.

---

## Step 2: Application Settings

In the `<appSettings>` section, update the following keys.

### Application Identity

| Key | What to Enter | Example |
|-----|---------------|---------|
| `ourapplication` | Your application display name | `DataAILite` |
| `pagettl` | Browser tab / page title | `DataAILite in memory` |
| `unit` | Your organization or unit name | `MyCompany` |
| `unitenddate` | License expiration date | `2040-12-31 23:59:00` |

### Web URLs

| Key | What to Enter | Example |
|-----|---------------|---------|
| `unitOUReportsWeb` | Full URL to your DataAI site | `https://myserver/DataAILite/` |
| `unitRegistrationWeb` | Registration page URL | `https://myserver/DataAILite/` |
| `webour` | Your DataAI web URL | `https://myserver/DataAILite/` |
| `weboureports` | Reports web URL | `https://myserver/DataAILite/` |
| `webhelpdesk` | Help desk URL | `https://myserver/DataAILite/` |

### Database Reference

| Key | What to Enter | Example |
|-----|---------------|---------|
| `unitOURdbConnStr` | Connection string for operational DB | `Server=localhost; Database=DataAI; User ID=root; Password=secret;` |
| `OUReportsServer` | Operational database server hostname | `localhost` |

### Super User Password

| Key | What to Enter |
|-----|---------------|
| `superpass` | Password for the built-in super administrator account |

---

## Step 3: Email (SMTP) Settings

DataAI sends emails for user registration, password resets, and support tickets. Configure both locations:

### In `<appSettings>`:

| Key | What to Enter | Example |
|-----|---------------|---------|
| `SmtpCred` | SMTP server hostname | `smtp.gmail.com` |
| `smtpemail` | SMTP sender email address | `noreply@yourcompany.com` |
| `smtpemailpass` | SMTP email password or app password | `your-app-password` |
| `supportemail` | Email address for support tickets | `support@yourcompany.com` |

### In `<system.net>` / `<mailSettings>`:

```xml
<smtp deliveryMethod="Network" from="noreply@yourcompany.com">
  <network defaultCredentials="false"
           host="smtp.gmail.com"
           password="your-app-password"
           port="587"
           userName="noreply@yourcompany.com"
           enableSsl="true" />
</smtp>
```

**Gmail users:** You must use an [App Password](https://support.google.com/accounts/answer/185833), not your regular Gmail password. Enable 2-Step Verification first, then generate an App Password.

---

## Step 4: Google Maps API Key (Optional)

Required only if your reports use geographic map visualizations.

| Key | What to Enter |
|-----|---------------|
| `mapkey` | Your Google Maps JavaScript API key |

Get a key at: https://console.cloud.google.com/google/maps-api

If you are not using map reports, you can leave this as `[your google map key]` or set it to an empty string.

---

## Step 5: OpenAI Integration (Optional)

Required only if you want AI-powered data analysis and the ChatAI feature.

| Key | What to Enter | Example |
|-----|---------------|---------|
| `openaikey` | Your OpenAI API key | `sk-proj-abc123...` |
| `openaiorganization` | Your OpenAI organization ID | `org-abc123` |
| `apiURL` | OpenAI API endpoint URL | `https://api.openai.com/v1/chat/completions` |
| `openaimodel` | Model name | `gpt-4o` or `gpt-4o-mini` or `o3` or `o3-mini` |
| `openaimaxTokens` | Maximum token limit for responses | `128000` |

Get your API key at: https://platform.openai.com/api-keys

If you are not using AI features, leave these fields as their placeholder values.

---

## Step 6: File Upload Folder

| Key | What to Enter | Example |
|-----|---------------|---------|
| `fileupload` | Server folder path for uploaded CSV/data files | `C:\DataAI\uploads\` |

Make sure the IIS application pool identity has read/write permissions on this folder.

---

## Step 7: Database Case Sensitivity

These settings control how DataAI handles table and column name casing in SQL queries. Set them to match your database server's behavior.

| Key | Options | When to Use |
|-----|---------|-------------|
| `ourdbcase` | `lower`, `upper`, `mix`, `doublequoted`, or empty | Operational database |
| `csvdbcase` | `lower`, `upper`, `mix`, `doublequoted`, or empty | CSV import database |
| `userdbcase` | `lower`, `upper`, `mix`, `doublequoted`, or empty | User data database |

Typical values: MySQL uses `lower`, Oracle uses `upper`, PostgreSQL uses `lower`, SQL Server uses `mix`.

---

## Step 8: Database Provider Flags

If you are using a specific database engine, set its provider key to a non-empty value `OK` and leave the others empty:

```xml
<add key="MySqlProv" value="yes" />      <!-- MySQL -->
<add key="SQLServerProv" value="" />     <!-- SQL Server -->
<add key="CacheProv" value="" />         <!-- InterSystems Caché -->
<add key="IRISProv" value="" />          <!-- InterSystems IRIS -->
<add key="CSVProv" value="" />           <!-- CSV flat files -->
<add key="Oracle" value="" />            <!-- Oracle -->
<add key="ODBC" value="" />              <!-- ODBC generic -->
<add key="OleDb" value="" />             <!-- OLE DB generic -->
<add key="Npgsql" value="" />            <!-- PostgreSQL -->
```

Only enable the provider that matches your `<connectionStrings>` configuration.

---

## Other Settings (Usually No Changes Needed)

| Key | Default | Description |
|-----|---------|-------------|
| `maxretries` | `5` | Number of retry attempts for failed API calls |
| `SiteFor` | `Production` | Environment label (`Production` or `Development`) |
| `DaysFree` | `2000` | Number of free trial days for new registrations |
| `version` | `36-00` | DataAI version number (do not change) |
| `UnitAuthenticate` | `NO` | Whether to enforce unit-level authentication |
| `dataaidownpay` | `$10` | Download payment amount (if payments are enabled) |
| `dataaidbpay` | `$100` | Database setup payment amount |
| `ACEOLEDBversion` | `Provider=Microsoft.ACE.OLEDB.16.0;` | ACE OLE DB version for Excel/Access imports |

---

## Troubleshooting

**"Could not load file or assembly 'MySql.Data'"** — Make sure `MySql.Data.dll` (version 8.0.30) is in the `Bin` folder of your DataAI web application.

**Connection timeout errors** — Increase `executionTimeout` in the `<httpRuntime>` tag (default is 36000 seconds).

**Large file upload failures** — Increase `maxRequestLength` in the `<httpRuntime>` tag (default is 8192 KB = 8 MB). Value is in kilobytes.

**Email sending failures** — Verify your SMTP credentials. For Gmail, make sure you are using an App Password and that 2-Step Verification is enabled on the Google account.

**IIS 500 errors** — Set `<compilation debug="true">` to see detailed error messages, and check that `validateIntegratedModeConfiguration` is set to `false` in `<system.webServer>`.
