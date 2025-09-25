using System;
using UnityEngine;
using UnityTimer;


public class CombatMoveCondtionner : MonoBehaviour
{
    [SerializeField, Range(0f, 20f)] private float cooldownDurationBetweenMoves = 5f;
    public float CooldownDurationBetweenMoves => cooldownDurationBetweenMoves;

    [SerializeField] private CardMover cardMover;
    
    private BaseDamageor damageor;
    private BaseHealer healer;
    private SimpleSlower slower;

    private Timer cooldownTimer;

    public event Action OnStartCooldown;
    public event Action OnEndCooldown;
    public event Action<float> OnTickCooldown;

    private void Awake()
    {
        damageor = GetComponent<BaseDamageor>();
        healer = GetComponent<BaseHealer>();
        slower = GetComponent<SimpleSlower>();

        cardMover.OnPointerDownEvent += CardMover_OnPointerDownEvent;
        cardMover.OnPointerUpEvent += CardMover_OnPointerUpEvent;

        GameManager.Instance.OnStartCraftMode += OnStartCraftMode;
    }

    private void OnDestroy()
    {
        cardMover.OnPointerDownEvent -= CardMover_OnPointerDownEvent;
        cardMover.OnPointerUpEvent -= CardMover_OnPointerUpEvent;

        if(GameManager.HasInstance)
            GameManager.Instance.OnStartCraftMode -= OnStartCraftMode;

        cooldownTimer?.Cancel();
    }

    private void OnStartCraftMode()
    {
        if(cardMover != null)
            cardMover.enabled = true;

        if(damageor != null)
            damageor.StartAttack();
        else if(healer != null)
            healer.StartHeal();
        else if(slower != null)
            slower.SetCanCauseSlow(true);
    }

    private void CardMover_OnPointerUpEvent()
    {
        if(GameManager.Instance.CurrentGameMode != GameMode.COMBAT)
            return;

        if(damageor != null)
            damageor.StartAttack();
        else if(healer != null)
            healer.StartHeal();
        else if(slower != null)
            slower.SetCanCauseSlow(true);

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

        if(damageor != null)
            damageor.StopAttack();
        else if(healer != null)
            healer.StopHeal();
        else if(slower != null)
            slower.SetCanCauseSlow(false);
    }
}
