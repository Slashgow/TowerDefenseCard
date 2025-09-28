using UnityEngine;
using UnityEngine.Localization.Settings;

public class CardShop : Card, ILoadable, ISavable
{
    [SerializeField] private bool spawnBoosterOnStart;

    [SerializeField] private Shop shop;
    public Shop Shop => shop;

    [Header("Lock Graphics References")]
    [SerializeField] private Sprite lockSprite;
    [SerializeField] private SpriteColorChanger colorChanger;
   

    protected override void Awake()
    {
        cardSprite.sprite = cardData.CardSprite;
        backgroundSprite.sprite = cardData.CardBackgroundSprite;

        shop.OnUnlock += UpdateCardShopData;
        shop.OnUnlock += Shop_OnUnlock;
    }

    protected override void OnDestroy()
    {
       base.OnDestroy();
        shop.OnUnlock -= UpdateCardShopData;
        shop.OnUnlock -= Shop_OnUnlock;
    }


    protected override void Start()
    {
        base.Start();

        InitializeLockGraphics();
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

    private void Shop_OnUnlock()
    {
        colorChanger.enabled = false;
        backgroundSprite.color = Color.white;
        backgroundSprite.sprite = cardData.CardBackgroundSprite;
    }

    public void InitializeLockGraphics()
    {
        if (shop.IsUnlocked)
        {
            Shop_OnUnlock();
            return;
        }

        colorChanger.enabled = true;
        backgroundSprite.sprite = lockSprite;
    }

    public void Load(GameSaveData gameSaveData)
    {
        switch (shop.ShopID)
        {
            case ShopID.BASE:
                this.shop.isFirstBoosterOpened = gameSaveData.isBasePackFirstTimeOpened;
                break;
            case ShopID.DEFENSE:
                this.shop.isFirstBoosterOpened = gameSaveData.isDefensePackFirstTimeOpened;
                break;
            case ShopID.ENGINEERING:
                this.shop.isFirstBoosterOpened = gameSaveData.isEngineeringPackFirstTimeOpened;
                break;
            case ShopID.FOOD:
                this.shop.isFirstBoosterOpened = gameSaveData.isFoodPackFirstTimeOpened;
                break;
            default:
                break;
        }
        
    }

    public void Save(GameSaveData gameSaveData)
    {
        switch (shop.ShopID)
        {
            case ShopID.BASE:
                gameSaveData.isBasePackFirstTimeOpened = this.shop.isFirstBoosterOpened;
                break;
            case ShopID.DEFENSE:
                gameSaveData.isDefensePackFirstTimeOpened = this.shop.isFirstBoosterOpened;
                break;
            case ShopID.ENGINEERING:
                gameSaveData.isEngineeringPackFirstTimeOpened = this.shop.isFirstBoosterOpened;
                break;
            case ShopID.FOOD:
                gameSaveData.isFoodPackFirstTimeOpened = this.shop.isFirstBoosterOpened;
                break;
            default:
                break;
        }
    }
}
