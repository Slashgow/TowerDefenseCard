using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public class EliteAreaDamageor : SimpleDamageor
{
    [Header("Elite Area Ability")]
    [SerializeField, Range(0f,30f)] private float detectionRangeDamageable = 15f;
    [SerializeField] private GameObject dotAreaPrefab;
    [SerializeField, Range(0f,6f)] private float warningDuration = 2f;
    [SerializeField, Range(0f,2f)] private float dotAreaTickRate = 0.5f;
    [SerializeField, Range(0f,15f)] private float abilityInterval = 10f;
    [SerializeField, Range(0,6)] private int maxAreasPerCast = 3;

    [Header("Visual Settings")]
    [SerializeField] private Color warningColor = new Color(1f, 0.5f, 0f, 0.5f);
    [SerializeField] private Color activeColor = new Color(1f, 0f, 0f, 0.7f);
    [SerializeField, Range(0f,5f)] private float areaRadius = 2f;

    private Timer abilityTimer;
    private List<GameObject> activeAreas = new List<GameObject>();

    private void Start() => StartAbilityTimer();
    private void StartAbilityTimer() => abilityTimer = Timer.Register(abilityInterval, onComplete: () => TrySpawnDotAreas(), isLooped: true);

    private void TrySpawnDotAreas()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRangeDamageable, enemyLayer);
        List<Vector3> validPositions = new List<Vector3>();

        foreach (Collider2D hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null && hit.gameObject != this.gameObject)
            {
                validPositions.Add(hit.transform.position);
            }
        }

        if (validPositions.Count == 0)
            return;

        int areasToSpawn = Mathf.Min(validPositions.Count, maxAreasPerCast);

        for (int i = 0; i < areasToSpawn; i++)
        {
            SpawnDotAreaSequence(validPositions[i]);
        }
    }

    private void SpawnDotAreaSequence(Vector3 position)
    {
        GameObject warningArea = CreateWarningArea(position);
        activeAreas.Add(warningArea);

        Timer.Register(warningDuration,
            onComplete: () =>
            {
                if (warningArea == null)
                    return;

                if(warningArea.TryGetComponent(out DotArea dotArea))
                {
                    dotArea.Activate(DoTDuration, DoT, dotAreaTickRate, activeColor);
                    Timer.Register(DoTDuration, onComplete: () =>
                    {
                       if (warningArea != null)
                       {
                           activeAreas.Remove(warningArea);
                           Destroy(warningArea);
                       }
                    });
                }
            }
        );
    }

    private GameObject CreateWarningArea(Vector3 position)
    {
        GameObject areaObject = Instantiate(dotAreaPrefab, position, Quaternion.identity);

        if(areaObject.TryGetComponent(out DotArea dotArea))
        {
            dotArea.Initialize(areaRadius, warningColor, enemyLayer);
        }

        return areaObject;
    }

    
    private void OnDestroy()
    {
        abilityTimer?.Cancel();

        foreach (GameObject area in activeAreas)
        {
            if (area != null)
                Destroy(area);
        }
        activeAreas.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRangeDamageable);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, areaRadius);
    }
}
