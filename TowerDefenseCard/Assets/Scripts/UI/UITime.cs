using System;
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
        resumeButton.onClick.AddListener(() => OnResume(false));

        speedUpButton.onClick.AddListener(OnSpeedUp);
        speedDownButton.onClick.AddListener(OnSpeedDown);

        resumeButton.gameObject.SetActive(false);
        speedDownButton.gameObject.SetActive(false);

        uiInput.OnStartReceivingInput += OnStartReceivingInput;
        uiInput.OnStopReceivingInput += OnStopReceivingInput;
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

    public void OnResume(bool bypassInput)
    {
        if (!bypassInput && !uiInput.IsReceivingInput)
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
        pauseButton.interactable = false;
        resumeButton.interactable = false;
        speedUpButton.interactable = false;
        speedDownButton.interactable = false;
    }

    private void OnStartReceivingInput()
    {
        pauseButton.interactable = true;
        resumeButton.interactable = true;
        speedUpButton.interactable = true;
        speedDownButton.interactable = true;
    }

}
