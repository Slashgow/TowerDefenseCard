using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class CardDiscoveryState
{
    [SerializeField] private Card card;
    public Card Card => card;

    public bool isDiscovered;
    public bool isClickedAfterNotification;
}

[Serializable]
public class CardManagerSaveData
{
    [SerializeField]
    public List<CardDiscoveryState> allCards;

    public CardManagerSaveData(List<CardDiscoveryState> allCards)
    {
        //allCards = new List<CardDiscoveryState>();
        this.allCards = allCards;
    }
}

public class CardManager : MonoSingleton<CardManager>, ISavable, ILoadable
{
   public List<CardDiscoveryState> allCards = new List<CardDiscoveryState>();
    public List<CardDiscoveryState> AllCards => allCards;

    [SerializeField, Range(0, 100)] private int startMaxCardsAllowed = 30;
    public int StartMaxCardsAllowed => startMaxCardsAllowed;
    public int MaxCardsAllowed { get; private set; }
    public int CurrentNumberOfCards { get; private set; }
    public bool IsMaxCardsReached => CurrentNumberOfCards >= MaxCardsAllowed;

    private List<CardID> discoveredCardIDs = new List<CardID>();

    public event Action OnDiscoverNewCard = delegate { };
    public event Action<int, int> OnUpdateNumberOfCards;
    public event Action<int, int> OnUpdateMaxNumberOfCards;

    public int GetCurrentNumberOfCards()
    {
        Card[] allStartingCards = FindObjectsByType<Card>(FindObjectsSortMode.None);
        return allStartingCards.Count(card => card is not CardShop);
    }

    protected override void Awake()
    {
        base.Awake();
        TryLoadDiscoveredCard();
    }

    private void Start()
    {
        CraftingManager.Instance.OnCraftComplete += CraftingManager_OnCraftComplete;
        CraftingManager.Instance.OnDestroyCard += CraftingManager_OnDestroyCard;
        Reseller.OnResell += Reseller_OnResell;
        TowerDamageable.OnTowerDie += TowerDamageable_OnTowerDie;
        Booster.OnOpenCardIdea += Booster_OnOpenCardIdea;
        Booster.OnOpenBooster += Booster_OnOpenBooster;
        CardExploitation.OnDestroyCardExploitation += CardExploitation_OnDestroyCardExploitation;
    }

    private void OnDisable()
    {
        if (CraftingManager.HasInstance)
        {
            CraftingManager.Instance.OnCraftComplete -= CraftingManager_OnCraftComplete;
            CraftingManager.Instance.OnDestroyCard -= CraftingManager_OnDestroyCard;
        }
        Booster.OnOpenBooster -= Booster_OnOpenBooster;
        Booster.OnOpenCardIdea -= Booster_OnOpenCardIdea;
        Reseller.OnResell -= Reseller_OnResell;
        TowerDamageable.OnTowerDie -= TowerDamageable_OnTowerDie;
        CardExploitation.OnDestroyCardExploitation -= CardExploitation_OnDestroyCardExploitation;
    }
    private void CardExploitation_OnDestroyCardExploitation()
    {
        CurrentNumberOfCards--;
        OnUpdateNumberOfCards?.Invoke(CurrentNumberOfCards, MaxCardsAllowed);
    }

    private void TowerDamageable_OnTowerDie()
    {
        CurrentNumberOfCards--;
        OnUpdateNumberOfCards?.Invoke(CurrentNumberOfCards, MaxCardsAllowed);
    }
    private void CraftingManager_OnDestroyCard()
    {
        CurrentNumberOfCards--;
        OnUpdateNumberOfCards?.Invoke(CurrentNumberOfCards, MaxCardsAllowed);
    }
    private void Reseller_OnResell(int numberOfReselledCard)
    {
        CurrentNumberOfCards -= numberOfReselledCard;
        OnUpdateNumberOfCards?.Invoke(CurrentNumberOfCards, MaxCardsAllowed);
    }

    private void Booster_OnOpenCardIdea()
    {
        CurrentNumberOfCards++;
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

    public CardDiscoveryState GetCardDiscoveryStateByCardID(CardID cardID) => allCards.First(cardDiscoveryState => cardDiscoveryState.Card.CardData.CardID == cardID);

    public void SetNotificationStatusByCardID(CardID cardID, bool notificationStatus)
    {
        GetCardDiscoveryStateByCardID(cardID).isClickedAfterNotification = notificationStatus;
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

    [ContextMenu("Set All Cards To Not Discovered")]
    public void UncheckDiscovered()
    {
        allCards.ForEach(cardDiscoveryState => 
        {
            cardDiscoveryState.isDiscovered = false;
            cardDiscoveryState.isClickedAfterNotification = false;
        });
    }

    [ContextMenu("Set All Cards To Discovered")]
    public void CheckDiscovered()
    {
        allCards.ForEach(cardDiscoveryState => {
            cardDiscoveryState.isDiscovered = true;
            cardDiscoveryState.isClickedAfterNotification = false;
        });
    }

    public void Save(GameSaveData gameSaveData)
    {
        gameSaveData.currentNumberOfCards = CurrentNumberOfCards;
        gameSaveData.maxCardsAllowed = startMaxCardsAllowed;

        TrySaveDiscoveredCards();
    }

    public void TrySaveDiscoveredCards()
    {
        try
        {
            string json = JsonUtility.ToJson(new CardManagerSaveData(allCards), true);
            File.WriteAllText(SavePath.SavePathCardDiscovered, json);
            Debug.Log($"Game saved to {SavePath.SavePathCardDiscovered}", this);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}", this);
        }
    }

    public void Load(GameSaveData gameSaveData)
    {
        CurrentNumberOfCards = gameSaveData.currentNumberOfCards;
        MaxCardsAllowed = gameSaveData.maxCardsAllowed;
        OnUpdateMaxNumberOfCards?.Invoke(CurrentNumberOfCards, MaxCardsAllowed);
        OnUpdateNumberOfCards?.Invoke(CurrentNumberOfCards, MaxCardsAllowed);

        TryLoadDiscoveredCard();
    }

    private void TryLoadDiscoveredCard()
    {
        try
        {
            if (File.Exists(SavePath.SavePathCardDiscovered))
            {
                string json = File.ReadAllText(SavePath.SavePathCardDiscovered);
                CardManagerSaveData saveData = JsonUtility.FromJson<CardManagerSaveData>(json);

                if (saveData.allCards.Count == allCards.Count)
                {
                    for (int i = 0; i < saveData.allCards.Count; i++)
                    {
                        CardDiscoveryState cardDiscoveryState = saveData.allCards[i];
                        allCards[i].isDiscovered = cardDiscoveryState.isDiscovered;
                        allCards[i].isClickedAfterNotification = cardDiscoveryState.isClickedAfterNotification;
                    }
                }
                Debug.Log($"Game loaded from {SavePath.SavePathCardDiscovered}", this);
            }
            else
            {
                Debug.Log("No save file found, using default GameMode", this);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}", this);
        }
    }
}
