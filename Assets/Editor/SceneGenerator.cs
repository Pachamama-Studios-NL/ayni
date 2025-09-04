using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneGenerator
{
    private const string ScenesFolder = "Assets/Scenes";

    [MenuItem("Tools/Scenes/Create Basic Scenes")]
    public static void CreateBasicScenes()
    {
        EnsureScenesFolder();
        CreateSceneIfMissing("MainMenu");
        CreateSceneIfMissing("Game");
        SyncBuildSettings();
        Debug.Log("Created basic scenes (if missing) and synced Build Settings.");
    }

    

    [MenuItem("Tools/Scenes/Sync Build Settings")] 
    public static void SyncBuildSettingsMenu()
    {
        SyncBuildSettings();
        Debug.Log("Build Settings synced to scenes under Assets/Scenes.");
    }

    private static void EnsureScenesFolder()
    {
        if (!AssetDatabase.IsValidFolder(ScenesFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
        }
    }

    private static void CreateSceneIfMissing(string sceneName)
    {
        var path = Path.Combine(ScenesFolder, sceneName + ".unity");
        if (File.Exists(path))
        {
            return;
        }

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var camGO = new GameObject("Main Camera");
        camGO.AddComponent<Camera>();
        camGO.tag = "MainCamera";

        var lightGO = new GameObject("Directional Light");
        var light = lightGO.AddComponent<Light>();
        light.type = LightType.Directional;

        EditorSceneManager.SaveScene(scene, path);
        AssetDatabase.ImportAsset(path);
    }

    private static void SyncBuildSettings()
    {
        var guids = AssetDatabase.FindAssets("t:Scene", new[] { ScenesFolder });
        var scenes = new EditorBuildSettingsScene[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
            scenes[i] = new EditorBuildSettingsScene(path, true);
        }
        EditorBuildSettings.scenes = scenes;
    }
}
