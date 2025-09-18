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

        shop.OnUnlock += UpdateCardShopData;
    }

    protected override void Start()
    {
        base.Start();

        UpdateUI();

        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChange;
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChange;

        if (spawnBoosterOnStart && !SavePath.SaveExists)
        {
            TryPurchaseBooster(true);
        }
    }

    public void TryPurchaseBooster(bool bypassLocked)
    {
        if (!bypassLocked && !shop.IsUnlocked)
            return;

        ShopManager.Instance.TryPurchaseBooster(this.shop);
        shop.ResetShopCost();
        UpdateUI();
    }

    private void OnLocaleChange(UnityEngine.Localization.Locale Locale)
    {
        UpdateCardShopData();
    }

    public void UpdateCardShopData()
    {
        cardUI.SetupCard(cardData.CardName.GetLocalizedString(), shop.CurrentShopCost.ToString());
    }
    private void UpdateCardShopDataLock()
    {
        cardUI.SetupCard(shop.LockUIDescription, shop.ShopCost.ToString());
    }
    private void UpdateUI()
    {
        if (shop.IsUnlocked)
            UpdateCardShopData();
        else
            UpdateCardShopDataLock();
    }
}
