using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityTimer;

public class Tanuki : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScrollTextWithVoice scrollTextWithVoice;
    [SerializeField] private TextMeshProUGUI tanukiTextWorldSpace;
    [SerializeField] private Image bubbleImageWorldSpace;
    [SerializeField] private Image tanukiImageWorldSpace;
    [SerializeField] private TextMeshProUGUI tanukiTextScreenSpace;
    [SerializeField] private Image bubbleImageScreenSpace;
    [SerializeField] private Image tanukiImageScreenSpace;
    [SerializeField] private CardSpawner merchantSpawner;
    [SerializeField] private CardSpawner workerSpawner;

    [Header("Settings")]
    [SerializeField, Range(0f,500f)] private float lastLineDurationThresholdForTeasing = 30f;
    [SerializeField, Range(0f,15f)] private float keepShowingDuration = 10f;
    [SerializeField] private Sprite defaultTanukiSprite;
    [SerializeField, Range(0f, 20f)] private float timeBeforeStartTutoText = 15f;
    [SerializeField, Range(0f, 10f)] private float timeBetweenTextInQueue = 3f;
    [SerializeField] private bool canHaveSameLineSuccessively = false;

    [Header("Lines")]
    [SerializeField] private TanukiLine linesWhenPlayerHit;
    [SerializeField] private TanukiLine linesAfterWaveNoHit;
    [SerializeField] private TanukiLine linesAfterWaveHit;
    [SerializeField] private TanukiLine linesMaxCardsReached;
    [SerializeField] private TanukiLine linesMaxDefenseReached;
    [SerializeField] private TanukiLine linesLateDefense;
    [SerializeField] private TanukiLine linesTeasing;
    [SerializeField] private TanukiLine linesOnCraftTemple;
    [SerializeField] private TanukiLine linesOnCraftFirstDefense;
    [SerializeField] private TanukiLine linesOnFirstCraftComplete;
    [SerializeField] private TanukiLine linesOnFirstFactoryComplete;
    [SerializeField] private TanukiLine linesStackShortcuts;
    [SerializeField] private TanukiLine linesTutoSellForBooster;
    [SerializeField] private TanukiLine linesTutoDropInkOnShopToBuy;
    [SerializeField] private TanukiLine linesResetCraftingTimerEasyModeNoDefense;
    [SerializeField] private TanukiLine linesOnMerchantSpawn;
    [SerializeField] private TanukiLine linesOnWorkerSpawn;

    private Timer startShowTutoTimer;
    private Timer lastLineTimer;
    private Timer queueDelayTimer;
    private Queue<TanukiLine> dialogueQueue = new Queue<TanukiLine>();
    private bool isDisplayingText = false;

    public void ShowTanukiText(TanukiLine tanukiLine)
    {
        if(dialogueQueue.Count > 0 && !canHaveSameLineSuccessively && dialogueQueue.Peek() == tanukiLine)
            return;

        dialogueQueue.Enqueue(tanukiLine);

        if (!isDisplayingText)
            ProcessNextInQueue();
    }
    private void ProcessNextInQueue()
    {
        if (dialogueQueue.Count == 0)
        {
            isDisplayingText = false;
            return;
        }

        TanukiLine nextLine = dialogueQueue.Dequeue();

        DisplayTanukiLine(nextLine);
    }
    private void ProcessNextInQueueWithDelay()
    {
        if (dialogueQueue.Count == 0)
        {
            isDisplayingText = false;
            return;
        }

        queueDelayTimer = Timer.Register(timeBetweenTextInQueue, onComplete: ProcessNextInQueue);
    }
    private void DisplayTanukiLine(TanukiLine tanukiLine)
    {
        isDisplayingText = true;

        lastLineTimer?.Cancel();

        if(bubbleImageWorldSpace != null)
            bubbleImageWorldSpace.enabled = true;

        if(bubbleImageScreenSpace != null)
            bubbleImageScreenSpace.enabled = true;

        TanukiLineData tanukiLineData = tanukiLine.GetRandomTanukiLineData();

        if(tanukiImageScreenSpace != null)
            tanukiImageScreenSpace.enabled = true;

        if(tanukiImageWorldSpace != null && tanukiLineData.TanukiSprite != null)
            tanukiImageWorldSpace.sprite = tanukiLineData.TanukiSprite;

        if (tanukiImageScreenSpace != null && tanukiLineData.TanukiSprite != null)
            tanukiImageScreenSpace.sprite = tanukiLineData.TanukiSprite;

#if UNITY_WEBGL
        tanukiLineData.Line.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                scrollTextWithVoice.TypeText(handle.Result, tanukiTextWorldSpace, keepShowingDuration);
                scrollTextWithVoice.TypeText(handle.Result, tanukiTextScreenSpace, keepShowingDuration);
            }
        };
#endif

#if !UNITY_WEBGL
             scrollTextWithVoice.TypeText(tanukiLineData.Line.GetLocalizedString(), tanukiTextWorldSpace, keepShowingDuration);
              scrollTextWithVoice.TypeText(tanukiLineData.Line.GetLocalizedString(), tanukiTextScreenSpace, keepShowingDuration);
#endif

        lastLineTimer = Timer.Register(lastLineDurationThresholdForTeasing, onComplete: () => ShowTanukiText(linesTeasing));
    }

    private void Start()
    {
        tanukiTextWorldSpace.text = string.Empty;
        tanukiTextScreenSpace.text = string.Empty;
        bubbleImageWorldSpace.enabled = false;
        bubbleImageScreenSpace.enabled = false;
        tanukiImageScreenSpace.enabled = false;

        if (TutorialController.Instance.FirstTimePlaying)
        {
            startShowTutoTimer = Timer.Register(timeBeforeStartTutoText, () =>
            {
                ShowTanukiText(linesTutoSellForBooster);
                ShowTanukiText(linesTutoDropInkOnShopToBuy);
            });
        }

        scrollTextWithVoice.OnHideTextComplete += ScrollTextWithVoice_OnHideTextComplete;
        PlayerHealth.OnPlayerHit += PlayerHealth_OnPlayerHit;
        CardManager.Instance.OnMaxCardsReached += CardManager_OnMaxCardsReached;
        CardManager.Instance.OnMaxCardsDefenseReached += CardManager_OnMaxCardsDefenseReached;
        WaveManager.Instance.OnWaveEnd += WaveManager_OnWaveEnd;
        WaveManager.OnPlayerWasHitThisWave += WaveManager_OnPlayerWasHitThisWave;
        WaveManager.OnPlayerWasNotHitThisWave += WaveManager_OnPlayerWasNotHitThisWave;
        CraftingManager.Instance.OnHalfTimeCraftingMode += CraftingManager_OnHalfTimeCraftingMode;
        SuccessManager.Instance.FirstCraftSuccess.OnComplete += OnCompleteFirstCraft;
        SuccessManager.Instance.FirstFactorySuccess.OnComplete += OnCompleteFirstFactory;
        SuccessManager.Instance.CraftFirstDefenseSuccess.OnComplete += OnCompleteFirstDefense;
        SuccessManager.Instance.CraftTempleSuccess.OnComplete += OnCompleteCraftTemple;
        CraftingManager.OnResetCraftingManagerEasyModeNoDefense += CraftingManager_OnResetCraftingManagerEasyModeNoDefense;
        merchantSpawner.OnSpawnCard += MerchantSpawner_OnSpawnCard;
        workerSpawner.OnSpawnCard += WorkerSpawner_OnSpawnCard;
    }



    private void OnDestroy()
    {
        scrollTextWithVoice.OnHideTextComplete -= ScrollTextWithVoice_OnHideTextComplete;
        PlayerHealth.OnPlayerHit -= PlayerHealth_OnPlayerHit;
        if (CardManager.HasInstance)
        {
            CardManager.Instance.OnMaxCardsReached -= CardManager_OnMaxCardsReached;
            CardManager.Instance.OnMaxCardsDefenseReached -= CardManager_OnMaxCardsDefenseReached; 
        }

        if(WaveManager.HasInstance)
        {
            WaveManager.Instance.OnWaveEnd -= WaveManager_OnWaveEnd;
            WaveManager.OnPlayerWasHitThisWave -= WaveManager_OnPlayerWasHitThisWave;
            WaveManager.OnPlayerWasNotHitThisWave -= WaveManager_OnPlayerWasNotHitThisWave;
        }

        if (CraftingManager.HasInstance)
        {
            CraftingManager.Instance.OnHalfTimeCraftingMode -= CraftingManager_OnHalfTimeCraftingMode;
            CraftingManager.OnResetCraftingManagerEasyModeNoDefense -= CraftingManager_OnResetCraftingManagerEasyModeNoDefense;
        }
            

        if (SuccessManager.HasInstance)
        {
            SuccessManager.Instance.FirstCraftSuccess.OnComplete -= OnCompleteFirstCraft;
            SuccessManager.Instance.FirstFactorySuccess.OnComplete -= OnCompleteFirstFactory;
            SuccessManager.Instance.CraftFirstDefenseSuccess.OnComplete -= OnCompleteFirstDefense;
            SuccessManager.Instance.CraftTempleSuccess.OnComplete -= OnCompleteCraftTemple;
        }

        merchantSpawner.OnSpawnCard -= MerchantSpawner_OnSpawnCard;
        workerSpawner.OnSpawnCard -= WorkerSpawner_OnSpawnCard;

        lastLineTimer?.Cancel();
        queueDelayTimer?.Cancel();
        startShowTutoTimer?.Cancel();
        dialogueQueue.Clear();
    }

    private void ScrollTextWithVoice_OnHideTextComplete(TextMeshProUGUI textMeshProUGUI)
    {
        bubbleImageWorldSpace.enabled = false;
        bubbleImageScreenSpace.enabled = false;
        tanukiImageWorldSpace.sprite = defaultTanukiSprite;
        tanukiImageScreenSpace.enabled = false;
        ProcessNextInQueueWithDelay();
    }

    private void PlayerHealth_OnPlayerHit() => ShowTanukiText(linesWhenPlayerHit);
    private void CardManager_OnMaxCardsDefenseReached() => ShowTanukiText(linesMaxDefenseReached);
    private void CardManager_OnMaxCardsReached() => ShowTanukiText(linesMaxCardsReached);
    private void WaveManager_OnPlayerWasNotHitThisWave() => ShowTanukiText(linesAfterWaveNoHit);
    private void WaveManager_OnPlayerWasHitThisWave() => ShowTanukiText(linesAfterWaveHit);
    private void WaveManager_OnWaveEnd(int waveIndex)
    {
        if(waveIndex == 1)
            ShowTanukiText(linesStackShortcuts);
    }

    private void CraftingManager_OnResetCraftingManagerEasyModeNoDefense() => ShowTanukiText(linesResetCraftingTimerEasyModeNoDefense);

    private void CraftingManager_OnHalfTimeCraftingMode()
    {
        if (CardManager.Instance.CurrentNumberOfDefenseCards <= 0)
            ShowTanukiText(linesLateDefense);
    }

    private void OnCompleteCraftTemple(SuccessData successData) => ShowTanukiText(linesOnCraftTemple);
    private void OnCompleteFirstDefense(SuccessData successData) => ShowTanukiText(linesOnCraftFirstDefense);
    private void OnCompleteFirstFactory(SuccessData successData) => ShowTanukiText(linesOnFirstFactoryComplete);
    private void OnCompleteFirstCraft(SuccessData successData) => ShowTanukiText(linesOnFirstCraftComplete);
    private void MerchantSpawner_OnSpawnCard(CardID cardID)
    {
        if(cardID == CardID.MERCHANT)
            ShowTanukiText(linesOnMerchantSpawn);
    }
    private void WorkerSpawner_OnSpawnCard(CardID cardID)
    {
        if (cardID == CardID.WORKER)
            ShowTanukiText(linesOnWorkerSpawn);
    }
}
