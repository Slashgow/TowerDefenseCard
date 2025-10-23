using UnityEngine;
using System;

[Serializable]
public class EndlessWaveConfig
{
    [Header("Enemy Pool")]
    [Tooltip("Pool of enemy prefabs to randomly spawn from")]
    public GameObject[] enemyPrefabPool;

    [Header("Path Pool")]
    [Tooltip("Pool of path IDs to randomly choose from")]
    public SplineID[] pathPool;

    [Range(1, 10)]
    [Tooltip("Maximum number of different paths to use per wave")]
    public int maxPathsPerWave = 3;

    [Header("Spawn Settings")]
    [Range(0, 40), Tooltip("Total number of enemies to spawn in this endless wave")]
    public int totalEnemyCount = 20;

    [Range(0.1f, 5f)]
    [Tooltip("Time between each enemy spawn")]
    public float spawnInterval = 1f;

    [Header("Scaling (Optional)")]
    [Range(0,15), Tooltip("Increase enemy count each wave by this amount")]
    public int enemyCountIncreasePerWave = 2;

    [Range(0f, 2f), Tooltip("Decrease spawn interval each wave by this amount (min 0.1s)")]
    public float spawnIntervalDecreasePerWave = 0.05f;
}
