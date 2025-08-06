using UnityEngine;
using DG.Tweening;

public class Shaker : Effect
{
    [SerializeField, Range(0f, 2f)] private float shakeDuration = 0.3f;
    [SerializeField, Range(0f, 10f)] private float shakeMagnitude = 0.1f;
    [SerializeField] private AnimationCurve shakeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    [SerializeField, Range(0, 20)] private int shakeVibrato = 10;
    [SerializeField, Range(0f,180f)] private float shakeRandomness = 90f;

    private float originalRotation;
    private Tween shakeTween;

    private ContinuousCardMovement continuousCardMovement;
    private void Awake() => continuousCardMovement = GetComponent<ContinuousCardMovement>();

    public override void DoEffect() => Shake();

    public void Shake()
    {
        if (continuousCardMovement != null)
            continuousCardMovement.StopTilt();

        shakeTween?.Kill();

        originalRotation = transform.eulerAngles.z;

        shakeTween = transform.DOShakeRotation(
            shakeDuration,
            new Vector3(0, 0, shakeMagnitude),
            shakeVibrato,
            shakeRandomness,
            randomnessMode : ShakeRandomnessMode.Harmonic)
            .SetEase(shakeCurve)
            .OnComplete(() => {
                transform.rotation = Quaternion.Euler(0, 0, originalRotation);

                if(continuousCardMovement != null)
                    continuousCardMovement.StartTilt();

                });
    }

    private void OnDestroy()
    {
        shakeTween?.Kill();
    }
}
