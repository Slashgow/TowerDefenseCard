using UnityEngine;

[RequireComponent(typeof(CanvasRenderer))]
public class UIOutlineUnscaled : UIOutline
{
    [SerializeField] private bool rebuildOnUpdate = false;

    private void Update()
    {
        materialForRendering.SetFloat("_UnscaledTime", Time.unscaledTime);

        if(rebuildOnUpdate)
            SetVerticesDirty(); 
    }
}
