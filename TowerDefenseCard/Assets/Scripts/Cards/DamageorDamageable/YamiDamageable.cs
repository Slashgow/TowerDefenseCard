using System;

public class YamiDamageable :BaseDamageable
{

    public static event Action OnYamiDie;
    public override void Die()
    {
        base.Die();
        OnYamiDie?.Invoke();

        Destroy(this.gameObject, 5f);

    }
}
