using System;
using System.Xml;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityTimer;

public class InputShower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private QuestManager mainQuestManager;
    [SerializeField] private TextMeshProUGUI inputDisplayText, secondInputDisplayText;

    [SerializeField, Range(0f, 20f)] private float startInputDisplayDuration = 12f;
    [SerializeField, Range(0f, 5f)] private float fadeInDuration = 2f;
    [SerializeField, Range(0f, 5f)] private float fadeOutDuration = 2f;

    [Header("Move Input")]
    [SerializeField] private LocalizedString moveInputDisplayText;
    [SerializeField] private LocalizedString zoomInputDisplayText;

    [Header("Pause Input")]
    [SerializeField] private LocalizedString pauseInputDisplayText;
    [SerializeField] private string questIDPauseInput;
    private Quest questPauseInput;

    [Header("Craft Menu Input")]
    [SerializeField] private LocalizedString craftMenuInputDisplayText;
    [SerializeField] private string questIDCraftMenuInput;
    private Quest questCraftMenuInput;

    [Header("Speed Up/Down Input")]
    [SerializeField] private LocalizedString speedUpDownInputDisplayText;
    [SerializeField] private string questIDSpeedUpDownInput;
    private Quest questSpeedUpDownInput;

    [Header("Magnet Input")]
    [SerializeField] private LocalizedString magnetInputDisplayText;
    [SerializeField, Range(0f, 20f)] private float magnetInputDisplayDuration = 8f;

    private Timer startTimer;
    private Timer magnetTimer;
    private Tween fadeTween;
    private Tween fadeTweenSecond;

    private void Awake()
    {
        questPauseInput = mainQuestManager.GetQuestByID(questIDPauseInput);
        questCraftMenuInput = mainQuestManager.GetQuestByID(questIDCraftMenuInput);
        questSpeedUpDownInput = mainQuestManager.GetQuestByID(questIDSpeedUpDownInput);

        questPauseInput.OnUnlockQuest += QuestPauseInput_OnUnlockQuest;
        questPauseInput.OnCompleteQuest += QuestPauseInput_OnCompleteQuest;
        questCraftMenuInput.OnUnlockQuest += QuestCraftMenuInput_OnUnlockQuest;
        questCraftMenuInput.OnCompleteQuest += QuestCraftMenuInput_OnCompleteQuest;
        questSpeedUpDownInput.OnUnlockQuest += QuestSpeedUpDownInput_OnUnlockQuest;
        questSpeedUpDownInput.OnCompleteQuest += QuestSpeedUpDownInput_OnCompleteQuest;

        WaveManager.Instance.OnWaveEnd += WaveManager_OnWaveEnd;
    }

    private void Start()
    {
        if (SavePath.TutorialSaveExists)
        {
            inputDisplayText.gameObject.SetActive(false);
            secondInputDisplayText.gameObject.SetActive(false);
            return;
        }
           

        DisplayInputText(moveInputDisplayText);
        DisplaySecondInputText(zoomInputDisplayText);
        startTimer = Timer.Register(startInputDisplayDuration, onComplete:() => {
            HideInputText();
            HideSecondInputText();
        });
    }
    private void OnDestroy()
    {
        questPauseInput.OnUnlockQuest -= QuestPauseInput_OnUnlockQuest;
        questPauseInput.OnCompleteQuest -= QuestPauseInput_OnCompleteQuest;
        questCraftMenuInput.OnUnlockQuest -= QuestCraftMenuInput_OnUnlockQuest;
        questCraftMenuInput.OnCompleteQuest -= QuestCraftMenuInput_OnCompleteQuest;
        questSpeedUpDownInput.OnUnlockQuest -= QuestSpeedUpDownInput_OnUnlockQuest;
        questSpeedUpDownInput.OnCompleteQuest -= QuestSpeedUpDownInput_OnCompleteQuest;

        startTimer?.Cancel();
        magnetTimer?.Cancel();
    }
    private void QuestSpeedUpDownInput_OnCompleteQuest(Quest quest) => HideInputText();
    private void QuestSpeedUpDownInput_OnUnlockQuest(Quest quest) => DisplayInputText(speedUpDownInputDisplayText);
    private void QuestCraftMenuInput_OnCompleteQuest(Quest quest) => HideInputText();
    private void QuestCraftMenuInput_OnUnlockQuest(Quest quest) => DisplayInputText(craftMenuInputDisplayText);
    private void QuestPauseInput_OnCompleteQuest(Quest quest) => HideInputText();
    private void QuestPauseInput_OnUnlockQuest(Quest questPause) => DisplayInputText(pauseInputDisplayText);
    private void WaveManager_OnWaveEnd(int waveIndex)
    {
        if(waveIndex != 1) 
            return;

        DisplayInputText(magnetInputDisplayText);
        magnetTimer = Timer.Register(magnetInputDisplayDuration, onComplete: HideInputText);
    }

    private void DisplayInputText(LocalizedString inputText)
    {
        inputDisplayText.gameObject.SetActive(true);

#if UNITY_WEBGL
        inputText.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                inputDisplayText.text = handle.Result;
            }
        };
#endif

#if !UNITY_WEBGL
           inputDisplayText.text = inputText.GetLocalizedString();
#endif

        Color currentColor = inputDisplayText.color;
        currentColor.a = 0f;
        inputDisplayText.color = currentColor;
        
        fadeTween?.Kill();  
        fadeTween = DOTween.To(
           () => inputDisplayText.color.a,
           alpha => inputDisplayText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha),
           0.5f,
           fadeInDuration
       );
    }

    private void DisplaySecondInputText(LocalizedString inputText)
    {
        secondInputDisplayText.gameObject.SetActive(true);

#if UNITY_WEBGL
        inputText.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                secondInputDisplayText.text = handle.Result;
            }
        };
#endif

#if !UNITY_WEBGL
            secondInputDisplayText.text = inputText.GetLocalizedString();
#endif



        Color currentColor = secondInputDisplayText.color;
        currentColor.a = 0f;
        secondInputDisplayText.color = currentColor;

        fadeTweenSecond?.Kill();
        fadeTweenSecond = DOTween.To(
           () => secondInputDisplayText.color.a,
           alpha => secondInputDisplayText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha),
           0.5f,
           fadeInDuration
       );
    }

    private void HideInputText()
    {
        Color currentColor = inputDisplayText.color;

        fadeTween?.Kill();
        fadeTween = DOTween.To(
            () => inputDisplayText.color.a,
            alpha => inputDisplayText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha),
            0f,
            fadeOutDuration
        ).OnComplete(() => inputDisplayText.gameObject.SetActive(false));
    }

    private void HideSecondInputText()
    {
        Color currentColor = secondInputDisplayText.color;

        fadeTweenSecond?.Kill();
        fadeTweenSecond = DOTween.To(
            () => secondInputDisplayText.color.a,
            alpha => secondInputDisplayText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha),
            0f,
            fadeOutDuration
        ).OnComplete(() => secondInputDisplayText.gameObject.SetActive(false));
    }
}
