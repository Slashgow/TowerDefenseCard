using System;
using UnityEngine;
using UnityTimer;

public class SimplePushbacker : MonoBehaviour, IPushbacker
{
    [Header("Pushback Settings")]
    [SerializeField] private bool autoSyncWithDamageor = false;
    [SerializeField, Range(0f, 10f)] private float pushbackCooldown = 5f;
    [SerializeField, Range(0f, 10f)] private float pushbackDistance = 2f;
    [SerializeField, Range(0f, 100f)] private float pushbackChance = 50f;
    [SerializeField] private bool usePushbackRange = true;
    [SerializeField, Range(0f, 15f)] private float pushbackRange = 8f;
    [SerializeField, Range(0f, 360f)] private float pushbackFieldOfView = 90f;
    [SerializeField] private bool canCausePushback = true;
    [SerializeField] private GameObject pushbackEffectPrefab;
    [SerializeField] private LayerMask pushbackableLayer = -1;

    private Timer pushbackTimer;
    private IDamageor damageor;

    public float PushbackCooldown => pushbackCooldown;
    public float PushbackDistance => pushbackDistance;
    public float PushbackChance => pushbackChance;
    public float PushbackRange => usePushbackRange ? pushbackRange : damageor.AttackRange;
    public float PushbackFieldOfView => pushbackFieldOfView;
    public bool CanCausePushback => canCausePushback;

    private void Awake()
    {
        if (usePushbackRange)
            return;

        damageor = GetComponent<IDamageor>();

        if (damageor == null)
            Debug.LogWarning("No pushback range use and no damageor found");
    }

    private void Start()
    {
        if (canCausePushback && !autoSyncWithDamageor)
            pushbackTimer = Timer.Register(pushbackCooldown, onComplete: PushbackNearbyTargets, isLooped: true);

        else if (canCausePushback && autoSyncWithDamageor)
            damageor.OnAttackEvent += Damageor_OnAttackEvent;
    }

    private void Damageor_OnAttackEvent(BaseDamageable damageable)
    {
        if (damageable.TryGetComponent(out IPushbackable pushbackable))
        {
            Vector3 direction = (damageable.transform.position - transform.position).normalized;
            ApplyPushbackToTarget(pushbackable, direction);
        }
    }

    public void PushbackNearbyTargets()
    {
        if (!canCausePushback)
            return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, PushbackRange, pushbackableLayer);

        foreach (var hit in hits)
        {
            IPushbackable pushbackable = hit.GetComponent<IPushbackable>();
            if (pushbackable != null && pushbackable.CanBePushedBack)
            {
                Vector3 direction = (hit.transform.position - transform.position).normalized;

                if (usePushbackRange && IsTargetInFieldOfView(hit.transform))
                    ApplyPushbackToTarget(pushbackable, direction);
                else if (!usePushbackRange)
                    ApplyPushbackToTarget(pushbackable, direction);
            }
        }
    }

    private bool IsTargetInFieldOfView(Transform target)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Vector3 forward = -transform.up;
        float dotProduct = Vector3.Dot(forward, directionToTarget);
        float fieldOfViewThreshold = Mathf.Cos((pushbackFieldOfView * 0.5f) * Mathf.Deg2Rad);
        return dotProduct >= fieldOfViewThreshold;
    }

    public void ApplyPushbackToTarget(IPushbackable target, Vector3 direction)
    {
        if (!canCausePushback || target == null)
            return;

        float randomValue = UnityEngine.Random.Range(0f, 100f);
        if (randomValue > pushbackChance)
            return;

        target.ApplyPushback(pushbackDistance, direction);

        if (pushbackEffectPrefab != null && target is MonoBehaviour targetMono)
        {
            Instantiate(pushbackEffectPrefab, targetMono.transform.position, Quaternion.identity);
        }

        string targetName = target is MonoBehaviour mono ? mono.gameObject.name : "Unknown";
        Debug.Log($"{gameObject.name} caused pushback on {targetName} for {pushbackDistance} units");
    }

    public void SetCanCausePushback(bool enabled)
    {
        canCausePushback = enabled;

        if (enabled && (pushbackTimer == null || pushbackTimer.isCompleted))
        {
            pushbackTimer = Timer.Register(pushbackCooldown, PushbackNearbyTargets, isLooped: true);
        }
        else if (!enabled && pushbackTimer != null)
        {
            pushbackTimer.Cancel();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!canCausePushback || !usePushbackRange) return;

        // Draw pushback range
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, pushbackRange);

        // Draw field of view
        Gizmos.color = Color.yellow;
        Vector3 forward = -transform.up;
        float halfFOV = pushbackFieldOfView * 0.5f;

        // Calculate the two edges of the field of view
        Vector3 leftBoundary = Quaternion.Euler(0, 0, halfFOV) * forward * pushbackRange;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, -halfFOV) * forward * pushbackRange;

        // Draw the field of view cone
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        // Draw arc
        Vector3 previousPoint = transform.position + leftBoundary;
        for (int i = 1; i <= 20; i++)
        {
            float angle = Mathf.Lerp(halfFOV, -halfFOV, i / 20f);
            Vector3 point = transform.position + Quaternion.Euler(0, 0, angle) * forward * pushbackRange;
            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }
    }
#endif

    private void OnDestroy()
    {
        pushbackTimer?.Cancel();

        if (canCausePushback && autoSyncWithDamageor)
            damageor.OnAttackEvent -= Damageor_OnAttackEvent;
    }
}