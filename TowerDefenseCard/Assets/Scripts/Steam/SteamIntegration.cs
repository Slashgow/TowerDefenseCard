using UnityEngine;
using Steamworks;
using NaughtyAttributes;

public class SteamIntegration : MonoSingleton<SteamIntegration>
{
    [SerializeField] private bool useDefaultAppID = true;
    [SerializeField, ShowIf("useDefaultAppID")] private uint appID = 4053750;
    [SerializeField] private bool usePlaytestAppID = false;
    [SerializeField, ShowIf("usePlaytestAppID")] private uint appIDPlaytest = 480;
    [SerializeField] private bool useDemoAppID = false;
    [SerializeField, ShowIf("useDemoAppID")] private uint appIDDemo = 4061600;
    [SerializeField] private Logger logger;

#if !UNITY_WEBGL
    private void Start()
    {
        try
        {
            if (usePlaytestAppID)
            {
                SteamClient.Init(appIDPlaytest);
                logger.Log($"Initialized Steamworks with Playtest App ID: {appIDPlaytest}", this);
            }
            else if (useDemoAppID)
            {
                SteamClient.Init(appIDDemo);
                logger.Log($"Initialized Steamworks with Demo App ID: {appIDDemo}", this);
            }
            else if (useDefaultAppID)
            {
                SteamClient.Init(appID);
                logger.Log($"Initialized Steamworks with Default App ID: {appID}", this);
            }
            logger.Log(SteamClient.Name, this);


            SuccessManager.Instance.AllSuccessData.ForEach(success => success.OnComplete += UnlockAchievement);
        }
        catch (System.Exception e)
        {
            // Something went wrong - it's one of these:
            //
            //     Steam is closed?
            //     Can't find steam_api dll?
            //     Don't have permission to play app?
            //
            logger.LogError(e.Message, this);
        }
    }

    private void Update()
    {
        SteamClient.RunCallbacks();
    }

    private void OnDestroy()
    {
        if(SuccessManager.HasInstance)
            SuccessManager.Instance.AllSuccessData.ForEach(success => success.OnComplete -= UnlockAchievement);

        logger.Log("Shutting down Steamworks...", this);
        SteamClient.Shutdown();
    }

    public void IsThisAchievementUnlocked(string id)
    {
        var achievement = new Steamworks.Data.Achievement(id);
        logger.Log($"Achievement {id} unlocked: {achievement.State}", this);
    }

    public void UnlockAchievement(SuccessData successData)
    {
        string id = successData.SteamId;
        var achievement = new Steamworks.Data.Achievement(id);
        if (achievement.State)
        {
            logger.Log($"Achievement {id} already unlocked", this);
            return;
        }
        achievement.Trigger();
        logger.Log($"Achievement {id} unlocked", this);
    }

    public void ClearAchievementStatus(SuccessData successData)
    {
        string id = successData.SteamId;
        var achievement = new Steamworks.Data.Achievement(id);
        achievement.Clear();
        successData.Reset();
        logger.Log($"Achievement {id} status cleared", this);
    }

    [Button("Reset All Achievements")]
    public void ResetAllAchievements()
    {
        SuccessManager.Instance.AllSuccessData.ForEach(success => ClearAchievementStatus(success));
        logger.Log("All achievements reset", this);
    }


    public void SetStats(SuccessStatData successStatData)
    {
        logger.Log($"successStatData | boosterOpenedCounterAllTime : {successStatData.boosterOpenedCounterAllTime.ToString()}", this);
        logger.Log($"successStatData | craftedCardCounterAllTime : {successStatData.craftedCardCounterAllTime.ToString()}", this);
        logger.Log($"successStatData | soldCardCounterAllTime : {successStatData.soldCardCounterAllTime.ToString()}", this);


        bool setBoosterOpenStat = SteamUserStats.SetStat("boosterOpenedCounterAllTime", successStatData.boosterOpenedCounterAllTime);
        logger.Log($"SetStat boosterOpenedCounterAllTime success: {setBoosterOpenStat}", this);
        SteamUserStats.SetStat("craftedCardCounterAllTime", successStatData.craftedCardCounterAllTime);
        SteamUserStats.SetStat("soldCardCounterAllTime", successStatData.soldCardCounterAllTime);

        PrintStats();
    }

    [Button("Print Stats")]
    public void PrintStats()
    {
        logger.Log($"boosterOpenedCounterAllTime : {SteamUserStats.GetStatInt("boosterOpenedCounterAllTime")}", this);
        logger.Log($"craftedCardCounterAllTime : {SteamUserStats.GetStatInt("craftedCardCounterAllTime")}", this);
        logger.Log($"soldCardCounterAllTime : {SteamUserStats.GetStatInt("soldCardCounterAllTime")}", this);
    }

    public void StoreStats()
    {
        SteamUserStats.StoreStats();
    }

    [Button("Set Random Stats")]
    public void SetRandomStats()
    {
        SteamUserStats.SetStat("boosterOpenedCounterAllTime", 10);
        SteamUserStats.SetStat("craftedCardCounterAllTime", 154);
        SteamUserStats.SetStat("soldCardCounterAllTime", 20);
    }

    [Button("Reset All Stats")]
    public void ResetAllStats()
    {
        SteamUserStats.SetStat("boosterOpenedCounterAllTime", 0);
        SteamUserStats.SetStat("craftedCardCounterAllTime", 0);
        SteamUserStats.SetStat("soldCardCounterAllTime", 0);
    }
#endif
}
