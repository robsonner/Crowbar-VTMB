# VCLOTH version 1: cloth data attached to the existing REF mesh

Crowbar exports `<reference-stem>.vcloth` instead of `<reference-stem>_CLOTH.smd`.
It exports no cloth-role armature, artificial node bones or duplicate cloth mesh.
Ordinary REF SMD geometry, UVs, normals and real skeletal weights are unchanged.
Capsule SMD helpers and their existing QC commands are independent and remain.

```qc
$Cloth "fabric_material"
{
    model "character_REF.smd"
    source "character_REF.vcloth"
}
```

The paired native VTMB compiler now supports the `source` directive and preserves
the simulation tables. The Blender add-on remains a separate future step. Do not
replace `source` with a legacy reconstruction policy: that would discard the
preserved simulation data.

## Deferred visual issues (2026-10-31 checkpoint)

User game tests confirm moving cloth, but report a strip/seam or gap at the
rigid/cloth boundary on Ventrue and the newly compiled Jeanette skirt. The user
is unsure whether the Jeanette skirt strip existed previously. Older Jeanette
face/neck holes may be a separate issue. Both are explicitly deferred; this
checkpoint does not claim to fix them.

A read-only comparison found matching Ventrue coat face membership, cloth maps
and named weights. Four Jeanette face vertices have changed weight profiles;
nonzero Type0 fields and original VTX code 18 remain unresolved. A missing bone
influence is a hypothesis, not an established format rule or runtime cause.
Workspace evidence: `tools/skinning_audit_report.md` and
`tools/ventrue_vtx_boundary_report.md` in the parent workspace.

## File contract

The file is UTF-8 JSON without a BOM. Root fields:

| Field | Meaning |
| --- | --- |
| `format`, `version` | `"vtmb-cloth"`, integer `1`; reject unsupported versions |
| `reference_smd` | REF filename relative to this VCLOTH file's directory |
| `source_model` | Original MDL model name, provenance only |
| `lod` | `0` in version 1 |
| `coordinate_space` | `"reference_model"` |
| `reference` | Original vertex identities, actual SMD corner attributes, triangles, real bones and maps |
| `simulation` | Original points, springs and decoded surface/runtime tables |

`reference.vertices` is indexed by its `id`, the original model vertex ID.
Each record contains `source_position`, `source_normal`, `source_uv` and original
`skinning` links `{bone, weight_byte}`. `exported` contains the actual emitted SMD
`position`, `normal`, `uv`, `parent_bone` and `[bone, weight]` `skinning` links.
It is null for original vertices absent from exported LOD 0 triangles.

`reference.triangles` follows actual REF SMD face/corner order, with `material`
and three original `vertices` IDs. This captures triangle context and UV/normal
splits without adding anything to SMD. `reference.bones` carries real bone IDs,
names, parents and original reference transforms (rotation is XYZW quaternion).

`reference.meshes` gives each mesh's material, vertex range and explicit render
`bindings`. Each binding has `vertex`, simulation `node`, preserved `node_flags`
(bit 15, meaning not generalized here) and original `tangent_index`. Unbound
render vertices have no binding; they remain normally bone-skinned. Node and
render vertex positions need not coincide. Do not merge identities by position.

`simulation.nodes` is indexed by `id`. Each has `fixed`, reference-model
`position`, and `reference_vertex` selecting its real skinning. A fixed node
not used by render bindings is still retained. No rigid/anchor alias promotion
is necessary: original vertex IDs and their triangle contexts remain distinct.

`simulation.springs` preserves order, node pairs (`nodes`), `family` (`structural`
or `bending`), signed `weight_a`, signed `weight_b`, and `rest_length_squared`.
Original coefficients are preserved individually, not inferred from geometry or
reduced to one bend ratio. JSON uses round-trip numeric formatting, so float32
coefficients can be reconstructed exactly. Positions/attributes in `exported`
retain ordinary SMD's six-decimal precision; original positions remain separate.

Other simulation fields preserve documented decoded tables:

- `time_step_scale`: descriptor integration scalar, formerly mislabeled stiffness.
- `edges`: `[nodeA, nodeB]` records, original order and orientation.
- `normal_edge_prefix`: original edge-prefix boundary, not inferred free-edge count.
- `triangles`: `[edge0, edge1, node0, node1, node2]` records.
- `tangents`: `{edges: [edge0, edge1], coefficients: [coefficient0, coefficient1]}`.
- `collision_neighbors`: three node IDs per original free-node neighbor record.
- `structural_batch_count`, `batch_sizes`: original structural/bending SIMD batches.

These tables are data attached to the existing REF mesh, not an instruction to
create another Blender mesh. A future add-on must work directly on imported REF
and keep cloth attributes out of ordinary SMD bones, materials and skin weights.

## Matching and editing requirements for future consumers

Original vertex IDs are file-local identities, not Blender vertex indices. Match
using actual exported attributes, material and triangle context. A global
position-only or position/UV-only lookup is insufficient. Diagnose ambiguous
matches instead of silently welding, guessing or assigning different nodes.
The reference snapshot permits fresh import from SMD + VCLOTH without a `.blend`.
It does not claim that the future matching algorithm has already been validated.

Unchanged data should be preserved. An editor that changes topology or spring
order must rebuild/validate dependent maps, surface tables and batches rather
than reusing stale records. Never treat all stored coefficients as a single
artist-facing stiffness parameter; their complete authoring semantics are not
known yet. Unknown future fields/versions must not be silently discarded.

## Scope, failures and migration

Version 1 supports one cloth instance per submodel, Type 0 reference vertices
and LOD 0 without cloth LOD interpolation. Unsupported or invalid data produces
a QC warning instead of a fabricated/fallback cloth block. REF export failures
prevent claiming a successful cloth export. Existing authored `_CLOTH.smd` files
are not deleted; decompile into a fresh directory to avoid stale files.

`tools/validate_vcloth_export.py` in the workspace compares exports against
original MDL table values (including float32 spring coefficients), raw node maps,
and actual REF SMD corners. Without `--vcloth`, it only probes input-table range
assumptions. Those probes passed on Malkavian, Jeanette and male Ventrue armor 3.
Actual new exports and Blender/compiler round trips remain unverified until
separate tests; building alone does not establish end-to-end acceptance.
