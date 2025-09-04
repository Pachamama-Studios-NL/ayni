using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildPipelineMenu
{
    [MenuItem("Tools/Build/Windows64")]
    public static void BuildWin64()
    {
        var outputDir = Path.Combine("Builds", "Windows64");
        Directory.CreateDirectory(outputDir);
        var exePath = Path.Combine(outputDir, "Ayni.exe");
        var report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, exePath, BuildTarget.StandaloneWindows64, BuildOptions.None);
        Debug.Log($"Build result: {report.summary.result} size={report.summary.totalSize}");
        if (report.summary.result != BuildResult.Succeeded)
        {
            Debug.LogError("Windows64 build failed. Check console for details.");
        }
    }
}

