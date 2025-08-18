using System;
using DG.Tweening;
using UnityEngine;

public class RotateObject : Effect
{
    [SerializeField, Range(0f, 10f)] private float rotationDuration = 1f; 
    [SerializeField] private AnimationCurve rotationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField, Range(-360f, 360f)] private float startAngle = 0f; 
    [SerializeField, Range(-360f, 360f)] private float endAngle = 180f; 
    [SerializeField] private RotateMode rotateMode = RotateMode.Fast; 

    private Tween rotationTween;
    public void Rotate()
    {
        rotationTween?.Kill();

        Vector3 currentRotation = transform.localEulerAngles;
        Vector3 normalizedAxis = rotationAxis.normalized;

        Vector3 startOffset = normalizedAxis * startAngle;
        Vector3 endOffset = normalizedAxis * endAngle;

        transform.localEulerAngles = currentRotation + startOffset;

        rotationTween = transform.DOLocalRotate(currentRotation + endOffset, rotationDuration, rotateMode)
            .SetEase(rotationCurve)
            .SetRelative(false)
            .SetUpdate(true);
    }

    public override void DoEffect() => Rotate();

    private void OnDisable()
    {
        rotationTween?.Kill();
        this.transform.rotation = Quaternion.identity;
    }

}
