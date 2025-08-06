using System;

public interface IDamageor
{
    public bool CanAttack { get; }
    public float Damage { get;}
    public float AttackRange { get; }
    public float AttackSpeed { get; }
    public float AttackArea { get;  }
    public DamageType DamageType { get; }
    public event Action<BaseDamageable> OnAttackEvent;

}
