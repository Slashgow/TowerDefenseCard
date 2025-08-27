using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TanukiLine
{
    [SerializeField] private List<TanukiLineData> lines = new List<TanukiLineData>();
    public TanukiLineData GetRandomTanukiLineData() => lines[UnityEngine.Random.Range(0, lines.Count)];
}
