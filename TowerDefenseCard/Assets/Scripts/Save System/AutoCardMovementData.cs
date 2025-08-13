using System;


[Serializable]
public class AutoCardMovementData
{
    public CardID cardID;
    public SplineID splineID;
    public float splineLength;
    public bool isMoving;
    public float currentDistance;
    public bool hasCompletedFirstLoop;

}
