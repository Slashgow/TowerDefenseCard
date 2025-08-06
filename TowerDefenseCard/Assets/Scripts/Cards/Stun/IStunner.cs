public interface IStunner
{
    float StunCooldown { get; }
    float StunDuration { get; }
    float StunChance { get; }
    float StunRange { get; }
    float StunFieldOfView { get; }
    bool CanCauseStun { get; }

    void ApplyStunToTarget(IStunnable target);
    void StunNearbyTargets();
}

