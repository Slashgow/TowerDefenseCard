using System;
using UnityEngine.Events;

public class PlayerHealth : BaseDamageable
{
    public static event Action OnPlayerDie;
    public UnityEvent OnPlayerTakeDamageUnity;
    public override void TakeDamage(float damage)
    {
        OnPlayerTakeDamageUnity?.Invoke();

        base.TakeDamage(damage);
    }

    public override void Die()
    {
        base.Die();
        OnPlayerDie?.Invoke();
    }
}
