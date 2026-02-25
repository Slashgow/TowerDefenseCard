using System;
using UnityEngine;
using UnityEngine.Events;


public class SimpleStealer : MonoBehaviour, IStealer
{
    [Header("References")]
    [SerializeField] private BaseDamageable damageable;
    [SerializeField] private JumpCardMovement jumpCardMovement;

    [SerializeField] private Vector3 carryOffset = Vector3.up;
    [SerializeField, Range(0f,2f)] private float stealRange = 1f;
    [SerializeField] private LayerMask stealableLayerMask;
    public float StealRange => stealRange;
    public LayerMask StealableLayerMask => stealableLayerMask;

    private IStealable currentStealable;
    public IStealable CurrentStealable => currentStealable;

    public Vector3 CarryOffset => carryOffset;

    public event Action OnSteal;
    public UnityEvent OnStealUnity;

    public JumpCardMovement JumpCardMovement => jumpCardMovement;

    private void OnEnable()
    {
        damageable.OnDie += Damageable_OnDie;
        jumpCardMovement.OnReachTarget += JumpCardMovement_OnReachTarget;
    }

    private void OnDisable()
    {
        damageable.OnDie -= Damageable_OnDie;
        jumpCardMovement.OnReachTarget -= JumpCardMovement_OnReachTarget;
    }

    private void Damageable_OnDie()
    {
        if (currentStealable != null)
            currentStealable.RemoveSteal();
    }
    private void JumpCardMovement_OnReachTarget(Vector3 targetPosition)
    {
        if(targetPosition != jumpCardMovement.OriginalPosition)
            StealTarget();
        else
        {
            currentStealable.Destroy();
            Destroy(this.gameObject);
        } 

    }

    public void StealTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, StealRange, StealableLayerMask);

        foreach (var hit in hits)
        {
            IStealable stealable = hit.GetComponent<IStealable>();
            if (stealable != null && !stealable.IsStolen)
            {
                stealable.ApplySteal(transform, carryOffset);
                currentStealable = stealable;
                OnSteal?.Invoke();
                OnStealUnity?.Invoke();
                jumpCardMovement.MoveTo(jumpCardMovement.OriginalPosition);
                return;
            }
        }

        RandomEventManager.Instance.MoveToNewTarget(this);
    }
}
