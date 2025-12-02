# Contributing to DevourCore

Thank you for considering contributing to **DevourCore**!

This document describes a simple process to keep the project clean, stable, and easy to work with.

---

## 📂 Project Style & Principles

- Target runtime: **MelonLoader + DEVOUR**
- Language: **C#**
- Keep changes **modular** (tabs, features, helpers).
- Prefer **clear names** over short ones.
- Avoid unnecessary dependencies.

---

## 🧩 Branch & Pull Request Workflow

1. **Fork** the repository.
2. Create a new branch for your change:
   ```bash
   git checkout -b feature/my-new-feature
   ```
3. Make your changes and add tests or examples if applicable.
4. Run your build locally and verify it works in-game.
5. Commit with a clear message:
   ```bash
   git commit -m "Add X feature to Speedrun tab"
   ```
6. Push your branch and open a **Pull Request**:
   - Describe what you changed.
   - Attach screenshots if it's a UI/visual change.
   - Mention any known limitations.

---

## ✅ Coding Guidelines

- Follow existing code style in the project.
- Keep classes and methods **focused on one responsibility**.
- Use **comments** where behavior is non-obvious.
- Avoid hardcoding map-specific or player-specific data where possible; use configs/preferences instead.

---

## 🐞 Reporting Issues

If you find a bug or want to request a feature:

- Open a **GitHub Issue**.
- Include:
  - DEVOUR version
  - MelonLoader version
  - DevourCore version
  - Steps to reproduce
  - Log excerpts if relevant (without sensitive information)

---

## 🤝 Code of Conduct

- Be respectful.
- Keep discussions technical and constructive.
- Remember that everyone is here to learn and improve.

---

Thank you again for helping improve **DevourCore**.
