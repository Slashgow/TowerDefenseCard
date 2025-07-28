using System;
using UnityEngine;
using UnityTimer;

public class Fearable : MonoBehaviour, IFearable
{
    [Header("Fear")]
    [SerializeField] private bool canBeFeared = true;

    private bool isFeared = false;
    private float currentFearDuration = 0f;
    private Timer fearTimer;
    private BaseDamageor damageor;

    public bool IsFeared => isFeared;
    public float FearDuration => currentFearDuration;
    public event Action<float> OnFearApplied;
    public event Action OnFearRemoved;

    private void Awake() => damageor = GetComponent<BaseDamageor>();

    public void ApplyFear(float duration)
    {
        if (!canBeFeared)
            return;

        fearTimer?.Cancel();
        isFeared = true;
        currentFearDuration = duration;

        if (damageor != null)
            damageor.enabled = false;

        fearTimer = Timer.Register(duration, RemoveFear);
        OnFearApplied?.Invoke(duration);
        Debug.Log($"{gameObject.name} is now feared for {duration} seconds");
    }

    public void RemoveFear()
    {
        if (!isFeared)
            return;

        isFeared = false;
        currentFearDuration = 0f;
        fearTimer?.Cancel();

        if (damageor != null)
            damageor.enabled = true;

        OnFearRemoved?.Invoke();
        Debug.Log($"{gameObject.name} is no longer feared");
    }

    private void OnDestroy()
    {
        fearTimer?.Cancel();
    }
}
