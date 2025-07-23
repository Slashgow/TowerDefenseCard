public class HealRangeEffect : RangeEffect
{
    private IHealer healer;
    private void Awake()
    {
        healer = GetComponent<IHealer>();
        range = healer.HealRange;
    }
}
