using UnityEngine;

[RequireComponent(typeof(CanvasRenderer))]
public class UIOutlineUnscaled : UIOutline
{
    private void Update()
    {
        materialForRendering.SetFloat("_UnscaledTime", Time.unscaledTime);
    }
}
