public class HealRangeMeshEffect : MeshRangeEffect
{
    private IHealer healer;
    private void Awake()
    {
        healer = GetComponent<IHealer>();
        range = healer.HealRange;
    }
}
