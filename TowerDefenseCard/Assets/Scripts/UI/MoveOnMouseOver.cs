using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveOnMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;

    [SerializeField, Range(-200f,30f)]
    private float xEndPivot;

    [SerializeField, Range(0f, 5f)]
    private float duration = 0.3f;

    [SerializeField]
    private Ease easing;

    private Tween moveTween;
    private Vector2 originPivot;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originPivot = rectTransform.pivot;
    }

    public void MoveHorizontally()
    {
        //Debug.Log("on mouse over");

        if(moveTween != null)
            moveTween.Kill();

        moveTween = rectTransform.DOPivotX(xEndPivot, duration).SetEase(easing).SetUpdate(true);

    }

    public void ResetPosition()
    {
        //Debug.Log("on mouse exit");
        if (moveTween != null)
            moveTween.Kill();

        moveTween = rectTransform.DOPivotX(originPivot.x, duration).SetEase(easing).SetUpdate(true);
    }

    private void OnDisable()
    {
        rectTransform.pivot = originPivot;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MoveHorizontally();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetPosition();
    }
}
