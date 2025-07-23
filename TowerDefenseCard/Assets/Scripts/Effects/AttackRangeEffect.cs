public class AttackRangeEffect : RangeEffect
{
    private IDamageor damageorSource;
    private void Awake()
    {
        damageorSource = GetComponent<IDamageor>();
        range = damageorSource.AttackRange;
    }
}
