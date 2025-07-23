using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ColorThemeManager : PersistentMonoSingleton<ColorThemeManager>
{
    [SerializeField] private ColorTheme startupColorTheme;
    [SerializeField] private List<ColorsTheme> colorsThemes = new List<ColorsTheme>();

    public static event Action<ColorTheme> OnChangeColorTheme;

    public ColorTheme CurrentColorTheme;

    public void ChanceColorTheme(ColorTheme targetColorTheme)
    {
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
