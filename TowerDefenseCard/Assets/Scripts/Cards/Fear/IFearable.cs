using System;

public interface IFearable
{
    bool IsFeared { get; }
    float FearDuration { get; }
    void ApplyFear(float duration);
    void RemoveFear();

    event Action<float> OnFearApplied;
    event Action OnFearRemoved;
}
