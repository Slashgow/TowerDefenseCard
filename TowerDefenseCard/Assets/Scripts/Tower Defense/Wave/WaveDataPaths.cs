using UnityEngine;
using System;
using SplineMesh;

[Serializable]
public class WaveDataPaths
{
    [SerializeField] private WaveData waveData;
    public WaveData WaveData => waveData;

    [SerializeField] private Spline[] paths;
    public Spline[] Paths => paths;
}
