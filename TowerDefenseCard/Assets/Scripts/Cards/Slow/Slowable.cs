using System;
using UnityEngine;
using UnityTimer;

public class Slowable : MonoBehaviour, ISlowable
{
    [Header("Slow")]
    [SerializeField] private bool canBeSlowed = true;

    private bool isSlowed = false;
    private float currentSlowDuration = 0f;
    private float currentSlowMultiplier = 1f;
    private Timer slowTimer;
    private AutoCardMovement autoCardMovement;

    public bool IsSlowed => isSlowed;
    public float SlowDuration => currentSlowDuration;
    public float SlowMultiplier => currentSlowMultiplier;
    public event Action<float, float> OnSlowApplied;
    public event Action OnSlowRemoved;

    private void Awake()
    {
        autoCardMovement = GetComponent<AutoCardMovement>();
    }

    public void ApplySlow(float duration, float slowMultiplier)
    {
        if (!canBeSlowed)
            return;

        slowTimer?.Cancel();
        isSlowed = true;
        currentSlowDuration = duration;
        currentSlowMultiplier = slowMultiplier;

        if (autoCardMovement != null)
            autoCardMovement.ApplySlow(slowMultiplier);

        slowTimer = Timer.Register(duration, onComplete: RemoveSlow);
        OnSlowApplied?.Invoke(duration, slowMultiplier);
        Debug.Log($"{gameObject.name} is now slowed for {duration} seconds with {slowMultiplier}x multiplier");
    }

    public void RemoveSlow()
    {
        if (!isSlowed)
            return;

        isSlowed = false;
        currentSlowDuration = 0f;
        float previousMultiplier = currentSlowMultiplier;
        currentSlowMultiplier = 1f;
        slowTimer?.Cancel();

        if (autoCardMovement != null)
            autoCardMovement.RemoveSlow();

        OnSlowRemoved?.Invoke();
        Debug.Log($"{gameObject.name} is no longer slowed");
    }

    private void OnDestroy()
    {
        slowTimer?.Cancel();
    }
}