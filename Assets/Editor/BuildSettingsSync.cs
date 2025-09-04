using UnityEditor;

public static class BuildSettingsSync
{
    [MenuItem("Tools/Build/Sync Build Settings")]
    public static void Sync()
    {
        // Reuse SceneGenerator's sync if available; otherwise, inline minimal logic
        var method = typeof(SceneGenerator).GetMethod("SyncBuildSettingsMenu", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        if (method != null)
        {
            method.Invoke(null, null);
        }
        else
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
        UnityEngine.Debug.Log("Build Settings synced.");
    }
}

