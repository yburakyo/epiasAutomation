# epiasAutomation

Automates PTF report download and mail automation from EPİAŞ using Selenium WebDriver and .NET 9.

## Requirements
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Google Chrome (latest stable version)
- Valid EPİAŞ account credentials
- Gmail account with App Password


## Installation

1. Clone the repository:
```bash
git clone https://github.com/yburakyo/epiasAutomation.git
cd epiasAutomation
```

2. Restore dependencies:
```bash
dotnet restore
```
3. Manually edit the appsettings.json file to use desired email credentials.

## Usage

1. Start Chrome with debugging:
```powershell
& "C:\Program Files\Google\Chrome\Application\chrome.exe" --remote-debugging-port=9222
```

2. Manually log in to EPİAŞ and navigate to homepage

3. Run the automation:
```bash
dotnet build
dotnet test
```