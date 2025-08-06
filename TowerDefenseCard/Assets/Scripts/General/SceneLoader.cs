using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : PersistentMonoSingleton<SceneLoader>
{
    [SerializeField] private Animator transitionAnimator;
    [SerializeField, Range(0, 10f)] private float transitionTime;

    private float timeElapsed = 0f;

    public readonly int END_SCENE = Animator.StringToHash("EndScene");

    public event Action<int> OnSceneLoaded;

    private void OnEnable() => SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= SceneManager_sceneLoaded;

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
        SceneManager.LoadScene( (SceneManager.GetActiveScene().buildIndex + 1) % (SceneManager.sceneCount + 1));
    }

    public void LoadNextSceneAsync()
    {
        StartCoroutine(LoadNextSceneAsyncCoroutine());
    }

    private IEnumerator LoadNextSceneAsyncCoroutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync((SceneManager.GetActiveScene().buildIndex + 1) % (SceneManager.sceneCount + 1));

        timeElapsed = 0f;
        transitionAnimator.SetTrigger(END_SCENE);

        while (!asyncLoad.isDone && transitionTime < timeElapsed)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
