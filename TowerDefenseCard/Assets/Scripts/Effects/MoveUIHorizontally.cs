using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class MoveUIHorizontally : Effect
{
    private RectTransform rectTransform;

    [SerializeField, Range(-200f, 30f)]
    private float xEndPivot;

    [SerializeField, Range(0f, 5f)]
    private float duration = 0.3f;

    [SerializeField]
    private Ease easing;

    private Tween moveTween;
    private Vector2 originPivot;

    public UnityEvent OnMoveEnd;
    public UnityEvent OnResetEnd;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originPivot = rectTransform.pivot;
    }

    public void MoveHorizontally()
    {
        //Debug.Log("on mouse over");

        if (moveTween != null)
            moveTween.Kill();

        moveTween = rectTransform.DOPivotX(xEndPivot, duration)
            .SetEase(easing)
            .SetUpdate(true)
            .OnComplete(() => OnMoveEnd?.Invoke());

    }

    public void ResetPosition()
    {
        //Debug.Log("on mouse exit");
        if (moveTween != null)
            moveTween.Kill();

        moveTween = rectTransform.DOPivotX(originPivot.x, duration)
            .SetEase(easing)
            .SetUpdate(true)
            .OnComplete(() => OnResetEnd?.Invoke()); ;
    }

    private void OnDisable()
    {
        rectTransform.pivot = originPivot;
    }

    public override void DoEffect() => MoveHorizontally();
}
