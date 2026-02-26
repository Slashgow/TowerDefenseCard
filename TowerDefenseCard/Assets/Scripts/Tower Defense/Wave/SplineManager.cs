using UnityEngine;
using System.Linq;
using SplineMesh;

public class SplineManager : MonoSingleton<SplineManager>
{
    [SerializeField] private SplineData[] splineDatas;
    public Spline GetSplineByID(SplineID splineID) => splineDatas.FirstOrDefault(s => s.SplineID == splineID).Spline;
    public SplineData GetSplineDataByID(SplineID splineID) => splineDatas.FirstOrDefault(s => s.SplineID == splineID);

    public void ToggleVisuals(SplineID[] splineIDs, bool enable)
    {
        foreach (var splineID in splineIDs)
        {
            GetSplineByID(splineID).gameObject.SetActive(enable);
        }
    }

    public void HideAllVisuals()
    {
        foreach (var splineData in splineDatas)
        {
            splineData.Spline.gameObject.SetActive(false);
        }
    }
}
