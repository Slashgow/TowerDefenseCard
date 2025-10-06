using UnityEngine;
using Steamworks;
using NaughtyAttributes;

public class SteamIntegration : MonoSingleton<SteamIntegration>
{
    [SerializeField] private Logger logger;

    private void Start()
    {
        try
        {
            SteamClient.Init(4053750);
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
}
