using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class SelectableUIColorChanger : MonoBehaviour, IColorable
{
    [SerializeField] private bool useColorTheme;
    [SerializeField] private ColorID normalColorID, highlitedColorID, pressedColorID;

    public bool UseColorTheme => useColorTheme;
    public ColorID ColorID => throw new System.NotImplementedException();

    private Selectable selectable;

    private void Awake() => selectable = GetComponent<Selectable>();

    private void Start()
    {
        if (!useColorTheme)
            return;
        
        OnChangeColorTheme(ColorThemeManager.Instance.CurrentColorTheme);
        ColorThemeManager.OnChangeColorTheme += OnChangeColorTheme;
    }

    private void OnDestroy()
    {
        if (!useColorTheme)
            return;

        ColorThemeManager.OnChangeColorTheme -= OnChangeColorTheme;
    }

    public void OnChangeColorTheme(ColorTheme colorTheme)
    {
        ColorBlock colorBlock = selectable.colors;
        colorBlock.normalColor = ColorThemeManager.Instance.GetColorByThemeAndID(colorTheme, normalColorID);
        colorBlock.highlightedColor = ColorThemeManager.Instance.GetColorByThemeAndID(colorTheme, highlitedColorID);
        colorBlock.pressedColor = ColorThemeManager.Instance.GetColorByThemeAndID(colorTheme, pressedColorID);
        selectable.colors = colorBlock;
    }

}
