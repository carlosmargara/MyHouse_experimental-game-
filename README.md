# MyHouse – Experimental Atmospheric Horror

An experimental first-person psychological horror experience set inside a meticulously reconstructed domestic space.

This project prioritizes **atmosphere over complex mechanics**, exploring spatial tension, lighting, dynamic audio design, and visual aesthetics heavily inspired by classic 1990s survival horror titles.

---

## 🎥 Concept

MyHouse is conceived as an **interactive atmospheric study**.

The core objective is to take a familiar, mundane apartment layout and turn it into an uncanny space of dread through:

- Hybrid camera perspective (first-person navigation combined with fixed cinematic angles).
- Diegetic lighting as a primary survival and discovery tool.
- Dynamic, spatialized audio layers that react to the environment.
- Retro PS1/late-90s visual fidelity (dithering, low-resolution render targets, and stylized shaders).

---

## 🕯 Core Features

- **Hybrid Navigation:** Seamless first-person movement alongside retro fixed-camera setups.
- **Dynamic Lighting:** Interactive lighter system driving real-time light and shadow casting.
- **Object Inspection & Interaction:** Context-aware interaction system for doors, drawers, inspectable items, and narrative props.
- **Adaptive Audio Engine:** Integrated spatial audio, surface-dependent soundscapes, and tension layers powered by FMOD.
- **Retro Visual Styling:** Custom low-res render texture pipeline, CRT/dithering post-processing, and low-poly hard-surface modeling.

---

## 🛠 Tech Stack

- **Engine:** Unity (Built-in Render Pipeline)
- **Audio Middleware:** FMOD Studio
- **3D Assets & Environment:** Blender (Hard-surface modeling, low-poly UV workflows)
- **Programming:** C# (Custom interaction, camera controllers, and input handlers)
- **Visuals & Textures:** Krita & custom post-processing shaders

---

## 🎮 Controls

| Action | Input |
|--------|-------|
| Move | `W` `A` `S` `D` |
| Look / Aim | `Mouse` |
| Interact / Inspect | `E` |
| Toggle Lighter | `F` |
| Inventory / Examine | `Tab` / `I` |

---

## 🚧 Development Status

The core technical foundation, environmental layout, and audio systems are fully implemented. The project is currently in its final balancing and content-polishing stage.

**Implemented systems:**
- [x] Full architectural modeling & texturing of the domestic space
- [x] First-person & fixed camera switching logic
- [x] Lighter mechanics and lighting pass
- [x] FMOD spatial audio and ambient snapshot routing
- [x] Object inspection mechanics and UI integration

**Final polish in progress:**
- [ ] Narrative arc triggers and event pacing (target: 20–30 minute focused experience)
- [ ] Final audio mix and master bank pass
- [ ] Standalone playable demo build

---

## 📌 Author

**Carlos Margara (Chars)**  
Indie Game Developer & Sound Designer  
*Focused on retro aesthetics, atmospheric horror, and interactive audio.*

---

## License

This project is licensed under the MIT License - see the LICENSE file for details.