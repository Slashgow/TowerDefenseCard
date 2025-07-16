using System;
using System.Collections.Generic;
using UnityEngine;

public class Reseller : MonoBehaviour
{
    public static event Action<int> OnResell;

    public void Resell(List<Card> cards)
    {
        int coinAmount = 0;
        foreach (Card card in cards)
        {
            Debug.Log(card);
            coinAmount += card.CardData.Cost;
            CraftingManager.Instance.TryCancelCraft(card);

            if(card is CardStorage)
            {
                CardStorage cardStorage = (CardStorage)card;
                CardManager.Instance.DecreaseMaxCardsAllowed(cardStorage.NumberOfAdditionalCardsAllowed);
            }
        }
        ShopManager.Instance.AddPlayerCoin(coinAmount);

        OnResell?.Invoke(cards.Count);
        CardUtility.DestroyAllCards(cards);
    }
}
