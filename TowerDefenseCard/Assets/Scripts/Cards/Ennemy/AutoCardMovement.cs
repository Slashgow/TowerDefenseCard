using SplineMesh;
using UnityEngine;


public abstract class AutoCardMovement : BaseCardMovement
{
    [SerializeField, Range(0f, 10f)] protected float moveSpeed = 2f;
    [SerializeField] protected bool loop = true;

    protected Spline spline;
    protected float splineLength;
    protected bool isMoving = false;
    protected float currentDistance = 0f;
    protected bool hasCompletedFirstLoop = false;

    public virtual void Init(Spline spawnedSpline)
    {
        hasCompletedFirstLoop = false;

        if (spawnedSpline == null)
        {
            Debug.LogWarning("SplineContainer not assigned to " + this.gameObject.name);
            return;
        }

        spline = spawnedSpline;

        splineLength = spline.Length;
        if (splineLength > 0)
        {
            currentDistance = 0f;
            StartMoving();
        }
        else
            Debug.LogWarning("Spline length is zero or invalid for " + gameObject.name);
    }

    public virtual void StartMoving() => isMoving = true;
    public virtual void StopMoving() => isMoving = false;
}
