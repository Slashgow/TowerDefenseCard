using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

[Serializable]
public class GameSaveData 
{
    public float currentPlayerHealth;
    public int currentNumberOfCards;
    public int maxCardsAllowed;
    public float craftTimeElapsed;
    public int currentPlayerCoin;
    public GameMode gameMode;
    public int currentWaveIndex;
    public int currentWaveEnnemyIndex;
    public int currentEnnemyCount;
    public int amountOfSpawnedEnemies;
    public List<StackSaveData> cardStacks;
    public List<AutoCardMovementData> autoCardMovementDatas;
    public List<QuestSaveData> questSaveDatas;

    public GameSaveData()
    {
        cardStacks = new List<StackSaveData>();
        autoCardMovementDatas = new List<AutoCardMovementData>();
        questSaveDatas = new List<QuestSaveData>();
    }
    public void AddAutoCardMovement(AutoCardMovementData autoCardMovementData)
    {
        autoCardMovementDatas.Add(autoCardMovementData);
    }

    public AutoCardMovementData GetAutoCardMovementDataByCardID(CardID cardID)
    {
        return autoCardMovementDatas.FirstOrDefault(autoCardMovementData => autoCardMovementData.cardID == cardID);
    }

    public AutoCardMovementData GetAutoCardMovementDataByIDAndDistance(CardID cardID, float currentDistance)
    {
        return autoCardMovementDatas.FirstOrDefault(autoCardMovementData => autoCardMovementData.cardID == cardID 
                                                    && autoCardMovementData.currentDistance == currentDistance);
    }

    public void AddQuestSaveData(QuestSaveData questSaveData)
    {
        questSaveDatas.Add(questSaveData);
    }

    public QuestSaveData GetQuestSaveDataByQuestID(string questID)
    {
        return questSaveDatas.FirstOrDefault(questSaveData => questSaveData.questID == questID);
    }
}
