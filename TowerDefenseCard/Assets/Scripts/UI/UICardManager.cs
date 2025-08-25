using DG.Tweening;
using TMPro;
using UnityEngine;

public class UICardManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numberOfCardText;

    [Header("Outline Animation")]
    [SerializeField] private UIOutlineUnscaled uiOutlineUnscaled;
    [SerializeField, Range(0f, 20f)] private float minWidthOutline = 8f;
    [SerializeField, Range(0f, 20f)] private float maxWidthOutline = 16f;
    [SerializeField, Range(0f, 5f)] private float cycleDuration = 3f;
    [SerializeField] private Ease ease;

    private Tween outlineTween;

    private void Start()
    {
        CardManager.Instance.OnUpdateNumberOfCards += CardManager_OnUpdateNumberOfCards;
        CardManager.Instance.OnUpdateMaxNumberOfCards += CardManager_OnUpdateNumberOfCards;

        CardManager_OnUpdateNumberOfCards(CardManager.Instance.CurrentNumberOfCards, CardManager.Instance.MaxCardsAllowed);
    }

    private void OnDisable()
    {
        if (CardManager.HasInstance)
        {
            CardManager.Instance.OnUpdateNumberOfCards -= CardManager_OnUpdateNumberOfCards;
            CardManager.Instance.OnUpdateMaxNumberOfCards -= CardManager_OnUpdateNumberOfCards;
        }
    }

    private void CardManager_OnUpdateNumberOfCards(int numberOfCards, int maxNumberOfCards)
    {
        UpdateText(numberOfCards, maxNumberOfCards);

        if (CardManager.Instance.IsMaxCardsReached)
        {
            outlineTween = DOTween.To(() => uiOutlineUnscaled._outlineWidth, x => uiOutlineUnscaled._outlineWidth = x, maxWidthOutline, cycleDuration).
                SetLoops(-1, LoopType.Yoyo);
            uiOutlineUnscaled.enabled = true;
        }

        else
        {
            outlineTween?.Kill();
            uiOutlineUnscaled.enabled = false;
        }
    }

    private void UpdateText(int numberOfCards, int maxNumberOfCards)
    {
        numberOfCardText.text = $"{numberOfCards}/{maxNumberOfCards}";
    }
}
