using UnityEngine;

public class SteamIntegration : MonoBehaviour
{
    [SerializeField] private Logger logger;

    private void Start()
    {
        try
        {
            Steamworks.SteamClient.Init(4053750);
            logger.Log(Steamworks.SteamClient.Name, this);
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
        Steamworks.SteamClient.RunCallbacks();
    }

    private void OnDestroy()
    {
        Steamworks.SteamClient.Shutdown();
    }
}
