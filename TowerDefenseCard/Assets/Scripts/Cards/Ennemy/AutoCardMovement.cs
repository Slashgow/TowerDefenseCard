using SplineMesh;
using UnityEngine;


public abstract class AutoCardMovement : BaseCardMovement, ILoadable, ISavable
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

    public void Load(GameSaveData gameSaveData)
    {
        AutoCardMovementData autoCardMovementData = gameSaveData.GetAutoCardMovementDataByCardID(GetComponent<Card>().CardData.CardID);
        this.spline = autoCardMovementData.spline;
        this.splineLength = autoCardMovementData.splineLength;
        this.isMoving = autoCardMovementData.isMoving;
        this.currentDistance = autoCardMovementData.currentDistance;
        this.hasCompletedFirstLoop = autoCardMovementData.hasCompletedFirstLoop;
    }

    public void Save(GameSaveData gameSaveData)
    {
        gameSaveData.AddAutoCardMovement(new AutoCardMovementData
        {
            cardID = GetComponent<Card>().CardData.CardID,
            spline = this.spline,
            splineLength = this.splineLength,
            isMoving = this.isMoving,
            currentDistance = this.currentDistance,
            hasCompletedFirstLoop = this.hasCompletedFirstLoop,
        });
    }

    public virtual void StartMoving() => isMoving = true;
    public virtual void StopMoving() => isMoving = false;
}
