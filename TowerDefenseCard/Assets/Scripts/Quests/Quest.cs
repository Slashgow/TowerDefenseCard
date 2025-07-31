using System;
using UnityEngine;

[System.Serializable]
public class Quest : ILoadable, ISavable
{
    [SerializeField] private string questId;
    public string QuestId => questId;

    [SerializeField] private string title;
    public string Title => title;

    [SerializeField] private string description;
    public string Description => description;

    [SerializeField] private QuestCondition condition;
    public QuestCondition Condition => condition;

    [SerializeField] private int goalCount;
    public int GoalCount => goalCount;

    private int currentProgress;
    public int CurrentProgress => currentProgress;

    public bool IsCompleted => condition.IsCompleted();

    public event Action<Quest> OnCompleteQuest;

    public void Setup() => condition.Setup(this);

    public void IncrementProgress()
    {
        currentProgress++;
        if (IsCompleted)
        {
            OnCompleteQuest?.Invoke(this); // Trigger event when completed
        }
    }

    public void ResetProgress()
    {
        currentProgress = 0;
    }

    public void CheckProgress()
    {
        if (IsCompleted)
            OnCompleteQuest?.Invoke(this);
    }

    public void Save(GameSaveData gameSaveData)
    {
        gameSaveData.AddQuestSaveData(new QuestSaveData
        {
            questID = this.questId,
            currentProgress = this.currentProgress,
        });
    }
    public void Load(GameSaveData gameSaveData)
    {
        QuestSaveData questSaveData = gameSaveData.GetQuestSaveDataByQuestID(this.questId);
        this.currentProgress = questSaveData.currentProgress;
    }
}