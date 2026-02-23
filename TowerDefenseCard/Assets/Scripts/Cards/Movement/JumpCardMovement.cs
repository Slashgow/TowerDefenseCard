using UnityEngine;
using DG.Tweening;
using System;

public class JumpCardMovement : BaseCardMovement
{
    [Header("Jump Movement Settings")]
    [SerializeField, Range(0.1f, 10f)] private float moveSpeed = 2f;
    [SerializeField, Range(0.1f, 5f)] private float jumpDistance = 1f;
    [SerializeField, Range(0.1f, 2f)] private float jumpHeight = 0.5f;
    [SerializeField, Range(0.01f, 1f)] private float jumpDuration = 0.3f;
    [SerializeField, Range(0f, 1f)] private float pauseBetweenJumps = 0.1f;
    [SerializeField] private Ease jumpEase = Ease.OutQuad;
    [SerializeField] private bool stopOnReachTarget = true;

    [Header("Debug")]
    [SerializeField] private bool showGizmos = true;

    public Vector3 OriginalPosition { get; private set; }
    private Vector3 targetPosition;
    private Vector3 currentJumpTarget;
    private bool isJumping = false;
    private bool isMovingToTarget = false;
    private Sequence jumpSequence;
    private Tween horizontalTween;
    private Tween verticalTween;
    private Tween pauseTween;
    public bool IsMoving => isMovingToTarget;
    public bool IsJumping => isJumping;
    public Vector3 TargetPosition => targetPosition;

    public event Action<Vector3> OnReachTarget;

    protected override void Awake()
    {
        base.Awake();
        OriginalPosition = transform.position;
        targetPosition = transform.position;

    }

    public void MoveTo(Vector3 target)
    {
        targetPosition = new Vector3(target.x, target.y, transform.position.z);
        isMovingToTarget = true;

        if (!isJumping)
            StartNextJump();
    }

    public void StopMoving()
    {
        isMovingToTarget = false;
        StopJumping();
    }
    public bool HasReachedTarget() => Vector3.Distance(transform.position, targetPosition) < 0.1f;

    private void StartNextJump()
    {
        if (this == null) return; // Object has been destroyed

        if (!isMovingToTarget || isJumping)
            return;

        Vector3 currentPos = transform.position;
        float distanceToTarget = Vector3.Distance(currentPos, targetPosition);

        if (distanceToTarget < 0.1f)
        {
            if (stopOnReachTarget)
            {
                isMovingToTarget = false;
                OnReachTarget?.Invoke(targetPosition);
                return;
            }
        }

        Vector3 directionToTarget = (targetPosition - currentPos).normalized;
        float actualJumpDistance = Mathf.Min(jumpDistance, distanceToTarget);
        currentJumpTarget = currentPos + directionToTarget * actualJumpDistance;
        currentJumpTarget.z = transform.position.z;

        ExecuteJump();
    }

    private void ExecuteJump()
    {
        if (isJumping)
            return;

        isJumping = true;

        if (jumpSequence != null && jumpSequence.IsActive())
            jumpSequence.Kill();

        Vector3 startPos = transform.position;

        float distance = Vector3.Distance(startPos, currentJumpTarget);
        float actualJumpDuration = distance / moveSpeed;
        actualJumpDuration = Mathf.Max(jumpDuration, actualJumpDuration); 

        jumpSequence = DOTween.Sequence();
        horizontalTween = transform.DOMove(currentJumpTarget, actualJumpDuration).SetEase(Ease.Linear);
        Vector3 peakPosition = Vector3.Lerp(startPos, currentJumpTarget, 0.5f);
        peakPosition.y += jumpHeight;
        verticalTween = transform.DOMoveY(peakPosition.y, actualJumpDuration * 0.5f).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo);

        jumpSequence.Insert(0, horizontalTween);
        jumpSequence.Insert(0, verticalTween);

        jumpSequence.OnComplete(() => {
            if (this == null) return;

            isJumping = false;
            transform.position = currentJumpTarget; 

            if (pauseBetweenJumps > 0)
            {
                pauseTween = DOVirtual.DelayedCall(pauseBetweenJumps, () => {
                    if (this == null) 
                        return;

                    if (isMovingToTarget)
                        StartNextJump();
                });
            }
            else
            {
                if (isMovingToTarget)
                    StartNextJump();
            }
        });

        jumpSequence.SetEase(jumpEase);
    }

    private void StopJumping()
    {
        isJumping = false;

        horizontalTween?.Kill();
        verticalTween?.Kill();
        jumpSequence?.Kill();
        pauseTween?.Kill();
    }

    private void OnDestroy() => StopJumping();
    private void OnDisable() => StopJumping();

    private void Update()
    {
        if (transform.position.z != 0f)
        {
            Vector3 pos = transform.position;
            pos.z = 0f;
            transform.position = pos;
        }
    }

    #region Gizmos
    private void OnDrawGizmos()
    {
        if (!showGizmos)
            return;

        // Draw current target position
        if (isMovingToTarget)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetPosition, 0.2f);

            // Draw line to target
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetPosition);
        }

        if (isJumping)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(currentJumpTarget, 0.15f);

  
            Gizmos.color = Color.cyan;
            Vector3 peakPos = Vector3.Lerp(transform.position, currentJumpTarget, 0.5f);
            peakPos.y += jumpHeight;
            Gizmos.DrawLine(transform.position, peakPos);
            Gizmos.DrawLine(peakPos, currentJumpTarget);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, jumpDistance);
    }
    #endregion
}