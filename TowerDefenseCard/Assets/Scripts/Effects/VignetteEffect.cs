using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class VignetteEffect : Effect
{
    [Header("Parameters")]
    [SerializeField, Range(0f, 5f)] private float duration = 0.5f;

    [SerializeField] private bool lerpColor;
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    [SerializeField] private bool shouldGoBackToOriginColor;

    [SerializeField] private bool lerpIntensity;
    [SerializeField, Range(0f, 1f)] private float startIntensity = 0f;
    [SerializeField, Range(0f, 1f)] private float endIntensity = 0.5f;

    [SerializeField] private bool lerpSmoothness;
    [SerializeField, Range(0f, 1f)] private float startSmoothness = 0f;
    [SerializeField, Range(0f, 1f)] private float endSmoothness = 0.5f;

    [SerializeField] private Ease ease = Ease.Linear;
    [SerializeField] private bool disableVignetteOnComplete;

    [Header("References")]
    [SerializeField] private Volume globalVolume;
    private Vignette vignette;

    private Tween colorTween;

    void Start()
    {
        if (globalVolume.profile.TryGet<Vignette>(out vignette))
        {

        }
    }


    public override void DoEffect()
    {
        if (vignette == null)
            return;

        if (lerpColor)
        {
            vignette.color.value = startColor;
            vignette.active = true;

            if(colorTween != null )
            {
                colorTween = null;
                colorTween.Kill();
            }

            colorTween = DOTween.To(() => vignette.color.value,
                                     x => vignette.color.value = x,
                                     endColor,
                                     duration).SetLoops(2,LoopType.Yoyo).OnComplete(() =>
                                     {
                                         if (disableVignetteOnComplete)
                                             vignette.active = false;
                                     });
        }

        if (lerpIntensity)
        {
            vignette.intensity.value = startIntensity;
            vignette.active = true;

            DOTween.To(() => vignette.intensity.value,
                      x => vignette.intensity.value = x,
                      endIntensity,
                      duration).OnComplete(() =>
                      {
                          if (disableVignetteOnComplete)
                              vignette.active = false;
                      });
        }

        if (lerpSmoothness)
        {
            vignette.smoothness.value = startSmoothness;
            vignette.active = true;

            DOTween.To(() => vignette.smoothness.value,
                      x => vignette.smoothness.value = x,
                      endSmoothness,
                      duration).OnComplete(() =>
                      {
                          if (disableVignetteOnComplete)
                              vignette.active = false;
                      }); ;
        }
    }

    private void OnDestroy()
    {
        if(colorTween != null)
            colorTween.Kill();
    }
}
