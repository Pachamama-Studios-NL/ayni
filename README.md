# Unity Project

This repository contains a Unity project built with Unity `6000.2.2f1`.

## Getting Started

- Unity: Install via Unity Hub (`6000.2.2f1`).
- Git LFS: Install and enable once: `git lfs install`.
- Clone with LFS: `git clone --recursive` (or ensure LFS pulls on first checkout).

## Repo Conventions

- Asset serialization: Use Unity Editor > Project Settings > Editor > Asset Serialization = Force Text.
- Visible meta files: Editor > Version Control = Visible Meta Files.
- Do not commit generated folders: `Library/`, `Temp/`, `Build/`, etc. (enforced by `.gitignore`).
- Large binaries tracked by LFS: common textures, audio, models, videos, fonts, and plugin binaries (see `.gitattributes`).

## Smart Merge (UnityYAMLMerge)

Unity scenes, prefabs, materials, and assets merge via Unity Smart Merge when conflicts occur.

Configure locally (once):

```bash
git config merge.unityyamlmerge.name "Unity SmartMerge"
git config merge.unityyamlmerge.driver "UnityYAMLMerge merge -p %O %A %B %P"
```

The repository applies Smart Merge to: `*.unity`, `*.prefab`, `*.asset`, `*.mat`, `*.anim`, `*.controller`.

## Line Endings

The repo normalizes text with `* text=auto` and prefers LF for source/text files. If you’re on Windows, we recommend:

```bash
git config core.autocrlf input
```

## Packages and Project Version

The project pins packages via `Packages/manifest.json` and `Packages/packages-lock.json`. Unity version is recorded in `ProjectSettings/ProjectVersion.txt` and used by CI.

## Continuous Integration

GitHub Actions workflow (`.github/workflows/unity-ci.yml`) runs tests on PRs and builds on `staging` and `main` (license required). Version tags (`v*`) also build.

- Secrets required for builds:
  - `UNITY_LICENSE`: Unity (Personal/Plus/Pro) license file content. See game-ci docs for how to obtain the base64 license string.
- The workflow auto-detects Unity version from `ProjectSettings/ProjectVersion.txt` and caches `Library/` to speed up imports.

## Running Tests Locally

From the project root:

```bash
"/path/to/Unity" -batchmode -projectPath "$(pwd)" -runTests -testPlatform EditMode -logFile -quit
"/path/to/Unity" -batchmode -projectPath "$(pwd)" -runTests -testPlatform PlayMode -logFile -quit
```

## LFS Migration (if adding to an existing repo)

If assets were committed before `.gitattributes` existed, consider migrating:

```bash
git lfs migrate import --include="*.png,*.jpg,*.psd,*.fbx,*.wav,*.mp4,*.ttf,*.dll"
```

Coordinate with your team before rewriting history. Alternatively, re-stage going forward:

```bash
git rm --cached -r . && git reset -- . && git add . && git commit -m "Re-stage assets under LFS"
```

## Branching and Reviews

- Base new feature branches off `staging`.
- Open PRs from feature branches into `staging` for testing and review.
- CI runs tests on PRs to `staging` and on pushes to `staging`.
- Builds run on `staging` (for QA validation) and on `main`/tags (for releases).
- Optional: tag releases (`vX.Y.Z`) to produce release builds.
- Avoid committing builds; publish build artifacts via CI instead.

## Branch Protection

- Protect `staging` and `main` in Settings → Branches.
- Require PRs with at least 1 approval and passing checks.
- Required check name: `Unity CI / tests`.
- Keep branches up to date and require conversations resolved.
- See `.github/BRANCH_PROTECTION.md` for detailed recommendations.

## Using AI Assistants

- Unity AI — best for: quick in‑Editor help that can apply changes directly to open assets.
  - Code/shader snippets, small MonoBehaviours, inspector/tooling tweaks.
  - Asset generation (textures/sprites) via AI Generators.
  - Contextual edits to scenes/prefabs while the Editor is open.
- Codex CLI — best for: repo‑wide work outside the Editor.
  - CI setup, tests/coverage, GitHub workflows, documentation, codebase refactors.
  - Multi‑file edits, scripts under `Assets/Editor/`, package/version pinning.
  - Deterministic changes you want reviewed in PRs before entering the Editor.
- Practical note: Unity AI packages and availability vary by org/seat; pinning occurs via `Packages/manifest.json`.
- Privacy: avoid pasting proprietary code into cloud tools unless approved by your org policy.

## Unity AI Setup

- Enable in Editor:
  - Use the top toolbar AI menu → Install/Enable AI packages (or Window → Package Manager → Unity Registry, install `com.unity.ai.assistant`, `com.unity.ai.generators`, and dependencies).
  - Open the AI panel from the AI menu and sign in if prompted.
- Verify install:
  - Package Manager shows the AI packages; AI dropdown exposes Assistant and Generators.
- What to commit:
  - `Packages/manifest.json` and `Packages/packages-lock.json`.
  - Any new settings under `ProjectSettings/Packages/com.unity.ai.*`.
  - Do not commit `Library/` or cache directories.
- Access/licensing:
  - Availability depends on your Unity org and seat assignment; not all users will see the packages.
- Security:
  - Follow org policy for sharing proprietary code with cloud tools.

## License

MIT — see `LICENSE` for details.
