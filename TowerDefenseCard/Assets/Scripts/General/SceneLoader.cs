using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : PersistentMonoSingleton<SceneLoader>
{
    private TransitionManager transitionManager;
    private float timeElapsed = 0f;

    public event Action<int> OnSceneLoaded;

    private void OnEnable() => SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= SceneManager_sceneLoaded;

    protected override void Awake()
    {
        base.Awake();
        TransitionManager transitionManager = FindFirstObjectByType<TransitionManager>();
        RegisterTransitionAnimator(transitionManager);
    }
    public void RegisterTransitionAnimator(TransitionManager transitionManager) => this.transitionManager = transitionManager;
    public void UnregisterTransitionAnimator() => this.transitionManager = null;

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        OnSceneLoaded?.Invoke(scene.buildIndex);
    }

    public void LoadMainMenu() => LoadSceneAsync(0);
    public void LoadNextScene()
    {
        SceneManager.LoadScene( (SceneManager.GetActiveScene().buildIndex + 1) % (SceneManager.sceneCountInBuildSettings));
    }

    public void LoadNextSceneAsync()
    {
        StartCoroutine(LoadSceneAsyncCoroutine((SceneManager.GetActiveScene().buildIndex + 1) % (SceneManager.sceneCountInBuildSettings)));
    }

    public void LoadSceneAsync(int buildIndex) => StartCoroutine(LoadSceneAsyncCoroutine(buildIndex));

    private IEnumerator LoadSceneAsyncCoroutine(int buildIndex)
    {
        timeElapsed = 0f;
        transitionManager.EndSceneAnimation();

        while (timeElapsed < transitionManager.TransitionTime)
        {
            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(buildIndex);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
