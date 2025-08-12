using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
public class ImageUIColorChanger : Effect, IColorable
{
    [Header("Color Theme")]
    [SerializeField] private bool useColorTheme;
    [SerializeField] private ColorID colorID;

    [Header("Settings")]
    [SerializeField] private Color endColor;
    [SerializeField, Range(0f, 5f)] private float timeToReachEndColor;
    [SerializeField] private bool shouldGoBackToOriginColor;
    [SerializeField] private Ease easing;

    private Image image;
    private Color originColor;
    private Tween colorTween;

    public bool UseColorTheme => useColorTheme;
    public ColorID ColorID => colorID;

    protected virtual void Awake()
    {
        image = GetComponent<Image>();
        originColor = image.color;
    }

    private void Start()
    {
        if (useColorTheme)
            OnChangeColorTheme(ColorThemeManager.Instance.CurrentColorTheme);
            
        ColorThemeManager.OnChangeColorTheme += OnChangeColorTheme;
    }

    public void OnChangeColorTheme(ColorTheme newColorTheme)
    {
        endColor = ColorThemeManager.Instance.GetColorByThemeAndID(newColorTheme, colorID);
        DoEffect();
    }

    public override void DoEffect() => ChangeColor();

    public void ChangeColor()
    {
        image.color = originColor;

        if (colorTween != null)
            colorTween.Kill();

        if (!shouldGoBackToOriginColor)
            colorTween = image.DOColor(endColor, timeToReachEndColor).SetEase(easing).SetUpdate(true);
        else
            colorTween = image.DOColor(endColor, timeToReachEndColor).SetEase(easing).SetUpdate(true).OnComplete(() => colorTween.Rewind());
    }

    private void OnDestroy()
    {
        if (colorTween != null)
            colorTween.Kill();

        ColorThemeManager.OnChangeColorTheme -= OnChangeColorTheme;
    }
}
