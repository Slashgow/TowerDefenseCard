using DG.Tweening;
using UnityEngine;

public class OutlineWidthEffect : Effect
{
    [Header("Outline Animation")]
    [SerializeField] private UIOutlineUnscaled uiOutlineUnscaled;
    [SerializeField, Range(0f, 20f)] private float minWidthOutline = 8f;
    [SerializeField, Range(0f, 20f)] private float maxWidthOutline = 16f;
    [SerializeField, Range(0f, 5f)] private float cycleDuration = 3f;
    [SerializeField] private Ease ease;

    private Tween outlineTween;

    public override void DoEffect()
    {
        uiOutlineUnscaled.enabled = true;

        outlineTween = DOTween.To(() => uiOutlineUnscaled._outlineWidth, x => uiOutlineUnscaled._outlineWidth = x, maxWidthOutline, cycleDuration).
                SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
    }

    public void StopEffect()
    {
        uiOutlineUnscaled.enabled = false;
        outlineTween?.Kill();
    }
}
