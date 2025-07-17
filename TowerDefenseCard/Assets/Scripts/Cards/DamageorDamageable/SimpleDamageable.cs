public class SimpleDamageable : BaseDamageable
{

    public override void Die()
    {
        base.Die();
        Destroy(this.gameObject);
    }
}
