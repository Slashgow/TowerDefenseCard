using UnityEngine;
using UnityTimer;

public class WarningCraftUpgrade : MonoBehaviour
{
    [SerializeField] private Vector3 worldOffset = Vector3.up * 2f;
    [SerializeField] private Transform parent;
    [SerializeField] private WarningMessage warningPrefab;
    [SerializeField, Range(0f, 5f)] private float warningMessageDuration;

    private Timer warningTimer;
    private WarningMessage warningInstance;

    private void OnDestroy()
    {
        warningTimer?.Cancel();
    }

    private void OnEnable()
    {
        CraftingManager.OnTryToCraftButHasUpgrade += CraftingManager_OnTryToCraftButHasUpgrade;
    }

    private void OnDisable()
    {
        CraftingManager.OnTryToCraftButHasUpgrade -= CraftingManager_OnTryToCraftButHasUpgrade;
    }

    private void CraftingManager_OnTryToCraftButHasUpgrade(Vector3 worldPosition)
    {
        if (warningInstance != null)
        {
            warningTimer?.Cancel();
            Destroy(warningInstance.gameObject);
        }


        warningInstance = Instantiate(warningPrefab, parent);

        warningInstance.transform.position = worldPosition + worldOffset;

        warningTimer = Timer.Register(warningMessageDuration, () =>
        {
            if (warningInstance != null)
            {
                Destroy(warningInstance.gameObject);
                warningInstance = null;
            }
        });
    }
}
