using UnityEngine;
using UnityTimer;

public class SimpleHealer : BaseHealer
{
    [Header("Healer Specific")]
    [SerializeField] private bool autoHeal = true;
    public bool AutoHeal => autoHeal;

    private Timer autoHealTimer;

    protected override void Start()
    {
        base.Start();

        if (autoHeal)
            StartAutoHealTimer();
    }

    private void StartAutoHealTimer() => autoHealTimer = Timer.Register(HealCooldown, onComplete: AutoHealNearbyTargets, isLooped: true);
    public override bool CanHeal() => IsCooldownReady();
    public override void PerformHeal(IHealable target)
    {
        if (target != null && CanHeal())
            base.PerformHeal(target);
    }

    private void AutoHealNearbyTargets()
    {
        if (!autoHeal || !CanHeal())
            return;

        var targets = FindHealableTargetsInRange();

        IHealable bestTarget = FindBestTarget(targets);

        if (bestTarget != null)
            PerformHeal(bestTarget);
    }

    private IHealable FindBestTarget(IHealable[] targets)
    {
        IHealable bestTarget = null;
        float lowestHealthPercent = 1f;

        foreach (var target in targets)
        {
            float healthPercent = target.CurrentHealth / target.MaxHealth;
            if (healthPercent < lowestHealthPercent && healthPercent < 1f)
            {
                lowestHealthPercent = healthPercent;
                bestTarget = target;
            }
        }

        return bestTarget;
    }

    public void SetAutoHeal(bool enabled)
    {
        autoHeal = enabled;

        if (autoHeal && (autoHealTimer == null || autoHealTimer.isCompleted))
            StartAutoHealTimer();

        else if (!autoHeal && autoHealTimer != null)
            autoHealTimer.Cancel();
    }


    public void TriggerHeal()
    {
        if (!CanHeal()) 
            return;

        var targets = FindHealableTargetsInRange();

        if (targets.Length > 0)
            PerformHeal(targets[0]);
    }

    private void OnDestroy()
    {
        cooldownTimer?.Cancel();
        autoHealTimer?.Cancel();
    }

    protected override void OnHealPerformed(IHealable target)
    {
        base.OnHealPerformed(target);
    }
}
