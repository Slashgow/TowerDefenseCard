using System;
using UnityEngine;
using UnityEngine.UI;

public class UIGameSettings : MonoSingleton<UIGameSettings>
{
    [SerializeField] private Toggle enablePauseToggle;
    [SerializeField] private Toggle enableScreenShakeToggle;

    public void SetPauseToggle(bool value) => enablePauseToggle.isOn = value;

    private void Start()
    {
        LoadSettings();
    }


    private void LoadSettings()
    {
        enablePauseToggle.isOn = GameSettingsManager.Instance.IsPauseEnable;
        enableScreenShakeToggle.isOn = GameSettingsManager.Instance.IsScreenShakeEnable;
    }

    public void SetPause(bool value) => GameSettingsManager.Instance.SetIsPauseEnable(value);

    public void SetScreenShake(bool value) => GameSettingsManager.Instance.SetIsScreenShakeEnable(value);
}
