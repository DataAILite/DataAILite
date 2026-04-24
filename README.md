# DataAI

DataAI is an AI-powered data analysis and reporting platform designed for flexible integration with multiple databases and advanced visualization capabilities.

---

## 🚀 Features

- AI-driven data analysis
- Dynamic report generation
- Multi-database support (MySQL, Oracle, InterSystems IRIS, others)
- Web-based interface (ASP.NET)
- OpenAI-powered insights
- Google Maps integration (optional)
- User registration and management

---

## ⚖️ License

DataAI is licensed under the **GNU General Public License v3.0 (GPL v3)**.

You are free to:
- Use the software
- Modify the source code
- Distribute the software

Requirements:
- Any distributed modifications MUST also be open source under GPL v3
- No warranty is provided

Full license: https://www.gnu.org/licenses/gpl-3.0.html

---

## 🛠 Installation

### Option 1 – Installer (Recommended)

1. Download `InstallDataAI.zip` from DataAI.link
2. Extract the archive
3. Right-click `InstallDataAI.exe`
4. Select **Run as Administrator**
5. Follow the installer instructions

### Option 2 – Manual Installation

1. Download `DataAI.zip` from:
   http://DataAI.link

2. Extract to:

C:\inetpub\wwwroot\DataAI\

3. Open IIS Manager

4. Convert the `DataAI` folder into an Application

---

## ⚙️ Configuration

Update `web.config` with the following:

### 🗄️ 1. Operational Database (for report definitions)

- Server
- Database / Namespace
- User ID
- Password
- Port (if required)

> MySQL is recommended for the operational database

---

### 📊 2. Data Database (your actual data)

- Server
- Database / Namespace
- User ID
- Password
- Port (if required)

---

### 🧩 3. System Database (Oracle / InterSystems IRIS only)

- System database password

---

### 📧 4. Email (SMTP)

- Email address
- Email password

---

### 🌐 5. Application Settings

- Application URL  
  https://[your-domain]/DataAI/

- Website title
- Support email
- Upload folder path
- Google Maps API Key (optional)

---

### 🤖 6. OpenAI Configuration

- OpenAI API Key
- OpenAI Organization Code
- Model: gpt-4o / gpt-4o-mini / o3 / o3-mini
- Max Tokens:
  - 128000 → gpt-4o / gpt-4o-mini  
  - 200000 → o3 / o3-mini  

---

## ▶️ Running the Application

https://[your-domain]/DataAI/Default.aspx

---

## 👤 Getting Started

1. Register a user account
2. Log in
3. Start using DataAI

---

## ⚠️ Database Licensing Notice

If you are using:

- Oracle Database
- InterSystems IRIS (or other InterSystems products)

You MUST have a valid license from the respective vendor.

DataAI does NOT provide database licenses and does NOT include Oracle or InterSystems software.

It is the user’s responsibility to:
- Obtain proper licenses
- Comply with vendor terms
- Ensure legal usage of those database systems

---

## 🆘 Support

https://oureports.net/OUReports/ContactUs.aspx

---

## 🔒 Disclaimer

This software is provided "AS IS", without warranty of any kind.
