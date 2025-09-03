using System;

public class TowerDamageable : BaseDamageable
{
    public static event Action OnTowerDie;

    public override void Die()
    {
        base.Die();
        OnTowerDie?.Invoke();

        if (TryGetComponent(out Card card))
        {
            Card[] children = card.GetComponentsInChildren<Card>();
            if(children != null && children.Length > 1)
                children[1].OnUnstack();
        }
        

        Destroy(this.gameObject);


    }
}
