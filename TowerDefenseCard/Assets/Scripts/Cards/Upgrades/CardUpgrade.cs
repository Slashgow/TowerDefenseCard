using System.Linq;
using UnityEngine;

public class CardUpgrade : Card
{
    [SerializeField] private UpgradeData upgradeData;
    public UpgradeData UpgradeData => upgradeData;

    public override void OnStack(Card targetCard)
    {
        base.OnStack(targetCard);

        var upgradables = targetCard.GetComponentsInParent<IUpgradable>();

        if (upgradables.Length > 0)
        {
            foreach ( var upgradable in upgradables)
            {
                //IUpgradable upgradable = (IUpgradable)targetCard;
                if (upgradable.CanApplyUpgrade(upgradeData))
                {
                    upgradable.ApplyUpgrade(upgradeData);
                    Debug.Log($"Upgrade {upgradeData.UpgradeName} applied to {targetCard.name}");
                }
                else
                {
                    Debug.LogWarning($"Upgrade {upgradeData.UpgradeName} cannot be applied to {targetCard.name} (duplicate or invalid)");
                    // Optionally unstack if duplicate to maintain consistency
                    if (transform.parent == targetCard.transform)
                    {
                        transform.SetParent(null, true);
                        transform.position = Vector3.zero; // Reset position or handle return
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning($"Cannot apply upgrade {upgradeData.UpgradeName} to {targetCard.name}: Target does not support IUpgradable");
            // Unstack if incompatible
            if (transform.parent == targetCard.transform)
            {
                transform.SetParent(null, true);
                transform.position = Vector3.zero; // Reset position
            }
        }

        var childrenUpgrades = GetComponentsInChildren<CardUpgrade>().Skip(1);
        foreach (var childUpgrade in childrenUpgrades)
        {
            //Debug.Log($"child upgrade | {childUpgrade}");
            childUpgrade.OnStack(targetCard);
        }
    }

    // Override to handle stacking initiation from CardMover
    public override void OnStackInitiate(Card targetCard)
    {
        // This method is called when the card is about to be stacked (e.g., in TryStackCards)
        OnStack(targetCard); // Apply the upgrade logic
        base.OnStackInitiate(targetCard); // Allow default stacking behavior
    }

    // Handle unstacking when removed
    public override void OnUnstack(Card targetCard)
    {
        var upgradables = targetCard.GetComponentsInParent<IUpgradable>();

        if (upgradables.Length > 0)
        {
            foreach( var upgradable in upgradables)
            {
                upgradable.RemoveUpgrade(upgradeData.UpgradeName);
                Debug.Log($"Upgrade {upgradeData.UpgradeName} removed from {targetCard.name}");
            }
        }
        base.OnUnstack(targetCard);

        var childrenUpgrades = GetComponentsInChildren<CardUpgrade>().Skip(1);
        foreach (var childUpgrade in childrenUpgrades)
        {
            childUpgrade.OnUnstack(targetCard);
        }
    }
}