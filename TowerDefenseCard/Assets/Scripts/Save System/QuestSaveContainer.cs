using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class QuestSaveContainer
{
    public List<QuestSaveData> mainQuestSaveDatas;
    public List<QuestSaveData> secondaryQuestSaveDatas;

    public QuestSaveContainer()
    {
        mainQuestSaveDatas = new List<QuestSaveData>();
        secondaryQuestSaveDatas = new List<QuestSaveData>();
    }

    public void AddMainQuestSaveData(QuestSaveData questSaveData)
    {
        mainQuestSaveDatas.RemoveAll(q => q.questID == questSaveData.questID);
        mainQuestSaveDatas.Add(questSaveData);
    }

    public QuestSaveData GetMainQuestSaveDataByQuestID(string questID)
    {
        return mainQuestSaveDatas.FirstOrDefault(questSaveData => questSaveData.questID == questID);
    }

    public void AddSecondaryQuestSaveData(QuestSaveData questSaveData)
    {
        secondaryQuestSaveDatas.RemoveAll(q => q.questID == questSaveData.questID);
        secondaryQuestSaveDatas.Add(questSaveData);
    }

    public QuestSaveData GetSecondaryQuestSaveDataByQuestID(string questID)
    {
        return secondaryQuestSaveDatas.FirstOrDefault(questSaveData => questSaveData.questID == questID);
    }
}