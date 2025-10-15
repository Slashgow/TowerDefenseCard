using System;
using System.Collections.Generic;
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
    public AutoCardMovementData autoCardMovementSaveData;
    public int currentAmountOfCurrency;
    public List<UpgradeSlotSaveData>upgradeSlotSaveDatas;

    public CardSaveData()
    {
        cardID = CardID.BAMBOO;
        position = Vector3.zero;
        rotation = Quaternion.identity;
        scale = Vector3.one;
        stackCount = 1;
        boosterSaveData = null;
        cardIdeaSaveData = null;
        currentAmountOfCurrency = 0;
        autoCardMovementSaveData = null;
        upgradeSlotSaveDatas = new List<UpgradeSlotSaveData>();
    }

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

    public CardSaveData(CardID cardID, Transform transform, int stackCount, BoosterSaveData boosterSaveData, CardIdeaSaveData cardIdeaSaveData, int currentAmountOfCurrency, AutoCardMovementData autoCardMovementData, List<UpgradeSlotSaveData> upgradeSlotSaveDatas)
    {
        this.cardID = cardID;
        this.position = transform.position;
        this.rotation = transform.rotation;
        this.scale = transform.localScale;
        this.stackCount = stackCount;
        this.boosterSaveData = boosterSaveData;
        this.cardIdeaSaveData = cardIdeaSaveData;
        this.currentAmountOfCurrency = currentAmountOfCurrency;
        this.autoCardMovementSaveData = autoCardMovementData;
        this.upgradeSlotSaveDatas = upgradeSlotSaveDatas;
    }
}
