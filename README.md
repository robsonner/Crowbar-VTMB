# Crowbar-VTMB
Crowbar-VTMB - GoldSource and Source Engine Modding Tool (VTMB Edition)

## About Crowbar-VTMB
Crowbar-VTMB is a modified version of Crowbar originally created by ZeqMacaw.
* **Version:** v0.74-vtmb.4
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

## Using this release

Extract the release ZIP to a writable folder and run `CrowbarVTMB.exe`.
Windows and .NET Framework 4.8 are required. The package includes runtime
dependencies but excludes personal settings, game assets and the compiler.

Decompile the original MDL together with its VTX and optional PHY companions.
Use fresh exports to obtain the four-influence skinning, eye-material and PHY
reference-pose corrections; existing source files are not repaired automatically.
Keep authored sources separately when re-exporting.

VTMB exports now include `.vhb` AxisInterp rules and, where applicable, `.vcloth`
data and a separate capsule helper SMD. Recompiling the custom QC/VCLOTH commands
requires the matching updated `studiomdl_vtmb` toolchain; ordinary Valve compilers
do not implement these extensions. See [VCLOTH](VTMB_VCLOTH.md) and
[capsule export](VTMB_CAPSULE_PREVIEWS.md).

QC exports preserve explicit VTMB model-space bone rotations using
`$vtmbbonerotation`; recompilation requires a native VTMB compiler supporting
this command. For Blender cloth editing, keep REF SMD and VCLOTH together with
the same filename stem. See [bone rotation](VTMB_BONE_ROTATION.md).

This is a community release with selected in-game validation. Full coverage of
facial deformation encoding, complex animation layers and all model/LOD variants
is not established. See the release notes for scope and limitations.
