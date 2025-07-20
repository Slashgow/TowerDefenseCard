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
    [SerializeField] private LocalizedString startCombatPhaseLocalizedString, startCraftPhaseLocalizedString;
    [SerializeField] private UIPageController uIPageController;
    [SerializeField] private UITime uITime;

    [Header("Transition")]
    [SerializeField, Range(0f, 5f)] public float transitionDuration;

    [Header("Tween Background")]
    [SerializeField, Range(0f,3f)] public float backgroundDuration = 0.5f;
    [SerializeField] private Ease backgroundEase = Ease.InOutSine;
    [SerializeField, Range(0f, 1f)] private float startAlpha;
    [SerializeField, Range(0f, 1f)] private float endAlpha;

    [Header("Tween Text Size")]
    [SerializeField] TextSizeEffect textSizeEffect;


    private IEnumerator Start()
    {
        GameManager.Instance.OnStartCombatMode += GameManager_OnStartCombatMode;
        GameManager.Instance.OnStartCraftMode += GameManager_OnStartCraftMode;

        yield return new WaitForSeconds(3f);
        DoTransitionEffect(true);
    }

    private void OnDestroy()
    {
        if (GameManager.HasInstance)
        {
            GameManager.Instance.OnStartCombatMode -= GameManager_OnStartCombatMode;
            GameManager.Instance.OnStartCraftMode -= GameManager_OnStartCraftMode;
        }
    }

    private void GameManager_OnStartCraftMode() => DoTransitionEffect(true);
    private void GameManager_OnStartCombatMode() => DoTransitionEffect(false);

    private void DoTransitionEffect(bool isStartCraftMode)
    {
        Timer.Register(transitionDuration, onComplete: () =>
        {
           DoFadeOutEffect(isStartCraftMode);
        }, useRealTime: true);

        uITime.OnPause();
        uIPageController.ShowPage(this);
        FadeBackgroundAlpha(startAlpha, endAlpha);
        SetTransitionText(isStartCraftMode);
        textSizeEffect.DoEffect();
    }

    private void DoFadeOutEffect(bool isStartCraftMode)
    {
        Timer.Register(textSizeEffect.TextSizeDuration, onComplete: () =>
        {
            uITime.OnResume();
            Hide();
            logger.Log("Transition Finished", this);
        }, useRealTime: true);

        FadeBackgroundAlpha(endAlpha, startAlpha);
        SetTransitionText(isStartCraftMode);
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


    private void SetTransitionText(bool isStartCraftMode)
    {
        if (isStartCraftMode)
            transitionText.text = startCraftPhaseLocalizedString.GetLocalizedString();
        else
            transitionText.text = startCombatPhaseLocalizedString.GetLocalizedString();
    }

}
