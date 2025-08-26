public class EndWaveSpawner : CardSpawner
{
    protected override void Start()
    {
        base.Start();

        WaveManager.Instance.OnWaveEnd += SpawnCard;
    }

    private void OnDestroy()
    {
        if (WaveManager.HasInstance)
            WaveManager.Instance.OnWaveEnd -= SpawnCard;
    }
}
