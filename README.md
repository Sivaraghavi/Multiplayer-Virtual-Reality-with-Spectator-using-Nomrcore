# Virtual Reality Multiplayer with AR Spectator using Normocore 
 
# [Project Title]

An immersive multiplayer experience in virtual environments with support for  spectatorship

---

## About
This project presents a virtual reality multiplayer system where users can collaborate in shared 3D environments. It includes features such as synchronized object interaction, avatar customization, and dynamic environment switching. A unique VR Spectator Mode allows users to view ongoing sessions from an observer’s perspective, adding a new layer of engagement and usability.

The goal is to enhance collaborative learning and interactive showcases in fields like training, simulation, and virtual showrooms. The intended audience includes XR developers, educators, and simulation designers.

Demo Video (VR Multiplayer): [Watch Demo](https://youtu.be/Rlizt6l1Gu4)

Demo Video (Spectator Mode): [Watch Demo](https://youtu.be/cqKkjL-icfQ)

R&D Document: ./RND.docx

Knowledge Transfer Document: ./KnowledgeTransfer.docx

## Quick Start

1. Prerequisites:
   - Unity Editor: Version `2022.3.55f1` (Specify the exact version used)
   - Blender (for 3D modeling)
   - Normcore (for Networking) 
   - Git (Ensure Git LFS is installed)

   Note: To install Git LFS, run `winget install GitHub.GitLFS`, then in your project directory run `git lfs install`. <!-- Git lfs is important to sync large files with git -->

2. Clone the Repository:

   ```bash
   git clone https://github.com/Sivaraghavi/Virtual-Reality-with-Augmented-Reality-Spectator-using-Nomrcore-
   cd Virtual-Reality-with-Augmented-Reality-Spectator-using-Nomrcore-
   ```

3. Open the cloned folder using Unity Hub.

4. Run:
   - 
   - Open the main scene: `Virtual-Reality-with-Augmented-Reality-Spectator-using-Nomrcore-\Assets\Scenes\Flow` 
   - Click the Play button ▶️ in the Unity Editor.

---

## Features

List the main functionalities or highlights of your project.

- Feature 1 (e.g., Real-time multiplayer using Photon PUN 2)
- Feature 2 (e.g., Dynamic day/night cycle affecting gameplay)
- Feature 3 (e.g., Procedurally generated level layouts)
- Feature 4 (e.g., Custom shader for water effects)
- Feature 5 (e.g., Integration with backend for leaderboards)

---

## Dependencies <!-- (Extra Tools/Frameworks/Packages) -->

- Unity: TextMesh Pro, Cinemachine, ML Agents, ...
- External: Photon PUN 2, NewtonsoftJson, Normcore, Google AI Speech, ...
- etc.

---

## Project Structure Overview

```
MyProject/                     # Root directory
├── Assets/                    # Core Unity assets
│   ├── Scenes/                # .unity scene files
│   ├── Scripts/               # C# scripts
│   ├── Prefabs/               # Prefab templates
│   ├── Art/                   # Models, textures, sprites
│   ├── Audio/                 # Sound effects & music
│   └── Materials/             # Shaders & materials
├── Builds/                    # Compiled game builds
├── Demo.mp4                   # Demo video
├── RND.docx                   # R&D document
├── KnowledgeTransfer.docx     # Knowledge-Transfer document
├── Packages/                  # Unity package dependencies and manifests
├── ProjectSettings/           # Unity configuration files
├── .gitignore                 # Git ignore rules
└── README.md                  # Project overview and setup instructions (This file)
```

---

## Configuration

<!-- List any important settings that can be adjusted or need to be modified. -->
<!-- remove / add more if needed -->

| Setting | Location | Description | Default Value |
|---------|----------|-------------|---------------|
| Player Speed | `PlayerController` Script | Adjusts the movement speed of the player. | `5.0` |
| API Key/Endpoint | `Config/NetworkConfig.asset` | Base URL for the backend server. | `""` |
| Graphics Quality | `Project Settings > Quality` | Selects the rendering quality level. | `High` |
| (add more)        | (component / asset)             | (what it controls)                          | (value) | 

---

## Contact

- **Intern:** [Sivaraghavi U.R.](https://www.linkedin.com/in/username/)
  - Email: [sivaraghavi6103@gmail.com](sivaraghavi6103@gmail.com)

- **Mentor:** Praveen Krishna
