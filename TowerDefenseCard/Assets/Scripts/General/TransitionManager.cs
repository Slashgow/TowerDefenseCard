using DG.Tweening;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    [SerializeField, Range(0, 10f)] private float transitionTime;
    [SerializeField] private Ease transitionEase = Ease.InOutQuad;

    [Header("References")]
    [SerializeField] private RectTransform rightTransform;
    [SerializeField] private RectTransform leftTransform;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Start Scene Animation")]
    [SerializeField, Range(-3000f,3000f)] private float startRightAnchoredPositionStart;
    [SerializeField, Range(-3000f, 3000f)] private float startLeftAnchoredPositionStart;
    [SerializeField, Range(-3000f, 3000f)] private float endRightAnchoredPositionStart;
    [SerializeField, Range(-3000f, 3000f)] private float endLeftAnchoredPositionStart;

    [Header("End Scene Animation")]
    [SerializeField, Range(-3000f, 3000f)] private float startRightAnchoredPositionEnd;
    [SerializeField, Range(-3000f, 3000f)] private float startLeftAnchoredPositionEnd;
    [SerializeField, Range(-3000f, 3000f)] private float endRightAnchoredPositionEnd;
    [SerializeField, Range(-3000f, 3000f)] private float endLeftAnchoredPositionEnd;
    public float TransitionTime => transitionTime;

    private void Start()
    {
        if (SceneLoader.HasInstance)
            SceneLoader.Instance.RegisterTransitionAnimator(this);

        StartSceneAnimation();
    }

    public void StartSceneAnimation()
    {
        rightTransform.anchoredPosition = new Vector2(startRightAnchoredPositionStart, 0f);
        leftTransform.anchoredPosition = new Vector2(startLeftAnchoredPositionStart, 0f);

        rightTransform.DOAnchorPos(new Vector2(endRightAnchoredPositionStart, 0f), transitionTime)
            .SetEase(transitionEase)
            .SetUpdate(true);

        leftTransform.DOAnchorPos(new Vector2(endLeftAnchoredPositionStart, 0f), transitionTime)
            .SetEase(transitionEase)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            });
    }

    public void EndSceneAnimation()
    {

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        rightTransform.anchoredPosition = new Vector2(startRightAnchoredPositionEnd, 0f);
        leftTransform.anchoredPosition = new Vector2(startLeftAnchoredPositionEnd, 0f);

        rightTransform.DOAnchorPos(new Vector2(endRightAnchoredPositionEnd, 0f), transitionTime)
            .SetEase(transitionEase)
            .SetUpdate(true);
        leftTransform.DOAnchorPos(new Vector2(endLeftAnchoredPositionEnd, 0f), transitionTime)
            .SetEase(transitionEase)
            .SetUpdate(true);
    }

    private void OnDestroy()
    {
        if (SceneLoader.HasInstance)
            SceneLoader.Instance.UnregisterTransitionAnimator();
    }
}
