using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityTimer;

public class Cloner : MonoBehaviour
{
    [Header("Clone Settings")]
    [Tooltip("Prefab to use for creating clones")]
    [SerializeField] private GameObject clonePrefab;

    [Tooltip("Number of clones to spawn each time")]
    [SerializeField, Range(1, 10)] private int clonesPerSpawn = 3;

    [Tooltip("Time between clone spawning attempts (in seconds)")]
    [SerializeField, Range(1f, 15f)] private float cloneCooldown = 5f;

    [Tooltip("How long clones stay alive (in seconds)")]
    [SerializeField, Range(1f, 30f)] private float cloneLifetime = 10f;

    [Tooltip("Maximum number of clones that can exist at once")]
    [SerializeField, Range(1, 20)] private int maxActiveClones = 6;

    [Tooltip("Probability of spawning clones when cooldown is ready (0-1)")]
    [SerializeField, Range(0f, 1f)] private float cloneProbability = 0.8f;

    [Header("Clone Spawn Positioning")]
    [Tooltip("Minimum distance from original when spawning clones")]
    [SerializeField, Range(1f, 10f)] private float minSpawnDistance = 2f;

    [Tooltip("Maximum distance from original when spawning clones")]
    [SerializeField, Range(2f, 15f)] private float maxSpawnDistance = 5f;

    [SerializeField, Range(0f,5f)] private float timeToReachSpline = 2f;
    [SerializeField, Range(0, 5f)] private float timeVarianceToReachSpline = 1f;

    private Timer cloneCooldownTimer;
    private List<GameObject> activeClones = new List<GameObject>();
    private bool canSpawnClones = false;
    private bool isActive = false;

    public int ActiveCloneCount => activeClones.Count;
    public bool CanSpawnClones => canSpawnClones;
    public bool IsActive => isActive;
    public float CloneCooldownRemaining => cloneCooldownTimer != null ? cloneCooldownTimer.GetTimeRemaining() : 0f;

    private void Start()
    {
        InitializeCloneSystem();
    }

    private void InitializeCloneSystem()
    {
        if (clonePrefab == null)
        {
            Debug.LogWarning($"Clone prefab not assigned to {gameObject.name}");
            return;
        }

        cloneCooldownTimer = Timer.Register(cloneCooldown, () =>
        {
            isActive = true;
            canSpawnClones = true;
            RestartCooldownTimer();
        });
    }

    private void RestartCooldownTimer()
    {
        if (isActive)
        {
            cloneCooldownTimer = Timer.Register(cloneCooldown, () =>
            {
                canSpawnClones = true;
                RestartCooldownTimer();
            });
        }
    }

    private void Update()
    {
        if (!isActive)
            return;

        CleanupDestroyedClones();

        if (canSpawnClones && ShouldSpawnClones())
            SpawnClones();
    }

    private void CleanupDestroyedClones() => activeClones.RemoveAll(clone => clone == null);
    private bool ShouldSpawnClones() => activeClones.Count < maxActiveClones && Random.Range(0f, 1f) <= cloneProbability;

    private void SpawnClones()
    {
        canSpawnClones = false;

        int clonesToSpawn = Mathf.Min(clonesPerSpawn, maxActiveClones - activeClones.Count);

        for (int i = 0; i < clonesToSpawn; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject clone = CreateClone(spawnPosition);

            if (clone != null)
            {
                activeClones.Add(clone);
                SetupClone(clone);
            }
        }

        OnClonesSpawned(clonesToSpawn);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 basePosition = transform.position;

        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);

        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * distance,
            Mathf.Sin(angle) * distance,
            0f
        );

        return basePosition + offset;
    }

    private GameObject CreateClone(Vector3 position)
    {
        GameObject clone = Instantiate(clonePrefab, position, transform.rotation);
        return clone;
    }

    private void SetupClone(GameObject clone)
    {
        if(clone.TryGetComponent(out AutoCardMovement autoMovement))
        {
            AutoCardMovement autoCardMovementCloner = GetComponent<AutoCardMovement>();
            SplineData splineData = SplineManager.Instance.GetSplineDataByID(autoCardMovementCloner.SplineID);

            autoMovement.Load(autoCardMovementCloner.Save());
            autoMovement.StopMoving();
            //autoMovement.Init(splineData);

            float timeToReachSpline = this.timeToReachSpline + Random.Range(-timeVarianceToReachSpline, timeVarianceToReachSpline);
            clone.transform.DOMove(splineData.Spline.GetSampleAtDistance(autoMovement.CurrentDistance).location, timeToReachSpline)
                .OnComplete(() => autoMovement.StartMoving()); 
        }
     
        clone.GetComponent<Clonnable>().Initialize(this, cloneLifetime);
        OnCloneCreated(clone);
    }

    public void DestroyClone(GameObject clone)
    {
        if (clone != null)
        {
            activeClones.Remove(clone);
            OnCloneDestroyed(clone);
            Destroy(clone);
        }
    }

    public void DestroyAllClones()
    {
        for (int i = activeClones.Count - 1; i >= 0; i--)
        {
            if (activeClones[i] != null)
            {
                Destroy(activeClones[i]);
            }
        }
        activeClones.Clear();
    }

    public void StartCloning()
    {
        isActive = true;
        canSpawnClones = true;

        if (cloneCooldownTimer == null || cloneCooldownTimer.isDone)
        {
            InitializeCloneSystem();
        }
    }

    public void StopCloning()
    {
        isActive = false;
        canSpawnClones = false;

        if (cloneCooldownTimer != null && !cloneCooldownTimer.isDone)
        {
            cloneCooldownTimer.Cancel();
        }
    }

    public void ForceSpawnClones()
    {
        if (isActive)
        {
            canSpawnClones = true;
            SpawnClones();
        }
    }
  

    protected virtual void OnClonesSpawned(int count)
    {
        Debug.Log($"Spawned {count} ninja clones!");
    }

    protected virtual void OnCloneCreated(GameObject clone)
    {
    }

    protected virtual void OnCloneDestroyed(GameObject clone)
    {
    }

    private void OnDestroy()
    {
        if (cloneCooldownTimer != null && !cloneCooldownTimer.isDone)
            cloneCooldownTimer.Cancel();
  
        DestroyAllClones();
    }
}
