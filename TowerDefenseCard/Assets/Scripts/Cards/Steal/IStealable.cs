using System;
using UnityEngine;

public interface IStealable
{
    public bool IsStolen { get; }
    public void ApplySteal(Transform stealerTransform, Vector3 offset);
    public void RemoveSteal();
    public void Destroy();

    public event Action OnStealApplied;
    public event Action OnStealRemoved;
}
