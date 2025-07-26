using System;
using System.Collections.Generic;
using UnityEngine;

public class Reseller : MonoSingleton<Reseller>
{
    [SerializeField] private Logger logger;
    [SerializeField] private PoolingSystem currencyPool;
    [SerializeField] private Transform spawnPoint;

    public PoolingSystem CurrencyPool => currencyPool;
    public static event Action<int> OnResell;
    public void Resell(List<Card> cards)
    {
        int coinAmount = 0;

        if(cards.Exists(card => card is Currency || card is CardWorker))
            return;

        foreach (Card card in cards)
        {
            coinAmount += card.CardData.Cost;
            CraftingManager.Instance.TryCancelCraft(card);

            if(card is CardStorage)
            {
                CardStorage cardStorage = (CardStorage)card;
                CardManager.Instance.DecreaseMaxCardsAllowed(cardStorage.NumberOfAdditionalCardsAllowed);
            }
        }

        for (int i = 0; i < coinAmount; i++)
        {
            logger.Log($"spawn currency | {i} | coint amount {coinAmount} ", this);
            GameObject currencyGameObjectInstance = currencyPool.GetPrefabFromPool(spawnPoint.position);
            currencyGameObjectInstance.GetComponent<Currency>().Setup(currencyPool);
        }

        ShopManager.Instance.AddPlayerCoin(coinAmount);

        OnResell?.Invoke(cards.Count);
        CardUtility.DestroyAllCards(cards);
    }
}
