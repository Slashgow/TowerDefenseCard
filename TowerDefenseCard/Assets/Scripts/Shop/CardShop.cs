using UnityEngine;
using UnityEngine.EventSystems;

public class CardShop : Card, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private Shop shop;

    protected override void OnEnable()
    {
        cardSprite.sprite = cardData.CardSprite;
        backgroundSprite.sprite = cardData.CardBackgroundSprite;
        cardUI.SetupCard(cardData.CardName, shop.ShopCost.ToString());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("On Pointer down shop");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("On Pointer up shop");
        ShopManager.Instance.TryPurchaseBooster(this.shop);
    }
}
