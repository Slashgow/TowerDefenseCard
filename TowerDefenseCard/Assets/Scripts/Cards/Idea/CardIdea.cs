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
        cardIdeaUI.SetupCard(cardData.CardName.GetLocalizedString(), cardData.Cost.ToString(), card.CardData.CardName.GetLocalizedString());
    }

    private void OnLocaleChange(UnityEngine.Localization.Locale Locale)
    {
        cardIdeaUI.SetupCard(cardData.CardName.GetLocalizedString(), cardData.Cost.ToString(), card.CardData.CardName.GetLocalizedString());
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
