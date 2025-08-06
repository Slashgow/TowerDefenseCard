public class HealRangeEffect : LineRendererRangeEffect
{
    private IHealer healer;
    private void Awake()
    {
        healer = GetComponent<IHealer>();
        range = healer.HealRange;
    }
}
