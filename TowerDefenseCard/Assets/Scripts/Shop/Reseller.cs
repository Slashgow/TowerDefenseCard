using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

public class Reseller : MonoSingleton<Reseller>
{
    [SerializeField] private Logger logger;
    [SerializeField] private PoolingSystem currencyPool;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private SpriteShapeRenderer cardOutine;
    public SpriteShapeRenderer CardOutline => cardOutine;

    public PoolingSystem CurrencyPool => currencyPool;
    public static event Action<int> OnResell;
    public static event Action<CardID> OnResellCardID;
    public UnityEvent OnResellUnity;

    protected override void Awake()
    {
        base.Awake();
        cardOutine.enabled = false;
    }
    public void Resell(List<Card> cards)
    {
        int coinAmount = 0;

        if(cards.Exists(card => card is Currency || card is CardWorker))
            return;

        if(ShopManager.Instance.CurrentPlayerCoin < ShopManager.Instance.MinimumShopCost)
        {
            if (CardManager.Instance.TotalCostCardsOnBoard < ShopManager.Instance.MinimumShopCost)
                return;
        }

        foreach (Card card in cards)
        {
            coinAmount += card.CardData.Cost;
            CraftingManager.Instance.TryCancelCraft(card);
        }

        for (int i = 0; i < coinAmount; i++)
        {
            logger.Log($"spawn currency | {i} | coint amount {coinAmount} ", this);
            GameObject currencyGameObjectInstance = currencyPool.GetPrefabFromPool(spawnPoint.position);
            currencyGameObjectInstance.GetComponent<Currency>().Setup(currencyPool);
        }

        ShopManager.Instance.AddPlayerCoin(coinAmount);

        OnResellUnity?.Invoke();
        OnResell?.Invoke(cards.Count);

        cards.ForEach(card => OnResellCardID?.Invoke(card.CardData.CardID));
        CardUtility.DestroyAllCards(cards);
    }
}
