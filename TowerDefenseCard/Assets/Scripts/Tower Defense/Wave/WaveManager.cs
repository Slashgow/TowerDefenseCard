using UnityEngine;
using System.Collections;
using UnityEngine.Splines;

public class WaveManager : MonoSingleton<WaveManager>
{
    [SerializeField] private WaveData[] waveDataArray; 
    [SerializeField] private SplineContainer[] paths; 
    [SerializeField, Range(0f,10f)] private float initialWaveDelay = 5f; 
    private int currentWaveIndex = 0;
    private bool isWaveActive = false;

    private int indexSortingOrder = 0;
     
    private void Start()
    {
        TriggerNextWave();
    }
    private IEnumerator StartWaveSequence()
    {
        indexSortingOrder = 0;

        yield return new WaitForSeconds(initialWaveDelay);

        while (currentWaveIndex < waveDataArray.Length)
        {
            yield return StartCoroutine(SpawnWave(waveDataArray[currentWaveIndex]));
            currentWaveIndex++;
            if (currentWaveIndex < waveDataArray.Length)
            {
                yield return new WaitForSeconds(waveDataArray[currentWaveIndex].WaveDelay);
            }
        }
    }

    private IEnumerator SpawnWave(WaveData waveData)
    {
        isWaveActive = true;
        foreach (var enemy in waveData.EnnemyWaves)
        {
            for (int i = 0; i < enemy.Count; i++)
            {
                if (paths.Length > 0)
                {
                    SplineContainer path = paths[Random.Range(0, paths.Length)];
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
    }

    private bool AreAllEnemiesDefeated()
    {
        return FindObjectsByType<EnemyCardMovement>(FindObjectsSortMode.None).Length == 0;
    }

  
    public void TriggerNextWave()
    {
        if (!isWaveActive && currentWaveIndex < waveDataArray.Length)
        {
            StopAllCoroutines();
            StartCoroutine(StartWaveSequence());
        }
    }
}