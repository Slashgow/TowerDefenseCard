
using System;

public class TowerDamageable : BaseDamageable
{
    public static event Action OnTowerDie;
    public override void Die()
    {
        base.Die();
        OnTowerDie?.Invoke();
        Destroy(this.gameObject);
    }
}
