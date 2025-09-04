public class EndWaveSpawner : CardSpawner
{
    protected override void Start()
    {
        base.Start();

        WaveManager.Instance.OnWaveEnd += SpawnCardAtEndOfWave;
    }

    private void OnDestroy()
    {
        if (WaveManager.HasInstance)
            WaveManager.Instance.OnWaveEnd -= SpawnCardAtEndOfWave;
    }

    public void SpawnCardAtEndOfWave(int waveIndex) => SpawnCard();
}
