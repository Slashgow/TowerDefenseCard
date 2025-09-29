using UnityEngine;
using UnityTimer;

public class SimpleSlower : MonoBehaviour, ISlower
{
    [Header("Slow Settings")]
    [SerializeField] private bool autoSyncWithDamageor = false;
    [SerializeField, Range(0f, 10f)] private float slowCooldown = 5f;
    [SerializeField, Range(0f, 10f)] private float slowDuration = 3f;
    [SerializeField, Range(0f, 1f)] private float slowMultiplier = 0.5f;
    [SerializeField, Range(0f, 100f)] private float slowChance = 25f;
    [SerializeField] private bool useSlowRange = true;
    [SerializeField, Range(0f, 15f)] private float slowRange = 8f;
    [SerializeField, Range(0f, 360f)] private float slowFieldOfView = 90f;
    [SerializeField] private bool canCauseSlow = true;
    [SerializeField] private Vector3 slowEffectSpawnOffset;
    [SerializeField] private GameObject slowEffectPrefab;
    [SerializeField] private LayerMask slowableLayer = -1;

    private Timer slowTimer;
    private IDamageor damageor;

    public float SlowCooldown => slowCooldown;
    public float SlowDuration => slowDuration;
    public float SlowMultiplier => slowMultiplier;
    public float SlowChance => slowChance;
    public float SlowRange => useSlowRange ? slowRange : damageor.AttackRange;
    public float SlowFieldOfView => slowFieldOfView;
    public bool CanCauseSlow => canCauseSlow;

    private void Awake()
    {
        if (useSlowRange && !autoSyncWithDamageor)
            return;

        damageor = GetComponent<IDamageor>();

        if (damageor == null)
            Debug.LogWarning("no slow range use and not damageor found");
    }

    private void Start()
    {
        if (canCauseSlow && !autoSyncWithDamageor)
            slowTimer = Timer.Register(slowCooldown, onComplete: SlowNearbyTargets, isLooped: true);

        else if (canCauseSlow && autoSyncWithDamageor)
            damageor.OnAttackEvent += Damageor_OnAttackEvent;
    }

    private void Damageor_OnAttackEvent(BaseDamageable damageable)
    {
        if (damageable.TryGetComponent(out ISlowable slowable))
            ApplySlowToTarget(slowable);
    }

    public void SlowNearbyTargets()
    {
        if (!canCauseSlow)
            return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, SlowRange, slowableLayer);
        foreach (var hit in hits)
        {
            ISlowable slowable = hit.GetComponent<ISlowable>();
            if (slowable != null && !slowable.IsSlowed)
            {
                if (useSlowRange && IsTargetInFieldOfView(hit.transform))
                    ApplySlowToTarget(slowable);
                else if (!useSlowRange)
                    ApplySlowToTarget(slowable);
            }
        }
    }

    private bool IsTargetInFieldOfView(Transform target)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Vector3 forward = -transform.up;
        float dotProduct = Vector3.Dot(forward, directionToTarget);
        float fieldOfViewThreshold = Mathf.Cos((slowFieldOfView * 0.5f) * Mathf.Deg2Rad);
        return dotProduct >= fieldOfViewThreshold;
    }

    public void ApplySlowToTarget(ISlowable target)
    {
        if (!canCauseSlow || target == null)
            return;

        float randomValue = UnityEngine.Random.Range(0f, 100f);
        if (randomValue > slowChance)
            return;

        target.ApplySlow(slowDuration, slowMultiplier);

        if (slowEffectPrefab != null && target is MonoBehaviour targetMono)
        {
            GameObject slowEffectInstance = Instantiate(slowEffectPrefab, targetMono.transform);
            slowEffectInstance.transform.localRotation = Quaternion.identity;
            slowEffectInstance.transform.localPosition = slowEffectSpawnOffset;
            Timer.Register(slowDuration, onComplete: () => Destroy(slowEffectInstance));
        }

        string targetName = target is MonoBehaviour mono ? mono.gameObject.name : "Unknown";
        Debug.Log($"{gameObject.name} caused slow on {targetName} for {slowDuration} seconds with {slowMultiplier}x multiplier");
    }

    public void SetCanCauseSlow(bool enabled)
    {
        canCauseSlow = enabled;

        if (enabled && (slowTimer == null || slowTimer.isCompleted))
        {
            slowTimer = Timer.Register(slowCooldown, onComplete: SlowNearbyTargets, isLooped: true);
        }
        else if (!enabled && slowTimer != null)
        {
            slowTimer.Cancel();
            slowTimer = null;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!canCauseSlow) return;

        // Draw slow range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, slowRange);

        // Draw field of view
        Gizmos.color = Color.gray;
        Vector3 forward = -transform.up;
        float halfFOV = slowFieldOfView * 0.5f;

        // Calculate the two edges of the field of view
        Vector3 leftBoundary = Quaternion.Euler(0, 0, halfFOV) * forward * slowRange;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, -halfFOV) * forward * slowRange;

        // Draw the field of view cone
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        // Draw arc
        Vector3 previousPoint = transform.position + leftBoundary;
        for (int i = 1; i <= 20; i++)
        {
            float angle = Mathf.Lerp(halfFOV, -halfFOV, i / 20f);
            Vector3 point = transform.position + Quaternion.Euler(0, 0, angle) * forward * slowRange;
            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }
    }
#endif

    private void OnDestroy()
    {
        slowTimer?.Cancel();

        if (canCauseSlow && autoSyncWithDamageor)
            damageor.OnAttackEvent -= Damageor_OnAttackEvent;
    }
}