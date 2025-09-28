using DG.Tweening;
using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
public class SpriteColorChanger : Effect, IColorable
{
    [Header("Color Theme")]
    [SerializeField] private bool useColorTheme;
    [SerializeField] private ColorID colorID;

    [Header("Settings")]
    [SerializeField] private Color endColor;
    [SerializeField, Range(0f, 5f)] private float timeToReachEndColor;
    [SerializeField] private bool shouldGoBackToOriginColor;
    [SerializeField] private Ease easing;

    private SpriteRenderer spriteRenderer;
    private Color originColor;
    private Color disableColor;
    private Tween colorTween;

    public bool UseColorTheme => useColorTheme;
    public ColorID ColorID => colorID;

    private bool disableInAdvance;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originColor = spriteRenderer.color;
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
        if(disableInAdvance)
            return;

        spriteRenderer.color = originColor;

        if(colorTween != null)
            colorTween.Kill();

        if(!shouldGoBackToOriginColor)
            colorTween = spriteRenderer.DOColor(endColor, timeToReachEndColor).SetEase(easing);
        else
            colorTween = spriteRenderer.DOColor(endColor, timeToReachEndColor).SetEase(easing).SetLoops(2, LoopType.Yoyo);

        originColor = endColor;
    }


    public void KillTween(bool disableInAdvance)
    {
        if (colorTween != null)
            colorTween.Kill();

       this.disableInAdvance = disableInAdvance;
    }

    private void OnDestroy()
    {
        if (colorTween != null)
            colorTween.Kill();

        ColorThemeManager.OnChangeColorTheme -= OnChangeColorTheme;
    }

}
