using UnityEngine;
using UnityEngine.EventSystems;

public class CardShop : Card
{
    [SerializeField] private Shop shop;
    public Shop Shop => shop;   

    protected override void OnEnable()
    {
        cardSprite.sprite = cardData.CardSprite;
        backgroundSprite.sprite = cardData.CardBackgroundSprite;
        cardUI.SetupCard(cardData.CardName, shop.ShopCost.ToString());
    }

    public void TryPurchaseBooster() => ShopManager.Instance.TryPurchaseBooster(this.shop);
}
