using System;
using System.Collections.Generic;
using UnityEngine;
public class EndWaveSpawner : CardSpawner
{
    [SerializeField] private bool spawnEachWave = true;
    [SerializeField] private List<int> waveIndexToSpawn = new List<int>();

    private int currentIndex = 0;

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

    public void SpawnCardAtEndOfWave(int waveIndex)
    {
        if(spawnEachWave)
            SpawnCard();
        else if(waveIndexToSpawn[currentIndex] < waveIndex && currentIndex < waveIndexToSpawn.Count)
        {
            currentIndex++;
            SpawnCard();
        }
    }
}
