using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ColorThemeManager : MonoSingleton<ColorThemeManager>
{
    [SerializeField] private ColorTheme startColorTheme = ColorTheme.Rose;
    [SerializeField] private bool startWithRandomColorTheme = false;
    [SerializeField] private Logger logger;
    [SerializeField] private List<ColorsTheme> colorsThemes = new List<ColorsTheme>();

    public static event Action<ColorTheme> OnChangeColorTheme;

    public ColorTheme CurrentColorTheme;
    protected override void Awake()
    {
        base.Awake();

        CurrentColorTheme = startColorTheme;

        if (startWithRandomColorTheme)
        {
            int randomIndex = UnityEngine.Random.Range(0, Enum.GetValues(typeof(ColorTheme)).Length);
            startColorTheme = (ColorTheme)randomIndex;
        }
    }

    private void Start() => ChanceColorTheme(startColorTheme);

    public void ChanceColorTheme(ColorTheme targetColorTheme)
    {
        logger.Log($"Change color theme to : {targetColorTheme}", this);
        CurrentColorTheme = targetColorTheme;
        OnChangeColorTheme?.Invoke(targetColorTheme);
    }

    public void ChangeThemeToYellow() => ChanceColorTheme(ColorTheme.Yellow);
    public void ChangeThemeToRose() => ChanceColorTheme(ColorTheme.Rose);

    public ColorsTheme GetColorsByColorTheme(ColorTheme colorTheme) => colorsThemes.First(colorsTheme => colorsTheme.ColorTheme == colorTheme);

    public Color GetColorByThemeAndID(ColorTheme colorTheme, ColorID colorID)
    {
        ColorsTheme colorsTheme = GetColorsByColorTheme(colorTheme);
        return colorsTheme.GetColorByID(colorID);
    }
    

}
