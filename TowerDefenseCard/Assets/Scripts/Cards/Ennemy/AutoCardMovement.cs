using SplineMesh;
using UnityEngine;


public abstract class AutoCardMovement : BaseCardMovement
{
    [SerializeField, Range(0f, 10f)] protected float moveSpeed = 2f;
    [SerializeField] protected bool loop = true;

    public float MoveSpeed => moveSpeed;
    protected SplineID splineID = SplineID.NONE;
    public SplineID SplineID => splineID;

    protected Spline spline;
    protected float splineLength;
    protected bool isMoving = false;
    protected float currentDistance = 0f;
    public float CurrentDistance => currentDistance;
    protected bool hasCompletedFirstLoop = false;

    private void Start()
    {
        if(splineID != SplineID.NONE)
            spline = SplineManager.Instance.GetSplineByID(splineID);
    }

    public virtual void Init(SplineData spawnedSpline)
    {
        hasCompletedFirstLoop = false;

        if (spawnedSpline == null)
        {
            Debug.LogWarning("SplineContainer not assigned to " + this.gameObject.name);
            return;
        }

        splineID = spawnedSpline.SplineID;
        spline = spawnedSpline.Spline;

        splineLength = spline.Length;
        if (splineLength > 0)
        {
            currentDistance = 0f;
            StartMoving();
        }
        else
            Debug.LogWarning("Spline length is zero or invalid for " + gameObject.name);
    }

    public void Load(AutoCardMovementData autoCardMovementData)
    {
        this.splineID = autoCardMovementData.splineID;
        this.splineLength = autoCardMovementData.splineLength;
        this.isMoving = autoCardMovementData.isMoving;
        this.currentDistance = autoCardMovementData.currentDistance;
        this.hasCompletedFirstLoop = autoCardMovementData.hasCompletedFirstLoop;
    }

    public AutoCardMovementData Save()
    {
        return new AutoCardMovementData
        {
            cardID = GetComponent<Card>().CardData.CardID,
            splineID = this.splineID,
            splineLength = this.splineLength,
            isMoving = this.isMoving,
            currentDistance = this.currentDistance,
            hasCompletedFirstLoop = this.hasCompletedFirstLoop,
        };
    }

    public virtual void StartMoving() => isMoving = true;
    public virtual void StopMoving() => isMoving = false;
}
