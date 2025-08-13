using UnityEngine;
using System;
using SplineMesh;

[Serializable]
public class SplineData
{
    [SerializeField] private SplineID splineID;
    [SerializeField] private Spline spline;
    public SplineID SplineID => splineID;
    public Spline Spline => spline;
}
