using UnityEngine;
using UnityTimer;

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

    [SerializeField] private bool lagPosition = true;

    [SerializeField] private bool lagRotation = true;

    [SerializeField, Range(0f,3f)] private float disableDelay = 1f;

    private Vector3 targetLocalPosition = Vector3.zero;
    private Quaternion targetLocalRotation = Quaternion.identity;

    private Vector3 positionVelocity = Vector3.zero;

    private float parentStartMoveTime;
    private bool isLagging = false;

    private Vector3 lastParentWorldPosition;

    private bool wasInitialized = false;

    private Timer disableTimer;

    private void OnEnable()
    {
        InitializeTracking();
    }

    private void InitializeTracking()
    {
        if (transform.parent != null)
        {
            lastParentWorldPosition = transform.parent.position;
        }

        if(TryGetComponent(out CardMover cardMover))
        {
            SetTargetLocalPosition(cardMover.TargetStackPosition);
        }
   
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

        DetectParentMovement();

        float timeSinceParentMove = Time.unscaledTime - parentStartMoveTime;
        bool delayElapsed = timeSinceParentMove >= followDelay;

        if (isLagging)
        {
            if (!delayElapsed)
                FollowTargetWithOffset();
            else
            {
                CatchUpTarget();

                if (Vector3.Distance(transform.localPosition, targetLocalPosition) < 0.01f &&
                    Quaternion.Angle(transform.localRotation, targetLocalRotation) < 1f)
                {
                    isLagging = false;
                }
            }
        }
    }

    private void CatchUpTarget()
    {
        if (lagPosition)
            CatchUpPosition();

        if (lagRotation)
            CatchUpRotation();
    }

    private void FollowTargetWithOffset()
    {
        if (lagPosition)
            transform.localPosition = lagLocalOffset;

        if (lagRotation)
            transform.localRotation = Quaternion.identity;
    }

    private void DetectParentMovement()
    {
        Vector3 currentParentPos = transform.parent.position;

        if (Vector3.Distance(currentParentPos, lastParentWorldPosition) > positionDistanceThreshold)
        {
            if (!isLagging)
            {
                isLagging = true;
                parentStartMoveTime = Time.unscaledTime;
            }
            else
            {
                parentStartMoveTime = Time.unscaledTime;
            }
        }

        lastParentWorldPosition = currentParentPos;
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


    public void SetTargetLocalPosition(Vector3 target) => targetLocalPosition = target;
    public void SetTargetLocalRotation(Quaternion target) => targetLocalRotation = target;

    public void SnapToTarget()
    {
        transform.localPosition = targetLocalPosition;
        transform.localRotation = targetLocalRotation;
        positionVelocity = Vector3.zero;
        isLagging = false;
    }

    public void StartLag()
    {
        isLagging = true;
        parentStartMoveTime = Time.unscaledTime;
    }

    public void StopLag() => isLagging = false;

    public void ScheduleDisable()
    {
        disableTimer?.Cancel();
        disableTimer = Timer.Register(disableDelay, onComplete: () => OnCompleteDisableTimer(), useRealTime: true);
    }

    public void CancelDisableSchedule() => disableTimer?.Cancel();

    private void OnCompleteDisableTimer()
    {
        enabled = false;
    }
    private void OnDisable()
    {
        disableTimer?.Cancel();
    }
}