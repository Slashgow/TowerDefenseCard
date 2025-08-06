using System;
using UnityEngine;

public interface IPushbackable
{
    bool IsPushingBack { get; }
    bool CanBePushedBack { get; }
    float PushbackTime { get; }
    void ApplyPushback(float distance, Vector3 direction);

    event Action<float, Vector3> OnPushbackApplied;
}

