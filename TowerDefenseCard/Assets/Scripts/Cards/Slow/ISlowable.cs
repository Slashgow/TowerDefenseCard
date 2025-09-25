using System;

public interface ISlowable
{
    bool IsSlowed { get; }
    float SlowDuration { get; }
    float SlowMultiplier { get; }
    void ApplySlow(float duration, float slowMultiplier);
    void RemoveSlow();

    event Action<float, float> OnSlowApplied;
    event Action OnSlowRemoved;
}
