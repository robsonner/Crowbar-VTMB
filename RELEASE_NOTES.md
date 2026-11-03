# Crowbar-VTMB release notes

## v0.74-vtmb.3 — 2026-11-03

Crowbar-VTMB release addressing unskinned vertex formats, packed flexes, and PHY collision export reliability.

### Changes since v0.74-vtmb.2

- Decode v2531 unskinned vertex formats, packed flexes, and header fields from the game's code (PR #1).
- Resolve PHY hull bone index -1 for models using an explicit solid reference pose by matching unambiguous identity-bind root bones.
- Safely stage and atomically replace physics SMD files to prevent empty or corrupted outputs on export errors.
- Ignore non-terminal tree nodes during PHY triangle export and throw if no valid collision triangles are produced.
- Infer QC $rootbone for unbound single-solid models and omit empty $rootbone directives.

## v0.74-vtmb.2 — 2026-11-02

Crowbar-VTMB is a community fork of ZeqMacaw's Crowbar for Vampire: The
Masquerade – Bloodlines (MDL v2531). This release improves source preservation
and adds cloth export for the updated native VTMB authoring toolchain.

### Changes since v0.74-vtmb.1

- Preserve up to four Type 0 skinning influences in REF/LOD SMD and VCLOTH
  exports, including the implicit fourth weight. The paired compiler correction
  resolved the user-tested Jeanette skinning issue.
- Preserve eye mesh-to-eyeball material bindings from the model instead of
  guessing left/right assignments from texture names. The QC correction fixed
  Jeanette's white eyes in the user's game test.
- Read explicit PHY solid origin/angles and use them for v2531 collision SMD
  export. The paired compiler/decompiler correction resolved the user-tested
  Sheriff collision placement issue. Files without those fields retain the
  previous MDL bind conversion.
- Export AxisInterp procedural rules as .vhb and reference that filename in QC.
- Export preserved cloth simulation data as a version 1 .vcloth sidecar attached
  to the existing REF mesh. No duplicate cloth mesh or artificial cloth rig is
  required by the current export workflow.
- Export collision capsules as a separate, rigged *_CAPSULES.smd helper using
  real bones, with matching QC commands for the updated VTMB compiler.
- Include the VTMB physics-chain, cloth-capsule and model-flag export work added
  since the previous release.

### Installation and migration

Extract the ZIP into a writable folder and run CrowbarVTMB.exe. Windows with
.NET Framework 4.8 is required. Runtime dependencies are included. Personal
settings, game-derived assets, the compiler and the Blender add-on are excluded.

Decompile the ORIGINAL MDL with its VTX and optional PHY companions into a new
folder. Existing exported SMD/QC/VCLOTH files do not receive fixes automatically.
Keep authored geometry and capsule changes separately when re-exporting.

Custom VTMB commands and VCLOTH require the matching updated studiomdl_vtmb
compiler. Capsule SMDs are helpers, not render meshes; use the compiler's capsule
helper support. See VTMB_VCLOTH.md and VTMB_CAPSULE_PREVIEWS.md for details.

### Validation and limitations

Release/x86 build targets .NET Framework 4.8. Selected Jeanette skinning/eye,
Sheriff PHY and cloth/capsule workflows have user-reported in-game acceptance.
This is not exhaustive coverage of arbitrary models, render paths or LODs.

Facial vertex deformation encoding still requires investigation; full facial
fidelity is not claimed. Complex animation ownership masks, autolayers and melee
metadata preservation remain open work in the paired toolchain. Complete Type
1/2 decoding and lossless reproduction of every v2531 feature are not claimed.
Current preserved-cloth authoring is scoped to LOD0. These changes do not include
an engine solver preview or automatic capsule generation.

### Credits and license

Original Crowbar: ZeqMacaw. VTMB fork: robsonner.
License: Creative Commons Attribution-ShareAlike 3.0 Unported; see LICENSE.txt.
Repository: https://github.com/robsonner/Crowbar-VTMB
The original Crowbar author does not endorse or support this fork. Report issues
to the fork repository. No game-derived model or texture files are distributed.

### Previous release

v0.74-vtmb.1 introduced this fork's VTMB decompilation workflow. Its original
release notes remain in that tag's source history; the scope above supersedes
older blanket claims of complete format or facial-animation support.
