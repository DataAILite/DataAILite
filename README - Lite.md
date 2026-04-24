# DataAILite

DataAILite is an open-source AI-powered project management and data analysis system built with ASP.NET and VB.NET.

---

## 🚀 Features

- AI-driven data analysis
- In-memory processing (no persistent storage after logout)
- Interactive dashboards and reports
- Google Maps / Earth integration
- OpenAI-powered insights

---

## ⚖️ License

DataAILite is licensed under the **GNU General Public License v3.0 (GPL v3)**.

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

1. Download `InstallDataAILite.zip` from DataAI.link
2. Extract the archive
3. Right-click `InstallDataAILite.exe`
4. Select **Run as Administrator**
5. Follow the installer instructions

---

### Option 2 – Manual Installation

1. Download `DataAILite.zip` from DataAI.link

2. Extract to:

wwwroot\DataAILite\

3. Open IIS Manager
4. Convert the folder into an Application
5. Set Application Pool Identity to Anonymous Authentication
6. Update web.config with your settings

---

## ⚙️ Configuration

### Web Settings

Update in web.config:

- Application URL: https://[your-domain]/DataAILite/
- Website title
- Upload folder path
- Google Maps API Key

---

### 🤖 OpenAI Settings

Configure:

- API Key
- Organization ID
- Base URL
- Model: gpt-4o / gpt-4o-mini / o3 / o3-mini
- Max Tokens:
  - 128000 → gpt-4o / gpt-4o-mini
  - 200000 → o3 / o3-mini

---

## 🌐 Running the Application

Open in browser:

https://[your-domain]/DataAILite/Default.aspx

---

## 🔧 IIS Configuration

Ensure:
- Application created in IIS
- Correct Application Pool
- Anonymous Authentication enabled

---

## ⚠️ Important Notes

- Runs entirely in memory
- No data stored after logout
- Session-based processing

---

## 📚 Documentation

http://DataAI.link

---

## 🆘 Support

https://oureports.net/OUReports/ContactUs.aspx

---

## 🔒 Disclaimer

This software is provided "AS IS", without warranty of any kind.

---

## 🤝 Contributing

By contributing, you agree your contributions are licensed under GPL v3.
