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

    [SerializeField, Range(0, 100)] private int startMaxCardsAllowed = 30;
    public int MaxCardsAllowed { get; private set; }
    public int CurrentNumberOfCards { get; private set; }
    public bool IsMaxCardsReached => CurrentNumberOfCards >= MaxCardsAllowed;

    private List<CardID> discoveredCardIDs = new List<CardID>();

    public event Action OnDiscoverNewCard = delegate { };
    public event Action<int, int> OnUpdateNumberOfCards;
    public event Action<int, int> OnUpdateMaxNumberOfCards;

    protected override void Awake()
    {
        base.Awake();
        MaxCardsAllowed = startMaxCardsAllowed;
        CurrentNumberOfCards = FindObjectsByType<Card>(FindObjectsSortMode.None).Length;
    }

    private void Start()
    {
        CraftingManager.Instance.OnCraftComplete += CraftingManager_OnCraftComplete;
        Reseller.OnResell += Reseller_OnResell;
        Booster.OnOpenBooster += Booster_OnOpenBooster;
    }

    private void OnDisable()
    {
        if(CraftingManager.HasInstance)
            CraftingManager.Instance.OnCraftComplete -= CraftingManager_OnCraftComplete;

        Booster.OnOpenBooster -= Booster_OnOpenBooster;
        Reseller.OnResell -= Reseller_OnResell;
    }
    private void Reseller_OnResell(int numberOfReselledCard)
    {
        CurrentNumberOfCards -= numberOfReselledCard;
        OnUpdateNumberOfCards?.Invoke(CurrentNumberOfCards, MaxCardsAllowed);
    }

    private void Booster_OnOpenBooster(CardID cardID)
    {
        CheckCardDiscoveryState(cardID);
        CurrentNumberOfCards++;
        OnUpdateNumberOfCards?.Invoke(CurrentNumberOfCards, MaxCardsAllowed);
    }

    private void CraftingManager_OnCraftComplete(int craftID, CardID outputCardID)
    {
        Debug.Log("card manager - on craft complete");
        CheckCardDiscoveryState(outputCardID);
        CurrentNumberOfCards++;
        OnUpdateNumberOfCards?.Invoke(CurrentNumberOfCards, MaxCardsAllowed);
    }

    public void CheckCardDiscoveryState(CardID outputCardID)
    {
        CardDiscoveryState matchingCardIDState = allCards.First(cardDiscoveryState => cardDiscoveryState.Card.CardData.CardID == outputCardID);

        if (matchingCardIDState == null)
            return;

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

    public void IncreaseMaxCardsAllowed(int additionalCards)
    {
        MaxCardsAllowed += additionalCards;
        OnUpdateMaxNumberOfCards?.Invoke(CurrentNumberOfCards, MaxCardsAllowed);
    }
    public void DecreaseMaxCardsAllowed(int additionalCards)
    {
        MaxCardsAllowed -= additionalCards;
        OnUpdateMaxNumberOfCards?.Invoke(CurrentNumberOfCards, MaxCardsAllowed);
    }


}
