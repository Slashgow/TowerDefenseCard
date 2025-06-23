using UnityEngine;
using UnityEngine.Splines;
using System.Collections;

public class EnemyCardMovement : BaseCardMovement 
{   
    [SerializeField, Range(0f,10f)] private float moveSpeed = 2f; // Base speed in units per second
    [SerializeField] private bool loop = true;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f); 
    [Tooltip("Total duration for one full path traversal")]
    [SerializeField, Range(0f,40f)] private float moveDuration = 2f;
    [Tooltip("Duration of smooth hop per step")]
    [SerializeField, Range(0f,1f)] private float hopDuration = 0.1f;
    [Tooltip("Pause time after each hop")]
    [SerializeField, Range(0f,1f)] private float breakDuration = 0.05f; 

    private SplineContainer splineContainer;
    private float splineLength;
    private bool isMoving = false;
    private int currentStep = 0;
    private int numberOfSteps; // Calculated at runtime

    public void Init(SplineContainer spawnedSplineContainer)
    {
        if(spawnedSplineContainer == null)
        {
            Debug.LogWarning("SplineContainer not assigned to " + gameObject.name);
            return;
        }

        splineContainer = spawnedSplineContainer;
        
        splineLength = splineContainer.Spline.GetLength();
        if (splineLength > 0)
        {
            numberOfSteps = Mathf.Max(1, Mathf.RoundToInt(moveDuration / (hopDuration + breakDuration)));
            StartCoroutine(MoveInSteps());
        }
        else
        {
            Debug.LogWarning("Spline length is zero or invalid for " + gameObject.name);
        } 
    }

    private IEnumerator MoveInSteps()
    {
        isMoving = true;
        float stepDistance = splineLength / numberOfSteps; // Distance per step

        while (isMoving)
        {
            float progress = (float)currentStep / numberOfSteps; // [0, 1]
            if (!loop && progress >= 1f)
            {
                isMoving = false; // Stop if not looping
                yield break;
            }

            progress = progress % 1f; // Loop by resetting
            float curveT = animationCurve.Evaluate(progress); // Apply curve to step timing
            float targetDistance = curveT * splineLength;
            Vector3 targetPosition = SplineUtility.EvaluatePosition(splineContainer.Spline, targetDistance / splineLength);

            // Smooth hop to target position
            Vector3 startPosition = transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < hopDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / hopDuration;
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null; // Wait for next frame
            }

            transform.position = targetPosition; // Ensure exact target position
            currentStep = (currentStep + 1) % numberOfSteps; // Move to next step
            yield return new WaitForSeconds(breakDuration); // Pause after hop
        }
    }

    void Update()
    {
        // Maintain 2D z-position for layering
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    // Optional: Public method to adjust parameters at runtime
    public void SetMoveParameters(float duration, float hopDur, float breakDur)
    {
        moveDuration = duration;
        hopDuration = hopDur;
        breakDuration = breakDur;
        numberOfSteps = Mathf.Max(1, Mathf.RoundToInt(moveDuration / (hopDuration + breakDuration)));
        if (isMoving) StartCoroutine(MoveInSteps()); // Restart with new settings
    }
}