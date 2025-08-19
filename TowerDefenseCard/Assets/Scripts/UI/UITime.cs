using UnityEngine;
using UnityEngine.UI;

public class UITime : MonoBehaviour
{
    [SerializeField] private UIInput uiInput;
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
        if(!uiInput.IsReceivingInput)
            return;

        GameManager.Instance.ResetGameSpeed();

        speedUpButton.gameObject.SetActive(true);
        speedDownButton.gameObject.SetActive(false);
    }

    private void OnSpeedUp()
    {
        if (!uiInput.IsReceivingInput)
            return;

        GameManager.Instance.SpeedUpGame();

        speedUpButton.gameObject.SetActive(false);
        speedDownButton.gameObject.SetActive(true);
    }

    public void OnResume()
    {
        if (!uiInput.IsReceivingInput)
            return;

        GameManager.Instance.Resume();
        pauseButton.gameObject.SetActive(true);
        resumeButton.gameObject.SetActive(false);

        speedUpButton.interactable = true;
        speedDownButton.interactable = true;
    }

    public void OnPause()
    {
        if (!uiInput.IsReceivingInput)
            return;

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
        if(speedUpButton.gameObject.activeSelf && speedUpButton.interactable)
            OnSpeedUp();
        else if(speedDownButton.gameObject.activeSelf && speedDownButton.interactable)
            OnSpeedDown();
    }
}
