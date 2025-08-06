using System;
using UnityEngine;
using UnityTimer;

public class Stunnable : MonoBehaviour, IStunnable
{
    [Header("Stun")]
    [SerializeField] private bool canBeStunned = true;

    private bool isStunned = false;
    private float currentStunDuration = 0f;
    private Timer stunTimer;
    private AutoCardMovement autoCardMovement;
    private BaseDamageor damageor;

    public bool IsStunned => isStunned;
    public float StunDuration => currentStunDuration;
    public event Action<float> OnStunApplied;
    public event Action OnStunRemoved;

    private void Awake()
    {
        autoCardMovement = GetComponent<AutoCardMovement>();
        damageor = GetComponent<BaseDamageor>();
    }

    public void ApplyStun(float duration)
    {
        if (!canBeStunned)
            return;

        stunTimer?.Cancel();
        isStunned = true;
        currentStunDuration = duration;

        if (autoCardMovement != null)
            autoCardMovement.StopMoving();

        if(damageor != null)
            damageor.StopAttack();

        stunTimer = Timer.Register(duration, onComplete : RemoveStun);
        OnStunApplied?.Invoke(duration);
        Debug.Log($"{gameObject.name} is now stunned for {duration} seconds");
    }

    public void RemoveStun()
    {
        if (!isStunned)
            return;

        isStunned = false;
        currentStunDuration = 0f;
        stunTimer?.Cancel();

        if (autoCardMovement != null)
            autoCardMovement.StartMoving();

        if (damageor != null)
            damageor.StartAttack();

        OnStunRemoved?.Invoke();
        Debug.Log($"{gameObject.name} is no longer stunned");
    }

    private void OnDestroy()
    {
        stunTimer?.Cancel();
    }
}