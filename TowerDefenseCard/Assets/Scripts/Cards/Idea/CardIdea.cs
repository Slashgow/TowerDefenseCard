using UnityEngine.EventSystems;
using UnityEngine;
public class CardIdea : Card, IPointerDownHandler
{
    [SerializeField] private CardIdeaUI cardIdeaUI;

    private Card card;

    public void Initialize(Card card)
    {
        this.card = card;
        cardIdeaUI.SetupCard(cardData.CardName, cardData.Cost.ToString(), card.CardData.CardName);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CardManager.Instance.CheckCardDiscoveryState(card.CardData.CardID);
    }
}
