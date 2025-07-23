using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public struct ColorsTheme
{
    [SerializeField] private ColorTheme colorTheme;
    [SerializeField] private List<ColorWithID> colors;

    public ColorTheme ColorTheme => colorTheme;
    public List<ColorWithID> Colors => colors;

    public Color GetColorByID(ColorID colorID) => colors.First(colorWithID => colorWithID.ColorID == colorID).Color;
}
