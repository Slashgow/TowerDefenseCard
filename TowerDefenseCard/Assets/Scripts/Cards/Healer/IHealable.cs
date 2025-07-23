using System;

public interface IHealable : IDamageable
{
    void Heal(float amount);
    bool IsAlive => !IsDead;

    public event Action<float> OnHeal;
}
