using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CardDiscoveryState
{
    [SerializeField] private Card card;
    public Card Card => card;

    public bool isDiscovered;
}

public class CardManager : MonoSingleton<CardManager>
{
    [SerializeField] private List<CardDiscoveryState> allCards = new List<CardDiscoveryState>();
    public List<CardDiscoveryState> AllCards => allCards;

    private List<CardID> discoveredCardIDs = new List<CardID>();

    public event Action OnDiscoverNewCard = delegate { };

    private void OnEnable()
    {
        CraftingManager.Instance.OnCraftComplete += CraftingManager_OnCraftComplete;
    }

    private void OnDisable()
    {
        CraftingManager.Instance.OnCraftComplete -= CraftingManager_OnCraftComplete;
    }

    private void CraftingManager_OnCraftComplete(int craftID, CardID outputCardID)
    {
        CardDiscoveryState matchingCardIDState = allCards.First(cardDiscoveryState => cardDiscoveryState.Card.CardData.CardID == outputCardID);

        if (!matchingCardIDState.isDiscovered)
        {
            matchingCardIDState.isDiscovered = true;
            discoveredCardIDs.Add(matchingCardIDState.Card.CardData.CardID);
            OnDiscoverNewCard?.Invoke();
        }
    }

    public List<Card> GetAllCardsDiscovered()
    {
        List<Card> discoveredCards = new List<Card>();

        foreach (CardDiscoveryState cardDiscoveryState in allCards)
        {
            if(cardDiscoveryState.isDiscovered)
                discoveredCards.Add(cardDiscoveryState.Card);
        }
        return discoveredCards;
    }

    public Card GetCardPrefabByCardID(CardID cardID)
    {
        return allCards.First(cardDiscoveryState => cardDiscoveryState.Card.CardData.CardID == cardID).Card;
    }
}
