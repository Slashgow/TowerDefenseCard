using UnityEngine;
using UnityEngine.Splines;
using System;

[Serializable]
public class WaveDataPaths
{
    [SerializeField] private WaveData waveData;
    public WaveData WaveData => waveData;

    [SerializeField] private SplineContainer[] paths;
    public SplineContainer[] Paths => paths;
}
