using System;
using UnityEngine;

[Serializable]
public class RandomEventSettings
{
    [Header("Spawn Configuration")]
    [SerializeField, Range(10f, 300f)] private float minSpawnInterval = 30f;
    [SerializeField, Range(10f, 300f)] private float maxSpawnInterval = 120f;
    [SerializeField, Range(0, 20)] private int minimumWaveToStart = 2;
    [SerializeField, Range(0f, 1f)] private float spawnProbability = 0.7f;
    [SerializeField, Range(0f, 100f)] private float maxTimeRemainingBeforeWaveStartAllowed = 60f;

    [Header("Enemy Configuration")]
    [SerializeField] private Ennemy[] enemyPrefabs;
    [SerializeField, Range(1f, 10f)] private float spawnRadius = 5f;
    [SerializeField, Range(0.5f, 3f)] private float spawnRadiusVariation = 1f;

    [Header("Target Selection")]
    [SerializeField] private bool canTargetDefenseCards = true;
    [SerializeField] private bool canTargetWorkerCards = false;
    [SerializeField] private bool canTargetCurrencyCards = false;
    [SerializeField] private bool canTargetShopCards = false;
    [SerializeField] private bool canTargetRessourceCards = true;
    [SerializeField] private bool canTargetRessourceGeneratorCards = true;
    [SerializeField] private bool canTargetCardExploitation = false;

    [Header("Spawn Bounds")]
    [SerializeField] private Transform spawnBoundsCenter;
    [SerializeField, Range(5f, 50f)] private float maxSpawnDistance = 20f;

    public float MinSpawnInterval => minSpawnInterval;
    public float MaxSpawnInterval => maxSpawnInterval;
    public int MinimumWaveToStart => minimumWaveToStart;
    public float SpawnProbability => spawnProbability;
    public float MaxTimeRemainingBeforeWaveStartAllowed => maxTimeRemainingBeforeWaveStartAllowed;
    public Ennemy[] EnemyPrefabs => enemyPrefabs;
    public float SpawnRadius => spawnRadius;
    public float SpawnRadiusVariation => spawnRadiusVariation;

    public bool CanTargetDefenseCards => canTargetDefenseCards;
    public bool CanTargetWorkerCards => canTargetWorkerCards;
    public bool CanTargetCurrencyCards => canTargetCurrencyCards;
    public bool CanTargetShopCards => canTargetShopCards;
    public bool CanTargetRessourceCards => canTargetRessourceCards;
    public bool CanTargetCardExploitation => canTargetCardExploitation;
    public bool CanTargetRessourceGeneratorCards => canTargetRessourceGeneratorCards;

    public Transform SpawnBoundsCenter => spawnBoundsCenter;
    public float MaxSpawnDistance => maxSpawnDistance;

    public bool IsValid => enemyPrefabs != null && enemyPrefabs.Length > 0;
}
