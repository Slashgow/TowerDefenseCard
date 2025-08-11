using System;
using UnityEngine;
using UnityTimer;

public class SimpleFearor : MonoBehaviour, IFearor
{
    [Header("Fear Settings")]
    [SerializeField] private bool autoSyncWithDamageor = true;
    [SerializeField, Range(0f,10f)] private float fearCooldown = 2f;
    [SerializeField, Range(0f, 10f)] private float fearDuration = 3f;
    [SerializeField, Range(0f, 100f)] private float fearChance = 25f;
    [SerializeField] private bool useFearRange = false;
    [SerializeField, Range(0f, 15f)] private float fearRange = 8f;
    [SerializeField, Range(0f, 360f)] private float fearFieldOfView = 90f;
    [SerializeField] private bool canCauseFear = true;
    [SerializeField] private Vector3 fearEffectSpawnOffset = Vector3.up;
    [SerializeField] private GameObject fearEffectPrefab;
    [SerializeField] private LayerMask fearableLayer = -1;

    private Timer fearTimer;
    private IDamageor damageor;

    public float FearCooldown => fearCooldown;
    public float FearDuration => fearDuration;
    public float FearChance => fearChance;
    public float FearRange => useFearRange ? fearRange : damageor.AttackRange;
    public float FearFieldOfView => fearFieldOfView;
    public bool CanCauseFear => canCauseFear;
    private void Awake()
    {
        if (useFearRange && !autoSyncWithDamageor)
            return;

        damageor = GetComponent<IDamageor>();

        if (damageor == null)
            Debug.LogWarning("no stun range use and not damageor found");
    }


    private void Start()
    {
        if (canCauseFear && !autoSyncWithDamageor)
            fearTimer = Timer.Register(fearCooldown, FearNearbyTargets, isLooped: true);

        else if (canCauseFear && autoSyncWithDamageor)
            damageor.OnAttackEvent += Damageor_OnAttackEvent;
    }

    private void Damageor_OnAttackEvent(BaseDamageable damageable)
    {
        Debug.Log($"Try to fear {damageable}");
        if (damageable.TryGetComponent(out IFearable fearable))
            ApplyFearToTarget(fearable);
    }

    public void FearNearbyTargets()
    {
        if (!canCauseFear) 
            return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, FearRange, fearableLayer);

        foreach (var hit in hits)
        {
            IFearable fearable = hit.GetComponent<IFearable>();
            if (fearable != null && !fearable.IsFeared)
            {
                if (IsTargetInFieldOfView(hit.transform))
                    ApplyFearToTarget(fearable);
            }
        }
    }

    private bool IsTargetInFieldOfView(Transform target)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Vector3 forward = -transform.up; 
        float dotProduct = Vector3.Dot(forward, directionToTarget);
        float fieldOfViewThreshold = Mathf.Cos((fearFieldOfView * 0.5f) * Mathf.Deg2Rad);
        return dotProduct >= fieldOfViewThreshold;
    }

    public void ApplyFearToTarget(IFearable target)
    {
        if (!canCauseFear || target == null) 
            return;

        float randomValue = UnityEngine.Random.Range(0f, 100f);
        if (randomValue > fearChance) 
            return;

        target.ApplyFear(fearDuration);

        if (fearEffectPrefab != null && target is MonoBehaviour targetMono)
        {
            GameObject fearEffectInstance = Instantiate(fearEffectPrefab,targetMono.transform);
            fearEffectInstance.transform.localPosition = fearEffectSpawnOffset;
            fearEffectInstance.transform.localRotation = Quaternion.identity;
            Timer.Register(fearDuration, onComplete: () => Destroy(fearEffectInstance));
        }

        string targetName = target is MonoBehaviour mono ? mono.gameObject.name : "Unknown";
        Debug.Log($"{gameObject.name} caused fear on {targetName} for {fearDuration} seconds");
    }

    public void SetCanCauseFear(bool enabled)
    {
        canCauseFear = enabled;

        if (enabled && (fearTimer == null || fearTimer.isCompleted))
        {
            fearTimer = Timer.Register(fearCooldown, FearNearbyTargets, isLooped: true);
        }
        else if (!enabled && fearTimer != null)
        {
            fearTimer.Cancel();
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!canCauseFear) return;

        // Draw fear range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, FearRange);

        if(!useFearRange)
            return;

        // Draw field of view
        Gizmos.color = Color.yellow;
        Vector3 forward = -transform.up;
        float halfFOV = fearFieldOfView * 0.5f;

        // Calculate the two edges of the field of view
        Vector3 leftBoundary = Quaternion.Euler(0, 0, halfFOV) * forward * FearRange;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, -halfFOV) * forward * FearRange;

        // Draw the field of view cone
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        // Draw arc
        Vector3 previousPoint = transform.position + leftBoundary;
        for (int i = 1; i <= 20; i++)
        {
            float angle = Mathf.Lerp(halfFOV, -halfFOV, i / 20f);
            Vector3 point = transform.position + Quaternion.Euler(0, 0, angle) * forward * FearRange;
            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }
    }
#endif

    private void OnDestroy()
    {
        fearTimer?.Cancel();
    }
}