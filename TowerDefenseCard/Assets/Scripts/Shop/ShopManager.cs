using UnityEngine;
using System.Collections.Generic;
using System;

public class ShopManager : MonoSingleton<ShopManager>, ILoadable, ISavable
{
    [SerializeField] private Logger logger;
    [SerializeField] private List<CardShop> cardShops;
    [SerializeField, Range(0, 100)] private int startPlayerCoin = 10;
    public int StartPlayerCoin => startPlayerCoin;

    public event Action<int> OnUpdatePlayerCoin = delegate { };

    [SerializeField, HideInInspector] private int currentPlayerCoin;
    public int CurrentPlayerCoin => currentPlayerCoin;

    public static event Action OnPurchaseBooster;

    private void Start()
    {
        OnUpdatePlayerCoin?.Invoke(CurrentPlayerCoin);
    }

    public void TryPurchaseBooster(Shop selectedShop)
    {
        if (CurrentPlayerCoin < selectedShop.CurrentShopCost)
        {
            logger.Log("Not enough YenCoins!",this);
            return;
        }

        currentPlayerCoin -= selectedShop.CurrentShopCost;
        OnUpdatePlayerCoin?.Invoke(CurrentPlayerCoin);
        OnPurchaseBooster?.Invoke();
        GameObject booster = Instantiate(selectedShop.Booster.gameObject, selectedShop.SpawnPoint.position, Quaternion.identity);
        Booster boosterComponent = booster.GetComponent<Booster>();

        if (boosterComponent != null)
            boosterComponent.Initialize(selectedShop); 
        else
            logger.LogError("BoosterPrefab missing Booster component!", this);

        logger.Log("Booster purchased successfully!", this);
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

    public void Load(GameSaveData gameSaveData) => AddPlayerCoin(gameSaveData.currentPlayerCoin);
    public void Save(GameSaveData gameSaveData) => gameSaveData.currentPlayerCoin = currentPlayerCoin;
}