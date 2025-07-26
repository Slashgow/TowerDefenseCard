using UnityEngine;
using UnityEngine.Splines;
using UnityTimer;

public class HopCardMovement : AutoCardMovement
{
    [Header("General Movement")]
    //[SerializeField] private AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [Tooltip("Duration of smooth hop per step")]
    [SerializeField, Range(0f, 1f)] private float hopDuration = 0.1f;
    [Tooltip("Pause time after each hop")]
    [SerializeField, Range(0f, 1f)] private float breakDuration = 0.05f;


    private Timer currentHopTimer;
    private Timer currentBreakTimer;
    private Vector3 hopStartPosition;
    private Vector3 hopTargetPosition;

    public override void StartMoving()
    {
        base.StartMoving();
        StartNextHop();
    }

    private void StartNextHop()
    {
        if (!loop && hasCompletedFirstLoop)
        {
            isMoving = false;
            return;
        }

        // Calculate distance to move this step based on speed and hop duration
        float stepDistance = moveSpeed * hopDuration;
        float targetDistance = currentDistance + stepDistance;

        // Handle wrapping around the spline
        if (targetDistance >= splineLength)
        {
            if (loop)
            {
                targetDistance = targetDistance % splineLength;
                hasCompletedFirstLoop = true;
            }
            else
                targetDistance = splineLength;
        }

        // Get current and target positions
        //float currentProgress = currentDistance / splineLength;
        //float targetProgress = targetDistance / splineLength;

        // Apply animation curve
       //float curvedCurrentProgress = animationCurve.Evaluate(currentProgress);
       //float curvedTargetProgress = animationCurve.Evaluate(targetProgress);

        hopStartPosition = spline.GetSampleAtDistance(currentDistance).location;
        hopTargetPosition = spline.GetSampleAtDistance(targetDistance).location;

        // Start hop timer
        currentHopTimer = Timer.Register(hopDuration,
            onUpdate: (secondsElapsed) => {
                float t = secondsElapsed / hopDuration;
                transform.position = Vector3.Lerp(hopStartPosition, hopTargetPosition, t);
            },
            onComplete: () => {
                transform.position = hopTargetPosition;
                currentDistance = targetDistance;
                StartBreak();
            });
    }

    private void StartBreak()
    {
        if (breakDuration <= 0 && isMoving)
        {
            StartNextHop();
            return;
        }

        currentBreakTimer = Timer.Register(breakDuration, 
            onComplete: () => {
                if (isMoving)
                    StartNextHop();
            });
    }

    public override void StopMoving()
    {
        base.StopMoving();

        if (currentHopTimer != null)
        {
            currentHopTimer.Cancel();
            currentHopTimer = null;
        }

        if (currentBreakTimer != null)
        {
            currentBreakTimer.Cancel();
            currentBreakTimer = null;
        }
    }

    private void Update()
    {
        // Maintain 2D z-position for layering
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    private void OnDestroy()
    {
        StopMoving();
    }
}