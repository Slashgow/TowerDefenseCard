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
    public bool isLocked;
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
    [SerializeField] private bool listenToEvents = true;

   public List<CardDiscoveryState> allCards = new List<CardDiscoveryState>();
    public List<CardDiscoveryState> AllCards => allCards;

    [SerializeField, Range(0, 100)] private int startMaxCardsAllowed = 30;
    public int StartMaxCardsAllowed => DifficultyManager.HasInstance ? DifficultyManager.Instance.CurrentDifficultyData.StartMaxCardsAllowed : startMaxCardsAllowed;
    public int MaxCardsAllowed { get; private set; }
    public int CurrentNumberOfCards { get; private set; }

    [SerializeField, Range(0, 100)] private int startMaxCardsDefenseAllowed = 2;
    public int StartMaxCardsDefenseAllowed => DifficultyManager.HasInstance ? DifficultyManager.Instance.CurrentDifficultyData.StartMaxDefenseCardsAllowed : startMaxCardsDefenseAllowed;
    public int MaxCardsDefenseAllowed { get; private set; }
    public int CurrentNumberOfDefenseCards { get; private set; }
    public bool IsMaxDefenseCardsReached => CurrentNumberOfDefenseCards >= MaxCardsDefenseAllowed;

    public bool IsMaxCardsReached => CurrentNumberOfCards >= MaxCardsAllowed;

    private List<CardID> discoveredCardIDs = new List<CardID>();

    [SerializeField] private bool showOutlineTooltip = true;

    private List<Card> cardsOnBoard = new List<Card>();
    public List<Card> CardsOnBoard => new List<Card>(cardsOnBoard);
    public event Action<Card> OnCardAddedToBoard = delegate { };
    public event Action<Card> OnCardRemovedFromBoard = delegate { };

    public int TotalCostCardsOnBoard => cardsOnBoard.Sum(card => card.CardData.Cost);

    public event Action OnDiscoverNewCard = delegate { };
    public static event Action OnDiscoverAllCards = delegate { };
    public event Action OnDiscoverArcher = delegate { };
    public event Action OnDiscoverBarn = delegate { };
    public event Action<int, int> OnUpdateNumberOfCards;
    public event Action<int, int> OnUpdateMaxNumberOfCards;
    public event Action<int, int> OnUpdateNumberOfDefenseCards;
    public event Action<int, int> OnUpdateMaxNumberOfDefenseCards;
    public event Action OnMaxCardsReached;
    public event Action OnMaxCardsDefenseReached;

    private int ennemyCount;
    public int EnnemyCount
    {
        get
        {
            if(ennemyCount == 0)
                ennemyCount = AllCards.Count(cardDiscoveryState => cardDiscoveryState.Card is Ennemy);

            return ennemyCount;
        }
    }

    private int cardWithoutEnnemyCount;
    public int CardWithoutEnnemyCount
    {
        get
        {
            if(cardWithoutEnnemyCount == 0)
                cardWithoutEnnemyCount = AllCards.Count - EnnemyCount - 3; // booster and card idea

            return cardWithoutEnnemyCount;
        }
    }

    public int AllDiscoverableCards => AllCards.Count - 3;// booster and card idea

    public int GetCurrentNumberOfCards()
    {
        Card[] allStartingCards = FindObjectsByType<Card>(FindObjectsSortMode.None);
        return allStartingCards.Count(card => card is not CardShop);
    }

    public int GetCurrentNumberOfDefenseCards()
    {
        return FindObjectsByType<CardDefense>(FindObjectsSortMode.None).Length;
    }

    protected override void Awake()
    {
        base.Awake();
        TryLoadDiscoveredCard();
        CardDefense.OnDestroyAnyCardDefense += CardDefense_OnDestroyAnyCardDefense;
        CardDefense.OnCreateAnyCardDefense += CardDefense_OnCreateAnyCardDefense;
    }

    private void Start()
    {
        if(!DemoManager.Instance.UseDemoMode)
            allCards.ForEach(cardDiscoveryState => cardDiscoveryState.isLocked = false);

        if (!listenToEvents)
            return;

        if(WaveManager.HasInstance)
            WaveManager.Instance.OnSpawnEnnemy += WaveManager_OnSpawnEnnemy;

        CraftingManager.Instance.OnCraftComplete += CraftingManager_OnCraftComplete;
        CraftingManager.Instance.OnDestroyCard += CraftingManager_OnDestroyCard;
        Reseller.OnResell += Reseller_OnResell;
        TowerDamageable.OnTowerDie += TowerDamageable_OnTowerDie;
        Booster.OnOpenCardIdea += Booster_OnOpenCardIdea;
        Booster.OnOpenBooster += Booster_OnOpenBooster;
        CardExploitation.OnDestroyCardExploitation += CardExploitation_OnDestroyCardExploitation;
        Stealable.OnDestroy += IStealable_OnDestroy;
        CardRecruter.OnAnyRecruitmentComplete += CardRecruter_OnAnyRecruitmentComplete;
    }

    private void OnDisable()
    {
        if (!listenToEvents)
            return;

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
        CardDefense.OnDestroyAnyCardDefense -= CardDefense_OnDestroyAnyCardDefense;
        CardDefense.OnCreateAnyCardDefense -= CardDefense_OnCreateAnyCardDefense;
        Stealable.OnDestroy -= IStealable_OnDestroy;
        CardRecruter.OnAnyRecruitmentComplete -= CardRecruter_OnAnyRecruitmentComplete;

        if (WaveManager.HasInstance)
            WaveManager.Instance.OnSpawnEnnemy -= WaveManager_OnSpawnEnnemy;
    }

    public void AddCardToBoard(Card card)
    {
        if (card != null && !cardsOnBoard.Contains(card))
        {
            cardsOnBoard.Add(card);
            OnCardAddedToBoard?.Invoke(card);   
        }
    }

    public void RemoveCardFromBoard(Card card)
    {
        if (card != null && cardsOnBoard.Contains(card))
        {
            cardsOnBoard.Remove(card);
            OnCardRemovedFromBoard?.Invoke(card);
        }
    }

    public void ToggleCardsOutline(Card movedCard)
    {
        if(!showOutlineTooltip)
            return;

        if (movedCard is Currency)
        {
            Reseller.Instance.CardOutline.enabled = false;
            cardsOnBoard.ForEach(card => 
            {
                if(card == movedCard || card.StackedCards.Count > 0 || card.EntireStackParent.Contains(movedCard))
                    card.CardOutline.enabled = false;
                else if (card is Currency || card is CardCurrencyCollecter)
                    card.CardOutline.enabled = true;
                else if (card is CardShop)
                {
                    CardShop cardShop = (CardShop)card;
                    if (TotalCostCardsOnBoard < ShopManager.Instance.MinimumShopCost || !cardShop.Shop.IsUnlocked) // -availableCurrency
                        card.CardOutline.enabled = false;
                    else
                        card.CardOutline.enabled = true;
                }
                else
                    card.CardOutline.enabled = false;
            });
        }
        else
        {
            Reseller.Instance.CardOutline.enabled = movedCard is CardWorker ? false : true;

            if (ShopManager.Instance.CurrentPlayerCoin < ShopManager.Instance.MinimumShopCost)
            {
                if (CardManager.Instance.TotalCostCardsOnBoard < ShopManager.Instance.MinimumShopCost)
                    Reseller.Instance.CardOutline.enabled = false;
            }


            cardsOnBoard.ForEach(card =>
            {
                if (card is CardShop || card is Currency || card is CardCurrencyCollecter || !card.CardData.IsStackable
                    || card.StackedCards.Count > 0 || card == movedCard || card.EntireStackParent.Contains(movedCard))
                    card.CardOutline.enabled = false;
                else if (card is CardExploitation && movedCard is not CardWorker)
                    card.CardOutline.enabled = false;
                else
                    card.CardOutline.enabled = true;
            });
        }
    }

    public void HideAllCardsOutline()
    {
        Reseller.Instance.CardOutline.enabled = false;
        cardsOnBoard.ForEach(card => card.CardOutline.enabled = false);
    }
    private void UpdateCurrentNumberOfCard(int additionnalCard)
    {
        CurrentNumberOfCards += additionnalCard;
        OnUpdateNumberOfCards?.Invoke(CurrentNumberOfCards, MaxCardsAllowed);

        if(IsMaxCardsReached)
            OnMaxCardsReached?.Invoke();
    }

    private void IStealable_OnDestroy() => UpdateCurrentNumberOfCard(-1);
    private void CardExploitation_OnDestroyCardExploitation() => UpdateCurrentNumberOfCard(-1);
    private void TowerDamageable_OnTowerDie(int numberOfDestroyedCard) => UpdateCurrentNumberOfCard(-numberOfDestroyedCard);
    private void CraftingManager_OnDestroyCard(CardID cardID)
    {
        if(cardID == CardID.CURRENCY)
        {
            ShopManager.Instance.RemovePlayerCoin(1);
            return;
        }

        UpdateCurrentNumberOfCard(-1);
    }

    private void Reseller_OnResell(int numberOfReselledCard) => UpdateCurrentNumberOfCard(-numberOfReselledCard);
    private void Booster_OnOpenCardIdea() => UpdateCurrentNumberOfCard(1);
    private void CardRecruter_OnAnyRecruitmentComplete() => UpdateCurrentNumberOfCard(1);
    private void Booster_OnOpenBooster(CardID cardID)
    {
        CheckCardDiscoveryState(cardID);
        UpdateCurrentNumberOfCard(1);
    }


    private void WaveManager_OnSpawnEnnemy(CardID cardID) => CheckCardDiscoveryState(cardID);


    private void CraftingManager_OnCraftComplete(int craftID, CardID outputCardID)
    {
        //Debug.Log("card manager - on craft complete");
        CheckCardDiscoveryState(outputCardID);

        if (outputCardID == CardID.CURRENCY)
            return;

        UpdateCurrentNumberOfCard(1);
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

            if(AllDiscoverableCards >= allCards.Count(cardDiscoveryState => cardDiscoveryState.isDiscovered) - 3)
            {
                OnDiscoverAllCards?.Invoke();
            }

            if(outputCardID == CardID.ARCHER)
                OnDiscoverArcher?.Invoke();
            else if(outputCardID == CardID.BARN)
                OnDiscoverBarn?.Invoke();
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

    public void IncreaseMaxCardsDefenseAllowed(int additionalCards)
    {
        MaxCardsDefenseAllowed += additionalCards;
        OnUpdateMaxNumberOfDefenseCards?.Invoke(CurrentNumberOfDefenseCards, MaxCardsDefenseAllowed);
    }
    public void DecreaseMaxCardsDefenseAllowed(int additionalCards)
    {
        MaxCardsDefenseAllowed -= additionalCards;
        OnUpdateMaxNumberOfDefenseCards?.Invoke(CurrentNumberOfDefenseCards, MaxCardsDefenseAllowed);
    }
    private void CardDefense_OnCreateAnyCardDefense() => UpdateNumberOfDefenseCards(1);
    private void CardDefense_OnDestroyAnyCardDefense() => UpdateNumberOfDefenseCards(-1);
    private void UpdateNumberOfDefenseCards(int additionnalCard)
    {
        Debug.Log($"Updating Defense Cards Count: {CurrentNumberOfDefenseCards} + {additionnalCard}");
        CurrentNumberOfDefenseCards += additionnalCard;
        OnUpdateMaxNumberOfDefenseCards?.Invoke(CurrentNumberOfDefenseCards, MaxCardsDefenseAllowed);

        if(IsMaxDefenseCardsReached)
            OnMaxCardsDefenseReached?.Invoke();
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
        //gameSaveData.maxCardsAllowed = MaxCardsAllowed;
        //gameSaveData.maxDefenseCardsAllowed = MaxCardsDefenseAllowed;
        //gameSaveData.currentNumberOfDefenseCards = CurrentNumberOfDefenseCards;

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
        MaxCardsAllowed = StartMaxCardsAllowed;
        //CurrentNumberOfDefenseCards = gameSaveData.currentNumberOfDefenseCards;
        MaxCardsDefenseAllowed = StartMaxCardsDefenseAllowed;
        OnUpdateMaxNumberOfCards?.Invoke(CurrentNumberOfCards, MaxCardsAllowed);
        OnUpdateNumberOfCards?.Invoke(CurrentNumberOfCards, MaxCardsAllowed);
        OnUpdateMaxNumberOfDefenseCards?.Invoke(CurrentNumberOfDefenseCards, MaxCardsDefenseAllowed);
        OnUpdateNumberOfDefenseCards?.Invoke(CurrentNumberOfDefenseCards, MaxCardsDefenseAllowed);
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
                        //allCards[i].isLocked = cardDiscoveryState.isLocked;
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
