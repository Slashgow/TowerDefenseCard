using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class CardUpgrade : Card
{
    [SerializeField] private UpgradeData upgradeData;
    public UpgradeData UpgradeData => upgradeData;

    public static event Action OnAppliedAnyUpgrade;
    public static event Action OnRemovedAnyUpgrade;

    private Coroutine coroutine;
    private IEnumerator Reposition(Card card)
    {
        yield return new WaitForEndOfFrame(); // Wait for the current frame to finish
        card.transform.position = this.transform.position + Vector3.right * 2f;
        card.transform.rotation = Quaternion.identity; // Reset rotation if needed
    }

    public override void OnStack(Card targetCard)
    {
        base.OnStack(targetCard);

        var upgradables = targetCard.GetComponentsInParent<IUpgradable>();

        if (upgradables.Length > 0)
        {
            // unstack cards if try to stack several cards stacked at once
            var childCards = GetComponentsInChildren<Card>().Skip(1).ToList();
            if(childCards.Count > 0)
            {
                Card cardToTruncate = childCards[0];

                ParentFollower follower = cardToTruncate.GetComponent<ParentFollower>();
                if (follower != null)
                {
                    follower.enabled = false;
                    follower.CancelDisableSchedule();
                }

                cardToTruncate.OnUnstack();

                if(coroutine != null)
                {
                    StopCoroutine(coroutine);
                    coroutine = null;
                }

                coroutine = StartCoroutine(Reposition(cardToTruncate));

                //cardToTruncate.transform.position = this.transform.position + Vector3.right * 2f;
            }

            foreach ( var upgradable in upgradables)
            {
                //IUpgradable upgradable = (IUpgradable)targetCard;
                if (upgradable.CanApplyUpgrade(upgradeData))
                {
                    upgradable.ApplyUpgrade(upgradeData);
                    OnAppliedAnyUpgrade?.Invoke();
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
    public override void OnUnstack()
    {
        if(StackParent != null)
        {
            var upgradables = StackParent.GetComponentsInParent<IUpgradable>();
            if (upgradables.Length > 0)
            {
                foreach (var upgradable in upgradables)
                {
                    upgradable.RemoveUpgrade(upgradeData.UpgradeName);
                    OnRemovedAnyUpgrade?.Invoke();
                    Debug.Log($"Upgrade {upgradeData.UpgradeName} removed from {StackParent.name}");
                }
            }
        }

        base.OnUnstack();


        //var childrenUpgrades = GetComponentsInChildren<CardUpgrade>().Skip(1);
        //foreach (var childUpgrade in childrenUpgrades)
        //{
        //    childUpgrade.OnUnstack();
        //}

        // ???????????????????? sert quand on veut mettre upgrade sur defense mais plusieurs upgrade stacker
        //if(StackParent != null)
        //{
        //    var upgradables = StackParent.GetComponentsInParent<IUpgradable>();
        //    if (upgradables.Length <= 0)
        //        return;
        //
        //    var childrenUpgrades = GetComponentsInChildren<CardUpgrade>().Skip(1);
        //    foreach (var childUpgrade in childrenUpgrades)
        //    {
        //        childUpgrade.OnUnstack();
        //    }
        //}

    }
}