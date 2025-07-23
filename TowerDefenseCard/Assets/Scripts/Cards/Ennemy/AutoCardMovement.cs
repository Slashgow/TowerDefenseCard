using UnityEngine;
using UnityEngine.Splines;

public abstract class AutoCardMovement : BaseCardMovement
{
    [SerializeField, Range(0f, 10f)] protected float moveSpeed = 2f;
    [SerializeField] protected bool loop = true;

    protected SplineContainer splineContainer;
    protected float splineLength;
    protected bool isMoving = false;
    protected float currentDistance = 0f;
    protected bool hasCompletedFirstLoop = false;

    public virtual void Init(SplineContainer spawnedSplineContainer)
    {
        hasCompletedFirstLoop = false;

        if (spawnedSplineContainer == null)
        {
            Debug.LogWarning("SplineContainer not assigned to " + this.gameObject.name);
            return;
        }

        splineContainer = spawnedSplineContainer;

        splineLength = splineContainer.Spline.GetLength();
        if (splineLength > 0)
        {
            currentDistance = 0f;
            StartMoving();
        }
        else
            Debug.LogWarning("Spline length is zero or invalid for " + gameObject.name);
    }

    protected virtual void StartMoving() => isMoving = true;
    protected virtual void StopMoving() => isMoving = false;
}
