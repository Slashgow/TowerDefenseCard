using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageUIColorChanger : Effect
{
    [SerializeField] private Color endColor;
    [SerializeField, Range(0f, 5f)] private float timeToReachEndColor;
    [SerializeField] private bool shouldGoBackToOriginColor;
    [SerializeField] private Ease easing;

    private Image image;
    private Color originColor;
    private Tween colorTween;

    protected virtual void Awake()
    {
        image = GetComponent<Image>();
        originColor = image.color;
    }
    public override void DoEffect() => ChangeColor();

    public void ChangeColor()
    {
        image.color = originColor;

        if (colorTween != null)
            colorTween.Kill();

        if (!shouldGoBackToOriginColor)
            colorTween = image.DOColor(endColor, timeToReachEndColor).SetEase(easing);
        else
            colorTween = image.DOColor(endColor, timeToReachEndColor).SetEase(easing).OnComplete(() => colorTween.Rewind());
    }

    private void OnDestroy()
    {
        if (colorTween != null)
            colorTween.Kill();
    }


}
