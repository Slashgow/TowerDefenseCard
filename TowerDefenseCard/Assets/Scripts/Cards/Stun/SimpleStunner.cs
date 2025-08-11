using UnityEngine;
using UnityTimer;

public class SimpleStunner : MonoBehaviour, IStunner
{
    [Header("Stun Settings")]
    [SerializeField] private bool autoSyncWithDamageor = false;
    [SerializeField, Range(0f,10f)] private float stunCooldown = 5f;
    [SerializeField, Range(0f, 10f)] private float stunDuration = 3f;
    [SerializeField, Range(0f, 100f)] private float stunChance = 25f;
    [SerializeField] private bool useStunRange = true;
    [SerializeField, Range(0f, 15f)] private float stunRange = 8f;
    [SerializeField, Range(0f, 360f)] private float stunFieldOfView = 90f;
    [SerializeField] private bool canCauseStun = true;
    [SerializeField] private Vector3 stunEffectSpawnOffset;
    [SerializeField] private GameObject stunEffectPrefab;
    [SerializeField] private LayerMask stunnableLayer = -1;

    private Timer stunTimer;
    private IDamageor damageor;

    public float StunCooldown => stunCooldown;
    public float StunDuration => stunDuration;
    public float StunChance => stunChance;
    public float StunRange => useStunRange ? stunRange : damageor.AttackRange;
    public float StunFieldOfView => stunFieldOfView;
    public bool CanCauseStun => canCauseStun;

    private void Awake()
    {
        if (useStunRange && !autoSyncWithDamageor)
            return;

        damageor = GetComponent<IDamageor>();

        if (damageor == null)
            Debug.LogWarning("no stun range use and not damageor found");
    }

    private void Start()
    {
        if (canCauseStun && !autoSyncWithDamageor)
            stunTimer = Timer.Register(stunCooldown, onComplete: StunNearbyTargets, isLooped: true);

        else if (canCauseStun && autoSyncWithDamageor)
            damageor.OnAttackEvent += Damageor_OnAttackEvent;
    }
    private void Damageor_OnAttackEvent(BaseDamageable damageable)
    {
        if(damageable.TryGetComponent(out IStunnable stunnable))
            ApplyStunToTarget(stunnable);
    }

    public void StunNearbyTargets()
    {
        if (!canCauseStun)
            return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, StunRange, stunnableLayer);

        foreach (var hit in hits)
        {
            IStunnable stunnable = hit.GetComponent<IStunnable>();
            if (stunnable != null && !stunnable.IsStunned)
            {
                if (useStunRange && IsTargetInFieldOfView(hit.transform))
                    ApplyStunToTarget(stunnable);
                else if (!useStunRange)
                    ApplyStunToTarget(stunnable);
            }
        }
    }

    private bool IsTargetInFieldOfView(Transform target)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Vector3 forward = -transform.up;
        float dotProduct = Vector3.Dot(forward, directionToTarget);
        float fieldOfViewThreshold = Mathf.Cos((stunFieldOfView * 0.5f) * Mathf.Deg2Rad);
        return dotProduct >= fieldOfViewThreshold;
    }

    public void ApplyStunToTarget(IStunnable target)
    {
        if (!canCauseStun || target == null)
            return;

        float randomValue = UnityEngine.Random.Range(0f, 100f);
        if (randomValue > stunChance)
            return;

        target.ApplyStun(stunDuration);

        if (stunEffectPrefab != null && target is MonoBehaviour targetMono)
        {
            GameObject stunEffectInstance = Instantiate(stunEffectPrefab, targetMono.transform);
            stunEffectInstance.transform.localRotation = Quaternion.identity;
            stunEffectInstance.transform.localPosition = stunEffectSpawnOffset;
            Timer.Register(stunDuration, onComplete: () =>  Destroy(stunEffectInstance));
        }

        string targetName = target is MonoBehaviour mono ? mono.gameObject.name : "Unknown";
        Debug.Log($"{gameObject.name} caused stun on {targetName} for {stunDuration} seconds");
    }

    public void SetCanCauseStun(bool enabled)
    {
        canCauseStun = enabled;

        if (enabled && (stunTimer == null || stunTimer.isCompleted))
        {
            stunTimer = Timer.Register(stunCooldown, StunNearbyTargets, isLooped: true);
        }
        else if (!enabled && stunTimer != null)
        {
            stunTimer.Cancel();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!canCauseStun) return;

        // Draw stun range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stunRange);

        // Draw field of view
        Gizmos.color = Color.cyan;
        Vector3 forward = -transform.up;
        float halfFOV = stunFieldOfView * 0.5f;

        // Calculate the two edges of the field of view
        Vector3 leftBoundary = Quaternion.Euler(0, 0, halfFOV) * forward * stunRange;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, -halfFOV) * forward * stunRange;

        // Draw the field of view cone
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        // Draw arc
        Vector3 previousPoint = transform.position + leftBoundary;
        for (int i = 1; i <= 20; i++)
        {
            float angle = Mathf.Lerp(halfFOV, -halfFOV, i / 20f);
            Vector3 point = transform.position + Quaternion.Euler(0, 0, angle) * forward * stunRange;
            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }
    }
#endif

    private void OnDestroy()
    {
        stunTimer?.Cancel();

        if (canCauseStun && autoSyncWithDamageor)
            damageor.OnAttackEvent -= Damageor_OnAttackEvent;
    }
}