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

    private void Awake() => SetupQuests();

    private void Start() => InitializeQuests();


    private void OnDestroy()
    {
        foreach (var quest in availableQuests)
        {
            quest.OnCompleteQuest -= OnQuestComplete;
        }
    }

    private void SetupQuests()
    {
        foreach (var quest in availableQuests)
        {
            quest.Setup();
            activeQuests[quest.QuestId] = quest;
        }
    }

    private void InitializeQuests()
    {
        foreach (var quest in availableQuests)
        {
          
            quest.OnCompleteQuest += OnQuestComplete;
            quest.CheckProgress();
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

    private void OnValidate()
    {
        for (int i = 0; i<transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            child.name = $"Quest {i + 1}";
        }
    }
}