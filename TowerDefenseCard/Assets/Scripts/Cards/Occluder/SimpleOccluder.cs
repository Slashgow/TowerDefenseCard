using System;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public class SimpleOccluder : MonoBehaviour, IOccluder
{
    [Header("Occluder Settings")]
    [SerializeField] private float occlusionRange = 5f;
    [SerializeField, Range(0f, 360f)] private float occlusionFieldOfView = 90f;
    [SerializeField, Range(0f, 1f)] private float directionDotThreshold = 0.8f;
    [SerializeField] private LayerMask protectedLayer;
    [SerializeField] private bool canOcclude = true;
    [SerializeField, Range(0f,5f)] private float checkForProtectedDefenseInterval = 1f;

    public float OcclusionRange => occlusionRange;
    public float OcclusionFieldOfView => occlusionFieldOfView;
    public bool CanOcclude => canOcclude;

    private Timer occlusionCheckTimer;
    private List<IDamageable> protectedTargets = new List<IDamageable>();

    private void Start()
    {
        GameManager.Instance.OnStartCombatMode += GameManager_OnStartCombatMode;
        GameManager.Instance.OnEndCombatMode += GameManager_OnEndCombatMode;
    }
    private void OnDestroy()
    {
        occlusionCheckTimer?.Cancel();

        if (GameManager.HasInstance)
        {
            GameManager.Instance.OnStartCombatMode -= GameManager_OnStartCombatMode;
            GameManager.Instance.OnEndCombatMode -= GameManager_OnEndCombatMode;
        }
    }

    private void GameManager_OnEndCombatMode()
    {
        occlusionCheckTimer?.Cancel();
        occlusionCheckTimer = null;
    }

    private void GameManager_OnStartCombatMode()
    {
        if (!canOcclude)
            return;

        occlusionCheckTimer = Timer.Register(checkForProtectedDefenseInterval, CheckForProtectedDefense, isLooped: true);
    }

    private bool IsTargetInProtectionZone(Vector3 targetPosition)
    {
        // Check if target is within occlusion range
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance > occlusionRange)
            return false;

        return IsTargetInFieldOfView(targetPosition);
    }

    private bool IsTargetInFieldOfView(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        Vector3 forward = transform.up; 
        float dotProduct = Vector3.Dot(forward, directionToTarget);
        float fieldOfViewThreshold = Mathf.Cos((occlusionFieldOfView * 0.5f) * Mathf.Deg2Rad);
        return dotProduct >= fieldOfViewThreshold;
    }


    private void CheckForProtectedDefense()
    {
        protectedTargets.ForEach(protectedTargets => protectedTargets.IsProtected = false);
        protectedTargets.Clear();

        if (!canOcclude)
            return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, occlusionRange, protectedLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            if (IsTargetInProtectionZone(hit.transform.position) && hit.TryGetComponent(out IDamageable damageable))
            {
                damageable.IsProtected = true;
                protectedTargets.Add(damageable);
            }
        }
    }

    public void SetCanOcclude(bool enabled)
    {
        canOcclude = enabled;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!canOcclude) return;

        // Visualize occlusion range
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, occlusionRange);

        // Draw field of view for protection zone
        Gizmos.color = Color.green;
        Vector3 forward = transform.right; // Protection direction
        float halfFOV = occlusionFieldOfView * 0.5f;

        // Calculate the two edges of the field of view
        Vector3 leftBoundary = Quaternion.Euler(0, 0, halfFOV) * forward * occlusionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, -halfFOV) * forward * occlusionRange;

        // Draw the field of view cone
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        // Draw arc
        Vector3 previousPoint = transform.position + leftBoundary;
        for (int i = 1; i <= 20; i++)
        {
            float angle = Mathf.Lerp(halfFOV, -halfFOV, i / 20f);
            Vector3 point = transform.position + Quaternion.Euler(0, 0, angle) * forward * occlusionRange;
            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }
    }
#endif
}