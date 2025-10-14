using TMPro;
using UnityEngine;

public class UIUpgradeDescriptionSpawner : MonoBehaviour
{
    [SerializeField] private UIUpgradeDescription upgradeDescriptionPrefab;
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private Vector3 worldOffset = Vector3.up * 2f;

    private UIUpgradeDescription currentUpgradeDescription;

    private void Start()
    {
        UIUpgradeSlot.OnHoverEnter += UIUpgradeSlot_OnHoverEnter;
        UIUpgradeSlot.OnHoverExit += UIUpgradeSlot_OnHoverExit;
        UIUpgradeSlot.OnClick += UIUpgradeSlot_OnHoverExit;
    }

    private void OnDestroy()
    {
        UIUpgradeSlot.OnHoverEnter -= UIUpgradeSlot_OnHoverEnter;
        UIUpgradeSlot.OnHoverExit -= UIUpgradeSlot_OnHoverExit;
        UIUpgradeSlot.OnClick -= UIUpgradeSlot_OnHoverExit;
    }

    private void UIUpgradeSlot_OnHoverExit()
    {
        if (currentUpgradeDescription == null)
            return;

        Destroy(currentUpgradeDescription.gameObject);
    }

    private void UIUpgradeSlot_OnHoverEnter(Vector3 worldPosition, CardID cardID)
    {
        GameObject currentUpgradeDescriptionGameObject = Instantiate(upgradeDescriptionPrefab.gameObject, worldPosition + worldOffset, Quaternion.identity, worldCanvas.transform);
        currentUpgradeDescription = currentUpgradeDescriptionGameObject.GetComponent<UIUpgradeDescription>();
        currentUpgradeDescription.Init(cardID);
    }

}