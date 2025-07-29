using System;
using System.IO;
using UnityEngine;

public class GameSaveSystem : MonoSingleton<GameSaveSystem>
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private CraftingManager craftingManager;

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

        Card[] cardsOnBoard = FindObjectsByType<Card>(FindObjectsSortMode.None);
        foreach (Card card in cardsOnBoard)
        {

        }
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
            currentPlayerCoin = 0,
            craftTimeElapsed = 0,
        };
        Load(saveData);
    }
}