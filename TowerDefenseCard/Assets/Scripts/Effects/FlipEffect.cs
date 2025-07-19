using System;
using DG.Tweening;
using UnityEngine;

public class FlipEffect : Effect
{
    [SerializeField, Range(0f, 5f)] private float duration;
    [SerializeField] private Axis axis;
    [SerializeField, Range(-180f,360f)] private float startRotation, endRotation;
    [SerializeField] private AnimationCurve ease;
    [SerializeField] private RotateMode rotateMode;

    private void Flip()
    {
        switch (axis)
        {
            case Axis.X:
                this.transform.localEulerAngles = new Vector3(startRotation, 0, 0);
                this.transform.DOLocalRotate(new Vector3(endRotation, this.transform.localEulerAngles.y, this.transform.localEulerAngles.z), duration, rotateMode).
                    SetEase(ease).SetUpdate(true);
                break;
            case Axis.Y:
                this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, startRotation, this.transform.localEulerAngles.z);
                this.transform.DOLocalRotate(new Vector3(this.transform.localEulerAngles.x, endRotation, this.transform.localEulerAngles.z), duration, rotateMode).
                    SetEase(ease).SetUpdate(true); 
                break;
            case Axis.Z:
                this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, this.transform.localEulerAngles.y, startRotation);
                this.transform.DOLocalRotate(new Vector3(this.transform.localEulerAngles.x, this.transform.localEulerAngles.y, endRotation), duration, rotateMode).
                    SetEase(ease).SetUpdate(true);
                break;
            default:
                break;
        }
    }

    public override void DoEffect() => Flip();
}
