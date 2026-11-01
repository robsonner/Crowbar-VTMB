# Experimental v2531 four-influence export

Branch: `experiment/vtmb-four-influences` (2026-11-01).
Baseline commit: `7ed780a`.

The user authorized an experimental implementation based on the external
v2531 article without waiting for Ghidra MCP verification. These semantics
remain a working hypothesis, not newly established executable facts.

Type 0 decoding now reads three byte weights, a selector, and four short bone
indices. Active count is `max(1, selector % 5)`. For four active slots the final
weight is `255 - sum(stored weights)`. REF SMD and VCLOTH consume the same decoded
four-slot arrays. The on-disk vertex stride remains 44 bytes.

Build: Release/x86, .NET Framework v4.8. Output:
`Crowbar/bin/x86/Release/CrowbarVTMB.exe`.

For a manual trial, decompile the ORIGINAL game MDL into a separate directory
with this build, then use the matching experimental compiler. Previously
exported SMD/VCLOTH files do not recover the missing influence automatically.
Keep existing authored files separately; do not overwrite capsule/cloth edits.
No model compilation, automated round-trip campaign or runtime acceptance was
performed for this experiment. The solution build passed.
