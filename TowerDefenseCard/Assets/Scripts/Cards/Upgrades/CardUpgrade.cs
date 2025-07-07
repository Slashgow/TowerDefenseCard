using UnityEngine;

public class CardUpgrade : Card
{
    [SerializeField] private UpgradeData upgradeData;

    public override void OnStack(Card targetCard)
    {
        base.OnStack(targetCard);

        // Check if the target card implements IDamageor and IUpgradable
        if (targetCard is IUpgradable)
        {
            IUpgradable upgradable = (IUpgradable)targetCard;
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
        else
        {
            Debug.LogWarning($"Cannot apply upgrade {upgradeData.UpgradeName} to {targetCard.name}: Target does not support IDamageor or IUpgradable");
            // Unstack if incompatible
            if (transform.parent == targetCard.transform)
            {
                transform.SetParent(null, true);
                transform.position = Vector3.zero; // Reset position
            }
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
        if (targetCard is IUpgradable)
        {
            IUpgradable upgradable = (IUpgradable)targetCard;
            upgradable.RemoveUpgrade(upgradeData.UpgradeName);
            Debug.Log($"Upgrade {upgradeData.UpgradeName} removed from {targetCard.name}");
        }
        base.OnUnstack(targetCard);
    }
}