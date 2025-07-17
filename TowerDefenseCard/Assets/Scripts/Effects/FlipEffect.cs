using System;
using DG.Tweening;
using UnityEngine;

public class FlipEffect : MonoBehaviour
{
    [SerializeField, Range(0f, 5f)] private float duration;
    [SerializeField] private AnimationCurve curve;
    [SerializeField, Range(0f,180f)] private float startXRotationn, endXRotation;

    private Tween flipTween;
    private void OnEnable()
    {
        Flip();
    }

    private void Flip()
    {
        this.transform.localEulerAngles = new Vector3(startXRotationn, this.transform.localEulerAngles.y, this.transform.localEulerAngles.z);
        this.transform.DOLocalRotate(new Vector3(endXRotation, this.transform.localEulerAngles.y, this.transform.localEulerAngles.z), duration, RotateMode.WorldAxisAdd).SetUpdate(true);
    }

   
}
