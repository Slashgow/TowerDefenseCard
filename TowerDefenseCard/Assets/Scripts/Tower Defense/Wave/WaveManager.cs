using UnityEngine;
using System.Collections;
using UnityEngine.Splines;
using System;

public class WaveManager : MonoSingleton<WaveManager>
{
    [SerializeField] private WaveDataPaths[] waveDataPaths;
    [SerializeField, Range(0f,10f)] private float initialWaveDelay = 5f; 
    private int currentWaveIndex = 0;
    private bool isWaveActive = false;

    private int indexSortingOrder = 0;

    public event Action<int> OnWaveStart;
    public event Action OnWaveEnd;

    public int NumberOfWaves => waveDataPaths.Length;
    public bool IsAllWavesCompleted => !(currentWaveIndex < waveDataPaths.Length);

    private void OnEnable()
    {
        GameManager.Instance.OnStartCombatMode -= GameManager_OnStartCombatMode;
        GameManager.Instance.OnStartCombatMode += GameManager_OnStartCombatMode;
    }

    private void GameManager_OnStartCombatMode()
    {
        TriggerNextWave();
    }


    private IEnumerator SpawnWave(WaveData waveData)
    {
        isWaveActive = true;
        foreach (var enemy in waveData.EnnemyWaves)
        {
            for (int i = 0; i < enemy.Count; i++)
            {
                if (waveDataPaths[currentWaveIndex].Paths.Length > 0)
                {
                    SplineContainer path = waveDataPaths[currentWaveIndex].Paths[UnityEngine.Random.Range(0, waveDataPaths[currentWaveIndex].Paths.Length)];
                    GameObject newEnemy = Instantiate(enemy.EnemyPrefab, path.EvaluatePosition(0, 0), Quaternion.identity);
                    CardUtility.AssignSortingOrderRecursively(newEnemy.transform, indexSortingOrder);
                    newEnemy.GetComponent<EnemyCardMovement>().Init(path);
                    yield return new WaitForSeconds(enemy.SpawnInterval);
                }
                indexSortingOrder += 3;
            }
        }
        yield return new WaitUntil(() => AreAllEnemiesDefeated()); // Wait until all enemies are gone
        isWaveActive = false;
        currentWaveIndex++;
        OnWaveEnd?.Invoke();
    }

    private bool AreAllEnemiesDefeated()
    {
        return FindObjectsByType<EnemyCardMovement>(FindObjectsSortMode.None).Length == 0;
    }

  
    public void TriggerNextWave()
    {
        if (!isWaveActive && currentWaveIndex < waveDataPaths.Length)
        {
            StopAllCoroutines();
            OnWaveStart?.Invoke(currentWaveIndex + 1);
            StartCoroutine(SpawnWave(waveDataPaths[currentWaveIndex].WaveData));
        }
    }
}