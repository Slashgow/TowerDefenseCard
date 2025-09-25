using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ImageUIAlphaColorChanger : Effect
{
    [SerializeField, Range(0f, 1f)] private float targetAlpha = 1f;
    [SerializeField, Range(0f, 10f)] private float duration = 1f;
    [SerializeField] private Ease easing = Ease.Linear;

    [Header("Flash Ease Case")]
    [SerializeField, Range(0, 40)] private int overshoot;
    [SerializeField, Range(-1f, 1f)] private float period;

    private Tween colorTween;
    private Image image;

    protected void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnDestroy()
    {
        if (colorTween != null)
            colorTween.Kill();
    }
    public override void DoEffect()
    {
        colorTween?.Kill();

        colorTween = image.DOFade(targetAlpha, duration)
             .SetEase(easing, overshoot, period)
             .SetUpdate(true);
    }
}
