using UnityEngine;
using System.Collections;

[RequireComponent(typeof(IDamageable))]
public class HitShaker : MonoBehaviour
{
    [SerializeField, Range(0f,2f)] private float shakeDuration = 0.3f; 
    [SerializeField, Range(0f,2f)] private float shakeMagnitude = 0.1f; 
    [SerializeField] private AnimationCurve shakeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f); 

    private IDamageable damageable;
    private Vector3 originalPosition;
    private Coroutine coroutine;

    private void Awake() => damageable = GetComponent<IDamageable>();
    private void OnEnable() => damageable.OnTakeDamage += Damageable_OnTakeDamage;
    private void OnDisable() => damageable.OnTakeDamage -= Damageable_OnTakeDamage;
    private void Damageable_OnTakeDamage(float currentHealth) => ShakeOnHit();
    public void ShakeOnHit()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        coroutine = StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        originalPosition = transform.position; 
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            elapsedTime += Time.deltaTime;
            float curveValue = shakeCurve.Evaluate(elapsedTime / shakeDuration); 
            float xOffset = Random.Range(-1f, 1f) * shakeMagnitude * curveValue;
            float yOffset = Random.Range(-1f, 1f) * shakeMagnitude * curveValue;

            transform.position = originalPosition + new Vector3(xOffset, yOffset, 0f);
            yield return null;
        }

        transform.position = originalPosition;
    }
}