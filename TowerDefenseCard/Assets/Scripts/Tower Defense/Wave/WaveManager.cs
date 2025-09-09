using UnityEngine;
using System.Collections;
using System;

public class WaveManager : MonoSingleton<WaveManager>, ILoadable, ISavable
{
    [SerializeField] private WaveDataPaths[] waveDataPaths;
    public WaveDataPaths[] WaveDataPaths => waveDataPaths;

    [SerializeField, Range(0f,10f)] private float initialWaveDelay = 5f;

    private int currentEnnemyCount;
    private int currentWaveEnnemyIndex = 0;
    private int currentWaveIndex = 0;
    public int CurrentWaveIndex => currentWaveIndex;

    private bool isWaveActive = false;

    private int indexSortingOrder = 0;

    public event Action<int> OnWaveStart;
    public event Action<int> OnWaveEnd;

    public int NumberOfWaves => waveDataPaths.Length;
    public bool IsAllWavesCompleted => !(currentWaveIndex < waveDataPaths.Length);

    public Vector3 CurrentWaveFirstPathStartPosition => SplineManager.Instance.GetSplineDataByID(waveDataPaths[currentWaveIndex].Paths[0])
        .Spline.GetSampleAtDistance(0f).location;

    private int amountOfSpawnedEnemies;
    private int amountOfEnemiesInCurrentWave;

    private bool playerWasHitThisWave = false;
    public static event Action OnPlayerWasHitThisWave;
    public static event Action OnPlayerWasNotHitThisWave;
    public event Action<CardID> OnSpawnEnnemy;

    private void OnEnable()
    {
        GameManager.Instance.OnStartCombatMode -= GameManager_OnStartCombatMode;
        GameManager.Instance.OnStartCombatMode += GameManager_OnStartCombatMode;

        PlayerHealth.OnPlayerHit += PlayerHealth_OnPlayerHit;
    }

    private void PlayerHealth_OnPlayerHit() => playerWasHitThisWave = true;

    private void Start() => ShowOnlyFirstPathVisual();

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
                    SplineID pathID = waveDataPaths[currentWaveIndex].Paths[UnityEngine.Random.Range(0, waveDataPaths[currentWaveIndex].Paths.Length)];
                    SplineData pathData = SplineManager.Instance.GetSplineDataByID(pathID);
                    GameObject newEnemy = Instantiate(enemy.EnemyPrefab, pathData.Spline.GetSampleAtDistance(0f).location, Quaternion.identity);
                    OnSpawnEnnemy?.Invoke(enemy.EnemyPrefab.GetComponent<Card>().CardData.CardID);
                    amountOfSpawnedEnemies++;
                    CardUtility.AssignSortingOrderRecursively(newEnemy.transform, indexSortingOrder);
                    newEnemy.GetComponent<AutoCardMovement>().Init(pathData);
                    yield return new WaitForSeconds(enemy.SpawnInterval);
                }
                indexSortingOrder += 3;
                currentEnnemyCount = 0;
            }
        }
        yield return new WaitUntil(() => AreAllEnemiesDefeated()); // Wait until all enemies are gone
        isWaveActive = false;
        currentWaveIndex++;
        OnWaveEnd?.Invoke(currentWaveIndex);

        if(!playerWasHitThisWave)
            OnPlayerWasNotHitThisWave?.Invoke();
        else
            OnPlayerWasHitThisWave?.Invoke();

        ResetWaveParameters();
        HidePreviousWaveVisuals();
        ShowNextWaveVisuals();
    }

    private void ResetWaveParameters()
    {
        currentEnnemyCount = 0;
        currentWaveEnnemyIndex = 0;
        amountOfSpawnedEnemies = 0;
        playerWasHitThisWave = false;
    }

    private bool AreAllEnemiesDefeated()
    {
        return FindObjectsByType<AutoCardMovement>(FindObjectsSortMode.None).Length == 0 && 
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

        SplineManager.Instance.ToggleVisuals(waveDataPaths[currentWaveIndex].Paths, true);
    }

    private void HidePreviousWaveVisuals()
    {
        if (currentWaveIndex == 0)
            return;

        SplineManager.Instance.ToggleVisuals(waveDataPaths[currentWaveIndex - 1].Paths, false);
    }

    private void ShowOnlyFirstPathVisual()
    {
        for (int i = 0; i < waveDataPaths.Length; i++)
        {
            WaveDataPaths waveDataPath = waveDataPaths[i];
            SplineManager.Instance.ToggleVisuals(waveDataPaths[i].Paths, false);

        }
        SplineManager.Instance.ToggleVisuals(waveDataPaths[currentWaveIndex].Paths, true);
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