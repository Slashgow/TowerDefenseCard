using System;
using UnityEngine;


public class SimpleStealer : MonoBehaviour, IStealer
{
    [SerializeField] private Vector3 carryOffset = Vector3.up;
    [SerializeField, Range(0f,2f)] private float stealRange = 1f;
    [SerializeField] private LayerMask stealableLayerMask;
    public float StealRange => stealRange;
    public LayerMask StealableLayerMask => stealableLayerMask;

    private IStealable currentStealable;
    public IStealable CurrentStealable => currentStealable;

    public Vector3 CarryOffset => carryOffset;

    private BaseDamageable damageable;
    private JumpCardMovement jumpCardMovement;

    private void Awake()
    {
        damageable = GetComponent<BaseDamageable>();
        damageable.OnDie += Damageable_OnDie;

        jumpCardMovement = GetComponent<JumpCardMovement>();
        jumpCardMovement.OnReachTarget += JumpCardMovement_OnReachTarget;
    }

    private void OnDestroy()
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
                jumpCardMovement.MoveTo(jumpCardMovement.OriginalPosition);
                return;
            }
        }
    }
}
