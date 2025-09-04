using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class ProjectValidator
{
    [MenuItem("Tools/Project/Run Validation")] 
    public static void Run()
    {
        var issues = new List<string>();

        // 1) Scenes present and in Build Settings
        ValidateScenes(issues);

        // 2) Render Pipeline assigned (URP/HDRP/etc.)
        ValidateRenderPipeline(issues);

        // 3) Missing .meta files (basic heuristic)
        ValidateMetas(issues);

        // 4) Large assets (>100MB) that might need LFS
        ValidateLargeFiles(issues, 100 * 1024 * 1024);

        if (issues.Count == 0)
        {
            Debug.Log("Project validation passed: no issues found.");
        }
        else
        {
            foreach (var i in issues)
            {
                Debug.LogWarning(i);
            }
            Debug.LogError($"Project validation completed with {issues.Count} issue(s). See warnings above.");
        }
    }

    private static void ValidateScenes(List<string> issues)
    {
        var guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
        if (guids.Length == 0)
        {
            issues.Add("No scenes found under Assets/Scenes. Use Tools/Scenes/Create Basic Scenes.");
        }

        var buildScenes = EditorBuildSettings.scenes;
        if (buildScenes == null || buildScenes.Length == 0)
        {
            issues.Add("Build Settings has no scenes. Use Tools/Scenes/Sync Build Settings.");
        }
    }

    private static void ValidateRenderPipeline(List<string> issues)
    {
        // Unity 2021+ exposes defaultRenderPipeline; currentRenderPipeline is the active (per-quality) one
        RenderPipelineAsset rp = null;
        try { rp = GraphicsSettings.defaultRenderPipeline; } catch { /* property may differ on older versions */ }
        if (rp == null)
        {
            try { rp = GraphicsSettings.currentRenderPipeline; } catch { /* ignore */ }
        }
        if (rp == null)
        {
            issues.Add("No Render Pipeline Asset assigned (URP/HDRP). Check Project Settings > Graphics.");
        }
    }

    private static void ValidateMetas(List<string> issues)
    {
        // Check a subset to avoid heavy scans; still useful for catching accidental deletions
        var root = Application.dataPath;
        foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
        {
            if (file.EndsWith(".meta")) continue;
            // Skip Library or hidden/system, though under Assets there shouldn't be any
            var meta = file + ".meta";
            if (!File.Exists(meta))
            {
                var rel = "Assets" + file.Substring(root.Length).Replace('\\', '/');
                issues.Add($"Missing meta for: {rel}. Consider reimporting or forcing meta creation.");
            }
        }
    }

    private static void ValidateLargeFiles(List<string> issues, long threshold)
    {
        var root = Application.dataPath;
        foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
        {
            if (file.EndsWith(".meta")) continue;
            try
            {
                var info = new FileInfo(file);
                if (info.Length >= threshold)
                {
                    var rel = "Assets" + file.Substring(root.Length).Replace('\\', '/');
                    issues.Add($"Large asset (>100MB): {rel} ({info.Length / (1024*1024)} MB). Ensure Git LFS tracks this type.");
                }
            }
            catch { /* ignore permission or transient errors */ }
        }
    }
}

