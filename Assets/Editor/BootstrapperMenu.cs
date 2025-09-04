using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BootstrapperMenu
{
    [MenuItem("Tools/Play/Set Startup Scene From Selection", validate = true)]
    private static bool ValidateSetStartup() => Selection.activeObject is SceneAsset;

    [MenuItem("Tools/Play/Set Startup Scene From Selection")] 
    public static void SetStartupScene()
    {
        var scene = Selection.activeObject as SceneAsset;
        if (scene == null)
        {
            Debug.LogWarning("Select a Scene asset in the Project window.");
            return;
        }
        EditorSceneManager.playModeStartScene = scene;
        Debug.Log($"Play Mode startup scene set to: {scene.name}");
    }

    [MenuItem("Tools/Play/Clear Startup Scene")] 
    public static void ClearStartupScene()
    {
        EditorSceneManager.playModeStartScene = null;
        Debug.Log("Play Mode startup scene cleared (uses current open scene).");
    }

    [MenuItem("Tools/Play/Open Startup Scene")] 
    public static void OpenStartupScene()
    {
        var s = EditorSceneManager.playModeStartScene;
        if (s == null)
        {
            Debug.LogWarning("No startup scene set.");
            return;
        }
        var path = AssetDatabase.GetAssetPath(s);
        if (!string.IsNullOrEmpty(path))
        {
            EditorSceneManager.OpenScene(path);
            Debug.Log($"Opened startup scene: {s.name}");
        }
    }
}

