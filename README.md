# Crowbar
 Crowbar - GoldSource and Source Engine Modding Tool
 
 [Website](https://steamcommunity.com/groups/CrowbarTool)
 
 IMPORTANT: When providing link to Crowbar, please only use the above website link, because that page provides info and links to everything related to Crowbar.

# Building
I currently build via Visual Basic in Visual Studio Community 2017.
I use Debug x86 when debugging and Release x86 when releasing to public.

I tested building in Visual Studio Community 2019 on 15-Apr-2021. All I had to change were the settings from Debug Any CPU and Release Any CPU to Debug x86 and Release x86 at the top in the default toolbars.

## Building via Command Line (Visual Studio 2022 / modern MSBuild):
Always specify `/p:TargetFrameworkVersion=v4.8` to target modern .NET Framework 4.8:
```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" "Crowbar.sln" /p:Configuration=Release /p:Platform=x86 /p:TargetFrameworkVersion=v4.8 /t:Build
```
Binary output: `Crowbar\bin\x86\Release\Crowbar.exe`
