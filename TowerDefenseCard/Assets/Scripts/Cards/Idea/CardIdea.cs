using UnityEngine.EventSystems;

public class CardIdea : Card, IPointerUpHandler
{
    private Card card;

    public void Initialize(Card card)
    {
        this.card = card;   
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CardManager.Instance.CheckCardDiscoveryState(card.CardData.CardID);
        Destroy(this.gameObject);
    }
}
