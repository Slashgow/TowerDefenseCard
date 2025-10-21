using UnityEngine;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public class ShopManager : MonoSingleton<ShopManager>, ILoadable, ISavable
{
    [SerializeField] private Logger logger;
    [SerializeField, Range(0, 100)] private int startPlayerCoin = 10;
    public int StartPlayerCoin => startPlayerCoin;

    public event Action<int> OnUpdatePlayerCoin = delegate { };

    [SerializeField, HideInInspector] private int currentPlayerCoin;
    public int CurrentPlayerCoin => currentPlayerCoin;

    [SerializeField] private CardShop cheapestShop;
    public int MinimumShopCost => cheapestShop.Shop.ShopCost;

    [SerializeField] private List<CardShop> cardShops = new List<CardShop>();

    [SerializeField] private PoolingSystem currencyPool;
    [SerializeField] private Transform spawnPoint;

    public static event Action OnPurchaseBooster;
    public static event Action OnPurchasePartiallyBoosterEvent;

    public static void OnPurchasePartiallyBooster() => OnPurchasePartiallyBoosterEvent?.Invoke();
    public UnityEvent OnPurchaseBoosterUnity;

    public int AdditionalInkFromEarlyStart => (int)Mathf.Max(Mathf.Ceil(CraftingManager.Instance.RemainingTime), 0f);

    private void Start()
    {
        OnUpdatePlayerCoin?.Invoke(CurrentPlayerCoin);
    }

    public bool TryPurchaseBooster(Shop selectedShop)
    {
        if (CurrentPlayerCoin < selectedShop.CurrentShopCost)
        {
            logger.Log("Not enough YenCoins!",this);
            return false;
        }

        currentPlayerCoin -= selectedShop.CurrentShopCost;
        OnUpdatePlayerCoin?.Invoke(CurrentPlayerCoin);
        OnPurchaseBoosterUnity?.Invoke();
        OnPurchaseBooster?.Invoke();
        GameObject booster = Instantiate(selectedShop.Booster.gameObject, selectedShop.SpawnPoint.position, Quaternion.identity);
        Booster boosterComponent = booster.GetComponent<Booster>();

        if (boosterComponent != null)
            boosterComponent.Initialize(selectedShop); 
        else
            logger.LogError("BoosterPrefab missing Booster component!", this);

        logger.Log("Booster purchased successfully!", this);
        return true;
    }

    public void AddPlayerCoin(int coinAmount)
    {
        currentPlayerCoin += coinAmount;
        OnUpdatePlayerCoin?.Invoke(CurrentPlayerCoin);
    }

    public void RemovePlayerCoin(int coinAmount)
    {
        if (coinAmount > currentPlayerCoin)
        {
            logger.LogError("Not enough coins to remove!", this);
            return;
        }
        currentPlayerCoin -= coinAmount;
        OnUpdatePlayerCoin?.Invoke(CurrentPlayerCoin);
    }

    public void SpawnAdditionalInkEarlyStart()
    {
        for (int i = 0; i < AdditionalInkFromEarlyStart; i++)
        {
            logger.Log($"spawn currency | {i} | coint amount {AdditionalInkFromEarlyStart} ", this);
            GameObject currencyGameObjectInstance = currencyPool.GetPrefabFromPool(spawnPoint.position);
            currencyGameObjectInstance.GetComponent<Currency>().Setup(currencyPool);
        }
        AddPlayerCoin(AdditionalInkFromEarlyStart);
    }

    public Shop GetShopByID(ShopID shopID) => cardShops.FirstOrDefault(cardShop => cardShop.Shop.ShopID == shopID).Shop;
    public void Load(GameSaveData gameSaveData) => AddPlayerCoin(gameSaveData.currentPlayerCoin);
    public void Save(GameSaveData gameSaveData) => gameSaveData.currentPlayerCoin = currentPlayerCoin;
}