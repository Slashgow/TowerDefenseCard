using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDisplay : MonoSingleton<UIDisplay>
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle vsyncToggle;
    [SerializeField] private TMP_Dropdown framerateDropdown;
    [SerializeField] private TMP_Dropdown windowModeDropdown;

    private void Start()
    {
        PopulateDropdowns();
        LoadSettings();
    }

    private void PopulateDropdowns()
    {
        // Populate Resolution Dropdown
        resolutionDropdown.ClearOptions();
        var resolutions = Screen.resolutions.Select(r => $"{r.width}x{r.height} @{r.refreshRateRatio}Hz").ToList();
        resolutionDropdown.AddOptions(resolutions);

        // Populate Quality Dropdown
        qualityDropdown.ClearOptions();
        string[] qualityLevels = QualitySettings.names;
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(qualityLevels));

        // Populate Framerate Dropdown
        framerateDropdown.ClearOptions();
        int[] framerateOptions = DisplayManager.Instance.framerateOptions; // Common framerate options
        framerateDropdown.AddOptions(framerateOptions.Select(f => f.ToString() + " FPS").ToList());

        // Populate Window Mode Dropdown
        windowModeDropdown.ClearOptions();
        windowModeDropdown.AddOptions(new System.Collections.Generic.List<string> { "Windowed", "Fullscreen" });
    }

    private void LoadSettings()
    {
        var currentResolution = Screen.resolutions.FirstOrDefault(r => r.width == DisplayManager.Instance.CurrentResolution.x && r.height == DisplayManager.Instance.CurrentResolution.y);
        resolutionDropdown.value = Screen.resolutions.ToList().IndexOf(currentResolution);
        qualityDropdown.value = DisplayManager.Instance.CurrentQuality;
        vsyncToggle.isOn = DisplayManager.Instance.CurrentVSync;
        framerateDropdown.value = GetIndexByFramerate(DisplayManager.Instance.CurrentFramerate);
        windowModeDropdown.value = DisplayManager.Instance.CurrentWindowMode == FullScreenMode.FullScreenWindow ? 1 : 0;
    }

    private int GetIndexByFramerate(int framerate)
    {
        for (int i = 0; i < DisplayManager.Instance.framerateOptions.Length; i++)
        {
            if (DisplayManager.Instance.framerateOptions[i] == framerate) return i;
        }
        return 1; // Default to 60 FPS if not found
    }

    private int GetFramerateByIndex(int index)
    {
        for (int i = 0; i < DisplayManager.Instance.framerateOptions.Length; i++)
        {
            if (i == index)
                return DisplayManager.Instance.framerateOptions[i];
        }
        return 60;
    }

    public void SetResolution(int index) => DisplayManager.Instance.SetResolution(Screen.resolutions[index]);
    public void SetQuality(int index) => DisplayManager.Instance.SetQuality(index);
    public void SetVSync(bool value) => DisplayManager.Instance.SetVSync(value);
    public void SetFramerate(int index) => DisplayManager.Instance.SetFramerate(GetFramerateByIndex(index));
    public void SetWindowMode(int index) => DisplayManager.Instance.SetWindowMode(index == 1 ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
}
