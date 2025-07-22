using System;
using UnityEngine;

[System.Serializable]
public class Quest
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

    public bool IsCompleted => condition.IsCompleted(this);

    public event Action<Quest> OnCompleteQuest;

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
}