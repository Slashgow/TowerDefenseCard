using UnityEngine;
using UnityEngine.Audio;

public class VolumeManager : MonoSingleton<VolumeManager>
{
    [SerializeField, Range(0.0001f, 1f)]
    private float defaultMainVolume, defaultMusicVolume, defaultSfXVolume, defaultTanukiVolume;

    [SerializeField]
    private AudioMixer audioMixer;

    public float CurrentMainVolume { get; private set; }
    public float CurrentMusicVolume { get; private set; }
    public float CurrentSFXVolume { get; private set; }
    public float CurrentTanukiVolume { get; private set; }

    public const string MAIN_VOLUME_ID = "MAIN_VOLUME";
    public const string MUSIC_VOLUME_ID = "MUSIC_VOLUME";
    public const string SFX_VOLUME_ID = "SFX_VOLUME";
    public const string TANUKI_VOLUME_ID = "TANUKI_VOLUME";


    protected override void Awake()
    {
        base.Awake();

        if(PlayerPrefs.HasKey(MAIN_VOLUME_ID))
            CurrentMainVolume = PlayerPrefs.GetFloat(MAIN_VOLUME_ID);
        else
            CurrentMainVolume = defaultMainVolume;

        if(PlayerPrefs.HasKey(MUSIC_VOLUME_ID))
            CurrentMusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_ID);
        else
            CurrentMusicVolume= defaultMusicVolume;

        if(PlayerPrefs.HasKey(SFX_VOLUME_ID))
            CurrentSFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME_ID);
        else 
            CurrentSFXVolume = defaultSfXVolume;

        if(PlayerPrefs.HasKey(TANUKI_VOLUME_ID))
            CurrentTanukiVolume = PlayerPrefs.GetFloat(TANUKI_VOLUME_ID);
        else
            CurrentTanukiVolume= defaultTanukiVolume;
    }

    private void Start()
    {
        SetMainVolume(CurrentMainVolume);
        SetMusicVolume(CurrentMusicVolume);
        SetSFXVolume(CurrentSFXVolume);
        SetTanukiVolume(CurrentTanukiVolume);
    }

    public void SetMainVolume(float value)
    {
        audioMixer.SetFloat("mainVolume", Mathf.Log10(value) * 20);
        CurrentMainVolume = value;
        PlayerPrefs.SetFloat(MAIN_VOLUME_ID, CurrentMainVolume);
    }
    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(value) * 20);
        CurrentMusicVolume = value;
        PlayerPrefs.SetFloat(MUSIC_VOLUME_ID, CurrentMusicVolume);
    }
    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(value) * 20);
        CurrentSFXVolume = value;
        PlayerPrefs.SetFloat(SFX_VOLUME_ID, CurrentSFXVolume);
    }

    public void SetTanukiVolume(float value)
    {
        audioMixer.SetFloat("tanukiVolume", Mathf.Log10(value) * 20);
        CurrentTanukiVolume = value;
        PlayerPrefs.SetFloat(TANUKI_VOLUME_ID, CurrentTanukiVolume);
    }
}
