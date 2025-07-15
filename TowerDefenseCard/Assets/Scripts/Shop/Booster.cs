using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;
using System;

public class Booster : Card, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, Range(1, 10)] private int maxCardCount = 3; 
    public int MaxCardCount => maxCardCount;

    private int remainingCards;
    private List<ShopItem> cardPool;

    public static event Action OnOpenBooster;

    public void Initialize(List<ShopItem> pool)
    {
        cardPool = new List<ShopItem>(pool);
        remainingCards = maxCardCount;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        TrySpawnCardFromShopItemList();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //TrySpawnCardFromShopItemList();
    }

    private void TrySpawnCardFromShopItemList()
    {
        if (remainingCards > 0)
        {
            // Weighted random selection from the pool
            float totalWeight = cardPool.Sum(item => item.DropPercentage);

            if (totalWeight <= 0)
                totalWeight = 1f;

            float roll = UnityEngine.Random.value;
            ShopItem selectedItem = null;
            float cumulativeWeight = 0f;

            foreach (var item in cardPool)
            {
                cumulativeWeight += item.DropPercentage / totalWeight;
                if (roll <= cumulativeWeight)
                {
                    selectedItem = item;
                    break;
                }
            }

            if (selectedItem == null && cardPool.Count > 0)
                selectedItem = cardPool[0];

            if (selectedItem != null)
            {
                Instantiate(selectedItem.CardPrefab, this.transform.position, Quaternion.identity);
                remainingCards--;
                Debug.Log($"{selectedItem.CardPrefab.GetComponent<Card>().CardData.CardName} spawned from booster! {remainingCards} cards left.");
                OnOpenBooster?.Invoke();
            }

            if (remainingCards <= 0)
            {
                Destroy(gameObject);
                Debug.Log("Booster exhausted and destroyed!");
            }
        }
    }
}