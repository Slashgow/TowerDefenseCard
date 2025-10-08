using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class RectTransformScaleEffect : Effect
{
    [SerializeField] private bool changeStartScale = false;
    [SerializeField, Range(0f, 2f)] private float startScale = 1f;

    [SerializeField, Range(0f, 2f)] private float endScale;
    [SerializeField, Range(0f, 5f)] private float duration = 0.3f;
    [SerializeField] private Ease easing;
    [SerializeField, Range(-1,20)] private int loops = -1;
    [SerializeField] private LoopType loopType = LoopType.Yoyo;

    private float originalScale;
    private Tween moveTween;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale.x;
    }

    public override void DoEffect()
    {
        if (moveTween != null)
            moveTween.Kill();

        if (changeStartScale)
            rectTransform.localScale = startScale * Vector3.one;

        moveTween = rectTransform.DOScale(endScale, duration).SetEase(easing).SetLoops(loops, loopType).SetUpdate(true);
    }

    private void OnDisable()
    {
        this.rectTransform.localScale = originalScale * Vector3.one;

        if (moveTween != null)
            moveTween.Kill();
    }
}
