using DG.Tweening;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(TextMeshProUGUI))]
public class TextMeshProUGUIColorChanger : Effect, IColorable
{
    [Header("Color Theme")]
    [SerializeField] private bool useColorTheme;
    [SerializeField] private ColorID colorID;

    [Header("Settings")]
    [SerializeField] private Color endColor;
    [SerializeField, Range(0f, 5f)] private float timeToReachEndColor;
    [SerializeField] private bool shouldGoBackToOriginColor;
    [SerializeField] private Ease easing;

    private TextMeshProUGUI textMeshProUGUI;
    private Color originColor;
    private Tween colorTween;

    public bool UseColorTheme => useColorTheme;
    public ColorID ColorID => colorID;

    protected virtual void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        originColor = textMeshProUGUI.color;
    }

    private void Start()
    {
        if (useColorTheme)
            OnChangeColorTheme(ColorThemeManager.Instance.CurrentColorTheme);

        ColorThemeManager.OnChangeColorTheme += OnChangeColorTheme;
    }

    public override void DoEffect() => ChangeColor();

    public void ChangeColor()
    {
        textMeshProUGUI.color = originColor;

        if (colorTween != null)
            colorTween.Kill();

        if (!shouldGoBackToOriginColor)
            colorTween = textMeshProUGUI.DOColor(endColor, timeToReachEndColor).SetEase(easing);
        else
            colorTween = textMeshProUGUI.DOColor(endColor, timeToReachEndColor).SetEase(easing).OnComplete(() => colorTween.Rewind());
    }

    private void OnDestroy()
    {
        if (colorTween != null)
            colorTween.Kill();

        ColorThemeManager.OnChangeColorTheme -= OnChangeColorTheme;
    }

    public void OnChangeColorTheme(ColorTheme colorTheme)
    {
        endColor = ColorThemeManager.Instance.GetColorByThemeAndID(colorTheme, colorID);
        DoEffect();
    }
}
