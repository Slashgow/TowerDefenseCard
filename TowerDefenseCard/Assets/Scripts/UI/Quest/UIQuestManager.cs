using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class UIQuestManager : UIPage, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField] private UIQuest UIQuestPrefab;

    [SerializeField] private bool autoPinOnAwake = true;
    [SerializeField] private Toggle pinToggle;
    [SerializeField] private UIQuestTab uiQuestTab;

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

    private bool isPin = false;
    public UnityEvent OnExitNotPin;

    private List<UIQuest> mainUIQuests = new List<UIQuest>();
    private List<UIQuest> secondaryUIQuests = new List<UIQuest>();

    protected override void Awake()
    {
        base.Awake();

        dropButtonMainQuest.onClick.AddListener(ToggleScrollRectMainQuest);
        dropButtonSecondaryQuest.onClick.AddListener(ToggleScrollRectSecondaryQuest);

        dropButtonMainQuest.GetComponent<RectTransform>().localEulerAngles = new Vector3(0f, 0f, 180f);
        dropButtonSecondaryQuest.GetComponent<RectTransform>().localEulerAngles = new Vector3(0f, 0f, 180f);

        mainQuestManager.OnQuestCompleted += MainQuestManager_OnQuestCompleted;
        secondaryQuestManager.OnQuestCompleted += SecondaryQuestManager_OnQuestCompleted;

        isPin = autoPinOnAwake;
        pinToggle.isOn = isPin;
        pinToggle.onValueChanged.AddListener(OnClickOnPinToggle);

        if (!isPin)
            Hide();
    }

    private void OnDestroy()
    {
        dropButtonMainQuest.onClick.RemoveListener(ToggleScrollRectMainQuest);
        dropButtonSecondaryQuest.onClick.RemoveListener(ToggleScrollRectSecondaryQuest);
        mainQuestManager.OnQuestCompleted -= MainQuestManager_OnQuestCompleted;
        secondaryQuestManager.OnQuestCompleted -= SecondaryQuestManager_OnQuestCompleted;
        pinToggle.onValueChanged.RemoveListener(OnClickOnPinToggle);
    }
    private void Start()
    {
        InitializeQuestsUI();

        LocalizationSettings.SelectedLocaleChanged -= LocalizationSettings_SelectedLocaleChanged;
        LocalizationSettings.SelectedLocaleChanged += LocalizationSettings_SelectedLocaleChanged;
    }

    private void LocalizationSettings_SelectedLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        for (int i = 0; i < mainQuestManager.AvailableQuests.Count; i++)
        {
            Quest quest = mainQuestManager.AvailableQuests[i];
            mainUIQuests[i].Setup(quest.Description.GetLocalizedString(), quest.IsCompleted, quest);

        }
        for (int i = 0; i < secondaryQuestManager.AvailableQuests.Count; i++)
        {
            Quest quest = secondaryQuestManager.AvailableQuests[i];
            secondaryUIQuests[i].Setup(quest.Description.GetLocalizedString(), quest.IsCompleted, quest);

        }
    }

    private void ToggleScrollRectMainQuest()
    {
        bool enable = scrollRectMainQuest.gameObject.activeSelf;
        dropButtonMainQuest.GetComponent<RectTransform>().localEulerAngles = !enable ? 
            new Vector3(0f, 0f, 180f) : new Vector3(0f, 0f, 0f);
        scrollRectMainQuest.gameObject.SetActive(!enable);
    }
    private void ToggleScrollRectSecondaryQuest()
    {
        bool enable = scrollRectSecondaryQuest.gameObject.activeSelf;
        dropButtonSecondaryQuest.GetComponent<RectTransform>().localEulerAngles = !enable ?
            new Vector3(0f, 0f, 180f) : new Vector3(0f, 0f, 0f);
        scrollRectSecondaryQuest.gameObject.SetActive(!enable);
    }

    public void InitializeQuestsUI()
    {
        titleMainQuestCountText.text = $"({mainQuestManager.CompletedQuestCount}/{mainQuestManager.AvailableQuestCount})";

        titleSecondaryQuestCountText.text = $"({secondaryQuestManager.CompletedQuestCount}/{secondaryQuestManager.AvailableQuestCount})";

        foreach (Quest quest in mainQuestManager.AvailableQuests)
        {
            GameObject uiQuestGameObjectInstance = Instantiate(UIQuestPrefab.gameObject, contentScrollViewMainQuest);
            UIQuest uIQuestInstance = uiQuestGameObjectInstance.GetComponent<UIQuest>();
            uIQuestInstance.Setup(quest.Description.GetLocalizedString(), quest.IsCompleted, quest);
            mainUIQuests.Add(uIQuestInstance);

        }
        foreach (Quest quest in secondaryQuestManager.AvailableQuests)
        {
            GameObject uiQuestGameObjectInstance = Instantiate(UIQuestPrefab.gameObject, contentScrollViewSecondaryQuest);
            UIQuest uIQuestInstance = uiQuestGameObjectInstance.GetComponent<UIQuest>();
            uIQuestInstance.Setup(quest.Description.GetLocalizedString(), quest.IsCompleted, quest);
            secondaryUIQuests.Add(uIQuestInstance);
        }
    }

    private void MainQuestManager_OnQuestCompleted(Quest quest)
    {
        UIQuest uiQuest = GetMainUIQuestByQuestID(quest.QuestId);

        if (uiQuest == null)
        {
            Debug.LogWarning($"UIQuest not found for Quest ID: {quest.QuestId}");
            return;
        }

        uiQuest.SetQuestAsCompleted();
        titleMainQuestCountText.text = $"({mainQuestManager.CompletedQuestCount}/{mainQuestManager.AvailableQuestCount})";
    }
    private void SecondaryQuestManager_OnQuestCompleted(Quest quest)
    {
        UIQuest uiQuest = GetSecondaryUIQuestByQuestID(quest.QuestId);

        if(uiQuest == null)
        {
            Debug.LogWarning($"UIQuest not found for Quest ID: {quest.QuestId}");
            return;
        }

        uiQuest.SetQuestAsCompleted();
        titleSecondaryQuestCountText.text = $"({secondaryQuestManager.CompletedQuestCount}/{secondaryQuestManager.AvailableQuestCount})";
    }

    private void OnClickOnPinToggle(bool isOn) => isPin = isOn;

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isPin)
        {
            OnExitNotPin?.Invoke();
        }

        CameraMovement.Instance.IsDraggindEnable = true;
        CameraMovement.Instance.IsZoomingEnable = true;

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        CameraMovement.Instance.IsDraggindEnable = false;
        CameraMovement.Instance.IsZoomingEnable = false;
    }

    private UIQuest GetMainUIQuestByQuestID(string questID)=>  mainUIQuests.FirstOrDefault(uiQuest => uiQuest.Quest.QuestId == questID);
    private UIQuest GetSecondaryUIQuestByQuestID(string questID) => secondaryUIQuests.FirstOrDefault(uiQuest => uiQuest.Quest.QuestId == questID);

 
}
