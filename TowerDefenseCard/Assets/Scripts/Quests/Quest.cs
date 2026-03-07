using System;
using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public class Quest
{
    [SerializeField] private string questId;
    public string QuestId => questId;

    [SerializeField] private LocalizedString title;
    public LocalizedString Title => title;

    [SerializeField] private LocalizedString description;
    public LocalizedString Description => description;

    [SerializeField] private QuestCondition condition;
    public QuestCondition Condition => condition;

    [SerializeField] private int goalCount;
    public int GoalCount => goalCount;

    private int currentProgress;
    public int CurrentProgress => currentProgress;

    [SerializeField] private bool isLocked = true; 
    public bool IsLocked => isLocked;

    [SerializeField] public bool isDemoLocked = false;
    public bool IsDemoLocked => isDemoLocked;

    public bool IsCompleted => condition.IsCompleted() && !isLocked && !isDemoLocked;

    public event Action<Quest> OnCompleteQuest;
    public event Action<Quest> OnUnlockQuest;

    public void Setup() => condition.Setup(this);

    public void IncrementProgress()
    {
        currentProgress++;

        if (isLocked || isDemoLocked)
            return;

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
        if (isLocked || isDemoLocked)
            return;

        if (IsCompleted)
            OnCompleteQuest?.Invoke(this);
    }
    public void UnlockQuest()
    {
        if (isDemoLocked && DemoManager.Instance.UseDemoMode)
        {
            Debug.LogWarning($"Cannot unlock quest '{questId}' - it is demo locked.");
            return;
        }

        Debug.Log($"Unlocking quest '{questId}'");
        isLocked = false;
        OnUnlockQuest?.Invoke(this);
    }

    public void LockQuest() => isLocked = true;

    public void Save(QuestSaveContainer questSaveContainer)
    {
        questSaveContainer.AddMainQuestSaveData(new QuestSaveData
        {
            questID = this.questId,
            currentProgress = this.currentProgress,
            isLocked = this.isLocked
        });
    }
    public void Load(QuestSaveData questSaveData)
    {
        this.currentProgress = questSaveData.currentProgress;
        this.isLocked = questSaveData.isLocked;
        Debug.Log($"Loaded quest '{questId}' with progress {currentProgress} and locked status {isLocked}");
    }
}