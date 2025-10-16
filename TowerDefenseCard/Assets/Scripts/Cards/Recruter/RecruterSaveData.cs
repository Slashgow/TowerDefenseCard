using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class RecruterSaveData
{
    public List<ActiveRecruitmentData> activeRecruitments;

    public RecruterSaveData()
    {
        activeRecruitments = new List<ActiveRecruitmentData>();
    }

    [Serializable]
    public class ActiveRecruitmentData
    {
        public int recruitmentID;
        public float remainingTime;
        public CardID cardIDToSpawn;
        public Vector3 spawnOffset;
        public CraftingRecipe triggerRecipe;
        public float recruitmentDelay;

        public ActiveRecruitmentData(int id, float time, CardID cardID, Vector3 offset, CraftingRecipe recipe, float delay)
        {
            recruitmentID = id;
            remainingTime = time;
            cardIDToSpawn = cardID;
            spawnOffset = offset;
            triggerRecipe = recipe;
            recruitmentDelay = delay;
        }
    }
}
