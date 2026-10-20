# Crowbar-VTMB
Crowbar-VTMB - GoldSource and Source Engine Modding Tool (VTMB Edition)

## About Crowbar-VTMB
Crowbar-VTMB is a modified version of Crowbar originally created by ZeqMacaw.
* **Version:** v0.74-vtmb.1
* **Release Notes:** [RELEASE_NOTES.md](RELEASE_NOTES.md)
* **Original Author:** ZeqMacaw
* **Modified by:** robsonner
* **Original License:** Creative Commons Attribution-ShareAlike 3.0 Unported (CC BY-SA 3.0)
* **License Link:** [https://creativecommons.org/licenses/by-sa/3.0/deed.en_US](https://creativecommons.org/licenses/by-sa/3.0/deed.en_US)

> **Disclaimer:** The original author (ZeqMacaw) does not endorse, sponsor, or officially support this modified version. All support requests, issues, and inquiries regarding Crowbar-VTMB should be directed to the repository maintainers, not to the original author.

Original Crowbar Website: [Crowbar Steam Community Group](https://steamcommunity.com/groups/CrowbarTool)

IMPORTANT: When providing links to the original Crowbar, please only use the above website link, as that page provides info and links to everything related to official Crowbar releases.

## Building via Command Line (Visual Studio 2022 / modern MSBuild):
Always specify `/p:TargetFrameworkVersion=v4.8` to target modern .NET Framework 4.8:
```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" "Crowbar.sln" /p:Configuration=Release /p:Platform=x86 /p:TargetFrameworkVersion=v4.8 /t:Build
```
Binary output: `Crowbar\bin\x86\Release\CrowbarVTMB.exe`
