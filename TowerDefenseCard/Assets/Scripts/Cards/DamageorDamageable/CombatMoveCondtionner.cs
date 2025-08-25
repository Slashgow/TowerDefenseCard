using System;
using UnityEngine;
using UnityTimer;


public class CombatMoveCondtionner : MonoBehaviour
{
    [SerializeField, Range(0f, 20f)] private float cooldownDurationBetweenMoves = 5f;
    public float CooldownDurationBetweenMoves => cooldownDurationBetweenMoves;

    [SerializeField] private CardMover cardMover;
    
    private BaseDamageor damageor;

    private Timer cooldownTimer;

    public event Action OnStartCooldown;
    public event Action OnEndCooldown;
    public event Action<float> OnTickCooldown;

    private void Awake()
    {
        damageor = GetComponent<BaseDamageor>();

        cardMover.OnPointerDownEvent += CardMover_OnPointerDownEvent;
        cardMover.OnPointerUpEvent += CardMover_OnPointerUpEvent;

        GameManager.Instance.OnStartCraftMode += OnStartCraftMode;
    }

    private void OnDestroy()
    {
        cardMover.OnPointerDownEvent -= CardMover_OnPointerDownEvent;
        cardMover.OnPointerUpEvent -= CardMover_OnPointerUpEvent;

        cooldownTimer?.Cancel();
    }

    private void OnStartCraftMode()
    {
        cardMover.enabled = true;
        damageor.StartAttack();
    }

    private void CardMover_OnPointerUpEvent()
    {
        if(GameManager.Instance.CurrentGameMode != GameMode.COMBAT)
            return;

        damageor.StartAttack();
        cardMover.DisableAutoMoveOnEnable();
        cardMover.enabled = false;
        OnStartCooldown?.Invoke();
        cooldownTimer = Timer.Register(cooldownDurationBetweenMoves, onComplete: () => {
            cardMover.enabled = true;
            OnEndCooldown?.Invoke();
        }, onUpdate: timeElapsed => OnTickCooldown?.Invoke(timeElapsed));
    }

    private void CardMover_OnPointerDownEvent()
    {
        if (GameManager.Instance.CurrentGameMode != GameMode.COMBAT)
            return;

        damageor.StopAttack();
    }
}
