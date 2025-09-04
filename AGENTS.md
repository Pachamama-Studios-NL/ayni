# Agents Guide for This Unity Project

This document helps automation agents and maintainers work efficiently in this repo. It captures expected workflows, project conventions, and high‑leverage Editor automations that reduce manual Unity work.

## Environment & Conventions

- Unity: `6000.2.2f1` (see `ProjectSettings/ProjectVersion.txt`).
- Branching: feature branches base off `staging`; merge to `staging` for test/validation; merge to `main` for deployment. See README and `.github/BRANCH_PROTECTION.md`.
- CI: `.github/workflows/unity-ci.yml` runs Edit/Play Mode tests with coverage; builds on `main`/tags when a license is present.
- Tests: NUnit via Unity Test Framework. Assemblies in `Assets/Tests/EditMode` and `Assets/Tests/PlayMode`.
- Coverage: `com.unity.testtools.codecoverage` enabled in CI with filters excluding test/Unity assemblies. Reports upload as artifact `CodeCoverage`.
- LFS: `.gitattributes` tracks common binary assets. Ensure `git lfs install` is run locally.

## Useful Commands

- Run EditMode tests (headless):
  - `"/path/to/Unity" -batchmode -projectPath "$(pwd)" -runTests -testPlatform EditMode -logFile -quit`
- Run PlayMode tests (headless):
  - `"/path/to/Unity" -batchmode -projectPath "$(pwd)" -runTests -testPlatform PlayMode -logFile -quit`
- Enable coverage (local):
  - Add `-enableCodeCoverage -coverageOptions "generateHtmlReport;path=CodeCoverage"`

## Editor Automation: Recommended Scripts

Place Editor scripts under `Assets/Editor/` (Unity compiles them into the Editor assembly). Below are suggested utilities with brief outlines. Ask before adding if scope is unclear.

1) Scene Generator (Menu)
- Purpose: Create boilerplate scenes and add them to Build Settings in order.
- Path: `Assets/Editor/SceneGenerator.cs`
- Sketch:
  - `MenuItem("Tools/Scenes/Create Basic Scenes")`
  - Ensure folder `Assets/Scenes` exists
  - Create scenes (Camera + Light; optional UI Canvas)
  - Add to `EditorBuildSettings.scenes` in order

2) Build Settings Sync
- Purpose: Ensure all scenes under `Assets/Scenes` are registered in Build Settings (idempotent).
- Path: `Assets/Editor/BuildSettingsSync.cs`
- Sketch:
  - Enumerate `AssetDatabase.FindAssets("t:Scene", new[]{"Assets/Scenes"})`
  - Build `EditorBuildSettingsScene[]` with `enabled = true`

3) Project Validator
- Purpose: One‑click validation for common pitfalls before commits/CI.
- Path: `Assets/Editor/ProjectValidator.cs`
- Checks:
  - Missing `.meta` files (rare with Visible Meta Files, but check)
  - Tag/Layer presence (e.g., required by code)
  - URP global settings assigned
  - Disallowed large files not tracked by LFS (e.g., >100MB)

4) Test Runner Shortcuts
- Purpose: Run Edit/Play Mode tests from a menu; optionally toggle coverage flags.
- Path: `Assets/Editor/TestRunnerMenu.cs`
- Sketch:
  - `MenuItem("Tools/Tests/Run EditMode")` → `TestRunnerApi` calls
  - `MenuItem("Tools/Tests/Run All (Coverage)")` → set `CodeCoverageSession.EnableCodeCoverage(true)` in 6000 or pass args via command API if available

5) Asset Import Settings Enforcer
- Purpose: Normalize import settings for textures/audio/models to avoid churn.
- Path: `Assets/Editor/ImportSettingsEnforcer.cs`
- Sketch:
  - Implement `AssetPostprocessor`
  - In `OnPreprocessTexture`/`OnPreprocessAudio`, set desired defaults (compression, sRGB, max size)

6) Bootstrapper (Play Entry)
- Purpose: Control which scene loads first when entering Play Mode in the Editor.
- Path: `Assets/Editor/BootstrapperMenu.cs` + `Assets/Scripts/Bootstrapper.cs`
- Sketch:
  - Editor menu to set a default startup scene or load a given scene additively
  - Runtime `Bootstrapper` loads target scene if not present

7) Addressables Helpers (optional if adopted)
- Purpose: Bulk‑label assets, validate groups, and build addressable content.
- Path: `Assets/Editor/AddressablesTools.cs`

8) Package Version Pin/Upgrade
- Purpose: Guardrail around `Packages/manifest.json` updates.
- Path: `Assets/Editor/PackagesGuard.cs`
- Sketch:
  - Provide menus: "Lock current versions", "Check for upgrades", "Apply safe upgrades" (reads/writes manifest; uses `UnityEditor.PackageManager`)

9) Code Templates / Script Wizards
- Purpose: Add menu items to create scripts with a standard header/namespace.
- Path: `Assets/Editor/ScriptTemplates.cs`

10) Build Pipeline Menu
- Purpose: Build targets locally in one click mirroring CI builder.
- Path: `Assets/Editor/BuildPipelineMenu.cs`
- Sketch:
  - `MenuItem("Tools/Build/Windows64")` → `BuildPipeline.BuildPlayer` with `BuildTarget.StandaloneWindows64`

## Minimal Code Sketches

These are compact examples; adapt as needed.

- Scene Generator
```
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public static class SceneGenerator
{
    [MenuItem("Tools/Scenes/Create Basic Scenes")]
    public static void CreateBasicScenes()
    {
        var scenesPath = "Assets/Scenes";
        if (!AssetDatabase.IsValidFolder(scenesPath))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
        }

        CreateScene("MainMenu");
        CreateScene("Game");
        SyncBuildSettings();
    }

    static void CreateScene(string name)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var cam = new GameObject("Main Camera");
        cam.AddComponent<Camera>();
        var light = new GameObject("Directional Light");
        light.AddComponent<Light>().type = LightType.Directional;
        var path = $"Assets/Scenes/{name}.unity";
        EditorSceneManager.SaveScene(scene, path);
        AssetDatabase.ImportAsset(path);
    }

    static void SyncBuildSettings()
    {
        var guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
        var scenes = new EditorBuildSettingsScene[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
            scenes[i] = new EditorBuildSettingsScene(path, true);
        }
        EditorBuildSettings.scenes = scenes;
    }
}
```

- Build Pipeline (Windows64)
```
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildPipelineMenu
{
    [MenuItem("Tools/Build/Windows64")]
    public static void BuildWin64()
    {
        var output = "Builds/Windows64";
        System.IO.Directory.CreateDirectory(output);
        var report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, output + "/Ayni.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
        Debug.Log($"Build result: {report.summary.result} size={report.summary.totalSize}");
    }
}
```

## Agent Playbook (High‑Level)

- Repo discovery: list files; read `Packages/manifest.json`, `ProjectSettings/ProjectVersion.txt`, and CI workflow.
- Validate setup: ensure Test Framework and Code Coverage packages exist.
- Test/coverage: prefer CI to compute coverage; local runs add `-enableCodeCoverage`.
- Editor automations: add scripts under `Assets/Editor/` and keep changes minimal and focused.
- Don’t commit `Library/`, `Temp/`, `Builds/`, or coverage output (`CodeCoverage/`).

## Unity AI vs. Codex CLI

- Prefer Unity AI Assistant when:
  - You are already in the Editor and need small, contextual changes (scripts, shaders, UI tweaks) applied live.
  - You want AI to modify open scenes/prefabs or generate assets via AI Generators.
  - You need rapid iteration with immediate visual feedback in Play Mode.

- Prefer Codex CLI when:
  - Making repository‑wide, reviewable changes (tests, CI, coverage, docs, package pins, editor tooling).
  - Performing multi‑file refactors or creating deterministic automation under `Assets/Editor/`.
  - You want to propose changes via PR with code review and CI validation before opening the Editor.

- Notes
  - Unity AI packages (e.g., `com.unity.ai.assistant`, `com.unity.ai.generators`) require org/seat access and may not appear for all users.
  - Keep secrets/keys out of the repo; avoid sharing sensitive code with cloud tools unless approved by policy.

## Future Enhancements

- PR coverage summary bot (parse coverage HTML/XML and post a comment).
- Static analysis (Roslyn analyzers) if team chooses to adopt.
- Automated asset compliance checks (texture max size, compression, mipmaps) tailored to platforms.

If you want any of the above Editor scripts implemented now, say the word and I’ll add them under `Assets/Editor/`.
