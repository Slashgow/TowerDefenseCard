using UnityEngine;
using UnityTimer;

public abstract class CardBaseTower : Card, IDamageor, IUpgradable
{
    [SerializeField] private CardBaseTowerData cardBaseTowerData;
    [SerializeField] protected LayerMask enemyLayer; 

    private Timer attackTimer;

    public float AttackRange => cardBaseTowerData.AttackRange;
    public float AttackSpeed => cardBaseTowerData.AttackSpeed;
    public float AttackArea => cardBaseTowerData.AttackArea;
    public DamageType DamageType => cardBaseTowerData.DamageType;
    public float Damage => cardBaseTowerData.Damage;

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
