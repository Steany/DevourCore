# DevourCore

<p align="center">
  <b>A lightweight client for DEVOUR with QoL features, performance tools, speedrun utilities, and visual customization.</b>
</p>

<p align="center">
  <img src="https://img.shields.io/github/stars/Steany/DevourCore?label=stars&color=yellow" />
  <img src="https://img.shields.io/github/issues/Steany/DevourCore?label=issues&color=orange" />
  <img src="https://img.shields.io/github/contributors/Steany/DevourCore?label=contributors&color=blue" />
  <img src="https://img.shields.io/github/downloads/Steany/DevourCore/latest/DevourCore.dll?label=latest%20download&color=blueviolet" />
  <img src="https://img.shields.io/badge/game-devour-blueviolet" />
  <img src="https://img.shields.io/badge/loader-melonloader-orange" />
  <img src="https://img.shields.io/badge/version-1.0.0-purple" />
</p>

---

## Table of Contents
- Overview
- Installation
- Features
- Contributing
- Final Notes
- License

---

## Overview

DevourCore is a modular client built for DEVOUR using MelonLoader.  
It aims to improve performance, provide visual customization, and offer QoL features and speedrunning tools.

---

## Installation

### 1. Install .NET 6.0
Download and install the runtime if you don’t already have it.

### 2. Install MelonLoader
Download MelonLoader and point it to `DEVOUR.exe`.

### 3. Run DEVOUR once
MelonLoader will generate the required folders automatically.

### 4. Install DevourCore
Download **DevourCore.dll** from Releases and place it in:

```
DEVOUR/Mods/
```

### 5. Launch DEVOUR
DevourCore should load automatically.

---

## Features

### Optimization
![Optimization](./images/optimize.png)
- Render distance control  
- Weather and particle toggles  

### HSV Customization
![HSV](./images/hsv.png)
- 666 icon HSV  
- Outfit HSV  

### Speedrun Tools
![Speedrun](./images/speedrun.png)
- Instant interaction  
- Attic spawn  
- Auto-start options  

### Custom FOV
![FOV](./images/fov.png)
- FOV slider  
- Hotkey toggle  

### Anticheat
![Anticheat](./images/anticheat.png)
- Speed anomaly detection  
- Adjustable alerts  

### Menu Customization
![Menu](./images/menu.png)
- Map-based menu backgrounds  
- Music toggle  

### Client Settings
![Settings](./images/settings.png)
- Menu keybind  
- Theme color  
- Category visibility  
- Reset settings  

---

## Contributing

Contributions are welcome.  
To help keep the project consistent, please follow these guidelines.

### Project Principles
- Runtime: MelonLoader + DEVOUR  
- Language: C#  
- Keep features modular  
- Use clear naming  
- Avoid unnecessary dependencies  

### Workflow

#### Fork the repository

#### Create a feature branch
```
git checkout -b feature/my-feature
```

#### Make and test your changes

#### Commit with a clear message
```
git commit -m "Describe your change"
```

#### Push and open a Pull Request
Explain what changed, include screenshots if needed, and mention any limitations.

### Coding Style
- Keep methods/classes focused  
- Comment non-obvious behavior  
- Use configs instead of hardcoded values  

### Reporting Issues
Provide:
- DEVOUR version  
- MelonLoader version  
- DevourCore version  
- Steps to reproduce  
- Log excerpts if useful  

---

## Final Notes

DevourCore is designed for customization, experimentation, and QoL improvements.  
Please use it responsibly and avoid disrupting other players.

---

## License

DevourCore is released under GNU GPL-3.0.  
See the `LICENSE` file for details.
