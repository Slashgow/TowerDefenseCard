using System;
using UnityEngine;

[Serializable]
public struct CardSaveData
{
    public CardID cardID;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public int stackCount;

    public CardSaveData(CardID cardID, Transform transform, int stackCount)
    {
        this.cardID = cardID;
        this.position = transform.position;
        this.rotation = transform.rotation;
        this.scale = transform.localScale;
        this.stackCount = stackCount;
    }
}
