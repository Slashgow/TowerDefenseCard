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
    [SerializeField] private CardShop basePackCardShop;
    [SerializeField] private CardShop defensePackCardShop;
    [SerializeField] private CardShop engineeringPackCardShop;
    [SerializeField] private CardShop foodPackCardShop;
    [SerializeField] private SuccessSaveSystem successSaveSystem;

    [SerializeField] private Logger logger;

    protected override void Awake()
    {
        base.Awake();
        LoadGame();
    }

    private void Start()
    {
        PlayerHealth.OnPlayerDie += PlayerHealth_OnPlayerDie;
        GameManager.Instance.OnDefeatAllWaves += GameManager_OnDefeatAllWaves;
    }

    private void OnDestroy()
    {
        PlayerHealth.OnPlayerDie -= PlayerHealth_OnPlayerDie;

        if (GameManager.HasInstance)
        {
            GameManager.Instance.OnDefeatAllWaves -= GameManager_OnDefeatAllWaves;
        }
    }

    private void PlayerHealth_OnPlayerDie()
    {
        GameManager.Instance.ResetGameSaveAndSaveCards();
    }

    private void GameManager_OnDefeatAllWaves()
    {
        GameManager.Instance.ResetGameSaveAndSaveCards();
    }

    public static void ResetGameSave()
    {
        SavePath.DeleteGameSaves();
        SuccessSaveSystem.ResetSaveStatsByGame();
    }


    public void SaveGame() => SaveGame(SavePath.SaveFilePath);

    public void SaveGame(string filePath)
    {
        try
        {
            GameSaveData saveData = new GameSaveData();

            Save(saveData);

            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(filePath, json);

            string saveType = filePath.Contains("AutoSaves") ? "Auto" : "Manual";
            logger.Log($"{saveType} game saved to {filePath}", this);
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to save game to {filePath}: {e.Message}", this);
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
        basePackCardShop.Save(saveData);
        defensePackCardShop.Save(saveData);
        engineeringPackCardShop.Save(saveData);
        foodPackCardShop.Save(saveData);
        SaveCards(saveData);

        if(DifficultyManager.HasInstance)
            DifficultyManager.Instance.Save(saveData);
    }
    private void SaveCards(GameSaveData saveData)
    {
        Card[] cardsOnBoard = FindObjectsByType<Card>(FindObjectsSortMode.None);
        HashSet<Card> processedCards = new HashSet<Card>();

        foreach (Card card in cardsOnBoard)
        {
            if (card is CardShop && card is not Merchant)
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
                    List<UpgradeSlotSaveData> upgradeSlotSaveDatas = new List<UpgradeSlotSaveData>();
                    RecruterSaveData recruterSaveData = null;

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
                    else if (stackCard is CardRecruter)
                    {
                        CardRecruter recruter = (CardRecruter)stackCard;
                        recruterSaveData = recruter.Save();
                    }

                    else if(!stackCard.IsStackRoot() && stackCard is CardUpgrade)
                    {
                        continue;
                    }

                    else if (stackCard.GetComponentInChildren<UIUpgrades>())
                    {
                        UIUpgrades uIUpgrades = stackCard.GetComponentInChildren<UIUpgrades>();
                        foreach (UIUpgradeSlot upgradeSlot in uIUpgrades.UpgradeSlots)
                        {
                            if (upgradeSlot != null)
                            {
                                UpgradeSlotSaveData upgradeSlotSaveData = upgradeSlot.Save();
                                upgradeSlotSaveDatas.Add(upgradeSlotSaveData);
                            }
                        }
                    }

                        CardSaveData cardSaveData = new CardSaveData(
                            stackCard.CardData.CardID,
                            stackCard.transform,
                            stackCard.StackCount,
                            boosterSaveData,
                            cardIdeaSaveData,
                            currentAmountOfCurrency,
                            autoCardMovementData,
                            upgradeSlotSaveDatas,
                            recruterSaveData
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
            string mostRecentSavePath = SavePath.GetMostRecentSavePath();

            if (!string.IsNullOrEmpty(mostRecentSavePath))
            {
                LoadGameFromPath(mostRecentSavePath);

                string saveType = mostRecentSavePath.Contains("AutoSaves") ? "auto save" : "manual save";
                logger.Log($"Game loaded from {saveType}: {mostRecentSavePath}", this);
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

    public void LoadGameFromPath(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
                Load(saveData);
                logger.Log($"Game loaded from {filePath}", this);
            }
            else
            {
                logger.LogError($"Save file not found: {filePath}", this);
                LoadDefault();
            }
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to load game from {filePath}: {e.Message}", this);
            LoadDefault();
        }
    }


    public void LoadManualSave()
    {
        try
        {
            if (SavePath.SaveExists)
            {
                LoadGameFromPath(SavePath.SaveFilePath);
            }
            else
            {
                logger.Log("No manual save file found", this);
                LoadDefault();
            }
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to load manual save: {e.Message}", this);
            LoadDefault();
        }
    }


    public void LoadAutoSave()
    {
        try
        {
            if (SavePath.AutoSaveExists)
            {
                LoadGameFromPath(SavePath.AutoSaveFilePath);
            }
            else
            {
                logger.Log("No auto save file found", this);
                LoadDefault();
            }
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to load auto save: {e.Message}", this);
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
        basePackCardShop.Load(saveData);
        defensePackCardShop.Load(saveData);
        engineeringPackCardShop.Load(saveData);
        foodPackCardShop.Load(saveData);
        //successSaveSystem.Load();
        LoadCards(saveData);

        if (DifficultyManager.HasInstance)
            DifficultyManager.Instance.Load(saveData);
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

            if (newCard is Booster)
            {
                Booster booster = (Booster)newCard;
                booster.Load(cardData.boosterSaveData);
            }

            else if (newCard is CardIdea)
            {
                CardIdea cardIdea = (CardIdea)newCard;
                cardIdea.Load(cardData.cardIdeaSaveData);
            }

            else if (newCard is CardCurrencyCollecter)
            {
                CardCurrencyCollecter cardCurrencyCollecter = (CardCurrencyCollecter)newCard;
                cardCurrencyCollecter.Load(cardData);

            }
            else if (newCard is CardRecruter)
            {
                CardRecruter recruter = (CardRecruter)newCard;
                recruter.Load(cardData.recruterSaveData);
            }


            else if (newCard.TryGetComponent(out AutoCardMovement autoCardMovement))
                autoCardMovement.Load(cardData.autoCardMovementSaveData);


            else if (cardData.upgradeSlotSaveDatas != null && cardData.upgradeSlotSaveDatas.Count > 0)
            {

                if (newCard.GetComponentInChildren<UIUpgrades>())
                {
                    UIUpgrades uIUpgrades = newCard.GetComponentInChildren<UIUpgrades>();
                    if (uIUpgrades.UpgradeSlots.Length < cardData.upgradeSlotSaveDatas.Count)
                    {
                        logger.LogWarning($"Card {newCard.CardData.CardID} has fewer upgrade slots ({uIUpgrades.UpgradeSlots.Length}) " +
                            $"than saved upgrades ({cardData.upgradeSlotSaveDatas.Count}). Some upgrades may not be loaded correctly.", this);
                    }
                    else
                    {
                        for (int j = 0; j < cardData.upgradeSlotSaveDatas.Count; j++)
                        {
                            uIUpgrades.UpgradeSlots[j].Load(cardData.upgradeSlotSaveDatas[j]);
                        }
                    }
                } 
            }

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
            //maxCardsAllowed = cardManager.StartMaxCardsAllowed,
            currentNumberOfCards = cardManager.GetCurrentNumberOfCards(),
            //maxDefenseCardsAllowed = cardManager.StartMaxCardsDefenseAllowed,
            //currentNumberOfDefenseCards = cardManager.GetCurrentNumberOfDefenseCards(),
            isBasePackFirstTimeOpened = false,
            basePackRiggedCardIndex = 0,
            isDefensePackFirstTimeOpened = false,
            isEngineeringPackFirstTimeOpened = false,
            isFoodPackFirstTimeOpened = false,
            gameDifficulty = DifficultyManager.HasInstance ? DifficultyManager.Instance.CurrentDifficultyData.Difficulty : GameDifficulty.EASY
        };
        Load(saveData);
    }
}