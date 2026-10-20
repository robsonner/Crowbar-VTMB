# Crowbar-VTMB — Release Notes

## Version v0.74-vtmb.1

### Overview
**Crowbar-VTMB** is a dedicated fork of the Crowbar GoldSource/Source engine modding tool (originally created by ZeqMacaw), customized specifically for **Vampire: The Masquerade – Bloodlines** (VTMB, model format MDL v2531).

This release introduces native support for Troika's unique Source 2004 branch, enabling seamless decompilation of Bloodlines models into standard, editable QC, SMD, VTA, and VRD source files for authoring, modding, and modern recompilation workflows.

---

### Key Features & Highlights

#### 1. Dedicated VTMB Model Format Support (MDL v2531)
* **Comprehensive Decompilation:** Native decompilation of Vampire: The Masquerade – Bloodlines `.mdl`, `.vtx`, and `.phy` files into clean, standard-compliant source assets.
* **Full Vertex Format Support:**
  * **Type 0 Vertices:** Complete support for uncompressed 32-bit floating-point vertex serialization (44 bytes per vertex: position XYZ, normals XYZ, UV, and bone weights/indices).
  * **Type 1 & Type 2 Vertices:** Full decoding support for legacy 16-bit and 8-bit quantized vertex formats.
* **VTMB-Specific Header Integration:**
  * **Custom `$phonemefilter` QC Extension:** Introduces a custom `$phonemefilter <min> <max>` QC command (*not part of the official Valve/Troika QC command set*). This custom extension was developed specifically for our toolchain to read, preserve, and serialize Troika's unique phoneme transition/crossfade thresholds stored in the model header (offsets `0xE8` and `0xEC`).
  * Accurate bounding box (`$cbox`, `$bbox`), eye position (`$eyeposition`), and illumination origin (`$illumposition`) extraction.

#### 2. Geometry, Rigging & Meshes (`.smd`)
* **Mesh & Strips Reconstruction:** Robust pairing with VTMB companion `.vtx` (v107) files to accurately reconstruct polygon triangles, strips, and body parts.
* **Bone Rigging & Skinning:** Bind-pose (`skeleton` time 0) and vertex weighting (up to 3 bone weights per vertex) exported cleanly into reference and LOD SMD files.
* **LOD Support:** Proper separation and export of Level-of-Detail geometry.

#### 3. Skeletal Animations (`.smd`)
* **Embedded Sequence Decompression:** Direct decompression of RLE rotation and translation keyframes embedded within the `.mdl` file (Bloodlines does not use external streaming `.ani` files).
* **Automatic Sequence Splitting:** All animations and sequences are organized and extracted into individual SMD files with proper activity names, framerates, loop flags, and events.

#### 4. Facial Animation & Flexes (`.vta`)
* **Blendshapes & Vertex Animations:** Full extraction of facial expressions, eyelid tracking, and phoneme vertex deltas into `.vta` files.
* **QC Flex Rules Generation:** Clean reconstruction of `$flexfile`, flex controllers, flex descriptions, and flex evaluation expressions.

#### 5. Procedural Bones & Physics
* **Procedural Bone Rules (`.vrd`):** Full export of `AxisInterp` and `QuatInterp` procedural bone definitions into companion `.vrd` files referenced via `$proceduralbones`.
* **Collision Physics (`.phy`):** Physics collision hulls parsed and converted to `_physics.smd` along with `$collisionmodel` and `$collisiontext` blocks.


---

### System Requirements & Compatibility
* **Operating System:** Windows 10 (version 1903 or later) / Windows 11.
* **Runtime:** .NET Framework 4.8 (pre-installed natively on modern Windows 10/11 out-of-the-box).
* **Dependencies:** None. All required native and managed components (`lzma`, `steam_api`, `Steamworks.NET`, `CrowbarSteamPipe`, `lzham_x86`) are embedded and self-contained.

---

### Attribution & License

* **Original Author:** ZeqMacaw
* **Modified by:** robsonner
* **License:** Creative Commons Attribution-ShareAlike 3.0 Unported (CC BY-SA 3.0)
* **License URL:** [https://creativecommons.org/licenses/by-sa/3.0/deed.en_US](https://creativecommons.org/licenses/by-sa/3.0/deed.en_US)
* **Source Repository:** [https://github.com/robsonner/Crowbar-VTMB](https://github.com/robsonner/Crowbar-VTMB)

> **Notice:** This project is a community-driven modification. The original author (ZeqMacaw) does not endorse, sponsor, or officially support this modified version. All support inquiries and issues should be directed to the [Crowbar-VTMB repository](https://github.com/robsonner/Crowbar-VTMB).
