using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private Logger logger;
    [SerializeField] private List<Quest> availableQuests = new List<Quest>();
    private Dictionary<string, Quest> activeQuests = new Dictionary<string, Quest>();

    public int AvailableQuestCount => availableQuests.Count;
    public int CompletedQuestCount => availableQuests.Count - activeQuests.Keys.Count;
    public List<Quest> AvailableQuests => availableQuests;

    public event Action<Quest> OnQuestCompleted;

    private void Awake()
    {
        InitializeQuests();
    }

 
    private void OnDestroy()
    {
        foreach (var quest in availableQuests)
        {
            quest.OnCompleteQuest -= OnQuestComplete;
        }
    }

    private void InitializeQuests()
    {
        foreach (var quest in availableQuests)
        {
            quest.Setup();
            activeQuests[quest.QuestId] = quest;
            quest.ResetProgress();
            quest.OnCompleteQuest += OnQuestComplete;
        }
    }

    private void OnQuestComplete(Quest quest)
    {
        if (activeQuests.ContainsKey(quest.QuestId))
        {
            activeQuests.Remove(quest.QuestId); 
            OnQuestCompleted?.Invoke(quest);
            logger.Log($"Quest '{quest.Title}' marked as completed and removed from active quests!", this);
        }
    }

    public Quest GetQuest(string questId)
    {
        return activeQuests.ContainsKey(questId) ? activeQuests[questId] : null;
    }

    public List<Quest> GetActiveQuests()
    {
        return new List<Quest>(activeQuests.Values);
    }
}