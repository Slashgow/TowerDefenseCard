using UnityEngine;
public class CardBaseTower : Card, IDamageor, IDamageable, IUpgradable
{
    [SerializeField] private CardBaseTowerData cardBaseTowerData;

    public float AttackRange => cardBaseTowerData.AttackRange;
    public float AttackSpeed => cardBaseTowerData.AttackSpeed;
    public float AttackArea => cardBaseTowerData.AttackArea;
    public DamageType DamageType => cardBaseTowerData.DamageType;
}
