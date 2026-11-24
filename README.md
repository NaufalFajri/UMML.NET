# UMML.NET

## Building from Source

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows 10/11 (x64)

### Build Commands
```bash
# Clone the repository
git clone https://github.com/NaufalFajri/UMML.NET.git
cd UMML.NET

# Build Release binary
dotnet build UMML.sln -c Release
```
The compiled executable will be located at:
`bin\Release\net8.0-windows\UMML.exe`

---

## Credits & Acknowledgments
- Based on the original UMML / Tumugu mod loader.
- Powered by [AssetsTools.NET](https://github.com/nesrak1/AssetsTools.NET) by nesrak1.
- SQLiteCipher via `sqlite-net-base` & `SQLite3MC.PCLRaw.bundle`.
