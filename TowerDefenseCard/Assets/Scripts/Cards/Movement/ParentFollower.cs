using UnityEngine;

public class ParentFollower : MonoBehaviour
{
    [Header("Lag Settings")]
    [SerializeField, Range(0f, 1f)]
    [Tooltip("Delay before starting to follow parent")]
    private float followDelay = 0.1f;

    [SerializeField, Range(0f, 1f)]
    [Tooltip("How quickly to catch up to target position")]
    private float catchUpSpeed = 0.2f;

    [SerializeField] private Vector3 lagLocalOffset;

    [SerializeField, Range(0f,0.5f)] private float positionDistanceThreshold = 0.01f;
    [SerializeField, Range(0f, 2f)] private float angleDifferenceThreshold = 0.5f;

    [SerializeField]
    private bool lagPosition = true;

    [SerializeField]
    private bool lagRotation = true;

    // Target local position (where we should end up)
    private Vector3 targetLocalPosition = Vector3.zero;
    private Quaternion targetLocalRotation = Quaternion.identity;

    // World position we're trying to hold while parent moves
    private Vector3 frozenWorldPosition;
    private Quaternion frozenWorldRotation;

    // Velocity for smooth damping
    private Vector3 positionVelocity = Vector3.zero;

    // Timing
    private float parentStartMoveTime;
    private bool isLagging = false;

    // Track parent movement
    private Vector3 lastParentWorldPosition;
    private Quaternion lastParentWorldRotation;
    private bool wasInitialized = false;

    private void OnEnable()
    {
        InitializeTracking();
    }

    private void InitializeTracking()
    {
        if (transform.parent != null)
        {
            lastParentWorldPosition = transform.parent.position;
            lastParentWorldRotation = transform.parent.rotation;
        }

        if(TryGetComponent(out CardMover cardMover))
        {
            SetTargetLocalPosition(cardMover.TargetStackPosition);
        }
   

        frozenWorldPosition = transform.position;
        frozenWorldRotation = transform.rotation;
        positionVelocity = Vector3.zero;
        isLagging = false;
        wasInitialized = true;
    }

    private void LateUpdate()
    {
        if (!wasInitialized || transform.parent == null)
        {
            InitializeTracking();
            return;
        }

        // Detect if parent moved
        Vector3 currentParentPos = transform.parent.position;
        Quaternion currentParentRot = transform.parent.rotation;

        bool parentMovedThisFrame = false;

        if (Vector3.Distance(currentParentPos, lastParentWorldPosition) > positionDistanceThreshold)
        // || Quaternion.Angle(currentParentRot, lastParentWorldRotation) > angleDifferenceThreshold)
        {
            parentMovedThisFrame = true;

            // If not already lagging, start now and freeze current world position
            if (!isLagging)
            {
                isLagging = true;
                parentStartMoveTime = Time.unscaledTime;
                frozenWorldPosition = transform.position;
                frozenWorldRotation = transform.rotation;
            }
            else
            {
                // Parent is still moving, keep updating the start time
                parentStartMoveTime = Time.unscaledTime;
            }
        }

        lastParentWorldPosition = currentParentPos;
        lastParentWorldRotation = currentParentRot;

        // Calculate if delay has elapsed
        float timeSinceParentMove = Time.unscaledTime - parentStartMoveTime;
        bool delayElapsed = timeSinceParentMove >= followDelay;

        if (isLagging)
        {
            if (!delayElapsed)
            {
                // DURING DELAY: Compensate for parent movement to hold world position
                if (lagPosition)
                {
                    CompensatePosition();
                }

                if (lagRotation)
                {
                    CompensateRotation();
                }
            }
            else
            {
                // AFTER DELAY: Smoothly return to target local position
                if (lagPosition)
                {
                    CatchUpPosition();
                }

                if (lagRotation)
                {
                    CatchUpRotation();
                }

                // Check if we've caught up (close enough to target)
                if (Vector3.Distance(transform.localPosition, targetLocalPosition) < 0.01f &&
                    Quaternion.Angle(transform.localRotation, targetLocalRotation) < 1f)
                {
                    isLagging = false;
                }
            }
        }
    }

    private void CompensatePosition()
    {
        // Convert frozen world position back to local space
        // This counteracts the parent's automatic movement
        //Vector3 compensatedLocal = transform.parent.InverseTransformPoint(frozenWorldPosition);
        transform.localPosition = lagLocalOffset;  //compensatedLocal;
    }

    private void CompensateRotation()
    {
        // Convert frozen world rotation back to local space
        //Quaternion compensatedLocal = Quaternion.Inverse(transform.parent.rotation) * frozenWorldRotation;
        transform.localRotation = Quaternion.identity; //compensatedLocal;
    }

    private void CatchUpPosition()
    {
        // Smoothly interpolate from current local position to target
        Vector3 newLocal = Vector3.SmoothDamp(
            transform.localPosition,
            targetLocalPosition,
            ref positionVelocity,
            catchUpSpeed,
            Mathf.Infinity,
            Time.unscaledDeltaTime
        );

        transform.localPosition = newLocal;
    }

    private void CatchUpRotation()
    {
        // Smoothly interpolate rotation
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetLocalRotation,
            Time.unscaledDeltaTime / Mathf.Max(0.01f, catchUpSpeed)
        );
    }

    /// <summary>
    /// Set the target local position to move towards (default is Vector3.zero)
    /// </summary>
    public void SetTargetLocalPosition(Vector3 target)
    {
        targetLocalPosition = target;
    }

    /// <summary>
    /// Set the target local rotation (default is Quaternion.identity)
    /// </summary>
    public void SetTargetLocalRotation(Quaternion target)
    {
        targetLocalRotation = target;
    }

    /// <summary>
    /// Immediately snap to target position without lag
    /// </summary>
    public void SnapToTarget()
    {
        transform.localPosition = targetLocalPosition;
        transform.localRotation = targetLocalRotation;
        positionVelocity = Vector3.zero;
        isLagging = false;
    }

    /// <summary>
    /// Manually trigger lag effect (useful when you know parent is about to move)
    /// </summary>
    public void StartLag()
    {
        isLagging = true;
        parentStartMoveTime = Time.unscaledTime;
        frozenWorldPosition = transform.position;
        frozenWorldRotation = transform.rotation;
    }

    /// <summary>
    /// Stop lagging and immediately start catching up
    /// </summary>
    public void StopLag()
    {
        isLagging = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!enabled || !Application.isPlaying || transform.parent == null)
            return;

        // Draw current position (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.05f);

        // Draw frozen position (blue) - where we're trying to stay
        if (isLagging)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(frozenWorldPosition, 0.06f);
            Gizmos.DrawLine(transform.position, frozenWorldPosition);
        }

        // Draw target position (green) - where we should end up
        Vector3 targetWorld = transform.parent.TransformPoint(targetLocalPosition);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(targetWorld, 0.05f);
        Gizmos.DrawLine(transform.position, targetWorld);
    }
#endif

}