# Third-Party Notices

This project, DataAI, uses third-party libraries distributed via NuGet.

All listed dependencies are believed to be compatible with the GNU General Public License v3.0 (GPL v3).

---

## .NET Platform

This project is built using Microsoft .NET / ASP.NET.

License:
- .NET (modern versions): MIT License

https://dotnet.microsoft.com/

---

## NuGet Dependencies

The following NuGet packages are used in this project:

### Core Libraries

- Google.Protobuf (v3.19.4) — BSD 3-Clause License  
- K4os.Compression.LZ4 (v1.2.6) — MIT License  
- K4os.Compression.LZ4.Streams (v1.2.6) — MIT License  
- K4os.Hash.xxHash (v1.0.6) — MIT License  

---

### Microsoft Libraries

- Microsoft.Bcl.AsyncInterfaces (v1.1.0) — MIT License  
- Microsoft.Data.Sqlite (v10.0.0-preview) — MIT License  
- Microsoft.Data.Sqlite.Core — MIT License  
- Microsoft.OpenApi (v1.6.14) — MIT License  

---

### Database & Data Access

- MySql.Data (v8.0.30) — GPL v2 with FOSS Exception  

---

### Office / Interop

- Microsoft.Office.Interop.Excel (v15.0.4795.1000)  
  License: Microsoft proprietary (requires Microsoft Office license)

---

### PDF & Security

- PDFsharp (v1.32.3057.0) — MIT License  
- Portable.BouncyCastle (v1.9.0) — MIT License  

---

### SQLite Components

- SQLitePCLRaw.bundle_e_sqlite3 (v2.1.11) — MIT License  
- SQLitePCLRaw.core — MIT License  
- SQLitePCLRaw.lib.e_sqlite3 — Public Domain / SQLite License  
- SQLitePCLRaw.provider.dynamic_cdecl — MIT License  

---

### System Libraries

These are part of .NET ecosystem and typically MIT licensed:

- System.Buffers  
- System.IO  
- System.Memory  
- System.Net.Http  
- System.Numerics.Vectors  
- System.Runtime  
- System.Runtime.CompilerServices.Unsafe  
- System.Security.Cryptography.*  
- System.Text.Encodings.Web  
- System.Text.Json  
- System.Threading.Channels  
- System.Threading.Tasks.Extensions  
- System.ValueTuple  

---

## External Services

### OpenAI

- Not included in this repository
- Requires API key
- Subject to OpenAI terms

https://openai.com/

---

### Google Maps (Optional)

- Not included in this repository
- Requires API key
- Subject to Google terms

https://developers.google.com/maps

---

## Removed Components

The following are NOT included in this open-source release:

- Oracle Database drivers  
- InterSystems IRIS drivers  

Users must obtain their own licenses if using those systems.

---

## Special Notes

### Microsoft Office Interop

Use of Microsoft.Office.Interop.Excel requires:

- A valid Microsoft Office installation
- Compliance with Microsoft licensing terms

---

### MySQL Connector

MySql.Data is licensed under:

- GPL v2 with FOSS Exception

This is compatible with GPL v3 usage in this project.

---

## Compliance Responsibility

Users are responsible for:

- Ensuring compliance with all third-party licenses
- Obtaining required licenses for external software
- Verifying compatibility when adding new dependencies

---

## Contributions

Contributors must ensure that any added dependencies:

- Are GPL-compatible
- Do not introduce restrictive licensing terms

---

## Disclaimer

Third-party components are provided under their respective licenses.

DataAI is distributed "AS IS", without warranty of any kind.