using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextSizeEffect : Effect
{
    [SerializeField] private TextMeshProUGUI transitionText;

    [Header("Tween Text Size")]
    [SerializeField, Range(0f, 3f)] public float textSizeDuration = 0.5f;
    public float TextSizeDuration => textSizeDuration;
    [SerializeField] private AnimationCurve textSizeEase;
    [SerializeField, Range(0f, 200f)] private float startTextSize;
    [SerializeField, Range(0f, 200f)] private float endTextSize;

    private float timeElapsed;
    private Coroutine textSizeCoroutine;
    public override void DoEffect() => LerpTextSize(startTextSize, endTextSize);

    public void ReverseEffect() => LerpTextSize(endTextSize, startTextSize);

    private void LerpTextSize(float startTextSize, float endTextSize)
    {
        transitionText.fontSize = startTextSize;
        if (textSizeCoroutine != null)
        {
            StopCoroutine(textSizeCoroutine);
            textSizeCoroutine = null;
        }
        textSizeCoroutine = StartCoroutine(LerpTextSizeCoroutine(startTextSize, endTextSize));
    }

    private IEnumerator LerpTextSizeCoroutine(float startTextSize, float endTextSize)
    {
        timeElapsed = 0f;
        while (timeElapsed <= textSizeDuration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            timeElapsed = Mathf.Clamp(timeElapsed, 0, textSizeDuration);
            transitionText.fontSize = Mathf.Lerp(startTextSize, endTextSize, textSizeEase.Evaluate(timeElapsed / textSizeDuration));
            yield return null;
        }
    }
}
