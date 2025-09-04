using UnityEditor;
using UnityEngine;

public class ImportSettingsEnforcer : AssetPostprocessor
{
    // Texture defaults
    void OnPreprocessTexture()
    {
        var ti = (TextureImporter)assetImporter;
        // Reasonable defaults; adjust to your project's needs
        ti.textureType = TextureImporterType.Default;
        ti.mipmapEnabled = true;
        ti.maxTextureSize = 2048;
        ti.textureCompression = TextureImporterCompression.CompressedHQ;
        ti.alphaIsTransparency = (ti.DoesSourceTextureHaveAlpha());
    }

    // Audio defaults
    void OnPreprocessAudio()
    {
        var ai = (AudioImporter)assetImporter;
        var settings = ai.defaultSampleSettings;
        settings.loadType = AudioClipLoadType.CompressedInMemory;
        settings.quality = 0.7f;
        ai.defaultSampleSettings = settings;
        ai.forceToMono = false;
    }

    [MenuItem("Tools/Assets/Reimport Selected With Defaults")]
    public static void ReimportSelected()
    {
        foreach (var obj in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }
        Debug.Log("Reimported selected assets with enforced import settings.");
    }
}

