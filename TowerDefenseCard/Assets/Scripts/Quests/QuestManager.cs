using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private Logger logger;
    [SerializeField] private bool isMainQuestManager = true;
    [SerializeField] private List<Quest> availableQuests = new List<Quest>();
    private Dictionary<string, Quest> activeQuests = new Dictionary<string, Quest>();

    public int AvailableQuestCount => availableQuests.Count;
    public int CompletedQuestCount => availableQuests.Count - activeQuests.Keys.Count;
    public List<Quest> AvailableQuests => availableQuests;

    public event Action<Quest> OnQuestCompleted;
    public event Action<Quest> OnQuestUnlocked;
    public static event Action OnAnyQuestCompleted;


    private void Awake()
    {
        SetupQuests();
        LoadQuestProgress();
    }

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
        if (availableQuests.Count > 0)
        {
            availableQuests[0].UnlockQuest();
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
            OnAnyQuestCompleted?.Invoke();
            logger.Log($"Quest '{quest.Title}' marked as completed and removed from active quests!", this);
            UnlockNextQuest(quest);
        }
    }

    private void UnlockNextQuest(Quest completedQuest)
    {
        int completedIndex = availableQuests.FindIndex(q => q.QuestId == completedQuest.QuestId);

        if (completedIndex == -1)
        {
            logger.Log($"Warning: Completed quest '{completedQuest.QuestId}' not found in available quests list!", this);
            return;
        }

        int nextIndex = completedIndex + 1;
        if (nextIndex < availableQuests.Count)
        {
            Quest nextQuest = availableQuests[nextIndex];
            if (nextQuest.IsLocked)
            {
                nextQuest.UnlockQuest();
                OnQuestUnlocked?.Invoke(nextQuest);
                logger.Log($"Quest '{nextQuest.Title}' has been unlocked!", this);
            }
        }
        else
        {
            logger.Log("All quests have been completed!", this);
        }
        SaveQuestProgress();
    }
    
    public Quest GetQuestByID(string questId) => availableQuests.FirstOrDefault(q => q.QuestId == questId);
    public Quest GetActiveQuestByID(string questId) => activeQuests.ContainsKey(questId) ? activeQuests[questId] : null;
    public List<Quest> GetActiveQuests() => new List<Quest>(activeQuests.Values);
    private void SaveQuestProgress()
    {
        try
        {
            QuestSaveContainer questSaveContainer = new QuestSaveContainer();
            if (SavePath.QuestSaveExists)
            {
                string json = File.ReadAllText(SavePath.QuestSaveFilePath);
                questSaveContainer = JsonUtility.FromJson<QuestSaveContainer>(json);
            }

            foreach (var quest in availableQuests)
            {
                QuestSaveData questSaveData = new QuestSaveData
                {
                    questID = quest.QuestId,
                    currentProgress = quest.CurrentProgress,
                    isLocked = quest.IsLocked
                };

                if (isMainQuestManager)
                    questSaveContainer.AddMainQuestSaveData(questSaveData);
                else
                    questSaveContainer.AddSecondaryQuestSaveData(questSaveData);
            }

            string updatedJson = JsonUtility.ToJson(questSaveContainer, true);
            File.WriteAllText(SavePath.QuestSaveFilePath, updatedJson);
            logger.Log($"Quest progress saved to {SavePath.QuestSaveFilePath}", this);
        }
        catch (Exception e)
        {
            logger.Log($"Failed to save quest progress: {e.Message}", this);
        }
    }

    private void LoadQuestProgress()
    {
        try
        {
            if (SavePath.QuestSaveExists)
            {
                string json = File.ReadAllText(SavePath.QuestSaveFilePath);
                QuestSaveContainer questSaveContainer = JsonUtility.FromJson<QuestSaveContainer>(json);

                foreach (var quest in availableQuests)
                {
                    QuestSaveData questSaveData;
                    if (isMainQuestManager)
                        questSaveData = questSaveContainer.GetMainQuestSaveDataByQuestID(quest.QuestId);
                    else
                        questSaveData = questSaveContainer.GetSecondaryQuestSaveDataByQuestID(quest.QuestId);

                    if (questSaveData.questID != null)
                    {
                        quest.Load(questSaveData);

                        if (quest.IsCompleted)
                        {
                            OnQuestComplete(quest);
                        }
                    }
                }

                logger.Log($"Quest progress loaded from {SavePath.QuestSaveFilePath}", this);
            }
            else
            {
                logger.Log("No quest save file found, starting with default quest states", this);
            }
        }
        catch (Exception e)
        {
            logger.Log($"Failed to load quest progress: {e.Message}", this);
        }
    }

    public static void ResetQuestSave()
    {
        if (File.Exists(SavePath.QuestSaveFilePath))
        {
            File.Delete(SavePath.QuestSaveFilePath);
        }
    }

    private void OnValidate()
    {
        for (int i = 0; i<transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            if(availableQuests.Count <= i)
            {
                Debug.LogWarning($"QuestManager: Not enough quests available to assign to child {i}. Available: {availableQuests.Count}, Required: {i + 1}");
                break;
            }

            child.name = $"Quest {i + 1}_{availableQuests[i].QuestId}";
        }
    }
}