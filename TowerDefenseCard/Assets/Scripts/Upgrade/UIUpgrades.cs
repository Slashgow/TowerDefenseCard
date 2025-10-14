using System.Linq;
using UnityEngine;

public class UIUpgrades : MonoBehaviour
{
    [SerializeField] private UIUpgradeSlot[] upgradeSlots;

    public UIUpgradeSlot[] UpgradeSlots => upgradeSlots;


    public bool HasEmptySlot() => upgradeSlots.Any(upgradeSlots => upgradeSlots.IsEmpty);

    public UIUpgradeSlot GetFirstEmptySlot()
    {
        foreach (var slot in upgradeSlots)
        {
            if (slot.IsEmpty)
                return slot;
        }
        return null;
    }

}
