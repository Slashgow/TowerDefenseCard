using UnityEngine;
using UnityEngine.UI;

public class GameModeProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image filledImage;

    private float totalTimeCraftingMode;

    private void Start()
    {
        filledImage.fillAmount = 0;
        totalTimeCraftingMode = CraftingManager.Instance.OriginaCurrentTotalTimeCraftMode;
        CraftingManager.Instance.OnStartCraftTimer -= CraftingManager_StartCraftTimer;
        CraftingManager.Instance.OnStartCraftTimer += CraftingManager_StartCraftTimer;
        CraftingManager.Instance.OnTickTimeCraftingMode -= CraftingManager_OnTickTimeCraftingMode;
        CraftingManager.Instance.OnTickTimeCraftingMode += CraftingManager_OnTickTimeCraftingMode;
    }

    private void OnDestroy()
    {
        if(CraftingManager.HasInstance)
        {
            CraftingManager.Instance.OnStartCraftTimer -= CraftingManager_StartCraftTimer;
            CraftingManager.Instance.OnStartCraftTimer -= CraftingManager_StartCraftTimer;
            CraftingManager.Instance.OnTickTimeCraftingMode -= CraftingManager_OnTickTimeCraftingMode;
            CraftingManager.Instance.OnTickTimeCraftingMode -= CraftingManager_OnTickTimeCraftingMode;
        }
    }

    private void CraftingManager_StartCraftTimer()
    {
        filledImage.fillAmount = 0;
        totalTimeCraftingMode = CraftingManager.Instance.OriginaCurrentTotalTimeCraftMode;
    }

    private void CraftingManager_OnTickTimeCraftingMode(float timeElapsed)
    {
        filledImage.fillAmount = timeElapsed / totalTimeCraftingMode;
    }
}
