using System;
using UnityEngine;
using UnityEngine.UI;

public class UITime : MonoBehaviour
{
    [SerializeField] private UIInput uiInput;
    [SerializeField] private Button pauseButton, resumeButton;
    [SerializeField] private Button speedUpButton, speedDownButton;

    private void Start()
    {
        pauseButton.onClick.AddListener(OnPause);
        resumeButton.onClick.AddListener(() => OnResume(false));

        speedUpButton.onClick.AddListener(OnSpeedUp);
        speedDownButton.onClick.AddListener(OnSpeedDown);

        resumeButton.gameObject.SetActive(false);
        speedDownButton.gameObject.SetActive(false);

        uiInput.OnStartReceivingInput += OnStartReceivingInput;
        uiInput.OnStopReceivingInput += OnStopReceivingInput;

        uiInput.OnPauseBlocked += UiInput_OnPauseBlocked;
        uiInput.OnPauseUnBlocked += UiInput_OnPauseUnBlocked;

        GameSettingsManager.OnChangePauseSettings += GameSettingsManager_OnChangePauseSettings;

        if (GameSettingsManager.Instance.IsPauseEnable)
        {
            pauseButton.interactable = true;
            resumeButton.interactable = true;
        }
        else
        {
            pauseButton.interactable = false;
            resumeButton.interactable = false;
        }
    }

 

    private void OnDisable()
    {
        pauseButton.onClick.RemoveAllListeners();
        resumeButton.onClick.RemoveAllListeners();
        speedUpButton.onClick.RemoveAllListeners();
        speedDownButton.onClick.RemoveAllListeners();
        uiInput.OnStartReceivingInput -= OnStartReceivingInput;
        uiInput.OnStopReceivingInput -= OnStopReceivingInput;
        uiInput.OnPauseBlocked -= UiInput_OnPauseBlocked;
        uiInput.OnPauseUnBlocked -= UiInput_OnPauseUnBlocked;
        GameSettingsManager.OnChangePauseSettings -= GameSettingsManager_OnChangePauseSettings;
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

    public void OnResume(bool bypassInput)
    {
        if (!bypassInput && !uiInput.IsReceivingInput)
            return;

        //Debug.Log("resume");
        pauseButton.gameObject.SetActive(true);
        resumeButton.gameObject.SetActive(false);

        speedUpButton.interactable = true;
        speedDownButton.interactable = true;
        OnSpeedDown();

        GameManager.Instance.Resume();
       
    }

    public void OnPause()
    {
        if (!uiInput.IsReceivingInput)
            return;

        if (!GameSettingsManager.Instance.IsPauseEnable)
            return;
        //Debug.Log("Pause");
        pauseButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(true);

        speedUpButton.interactable = false;
        speedDownButton.interactable = false;
        GameManager.Instance.Pause();
      
    }

    public void TogglePlayResume()
    {
        //if(pauseButton.gameObject.activeSelf)
        if (!GameManager.Instance.IsPaused)
            OnPause();
        else
            OnResume(false);
    }

    public void ToggleSpeed()
    {
        if(speedUpButton.gameObject.activeSelf && speedUpButton.interactable)
            OnSpeedUp();
        else if(speedDownButton.gameObject.activeSelf && speedDownButton.interactable)
            OnSpeedDown();
    }

    private void OnStopReceivingInput()
    {
        Debug.Log("on stop receive input");
       
        pauseButton.interactable = false;
        resumeButton.interactable = false;
        speedUpButton.interactable = false;
        speedDownButton.interactable = false;
    }

    private void OnStartReceivingInput()
    {
        Debug.Log(" on start receive input");

        if (GameSettingsManager.Instance.IsPauseEnable && !uiInput.IsPauseBlocked)
        {
            pauseButton.interactable = true;
            resumeButton.interactable = true;
        }
        speedUpButton.interactable = true;
        speedDownButton.interactable = true;
    }

    private void GameSettingsManager_OnChangePauseSettings(bool value)
    {
        if (value && !uiInput.IsPauseBlocked)
            UiInput_OnPauseUnBlocked();
        else
            UiInput_OnPauseBlocked();
    }

    private void UiInput_OnPauseUnBlocked()
    {
        Debug.Log(" on pause unblocked");
        pauseButton.interactable = true;
        resumeButton.interactable = true;
    }

    private void UiInput_OnPauseBlocked()
    {
        Debug.Log(" on pause blocked");
        pauseButton.interactable = false;
        resumeButton.interactable = false;
    }
}
