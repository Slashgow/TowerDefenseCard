using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
public class CardIdea : Card, IPointerDownHandler
{
    [SerializeField] private CardIdeaUI cardIdeaUI;

    private Card card;

    public void Initialize(Card card)
    {
        this.card = card;

#if UNITY_WEBGL

        string cardName = string.Empty;
        card.CardData.CardName.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                cardName = handle.Result;
            }
        };

        cardData.CardName.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                cardIdeaUI.SetupCard(handle.Result, cardData.Cost.ToString(), cardName);
            }
        };
#endif

#if !UNITY_WEBGL
        cardIdeaUI.SetupCard(cardData.CardName.GetLocalizedString(), cardData.Cost.ToString(), card.CardData.CardName.GetLocalizedString());
#endif
    }

    private void OnLocaleChange(UnityEngine.Localization.Locale Locale)
    {
#if UNITY_WEBGL

        string cardName = string.Empty;
        card.CardData.CardName.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                cardName = handle.Result;
            }
        };

        cardData.CardName.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                cardIdeaUI.SetupCard(handle.Result, cardData.Cost.ToString(), cardName);
            }
        };
#endif

#if !UNITY_WEBGL
        cardIdeaUI.SetupCard(cardData.CardName.GetLocalizedString(), cardData.Cost.ToString(), card.CardData.CardName.GetLocalizedString());
#endif
    }

    protected override void Start()
    {
        base.Start();
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChange;
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChange;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        CardManager.Instance.CheckCardDiscoveryState(card.CardData.CardID);
    }

    public CardIdeaSaveData Save() => new CardIdeaSaveData { card = this.card };
    public void Load(CardIdeaSaveData cardIdeaSaveData)
    {
        Initialize(cardIdeaSaveData.card);
    }
}
