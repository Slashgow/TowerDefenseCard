using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "William/CardDamageorData")]
public class CardDamageorData : ScriptableObject
{
    [SerializeField, Range(0f, 100f)] private float damage;
    public float Damage => damage;

    [SerializeField, Range(0f,10f)] private float attackRange;
    public float AttackRange => attackRange;

    [SerializeField, Range(0f, 10f)] private float attackSpeed;
    public float AttackSpeed => attackSpeed;

    [SerializeField, Range(0f, 10f)] private float attackArea;
    public float AttackArea => attackArea;

    [SerializeField] private DamageType damageType;
    public DamageType DamageType => DamageType;

    [SerializeField, Range(0f, 10f)] private float dot;
    public float DoT => dot;

    [SerializeField, Range(0f, 10f)] private float dotDuration;
    public float DoTDuration => dotDuration;
}