using UnityEngine;

public interface IPushbacker
{
    float PushbackCooldown { get; }
    float PushbackDistance { get; }
    float PushbackChance { get; }
    float PushbackRange { get; }
    float PushbackFieldOfView { get; }
    bool CanCausePushback { get; }

    void ApplyPushbackToTarget(IPushbackable target, Vector3 direction);
    void PushbackNearbyTargets();
}
