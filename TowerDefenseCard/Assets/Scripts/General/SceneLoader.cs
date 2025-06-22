using System;
using UnityEngine.SceneManagement;

public class SceneLoader : PersistentMonoSingleton<SceneLoader>
{
    public event Action<int> OnSceneLoaded;

    private void OnEnable() => SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= SceneManager_sceneLoaded;

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        OnSceneLoaded?.Invoke(scene.buildIndex);
    }
}
