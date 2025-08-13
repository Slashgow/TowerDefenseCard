using UnityEngine;
using System;

[Serializable]
public class WaveDataPaths
{
    [SerializeField] private WaveData waveData;
    public WaveData WaveData => waveData;

    [SerializeField] private SplineID[] paths;
    public SplineID[] Paths => paths;
}
