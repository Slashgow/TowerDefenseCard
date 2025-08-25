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
        GameManager.Instance.OnStartCraftMode -= GameManager_OnStartCraftMode;
        GameManager.Instance.OnStartCraftMode += GameManager_OnStartCraftMode;
        CraftingManager.Instance.OnTickTimeCraftingMode -= CraftingManager_OnTickTimeCraftingMode;
        CraftingManager.Instance.OnTickTimeCraftingMode += CraftingManager_OnTickTimeCraftingMode;
    }

    private void GameManager_OnStartCraftMode()
    {
        filledImage.fillAmount = 0;
        totalTimeCraftingMode = CraftingManager.Instance.OriginaCurrentTotalTimeCraftMode;
    }

    private void CraftingManager_OnTickTimeCraftingMode(float timeElapsed)
    {
        filledImage.fillAmount = timeElapsed / totalTimeCraftingMode;
    }
}
