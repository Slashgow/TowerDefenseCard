using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;


public class ShopManager : MonoSingleton<ShopManager>
{
    [SerializeField, Range(0, 50)] private int shopCost;
    [SerializeField] private List<ShopItem> shopItems;
    [SerializeField, Range(0,100)] private int startPlayerCoin = 10; 
    [SerializeField] private Transform spawnPoint; 
  

    public event Action<int> OnUpdatePlayerCoin = delegate { };
    public int CurrentPlayerCoin { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        CurrentPlayerCoin = startPlayerCoin;
    }

    private void Start() => OnUpdatePlayerCoin?.Invoke(CurrentPlayerCoin);

    public void TryPurchaseWeightedCard()
    {
        if (CurrentPlayerCoin < shopCost)
        {
            Debug.Log("Not enough YenCoins!");
            return;
        }

        if (shopItems == null || shopItems.Count == 0) 
            return;

        float totalWeight = shopItems.Sum(item => item.DropPercentage);
        if (totalWeight <= 0) 
            totalWeight = 1f; 
        var weightedItems = shopItems.Select(item => new { Item = item, Weight = item.DropPercentage / totalWeight }).ToList();

        float roll = UnityEngine.Random.value; // 0 to 1
        ShopItem selectedItem = null;
        float cumulativeWeight = 0f;

        foreach (var weightedItem in weightedItems)
        {
            cumulativeWeight += weightedItem.Weight;
            if (roll <= cumulativeWeight)
            {
                selectedItem = weightedItem.Item;
                break;
            }
        }

        if (selectedItem == null) 
            selectedItem = weightedItems[0].Item;


        CurrentPlayerCoin -= shopCost;
        OnUpdatePlayerCoin?.Invoke(CurrentPlayerCoin);

        Instantiate(selectedItem.CardPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log($"{selectedItem.CardPrefab.GetComponent<Card>().CardData.CardName} purchased successfully!");

    }

    public void AddPlayerCoin(int cointAmount)
    {
        CurrentPlayerCoin += cointAmount;
        OnUpdatePlayerCoin?.Invoke(CurrentPlayerCoin);
    }
}