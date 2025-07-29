using UnityEngine;
using System.Collections;

using System;
using SplineMesh;

public class WaveManager : MonoSingleton<WaveManager>, ILoadable, ISavable
{
    [SerializeField] private WaveDataPaths[] waveDataPaths;
    [SerializeField, Range(0f,10f)] private float initialWaveDelay = 5f;

    private int currentEnnemyCount;
    private int currentWaveEnnemyIndex = 0;
    private int currentWaveIndex = 0;
    public int CurrentWaveIndex => currentWaveIndex;

    private bool isWaveActive = false;

    private int indexSortingOrder = 0;

    public event Action<int> OnWaveStart;
    public event Action OnWaveEnd;

    public int NumberOfWaves => waveDataPaths.Length;
    public bool IsAllWavesCompleted => !(currentWaveIndex < waveDataPaths.Length);

    private int amountOfSpawnedEnemies;
    private int amountOfEnemiesInCurrentWave;

    private void OnEnable()
    {
        GameManager.Instance.OnStartCombatMode -= GameManager_OnStartCombatMode;
        GameManager.Instance.OnStartCombatMode += GameManager_OnStartCombatMode;

        ShowOnlyFirstPathVisual();
    }

    private void GameManager_OnStartCombatMode()
    {
        TriggerNextWave();
    }


    private IEnumerator SpawnWave(WaveData waveData)
    {
        isWaveActive = true;
        SetAmountOfEnemiesInCurrentWave();
        for (int j = currentWaveEnnemyIndex; j < waveData.EnnemyWaves.Count; j++)
        {
            currentWaveEnnemyIndex = j;
            WaveData.WaveEnemy enemy = waveData.EnnemyWaves[j];
            
            for (int i = currentEnnemyCount; i < enemy.Count; i++)
            {
                currentEnnemyCount = i;
                if (waveDataPaths[currentWaveIndex].Paths.Length > 0)
                {
                    Spline path = waveDataPaths[currentWaveIndex].Paths[UnityEngine.Random.Range(0, waveDataPaths[currentWaveIndex].Paths.Length)];
                    GameObject newEnemy = Instantiate(enemy.EnemyPrefab, path.GetSampleAtDistance(0f).location, Quaternion.identity);
                    amountOfSpawnedEnemies++;
                    CardUtility.AssignSortingOrderRecursively(newEnemy.transform, indexSortingOrder);
                    newEnemy.GetComponent<AutoCardMovement>().Init(path);
                    yield return new WaitForSeconds(enemy.SpawnInterval);
                }
                indexSortingOrder += 3;
                currentEnnemyCount = 0;
            }
        }
        yield return new WaitUntil(() => AreAllEnemiesDefeated()); // Wait until all enemies are gone
        isWaveActive = false;
        currentWaveIndex++;
        OnWaveEnd?.Invoke();
        ResetWaveParameters();
        ShowNextWaveVisuals();
        HidePreviousWaveVisuals();
    }

    private void ResetWaveParameters()
    {
        currentEnnemyCount = 0;
        currentWaveEnnemyIndex = 0;
        amountOfSpawnedEnemies = 0;
    }

    private bool AreAllEnemiesDefeated()
    {
        return FindObjectsByType<HopCardMovement>(FindObjectsSortMode.None).Length == 0 && 
            amountOfSpawnedEnemies >= amountOfEnemiesInCurrentWave;
    }

    private void SetAmountOfEnemiesInCurrentWave()
    {
        amountOfEnemiesInCurrentWave = 0;
        foreach (var waveEnemy in waveDataPaths[currentWaveIndex].WaveData.EnnemyWaves)
        {
            amountOfEnemiesInCurrentWave += waveEnemy.Count;
        }
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

    private void ShowNextWaveVisuals()
    {
        if (currentWaveIndex >= waveDataPaths.Length)
            return;

        foreach (var path in waveDataPaths[currentWaveIndex].Paths)
        {
            path.gameObject.SetActive(true);
        }
    }

    private void HidePreviousWaveVisuals()
    {
        if (currentWaveIndex == 0)
            return;

        foreach (var path in waveDataPaths[currentWaveIndex - 1].Paths)
        {
            path.gameObject.SetActive(false);
        }
    }

    private void ShowOnlyFirstPathVisual()
    {
        foreach (var path in waveDataPaths[0].Paths)
        {
            path.gameObject.SetActive(true);
        }

        for (int i = 1; i < waveDataPaths.Length; i++)
        {
            WaveDataPaths waveDataPath = waveDataPaths[i];
            foreach (var path in waveDataPath.Paths)
            {
                path.gameObject.SetActive(false);
            }
        }
    }

    public void Load(GameSaveData saveData)
    {
        this.currentWaveIndex = saveData.currentWaveIndex;
        this.currentWaveEnnemyIndex = saveData.currentWaveEnnemyIndex;
        this.currentEnnemyCount = saveData.currentEnnemyCount;
        this.amountOfSpawnedEnemies = saveData.amountOfSpawnedEnemies;
    }

    public void Save(GameSaveData saveData)
    {
        saveData.currentWaveIndex = this.currentWaveIndex;
        saveData.currentWaveEnnemyIndex = this.currentWaveEnnemyIndex;
        saveData.currentEnnemyCount = this.currentEnnemyCount;
        saveData.amountOfSpawnedEnemies = this.amountOfSpawnedEnemies;
    }
}