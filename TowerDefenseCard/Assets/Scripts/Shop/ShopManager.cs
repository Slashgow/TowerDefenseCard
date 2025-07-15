using UnityEngine;
using System.Collections.Generic;
using System;

public class ShopManager : MonoSingleton<ShopManager>
{
    [SerializeField] private List<CardShop> cardShops;
    [SerializeField, Range(0, 100)] private int startPlayerCoin = 10;

    public event Action<int> OnUpdatePlayerCoin = delegate { };

    [SerializeField, HideInInspector] private int currentPlayerCoin;
    public int CurrentPlayerCoin => currentPlayerCoin;

    protected override void Awake()
    {
        base.Awake();
        currentPlayerCoin = startPlayerCoin;
    }

    private void Start() => OnUpdatePlayerCoin?.Invoke(CurrentPlayerCoin);

    public void TryPurchaseBooster(Shop selectedShop)
    {
        if (CurrentPlayerCoin < selectedShop.ShopCost)
        {
            Debug.Log("Not enough YenCoins!");
            return;
        }

        currentPlayerCoin -= selectedShop.ShopCost;
        OnUpdatePlayerCoin?.Invoke(CurrentPlayerCoin);
        
        GameObject booster = Instantiate(selectedShop.Booster.gameObject, selectedShop.SpawnPoint.position, Quaternion.identity);
        Booster boosterComponent = booster.GetComponent<Booster>();

        if (boosterComponent != null)
            boosterComponent.Initialize(selectedShop.ShopItems); 
        else
            Debug.LogError("BoosterPrefab missing Booster component!");
        
        Debug.Log("Booster purchased successfully!");
    }

    public void AddPlayerCoin(int coinAmount)
    {
        currentPlayerCoin += coinAmount;
        OnUpdatePlayerCoin?.Invoke(CurrentPlayerCoin);
    }
}