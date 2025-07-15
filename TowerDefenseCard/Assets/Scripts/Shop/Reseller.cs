using System;
using System.Collections.Generic;
using UnityEngine;

public class Reseller : MonoBehaviour
{
    public event Action OnResell;

    public void Resell(List<Card> cards)
    {
        int coinAmount = 0;
        foreach (Card card in cards)
        {
            Debug.Log(card);
            coinAmount += card.CardData.Cost;
            CraftingManager.Instance.TryCancelCraft(card);
        }
        ShopManager.Instance.AddPlayerCoin(coinAmount);

        CardUtility.DestroyAllCards(cards);
        OnResell?.Invoke();
    }
}
