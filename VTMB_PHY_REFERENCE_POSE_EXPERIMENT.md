# Experimental v2531 PHY reference pose

Date: 2026-11-02. Branch: `experiment/vtmb-phy-reference-pose`.
Baseline: `dcf176e` (clean and already committed before this experiment).

Original Sheriff PHY solids contain origin/angles distinct from the MDL bind.
The shared PHY reader now retains finite three-component origin/angles values.
The v2531 SMD writer uses that explicit pose to convert collision vertices into
model space. Without either field, the previous MDL inverse-bind conversion is
retained; a partial pose is rejected. No model-specific offsets are introduced.

The Release/x86 .NET Framework 4.8 build succeeded after the user closed the
running executable. An earlier build compiled the source but could not replace
the locked EXE. The subsequent normal build updated
`Crowbar/bin/x86/Release/CrowbarVTMB.exe`.

For manual testing, decompile the ORIGINAL Sheriff MDL/PHY pair into a fresh
folder, inspect sheriff_physics.smd, and compile with the matching experimental
compiler. Old collision SMD files retain their previous coordinates. Preserve
authored cloth/capsule edits separately. Runtime acceptance is pending; no
executable semantics were newly established through Ghidra.
