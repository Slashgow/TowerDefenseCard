using UnityEngine;
using UnityEngine.Localization.Settings;

public class CardShop : Card
{
    [SerializeField] private bool spawnBoosterOnStart;

    [SerializeField] private Shop shop;
    public Shop Shop => shop;   

    protected override void Awake()
    {
        cardSprite.sprite = cardData.CardSprite;
        backgroundSprite.sprite = cardData.CardBackgroundSprite;
        cardUI.SetupCard(cardData.CardName.GetLocalizedString(), shop.ShopCost.ToString());
    }

    protected override void Start()
    {
        base.Start();

        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChange;
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChange;

        if (spawnBoosterOnStart && !SavePath.SaveExists)
        {
            TryPurchaseBooster();
        }
    }

    public void TryPurchaseBooster()
    {
        ShopManager.Instance.TryPurchaseBooster(this.shop);
        shop.ResetShopCost();
        UpdateCardShopData();
    }

    private void OnLocaleChange(UnityEngine.Localization.Locale Locale)
    {
        UpdateCardShopData();
    }

    public void UpdateCardShopData()
    {
        cardUI.SetupCard(cardData.CardName.GetLocalizedString(), shop.CurrentShopCost.ToString());
    }
}
