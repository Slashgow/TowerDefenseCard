using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoSingleton<QuestManager>
{
    [SerializeField] private Logger logger;
    [SerializeField] private List<Quest> availableQuests = new List<Quest>();
    private Dictionary<string, Quest> activeQuests = new Dictionary<string, Quest>();

    public event Action<Quest> OnQuestCompleted;

    protected override void Awake()
    {
        base.Awake();
        InitializeQuests();
    }

    private void Start()
    {
        // Subscribe to game events
        //CraftingManager.Instance.OnCraftComplete += OnCraftComplete;
        //ShopManager.OnPurchaseBooster += OnPurchaseBooster;
    }

    private void OnDestroy()
    {
        //if (CraftingManager.HasInstance)
        //    CraftingManager.Instance.OnCraftComplete -= OnCraftComplete;
        //
        //if(ShopManager.HasInstance)
        //    ShopManager.OnPurchaseBooster -= OnPurchaseBooster;
        //
        foreach (var quest in availableQuests)
        {
            quest.OnCompleteQuest -= OnQuestComplete;
        }
    }

    private void InitializeQuests()
    {
        foreach (var quest in availableQuests)
        {
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

    //public void OnCraftComplete(int craftId, CardID outputCardId)
    //{
    //    foreach (var quest in activeQuests.Values)
    //    {
    //        quest.Condition.OnActionPerformed(quest, outputCardId);
    //    }
    //}
    //
    //public void OnPurchaseBooster()
    //{
    //    foreach (var quest in activeQuests.Values)
    //    {
    //        quest.Condition.OnActionPerformed(quest, null);
    //    }
    //}

    public Quest GetQuest(string questId)
    {
        return activeQuests.ContainsKey(questId) ? activeQuests[questId] : null;
    }

    public List<Quest> GetActiveQuests()
    {
        return new List<Quest>(activeQuests.Values);
    }
}