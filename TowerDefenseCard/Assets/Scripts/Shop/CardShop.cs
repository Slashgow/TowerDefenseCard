using UnityEngine;

public class CardShop : Card
{
    [SerializeField] private bool spawnBoosterOnStart;

    [SerializeField] private Shop shop;
    public Shop Shop => shop;   

    protected override void OnEnable()
    {
        cardSprite.sprite = cardData.CardSprite;
        backgroundSprite.sprite = cardData.CardBackgroundSprite;
        cardUI.SetupCard(cardData.CardName, shop.ShopCost.ToString());
    }

    protected override void Start()
    {
        base.Start();
        if (spawnBoosterOnStart)
        {
            TryPurchaseBooster();
        }
    }

    public void TryPurchaseBooster() => ShopManager.Instance.TryPurchaseBooster(this.shop);
}
