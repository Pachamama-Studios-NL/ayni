using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    [Tooltip("Scene to load at startup if not already loaded.")]
    public string sceneToLoad = "Game";

    [Tooltip("Load the target scene additively instead of single.")]
    public bool loadAdditive = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (string.IsNullOrEmpty(sceneToLoad)) return;

        // If already in the target scene, do nothing
        var active = SceneManager.GetActiveScene();
        if (active.name == sceneToLoad) return;

        var mode = loadAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        SceneManager.LoadSceneAsync(sceneToLoad, mode);
    }
}

