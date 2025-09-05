using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSaveSystem : MonoSingleton<GameSaveSystem>
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private CraftingManager craftingManager;
    [SerializeField] private CardManager cardManager;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private QuestManager mainQuestManager;
    [SerializeField] private QuestManager secondaryQuestManager;
    [SerializeField] private SuccessSaveSystem successSaveSystem;

    [SerializeField] private Logger logger;

    protected override void Awake()
    {
        base.Awake();
        LoadGame();
    }

    public static void ResetSave()
    {
        if(SavePath.SaveExists)
        {
            File.Delete(SavePath.SaveFilePath);
            SuccessSaveSystem.ResetSaveStatsByGame();
            //File.Delete(SavePath.SavePathCardDiscovered);
        }
    }

    public void SaveGame()
    {
        try
        {
            GameSaveData saveData = new GameSaveData();

            Save(saveData);

            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(SavePath.SaveFilePath, json);
            logger.Log($"Game saved to {SavePath.SaveFilePath}", this);
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to save game: {e.Message}", this);
        }
    }

    private void Save(GameSaveData saveData)
    {
        gameManager.Save(saveData);
        waveManager.Save(saveData);
        shopManager.Save(saveData);
        craftingManager.Save(saveData);
        cardManager.Save(saveData);
        playerHealth.Save(saveData);
        successSaveSystem.Save();
        SaveCards(saveData);
        mainQuestManager.AvailableQuests.ForEach(quest => quest.Save(saveData));
        secondaryQuestManager.AvailableQuests.ForEach(quest => quest.Save(saveData));
    }
    private void SaveCards(GameSaveData saveData)
    {
        Card[] cardsOnBoard = FindObjectsByType<Card>(FindObjectsSortMode.None);
        HashSet<Card> processedCards = new HashSet<Card>();

        foreach (Card card in cardsOnBoard)
        {
            if (card is CardShop)
                continue;

            if (card.GetComponent<PlayerHealth>())
                continue;

            if (card.IsStackRoot() && !processedCards.Contains(card))
            {
                StackSaveData stackData = new StackSaveData();

                // Get the entire stack starting from this root
                List<Card> stackCards = CardUtility.GetAllCards(card.gameObject);

                foreach (Card stackCard in stackCards)
                {
                    BoosterSaveData boosterSaveData = null;
                    CardIdeaSaveData cardIdeaSaveData = null;
                    AutoCardMovementData autoCardMovementData = null;
                    int currentAmountOfCurrency = 0;

                    if (stackCard is Booster)
                    {
                        Booster booster = stackCard as Booster;
                        boosterSaveData = booster.Save();
                    }
                    else if(stackCard is CardIdea)
                    {
                        CardIdea cardIdea = stackCard as CardIdea;
                        cardIdeaSaveData = cardIdea.Save();
                    }
                    else if(stackCard is CardCurrencyCollecter)
                    {
                        CardCurrencyCollecter cardCurrencyCollecter = (CardCurrencyCollecter)stackCard;
                        currentAmountOfCurrency = cardCurrencyCollecter.Save();
                    }
                    else if (stackCard.TryGetComponent(out AutoCardMovement autoCardMovement))
                    {
                        autoCardMovementData = autoCardMovement.Save();
                    }

                        CardSaveData cardSaveData = new CardSaveData(
                            stackCard.CardData.CardID,
                            stackCard.transform,
                            stackCard.StackCount,
                            boosterSaveData,
                            cardIdeaSaveData,
                            currentAmountOfCurrency,
                            autoCardMovementData
                        );

                    stackData.AddCard(cardSaveData);
                    processedCards.Add(stackCard);
                }

                saveData.cardStacks.Add(stackData);
            }
        }

        logger.Log($"Saved {saveData.cardStacks.Count} card stacks with total cards: {processedCards.Count}", this);
    }

    public void LoadGame()
    {
        try
        {
            if (File.Exists(SavePath.SaveFilePath))
            {
                string json = File.ReadAllText(SavePath.SaveFilePath);
                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
                Load(saveData);
                logger.Log($"Game loaded from {SavePath.SaveFilePath}", this);
            }
            else
            {
                logger.Log("No save file found, using default GameMode", this);
                LoadDefault();
            }
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to load game: {e.Message}", this);
            LoadDefault();
        }
    }

    private void Load(GameSaveData saveData)
    {
        gameManager.Load(saveData);
        waveManager.Load(saveData);
        shopManager.Load(saveData);
        craftingManager.Load(saveData);
        cardManager.Load(saveData);
        playerHealth.Load(saveData);
        successSaveSystem.Load();
        LoadCards(saveData);
        mainQuestManager.AvailableQuests.ForEach(quest => quest.Load(saveData));
        secondaryQuestManager.AvailableQuests.ForEach(quest => quest.Load(saveData));
    }

    private void LoadCards(GameSaveData saveData)
    {
       //// Clear existing cards
       //Card[] existingCards = FindObjectsByType<Card>(FindObjectsSortMode.None);
       //foreach (Card card in existingCards)
       //{
       //    DestroyImmediate(card.gameObject);
       //}

        // Load each stack
        foreach (StackSaveData stackData in saveData.cardStacks)
        {
            LoadStack(stackData, saveData);
        }

        logger.Log($"Loaded {saveData.cardStacks.Count} card stacks", this);
    }

    private void LoadStack(StackSaveData stackData, GameSaveData saveData)
    {
        if (stackData.cards.Count == 0) 
            return;

        for (int i = 0; i < stackData.cards.Count; i++)
        {
            CardSaveData cardData = stackData.cards[i];

            Card newCardPrefab = cardManager.GetCardPrefabByCardID(cardData.cardID);
            Card newCard = Instantiate(newCardPrefab, cardData.position, cardData.rotation);

            if (newCard == null)
            {
                logger.LogError($"Failed to instantiate card with ID: {cardData.cardID}", this);
                continue;
            }

            if(newCard.TryGetComponent(out BaseCardMovement baseCardMovement))
            {
                baseCardMovement.StopSmoothMove();
                //baseCardMovement.InitializeSortOrder();
            }

            if(newCard is Booster)
            {
                Booster booster = (Booster)newCard;
                booster.Load(cardData.boosterSaveData);
            }

            else if(newCard is CardIdea)
            {
                CardIdea cardIdea = (CardIdea)newCard;
                cardIdea.Load(cardData.cardIdeaSaveData);
            }

            else if(newCard is CardCurrencyCollecter)
            {
                CardCurrencyCollecter cardCurrencyCollecter = (CardCurrencyCollecter)newCard;
                cardCurrencyCollecter.Load(cardData);
                
            }

            else if (newCard.TryGetComponent(out AutoCardMovement autoCardMovement))
                autoCardMovement.Load(cardData.autoCardMovementSaveData);

            newCard.transform.position = cardData.position;
            newCard.transform.rotation = cardData.rotation;
            newCard.transform.localScale = cardData.scale;
            //newCard.StackCount = cardData.stackCount;
        }
    }

    private void LoadDefault()
    {
        GameSaveData saveData = new GameSaveData()
        {
            currentPlayerHealth = playerHealth.MaxHealth,
            gameMode = GameMode.CRAFTING,
            currentWaveIndex = 0,
            currentWaveEnnemyIndex = 0,
            currentEnnemyCount = 0,
            amountOfSpawnedEnemies = 0,
            currentPlayerCoin = shopManager.StartPlayerCoin,
            craftTimeElapsed = 0,
            maxCardsAllowed = cardManager.StartMaxCardsAllowed,
            currentNumberOfCards = cardManager.GetCurrentNumberOfCards(),
            maxDefenseCardsAllowed = cardManager.StartMaxCardsDefenseAllowed,
            currentNumberOfDefenseCards = cardManager.GetCurrentNumberOfDefenseCards()
        };
        Load(saveData);
    }
}