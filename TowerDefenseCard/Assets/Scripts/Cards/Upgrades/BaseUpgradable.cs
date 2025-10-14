using System.Linq;
using UnityEngine;

public abstract class BaseUpgradable : MonoBehaviour, IUpgradable
{
    private UpgradeData[] appliedUpgrades = new UpgradeData[0];

    protected float GetTotalUpgradeMultiplier(float baseValue, System.Func<UpgradeData, float> getMultiplier)
    {
        return appliedUpgrades.Aggregate(1f, (acc, u) => acc * getMultiplier(u));
    }

    protected float GetTotalUpgradeFlatBonus(float baseValue, System.Func<UpgradeData, float> getBonus)
    {
        return appliedUpgrades.Sum(u => getBonus(u));
    }

    public virtual void ApplyUpgrade(UpgradeData upgrade)
    {
        if (CanApplyUpgrade(upgrade))
        {
            System.Array.Resize(ref appliedUpgrades, appliedUpgrades.Length + 1);
            appliedUpgrades[appliedUpgrades.Length - 1] = upgrade;
            Debug.Log($"Applied upgrade {upgrade.UpgradeName} to {gameObject.name}");
        }
    }

    public bool CanApplyUpgrade(UpgradeData upgrade)
    {
        // Prevent duplicate upgrades (simplistic check; enhance as needed)
        return true;

        //return !System.Array.Exists(appliedUpgrades, u => u.UpgradeName == upgrade.UpgradeName);
    }

    public virtual bool RemoveUpgrade(string upgradeName)
    {
        int index = System.Array.FindIndex(appliedUpgrades, u => u.UpgradeName == upgradeName);
        if (index >= 0)
        {
            UpgradeData[] newUpgrades = new UpgradeData[appliedUpgrades.Length - 1];
            System.Array.Copy(appliedUpgrades, 0, newUpgrades, 0, index);
            System.Array.Copy(appliedUpgrades, index + 1, newUpgrades, index, appliedUpgrades.Length - index - 1);
            appliedUpgrades = newUpgrades;
            Debug.Log($"Removed upgrade {upgradeName} from {gameObject.name}");
            return true;
        }
        return false;
    }

    public UpgradeData[] GetAppliedUpgrades()
    {
        return appliedUpgrades;
    }
}
