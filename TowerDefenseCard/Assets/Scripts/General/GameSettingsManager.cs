using System;
using UnityEngine;

public class GameSettingsManager : MonoSingleton<GameSettingsManager>
{
    [SerializeField] private bool defaultPauseEnable = true;
    [SerializeField] private bool defaultScreenShake = true;

    public bool IsPauseEnable {  get; private set; }
    public bool IsScreenShakeEnable { get; private set; }

    public const string IS_PAUSE_ENABLE_ID = "IsPauseEnable";
    public const string IS_SCREEN_SHAKE_ENABLE_ID = "IsScreenShakeEnable";

    private bool isTemporaryPauseSave;

    public static event Action<bool> OnChangePauseSettings;
    protected override void Awake()
    {
        base.Awake();

        IsPauseEnable = PlayerPrefs.GetInt(IS_PAUSE_ENABLE_ID, defaultPauseEnable ? 1 : 0) == 1;
        IsScreenShakeEnable = PlayerPrefs.GetInt(IS_SCREEN_SHAKE_ENABLE_ID, defaultScreenShake ? 1 : 0) == 1;

        Debug.Log($"Is Pause Enable on awake : {IsPauseEnable}");
        Debug.Log($"IsScreenShakeEnable on awake : {IsScreenShakeEnable}");
    }

    private void Start()
    {
        ApplySettings();
    }

    private void ApplySettings()
    {
        SetIsPauseEnable(IsPauseEnable);
        SetIsScreenShakeEnable(IsScreenShakeEnable);
    }

    public void SetIsScreenShakeEnable(bool value)
    {
        IsScreenShakeEnable = value;
        PlayerPrefs.SetInt(IS_SCREEN_SHAKE_ENABLE_ID, IsScreenShakeEnable ? 1 : 0);
    }

    public void SetIsPauseEnable(bool value)
    {
        IsPauseEnable = value;
        OnChangePauseSettings?.Invoke(value);
        PlayerPrefs.SetInt(IS_PAUSE_ENABLE_ID , IsPauseEnable ? 1 :0);
        Debug.Log($"save Is Pause Enable : {IsPauseEnable}");
    }

    public void AllowTemporaryPause()
    {
        isTemporaryPauseSave = IsPauseEnable;
        IsPauseEnable = true;
        UIGameSettings.Instance.SetPauseToggle(IsPauseEnable);
        OnChangePauseSettings?.Invoke(IsPauseEnable);
    }

    public void BackToPreviousPauseSetting()
    {
        IsPauseEnable = true;
        UIGameSettings.Instance.SetPauseToggle(IsPauseEnable);
        OnChangePauseSettings?.Invoke(IsPauseEnable);
    }
}
