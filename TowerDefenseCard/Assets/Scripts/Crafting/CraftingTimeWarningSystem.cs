using UnityEngine;
using System;
using UnityEngine.Events;

public class CraftingTimeWarningSystem : MonoBehaviour
{
    [SerializeField] private float warningThreshold = 10f;
    [SerializeField] private UnityEvent OnSecondTickUnityEvent;
    public event Action<int> OnSecondTick;
    public event Action OnWarningThresholdReached;

    private int lastSecondTriggered = -1;
    private bool hasReachedThreshold = false;
    private bool isWarningActive = false;

    private void Start()
    {
        if (CraftingManager.HasInstance)
        {
            CraftingManager.Instance.OnTickTimeCraftingMode += HandleTimeTick;
            CraftingManager.Instance.OnStartCraftTimer += ResetWarningSystem;
        }
    }

    private void OnDestroy()
    {
        if (CraftingManager.HasInstance)
        {
            CraftingManager.Instance.OnTickTimeCraftingMode -= HandleTimeTick;
            CraftingManager.Instance.OnStartCraftTimer -= ResetWarningSystem;
        }
    }

    private void HandleTimeTick(float timeElapsed)
    {
        if (!CraftingManager.HasInstance) 
            return;

        float remainingTime = CraftingManager.Instance.RemainingTime;

        if (remainingTime <= warningThreshold && !hasReachedThreshold)
        {
            hasReachedThreshold = true;
            isWarningActive = true;
            OnWarningThresholdReached?.Invoke();
            Debug.Log($"[CraftingWarning] Warning threshold reached! {remainingTime:F1} seconds remaining");
        }

        if (isWarningActive && remainingTime > 0)
        {
            int currentSecond = Mathf.CeilToInt(remainingTime);

            if (currentSecond != lastSecondTriggered && currentSecond <= warningThreshold)
            {
                lastSecondTriggered = currentSecond;
                OnSecondTick?.Invoke(currentSecond);
                OnSecondTickUnityEvent?.Invoke();
                Debug.Log($"[CraftingWarning] {currentSecond} second(s) remaining");
            }
        }
    }

    private void ResetWarningSystem()
    {
        lastSecondTriggered = -1;
        hasReachedThreshold = false;
        isWarningActive = false;
        Debug.Log("[CraftingWarning] Warning system reset");
    }

    public void SetWarningThreshold(float threshold)
    {
        warningThreshold = threshold;
    }

    public bool IsInWarningPeriod()
    {
        return isWarningActive;
    }
}