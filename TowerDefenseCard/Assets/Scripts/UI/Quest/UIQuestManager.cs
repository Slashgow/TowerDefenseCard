using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class UIQuestManager : MonoBehaviour
{
    [SerializeField] private UIQuest UIQuestPrefab;

    [Header("Main Quests")]
    [SerializeField] private QuestManager mainQuestManager;
    [SerializeField] private TextMeshProUGUI titleMainQuestCountText;
    [SerializeField] private Button dropButtonMainQuest;
    [SerializeField] private ScrollRect scrollRectMainQuest;
    [SerializeField] private Transform contentScrollViewMainQuest;

    [Header("Secondary Quests")]
    [SerializeField] private QuestManager secondaryQuestManager;
    [SerializeField] private TextMeshProUGUI titleSecondaryQuestCountText;
    [SerializeField] private Button dropButtonSecondaryQuest;
    [SerializeField] private ScrollRect scrollRectSecondaryQuest;
    [SerializeField] private Transform contentScrollViewSecondaryQuest;

    private void Awake()
    {
        dropButtonMainQuest.onClick.AddListener(ToggleScrollRectMainQuest);
        dropButtonSecondaryQuest.onClick.AddListener(ToggleScrollRectSecondaryQuest);
    }
    private void OnDestroy()
    {
        dropButtonMainQuest.onClick.RemoveListener(ToggleScrollRectMainQuest);
        dropButtonSecondaryQuest.onClick.RemoveListener(ToggleScrollRectSecondaryQuest);
    }
    private void Start()
    {
        InitializeQuestsUI();
    }
    private void ToggleScrollRectMainQuest()
    {
        bool enable = scrollRectMainQuest.gameObject.activeSelf;
        scrollRectMainQuest.gameObject.SetActive(!enable);
    }
    private void ToggleScrollRectSecondaryQuest()
    {
        bool enable = scrollRectSecondaryQuest.gameObject.activeSelf;
        scrollRectSecondaryQuest.gameObject.SetActive(!enable);
    }

    public void InitializeQuestsUI()
    {
        titleMainQuestCountText.text = $"({mainQuestManager.CompletedQuestCount}/{mainQuestManager.AvailableQuestCount})";

        titleSecondaryQuestCountText.text = $"({secondaryQuestManager.CompletedQuestCount}/{secondaryQuestManager.AvailableQuestCount})";

        foreach (Quest quest in mainQuestManager.AvailableQuests)
        {
            GameObject uiQuestInstance = Instantiate(UIQuestPrefab.gameObject, contentScrollViewMainQuest);
            uiQuestInstance.GetComponent<UIQuest>().Setup(quest.Description);
        }
        foreach (Quest quest in mainQuestManager.AvailableQuests)
        {
            GameObject uiQuestInstance = Instantiate(UIQuestPrefab.gameObject, contentScrollViewSecondaryQuest);
            uiQuestInstance.GetComponent<UIQuest>().Setup(quest.Description);
        }
    }
}
