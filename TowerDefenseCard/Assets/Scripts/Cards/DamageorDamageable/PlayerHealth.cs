using System;

public class PlayerHealth : BaseDamageable
{
    public static event Action OnPlayerDie;

    public override void Die()
    {
        base.Die();
        OnPlayerDie?.Invoke();
    }
}
