using UnityEngine;
using UnityEngine.UI;

public class UITime : MonoBehaviour
{
    [SerializeField] private Button pauseButton, resumeButton;

    private void OnEnable()
    {
        pauseButton.onClick.AddListener(OnPause);
        resumeButton.onClick.AddListener(OnResume);
        resumeButton.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        pauseButton.onClick.RemoveAllListeners();
        resumeButton.onClick.RemoveAllListeners();
    }

    private void OnResume()
    {
        GameManager.Instance.Resume();
        pauseButton.gameObject.SetActive(true);
        resumeButton.gameObject.SetActive(false);
    }

    private void OnPause()
    {
        GameManager.Instance.Pause();
        pauseButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(true);
    }
}
