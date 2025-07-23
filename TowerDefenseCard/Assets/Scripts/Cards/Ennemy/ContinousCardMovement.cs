using UnityEngine;

public class ContinuousCardMovement : AutoCardMovement
{
    [Header("Tilt Settings")]
    [Tooltip("Maximum tilt angle in degrees")]
    [SerializeField, Range(0f, 45f)] private float tiltAmount = 15f;
    [Tooltip("Speed of tilt alternation (cycles per second)")]
    [SerializeField, Range(0f, 5f)] private float tiltSpeed = 1f;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private float tiltTimer = 0f;
    private Vector3 originalRotation;

    protected override void StartMoving()
    {
        base.StartMoving();
        originalRotation = transform.eulerAngles;
    }

    private void Update()
    {
        if (!isMoving) 
            return;

        MoveAlongSpline();
        UpdateTilt();

        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
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
                currentDistance = splineLength;
        }

        float progress = currentDistance / splineLength;
        float curvedProgress = animationCurve.Evaluate(progress);

        Vector3 newPosition = splineContainer.EvaluatePosition(0, curvedProgress);
        transform.position = newPosition;
    }

    private void UpdateTilt()
    {
        if (tiltSpeed <= 0f || tiltAmount <= 0f) 
            return;

        tiltTimer += Time.deltaTime * tiltSpeed;

        float tiltValue = Mathf.Sin(tiltTimer * 2f * Mathf.PI) * tiltAmount;

        Vector3 currentRotation = originalRotation;
        currentRotation.z = originalRotation.z + tiltValue - 180f;
        currentRotation.x = 0f; // Force X rotation to 0
        currentRotation.y = 0f;
        transform.eulerAngles = currentRotation;
    }

    protected override void StopMoving()
    {
        base.StopMoving();

        if (transform != null)
            transform.eulerAngles = originalRotation;
    }

    private void OnDestroy()
    {
        StopMoving();
    }
}