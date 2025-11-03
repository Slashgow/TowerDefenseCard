using System;
using System.IO;
using Steamworks;
using UnityEngine;

public class SuccessSaveSystem : MonoSingleton<SuccessSaveSystem>
{
    [SerializeField] private SuccessManager successManager;
    [SerializeField] private Logger logger;

    protected override void Awake()
    {
        Load();
    }

    private void Start()
    {
        if (successManager == null)
            return;

        foreach (SuccessData success in successManager.AllSuccessData)
        {
            success.OnComplete += OnSuccessCompleted;
        }
    }

    private void OnDestroy()
    {
        if (successManager == null)
            return;

        foreach (SuccessData success in successManager.AllSuccessData)
        {
            success.OnComplete -= OnSuccessCompleted;
        }
    }

    private void OnSuccessCompleted(SuccessData successData) => Save();

    public void Save()
    {
        if (successManager == null)
        {
            logger.LogWarning("SuccessManager is null, cannot save success data", this);
            return;
        }

        try
        {
            SuccessStatData currentStats = successManager.SuccessStatData;

#if !UNITY_WEBGL
            if(SteamClient.IsValid)
                SteamIntegration.Instance.SetStats(currentStats);
            //SteamIntegration.Instance.StoreStats();
#endif

            SuccessSaveData saveData = new SuccessSaveData(currentStats.boosterOpenedCounterAllTime, currentStats.boosterOpenedCounterInGame,
                currentStats.craftedCardCounterAllTime, currentStats.soldCardCounterAllTime, 
                currentStats.factoriesIDThisGame, currentStats.defenseIDThisGame, successManager.AllSuccessData);

            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(SavePath.SavePathSuccess, json);

            logger.Log($"Success data saved to {SavePath.SavePathSuccess}", this);
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to save success data: {e.Message}", this);
        }
    }

    public void Load()
    {
        try
        {
            if (SavePath.SaveSuccessExists)
            {
                string json = File.ReadAllText(SavePath.SavePathSuccess);
                SuccessSaveData saveData = JsonUtility.FromJson<SuccessSaveData>(json);

                if (saveData != null)
                {
                    successManager.LoadFromSuccessSaveData(saveData);
                    logger.Log($"Success data loaded from {SavePath.SavePathSuccess}", this);
                }
                else
                {
                    successManager.LoadDefault();
                    logger.LogWarning("Success save data is corrupted, using defaults", this);
                }
            }
            else
            {
                successManager.LoadDefault();
                logger.Log("No success save file found, using default values", this);
            }
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to load success data: {e.Message}", this);
        }
    }

    public void ForceSave()
    {
        Save();
    }

    public static void ResetSaveStatsByGame()
    {
        try
        {
            if (SavePath.SaveSuccessExists)
            {
                string json = File.ReadAllText(SavePath.SavePathSuccess);
                SuccessSaveData saveData = JsonUtility.FromJson<SuccessSaveData>(json);

                if (saveData != null)
                {
                   saveData.successStatData.ResetGameSpecificData();

                    string jsonModified = JsonUtility.ToJson(saveData, true);
                    File.WriteAllText(SavePath.SavePathSuccess, jsonModified);

                    Debug.Log($"Reset Game specific data {SavePath.SavePathSuccess}");
                }
                else
                {
                    Debug.LogWarning("Success save data is corrupted");
                }
            }
            else
            {
                Debug.Log("No success save file found, cant reset game specific data");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to Reset Game specific data: {e.Message}");
        }
    }
}