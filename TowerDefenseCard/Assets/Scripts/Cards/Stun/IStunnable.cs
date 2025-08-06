using System;

public interface IStunnable
{
    bool IsStunned { get; }
    float StunDuration { get; }
    void ApplyStun(float duration);
    void RemoveStun();

    event Action<float> OnStunApplied;
    event Action OnStunRemoved;
}
