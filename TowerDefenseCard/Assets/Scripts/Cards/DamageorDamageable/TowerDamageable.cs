using System;
using System.Collections.Generic;

public class TowerDamageable : BaseDamageable
{
    public static event Action<int> OnTowerDie;

    public override void Die()
    {
        base.Die();
 
        int numberOfDestroyedUpgrades = 0;

        if (TryGetComponent(out Card card))
        {
            Card[] children = card.GetComponentsInChildren<Card>();
            List<CardUpgrade> upgradesToDestroy = new List<CardUpgrade>();
            Card firstNonUpgrade = null;

            for (int i = 0; i < children.Length; i++)
            {
                Card child = children[i];
                if (child is CardUpgrade upgrade)
                    upgradesToDestroy.Add(upgrade);

                else if (i==1 && firstNonUpgrade == null)
                    firstNonUpgrade = child;
            }

            numberOfDestroyedUpgrades = upgradesToDestroy.Count;

            foreach (CardUpgrade upgrade in upgradesToDestroy)
            {
                upgrade.OnUnstack();
                Destroy(upgrade.gameObject);
            }

            if (firstNonUpgrade != null)
                firstNonUpgrade.OnUnstack();
        }


        OnTowerDie?.Invoke(numberOfDestroyedUpgrades + 1);

        Destroy(this.gameObject);


    }
}
