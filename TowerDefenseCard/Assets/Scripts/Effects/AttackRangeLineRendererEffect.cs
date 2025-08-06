public class AttackRangeLineRendererEffect : LineRendererRangeEffect
{
    private IDamageor damageorSource;
    private void Awake()
    {
        damageorSource = GetComponent<IDamageor>();
        range = damageorSource.AttackRange;
    }
}
