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

    [SerializeField] private Logger logger;
    
    private static string saveFilePath;
    public static bool saveExists => File.Exists(saveFilePath);

    protected override void Awake()
    {
        base.Awake();
        saveFilePath = Path.Combine(Application.persistentDataPath, "gameSave.json");
        LoadGame();
    }


    public void SaveGame()
    {
        try
        {
            GameSaveData saveData = new GameSaveData();

            Save(saveData);

            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(saveFilePath, json);
            logger.Log($"Game saved to {saveFilePath}", this);
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
        SaveCards(saveData);
    }
    private void SaveCards(GameSaveData saveData)
    {
        Card[] cardsOnBoard = FindObjectsByType<Card>(FindObjectsSortMode.None);
        HashSet<Card> processedCards = new HashSet<Card>();

        foreach (Card card in cardsOnBoard)
        {
            if (card is CardShop)
                continue;

            if (card.IsStackRoot() && !processedCards.Contains(card))
            {
                StackSaveData stackData = new StackSaveData();

                // Get the entire stack starting from this root
                List<Card> stackCards = CardUtility.GetAllCards(card.gameObject);

                foreach (Card stackCard in stackCards)
                {
                    CardSaveData cardSaveData = new CardSaveData(
                        stackCard.CardData.CardID,
                        stackCard.transform,
                        stackCard.StackCount
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
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
                Load(saveData);
                logger.Log($"Game loaded from {saveFilePath}", this);
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
        LoadCards(saveData);
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
            LoadStack(stackData);
        }

        logger.Log($"Loaded {saveData.cardStacks.Count} card stacks", this);
    }

    private void LoadStack(StackSaveData stackData)
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
                baseCardMovement.InitializeSortOrder();
            }

            newCard.transform.position = cardData.position;
            newCard.transform.rotation = cardData.rotation;
            newCard.transform.localScale = cardData.scale;
            newCard.StackCount = cardData.stackCount;
        }
    }

    private void LoadDefault()
    {
        GameSaveData saveData = new GameSaveData()
        {
            gameMode = GameMode.CRAFTING,
            currentWaveIndex = 0,
            currentWaveEnnemyIndex = 0,
            currentEnnemyCount = 0,
            amountOfSpawnedEnemies = 0,
            currentPlayerCoin = shopManager.StartPlayerCoin,
            craftTimeElapsed = 0,
            maxCardsAllowed = cardManager.StartMaxCardsAllowed,
            currentNumberOfCards = cardManager.GetCurrentNumberOfCards()
        };
        Load(saveData);
    }
}