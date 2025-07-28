public interface IFearor
{
    float FearDuration { get; }
    float FearChance { get; }
    float FearRange { get; }
    float FearFieldOfView { get; }
    bool CanCauseFear { get; }

    void ApplyFearToTarget(IFearable target);
    void FearNearbyTargets();
}
