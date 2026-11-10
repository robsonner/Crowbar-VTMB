# VTMB split-inheritance QC preservation

For v2531 models, QC export writes one native-compiler extension for each source
MDL bone whose flags contain bit 0x02:

```qc
$vtmbbonerotation "bone_name" "model"
```

Bone names and flags come directly from the MDL. The exporter does not assume a
particular name, position, hierarchy, count, or presence of include models.
Bones without the bit need no declaration because parent-local rotation is the
compiler default. The QC keyword follows the mixed-case export preference.

This preserves rotation-space semantics alongside standard SMD animations.
The existing SMD exporter still converts model/root-space rotation into
parent-local rotation. The native VTMB compiler consumes the declaration before
animation compression and restores the corresponding encoding and bit 0x02.
VHB/VRD helper rules, physics root selection, other bone usage flags, and
animation masks/autolayers are separate mechanisms and are not changed here.

Use a compiler supporting this extension (branch `fix/explicit-vtmb-bone-rotation`
or a later integrated build). Older compilers may reject the new QC command.
This cannot recover a flag already lost in a previously malformed MDL; decompile
an original model or a model rebuilt with the correct explicit declaration.

The user accepted the explicit-rotation compiler fix for the fitted Christof
replacement in game on 2026-11-08. That is scoped acceptance of the compiler
test, not evidence of exhaustive decompiler round-trip coverage.
