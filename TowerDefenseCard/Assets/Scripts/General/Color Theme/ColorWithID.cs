using UnityEngine;
using System;

[Serializable]
public struct ColorWithID
{
    [SerializeField] private ColorID colorID;
    [SerializeField] private Color color;
    public ColorID ColorID => colorID;
    public Color Color => color;
}
