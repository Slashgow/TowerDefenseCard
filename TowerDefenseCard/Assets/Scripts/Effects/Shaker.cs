using UnityEngine;
using System.Collections;

public class Shaker : Effect
{
    [SerializeField, Range(0f, 2f)] private float shakeDuration = 0.3f;
    [SerializeField, Range(0f, 2f)] private float shakeMagnitude = 0.1f;
    [SerializeField] private AnimationCurve shakeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    private Vector3 originalPosition;
    private Coroutine coroutine;

    public override void DoEffect() => Shake();

    public void Shake()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        coroutine = StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        originalPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float curveValue = shakeCurve.Evaluate(elapsedTime / shakeDuration);
            float xOffset = Random.Range(-1f, 1f) * shakeMagnitude * curveValue;
            float yOffset = Random.Range(-1f, 1f) * shakeMagnitude * curveValue;

            transform.position = originalPosition + new Vector3(xOffset, yOffset, 0f);
            yield return null;
        }

        transform.position = originalPosition;
    }
}
