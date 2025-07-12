using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageManager : MonoSingleton<LanguageManager>
{
    [SerializeField] private int defaultLocaleIndex = 0; 

    public int CurrentLocaleIndex { get; private set; }

    public const string LOCALE_INDEX_ID = "LocaleIndex";

    protected override void Awake()
    {
        base.Awake();

        CurrentLocaleIndex = PlayerPrefs.GetInt(LOCALE_INDEX_ID, defaultLocaleIndex);
        //SetLanguage(CurrentLocaleIndex); 
    }

    private void Start()
    {
        ApplySettings();
    }

    public void SetLanguage(int index)
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        if (index >= 0 && index < LocalizationSettings.AvailableLocales.Locales.Count)
        {
            CurrentLocaleIndex = index;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(locales[index].Identifier);
            SaveSettings();
        }
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt(LOCALE_INDEX_ID, CurrentLocaleIndex);
        PlayerPrefs.Save();
    }

    private void ApplySettings()
    {
        SetLanguage(CurrentLocaleIndex);
    }
}
