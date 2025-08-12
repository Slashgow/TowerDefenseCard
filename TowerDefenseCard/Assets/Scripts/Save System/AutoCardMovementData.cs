using System;
using SplineMesh;

[Serializable]
public class AutoCardMovementData
{
    public CardID cardID;
    public Spline spline;
    public float splineLength;
    public bool isMoving;
    public float currentDistance;
    public bool hasCompletedFirstLoop;

}
