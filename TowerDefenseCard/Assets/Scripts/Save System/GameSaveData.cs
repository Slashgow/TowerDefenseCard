using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class GameSaveData 
{
    public float currentPlayerHealth;
    public int currentNumberOfCards;
    public int maxCardsAllowed;
    //public int currentNumberOfDefenseCards;
    public int maxDefenseCardsAllowed;
    public float craftTimeElapsed;
    public int currentPlayerCoin;
    public GameMode gameMode;
    public int currentWaveIndex;
    public int currentWaveEnnemyIndex;
    public int currentEnnemyCount;
    public int amountOfSpawnedEnemies;
    public bool isBasePackFirstTimeOpened;
    public bool isDefensePackFirstTimeOpened;
    public bool isEngineeringPackFirstTimeOpened;
    public bool isFoodPackFirstTimeOpened;
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
        QuestSaveData questSaveData = questSaveDatas.FirstOrDefault(questSaveData => questSaveData.questID == questID);

        if(questSaveData.questID == null)
        {
            questSaveData.isLocked = true;
            questSaveData.currentProgress = 0;
        }
        return questSaveData;
    }
}
