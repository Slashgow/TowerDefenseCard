using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using UnityTimer;

public class UIPageGameModeTransition : UIPage
{
    [Header("References")]
    [SerializeField] private Logger logger;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI transitionText;
    [SerializeField] private LocalizedString startCombatPhaseLocalizedString, startCraftPhaseLocalizedString, WaveLocalizedString;
    [SerializeField] private LocalizedString defeatAllWavesLocalizedString, playerDieLocalizedString;
    [SerializeField] private UIPageController uIPageController;
    [SerializeField] private UITime uiTime;
    [SerializeField] private UIInput uiInput;

    [Header("Transition")]
    [SerializeField, Range(0f, 5f)] public float transitionDuration;

    [Header("Tween Background")]
    [SerializeField, Range(0f,3f)] public float backgroundDuration = 0.5f;
    [SerializeField] private Ease backgroundEase = Ease.InOutSine;
    [SerializeField, Range(0f, 1f)] private float startAlpha;
    [SerializeField, Range(0f, 1f)] private float endAlpha;

    [Header("Tween Text Size")]
    [SerializeField] TextSizeEffect textSizeEffect;


    private void Start()
    {
        GameManager.Instance.OnEndCraftMode += GameManager_OnStartCombatMode;
        GameManager.Instance.OnStartCraftMode += GameManager_OnStartCraftMode;
        GameManager.Instance.OnDefeatAllWaves += GameManager_OnDefeatAllWaves;
        PlayerHealth.OnPlayerDie += PlayerHealth_OnPlayerDie;
    }



    private void OnDestroy()
    {
        if (GameManager.HasInstance)
        {
            GameManager.Instance.OnEndCraftMode -= GameManager_OnStartCombatMode;
            GameManager.Instance.OnStartCraftMode -= GameManager_OnStartCraftMode;
            GameManager.Instance.OnDefeatAllWaves -= GameManager_OnDefeatAllWaves;
        }
        PlayerHealth.OnPlayerDie -= PlayerHealth_OnPlayerDie;
    }

    private void GameManager_OnStartCraftMode() => DoTransitionEffect(startCraftPhaseLocalizedString.GetLocalizedString());
    private void GameManager_OnStartCombatMode()
    {
        string text = $"{WaveLocalizedString.GetLocalizedString()} {WaveManager.Instance.CurrentWaveIndex + 1} / {(WaveManager.Instance.EnableEndlessMode ? "-" : WaveManager.Instance.NumberOfWaves)} \n";
        text += startCombatPhaseLocalizedString.GetLocalizedString();
        DoTransitionEffect(text);
    }

    private void GameManager_OnDefeatAllWaves() => DoTransitionEffect(defeatAllWavesLocalizedString.GetLocalizedString());
    private void PlayerHealth_OnPlayerDie() => DoTransitionEffect(playerDieLocalizedString.GetLocalizedString());

    private void DoTransitionEffect(string text)
    {
        Timer.Register(transitionDuration, onComplete: () =>
        {
           DoFadeOutEffect(text);
        }, useRealTime: true);

        GameSettingsManager.Instance.AllowTemporaryPause();
        uiTime.OnPause();
        uiInput.StopRecevingInput();
        uIPageController.ShowPage(this);
        FadeBackgroundAlpha(startAlpha, endAlpha);
        SetTransitionText(text);
        textSizeEffect.DoEffect();
    }

    private void DoFadeOutEffect(string text)
    {
        Timer.Register(textSizeEffect.TextSizeDuration, onComplete: () =>
        {
            GameSettingsManager.Instance.BackToPreviousPauseSetting();
            uiInput.StartReceivingInput();
            uiTime.OnResume(true);
            //Hide();
            uIPageController.ShowGamePage();
            GameManager.Instance.CurrentGameState = GameState.PLAY;
            if (GameManager.Instance.isFinished || PlayerHealth.IsPlayerDead)
            {
                SceneLoader.Instance.LoadNextSceneAsync();
            }

            logger.Log("Transition Finished", this);
        }, useRealTime: true);

        FadeBackgroundAlpha(endAlpha, startAlpha);
        SetTransitionText(text);
        textSizeEffect.ReverseEffect();
    }

    private void FadeBackgroundAlpha(float startAlpha, float endAlpha)
    {
        Color backgroundColor = backgroundImage.color;
        backgroundColor.a = startAlpha;
        backgroundImage.color = backgroundColor;

        backgroundImage.DOFade(endAlpha, backgroundDuration).
            SetEase(backgroundEase).
            SetUpdate(true);
    }

    private void SetTransitionText(string text) => transitionText.text = text;

}
