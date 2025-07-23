using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ChromaticAberrationEffect : Effect
{
    [Header("Parameters")]
    [SerializeField, Range(0f, 5f)] private float duration = 0.5f;
    [SerializeField] private bool lerpIntensity;
    [SerializeField, Range(0f, 1f)] private float startIntensity = 0f;
    [SerializeField, Range(0f, 1f)] private float endIntensity = 0.5f;
    [SerializeField] private bool disableChromaticAberrationOnComplete;
    [SerializeField] private bool yoyo;

    [Header("References")]
    [SerializeField] private Volume globalVolume;

    private ChromaticAberration chromaticAberration;
     private void Start()
     {
        if (globalVolume.profile.TryGet<ChromaticAberration>(out chromaticAberration))
        {

        }
     }


    public override void DoEffect()
    {
        if (lerpIntensity)
        {
            chromaticAberration.intensity.value = startIntensity;
            chromaticAberration.active = true;

            if (!yoyo)
            {
                DOTween.To(() => chromaticAberration.intensity.value,
                      x => chromaticAberration.intensity.value = x,
                      endIntensity,
                      duration).OnComplete(() =>
                      {
                          if (disableChromaticAberrationOnComplete)
                              chromaticAberration.active = false;
                      });
            }
            else
            {
                DOTween.To(() => chromaticAberration.intensity.value,
                      x => chromaticAberration.intensity.value = x,
                      endIntensity,
                      duration).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
                      {
                          if (disableChromaticAberrationOnComplete)
                              chromaticAberration.active = false;
                      });
            }
            
        }
    }
}
