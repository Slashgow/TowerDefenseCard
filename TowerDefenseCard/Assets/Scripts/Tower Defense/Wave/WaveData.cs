using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "NewWaveData", menuName = "William/WaveData", order = 1)]
public class WaveData : ScriptableObject
{
    [System.Serializable]
    public class WaveEnemy
    {
        [SerializeField] private GameObject enemyPrefab;
        public GameObject EnemyPrefab => enemyPrefab;

        [SerializeField, Range(0,50)] private int count;
        public int Count => count;

        [SerializeField, Range(0f,10f)] private float spawnInterval;
        public float SpawnInterval => spawnInterval;
    }

    [SerializeField] private List<WaveEnemy> ennemyWaves = new List<WaveEnemy>();
    public List<WaveEnemy> EnnemyWaves => ennemyWaves;

    [Tooltip("Optional: Set to 0 to calculate automatically based on spawn intervals and counts.")]
    [SerializeField, Range(0f,300f)] private float waveDelay = 0f; 
    public float WaveDelay
    {
        get 
        { 
            if( waveDelay > 0f)
                return waveDelay;
            
            return CalculateWaveDelay();    
        }
    }

    [Tooltip("delay buffer in s after a wave is finished before triggering next wave")]
    [SerializeField, Range(1f, 15f)] private float delayBuffer = 5f; 

    private float CalculateWaveDelay()
    {
        if (this.ennemyWaves == null || this.ennemyWaves.Count == 0) return 0f;

        float maxSpawnTime = this.ennemyWaves.Max(enemy => (enemy.Count - 1) * enemy.SpawnInterval); 
        return maxSpawnTime + this.delayBuffer; // Add buffer time
    }
}