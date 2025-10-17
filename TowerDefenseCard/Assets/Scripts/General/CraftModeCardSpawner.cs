using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public class CraftModeCardSpawner : CardSpawner
{
    [Header("Wave Spawn Settings")]
    [SerializeField] private List<int> guaranteedSpawnWaveIndexes = new List<int>();
    [SerializeField] private List<int> bannedSpawnWaveIndexes = new List<int>();
    [SerializeField] private bool allowMultipleSpawnsPerWave = false;
    [SerializeField, Range(0f, 1f)] private float oddWaveSpawnProbability = 0.5f;
    [SerializeField, Range(0f, 1f)] private float evenWaveSpawnProbability = 0.5f;

    [Header("Warning Settings")]
    [SerializeField] private GameObject warningPrefab;
    [SerializeField] private float delayBeforeCardSpawn = 2f;

    private bool isCraftMode = false;
    private bool hasSpawnedThisWave = false;
    private GameObject currentWarningInstance;
    private Timer spawnTimer;

    protected override void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStartCraftMode += HandleCraftModeStart;
            GameManager.Instance.OnEndCraftMode += HandleCraftModeEnd;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStartCraftMode -= HandleCraftModeStart;
            GameManager.Instance.OnEndCraftMode -= HandleCraftModeEnd;
        }

        CleanTimerAndPrefabs();
    }

    private void CleanTimerAndPrefabs()
    {
        if (spawnTimer != null)
        {
            spawnTimer.Cancel();
            spawnTimer = null;
        }

        if (currentWarningInstance != null)
        {
            Destroy(currentWarningInstance);
        }
    }

    private void HandleCraftModeStart()
    {
        isCraftMode = true;
        hasSpawnedThisWave = false;

        TrySpawnCard();
    }

    private void HandleCraftModeEnd()
    {
        isCraftMode = false;

        CleanTimerAndPrefabs();
    }

    private void TrySpawnCard()
    {
        if (!isCraftMode)
            return;

        if (hasSpawnedThisWave && !allowMultipleSpawnsPerWave)
            return;

        int currentWaveIndex = WaveManager.Instance.CurrentWaveIndex;

        if (guaranteedSpawnWaveIndexes.Contains(currentWaveIndex))
        {
            SpawnWithWarning();
            hasSpawnedThisWave = true;
            return;
        }

        if(bannedSpawnWaveIndexes.Contains(currentWaveIndex))
            return;

        float spawnChance = (currentWaveIndex % 2 == 0) ? evenWaveSpawnProbability : oddWaveSpawnProbability;

        if (Random.value <= spawnChance)
        {
            SpawnWithWarning();
            hasSpawnedThisWave = true;
        }
    }

    private void SpawnWithWarning()
    {
        if (warningPrefab != null)
        {
            currentWarningInstance = Instantiate(warningPrefab, spawnPoint.position, Quaternion.identity);
        }

        spawnTimer = Timer.Register(delayBeforeCardSpawn, onComplete:() =>
        {
            if (currentWarningInstance != null)
            {
                Destroy(currentWarningInstance);
                currentWarningInstance = null;
            }

            SpawnCard();
        });
    }
}
