using UnityEngine;
using UnityEngine.UI;

public class UIVolume : MonoSingleton<UIVolume>
{
    [SerializeField]
    private Slider mainVolumeSlider, musicSlider, sfxSlider, tanukiSlider;

    private void Start()
    {
        mainVolumeSlider.value = VolumeManager.Instance.CurrentMainVolume;
        musicSlider.value = VolumeManager.Instance.CurrentMusicVolume;
        sfxSlider.value = VolumeManager.Instance.CurrentSFXVolume;
        tanukiSlider.value = VolumeManager.Instance.CurrentTanukiVolume;
        SetVolumeMaster(VolumeManager.Instance.CurrentMainVolume);
        SetVolumeMusic(VolumeManager.Instance.CurrentMusicVolume);
        SetVolumeSFX(VolumeManager.Instance.CurrentSFXVolume);
        SetVolumeTanuki(VolumeManager.Instance.CurrentTanukiVolume);
    }

    public void SetVolumeMaster(float sliderValue) => VolumeManager.Instance.SetMainVolume(sliderValue);
    public void SetVolumeSFX(float sliderValue) => VolumeManager.Instance.SetSFXVolume(sliderValue);
    public void SetVolumeMusic(float sliderValue) => VolumeManager.Instance.SetMusicVolume(sliderValue);
    public void SetVolumeTanuki(float sliderValue) => VolumeManager.Instance.SetTanukiVolume(sliderValue);

}
