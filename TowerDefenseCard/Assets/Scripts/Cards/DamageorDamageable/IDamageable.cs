using System;
using System.Numerics;

public interface IDamageable
{
    public float MaxHealth { get; }
    public float CurrentHealth { get; }
    public void TakeDamage(float damage);
    public void Die();
    public bool IsDead { get; }

    public bool IsProtected { get; set; }
    public bool CanBeProtected { get; }

    public event Action<float> OnTakeDamage;
    public static event Action<float, Vector3> OnAnyDamageableTakeDamage;
    public event Action OnDie;
}