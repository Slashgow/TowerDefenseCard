using UnityEngine;
using UnityEngine.UI;

public class UITime : MonoBehaviour
{
    [SerializeField] private Button pauseButton, resumeButton;
    [SerializeField] private Button speedUpButton, speedDownButton;

    private void OnEnable()
    {
        pauseButton.onClick.AddListener(OnPause);
        resumeButton.onClick.AddListener(OnResume);

        speedUpButton.onClick.AddListener(OnSpeedUp);
        speedDownButton.onClick.AddListener(OnSpeedDown);

        resumeButton.gameObject.SetActive(false);
        speedDownButton.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        pauseButton.onClick.RemoveAllListeners();
        resumeButton.onClick.RemoveAllListeners();
        speedUpButton.onClick.RemoveAllListeners();
        speedDownButton.onClick.RemoveAllListeners();
    }

    private void OnSpeedDown()
    {
        GameManager.Instance.ResetGameSpeed();

        speedUpButton.gameObject.SetActive(true);
        speedDownButton.gameObject.SetActive(false);
    }

    private void OnSpeedUp()
    {
        GameManager.Instance.SpeedUpGame();

        speedUpButton.gameObject.SetActive(false);
        speedDownButton.gameObject.SetActive(true);
    }

    private void OnResume()
    {
        GameManager.Instance.Resume();
        pauseButton.gameObject.SetActive(true);
        resumeButton.gameObject.SetActive(false);

        speedUpButton.interactable = true;
        speedDownButton.interactable = true;
    }

    private void OnPause()
    {
        GameManager.Instance.Pause();
        pauseButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(true);

        speedUpButton.interactable = false;
        speedDownButton.interactable = false;
    }

    public void TogglePlayResume()
    {
        if(pauseButton.gameObject.activeSelf)
            OnPause();
        else
            OnResume();
    }

    public void ToggleSpeed()
    {
        if(speedUpButton.gameObject.activeSelf)
            OnSpeedUp();
        else
            OnSpeedDown();
    }
}
