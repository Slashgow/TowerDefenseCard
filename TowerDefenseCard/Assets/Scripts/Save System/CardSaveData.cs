using System;
using UnityEngine;

[Serializable]
public class CardSaveData
{
    public CardID cardID;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public int stackCount;
    public BoosterSaveData boosterSaveData;
    public CardIdeaSaveData cardIdeaSaveData;
    public int currentAmountOfCurrency;

    public CardSaveData(CardID cardID, Transform transform, int stackCount)
    {
        this.cardID = cardID;
        this.position = transform.position;
        this.rotation = transform.rotation;
        this.scale = transform.localScale;
        this.stackCount = stackCount;
        boosterSaveData = null;
        cardIdeaSaveData = null;
    }

    public CardSaveData(CardID cardID, Transform transform, int stackCount, BoosterSaveData boosterSaveData)
    {
        this.cardID = cardID;
        this.position = transform.position;
        this.rotation = transform.rotation;
        this.scale = transform.localScale;
        this.stackCount = stackCount;
        this.boosterSaveData = boosterSaveData;
        cardIdeaSaveData = null;
    }
    public CardSaveData(CardID cardID, Transform transform, int stackCount, BoosterSaveData boosterSaveData, CardIdeaSaveData cardIdeaSaveData, int currentAmountOfCurrency)
    {
        this.cardID = cardID;
        this.position = transform.position;
        this.rotation = transform.rotation;
        this.scale = transform.localScale;
        this.stackCount = stackCount;
        this.boosterSaveData = boosterSaveData;
        this.cardIdeaSaveData = cardIdeaSaveData;
        this.currentAmountOfCurrency = currentAmountOfCurrency;
    }
}
