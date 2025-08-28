using UnityEngine;
using UnityTimer;

public class DashCardMovement : ContinuousCardMovement
{
    [Header("Dash Settings")]
    [Tooltip("Distance to dash forward along the spline")]
    [SerializeField, Range(0.5f, 10f)] private float dashDistance = 3f;

    [Tooltip("Time between possible dashes (in seconds)")]
    [SerializeField, Range(1f, 10f)] private float dashCooldown = 3f;

    [Tooltip("Speed multiplier during dash")]
    [SerializeField, Range(2f, 10f)] private float dashSpeedMultiplier = 5f;

    [Tooltip("Duration of the dash in seconds")]
    [SerializeField, Range(0.1f, 1f)] private float dashDuration = 0.3f;

    [Tooltip("Probability of dashing when cooldown is ready (0-1)")]
    [SerializeField, Range(0f, 1f)] private float dashProbability = 0.7f;

    private Timer dashCooldownTimer;
    private Timer dashTimer;
    private bool isDashing = false;
    private float dashStartDistance;
    private float dashTargetDistance;
    private bool canDash = false;

    public bool IsDashing => isDashing;
    public bool CanDash => canDash;
    public float DashCooldownRemaining => dashCooldownTimer != null ? dashCooldownTimer.GetTimeRemaining() : 0f;

    protected override void Awake()
    {
        base.Awake();
        InitializeDashCooldown();
    }

    private void InitializeDashCooldown()
    {
        dashCooldownTimer = Timer.Register(dashCooldown, () =>
        {
            canDash = true;
            RestartCooldownTimer();
        });
    }

    private void RestartCooldownTimer()
    {
        if (isMoving)
        {
            dashCooldownTimer = Timer.Register(dashCooldown, () =>
            {
                canDash = true;
                RestartCooldownTimer();
            });
        }
    }

    public override void StartMoving()
    {
        base.StartMoving();
        canDash = true; 
        isDashing = false;

        if (dashCooldownTimer == null || dashCooldownTimer.isDone)
            InitializeDashCooldown();
    }

    private void Update()
    {
        if (!isMoving)
            return;

        if (!isDashing && canDash && ShouldDash())
            StartDash();

        if (isDashing)
            UpdateDash();
        else
            MoveAlongSpline();

        UpdateTilt();
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    private bool ShouldDash() => Random.Range(0f, 1f) <= dashProbability;

    private void StartDash()
    {
        isDashing = true;
        canDash = false; 
        dashStartDistance = currentDistance;

        dashTargetDistance = currentDistance + dashDistance;

        if (dashTargetDistance >= splineLength)
        {
            if (loop)
                dashTargetDistance = dashTargetDistance % splineLength;
            else
                dashTargetDistance = splineLength;
        }

        //lastDashTime = Time.time;

        dashTimer = Timer.Register(dashDuration, () =>
        {
            CompleteDash();
        });

        OnDashStart();
    }

    private void CompleteDash()
    {
        currentDistance = dashTargetDistance;
        isDashing = false;
        OnDashEnd();
    }

    private void UpdateDash()
    {
        if (dashTimer == null || dashTimer.isDone)
            return;

        float dashProgress = 1f - (dashTimer.GetTimeRemaining() / dashDuration);
        if (loop && dashTargetDistance < dashStartDistance)
        {
            float totalDashDistance = (splineLength - dashStartDistance) + dashTargetDistance;
            float currentDashDistance = totalDashDistance * dashProgress;

            if (currentDashDistance <= splineLength - dashStartDistance)
            {
                currentDistance = dashStartDistance + currentDashDistance;
            }
            else
            {
                currentDistance = currentDashDistance - (splineLength - dashStartDistance);
            }
        }
        else
        {
            currentDistance = Mathf.Lerp(dashStartDistance, dashTargetDistance, dashProgress);
        }

        Vector3 newPosition = spline.GetSampleAtDistance(currentDistance).location;
        transform.position = newPosition;

        if (currentDistance >= splineLength && !loop)
            hasCompletedFirstLoop = true;
        else if (loop && !hasCompletedFirstLoop && currentDistance >= splineLength)
            hasCompletedFirstLoop = true;
    }

    private void MoveAlongSpline()
    {
        if (!loop && hasCompletedFirstLoop)
        {
            StopMoving();
            return;
        }

        float distanceToMove = moveSpeed * Time.deltaTime;
        currentDistance += distanceToMove;

        if (currentDistance >= splineLength)
        {
            if (loop)
            {
                currentDistance = currentDistance % splineLength;
                hasCompletedFirstLoop = true;
            }
            else
            {
                currentDistance = splineLength;
                hasCompletedFirstLoop = true;
            }
        }

        Vector3 newPosition = spline.GetSampleAtDistance(currentDistance).location;
        transform.position = newPosition;
    }

    public override void StopMoving()
    {
        base.StopMoving();
        isDashing = false;
        canDash = false;

        if (dashCooldownTimer != null && !dashCooldownTimer.isDone)
            dashCooldownTimer.Cancel();

        if (dashTimer != null && !dashTimer.isDone)
            dashTimer.Cancel();
    }

    public void ForceDash()
    {
        if (isMoving && !isDashing)
        {
            canDash = true;
            StartDash();
        }
    }

    public void SetDashCooldown(float newCooldown)
    {
        dashCooldown = Mathf.Max(0.1f, newCooldown);

        // Restart cooldown timer with new duration if currently running
        if (isMoving && (dashCooldownTimer == null || dashCooldownTimer.isDone))
        {
            RestartCooldownTimer();
        }
    }

    public void SetDashDistance(float newDistance)
    {
        dashDistance = Mathf.Max(0.1f, newDistance);
    }

    public void SetDashProbability(float newProbability)
    {
        dashProbability = Mathf.Clamp01(newProbability);
    }

    // Virtual methods for customization
    protected virtual void OnDashStart()
    {
        // Override this for visual/audio effects when dash starts
        // Examples: particle effects, sound, screen shake, etc.
    }

    protected virtual void OnDashEnd()
    {
        // Override this for visual/audio effects when dash ends
    }

    // Property accessors
  

    private void OnDestroy()
    {
        // Clean up timers when object is destroyed
        if (dashCooldownTimer != null && !dashCooldownTimer.isDone)
        {
            dashCooldownTimer.Cancel();
        }

        if (dashTimer != null && !dashTimer.isDone)
        {
            dashTimer.Cancel();
        }
    }
}