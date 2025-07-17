using DG.Tweening;
using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
public class SpriteColorChanger : Effect
{
    [SerializeField] private Color endColor;
    [SerializeField, Range(0f, 5f)] private float timeToReachEndColor;
    [SerializeField] private bool shouldGoBackToOriginColor;
    [SerializeField] private Ease easing;

    private SpriteRenderer spriteRenderer;
    private Color originColor;
    private Tween colorTween;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originColor = spriteRenderer.color;
    }
    public override void DoEffect() => ChangeColor();

    public void ChangeColor()
    {
        spriteRenderer.color = originColor;

        if(colorTween != null)
            colorTween.Kill();

        if(!shouldGoBackToOriginColor)
            colorTween = spriteRenderer.DOColor(endColor, timeToReachEndColor).SetEase(easing);
        else
            colorTween = spriteRenderer.DOColor(endColor, timeToReachEndColor).SetEase(easing).OnComplete(() => colorTween.Rewind());
    }

    private void OnDestroy()
    {
        if (colorTween != null)
            colorTween.Kill();
    }

  
}
