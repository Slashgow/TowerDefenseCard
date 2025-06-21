using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "William/CardBaseTowerData")]
public class CardBaseTowerData : ScriptableObject
{
    [SerializeField, Range(0f,10f)] private float attackRange;
    public float AttackRange => attackRange;

    [SerializeField, Range(0f, 10f)] private float attackSpeed;
    public float AttackSpeed => attackSpeed;

    [SerializeField, Range(0f, 10f)] private float attackArea;
    public float AttackArea => attackArea;

    [SerializeField] private DamageType damageType;
    public DamageType DamageType => DamageType;
}