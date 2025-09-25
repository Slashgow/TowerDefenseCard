public interface ISlower
{
    float SlowCooldown { get; }
    float SlowDuration { get; }
    float SlowMultiplier { get; }
    float SlowChance { get; }
    float SlowRange { get; }
    float SlowFieldOfView { get; }
    bool CanCauseSlow { get; }

    void ApplySlowToTarget(ISlowable target);
    void SlowNearbyTargets();
}