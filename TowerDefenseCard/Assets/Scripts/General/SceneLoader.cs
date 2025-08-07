using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : PersistentMonoSingleton<SceneLoader>
{
    private float transitionTime;
    private Animator transitionAnimator;
    private float timeElapsed = 0f;

    public event Action<int> OnSceneLoaded;

    private void OnEnable() => SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= SceneManager_sceneLoaded;

    protected override void Awake()
    {
        base.Awake();
        TransitionManager transitionManager = FindFirstObjectByType<TransitionManager>();
        RegisterTransitionAnimator(transitionManager.Animator, transitionManager.TransitionTime);
    }
    public void RegisterTransitionAnimator(Animator animator, float transitionTime)
    {
        transitionAnimator = animator;
        this.transitionTime = transitionTime;
    }

    public void UnregisterTransitionAnimator()
    {
        transitionAnimator = null;
        transitionTime = 0f;
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        OnSceneLoaded?.Invoke(scene.buildIndex);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene( (SceneManager.GetActiveScene().buildIndex + 1) % (SceneManager.sceneCountInBuildSettings));
    }

    public void LoadNextSceneAsync()
    {
        StartCoroutine(LoadNextSceneAsyncCoroutine());
    }

    private IEnumerator LoadNextSceneAsyncCoroutine()
    {
        timeElapsed = 0f;
        transitionAnimator.SetTrigger("EndScene");

        while (timeElapsed < transitionTime)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync((SceneManager.GetActiveScene().buildIndex + 1) % (SceneManager.sceneCountInBuildSettings));

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        transitionAnimator.SetTrigger("StartScene");
    }
}
