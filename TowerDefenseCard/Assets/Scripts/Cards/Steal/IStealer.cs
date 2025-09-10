using System;
using UnityEngine;

public interface IStealer
{
    public Vector3 CarryOffset { get;}
    public IStealable CurrentStealable { get; }
    public LayerMask StealableLayerMask { get; }
    public float StealRange { get; }
    public void StealTarget();

    public event Action OnSteal;
}
