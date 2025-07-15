using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScaleOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;

    [SerializeField, Range(0f, 2f)]
    private float endScale;

    [SerializeField, Range(0f, 5f)]
    private float duration = 0.3f;

    [SerializeField]
    private Ease easing;

    private Tween moveTween;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Scale()
    {
        if (moveTween != null)
            moveTween.Kill();

        moveTween = rectTransform.DOScale(endScale, duration).SetEase(easing).SetUpdate(true);

    }

    public void ResetPosition()
    {
        //Debug.Log("on mouse exit");
        if (moveTween != null)
            moveTween.Kill();

        moveTween = rectTransform.DOScale(1f, duration).SetEase(easing).SetUpdate(true);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        Scale();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetPosition();
    }
}
