# VTMB collision capsule SMD previews

Crowbar exports all collision capsule previews for a submodel into one separate
`<reference-stem>_CAPSULES.smd`. For example, `character_REF.smd` gets
`character_REF_CAPSULES.smd`. REF and CLOTH stay free of preview triangles.
There is no separate SMD per capsule.

The capsule SMD contains the original real skeleton and reference pose. Each
capsule has material `__vtmb_capsule_<index>` (four digits) and full weight on
its real bone. The round-ended surface has 24 radial segments and six
subdivisions per hemisphere, in reference-model coordinates.

QC automatically includes this command after the real model/flex definitions:

```qc
$body "__vtmb_cloth_capsules" "character_REF_CAPSULES.smd"
```

Unmodified Blender Source Tools recognizes this ordinary $body command and
imports a separate mesh using the QC's shared armature. With skeletal animation
import enabled, body and capsules follow the same actions. One capsule-set SMD
means one object containing disconnected capsules; they can optionally be
separated by material or loose parts in Blender.

Each capsule-bearing submodel gets one helper file. Subsequent helper body names
have numeric suffixes (`__vtmb_cloth_capsules_0001`, etc.) to avoid collisions.
This does not expand the compiler's current single-cloth generation scope.

Only same-bone, finite, positive-radius, nonzero-axis capsules receive solid
previews. Unsupported two-bone and invalid records produce explicit comments in
the capsule SMD. Original collision capsule QC commands are retained. Constraint
records remain QC-only. No model-name, bone-name, coordinate or fixed-count
exceptions are used. Higher LODs do not get duplicate capsule helper files.

## Compiler support

Use studiomdl_vtmb with reserved capsule-helper body support. It reads the helper
as authoring data, replaces same-bone collision capsule QC records for the target
model, and excludes helper triangles from render geometry. Split-bone records
and constraints remain QC-only. Older compilers must not receive these helpers
as ordinary render bodies.

REF no longer needs preview faces removed. For exports from the earlier embedded
preview build, decompile into a new directory rather than mixing old REF helper
faces with a new separate capsule file. Preserve manually edited source projects.

## Validation status

Source diff whitespace checks passed before build. Standard Source Tools QC
import code was inspected for $body loading and shared-armature selection.
End-to-end acceptance of the new export and visual comparison remain pending;
the earlier standalone Blender diagnostic only validated transform behavior.
Build outcome is reported separately in the chat.
