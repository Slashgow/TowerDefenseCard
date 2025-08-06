using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Pushbackable : MonoBehaviour, IPushbackable
{
    [Header("Pushback")]
    [SerializeField] private bool canBePushedBack = true;
    [SerializeField, Range(0f,3f)] private float pushbackTime = 1f;
    [SerializeField] private Ease pushbackEase = Ease.OutQuad;
    [SerializeField] private Ease returnEase = Ease.InOutQuad;

    private AutoCardMovement autoCardMovement;
    private bool isPushingBack = false;
    private Sequence pushbackSequence;

    public bool IsPushingBack => isPushingBack;
    public bool CanBePushedBack => canBePushedBack;
    public float PushbackTime => pushbackTime;
    public event Action<float, Vector3> OnPushbackApplied;

    private void Awake() => autoCardMovement = GetComponent<AutoCardMovement>();

    public void ApplyPushback(float distance, Vector3 direction)
    {
        if (!canBePushedBack || autoCardMovement == null || isPushingBack)
            return;

        pushbackSequence?.Kill();

        ExecutePushbackSequence(distance, direction);
        OnPushbackApplied?.Invoke(distance, direction);
        Debug.Log($"{gameObject.name} is being pushed back by {distance} units");
    }

    private void ExecutePushbackSequence(float distance, Vector3 direction)
    {
        isPushingBack = true;
        autoCardMovement.StopMoving();

        Vector3 originalPosition = transform.position;
        Vector3 targetPosition = originalPosition + direction.normalized * distance;

        float moveSpeed = autoCardMovement.MoveSpeed;
        float returnDistance = Vector3.Distance(targetPosition, originalPosition);
        float returnTime = returnDistance / moveSpeed;

        pushbackSequence = DOTween.Sequence();
        pushbackSequence.Append(transform.DOMove(targetPosition, pushbackTime).SetEase(pushbackEase));
        pushbackSequence.Append(transform.DOMove(originalPosition, returnTime).SetEase(returnEase));

        pushbackSequence.OnComplete(() =>
        {
            autoCardMovement.StartMoving();
            isPushingBack = false;
            Debug.Log($"{gameObject.name} has returned to original position and resumed movement");
        });
    }

    private void OnDestroy()
    {
        pushbackSequence?.Kill();
    }
}
