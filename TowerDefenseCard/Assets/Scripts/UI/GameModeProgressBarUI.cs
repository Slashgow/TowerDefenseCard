using UnityEngine;
using UnityEngine.UI;

public class GameModeProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image filledImage;

    private float totalTimeCraftingMode;

    private void OnEnable()
    {
        filledImage.fillAmount = 0;
        totalTimeCraftingMode = CraftingManager.Instance.TimeCraftMode;
        CraftingManager.Instance.OnTickTimeCraftingMode += CraftingManager_OnTickTimeCraftingMode;
    }

    private void OnDisable()
    {
        CraftingManager.Instance.OnTickTimeCraftingMode -= CraftingManager_OnTickTimeCraftingMode;
    }

    private void CraftingManager_OnTickTimeCraftingMode(float timeElapsed)
    {
        filledImage.fillAmount = timeElapsed / totalTimeCraftingMode;
    }
}
