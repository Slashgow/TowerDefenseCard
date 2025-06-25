using UnityEngine;
using UnityTimer;

public abstract class CardBaseDamageor : Card, IDamageor, IUpgradable
{
    [SerializeField] protected CardDamageorData cardDamageorData;
    [SerializeField] protected LayerMask enemyLayer; 

    protected Timer attackTimer;

    public float AttackRange => cardDamageorData.AttackRange;
    public float AttackSpeed => cardDamageorData.AttackSpeed;
    public float AttackArea => cardDamageorData.AttackArea;
    public DamageType DamageType => cardDamageorData.DamageType;
    public float Damage => cardDamageorData.Damage;

    protected override void Start()
    {
        base.Start();
        attackTimer = Timer.Register(1f / AttackSpeed, onComplete: () => Attack(), isLooped: true); // TO DO : Only attack during defense phase
    }

    protected abstract void Attack();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}
