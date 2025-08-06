public class AttackRangeMeshEffect : MeshRangeEffect
{
    private IDamageor damageorSource;
    private void Awake()
    {
        damageorSource = GetComponent<IDamageor>();
        range = damageorSource.AttackRange;
    }
}
